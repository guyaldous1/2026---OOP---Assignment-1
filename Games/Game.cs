abstract class Game : IGameContext
{
    private PlayerFactory PlayerFactory;

    private MoveHistory MoveHistory;

    public Game()
    {
        PlayerFactory = new PlayerFactory(this);
        MoveHistory = new MoveHistory();
    }

    public Game(GameStateMemento state)
    {
        PlayerFactory = new PlayerFactory(this);

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

        Player1 = PlayerFactory.CreateFromState(state.Player1Type, 1);
        Player2 = PlayerFactory.CreateFromState(state.Player2Type, 2);
        TurnNumber = state.TurnNumber;

        Boards = new Board[boardCount];
        for (int i = 0; i < boardCount; i++)
            Boards[i] = new Board(boardValues[i + 1]); // offset by 1 because of length value at start of string

        Pieces = new Piece[pieceCount];
        for (int i = 0; i < pieceCount; i++)
            Pieces[i] = new Piece(pieceValues[i + 1]); // offset by 1 because of length value at start of string

        MoveHistory = new MoveHistory(state.MoveHistory);
    }

    // Properties - marked as 'null!' because they are initialized in Setup()
    public Player Player1 { get; private set; } = null;

    public Player Player2 { get; private set; } = null;

    public Board[] Boards { get; set; } = null;

    public Piece[] Pieces { get; set; } = Array.Empty<Piece>();

    public int TurnNumber { get; private set; } = 1;

    public bool Finished { get; protected set; } = false;

    public virtual string GameType { get; }

    public virtual bool PlayerSelectsPiece => false;

    // Expression-bodied properties for cleaner logic
    public Player WhoseTurn => TurnNumber % 2 == 0 ? Player2 : Player1;

    public Player WhoseNotTurn => TurnNumber % 2 == 0 ? Player1 : Player2;

    public Square[] AllAvailableSquares => Boards.SelectMany(board => board.SquaresAvailable).ToArray();

    public List<Square[]> AllFullLines => Boards.SelectMany(board => board.FullLines).ToList();

    public List<Square> AllSquares => Boards.SelectMany(board => board.Squares).ToList();

    public GameStateMemento CaptureState()
    {
        string boardState = $"{Boards.Length}";
        for (int boardCount = 0; boardCount < Boards.Length; boardCount++)
            boardState += $"_{Boards[boardCount].CaptureState()}";

        string pieceState = $"{Pieces.Length}";
        for (int pieceCount = 0; pieceCount < Pieces.Length; pieceCount++)
            pieceState += $",{Pieces[pieceCount].CaptureState()}";

        var state = new GameStateMemento
        {
            GameType = GameType,
            Boards = boardState,
            Pieces = pieceState,
            TurnNumber = TurnNumber,
            Player1Type = Player1.CaptureState(),
            Player2Type = Player2.CaptureState(),
            MoveHistory = MoveHistory.CaptureState()
        };

        return state;
    }

    public void StartGame()
    {
        Console.Clear();
        InitializePlayers();
        InitializeGameBoards();
        DrawBoards();
    }

    public void EndGame()
    {
        Console.WriteLine("--- Game Over ---");
        Console.ReadLine();
    }

    public void ShowHelp()
    {
        Console.Clear();

        Console.WriteLine("---Game Instructions---");
        GameSpecificHelp();

        Console.WriteLine("\n---Help Menu---");
        Console.WriteLine("\nYou can use any of the following commands to interact with this menu");

        Console.WriteLine("turn - return to the game");
        Console.WriteLine("load - load a different saved game file");
        Console.WriteLine("save - save the current game for later");
        Console.WriteLine("undo - undo the turn you just made");
        Console.WriteLine("redo - redo the any number of turns you've just undone");
        Console.WriteLine("quit - abandon the game and quit the program without saving");
    }

    public void PerformTurn()
    {
        Console.Clear();
        Console.WriteLine($"Turn {TurnNumber}: Player {WhoseTurn.Position}'s move.");

        DrawBoards();
        Move move = WhoseTurn.DoMove();
        DrawBoards();
        MoveHistory.StoreNewMove(move);
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
        return Pieces.FirstOrDefault(p => p.LocationSquareID == squareID)?.Value ?? " ";
    }

    public int GetPieceValueForSquareAsInt(int squareID)
    {
        Piece piece = Pieces.FirstOrDefault(p => p.LocationSquareID == squareID);
        return piece != null ? int.Parse(piece.Value) : 0;
    }

    public void DrawBoards()
    {
        Console.Clear();
        Console.WriteLine($"Turn {TurnNumber}. It's Player {WhoseTurn.Position}'s Turn");
        ShowRuleForTurn();

        DrawPlayerPieces(Player1);

        Console.WriteLine();

        //write each board layout
        foreach (Board board in Boards)
        {
            string[] boardValues = board.Squares.Select((_, i) => GetPieceValueForSquare(board.Squares[i].SquareID)).ToArray();
            Human human = WhoseTurn as Human;
            bool cursorIsOnThisBoard = (human?.Cursor.BoardID ?? -1) == board.BoardID;
            int cursorRow = cursorIsOnThisBoard ? human.Cursor.Row : -1;
            int cursorCol = cursorIsOnThisBoard ? human.Cursor.Col : -1;
            board.Draw(boardValues, cursorRow, cursorCol, WhoseTurn.Colour, human?.Cursor.Value ?? string.Empty);
            Console.WriteLine();
        }

        DrawPlayerPieces(Player2);
    }

    public string PlayerMoveInstructions()
    {
        string MoveInstructions = $"Player {WhoseTurn.Position}, use the arrow keys to navigate the remaining spaces and press enter to select one"
        + $"\nIf the space you want to use is inaccessible with the arrow keys, use the n and m keys to cycle through available spaces";

        if (GameType == "notakto")
        {
            MoveInstructions += $"\nYou can use the number keys 1,2 or 3 to navigate to alternate boards if the game type requires";
        }

        return MoveInstructions;
    }

    public void UndoMove()
    {
        Move move = MoveHistory.GetPreviousMove();
        Piece pieceToUnplace = Pieces.FirstOrDefault(p => p.PieceID == move.PieceID);
        Square squareToUnoccupy = AllSquares.FirstOrDefault(sq => sq.SquareID == move.SquareID);
        if (pieceToUnplace is null)
        {
            throw new MoveHistoryException("Piece not found!");
        }

        TurnNumber--;
        pieceToUnplace.Unplace(squareToUnoccupy);

        DrawBoards();
    }

    public void RedoMove()
    {
        Move move = MoveHistory.GetNextMove();
        Piece pieceToPlace = Pieces.FirstOrDefault(p => p.PieceID == move.PieceID);
        Square squareToOccupy = AllSquares.FirstOrDefault(sq => sq.SquareID == move.SquareID);
        if (squareToOccupy is null || pieceToPlace is null)
        {
            throw new MoveHistoryException("Piece/square not found!");
        }

        TurnNumber++;
        pieceToPlace.Place(squareToOccupy);

        DrawBoards();
    }

    public abstract void ResolveTurn();

    public abstract void ShowRuleForTurn();

    /// <summary>
    /// For games where a move can win, this will return winning moves. For a game where a move can lose, this will return non-losing moves.
    /// </summary>
    public abstract IEnumerable<Move> GetStrategicMoves();

    protected abstract void GameSpecificHelp();

    protected abstract void InitializeGameBoards();

    protected void InitializeBoards(int size, int boardCount, string pieceType)
    {
        // Create boards
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
                "xo" => (i % 2 == 0) ? "X" : "O",
                "numbers" => (i + 1).ToString(),
                "x" => "X",
                _ => throw new ArgumentException($"Unsupported piece type: {pieceType}")
            };

            int ownerPosition = (i % 2 == 0) ? 1 : 2;
            Pieces[i] = new Piece(val, ownerPosition);
        }
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

        Player1 = PlayerFactory.CreateHumanPlayer(1);
        bool p2IsHuman = mode == 1;
        Player2 = p2IsHuman ? PlayerFactory.CreateHumanPlayer(2) : PlayerFactory.CreateComputerPlayer(2);

        Console.Clear();
        Console.WriteLine($"-- Game Started: Human v {(p2IsHuman ? "Human" : "Computer")} --");
    }

    private void DrawPlayerPieces(Player player)
    {
        Console.ResetColor();
        Console.ForegroundColor = player.Colour;
        Piece[] playerPiecesAvailable = player.PiecesAvailable;
        if (playerPiecesAvailable.DistinctBy(piece => piece.Value).Count() == 1)
        {
            // All pieces are the same for player, we'll just show one of them
            Console.Write($"Player {player.Position}'s piece is: {playerPiecesAvailable.First().Value}");
        }
        else
        {
            Console.Write($"Player {player.Position}'s Remaining Pieces:");
            foreach (Piece p in player.PiecesAvailable)
            {
                Console.Write($" {p.Value}");
            }
        }

        Console.ResetColor();
        Console.WriteLine();
    }
}