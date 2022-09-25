using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Defines custom service validation rules.
    /// </summary>
    internal class FileCabinetCustomService : FileCabinetService
    {
        /// <inheritdoc/>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }

        /// <inheritdoc/>
        public override string GetServiceName()
        {
            return "custom";
        }
    }
}
