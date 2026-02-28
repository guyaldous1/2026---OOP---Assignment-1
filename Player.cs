

abstract class Player(int Pos, Game game)
{
    public int Position = Pos;
    public Piece[] Pieces => Array.FindAll(Game.Pieces, p => p.Owner.Position == this.Position);
    public Piece[] PiecesAvailable => Array.FindAll(this.Pieces, p => p.Location == null);
    public Game Game => game;
    public abstract void DoMove();
}

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
        Piece piece = null;
        Square sq;
        bool selected = false;
        
         //Select A Piece
        while (true) 
        {
            Console.WriteLine($"Player {this.Position}, enter the number of the piece you'd like to use and press enter to confirm:");
            if (int.TryParse(Console.ReadLine(), out int pieceEntered))
            {
                piece = this.PiecesAvailable.FirstOrDefault(x => x.Value == pieceEntered);
                if (piece != null) break;
            }
            Console.WriteLine($"That's not a valid piece, try again Player {this.Position}");
        }
        // Initialise Cursor
        this.Cursor.Location = Game.Board.SquaresAvailable[0];
        this.Cursor.Value = piece.Value;
        Game.Board.Draw();
        //Only accept valid inputs based on the keystrokes in this array
        ConsoleKey[] validKeys = [ConsoleKey.N, ConsoleKey.M, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter];
        ConsoleKeyInfo key;
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

                Game.Board.Draw();
            }
            else
            {
                Console.WriteLine("Double check the controls and try navigating with either the arrow keys or n&m.");
            }
        } while (!selected);
        sq = Game.Board.Squares.FirstOrDefault(x => x.Row == this.Cursor.Location.Row && x.Col == this.Cursor.Location.Col);
        //Place Piece
        sq.PlacePiece(piece);
        //remove cursor from the board
        this.Cursor.Location = null;
    }
}
class Computer(int Pos, Game Game) : Player(Pos, Game)
{
    public override void DoMove()
    {
        Square? sq = null;
        List<Square[]> AlmostFullLines = [];
        Piece? p = null;

        //Build a list of lines that have one space free/almost full
        foreach (Square[] line in Game.Board.Lines!)
        {
            int countOfFullLines = line.Count(el => el.Value != null);
            bool isAlmostFull = countOfFullLines+1 == Game.Board.Size;

            if(isAlmostFull) AlmostFullLines.Add(line);
        }
        //check all available spots for a winning move
        if (AlmostFullLines.Count > 0)
        {
            foreach (Square[] line in AlmostFullLines)
            {
                int lineSum = line.Aggregate(0, (acc, el) => el.Value != null ? el.Value.Value + acc : acc);
                int requires = Game.TargetNumber - lineSum;

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
            var availSqurares = Game.Board.SquaresAvailable;
            var availPieces = this.PiecesAvailable;

            Random rng = new Random();
            int sqrnum = rng.Next(0, Game.Board.SquaresAvailable.Length - 1);
            int piecenum = rng.Next(0, this.PiecesAvailable.Length - 1);

            sq = availSqurares[sqrnum];
            p = availPieces[piecenum];
        }
        //set a space after resolution of the above
        sq.PlacePiece(p);
    }
}