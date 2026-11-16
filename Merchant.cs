/// <summary>A numbered choice the Player can make at the Merchant.</summary>
public enum MerchantChoice
{
    Armor = 1,
    Weapon = 2,
    Leave = 3
}

/// <summary>What happened when the Player answered the Merchant.</summary>
public enum MerchantOutcome
{
    Armor,
    Weapon,
    Left,
    Refused
}

/// <summary>
/// The South Path Merchant's rules. No console input or output and no state of its own:
/// whether the Merchant still makes an offer depends only on the Player.
/// </summary>
public static class Merchant
{
    /// <summary>True while the Player owns no Magical Item.</summary>
    public static bool WillOffer(Player player)
    {
        bool hasMagicalWeapon = player.Weapon != null && player.Weapon.IsMagical;
        return player.Armor == null && !hasMagicalWeapon;
    }

    /// <summary>Applies the Player's choice. Updates the Player only for Armor or Weapon.</summary>
    public static MerchantOutcome Choose(Player player, MerchantChoice choice)
    {
        if (!WillOffer(player))
            return MerchantOutcome.Refused;

        switch (choice)
        {
            case MerchantChoice.Armor:
                player.Armor = new Armor();
                return MerchantOutcome.Armor;

            case MerchantChoice.Weapon when player.Weapon != null:
                player.Weapon = player.Weapon.Enchant();
                return MerchantOutcome.Weapon;

            default:
                return MerchantOutcome.Left;
        }
    }

    /// <summary>Reads a Command typed at the Merchant: "1", "2" or "3".</summary>
    public static bool TryParseChoice(string? input, out MerchantChoice choice)
    {
        switch (input?.Trim())
        {
            case "1": choice = MerchantChoice.Armor; return true;
            case "2": choice = MerchantChoice.Weapon; return true;
            case "3": choice = MerchantChoice.Leave; return true;
            default: choice = default; return false;
        }
    }
}
