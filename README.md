## Delivery 03 - Shopping Game

### Description

The goal is to develop an RPG-style item shop. The player must be able to buy and sell items.

### Features

 - Game start and end logic: 3 scenes
   - Title scene (press ENTER to start) -> NAME: Title
   - Gameplay scene (item shop) -> NAME: Gameplay
   - Ending scene (optional) -> NAME: Ending
 - Two inventories available: Player inventory and Shop inventory
   - At least 3 types of items: Food, Potions, Weapons
   - Every items has a cost in Coins, it could be same to Buy/Sell
   - Every item must have some name, description and graphics
   - Consumable items have an additional “LifeRestore” value
 - Items can be selected with Mouse, with UI elements to Buy/Sell/Use them
   - UI Buttons:
     - [Buy Selected Item]
     - [Sell Selected Item]
     - [Use Item]
 - Items usage and Player Life Bar
   - Items in Player Inventory can be Used (consumed)
   - UI Element: Player Life Bar (for consumable items)
     - Damage should be taken on Mouse pressed over bar or specific button
 - Items can be drag and drop between inventories with mouse
   - Equivalent behavior to Buy Selected Item and Sell Selected Item
   - If item is dropped outside an inventory, no transaction is done
 - Buttons and UI elements should be localized: English, Spanish, Catalan
   - UI dropdown box to selected inventory language
   - Items name and description, a box can be used for that
   
#### Additional Features

 - UI/Items Animations: Life Damage, Items, Drag&Drop, Consume Items, Coins drop…
 - Audio fx, background music

### Controls

 - Mouse: Items selection and UI buttons interaction
 - Keyboard:
   - ENTER to start/restart game
   - ESCAPE to exit game and close program

### Developers

 - Alejandro Belmonte ([sarkamist](https://github.com/sarkamist/))
 - Pau Bofi ([PauBofi](https://github.com/PauBofi))
 - Luca De Marco ([LukeByMark](https://github.com/LukeByMark))
 - Laia Campos ([Loyan06](https://github.com/Loyan06))
 - Francina Suñer ([r3daveng3r](https://github.com/r3daveng3r))

### License

This game sources are licensed under MIT license. Check [LICENSE](LICENSE) for further details.
