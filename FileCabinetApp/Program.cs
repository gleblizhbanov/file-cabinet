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
            new ("list", List),
            new ("create", Create),
            new ("exit", Exit),
        };

        private static readonly string[][] HelpMessages =
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "stat", "prints the record count", "The 'stat' command prints the record count." },
            new string[] { "list", "prints the list of all records", "The 'stat' command prints the list of all records." },
            new string[] { "create", "allows you to create a new record", "The 'create' command allows you to create a new record." },
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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
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
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string lastName = Console.ReadLine();

            Console.Write("Date of birth: ");
            DateTime dateOfBirth = DateTime.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

            Console.Write("Sex: ");
            Sex sex = Enum.Parse<Sex>(Console.ReadLine(), ignoreCase: true);

            Console.Write("Budget (with currency sign): ");
            string budget = Console.ReadLine();
            char currency = char.IsDigit(budget[0]) ? budget[^1] : budget[0];
            decimal amount = decimal.Parse(budget[1..], NumberStyles.Currency, CultureInfo.InvariantCulture);
            Console.Write("Kids count: ");
            short kidsCount = Convert.ToInt16(Console.ReadLine(), CultureInfo.InvariantCulture);

            int id = FileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, sex, kidsCount, amount, currency);
            Console.WriteLine($"Record #{id} is created.");
        }

        private static void List(string parameters)
        {
            var records = FileCabinetService.GetRecords();
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {record.Sex}, has {record.KidsCount} kids, budget : {record.Currency}{record.Budget}");
            }
        }
    }
}