using OpenQA.Selenium;
using System;
using System.Text.RegularExpressions;
using Assets.BitMex.WebDriver;

namespace Assets.BitMex
{
    public class BitMexDriverService
    {
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
        private const string XPathSymbol = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[3]/h4";
        private const string XPathMarketPrice = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[3]/ul/div/div/div[1]/span";
        private const string XPathPositionViewButton = "//*[@id=\"content\"]/div/span/div[2]/div/div/div[2]/div[1]/div[5]/section/header/span/div/ul/li[1]/span/span";
        private const string XPathActivatedOrderViewButton = "//*[@id=\"content\"]/div/span/div[2]/div/div/div[2]/div[1]/div[5]/section/header/span/div/ul/li[3]/span/span";
        private const string XPathOderTargetMarketButton = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[1]/ul/div/div/ul/li[2]/span/span";
        private const string XPathOderTargetSpecifedButton = "//*[@id=\"content\"]/div/span/div[1]/div/div/li[1]/ul/div/div/ul/li[1]/span/span";
        private const string XPathViewTable = "//*[@id=\"content\"]/div/span/div[2]/div/div/div[2]/div[1]/div[5]/section/div/div/div[2]/table/tbody";
        private const string XPatchSumCashedXBT = "//*[@id=\"header\"]/div[2]/a[1]/span/table/tbody/tr[1]/td[2]";

        private const string XPathLoginAccount = "//*[@id=\"header\"]/div[2]/div[3]/a/span[1]/span[1]";

        private IWebDriver driver;
        private string url;
        private BitMexCommandExecutor executor;
        private BitMexCommandRepository repository;


        public BitMexCommandRepository Repository
        {
            get
            {
                return this.repository;
            }
        }

        public BitMexCommandExecutor Executor
        {
            get
            {
                return this.executor;
            }
        }

        public BitMexDriverService()
        {
            this.executor = new BitMexCommandExecutor();
            this.repository = new BitMexCommandRepository();
        }

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

