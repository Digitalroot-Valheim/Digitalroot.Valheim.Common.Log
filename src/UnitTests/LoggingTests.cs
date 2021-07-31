using Digitalroot.Valheim.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTests
{
  public class LoggingTests
  {
    [Test(Author = "Digitalroot", Description = "Tests logging with the default source.", TestOf = typeof(Log)), Timeout(500)]
    public void LogTest()
    {
      var logger = new StaticSourceLogger();

      Log.Info(logger, nameof(Log.Info));
      Log.Debug(logger, nameof(Log.Debug));
      Log.Message(logger, nameof(Log.Message));
      Log.Error(logger, nameof(Log.Error));
      Log.Fatal(logger, nameof(Log.Fatal));
      Log.Warning(logger, nameof(Log.Warning));
      Log.Trace(logger, nameof(Log.Trace));
      Log.FlushLog(logger);

      FileInfo logFileInfo = new FileInfo("Digitalroot.Trace.log");
      System.Console.WriteLine(logFileInfo.FullName);
      Assert.That(logFileInfo, Is.Not.Null);
      Assert.That(logFileInfo.Exists, Is.True);
      using var fileStream = new FileStream(logFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var sr = new StreamReader(fileStream, Encoding.UTF8);

      while (!sr.EndOfStream)
      {
        string line = sr.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :Digitalroot] Info")
            .Or.EqualTo("[Debug  :Digitalroot] Debug")
            .Or.EqualTo("[Message:Digitalroot] Message")
            .Or.EqualTo("[Error  :Digitalroot] Error")
            .Or.EqualTo("[Fatal  :Digitalroot] Fatal")
            .Or.EqualTo("[Warning:Digitalroot] Warning")
            .Or.EqualTo("[All    :Digitalroot] Trace")
        );
      }
    }

    [Test(Author = "Digitalroot", Description = "Tests logging with a custom source.", TestOf = typeof(Log)), Timeout(500)]
    public void ChangeSourceTest()
    {
      var logger = new StaticSourceLogger("LogTest");
      Log.RegisterSource(logger, true);
      Log.Info(logger, nameof(Log.Info));
      Log.Debug(logger, nameof(Log.Debug));
      Log.Message(logger, nameof(Log.Message));
      Log.Error(logger, nameof(Log.Error));
      Log.Fatal(logger, nameof(Log.Fatal));
      Log.Warning(logger, nameof(Log.Warning));
      Log.Trace(logger, nameof(Log.Trace));
      Log.FlushLog(logger);

      FileInfo logFileInfo = new FileInfo("LogTest.Trace.log");
      System.Console.WriteLine(logFileInfo.FullName);
      Assert.That(logFileInfo, Is.Not.Null, $"logFileInfo != null : {logFileInfo != null}");
      Assert.That(logFileInfo.Exists, Is.True, $"logFileInfo.Exists : {logFileInfo.Exists}, {logFileInfo.FullName}");

      using var fileStream = new FileStream(logFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var sr = new StreamReader(fileStream, Encoding.UTF8);

      while (!sr.EndOfStream)
      {
        string line = sr.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :   LogTest] Info")
            .Or.EqualTo("[Debug  :   LogTest] Debug")
            .Or.EqualTo("[Message:   LogTest] Message")
            .Or.EqualTo("[Error  :   LogTest] Error")
            .Or.EqualTo("[Fatal  :   LogTest] Fatal")
            .Or.EqualTo("[Warning:   LogTest] Warning")
            .Or.EqualTo("[All    :   LogTest] Trace")
        );
      }
    }

    [Test(Author = "Digitalroot", Description = "Tests logging with a custom source.", TestOf = typeof(Log)), Timeout(500)]
    public void MultiSourceTest()
    {
      var loggerA = new StaticSourceLogger("loggerA");
      Log.RegisterSource(loggerA, true);
      Log.Info(loggerA, nameof(Log.Info));
      Log.Debug(loggerA, nameof(Log.Debug));
      Log.Message(loggerA, nameof(Log.Message));
      Log.Error(loggerA, nameof(Log.Error));
      Log.Fatal(loggerA, nameof(Log.Fatal));
      Log.Warning(loggerA, nameof(Log.Warning));
      Log.Trace(loggerA, nameof(Log.Trace));
      Log.FlushLog(loggerA);

      var loggerB = new StaticSourceLogger("loggerB");
      Log.RegisterSource(loggerB, true);
      Log.Info(loggerB, nameof(Log.Info));
      Log.Debug(loggerB, nameof(Log.Debug));
      Log.Message(loggerB, nameof(Log.Message));
      Log.Error(loggerB, nameof(Log.Error));
      Log.Fatal(loggerB, nameof(Log.Fatal));
      Log.Warning(loggerB, nameof(Log.Warning));
      Log.Trace(loggerB, nameof(Log.Trace));
      Log.FlushLog(loggerB);

      var loggerC = new StaticSourceLogger("loggerC");
      Log.RegisterSource(loggerC, true);
      Log.Info(loggerC, nameof(Log.Info));
      Log.Debug(loggerC, nameof(Log.Debug));
      Log.Message(loggerC, nameof(Log.Message));
      Log.Error(loggerC, nameof(Log.Error));
      Log.Fatal(loggerC, nameof(Log.Fatal));
      Log.Warning(loggerC, nameof(Log.Warning));
      Log.Trace(loggerC, nameof(Log.Trace));
      Log.FlushLog(loggerC);

      FileInfo logFileInfoA = new FileInfo("loggerA.Trace.log");
      System.Console.WriteLine(logFileInfoA.FullName);
      Assert.That(logFileInfoA, Is.Not.Null);
      Assert.That(logFileInfoA.Exists, Is.True);

      using var fileStreamA = new FileStream(logFileInfoA.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var streamReaderA = new StreamReader(fileStreamA, Encoding.UTF8);

      while (!streamReaderA.EndOfStream)
      {
        string line = streamReaderA.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :   loggerA] Info")
            .Or.EqualTo("[Debug  :   loggerA] Debug")
            .Or.EqualTo("[Message:   loggerA] Message")
            .Or.EqualTo("[Error  :   loggerA] Error")
            .Or.EqualTo("[Fatal  :   loggerA] Fatal")
            .Or.EqualTo("[Warning:   loggerA] Warning")
            .Or.EqualTo("[All    :   loggerA] Trace")
        );
      }

      FileInfo logFileInfoB = new FileInfo("loggerB.Trace.log");
      System.Console.WriteLine(logFileInfoB.FullName);
      Assert.That(logFileInfoB, Is.Not.Null);
      Assert.That(logFileInfoB.Exists, Is.True);
      
      using var fileStreamB = new FileStream(logFileInfoB.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var streamReaderB = new StreamReader(fileStreamB, Encoding.UTF8);
      
      while (!streamReaderB.EndOfStream)
      {
        string line = streamReaderB.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :   loggerB] Info")
            .Or.EqualTo("[Debug  :   loggerB] Debug")
            .Or.EqualTo("[Message:   loggerB] Message")
            .Or.EqualTo("[Error  :   loggerB] Error")
            .Or.EqualTo("[Fatal  :   loggerB] Fatal")
            .Or.EqualTo("[Warning:   loggerB] Warning")
            .Or.EqualTo("[All    :   loggerB] Trace")
        );
      }

      FileInfo logFileInfoC = new FileInfo("loggerC.Trace.log");
      System.Console.WriteLine(logFileInfoC.FullName);
      Assert.That(logFileInfoC, Is.Not.Null);
      Assert.That(logFileInfoC.Exists, Is.True);

      using var fileStreamC = new FileStream(logFileInfoC.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var streamReaderC = new StreamReader(fileStreamC, Encoding.UTF8);

      while (!streamReaderC.EndOfStream)
      {
        string line = streamReaderC.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :   loggerC] Info")
            .Or.EqualTo("[Debug  :   loggerC] Debug")
            .Or.EqualTo("[Message:   loggerC] Message")
            .Or.EqualTo("[Error  :   loggerC] Error")
            .Or.EqualTo("[Fatal  :   loggerC] Fatal")
            .Or.EqualTo("[Warning:   loggerC] Warning")
            .Or.EqualTo("[All    :   loggerC] Trace")
        );
      }
    }

    [Test(Author = "Digitalroot", Description = "Tests logging with a custom source.", TestOf = typeof(Log)), Timeout(500)]
    public void MultiSourceWeaveTest()
    {
      var loggerA = new StaticSourceLogger("loggerA");
      Log.RegisterSource(loggerA, true);
      var loggerB = new StaticSourceLogger("loggerB");
      Log.RegisterSource(loggerB, true);
      var loggerC = new StaticSourceLogger("loggerC");
      Log.RegisterSource(loggerC, true);

      var loggers = new List<ITraceableLogging> {loggerA, loggerB, loggerC};
      Random rnd = new Random();
      for (int i = 0; i < 10; i++)
      {
        Log.Info(loggers[rnd.Next(0, 3)], nameof(Log.Info));
        Log.Info(loggers[rnd.Next(0, 3)], nameof(Log.Info));
        Log.Info(loggers[rnd.Next(0, 3)], nameof(Log.Info));
        Log.Debug(loggers[rnd.Next(0, 3)], nameof(Log.Debug));
        Log.Debug(loggers[rnd.Next(0, 3)], nameof(Log.Debug));
        Log.Debug(loggers[rnd.Next(0, 3)], nameof(Log.Debug));
        Log.Message(loggers[rnd.Next(0, 3)], nameof(Log.Message));
        Log.Message(loggers[rnd.Next(0, 3)], nameof(Log.Message));
        Log.Message(loggers[rnd.Next(0, 3)], nameof(Log.Message));
        Log.Error(loggers[rnd.Next(0, 3)], nameof(Log.Error));
        Log.Error(loggers[rnd.Next(0, 3)], nameof(Log.Error));
        Log.Error(loggers[rnd.Next(0, 3)], nameof(Log.Error));
        Log.Fatal(loggers[rnd.Next(0, 3)], nameof(Log.Fatal));
        Log.Fatal(loggers[rnd.Next(0, 3)], nameof(Log.Fatal));
        Log.Fatal(loggers[rnd.Next(0, 3)], nameof(Log.Fatal));
        Log.Warning(loggers[rnd.Next(0, 3)], nameof(Log.Warning));
        Log.Warning(loggers[rnd.Next(0, 3)], nameof(Log.Warning));
        Log.Warning(loggers[rnd.Next(0, 3)], nameof(Log.Warning));
        Log.Trace(loggers[rnd.Next(0, 3)], nameof(Log.Trace));
        Log.Trace(loggers[rnd.Next(0, 3)], nameof(Log.Trace));
        Log.Trace(loggers[rnd.Next(0, 3)], nameof(Log.Trace));
        Log.Info(loggers[rnd.Next(0, 3)], nameof(Log.Info));
        Log.Debug(loggers[rnd.Next(0, 3)], nameof(Log.Debug));
        Log.Message(loggers[rnd.Next(0, 3)], nameof(Log.Message));
        Log.Error(loggers[rnd.Next(0, 3)], nameof(Log.Error));
        Log.Fatal(loggers[rnd.Next(0, 3)], nameof(Log.Fatal));
        Log.Warning(loggers[rnd.Next(0, 3)], nameof(Log.Warning));
        Log.Trace(loggers[rnd.Next(0, 3)], nameof(Log.Trace));
        Log.Info(loggers[rnd.Next(0, 3)], nameof(Log.Info));
        Log.Debug(loggers[rnd.Next(0, 3)], nameof(Log.Debug));
        Log.Message(loggers[rnd.Next(0, 3)], nameof(Log.Message));
        Log.Error(loggers[rnd.Next(0, 3)], nameof(Log.Error));
        Log.Fatal(loggers[rnd.Next(0, 3)], nameof(Log.Fatal));
        Log.Warning(loggers[rnd.Next(0, 3)], nameof(Log.Warning));
        Log.Trace(loggers[rnd.Next(0, 3)], nameof(Log.Trace));
      }

      Log.FlushLog(loggerA);
      Log.FlushLog(loggerB);
      Log.FlushLog(loggerC);

      FileInfo logFileInfoA = new FileInfo("loggerA.Trace.log");
      System.Console.WriteLine(logFileInfoA.FullName);
      Assert.That(logFileInfoA, Is.Not.Null);
      Assert.That(logFileInfoA.Exists, Is.True);

      using var fileStreamA = new FileStream(logFileInfoA.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var streamReaderA = new StreamReader(fileStreamA, Encoding.UTF8);

      while (!streamReaderA.EndOfStream)
      {
        string line = streamReaderA.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :   loggerA] Info")
            .Or.EqualTo("[Debug  :   loggerA] Debug")
            .Or.EqualTo("[Message:   loggerA] Message")
            .Or.EqualTo("[Error  :   loggerA] Error")
            .Or.EqualTo("[Fatal  :   loggerA] Fatal")
            .Or.EqualTo("[Warning:   loggerA] Warning")
            .Or.EqualTo("[All    :   loggerA] Trace")
        );
      }

      FileInfo logFileInfoB = new FileInfo("loggerB.Trace.log");
      System.Console.WriteLine(logFileInfoB.FullName);
      Assert.That(logFileInfoB, Is.Not.Null);
      Assert.That(logFileInfoB.Exists, Is.True);

      using var fileStreamB = new FileStream(logFileInfoB.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var streamReaderB = new StreamReader(fileStreamB, Encoding.UTF8);

      while (!streamReaderB.EndOfStream)
      {
        string line = streamReaderB.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :   loggerB] Info")
            .Or.EqualTo("[Debug  :   loggerB] Debug")
            .Or.EqualTo("[Message:   loggerB] Message")
            .Or.EqualTo("[Error  :   loggerB] Error")
            .Or.EqualTo("[Fatal  :   loggerB] Fatal")
            .Or.EqualTo("[Warning:   loggerB] Warning")
            .Or.EqualTo("[All    :   loggerB] Trace")
        );
      }

      FileInfo logFileInfoC = new FileInfo("loggerC.Trace.log");
      System.Console.WriteLine(logFileInfoC.FullName);
      Assert.That(logFileInfoC, Is.Not.Null);
      Assert.That(logFileInfoC.Exists, Is.True);

      using var fileStreamC = new FileStream(logFileInfoC.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None);
      using var streamReaderC = new StreamReader(fileStreamC, Encoding.UTF8);

      while (!streamReaderC.EndOfStream)
      {
        string line = streamReaderC.ReadLine();
        Assert.That(line,
          Is.EqualTo("[Info   :   loggerC] Info")
            .Or.EqualTo("[Debug  :   loggerC] Debug")
            .Or.EqualTo("[Message:   loggerC] Message")
            .Or.EqualTo("[Error  :   loggerC] Error")
            .Or.EqualTo("[Fatal  :   loggerC] Fatal")
            .Or.EqualTo("[Warning:   loggerC] Warning")
            .Or.EqualTo("[All    :   loggerC] Trace")
        );
      }
    }
  }
}
