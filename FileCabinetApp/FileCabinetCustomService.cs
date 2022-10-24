using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Defines custom service validation rules.
    /// </summary>
    internal class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCustomService"/> class.
        /// </summary>
        public FileCabinetCustomService()
            : base(new CustomValidator())
        {
        }

        /// <inheritdoc/>
        public override string GetServiceName()
        {
            return "custom";
        }
    }
}
