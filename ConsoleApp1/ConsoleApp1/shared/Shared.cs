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
            catch (Exception)
            {

                Console.WriteLine("Element does't exitst by: ", by.ToString());
            }
            return false;
        }

        


    }
}
