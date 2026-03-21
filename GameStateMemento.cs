using System;
using System.Collections.Generic;
using System.Text;



// ============================================================
// GAME STATE - Snapshot for save/load (Memento pattern)
// ============================================================
class GameStateMemento
{
    public string GameType { get; set; } = "";
    public int BoardSize { get; set; }
    public int BoardCount { get; set; }
    // Serialized as flat arrays: boardIndex * rows * cols + row * cols + col
    public List<string> BoardValues { get; set; } = new(); // TODO this will be replaced by serialising pieces
    public int CurrentPlayerIndex { get; set; }
    public required string Player1Type { get; set; }
    public required string Player2Type { get; set; }

//TODO    public List<Piece> MoveHistory { get; set; } = new();
//TODO    public int HistoryPointer { get; set; }
}
