using BepInEx;
using BepInEx.Logging;
using Digitalroot.Valheim.Common.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Timers;

namespace Digitalroot.Valheim.Common
{
  internal class TraceLogger : IDisposable
  {
    internal readonly ManualLogSource LoggerRef;
    private readonly string _source;
    private const int FlushTick = 300;
    private readonly object _fileLock = new object();
    private readonly System.Timers.Timer _timer;
    private readonly FileStream _fileStream;
    private readonly StreamWriter _streamWriter;
    public bool IsTraceEnabled { get; }
    public TraceLogger(string source, bool enableTrace)
    {
      _source = source;
      IsTraceEnabled = enableTrace;
      LoggerRef = Logger.CreateLogSource(_source);
      var traceFileInfo = new FileInfo(Path.Combine(Paths.BepInExRootPath ?? AssemblyDirectory.FullName, $"{_source}.Trace.log"));

      if (traceFileInfo.Exists)
      {
        traceFileInfo.Delete();
        traceFileInfo.Refresh();
      }

      _timer = new System.Timers.Timer(FlushTick)
      {
        AutoReset = true, Enabled = enableTrace
      };

      _timer.Elapsed += TimerElapsed;

      if (enableTrace)
      {
        lock (_fileLock)
        {
          _fileStream ??= new FileStream(traceFileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough);
          _streamWriter ??= new StreamWriter(_fileStream, Encoding.UTF8, 4096);
          _timer.Start();
          LoggerRef.LogEvent += OnLogEvent;
        }
      }
    }

    private void StopTrace()
    {
      LoggerRef.LogEvent -= OnLogEvent;
      _timer.Stop();
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
      lock (_fileLock)
      {
        _streamWriter?.Flush();
      }
    }

    private void OnLogEvent(object sender, LogEventArgs e)
    {
      if (e.Source.SourceName != _source && e.Level != (LogLevel.Error | LogLevel.Fatal)) return;
      lock (_fileLock)
      {
        if (e.Data is string)
        {
          _streamWriter.WriteLine($"[{e.Level,-7}:{e.Source.SourceName,10}] {e.Data}");
        }
        else
        {
          _streamWriter.WriteLine($"[{e.Level,-7}:{e.Source.SourceName,10}] {JsonSerializationProvider.ToJson(e.Data)}");
        }
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

    #region IDisposable

    /// <inheritdoc />
    public void Dispose()
    {
      _timer?.Stop();
      _timer?.Dispose();
      _streamWriter?.Flush();
      _fileStream?.Flush();
      _streamWriter?.Close();
      _fileStream?.Close();
      _streamWriter?.Dispose();
      _fileStream?.Dispose();
    }

    #endregion

    public void Flush()
    {
      _streamWriter?.Flush();
      _fileStream?.Flush();
    }
  }
}
