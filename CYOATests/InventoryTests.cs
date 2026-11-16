// CYOATests/InventoryTests.cs
// Inventory seam: the Inventory lines are built without the console.
[TestClass]
public class InventoryTests
{
    private static Messages LoadMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    private static Player CreatePlayer(Weapon? weapon = null)
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = weapon ?? new Weapon("long sword", 12, "-)=====>")
        };
    }

    [TestMethod]
    public void PlainWeapon_ShowsHeadingWeaponAndDamageOnly()
    {
        IReadOnlyList<string> lines = Inventory.Lines(CreatePlayer(), LoadMessages("English"));

        CollectionAssert.AreEqual(new[]
        {
            "----- Inventory -----",
            "Weapon: long sword",
            "Weapon's Maximum Damage Points: 12"
        }, lines.ToArray());
    }

    [TestMethod]
    public void MagicalWeapon_ShowsMagicalNameAndRaisedDamage()
    {
        Player player = CreatePlayer(new Weapon("long sword", 12, "-)=====>").Enchant());

        IReadOnlyList<string> lines = Inventory.Lines(player, LoadMessages("English"));

        CollectionAssert.Contains(lines.ToArray(), "Weapon: magical long sword");
        CollectionAssert.Contains(lines.ToArray(), "Weapon's Maximum Damage Points: 14");
    }

    [TestMethod]
    [DataRow("English", "long sword", 12, "Weapon: long sword", "Weapon's Maximum Damage Points: 12")]
    [DataRow("English", "lightning bolt spell", 12, "Weapon: lightning bolt spell", "Weapon's Maximum Damage Points: 12")]
    [DataRow("English", "dagger", 6, "Weapon: dagger", "Weapon's Maximum Damage Points: 6")]
    [DataRow("English", "long bow", 8, "Weapon: long bow", "Weapon's Maximum Damage Points: 8")]
    [DataRow("Italian", "long sword", 12, "Arma: spada lunga", "Danno Massimo dell'Arma: 12")]
    public void EachOccupationsWeapon_IsShownTranslated(
        string language, string weaponType, int maxDamage, string weaponLine, string damageLine)
    {
        Player player = CreatePlayer(new Weapon(weaponType, maxDamage, "art"));

        string[] lines = Inventory.Lines(player, LoadMessages(language)).ToArray();

        Assert.AreEqual(3, lines.Length);
        Assert.AreEqual(weaponLine, lines[1]);
        Assert.AreEqual(damageLine, lines[2]);
    }

    [TestMethod]
    public void MagicalWeapon_InSpanish_ShowsTranslatedNameAndHeading()
    {
        Player player = CreatePlayer(new Weapon("dagger", 6, "-)==>").Enchant());

        CollectionAssert.AreEqual(new[]
        {
            "----- Inventario -----",
            "Arma: daga magica",
            "Puntos Maximos de Dano del Arma: 8"
        }, Inventory.Lines(player, LoadMessages("Spanish")).ToArray());
    }

    [TestMethod]
    [DataRow("English", "----- Inventory -----", "Weapon: long sword", "Weapon's Maximum Damage Points: 12",
        "Armor: magical armor", "Armor Damage Reduction: 2")]
    [DataRow("French", "----- Inventaire -----", "Arme: epee longue", "Points de Dommages Maximum: 12",
        "Armure : armure magique", "Reduction des Degats de l'Armure : 2")]
    public void WeaponAndMagicalArmor_ShowsArmorNameAndReduction(
        string language, string header, string weaponLine, string damageLine, string armorLine, string reductionLine)
    {
        Player player = CreatePlayer();
        player.Armor = new Armor();

        CollectionAssert.AreEqual(new[] { header, weaponLine, damageLine, armorLine, reductionLine },
            Inventory.Lines(player, LoadMessages(language)).ToArray());
    }

    [TestMethod]
    public void LookingAtInventory_LeavesPlayerUnchanged()
    {
        Player player = CreatePlayer();
        Weapon weapon = player.Weapon!;

        Inventory.Lines(player, LoadMessages("English"));

        Assert.AreSame(weapon, player.Weapon);
        Assert.AreEqual(12, player.Weapon!.MaxDamage);
        Assert.IsNull(player.Armor);
        Assert.AreEqual(25, player.HealthPoints);
    }
}
