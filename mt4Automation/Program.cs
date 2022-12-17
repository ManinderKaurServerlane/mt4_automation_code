using System;
using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;

namespace Mt4Automation
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxRetryCount = 20;
            bool exceptionRaised = false;
            while (maxRetryCount > 0)
            {
                String[] liquidityProviders = {
                    "2030125: Dharmesh Vinodray Kanabar",
                    "8001130: Dharmesh  Vinodray Kanabar",
                    "56487: Vardhak Ashok Kamdar",
                    "675233: Viplv Vijay Kumar",
                    "14628787: Viplv Vijaykumar Jain",
                    "2606012: Vardhak Ashok Kamdar",
                    "372798: Honor Investments LLC",
                    "1655839: Honor Investments LLC"
                };
                exceptionRaised = false;
                
                while (true)
                {
                    int i = 0;
                    if (exceptionRaised) break;

                    //Initialize application
                    var application = Application.Launch("C:\\Program Files (x86)\\MetaTrader ECN - FXOpen\\terminal.exe");
                    var mainWindow = application.GetMainWindow(new UIA3Automation());
                    ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                    
                    try
                    {
                        while (i < liquidityProviders.Length)
                        {
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
                            Process.Start("C:\\Users\\Administrator\\code\\autoit_mt4_automation\\checkEaAlert.exe");
                            Thread.Sleep(2000);
                            Process.Start("C:\\Users\\Administrator\\code\\autoit_mt4_automation\\eaConfirmation.exe");
                            i++;
                        }
                        application.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        exceptionRaised = true;
                        application.Close();
                        maxRetryCount--;
                        break;
                    }
                }
            }


        }
    }
}
