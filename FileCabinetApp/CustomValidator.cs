using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Validates UnvalidatedRecordData using custom methods.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public Tuple<bool, string> ValidateNameString(string name)
        {
            bool isValid = true;
            if (name is null)
            {
                isValid = false;
            }
            else if (name.Length < 1 || name.Length > 50 || name.Trim() == string.Empty)
            {
                isValid = false;
            }

            return Tuple.Create(isValid, "Name cannot be more than 50 digits and less than 1 digit");
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateDateTime(DateTime dateTime)
        {
            bool isValid = true;
            var marginDate = new DateTime(year: 1200, month: 1, day: 1);
            if (dateTime.CompareTo(DateTime.Now) > 0 || dateTime.CompareTo(marginDate) < 0)
            {
                isValid = false;
            }

            return Tuple.Create(isValid, "DateOfBirth cannot be earlier than 1200.1.1");
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateSex(char sex)
        {
            bool isValid = true;
            if (!char.IsLetter(sex) && !char.IsDigit(sex))
            {
                isValid = false;
            }

            return Tuple.Create(isValid, "Sex must be a letter or digit.");
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateWeight(short weight)
        {
            bool isValid = true;
            if (weight < 0 || weight > 450)
            {
                isValid = false;
            }

            return Tuple.Create(isValid, "Weight must be from 0 to 450");
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateHeight(decimal height)
        {
            bool isValid = true;
            if (height < 0 || height > 600)
            {
                isValid = false;
            }

            return Tuple.Create(isValid, "Height must be from 0 to 600");
        }

        /// <inheritdoc/>
        public string GetValidatorName()
        {
            return "custom";
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateRecord(FileCabinetRecord record)
        {
            if (!this.ValidateNameString(record.FirstName).Item1)
            {
                return this.ValidateNameString(record.FirstName);
            }
            else if (!this.ValidateNameString(record.LastName).Item1)
            {
                return this.ValidateNameString(record.LastName);
            }
            else if (!this.ValidateSex(record.Sex).Item1)
            {
                return this.ValidateSex(record.Sex);
            }
            else if (!this.ValidateWeight(record.Weight).Item1)
            {
                return this.ValidateWeight(record.Weight);
            }
            else if (!this.ValidateHeight(record.Height).Item1)
            {
                return this.ValidateHeight(record.Height);
            }
            else if (!this.ValidateDateTime(record.DateOfBirth).Item1)
            {
                return this.ValidateDateTime(record.DateOfBirth);
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}
