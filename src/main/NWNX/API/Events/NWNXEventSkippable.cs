using NWN.Core.NWNX;

namespace NWNX.API.Events
{
  public abstract class NWNXEventSkippable<T> : NWNXEvent<T> where T : NWNXEvent<T>
  {
    /// <summary>
    /// Gets or sets a value indicating whether this event will be skipped.
    /// </summary>
    public bool Skip { get; set; }

    protected override void ProcessEvent()
    {
      Skip = false;
      InvokeCallbacks();
      CheckEventSkip();
    }

    protected void CheckEventSkip()
    {
      if (Skip)
      {
        EventsPlugin.SkipEvent();
      }
    }
  }
}
