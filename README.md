# SQLiteHelper: Klassen und Funktionen

![NET](https://img.shields.io/badge/NET-8.0-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![VS2022](https://img.shields.io/badge/Visual%20Studio-2022-white.svg)
![Version](https://img.shields.io/badge/Version-1.0.2025.0-yellow.svg)]

In diesem Repository sammle ich Klassen und Funktionen rund um die [SQLite Core Database](https://system.data.sqlite.org/home/doc/trunk/www/index.wiki).

# RecordSet

Diese Klasse soll die Rückgabe von Datenbankergebnisse vereinfachen. Dazu kann ein Datentyp gewählt werden, in dem ich die Daten aus einer SQL Anweisung zurück bekommen will.
Mit Get() können Daten gelesen und mit Set() kann ein einzelner Wert auf die Datenbank geschrieben werden.
```csharp
int result = 0;

using (ConnectionRepository repository = new ConnectionRepository())
{
    string sql = "select last_number from user_sequences where sequence_NAME = 'MAIN'";
    using (RecordSet<int> recordSet = new RecordSet<int>())
    {
        recordSet.Connection = <ConnectionObject>;
        recordSet.SQL = sql;
        recordSet.ResultTyp = RecordSetResult.Scalar;
        result = recordSet.Get().Result;
    }
 }
```
oder in verkürzter schreibweise
```csharp
int result = 0;

string sql = "select last_number from user_sequences where sequence_NAME = 'MAIN'";
result = new RecordSet<int>(<ConnectionObject>, sql, RecordSetResult.Scalar).Get().Result;
```
Vereinfachtes Schreiben eines einzelnen Wertes
```csharp
int result = 0;

string sqlText = $"UPDATE {TABLENAME} SET STATUS_ID = 100  WHERE ID = 1";
result = new RecordSet<int>(this.Connection, sqlText, RecordSetResult.Scalar).Set().Result;
```


Mögliche Rückgabetypen (bei Get())

| Typ  |
|:----------------|
| DataRow         |
| DataTable       |
| ICollectionView |
| string |
| DateTime |
| bool |
| int,long |
| decimal,double, float, Single|

Mögliche Übergabetypen (bei Set())

| Typ  |
|:----------------|
| int, long         |
| decimal, double, float     |
| bool         |
| string|