                this.executor.Stop();
                this.driver.Quit();
                this.driver.Dispose();
                this.driver = null;
            }
            catch (System.Exception)
            {
            }
        }

        public bool HandleBuy()
        {
            var elementBuy = this.driver.SafeFindElement(By.CssSelector(CssBuyButton));
            if (elementBuy.Enabled == false)
                return false;

            elementBuy.Click();

            HandleClearOrderConfirmationWindow();
            return true;
        }

        public bool HandleSell()
        {
            var elementSell = this.driver.SafeFindElement(By.CssSelector(CssSellButton));
            if (elementSell.Enabled == false)
                return false;

            elementSell.Click();

            HandleClearOrderConfirmationWindow();
            return true;
        }

        public string HandleGetCurrentSymbol()
        {
            var elementSymbol = driver.FindElement(By.XPath(XPathSymbol));
            if (elementSymbol == null)
            {
                throw new BitMexDriverServiceException();
            }

            return elementSymbol.Text.Split(':')[1].Trim();
        }

        public int HandleGetCurrentPositionCount(string symbol)
        {
            var elementPositionView = driver.SafeFindElement(By.XPath(XPathPositionViewButton));
            elementPositionView.Click();

            var elementViewTable = driver.SafeFindElement(By.XPath(XPathViewTable));

            var elements = elementViewTable.SafeFindElements(By.TagName("tr"));

            foreach (var element in elements)
            {
                var elementSymbol = element.SafeFindElement(By.ClassName("symbol"), false);
                if (elementSymbol == null)
                {
                    return 0;
                }

                if (elementSymbol.Text.Equals(symbol) == true)
                {
                    var elementCurrentQty = element.SafeFindElement(By.ClassName("currentQty"));
                    return Int32.Parse(elementCurrentQty.Text);
                }
            }

            return 0;
        }

        private decimal HandleGetCrossSelectionMaxLeverage()
        {                     
            var element = driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[3]/div/div/div/div[4]"));
            var crossSelections = Regex.Split(element.Text, "\r\n");
            var maxValue = crossSelections[crossSelections.Length - 1].Split('x')[0];
            return decimal.Parse(maxValue);
        }

        public void HandleForceChangeFocus(string xPath)
        {
            // 실시간 매크로 매수/매도 시 현재 보고있는 주문타입이 다를경우 강재로 이동시켜줌 
            // 스퀘줄 구매 동일 동작 
            var elementOrderTarget = driver.SafeFindElement(By.XPath(xPath));
            elementOrderTarget.Click();
        }

        public bool HandleIsSameOrderTap(string xPath)
        {
            return true;
        }

        public bool HandleOrderMarketQty(decimal qty, int magnification, decimal fixedAvailableXbt, string symbol)
        {
            // 강제로 주문 탭 전환 
            HandleForceChangeFocus(XPathOderTargetMarketButton);
            // 실시간 매크로 시 주문 탭이 다를경우 무시하려면 수정 필요 
            //HandleIsSameOrderTap(XPathOderTargetMarketButton);

            decimal seletedQty = qty;

            // 수량 미지정시 자동계산 
            if (seletedQty == 0)
            {
                var position = HandleGetCurrentPositionCount(symbol);

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
                        var slider = driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[3]/div/div/div/div[3]"), false);
                        if (slider == null)
                        {
                            return false;
                        }

                        var index = slider.GetAttribute("aria-valuenow");
                        if (Int32.Parse(index) == 0)
                        {
                            leverage = HandleGetCrossSelectionMaxLeverage();
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

        public bool HandleOrderSpecifiedQty(decimal qty, int magnification, decimal specifiedAditional, decimal fixedAvailableXbt, string symbol)
        {
            // 강제로 주문 탭 전환
            HandleForceChangeFocus(XPathOderTargetSpecifedButton);
            // 실시간 매크로 시 주문 탭이 다를경우 무시하려면 수정 필요 
            //HandleIsSameOrderTap(XPathOderTargetSpecifedButton);

            var elementMarketPrice = driver.SafeFindElement(By.XPath(XPathMarketPrice));
            var price = decimal.Parse(elementMarketPrice.Text) + specifiedAditional;

            decimal seletedQty = qty;

            //수량 미지정시 자동계산
            if (seletedQty == 0)
            {
                var position = HandleGetCurrentPositionCount(symbol);

                if (position == 0)
                {
                    // 고정 xbt
                    var xbt = fixedAvailableXbt;

                    if (xbt == 0)
                    {
                        var elementRemainXBT = driver.SafeFindElement(By.XPath("//*[@id=\"header\"]/div[2]/a[1]/span/table/tbody/tr[2]/td[2]"));
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
                            leverage = HandleGetCrossSelectionMaxLeverage();
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

        public void HandleClearPosition(string symbol, bool isClear = false)
        {
            // 포지션 창 클릭
            var elementOrderTarget = driver.SafeFindElement(By.XPath(XPathPositionViewButton));
            elementOrderTarget.Click();

            // 포지션 정보
            var elementTable = driver.SafeFindElement(By.XPath(XPathViewTable));
            var elementPositions = elementTable.SafeFindElements(By.TagName("tr"));

            if (elementPositions.Count > 0)
            {
                foreach (var elementPosition in elementPositions)
                {
                    var elementSymbol = elementPosition.SafeFindElement(By.ClassName("symbol"));

                    if (elementSymbol.Text.Equals(symbol) == true)
                    {
                        var elementActions = elementPosition.SafeFindElement(By.ClassName("actions"));
                        var elementCancle = elementActions.SafeFindElement(By.CssSelector("div.btn.btn-danger.btn-sm"));
                        elementCancle.Click();
                        
                        var elementClearConfirmation = this.driver.SafeFindElement(By.CssSelector("button.btn-lg.btn.btn-primary"), false);
                        if (elementClearConfirmation == null)
                        {
                            return;
                        }
                        elementClearConfirmation.Click();
                        break;
                    }
                }
            }
        }

        public void HandleCancleActivatedOrders(string symbol, bool isClear = false)
        {
            // 주문 창 클릭
            HandleForceChangeFocus(XPathActivatedOrderViewButton);

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
            var marketPrice = this.driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/div[1]/div/span[2]/span[1]/span[1]"));
            return decimal.Parse(marketPrice.Text);
        }

        public void HandleClearOrderConfirmationWindow()
        {
            var elementOrderConfirmation = this.driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span[2]/div/section/div/div[4]/button[2]"), false);
            if (elementOrderConfirmation == null)
            {
                return;
            }
            elementOrderConfirmation.Click();
        }

        public bool IsInvaildEmail(string email)
        {
            //var elementEmail = this.driver.SafeFindElement(By.XPath("//*[@id=\"header\"]/div[2]/div[3]/a/span[1]/span[1]"), false);
            //if (elementEmail == null)
            //{
            //    return false;
            //}
            //return elementEmail.Text.Equals(email);
            return true;
        }
    }
}
