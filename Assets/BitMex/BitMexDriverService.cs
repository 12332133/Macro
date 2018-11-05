using OpenQA.Selenium;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexDriverService
    {
        private IWebDriver driver;
        private string url;

        private const string SymbolXbtUsd = "XBTUSD";
        private const string SymbolXBTJPY = "XBTJPY";
        private const string SymbolADAM18 = "ADAM18";
        private const string SymbolBCHM18 = "BCHM18";
        private const string SymbolETHXBT = "ETHXBT";
        private const string SymbolLTCM18 = "LTCM18";

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

        public void SetDriver(IWebDriver driver, string url)
        {
            this.url = url;
            this.driver = driver;
        }

        public void OpenService(IWebDriver driver, string url)
        {
            CloseDriver();
            SetDriver(driver, url);

            this.driver.Navigate().GoToUrl(this.url);
        }

        public bool IsDriverOpen()
        {
            if (this.driver == null)
            {
                try
                {
                    return false;
                }
                catch (System.Exception)
                {
                    CloseDriver();
                    return false;
                }
            }

            return true;
        }

        public void CloseDriver()
        {
            try
            {
                if (this.driver == null)
                    return;

                this.driver.Quit();
                this.driver.Dispose();
                this.driver = null;
            }
            catch (System.Exception)
            {
            }
        }

        public bool OperationBuy()
        {
            var element = this.driver.FindElement(By.CssSelector(CssBuyButton));

            if (element.Enabled == false)
                return false;

            element.Click();
            return true;
        }

        public bool OperationSell()
        {
            var element = this.driver.FindElement(By.CssSelector(CssSellButton));

            if (element.Enabled == false)
                return false;

            element.Click();
            return true;
        }

        public string OperationGetCurrentSymbol()
        {
            var element = driver.FindElement(By.XPath(XPathSymbol));
            return element.Text.Split(':')[1].Trim();
        }

        public int OperationCurrentPositionCount(string symbol)
        {
            driver.FindElement(By.XPath(XPathPositionViewButton)).Click();

            var elements = driver.FindElement(By.XPath(XPathViewTable)).FindElements(By.TagName("tr"));

            if (elements.Count > 0)
            {
                foreach (var element in elements)
                {
                    IWebElement elementSymbol = null;

                    try
                    {
                        elementSymbol = element.FindElement(By.ClassName("symbol"));
                    }
                    catch (System.Exception)
                    {
                        return 0;
                    }

                    if (elementSymbol.Text.Equals(symbol) == true)
                    {
                        return Int32.Parse(element.FindElement(By.ClassName("currentQty")).Text);
                    }
                }
            }

            return 0;
        }

        private decimal GetCrossSelectionMaxLeverage()
        {
            var element = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[3]/div/div/div/div[4]"));
            var crossSelections = Regex.Split(element.Text, "\r\n");
            var maxValue = crossSelections[crossSelections.Length - 1].Split('x')[0];
            return decimal.Parse(maxValue);
        }

        public bool OperationOrderMarketQty(decimal qty, int magnification, decimal fixedAvailableXbt, string symbol)
        {
            driver.FindElement(By.XPath(XPathOderTargetMarketButton)).Click();

            decimal seletedQty = qty;

            // 수량 미지정시 자동계산 
            if (seletedQty == 0)
            {
                var position = OperationCurrentPositionCount(symbol);

                if (position == 0)
                {
                    // 고정 xbt
                    var xbt = fixedAvailableXbt;

                    if (xbt == 0)
                    {
                        var elementRemainXBT = driver.FindElement(By.XPath("//*[@id=\"header\"]/div[2]/a[1]/span/table/tbody/tr[2]/td[2]"));
                        xbt = decimal.Parse(elementRemainXBT.Text.Split(' ')[0]);
                    }

                    //교차 선택
                    decimal leverage = 0;
                    try
                    {
                        var slider = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[3]/div/div/div/div[3]"));
                        var index = slider.GetAttribute("aria-valuenow");

                        if (Int32.Parse(index) == 0)
                        {
                            leverage = GetCrossSelectionMaxLeverage();
                        }
                        else
                        {
                            var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                            leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0]);
                        }
                    }
                    catch
                    {
                        var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                        leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0]);
                    }

                    var elementMarketPrice = driver.FindElement(By.XPath(XPathMarketPrice));
                    var price = decimal.Parse(elementMarketPrice.Text);

                    if (symbol.Equals(SymbolXbtUsd) == true) // bitcoin usd only differnt algo
                    {
                        seletedQty = Math.Floor(xbt * leverage * price * ((decimal)magnification / 100));
                    }
                    else
                    {
                        seletedQty = Math.Floor(xbt * leverage * ((decimal)magnification / 100) / price);
                    }
                }
                else
                {
                    seletedQty = Math.Floor(position * ((decimal)magnification / 100));
                }

                if (seletedQty <= 0)
                    return false;
            }

            var elementOderQty = this.driver.FindElement(By.XPath(XPathOrderMarkerQtyTextBox));
            elementOderQty.Clear();
            elementOderQty.SendKeys(seletedQty.ToString());

            return true;
        }

        public bool OperationOrderSpecifiedQty(decimal qty, int magnification, decimal specifiedAditional, decimal fixedAvailableXbt, string symbol)
        {
            driver.FindElement(By.XPath(XPathOderTargetSpecifedButton)).Click();

            var elementMarketPrice = driver.FindElement(By.XPath(XPathMarketPrice));
            var price = decimal.Parse(elementMarketPrice.Text) + specifiedAditional;

            decimal seletedQty = qty;

            //수량 미지정시 자동계산
            if (seletedQty == 0)
            {
                var position = OperationCurrentPositionCount(symbol);

                if (position == 0)
                {
                    // 고정 xbt
                    var xbt = fixedAvailableXbt;

                    if (xbt == 0)
                    {
                        var elementRemainXBT = driver.FindElement(By.XPath("//*[@id=\"header\"]/div[2]/a[1]/span/table/tbody/tr[2]/td[2]"));
                        xbt = decimal.Parse(elementRemainXBT.Text.Split(' ')[0]);
                    }

                    //교차 선택
                    decimal leverage = 0;
                    try
                    {
                        var slider = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[3]/div/div/div/div[3]"));
                        var index = slider.GetAttribute("aria-valuenow");

                        if (Int32.Parse(index) == 0)
                        {
                            leverage = GetCrossSelectionMaxLeverage();
                        }
                        else
                        {
                            var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                            leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0]);
                        }
                    }
                    catch
                    {
                        var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                        leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0]);
                    }

                    if (symbol.Equals(SymbolXbtUsd) == true) // bitcoin usd only differnt algo
                    {
                        seletedQty = Math.Floor(xbt * leverage * price * ((decimal)magnification / 100));
                    }
                    else
                    {
                        seletedQty = Math.Floor(xbt * leverage * ((decimal)magnification / 100) / price);
                    }
                }
                else
                {
                    seletedQty = Math.Floor(position * ((decimal)magnification / 100));
                }

                if (seletedQty <= 0)
                    return false;
            }

            var elementOderQty = this.driver.FindElement(By.XPath(XPathOrderSpecifiedQtyTextBox));
            elementOderQty.Clear();
            elementOderQty.SendKeys(seletedQty.ToString());

            var elementOderPrice = this.driver.FindElement(By.XPath(XPathOrderSpecifiedPriceTextBox));
            elementOderPrice.Clear();
            elementOderPrice.SendKeys(price.ToString());

            return true;
        }

        public void OperationClearPosition(string symbol, bool isClear = false)
        {
            driver.FindElement(By.XPath(XPathPositionViewButton)).Click();

            var table = driver.FindElement(By.XPath(XPathViewTable));

            var elements = table.FindElements(By.TagName("tr"));

            if (elements.Count > 0)
            {
                foreach (var element in elements)
                {
                    if (element.FindElement(By.ClassName("symbol")).Text.Equals(symbol) == true)
                    {
                        element.FindElement(By.ClassName("actions")).FindElement(By.CssSelector("div.btn.btn-danger.btn-sm")).Click();
                        break;
                    }
                }
            }
        }

        public void OperationCancleActivatedOrders(string symbol, bool isClear = false)
        {
            driver.FindElement(By.XPath(XPathActivatedOrderViewButton)).Click();

            var table = driver.FindElement(By.XPath(XPathViewTable));

            var elements = table.FindElements(By.TagName("tr"));

            if (elements.Count > 0)
            {
                if (isClear == false)
                {
                    foreach (var element in elements)
                    {
                        if (element.FindElement(By.ClassName("symbol")).Text.Equals(symbol) == true)
                        {
                            var elementStatus = element.FindElement(By.ClassName("ordStatus"));

                            if (elementStatus.Text.Equals("New") || elementStatus.Text.Equals("Partially Filled"))
                            {
                                element.FindElement(By.ClassName("actions")).Click();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var element = elements[0];
                    element.FindElement(By.ClassName("actions")).Click();
                }
            }
        }

        public decimal OperationGetMarketPrice(string symbol = SymbolXbtUsd)
        {
            var marketPrice = this.driver.FindElement(By.XPath("//*[@id=\"content\"]/div/div[1]/div/span[2]/span[1]/span[1]"));
            return decimal.Parse(marketPrice.Text);
        }

        //public bool IsLoginBitMexAccount()
        //{
        //    try
        //    {
        //        this.driver.FindElement(By.XPath(BitMexService.XPathLoginAccount));
        //        return true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public bool IsInvaildEmail(string email)
        {
            try
            {
                var elementEmail = this.driver.FindElement(By.XPath("//*[@id=\"header\"]/div[2]/div[3]/a/span[1]/span[1]"));
                return elementEmail.Text.Equals(email);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public string GetLoginBitMexAccountEmail()
        {
            try
            {
                var elementEmail = this.driver.FindElement(By.XPath("//*[@id=\"header\"]/div[2]/div[3]/a/span[1]/span[1]"));
                return elementEmail.Text;
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        //public bool IsLoginBitMex()
        //{
        //    try
        //    {
        //        this.driver.FindElement(By.XPath(BitMexService.XPatchSumCashedXBT));
        //        return true;
        //    }
        //    catch (System.Exception)
        //    {
        //        return false;
        //    }
        //}
    }
}
