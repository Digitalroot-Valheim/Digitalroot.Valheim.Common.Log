using BepInEx;
using BepInEx.Logging;
using Digitalroot.Valheim.Common.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Digitalroot.Valheim.Common
{
  internal class TraceLogger
  {
    internal readonly ManualLogSource LoggerRef;
    private readonly string _source;
    private readonly FileInfo _traceFileInfo;
    public bool IsTraceEnabled { get; private set; }

    public TraceLogger(string source, bool enableTrace)
    {
      _source = source;
      IsTraceEnabled = enableTrace;
      LoggerRef = Logger.CreateLogSource(_source);
      _traceFileInfo = new FileInfo(Path.Combine(Paths.BepInExRootPath ?? AssemblyDirectory.FullName, "logs", $"{_source}.Trace.log"));
      if (_traceFileInfo.DirectoryName != null) Directory.CreateDirectory(_traceFileInfo.DirectoryName);
      if (_traceFileInfo.Exists)
      {
        _traceFileInfo.Delete();
        _traceFileInfo.Refresh();
      }

      LoggerRef.LogEvent += OnLogEvent;
    }


    public void EnableTrace()
    {
      IsTraceEnabled = true;
    }

    public void DisableTrace()
    {
      IsTraceEnabled = false;
    }

    private void StopTrace()
    {
      LoggerRef.LogEvent -= OnLogEvent;
    }

    private void OnLogEvent(object sender, LogEventArgs e)
    {
      if (e.Source.SourceName != _source || !IsTraceEnabled) return;

      using var mutex = new Mutex(false, $"Digitalroot.Valheim.Common.TraceLogger.{_source}");
      mutex.WaitOne();
      try
      {
        if (e.Data is string)
        {
          var msg = $"[{e.Level,-7}:{e.Source.SourceName,10}] {e.Data}{Environment.NewLine}";
          File.AppendAllText(_traceFileInfo.FullName, msg, Encoding.UTF8);
          // WriteTrace(msg, e);
        }
        else
        {
          var msg = $"[{e.Level,-7}:{e.Source.SourceName,10}] {JsonSerializationProvider.ToJson(e.Data)}{Environment.NewLine}";
          File.AppendAllText(_traceFileInfo.FullName, msg, Encoding.UTF8);
          // WriteTrace(msg, e);
        }
      }
      finally
      {
        mutex.ReleaseMutex();
      }
    }

    private void WriteTrace(string msg, LogEventArgs e)
    {
      switch (e.Level)
      {
        case LogLevel.Fatal:
        case LogLevel.Error:
          Trace.TraceError($"WriteTrace.TraceError: {msg}\n");
          break;

        case LogLevel.Warning:
          Trace.TraceWarning($"WriteTrace.TraceWarning: {msg}\n");
          break;

        case LogLevel.None:
        case LogLevel.Message:
        case LogLevel.Info:
        case LogLevel.Debug:
        case LogLevel.All:
          Trace.TraceInformation($"WriteTrace.TraceWarning: {msg}\n");
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
      
    }

    private DirectoryInfo AssemblyDirectory
    {
      get
      {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new(codeBase);
        var fileInfo = new FileInfo(Uri.UnescapeDataString(uri.Path));
        return fileInfo.Directory;
      }
    }
  }
}
