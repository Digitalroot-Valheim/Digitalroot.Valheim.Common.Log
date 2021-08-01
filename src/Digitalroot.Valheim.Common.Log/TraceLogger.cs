using BepInEx;
using BepInEx.Logging;
using Digitalroot.Valheim.Common.Json;
using System;
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
    private readonly object _fileLock = new();
    private readonly FileInfo _traceFileInfo;
    private readonly EventWaitHandle _waitHandle;
    public bool IsTraceEnabled { get; }

    public TraceLogger(string source, bool enableTrace)
    {
      _source = source;
      IsTraceEnabled = enableTrace;
      LoggerRef = Logger.CreateLogSource(_source);
      _waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, $"Digitalroot.Valheim.Common.TraceLogger.{_source}");
      _traceFileInfo = new FileInfo(Path.Combine(Paths.BepInExRootPath ?? AssemblyDirectory.FullName, $"{_source}.Trace.log"));

      if (_traceFileInfo.Exists)
      {
        _traceFileInfo.Delete();
        _traceFileInfo.Refresh();
      }

      if (enableTrace)
      {
        LoggerRef.LogEvent += OnLogEvent;
      }
    }

    private void StopTrace()
    {
      LoggerRef.LogEvent -= OnLogEvent;
    }

    private void OnLogEvent(object sender, LogEventArgs e)
    {
      if (e.Source.SourceName != _source && e.Level != (LogLevel.Error | LogLevel.Fatal)) return;

      _waitHandle.WaitOne();
      try
      {
        if (e.Data is string)
        {
          File.AppendAllText(_traceFileInfo.FullName, $"[{e.Level,-7}:{e.Source.SourceName,10}] {e.Data}{Environment.NewLine}", Encoding.UTF8);
        }
        else
        {
          File.AppendAllText(_traceFileInfo.FullName, $"[{e.Level,-7}:{e.Source.SourceName,10}] {JsonSerializationProvider.ToJson(e.Data)}{Environment.NewLine}", Encoding.UTF8);
        }
      }
      finally
      {
        _waitHandle.Set();
      }
    }

    private DirectoryInfo AssemblyDirectory
    {
      get
      {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        var fileInfo = new FileInfo(Uri.UnescapeDataString(uri.Path));
        return fileInfo.Directory;
      }
    }
  }
}
