using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Mechdusa.Commands;

public class CraftOcramRazor : Models.Command
{
    public override bool AllowServer => false;
    public override string[] Aliases { get; set; } = { "craftocramrazor", "cor" };
    public override string PermissionNode { get; set; } = "";

    public override void Execute(CommandArgs args)
    {
        if (Main.remixWorld || Main.getGoodWorld || !PluginSettings.Config.Enabled)
        {
            args.Player.SendMessage("Disabled.", Color.Red);
            return;
        }

        if (args.Player == null || !args.Player.Active)
            return;
        TSPlayer player = args.Player;
        if (PluginSettings.Config.DisableIndividualMech)
        {
            player.SendMessage(
                "An Ocram's Razor will automatically spawn in your inventory once you have all three Mech Boss spawners.",
                Color.LimeGreen
            );
            return;
        }

        HashSet<int>? bossSpawnerInvIndexes = Utilities.FindMechSpawnerIndexes(args.Player.TPlayer);
        if (bossSpawnerInvIndexes == null)
        {
            player.SendMessage(
                $"You don't have all three different Mech Boss spawners to craft Ocram's Razor.",
                Color.Red
            );
            return;
        }

        int amount = Utilities.TryCraftOcramRazor(player, bossSpawnerInvIndexes);
        if (amount == 0)
        {
            player.SendMessage("Your inventory seems full.", Color.Red);
            return;
        }

        player.SendMessage(
            $"Successfully crafted {amount} [c/{Terraria.ID.Colors.RarityOrange.Hex3()}:Ocram's Razor].",
            Color.LimeGreen
        );
    }
}
