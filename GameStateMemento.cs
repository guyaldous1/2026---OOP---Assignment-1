// ============================================================
// GAME STATE - Snapshot for save/load (Memento pattern)
// ============================================================
class GameStateMemento
{
    public string GameType { get; set; }
    public string Boards { get; set; }
    public string Pieces { get; set; }
    public string Player1Type { get; set; }
    public string Player2Type { get; set; }
    public int TurnNumber { get; set; }
    public string MoveHistory { get; set; }
}
