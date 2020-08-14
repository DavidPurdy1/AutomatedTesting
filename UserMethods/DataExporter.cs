using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using log4net;

namespace ConsoleTests.src {
    /// <summary>
    /// Exports Parsed Data from the .txt file the tests output on cleanup and sends that to sql tables
    /// </summary>
    class DataExporter
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
            command.Parameters.Add("modifiedBy", SqlDbType.NVarChar, 50).Value = run.CreatedBy;
            command.Parameters.Add("modifiedDate", SqlDbType.DateTime, 50).Value = run.CreatedDate;

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
            command.Parameters.Add("testDate", SqlDbType.DateTime, 50).Value = tcase.CreatedDate;
            command.Parameters.Add("imagePath", SqlDbType.NVarChar, 256).Value = tcase.ImagePath;
            command.Parameters.Add("createdDate", SqlDbType.DateTime).Value = tcase.CreatedDate;
            command.Parameters.Add("createdBy", SqlDbType.NVarChar, 50).Value = tcase.CreatedBy;
            command.Parameters.Add("modifiedBy", SqlDbType.NVarChar, 50).Value = tcase.CreatedBy;
            command.Parameters.Add("modifiedDate", SqlDbType.DateTime, 50).Value = tcase.CreatedDate;
            command.Parameters.Add("testRunId", SqlDbType.BigInt, 50).Value = tcase.TestRunId - 1;

            command.ExecuteNonQuery();
            connection.Close();
        }
        private void AddToTestDocTable(DocumentObject document) {
            //running the command
            SqlCommand command = new SqlCommand();
            connection.Open();
            command.CommandTimeout = 60;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "spAddTestCaseData";

            command.Parameters.Add("documentGuid", SqlDbType.NVarChar, 256).Value = document.DocumentId;

            command.ExecuteNonQuery();
            connection.Close();
        }
        public void HandleDataExport(TestRunObject run) {
            AddToTestRunTable(run); 
            foreach( var tcase in run.TestCases) {
                AddToTestCaseTable(tcase); 
                foreach(var doc in tcase.Documents) {
                    AddToTestDocTable(doc); 
                }
            }
        }
        //public void ParseFile(TestData data)
        //{
        //    foreach (string file in Directory.EnumerateFiles(ConfigurationManager.AppSettings.Get("FileLocation")))
        //    {
        //        string[] lines = File.ReadAllLines(file);

        //        //all the information for the test runs is here
        //        data.CreatedDate = DateTime.Parse(lines[1].Substring(0, lines[1].Length - 2));
        //        data.TestsPassed = int.Parse(lines[2].Substring(14));
        //        data.TestsFailed = int.Parse(lines[3].Substring(14));
        //        data.CreatedBy = lines[4].Substring(8);
        //        data.ApplicationVersion = lines[5].Substring(13);
        //        data.ApplicationName = lines[6].Substring(10);

        //        AddToTestRunTable(data);

        //        //The test case names, test case status, image path all assigned here.
        //        //Some of the data from above is used in the test case table
        //        int i = 7;

        //        while (i < lines.Length)
        //        {
        //            data.TestName = lines[i].Substring(8);

        //            if (lines[i].Substring(0, 8).Equals("cancel| "))
        //            {
        //                data.TestStatus = 3;
        //                data.ImagePath = null;
        //                if (data.TestName.Equals("TEST1_6_DOCUMENTS"))
        //                {
        //                    string[] values = lines[i + 1].Split(',');
        //                    for (int j = 0; j < values.Length; j++)
        //                    {
        //                        AddToDocumentDataTable(data, values[j]);
        //                    }
        //                    i += 2;
        //                }
        //                else
        //                {
        //                    i++;
        //                }
        //            }
        //            else if (lines[i].Substring(0, 8).Equals("passed| "))
        //            {
        //                data.TestStatus = 1;
        //                data.ImagePath = null;
        //                if (data.TestName.Equals("TEST1_6_DOCUMENTS"))
        //                {
        //                    string[] values = lines[i + 1].Split(',');
        //                    for (int j = 0; j < values.Length; j++)
        //                    {
        //                        AddToDocumentDataTable(data, values[j]);
        //                    }
        //                    i += 2;
        //                }
        //                else
        //                {
        //                    i++;
        //                }
        //            }
        //            else
        //            {
        //                data.TestStatus = 0;
        //                data.ImagePath = lines[i + 1];
        //                if (data.TestName.Equals("TEST1_6_DOCUMENTS"))
        //                {
        //                    string[] values = lines[i + 2].Split(',');
        //                    for (int j = 0; j < values.Length; j++)
        //                    {
        //                        AddToDocumentDataTable(data, values[j]);
        //                    }
        //                    i += 3;
        //                }
        //                else
        //                {
        //                    i += 2;
        //                }
        //            }
        //            AddToTestCaseTable(data);
        //        }
        //        File.Move(file, ConfigurationManager.AppSettings.Get("ReadFileLocation") + "Test " + (data.TestRunId - 1).ToString() + ".txt");
        //    }
        //}
        public void Print(string method, string toPrint)
        {
            debugLog.Info(method + " " + toPrint);
        }
    }  
}
