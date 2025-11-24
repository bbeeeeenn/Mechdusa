using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Mechdusa.Events;

public class OnNpcKilled : Models.Event
{
    public override void Disable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NpcKilled.Deregister(plugin, EventMethod);
    }

    public override void Enable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NpcKilled.Register(plugin, EventMethod);
    }

    private void EventMethod(NpcKilledEventArgs args)
    {
        if (Main.remixWorld || Main.getGoodWorld || !PluginSettings.Config.Enabled)
        {
            return;
        }

        if (Variables.SpawnedMechQueen && Variables.MechsLeft.Remove(args.npc.whoAmI))
        {
            if (!Variables.MechBossAndSummonItem.ContainsKey(args.npc.netID))
            {
                Variables.SpawnedMechQueen = false;
                return;
            }
            if (Variables.MechsLeft.Count == 0)
            {
                // Jackpot
                Utilities.DropRewards(args.npc.position);
                Variables.SpawnedMechQueen = false;
            }
        }
    }
}
