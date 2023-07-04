# CS50 Final Project - Freedom Hero

_"Freedom Hero" is the title of a small, turn-based game which uses "dices" as it's main mechanic for actions. The idea was given birth by my best friend and I, and as such, in one of our talks, we decided if I could program a small version of the idea as an actual videogame for the project of this course: This is the result._


## Underlying structure of the Game - Reasons for submission.

In this segment, it will be explained the underlying structure of this project, and along the way, the explanation of why I believe this project meets the distinction and complexity requirements to be submitted as the Final Project of this course.

### General Structure - Scenes.

On a summarized explanation, the core of the game is made by 3 scenes: _Character Creation Scene_, _Map Scene_, and _Combat Scene_. You could call these 3 the states of the game. Other small scenes are also present, like the Title Screen, Game Over, Victory screen and Credits; but the complexity of the game is focused on the 3 scenes described earlier.
Now, we'll delve deeper into these 3 scenes, signaling the distinctiveness and complexity of each.

### Character Creation Scene

The Scene where you create your character. Most of the data needed for this scene isn't in the code for the scene exactly; this data is stored on JSON files on the [Assets/StreamingAssets] folder.
Example of these are the CharacterTexts.json, the PlayerSkillFile.json and the ClassDescriptions.json.

These JSON files, specially the 2 later ones, have an underlying structure that allows its easy readability, to be later imported into data classes in the game. These files were made specifically with that function in mind, also allowing those to be extended in a quite simple manner for future needs.

Files aside, the functionality of this scene gives tribute to some table games where one can choose their class, and also, assign its own stats by the use of dices. This mechanic is the core of creating your character in the game. But in here, a little twist was added.

In here, the way you assign your stats will also have an impact on what Skills you can currently equip on your Light, Heavy and Special slots. Be sure to choose a combination that will fit you, as not always the strongest skill might be the best on your build!

### Map Scene

Arguably, one of the "hardest" scenes to make, and one which I had fun doing, is this one. The Map.

While the Map itself is already "defined" [as a hidden 9x9 tiles square], the way the map is build is based on your movement choice. You start in the center of the map, with only that tile unlocked. As you roll the dice and decide your moving direction, the map will "expand" [actually revealing more of the hidden tiles], giving the player the feeling that their choice affect how the map grows.
One of the things that is semi-randomized, is the presence or absence of locked edges. These locked edges represent an area you can't traverse, so the map itself becomes more varied with the way you decide to roam.

The content of the tiles itself are decided on-demand, that is, when you decide to move to a place not yet discovered, a new tile will be generated, and the content of it will be randomized: You can get an event popping out, finding an item, an enemy, or who knows? Maybe you'll get the exit on your first move! (Okay, no, this is impossible. There are certain conditions for the Exit to be able to pop out, but I'm not telling those here, you better find out while playing!)

In summary, what happens on this scene is the following:

-You roll the dice, to see how much spaces you can move. The movement is done tile per tile.

-When you move to a direction there is no tile, a tile will be generated on-demand.

-When the title is being generated, some variables are taken into consideration (example the number of tiles moved, the last time you found an event tile, number of enemies) to see if the tile will have an event or not, to add variety to the gameplay.

-The player will move to the tile.

-If the tile has an event, this will be processed based on the event type.

-Continue with the movement until your movement number is over, and then roll the dice again.

I do believe this scene has a good level of differentiation and complexity to be considered in the project. After all, it adds a good twist to the map generation and also include some nice mechanics to process events on the tiles.

### Combat Scene

The more complex scene to program to be honest, I won't lie.

The Combat is a turn-based RPG combat in first-person view, where the enemies are on the center of the screen, the battle log indication is on the top, and your character's stats is at the bottom. In this game, you can be faced against at most 3 enemies at the same time; so think about your skill usage carefully.

Besides the base stats described above, there are 2 more stats for your character:

- Action Points (AP): Max at 3, it is consumed when using Heavy or Special skills, it's recovered by 1 each turn (as said before)

- Shield: Some skills might generate you a shield, that serves as a cover you to some damage.

The code for the Combat is managed on 1 component (except the enemy blinking code, which is stored on another class). This choice was made so the core of the combat could be re-used on other turn-based combat RPG games in the future.

The turn order is stored on a Queue, starting with the player and then the enemies, allowing a set order to be followed. After popping a turn, it is processed.

