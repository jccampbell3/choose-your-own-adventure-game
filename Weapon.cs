public class Weapon
{
    // How much the Merchant's enchantment raises a Weapon's maximum damage.
    public const int EnchantmentBonus = 2;

    public string Type { get; set; }
    public int MaxDamage { get; set; }
    public string AsciiArt { get; set; }

    // Stored explicitly so translations or renames can't change what counts as magical (ADR 0002).
    public bool IsMagical { get; init; }

    public Weapon(string type, int maxDamage, string asciiArt)
    {
        Type = type;
        MaxDamage = maxDamage;
        AsciiArt = asciiArt;
    }

    /// <summary>
    /// Returns the magical version of this Weapon: Canonical Name "magical &lt;name&gt;",
    /// maximum damage +2, same ASCII art.
    /// </summary>
    public Weapon Enchant()
    {
        return new Weapon("magical " + Type, MaxDamage + EnchantmentBonus, AsciiArt)
        {
            IsMagical = true
        };
    }
}
