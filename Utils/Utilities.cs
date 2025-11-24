using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Mechdusa;

public static class Utilities
{
    public static List<int>? FindMechSpawnerIndexes(Player plr)
    {
        List<int> indexes = new();
        Item[] inv = plr.inventory;
        List<int> found = new();
        for (int i = 0; i < 50 && indexes.Count < 3; i++)
        {
            Item currItem = inv[i];
            if (
                Variables.MechBossAndSummonItem.ContainsValue(currItem.netID)
                && currItem.stack > 0
                && !found.Contains(currItem.netID)
            )
            {
                found.Add(currItem.netID);
                indexes.Add(i);
            }
        }
        if (indexes.Count == 3)
        {
            return indexes;
        }
        return null;
    }

    public static int TryCraftOcramRazor(TSPlayer plr, List<int> indexes)
    {
        Item[] inv = plr.TPlayer.inventory;

        int minAmountToCraft = 9999;
        foreach (int i in indexes)
        {
            if (minAmountToCraft > inv[i].stack)
                minAmountToCraft = inv[i].stack;
        }

        int availableSlotIndex = -1;
        for (int i = 0; i < 50; i++)
        {
            Item currItem = inv[i];
            if (
                currItem.netID == ItemID.MechdusaSummon
                && currItem.stack < 9999
                && currItem.stack >= 1
            )
            {
                availableSlotIndex = i;
                minAmountToCraft = Math.Min(minAmountToCraft, 9999 - currItem.stack);
                break;
            }
        }
        for (int i = 0; i < 50 && availableSlotIndex == -1; i++)
        {
            Item currItem = inv[i];
            if (
                currItem.stack == 0
                || (
                    Variables.MechBossAndSummonItem.ContainsValue(currItem.netID)
                    && currItem.stack == minAmountToCraft
                )
            )
            {
                availableSlotIndex = i;
                break;
            }
        }
        if (availableSlotIndex == -1)
        {
            return 0;
        }

        HashSet<int> ignoreSlot = plr.GetData<HashSet<int>>("ignoreSlot") ?? new();
        // Consume Mech summon
        foreach (int slot in indexes)
        {
            inv[slot].stack -= minAmountToCraft;
            ignoreSlot.Add(slot);
            plr.SetData("ignoreSlot", ignoreSlot);
            plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, slot);
        }
        // Give Ocram's Razor
        if (inv[availableSlotIndex].netID == ItemID.MechdusaSummon)
        {
            inv[availableSlotIndex].stack += minAmountToCraft;
        }
        else
        {
            Item ocramRazor = TShock.Utils.GetItemById(ItemID.MechdusaSummon);
            ocramRazor.stack = minAmountToCraft;
            inv[availableSlotIndex] = ocramRazor;
        }
        plr.SendData(PacketTypes.PlayerSlot, null, plr.Index, availableSlotIndex);
        plr.SetData("ignoreSlot", new HashSet<int>());

        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(plr.TPlayer.position, 213), plr.Index);
        return minAmountToCraft;
    }

    public static bool TrySpawnMechQueen(TSPlayer targetPlayer)
    {
        int onWhichPlayer = targetPlayer.Index;
        NPC.mechQueen = -2;
        NPC.SpawnOnPlayer(onWhichPlayer, 127); // Skeletron Prime
        NPC.mechQueen = NPC.FindFirstNPC(127);
        if (NPC.mechQueen < 0 || 200 <= NPC.mechQueen)
        {
            return false;
        }
        NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, NPC.mechQueen);

        if (
            NPC.NewNPC(
                NPC.GetBossSpawnSource(onWhichPlayer),
                (int)Main.npc[NPC.mechQueen].Center.X,
                (int)Main.npc[NPC.mechQueen].Center.Y,
                125, // Retinazer
                1
            ) == 200
        )
            return false;
        if (
            NPC.NewNPC(
                NPC.GetBossSpawnSource(onWhichPlayer),
                (int)Main.npc[NPC.mechQueen].Center.X,
                (int)Main.npc[NPC.mechQueen].Center.Y,
                126, // Spazmatism
                1
            ) == 200
        )
            return false;
        int num = NPC.NewNPC(
            NPC.GetBossSpawnSource(onWhichPlayer),
            (int)Main.npc[NPC.mechQueen].Center.X,
            (int)Main.npc[NPC.mechQueen].Center.Y,
            134, // Destroyer
            1
        );
        if (num == 200)
            return false;
        if (
            NPC.NewNPC(
                NPC.GetBossSpawnSource(onWhichPlayer),
                (int)Main.npc[NPC.mechQueen].Center.X,
                (int)Main.npc[NPC.mechQueen].Center.Y,
                139,
                1,
                0f,
                0f,
                num,
                -1f
            ) == 200
        )
            return false;
        if (
            NPC.NewNPC(
                NPC.GetBossSpawnSource(onWhichPlayer),
                (int)Main.npc[NPC.mechQueen].Center.X,
                (int)Main.npc[NPC.mechQueen].Center.Y,
                139,
                1,
                0f,
                0f,
                num,
                1f
            ) == 200
        )
            return false;

        Variables.SpawnedMechQueen = true;
        return true;
    }

    public static void DropRewards(Vector2 position)
    {
        Rewards drops = PluginSettings.Config.Rewards;

        int totalWeight = drops.PossibleDrops.Sum(item => item.Weight);
        if (drops.PossibleDrops.Count < 1 || totalWeight == 0 || drops.dropAmount < 1)
        {
            return;
        }

        List<Item> rewards = new();
        for (int i = 0; i < drops.dropAmount; i++)
        {
            int roll = Random.Shared.Next(totalWeight);

            bool done = false;
            foreach (var item in drops.PossibleDrops)
            {
                if (roll < item.Weight)
                {
                    Item reward = new()
                    {
                        netID = item.NetID,
                        stack = item.Stack,
                        prefix = item.PrefixID,
                    };
                    reward.netDefaults(item.NetID);
                    rewards.Add(reward);
                    done = true;
                    break;
                }
                roll -= item.Weight;
            }
            if (!done)
            {
                throw new InvalidOperationException("Weighted roll failed.");
            }
        }

        // Give the rewards
        if (drops.givePerPlayer)
        {
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active)
                {
                    foreach (Item item in rewards)
                    {
                        player.GiveItem(item.netID, item.stack, item.prefix);
                    }
                }
            }
        }
        else
        {
            foreach (Item item in rewards)
            {
                int index = Item.NewItem(
                    null,
                    position,
                    Vector2.Zero,
                    item.netID,
                    Stack: item.stack,
                    prefixGiven: item.prefix
                );
                NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, null, index);
            }
        }
    }
}
