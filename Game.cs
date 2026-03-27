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

    private PlayerFactory PlayerFactory;

    private MoveHistory MoveHistory;

    // Expression-bodied properties for cleaner logic
    public Player WhoseTurn => TurnNumber % 2 == 0 ? Player2 : Player1;
    public Player WhoseNotTurn => TurnNumber % 2 == 0 ? Player1 : Player2;

    public Game()
    {
        this.PlayerFactory = new PlayerFactory(this);
        this.MoveHistory = new MoveHistory();
    }

    public Game(GameStateMemento state) : this()
    {
        int boardCount = 0;
        int pieceCount = 0;
        string[] boardValues = null;
        string[] pieceValues = null;
        try
        {
            boardValues = state.Boards.Split('_');
            boardCount = Convert.ToInt32(boardValues[0]);
            pieceValues = state.Pieces.Split(',');
            pieceCount = Convert.ToInt32(pieceValues[0]);
        }
        catch
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(Game)}");
        }

        if (boardValues.Length != boardCount + 1 || pieceValues.Length != pieceCount + 1)
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(Game)}'s boards and pieces");
        }

        this.Player1 = this.PlayerFactory.CreateFromState(state.Player1Type, 1);
        this.Player2 = this.PlayerFactory.CreateFromState(state.Player2Type, 2);
        this.TurnNumber = state.TurnNumber;

        this.Boards = new Board[boardCount];
        for (int i = 0; i < boardCount; i++)
            this.Boards[i] = new Board(boardValues[i + 1]); // offset by 1 because of length value at start of string

        this.Pieces = new Piece[pieceCount];
        for (int i = 0; i < pieceCount; i++)
            this.Pieces[i] = new Piece(this, pieceValues[i + 1]); // offset by 1 because of length value at start of string
    }

    public GameStateMemento CaptureState()
    {
        string boardState = $"{this.Boards.Length}";
        for (int boardCount = 0; boardCount < Boards.Length; boardCount++)
            boardState += $"_{this.Boards[boardCount].CaptureState()}";

        string pieceState = $"{this.Pieces.Length}";
        for (int pieceCount = 0; pieceCount < Pieces.Length; pieceCount++)
            pieceState += $",{this.Pieces[pieceCount].CaptureState()}";

        var state = new GameStateMemento
        {
            GameType = this.GameType,
            Boards = boardState,
            Pieces = pieceState,
            TurnNumber = this.TurnNumber,
            Player1Type = this.Player1.CaptureState(),
            Player2Type = this.Player2.CaptureState(),
            MoveHistory = this.MoveHistory.CaptureState()
        };

        return state;
    }

    public void StartGame()
    {
        Console.Clear();
        InitializePlayers();
        InitializeGameBoards();
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


        this.Player1 = this.PlayerFactory.CreateHumanPlayer(1);
        bool p2IsHuman = mode == 1;
        this.Player2 = p2IsHuman ? this.PlayerFactory.CreateHumanPlayer(2) : this.PlayerFactory.CreateComputerPlayer(2);
        
        Console.Clear();
        Console.WriteLine($"-- Game Started: Human v {(p2IsHuman ? "Human" : "Computer")} --");
    }

    public void ShowHelp()
    {
        // TODO stub for now
    }

    public void PerformTurn()
    {
        DrawBoards();
        Console.WriteLine($"Turn {TurnNumber}: Player {WhoseTurn.Position}'s move.");

        Move move = WhoseTurn.DoMove();
        this.MoveHistory.StoreNewMove(move);
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

    public string GetPieceValueForSquare(int squareID)
    {
        return this.Pieces.FirstOrDefault(p => p.LocationSquareID == squareID)?.Value ?? "-";
    }

    public int GetPieceValueForSquareAsInt(int squareID)
    {
        var piece = this.Pieces.FirstOrDefault(p => p.LocationSquareID == squareID);
        return piece != null ? int.Parse(piece.Value) : 0;
    }

    public abstract void ResolveTurn();

    public abstract void ShowRuleForTurn();

    private void DrawPlayerPieces(Player player)
    {
        Console.ResetColor();
        Console.ForegroundColor = player.Colour;
        Console.Write($"Player {player.Position}'s Remaining Pieces:");
        foreach (Piece p in player.PiecesAvailable)
        {
            Console.Write($" {p.Value}");
        }
        Console.ResetColor();
        Console.Write('\n');
    }

    public void DrawBoards()
    {
        Console.Clear();
        Console.WriteLine($"Turn {this.TurnNumber}. It's Player {this.WhoseTurn.Position}'s Turn");
        ShowRuleForTurn();
    
        DrawPlayerPieces(Player1);

        Console.Write("\n");

        //write each board layout
        foreach (Board board in this.Boards)
        {
            for (int i = 0; i < board.Squares.Length; i++)
            {
                if(WhoseTurn is Human human && human.Cursor.Location == board.Squares[i])
                {
                    Console.ForegroundColor = WhoseTurn.Colour;
                    Console.Write($"({human.Cursor.Value})");
                }
                else if(!board.Squares[i].IsOccupied)
                {
                    Console.ResetColor();
                    Console.Write($"( )");
                }
                else
                {   
                    Console.ResetColor();
                    Console.Write($"({GetPieceValueForSquare(board.Squares[i].SquareID)})");
                }
                if((i + 1) % board.Size == 0) Console.Write("\n");
            }
            Console.Write("\n");
        }
        
        DrawPlayerPieces(Player2);
        
    }

    public Board GetBoard(int index = 0) => Boards?[index] ?? null!;

    public Piece[] GetPieces() => Pieces;

    public Board[] GetBoards() => Boards;

    public Square[] AllAvailableSquares => GetBoards().SelectMany(board => board.SquaresAvailable).ToArray();
    public List<Square[]> AllFullLines => GetBoards().SelectMany(board => board.FullLines).ToList();
    public List<Square> AllSquares => GetBoards().SelectMany(board => board.AllSquares).ToList();

    protected abstract void InitializeGameBoards();
    protected void InitializeBoards(int size, int boardCount, string pieceType)
    {
        //create boards
        Boards = new Board[boardCount];

        for (int i = 0; i < boardCount; i++)
        {
            Boards[i] = new Board(size, i);
        }

        // Create pieces and assign players
        int pieceCount = size * size * boardCount;
        Pieces = new Piece[pieceCount];

        for (int i = 0; i < pieceCount; i++)
        {
            string val = pieceType switch
            {
                "xo"      => (i % 2 == 0) ? "X" : "O",
                "numbers" => (i + 1).ToString(),
                "x"       => "X",
                _         => throw new ArgumentException($"Unsupported piece type: {pieceType}")
            };

            int ownerPosition = (i % 2 == 0) ? 1 : 2;
            Pieces[i] = new Piece(val, this, ownerPosition);
        }
        
    }
    public abstract bool CalculateComMove(Computer com, out Move move);

    public string PlayerMoveInstructions()
    {
        string MoveInstructions = $"Player {WhoseTurn.Position}, use the arrow keys to navigate the remaining spaces and press enter to select one"
        + $"\nIf the space you want to use is inaccessible with the arrow keys, use the n and m keys to cycle through available spaces";
        
        if(GameType == "notakto")
        {
            MoveInstructions += $"\nYou can use the number keys 1,2 or 3 to navigate to alternate boards if the game type requires";
        }

        return MoveInstructions;
    }
     
    public void UndoMove()
    {
        Move move = this.MoveHistory.GetPreviousMove();
        Piece pieceToUnplace = this.Pieces.FirstOrDefault(p => p.PieceID == move.PieceID);
        Square squareToUnoccupy = AllSquares.FirstOrDefault(sq => sq.SquareID == move.SquareID);
        if (pieceToUnplace is null)
        {
            throw new MoveHistoryException("Piece not found!");
        }

        TurnNumber--;
        pieceToUnplace.Unplace();
        squareToUnoccupy.IsOccupied = false;

        DrawBoards();
    }

    public void RedoMove()
    {
        Move move = this.MoveHistory.GetNextMove();
        Piece pieceToPlace = this.Pieces.FirstOrDefault(p => p.PieceID == move.PieceID);
        Square squareToOccupy = AllSquares.FirstOrDefault(sq => sq.SquareID == move.SquareID);
        if (squareToOccupy is null || pieceToPlace is null)
        {
            throw new MoveHistoryException("Piece/square not found!");
        }

        TurnNumber++;
        pieceToPlace.Place(squareToOccupy.SquareID);
        squareToOccupy.IsOccupied = true;

        DrawBoards();
    }
}