using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text.Json;

// ============================================================
// MAIN PROGRAM
// ============================================================
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        ShowBanner();

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

        Console.WriteLine("  Press any key to exit...");
        Console.ReadKey();
    }

    static void ShowBanner()
    {
        Console.Clear();
        Console.WriteLine("  ╔══════════════════════════════════════════╗");
        Console.WriteLine("  ║      Board Games Framework v3.0         ║");
        Console.WriteLine("  ║  Numerical TTT  •  Notakto  •  Gomoku   ║");
        Console.WriteLine("  ╚══════════════════════════════════════════╝");
    }

    static string GameTypeNumericToGameType(string gameTypeNumeric)
    {
        switch(gameTypeNumeric)
        {
            case "2": return "notakto";
            case "3": return "gomoku";
            default: return "tictactoe";
        }
    }

    static void RunGame(Game game)
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
                default:
                    break;
            }
            

            if (!finished)
                command = game.WhoseTurn is Human ? GetCommand().ToLower() : "turn";
        }
    }

    static private string GetCommand()
    {
        Console.Write("Enter command (Enter for next turn, \"help\" for game instructions)" + Environment.NewLine + "> ");
        string? command = Console.ReadLine();
        return string.IsNullOrWhiteSpace(command) ? "turn" : command;
    }

    static private void SaveGame(Game game)
    {
        string json = JsonSerializer.Serialize(game.CaptureState(), new JsonSerializerOptions { WriteIndented = true });
        string filename = $"savegame_{game.GameType}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        File.WriteAllText(filename, json);
        Console.WriteLine($"  💾 Game saved to {filename}");
    }

    static private Game LoadGame()
    {
        // Check for saved games
        var saveFiles = Directory.GetFiles(".", "savegame_*.json").OrderByDescending(f => f).ToArray();
        Game? game = null;

        if (saveFiles.Length == 0)
        {
            Console.WriteLine("\n  💾 No saved games found:");
            return null;
        }

        Console.WriteLine("\n  💾 Saved games found:");
        for (int i = 0; i < Math.Min(saveFiles.Length, 5); i++)
            Console.WriteLine($"     {i + 1}. {Path.GetFileName(saveFiles[i])}");
        Console.Write("\n  Load a save? (enter number or press Enter for new game): ");
        string? choice = Console.ReadLine()?.Trim();

        if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= Math.Min(saveFiles.Length, 5))
        {
            try
            {
                string json = File.ReadAllText(saveFiles[idx - 1]);
                var state = JsonSerializer.Deserialize<GameStateMemento>(json)!;
                game = GameFactory.CreateFromState(state);
                Console.WriteLine("  ✅ Game loaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Failed to load: {ex.Message}");
            }
        }

        return game;
    }
}
