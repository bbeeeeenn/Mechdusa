using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Mechdusa.Commands;

public class Test : Models.Command
{
    public override string[] Aliases { get; set; } = { "t" };
    public override string PermissionNode { get; set; } = "";

    public override void Execute(CommandArgs args)
    {
        TSPlayer plr = args.Player;
        if (plr == null || !plr.Active)
            return;

        plr.SendMessage(
            $"{string.Join(", ", Variables.MechsLeft.Select(m => Main.npc[m].FullName))}",
            Color.AliceBlue
        );
    }
}
