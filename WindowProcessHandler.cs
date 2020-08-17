using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleTests {
    /// <summary>
    /// Handles checks for processes running, closing processes, and getting and setting windows 
    /// </summary>
    public class WindowProcessHandler
    {
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        public string WindowTitle { get; set; }
        public string WindowProcess { get; set; }

        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(int hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        /// <summary>
        /// Gets the active window Process and Title
        /// </summary>
        public void GetActiveWindow()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);

            int handle = GetForegroundWindow();
            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                WindowTitle = Buff.ToString();

                GetWindowThreadProcessId(handle, out uint lpdwProcessId);

                WindowProcess = Process.GetProcessById((int)lpdwProcessId).ProcessName;
            }
        }
        /// <summary>
        /// Sets the active window Process and Title
        /// </summary>
        public void SetAsActiveWindow(Process p)
        {
            Print("Process: {" + p.ProcessName + "} ID: {" + p.Id + "} Window title: {" + p.MainWindowTitle + "}", "Setting as active Window");
            SetForegroundWindow(p.MainWindowHandle);
        }
        /// <summary>
        /// Closes the winium Desktop driver if it hasn't already
        /// </summary>
        public static void CloseExtraDriverInstances() {
            foreach (Process process in Process.GetProcesses()) {
                if (process.ProcessName.Contains("Winium.Desktop")) {
                    process.Kill();
                }
            }
        }
        /// <summary>
        /// Closes previous instances of Intact
        /// </summary>
        public static void CloseIntact() {
            foreach (Process p in Process.GetProcessesByName("Intact")) {
                p.Kill();
            }
        }
        ///<summary>
        ///<para>Default checks on call for both interruptions and for errors at end of testcase</para>
        ///<para>Throws AssertFail if there is an error present, Throws AssertInconclusive if Intact is not top window</para>
        ///</summary>s
        public void EndOfTestCheck(bool errorcheck = true, bool interuptcheck = true) {
            GetActiveWindow(); 
            CheckForIntactErrorMessage(errorcheck);
            CheckForInterruptions(interuptcheck);
        }
        private void CheckForInterruptions(bool toggleInterruptCheck) {
            if (toggleInterruptCheck) {
                if (WindowProcess != "Intact") {
                    Print( WindowProcess + " || " + WindowTitle);
                    Print("The current top window isn't intact, test interrupted");
                    throw new AssertInconclusiveException("The current top window is not intact, test interrupted");
                }
            }
        }
        private void CheckForIntactErrorMessage(bool toggleErrorCheck) {
            if (toggleErrorCheck) {
                foreach (Process process in Process.GetProcesses()) {
                    var title = process.MainWindowTitle;
                    if (title.Contains("Error") || title.Contains("Exception") || title.Contains("exception") || title.Contains("error")) {
                        SetAsActiveWindow(process); 
                        Print("Handler detects an error message:");
                        throw new AssertFailedException("Cleanup detects an error message");
                    }
                }
                Print("There is no error present");
            }
        }
        /// <summary>
        /// Gets rid of an error message so that the error message won't linger past that test case
        /// </summary>
        public static void DisposeErrorMessages() {
            foreach (Process process in Process.GetProcesses()) {
                var title = process.MainWindowTitle;
                if (title.Contains("Error") || title.Contains("Exception") || title.Contains("exception") || title.Contains("error")) {
                    process.Kill();
                }
            }
        }
        public void Print(string toPrint, string method = "") {
            debugLog.Info(method + " " + toPrint);
        }
    }
}