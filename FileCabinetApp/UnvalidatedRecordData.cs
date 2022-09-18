using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Class <c>UnvalidatedRecordData</c> stores unvalidated information about a person.
    /// </summary>
    public class UnvalidatedRecordData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnvalidatedRecordData"/> class.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <param name="lastName">Person's last name.</param>
        /// <param name="sex">Person's sex.</param>
        /// <param name="weight">Person's weight.</param>
        /// <param name="height">Persons's height.</param>
        /// <param name="dateOfBirth">Person's date of birth.</param>
        public UnvalidatedRecordData(string firstName, string lastName, char sex, short weight, decimal height, DateTime dateOfBirth)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Sex = sex;
            this.Weight = weight;
            this.Height = height;
            this.DateOfBirth = dateOfBirth;
        }

        /// <summary>
        /// Gets or sets firstName value.
        /// </summary>
        /// <value>
        /// firstName represents person's first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets lastName value.
        /// </summary>
        /// <value>
        /// lastName represents person's last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets sex value.
        /// </summary>
        /// <value>
        /// sex represents person's sex.
        /// </value>
        public char Sex { get; set; }

        /// <summary>
        /// Gets or sets weight value.
        /// </summary>
        /// <value>
        /// weight represents person's weight in kilograms.
        /// </value>
        public short Weight { get; set; }

        /// <summary>
        /// Gets or sets height value.
        /// </summary>
        /// <value>
        /// height represents person's height in cm.
        /// </value>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets dateOfBirth value.
        /// </summary>
        /// <value>
        /// dateOfBirth represents person's date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }
    }
}