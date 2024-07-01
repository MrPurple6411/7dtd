namespace TwitchParachuteSpawns;

using UnityEngine;
using HarmonyLib;
using GameEvent.SequenceActions;
using TwitchParachuteSpawns.Mono;

public class Main : IModApi
{
    public void InitMod(Mod mod)
    {
        Harmony.CreateAndPatchAll(this.GetType());
        Log.Out("TwitchParachuteSpawns Loaded");
    }

    [HarmonyPatch(typeof(ActionBaseSpawn), nameof(ActionBaseSpawn.SpawnEntity)), HarmonyPrefix]
    private static bool SpawnEntity_Prefix(ActionBaseSpawn __instance, ref Entity __result, int spawnedEntityID, Entity target, Vector3 startPoint, float minDistance, float maxDistance, bool spawnInSafe, float yOffset)
    {
        if (__instance.airSpawn)
            return true;
        
        World world = GameManager.Instance.World;
        Vector3 vector = ((target != null) ? new Vector3(0f, target.transform.eulerAngles.y + 180f, 0f) : Vector3.zero);
        Vector3 zero;
        Entity entity = null;
        if (ActionBaseSpawn.FindValidPosition(out zero, startPoint, minDistance, maxDistance, spawnInSafe, yOffset, __instance.airSpawn))
        {
            Ray ray = new(zero + new Vector3(0f, 50.5f, 0f), Vector3.down);
            bool skyNotClear = Voxel.Raycast(world, ray, 50f, false, false);

            entity = EntityFactory.CreateEntity(spawnedEntityID, zero + new Vector3(0f, skyNotClear? 0.5f: 50.5f, 0f), vector, (target != null) ? target.entityId : (-1), __instance.Owner.ExtraData);
            entity.SetSpawnerSource(EnumSpawnerSource.Dynamic);
            world.SpawnEntityInWorld(entity);

            if (!skyNotClear)
            {
                entity.gameObject.AddComponent<ZChute>();
            }
        }
        __result = entity;
        return false;
    }
}
