﻿using BepInEx;
using BepInEx.Logging;
using Digitalroot.Valheim.Common.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

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
      _traceFileInfo = new FileInfo(Path.Combine(Paths.BepInExRootPath ?? AssemblyDirectory.FullName, $"{_source}.Trace.log"));

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

    [UsedImplicitly]
    public void DisableTrace()
    {
      IsTraceEnabled = false;
    }

    [UsedImplicitly]
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
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
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
        mutex.ReleaseMutex();
      }
    }

    private DirectoryInfo AssemblyDirectory
    {
      get
      {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var fileInfo = new FileInfo(Uri.UnescapeDataString(uri.Path));
        return fileInfo.Directory;
      }
    }
  }
}
