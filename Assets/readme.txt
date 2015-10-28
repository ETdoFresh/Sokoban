S02E00 - Sokoban - Getting Started
- Start Unity Project
- Create a Ground Plane/Quad
- Create a Player Capsule
- Add a few scripts so that the Player Capsule can walk around the Ground (old 2d style!)

S02E01 - Sokoban - The Boxes
- Create Solid boxes/Obstacle
- Refine Grid System

S02E02 - Sokoban - The Incredible Moving Box
- Create Moving boxes
- Create Targets
- Create Simple Win Condition
- Refine Grid System

S02E03 - Sokoban - Quick SSH/Git
- Setup a remote Git on SSH

S02E04 - Sokoban - Read Level File
- Created Prefabs
- Created Level Class to hold basic Level Data
- Created a static Level Reader to read text in 2d char array
- Moved functions around in LevelController and MainGrid
- Removed some event code in LevelController
- Process for loading a level
  - Scene starts up
  - LevelController OnEnable read Level File (Creates Level Class)
  - LevelController OnEnable create grid object
  - LevelController Start instantiates object from Level Class

S02E05 - Sokoban - Basic UI / Game Flow
- Logo Scene
- Menu Scene with Start Button
- Game Scene (what we've already done so far)
- Game Controller to mange the transitions between Screens (Prefabs)
- Changed Camera on Sokoban Scene

S02E06 - Sokoban - More on Game Flow
- Restart Level
- Added a Second Level
- Added a Complete Menu
- Added a Fail Menu
- Renamed Some Scenes to Make Better Sense

S02E07 - Sokoban - Level Select and Scoring
- Count Number of Moves
- Level Select

S02E08 - Sokoban - Bug Fixing Session
- Stop movement when Menu Screen is up!
- Resize Ground based on level size

S02E09 - Sokoban - Cinematic Component
- Create a structure so that cinematics happen before, after, or during the level
  - Pause Game, and watch (like a mini movie)
  - Cinematics for this game should be no more than 10-20 seconds

S02E10 - Sokoban - Create a destrudible box!
- Create destructible boxes... ie fail condition (x moves and box go boom)
- Create a timer (another fail condition perhaps?)


  Later...
  - Finish Cinematics (After last level, ie THE END)
  - Come up with a cinema script (like text file explaining camera, sounds, effects, etc)
  - Level changing dynamics, such as additional boxes and targets
  - Create animation as player moves from cell to cell
  - Add some push sounds and basic music
  - Track scores, track progress, track level completion, etc.

-----------------------------------------------------------------------------------------
Bugs
-----------------------------------------------------------------------------------------
- On Ending Cinematic, the cinematic destroys itself... it shouldn't
- Ending Cinematic show up during/after Complete Menu shows up. Should play cinematic first, then show complete menu
