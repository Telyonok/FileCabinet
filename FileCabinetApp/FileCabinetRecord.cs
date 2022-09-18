namespace FileCabinetApp
{
    /// <summary>
    /// Class <c>FileCabinetRecord</c> stores information about a person.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets id value.
        /// </summary>
        /// <value>
        /// id represents record's identification number.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets firstName value.
        /// </summary>
        /// <value>
        /// firstName represents person's first name.
        /// </value>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets lastName value.
        /// </summary>
        /// <value>
        /// lastName represents person's last name.
        /// </value>
        public string? LastName { get; set; }

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