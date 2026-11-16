// CYOATests/StartingHealthTests.cs
// Character-creation seam: a scripted Die drives every roll, scripted input answers the prompts.
using static TestSupport;

[TestClass]
public class StartingHealthTests
{
    // Creates a Fighter of the given Race; "2" at the end proceeds, "1" first would show the stats screen.
    private static (Player player, string output) CreateCharacter(string race, int[] rolls, params string[] nextActions)
    {
        ScriptedDie die = new ScriptedDie();
        die.Script(rolls);
        Player player = new Player(die);

        string[] lines = new[] { race, "Hero", "fighter", "roll", "roll", "roll" }
            .Concat(nextActions.Length > 0 ? nextActions : new[] { "2" })
            .ToArray();

        StringWriter output = new StringWriter();
        player.CreateCharacter(LoadMessages(), new Prompt(ScriptedInput(lines), output), output);

        return (player, output.ToString());
    }

    // Halfling roll order: Strength d20, Strength modifier d4, Agility d20, Agility bonus d4, health d20.
    private static int[] HalflingRolls(int strengthModifier, int healthRoll) =>
        new[] { 10, strengthModifier, 10, 2, healthRoll };

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void Halfling_HealthRollOfOne_StartsWithOneHealthPoint(int strengthModifier)
    {
        (Player player, _) = CreateCharacter("halfling", HalflingRolls(strengthModifier, healthRoll: 1));

        Assert.AreEqual(1, player.HealthPoints);
    }

    [TestMethod]
    [DataRow(5, 2, 3)]
    [DataRow(2, 1, 1)]
    [DataRow(20, 4, 16)]
    public void Halfling_HealthAlreadyOneOrMore_KeepsRolledValue(int healthRoll, int strengthModifier, int expected)
    {
        (Player player, _) = CreateCharacter("halfling", HalflingRolls(strengthModifier, healthRoll));

        Assert.AreEqual(expected, player.HealthPoints);
    }

    [TestMethod]
    public void Human_HealthRollOfOne_StillGetsPositiveModifier()
    {
        // Human roll order: Strength d20, Strength modifier d4, Agility d20, health d20.
        (Player player, _) = CreateCharacter("human", new[] { 10, 4, 10, 1 });

        Assert.AreEqual(5, player.HealthPoints);
    }

    [TestMethod]
    public void Halfling_FlooredHealth_LeavesStrengthAndAgilityUnchanged()
    {
        (Player player, _) = CreateCharacter("halfling", HalflingRolls(strengthModifier: 4, healthRoll: 1));

        Assert.AreEqual(6, player.Strength, "Strength 10 minus the Halfling modifier 4.");
        Assert.AreEqual(12, player.Agility, "Agility 10 plus the Halfling bonus 2.");
    }

    [TestMethod]
    public void StatsScreen_ShowsFlooredStartingHealth()
    {
        // "1" shows the stats screen, then "2" proceeds.
        (_, string output) = CreateCharacter("halfling", HalflingRolls(strengthModifier: 3, healthRoll: 1), "1", "2");

        StringAssert.Contains(output, "Health Points: 1" + Environment.NewLine);
        Assert.IsFalse(output.Contains("Health Points: -"), "No negative starting health on the stats screen.");
        Assert.IsFalse(output.Contains("Health Points: 0"), "No zero starting health on the stats screen.");
    }
}
