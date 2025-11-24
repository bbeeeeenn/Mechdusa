# Mechdusa

A plugin that allows non-`getfixedboi` worlds to spawn Mechdusa.

## Configuration

-  **`Enabled`** (Default: `true`) — Turns the plugin on or off.
-  **`DisableIndividualMech`** (Default: `false`) — If `true`, players cannot summon the three mech bosses individually. If a player has all three mech spawners, they will automatically craft into **Ocram’s Razor**.
-  **`SpawnMechdusaCommandAliases`** — The list of command aliases used to spawn Mechdusa via command.  
   Requires the `mechdusa.admin` permission.  
   _Note:_ Restart the server for alias changes to apply.
-  **`SendCraftingHintIntervalSeconds`** (Default: `20`) — Controls how frequently players are notified about how to obtain **Ocram’s Razor**.

-  **`Rewards`** — Controls the custom reward distribution after defeating Mechdusa.
   -  **`GivePerPlayer`** (Default: `false`)
      -  `true`: Each player receives their own rolled rewards.
      -  `false`: All rewards drop at Mechdusa’s death location.
   -  **`LootDropAmount`** (Default: `1`)  
      Determines how many times the loot table is rolled.
   -  **`List of possible drops`** — A list of items Mechdusa may drop. Each entry includes:
      -  **`Label`** — A custom name for identifying the item.
      -  **`NetID`** — Terraria Item ID.
      -  **`Weight`** — A value controlling how likely the item is to be selected (must be ≥ 1).  
         Higher weight = more common.
      -  **`Stack`** — Amount of the item awarded.
      -  **`PrefixID`** — Item prefix/modifier.

## Installation

1. [Download the plugin `.dll` file from the Releases page](https://github.com/bbeeeeenn/Mechdusa/releases).
2. Place the file in your TShock server’s `ServerPlugins` folder.
3. Restart your TShock server.
4. The plugin is now active and ready to use.
