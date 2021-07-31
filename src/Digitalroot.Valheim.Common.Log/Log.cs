using BepInEx.Logging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Digitalroot.Valheim.Common
{
  /// <summary>
  /// License: "GNU Affero General Public License v3.0"
  /// </summary>
  public sealed class Log : IDisposable
  {
    private static readonly Dictionary<string, TraceLogger> TraceLoggers = new Dictionary<string, TraceLogger>();

    private static Log Instance { get; } = new Log();

    static Log()
    {
    }

    private Log()
    {
      // Create Default TraceLogger
      TraceLoggers.Add(nameof(Digitalroot), new TraceLogger(nameof(Digitalroot), true));
    }

    #region Implementation of IDisposable

    /// <inheritdoc />
    public void Dispose()
    {
      foreach (var traceLogger in TraceLoggers.Values)
      {
        traceLogger.Dispose();
      }
    }

    #endregion

    public static void FlushLog(ITraceableLogging sender)
    {
      if (TraceLoggers.ContainsKey(sender.Source))
      {
        TraceLoggers[sender.Source].Flush();
      }
    }

    public static void FlushAllLogs()
    {
      foreach (var traceLogger in TraceLoggers.Values)
      {
        traceLogger.Flush();
      }
    }

    public static void RegisterSource(ITraceableLogging sender, bool enabledTrace)
    {
      if (TraceLoggers.ContainsKey(sender.Source)) return;
      TraceLoggers.Add(sender.Source, new TraceLogger(sender.Source, enabledTrace));
    }
    
    #region Logging

    [UsedImplicitly]
    public static void Fatal(ITraceableLogging sender, Exception e, int i = 1)
    {
      Fatal(sender, $"Message: {e.Message}");
      Fatal(sender, $"StackTrace: {e.StackTrace}");
      Fatal(sender, $"Source: {e.Source}");
      if (e.Data.Count > 0)
      {
        foreach (var key in e.Data.Keys)
        {
          Fatal(sender, $"key: {key}, value: {e.Data[key]}");
        }
      }

      if (e.InnerException != null)
      {
        Fatal(sender, $"--- InnerException [{i}][Start] ---");
        Fatal(sender, e.InnerException, ++i);
      }
    }

    private static TraceLogger GetTraceLogger(ITraceableLogging sender)
    {
      return TraceLoggers.ContainsKey(sender.Source) ? TraceLoggers[sender.Source] : TraceLoggers[nameof(Digitalroot)];
    }

    [UsedImplicitly]
    public static void Fatal(ITraceableLogging sender, object value)
    {
      GetTraceLogger(sender).LoggerRef.LogFatal(value);
    }

    [UsedImplicitly]
    public static void Error(ITraceableLogging sender, Exception e, int i = 1)
    {
      Error(sender, $"Message: {e.Message}");
      Error(sender, $"StackTrace: {e.StackTrace}");
      Error(sender, $"Source: {e.Source}");
      if (e.Data.Count > 0)
      {
        foreach (var key in e.Data.Keys)
        {
          Error(sender, $"key: {key}, value: {e.Data[key]}");
        }
      }

      if (e.InnerException == null) return;
      Error(sender, $"--- InnerException [{i}][Start] ---");
      Error(sender, e.InnerException, ++i);
    }

    [UsedImplicitly]
    public static void Error(ITraceableLogging sender, object value)
    {
      GetTraceLogger(sender).LoggerRef.LogError(value);
    }

    [UsedImplicitly]
    public static void Warning(ITraceableLogging sender, object value)
    {
      GetTraceLogger(sender).LoggerRef.LogWarning(value);
    }

    [UsedImplicitly]
    public static void Message(ITraceableLogging sender, object value)
    {
      GetTraceLogger(sender).LoggerRef.LogMessage(value);
    }

    [UsedImplicitly]
    public static void Info(ITraceableLogging sender, object value)
    {
      GetTraceLogger(sender).LoggerRef.LogInfo(value);
    }

    [UsedImplicitly]
    public static void Debug(ITraceableLogging sender, object value)
    {
      GetTraceLogger(sender).LoggerRef.LogDebug(value);
    }

    [UsedImplicitly]
    public static void Trace(ITraceableLogging sender, object value)
    {
      if (GetTraceLogger(sender).IsTraceEnabled)
      {
        GetTraceLogger(sender).LoggerRef.Log(LogLevel.All, value);
      }
    }

    #endregion

    public static void OnDestroy()
    {
      Instance.Dispose();
    }
  }
}
