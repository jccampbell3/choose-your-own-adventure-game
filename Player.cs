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

    public void CreateCharacter(Messages messages, Prompt prompt, TextWriter writer)
    {
        Race = PromptForRace(messages, prompt);
        Name = PromptForName(messages, prompt);
        Occupation = PromptForOccupation(messages, prompt);

        AssignWeaponByOccupation();

        int strengthModifier = RollStrength(messages, prompt);
        RollAgility(messages, prompt);
        RollHealthPoints(messages, strengthModifier, prompt);

        PromptForNextAction(messages, prompt, writer);
    }

    // Any Race name in the current Language; returns its Canonical Name.
    private static string PromptForRace(Messages messages, Prompt prompt)
    {
        bool Parse(string? input, out string race)
        {
            race = messages.IsValidRace(input) ? messages.NormalizeRace(input!.Trim()) : "";
            return race != "";
        }

        return prompt.Ask<string>(messages.GetMessage("race_prompt"), Parse, messages.GetMessage("race_invalid"));
    }

    private static string PromptForName(Messages messages, Prompt prompt)
    {
        static bool Parse(string? input, out string name)
        {
            name = input?.Trim() ?? "";
            return name != "";
        }

        return prompt.Ask<string>(messages.GetMessage("name_prompt"), Parse, messages.GetMessage("name_invalid"));
    }

    // Any Occupation name in the current Language; returns its Canonical Name.
    private static string PromptForOccupation(Messages messages, Prompt prompt)
    {
        bool Parse(string? input, out string occupation)
        {
            occupation = messages.IsValidOccupation(input) ? messages.NormalizeOccupation(input!.Trim()) : "";
            return occupation != "";
        }

        return prompt.Ask<string>(messages.GetMessage("occupation_prompt"), Parse, messages.GetMessage("occupation_invalid"));
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

    private int RollStrength(Messages messages, Prompt prompt)
    {
        PromptRoll(messages.GetMessage("roll_strength"), messages, prompt);
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

    private void RollAgility(Messages messages, Prompt prompt)
    {
        PromptRoll(messages.GetMessage("roll_agility"), messages, prompt);
        Agility = _die.Roll(20);

        if (Race == "Halfling" || Race == "Elf")
            Agility += _die.Roll(4);
    }

    private void RollHealthPoints(Messages messages, int strengthModifier, Prompt prompt)
    {
        PromptRoll(messages.GetMessage("roll_health"), messages, prompt);
        // Never below 1, like the Shrine's Curse: a Halfling's negative modifier must not start them dead.
        HealthPoints = Math.Max(MinimumStartingHealthPoints, _die.Roll(20) + strengthModifier);
    }

    private void PromptForNextAction(Messages messages, Prompt prompt, TextWriter writer)
    {
        // "1" lists the stats and asks again; "2" proceeds on the adventure.
        static bool Parse(string? input, out string choice)
        {
            choice = input?.Trim() ?? "";
            return choice == "1" || choice == "2";
        }

        while (prompt.Ask<string>(messages.GetMessage("next_action"), Parse, messages.GetMessage("invalid")) == "1")
            DisplayStats(messages, writer);
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

    private static void PromptRoll(string question, Messages messages, Prompt prompt)
    {
        static bool Parse(string? input, out bool rolled)
        {
            rolled = input?.Trim().ToLower() == "roll";
            return rolled;
        }

        prompt.Ask<bool>(question, Parse, messages.GetMessage("roll_invalid"));
    }
}