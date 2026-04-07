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

    public List<Square[]> FullLines => GetLines().Where(line => line.All(square => square.IsOccupied)).ToList();

    public List<Square[]> GetLines(bool includeAllDiagonals = false)
    {
        List<Square[]> allLines = new();
        for (int i = 0; i < Size; i++)
        {
            allLines.Add(Row(i));
            allLines.Add(Column(i));
        }
        if (includeAllDiagonals)
        {
            allLines.AddRange(AllDiagonals());
        }
        else
        {
            allLines.Add(Diagonal(true));
            allLines.Add(Diagonal(false));
        }

        return allLines;
    }

    public string CaptureState()
    {
        string result = $"{BoardID},{Size}";
        for (int squarePos = 0; squarePos < Size * Size; squarePos++)
            result += $",{Squares[squarePos].CaptureState()}";
        return result;
    }

    public void Draw(string[] squareValues, int cursorRow, int cursorCol, ConsoleColor cursorColor, string cursorValue)
    {
        bool needsPadding = squareValues.Any(v => v.Length > 1);

        for (int i = 0; i < Squares.Length; i++)
        {
            if (Squares[i].Row == cursorRow && Squares[i].Col == cursorCol)
            {
                Console.ForegroundColor = cursorColor;
                string padding = needsPadding && cursorValue.Length == 1 ? " " : string.Empty;
                Console.Write($"({cursorValue})" + padding);
            }
            else if (!Squares[i].IsOccupied)
            {
                Console.ResetColor();
                Console.Write($"( )" + (needsPadding ? " " : string.Empty));
            }
            else
            {
                Console.ResetColor();
                string padding = needsPadding && squareValues[i].Length == 1 ? " " : string.Empty;
                Console.Write($"({squareValues[i]}){padding}");
            }
            if ((i + 1) % Size == 0) Console.Write("\n");
        }
    }

    private Square[] Column(int ColNum) => Array.FindAll(Squares, s => s.Col == ColNum);

    private Square[] Row(int RowNum) => Array.FindAll(Squares, s => s.Row == RowNum);

    private Square[] Diagonal(bool LtoR) => Array.FindAll(Squares, s => LtoR ? s.Row == s.Col : s.Row + s.Col == Size - 1);

    /// <summary>
    /// In some games, we want to test lines from all diagonals, not just the two top-to-bottom diagonals
    /// </summary>
    private List<Square[]> AllDiagonals()
    {
        if (Size < 3)
        {
            // No partials in this case
            return [Diagonal(true), Diagonal(false)];
        }

        List<Square[]> diagonals = [];

        // Iterate through the rows. For each row we have 4 diagonals -- 2 ascending, 2 descending. For example, for row 1 in a 5x5 grid we would get these 4:
        // 2 ascending (one starting at row 1, one ending at row 1), 2 descending (one starting at row 1, one ending at row 1):
        // ( ) (x) ( ) ( ) ( )    ( ) ( ) ( ) ( ) ( )                ( ) ( ) ( ) ( ) ( )    ( ) ( ) ( ) (x) ( )
        // (x) ( ) ( ) ( ) ( )    ( ) ( ) ( ) ( ) (x)                (x) ( ) ( ) ( ) ( )    ( ) ( ) ( ) ( ) (x)
        // ( ) ( ) ( ) ( ) ( )    ( ) ( ) ( ) (x) ( )                ( ) (x) ( ) ( ) ( )    ( ) ( ) ( ) ( ) ( )
        // ( ) ( ) ( ) ( ) ( )    ( ) ( ) (x) ( ) ( )                ( ) ( ) (x) ( ) ( )    ( ) ( ) ( ) ( ) ( )
        // ( ) ( ) ( ) ( ) ( )    ( ) (x) ( ) ( ) ( )                ( ) ( ) ( ) (x) ( )    ( ) ( ) ( ) ( ) ( )
        for (int rowStart = 0; rowStart < Size - 1; rowStart++)
        {
            List<Square> ascEndingAtRow = [];
            List<Square> descStartingAtRow = [];
            for (int row = rowStart; row < Size; row++)
            {
                ascEndingAtRow.Add(GetSquare(row, Size - (row - rowStart) - 1)); // leaving parenthese to show algorithm: row-rowStart is our counter
                descStartingAtRow.Add(GetSquare(row, row - rowStart));
            }
            diagonals.Add(ascEndingAtRow.ToArray());
            diagonals.Add(descStartingAtRow.ToArray());

            List<Square> ascStartingAtRow = [];
            List<Square> descEndingAtRow = [];
            for (int row = rowStart; row >= 0; row--)
            {
                ascStartingAtRow.Add(GetSquare(row, rowStart - row));
                descEndingAtRow.Add(GetSquare(row, Size - (rowStart - row) - 1)); // leaving parenthese to show algorithm: rowStart-row is our counter
            }
            diagonals.Add(ascStartingAtRow.ToArray());
            diagonals.Add(descEndingAtRow.ToArray());
        }

        return diagonals;
    }

    private Square GetSquare(int row, int col) => Squares.FirstOrDefault(s => s.Row == row && s.Col == col);
}

