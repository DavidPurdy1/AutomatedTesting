using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleTests.src {
    /// <summary>
    /// Methods that use the Document Collector, BatchReview and InZone
    /// </summary>
    public class DocumentCollect {
        IWebElement window; 
        readonly WiniumMethods m;
        string method = "";
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        readonly Actions action; 
        public DocumentCollect(WiniumMethods m, Actions action) {
            this.m = m;
            this.action = action;
        }

        /// <summary> This method is going to add documents to batch review and then run through and both add a document to an existing document and attribute a new one.</summary>
        public void BatchReview() {
            method = MethodBase.GetCurrentMethod().Name;
            AddDocsToCollector();

            window = m.Locate(By.Id("frmIntactMain"));

            window = m.Locate(By.Id("radPanelBar1"), window);
            window = m.Locate(By.Id("pageIntact"), window);
            window = m.Locate(By.Id("lstIntact"), window);
            m.Click(By.Name("Batch Review"), window);
            m.Click(By.Id("6"));
            Thread.Sleep(1000);
            m.Click(By.Id("6"));


            //attribute test from batch review...
            BatchAttribution();

            //add to document test from batch review... 
            AddDocBatchReview();

            m.Click(By.Id("btnClose"));
        }
        private void AddDocBatchReview() {
            m.Click(By.Id("btnAddToDoc"));
            m.Click(By.Name("DEFAULT DEF"));
            m.Click(By.Name("DEFAULT DEFINITION TEST"));
            m.Click(By.Id("rbtnOK"));
            Thread.Sleep(2000);
            m.Locate(By.Name("&OK"));
            Thread.Sleep(2000);
            window = m.Locate(By.Id("frmInsertPagesVersion"));
            m.Click(By.Id("btnOK"), window);
            m.Locate(By.Id("frmDocument"));

            //rotate 
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            m.Click(By.Id("lblType"));
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            action.MoveByOffset(375, -37).Click().Click().Click().Click().Click().Build().Perform();
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);

            //custom fields
            Print(method, "custom fields");
            m.Click(By.Id("lblType"));
            action.MoveByOffset(150, 240).Click().SendKeys("1/1/2000").
                MoveByOffset(0, 20).Click().SendKeys("10").MoveByOffset(0, 20).Click().SendKeys("BATCH REVIEW ADD").Build().Perform();

            //edit fields
            Print(method, "edit fields ");
            m.Click(By.Id("lblType"));
            action.MoveByOffset(170, 80).Click().SendKeys("1/1/2000").MoveByOffset(0, 20).Click().SendKeys("BATCH AUTHOR TEST").
                MoveByOffset(0, 40).Click().SendKeys("BATCH ADDING TO ANOTHER DOCUMENT TEST").Build().Perform();

            m.Click(By.Name("Save"));
            m.Click(By.Name("Close"));
        }
        private void BatchAttribution() {
            window = m.Locate(By.Id("frmBatchReview"));
            m.Click(By.Id("btnAttribute"), window);
            Thread.Sleep(2000);
            window = m.Locate(By.Id("frmDocument"));

            //rotate 
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            m.Click(By.Id("lblType"));
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            action.MoveByOffset(375, -37).Click().Click().Build().Perform();
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);

            //custom fields
            Print(method, "custom fields");
            m.Click(By.Id("lblType"));
            action.MoveByOffset(150, 240).Click().SendKeys("1/1/2000").
                MoveByOffset(0, 20).Click().SendKeys("10").MoveByOffset(0, 20).Click().SendKeys("BATCH REVIEW ATTRIBUTION").Build().Perform();

            //edit fields
            Print(method, "edit fields ");
            m.Click(By.Id("lblType"));
            action.MoveByOffset(170, 80).Click().SendKeys("1/1/2000").MoveByOffset(0, 20).Click().SendKeys("BATCH AUTHOR TEST").
                MoveByOffset(0, 40).Click().SendKeys("BATCH ATTRIBUTING A DOCUMENT TEST").Build().Perform();

            m.Click(By.Id("btnSave"), window);
            m.Click(By.Id("btnClose"), window);
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
        private void Print(string method, string toPrint) {
            debugLog.Info(method + " " + toPrint);
        }
    }
}
