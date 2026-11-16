// CYOATests/TestSupport.cs
// Helpers shared by the game-level, character-creation and rules tests. Use with `using static TestSupport;`.

/// <summary>A Die that returns preset rolls in order (1 once the script runs out).</summary>
public class ScriptedDie : Die
{
    private readonly Queue<int> _rolls = new();

    public void Script(params int[] rolls)
    {
        foreach (int roll in rolls)
            _rolls.Enqueue(roll);
    }

    public override int Roll(int sides)
    {
        return _rolls.Count > 0 ? _rolls.Dequeue() : 1;
    }
}

public static class TestSupport
{
    /// <summary>Language 1 (English), main menu 1, then a Human Fighter named Hero, three rolls, and "proceed".</summary>
    public static readonly string[] CreateEnglishFighter =
    {
        "1", "1", "human", "Hero", "fighter", "roll", "roll", "roll", "2"
    };

    /// <summary>Messages loaded for one Language.</summary>
    public static Messages LoadMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    /// <summary>
    /// Plays the real game with the given input lines and returns everything it printed.
    /// Every roll is 1, so the run is deterministic; use the overload to script the rolls.
    /// Scripts must end the game (for example with "e"), because the path prompt loops forever when input runs out.
    /// </summary>
    public static string RunGame(params string[] lines) => RunGame(new ScriptedDie(), lines);

    /// <summary>Plays the real game with the given input lines and Die, and returns everything it printed.</summary>
    public static string RunGame(ScriptedDie die, params string[] lines)
    {
        StringWriter output = new StringWriter();
        new Game(ScriptedInput(lines), output, die).StartGame();
        return output.ToString();
    }

    /// <summary>A reader that answers each prompt with the next line.</summary>
    public static TextReader ScriptedInput(params string[] lines) =>
        new StringReader(string.Join(Environment.NewLine, lines) + Environment.NewLine);

    /// <summary>How many times <paramref name="value"/> appears in <paramref name="text"/>, without overlaps.</summary>
    public static int CountOccurrences(string text, string value)
    {
        int count = 0;
        for (int i = text.IndexOf(value); i >= 0; i = text.IndexOf(value, i + value.Length))
            count++;
        return count;
    }
}
