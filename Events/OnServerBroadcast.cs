using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Mechdusa.Events;

public class OnServerBroadcast : Models.Event
{
    public override void Disable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.ServerBroadcast.Deregister(plugin, EventMethod);
    }

    public override void Enable(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.ServerBroadcast.Register(plugin, EventMethod);
    }

    private void EventMethod(ServerBroadcastEventArgs args)
    {
        if (
            Main.remixWorld
            || Main.getGoodWorld
            || !PluginSettings.Config.Enabled
            || !PluginSettings.Config.DisableIndividualMech
        )
            return;

        string text = args.Message.ToString();

        List<string> bannedStrings = new()
        {
            "You feel vibrations",
            "This is going to be a terrible night",
            "The air is getting colder around you",
        };

        // Console.WriteLine(text); // for Debugging

        if (bannedStrings.Any((e) => text.StartsWith(e)))
        {
            args.Handled = true;
        }
    }
}
