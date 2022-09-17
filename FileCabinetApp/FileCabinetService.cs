﻿namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, char sex, short weight, decimal height, DateTime dateOfBirth)
        {
            ValidateInput(firstName, lastName, sex, weight, height, dateOfBirth);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                Sex = sex,
                Weight = weight,
                Height = height,
                DateOfBirth = dateOfBirth,
            };

            this.list.Add(record);

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, char sex, short weight, decimal height, DateTime dateOfBirth)
        {
            ValidateInput(firstName, lastName, sex, weight, height, dateOfBirth);
            this.list[id - 1].FirstName = firstName;
            this.list[id - 1].LastName = lastName;
            this.list[id - 1].Sex = sex;
            this.list[id - 1].Weight = weight;
            this.list[id - 1].Height = height;
            this.list[id - 1].DateOfBirth = dateOfBirth;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            var records = from rec in this.list
                          where string.Equals(rec.FirstName, firstName, StringComparison.OrdinalIgnoreCase)
                          select rec;
            return records.ToArray();
        }

        private static void ValidateNameString(string name)
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

        private static void ValidateDateTime(DateTime dateTime)
        {
            var marginDate = new DateTime(year: 1950, month: 1, day: 1);
            if (dateTime.CompareTo(DateTime.Now) > 0 || dateTime.CompareTo(marginDate) < 0)
            {
                throw new ArgumentException("Incorrect Date of birth");
            }
        }

        private static void ValidateSex(char sex)
        {
            if (!char.IsLetter(sex))
            {
                throw new ArgumentException("Sex should be a letter!");
            }
        }

        private static void ValidateWeight(short weight)
        {
            if (weight < 0 || weight > 300)
            {
                throw new ArgumentException("Weight must be from 0 to 300");
            }
        }

        private static void ValidateHeight(decimal height)
        {
            if (height < 0 || height > 300)
            {
                throw new ArgumentException("Height must be from 0 to 300");
            }
        }

        private static void ValidateInput(string firstName, string lastName, char sex, short weight, decimal height, DateTime dateOfBirth)
        {
            ValidateNameString(firstName);
            ValidateNameString(lastName);
            ValidateDateTime(dateOfBirth);
            ValidateSex(sex);
            ValidateWeight(weight);
            ValidateHeight(height);
        }
    }
}