class Piece
{
    static private int lastPieceID = 0;

    public int PieceID = ++lastPieceID;
    public string Value { get; set; }
    public int LocationSquareID { get; private set; } = -1;
    public int OwnerPosition { get; }

    public Piece(string val, int ownerPosition)
    {
        Value = val;
        OwnerPosition = ownerPosition;
    }

    public Piece(string state)
    {
        string[] values;
        try
        {
            values = state.Split('|');
            PieceID = Convert.ToInt32(values[0]);
            Value = values[1];
            OwnerPosition = Convert.ToInt32(values[2]);
            LocationSquareID = Convert.ToInt32(values[3]);
        }
        catch
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(Piece)}");
        }
    }

    public string CaptureState()
    {
        return $"{PieceID}|{Value}|{OwnerPosition}|{LocationSquareID}";
    }

    public void Place(Square square)
    {
        LocationSquareID = square.SquareID;
        square.IsOccupied = true;
    }

    public void Unplace(Square square)
    {
        LocationSquareID = -1;
        square.IsOccupied = false;
    }
}