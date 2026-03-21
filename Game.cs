abstract class Game
{
    // Properties - marked as 'null!' because they are initialized in Setup()
    public Player Player1 { get; private set; } = null!;
    public Player Player2 { get; private set; } = null!;
    public Board[] Boards { get; protected set; } = null!;
    public Piece[] Pieces { get; protected set; } = Array.Empty<Piece>();
    
    public int TurnNumber { get; private set; } = 1;
    public bool Finished { get; protected set; } = false;

    public virtual string GameType { get; }

    private string gameMode;

    // Expression-bodied properties for cleaner logic
    public Player WhoseTurn => TurnNumber % 2 == 0 ? Player2 : Player1;

    public Game()
    {
        // Constructor is now "Lightweight." 
        // It only prepares the object, it doesn't run the game.
    }

    public Game(GameStateMemento state)
    {
        GameType = state.GameType;
        // TODO stub for now.
    }

    public void StartGame()
    {
        Console.Clear();
        InitializePlayers();
        InitializeBoards();
    }

    public void EndGame()
    {
        Console.WriteLine("--- Game Over ---");
        Console.ReadLine();
    }

    private void InitializePlayers()
    {
        int mode = 0;
        while (mode != 1 && mode != 2)
        {
            Console.WriteLine("-- How would you like to play:");
            Console.WriteLine("1: Human v Human");
            Console.WriteLine("2: Human v Computer");
            
            if (!int.TryParse(Console.ReadLine(), out mode) || (mode != 1 && mode != 2))
            {
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
            }
        }

        this.Player1 = new Human(1, this);
        bool p2IsHuman = (mode == 1);
        this.gameMode = p2IsHuman ? "HvH" : "HvC";
        this.Player2 = p2IsHuman ? new Human(2, this) : new Computer(2, this);
        
        Console.Clear();
        Console.WriteLine($"-- Game Started: Human v {(p2IsHuman ? "Human" : "Computer")} --");
    }

    public void ShowHelp()
    {
        // TODO stub for now
    }

    public virtual GameStateMemento CaptureState()
    {
        var state = new GameStateMemento
        {
            GameType = GameType,
            Mode = this.gameMode,
            BoardSize = this.Boards[0].Size,
            BoardCount = this.Boards.Length,
            CurrentPlayerIndex = WhoseTurn == this.Player1 ? 1 : 2,

            // TODO serialize history
            //MoveHistory = History.GetAll(),
            //HistoryPointer = History.Pointer
        };

        foreach (var board in Boards)
            state.BoardValues.AddRange(board.Squares.Select(s => s.Value?.Value.ToString() ?? "").ToList());

        return state;
    }

    public void PerformTurn()
    {
        DrawBoards();
        Console.WriteLine($"Turn {TurnNumber}: Player {WhoseTurn.Position}'s move.");

        WhoseTurn.DoMove();
        ResolveTurn();

        if (!Finished)
        {
            TurnNumber++;
        }
        else
        {
            EndGame();
        }
    }

    public abstract void ResolveTurn();

    public abstract void ShowRuleForTurn();

    protected abstract void InitializeBoards();

    protected abstract void DrawBoards();

    private string GetCommand()
    {
        // TODO stub for now
        Console.Write("Enter command (Enter for next turn)" + Environment.NewLine + "> ");
        string? command = Console.ReadLine();
        return string.IsNullOrWhiteSpace(command) ? "turn" : command ;
    }
}