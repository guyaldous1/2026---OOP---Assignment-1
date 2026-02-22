class Board
{
    public int Size;
    public Square[] Squares;
    public Square[] SquaresAvailable => Array.FindAll(this.Squares, s => s.Value == null);
    public Square[] Column(int ColNum) => Array.FindAll(this.Squares, s => s.Col == ColNum);
    public Square[] Row(int RowNum) => Array.FindAll(this.Squares, s => s.Row == RowNum);
    public Square[] Diagonal(bool LtoR) => Array.FindAll(this.Squares, s => LtoR ? s.Row == s.Col : s.Row + s.Col == this.Size - 1);
    public List<Square[]> Lines
    {
        get
        {
            List<Square[]> allLines = new();
            for (int i = 0; i < Size; i++)
            {
                allLines.Add(this.Row(i));
                allLines.Add(this.Column(i));
            }
            allLines.Add(this.Diagonal(true));
            allLines.Add(this.Diagonal(false));
            return allLines;
        }
    }
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
    private static readonly string[] ValidDirections = ["left", "right", "up", "down", "next", "prev"];
    public void MoveLocation(string direction)
    {
        //FIXME added error here for use of invalid direction or moving off the board
        if (!ValidDirections.Contains(direction)) return;
        var Board = this.Owner.Game.Board;

        Square? moveTo = null;

        // 1. Get available squares in relevant row or column
        // 2. Filter by are greater or less than current position based on direction selected
        // 3. orders them by size based on direction selected
        // 4. set the first available as the new square
        if(direction == "left")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row == this.Location.Row && x.Col < this.Location.Col)
            .OrderByDescending(x => x.Col)
            .FirstOrDefault();
        else if(direction == "right")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row == this.Location.Row && x.Col > this.Location.Col)
            .OrderBy(x => x.Col)
            .FirstOrDefault();
        else if(direction == "up")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row < this.Location.Row && x.Col == this.Location.Col)
            .OrderByDescending(x => x.Row)
            .FirstOrDefault();
        else if(direction == "down")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row > this.Location.Row && x.Col == this.Location.Col)
            .OrderBy(x => x.Row)
            .FirstOrDefault();
        else if(direction == "next"){
            try{
                int cur = Array.IndexOf(Board.SquaresAvailable, this.Location);
                moveTo = Board.SquaresAvailable[cur+1];
            } catch(IndexOutOfRangeException e)
            {
                Console.WriteLine("That would take you off the board, try again.");
            }
        }
        else if(direction == "prev"){
            try{
                int cur = Array.IndexOf(Board.SquaresAvailable, this.Location);
                moveTo = Board.SquaresAvailable[cur-1];
            } catch(IndexOutOfRangeException e)
            {
                Console.WriteLine("That would take you off the board, try again.");
            }
        }

        if (moveTo != null) this.Location = moveTo;
    }
}