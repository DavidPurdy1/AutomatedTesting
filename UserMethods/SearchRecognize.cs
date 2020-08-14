using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleTests.src {
    /// <summary>
    /// Methods for Search and Recognize Feature
    /// </summary>
    public class SearchRecognize {
        #region fields
        IWebElement window;
        readonly WiniumMethods m;
        string method = "";
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        readonly Actions action;
        #endregion

        public SearchRecognize(WiniumMethods m, Actions action) {
            this.m = m;
            this.action = action;
        }

        private void AddRecognition() {
            m.Click(By.Name("Recognize"));
            window = m.Locate(By.Id("frmMainInteractive"));
            Thread.Sleep(1500);
            window = m.Locate(By.Id("btnSelect"), window);
            m.Click(By.Name("Select All"), window);
            m.Click(By.Id("btnRecgonize"));
            Thread.Sleep(5000);
            m.Click(By.Id("btnClose"));
        }
        /// <summary><para>Tests Intact Recognition</para>
        /// <para>definitionName: definition of what you want to recognize, documentName: document name,input: string to search to see if the page recognizes it </para>
        /// </summary>
        public void Recognition(string definitionName, string documentName, string input) {
            CreateDocumentForRecognize(); 
            AddRecognition();
            OpenOrganizer();
            m.Click(By.Name(definitionName));
            window = m.Locate(By.Name(documentName));
            action.DoubleClick(window).Build().Perform();
            Thread.Sleep(2000);

            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            m.Click(By.Id("lblType"));
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            action.MoveByOffset(470, -40).Click().Build().Perform();
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);

            window = m.Locate(By.Name("Find"));
            m.SendKeys(By.Name(""), input, m.Locate(By.Id("txtFind"), window));
            m.Click(By.Id("btnFind"));

            if (m.IsElementPresent(By.Name("Search Text Not Found"), window)) {
                Print(method, "Search Text not found for Recognize");
                //throw new AssertFailedException("Search Text not found for Recognize");
            }
            m.Click(By.Id("btnCancel"));
            m.Click(By.Id("btnClose"));
        }
        /// <summary>Searches intact for a keyword</summary>
        public void Search(string searchInput) {
            method = MethodBase.GetCurrentMethod().Name;
            window = m.Locate(By.Id("frmIntactMain"));
            window = m.Locate(By.Name("radMenu1"), window);
            m.SendKeys(By.ClassName("WindowsForms10.EDIT.app.0.5c39d4"), searchInput, window);
            m.Click(By.Name("Search"), window);

            Thread.Sleep(1000);
            if (m.IsElementPresent(By.Name("Quick Search"))) {
                Print(method, "Result not found");
                //throw new AssertFailedException(method + ": Result Not Found");
            } else {
                Print(method, "Result Found");
            }
        }
        private void OpenOrganizer() {
            method = MethodBase.GetCurrentMethod().Name;
            Print(method, " Started");
            m.Click(By.Name("Organizer"));
            Print(method, " Finished");
        }
        /**Creates a simple document that is recognized
         */
        private void CreateDocumentForRecognize(bool isPDF = true, string docPath = "", int? fileNumber = 0) {
            method = MethodBase.GetCurrentMethod().Name;
            Print(method, "Started");

            m.Click(By.Name("Add Document"));

            //add document button (+ icon)
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            m.Click(By.Id("lblType"));
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);
            action.MoveByOffset(20, -40).Click().MoveByOffset(20, 60).Click().Build().Perform();
            Print(method, "x: " + Cursor.Position.X + " y: " + Cursor.Position.Y);

            //find the document to add in file explorer
            //configure docpath in app.config, takes arg of pdf or tif 
            if (docPath.Length < 1) {
                docPath = ConfigurationManager.AppSettings.Get("AddDocumentStorage");
            }
            m.SendKeys(By.Id("1001"), docPath);
            Print(method, "Go to \"" + docPath + "\"");
            m.Click(By.Name("Go to \"" + docPath + "\""));

            var rand = new Random();
            if (isPDF) {
                Winium.Elements.Desktop.ComboBox filesOfType = new Winium.Elements.Desktop.ComboBox(m.Locate(By.Name("Files of type:")));
                filesOfType.SendKeys("p");
                filesOfType.SendKeys(OpenQA.Selenium.Keys.Enter);
                Thread.Sleep(500);
                if (fileNumber == 0) {
                    action.MoveToElement(m.Locate(By.Id(rand.Next(Directory.GetFiles(docPath, "*.pdf").Length).ToString()))).DoubleClick().Build().Perform();
                } else {
                    action.MoveToElement(m.Locate(By.Id(fileNumber.ToString()))).DoubleClick().Build().Perform();
                }
                m.Click(By.Name("Open"));
            } else {
                if (fileNumber == 0) {
                    action.MoveToElement(m.Locate(By.Id(rand.Next(Directory.GetFiles(docPath, "*.tif").Length).ToString()))).DoubleClick().Build().Perform();
                } else {
                    action.MoveToElement(m.Locate(By.Id(fileNumber.ToString()))).DoubleClick().Build().Perform();
                }
                m.Click(By.Name("Open"));
            }

            Print(method, "save and quit");
            m.Click(By.Id("btnSave"));
            m.Click(By.Id("btnClose"));
            Print(method, "Finished");
        }
        private void Print(string method, string toPrint) {
            debugLog.Info(method + " " + toPrint);
        }
    }
}
