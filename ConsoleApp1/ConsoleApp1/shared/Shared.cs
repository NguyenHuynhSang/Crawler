using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.shared
{
    public static class shared
    {
        public static bool IsElementExitst(this IWebDriver webDriver, By by)
        {
            try
            {
                webDriver.FindElement(by);
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Element does't exitst by: ", by.ToString());
                Console.WriteLine("Ex detail: ", ex.ToString());
            }
            return false;
        }
    }
}
