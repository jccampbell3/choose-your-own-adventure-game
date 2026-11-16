public class Player
{
    public const int MinimumStartingHealthPoints = 1;

    private readonly Die _die;

    // Rolls with the given Die (tests pass a scripted one); with none, an ordinary Die.
    public Player(Die? die = null)
    {
        _die = die ?? new Die();
    }

    public string Name { get; set; } = "";
    public string Race { get; set; } = "";
    public string Occupation { get; set; } = "";

    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    public Weapon? Weapon { get; set; }

    // Magical Armor from the Merchant; null when the Player has none.
    public Armor? Armor { get; set; }

    // True once the Player has made their one Prayer at the Shrine. Not shown in the stats.
    public bool HasPrayed { get; set; }

    public void CreateCharacter(Messages messages, TextReader reader, TextWriter writer)
    {
        Race = PromptForRace(messages, reader, writer);
        Name = PromptForName(messages, reader, writer);
        Occupation = PromptForOccupation(messages, reader, writer);

        AssignWeaponByOccupation();

        int strengthModifier = RollStrength(messages, reader, writer);
        RollAgility(messages, reader, writer);
        RollHealthPoints(messages, strengthModifier, reader, writer);

        PromptForNextAction(messages, reader, writer);
    }

    private string PromptForRace(Messages messages, TextReader reader, TextWriter writer)
    {
        while (true)
        {
            writer.WriteLine(messages.GetMessage("race_prompt"));
            string? input = reader.ReadLine()?.Trim();

            if (messages.IsValidRace(input))
                return messages.NormalizeRace(input!);

            writer.WriteLine(messages.GetMessage("race_invalid"));
        }
    }

    private string PromptForName(Messages messages, TextReader reader, TextWriter writer)
    {
        while (true)
        {
            writer.WriteLine(messages.GetMessage("name_prompt"));
            string? input = reader.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            writer.WriteLine(messages.GetMessage("name_invalid"));
        }
    }

    private string PromptForOccupation(Messages messages, TextReader reader, TextWriter writer)
    {
        while (true)
        {
            writer.WriteLine(messages.GetMessage("occupation_prompt"));
            string? input = reader.ReadLine()?.Trim();

            if (messages.IsValidOccupation(input))
                return messages.NormalizeOccupation(input!);

            writer.WriteLine(messages.GetMessage("occupation_invalid"));
        }
    }

    private void AssignWeaponByOccupation()
    {
        switch (Occupation.ToLower())
        {
            case "fighter":
                Weapon = new Weapon("long sword", 12, "-)=====>");
                break;
            case "magician":
                Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~");
                break;
            case "thief":
                Weapon = new Weapon("dagger", 6, "-)==>");
                break;
            case "archer":
                Weapon = new Weapon("long bow", 8, "}    -->");
                break;
        }
    }

    private int RollStrength(Messages messages, TextReader reader, TextWriter writer)
    {
        PromptRoll(messages.GetMessage("roll_strength"), messages, reader, writer);
        Strength = _die.Roll(20);

        int modifier = _die.Roll(4);

        if (Race == "Halfling")
        {
            Strength -= modifier;
            return -modifier;
        }

        Strength += modifier;
        return modifier;
    }

    private void RollAgility(Messages messages, TextReader reader, TextWriter writer)
    {
        PromptRoll(messages.GetMessage("roll_agility"), messages, reader, writer);
        Agility = _die.Roll(20);

        if (Race == "Halfling" || Race == "Elf")
            Agility += _die.Roll(4);
    }

    private void RollHealthPoints(Messages messages, int strengthModifier, TextReader reader, TextWriter writer)
    {
        PromptRoll(messages.GetMessage("roll_health"), messages, reader, writer);
        // Never below 1, like the Shrine's Curse: a Halfling's negative modifier must not start them dead.
        HealthPoints = Math.Max(MinimumStartingHealthPoints, _die.Roll(20) + strengthModifier);
    }

    private void PromptForNextAction(Messages messages, TextReader reader, TextWriter writer)
    {
        while (true)
        {
            writer.WriteLine(messages.GetMessage("next_action"));
            string? input = reader.ReadLine()?.Trim();

            if (input == "1")
                DisplayStats(messages, writer);
            else if (input == "2")
                return;
            else
                writer.WriteLine(messages.GetMessage("invalid"));
        }
    }

    public void DisplayStats(Messages messages, TextWriter writer)
    {
        writer.WriteLine(messages.GetMessage("stats_header"));
        writer.WriteLine(messages.GetMessage("stats_name") + Name);
        writer.WriteLine(messages.GetMessage("stats_race") + messages.TranslateRaceForDisplay(Race));
        writer.WriteLine(messages.GetMessage("stats_occupation") + messages.TranslateOccupationForDisplay(Occupation));
        writer.WriteLine(messages.GetMessage("stats_strength") + Strength);
        writer.WriteLine(messages.GetMessage("stats_agility") + Agility);
        writer.WriteLine(messages.GetMessage("stats_health") + HealthPoints);

        if (Weapon != null)
        {
            writer.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
            writer.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
        }

        if (Armor != null)
        {
            writer.WriteLine(messages.GetMessage("stats_armor") + messages.GetMessage("armor_name"));
            writer.WriteLine(messages.GetMessage("stats_damage_reduction") + Armor.DamageReduction);
        }
    }

    public int RollAttack()
    {
        return _die.Roll(20);
    }

    public bool AttackHits(int attackRoll)
    {
        return attackRoll <= Strength;
    }

    public int RollDamage()
    {
        if (Weapon == null)
            return 0;

        return _die.Roll(Weapon.MaxDamage);
    }

    public string Attack(Dragon dragon, Messages messages)
    {
        if (Weapon == null)
            return "You have no weapon to attack with.";

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(Occupation),
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        // If this fails to compile, change AsciiArt to whatever exists in Weapon.cs
        output.Add(Weapon.AsciiArt);

        int attackRoll = RollAttack();

        // MISS
        if (!AttackHits(attackRoll))
        {
            output.Add(string.Format(messages.GetMessage("player_missed_dragon"), dragon.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("player_hit_dragon"), dragon.Name));

        int defenseRoll = _die.Roll(20);

        // DEFENSE
        if (defenseRoll <= dragon.Agility)
        {
            output.Add(string.Format(messages.GetMessage("dragon_defended_player_attack"), dragon.Name));
            output.Add(string.Format(
                dragon.GetRandomDefenseTaunt(messages),
                messages.TranslateRaceForDisplay(Race)
            ));
            return string.Join(Environment.NewLine, output);
        }

        // DAMAGE
        int damage = RollDamage();
        dragon.HealthPoints -= damage;

        output.Add(string.Format(messages.GetMessage("player_damage_dealt"), damage));
        output.Add(string.Format(messages.GetMessage("dragon_health_now"), dragon.Name, dragon.HealthPoints));

        // END
        if (dragon.HealthPoints <= 0)
            output.Add(messages.GetMessage("dragon_defeated_narrative"));
        else
            output.Add(dragon.GetRandomDamageReply(messages));

        return string.Join(Environment.NewLine, output);
    }

    private void PromptRoll(string prompt, Messages messages, TextReader reader, TextWriter writer)
    {
        while (true)
        {
            writer.WriteLine(prompt);
            string? input = reader.ReadLine()?.Trim().ToLower();

            if (input == "roll")
                return;

            writer.WriteLine(messages.GetMessage("roll_invalid"));
        }
    }
}