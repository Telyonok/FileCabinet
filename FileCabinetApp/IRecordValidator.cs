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
        /// Returns service name.
        /// </summary>
        /// <returns>Service name.</returns>
        public string GetValidatorName();

        /// <summary>
        /// Validates name parameter.
        /// </summary>
        /// <param name="name">Person's name.</param>
        /// <returns>Tuple, where first item indicates if validation is succesful and second item is the name of validated parameter.</returns>
        public Tuple<bool, string> ValidateNameString(string name);

        /// <summary>
        /// Validates date of birth parameter.
        /// </summary>
        /// <param name="dateTime">Person's birth day.</param>
        /// <returns>Tuple, where first item indicates if validation is succesful and second item is the name of validated parameter.</returns>
        public Tuple<bool, string> ValidateDateTime(DateTime dateTime);

        /// <summary>
        /// Validates sex parameter.
        /// </summary>
        /// <param name="sex">Person's sex.</param>
        /// <returns>Tuple, where first item indicates if validation is succesful and second item is the name of validated parameter.</returns>
        public Tuple<bool, string> ValidateSex(char sex);

        /// <summary>
        /// Validates weight parameter.
        /// </summary>
        /// <param name="weight">Person's weight in kg.</param>
        /// <returns>Tuple, where first item indicates if validation is succesful and second item is the name of validated parameter.</returns>
        public Tuple<bool, string> ValidateWeight(short weight);

        /// <summary>
        /// Validates height parameter.
        /// </summary>
        /// <param name="height">Person's height in cm.</param>
        /// <returns>Tuple, where first item indicates if validation is succesful and second item is the name of validated parameter.</returns>
        public Tuple<bool, string> ValidateHeight(decimal height);
    }
}
