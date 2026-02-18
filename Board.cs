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

        Console.WriteLine($"Not bed gud soize of {this.Size} {this.Squares.Length}");
    }

    public void Show()
    {

        for (int i = 0; i < this.Squares.Length; i++)
        {
            var sq = this.Squares[i];

            Console.Write($"({sq.Row} | {sq.Col})");
            if((i + 1) % this.Size == 0) Console.Write("\n");
        }
    }
}

class Square
{
    public int? Value {set; get;}

    public int Row;
    public int Col;
    

    public Square (int row, int col)
    {
        this.Row = row;
        this.Col = col;

        this.Value = row * col;
    }
}