using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomatedTestingLib {
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

            try {
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

                run.TestRunId = int.Parse(command.ExecuteScalar().ToString());
                connection.Close();

            }catch(SqlException e) {
                Print(e.StackTrace, "AddToTestRunTable" ); 
            }catch(InvalidOperationException e) {
                Print(e.StackTrace, "AddToTestRunTable");
            }
        }
        private void AddToTestCaseTable(TestCaseObject tcase) { 
            //running the command
            SqlCommand command = new SqlCommand();
            try {
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

                tcase.TestCaseId = int.Parse(command.ExecuteScalar().ToString());
                connection.Close();
            } catch (SqlException e) {
                Print(e.StackTrace, "AddToTestCaseTable");
            } catch (InvalidOperationException e) {
                Print(e.StackTrace, "AddToTestCaseTable");
            }
        }
        private void AddToTestDocTable(DocumentObject document) {
            //running the command
            SqlCommand command = new SqlCommand();
            try {
                connection.Open();
                command.CommandTimeout = 60;
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "spAddTestDocData";

                command.Parameters.Add("guid", SqlDbType.NVarChar, 128).Value = document.DocumentId;
                command.Parameters.Add("testCaseId", SqlDbType.BigInt).Value = document.TestCaseId;
                var s = document.MetaData.ConvertToString();
                command.Parameters.Add("documentData", SqlDbType.NVarChar, 1000).Value = s;

                command.ExecuteNonQuery();
                connection.Close();
            } catch (SqlException e) {
                Print(e.StackTrace, "AddToTestDocTable");
            } catch (InvalidOperationException e) {
                Print(e.StackTrace, "AddToTestDocTable");
            }
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
                }
            }
        }

        /// <summary>
        /// retrieves data to validate values are correct on a document
        /// </summary>
        public void ValidateDocumentAdd(DocumentObject doc) {
            try {
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
                    int i = 0;
                    while (i < table.Rows.Count) {
                        var datatype = table.Rows[i]["DATA_TYPE"].ToString();
                        var dataval = table.Rows[i]["value"].ToString();
                        bool b = false;
                        string stype = "";

                        if (dataval != "") {

                            if (datatype.Equals("String")) {
                                foreach (var k in doc.MetaData.Strings) {
                                    Print("Predicted:" + k.ToString(), "actual:" + dataval.ToString());
                                    if (k.Equals(dataval)) {
                                        b = true;
                                        break;
                                    }
                                }
                                stype = "string";
                            } else if (datatype.Equals("Date")) {
                                var date = DateTime.Parse(dataval);
                                foreach (var k in doc.MetaData.Dates) {
                                    Print("Predicted:" + k.ToString(), "actual:" + date.ToString());
                                    if (k.Equals(date)) {
                                        b = true;
                                        break;
                                    }
                                }
                                stype = "date";
                            } else if (datatype.Equals("Number")) {
                                var index = dataval.IndexOf(".");
                                var intval = int.Parse(dataval.Substring(0, index));

                                foreach (var k in doc.MetaData.Numbers) {
                                    Print("Predicted:" + k.ToString(), "actual:" + intval.ToString());
                                    if (k.Equals(intval)) {
                                        b = true;
                                        break;
                                    }
                                }
                                stype = "number";
                            }
                            if (!b) {
                                throw new AssertFailedException(stype + " retrieved did not match " + stype + " going in");
                            }
                        }
                        i++;
                    }
                }
            } catch (SqlException e) {
                Print(e.StackTrace, "AddToTestRunTable");
            } catch (InvalidOperationException e) {
                Print(e.StackTrace, "AddToTestRunTable");
            }
        }
        private void Print(string toPrint, string method = "")
        {
            debugLog.Info(method + " " + toPrint);
        }
    }  
}
