

abstract class Player
{
    public string Type = "Player";
    public int Position;
    public void Write()
    {
        Console.WriteLine($"I am a {this.Type} and my position is Player {this.Position}.");
    }

    protected Player (int Pos)
    {
        this.Position = Pos;
    }
}

class Human : Player
{
    public Human(int Pos) : base (Pos)
    {
        this.Type = "Human";
    }
}
class Computer : Player
{

    public Computer(int Pos) : base (Pos)
    {
        this.Type = "Computer";
    }
}