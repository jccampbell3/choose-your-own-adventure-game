// CYOATests/MerchantTests.cs
// Merchant seam: the rules run without the console.
[TestClass]
public class MerchantTests
{
    private static Player CreatePlayer(string occupation = "Fighter", Weapon? weapon = null)
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = occupation,
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = weapon ?? new Weapon("long sword", 12, "-)=====>")
        };
    }

    [TestMethod]
    public void ChoosingArmor_GivesPlayerMagicalArmorWithReductionTwo()
    {
        Player player = CreatePlayer();

        MerchantOutcome outcome = Merchant.Choose(player, MerchantChoice.Armor);

        Assert.AreEqual(MerchantOutcome.Armor, outcome);
        Assert.IsNotNull(player.Armor);
        Assert.AreEqual(2, player.Armor.DamageReduction);
        Assert.AreEqual("magical armor", player.Armor.Type);
        Assert.AreEqual("long sword", player.Weapon!.Type, "Choosing Armor must not change the Weapon.");
        Assert.IsFalse(player.Weapon.IsMagical);
    }

    [TestMethod]
    [DataRow("long sword", 12, "-)=====>", "magical long sword", 14)]
    [DataRow("lightning bolt spell", 12, "zap~~~~~~", "magical lightning bolt spell", 14)]
    [DataRow("dagger", 6, "-)==>", "magical dagger", 8)]
    [DataRow("long bow", 8, "}    -->", "magical long bow", 10)]
    public void ChoosingWeapon_EnchantsPlayersOwnWeapon(
        string type, int maxDamage, string art, string expectedType, int expectedMax)
    {
        Player player = CreatePlayer(weapon: new Weapon(type, maxDamage, art));

        MerchantOutcome outcome = Merchant.Choose(player, MerchantChoice.Weapon);

        Assert.AreEqual(MerchantOutcome.Weapon, outcome);
        Assert.AreEqual(expectedType, player.Weapon!.Type);
        Assert.AreEqual(expectedMax, player.Weapon.MaxDamage);
        Assert.AreEqual(art, player.Weapon.AsciiArt);
        Assert.IsTrue(player.Weapon.IsMagical);
        Assert.IsNull(player.Armor);
    }

    [TestMethod]
    public void ChoosingLeave_LeavesPlayerUnchanged()
    {
        Player player = CreatePlayer();

        MerchantOutcome outcome = Merchant.Choose(player, MerchantChoice.Leave);

        Assert.AreEqual(MerchantOutcome.Left, outcome);
        Assert.IsNull(player.Armor);
        Assert.AreEqual("long sword", player.Weapon!.Type);
        Assert.AreEqual(12, player.Weapon.MaxDamage);
        Assert.IsFalse(player.Weapon.IsMagical);
    }

    [TestMethod]
    public void AfterLeaving_NextOfferIsAccepted()
    {
        Player player = CreatePlayer();
        Merchant.Choose(player, MerchantChoice.Leave);

        Assert.IsTrue(Merchant.WillOffer(player));
        Assert.AreEqual(MerchantOutcome.Armor, Merchant.Choose(player, MerchantChoice.Armor));
    }

    [TestMethod]
    [DataRow(MerchantChoice.Armor)]
    [DataRow(MerchantChoice.Weapon)]
    [DataRow(MerchantChoice.Leave)]
    public void AfterReceivingArmor_AnyChoiceIsRefusedAndPlayerUnchanged(MerchantChoice second)
    {
        Player player = CreatePlayer();
        Merchant.Choose(player, MerchantChoice.Armor);
        Armor armorBefore = player.Armor!;

        MerchantOutcome outcome = Merchant.Choose(player, second);

        Assert.AreEqual(MerchantOutcome.Refused, outcome);
        Assert.IsFalse(Merchant.WillOffer(player));
        Assert.AreSame(armorBefore, player.Armor);
        Assert.AreEqual("long sword", player.Weapon!.Type);
        Assert.AreEqual(12, player.Weapon.MaxDamage);
    }

    [TestMethod]
    [DataRow(MerchantChoice.Armor)]
    [DataRow(MerchantChoice.Weapon)]
    [DataRow(MerchantChoice.Leave)]
    public void AfterReceivingWeapon_AnyChoiceIsRefusedAndPlayerUnchanged(MerchantChoice second)
    {
        Player player = CreatePlayer();
        Merchant.Choose(player, MerchantChoice.Weapon);

        MerchantOutcome outcome = Merchant.Choose(player, second);

        Assert.AreEqual(MerchantOutcome.Refused, outcome);
        Assert.IsFalse(Merchant.WillOffer(player));
        Assert.IsNull(player.Armor);
        Assert.AreEqual("magical long sword", player.Weapon!.Type);
        Assert.AreEqual(14, player.Weapon.MaxDamage);
    }

    [TestMethod]
    public void MagicalWeapon_IsRecognizedByPropertyNotName()
    {
        // A Weapon whose name merely looks magical is not a Magical Item.
        Player player = CreatePlayer(weapon: new Weapon("magical long sword", 12, "-)=====>"));

        Assert.IsTrue(Merchant.WillOffer(player));
    }

    [TestMethod]
    [DataRow("1", MerchantChoice.Armor)]
    [DataRow(" 2 ", MerchantChoice.Weapon)]
    [DataRow("3", MerchantChoice.Leave)]
    public void TryParseChoice_AcceptsNumberedChoices(string input, MerchantChoice expected)
    {
        Assert.IsTrue(Merchant.TryParseChoice(input, out MerchantChoice choice));
        Assert.AreEqual(expected, choice);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("4")]
    [DataRow("armor")]
    public void TryParseChoice_RejectsAnythingElse(string? input)
    {
        Assert.IsFalse(Merchant.TryParseChoice(input, out _));
    }
}
