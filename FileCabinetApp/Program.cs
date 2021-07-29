using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Gleb Lizhbanov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static readonly FileCabinetService FileCabinetService = new ();
        private static readonly Tuple<string, Action<string>>[] Commands =
        {
            new ("help", PrintHelp),
            new ("stat", Stat),
            new ("create", Create),
            new ("edit", Edit),
            new ("list", List),
            new ("find", Find),
            new ("exit", Exit),
        };

        private static readonly string[][] HelpMessages =
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "stat", "prints the record count", "The 'stat' command prints the record count." },
            new string[] { "create", "allows you to create a new record", "The 'create' command allows you to create a new record." },
            new string[] { "edit", "allows you to edit an existing record", "The 'edit' command allows you to edit an existing record." },
            new string[] { "list", "prints the list of all records", "The 'list' command prints the list of all records." },
            new string[] { "find [PROPERTY]", "allows you to find all records with the given value of the property", "The 'find' command allows you to find all records with the given value of the property." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        private static bool isRunning = true;

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex].Split()[0], parameters.Split()[0], StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            bool isValid = false;
            do
            {
                Console.Write("First name: ");
                string firstName = Console.ReadLine();

                Console.Write("Last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Date of birth: ");
                DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);

                Console.Write("Sex: ");
                Enum.TryParse<Sex>(Console.ReadLine(), ignoreCase: true, out var sex);

                Console.Write("Budget (with currency sign): ");
                string budget = Console.ReadLine();

                char currency;
                decimal amount;
                if (char.IsDigit(budget[0]))
                {
                    currency = budget[^1];
                    decimal.TryParse(budget[..^1], NumberStyles.Currency, CultureInfo.InvariantCulture, out amount);
                }
                else
                {
                    currency = budget[0];
                    decimal.TryParse(budget[1..], NumberStyles.Currency, CultureInfo.InvariantCulture, out amount);
                }

                Console.Write("Kids count: ");
                short.TryParse(Console.ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture, out var kidsCount);

                try
                {
                    int id = FileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, sex, kidsCount, amount, currency);
                    Console.WriteLine($"Record #{id} is created.");
                    isValid = true;
                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine(exception.Message.Split('.')[0] + '.');
                }

                Console.WriteLine();
            }
            while (!isValid);
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
            {
                Console.WriteLine("Invalid id.");
                Console.WriteLine();
                return;
            }

            if (FileCabinetService.GetStat() < id)
            {
                Console.WriteLine($"#{id} record is not found.");
                Console.WriteLine();
                return;
            }

            bool isValid = false;
            do
            {
                Console.Write("First name: ");
                string firstName = Console.ReadLine();

                Console.Write("Last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Date of birth: ");
                DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);

                Console.Write("Sex: ");
                Enum.TryParse<Sex>(Console.ReadLine(), ignoreCase: true, out var sex);

                Console.Write("Budget (with currency sign): ");
                string budget = Console.ReadLine();

                char currency;
                decimal amount;
                if (char.IsDigit(budget[0]))
                {
                    currency = budget[^1];
                    decimal.TryParse(budget[..^1], NumberStyles.None, CultureInfo.InvariantCulture, out amount);
                }
                else
                {
                    currency = budget[0];
                    decimal.TryParse(budget[1..], NumberStyles.None, CultureInfo.InvariantCulture, out amount);
                }

                Console.Write("Kids count: ");
                short.TryParse(Console.ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture, out var kidsCount);

                try
                {
                    FileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, sex, kidsCount, amount, currency);
                    Console.WriteLine($"Record {id} is updated.");
                    isValid = true;
                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine(exception.Message.Split('.')[0] + '.');
                }

                Console.WriteLine();
            }
            while (!isValid);
        }

        private static void Find(string parameters)
        {
            var words = parameters.Split(' ', 2);

            if (words.Length < 2 || !words[1].StartsWith('\"') || !words[1].EndsWith('\"'))
            {
                Console.WriteLine("Invalid text to find. It must be in double quotes.");
                Console.WriteLine();
                return;
            }

            FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();
            if (words[0].Equals("FirstName", StringComparison.InvariantCultureIgnoreCase))
            {
                records = FileCabinetService.FindByFirstName(words[1][1..^1]);
            }
            else if (words[0].Equals("LastName", StringComparison.InvariantCultureIgnoreCase))
            {
                records = FileCabinetService.FindByLastName(words[1][1..^1]);
            }
            else if (words[0].Equals("DateOfBirth", StringComparison.InvariantCultureIgnoreCase))
            {
                DateTime.TryParse(words[1][1..^1], CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);
                records = FileCabinetService.FindByDateOfBirth(dateOfBirth);
            }

            if (records.Length == 0)
            {
                Console.WriteLine($"There is no record with such value of the {words[0].ToUpperInvariant()} property.");
            }
            else
            {
                foreach (var record in records)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {record.Sex}, has {record.KidsCount} kids, budget : {record.Currency}{record.Budget}");
                }
            }

            Console.WriteLine();
        }

        private static void List(string parameters)
        {
            var records = FileCabinetService.GetRecords();

            if (records.Length == 0)
            {
                Console.WriteLine("There is no records.");
            }
            else
            {
                foreach (var record in records)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {record.Sex}, has {record.KidsCount} kids, budget : {record.Currency}{record.Budget}");
                }
            }

            Console.WriteLine();
        }
    }
}