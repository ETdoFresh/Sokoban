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

S02E05 - Sokoban - Basic UI
- Menu Screen with Start Button
- Level Select
- Restart Level

-----------------------------------------------------------------------------------------
Bugs
-----------------------------------------------------------------------------------------