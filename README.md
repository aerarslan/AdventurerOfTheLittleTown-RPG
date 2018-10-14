# Text-Based-RPG [Object Oriented Programming]

## Notice

Development has ended!

## Text Based RPG

I created this repository to make practice of my OOP (Object-Oriented-Programming) skills. In this project, I'm going to make a simple Text Based RPG game that contains basic RPG game characteristics in C# on Windows Form Application.

## What are the classes/objects in our game?

For our game, we want to do these things:

* The player goes to locations.
* The player gets stronger when level up.
* The player can trade with vendors/stores in a certain Location.
* The player may need to have certain items to enter a location.
* The location might have a quest available.
* To complete a quest, the player must collect certain items and turn them in.
* The player can collect items by going to a location and fighting monsters there.
* The player fights monsters with weapons.
* The player can use a healing potion while fighting.
* The player can change the active weapon.
* The player receives loot items after defeating a monster.
* After turning in the quest, the player receives reward items.
* The player can choose to revive by sacrificing max hit points after death.

So, the nouns (classes/objects) in our game will be Player, Location, Item, Quest, Monster, Weapon and Healing Potion. We'll need a few more, but we can start with these.

## What's next?
This is a very simple RPG, and there is a lot you can do to expand it.

Here are a few ideas:

* Save the player's current game to disk, and re-load it later (Currently I did it with XML)
* Add a level requirement for some items
* Add a level requirement for some locations
* Add spells and scrolls
  * Level requirements for the spells
  * Cooldown (turn based) system for the spells
* Add different kinds of potions: increased damage, critical damage, etc.
* Add curse system : freeze, paralyze, poison, etc.
  * Add special weapons with curse ability
* Add randomization to battles
  * Determine if the player hits the monster
  * Determine if the monster hits the player
* Add player attributes: strength, dexterity, etc.
  * Use attributes in battle: evade chance, critical hit chance, critical damage, etc.
* Add armor and jewelry
  * Makes it more difficult for the monster to hit the player: defense point, armor etc.
  * Has special benefits: increased chance to hit, increased damage,critical hit chance, etc.
* Add crafting skills the player can acquire
* Add crafting recipes the player use
  * Require the appropriate skill
  * Requires components (inventory items)
* Make some quests repeatable
* Make quest chains (player must complete the Quest X before they can receive Quest Y)
* Add pets
  * Can help the player in battle by attacking opponents
  * Can help the player in battle by healing the player
  * Can help the player by cursing the opponents

There are also more programming techniques you can learn to make the program a little cleaner. In our code we already used some of these techniques.

* LINQ, when searching lists
* Events/delegates, to handle communication between the logic project and the UI project – which will let you move more logic code out of the UI project
* BindingList, so you don't have to repeatedly repopulate the DataGridViews and ComboBox in the UI

## What will the game look like?

![adventurer of the little town](https://user-images.githubusercontent.com/42182119/46920486-a102ff00-cff7-11e8-9c42-8cb35769d625.jpg)

![adventurer of the little town 2](https://user-images.githubusercontent.com/42182119/46920511-07881d00-cff8-11e8-9e3d-e3145c6d756f.jpg)
