class Board
{
    public int Size;
    public Square[] Squares;
    public int BoardID;
    public Square[] SquaresAvailable => Array.FindAll(this.Squares, s => !s.IsOccupied);
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
    public List<Square[]> FullLines => Lines.Where(line => line.All(square => square.IsOccupied)).ToList();
    public Board(int setSize, int boardID)
    {
        this.Size = setSize;
        this.Squares = new Square[Size * Size];
        this.BoardID = boardID;

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                Squares[row * Size + col] = new Square(row, col, boardID);
            }
        }
    }
}

class Square(int row, int col, int boardID)
{
    public bool IsOccupied = false;
    public int Row = row;
    public int Col = col;
    public int BoardID = boardID;
}