Both the player turn, and enemy turn have a similar structure, except that the player can use items. 

On your turn, if you choose a Skill, you'll have to throw the Dice of Willpower, as the damage is also calculated using dices. The formula for calculating the damage of the skill is as follow:

_-Light Attack:_ Roll a 4-sided Dice of Willpower. The damage is equal to the result of the dice plus the L stat of your character.

_-Heavy Attack:_ Roll a 20-sided Dice of Willpower. The damage is equal to the result of the dice plus the H stat of your character.

_-Special Attack:_ Roll a 6-sided Dice of Willpower. The damage is equal to the result of the dice multiplied by the S stat of your character.

After dealing the damage, it is verified if the skill has a secondary effect. Those being Extra Damage, Status Condition, generating a Shield or Recovering HP.

The effects can be fixed or variable. For variable effects Dice of Willpower are used. 

After verifying and processing additional effects, the turn ends, the player recovers 1 AP and its turn is set at the end of the queue, a new turn is pulled, and the next turn comes.

The enemy processing is similar to the player, after all, they do have a similar rule to calculate their damage. A difference being that the selection of the skill used by the enemy is randomized, using a "seemingly dice mechanic". The enemy has 1 to 2 numbers in the range of 1 to 6 that are set to do their Heavy and (in case of Greater Enemies) Special attack. A random number between 1 and 6 is "rolled", and if the number coincides with the enemy's set number, their respective skill is used, otherwise, only a Light attack is used.

Additional effects are processed the same way for enemies. Except that instead of using a dice like the player, the enemy only generates a random number if the skill has a variable effect.

I do consider that, while this combat is a somewhat basic turn-based RPG combat, I do believe the addition of the Dice mechanic and the way it was built is somewhat innovative. Also it is complex enough as it is able to manage all aspects from combat in a very understandable manner, as it can be adapted for future projects.

As it was explained on the Character Creation Scene, the data structure for the enemies, and the player skills are also stored on a JSON file, with a underlying structure that can be read by the game and turn it into usable skills and enemies of all sort, making it reusable and extendable if more variety wants to be added on the future.

### Additional

When building the game, it should be build with the resolution of 1024 x 768. The game and assets were made so they work in that resolution only.

## Game Instructions

Even though some of the information of the game has been told already. Here, we'll kind of simulate an instruction booklet, giving a small walkthrough of the game.



### Starting the game - Creating your character.

At starting the game, you'll be guided by an omnipotent entity, which will ask you for your name (Not assigning a name will set the player’s name as "Asuka"). After inputting your name, you'll be asked to choose 1 of the 4 classes: Archer, Mage, Paladin and Vampire. 
Different classes not only have different skills and playstyle, but they also confer you an additional bonus to 1 of your stats.

-Archer: Specializes at dealing damage, sometimes to multiple enemies. Gives you +1 to your Light attack stat.

-Mage: Specializes in spells, causing status conditions. Gives you +1 to your Heavy attack stat.

-Paladin: Specializes in endurance, as it's hard to take down. Gives you +20 HP.

-Vampire: Specializes in healing itself through attacks. Gives you +1 to your Special attack stat.

As you could guess, the character you choose will affect the skills you'll be able to use, and affect your stats a bit, so choose carefully.

