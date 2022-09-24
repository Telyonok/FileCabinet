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
        protected override void ValidateInput(UnvalidatedRecordData unvalidatedRecord)
        {
            this.ValidateNameString(unvalidatedRecord.FirstName);
            this.ValidateNameString(unvalidatedRecord.LastName);
            this.ValidateDateTime(unvalidatedRecord.DateOfBirth);
            this.ValidateSex(unvalidatedRecord.Sex);
            this.ValidateWeight(unvalidatedRecord.Weight);
            this.ValidateHeight(unvalidatedRecord.Height);
        }

        /// <inheritdoc/>
        protected override void ValidateNameString(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length < 2 || name.Length > 60 || name.Trim() == string.Empty)
            {
                throw new ArgumentException("String must be 2 to 60 characters long and consist not only of whitespaces", nameof(name));
            }
        }

        /// <inheritdoc/>
        protected override void ValidateDateTime(DateTime dateTime)
        {
            var marginDate = new DateTime(year: 1950, month: 1, day: 1);
            if (dateTime.CompareTo(DateTime.Now) > 0 || dateTime.CompareTo(marginDate) < 0)
            {
                throw new ArgumentException("Incorrect Date of birth");
            }
        }

        /// <inheritdoc/>
        protected override void ValidateSex(char sex)
        {
            if (!char.IsLetter(sex))
            {
                throw new ArgumentException("Sex should be a letter!");
            }
        }

        /// <inheritdoc/>
        protected override void ValidateWeight(short weight)
        {
            if (weight < 0 || weight > 300)
            {
                throw new ArgumentException("Weight must be from 0 to 300");
            }
        }

        /// <inheritdoc/>
        protected override void ValidateHeight(decimal height)
        {
            if (height < 0 || height > 300)
            {
                throw new ArgumentException("Height must be from 0 to 300");
            }
        }
    }
}
