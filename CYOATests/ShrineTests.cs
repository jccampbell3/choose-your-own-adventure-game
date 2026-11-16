// CYOATests/ShrineTests.cs
// Shrine seam: the rules run without the console, with a scripted Die for every roll.
[TestClass]
public class ShrineTests
{
    private static Player CreatePlayer(int healthPoints = 20)
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = healthPoints,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    [TestMethod]
    public void Leaving_LeavesPlayerUnchangedAndPrayerAvailable()
    {
        Player player = CreatePlayer();
        ScriptedDie die = new ScriptedDie();
        die.Script(6);

        ShrineResult result = Shrine.Choose(player, ShrineChoice.Leave, die);

        Assert.AreEqual(ShrineOutcome.Left, result.Outcome);
        Assert.IsNull(result.Roll, "Leaving rolls no die.");
        Assert.AreEqual(20, player.HealthPoints);
        Assert.IsTrue(Shrine.WillOffer(player));
    }

    private static ShrineResult PrayWithRoll(Player player, int roll)
    {
        ScriptedDie die = new ScriptedDie();
        die.Script(roll);
        return Shrine.Choose(player, ShrineChoice.Pray, die);
    }

    [TestMethod]
    [DataRow(1, ShrineOutcome.Curse, 17)]
    [DataRow(2, ShrineOutcome.Curse, 17)]
    [DataRow(3, ShrineOutcome.Nothing, 20)]
    [DataRow(4, ShrineOutcome.Nothing, 20)]
    [DataRow(5, ShrineOutcome.Blessing, 23)]
    [DataRow(6, ShrineOutcome.Blessing, 23)]
    public void Praying_RollDecidesOutcome(int roll, ShrineOutcome expected, int expectedHealth)
    {
        Player player = CreatePlayer(healthPoints: 20);

        ShrineResult result = PrayWithRoll(player, roll);

        Assert.AreEqual(expected, result.Outcome);
        Assert.AreEqual(roll, result.Roll, "The result reports the roll the Die returned.");
        Assert.AreEqual(expectedHealth, player.HealthPoints);
    }

    [TestMethod]
    public void NewPlayer_HasNotPrayedAndIsOffered()
    {
        Player player = CreatePlayer();

        Assert.IsFalse(player.HasPrayed);
        Assert.IsTrue(Shrine.WillOffer(player));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(6)]
    public void EveryPrayerOutcome_UsesUpThePrayer(int roll)
    {
        Player player = CreatePlayer();

        PrayWithRoll(player, roll);

        Assert.IsTrue(player.HasPrayed);
        Assert.IsFalse(Shrine.WillOffer(player));
    }

    [TestMethod]
    public void Leaving_DoesNotUseUpThePrayer()
    {
        Player player = CreatePlayer();
        Shrine.Choose(player, ShrineChoice.Leave, new ScriptedDie());

        Assert.IsFalse(player.HasPrayed);
        Assert.AreEqual(ShrineOutcome.Blessing, PrayWithRoll(player, 5).Outcome);
    }

    [TestMethod]
    [DataRow(ShrineChoice.Pray)]
    [DataRow(ShrineChoice.Leave)]
    public void AfterPraying_AnyChoiceIsSilentAndPlayerUnchanged(ShrineChoice second)
    {
        Player player = CreatePlayer(healthPoints: 20);
        PrayWithRoll(player, 6);
        ScriptedDie die = new ScriptedDie();
        die.Script(6);

        ShrineResult result = Shrine.Choose(player, second, die);

        Assert.AreEqual(ShrineOutcome.Silent, result.Outcome);
        Assert.IsNull(result.Roll);
        Assert.AreEqual(23, player.HealthPoints, "A silent Shrine must not Bless again.");
        Assert.IsTrue(player.HasPrayed);
    }

    [TestMethod]
    [DataRow(4, 1)]
    [DataRow(3, 1)]
    [DataRow(2, 1)]
    [DataRow(1, 1)]
    [DataRow(0, 0)]
    [DataRow(-2, -2)]
    public void Curse_NeverTakesPlayerBelowOneAndNeverHeals(int startingHealth, int expectedHealth)
    {
        Player player = CreatePlayer(healthPoints: startingHealth);

        ShrineResult result = PrayWithRoll(player, 1);

        Assert.AreEqual(ShrineOutcome.Curse, result.Outcome);
        Assert.AreEqual(expectedHealth, player.HealthPoints);
    }

    [TestMethod]
    public void Blessing_HasNoUpperLimit()
    {
        Player player = CreatePlayer(healthPoints: 40);

        PrayWithRoll(player, 6);

        Assert.AreEqual(43, player.HealthPoints);
    }

    [TestMethod]
    public void Curse_IsNotReducedByMagicalArmor()
    {
        Player player = CreatePlayer(healthPoints: 20);
        player.Armor = new Armor();

        PrayWithRoll(player, 2);

        Assert.AreEqual(17, player.HealthPoints);
    }

    [TestMethod]
    [DataRow("1", ShrineChoice.Pray)]
    [DataRow(" 1 ", ShrineChoice.Pray)]
    [DataRow("2", ShrineChoice.Leave)]
    [DataRow(" 2\t", ShrineChoice.Leave)]
    public void TryParseChoice_AcceptsNumberedChoices(string input, ShrineChoice expected)
    {
        Assert.IsTrue(Shrine.TryParseChoice(input, out ShrineChoice choice));
        Assert.AreEqual(expected, choice);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("3")]
    [DataRow("pray")]
    [DataRow("12")]
    public void TryParseChoice_RejectsAnythingElse(string? input)
    {
        Assert.IsFalse(Shrine.TryParseChoice(input, out _));
    }
}
