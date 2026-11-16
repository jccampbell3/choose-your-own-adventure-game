// CYOATests/WestPathGameTests.cs
// Game entry point seam: scripted input, captured output and a scripted Die drive the real game loop.
using static TestSupport;

[TestClass]
public class WestPathGameTests
{
    private const string ShrineOffer = "1. Pray";
    private const string PathPrompt = "Which do you take?";

    [TestMethod]
    public void WestLoop_InvalidInputThenLeave_ReturnsToCrossroads()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "west", "9", "2", "e" }).ToArray());

        StringAssert.Contains(output, "You take the West path");
        StringAssert.Contains(output, "the gods may bless you, or they may curse you");
        Assert.AreEqual(1, CountOccurrences(output, "Invalid choice. Please enter 1 or 2."));
        Assert.AreEqual(2, CountOccurrences(output, ShrineOffer), "The offer should be shown again after invalid input.");
        StringAssert.Contains(output, "You bow your head, but do not pray");
        Assert.AreEqual(2, CountOccurrences(output, PathPrompt), "The Player should return to the path prompt after leaving.");
        Assert.IsFalse(output.Contains("Invalid choice. Please enter 'n'"), "'west' must be a valid path.");
    }

    [TestMethod]
    public void WestLoop_EndToEnd_LeaveOfferedAgainPrayThenSilent()
    {
        // west, invalid, leave, west (offer again), pray, west (Silent), exit
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "w", "x", "2", "w", "1", "w", "e" }).ToArray());

        StringAssert.Contains(output, "Invalid choice. Please enter 1 or 2.");
        StringAssert.Contains(output, "You bow your head, but do not pray");
        Assert.AreEqual(3, CountOccurrences(output, ShrineOffer), "Offer: first visit, after invalid input, second visit; never on the third.");
        Assert.AreEqual(1, CountOccurrences(output, "The die shows"), "Only one Prayer per adventure.");
        StringAssert.Contains(output, "The Shrine is silent.");
        Assert.AreEqual(4, CountOccurrences(output, PathPrompt), "The Player returns to the path prompt after every visit.");

        int silentAt = output.IndexOf("The Shrine is silent.");
        Assert.IsTrue(output.LastIndexOf(ShrineOffer) < silentAt, "The silent visit must not show the offer.");
    }

    [TestMethod]
    [DataRow(6, "You are Blessed!", 14)]
    [DataRow(5, "You are Blessed!", 14)]
    [DataRow(4, "Nothing happens.", 11)]
    [DataRow(3, "Nothing happens.", 11)]
    [DataRow(2, "You are Cursed!", 8)]
    [DataRow(1, "You are Cursed!", 8)]
    public void WestLoop_Pray_ShowsScriptedRollOutcomeAndHealth(int roll, string outcome, int health)
    {
        // Dragon 10/10/10, then the Fighter: Strength 10+1, Agility 10, HealthPoints 10+1; then the Prayer.
        ScriptedDie die = new ScriptedDie();
        die.Script(10, 10, 10, 10, 1, 10, 10, roll);

        string output = RunGame(die, CreateEnglishFighter.Concat(new[] { "w", "1", "e" }).ToArray());

        StringAssert.Contains(output, $"The die shows {roll}.");
        StringAssert.Contains(output, outcome);
        int rollAt = output.IndexOf("The die shows");
        Assert.IsTrue(output.IndexOf($"Health Points: {health}", rollAt) > rollAt, "HealthPoints are shown after the Prayer.");
        Assert.AreEqual(2, CountOccurrences(output, PathPrompt), "The Player should return to the path prompt after praying.");
    }

    [TestMethod]
    [DataRow("2", new[] { "1", "humano", "Heroe", "guerrero" }, "El dado muestra", "Santuario")]
    [DataRow("3", new[] { "1", "humain", "Heros", "guerrier" }, "Le de montre", "Sanctuaire")]
    [DataRow("4", new[] { "1", "umano", "Eroe", "guerriero" }, "Il dado mostra", "Santuario")]
    public void WestLoop_InOtherLanguages_ShowsTranslatedShrineText(
        string language, string[] character, string rollText, string shrineName)
    {
        string[] lines = new[] { language }
            .Concat(character)
            .Concat(new[] { "roll", "roll", "roll", "2", "w", "x", "1", "e" })
            .ToArray();

        string output = RunGame(lines);

        StringAssert.Contains(output, rollText);
        StringAssert.Contains(output, shrineName);
        Assert.IsFalse(output.Contains("[shrine") || output.Contains("[west"), "No missing-key placeholders.");
    }
}
