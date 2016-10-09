# Path_Cutthroat
This project solves the combinatorial game of Cutthroat when played on a path of any length. I first wrote PathCutthroat.cs which takes the first step in reducing this game to the game of [Nim](https://en.wikipedia.org/wiki/Nim) by associating each path (determined by length) to its [Grundy number](https://en.wikipedia.org/wiki/Sprague%E2%80%93Grundy_theorem) using dynamic programming and the fact that removing a vertex from a path results in two paths of smaller length. Running PathCutthroat.cs will print to the console window all grundy numbers that were calculated, as well as then printing every second-player-wins paths (paths with Grundy number 0), it turns out that there are only 12 second-player-wins games and none after 408 (I have no proof of this fact but tested this using PathCutthroat.cs up to paths of length 35,000).

Next I implemented the game itself using a simple console window interface and then programmed the computer to play (perfectly) against a user (this is run through the Main() method of Game.cs). The computer plays perfectly (see CalcMove() method) by looking at the current state (a bunch of paths) as a game of nim (converting the paths to Grundy numbers using methods of PathCutthroat.cs) and then finding the [perfect play](https://en.wikipedia.org/wiki/Nim#Mathematical_theory) move in that game of nim, and finally converting that move back into a move of Cutthroat.
