# Unity 2D Topdown Template
Unity template for a 2D top down, drag-and-drop style building game.

Unity Version: `2021.3.13`

## Behavior Details
Players can create new play scenes, select from their inventory to add 
game-entities to a discrete grid play space, and "run" the factory they've 
created.

Some game-entities can be selected, drag/dropped, rotated, and destroyed by the 
player. 
Game-entities can also act upon other game-entities. 

Players can go back and forward in the history, both in terms of how they've 
moved game-entities, and the automations their factory has performed.

The game developer can create new game-entities and give them both common and 
custom behaviors.

## What's included?
- Scenes:
    - GameStart
    - LoadGame
    - PlayScene
- Systems:
    - Save:
        - New game
        - Load game
        - Delete save
        - Auto-save
    - Play Space
        - Configurable grid size
    - Camera:
        - Zoom
        - Panning
        - Panning bounds
    - Inventory:
        - Full inventory modal
        - Inventory hotbar
        - Hotbar item selection
    - Player Input:
        - Camera zoom+pan controls
        - Game-entity interactions:
            - Select (single & multi)
            - Drag/drop
            - Rotate
            - Copy
            - Delete
        - Undo/redo controls
        - Play/pause controls
    - Game Entities
        - Shared behaviors
        - Programmable autobehaviors
        - Repository for managing existence and availablity of game-entities
    - Game Entity Management
        - Multilevel grid layers
        - Enforced discrete position
        - Enforced single occupation per position
        - Query by position
        - Insert
        - Remove
    - Game Tick:
        - Tick event
        - Configurable tick duration
        - Execution of game-entity autobehaviors per tick
    - Developer Toggles:
        - Admin mode
        - Telemetry display
        - Game-entity debug display