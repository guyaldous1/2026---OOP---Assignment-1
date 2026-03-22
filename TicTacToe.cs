class TicTacToe : Game
{
    public override string GameType => "tictactoe";

    private int targetNumber => Boards[0].Size * (Boards[0].Size * Boards[0].Size + 1) / 2;

    public TicTacToe()
    {
    }

    public TicTacToe(GameStateMemento state): base(state)
    {
    }

    public override void ResolveTurn()
    {
        DrawBoards();

        foreach (Square[] line in this.Boards[0].Lines)
        {
            bool isFull = Array.TrueForAll(line, el => el.IsOccupied);
            if (!isFull) continue;

            int lineSum = line.Sum(GetPieceValueForSquareAsInt);

            if (lineSum == this.targetNumber)
            {
                this.Finished = true;
                Console.WriteLine($"Player {this.WhoseTurn.Position} Wins!");
                return;
            }
        }

        if (this.Boards[0].SquaresAvailable.Length <= 0)
        {
            this.Finished = true;
            Console.WriteLine("No winner, it's a tie!");
        }
    }

    public override void ShowRuleForTurn()
    {
        Console.WriteLine($"The Target Number is {targetNumber}");
    }

    protected override void InitializeBoards()//TODO refactor to have board creation factory?
    {
        int size = 0;
        int boardCount = 1;
        while (size < 2 || size > 10)
        {
            Console.WriteLine("-- Enter board size (2-10):");
            if (!int.TryParse(Console.ReadLine(), out size) || size < 2 || size > 10)
            {
                Console.WriteLine("Invalid size. Please choose a number between 2 and 10.");
            }
        }

        this.Boards = new Board[1];
        this.Boards[0] = new Board(size, 0);

        // Create pieces and assign players
        int pieceCount = size * size * boardCount;
        this.Pieces = new Piece[pieceCount];
        for (int i = 0; i < pieceCount; i++)
        {
            int val = i + 1;
            int ownerPosition = (i % 2 == 0) ? 1 : 2;
            this.Pieces[i] = new Piece(val.ToString(), this, ownerPosition);
        }
    }
    public override bool CalculateComMove(Computer com)
    {
        Square? sq = null;
        Piece? p = null;

        //Build a list of lines that have one space free/almost full        
        var AlmostFullLines = GetBoards()
        .SelectMany(board => board.Lines!)
        .Where(line => line.Count(sq => sq.IsOccupied) == line.Length - 1)
        .ToList();
        
        //check all available spots for a winning move
        if (AlmostFullLines.Count > 0)
        {
            foreach (Square[] line in AlmostFullLines)
            {
                int lineSum = line.Aggregate(0, (acc, el) => el.IsOccupied ? GetPieceValueForSquareAsInt(el) + acc : acc);
                int requires = targetNumber - lineSum;

                Piece? requiredPiece = com.PiecesAvailable.FirstOrDefault(el => int.Parse(el.Value) == requires);

                if(requiredPiece != null)
                {
                    Square emptySpace = line.First(el => !el.IsOccupied);

                    sq = emptySpace;
                    p = requiredPiece;

                    p.Place(sq);

                    return true;
                }
            }
        }

        return false;
    }
}