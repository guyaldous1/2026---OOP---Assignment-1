class Gomoku : Game
{
    public override string GameType => "gomoku";

    public Gomoku()
    {
    }

    public Gomoku(GameStateMemento state): base(state)
    {
    }

    public override void ResolveTurn()
    {
        DrawBoards();
        //TODO calculate win condition
        List<int> boardsWithFullLines = AllFullLines
            .Select(line => line[0].BoardID)
            .Distinct()                      
            .ToList();

        if (boardsWithFullLines.Count == 3)
        {
            this.Finished = true;
            Console.WriteLine($"Player {this.WhoseTurn.Position} Wins!");
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
        Console.WriteLine($"First line of 5 in a row wins, lines greater than 5 don't count");
    }

    protected override void InitializeGameBoards()
    {
        //always make 1 board of size 15
        int size = 15;
        int boardCount = 1;

        InitializeBoards(size, boardCount, "xo");
    }
    public override bool CalculateComMove(Computer com)
    {
        Square? sq = null;
        Piece? p = null;
        //TODO calculate computer winning move
        //Build a list of lines that have one space free/almost full        
        var AlmostFullLines = GetBoards()
        .SelectMany(board => board.Lines!)
        .Where(line => line.Count(sq => sq.IsOccupied) == line.Length - 1)
        .ToList();
        
        List<int> boardsWithFullLines = AllFullLines
            .Select(line => line[0].BoardID)
            .Distinct()                      
            .ToList();

        var boardIDWithWinningLine = GetBoards().FirstOrDefault(board => !boardsWithFullLines.Contains(board.BoardID));

        var winningLine = AlmostFullLines.FirstOrDefault(line => line[0].BoardID == boardIDWithWinningLine.BoardID);

        if (AlmostFullLines.Count > 0 && boardsWithFullLines.Count == 2 && boardIDWithWinningLine != null && winningLine != null)
        {
            
            Square winningSpace = winningLine.First(space => !space.IsOccupied);

            sq = winningSpace;
            p = com.PiecesAvailable.First();

            p.Place(sq);
            
            return true;
        }

        return false;
    }
}
