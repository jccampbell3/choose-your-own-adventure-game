// CYOATests/WestPathGameTests.cs
// Game entry point seam: scripted standard input drives the real game loop.
using static TestSupport;

[TestClass]
public class WestPathGameTests
{
    private const string ShrineOffer = "1. Pray";
    private const string PathPrompt = "Which do you take?";

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
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
    [DoNotParallelize] // redirects the shared Console
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
    [DoNotParallelize] // redirects the shared Console
    public void WestLoop_Pray_ShowsRollOutcomeAndHealth()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "w", "1", "e" }).ToArray());

        // The game's own Die is not scripted, so only check that one outcome appeared.
        StringAssert.Matches(output, new System.Text.RegularExpressions.Regex(@"The die shows [1-6]\."));
        int outcomes = new[] { "You are Blessed!", "Nothing happens.", "You are Cursed!" }
            .Count(text => output.Contains(text));
        Assert.AreEqual(1, outcomes, "Exactly one Prayer outcome should be shown.");
        int rollAt = output.IndexOf("The die shows");
        Assert.IsTrue(output.IndexOf("Health Points: ", rollAt) > rollAt, "HealthPoints are shown after the Prayer.");
        Assert.AreEqual(2, CountOccurrences(output, PathPrompt), "The Player should return to the path prompt after praying.");
    }

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
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
