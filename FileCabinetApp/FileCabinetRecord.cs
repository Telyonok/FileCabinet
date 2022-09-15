namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public char Sex { get; set; }

        public short Weight { get; set; }

        public decimal Height { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}