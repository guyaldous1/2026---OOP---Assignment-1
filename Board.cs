class Board
{
    private enum LineType { Top, Values, Filler, Border, Bottom }
    private enum CharType { Start, Width, Border, End }

    private const int SQUARE_WIDTH = 5;  // Doesn't format well for values less than 3
    private const int SQUARE_HEIGHT = 3; // Odd numbers are best
    private readonly char[,] formatChars = { { '┌', '─', '┬', '┐' }, { '│', ' ', '│', '│' }, { '│', ' ', '│', '│' }, { '├', '─', '┼', '┤' }, { '└', '─', '┴', '┘' } };

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
    public List<Square> AllSquares => Squares.ToList();
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

    public string CaptureState()
    {
        string result = $"{BoardID},{Size}";
        for (int squarePos = 0; squarePos < Size * Size; squarePos++)
            result += $",{Squares[squarePos].CaptureState()}";
        return result;
    }

    public void Draw2(string[] squareValues, int cursorRow, int cursorCol, ConsoleColor cursorColor, string cursorValue)
    {
        for (int i = 0; i < Squares.Length; i++)
        {
            if (Squares[i].Row == cursorRow && Squares[i].Col == cursorCol)
            {
                Console.ForegroundColor = cursorColor;
                ConsoleHelper.Write($"({cursorValue})");
            }
            else if (!Squares[i].IsOccupied)
            {
                Console.ResetColor();
                ConsoleHelper.Write($"( )");
            }
            else
            {
                Console.ResetColor();
                ConsoleHelper.Write($"({squareValues[i]})");
            }
            if ((i + 1) % Size == 0) ConsoleHelper.WriteLine();
        }
    }

    public void Draw(string[] squareValues, int cursorRow, int cursorCol, ConsoleColor cursorColor, string cursorValue)
    {
        DrawLine(LineType.Top);
        for (int row = 0; row < Size; row++)
        {
            int fillerRowsBefore = SQUARE_HEIGHT < 2 ? 0 : SQUARE_HEIGHT / 2;
            int fillerRowsAfter = SQUARE_HEIGHT - fillerRowsBefore - 1;
            DrawFillerRows(fillerRowsBefore);
            DrawLine(LineType.Values, row, squareValues, 
                row == cursorRow ? cursorCol : -1, 
                row == cursorRow ? cursorColor : ConsoleColor.White, 
                row == cursorRow ? cursorValue : string.Empty);
            DrawFillerRows(fillerRowsAfter);

            // If we're finishing the whole board, use the Bottom style otherwise draw a border between rows.
            LineType rowEndLineType = row == Size - 1 ? LineType.Bottom : LineType.Border;
            DrawLine(rowEndLineType, row);
        }
    }

    private void DrawFillerRows(int numRows)
    {
        for (int row = 0; row < numRows; row++)
            DrawLine(LineType.Filler);
    }

    private void DrawLine(LineType lineType, int row = -1, string[] squareValues = null, int cursorCol = -1, ConsoleColor cursorColor = ConsoleColor.White, string cursorValue = null)
    {
        ConsoleHelper.Write($"{formatChars[(int)lineType, (int)CharType.Start]}");

        for (int column = 0; column < Size; column++)
        {
            string outputLine;
            int squareOffset = row * Size + column;
            string valueToDraw = squareValues?[row * Size + column] ?? string.Empty;

            Console.ResetColor();
            if (lineType == LineType.Values) // draw the square's value between the lines, don't show zeroes
            {
                if (column == cursorCol)
                {
                    Console.ForegroundColor = cursorColor;
                    valueToDraw = cursorValue;
                }
                string formattedValue = string.IsNullOrWhiteSpace(valueToDraw) ? new string(' ', SQUARE_WIDTH / 2 + 1) : $"{valueToDraw, SQUARE_WIDTH / 2 + 1}";
                string rightPadding = formattedValue.Length < SQUARE_WIDTH ? new string(' ', SQUARE_WIDTH - formattedValue.Length) : string.Empty;
                outputLine = formattedValue + rightPadding;
            }
            else
                outputLine = new string(formatChars[(int)lineType, (int)CharType.Width], SQUARE_WIDTH);
            ConsoleHelper.Write(outputLine);
            Console.ResetColor();

            // The final column will end with the End character, others will end with the Border char
            char colChangeChar = column == Size - 1 ? formatChars[(int)lineType, (int)CharType.End] : formatChars[(int)lineType, (int)CharType.Border];
            ConsoleHelper.Write($"{colChangeChar}");
        }

        ConsoleHelper.WriteLine();
    }
}

