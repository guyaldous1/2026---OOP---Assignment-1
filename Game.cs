class Game
{
    // Properties - marked as 'null!' because they are initialized in Setup()
    public Player Player1 { get; private set; } = null!;
    public Player Player2 { get; private set; } = null!;
    public Board Board { get; private set; } = null!;
    public Piece[] Pieces { get; private set; } = Array.Empty<Piece>();
    
    public int TurnNumber { get; private set; } = 1;
    public bool Finished { get; private set; } = false;

    // Expression-bodied properties for cleaner logic
    public int TargetNumber => Board.Size * (Board.Size * Board.Size + 1) / 2;
    public Player WhoseTurn => TurnNumber % 2 == 0 ? Player2 : Player1;

    public Game()
    {
        // Constructor is now "Lightweight." 
        // It only prepares the object, it doesn't run the game.
    }

    public void Start()
    {
        Console.Clear();
        InitializePlayers();
        InitializeBoard();

        // THE GAME LOOP (Iterative instead of Recursive)
        // This runs until 'Finished' is set to true in ResolveTurn
        while (!Finished)
        {
            PerformTurn();
        }

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

        this.Player1 = new Human(1, this);
        bool p2IsHuman = (mode == 1);
        this.Player2 = p2IsHuman ? new Human(2, this) : new Computer(2, this);
        
        Console.Clear();
        Console.WriteLine($"-- Game Started: Human v {(p2IsHuman ? "Human" : "Computer")} --");
    }

    private void InitializeBoard()
    {
        int size = 0;
        while (size < 2 || size > 10)
        {
            Console.WriteLine("-- Enter board size (2-10):");
            if (!int.TryParse(Console.ReadLine(), out size) || size < 2 || size > 10)
            {
                Console.WriteLine("Invalid size. Please choose a number between 2 and 10.");
            }
        }

        this.Board = new Board(size, this);
        
        // Create pieces and assign players
        int pieceCount = size * size;
        this.Pieces = new Piece[pieceCount];
        for (int i = 0; i < pieceCount; i++)
        {
            int val = i + 1;
            Player owner = (i % 2 == 0) ? Player1 : Player2;
            this.Pieces[i] = new Piece(val, owner);
        }
    }

    private void PerformTurn()
    {
        this.Board.Draw();
        Console.WriteLine($"Turn {TurnNumber}: Player {WhoseTurn.Position}'s move.");
        
        WhoseTurn.DoMove();
        ResolveTurn();

        if (!Finished)
        {
            TurnNumber++;
        }
    }

    public void ResolveTurn()
    {
        this.Board.Draw();

        foreach (Square[] line in this.Board.Lines)
        {
            bool isFull = Array.TrueForAll(line, el => el.Value != null);
            if (!isFull) continue;

            int lineSum = line.Sum(el => el.Value!.Value);

            if (lineSum == this.TargetNumber)
            {
                this.Finished = true;
                Console.WriteLine($"Player {this.WhoseTurn.Position} Wins!");
                return;
            }
        }

        if (this.Board.SquaresAvailable.Length <= 0)
        {
            this.Finished = true;
            Console.WriteLine("No winner, it's a tie!");
        }
    }
}