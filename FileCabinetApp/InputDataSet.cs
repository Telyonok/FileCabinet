using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Set of input data.
    /// </summary>
    public class InputDataSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputDataSet"/> class.
        /// </summary>
        /// <param name="firstName">Person's firstname.</param>
        /// <param name="lastName">Person's lastname.</param>
        /// <param name="sex">Person's sex.</param>
        /// <param name="weight">Person's weight in kg.</param>
        /// <param name="height">Person's height in cm.</param>
        /// <param name="dateOfBirth">Person's date of birth.</param>
        public InputDataSet(string firstName, string lastName, char sex, short weight, decimal height, DateTime dateOfBirth)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Sex = sex;
            this.Weight = weight;
            this.Height = height;
            this.DateOfBirth = dateOfBirth;
        }

        /// <summary>
        /// Gets Person's firstname.
        /// </summary>
        /// <value>
        /// Person's firstname.
        /// </value>
        public string FirstName { get; }

        /// <summary>
        /// Gets Person's lastname.
        /// </summary>
        /// <value>
        /// Person's lastname.
        /// </value>
        public string LastName { get; }

        /// <summary>
        /// Gets Person's sex.
        /// </summary>
        /// <value>
        /// Person's sex.
        /// </value>
        public char Sex { get; }

        /// <summary>
        /// Gets Person's weight in kg.
        /// </summary>
        /// <value>
        /// Person's weight.
        /// </value>
        public short Weight { get; }

        /// <summary>
        /// Gets Person's height in cm.
        /// </summary>
        /// <value>
        /// Person's height.
        /// </value>
        public decimal Height { get; }

        /// <summary>
        /// Gets Person's date of birth.
        /// </summary>
        /// <value>
        /// Person's date of birth.
        /// </value>
        public DateTime DateOfBirth { get; }
    }
}
