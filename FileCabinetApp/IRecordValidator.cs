using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Validates UnvalidatedRecordData.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates unvalidateRecord's parameters.
        /// </summary>
        /// <param name="parameters">Parameters to validate.</param>
        public void ValidateParameters(UnvalidatedRecordData parameters);
    }
}
