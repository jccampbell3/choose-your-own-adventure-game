using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static TestSupport;

[TestClass]
public class DragonTests
{

    [TestMethod]
    public void NewDragon_InitializesRequiredProperties()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Assert.AreEqual("Smolderfang", dragon.Name);
        Assert.IsTrue(dragon.Strength >= 1 && dragon.Strength <= 20);
        Assert.IsTrue(dragon.Agility >= 1 && dragon.Agility <= 20);
        Assert.IsTrue(dragon.HealthPoints >= 1 && dragon.HealthPoints <= 20);

        Assert.IsNotNull(dragon.Weapon);
        Assert.AreEqual("claws", dragon.Weapon.Type);
        Assert.AreEqual(12, dragon.Weapon.MaxDamage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(dragon.Weapon.AsciiArt));
    }

    [TestMethod]
    public void GetRandomDefenseTaunt_ReturnsExpectedTaunt()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = LoadMessages();

        string taunt = dragon.GetRandomDefenseTaunt(messages);

        string[] validTaunts =
        {
        messages.GetMessage("dragon_defense_taunt_1"),
        messages.GetMessage("dragon_defense_taunt_2"),
        messages.GetMessage("dragon_defense_taunt_3"),
        messages.GetMessage("dragon_defense_taunt_4")
    };

        CollectionAssert.Contains(validTaunts, taunt);
    }

    [TestMethod]
    public void GetRandomDamageReply_ReturnsExpectedReply()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = LoadMessages();

        string reply = dragon.GetRandomDamageReply(messages);

        string[] validReplies =
        {
        messages.GetMessage("dragon_damage_reply_1"),
        messages.GetMessage("dragon_damage_reply_2"),
        messages.GetMessage("dragon_damage_reply_3"),
        messages.GetMessage("dragon_damage_reply_4")
    };

        CollectionAssert.Contains(validReplies, reply);
    }

    [TestMethod]
    public void Attack_ReturnsNonEmptyResultString()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        string result = dragon.Attack(player, LoadMessages());

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void Attack_DoesNotIncreasePlayerHealth()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        int startingHealth = player.HealthPoints;

        string result = dragon.Attack(player, LoadMessages());

        Assert.IsTrue(player.HealthPoints <= startingHealth);
    }

    [TestMethod]
    public void Attack_KeepsPlayerHealthNonNegative()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 0,
            HealthPoints = 1
        };

        string result = dragon.Attack(player, LoadMessages());

        Assert.IsTrue(player.HealthPoints >= 0);
    }
}

[TestClass]
public class GamePbi18Tests
{
    // The Dragon-encounter flow is tested through the game entry point in NorthPathGameTests.

    [TestMethod]
    public void DragonDisplayStats_UsesLocalizedLabels()
    {
        Dragon dragon = new Dragon(new Die());
        Messages messages = LoadMessages();
        StringWriter writer = new StringWriter();

        dragon.DisplayStats(messages, writer);

        string output = writer.ToString();
        StringAssert.Contains(output, messages.GetMessage("dragon_stats_header"));
        StringAssert.Contains(output, messages.GetMessage("stats_name"));
        StringAssert.Contains(output, messages.GetMessage("stats_strength"));
        StringAssert.Contains(output, messages.GetMessage("stats_agility"));
        StringAssert.Contains(output, messages.GetMessage("stats_health"));
        StringAssert.Contains(output, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(output, messages.GetMessage("stats_damage"));
    }

    [TestMethod]
    public void DragonEncounter_MessageKeys_AreLoaded()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Assert.AreNotEqual("[north_path_narrative]", messages.GetMessage("north_path_narrative"));
        Assert.AreNotEqual("[dragon_intro]", messages.GetMessage("dragon_intro"));
        Assert.AreNotEqual("[dragon_stats_intro]", messages.GetMessage("dragon_stats_intro"));
        Assert.AreNotEqual("[dragon_encounter_prompt]", messages.GetMessage("dragon_encounter_prompt"));
        Assert.AreNotEqual("[dragon_stats_header]", messages.GetMessage("dragon_stats_header"));
    }
}
