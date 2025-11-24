using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Mechdusa.Commands;

public class Mechdusa : Models.Command
{
    public override bool AllowServer => true;
    public override string[] Aliases { get; set; } =
        PluginSettings.Config.SpawnMechdusaCommandAliases;
    public override string PermissionNode { get; set; } = "mechdusa.admin";

    public override void Execute(CommandArgs args)
    {
        TSPlayer player = args.Player;
        Variables.AllowMechs();
        bool spawned;
        if (args.Parameters.Count == 0)
        {
            if (!player.RealPlayer)
            {
                player.SendMessage("Example usage: /mechdusa <playername>", Color.Red);
                return;
            }
            Variables.AllowMechs();
            spawned = Utilities.TrySpawnMechQueen(player);
        }
        else
        {
            string target = args.Parameters[0];
            bool targetIsIndex = int.TryParse(target, out int targetIndex);
            TSPlayer? targetPlayer = targetIsIndex ? TShock.Players[targetIndex] : null;
            if (!targetIsIndex)
            {
                foreach (TSPlayer plr in TShock.Players)
                {
                    if (plr == null || !plr.Active)
                    {
                        continue;
                    }
                    if (
                        plr.Name.ToLower().StartsWith(target.ToLower())
                        || plr.Account.Name.ToLower().StartsWith(target.ToLower())
                    )
                    {
                        targetPlayer = plr;
                        break;
                    }
                }
            }
            if (targetPlayer == null)
            {
                player.SendMessage("Target not found.", Color.Red);
                return;
            }
            Variables.AllowMechs();
            spawned = Utilities.TrySpawnMechQueen(targetPlayer);
        }

        if (spawned)
        {
            Main.dayTime = false;
            Main.time = 0;
            NetMessage.SendData((int)PacketTypes.TimeSet);
        }
        else
        {
            player.SendMessage("Mechdusa could not be summoned right now.", Color.Red);
        }
        // Variables.AllowedSpawn.Clear();
    }
}
