namespace TwitchSpawnsPersist
{
    using HarmonyLib;
    using Twitch;

    public class TwitchSpawnsPersist: IModApi
    {
        public void InitMod(Mod mod)
        {
            Harmony.CreateAndPatchAll(this.GetType());
            Log.Out("TwitchSpawnsPersist Loaded");
        }

        [HarmonyPatch(typeof(TwitchManager), nameof(TwitchManager.KillAllSpawnsForPlayer))]
        [HarmonyPrefix]
        private static bool KillAllSpawnsForPlayer_Prefix(TwitchManager __instance)
        {
            return false;
        }

        [HarmonyPatch(typeof(EntityAlive), nameof(EntityAlive.Update))]
        [HarmonyPrefix]
        private static void EntityEnemy_Prefix(EntityAlive __instance)
        {
            if(__instance is EntityEnemy enemy)
            {
                enemy.bIsChunkObserver = true;
                enemy.IsHordeZombie = true;
                //enemy.IsBloodMoon = true;
            }
        }
    }

}
