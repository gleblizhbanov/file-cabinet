using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates incoming parameters.
        /// </summary>
        /// <param name="record">Record parameter to validate.</param>
        void ValidateParameters(FileCabinetRecord record);
    }
}
