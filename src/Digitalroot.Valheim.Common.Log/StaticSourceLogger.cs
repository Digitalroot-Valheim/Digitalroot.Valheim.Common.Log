namespace Digitalroot.Valheim.Common
{
  public class StaticSourceLogger : ITraceableLogging
  {
    public StaticSourceLogger()
      : this("Digitalroot")
    {
    }

    public StaticSourceLogger(string source)
    {
      Source = source;
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source { get; }

    #endregion
  }
}
