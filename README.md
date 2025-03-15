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

![lDetail Interface](Docs/detailsView.gif)

Clicking on one of the items opens a separate window containing the advice for this item, its location and its quantities on your account.

## Notes
All crafting and salvage advice does not take TP prices into account at this point in time.


## Permission
I got written permission by zwei2stein on reddit to create this module

## Q&A
>The initial loading takes a long time

This is due to the time it takes for the API calls to be answered. Further calls will draw from a cache and be faster.

>The module seems frozen

On error the module will remove its UI elements and log the error to the Blishhud log file of the session.

>Inventory changes are not displayed

The api requests have an internal cooldown of 5 minutes to reduce this modules impact on other modules.

## Planned features

* Taking the current price of items into account when giving advice

* Translation for german, french, spanish (and potentially korean) users
