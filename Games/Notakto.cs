class Notakto : Game
{
    public Notakto()
    {
    }

    public Notakto(GameStateMemento state): base(state)
    {
    }

    public override string GameType => "notakto";

    public override void ResolveTurn()
    {
        List<int> boardsWithFullLines = AllFullLines
            .Select(line => line[0].BoardID)
            .Distinct()                      
            .ToList();

        if (boardsWithFullLines.Count == 3)
        {
            Finished = true;
            Console.WriteLine($"Player {WhoseNotTurn.Position} Wins!");
            return;
        }

        if (AllAvailableSquares.Length <= 0)
        {
            Finished = true;
            Console.WriteLine("No winner, it's a tie!");
        }
    }

    public override void ShowRuleForTurn()
    {
        Console.WriteLine($"The player who places the final X to complete a line on all three boards loses.");
    }

    public override IEnumerable<Move> GetStrategicMoves()
    {
        List<int> boardsWithFullLines = AllFullLines
            .Select(line => line[0].BoardID)
            .Distinct()
            .ToList();

        // Squares that would complete a line on a still-live board — playing here risks losing
        int[] dangerousSquareIDs = Boards
            .Where(board => !boardsWithFullLines.Contains(board.BoardID))
            .SelectMany(board => board.Lines!)
            .Where(line => line.Count(sq => sq.IsOccupied) == line.Length - 1)
            .Select(line => line.First(sq => !sq.IsOccupied).SquareID)
            .ToArray();

        // Safe squares are available squares that don't complete any line
        Square[] safeSquares = AllAvailableSquares
            .Where(sq => !dangerousSquareIDs.Contains(sq.SquareID))
            .ToArray();

        if (safeSquares.Length == 0) yield break;

        Piece piece = WhoseTurn.PiecesAvailable.First();
        foreach (Square safeSquare in safeSquares)
        {
            yield return new Move { PieceID = piece.PieceID, SquareID = safeSquare.SquareID };
        }
    }

    protected override void InitializeGameBoards()
    {
        //always make three boards of size 3
        int size = 3;
        int boardCount = 3;

        InitializeBoards(size, boardCount, "x");
    }

    protected override void GameSpecificHelp()
    {
        Console.WriteLine("You are playing a game of Notakto. The first player to create a complete column, row or diagonal on each board loses.");
    }
}
