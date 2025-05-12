//-----------------------------------------------------------------------
// <copyright file="RecordSet.cs" company="Lifeprojects.de">
//     Class: RecordSet
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>112.05.2025</date>
//
// <summary>
// Klasse zur vereinfachten verwenden von SQL Anweisungen. Die Klasse RecordSet ermöglicht es, einen bestimmen Datentyp aus der Datenbank zurückzugeben.
// </summary>
// <example>
// int result = new RecordSet<int>(this.Connection, "select count(*) from Table", RecordSetResult.Scalar).Get().Result;
//
// DataRow row = new RecordSet<DataRow>(this.Connection, "select * from Table where id = 1", RecordSetResult.DataRow).Get().Result;
//
// DataTable result = new RecordSet<DataTable>(repository.GetConnection, "select * from Table", RecordSetResult.DataTable).Get().Result;
//
//ICollectionView result = new RecordSet<ICollectionView>(repository.GetConnection, sql, RecordSetResult.CollectionView).Get().Result;
// </example>
//-----------------------------------------------------------------------

namespace Console.SQLiteHelper
{
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text.RegularExpressions;
    using System.Windows.Data;

    public class RecordSet<T> : IDisposable
    {
        private bool classIsDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSet"/> class.
        /// </summary>
        /// <param name="connection">Connection Objekt zur Datenbank</param>
        /// <param name="sql">SQL Anweisung</param>
        /// <param name="resultTyp">Ergebnis Typ</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RecordSet(SQLiteConnection connection, string sql, RecordSetResult resultTyp = RecordSetResult.DataTable)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("Das Connection-Objekt darf nicht 'null' sein");
            }

            if (string.IsNullOrEmpty(sql) == true)
            {
                throw new ArgumentNullException("Der String mit für die SQL-Anweisung darf nicht 'null' oder leer sein");
            }

            try
            {
                this.Connection = connection;
                this.SQL = sql;
                this.ResultTyp = resultTyp;
            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSet"/> class.
        /// </summary>
        /// <param name="connection">Connection Objekt zur Datenbank</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RecordSet(SQLiteConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("Das Connection-Objekt darf nicht 'null' sein");
            }

            try
            {
                this.Connection = connection;
                this.ResultTyp = RecordSetResult.DataTable;
            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }
        }

        public RecordSet()
        {
            try
            {
                this.ResultTyp = RecordSetResult.DataTable;
            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }
        }

        public T Result { get; private set; }

        public SQLiteConnection Connection { get; set; }

        public string SQL { get; set; }

        public RecordSetResult ResultTyp { get; set; }

        public RecordSet<T> Set()
        {
            try
            {
                if (this.CheckSetResultParameter(typeof(T)) == false)
                {
                    throw new ArgumentException($"Der Typ '{typeof(T).Name}' ist für das Schreiben des RecordSet '{ResultTyp.ToString()}' nicht gültig.");
                }

                if (ResultTyp == RecordSetResult.Scalar)
                {
                    this.Result = this.SetScalar();
                }

            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }

            return this;
        }

        public RecordSet<T> Get()
        {
            try
            {
                if (this.CheckGetResultParameter(typeof(T)) == false)
                {
                    throw new ArgumentException($"Der Typ '{typeof(T).Name}' ist für die Rückgabe des RecordSet Result beim '{ResultTyp.ToString()}' nicht gültig.");
                }

                if (ResultTyp == RecordSetResult.DataRow)
                {
                    this.Result = this.GetSingle();
                }
                else if (ResultTyp == RecordSetResult.CollectionView)
                {
                    this.Result = this.GetCollectionView();
                }
                else if (ResultTyp == RecordSetResult.Scalar)
                {
                    this.Result = this.GetScalar();
                }
                else if (ResultTyp == RecordSetResult.DataTable)
                {
                    this.Result = this.GetDataTable();
                }
            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }

            return this;
        }

        private T GetSingle()
        {
            object result = null;

            try
            {
                using (SQLiteCommand cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = this.SQL;

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            DataTable dt = new DataTable();
                            dt.TableName = this.ExtractTablename(this.SQL);
                            dt.Load(dr);
                            result = dt.Rows[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }

            return (T)result;
        }

        private T GetCollectionView()
        {
            ICollectionView result;

            try
            {
                using (SQLiteCommand cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = this.SQL;

                    DataTable dt = null;
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            dt = new DataTable();
                            dt.Load(dr);
                        }
                    }

                    result = CollectionViewSource.GetDefaultView(dt.Rows) as CollectionView;
                }
            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }

            return (T)result;
        }

        private T GetScalar()
        {
            object getAs = null;

            try
            {
                using (SQLiteCommand cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = this.SQL;
                    var result = cmd.ExecuteScalar();
                    getAs = result == null ? default(T) : (T)Convert.ChangeType(result, typeof(T));
                }

            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }

            return (T)getAs;
        }

        private T SetScalar()
        {
            object getAs = null;

            try
            {
                using (SQLiteCommand cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = this.SQL;
                    int? result = cmd.ExecuteNonQuery();
                    getAs = result == null ? default(T) : (T)Convert.ChangeType(result, typeof(T));
                }

            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }

            return (T)getAs;
        }

        private T GetDataTable()
        {
            object result = null;

            try
            {
                using (SQLiteCommand cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = this.SQL;

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            result = new DataTable();
                            ((DataTable)result).TableName = this.ExtractTablename(this.SQL);
                            ((DataTable)result).Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorText = ex.Message;
                throw;
            }

            return (T)result;
        }

        private string ExtractTablename(string sql)
        {
            try
            {
                List<string> tables = new List<string>();

                Regex r = new Regex(@"(from|join|into)\s+(?<table>\S+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Match ma = r.Match(sql);
                while (ma.Success)
                {
                    tables.Add(ma.Groups["table"].Value);
                    ma = ma.NextMatch();
                }

                return tables.FirstOrDefault().ToUpper();
            }
            catch (Exception)
            {
                return $"TAB{DateTime.Now.ToString("yyyyMMdd")}";
            }
        }

        private bool CheckSetResultParameter(Type type)
        {
            bool result = false;

            if (type.Name == typeof(int).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(long).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(bool).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(decimal).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(double).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(float).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(string).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(DateTime).Name)
            {
                result = true;
            }

            return result;
        }

        private bool CheckGetResultParameter(Type type)
        {
            bool result = false;

            if (type.Name== typeof(DataRow).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(DataTable).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(ICollectionView).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(string).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(DateTime).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(bool).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(int).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(long).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(Single).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(decimal).Name)
            {
                result = true;
            }
            else if (type.Name == typeof(float).Name)
            {
                result = true;
            }

            return result;
        }

        #region Dispose Function
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool classDisposing = false)
        {
            if (this.classIsDisposed == false)
            {
                if (classDisposing)
                {
                }
            }

            this.classIsDisposed = true;
        }
        #endregion Dispose Function
    }

    public enum RecordSetResult : int
    {
        [Description("Keine Aktion")]
        None = 0,
        [Description("Als Ergebnis wird ein DataRow zurückgegeben")]
        DataRow = 1,
        [Description("Als Ergebnis wird eine ICollectionView zurückgegeben")]
        CollectionView = 2,
        [Description("Als Ergebnis wird ein Feld aus der übergebenen SQL-Anweisung zurückgegeben")]
        Scalar = 3,
        [Description("Als Ergebnis wird ein DataTable zurückgegeben")]
        DataTable = 4,
    }
}
