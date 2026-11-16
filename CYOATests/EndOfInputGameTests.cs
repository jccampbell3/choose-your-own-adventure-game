// CYOATests/EndOfInputGameTests.cs
// Game entry point seam: when scripted input runs out at any prompt, the adventure ends cleanly.
using static TestSupport;

[TestClass]
public class EndOfInputGameTests
{
    private const string AdventureEnds = "Adventure ends. Bye.";

    private static void AssertEndsWith(string output, string expected) =>
        Assert.IsTrue(output.TrimEnd().EndsWith(expected), $"Expected the output to end with '{expected}':\n{output}");

    [TestMethod]
    public void NoInputAtAll_PicksEnglishThenEndsAtTheMainMenu()
    {
        string output = RunGame();

        StringAssert.Contains(output, "Choose; 1. Create Character or 2. Exit.");
        AssertEndsWith(output, AdventureEnds);
    }

    [TestMethod]
    public void InputEndsAtTheMainMenu_EndsTheAdventure()
    {
        string output = RunGame("1");

        Assert.AreEqual(1, CountOccurrences(output, "Choose; 1. Create Character or 2. Exit."));
        AssertEndsWith(output, AdventureEnds);
    }

    [TestMethod]
    public void InputEndsAtThePathPrompt_EndsTheAdventure()
    {
        string output = RunGame(CreateEnglishFighter);

        Assert.AreEqual(1, CountOccurrences(output, "Which do you take?"));
        AssertEndsWith(output, AdventureEnds);
    }

    [TestMethod]
    public void InputEndsAtTheMerchant_EndsTheAdventureWithoutLeaving()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "s" }).ToArray());

        StringAssert.Contains(output, "1. Magical Armor");
        Assert.IsFalse(output.Contains("You politely decline"), "End of input is not Leave.");
        AssertEndsWith(output, AdventureEnds);
    }

    [TestMethod]
    public void InputEndsAtTheShrine_EndsTheAdventureWithoutLeaving()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "w" }).ToArray());

        StringAssert.Contains(output, "1. Pray");
        Assert.IsFalse(output.Contains("You bow your head, but do not pray"), "End of input is not Leave.");
        AssertEndsWith(output, AdventureEnds);
    }

    [TestMethod]
    public void InputEndsAfterInvalidPathAndInventory_EndsTheAdventure()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "x", "i" }).ToArray());

        StringAssert.Contains(output, "Invalid choice. Please enter 'n', 's', 'w', 'i', or 'e'.");
        StringAssert.Contains(output, "----- Inventory -----");
        Assert.AreEqual(3, CountOccurrences(output, "Which do you take?"));
        AssertEndsWith(output, AdventureEnds);
    }

    [TestMethod]
    [DataRow("2", new[] { "1", "humano", "Heroe", "guerrero" }, "Spanish")]
    [DataRow("3", new[] { "1", "humain", "Heros", "guerrier" }, "French")]
    [DataRow("4", new[] { "1", "umano", "Eroe", "guerriero" }, "Italian")]
    public void InputEndsAtTheMerchant_InOtherLanguages_EndsInThatLanguage(string choice, string[] character, string language)
    {
        string[] lines = new[] { choice }
            .Concat(character)
            .Concat(new[] { "roll", "roll", "roll", "2", "s" })
            .ToArray();

        string output = RunGame(lines);

        AssertEndsWith(output, LoadMessages(language).GetMessage("adventure_ends"));
    }
}
