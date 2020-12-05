using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AutoLotDAL.Bulklmport
{
    public static class ProcessBulkImpor
    {
        private static SqlConnection _sqlConnection = null;
        private static readonly string connectionString = @"Data Source = (localdb)\mssqllocaldb; Integrated Security = true; Initial Catalog = AutoLot";
        
        private static void OpenConnection ()
        {
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        private static void CloseConnection ()
        {
            if (_sqlConnection?.State != ConnectionState.Closed)
            _sqlConnection?.Close();
        }

        public static void ExecuteBulkImport<T> (IEnumerable<T> records, string tableName)
        {
            OpenConnection();
            using (SqlConnection connection = _sqlConnection)
            {
                SqlBulkCopy bc = new SqlBulkCopy(connection)
                {
                    DestinationTableName = tableName
                };
                var DataREader = new MyDataReader<T>(records.ToList());
                try
                {
                    bc.WriteToServer(DataREader);
                }
                catch
                {
                    Console.WriteLine("Массовое копирование завершилось с ошибкой");
                }
                finally
                {
                    CloseConnection();
                }
            }
        }
    }
}
