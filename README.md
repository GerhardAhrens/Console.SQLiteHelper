# SQLiteHelper: Klassen und Funktionen

![NET](https://img.shields.io/badge/NET-8.0-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![VS2022](https://img.shields.io/badge/Visual%20Studio-2022-white.svg)
![Version](https://img.shields.io/badge/Version-1.0.2025.0-yellow.svg)]

In diesem Repository sammle ich Klassen und Funktionen rund um die [SQLite Core Database](https://system.data.sqlite.org/home/doc/trunk/www/index.wiki).

# RecordSet

Diese Klasse soll die R�ckgabe von Datenbankergebnisse vereinfachen. Dazu kann ein Datentyp gew�hlt werden, in dem ich die Daten aus einer SQL Anweisung zur�ck bekommen will.
Mit Get() k�nnen Daten gelesen und mit Execute() k�nnen Wert auf die Datenbank geschrieben oder gel�scht werden.
Die Klasse RecordSet ist als Extension f�r den Type **SQLiteConnection** programmiert.</br>
Es mu� immer ein offenen Connection-Objekt vorhanden sein.

Lesen von Daten aus einer Tabelle
```csharp
string sql = "select last_number from user_sequences where sequence_NAME = 'MAIN'";
int result = connectionObject.RecordSet<int>(sql).Get().Result;
```

R�ckgabe eines DataTable
```csharp
string sql = "SELECT Id, Name, Birthday, Age FROM TAB_Contact";
DataTable result = connectionObject.RecordSet<DataTable>(sql).Get().Result;
```

R�ckgabe eines DataTable mit Parameter als Dictionary<string,object>()
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

R�ckgabe eines DataRow (f�r einen einzelnen Record, LIMIT 1)
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

L�schen eines Eintrages
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


M�gliche R�ckgabetypen (bei Get())

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

M�gliche �bergabetypen (bei Set() oder Execute())

| Typ  |
|:----------------|
| int, long         |
| decimal, double, float     |
| bool         |
| string|

# Extension
## SQLRecordSetExtension

## DataTableExtensions
