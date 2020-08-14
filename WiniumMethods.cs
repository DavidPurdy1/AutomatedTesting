using System.Diagnostics;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Winium;
using System;
using System.Reflection;

namespace ConsoleTests.src
{
    /// <summary>
    /// Class containing methods used to locate elements, interact with the form, and driver actions
    /// </summary>
    public class WiniumMethods
    {
        string method;
        readonly WiniumDriver driver;
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");

        public WiniumMethods(WiniumDriver driver)
        {
            this.driver = driver;
        }
        /// <summary>
        /// Used to locate elements on the page by name, id, or class name
        /// <return>Located Element</return>
        /// </summary>
        public IWebElement Locate(By by)
        {
            method = MethodBase.GetCurrentMethod().Name;
            try
            {
                IWebElement element = driver.FindElement(by);
                Print(method, by.ToString() + " Has been Located");
                return element;
            }
            catch (NoSuchElementException e)
            {
                Print(method, " Failed on " + method + " Finding element" + by.ToString() + e.StackTrace);
                throw new AssertFailedException("Failed on " + method + " Finding element" + by.ToString());
            }
        }
        /// <summary>
        /// Used to locate elements on the page by name, id, or class name specifying a parent element
        /// <return>Located Element</return>
        /// </summary>
        public IWebElement Locate(By by, IWebElement parent)
        {
            method = MethodBase.GetCurrentMethod().Name;
            try
            {
                IWebElement element = parent.FindElement(by);
                Print(method, by.ToString() + " Has been Located");
                return element;
            }
            catch (NoSuchElementException e)
            {
                Print(method, " Failed on " + method + " Finding element" + by.ToString() + e.StackTrace);
                throw new AssertFailedException("Failed on " + method + " Finding element" + by.ToString());
            }
        }
        public void Click(By by)
        {
            method = MethodBase.GetCurrentMethod().Name;
            try
            {
                var element = Locate(by);
                if (element.Enabled)
                {
                    element.Click();
                    Print(method, by.ToString() + " Clicked");
                }
            }
            catch (StaleElementReferenceException e)
            {
                Print(method, "Could not click element " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not click element " + by.ToString());
            }
            catch (ElementNotVisibleException e)
            {
                Print(method, "Could not click element: Not on Screen " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not click element: Not on Screen " + by.ToString());
            }
        }
        public void Click(By by, IWebElement parent)
        {
            method = MethodBase.GetCurrentMethod().Name;
            try
            {
                var element = Locate(by, parent);
                if (element.Enabled)
                {
                    element.Click();
                    Print(method, by.ToString() + " Clicked");
                }
            }
            catch (StaleElementReferenceException e)
            {
                Print(method, "Could not click element " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not click element " + by.ToString());
            }
            catch (ElementNotVisibleException e)
            {
                Print(method, "Could not click element: Not on Screen " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not click element: Not on Screen " + by.ToString());
            }
        }
        public void SendKeys(By by, string input)
        {
            method = MethodBase.GetCurrentMethod().Name;
            try
            {
                var element = Locate(by);
                if (element.Enabled)
                {
                    element.SendKeys(input);
                    Print(method, by.ToString() + " sent " + input);
                }
            }
            catch (StaleElementReferenceException e)
            {
                Print(method, "Could not send to element " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not send to element " + by.ToString());
            }
            catch (InvalidElementStateException e)
            {
                Print(method, " element is not able to recieve keys" + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + " element is not able to recieve keys" + by.ToString());
            }
            catch (ElementNotVisibleException e)
            {
                Print(method, "Could not send to element: Not on Screen " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not send element: Not on Screen " + by.ToString());
            }
        }
        public void SendKeys(By by, string input, IWebElement parent)
        {
            method = MethodBase.GetCurrentMethod().Name;
            try
            {
                var element = Locate(by, parent);
                if (element.Enabled)
                {
                    element.SendKeys(input);
                    Print(method, by.ToString() + " sent " + input);
                }
            }
            catch (StaleElementReferenceException e)
            {
                Print(method, "Could not send to element " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not send to element " + by.ToString());
            }
            catch (InvalidElementStateException e)
            {
                Print(method, " element is not able to recieve keys" + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + " element is not able to recieve keys" + by.ToString());
            }
            catch (ElementNotVisibleException e)
            {
                Print(method, "Could not send to element: Not on Screen " + by.ToString() + e.StackTrace);
                throw new AssertFailedException(method + "Could not send element: Not on Screen " + by.ToString());
            }
        }
        /// <summary> Checks if element is on the page by name, id, or class name </summary>
        /// <returns>Bool: If element is present</returns>
        public bool IsElementPresent(By by)
        {
            return driver.FindElements(by).Count > 0;
        }
        /// <summary> Checks if element is on the page by name, id, or class name specifying parent</summary>
        /// <returns>Bool: If element is present</returns>
        public bool IsElementPresent(By by, IWebElement parent)
        {
            return parent.FindElements(by).Count > 0;
        }
        /// <summary>Gets top level window: the only info type that can be specified is "process"</summary>
        /// <returns>string: the name of the process or title of the window</returns>
        public string GetTopLevelWindowInformation(string infotype)
        {
            WindowActions data = new WindowActions();
            data.GetActiveWindow();

            if (infotype.Equals("process"))
            {
                return data.WindowProcess;
            }
            return data.WindowTitle;
        }
        public void SetTopLevelWindow(Process p)
        {
            Print(MethodBase.GetCurrentMethod().Name, "Process: {" + p.ProcessName + "} ID: {" + p.Id + "} Window title: {" + p.MainWindowTitle + "}");
            new WindowActions().SetAsActiveWindow(p);
        }
        public Screenshot GetScreenshot()
        {
            return ((ITakesScreenshot)driver).GetScreenshot();
        }
        public void CloseDriver()
        {
            driver.Quit();
            Print(method, "DRIVER CLOSED");
        }
        private void Print(string method, string toPrint = "", Exception e = null)
        {
            debugLog.Info(method + " " + toPrint + " " + e);
        }
    }
}