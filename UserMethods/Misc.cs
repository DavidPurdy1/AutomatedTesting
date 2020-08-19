using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Reflection;
using System.Threading;

namespace AutomatedTestingLib {
    /// <summary>
    /// Methods that arent typically ran but are good for regression
    /// <para>Warning: Some of these are not entirely implemented</para>
    /// </summary>
    public class Misc {
        #region
        IWebElement window;
        readonly WiniumMethods m;
        string method = "";
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        readonly Actions action;
        #endregion
        public Misc(WiniumMethods m, Actions action) {
            this.m = m;
            this.action = action;
        }
        public void OpenOrganizer() {
            method = MethodBase.GetCurrentMethod().Name;
            Print(method, " Started");
            m.Click(By.Name("Organizer"));
            Print(method, " Finished");
        }

        //Uncommon and full functionality not added to some of these
        public void OpenUtil() {
            //Document indexing
            window = m.Locate(By.Name("&Administration"));
            window = m.Locate(By.Name("Utilities"), window);
            m.Click(By.Name("Index Documents..."), window);
            window = m.Locate(By.Id("frmDocumentIndexing"));
            m.Click(By.Id("btnFull"), window);
            m.Click(By.Id("btnClose"), window);

            //TODO:Fix expired documents another table
            window = m.Locate(By.Name("&Administration"));
            window = m.Locate(By.Name("Utilities"), window);
            m.Click(By.Name("View Expired Documents..."), window);
            m.Click(By.Id("Close"));

            //View licenses
            //window = m.Locate(By.Name("&Administration"));
            //window = m.Locate(By.Name("Utilities"), window);
            //m.Click(By.Name("View Licenses..."), window);
            //m.Click(By.Name("Close"));

            //Change Background image
            window = m.Locate(By.Name("&Administration"));
            window = m.Locate(By.Name("Utilities"), window);
            m.Click(By.Name("Change Background Image..."), window);
            Thread.Sleep(1500);
            window = m.Locate(By.Name("Select Client Background Image"), m.Locate(By.Id("frmIntactMain")));
            m.Click(By.Id("btnOK"), window);

            //Batch Recognize skip for now because of recognize test...

            //Settings Console
            window = m.Locate(By.Name("&Administration"));
            window = m.Locate(By.Name("Utilities"), window);
            m.Click(By.Name("Settings Console..."), window);
            m.Click(By.Id("&OK"));
            m.Click(By.Id("Close"));

            //Diagnostics Console
            window = m.Locate(By.Name("&Administration"));
            window = m.Locate(By.Name("Utilities"), window);
            m.Click(By.Name("Diagnostics Utility..."), window);
            m.Click(By.Id("Close"));

            //Refile Documents
            window = m.Locate(By.Name("&Administration"));
            window = m.Locate(By.Name("Utilities"), window);
            m.Click(By.Name("Refile Documents..."), window);
            m.Click(By.Id("Close"));

            //View Recognize Errors 
            window = m.Locate(By.Name("&Administration"));
            window = m.Locate(By.Name("Utilities"), window);
            m.Click(By.Name("View Recognize Errors..."), window);
            m.Click(By.Id("Close"));
        }
        public void AddToIPack() {
            OpenOrganizer();
            window = m.Locate(By.Name("DEFAULT DEF"));
            action.MoveToElement(window).ContextClick().Build().Perform();
            window = m.Locate(By.Name("DropDown"));
            m.Click(By.Name("Add to iPack..."));
            m.Click(By.Name("Yes"));
            m.Click(By.Id("btnNewIpack"));
            Thread.Sleep(1000);
            window = m.Locate(By.Id("frmNewiPack"));
            m.SendKeys(By.Name(""), "test" + new Random().Next().ToString(), window);
            m.Click(By.Id("rbnOK"));
            m.Click(By.Id("btnOK"));
            window = m.Locate(By.Name("&Intact"), m.Locate(By.Name("radMenu1")));
            m.Click(By.Name("iPack"), window);
            //m.Click(By.Id("rbnBatch"));
            Thread.Sleep(1000);
            window = m.Locate(By.Id("frmIntactMain"));
            if (m.IsElementPresent(By.Name("No documents were selected"), window)) {
                m.Click(By.Name("OK"));
            }
            Thread.Sleep(1000);
            m.Click(By.Id("radButton1"));
            Thread.Sleep(5000);
        }
        public void AuditTrail() {
            window = m.Locate(By.Name("&Intact"), m.Locate(By.Name("radMenu1")));
            m.Click(By.Name("Audit Trail"), window);
            window = m.Locate(By.Id("frmAuditTrail"));

            m.Click(By.Id("rcbDate"), window);
            m.Click(By.Id("rbnFind"), window);

            m.Click(By.Id("rbnClose"), window);
        }
        public void Portfolio(string[] definitionNames = null) {
            //not finished
            //open and name portfolio
            window = m.Locate(By.Name("&Administration"), m.Locate(By.Name("radMenu1")));
            m.Click(By.Name("Portfolios"), window);
            Thread.Sleep(1000);
            m.Click(By.Id("btnAdd"));
            window = m.Locate(By.Id("frmPortfolioEdit"));
            m.Click(By.Id("label1"), window);
            action.MoveByOffset(35, 0).Click().SendKeys("Portfolio" + new Random().Next().ToString()).Build().Perform();

            //definitions to add and adds them
            if (definitionNames != null) {
                foreach (string defName in definitionNames) {
                    m.Click(By.Name(defName));
                    m.Click(By.Id("btnAdd"));
                }
            } else {
                m.Click(By.Id("btnAdd"));
            }
            m.Click(By.Id("btnSave"), window);
            m.Click(By.Id("btnClose"), window);

        }
        public void Print(string method, string toPrint) {
            debugLog.Info(method + " " + toPrint);
        }
    }
}