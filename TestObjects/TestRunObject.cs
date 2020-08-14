using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Configuration;
using ConsoleTests.src;
using log4net;
using Newtonsoft.Json;

namespace ConsoleTests
{
    /// <summary>
    /// Test Run containing Data at the Test Run level and functions of a Test Run
    /// <para>Each Test Run can contain multiple Test Cases.</para>
    /// </summary>
    public class TestRunObject
    {
        #region fields
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        public int TestRunId { get; set; }
        public int TestsFailed { get; set; } = 0; 
        public int TestsPassed { get; set; } = 0; 
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public List<TestCaseObject> TestCases { get; set; } = new List<TestCaseObject>();
        #endregion

        public TestRunObject()
        {
            GetAndSetTestRunId();
            CreatedDate = DateTime.Now;
            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            var AppInfo = FileVersionInfo.GetVersionInfo(ConfigurationManager.AppSettings.Get("IntactPath"));
            ApplicationVersion = AppInfo.FileVersion;
            ApplicationName = AppInfo.ProductName;
        }

        //gets test run id from sql and sets it
        private void GetAndSetTestRunId() {
            TestRunId = new DataRetrieval().GetTestRunId(); 
        }
        private void CommunicateDataValues() {
            foreach(var tcase in TestCases){
                tcase.TestRunId = TestRunId;
                tcase.CreatedBy = CreatedBy;

                if (tcase.TestStatus == 2) {
                    TestsPassed++;
                } else {
                    TestsFailed++;
                }
            }
        }
        public void AddTestCase(TestCaseObject tcase) {
            tcase.TestRunId = TestRunId;
            tcase.CreatedBy = CreatedBy;
            if (tcase.TestStatus == 2) {
                TestsPassed++;
            } else {
                TestsFailed++;
            }
            TestCases.Add(tcase); 
        }
        public void TestRunObjectCleanup() {
            //CloseExtraDriverInstances();
            CommunicateDataValues(); 
            //new DataExporter().HandleDataExport(this); 
        }
        public void PrintInfo() {
            Print(JsonConvert.SerializeObject(this)); 
        }
        private void Print(string toPrint, string method = "") {
            debugLog.Info(method + " " + toPrint);
        }
    }
}