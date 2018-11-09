using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Assets.BitMex.WebDriver
{
    public interface IDriverBuilder
    {
        IDriverBuilder HideCommandPromptWindow(bool isHide);
        IWebDriver build();
    }

    public class ChromeDriverBuilder : IDriverBuilder
    {
        private ChromeDriverService service;

        public ChromeDriverBuilder(string driverPath)
        {
            this.service = ChromeDriverService.CreateDefaultService(driverPath);
        }

        public IDriverBuilder HideCommandPromptWindow(bool isHide)
        {
            this.service.HideCommandPromptWindow = isHide;
            return this;
        }

        public IWebDriver build()
        {
            return new ChromeDriver(this.service);
        }
    }

    public class FireFoxDriverBuilder : IDriverBuilder
    {
        public IDriverBuilder HideCommandPromptWindow(bool isHide)
        {
            throw new NotImplementedException();
        }

        public IWebDriver build()
        {
            throw new NotImplementedException();
        }
    }
}
