/// <summary>A numbered choice the Player can make at the Shrine.</summary>
public enum ShrineChoice
{
    Pray = 1,
    Leave = 2
}

/// <summary>What happened when the Player answered the Shrine.</summary>
public enum ShrineOutcome
{
    Blessing,
    Nothing,
    Curse,
    Left,
    Silent
}

/// <summary>A Shrine outcome and the die roll behind it (null when no Prayer was made).</summary>
public readonly record struct ShrineResult(ShrineOutcome Outcome, int? Roll);

/// <summary>
/// The West Path Shrine's rules. No console input or output and no state of its own:
/// whether the Shrine still offers a Prayer depends only on the Player.
/// </summary>
public static class Shrine
{
    /// <summary>HealthPoints a Blessing adds and a Curse takes away.</summary>
    public const int PrayerHealthPoints = 3;

    /// <summary>True while the Player has not yet made their Prayer.</summary>
    public static bool WillOffer(Player player)
    {
        return !player.HasPrayed;
    }

    /// <summary>
    /// Applies the Player's choice. Pray rolls a d6: 1-2 Curse, 3-4 Nothing, 5-6 Blessing.
    /// Every Prayer uses up the Player's one Prayer; Leave changes nothing.
    /// Once the Player has prayed, any choice is Silent and changes nothing.
    /// </summary>
    public static ShrineResult Choose(Player player, ShrineChoice choice, Die die)
    {
        if (!WillOffer(player))
            return new ShrineResult(ShrineOutcome.Silent, null);

        if (choice != ShrineChoice.Pray)
            return new ShrineResult(ShrineOutcome.Left, null);

        player.HasPrayed = true;
        int roll = die.Roll(6);

        if (roll <= 2)
        {
            // A Curse never takes the Player below 1 and never raises HealthPoints; only the Dragon can kill.
            if (player.HealthPoints > 1)
                player.HealthPoints = Math.Max(1, player.HealthPoints - PrayerHealthPoints);
            return new ShrineResult(ShrineOutcome.Curse, roll);
        }

        if (roll <= 4)
            return new ShrineResult(ShrineOutcome.Nothing, roll);

        player.HealthPoints += PrayerHealthPoints;
        return new ShrineResult(ShrineOutcome.Blessing, roll);
    }

    /// <summary>Reads a Command typed at the Shrine: "1" or "2".</summary>
    public static bool TryParseChoice(string? input, out ShrineChoice choice)
    {
        switch (input?.Trim())
        {
            case "1": choice = ShrineChoice.Pray; return true;
            case "2": choice = ShrineChoice.Leave; return true;
            default: choice = default; return false;
        }
    }
}
