using System;
using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;

namespace Mt4Automation
{
    class Program
    { 
        static void Main(string[] args)
        {
            int maxRetryCount = int.MaxValue;
            bool exceptionRaised = false;
            String[] liquidityProviders = {
                        //"8001130: Dharmesh  Vinodray Kanabar",
                        "56487: Vardhak Ashok Kamdar",
                        "2030125: Dharmesh Vinodray Kanabar",
                        "675233: Viplv Vijay Kumar",
                        "14628787: Viplv Vijaykumar Jain",
                        "2606012: Vardhak Ashok Kamdar",
                        "372798: Honor Investments LLC",
                        "1655839: Honor Investments LLC"
                    };
            while (maxRetryCount > 0)
            {
                //Initialize application
                Application? application = Application.Launch("C:\\Program Files (x86)\\MetaTrader ECN - FXOpen\\terminal.exe");
                using UIA3Automation automation = new UIA3Automation();
                Window mainWindow = application.GetMainWindow(automation);

                AutomationElement root = automation.GetDesktop();
                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                AutomationElement loginWindow = root.FindFirstDescendant(cf.ByClassName("MetaQuotes::MetaTrader::4.00"));

                string checkEaAlertProcessPath = "D:\\serverlane\\autoit_mt4_automation\\checkEaAlert.exe";
                string eaConfirmationProcessPath = "D:\\serverlane\\autoit_mt4_automation\\eaConfirmation.exe";
                exceptionRaised = false;

                try
                {
                    for (int i = 0; i < liquidityProviders.Length; i++)
                    {
                        if (exceptionRaised) break;

                        string lpTrying = new String(liquidityProviders[i].Where(Char.IsDigit).ToArray());
                        string loggedLp = new String(loginWindow.Name.Where(Char.IsDigit).ToArray());

                        if (loggedLp.Contains(lpTrying))
                        {
                            continue;
                        }

                        Thread.Sleep(10000);
                        mainWindow.FindFirstDescendant(cf.ByName("OK")).AsButton().Click();
                        Thread.Sleep(1000);
                        mainWindow.FindFirstDescendant(cf.ByName(liquidityProviders[i])).AsButton().DoubleClick();
                        Thread.Sleep(2000);
                        mainWindow.FindFirstDescendant(cf.ByName("Login")).AsButton().Click();
                        Thread.Sleep(8000);
                        mainWindow.FindFirstDescendant(cf.ByName("OK")).AsButton().Click();
                        mainWindow.FindFirstDescendant(cf.ByName("Pytrader_MT4_EA_V2.07")).AsButton().DoubleClick();
                        Thread.Sleep(2000);
                        Process.Start(checkEaAlertProcessPath);
                        Thread.Sleep(2000);
                        Process.Start(eaConfirmationProcessPath);
                    }

                    Program? program = new();
                    program.KillProcess(checkEaAlertProcessPath);
                    program.KillProcess(eaConfirmationProcessPath);
                    program = null;
                    application?.Kill();
                    application?.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    application?.Kill();
                    application?.Dispose();
                    exceptionRaised = true;
                    maxRetryCount--;
                }
                finally
                {
                    Thread.Sleep(180 * 1000); //3 minutes wait for UI automation memory leak
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                }

            }

        }
        
        public void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    
    }
}
