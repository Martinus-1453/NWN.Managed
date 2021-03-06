using System;
using NWN.Core;

namespace NWN.API
{
  [CampaignVariableConverter(typeof(bool), typeof(Guid))]
  internal class ExtendedCampaignVariableConverters : ICampaignVariableConverter<bool>, ICampaignVariableConverter<Guid>
  {
    bool ICampaignVariableConverter<bool>.GetCampaign(string campaign, string name, NwPlayer player)
      => NWScript.GetCampaignInt(campaign, name, player).ToBool();

    void ICampaignVariableConverter<bool>.SetCampaign(string campaign, string name, bool value, NwPlayer player)
      => NWScript.SetCampaignInt(campaign, name, value.ToInt(), player);

    void ICampaignVariableConverter<bool>.ClearCampaign(string campaign, string name, NwPlayer player)
      => NWScript.DeleteCampaignVariable(campaign, name, player);

    Guid ICampaignVariableConverter<Guid>.GetCampaign(string campaign, string name, NwPlayer player)
    {
      string stored = NWScript.GetCampaignString(campaign, name, player);
      return string.IsNullOrEmpty(stored) ? Guid.Empty : Guid.Parse(stored);
    }

    void ICampaignVariableConverter<Guid>.SetCampaign(string campaign, string name, Guid value, NwPlayer player)
      => NWScript.SetCampaignString(campaign, name, value.ToUUIDString(), player);

    void ICampaignVariableConverter<Guid>.ClearCampaign(string campaign, string name, NwPlayer player)
      => NWScript.DeleteCampaignVariable(campaign, name, player);
  }
}
