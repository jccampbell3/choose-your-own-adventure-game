public class Game
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly Die _die;

    // True only when writing to the real console, the one place where text colour applies.
    private readonly bool _outputIsConsole;

    /// <summary>
    /// A game that reads, writes and rolls with what it is given. Anything left out is the real console
    /// or an ordinary Die. The console is read here, so a test that swaps it before creating the game still works.
    /// </summary>
    public Game(TextReader? input = null, TextWriter? output = null, Die? die = null)
    {
        _input = input ?? Console.In;
        _output = output ?? Console.Out;
        _outputIsConsole = output == null;
        _die = die ?? new Die();
    }

    public void StartGame()
    {
        // Title screen
        _output.WriteLine("=====================================");
        _output.WriteLine("Welcome to Choose Your Adventure Game");
        _output.WriteLine("=====================================\n");

        // The Dragon is created first (per sequence diagram), so its stats are the game Die's first rolls.
        Dragon dragon = new Dragon(_die);

        // Language selection shown before Messages dictionary is loaded
        _output.WriteLine("Select language / Selecciona idioma / Choisissez la langue / Scegli la lingua:");
        _output.WriteLine("1. English  2. Espanol  3. Francais  4. Italiano");
        _output.Write("Enter choice: ");
        string? langInput = _input.ReadLine()?.Trim();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", "4" => "Italian", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();
        // Display welcome message
        _output.WriteLine();
        _output.WriteLine(messages.GetMessage("welcome"));
        _output.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            _output.WriteLine(messages.GetMessage("menu"));
            _output.Write(messages.GetMessage("enter_choice"));
            string? choice = _input.ReadLine()?.Trim();

            if (choice == "1")
            {
                Player player = new Player(_die);
                player.CreateCharacter(messages, _input, _output);

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    string selectedPath = PromptForPath(player, messages);

                    if (selectedPath == "exit")
                    {
                        _output.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        HandleMerchantEncounter(player, messages);
                    }
                    else if (selectedPath == "west")
                    {
                        HandleShrineEncounter(player, messages);
                    }
                    else if (selectedPath == "north")
                    {
                        HandleDragonEncounter(player, dragon, messages);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else if (choice == "2")
            {
                _output.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
            else
            {
                _output.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    public void EndGame(bool playerWon, bool playerRetreated, Messages messages)
    {
        if (playerRetreated)
            _output.WriteLine(messages.GetMessage("retreat"));
        else if (playerWon)
            _output.WriteLine(messages.GetMessage("victory"));
        else
            _output.WriteLine(messages.GetMessage("defeat"));
    }

    public bool IsValidMenuChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2";
    }

    public bool IsValidPathChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "w" || trimmed == "west" ||
               trimmed == "i" || trimmed == "inventory" ||
               trimmed == "e" || trimmed == "exit";
    }

    public bool IsValidCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "a" || trimmed == "attack" ||
               trimmed == "r" || trimmed == "retreat";
    }

    private string PromptForPath(Player player, Messages messages)
    {
        while (true)
        {
            _output.WriteLine(messages.GetMessage("path_prompt_full"));
            _output.Write(messages.GetMessage("enter_choice"));
            string? input = _input.ReadLine()?.Trim().ToLower();

            if (input == "north" || input == "n")
                return "north";

            if (input == "south" || input == "s")
                return "south";

            if (input == "west" || input == "w")
                return "west";

            if (input == "exit" || input == "e")
                return "exit";

            if (input == "inventory" || input == "i")
            {
                foreach (string line in Inventory.Lines(player, messages))
                    _output.WriteLine(line);
                continue;
            }

            _output.WriteLine(messages.GetMessage("path_invalid"));
        }
    }

    // Thin console loop around the Merchant's rules; afterwards the Player is back at the path prompt.
    private void HandleMerchantEncounter(Player player, Messages messages)
    {
        _output.WriteLine(messages.GetMessage("south_path_narrative"));

        if (!Merchant.WillOffer(player))
        {
            _output.WriteLine(messages.GetMessage("merchant_refusal"));
            return;
        }

        _output.WriteLine(messages.GetMessage("merchant_greeting"));

        MerchantChoice choice;
        while (true)
        {
            _output.WriteLine(messages.GetMessage("merchant_offer"));
            _output.Write(messages.GetMessage("enter_choice"));
            string? input = _input.ReadLine();

            // End of input: treat as leaving so a closed console can't loop forever.
            if (input == null)
            {
                choice = MerchantChoice.Leave;
                break;
            }

            if (Merchant.TryParseChoice(input, out choice))
                break;

            _output.WriteLine(messages.GetMessage("merchant_invalid"));
        }

        switch (Merchant.Choose(player, choice))
        {
            case MerchantOutcome.Armor:
                _output.WriteLine(messages.GetMessage("merchant_received_armor"));
                break;
            case MerchantOutcome.Weapon:
                _output.WriteLine(string.Format(
                    messages.GetMessage("merchant_received_weapon"),
                    messages.TranslateWeaponForDisplay(player.Weapon!.Type)));
                break;
            case MerchantOutcome.Left:
                _output.WriteLine(messages.GetMessage("merchant_leave"));
                break;
            case MerchantOutcome.Refused:
                _output.WriteLine(messages.GetMessage("merchant_refusal"));
                break;
        }
    }

    // Thin console loop around the Shrine's rules; afterwards the Player is back at the path prompt.
    private void HandleShrineEncounter(Player player, Messages messages)
    {
        _output.WriteLine(messages.GetMessage("west_path_narrative"));

        if (!Shrine.WillOffer(player))
        {
            _output.WriteLine(messages.GetMessage("shrine_silent"));
            return;
        }

        _output.WriteLine(messages.GetMessage("shrine_greeting"));
        _output.WriteLine(messages.GetMessage("shrine_warning"));

        ShrineChoice choice;
        while (true)
        {
            _output.WriteLine(messages.GetMessage("shrine_offer"));
            _output.Write(messages.GetMessage("enter_choice"));
            string? input = _input.ReadLine();

            // End of input: treat as leaving so a closed console can't loop forever.
            if (input == null)
            {
                choice = ShrineChoice.Leave;
                break;
            }

            if (Shrine.TryParseChoice(input, out choice))
                break;

            _output.WriteLine(messages.GetMessage("shrine_invalid"));
        }

        ShrineResult result = Shrine.Choose(player, choice, _die);
        switch (result.Outcome)
        {
            case ShrineOutcome.Blessing:
                PrintPrayer(result.Roll, "shrine_blessing", player, messages);
                break;
            case ShrineOutcome.Nothing:
                PrintPrayer(result.Roll, "shrine_nothing", player, messages);
                break;
            case ShrineOutcome.Curse:
                PrintPrayer(result.Roll, "shrine_curse", player, messages);
                break;
            case ShrineOutcome.Left:
                _output.WriteLine(messages.GetMessage("shrine_leave"));
                break;
            case ShrineOutcome.Silent:
                _output.WriteLine(messages.GetMessage("shrine_silent"));
                break;
        }
    }

    // A Prayer prints the roll, then what the gods did, then the Player's HealthPoints.
    private void PrintPrayer(int? roll, string outcomeKey, Player player, Messages messages)
    {
        _output.WriteLine(string.Format(messages.GetMessage("shrine_roll"), roll));
        _output.WriteLine(messages.GetMessage(outcomeKey));
        _output.WriteLine(messages.GetMessage("stats_health") + player.HealthPoints);
    }

    private void HandleDragonEncounter(Player player, Dragon dragon, Messages messages)
    {
        _output.WriteLine(messages.GetMessage("north_path_narrative"));

        if (_outputIsConsole)
            Console.ForegroundColor = ConsoleColor.Red;
        _output.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        if (_outputIsConsole)
            Console.ResetColor();

        _output.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages, _output);
        while (true)
        {
            _output.WriteLine(messages.GetMessage("dragon_encounter_prompt"));
            _output.Write(messages.GetMessage("enter_choice"));
            string? input = _input.ReadLine()?.Trim().ToLower();

            if (input == "a" || input == "attack")
            {
                Combat combat = new Combat(player, dragon, messages, _input, _output);
                bool playerWon = combat.StartCombat();

                if (combat.PlayerRetreated)
                {
                    return; //combat already handled retreat message
                }
                else
                {
                   EndGame(playerWon, false, messages); 
                }
                
                return;
            }

        if (input == "r" || input == "retreat")
        {
            _output.WriteLine(messages.GetMessage("retreat"));
            return;
        }

        _output.WriteLine(messages.GetMessage("invalid"));
    }
}
}
