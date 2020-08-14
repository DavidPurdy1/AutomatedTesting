using System.Diagnostics;
using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;
using ConsoleTests.src;
using Newtonsoft.Json;

namespace ConsoleTests
{
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

        public TestCaseObject()
        {
            foreach (Process p in Process.GetProcessesByName("Intact"))
            {
                p.Kill();
            }
            CreatedDate = DateTime.Now;
            GetAndSetTestCaseId();
        }

        private void GetAndSetTestCaseId() {
            TestCaseId = new DataRetrieval().GetTestCaseId();
        }
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
                //imagePaths.Add(user.Cleanup().TakeScreenshot(TestContext.TestName) + ".PNG");
                Print("FAILED *****************************************");
            }
        }
        public void AddDocument(DocumentObject document) {
            if (Documents.Count > 0) {
                foreach (var doc in Documents) {
                    doc.TestCaseId = TestCaseId;
                }
            }
        }
        public void TestCaseCleanup() {
            //CommunicateDataValues();
            //CloseDriver();
        }
        public void PrintInfo() {
            Print(JsonConvert.SerializeObject(this));
        }
        public void Print(string toPrint, string method = "")
        {
            debugLog.Info(method + " " + toPrint);
        }
    }
}