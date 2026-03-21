

abstract class Player(int Pos, Game game)
{
    public int Position = Pos;
    public Piece[] Pieces => Array.FindAll(Game.Pieces, p => p.Owner.Position == this.Position);
    public Piece[] PiecesAvailable => Array.FindAll(this.Pieces, p => p.Location == null);
    public Game Game => game;
    public abstract void DoMove();
}

// TODO a lot of Boards[0] logic in here just to get things working. Needs attention to make Player rules-agnostic and get this logic from Game.
class Human : Player
{
    public Cursor Cursor;
    public Human(int Pos, Game Game) : base (Pos, Game)
    {
        Cursor = new Cursor(0, this);
    }
    public override void DoMove()
    {
        Console.ResetColor();
        //Select A Piece
        Piece? piece = null;
        while (piece == null)
        {
            Console.WriteLine($"Player {this.Position}, enter the number of the piece you'd like to use and press enter to confirm:");
            if (int.TryParse(Console.ReadLine(), out int val)) piece = PiecesAvailable.FirstOrDefault(x => x.Value == val);
            if (piece == null) Console.WriteLine($"That's not a valid piece, try again Player {this.Position}");
        }
        
        // Initialise Cursor
        this.Cursor.Location = Game.Boards[0].SquaresAvailable[0];
        this.Cursor.Value = piece.Value;
        Game.Boards[0].Draw();
        //Only accept valid inputs based on the keystrokes in this array
        ConsoleKey[] validKeys = [ConsoleKey.N, ConsoleKey.M, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter];
        ConsoleKeyInfo key;
        bool selected = false;
        do
        {
            Console.WriteLine($"Player {this.Position}, use the arrow keys to navigate the remaining spaces and press enter to select one");
            Console.WriteLine($"If the space you want to use is inaccessible with the arrow keys, use the n and m keys to cycle through available spaces");

            key = Console.ReadKey(true);
            if (validKeys.Contains(key.Key)){

                if(key.Key == ConsoleKey.LeftArrow)  this.Cursor.MoveLocation("left");
                if(key.Key == ConsoleKey.RightArrow) this.Cursor.MoveLocation("right");
                if(key.Key == ConsoleKey.UpArrow)    this.Cursor.MoveLocation("up");
                if(key.Key == ConsoleKey.DownArrow)  this.Cursor.MoveLocation("down");
                if(key.Key == ConsoleKey.N) this.Cursor.MoveLocation("prev");
                if(key.Key == ConsoleKey.M) this.Cursor.MoveLocation("next");

                if(key.Key == ConsoleKey.Enter)      selected = true;

                Game.Boards[0].Draw();
            }
            else
            {
                Console.WriteLine("Double check the controls and try navigating with either the arrow keys or n&m.");
            }
        } while (!selected);
        Square? sq = Game.Boards[0].Squares.FirstOrDefault(x => x.Row == this.Cursor.Location.Row && x.Col == this.Cursor.Location.Col);
        //Place Piece
        sq.PlacePiece(piece);
        //remove cursor from the board
        this.Cursor.Location = null;
    }
}
class Computer(int Pos, Game Game) : Player(Pos, Game)
{
    public override void DoMove() // TODO win logic needs to be owned by the game. We'll need to refactor this out of here and make Computer select from a list of Square provided by the game.
    {
        Square? sq = null;
        List<Square[]> AlmostFullLines = [];
        Piece? p = null;

        //Build a list of lines that have one space free/almost full
        foreach (Square[] line in Game.Boards[0].Lines!)
        {
            int countOfFullLines = line.Count(el => el.Value != null);
            bool isAlmostFull = countOfFullLines+1 == Game.Boards[0].Size;

            if(isAlmostFull) AlmostFullLines.Add(line);
        }
        //check all available spots for a winning move
        if (AlmostFullLines.Count > 0)
        {
            foreach (Square[] line in AlmostFullLines)
            {
                int lineSum = line.Aggregate(0, (acc, el) => el.Value != null ? el.Value.Value + acc : acc);
                int requires = ((TicTacToe)Game).targetNumber - lineSum; // TODO we can't leave this this way.

                Piece? requiredPiece = this.PiecesAvailable.FirstOrDefault(el => el.Value == requires);

                if(requiredPiece != null)
                {
                    Square emptySpace = line.First(el => el.Value == null);

                    sq = emptySpace;
                    p = requiredPiece;
                    // don't check lines if winning move is found
                    break;
                }
            }
        }
        //if no winning move is found, get available squares, and randomly pick a piece to place in a random square 
        if(sq == null)
        {
            var availSqurares = Game.Boards[0].SquaresAvailable;
            var availPieces = this.PiecesAvailable;

            Random rng = new Random();
            int sqrnum = rng.Next(0, Game.Boards[0].SquaresAvailable.Length - 1);
            int piecenum = rng.Next(0, this.PiecesAvailable.Length - 1);

            sq = availSqurares[sqrnum];
            p = availPieces[piecenum];
        }
        //set a space after resolution of the above
        sq.PlacePiece(p);
    }
}