# TCG Card Shop Simulator - Archipelago Client Mod

THIS IS IN EARLY ALPHA. THERE ARE MISSING FEATURES AND THERE WILL BE BUGS. PLEASE REPORT BUGS AND GIVE FEEDBACK IN THE FUTURE GAME DESIGN DISCORD CHANNEL

This Plugin is the TCG Card Shop Simulator implementation of a client to connect to an archipelago randomiser server and handle items and checks.

For the AP world see [https://github.com/FyreDay/Archipelago-TCGCardShopSimulator/releases]

## Setup

Mod TCG Card Shop Simulator with BepinEX with at least v5.4 Latest BepinEx is recommended

If you currently do not have TCG Card Shop Simulator modded, You have two options

1.Install BepinEX from either Nexus Mods or ThunderStore from the preconfigured mods available there

OR

2.Install BepinEX from [https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2]

### BepinEx Configuration

Once installed navigate to `TCG Card Shop Simulator\BepInEx\config` and open `BepInEx.cfg`

Under [ChainLoader] change HideManagerGameObject = false to HideManagerGameObject = true

### Install Plugin

Extract the latest release into `TCG Card Shop Simulator\BepInEx\plugins`

## Connecting

If installed correctly you should see a window in the top left with fields to enter the IP:Port, password, and Slot. New Game and load game should be disabled. If you are ever able to see your save slots your installation has been done incorrectly.

To Connect, Enter the data and hit the connect button. If the `Not Connected` Text changes to `Connected` You have succesfully connected. You May now click New Game if starting, or Load game if you have previously played on this seed.

## How This Rando Works

### Licenses

The each page in the restock shop is randomized, the items inherit the level requirements of the slot they end up in

You start with a license from each restock page to sell at the beginning of the game. 

Table Top page is currently not randomized, and not functioning perfectly. you still have to purchuse licenses there.

### Locations

By default these are considered Locations. More may be added in the future.

-Selling products equal to how many come in a box

-Extra Sell Chacks are added for your starting items licenses

-Buying Shop expansions

-Leveling up

#### Cards

Excluding the 80 Ghost Cards, there are 2904 Unique cards in the game, approximately 360 per card pack

Collecting a new card can be enabled to be checks using card sanity. You can choose how many sets are locations. if card sanity is on basic, only new cards in basic packs will be locations. If on destiny basic, all new cards in destiny basic, legendary, epic, rare and basic will be locations

### Items

These are the currently implemented Items 

-Item Licenses

-Employee unlocks

-Expansion unlocks

-DIY Items unlocks

#### 

### Goals

There are three Implemented Goals

#### Shop Expansion
	
Purchuse X shop expansions. The amount can be set in the yaml

#### Level

Get to shop Level X. The level can be set in the yaml

#### Ghost Search

This goal shuffles ghost cards as items into the world, when found in the multiworld they are put in the binder for you to sell or look at. when you are sent X Ghost cards you win. The amount can be set in the yaml