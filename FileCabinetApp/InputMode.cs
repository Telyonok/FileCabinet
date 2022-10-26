using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Input mode for <c>GetPlayerInputAndRecord</c> method.
    /// </summary>
    internal enum InputMode
    {
        /// <summary>
        /// Represents 'Create' mode.
        /// </summary>
        Create,

        /// <summary>
        /// Represents 'Edit' mode.
        /// </summary>
        Edit,
    }
}
