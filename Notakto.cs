class Notakto : Game
{
    public override string GameType => "notakto";

    public Notakto()
    {
    }

    public Notakto(GameStateMemento state): base(state)
    {
    }

    public override void ResolveTurn()
    {
        DrawBoards();

        List<int> boardsWithFullLines = AllFullLines
            .Select(line => line[0].BoardID)
            .Distinct()                      
            .ToList();

        if (boardsWithFullLines.Count == 3)
        {
            this.Finished = true;
            Console.WriteLine($"Player {this.WhoseNotTurn.Position} Wins!");
            return;
        }

        if (AllAvailableSquares.Length <= 0)
        {
            this.Finished = true;
            Console.WriteLine("No winner, it's a tie!");
        }
    }

    public override void ShowRuleForTurn()
    {
        Console.WriteLine($"Place an X to be the first to create a row of 3 in each board");
    }

    protected override void InitializeGameBoards()
    {
        //always make three boards of size 3
        int size = 3;
        int boardCount = 3;

        InitializeBoards(size, boardCount, "x");
    }

    public override IEnumerable<Move> GetStrategicMoves()
    {
        //Build a list of lines that have one space free/almost full        
        List<Square[]> AlmostFullLines = GetBoards()
        .SelectMany(board => board.Lines!)
        .Where(line => line.Count(sq => sq.IsOccupied) == line.Length - 1)
        .ToList();

        List<int> boardsWithFullLines = AllFullLines
            .Select(line => line[0].BoardID)
            .Distinct()
            .ToList();

        Board boardIDWithWinningLine = GetBoards().FirstOrDefault(board => !boardsWithFullLines.Contains(board.BoardID));
        if (boardIDWithWinningLine is null)
        {
            yield break;
        }

        var winningLine = AlmostFullLines.FirstOrDefault(line => line[0].BoardID == boardIDWithWinningLine.BoardID);
        if (winningLine is null)
        {
            yield break;
        }

        // We return a player-agnostic collection enumeration, so we'll include one X for each player here. Players can filter by their ownerPosition.
        var useablePieces = Pieces.DistinctBy(piece => piece.OwnerPosition);

        if (AlmostFullLines.Count > 0 && boardsWithFullLines.Count == 2 && boardIDWithWinningLine != null && winningLine != null)
        {
            Square winningSpace = winningLine.First(space => !space.IsOccupied);

            foreach(Piece useablePiece in useablePieces)
            {
                yield return new Move { PieceID = useablePiece.PieceID, SquareID = winningSpace.SquareID };
            }
        }
    }
}
