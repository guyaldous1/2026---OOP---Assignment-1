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
        Console.WriteLine($"Turn {Game.TurnNumber}. It's Player {Game.WhoseTurn.Position}'s Turn");
        Game.ShowRuleForTurn();
    
        //write player 1 pieces
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"Player 1's Remaining Pieces:");
        foreach (Piece p in Game.Player1.PiecesAvailable)
        {
            Console.Write($" {p.Value}");
        }
        Console.Write('\n');
        
        //write board layout
        for (int i = 0; i < this.Squares.Length; i++)
        {
            if(Game.WhoseTurn is Human human && human.Cursor.Location == this.Squares[i])
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"({human.Cursor.Value})");
            }
            else if(this.Squares[i].Value == null)
            {
                Console.ResetColor();
                Console.Write($"( )");
            }
            else
            {   
                Console.ResetColor();
                Console.Write($"({this.Squares[i].Value.Value})");
            }
            if((i + 1) % this.Size == 0) Console.Write("\n");
        }

        //write player 2 pieces
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Player 2's Remaining Pieces:");
        foreach (Piece p in Game.Player2.PiecesAvailable)
        {
            Console.Write($" {p.Value}");
        }
        Console.ResetColor();
        Console.Write('\n');
    }
}
class Square(int row, int col)
{
    public Piece? Value {private set; get;}
    public int Row = row;
    public int Col = col;
    public void PlacePiece(Piece piece)
    {
        this.Value = piece;
        piece.Location = this;
    } 
}
