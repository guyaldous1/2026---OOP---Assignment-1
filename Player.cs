

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

        int col;
        int row;
        Piece piece;
        Square sq;
        bool selected = false;
        
         //Select Piece
        do
        {
            Console.WriteLine($"Player {this.Position}, enter the piece you'd like to use");
            int pieceVal = Convert.ToInt32(Console.ReadLine());
            piece = this.PiecesAvailable.FirstOrDefault(x => x.Value == pieceVal);

            if (piece == null)
                Console.WriteLine("You Cunted it, try again.");

        } while (piece == null);


        // Initialise Cursor
        this.Cursor.Location = Game.Board.SquaresAvailable[0];
        this.Cursor.Value = piece.Value;
        Game.Board.Draw();

        //FIXME Cursor move (maybe should belong to cursor)? 
        ConsoleKey[] validKeys = [ConsoleKey.N, ConsoleKey.M, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter];
        ConsoleKeyInfo key;
        do
        {
            Console.WriteLine($"Player {Game.WhoseTurn.Position}: use the arrow keys to navigate the remaining spaces and press enter to select one");
            key = Console.ReadKey(true);
            // Console.Write(key.Key);
            if (!validKeys.Contains(key.Key)) continue;

                if(key.Key == ConsoleKey.LeftArrow)  this.Cursor.MoveLocation("left");
                if(key.Key == ConsoleKey.RightArrow) this.Cursor.MoveLocation("right");
                if(key.Key == ConsoleKey.UpArrow)    this.Cursor.MoveLocation("up");
                if(key.Key == ConsoleKey.DownArrow)  this.Cursor.MoveLocation("down");
                if(key.Key == ConsoleKey.N) this.Cursor.MoveLocation("prev");
                if(key.Key == ConsoleKey.M) this.Cursor.MoveLocation("next");

                if(key.Key == ConsoleKey.Enter)      selected = true;

                Game.Board.Draw();
        } while (!selected);
        
        sq = Game.Board.Squares.FirstOrDefault(x => x.Row == this.Cursor.Location.Row && x.Col == this.Cursor.Location.Col);

        //Place Piece
        sq.Value = piece;
        piece.Location = sq;

        this.Cursor.Location = null;
    }
}
class Computer(int Pos, Game Game) : Player(Pos, Game)
{
    
    public override void DoMove()
    {
        //TODO method for computer AI (checking related methods of board fullness)
        Console.WriteLine("Com Turn");

        //TODO if a line has a single gap, (length 1 under max) then check if it's a winning move and then place there
        //TODO if not, pick any random available space and place there

        Square sq;
        List<Square[]> AlmostFullLines;
        Piece p;

        foreach (Square[] line in Game.Board.Lines)
        {
            int countOfFullLines = line.Count(el => el.Value != null);
            bool isAlmostFull = countOfFullLines+1 == Game.Board.Size;
            Console.WriteLine($"{isAlmostFull}");

            if(!isAlmostFull) continue;
        }

        //check all available winning spots for a match
        if (AlmostFullLines.Length > 0)
        {
            
            foreach (Square[] line in AlmostFullLines)
            {
                Square emptySpace = line.First(el => el.Value == null);
            
                int lineSum = line.Aggregate(0, (acc, el) => el.Value != null ? el.Value.Value + acc : acc);
                Console.WriteLine($"a line is almost full with sum {lineSum}");

                int requires = Game.TargetNumber - lineSum;

                Piece requiredPiece = this.PiecesAvailable.FirstOrDefault(el => el.Value == requires);

                if(requiredPiece != null)
                {
                   sq = emptySpace;
                   p = requiredPiece;
                   break;
                } 
            }
            
        }

        //if square not set in the above
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
        //FIXME replace with new Piece.Place(square) method
        sq.Value = p;
        p.Location = sq;

    }
}