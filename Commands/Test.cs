using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using static Terraria.NetMessage;

namespace Mechdusa.Commands;

public class Test : Models.Command
{
    public override string[] Aliases { get; set; } = { "t" };
    public override string PermissionNode { get; set; } = "";
    private static int Index = 0;

    public override void Execute(CommandArgs args)
    {
        TSPlayer plr = args.Player;
        if (plr == null || !plr.Active)
            return;

        plr.SendMessage($"Playing index: {Index}", Color.AliceBlue);
        var playSoundAtPosition = plr.TPlayer.position;
        PlayNetSound(new NetSoundInfo(playSoundAtPosition, (ushort)Index), -1, -1);
        Index++;
    }
}
