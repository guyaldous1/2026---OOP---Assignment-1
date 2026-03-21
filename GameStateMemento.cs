using System;
using System.Collections.Generic;
using System.Text;



// ============================================================
// GAME STATE - Snapshot for save/load (Memento pattern)
// ============================================================
class GameStateMemento
{
    public string GameType { get; set; } = "";
    public string Mode { get; set; } = "";       // "HvH" or "HvC"
    public int BoardSize { get; set; }
    public int BoardCount { get; set; }
    // Serialized as flat arrays: boardIndex * rows * cols + row * cols + col
    public List<string> BoardValues { get; set; } = new();
    public int CurrentPlayerIndex { get; set; }
    public List<string> Player1Numbers { get; set; } = new();
    public List<string> Player2Numbers { get; set; } = new();
    public List<Piece> MoveHistory { get; set; } = new();
    public int HistoryPointer { get; set; }
}
