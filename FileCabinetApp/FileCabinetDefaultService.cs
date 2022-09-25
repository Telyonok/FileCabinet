using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Defines default service validation rules.
    /// </summary>
    internal class FileCabinetDefaultService : FileCabinetService
    {
        /// <inheritdoc/>
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }

        /// <inheritdoc/>
        public override string GetServiceName()
        {
            return "default";
        }
    }
}
