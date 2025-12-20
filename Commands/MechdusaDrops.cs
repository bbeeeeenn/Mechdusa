using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Mechdusa.Commands;

public class MechdusaDrops : Models.Command
{
    public override string[] Aliases { get; set; } = PluginSettings.Config.ShowDropsCommandAliases;
    public override string PermissionNode { get; set; } = "";

    public override void Execute(CommandArgs args)
    {
        string formattedString =
            $"-- Mechdusa's Drops --\n Roll Count: [c/ffffff:{PluginSettings.Config.Rewards.dropAmount}]\n Drops:";
        int totalWeight = PluginSettings.Config.Rewards.PossibleDrops.Sum(d => d.Weight);
        foreach (var drop in PluginSettings.Config.Rewards.PossibleDrops)
        {
            Item item = TShock.Utils.GetItemById(drop.NetID);
            item.stack = drop.Stack;
            item.prefix = drop.PrefixID;
            float chance = (float)drop.Weight / totalWeight * 100;
            string color = chance switch
            {
                >= 50 => Color.Gray.Hex3(),
                >= 20 => Color.Green.Hex3(),
                >= 5 => Color.Blue.Hex3(),
                >= 1 => Color.Purple.Hex3(),
                >= (float)0.1 => Color.Orange.Hex3(),
                _ => Color.Gold.Hex3(),
            };

            formattedString += string.Format(
                $"\n   [c/{color}:[{{0:F1}}%][c/{color}:]] {{1}}",
                chance,
                args.Player.RealPlayer
                    ? $"[i/{(TShock.Utils.GetItemById(drop.NetID).CanHavePrefixes() ? "p" + drop.PrefixID : "s" + drop.Stack)}:{drop.NetID}] [c/{color}:{item.HoverName}]"
                    : $"[c/{color}:{item.HoverName}]"
            );
        }
        args.Player.SendMessage(formattedString, Color.OrangeRed);
    }
}
