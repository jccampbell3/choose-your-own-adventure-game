// CYOATests/SouthPathGameTests.cs
// Game entry point seam: scripted standard input drives the real game loop.
using static TestSupport;

[TestClass]
public class SouthPathGameTests
{
    private const string OldSouthText = "You are bored and return home";

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
    public void SouthLoop_GetItemThenRefusedOnSecondVisit()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "s", "2", "s", "e" }).ToArray());

        StringAssert.Contains(output, "you meet a traveling Merchant");
        StringAssert.Contains(output, "The Merchant smiles");
        StringAssert.Contains(output, "You now carry: magical long sword.");
        StringAssert.Contains(output, "I cannot give you anything else.");
        Assert.AreEqual(1, CountOccurrences(output, "1. Magical Armor"), "The offer should be made only on the first visit.");
        Assert.AreEqual(3, CountOccurrences(output, "Which do you take?"), "The Player should return to the path prompt after each visit.");
        Assert.IsFalse(output.Contains(OldSouthText), "Old 'returned home bored' text must be gone.");
    }

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
    public void SouthLoop_LeaveThenOfferedAgain()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "s", "3", "s", "1", "e" }).ToArray());

        StringAssert.Contains(output, "You politely decline");
        Assert.AreEqual(2, CountOccurrences(output, "1. Magical Armor"));
        StringAssert.Contains(output, "You put on the magical armor.");
    }

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
    public void SouthLoop_InvalidInputAtMerchant_ShowsErrorAndAsksAgain()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "s", "7", "armor", "1", "e" }).ToArray());

        Assert.AreEqual(2, CountOccurrences(output, "Invalid choice. Please enter 1, 2 or 3."));
        Assert.AreEqual(3, CountOccurrences(output, "1. Magical Armor"), "The offer should be shown again after each invalid entry.");
        StringAssert.Contains(output, "You put on the magical armor.");
    }

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
    public void SouthLoop_InItalian_ShowsTranslatedMerchantText()
    {
        string[] createItalianArcher = { "4", "1", "umano", "Eroe", "arciere", "roll", "roll", "roll", "2" };

        string output = RunGame(createItalianArcher.Concat(new[] { "s", "2", "s", "e" }).ToArray());

        StringAssert.Contains(output, "Il Mercante sorride");
        StringAssert.Contains(output, "Ora porti: arco lungo magico.");
        StringAssert.Contains(output, "Non posso darti nient'altro.");
        Assert.IsFalse(output.Contains("[merchant"), "No missing-key placeholders.");
    }
}
