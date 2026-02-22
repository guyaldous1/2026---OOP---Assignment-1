

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
    }
}
class Computer(int Pos, Game game) : Player(Pos, game)
{
    
    public override void DoMove()
    {
        //TODO method for computer AI (checking related methods of board fullness)
        Console.WriteLine("Com Turn");
    }
}