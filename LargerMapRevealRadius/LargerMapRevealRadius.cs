namespace LargerMapRevealRadius;

using HarmonyLib;

public class LargerMapRevealRadius : IModApi
{
    public void InitMod(Mod mod)
    {
        Harmony.CreateAndPatchAll(typeof(Patches));
        Log.Out("LargerMapRevealRadius Loaded");
    }
}