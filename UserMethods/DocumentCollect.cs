using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace AutomatedTestingLib {
    /// <summary>
    /// Methods that use the Document Collector, BatchReview and InZone
    /// </summary>
    public class DocumentCollect {
        IWebElement window; 
        readonly WiniumMethods m;
        string method = "";
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        readonly Actions action;
        private readonly WindowProcessHandler handler;
        readonly int i = 0;
        public DocumentCollect(WiniumMethods m, Actions action) {
            this.m = m;
            this.action = action;
            handler = new WindowProcessHandler(); 
        }

        /// <summary> This method is going to add documents to batch review and then run through and both add a document to an existing document and attribute a new one.</summary>
        public void BatchReview(TestCaseObject tcase) {
            pt(i); 
            method = MethodBase.GetCurrentMethod().Name;
            AddDocsToCollector();
           
            window = m.Locate(By.Id("frmIntactMain"));
            handler.GetActiveWindow();
            var mainIntact = handler.WindowProcess; 
            window = m.Locate(By.Id("radPanelBar1"), window);
            window = m.Locate(By.Id("pageIntact"), window);
            window = m.Locate(By.Id("lstIntact"), window);
            m.Click(By.Name("Batch Review"), window);
            Thread.Sleep(1000); 
            m.Click(By.Id("6"));
            Thread.Sleep(1000);
            m.Click(By.Id("6"));


            //attribute test from batch review...
            BatchCreate(tcase);

            //add to document test from batch review... 
            AddDocBatchReview();

            m.Click(By.Id("btnClose"));
            handler.SetAsActiveWindow(mainIntact);
            pt(i);
        }
        private void AddDocBatchReview() {
            Thread.Sleep(2000);

            pt(i); 
            m.Click(By.Id("btnAddToDoc"));
            pt(i);
            m.Click(By.Name("DEFAULT DEF"));
            action.MoveByOffset(300, 0).DoubleClick().Build().Perform();
            Thread.Sleep(4000);

            window = m.Locate(By.Id("frmInsertPagesVersion"));
            m.Click(By.Id("btnOK"), window);
            m.Locate(By.Id("frmDocument"));

            //custom fields
            m.Click(By.Id("lblType"));
            action.MoveByOffset(150, 240).Click().SendKeys("1/1/2000").
                MoveByOffset(0, 20).Click().SendKeys("10").Build().Perform();

            //edit fields
            Print(method, "edit fields ");
            m.Click(By.Id("lblType"));
            action.MoveByOffset(170, 80).Click().SendKeys("1/1/2000").MoveByOffset(0, 20).Click().SendKeys("BATCH ADD").
                MoveByOffset(0, 40).Click().SendKeys("BATCH ADD TEST").Build().Perform();

            m.Click(By.Name("Save"));
            m.Click(By.Id("btnClose"));
        }

        public void BatchCreate(TestCaseObject tcase) {
            method = MethodBase.GetCurrentMethod().Name;
            Print("Started", method);
            Thread.Sleep(6000);
            pt(i); 
            window = m.Locate(By.Id("frmBatchReview"));
            if (m.IsElementPresent(By.Name("Maximize"), window)) {
                m.Click(By.Name("Maximize"), window);
            }
            pt(i); 
            handler.GetActiveWindow();
            pt(i); 
            m.Click(By.Id("btnAttribute"), window);
            Thread.Sleep(2000);

            DocumentObject document = new DocumentObject();

            m.Click(By.Id("lblType"));
            document.Type = "test";
            document.Definition = "def";

            action.MoveByOffset(30, 0).Click().SendKeys(document.Type).Build().Perform();
            m.Click(By.Id("lblType"));
            action.MoveByOffset(30, 27).Click().SendKeys(document.Definition).Build().Perform();

            //adding the metadata values
            m.Click(By.Id("lblType"));

            var date = DateTime.Now.Date;
            var num = new Random().Next();
            document.MetaData.AddData(date);
            document.MetaData.AddData(num);
            document.MetaData.AddData(document.DocumentId);
            action.MoveByOffset(150, 240).Click().SendKeys(date.ToString()).
                   MoveByOffset(0, 20).Click().SendKeys(num.ToString()).
                   MoveByOffset(0, 20).Click().SendKeys(document.DocumentId).Build().Perform();

                    //add author, expiration date, and summary
            m.Click(By.Id("lblType"));
            document.Author = "BATCH AUTHOR";
            document.Summary = "BATCH SUMMARY";
            action.MoveByOffset(170, 80).Click().SendKeys("1/1/2050").MoveByOffset(0, 20).Click().SendKeys(document.Author).
                   MoveByOffset(0, 40).Click().SendKeys(document.Summary).Build().Perform();

            //save and quit
            m.Click(By.Id("btnSave"));
            m.Click(By.Id("btnClose"));
            pt(i); 
            Print("Finished the document addition", method);
            tcase.AddDocument(document);
            new SQLDataHandler().ValidateDocumentAdd(document);
            pt(i); 
        }
        /// <summary>
        /// Collects documents by all definition from InZone.
        /// </summary>
        /// <returns>Throws an AssertFail if the definition recognized is not the same name as a file in directory</returns>
        public void InZone() {
            method = MethodBase.GetCurrentMethod().Name;
            AddDocsToCollector();
            window = m.Locate(By.Id("frmIntactMain"));
            window = m.Locate(By.Id("radMenu1"), window);
            m.Click(By.Name("&Intact"), window);
            window = m.Locate(By.Name("&Intact"), window);
            m.Click(By.Name("InZone"), window);
            Thread.Sleep(2000);
            window = m.Locate(By.Id("frmInZoneMain"));
            m.Click(By.Id("btnCollectScan"), window);
            Thread.Sleep(10000);

            bool hasPassed = false;
            string startPath = ConfigurationManager.AppSettings.Get("InZoneStartPath");
            foreach (string s in Directory.GetFiles(startPath)) {
                string test = Path.GetFileName(s);
                if (m.IsElementPresent(By.Name(test.Substring(0, test.Length - 4)))) {
                    hasPassed = true;
                    break;
                }
            }

            m.Click(By.Id("btnCommit"), window);
            Thread.Sleep(1000);
            m.Click(By.Id("btnClose"), window);
            if (!hasPassed) {
                Print(method, "InZone could not recognize those documents so it came as undefined");
                throw new AssertFailedException("InZone could not recognize those documents so it came in as undefined");
            }
        }
        /**
         * Method copies over files from one directory to another: Each time before InZone collects this is going to put files in the collector folder
         * Verify that the startPath always has files in it and those files shouldn't be removed from this folder when collected.
         */
        private void AddDocsToCollector() {
            method = MethodBase.GetCurrentMethod().Name;
            string startPath = ConfigurationManager.AppSettings.Get("InZoneStartPath");
            string endPath = ConfigurationManager.AppSettings.Get("InZoneCollectorPath");
            if (Directory.Exists(startPath) & Directory.Exists(endPath)) {
                foreach (string s in Directory.GetFiles(startPath)) {
                    File.Copy(s, Path.Combine(endPath, Path.GetFileName(s)), true);
                }
            } else {
                Print(method, "Starting or Ending path doesn't exist");
            }
        }
        private static void pt(int i) {
            i += 1; 
            debugLog.Info("{" + i.ToString() + "} "+ DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond); 
        }
        private void Print( string toPrint, string method = "") {
            debugLog.Info(method + " " + toPrint);
        }
    }
}
