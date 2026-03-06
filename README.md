Plan A - Block Puzzle Challenge
===============================================================


About this project:
---------------------

This is my solution for the Plan A Senior Unity Developer test. I used Unity 6 LTS (6000.3.7f1) with URP. The game is a 6x5 block puzzle where you tap blocks to collect groups of same color.

I didnt receive the art assets that are mentioned in the instructions so I went with solid colors and a clean dark UI. If the assets are available later I can swap them without changing any code.


How to run:
-----------

Open the project in Unity 6 LTS, load the Challenge scene from Assets/Scenes and press Play. Make sure the game view is set to a portrait aspect ratio like 9:16.


Task 1 - Asset Integration:
---------------------------

I started setting up the project structure and the main UI. I created a GameConfig ScriptableObject to keep all the game constants (grid size, moves count, block colors, spacing) so everything is configurable from the Inspector without touching code.

For the UI I used a ScreenSpace Overlay Canvas with CanvasScaler at 1080x1920 reference. The header shows score on the left and remaining moves on the right. I used manual anchors instead of LayoutGroups for the header because in a fixed portrait layout they give more control over the positioning without dealing with ContentSizeFitter.

The Game Over panel is a full screen dark overlay with the final score and a Replay button. It starts deactivated and shows up when moves reach zero.


Task 2 - Data Handling:
--------------------------

I implemented a GameManager singleton that holds score and moves. It uses C# Action events (OnScoreChanged, OnMovesChanged, OnGameOver, OnGameReset) so the UI stays decoupled from game logic.

UIManager sits on the Canvas and subscribes to those events. When GameManager updates score or moves, UIManager gets the callback and updates the TMP texts. I used GameObject.Find with known names to get the references at runtime instead of serialized fields. Its a bit less safe but avoids broken references if the scene gets rebuilt.

The MakeMove test button decreases moves by 1 and adds 10 points per click. When moves hit zero the Game Over panel shows up. Replay button calls ResetGame and puts everything back to initial state.


Task 3 - Puzzle Mechanic:
---------------------------

I splitted the code into a data layer and a view layer. The reason for this is that I wanted the grid logic to be testable and independent from Unity. GridModel is just a plain C# class with a 2D int array, no MonoBehaviour. Each int is a block type, -1 means the cell is empty. BlockCollector does a Breadth-First Search (BFS) from the tapped position to find all connected neighbors of the same type. I went with BFS over recursive Depth-First Search (DFS) because recursive calls can overflow the stack if the grid gets bigger, and honestly BFS with a queue is easier to read. GravityHandler walks each column bottom to top, shifts everything down to close gaps, then fills the remaining empty cells at the top with random blocks.

On the view side BlockView sits on each block GameObject and uses IPointerClickHandler so it works with both mouse and touch. When clicked it tells GridView which cell was tapped. GridView is the one that coordinates the full flow: it asks BlockCollector for the connected group, destroys block GameObjects, tells GameManager to register the move, waits 1 second, then runs GravityHandler and rebuilds the visuals.

The _isProcessing flag in GridView is important. I set it to true at the start of ProcessMove and back to false only after gravity and visual rebuild are done. Any click that comes in while the flag is true gets ignored. Without this the player could tap during the 1 second wait or during gravity and the grid data would go out of sync with the visuals. I considered using a state machine but a simple bool flag was enough for this scope.

I also needed to add Physics2DRaycaster to the camera. Without it EventSystem cant detect clicks on 2D colliders, so IPointerClickHandler on BlockView would never fire. This is something I always forget the first time.

For the blocks I used a white sprite that gets tinted with SpriteRenderer.color. The actual colors are defined in the GameConfig ScriptableObject so adding or removing block types is just editing the array, no code changes needed.
