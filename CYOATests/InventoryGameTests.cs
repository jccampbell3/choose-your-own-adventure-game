// CYOATests/InventoryGameTests.cs
// Game entry point seam: scripted standard input drives the real game loop.
[TestClass]
public class InventoryGameTests
{
    private const string PathPrompt = "Which do you take?";
    private const string InventoryHeader = "----- Inventory -----";

    // Language, main menu, race, name, occupation, three rolls, then "proceed".
    private static readonly string[] CreateEnglishFighter =
    {
        "1", "1", "human", "Hero", "fighter", "roll", "roll", "roll", "2"
    };

    private static string RunGame(params string[] lines)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        StringWriter output = new StringWriter();

        Console.SetIn(new StringReader(string.Join(Environment.NewLine, lines) + Environment.NewLine));
        Console.SetOut(output);
        try
        {
            new Game().StartGame();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        return output.ToString();
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        for (int i = text.IndexOf(value); i >= 0; i = text.IndexOf(value, i + value.Length))
            count++;
        return count;
    }

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
    public void InventoryCommand_ShowsWeaponThenPathPromptAgain()
    {
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "i", "e" }).ToArray());

        int headerAt = output.IndexOf(InventoryHeader);
        Assert.IsTrue(headerAt >= 0, "Typing 'i' should show the Inventory.");
        StringAssert.Contains(output, "Weapon: long sword");
        Assert.IsFalse(output.Substring(headerAt).Contains("Armor: "), "No Armor line without Magical Armor.");
        Assert.AreEqual(2, CountOccurrences(output, PathPrompt), "The path prompt is shown again after looking.");
        Assert.IsTrue(output.IndexOf(PathPrompt, headerAt) > headerAt, "The path prompt follows the Inventory.");
        Assert.IsFalse(output.Contains("Invalid choice. Please enter 'n'"), "'i' must be a valid Command.");
    }

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
    public void InventoryCommand_EndToEnd_ArmorAppearsAfterMerchant()
    {
        // look, go south and take the Armor, look again, exit
        string output = RunGame(CreateEnglishFighter.Concat(new[] { "i", "s", "1", "inventory", "e" }).ToArray());

        int firstLook = output.IndexOf(InventoryHeader);
        int secondLook = output.IndexOf(InventoryHeader, firstLook + 1);
        Assert.IsTrue(firstLook >= 0 && secondLook > firstLook, "Both looks should show the Inventory.");

        string beforeMerchant = output.Substring(firstLook, secondLook - firstLook);
        Assert.IsFalse(beforeMerchant.Contains("Armor: "), "No Armor line before the Merchant.");

        string afterMerchant = output.Substring(secondLook);
        StringAssert.Contains(afterMerchant, "Armor: magical armor");
        StringAssert.Contains(afterMerchant, "Armor Damage Reduction: 2");

        // Initial prompt, after the first look, after the Merchant, after the second look.
        Assert.AreEqual(4, CountOccurrences(output, PathPrompt), "The path prompt is shown again after each look.");
        Assert.IsTrue(output.IndexOf(PathPrompt, secondLook) > secondLook, "The path prompt follows the second look.");
    }

    [TestMethod]
    [DoNotParallelize] // redirects the shared Console
    [DataRow("2", new[] { "1", "humano", "Heroe", "guerrero" }, "----- Inventario -----", "Arma: espada larga")]
    [DataRow("4", new[] { "1", "umano", "Eroe", "guerriero" }, "----- Inventario -----", "Arma: spada lunga")]
    public void InventoryCommand_InOtherLanguages_ShowsTranslatedInventory(
        string language, string[] character, string header, string weaponLine)
    {
        string[] lines = new[] { language }
            .Concat(character)
            .Concat(new[] { "roll", "roll", "roll", "2", "i", "e" })
            .ToArray();

        string output = RunGame(lines);

        StringAssert.Contains(output, header);
        StringAssert.Contains(output, weaponLine);
        Assert.IsFalse(output.Contains("[inventory") || output.Contains("[stats"), "No missing-key placeholders.");
    }
}
