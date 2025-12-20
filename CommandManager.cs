using Mechdusa.Commands;
using Mechdusa.Models;

namespace Mechdusa;

public class CommandManager
{
    public static readonly List<Command> Commands = new()
    {
        // Commands
        new Commands.Mechdusa(),
        new CraftOcramRazor(),
        new MechdusaDrops(),
    };

    public static void RegisterAll()
    {
        foreach (Command command in Commands)
        {
            TShockAPI.Commands.ChatCommands.Add(command);
        }
    }
}
