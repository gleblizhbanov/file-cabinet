using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

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
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static readonly string[][] HelpMessages =
        {
            new string[]
            {
                "help",
                "prints the help screen",
                "The 'help' command prints the help screen.",
            },
            new string[]
            {
                "stat",
                "prints the record count",
                "The 'stat' command prints the record count.",
            },
            new string[]
            {
                "create",
                "allows you to create a new record",
                "The 'create' command allows you to create a new record.",
            },
            new string[]
            {
                "edit",
                "allows you to edit an existing record",
                "The 'edit' command allows you to edit an existing record.",
            },
            new string[]
            {
                "list",
                "prints the list of all records",
                "The 'list' command prints the list of all records.",
            },
            new string[]
            {
                "find [PROPERTY]",
                "allows you to find all records with the given value of the property",
                $"The 'find' command allows you to find all records with the given value of the property.{Environment.NewLine}Valid properties : FirstName, LastName, DateOfBirth, Sex, KidsCount, Budget",
            },
            new string[]
            {
                "export [FILETYPE]",
                "exports the current cabinet state into a file of FILETYPE type.",
                $"The 'export' command exports the current cabinet state into a file of FILETYPE type.{Environment.NewLine}Valid file types: CSV",
            },
            new string[]
            {
                "exit",
                "exits the application",
                "The 'exit' command exits the application.",
            },
        };

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator validator;
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
            if ((args.Length >= 2 && args[0] == "-v" &&
                 args[1].Equals("Custom", StringComparison.InvariantCultureIgnoreCase)) || (args.Length != 0 &&
                args[0].StartsWith("--validation-rules=", StringComparison.InvariantCulture) && args[0]
                    .EndsWith("--validation-rules=Custom", StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine(Resources.CustomValidationRulesMessage);
                validator = new CustomValidator();
            }
            else
            {
                Console.WriteLine(Resources.DefaultValidationRulesMessage);
                validator = new DefaultValidator();
            }

            fileCabinetService = new FileCabinetService();

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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex].Split()[0], parameters.Split()[0], StringComparison.InvariantCultureIgnoreCase));
                Console.WriteLine(index >= 0
                    ? HelpMessages[index][ExplanationHelpIndex]
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
            Console.Write(Resources.FirstNameEnteringRequest);
            string firstName = ReadInput(name => new Tuple<bool, string, string>(true, null, name), name => Validator(name, validator.ValidateName));

            Console.Write(Resources.LastNameEnteringRequest);
            string lastName = ReadInput(name => new Tuple<bool, string, string>(true, null, name), name => Validator(name, validator.ValidateName));

            Console.Write(Resources.DateOfBirthEnteringRequest);
            DateTime dateOfBirth = ReadInput(DateConverter, date => Validator(date, validator.ValidateDateOfBirth));

            Console.Write(Resources.SexEnteringRequest);
            Sex sex = ReadInput(SexConverter, conversionResult => Validator(conversionResult, validator.ValidateSex));

            Console.Write(Resources.BudgetEnteringRequest);
            Tuple<char, decimal> budget = ReadInput(BudgetConverter, BudgetValidator);

            Console.Write(Resources.KidsCountEnteringRequest);
            short kidsCount = ReadInput(CountConverter, count => Validator(count, validator.ValidateCount));

            var record = new FileCabinetRecord()
            {
                Id = fileCabinetService.GetStat() + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Sex = sex,
                KidsCount = kidsCount,
                Budget = budget.Item2,
                Currency = budget.Item1,
            };

            int id = fileCabinetService.CreateRecord(record);

            Console.WriteLine(Resources.RecordCreated, id);
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

            Console.Write(Resources.FirstNameEnteringRequest);
            string firstName = ReadInput(name => new Tuple<bool, string, string>(true, null, name), name => Validator(name, validator.ValidateName));

            Console.Write(Resources.LastNameEnteringRequest);
            string lastName = ReadInput(name => new Tuple<bool, string, string>(true, null, name), name => Validator(name, validator.ValidateName));

            Console.Write(Resources.DateOfBirthEnteringRequest);
            DateTime dateOfBirth = ReadInput(DateConverter, date => Validator(date, validator.ValidateDateOfBirth));

            Console.Write(Resources.SexEnteringRequest);
            Sex sex = ReadInput(SexConverter, conversionResult => Validator(conversionResult, validator.ValidateSex));

            Console.Write(Resources.BudgetEnteringRequest);
            Tuple<char, decimal> budget = ReadInput(BudgetConverter, BudgetValidator);

            Console.Write(Resources.KidsCountEnteringRequest);
            short kidsCount = ReadInput(CountConverter, count => Validator(count, validator.ValidateCount));

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Sex = sex,
                KidsCount = kidsCount,
                Budget = budget.Item2,
                Currency = budget.Item1,
            };

            fileCabinetService.EditRecord(id, record);
            Console.WriteLine(Resources.RecordUpdated, id);
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
            ReadOnlyCollection<FileCabinetRecord> records = null;
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
            else if (records is null || records.Count == 0)
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

            if (records.Count == 0)
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

        private static void Export(string parameters)
        {
            var parametersArray = parameters.Split(' ', 2);

            if (!parametersArray[0].Equals("CSV", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine(Resources.InvalidFileType);
                Console.WriteLine();
                return;
            }

            if (parametersArray.Length < 2)
            {
                Console.WriteLine(Resources.InvalidFilePath);
                Console.WriteLine();
                return;
            }

            string filePath = parametersArray[1];
            if (File.Exists(filePath))
            {
                Console.Write(Resources.FileExistsRewriteRequest, filePath);
                bool? rewrite = ReadInput(AnswerConverter, answer => answer is not null
                            ? new Tuple<bool, string>(true, null)
                            : new Tuple<bool, string>(false, string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "answer")));

                if (rewrite is not null && !rewrite.Value)
                {
                    return;
                }
            }

            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(filePath, append: false);
            }
            catch (IOException)
            {
                Console.WriteLine(Resources.CannotOpenFile, filePath);
                Console.WriteLine();
                return;
            }
            finally
            {
                writer?.Dispose();
            }

            var snapshot = fileCabinetService.MakeSnapshot();
            snapshot.SaveToCsv(writer);
            writer.Close();

            Console.WriteLine();
            Console.WriteLine(Resources.SuccessfulExportMessage, filePath);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine(Resources.ConversionFailedMessage, conversionResult.Item2);
                    continue;
                }

                T value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine(Resources.ValidationFailedMessage, validationResult.Item2);
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string> Validator<T>(T parameter, Action<T> parameterValidator)
        {
            try
            {
                parameterValidator(parameter);
            }
            catch (ArgumentException exception)
            {
                return new Tuple<bool, string>(false, exception.Message[..exception.Message.IndexOf(" (Parameter", StringComparison.InvariantCulture)]);
            }

            return new Tuple<bool, string>(true, null);
        }

        private static Tuple<bool, string, DateTime> DateConverter(string stringDate)
        {
            if (DateTime.TryParse(stringDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
            {
                return new Tuple<bool, string, DateTime>(true, null, dateOfBirth);
            }

            return new Tuple<bool, string, DateTime>(false, string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "date of birth"), dateOfBirth);
        }

        private static Tuple<bool, string, Sex> SexConverter(string sexString)
        {
            if (Enum.TryParse<Sex>(sexString, ignoreCase: true, out var sex))
            {
                return new Tuple<bool, string, Sex>(true, null, sex);
            }

            return new Tuple<bool, string, Sex>(false, string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsInvalidMessage, "sex"), sex);
        }

        private static Tuple<bool, string, Tuple<char, decimal>> BudgetConverter(string budgetString)
        {
            char currency;
            string message;
            bool isSuccessful = false;
            if ((!char.IsDigit(budgetString[0]) && decimal.TryParse(budgetString[1..], NumberStyles.None, CultureInfo.InvariantCulture, out var amountOfMoney)) ||
                (!char.IsDigit(budgetString[^1]) && decimal.TryParse(budgetString[..^1], NumberStyles.None, CultureInfo.InvariantCulture, out amountOfMoney)))
            {
                message = null;
                currency = budgetString.Single(i => !char.IsDigit(i));
                isSuccessful = true;
            }
            else
            {
                message = "Invalid budget format";
                currency = default;
                amountOfMoney = default;
            }

            return new Tuple<bool, string, Tuple<char, decimal>>(isSuccessful, message, new Tuple<char, decimal>(currency, amountOfMoney));
        }

        private static Tuple<bool, string, short> CountConverter(string input)
        {
            if (short.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out var count))
            {
                return new Tuple<bool, string, short>(true, null, count);
            }

            return new Tuple<bool, string, short>(false, "Invalid count", count);
        }

        private static Tuple<bool, string> BudgetValidator(Tuple<char, decimal> budget)
        {
            try
            {
                validator.ValidateCurrency(budget.Item1);
                validator.ValidateCount(budget.Item2);
            }
            catch (ArgumentException exception)
            {
                return new Tuple<bool, string>(false, exception.Message[..exception.Message.IndexOf(" (Parameter", StringComparison.InvariantCulture)]);
            }

            return new Tuple<bool, string>(true, null);
        }

        private static Tuple<bool, string, bool?> AnswerConverter(string answer)
        {
            if (answer.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) ||
                answer.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return new Tuple<bool, string, bool?>(true, null, true);
            }

            if (answer.Equals("No", StringComparison.InvariantCultureIgnoreCase) ||
                answer.Equals("N", StringComparison.InvariantCultureIgnoreCase))
            {
                return new Tuple<bool, string, bool?>(true, null, false);
            }

            return new Tuple<bool, string, bool?>(true, string.Format(CultureInfo.InvariantCulture, Resources.ConversionFailedMessage, "The answer is not ('yes' / 'y') or ('no' / 'n')."), null);
        }
    }
}