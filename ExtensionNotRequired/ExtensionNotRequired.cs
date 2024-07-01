namespace ExtensionNotRequired
{
    using HarmonyLib;
    using Twitch;

    public class ExtensionNotRequired: IModApi
    {
        public void InitMod(Mod mod)
        {
            Harmony.CreateAndPatchAll(this.GetType());
            Log.Out("ExtensionNotRequired Loaded");
        }

        private static bool logged = false;

        [HarmonyPatch(typeof(TwitchManager), nameof(TwitchManager.Update)), HarmonyPrefix, HarmonyPriority(Priority.First)]
        private static void Update_Prefix(TwitchManager __instance, float deltaTime)
        {
            __instance.ExtensionCheckTime += deltaTime;
            if(__instance.InitState == TwitchManager.InitStates.CheckingForExtension)
            {
                __instance.InitState = TwitchManager.InitStates.Authenticated;
            }

            if(!logged)
            {
                Log.Out("ExtensionNotRequired patched succesfully.");
                logged = true;
            }
        }
    }
}
