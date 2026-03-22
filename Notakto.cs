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

        //TODO end of turn logic for notakto

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
        Console.WriteLine($"Place an X to be the first to create a row of 3 in each board");
    }

    protected override void InitializeBoards()
    {
        //always make three boards of size 3
        int size = 3;
        int boards = 3;
        
        this.Boards = new Board[]{ 
            new Board(size, this, 0), 
            new Board(size, this, 1), 
            new Board(size, this, 2) 
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
}
