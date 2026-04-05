using System.Text.Json;

// ============================================================
// MAIN PROGRAM
// ============================================================
class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        ShowBanner();

        try
        {
            // Present load options, and fall back to game creation
            Game game = LoadGame();
            if (game == null)
            {
                Console.WriteLine("\n  Select game:");
                Console.WriteLine("    1. Numerical Tic-Tac-Toe");
                Console.WriteLine("    2. Notakto (Impartial Tic-Tac-Toe)");
                Console.WriteLine("    3. Gomoku");
                Console.Write("\n  Choice [1-3]: ");
                string gameType = Console.ReadLine()?.Trim() ?? "1";
                if (gameType is not ("1" or "2" or "3")) gameType = "1";

                game = GameFactory.CreateGame(GameTypeNumericToGameType(gameType));
                game.StartGame();
            }

            RunGame(game);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Game has encountered an unrecoverable error: {ex.ToString()}");
        }

        Console.WriteLine("  Press any key to exit...");
        Console.ReadKey();
    }

    private static void ShowBanner()
    {
        Console.Clear();
        Console.WriteLine("                ╔══════════════════════════════════════════╗");
        Console.WriteLine("                ║      Board Games Framework v3.0          ║");
        Console.WriteLine("                ║  Numerical TTT  •  Notakto  •  Gomoku    ║");
        Console.WriteLine("                ╚══════════════════════════════════════════╝");
        Console.WriteLine();
        Console.Write("Note: for best display results, please maximise your command window ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("NOW.");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static string GameTypeNumericToGameType(string gameTypeNumeric)
    {
        switch(gameTypeNumeric)
        {
            case "2": return "notakto";
            case "3": return "gomoku";
            default: return "tictactoe";
        }
    }

    private static void RunGame(Game game)
    {
        // THE GAME LOOP (Iterative instead of Recursive)
        // This runs until 'Finished' is set to true in ResolveTurn
        string command = "turn"; // first turn is automatic
        bool finished = game.Finished;
        while (!finished)
        {
            switch (command)
            {
                case "turn":
                    game.PerformTurn();
                    finished = game.Finished;
                    break;
                case "help":
                    game.ShowHelp();
                    break;
                case "quit":
                    finished = true;
                    break;
                case "load":
                    game = LoadGame();
                    finished = game.Finished;
                    break;
                case "save":
                    SaveGame(game);
                    break;
                case "undo":
                    UndoTurn(game);
                    break;
                case "redo":
                    RedoTurn(game);
                    break;
                default:
                    break;
            }
            
            if (!finished)
                command = game.WhoseTurn is Human ? GetCommand(command).ToLower() : "turn";
        }
    }

    private static string GetCommand(string currentCommand)
    {
        string helptext = "\n";

        if(currentCommand == "turn")
            helptext += "Press Enter for next turn or ";

        helptext += "Type a command and press Enter";

        if(currentCommand == "turn")
            helptext += "\n(use 'help' for rules and a list of available commands)";

        Console.Write($"{helptext}" + Environment.NewLine + "> ");
        string command = Console.ReadLine();
        return string.IsNullOrWhiteSpace(command) ? "turn" : command;
    }

    private static void SaveGame(Game game)
    {
        string json = JsonSerializer.Serialize(game.CaptureState(), new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory("saves");
        string filename = Path.Combine("saves", $"savegame_{game.GameType}_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        File.WriteAllText(filename, json);
        Console.WriteLine($"  💾 Game saved to {filename}");
    }

    private static Game LoadGame()
    {
        // Check for saved games
        string[] saveFiles = Directory.Exists("saves") ? Directory.GetFiles("saves", "savegame_*.json").OrderByDescending(f => f).ToArray() : [];
        Game game = null;

        if (saveFiles.Length == 0)
        {
            Console.WriteLine("\n  💾 No saved games found:");
            return null;
        }

        Console.WriteLine("\n  💾 Saved games found:");
        for (int i = 0; i < Math.Min(saveFiles.Length, 5); i++)
            Console.WriteLine($"     {i + 1}. {Path.GetFileName(saveFiles[i])}");
        Console.Write("\n  Load a save? (enter number or press Enter for new game): ");
        string choice = Console.ReadLine()?.Trim();

        if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= Math.Min(saveFiles.Length, 5))
        {
            try
            {
                string json = File.ReadAllText(saveFiles[idx - 1]);
                var state = JsonSerializer.Deserialize<GameStateMemento>(json)!;
                game = GameFactory.CreateFromState(state);
                Console.WriteLine("  ✅ Game loaded successfully!");
            }
            catch (DeserialisationException ex)
            {
                Console.WriteLine($"Game deserialisation failed! Check your file. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Failed to load: {ex.Message}");
            }
        }

        return game;
    }

    private static void UndoTurn(Game game)
    {
        if (game is null)
        {
            return;
        }

        try
        {
            game.UndoMove();
            if (game.Player2 is Computer)
            {
                // Do a second undo, otherwise the computer player will step in and play again. We'll undo back to before the human player last played.
                game.UndoMove();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Undo failed. Reason: {ex.Message}");
        }
    }

    private static void RedoTurn(Game game)
    {
        try
        {
            game?.RedoMove();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redo failed. Reason: {ex.Message}");
        }
    }
}
