A OOP programing assignment for Masters of Computer Science - Subject IFQ584

### Assignment details

In this assignment, your task is to develop the  numerical Tic-Tac-Toe game, where the board size  is specified by the user before each game begins, in a C# console application on .NET 10. The game provides a text-based command-line interface (e.g. using either ASCII or Unicode) for playing from a terminal. 

The program supports two different modes of play, including: 

- Human vs Human 
- Human vs Computer 

With human players, the program checks the validity of moves as they are entered. With computer players, the program makes a move that immediately wins the game; otherwise, if no immediate winning move is available, the computer player randomly selects a valid move. 

The program should provide a simple in-game help menu system to guide users with the available commands. Additionally, it can provide some examples of commands that may not be immediately apparent to users. 

#### Numerical Tic-Tac-Toe

Numerical Tic-Tac-Toe, a variation of the classic game invented by mathematician Ronald Graham, uses numbers 1 to 9. Two players take turns putting odd numbers (player 1) and even numbers (player 2) into the blank squares of a 3×3 board. Each number can be used only once. The first player to score 15 points (sum of 3 numbers) in a horizontal, vertical, or diagonal line wins the game. You can create a paper version of the game using the resources from [Numerical Tic Tac ToeLinks to an external site.](https://mathequalslove.net/numerical-tic-tac-toe/) (Math = Love, 2021).

Numerical Tic-Tac-Toe can be generalised to an  board, where the numbers  are divided between the two players (as odds and evens). The game plays exactly the same as the original 3×3 game, where two players alternately play by placing one of their numbers on the board of size . The first player to complete a line of  numbers that add up to  is the winner. 

#### Gameplay

Your program begins by prompting the player to select the desired game modes as well as the size of the board . 

For each player’s turn, the program displays the current game board and prompts the player to make a move, or view the help menu. When the player chooses to make a move, their turn ends, and the other player’s turn begins. If the other player is a computer player, they make a move immediately without prompting. If the player chooses to view the help menu, the available commands and possible examples are displayed. 

The two players take turns to make moves until the game is over. The game ends when one of the players wins the game or when the board is completely filled and no more move is possible; in this case, the game ends in a tie. When a game finishes, the program displays the final board and reports the results before exiting. 

Your program should also be able to handle invalid inputs by prompting the player to re-enter until a valid input is provided. 

#### Submission instructions

To submit, you must create a ZIP file that contains all your C# project files. Specifically, zip your working implementation, including the complete C# project source code for .NET 10, and upload this ZIP file to the submission point on this page. 

The submitted project files will be compiled and executed with .NET 10. **You must make sure that your submitted code can be compiled and run properly under this environment.** 

Uncompilable or inexecutable source code cannot be marked and will receive a zero. To confirm the version of .NET on your computer, simply open a terminal and execute the following command: 

dotnet --version 

To check that your project code can be compiled and executed on .NET 10, open a terminal in the folder containing the project file (.csproj) and run the following commands: 

dotnet clean dotnet run 

#### References

Math = Love. (2021). _Numerical Tic Tac Toe._ https://mathequalslove.net/numerical-tic-tac-toe/

### Assignment criteria

1. The game with board size 3 can be successfully played from start to finish as specified in the Gameplay section.
2. The game with arbitrary board size  can be successfully played from start to finish as specified in the Gameplay section.
3. The computer player can make a winning move whenever one is available; otherwise, it randomly selects a valid move.
4. Usability and code quality are excellent. The user interface and commands are simple and clear, with a helpful menu system. The program is bug-free and can handle invalid inputs. The source code is well-structured and commented for readability.