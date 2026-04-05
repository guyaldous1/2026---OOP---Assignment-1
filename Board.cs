class Board
{
    public Board(int setSize, int boardID)
    {
        Size = setSize;
        Squares = new Square[Size * Size];
        BoardID = boardID;

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                Squares[row * Size + col] = new Square(row, col, boardID);
            }
        }
    }

    public Board(string state)
    {
        string[] values;
        try
        {
            values = state.Split(',');
            BoardID = Convert.ToInt32(values[0]);
            Size = Convert.ToInt32(values[1]);
            Squares = new Square[Size * Size];
        }
        catch
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(Board)}");
        }

        if (values.Length != Size * Size + 2)
        {
            throw new DeserialisationException($"Invalid length of serialised state in {nameof(Square)}");
        }

        for (int squarePos = 0; squarePos < Size * Size; squarePos++)
            Squares[squarePos] = new Square(values[squarePos + 2]); // First two positions in state are BoardID and Size, our square state starts after this.
    }

    public int Size { get; }

    public int BoardID { get; }

    public Square[] Squares { get; }

    public Square[] SquaresAvailable => Array.FindAll(Squares, s => !s.IsOccupied);

    public List<Square[]> Lines
    {
        get
        {
            List<Square[]> allLines = new();
            for (int i = 0; i < Size; i++)
            {
                allLines.Add(Row(i));
                allLines.Add(Column(i));
            }
            allLines.Add(Diagonal(true));
            allLines.Add(Diagonal(false));
            return allLines;
        }
    }

    public List<Square[]> FullLines => Lines.Where(line => line.All(square => square.IsOccupied)).ToList();

    public string CaptureState()
    {
        string result = $"{BoardID},{Size}";
        for (int squarePos = 0; squarePos < Size * Size; squarePos++)
            result += $",{Squares[squarePos].CaptureState()}";
        return result;
    }

    public void Draw(string[] squareValues, int cursorRow, int cursorCol, ConsoleColor cursorColor, string cursorValue)
    {
        for (int i = 0; i < Squares.Length; i++)
        {
            if (Squares[i].Row == cursorRow && Squares[i].Col == cursorCol)
            {
                Console.ForegroundColor = cursorColor;
                Console.Write($"({cursorValue})");
            }
            else if (!Squares[i].IsOccupied)
            {
                Console.ResetColor();
                Console.Write($"( )");
            }
            else
            {
                Console.ResetColor();
                Console.Write($"({squareValues[i]})");
            }
            if ((i + 1) % Size == 0) Console.Write("\n");
        }
    }

    private Square[] Column(int ColNum) => Array.FindAll(Squares, s => s.Col == ColNum);

    private Square[] Row(int RowNum) => Array.FindAll(Squares, s => s.Row == RowNum);

    private Square[] Diagonal(bool LtoR) => Array.FindAll(Squares, s => LtoR ? s.Row == s.Col : s.Row + s.Col == Size - 1);
}

