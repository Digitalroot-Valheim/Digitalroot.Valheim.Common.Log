using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Timers;

namespace Digitalroot.Valheim.Common
{
  /// <summary>
  /// License: "GNU Affero General Public License v3.0"
  /// </summary>
  public sealed class Log : IDisposable
  {
    private ManualLogSource _loggerRef;
    private string _source = nameof(Digitalroot);
    private const int FlushTick = 300;

    private static Log Instance { get; } = new Log();

    static Log()
    {
    }

    private Log()
    {
      Init();
    }

    public void Dispose()
    {
      _timer.Stop();
      _timer.Dispose();
      _streamWriter.Flush();
      _fileStream.Flush();
      _streamWriter.Close();
      _fileStream.Close();
      _streamWriter.Dispose();
      _fileStream.Dispose();
    }

    private void Init()
    {
      if (_timer != null)
      {
        StopTrace();
        _timer.Dispose();
        _streamWriter.Flush();
        _fileStream.Flush();
        _streamWriter.Close();
        _fileStream.Close();
        _streamWriter.Dispose();
        _fileStream.Dispose();
      }

      _loggerRef = Logger.CreateLogSource(_source);
      _traceFileInfo = new FileInfo(Path.Combine(Paths.BepInExRootPath ?? AssemblyDirectory.FullName, $"{_source}.Trace.log"));

      if (_traceFileInfo.Exists)
      {
        _traceFileInfo.Delete();
        _traceFileInfo.Refresh();
      }

      if (IsTraceEnabled)
      {
        _timer = new System.Timers.Timer(FlushTick)
        {
          AutoReset = true,
          Enabled = true
        };
        _timer.Elapsed += TimerElapsed;

        StartTrace();
      }
    }

    public static void SetSource(string source)
    {
      Instance._source = source;
      Instance.Init();
    }
    
    #region Trace

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool IsTraceEnabled { get; private set; }

    private static readonly object FileLock = new object();
    private FileInfo _traceFileInfo;
    private FileStream _fileStream;
    private StreamWriter _streamWriter;
    private System.Timers.Timer _timer;

    private void StartTrace()
    {
      lock (FileLock)
      {
        if (_fileStream != null && !_fileStream.CanWrite)
        {
          _fileStream = null;
          _streamWriter = null;
          _timer = null;
        }

        if (_fileStream == null) _fileStream = new FileStream(_traceFileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough);
        if (_streamWriter == null) _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8, 4096);

        if (_timer == null)
        {
          _timer = new System.Timers.Timer(FlushTick);
        }

        _timer.Start();
        _loggerRef.LogEvent += OnLogEvent;
      }
    }

    private void StopTrace()
    {
      _timer.Stop();
      _loggerRef.LogEvent -= OnLogEvent;
    }

    private void OnLogEvent(object sender, LogEventArgs e)
    {
      if (e.Source.SourceName == _source || e.Level == (LogLevel.Error | LogLevel.Fatal))
      {
        lock (FileLock)
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
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
      lock (FileLock)
      {
        _streamWriter?.Flush();
      }
    }

    [UsedImplicitly]
    public static void EnableTrace()
    {
      Instance.StartTrace();
      IsTraceEnabled = true;
    }

    [UsedImplicitly]
    public static void DisableTrace()
    {
      Instance.StopTrace();
      IsTraceEnabled = false;
    }

    public static void ToggleTrace(bool value)
    {
      switch (value)
      {
        case true:
          EnableTrace();
          break;
        default:
          DisableTrace();
          break;
      }
    }

    #endregion

    #region Logging

    [UsedImplicitly]
    public static void Fatal(Exception e, int i = 1)
    {
      Fatal($"Message: {e.Message}");
      Fatal($"StackTrace: {e.StackTrace}");
      Fatal($"Source: {e.Source}");
      if (e.Data.Count > 0)
      {
        foreach (var key in e.Data.Keys)
        {
          Fatal($"key: {key}, value: {e.Data[key]}");
        }
      }

      if (e.InnerException != null)
      {
        Fatal($"--- InnerException [{i}][Start] ---");
        Fatal(e.InnerException, ++i);
      }
    }

    [UsedImplicitly]
    public static void Fatal(object value)
    {
      Instance._loggerRef.LogFatal(value);
    }

    [UsedImplicitly]
    public static void Error(Exception e, int i = 1)
    {
      Error($"Message: {e.Message}");
      Error($"StackTrace: {e.StackTrace}");
      Error($"Source: {e.Source}");
      if (e.Data.Count > 0)
      {
        foreach (var key in e.Data.Keys)
        {
          Error($"key: {key}, value: {e.Data[key]}");
        }
      }

      if (e.InnerException != null)
      {
        Error($"--- InnerException [{i}][Start] ---");
        Error(e.InnerException, ++i);
      }
    }

    [UsedImplicitly]
    public static void Error(object value)
    {
      Instance._loggerRef.LogError(value);
    }

    [UsedImplicitly]
    public static void Warning(object value)
    {
      Instance._loggerRef.LogWarning(value);
    }

    [UsedImplicitly]
    public static void Message(object value)
    {
      Instance._loggerRef.LogMessage(value);
    }

    [UsedImplicitly]
    public static void Info(object value)
    {
      Instance._loggerRef.LogInfo(value);
    }

    [UsedImplicitly]
    public static void Debug(object value)
    {
      Instance._loggerRef.LogDebug(value);
    }

    [UsedImplicitly]
    public static void Trace(object value)
    {
      if (IsTraceEnabled)
      {
        Instance._loggerRef.Log(LogLevel.All, value);
      }
    }

    #endregion

    public static void OnDestroy()
    {
      Instance.Dispose();
    }

    private static DirectoryInfo AssemblyDirectory
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
