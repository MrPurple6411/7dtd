namespace LootStageBoost;

using HarmonyLib;
using UnityEngine;
using static LootContainer;

public class LootStageBoost : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        Harmony.CreateAndPatchAll(this.GetType());
        Log.Out("LootStageBoost Loaded");
    }

    [HarmonyPatch(typeof(EntityPlayer), nameof(EntityPlayer.GetLootStage)), HarmonyPostfix]
    private static void GetLootStage_Postfix(ref int __result)
    {
        __result *= 100;
    }

    [HarmonyPatch(typeof(LootContainer), nameof(SpawnLootItemsFromList)), HarmonyPrefix]
    private static void SpawnLootItemsFromList_Prefix(ref int numToSpawn, ref float abundance)
    {
        if (numToSpawn > 0)
        {
            numToSpawn = Mathf.Max(Mathf.FloorToInt(numToSpawn * abundance), 1);
            abundance = 1;
        }            
    }
}