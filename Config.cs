using Mechdusa.Models;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;

namespace Mechdusa;

public class PluginSettings
{
    public static readonly string PluginDirectory = Path.Combine(
        TShock.SavePath,
        TShockPlugin.PluginName
    );
    public static readonly string ConfigPath = Path.Combine(PluginDirectory, "config.json");

    public static PluginSettings Config { get; set; } = new();
    #region Configs
    public bool Enabled = true;
    public bool DisableIndividualMech = true;
    public string[] SpawnMechdusaCommandAliases = new string[] { "mechdusa", "mq" };
    public int SendCraftingHintIntervalSeconds = 10;
    public string NOTE1 =
        "Everything will be set to normal if the world is already able to spawn Mechdusa.";
    public string NOTE2 =
        "If [DisableIndividualMech] is set to true, player inventories will automatically craft an Ocram's Razor if they have all three different mech spawners.";
    public string NOTE3 =
        "The spawn-mechdusa-command requires 'mechdusa.admin' permission. You have to restart the server before the [SpawnMechdusaCommandAliases] changes are applied.";
    public string NOTE4 =
        "Spawning multiple Mechdusa instances can cause unexpected behavior because of the max-NPC cap limitation. Though, this can only be achieved using the command.";
    #endregion
    public static void Save()
    {
        string configJson = JsonConvert.SerializeObject(Config, Formatting.Indented);
        File.WriteAllText(ConfigPath, configJson);
    }

    public static ResponseMessage Load()
    {
        if (!Directory.Exists(PluginDirectory))
        {
            Directory.CreateDirectory(PluginDirectory);
        }
        if (!File.Exists(ConfigPath))
        {
            Save();
            return new ResponseMessage()
            {
                Text =
                    $"[{TShockPlugin.PluginName}] Config file doesn't exist yet. A new one has been created.",
                Color = Color.Yellow,
            };
        }
        else
        {
            try
            {
                string json = File.ReadAllText(ConfigPath);
                PluginSettings? deserializedConfig = JsonConvert.DeserializeObject<PluginSettings>(
                    json
                );
                if (deserializedConfig != null)
                {
                    Config = deserializedConfig;
                    return new ResponseMessage()
                    {
                        Text = $"[{TShockPlugin.PluginName}] Loaded config.",
                        Color = Color.LimeGreen,
                    };
                }
                else
                {
                    return new ResponseMessage()
                    {
                        Text =
                            $"[{TShockPlugin.PluginName}] Config file was found, but deserialization returned null.",
                        Color = Color.Red,
                    };
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(
                    $"[{TShockPlugin.PluginName}] Error loading config: {ex.Message}"
                );
                return new ResponseMessage()
                {
                    Text =
                        $"[{TShockPlugin.PluginName}] Error loading config. Check logs for more details.",
                    Color = Color.Red,
                };
            }
        }
    }
}
