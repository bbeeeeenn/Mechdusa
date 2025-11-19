using System;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Mechdusa.Commands;

public class Test : Models.Command
{
    public override string[] Aliases { get; set; } = { "test" };
    public override string PermissionNode { get; set; } = "";

    public override void Execute(CommandArgs args)
    {
        TSPlayer plr = args.Player;
        if (args.Parameters.Count < 1 || plr == null || !plr.Active)
            return;

        if (!int.TryParse(args.Parameters[0], out int result))
            return;
        var x = plr.TPlayer.position.X;
        var y = plr.TPlayer.position.Y;
        BitsByte flag = new();
        flag[0] = true;
        flag[1] = true;
        flag[3] = true;

        plr.SendMessage($"Playing sound id: {result} at [{x},{y}]", Color.AliceBlue);
        NetMessage.SendData(
            (int)PacketTypes.PlayLegacySound,
            -1,
            -1,
            null,
            (int)x,
            y,
            result,
            flag
        );
    }
}
