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
    using System.ComponentModel;
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
                Console.WriteLine("4. Insert one Row");
                Console.WriteLine("5. Select mit verschiedenen Rückgabetypen");
                Console.WriteLine("6. Select ohne konkretes Ergebnis");
                Console.WriteLine("7. Update by Scalare");
                Console.WriteLine("8. Löschen eines Eintrages");
                Console.WriteLine("9. Neues DataRow erstellen");
                Console.WriteLine("A. Seletct mit Dictionary<string,object> Parameter");
                Console.WriteLine("B. Seletct mit SQLiteParameter");
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
                    else if (key == ConsoleKey.D4)
                    {
                        MenuPoint4();
                    }
                    else if (key == ConsoleKey.D5)
                    {
                        MenuPoint5();
                    }
                    else if (key == ConsoleKey.D6)
                    {
                        MenuPoint6();
                    }
                    else if (key == ConsoleKey.D7)
                    {
                        MenuPoint7();
                    }
                    else if (key == ConsoleKey.D8)
                    {
                        MenuPoint8();
                    }
                    else if (key == ConsoleKey.D9)
                    {
                        MenuPoint9();
                    }
                    else if (key == ConsoleKey.A)
                    {
                        MenuPointA();
                    }
                    else if (key == ConsoleKey.B)
                    {
                        MenuPointB();
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

        private static void MenuPoint4()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                ds.Insert(InsertNewRow);
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPoint5()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            SQLiteConnection connection = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                connection = ds.OpenConnection();
                string sql = "SELECT Id, Name, Birthday, Age FROM TAB_Contact";
                DataTable dtSelect = connection.RecordSet<DataTable>(sql).Get().Result;

                sql = "SELECT \r\nId, Name, Birthday, Age \r\nFROM TAB_Contact \r\nWHERE (Age = '64') \r\nAND (Name = 'Gerhard')";
                DataTable dtSeletWhere = connection.RecordSet<DataTable>(sql).Get().Result;

                sql = "SELECT \r\nId, Name, Birthday, Age \r\nFROM TAB_Contact\r\nLIMIT 2";
                DataTable dtSeletLimit = connection.RecordSet<DataTable>(sql).Get().Result;

                sql = "SELECT Id, Name, Birthday, Age FROM TAB_Contact LIMIT 1";
                DataRow selectLimitDataRow = connection.RecordSet<DataRow>(sql).Get().Result;

                /*
                sql = "SELECT Age, Name FROM TAB_Contact LIMIT 1";
                Dictionary<int,string> selectLimitDictA = connection.RecordSet<Dictionary<int, string>>(sql).Get().Result;

                sql = "SELECT Id, Name FROM TAB_Contact LIMIT 1";
                Dictionary<Guid, string> selectLimitDictB = connection.RecordSet<Dictionary<Guid, string>>(sql).Get().Result;

                sql = "SELECT Name, upper(Name) FROM TAB_Contact LIMIT 1";
                Dictionary<string, string> selectLimitDictC = connection.RecordSet<Dictionary<string, string>>(sql).Get().Result;
                */
                sql = "SELECT Name, upper(Name) FROM TAB_Contact LIMIT 1";
                Dictionary<string, object> selectLimitDictD = connection.RecordSet<Dictionary<string, object>>(sql).Get().Result;

                ds.CloseConnection();
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPoint6()
        {
            string sqlStatement = string.Empty;

            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            SQLiteConnection connection = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                connection = ds.OpenConnection();

                /* Beispiel wenn keine Daten gefunden werden */
                sqlStatement = "SELECT \r\nId, Name, Birthday, Age \r\nFROM TAB_Contact \r\nWHERE (Age = '65') \r\nAND (Name = 'Gerhard')";
                DataTable selectWhereFalseDataTable = connection.RecordSet<DataTable>(sqlStatement).Get().Result;

                sqlStatement = "SELECT Id, Name, Birthday, Age FROM TAB_Contact WHERE (Age = '65') AND (Name = 'Gerhard') LIMIT 1";
                DataRow SelectWhereFalseDataRow = connection.RecordSet<DataRow>(sqlStatement).Get().Result;

                sqlStatement = "SELECT \r\nId, Name, Birthday, Age \r\nFROM TAB_Contact \r\nWHERE (Age = '65') \r\nAND (Name = 'Gerhard')";
                ICollectionView dtSeletWhereICollectionView = connection.RecordSet<ICollectionView>(sqlStatement).Get().Result;

                sqlStatement = "SELECT Max(Age) FROM TAB_Contact WHERE (Age = '65') AND (Name = 'Gerhard')";
                int dtSeletWhereMax = connection.RecordSet<int>(sqlStatement).Get().Result;

                ds.CloseConnection();
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPoint7()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            SQLiteConnection connection = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                connection = ds.OpenConnection();

                string sql = "UPDATE TAB_Contact SET Age = 65 WHERE Id = 'c8487801-19d4-41f9-901a-a56768d68e9b'";
                int countUpdate = connection.RecordSet<int>(sql).Set().Result;

                ds.CloseConnection();
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPoint8()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            SQLiteConnection connection = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                connection = ds.OpenConnection();

                string sql = "DELETE TAB_Contact WHERE Id = 'c8487801-19d4-41f9-901a-a56768d68e9b'";
                int countUpdate = connection.RecordSet<int>(sql).Set().Result;

                ds.CloseConnection();
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPoint9()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            SQLiteConnection connection = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                connection = ds.OpenConnection();

                DataRow newRow = connection.RecordSet<DataRow>("TAB_Contact").New().Result;

                ds.CloseConnection();
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPointA()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            SQLiteConnection connection = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                connection = ds.OpenConnection();

                Dictionary<string,object> paramCollection = new Dictionary<string,object>();
                paramCollection.Add(":Age", 64);
                paramCollection.Add(":Name", "Gerhard");

                string sql = "SELECT \r\nId, Name, Birthday, Age \r\nFROM TAB_Contact \r\nWHERE (Age = :Age) \r\nAND (Name = :Name)";
                DataTable result = connection.RecordSet<DataTable>(sql, paramCollection).Get().Result;

                ds.CloseConnection();
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void MenuPointB()
        {
            Console.Clear();
            if (File.Exists(databasePath) == false)
            {
                Console.WriteLine($"Datenbank '{databasePath}' wurde noch nicht erstellt!!");
                Console.ReadKey();
                return;
            }

            SQLiteConnection connection = null;
            using (DatabaseService ds = new DatabaseService(databasePath))
            {
                connection = ds.OpenConnection();

                SQLiteParameter[] paramCollection = new SQLiteParameter[2];
                paramCollection[0] = new SQLiteParameter(":Age", 64);
                paramCollection[1] = new SQLiteParameter(":Name", "Gerhard");

                string sql = "SELECT \r\nId, Name, Birthday, Age \r\nFROM TAB_Contact \r\nWHERE (Age = :Age) \r\nAND (Name = :Name)";
                DataTable dtSelect = connection.RecordSet<DataTable>(sql, paramCollection).Get().Result;

                ds.CloseConnection();
            }

            Console.WriteLine("Eine Taste drücken für zurück zum Menü!");
            Console.ReadKey();
        }

        private static void InsertNewRow(SQLiteConnection sqliteConnection)
        {
            string sqlText = "INSERT INTO TAB_Contact (Id, Name, Birthday, Age) \r\nVALUES\r\n ('c8487801-19d4-41f9-901a-a56768d68e9b', 'Gerhard', '1960-06-28 00:00:00', '64')";
            sqliteConnection.RecordSet<int>(sqlText).Execute();

            sqlText = "INSERT INTO TAB_Contact (Id, Name, Birthday, Age) \r\nVALUES\r\n ('1f338a36-1730-41f6-94f9-15a5b105670f', 'Charlie', '1923-02-12 00:00:00', '2')";
            sqliteConnection.RecordSet<int>(sqlText).Execute();

            sqlText = "INSERT INTO TAB_Contact (Id, Name, Birthday, Age) \r\nVALUES\r\n ('5A202874-A649-4086-8BF3-AA8FEFD56F8A', 'Donald Duck', '1934-06-09 00:00:00', '91')";
            sqliteConnection.RecordSet<int>(sqlText).Execute();
        }

        private static void CreateTableInDB(SQLiteConnection sqliteConnection)
        {
            string sqlText = "CREATE TABLE IF NOT EXISTS TAB_Contact (Id VARCHAR(36),nName VARCHAR(50),Age Integer,Birthday DateTime, PRIMARY KEY (Id))";
            sqliteConnection.RecordSet<int>(sqlText).Execute();
        }
    }
}
