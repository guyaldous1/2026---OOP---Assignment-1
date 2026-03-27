class Square
{
    static private int lastSquareID = 0;

    public int SquareID { get; }  = ++lastSquareID;
    public bool IsOccupied { get; set; } = false;
    public int Row { get; }
    public int Col { get; }
    public int BoardID { get; }

    public Square() { }

    public Square(int row, int col, int boardID)
    {
        Row = row;
        Col = col;
        BoardID = boardID;
    }

    public Square(string state)
    {
        object[] values = [];
        try
        {
            values = state.Split('|');
        }
        catch
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(Square)}");
        }

        if (values.Length != 5)
        {
            throw new DeserialisationException($"Invalid length marker in {nameof(Square)}");
        }

        try
        {
            SquareID = Convert.ToInt32(values[0]);
            IsOccupied = Convert.ToBoolean(values[1]);
            Row = Convert.ToInt32(values[2]);
            Col = Convert.ToInt32(values[3]);
            BoardID = Convert.ToInt32(values[4]);
        }
        catch
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(Square)}");
        }
    }

    public string CaptureState()
    {
        return $"{SquareID}|{IsOccupied}|{Row}|{Col}|{BoardID}";
    }
}