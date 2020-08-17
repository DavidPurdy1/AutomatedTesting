using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleTests.src {
    /// <summary>
    /// Exports all data about testing to sql when it finishes. It also retrieves data to validate values are correct
    /// </summary>
    class SQLDataHandler
    {
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        private readonly SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings.Get("DBConnection"));

        private void AddToTestRunTable(TestRunObject run) {

            //running the command
            SqlCommand command = new SqlCommand();
            connection.Open();
            command.CommandTimeout = 60;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "spAddTestRunData";

            //adding the values to the parameter
            command.Parameters.Add("testerName", SqlDbType.NVarChar, 50).Value = run.CreatedBy; 
            command.Parameters.Add("applicationName", SqlDbType.NVarChar, 50).Value = run.ApplicationName;
            command.Parameters.Add("applicationVersion", SqlDbType.NVarChar, 50).Value = run.ApplicationVersion;
            command.Parameters.Add("testsFailed", SqlDbType.SmallInt).Value = run.TestsFailed;
            command.Parameters.Add("testsPassed", SqlDbType.SmallInt).Value = run.TestsPassed;
            command.Parameters.Add("createdDate", SqlDbType.DateTime).Value = run.CreatedDate;
            command.Parameters.Add("createdBy", SqlDbType.NVarChar, 50).Value = run.CreatedBy;

            command.ExecuteNonQuery();
            run.TestRunId = int.Parse(command.ExecuteScalar().ToString());
            connection.Close();
        }
        private void AddToTestCaseTable(TestCaseObject tcase) { 
            //running the command
            SqlCommand command = new SqlCommand();
            connection.Open();
            command.CommandTimeout = 60;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "spAddTestCaseData";
            
            //adding the values to the parameter
            command.Parameters.Add("testName", SqlDbType.NVarChar, 50).Value = tcase.TestName;
            command.Parameters.Add("testStatusId", SqlDbType.BigInt).Value = tcase.TestStatus;
            command.Parameters.Add("imagePath", SqlDbType.NVarChar, 256).Value = tcase.ImagePath;
            command.Parameters.Add("createdDate", SqlDbType.DateTime).Value = tcase.CreatedDate;
            command.Parameters.Add("createdBy", SqlDbType.NVarChar, 50).Value = tcase.CreatedBy;
            command.Parameters.Add("testRunId", SqlDbType.BigInt, 50).Value = tcase.TestRunId;

            command.ExecuteNonQuery();
            tcase.TestCaseId = int.Parse(command.ExecuteScalar().ToString());
            connection.Close();
        }
        private void AddToTestDocTable(DocumentObject document) {
            //running the command
            SqlCommand command = new SqlCommand();

            connection.Open();
            command.CommandTimeout = 60;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "spAddTestDocumentData";

            command.Parameters.Add("documentGuid", SqlDbType.NVarChar, 256).Value = document.DocumentId;
            command.Parameters.Add("testCaseId", SqlDbType.BigInt).Value = document.TestCaseId; 

            command.ExecuteNonQuery();
            connection.Close();
        }
        private void AddToTestDocFieldsTable(string field, string guid) {
            SqlCommand command = new SqlCommand();

            connection.Open();
            command.CommandTimeout = 60;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "spAddTestDocumentFields";

            command.Parameters.Add("documentGuid", SqlDbType.NVarChar, 256).Value = guid;
            command.Parameters.Add("CustomFieldVal", SqlDbType.BigInt).Value = field; 

            command.ExecuteNonQuery();
            connection.Close();
        }
        /// <summary>
        /// Exports all data about testing to sql when it finishes.
        /// </summary>
        public void HandleDataExport(TestRunObject run) {
            AddToTestRunTable(run);
            
            foreach( var tcase in run.TestCases) {
                tcase.TestRunId = run.TestRunId; 
                AddToTestCaseTable(tcase); 

                foreach(var doc in tcase.Documents) {
                    doc.TestCaseId = tcase.TestCaseId; 
                    AddToTestDocTable(doc);
                    
                    foreach(var field in doc.CustomFieldData) {
                        AddToTestDocFieldsTable(field, doc.DocumentId); 
                    }
                }
            }
        }
        /// <summary>
        /// retrieves data to validate values are correct on a document
        /// </summary>
        public void ValidateDocumentAdd(DocumentObject doc) {
            string method = MethodBase.GetCurrentMethod().Name;
            connection.Open();
            SqlCommand command = new SqlCommand {
                CommandTimeout = 60,
                Connection = connection,
                CommandType = CommandType.StoredProcedure,
                CommandText = "spGetDocumentDataFromMostRecent"
            };

            using (SqlDataReader reader = command.ExecuteReader()) {
                DataTable table = new DataTable();
                table.Load(reader);

                var index = table.Rows[0]["value"].ToString().IndexOf(".");
                string num = table.Rows[0]["value"].ToString().Substring(0, index);

                if (!doc.DocumentId.Equals(table.Rows[0]["DOCUMENT_NAME"].ToString())) {
                    Print("Document Name Data Does Not Match", method);
                    throw new AssertFailedException("Document Name Data Does Not Match");

                } else if (!doc.CustomFieldData[2].Equals(table.Rows[2]["value"].ToString())) {
                    Print("predicted " + doc.CustomFieldData[2], method);
                    Print("actual " + table.Rows[2]["value"].ToString(), method);

                    Print("Document guid string Data Does Not Match", method);
                    throw new AssertFailedException("Document guid string Data Does Not Match");

                } else if (!DateTime.Parse(doc.CustomFieldData[0]).Equals(DateTime.Parse(table.Rows[1]["value"].ToString()))) {
                    Print("predicted " + doc.CustomFieldData[0].ToString(), method);
                    Print("actual " + table.Rows[1]["value"].ToString(), method);

                    Print("Document date Data Does Not Match", method);
                    throw new AssertFailedException("Document date Data Does Not Match");

                } else if (!doc.CustomFieldData[1].Equals(num)) {
                    Print("predicted " + doc.CustomFieldData[1], method);
                    Print("actual " + table.Rows[0]["value"].ToString(), method);

                    Print("Document number Data Does Not Match", method);
                    throw new AssertFailedException("Document number Data Does Not Match");
                }
                reader.Close();
                connection.Close();
            }
            Print("Document Data Matches in Table", method);
        }
        private void Print(string toPrint, string method = "")
        {
            debugLog.Info(method + " " + toPrint);
        }
    }  
}
