using Assets.BitMex.WebDriver;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexDriverServiceHandler
    {
        private const string CssBuyButton = "button.btn-lg.btn.btn-block.btn-success.buy";
        private const string CssSellButton = "button.btn-lg.btn.btn-block.btn-danger.sell";
        private const string XPathOrderMarkerQtyTextBox = "//*[@id=\"orderQty\"]";
        private const string XPathOrderSpecifiedQtyTextBox = "//*[@id=\"orderQty\"]";
        private const string XPathOrderSpecifiedPriceTextBox = "//*[@id=\"price\"]";
        private const string XPathSymbol = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/h4";
        private const string XPathMarketPrice = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[3]/ul/div/div/div[1]/span";
        private const string XPathPositionViewButton = "//*[@id=\"content\"]/div/span/div[2]/div/div/div[2]/div[1]/div[5]/section/header/span/div/ul/li[1]/span/span";
        private const string XPathActivatedOrderViewButton = "//*[@id=\"content\"]/div/span/div[2]/div/div/div[2]/div[1]/div[5]/section/header/span/div/ul/li[3]/span/span";
        private const string XPathOderTargetMarketButton = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[1]/ul/div/div/ul/li[2]/span/span";
        private const string XPathOderTargetSpecifedButton = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[1]/ul/div/div/ul/li[1]/span/span";
        private const string XPathViewTable = "//*[@id=\"content\"]/div/span/div[2]/div/div/div[2]/div[1]/div[5]/section/div/div/div[2]/table/tbody";
        private const string XPatchSumCashedXBT = "//*[@id=\"header\"]/div[2]/a[1]/span/table/tbody/tr[1]/td[2]";

        private const string XPathLoginAccount = "//*[@id=\"header\"]/div[2]/div[3]/a/span[1]/span[1]";

        private IWebDriver driver;

        public BitMexDriverServiceHandler(IWebDriver driver)
        {
            this.driver = driver;
        }

        public bool HandleBuy()
        {
            var elementBuy = this.driver.SafeFindElement(By.CssSelector(CssBuyButton));
            if (elementBuy == null)
            {
                return false;
            }
            if (elementBuy.Enabled == false)
            {
                return false;
            }

            elementBuy.Click();
            return true;
        }

        public bool HandleSell()
        {
            var elementSell = this.driver.FindElement(By.CssSelector(CssSellButton));
            if (elementSell == null)
            {
                return false;
            }
            if (elementSell.Enabled == false)
            {
                return false;
            }

            elementSell.Click();
            return true;
        }

        public string HandleGetCurrentSymbol()
        {
            var elementSymbol = driver.SafeFindElement(By.XPath(XPathSymbol));
            if (elementSymbol == null)
            {
                return string.Empty;
            }

            return elementSymbol.Text.Split(':')[1].Trim();
        }
    }
}
