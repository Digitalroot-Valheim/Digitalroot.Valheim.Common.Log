using System.Diagnostics;

namespace Digitalroot.Valheim.Common
{
  [DebuggerDisplay("Source = {Source}, EnableTrace = {EnableTrace}", Name = "{Source}")]
  public class StaticSourceLogger : ITraceableLogging
  {
    public StaticSourceLogger(bool enableTrace = false)
      : this("Digitalroot", enableTrace)
    {
    }

    public StaticSourceLogger(string source, bool enableTrace = false)
    {
      Source = source;
      EnableTrace = enableTrace;
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public bool EnableTrace { get; }

#endregion
  }
}
