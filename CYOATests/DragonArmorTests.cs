// CYOATests/DragonArmorTests.cs
// Dragon attack seam: a scripted Die fixes every roll so damage is exact.

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

[TestClass]
public class DragonArmorTests
{
    private const int StartingHealth = 20;

    private static Messages GetMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    private static Player CreatePlayer(bool withArmor, int health = StartingHealth)
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 5,
            HealthPoints = health,
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Armor = withArmor ? new Armor() : null
        };
    }

    /// <summary>
    /// Builds a Dragon whose next attack hits (roll 1 vs Strength 20),
    /// is not dodged (roll 20 vs Player Agility 5), and rolls the given damage.
    /// </summary>
    private static Dragon CreateDragonThatHitsFor(int damage)
    {
        ScriptedDie die = new ScriptedDie();
        Dragon dragon = new Dragon(die)
        {
            Strength = 20,
            Agility = 10,
            HealthPoints = 30
        };
        die.Script(1, 20, damage);
        return dragon;
    }

    [TestMethod]
    [DataRow(1, 0)]
    [DataRow(2, 0)]
    [DataRow(3, 1)]
    [DataRow(12, 10)]
    public void Attack_PlayerWithArmor_LosesDamageMinusTwo(int rolledDamage, int expectedLoss)
    {
        Player player = CreatePlayer(withArmor: true);
        Dragon dragon = CreateDragonThatHitsFor(rolledDamage);

        dragon.Attack(player, GetMessages());

        Assert.AreEqual(StartingHealth - expectedLoss, player.HealthPoints);
    }

    [TestMethod]
    public void Attack_PlayerWithoutArmor_TakesFullDamage()
    {
        Player player = CreatePlayer(withArmor: false);
        Dragon dragon = CreateDragonThatHitsFor(7);

        string output = dragon.Attack(player, GetMessages());

        Assert.AreEqual(StartingHealth - 7, player.HealthPoints);
        StringAssert.Contains(output, "Smolderfang deals 7 damage.");
        Assert.IsFalse(output.Contains("armor"), "No armor text expected without Armor.");
    }

    [TestMethod]
    public void Attack_PartiallyAbsorbedHit_ShowsAbsorbedAmountDamageAndHealth()
    {
        Player player = CreatePlayer(withArmor: true);
        Dragon dragon = CreateDragonThatHitsFor(5);

        string output = dragon.Attack(player, GetMessages());

        StringAssert.Contains(output, "Your magical armor absorbs 2 damage.");
        StringAssert.Contains(output, "Smolderfang deals 3 damage.");
        StringAssert.Contains(output, "TestHero's health points are now 17.");
    }

    [TestMethod]
    public void Attack_FullyAbsorbedHit_ShowsAbsorbsBlowInsteadOfDamageLine()
    {
        Player player = CreatePlayer(withArmor: true);
        Dragon dragon = CreateDragonThatHitsFor(2);

        string output = dragon.Attack(player, GetMessages());

        StringAssert.Contains(output, "Your magical armor absorbs the whole blow!");
        Assert.IsFalse(output.Contains("deals"), "Damage-dealt line should not appear for a fully absorbed hit.");
        Assert.IsFalse(output.Contains("absorbs 2 damage"), "Partial-absorb line should not appear for a fully absorbed hit.");
        StringAssert.Contains(output, "TestHero's health points are now 20.");
    }

    [TestMethod]
    public void Attack_PlayerWithArmor_HealthNeverGoesBelowZero()
    {
        Player player = CreatePlayer(withArmor: true, health: 3);
        Dragon dragon = CreateDragonThatHitsFor(12);

        dragon.Attack(player, GetMessages());

        Assert.AreEqual(0, player.HealthPoints);
    }

    [TestMethod]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void Attack_ArmorMessages_AreTranslated(string language)
    {
        Messages messages = GetMessages(language);

        string partial = CreateDragonThatHitsFor(5).Attack(CreatePlayer(withArmor: true), messages);
        string full = CreateDragonThatHitsFor(1).Attack(CreatePlayer(withArmor: true), messages);

        Assert.IsFalse(partial.Contains("[armor_absorbs_damage]"), partial);
        Assert.IsFalse(full.Contains("[armor_absorbs_blow]"), full);
        Assert.IsFalse(partial.Contains("absorbs"), "English text leaked into " + language);
    }
}
