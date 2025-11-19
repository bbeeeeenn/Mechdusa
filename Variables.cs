using System.Diagnostics;

namespace Mechdusa;

public static class Variables
{
    public static readonly Dictionary<int, int> MechBossAndSummonItem = new()
    {
        { Terraria.ID.NPCID.TheDestroyer, Terraria.ID.ItemID.MechanicalWorm },
        { Terraria.ID.NPCID.Spazmatism, Terraria.ID.ItemID.MechanicalEye },
        { Terraria.ID.NPCID.Retinazer, Terraria.ID.ItemID.MechanicalEye },
        { Terraria.ID.NPCID.SkeletronPrime, Terraria.ID.ItemID.MechanicalSkull },
    };

    public static readonly HashSet<int> PreventItemUsage = new();
    public static readonly HashSet<int> AllowedSpawn = new();

    public static void AllowMechs()
    {
        foreach (int boss in MechBossAndSummonItem.Keys)
        {
            AllowedSpawn.Add(boss);
        }
    }
}
