// This script DOES NOT get attached to any GameObject in Unity.
// It just floats in the background and remembers things for us.

public enum Difficulty { Easy, Medium, Hard }

public static class GameSettings
{
    // Remembers the difficulty (Defaults to Medium)
    public static Difficulty CurrentDifficulty = Difficulty.Medium;

    // Remembers which mini-game to load
    public static string TargetScene = "";
}