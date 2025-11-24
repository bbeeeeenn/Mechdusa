using Mechdusa.Models;
using Terraria;
using TerrariaApi.Server;

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
    // For server-side Mech boss spawn check
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
            && !Variables.AllowedToSpawn.Remove(npc.netID)
        )
        {
            npc.active = false;
            args.Handled = true;
        }
    }
}
