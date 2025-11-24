using Mechdusa.Models;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria.ID;
using TShockAPI;

namespace Mechdusa;

public class Rewards
{
    public class Item
    {
        public string Label;
        public int NetID;
        public int Weight;
        public short Stack;
        public byte PrefixID;

        public Item(string _label, int _netId, int _weight, short _stack = 1, byte _prefix = 0)
        {
            Label = _label;
            NetID = _netId;
            Weight = _weight;
            Stack = _stack;
            PrefixID = _prefix;
        }
    }

    [JsonProperty("GivePerPlayer")]
    public bool givePerPlayer = true;

    [JsonProperty("LootDropAmount")]
    public int dropAmount = 1;

    [JsonProperty("List of possible drops")]
    public List<Item> PossibleDrops = new()
    {
        new("Waffle Iron PSSHH", ItemID.WaffleIron, 1, _prefix: PrefixID.Legendary),
        new("Uzi BRRT BRRT", ItemID.Uzi, 2, _prefix: PrefixID.Unreal),
    };
    public string[] RewardsHelp =
    {
        "- When [GivePerPlayer] is true, every player gets their own reward; if false, the loot drops at Mechdusa's death location.",
        "- [LootDropAmount] determines how many items will be rolled and awarded as rewards.",
        "- [Weight] determines the likelihood of an item being selected by the rarity system. Higher weights make the item more likely to be chosen, while lower weights make it rarer.",
        "- [Weight] should be greater than or equal to 1",
    };

    public Rewards() { }
}

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
    public bool DisableIndividualMech = false;
    public string[] SpawnMechdusaCommandAliases = { "mechdusa", "mq" };
    public int SendCraftingHintIntervalSeconds = 20;
    public string[] Help =
    {
        "- Everything will be set to normal if the world is already able to spawn Mechdusa.",
        "- If [DisableIndividualMech] is set to true, player inventories will automatically craft an Ocram's Razor if they have all three different mech spawners.",
        "- The spawn-mechdusa-command requires 'mechdusa.admin' permission. You have to restart the server before the [SpawnMechdusaCommandAliases] changes are applied.",
        "- Spawning multiple Mechdusa instances can cause unexpected behavior because of the max-NPC cap limitation. Though, this can only be achieved using the command.",
    };
    public Rewards Rewards = new();
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
                    json,
                    new JsonSerializerSettings()
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                    }
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
