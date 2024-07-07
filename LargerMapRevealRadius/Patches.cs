namespace LargerMapRevealRadius;

using HarmonyLib;

[HarmonyPatch]
 public static class Patches
{
    [HarmonyPatch(typeof(IMapChunkDatabase), nameof(IMapChunkDatabase.Add), [typeof(Vector3i), typeof(World)]), HarmonyPrefix, HarmonyPriority(Priority.First)]
    private static bool IMapChunkDatabase_Add_Prefix(IMapChunkDatabase __instance, Vector3i _chunkPos, World _world)
    {
        int num = 16;
        for (int i = -num; i <= num; i++)
        {
            for (int j = -num; j <= num; j++)
            {
                Chunk chunk = (Chunk)_world.GetChunkSync(_chunkPos.x + i, _chunkPos.z + j);
                if (chunk != null && !chunk.NeedsDecoration)
                {
                    __instance.Add(_chunkPos.x + i, _chunkPos.z + j, chunk.GetMapColors());
                }
            }
        }
        return false;
    }
}