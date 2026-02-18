class Board
{
    public int Size;
    public Square[] Squares;

    public Board(int setSize)
    {
        this.Size = setSize;
        this.Squares = new Square[Size * Size];

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                Squares[row * Size + col] = new Square(row, col);
            }
        }

        Console.WriteLine($"Not bed gud soize of {this.Size}");
    }

    public void Show()
    {
        foreach(Square sq in this.Squares)
        {
            Console.WriteLine(sq.Value);
        }
    }
}

class Square
{
    public int? Value {set; get;}

    public int Location = 0;

    public int Row;
    public int Col;
    

    public Square (int row, int col)
    {
        this.Row = row;
        this.Col = col;

        this.Value = row * col;
    }
}