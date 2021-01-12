using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace UseAlwaysEncrypted
{
    /// <summary>
    /// A simple class to demonstrate insert and query sensitive information using Always Encrypted functionality.
    /// Apart from the connection string keyword ColumnEncryptionSetting, rest of the code looks very similar to
    /// a regular SqlClient application code.
    /// 
    /// Pre-Requisites
    ///     .NET 4.6
    ///     SQL Script to setup the schema for Patents table.
    /// </summary>
    class Program
    {
        private static SqlConnection _sqlconn;

        /// <summary>
        /// Insert a row for a new patient.
        /// </summary>
        /// <param name="ssn">Patient's SSN.</param>
        /// <param name="firstName">Patient's First name</param>
        /// <param name="lastName">Patient's last name</param>
        /// <param name="birthdate">Patient's date of bith</param>
        private static void AddNewPatient(string ssn, string firstName, string lastName, DateTime birthdate)
        {
            SqlCommand cmd = _sqlconn.CreateCommand();

            // Use parameterized SQL to insert the data.
            //
            cmd.CommandText = @"INSERT INTO [dbo].[Patients] ([SSN], [FirstName], [LastName], [BirthDate]) VALUES (@SSN, @FirstName, @LastName, @BirthDate);";

            SqlParameter paramSSN = cmd.CreateParameter();
            paramSSN.ParameterName = @"@SSN";
            paramSSN.DbType = DbType.AnsiStringFixedLength;
            paramSSN.Direction = ParameterDirection.Input;
            paramSSN.Value = ssn;
            paramSSN.Size = 11;
            cmd.Parameters.Add(paramSSN);

            SqlParameter paramFirstName = cmd.CreateParameter();
            paramFirstName.ParameterName = @"@FirstName";
            paramFirstName.DbType = DbType.String;
            paramFirstName.Direction = ParameterDirection.Input;
            paramFirstName.Value = firstName;
            paramFirstName.Size = 50;
            cmd.Parameters.Add(paramFirstName);

            SqlParameter paramLastName = cmd.CreateParameter();
            paramLastName.ParameterName = @"@LastName";
            paramLastName.DbType = DbType.String;
            paramLastName.Direction = ParameterDirection.Input;
            paramLastName.Value = lastName;
            paramLastName.Size = 50;
            cmd.Parameters.Add(paramLastName);

            SqlParameter paramBirthdate = cmd.CreateParameter();
            paramBirthdate.ParameterName = @"@BirthDate";
            paramBirthdate.SqlDbType = SqlDbType.Date;
            paramBirthdate.Direction = ParameterDirection.Input;
            paramBirthdate.Value = birthdate;
            cmd.Parameters.Add(paramBirthdate);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Query the DB to find the patient with the desired SSN, and print the data in the console
        /// </summary>
        /// <param name="ssn">Patient's SSN</param>
        private static void FindAndPrintPatientInformation(string ssn)
        {
            SqlDataReader reader = null;
            try
            {
                reader = (ssn == null) ? FindAndPrintPatientInformationAll() : FindAndPrintPatientInformationSpecific(ssn);

                PrintPatientInformation(reader);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Query the DB to find all the patients and print the data in the console
        /// </summary>
        private static void FindAndPrintPatientInformation()
        {
            FindAndPrintPatientInformation(null);
        }

        /// <summary>
        /// Implementation for querying all patients in the DB
        /// </summary>
        /// <returns>A datareader with the query resultset</returns>
        private static SqlDataReader FindAndPrintPatientInformationAll()
        {
            SqlCommand cmd = _sqlconn.CreateCommand();

            // Normal select statement.
            //
            cmd.CommandText = @"SELECT [SSN], [FirstName], [LastName], [BirthDate] FROM [dbo].[Patients] ORDER BY [PatientId]";
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Implementation for querying a single patient, based on SSN
        /// </summary>
        /// <param name="ssn">Patient's SSN</param>
        /// <returns>A datareader with the query resultset</returns>
        private static SqlDataReader FindAndPrintPatientInformationSpecific(string ssn)
        {
            SqlCommand cmd = _sqlconn.CreateCommand();

            // Use parameterized SQL to query the data.
            //
            cmd.CommandText = @"SELECT [SSN], [FirstName], [LastName], [BirthDate] FROM [dbo].[Patients] WHERE [SSN] = @SSN;";

            SqlParameter paramSSN = cmd.CreateParameter();
            paramSSN.ParameterName = @"@SSN";
            paramSSN.DbType = DbType.AnsiStringFixedLength;
            paramSSN.Direction = ParameterDirection.Input;
            paramSSN.Value = ssn;
            paramSSN.Size = 11;
            cmd.Parameters.Add(paramSSN);

            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Format and print the patient data in the console.
        /// </summary>
        /// <param name="reader">the rowset with the patient's data</param>
        private static void PrintPatientInformation(SqlDataReader reader)
        {
            string breaker = new string('-', (19 * 4) + 9);
            Console.WriteLine();
            Console.WriteLine(breaker);
            Console.WriteLine(breaker);
            Console.WriteLine(@"| {0,15} |  {1,15} |  {2,15} | {3,25} |", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Console.WriteLine(breaker);
                    Console.WriteLine(@"| {0,15} |  {1,15} |  {2,15} | {3,25} | ", reader[0], reader[1], reader[2], ((DateTime)reader[3]).ToLongDateString());
                }
            }
            Console.WriteLine(breaker);
            Console.WriteLine(breaker);
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Print usage help on console
        /// </summary>
        static void PrintUsage()
        {
            Console.WriteLine(@"Usage: AlwaysEncryptedDemo <server_name> <database_name>");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                return;
            }

            SqlConnectionStringBuilder strbldr = new SqlConnectionStringBuilder();

            strbldr.DataSource = args[0];
            strbldr.InitialCatalog = args[1];
            strbldr.IntegratedSecurity = true;

            // Enable Always Encrypted in the connection we will use for this demo
            //
            strbldr.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled;

            _sqlconn = new SqlConnection(strbldr.ConnectionString);

            _sqlconn.Open();

            try
            {
                // Add a few rows to the table. 
                // Please notice that as far as the app is concerned, all data is in plaintext
                // 
                AddNewPatient("123-45-6789", "John", "Doe", new DateTime(1971, 5, 21));
                AddNewPatient("111-22-3333", "Joanne", "Doe", new DateTime(1974, 12, 1));
                AddNewPatient("562-00-6354", "Michael", "Park", new DateTime(1928, 11, 18));

                // Print a few individual entries as well as the whole table
                // Once again, the app handles the data as plaintext
                //
                FindAndPrintPatientInformation("123-45-6789");
                FindAndPrintPatientInformation("111-22-3333");
                FindAndPrintPatientInformation();
            }
            finally
            {
                _sqlconn.Close();
            }
        }
    }
}
