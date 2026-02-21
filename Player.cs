

abstract class Player(int Pos, Game game)
{
    public string Type = "Player";
    public int Position = Pos;
    public Piece[] Pieces => Array.FindAll(Game.Pieces, p => p.Owner.Position == this.Position);
    public Piece[] PiecesAvailable => Array.FindAll(this.Pieces, p => p.Location == null);
    public Game Game => game;

    public void Write()
    {
        Console.WriteLine($"I am a {this.Type} and my position is Player {this.Position}.");
    }

    public abstract void DoMove();
}

class Human : Player
{
    public Cursor Cursor;
    public Human(int Pos, Game Game) : base (Pos, Game)
    {
        this.Type = "Human";
        Cursor = new Cursor(0, this);
    }

    //TODO method for move options
    public override void DoMove()
    {
        int col;
        int row;
        Piece piece;
        Square sq;
        bool selected = false;
        
        this.Cursor.Location = Game.Board.SquaresAvailable[0];
        Game.Board.Draw();

        ConsoleKey[] validKeys = [ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter];

        ConsoleKeyInfo key;
        do
        {
                Console.WriteLine($"Player {Game.WhoseTurn.Position}: use the arrow keys to navigate the remaining spaces and press enter to select one");
                key = Console.ReadKey(true);

                if (!validKeys.Contains(key.Key)) continue;

                    if(key.Key == ConsoleKey.LeftArrow)  this.Cursor.MoveLocation("left");
                    if(key.Key == ConsoleKey.RightArrow) this.Cursor.MoveLocation("right");
                    if(key.Key == ConsoleKey.UpArrow)    this.Cursor.MoveLocation("up");
                    if(key.Key == ConsoleKey.DownArrow)  this.Cursor.MoveLocation("down");
                    if(key.Key == ConsoleKey.Enter)      selected = true;

                    Game.Board.Draw();
        } while (!selected);
        
        sq = Game.Board.Squares.FirstOrDefault(x => x.Row == this.Cursor.Location.Row && x.Col == this.Cursor.Location.Col);

        //Select Piece
        do
        {
            Console.WriteLine($"Player {this.Position}, enter the piece you'd like to use");
            int pieceVal = Convert.ToInt32(Console.ReadLine());
            piece = this.PiecesAvailable.FirstOrDefault(x => x.Value == pieceVal);

            if (piece == null)
                Console.WriteLine("You Cunted it, try again.");

        } while (piece == null);

        //Place Piece
        sq.Value = piece;
        piece.Location = sq;
    }
}
class Computer : Player
{

    public Computer(int Pos, Game game) : base (Pos, game)
    {
        this.Type = "Computer";
    }
    
    public override void DoMove()
    {
        //TODO method for computer AI (checking related methods of board fullness)
        Console.WriteLine("Com Turn");
    }
}