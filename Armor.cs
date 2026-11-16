/// <summary>
/// Magical Armor given by the Merchant. It reduces the damage of each Dragon hit.
/// </summary>
public class Armor
{
    public const string MagicalArmorName = "magical armor";
    public const int MagicalArmorReduction = 2;

    // Canonical Name; shown to the Player through the translated "armor_name" message.
    public string Type { get; }
    public int DamageReduction { get; }

    public Armor()
    {
        Type = MagicalArmorName;
        DamageReduction = MagicalArmorReduction;
    }

    /// <summary>Returns the damage left after this Armor; never below zero.</summary>
    public int Reduce(int damage)
    {
        return Math.Max(0, damage - DamageReduction);
    }
}
