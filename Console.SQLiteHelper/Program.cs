//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Lifeprojects.de">
//     Class: Program
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>12.05.2025 07:18:13</date>
//
// <summary>
// Konsolen Applikation mit Menü
// </summary>
//-----------------------------------------------------------------------

namespace Console.SQLiteHelper
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;

    public class Program
    {
        private static string databasePath = string.Empty;

        private static void Main(string[] args)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            databasePath = Path.Combine(new DirectoryInfo(currentDirectory).Parent.Parent.Parent.FullName, "_DemoData", "DemoDatabase.db");

            do
            {
                Console.Clear();
                Console.WriteLine("1. Create Database and Table");
                Console.WriteLine("2. Database Info");
                Console.WriteLine("3. Metadata Information");
                Console.WriteLine("X. Beenden");

                Console.WriteLine("Wählen Sie einen Menüpunkt oder 'x' für beenden");
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.X)
                {
                    Environment.Exit(0);
                }
                else
                {
                    if (key == ConsoleKey.D1)
                    {
                        MenuPoint1();
                    }
                    else if (key == ConsoleKey.D2)
                    {
                        MenuPoint2();
                    }
                    else if (key == ConsoleKey.D3)
                    {
                        MenuPoint3();
                    }
                }
            }
            while (true);
        }

        private static void MenuPoint1()
        {
            Console.Clear();

            if (File.Exists(databasePath) == true)
            {
                File.Delete(databasePath);
            }

            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                ds.Create(CreateTableInDB);
            }

            if (File.Exists(databasePath) == true)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde erstellt!!");
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPoint2()
        {
            Console.Clear();

            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            DataTable dbInfo = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                dbInfo = ds.GetSchema(TableSchemaTyp.Tables);
            }

            foreach (DataRow row in dbInfo.Rows)
            {
                Console.WriteLine($"Name={row[2]}");
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPoint3()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            List<Tuple<string, string, object, Type>> dbInfo = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                dbInfo = ds.MetadataInformation();
            }

            foreach (Tuple<string, string, object, Type> row in dbInfo)
            {
                Console.WriteLine($"Key={row.Item1}; Value={row.Item3}");
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void CreateTableInDB(SQLiteConnection sqliteConnection)
        {
            string sqlText = "CREATE TABLE IF NOT EXISTS TAB_Contact \r\n(\r\nId VARCHAR(36),\r\nName VARCHAR(50),\r\nAge Integer,\r\nBirthday DateTime\r\n, PRIMARY KEY \r\n(\r\nId\r\n))";
            sqliteConnection.CmdExecuteNonQuery(sqlText);
        }
    }
}
