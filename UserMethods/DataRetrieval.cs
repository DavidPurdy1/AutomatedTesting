using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;
using System;

namespace ConsoleTests.src
{
    /// <summary>
    ///<para>Verifies that a document is filed correctly by testing name(guid) and metadata are going to be the same</para>
    /// <para>Throws an assertfail for the test if the information passed in and what is found in the sql table is not the same</para>
    /// </summary>
    public class DataRetrieval
    {
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        readonly SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings.Get("DBConnection"));

        public void ValidateDocumentAdd(string guid, DateTime date, string number)
        {
            string method = MethodBase.GetCurrentMethod().Name;
            connection.Open();
            SqlCommand command = new SqlCommand {
                CommandTimeout = 60,
                Connection = connection,
                CommandType = CommandType.StoredProcedure,
                CommandText = "spGetDocumentDataFromMostRecent"
            };
            Print(method, "has started");
            using (SqlDataReader reader = command.ExecuteReader())
            {
                DataTable table = new DataTable();
                table.Load(reader);

                var index = table.Rows[0]["value"].ToString().IndexOf(".");
                string num = table.Rows[0]["value"].ToString().Substring(0, index);
                if (!guid.Equals(table.Rows[0]["DOCUMENT_NAME"].ToString()))
                {
                    Print(method, "Document Name Data Does Not Match");
                    throw new AssertFailedException("Document Name Data Does Not Match");
                }
                else if (!guid.Equals(table.Rows[2]["value"].ToString()))
                {
                    Print(method, "predicted " + guid);
                    Print(method, "actual " + table.Rows[2]["value"].ToString());

                    Print(method, "Document guid string Data Does Not Match");
                    throw new AssertFailedException("Document guid string Data Does Not Match");
                }
                else if (!date.Equals(DateTime.Parse(table.Rows[1]["value"].ToString())))
                {
                    Print(method, "predicted " + date.ToString());
                    Print(method, "actual " + table.Rows[1]["value"].ToString());

                    Print(method, "Document date Data Does Not Match");
                    throw new AssertFailedException("Document date Data Does Not Match");
                }
                else if (!number.Equals(num))
                {
                    Print(method, "predicted " + number);
                    Print(method, "actual " + table.Rows[0]["value"].ToString());

                    Print(method, "Document number Data Does Not Match");
                    throw new AssertFailedException("Document number Data Does Not Match");
                }
                reader.Close();
                connection.Close();
            }
            Print(method, "Document Data Matches in Table");
        }
        public int GetTestRunId() {
            int testRunId = -1;


            return testRunId;
        }
        public int GetTestCaseId() {
            int testCaseId = -1;


            return testCaseId; 
        }
        public void Print(string method, string toPrint)
        {
            debugLog.Info(method + " " + toPrint);
        }
    }
}
