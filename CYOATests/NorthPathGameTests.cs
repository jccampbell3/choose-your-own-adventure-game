// CYOATests/NorthPathGameTests.cs
// Game entry point seam: scripted input, captured output and a scripted Die drive a full Dragon fight.
using static TestSupport;

[TestClass]
public class NorthPathGameTests
{
    private const string Victory = "With the dragon vanquished, you collect the horde of gold";
    private const string Defeat = "The dragon laughs as your eyesight fades";
    private const string Retreat = "You quickly retreat. You come home poor but alive";
    private const string RetreatInCombat = "You hastily retreat licking your wounds.";

    // Roll order: Dragon Strength, Agility, HealthPoints; then the Human Fighter's
    // Strength, Strength modifier, Agility, health; then the fight.
    private static ScriptedDie Rolls(params int[] rolls)
    {
        ScriptedDie die = new ScriptedDie();
        die.Script(rolls);
        return die;
    }

    private static string GoNorth(ScriptedDie die, params string[] afterCreation) =>
        RunGame(die, CreateEnglishFighter.Concat(new[] { "n" }).Concat(afterCreation).ToArray());

    [TestMethod]
    public void Attack_PlayerKillsDragonWithFirstBlow_Victory()
    {
        // Dragon 1/1/5 HP. Player Strength 15+1, Agility 10, HealthPoints 10+1.
        // Player attack 1 hits, Dragon defence 20 fails, damage 12 kills the Dragon.
        string output = GoNorth(Rolls(1, 1, 5, 15, 1, 10, 10, 1, 20, 12), "a");

        StringAssert.Contains(output, "The dragon says 'I am Smolderfang. Leave or perish!'");
        StringAssert.Contains(output, "----- Dragon Stats -----");
        StringAssert.Contains(output, "You deal 12 damage.");
        StringAssert.Contains(output, "Smolderfang's health points are now -7.");
        StringAssert.Contains(output, Victory);
        Assert.IsFalse(output.Contains(Defeat));
    }

    [TestMethod]
    public void Attack_DragonKillsPlayerWithFirstBlow_Defeat()
    {
        // Dragon 20/20/20. Player Strength 1+1, Agility 1, HealthPoints 1+1.
        // Player attack 20 misses; Dragon attack 1 hits, Player defence 20 fails, damage 12.
        string output = GoNorth(Rolls(20, 20, 20, 1, 1, 1, 1, 20, 1, 20, 12), "a");

        StringAssert.Contains(output, Defeat);
        Assert.IsFalse(output.Contains(Victory));
    }

    [TestMethod]
    public void Retreat_AtTheEncounter_EndsWithRetreat()
    {
        string output = GoNorth(Rolls(10, 10, 10, 10, 1, 10, 10), "r");

        StringAssert.Contains(output, "Do you wish to retreat or attack?");
        StringAssert.Contains(output, Retreat);
        Assert.IsFalse(output.Contains("===== Combat Stats ====="), "Retreating before the fight starts no combat.");
    }

    [TestMethod]
    public void Retreat_DuringCombat_EndsWithCombatRetreat()
    {
        // Dragon 1/1/20. Player Strength 15+1, Agility 20, HealthPoints 20+1.
        // Player attack 20 misses; Dragon attack 20 misses; then the Player retreats.
        string output = GoNorth(Rolls(1, 1, 20, 15, 1, 20, 20, 20, 20), "a", "r");

        StringAssert.Contains(output, "===== Combat Stats =====");
        StringAssert.Contains(output, "Enter 'a' to attack or 'r' to retreat.");
        StringAssert.Contains(output, RetreatInCombat);
        Assert.IsFalse(output.Contains(Victory) || output.Contains(Defeat));
    }

    private const string PlayerAttacks = "you withdraw your long sword and attack the dragon.";
    private const string AttackPrompt = "Enter 'a' to attack or 'r' to retreat.";
    private const string Invalid = "Invalid choice. Please try again.";

    // Dragon 1/1/20. Player Strength 15+1, Agility 20, HealthPoints 20+1. Then two rounds where
    // both sides miss (Player attack 20, Dragon attack 20), so nobody's health changes.
    private static ScriptedDie TwoMissedRounds() => Rolls(1, 1, 20, 15, 1, 20, 20, 20, 20, 20, 20);

    [TestMethod]
    public void InvalidCombatInput_AsksAgainWithoutAttacking()
    {
        string output = GoNorth(TwoMissedRounds(), "a", "x", "a", "r");

        Assert.AreEqual(1, CountOccurrences(output, Invalid));
        Assert.AreEqual(2, CountOccurrences(output, PlayerAttacks), "Only 'a' attacks: the first round and the one after 'a'.");
        Assert.AreEqual(3, CountOccurrences(output, AttackPrompt), "After round 1, again after the invalid input, after round 2.");
        StringAssert.Contains(output, RetreatInCombat);
    }

    [TestMethod]
    public void InvalidDragonEncounterInput_AsksAgain()
    {
        string output = GoNorth(Rolls(10, 10, 10, 10, 1, 10, 10), "x", "r");

        Assert.AreEqual(1, CountOccurrences(output, Invalid));
        Assert.AreEqual(2, CountOccurrences(output, "Do you wish to retreat or attack?"));
        StringAssert.Contains(output, Retreat);
    }

    [TestMethod]
    public void InputEndsAtTheDragonEncounter_EndsTheAdventure()
    {
        string output = GoNorth(Rolls(10, 10, 10, 10, 1, 10, 10));

        StringAssert.Contains(output, "Do you wish to retreat or attack?");
        Assert.IsTrue(output.TrimEnd().EndsWith("Adventure ends. Bye."), output);
    }

    [TestMethod]
    public void InputEndsDuringCombat_EndsTheAdventureWithoutAnotherAttack()
    {
        string output = GoNorth(TwoMissedRounds(), "a");

        Assert.AreEqual(1, CountOccurrences(output, PlayerAttacks));
        StringAssert.Contains(output, AttackPrompt);
        Assert.IsTrue(output.TrimEnd().EndsWith("Adventure ends. Bye."), output);
    }

    [TestMethod]
    public void DragonEncounter_ShowsNarrativeIntroStatsAndPrompt()
    {
        Messages messages = LoadMessages();

        string output = GoNorth(Rolls(10, 10, 10, 10, 1, 10, 10), "r");

        StringAssert.Contains(output, messages.GetMessage("north_path_narrative"));
        StringAssert.Contains(output, string.Format(messages.GetMessage("dragon_intro"), "Smolderfang"));
        StringAssert.Contains(output, messages.GetMessage("dragon_stats_intro"));
        StringAssert.Contains(output, messages.GetMessage("dragon_encounter_prompt"));
    }
}
