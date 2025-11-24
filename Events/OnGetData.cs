using Mechdusa.Models;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace Mechdusa.Events;

public class OnGetData : Event
{
    public override void Disable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NetGetData.Deregister(plugin, EventMethod);
    }

    public override void Enable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NetGetData.Register(plugin, EventMethod);
    }

    private void EventMethod(GetDataEventArgs args)
    {
        // TShock.Utils.Broadcast($"{args.MsgID}", Color.AliceBlue);
        // return;
        if (Main.remixWorld || Main.getGoodWorld || !PluginSettings.Config.Enabled)
        {
            return;
        }

        using BinaryReader reader = new(
            new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)
        );
        switch (args.MsgID)
        {
            case PacketTypes.PlayerSlot:
            {
                byte playerId = reader.ReadByte();
                short slot = reader.ReadInt16();
                short stack = reader.ReadInt16();
                byte prefix = reader.ReadByte();
                short netId = reader.ReadInt16();

                PreventSummonItemConsumption(args, playerId, netId, slot, stack);
                SendHelpMessageForMechSummons(args, netId);
                AutoCraftOcramsRazor(args, playerId, netId, slot, prefix, stack);
                break;
            }
            case PacketTypes.SpawnBossorInvasion:
            {
                DisableBossMechSpawn(args);
                break;
            }
            case PacketTypes.PlayerUpdate:
            {
                UseOcramsRazor(args);
                break;
            }
            default:
                break;
        }
    }

    private static void UseOcramsRazor(GetDataEventArgs args)
    {
        using BinaryReader reader = new(
            new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)
        );
        byte playerid = reader.ReadByte();
        BitsByte control = reader.ReadByte();

        _ = reader.ReadByte();
        _ = reader.ReadByte();
        _ = reader.ReadByte();
        byte selectedItemSlot = reader.ReadByte();
        TSPlayer player = TShock.Players[playerid];
        if (player == null || !player.Active)
            return;

        bool useItem = control[5];
        Item selectedItem = player.TPlayer.inventory[selectedItemSlot];
        if (Main.dayTime || !useItem || selectedItem.netID != ItemID.MechdusaSummon)
        {
            return;
        }
        if (
            NPC.AnyNPCs(NPCID.SkeletronPrime)
            || NPC.AnyNPCs(NPCID.TheDestroyer)
            || NPC.AnyNPCs(NPCID.Retinazer)
            || NPC.AnyNPCs(NPCID.Spazmatism)
        )
        {
            return;
        }

        // Spawn mechdusa
        if (PluginSettings.Config.DisableIndividualMech)
        {
            Variables.AllowMechs(); // Temporarily allow spawning mechs
        }
        bool spawned = Utilities.TrySpawnMechQueen(player);
        if (spawned)
        {
            player.TPlayer.inventory[selectedItemSlot].stack--;
            player.SendData(PacketTypes.PlayerSlot, "", playerid, selectedItemSlot);
        }
    }

    private static void AutoCraftOcramsRazor(
        GetDataEventArgs args,
        int playerId,
        int netId,
        int slot,
        byte prefix,
        int stack
    )
    {
        TSPlayer player = TShock.Players[args.Msg.whoAmI];
        if (player == null)
        {
            return;
        }

        if (!Variables.MechBossAndSummonItem.ContainsValue(netId))
        {
            return;
        }

        HashSet<int> ignoreSlot = player.GetData<HashSet<int>>("ignoreSlot") ?? new();
        if (ignoreSlot.Remove(slot))
        {
            player.SetData("ignoreSlot", ignoreSlot);
            args.Handled = true;
            return;
        }

        if (!PluginSettings.Config.DisableIndividualMech || slot >= 50)
        {
            return;
        }

        Item item = TShock.Utils.GetItemById(netId);
        item.prefix = prefix;
        item.stack = stack;
        Main.player[playerId].inventory[slot] = item;
        args.Handled = true;

        List<int>? indexes = Utilities.FindMechSpawnerIndexes(player.TPlayer);
        if (indexes == null)
        {
            return;
        }
        int amount = Utilities.TryCraftOcramRazor(player, indexes);
        if (amount >= 1)
        {
            player.SendMessage(
                $"Auto-crafted {amount} [c/{Colors.RarityOrange.Hex3()}:Ocram's Razor].",
                Color.LimeGreen
            );
        }
    }

    private static void SendHelpMessageForMechSummons(GetDataEventArgs args, int netId)
    {
        TSPlayer player = TShock.Players[args.Msg.whoAmI];
        if (player == null)
        {
            return;
        }

        if (!Variables.MechBossAndSummonItem.ContainsValue(netId))
        {
            return;
        }

        DateTime lastInfoSent = player.GetData<DateTime>("lastInfoSent");
        // TShock.Utils.Broadcast(
        //     $"Uptime: {Variables.UpTime.Elapsed.Seconds}, lastSent: {lastInfoSent}, diff: {Variables.UpTime.Elapsed.Seconds - lastInfoSent}, isGreaterOrEqual: {(Variables.UpTime.Elapsed.Seconds - lastInfoSent) >= PluginSettings.Config.SendCraftingHintIntervalSeconds}",
        //     Color.AliceBlue
        // );
        if (
            (DateTime.Now - lastInfoSent).Seconds
            >= PluginSettings.Config.SendCraftingHintIntervalSeconds
        )
        {
            if (PluginSettings.Config.DisableIndividualMech)
            {
                player.SendMessage(
                    $"[c/{Colors.RarityOrange.Hex3()}:{TShock.Utils.GetItemById(netId).Name}], along with the other two Mech spawners, will automatically combine to [c/{Colors.RarityOrange.Hex3()}:Ocram's Razor] if you have at least one of each.",
                    Color.LimeGreen
                );
            }
            else
            {
                player.SendMessage(
                    $"[c/{Colors.RarityOrange.Hex3()}:{TShock.Utils.GetItemById(netId).Name}], along with the other two Mech spawners, can be combined into [c/{Colors.RarityOrange.Hex3()}:Ocram's Razor] using the command [c/ffffff:/craftocramrazor] or [c/ffffff:/cor].",
                    Color.LimeGreen
                );
            }
            lastInfoSent = DateTime.Now;
            player.SetData("lastInfoSent", lastInfoSent);
        }
    }

    private static void DisableBossMechSpawn(GetDataEventArgs args)
    {
        if (!PluginSettings.Config.DisableIndividualMech)
        {
            return;
        }

        using BinaryReader reader = new(
            new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)
        );

        short playerId = reader.ReadInt16();
        short type = reader.ReadInt16();

        if (Variables.MechBossAndSummonItem.ContainsKey(type))
        {
            Variables.PreventItemUsage.Add(Variables.MechBossAndSummonItem[type]);
            args.Handled = true;
        }
    }

    private static void PreventSummonItemConsumption(
        GetDataEventArgs args,
        int playerId,
        int netId,
        int slot,
        int stack
    )
    {
        if (!PluginSettings.Config.DisableIndividualMech)
            return;

        Player tplayer = Main.player[playerId];
        if (Variables.PreventItemUsage.Remove(netId) && tplayer.inventory[slot].stack > stack)
        {
            args.Handled = true;
            NetMessage.SendData((int)PacketTypes.PlayerSlot, playerId, -1, null, playerId, slot);
        }
    }
}
