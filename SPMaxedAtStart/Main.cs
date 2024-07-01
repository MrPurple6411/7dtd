namespace SPMaxedAtStart;

using HarmonyLib;
using System;

public class Main : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        Harmony.CreateAndPatchAll(this.GetType());
        Log.Out("SPMaxedAtStart Loaded");
    }

    [HarmonyPatch(typeof(EntityPlayer), nameof(EntityPlayer.Respawn)), HarmonyPrefix]
    private static void EntityPlayer_Respawn_Prefix(EntityPlayer __instance, RespawnType _reason)
    {
        var values = __instance.Progression.ProgressionValues.Dict.Values;
        foreach (ProgressionValue progressionValue in values)
        {
            if (progressionValue == null || progressionValue.level >= progressionValue.ProgressionClass.MaxLevel)
            {
                continue;
            }

            if (progressionValue.ProgressionClass.CurrencyType != ProgressionCurrencyType.SP)
            {
                progressionValue.level = progressionValue.ProgressionClass.MaxLevel;
            }
            else
            {
                __instance.Progression.SkillPoints = Math.Max(__instance.Progression.SkillPoints - progressionValue.ProgressionClass.MaxLevel - progressionValue.Level, 0);
                progressionValue.level = progressionValue.ProgressionClass.MaxLevel;
            }

            if (progressionValue.ProgressionClass.IsPerk)
            {
                __instance.MinEventContext.ProgressionValue = progressionValue;
                progressionValue.ProgressionClass.FireEvent(MinEventTypes.onPerkLevelChanged, __instance.MinEventContext);
            }
        }
    }
}
