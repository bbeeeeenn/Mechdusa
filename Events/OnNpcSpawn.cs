using System;
using IL.Terraria.ID;
using Mechdusa.Models;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Mechdusa.Events;

public class OnNpcSpawn : Event
{
    public override void Disable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NpcSpawn.Deregister(plugin, EventMethod);
    }

    public override void Enable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NpcSpawn.Register(plugin, EventMethod);
    }

    private void EventMethod(NpcSpawnEventArgs args)
    {
        if (
            Main.getGoodWorld
            || Main.remixWorld
            || !PluginSettings.Config.Enabled
            || !PluginSettings.Config.DisableIndividualMech
        )
        {
            return;
        }

        NPC npc = Main.npc[args.NpcId];
        if (
            Variables.MechBossAndSummonItem.ContainsKey(npc.netID)
            && !Variables.AllowedSpawn.Remove(npc.netID)
        )
        {
            npc.active = false;
            NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, args.NpcId);
        }
    }
}