After having chosen your desired class, you'll have to roll the 4 "Dices of Willpower". These dices are used to much stuff in the game. This time it will be used to "Imbue Power" into your character, to determine the amount of power you can give to any of your stats (HP, L, H, S). All your stats start at 0 and HP at 60.
(Note that a point in HP equals +20 HP, so in example, if I destine a value of 3 got from a dice to HP, you'd get +60 HP)

_-HP: Increases your Health Points, letting you be able to take more hits_

_-L: Light attack stat. Influences the power of your Light skill_

_-H: Heavy attack stat. Influences the power of your Heavy skill_

_-S: Special attack stat. Influences the power of your Special skill_

A thing to have in consideration is that stat distribution is made with the whole value of any of your dices, but the value as a whole. For example:

_If you have rolled the following numbers [3, 1, 4, 3] that means you can assign a combination of those 4 numbers to your stats, but you can't, for example, add 5 to one of your stats and subtracting the result from another dice._

Think well about the stat assignation, as it will affect the skills, you'll be able to choose. That is the final step for creating a character.

Once the assignation is done, the initial bonus for your class is added to the total of your stats, and then, you start choosing the skills, starting from the Light one, the Heavy one and lastly the Special one.

Now your character is created! It's time to reach freedom!!

### In Game - Map

The Map is an isometric-looking stage, where you'll roll a dice to check the number of steps you can do. The map grows as you advance, so finding dead ends might happen at some point. Get ready to be free!

Now your character appears on the map. On the top left, you have your character name, portrait and current HP/Max HP, on the top middle you have a button to Pause the game (there is an option to quit game) and on the top right, the number of steps you can do.
At arrival, you'll be asked to roll a dice, to determine the amount of steps you can do. After finishing moving, you will be asked to roll it again, until you're free.

The map expands as you move, so choose a route carefully. Also, while moving, you might find some events happening, maybe a random item will be there for you to pick, a trap might spur, a resting spot, or maybe even an enemy ambush! 
(Note: Enemy encounters reset your movement to 0, so you'll be asked to roll the dice again after combat)

As mentioned above, there are some special tiles on the map that might trigger different events. 

_The Event Tile: A random event will appear, such as short rests to recover HP, traps that reduce them, or even enemy encounters._

_The Item Tile: You will receive a random item._

_The Enemy Tile: Will trigger a random encounter._

_The Greater enemy Tile: Will trigger a special, stronger battle. Maximum of 2 of those will appear in the map._

Pave your path and reach freedom, regardless the risks ahead!

### In Game - Combat 

Combat occurs whenever you find an enemy tile, a greater enemy tile, or an event that describes an encounter. Combat is viewed with the enemies in the middle, the logs on the top and the character status and command screen at the bottom.

Battle starts with your turn; you have two options on the Command Menu:

-Skills: Use one of your skills.

-Items: Use an item (at the beginning you're giving one small potion and one middle potion)

Using skills will bring you to the skill menu, where it will show the skills you selected on the selection screen. Heavy and Special Skills do cost AP unlike Light Skills, which are free to use.
After selecting the skill, you have to roll a dice according to the skill type (L = d4, H = d20, S = d6), after this, damage is calculated with the already explained formulas and applied to the enemies.

The max amount of AP you can have is 3, so it's recommended to use Skills when the situation merits it.

There are skills that have additional effects, those were generally described before, but here we will go a bit more in depth about those:

-Extra Damage: Deals additional damage to the enemies. It can be fixed or variable. With variable having you throw another dice to apply the extra damage.

-Status Condition: Applies one of the status conditions described below. Can be fixed or variable. With variable you'll have to throw a d20 dice and get a value above 10 to apply the effect.

Status Conditions are classified in 2 categories. Action Conditions and Damage-Over-Turn (DOT) Conditions.

Action Conditions are those which affect the action of the one who has been affected by it, such are:

Frozen: Character unable to act for 2 turns.

Sleep: Character unable to act for 1 turn.

Paralyze: Character unable to act for 3 turns. Has a 50% to heal by itself each turn.

DOT Conditions are those that cause continuous damage to the one affected by it, and they fade away after a certain number of turns, or when the affected one reaches 1 HP. Such are:

Poison: Character takes 3 points of damage at the end of turn, for 4 turns.

Burn: Character takes 5 points of damage at the end of the turn, for 3 turns.

Bleed: Character takes 10 points of damage at the end of the turn, for 2 turns.

-Generate a Shield: A shield will be generated that will take an amount of damage from you. Usually a dice is thrown to determine the amount of shield you get.

-Recovering HP: You will recover some HP. Usually a dice is thrown to determine the amount of HP healed.

After applying the extra effects, your turn will be over, and the enemy(ies) turn will start. Enemies’ actions are similar to the character's actions, but with the difference that they are more random.

Play smart and win the battles ahead of you.

### Winning the Game

To win the game, you have to progress through the map, and find the "EXIT" tile. Once you've found it, the game will be over, you'll find freedom!

## Credits

Game Idea: Alex Ferrín and Arturo Mendoza.

Game Programming: Alex Ferrín

Game Art - Characters, enemies, and backgrounds: Grecia Vera

Game Art - Assets: Alex Ferrín

Music: RPG Music Pack - David Vitas (@davidvitas - Twitter)
