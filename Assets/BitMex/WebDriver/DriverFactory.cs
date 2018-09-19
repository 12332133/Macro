using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace Assets.BitMex.WebDriver
{
    public enum DriverType
    {
        Chrome = 0,
        FireFox = 1,
        Explore = 2,
    }

    public class DriverFactory
    {
        public static IWebDriver CreateDriver(DriverType type, string driverPath, bool hideCommandPromptWindow = false)
        {
            switch (type)
            {
                case DriverType.Chrome:
                    var service = ChromeDriverService.CreateDefaultService(driverPath);
                    service.HideCommandPromptWindow = hideCommandPromptWindow;
                    return new ChromeDriver(service);
                default:
                    throw new Exception();
            }
        }
    }


}
