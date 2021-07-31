using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides console file cabinet application.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Gleb Lizhbanov";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
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
            new string[] { "find [PROPERTY]", "allows you to find all records with the given value of the property", $"The 'find' command allows you to find all records with the given value of the property.{Environment.NewLine}Valid properties : FirstName, LastName, DateOfBirth, Sex, KidsCount, Budget" },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        private static FileCabinetService fileCabinetService;
        private static bool isRunning = true;

        /// <summary>
        /// Application's entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Console.WriteLine(Resources.GreetingMessage, DeveloperName);
            if ((args.Length >= 2 && args[0] == "-v" && args[1].Equals("Custom", StringComparison.InvariantCultureIgnoreCase)) || (args.Length != 0 &&
                args[0].StartsWith("--validation-rules=", StringComparison.InvariantCulture) && args[0].EndsWith("--validation-rules=Custom", StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine(Resources.CustomValidationRulesMessage);
                fileCabinetService = new FileCabinetService(new CustomValidator());
            }
            else
            {
                Console.WriteLine(Resources.DefaultValidationRulesMessage);
                fileCabinetService = new FileCabinetService(new DefaultValidator());
            }

            Console.WriteLine(Resources.GetHelpHint);
            Console.WriteLine();

            do
            {
                Console.Write(Resources.TypeCommandSign);
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Resources.GetHelpHint);
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
            Console.WriteLine(Resources.InvalidCommandMessage, command);
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex].Split()[0], parameters.Split()[0], StringComparison.InvariantCultureIgnoreCase));
                Console.WriteLine(index >= 0
                    ? HelpMessages[index][Program.ExplanationHelpIndex]
                    : $"There is no explanation for '{parameters}' command.");
            }
            else
            {
                Console.WriteLine(Resources.AvailableCommandsMessage);

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine(Resources.HelpCommandDescriptionLine, helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine(Resources.ExitingMessage);
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine(Resources.RecordsCountMessage, recordsCount);
            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            bool isValid = false;
            do
            {
                Console.Write(Resources.FirstNameEnteringRequest);
                string firstName = Console.ReadLine();

                Console.Write(Resources.LastNameEnteringRequest);
                string lastName = Console.ReadLine();

                Console.Write(Resources.DateOfBirthEnteringRequest);
                DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);

                Console.Write(Resources.SexEnteringRequest);
                Enum.TryParse<Sex>(Console.ReadLine(), ignoreCase: true, out var sex);

                Console.Write(Resources.BudgetEnteringRequest);
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

                Console.Write(Resources.KidsCountEnteringRequest);
                short.TryParse(Console.ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture, out var kidsCount);

                try
                {
                    var record = new FileCabinetRecord()
                    {
                        Id = fileCabinetService.GetStat() + 1,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dateOfBirth,
                        Sex = sex,
                        KidsCount = kidsCount,
                        Budget = amount,
                        Currency = currency,
                    };

                    int id = fileCabinetService.CreateRecord(record);
                    isValid = true;
                    Console.WriteLine(Resources.RecordCreated, id);
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
            if (!int.TryParse(parameters, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id == 0)
            {
                Console.WriteLine(Resources.InvalidIDMessage);
                Console.WriteLine();
                return;
            }

            if (fileCabinetService.GetStat() < id)
            {
                Console.WriteLine(Resources.RecordNotFound, id);
                Console.WriteLine();
                return;
            }

            bool isValid = false;
            do
            {
                Console.Write(Resources.FirstNameEnteringRequest);
                string firstName = Console.ReadLine();

                Console.Write(Resources.LastNameEnteringRequest);
                string lastName = Console.ReadLine();

                Console.Write(Resources.DateOfBirthEnteringRequest);
                DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);

                Console.Write(Resources.SexEnteringRequest);
                Enum.TryParse<Sex>(Console.ReadLine(), ignoreCase: true, out var sex);

                Console.Write(Resources.BudgetEnteringRequest);
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

                Console.Write(Resources.KidsCountEnteringRequest);
                short.TryParse(Console.ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture, out var kidsCount);

                try
                {
                    var record = new FileCabinetRecord
                    {
                        Id = id,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dateOfBirth,
                        Sex = sex,
                        KidsCount = kidsCount,
                        Budget = amount,
                        Currency = currency,
                    };

                    fileCabinetService.EditRecord(id, record);
                    Console.WriteLine(Resources.RecordUpdated, id);
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
                Console.WriteLine(Resources.InvalidValueToSearchMessage);
                Console.WriteLine();
                return;
            }

            bool propertyIsValid = true;
            FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();
            if (words[0].Equals("FirstName", StringComparison.InvariantCultureIgnoreCase))
            {
                records = fileCabinetService.FindByFirstName(words[1][1..^1]);
            }
            else if (words[0].Equals("LastName", StringComparison.InvariantCultureIgnoreCase))
            {
                records = fileCabinetService.FindByLastName(words[1][1..^1]);
            }
            else if (words[0].Equals("DateOfBirth", StringComparison.InvariantCultureIgnoreCase))
            {
                DateTime.TryParse(words[1][1..^1], CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);
                records = fileCabinetService.FindByDateOfBirth(dateOfBirth);
            }
            else if (words[0].Equals("Sex", StringComparison.InvariantCultureIgnoreCase))
            {
                Enum.TryParse<Sex>(words[1][1..^1], ignoreCase: true, out var sex);
                records = fileCabinetService.FindBySex(sex);
            }
            else if (words[0].Equals("KidsCount", StringComparison.InvariantCultureIgnoreCase))
            {
                short.TryParse(words[1][1..^1], NumberStyles.None, CultureInfo.InvariantCulture, out var kidsCount);
                records = fileCabinetService.FindByKidsCount(kidsCount);
            }
            else if (words[0].Equals("Budget", StringComparison.InvariantCultureIgnoreCase))
            {
                char currency;
                decimal amount;
                if (char.IsDigit(words[1][1]))
                {
                    currency = words[1][^2];
                    decimal.TryParse(words[1][1..^2], NumberStyles.None, CultureInfo.InvariantCulture, out amount);
                }
                else if (char.IsDigit(words[1][^2]))
                {
                    currency = words[1][1];
                    decimal.TryParse(words[1][2..^1], NumberStyles.None, CultureInfo.InvariantCulture, out amount);
                }
                else
                {
                    amount = default;
                    currency = default;
                }

                records = fileCabinetService.FindByBudget(amount, currency);
            }
            else
            {
                propertyIsValid = false;
            }

            if (!propertyIsValid)
            {
                Console.WriteLine(Resources.InvalidPropertyMessage, words[0].ToUpperInvariant());
            }
            else if (records.Length == 0)
            {
                Console.WriteLine(Resources.PropertyValueNotFoundMessage, words[0].ToUpperInvariant());
            }
            else
            {
                foreach (var record in records)
                {
                    Console.WriteLine(Resources.PersonalData, record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), record.Sex, record.KidsCount, record.Currency, record.Budget);
                }
            }

            Console.WriteLine();
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();

            if (records.Length == 0)
            {
                Console.WriteLine(Resources.NoRecordsMessage);
            }
            else
            {
                foreach (var record in records)
                {
                    Console.WriteLine(Resources.PersonalData, record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), record.Sex, record.KidsCount, record.Currency, record.Budget);
                }
            }

            Console.WriteLine();
        }
    }
}