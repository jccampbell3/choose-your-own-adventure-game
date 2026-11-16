[TestClass]
public class CombatTests
{
    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    private Player CreateTestPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private Dragon CreateTestDragon()
    {
        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return dragon;
    }

    [TestMethod]
    public void Combat_Constructor_InitializesPlayerAndDragon()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages, TextReader.Null, TextWriter.Null);

        Assert.AreSame(player, combat.player);
        Assert.AreSame(dragon, combat.dragon);
    }

    [TestMethod]
    public void Combat_PlayerRetreated_DefaultsToFalse()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages, TextReader.Null, TextWriter.Null);

        Assert.IsFalse(combat.PlayerRetreated);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages, TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesCombatHeaders()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages, TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_player_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_dragon_header"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesPlayerCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages, TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
        StringAssert.Contains(result, player.Weapon!.Type);
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesDragonCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages, TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, dragon.Name);
        StringAssert.Contains(result, dragon.Strength.ToString());
        StringAssert.Contains(result, dragon.Agility.ToString());
        StringAssert.Contains(result, dragon.HealthPoints.ToString());
        StringAssert.Contains(result, dragon.Weapon.Type);
        StringAssert.Contains(result, dragon.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages, TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
    }

    // --- Magical Items in the combat stats ---

    [TestMethod]
    public void GetCombatStatsDisplay_WithArmor_ShowsArmorAndReduction()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor();
        Combat combat = new Combat(player, CreateTestDragon(), CreateEnglishMessages(), TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, "Armor: magical armor");
        StringAssert.Contains(result, "Armor Damage Reduction: 2");
    }

    [TestMethod]
    public void GetCombatStatsDisplay_WithoutArmor_ShowsNoArmorLines()
    {
        Combat combat = new Combat(CreateTestPlayer(), CreateTestDragon(), CreateEnglishMessages(), TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(result.Contains("Armor"), result);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_WithMagicalWeapon_ShowsMagicalNameAndBoostedDamage()
    {
        Player player = CreateTestPlayer();
        Merchant.Choose(player, MerchantChoice.Weapon);
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        Combat combat = new Combat(player, CreateTestDragon(), messages, TextReader.Null, TextWriter.Null);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, "Arma: spada lunga magica");
        StringAssert.Contains(result, "Danno Massimo dell'Arma: 14");
    }

    [TestMethod]
    public void PlayerDisplayStats_ShowsArmorOnlyWhenOwned()
    {
        Messages messages = CreateEnglishMessages();
        Player player = CreateTestPlayer();

        string without = Capture(writer => player.DisplayStats(messages, writer));
        player.Armor = new Armor();
        string with = Capture(writer => player.DisplayStats(messages, writer));

        Assert.IsFalse(without.Contains("Armor"), without);
        StringAssert.Contains(with, "Armor: magical armor");
        StringAssert.Contains(with, "Armor Damage Reduction: 2");
    }

    private static string Capture(Action<TextWriter> write)
    {
        StringWriter writer = new StringWriter();
        write(writer);
        return writer.ToString();
    }
}
