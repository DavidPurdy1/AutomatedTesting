using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Configuration;
using log4net;
using Newtonsoft.Json;
using System.IO;

namespace AutomatedTestingLib
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

        /// <summary>
        /// Creates a Test Run and Initializes Values
        /// </summary>
        public TestRunObject()
        {
            CreatedDate = DateTime.Now;
            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            var AppInfo = FileVersionInfo.GetVersionInfo(ConfigurationManager.AppSettings.Get("IntactPath"));
            ApplicationVersion = AppInfo.FileVersion;
            ApplicationName = AppInfo.ProductName;
        }
        /// <summary>
        /// Adds a Test Case to the Run and communicates the status of the Test Case to the Test Run
        /// </summary>
        public void AddTestCase(TestCaseObject tcase) {
            tcase.CreatedBy = CreatedBy;
            if (tcase.TestStatus == 2) {
                TestsPassed++;
            } else {
                TestsFailed++;
            }
            TestCases.Add(tcase); 
        }
        public void TestRunObjectCleanup() {
            WindowProcessHandler.CloseExtraDriverInstances();  
            new SQLDataHandler().HandleDataExport(this); 
        }
        public void PrintInfo() {
            Print(JsonConvert.SerializeObject(this, Formatting.Indented)); 
        }
        public void WriteResultFile() {
            //YYYY-MM-DD__HH-MM-SS text file name
            string dateAndTime = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "__"
                + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();

            //writes the result file information
            using (StreamWriter file =
            new StreamWriter(ConfigurationManager.AppSettings.Get("FileLocation") + dateAndTime + ".txt", false)) {
                file.Write(JsonConvert.SerializeObject(this, Formatting.Indented)); 
            }
        }
        private void Print(string toPrint, string method = "") {
            debugLog.Info(method + " " + toPrint);
        }
    }
}