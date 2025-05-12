//-----------------------------------------------------------------------
// <copyright file="Contact.cs" company="Lifeprojects.de">
//     Class: Program
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>07.05.2025 14:27:39</date>
//
// <summary>
// Model Klasse zu Contact
// </summary>
//-----------------------------------------------------------------------

namespace Console.SQLiteHelper
{
    using System;

    public class Contact
    {
        public Contact(string name, DateTime birthday)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Birthday = birthday;
            this.Age = this.GetAge(birthday);
        }

        public Guid Id { get; set; }

        public string Name { get; }

        public int Age { get; }

        public DateTime Birthday { get; }

        private int GetAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

            return (a - b) / 10000;
        }
    }
}
