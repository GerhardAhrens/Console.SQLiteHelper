# SQLiteHelper: Klassen und Funktionen

![NET](https://img.shields.io/badge/NET-8.0-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![VS2022](https://img.shields.io/badge/Visual%20Studio-2022-white.svg)
![Version](https://img.shields.io/badge/Version-1.0.2025.0-yellow.svg)]

In diesem Repository sammle ich Klassen und Funktionen rund um die [SQLite Core Database](https://system.data.sqlite.org/home/doc/trunk/www/index.wiki).

# RecordSet

Diese Klasse soll die Rückgabe von Datenbankergebnisse vereinfachen. Dazu kann ein Datentyp gewählt werden, in dem ich die Daten aus einer SQL Anweisung zurück bekommen will.
Mit Get() können Daten gelesen und mit Execute() können Wert auf die Datenbank geschrieben oder gelöscht werden.
Die Klasse RecordSet ist als Extension für den Type **SQLiteConnection** programmiert.</br>
Es muß immer ein offenen Connection-Objekt vorhanden sein.

Lesen von Daten aus einer Tabelle
```csharp
string sql = "select last_number from user_sequences where sequence_NAME = 'MAIN'";
int result = connectionObject.RecordSet<int>(sql).Get().Result;
```

Rückgabe eines DataTable
```csharp
string sql = "SELECT Id, Name, Birthday, Age FROM TAB_Contact";
DataTable result = connectionObject.RecordSet<DataTable>(sql).Get().Result;
```

Rückgabe eines DataTable mit Parameter als Dictionary<string,object>()
```csharp
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
```

Rückgabe eines DataRow (für einen einzelnen Record, LIMIT 1)
```csharp
string sql = "SELECT Id, Name, Birthday, Age FROM TAB_Contact LIMIT 1";
DataRow result = connectionObject.RecordSet<DataRow>(sql).Get().Result;
```

Vereinfachtes Schreiben eines einzelnen Wertes
```csharp
SQLiteConnection connection = null;
using (DatabaseService ds = new DatabaseService(databasePath))
{
    connection = ds.OpenConnection();

    string sql = "UPDATE TAB_Contact SET Age = 65 WHERE Id = 'c8487801-19d4-41f9-901a-a56768d68e9b'";
    int countUpdate = ConnectionObject RecordSet<int>(sql).Execute().Result;

    ds.CloseConnection();
}
```

Löschen eines Eintrages
```csharp
SQLiteConnection connection = null;
using (DatabaseService ds = new DatabaseService(databasePath))
{
    connection = ds.OpenConnection();

    string sql = "DELETE TAB_Contact WHERE Id = 'c8487801-19d4-41f9-901a-a56768d68e9b'";
    int countDelete = ConnectionObject.RecordSet<int>(sql).Execute().Result;

    ds.CloseConnection();
}
```
Neues DataRow erstellen
```csharp
SQLiteConnection connection = null;
using (DatabaseService ds = new DatabaseService(databasePath))
{
    connection = ds.OpenConnection();

    DataRow newRow = connection.RecordSet<DataRow>("TAB_Contact").New().Result;

    ds.CloseConnection();
}
```


Mögliche Rückgabetypen (bei Get())

| Typ  |
|:----------------|
| DataRow         |
| DataTable       |
| ICollectionView |
| List\<T> |
| Dictionary\<,> |
| string |
| DateTime |
| bool |
| int,long |
| decimal,double, float, Single|

Mögliche Übergabetypen (bei Set() oder Execute())

| Typ  |
|:----------------|
| int, long         |
| decimal, double, float     |
| bool         |
| string|

# Extension
## SQLRecordSetExtension

## DataTableExtensions
