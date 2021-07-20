using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            // TODO: Implement this method.
            return 0;
        }

        public FileCabinetRecord[] GetRecords()
        {
            // TODO: Implement this method.
            return new FileCabinetRecord[] { };
        }

        public int GetStat()
        {
            // TODO: Implement this method.
            return 0;
        }
    }
}
