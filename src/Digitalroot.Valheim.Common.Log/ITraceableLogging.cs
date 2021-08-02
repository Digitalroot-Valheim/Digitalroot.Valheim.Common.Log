namespace Digitalroot.Valheim.Common
{
  public interface ITraceableLogging
  {
    string Source { get; }
    bool EnableTrace { get; }
  }
}
