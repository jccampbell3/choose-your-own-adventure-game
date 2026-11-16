/// <summary>Reads a typed answer into a choice; false when the answer is not valid.</summary>
public delegate bool ParseChoice<T>(string? input, out T choice);

/// <summary>Raised when input runs out at a prompt. Only the game's start method handles it, by ending the adventure.</summary>
public sealed class InputEndedException : Exception
{
    public InputEndedException() : base("Input ended while the game was waiting for an answer.") { }
}

/// <summary>
/// Asks the Player for input: shows the question, reads an answer, and asks again with an error until the answer
/// is valid. When input runs out it throws <see cref="InputEndedException"/>; callers never see a missing answer.
/// </summary>
public class Prompt
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public Prompt(TextReader input, TextWriter output)
    {
        _input = input;
        _output = output;
    }

    /// <summary>
    /// Shows <paramref name="question"/> on its own line, then <paramref name="answerLabel"/> (if any) on the line the
    /// Player types on, and returns the first answer <paramref name="parse"/> accepts. Each rejected answer shows
    /// <paramref name="invalid"/> and the question again.
    /// </summary>
    public T Ask<T>(string question, ParseChoice<T> parse, string invalid, string? answerLabel = null)
    {
        while (true)
        {
            _output.WriteLine(question);
            if (answerLabel != null)
                _output.Write(answerLabel);

            string? input = _input.ReadLine();
            if (input == null)
                throw new InputEndedException();

            if (parse(input, out T choice))
                return choice;

            _output.WriteLine(invalid);
        }
    }
}
