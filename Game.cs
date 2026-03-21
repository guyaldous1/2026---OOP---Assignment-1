abstract class Game : IGameContext
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

    private PlayerFactory playerFactory;

    // Expression-bodied properties for cleaner logic
    public Player WhoseTurn => TurnNumber % 2 == 0 ? Player2 : Player1;

    public Game()
    {
        this.playerFactory = new PlayerFactory(this);
    }

    public Game(GameStateMemento state) : this()
    {
        this.Player1 = this.playerFactory.CreateFromState(state.Player1Type, 1);
        this.Player2 = this.playerFactory.CreateFromState(state.Player2Type, 2);
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


        this.Player1 = this.playerFactory.CreateHumanPlayer(1);
        bool p2IsHuman = (mode == 1);
        this.gameMode = p2IsHuman ? "HvH" : "HvC";
        this.Player2 = p2IsHuman ? this.playerFactory.CreateHumanPlayer(2) : this.playerFactory.CreateComputerPlayer(2);
        
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
            BoardSize = this.Boards[0].Size,
            BoardCount = this.Boards.Length,
            CurrentPlayerIndex = WhoseTurn == this.Player1 ? 1 : 2,
            Player1Type = this.Player1.CaptureState(),
            Player2Type = this.Player2.CaptureState(),

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

    public abstract void DrawBoards();

    public Board GetBoard(int index = 0) => Boards?[index] ?? null!;

    public Piece[] GetPieces() => Pieces;

    protected abstract void InitializeBoards();
}