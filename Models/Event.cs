using TerrariaApi.Server;

namespace Mechdusa.Models;

public abstract class Event
{
    public abstract void Enable(TerrariaPlugin plugin);
    public abstract void Disable(TerrariaPlugin plugin);
}
