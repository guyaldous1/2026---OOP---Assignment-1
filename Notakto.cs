class Notakto : Game
{
    public override string GameType => "notakto";

    public int targetNumber => Boards[0].Size * (Boards[0].Size * Boards[0].Size + 1) / 2; // TODO this needs to be private after Computer loses its dependency on it.


    public Notakto()
    {
    }

    public Notakto(GameStateMemento state): base(state)
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
        Console.WriteLine($"Place an X to be the first to create a row of 3 in each board");
    }

    protected override void InitializeBoards()
    {
        //always make three boards of size 3
        int size = 3;
        int boards = 3;
        
        this.Boards = new Board[]{ 
            new Board(size, this), 
            new Board(size, this), 
            new Board(size, this) 
        };

        // Create pieces and assign players
        int pieceCount = size * size * boards;
        this.Pieces = new Piece[pieceCount];
        for (int i = 0; i < pieceCount; i++)
        {
            string val = "X";
            int ownerPosition = (i % 2 == 0) ? 1 : 2;
            this.Pieces[i] = new Piece(val, this, ownerPosition);
        }
    }

    public override void DrawBoards()
    {
        this.Boards[0].Draw();
    }
}
