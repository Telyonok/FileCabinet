namespace FileCabinetApp
{
    /// <summary>
    /// Class <c>FileCabinetRecord</c> stores information about a person.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        public FileCabinetRecord()
        {
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="id">Record's id.</param>
        /// <param name="firstName">Person's firstname.</param>
        /// <param name="lastName">Person's lastname.</param>
        /// <param name="sex">Person's sex.</param>
        /// <param name="weight">Person's weight in kg.</param>
        /// <param name="height">Person's height in cm.</param>
        /// <param name="dateOfBirth">Person's date of birth.</param>
        public FileCabinetRecord(int id, string firstName, string lastName, char sex, short weight, decimal height, DateTime dateOfBirth)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Sex = sex;
            this.Weight = weight;
            this.Height = height;
            this.DateOfBirth = dateOfBirth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="id">Record's id.</param>
        /// <param name="dataSet">Input data set.</param>
        public FileCabinetRecord(int id, InputDataSet dataSet)
        {
            this.Id = id;
            this.FirstName = dataSet.FirstName;
            this.LastName = dataSet.LastName;
            this.Sex = dataSet.Sex;
            this.Weight = dataSet.Weight;
            this.Height = dataSet.Height;
            this.DateOfBirth = dataSet.DateOfBirth;
        }

        /// <summary>
        /// Gets id value.
        /// </summary>
        /// <value>
        /// id represents record's identification number.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets firstName value.
        /// </summary>
        /// <value>
        /// firstName represents person's first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets lastName value.
        /// </summary>
        /// <value>
        /// lastName represents person's last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets sex value.
        /// </summary>
        /// <value>
        /// sex represents person's sex.
        /// </value>
        public char Sex { get; set; }

        /// <summary>
        /// Gets weight value.
        /// </summary>
        /// <value>
        /// weight represents person's weight in kilograms.
        /// </value>
        public short Weight { get; set; }

        /// <summary>
        /// Gets height value.
        /// </summary>
        /// <value>
        /// height represents person's height in cm.
        /// </value>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets dateOfBirth value.
        /// </summary>
        /// <value>
        /// dateOfBirth represents person's date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }
    }
}