class TicTacToe : Game
{
    private const int MIN_BOARD_SIZE = 3;
    private const int MAX_BOARD_SIZE = 10;

    public TicTacToe()
    {
    }

    public TicTacToe(GameStateMemento state): base(state)
    {
    }

    private int targetNumber => Boards[0].Size * (Boards[0].Size * Boards[0].Size + 1) / 2;

    public override string GameType => "tictactoe";

    public override bool PlayerSelectsPiece => true;

    public override void ResolveTurn()
    {
        foreach (Square[] line in Boards[0].Lines)
        {
            bool isFull = Array.TrueForAll(line, el => el.IsOccupied);
            if (!isFull) continue;

            int lineSum = line.Select(sq => sq.SquareID).Sum(GetPieceValueForSquareAsInt);

            if (lineSum == targetNumber)
            {
                Finished = true;
                Console.WriteLine($"Player {WhoseTurn.Position} Wins!");
                return;
            }
        }

        if (Boards[0].SquaresAvailable.Length <= 0)
        {
            Finished = true;
            Console.WriteLine("No winner, it's a tie!");
        }
    }

    public override void ShowRuleForTurn()
    {
        Console.WriteLine($"The Target Number is {targetNumber}");
    }

    public override IEnumerable<Move> GetStrategicMoves()
    {
        //Build a list of lines that have one space free/almost full        
        List<Square[]> AlmostFullLines = Boards
            .SelectMany(board => board.Lines)
            .Where(line => line.Count(sq => sq.IsOccupied) == line.Length - 1)
            .ToList();

        //check all available spots for winning moves
        if (AlmostFullLines.Count > 0)
        {
            foreach (Square[] line in AlmostFullLines)
            {
                int lineSum = line.Aggregate(0, (acc, el) => el.IsOccupied ? GetPieceValueForSquareAsInt(el.SquareID) + acc : acc);
                int requires = targetNumber - lineSum;

                Piece requiredPiece = Pieces.FirstOrDefault(piece => piece.LocationSquareID == -1 && int.Parse(piece.Value) == requires);
                if (requiredPiece != null)
                {
                    Square winningSpace = line.First(square => !square.IsOccupied);
                    yield return new Move { PieceID = requiredPiece.PieceID, SquareID = winningSpace.SquareID };
                }
            }
        }
    }

    protected override void InitializeGameBoards()
    {
        int size = 0;
        int boardCount = 1;
        while (size < MIN_BOARD_SIZE || size > MAX_BOARD_SIZE)
        {
            Console.WriteLine($"-- Enter board size ({MIN_BOARD_SIZE}-{MAX_BOARD_SIZE}):");
            if (!int.TryParse(Console.ReadLine(), out size) || size < MIN_BOARD_SIZE || size > MAX_BOARD_SIZE)
            {
                Console.WriteLine($"Invalid size. Please choose a number between {MIN_BOARD_SIZE} and {MAX_BOARD_SIZE}.");
            }
        }

        InitializeBoards(size, boardCount, "numbers");
    }

    protected override void GameSpecificHelp()
    {
        Console.WriteLine($"You are playing a game of Numerical TicTacToe. The first player to create a complete column, row or diagonal matching the target number {targetNumber} wins!");
    }
}