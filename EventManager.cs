using Mechdusa.Events;
using Mechdusa.Models;
using TerrariaApi.Server;

namespace Mechdusa;

public class EventManager
{
    public static readonly List<Event> events = new()
    {
        // Events
        new OnReload(),
        new OnGetData(),
        new OnServerBroadcast(),
        new OnNpcSpawn(),
        new OnNpcKilled(),
    };

    public static void RegisterAll(TerrariaPlugin plugin)
    {
        foreach (Event _event in events)
        {
            _event.Enable(plugin);
        }
    }

    public static void DeregisterAll(TerrariaPlugin plugin)
    {
        foreach (Event _event in events)
        {
            _event.Disable(plugin);
        }
    }
}
