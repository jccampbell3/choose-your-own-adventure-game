/// <summary>
/// The Player's Inventory: a read-only view of their Weapon and any Magical Armor.
/// No console input or output and no state of its own; looking changes nothing.
/// </summary>
public static class Inventory
{
    /// <summary>The translated lines to show when the Player looks at their Inventory.</summary>
    public static IReadOnlyList<string> Lines(Player player, Messages messages)
    {
        var lines = new List<string> { messages.GetMessage("inventory_header") };

        if (player.Weapon != null)
        {
            lines.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(player.Weapon.Type));
            lines.Add(messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        }

        // No Armor line without Magical Armor, so the Inventory never hints at the Merchant.
        if (player.Armor != null)
        {
            lines.Add(messages.GetMessage("stats_armor") + messages.GetMessage("armor_name"));
            lines.Add(messages.GetMessage("stats_damage_reduction") + player.Armor.DamageReduction);
        }

        return lines;
    }
}
