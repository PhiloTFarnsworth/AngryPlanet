# Goo Gods: Angry Planet

## To Play
Fire up Unity and create your own build.  At this point it's somewhat playable, but is on indefinite hiatus.  This readme is more for demonstration purposes and
general information about Angry Planet.

## Game Manual

Goo Gods: Angry Planet, is a 2-D turn-based game where you are beamed to an alien planet teeming with hostile flora and fauna, and you must collect a certain amount of obtanium to be beamed off the planet. The planet itself produces an immune response when you land, generating monsters and plants to thwart your plans. You must navigate the ever expanding bramble, avoid the dangerous fiends and gather a sack full of sparking rocks if you want to make it off this planet.

## Game Controls

Every move a character makes takes a full turn, so act wisely. Players can use "Esc" button to close the game at any time.

### Walking Controls:
    "7" or "q" - Move northwest.
    "4" or "a" - Move west.
    "1" or "z" - Move southwest.
    "c" or "3" - Move southeast.
    "d" or "6" - Move east.
    "e" or "9" - Move northeast.

### Ability Button:

Below the character information panel on the right side of screen is an ability button, which once pressed, will allow the player to use the ability named in the box. To use an ability, wait until its cooldown has gone down to 0 turns, then click the button. You then use the mouse to aim where you want the ability to take effect. All cooldowns and durations are based on game time, and not the player's movespeed (I.E A cooldown is always 5 game turns).

### Abilities:
    -Teleport:  The player moves to the clicked tile.
    -Fireball: An attack which targets all tokens within 2 tiles of the selected tile.
    Leaves fire clouds behind for 2 turns, which damage any token within it 10 health
    per turn.

## Hazards

The plant life on planetside is not only known for its caustic and noxious gas emissions, they also spread rampantly once their flower has grown. While plants can be damaged, even a mighty fireball can only delay their spread across the landscape. Meanwhile, monsters, expelled from the planets crust upon your intrusion, attack players from neighboring tiles. While they are not very bright, they are durable enough to impede your progress and whittle down your health.

## Classes

At the moment, the distinguishing feature between all monsters is their base color. This determines all their stats, which derives into the main catagories of health, attack, defense and speed.

* Red --Strong, but slow. Every move takes 14 turns.
* Yellow ^
* Green | Even stats. Every move takes 10 turns.
* Blue |
* Pink --Fast, but weak. Every move takes 6 turns.

These stats affect both their regular attacks, as well as their special abilities.

## Pickups

Obtainium is collected by moving onto a tile with a sparking rock ontop of it. The player receives 5 obtanium per pickup.

## Victory

Upon surpassing the level's obtanium goal, the player is beamed back up to the ship. The captain's thirst for obtanium means any victory is short lived, and his demands only grow if you succeed.

## Inspiration

At the most fundamental level, I wanted to create a living maze, in which the player must navigate, gather items, avoid enemies as the confines of the maze itself shift. Inspired by dungeon crawlers like Dungeon Crawl (http://crawl.develz.org/), I felt a turn based system would be interesting to implement. Instead of a square grid, I also wanted to implement a hexagon tile system to avoid the pitfalls of balancing diagonal movement, which was greatly aided by a wonderful write-up at Red Blob Games (https://www.redblobgames.com/grids/hexagons/). Finally, I had wanted to dabble in procedural generation of monsters and characters, and was greatly inspired by this blog post (http://web.archive.org/web/20080228054405/http://www.davebollinger.com/works/pixelrobots/).

## Extra Info
The 'map' that the player character navigates in Goo Gods is generated as the character moves along.
While this removes the possibility of backtracking to previously visited areas of the map, the way the plants spread across the map those areas would be rendered inaccessible anyway. Not only does this method reduce the overhead on what the game has to render, but it also is convenient way to manage generated objects and destroy them when they are no longer relevant to the player. The tiles that compose the map itself are based on Unity's Animated Tile objects, though instead of manually defining them all in the editor, there is one 'prototype' animation, which we adjust with the sprites that correspond to the tile selected during generation, then set the tile on Unity's tilemap.

The player, plants, monsters and items on the map are all subclasses of a GameObject dubbed a 'Game Token'. The 'Game Token' system is modeled after a board game, where you have a board below, and anything you place on top would be considered a token. While my inexperience in OOP methods are somewhat exposed in the source code, it has been useful to use inheritance to carry properties common properties like coordinates, and common methods like moving tokens. We carry this further with 'mob' scripts, which control how both player and AI-monsters are generated and rendered. In future iterations, I would like to have players and AI-monsters have access to the same abilities. This system should make that easier, where I can define the behaviors then plug them into the associated 'mobAI' and 'player' scripts.

Goo God's monster generation itself is an interesting process. At the moment, there is a "dna" system, where each monster is composed of random numbers between 0-255, which correspond to a body part to be rendered. On the player's end, this 'dna' is also used to generate a name for the player, displayed on the righthand information panel. There is plenty of room for refinement for the method, but the basic concept should mean that every combination of 'dna' has a unique appearance and name. However those dna changes aren't entirely distinct and I couldn't blame a player for not knowing the difference between a [214,99] Monster and a [12,138,233,96] Monster, the stats of a monster are determined by their color. These variants allow the player to identify the risk posed by the monster that might not be apparent otherwise. When abilities become further fleshed out, I expect that it will be interesting to see how creatures with identical abilities play with different stats.

## Code Information

Important scripts for understanding this project include:

* LevelGenerator.cs: Tile generation and the update function that updates creature movement.
* GameBoard.cs: Controls the master dictionary where game tokens are listed.
* GameToken.cs: The basic abstract class of an object placed in a level.
* Plant.cs: the class which generates the plant life and controls their behavior.
* Mob.cs: describes a monster, abstract Game Token script shared by mobAI and Player.
* Player.cs: controls most inputs and specific triggers for player behavior.
* MobAi.cs: The very simple decision maker for AI monsters.
