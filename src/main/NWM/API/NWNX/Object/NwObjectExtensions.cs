using NWN;
using NWNX;

namespace NWM.API.NWNX
{
  public static class NwObjectExtensions
  {
    public static string GetPersistentString(this NwObject obj, string key)
    {
      return ObjectPlugin.GetString(obj, key);
    }

    public static void SetPersistentString(this NwObject obj, string key, string value)
    {
      ObjectPlugin.SetString(obj, key, value, true);
    }
  }
}