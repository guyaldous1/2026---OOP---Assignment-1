class Board
{
    public int Size;
    public Square[] Squares;
    public Square[] SquaresAvailable => Array.FindAll(this.Squares, s => s.Value == null);
    private Game _game;

    public Board(int setSize, Game Game)
    {
        this.Size = setSize;
        this.Squares = new Square[Size * Size];
        this._game = Game;

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                Squares[row * Size + col] = new Square(row, col);
            }
        }

        Console.WriteLine($"Not bed gud soize of {this.Size} {this.Squares.Length}");
    }

    public void Draw()
    {
        Console.Clear();
        var Game = this._game;

        Console.Write($"Player 1's Remaining Pieces:");
        foreach (Piece p in Game.Player1.PiecesAvailable)
        {
            Console.Write($" {p.Value}");
        }
        Console.Write('\n');
        Console.Write($"Player 2's Remaining Pieces:");
        foreach (Piece p in Game.Player2.PiecesAvailable)
        {
            Console.Write($" {p.Value}");
        }
        Console.Write('\n');
        
        for (int i = 0; i < this.Squares.Length; i++)
        {
            if(Game.WhoseTurn is Human human && human.Cursor.Location == this.Squares[i])
                Console.Write($"({human.Cursor.Value})");
            else if(this.Squares[i].Value == null)
                Console.Write($"( )");
            else
                Console.Write($"({this.Squares[i].Value.Value})");

            if((i + 1) % this.Size == 0) Console.Write("\n");
        }
    }

    //TODO method for displaying column, row and diagonal fullness
    //TODO method for displaying column, row and diagonal count total
    //TODO method for get blank spaces (filter array of squares)
}

class Square(int row, int col)
{
    public Piece? Value {set; get;}
    public int Row = row;
    public int Col = col;
}


class Piece(int val, Player Player)
{
    public int Value = val;
    public Player Owner = Player;
    public Square? Location {set; get;}
}

class Cursor(int val, Player Player) : Piece(val, Player)
{
    private static readonly string[] ValidDirections = ["left", "right", "up", "down"];
    public void MoveLocation(string direction)
    {
        if (!ValidDirections.Contains(direction)) return;

        Square? next = null;

        if(direction == "left")
            next = this.Owner.Game.Board.SquaresAvailable
            .Where(x => x.Row == this.Location.Row && x.Col < this.Location.Col)
            .OrderByDescending(x => x.Col)
            .FirstOrDefault();
        else if(direction == "right")
            next = this.Owner.Game.Board.SquaresAvailable
            .Where(x => x.Row == this.Location.Row && x.Col > this.Location.Col)
            .OrderBy(x => x.Col)
            .FirstOrDefault();
        else if(direction == "up")
            next = this.Owner.Game.Board.SquaresAvailable
            .Where(x => x.Row < this.Location.Row && x.Col == this.Location.Col)
            .OrderByDescending(x => x.Row)
            .FirstOrDefault();
        else if(direction == "down")
            next = this.Owner.Game.Board.SquaresAvailable
            .Where(x => x.Row > this.Location.Row && x.Col == this.Location.Col)
            .OrderBy(x => x.Row)
            .FirstOrDefault();

        if (next != null) this.Location = next;
    }
}