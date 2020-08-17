using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;
using Newtonsoft.Json;
using AutomatedTestingLib;
using ConsoleTests.src;

namespace ConsoleTests {
    /// <summary>
    /// Test Case containing Data at the Test Case level and functions of a Test Case
    /// </summary>
    public class TestCaseObject
    {
        #region fields
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        public int TestRunId { get; set; }
        public int TestCaseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string TestName { get; set; } = "";
        public int TestStatus { get; set; } = -1;
        public string ImagePath { get; set; } = ""; 
        public List<DocumentObject> Documents { get; set; } = new List<DocumentObject>();
        #endregion

        /// <summary>
        ///Initializes values and closes any previous Intact instances
        /// </summary>
        public TestCaseObject()
        {
            WindowProcessHandler.CloseIntact(); 
            CreatedDate = DateTime.Now;
        }
        /// <summary>
        /// Adds the test result, name, and takes and saves the screenshot if failed and sets image path
        /// </summary>
        public void AddTestCaseResult(UnitTestOutcome result, string testName)
        {
            TestName = testName;
            if (result == UnitTestOutcome.Inconclusive)
            {
                TestStatus = 1;
                Print("Interrupted *****************************************");
            }
            else if (result == UnitTestOutcome.Passed)
            {
                TestStatus = 2;
                Print("PASSED *****************************************");
            }
            else
            {
                TestStatus = 3;
                ImagePath = ScreenshotHandler.CaptureDesktop(testName); 
                Print("FAILED *****************************************");
            }
        }
        public void AddDocument(DocumentObject document) {
            Documents.Add(document); 
        }
        /// <summary>
        /// Disposes any error messages that occur and closes the driver
        /// </summary>
        public void TestCaseCleanup(UserMethods user) {
            WindowProcessHandler.DisposeErrorMessages();
            user.CloseDriver(); 
        }
        public void PrintInfo() {
            Print(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        private void Print(string toPrint, string method = "")
        {
            debugLog.Info(method + " " + toPrint);
        }
    }
}