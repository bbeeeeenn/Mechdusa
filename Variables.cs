using Terraria;
using Terraria.ID;

namespace Mechdusa;

public static class Variables
{
    public static readonly Dictionary<int, int> MechBossesAndTheirSummonItem = new()
    {
        { NPCID.TheDestroyer, ItemID.MechanicalWorm },
        { NPCID.Spazmatism, ItemID.MechanicalEye },
        { NPCID.Retinazer, ItemID.MechanicalEye },
        { NPCID.SkeletronPrime, ItemID.MechanicalSkull },
    };

    public static readonly HashSet<int> PreventItemComsumption = new();
    public static readonly HashSet<int> AllowedToSpawn = new();

    public static void AllowMechs()
    {
        foreach (int boss in MechBossesAndTheirSummonItem.Keys)
        {
            AllowedToSpawn.Add(boss);
        }
    }

    public static readonly HashSet<int> MechsLeft = new(4);
    private static bool _SpawnedMechQueen = false;
    public static bool SpawnedMechQueen
    {
        get => _SpawnedMechQueen;
        set
        {
            _SpawnedMechQueen = value;
            MechsLeft.Clear();
            if (value)
            {
                foreach (int netId in MechBossesAndTheirSummonItem.Keys)
                {
                    MechsLeft.Add(NPC.FindFirstNPC(netId));
                }
            }
        }
    }
}
