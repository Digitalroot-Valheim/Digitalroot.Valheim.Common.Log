using Digitalroot.Valheim.Common;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.IO;
using System.Text;

namespace UnitTests
{
  public class LoggingTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test(Author = "Digitalroot", Description ="Tests logging with the default source.", TestOf = typeof(Log)), Timeout(500), Order(1)]
    public void LogTest()
    {
      Log.EnableTrace();
      Log.Info(nameof(Log.Info));
      Log.Debug(nameof(Log.Debug));
      Log.Message(nameof(Log.Message));
      Log.Error(nameof(Log.Error));
      Log.Fatal(nameof(Log.Fatal));
      Log.Warning(nameof(Log.Warning));
      Log.Trace(nameof(Log.Trace));
      Log.OnDestroy();

      FileInfo logFileInfo = new FileInfo("Digitalroot.Trace.log");
      Assert.That(logFileInfo, Is.Not.Null);
      Assert.That(logFileInfo.Exists, Is.True);
      foreach (var line in File.ReadLines(logFileInfo.FullName, Encoding.UTF8))
      {
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

    [Test(Author = "Digitalroot", Description = "Tests logging with a custom source.", TestOf = typeof(Log)), Timeout(500), Order(2)]
    public void ChangeSourceTest()
    {
      Log.EnableTrace();
      Log.SetSource(nameof(ChangeSourceTest));
      Log.Info(nameof(Log.Info));
      Log.Debug(nameof(Log.Debug));
      Log.Message(nameof(Log.Message));
      Log.Error(nameof(Log.Error));
      Log.Fatal(nameof(Log.Fatal));
      Log.Warning(nameof(Log.Warning));
      Log.Trace(nameof(Log.Trace));
      Log.OnDestroy();

      FileInfo logFileInfo = new FileInfo("ChangeSourceTest.Trace.log");
      Assert.That(logFileInfo, Is.Not.Null);
      Assert.That(logFileInfo.Exists, Is.True);
      foreach (var line in File.ReadLines(logFileInfo.FullName, Encoding.UTF8))
      {
        Assert.That(line,
          Is.EqualTo("[Info   :ChangeSourceTest] Info")
            .Or.EqualTo("[Debug  :ChangeSourceTest] Debug")
            .Or.EqualTo("[Message:ChangeSourceTest] Message")
            .Or.EqualTo("[Error  :ChangeSourceTest] Error")
            .Or.EqualTo("[Fatal  :ChangeSourceTest] Fatal")
            .Or.EqualTo("[Warning:ChangeSourceTest] Warning")
            .Or.EqualTo("[All    :ChangeSourceTest] Trace")
        );
      }
    }
  }
}