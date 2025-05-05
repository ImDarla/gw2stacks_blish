# gw2stacks
This is a port of zwei2steins [gw2stacks](https://github.com/zwei2stein/gw2stacks) to Blishhud.

Please report encountered errors either via pull requests or to ImDarla on the official Blishhud Discord.

## Tutorial
![Key interface](Docs/KeyInterface.png)

Basic functionality requires an API key with permissions for character, account and inventories to be added using the Blishhud API interface.

![Loading Interface](Docs/waitingForApi.gif)

Clicking the cog icon next to the blish hud icon starts the process of retrieving the informaton of the current account.

![Tab Interface](Docs/diverseTabs.gif)

After the information is retrieved and compiled a window with all of the gw2stacks advice tabs will be opened.

![lDetail Interface](Docs/detailsView.png)

Hovering over one of these items will display the advice for this item, its location and its quantities on your account.

![Blacklist Feature](Docs/ignoreFeature.png)

The blacklist feature for items can be enabled in the module settings.

![Blacklist Function](Docs/ignoreWindow.png)

Clicking on an Item in the advice list with the blacklist feature enabled, ignores the items from further advice. Blacklisted items are saved localy between module executions.

![Search Feature](Docs/searchBar.png)

Searching for an item name filters for this specific item. This feature ignores case and takes module localisation into account.

![Item Feature](Docs/itemFeature.png)

![Character Feature](Docs/inventoryRecreation.png)

Optional item and character based UI types can be chosen in the module settings.

In the settings additional functionality for both of these UI types can be enabled.

For the character based UI this is a switch between compact or normal inventory view as seen in the vanilla game.

For the item based UI this is an optinal shortcut to filter for an item directly.

## Notes
Korean and spanish translations are currently missing and will revert to english for the advice sections


## Permission
I got written permission by zwei2stein on reddit to create this module

## Q&A
>The initial loading takes a long time

Despite only requesting the minimum amount of information to the API, the bucket structure of blishhud api calls might delay requests by some time.

>The module seems frozen

On error the module will close any of its windows, lock the icon and log the error to the tooltip text of the icon and to the Blishhud log file of the session.

>Inventory changes are not displayed

The api requests have an internal cooldown of 5 minutes to reduce this modules impact on other modules.

## Planned features

* Translation for spanish (and potentially korean) users
* expansion of ignored feature to other windows
* expansion of advice to include non exotic/rare gear and containers
