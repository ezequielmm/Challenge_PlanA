Plan A - Block Puzzle Challenge


About this project

This is my solution for the Plan A Senior Unity Developer test. I used Unity 6 LTS (6000.3.7f1) with URP. The game is a 6x5 block puzzle where you tap blocks to collect groups of same color.

I didnt receive the art assets that are mentioned in the instructions so I went with solid colors and a clean dark UI. If the assets are available later I can swap them without changing any code.


How to run

Open the project in Unity 6 LTS, load the Challenge scene from Assets/Scenes and press Play. Make sure the game view is set to a portrait aspect ratio like 9:16.


Task 1 - Asset Integration

I started setting up the project structure and the main UI. I created a GameConfig ScriptableObject to keep all the game constants (grid size, moves count, block colors, spacing) so everything is configurable from the Inspector without touching code.

For the UI I used a ScreenSpace Overlay Canvas with CanvasScaler at 1080x1920 reference. The header shows score on the left and remaining moves on the right. I used manual anchors instead of LayoutGroups for the header because in a fixed portrait layout they give more control over the positioning without dealing with ContentSizeFitter.

The Game Over panel is a full screen dark overlay with the final score and a Replay button. It starts deactivated and shows up when moves reach zero.


Task 2 - Data Handling

I implemented a GameManager singleton that holds score and moves. It uses C# Action events (OnScoreChanged, OnMovesChanged, OnGameOver, OnGameReset) so the UI stays decoupled from game logic.

UIManager sits on the Canvas and subscribes to those events. When GameManager updates score or moves, UIManager gets the callback and updates the TMP texts. I used GameObject.Find with known names to get the references at runtime instead of serialized fields. Its a bit less safe but avoids broken references if the scene gets rebuilt.

The MakeMove test button decreases moves by 1 and adds 10 points per click. When moves hit zero the Game Over panel shows up. Replay button calls ResetGame and puts everything back to initial state.
