abstract class Player
{
    protected readonly IGameContext gameContext;

    public Player(int pos, IGameContext gameContext)
    {
        Position = pos;
        this.gameContext = gameContext;
    }

    public int Position { get; }

    public Piece[] Pieces => Array.FindAll(gameContext.GetPieces() ?? [], p => p.OwnerPosition == Position);

    public Piece[] PiecesAvailable => Array.FindAll(Pieces, p => p.LocationSquareID == -1);

    public ConsoleColor Colour => Position % 2 == 0 ? ConsoleColor.Red : ConsoleColor.Green;

    public string CaptureState() => this is Human ? "human" : "computer";

    public abstract Move DoMove();
}