using OpenQA.Selenium;
using System;
using System.Text.RegularExpressions;
using Assets.BitMex.WebDriver;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using Assets.BitMex.Commands;

namespace Assets.BitMex
{
    public class BitMexDriverService : IBitMexCommandHandler
    {
        public const string MainSymbol = "XBTUSD";

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

        private string url;
        private IWebDriver driver;
        private BitMexCommandExecutor executor;
        private BitMexCommandRepository repository;
        private BitMexCoinTable coinTable;

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

        public BitMexCoinTable CoinTable
        {
            get
            {
                return this.coinTable;
            }
        }

        public BitMexDriverService()
        {
            this.executor = new BitMexCommandExecutor();
            this.coinTable = new BitMexCoinTable();
            this.repository = new BitMexCommandRepository();
        }

        public void SetDriver(IWebDriver driver, string url)
        {
            this.url = url;
            this.driver = driver;
        }

        public void OpenService(IWebDriver driver, string url)
        {
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
                this.executor.Stop();

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
            var elementSymbol = driver.SafeFindElement(By.XPath(XPathSymbol));
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

        public decimal HandleGetCrossSelectionMaxLeverage()
        {                     
            var element = driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[3]/div/div/div/div[4]"));
            var crossSelections = Regex.Split(element.Text, "\r\n");
            var maxValue = crossSelections[crossSelections.Length - 1].Split('x')[0];
            return decimal.Parse(maxValue, System.Globalization.NumberStyles.Any);
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

        /// <summary>
        /// 시장가 주문
        /// </summary>
        /// <param name="qty">수량</param>
        /// <param name="magnification">배수 0~100</param>
        /// <param name="fixedAvailableXbt">고정xbt</param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HandleOrderMarketQty(decimal qty, int magnification, decimal fixedAvailableXbt, string symbol)
        {
            // 강제로 주문 탭 전환 
            //HandleForceChangeFocus(XPathOderTargetMarketButton);
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
                        xbt = decimal.Parse(elementRemainXBT.Text.Split(' ')[0], System.Globalization.NumberStyles.Any);
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

                        var index = slider.SafeGetAttribute("aria-valuenow");
                        if (Int32.Parse(index) == 0)
                        {
                            leverage = HandleGetCrossSelectionMaxLeverage();
                        }
                        else
                        {                                                              
                            var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                            leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0], System.Globalization.NumberStyles.Any);
                        }
                    }
                    catch
                    {
                        var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                        leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0], System.Globalization.NumberStyles.Any);
                    }

                    var elementMarketPrice = driver.FindElement(By.XPath(XPathMarketPrice));
                    var price = decimal.Parse(elementMarketPrice.Text, System.Globalization.NumberStyles.Any);

                    if (symbol.Equals(BitMexDriverService.MainSymbol) == true) // bitcoin usd only differnt algo
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
            //HandleForceChangeFocus(XPathOderTargetSpecifedButton);
            // 실시간 매크로 시 주문 탭이 다를경우 무시하려면 수정 필요 
            //HandleIsSameOrderTap(XPathOderTargetSpecifedButton);

            var elementMarketPrice = driver.SafeFindElement(By.XPath(XPathMarketPrice));
            var price = decimal.Parse(elementMarketPrice.Text, System.Globalization.NumberStyles.Any) + specifiedAditional;

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
                        var index = slider.SafeGetAttribute("aria-valuenow");

                        if (Int32.Parse(index) == 0)
                        {
                            leverage = HandleGetCrossSelectionMaxLeverage();
                        }
                        else
                        {
                            var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                            leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0], System.Globalization.NumberStyles.Any);
                        }
                    }
                    catch
                    {
                        var elementSelectedLeverage = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/span/div[1]/div/div/li[2]/ul/div/div/div[2]/div/div/span"));
                        leverage = decimal.Parse(elementSelectedLeverage.Text.Split('x')[0], System.Globalization.NumberStyles.Any);
                    }

                    if (symbol.Equals(BitMexDriverService.MainSymbol) == true) // bitcoin usd only differnt algo
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
                    if (elementPosition.SafeFindElement(By.ClassName("symbol")).Text.Equals(symbol) == true)
                    {
                        var elementActions = elementPosition.SafeFindElement(By.ClassName("actions"));
                        var elementCancle = elementActions.SafeFindElement(By.CssSelector("div.btn.btn-primary.btn-sm"));
                        elementCancle.Click();
                        break;
                    }
                }
            }
        }

        public void HandleCancleActivatedOrders(string symbol, bool isClear = false)
        {
            // 주문 창 클릭
            var elementOrderTarget = driver.SafeFindElement(By.XPath(XPathActivatedOrderViewButton));
            elementOrderTarget.Click();

            var table = driver.SafeFindElement(By.XPath(XPathViewTable));

            var elements = table.SafeFindElements(By.TagName("tr"));

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

        //public decimal OperationGetMarketPrice(string symbol = BitMexDriverService.MainSymbol)
        //{
        //    var marketPrice = this.driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/div[1]/div/span[2]/span[1]/span[1]"));
        //    return decimal.Parse(marketPrice.Text);
        //}

        public void HandleClearOrderConfirmationWindow()
        {
            var elementOrderConfirmation = this.driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span[2]/div/section/div/div[4]/button[2]"), false);
            if (elementOrderConfirmation == null)
            {
                return;
            }
            elementOrderConfirmation.Click();
        }

        public bool IsAuthenticatedAccount(string email)
        {
            var elementEmail = this.driver.SafeFindElement(By.CssSelector("span.visible-lg-inline-block.visible-sm-inline-block"), false);
            if (elementEmail == null)
            {
                return false;
            }
            return elementEmail.Text.Equals(email);
        }

        public bool HandleIsTradingPage()
        {
            return this.driver.SafeFindElement(By.CssSelector("span.visible-lg-inline-block.visible-sm-inline-block"), false) != null;
        }

        //public void HandleChangeCoinTab(string targetSymbol)
        //{
        //    var targetUrl = this.url + "/app/trade/"+ targetSymbol;
        //    if (this.driver.Url.Equals(targetUrl) == false)
        //    {
        //        this.driver.Navigate().GoToUrl(targetUrl);
        //    }
        //}

        public bool HandleChangeCoinTab(string rootCoinName, string coinName)
        {
            var elementCoinTabs = driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span/div[2]/div/div/div[1]/div/section/header/span/div/ul"));
            var elementRootCoins = elementCoinTabs.SafeFindElements(By.TagName("li"));
            foreach (var elementRootCoin in elementRootCoins)
            {
                var rootSymbol = elementRootCoin.SafeFindElement(By.ClassName("instrumentRootSymbol")).Text;
                if (rootCoinName.Equals(rootSymbol) == true)
                {
                    return elementRootCoin.SafeChainingClick(() =>
                    {
                        var elementExpiries = elementCoinTabs.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span/div[2]/div/div/div[1]/div/section/div/span"), false);
                        var elementChildCoins = elementExpiries.SafeFindElements(By.TagName("div"));
                        foreach (var elementChildCoin in elementChildCoins)
                        {
                            var childSymbol = elementChildCoin.SafeGetAttribute("title");
                            if (childSymbol.Equals(coinName) == true)
                            {
                                elementChildCoin.Click();
                                return true;
                            }
                        }

                        return false;
                    });

                    //elementRootCoin.Click();

                    //var elementExpiries = elementCoinTabs.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/span/div[2]/div/div/div[1]/div/section/div/span"), false);
                    //var elementChildCoins = elementExpiries.SafeFindElements(By.TagName("div"));
                    //foreach (var elementChildCoin in elementChildCoins)
                    //{
                    //    var childSymbol = elementChildCoin.SafeGetAttribute("title");
                    //    if (childSymbol.Equals(coinName) == true)
                    //    {
                    //        elementChildCoin.Click();
                    //        return true;
                    //    }
                    //}
                }
            }

            return false;
        }

        public void HandleSyncCointPrices() //bench 0.07s
        {
            var elementCoinsSection = driver.SafeFindElement(By.CssSelector("span.instruments.tickerBarSection"));
            var innerCoinText = elementCoinsSection.SafeGetAttribute("innerText");
            var splitTexts = new[] { "%", "-", "+" };

            foreach (var coin in this.CoinTable.Coins)
            {
                var length = innerCoinText.IndexOf(coin.Key);

                if (length == -1)
                {
                    continue;
                }

                var remainText = innerCoinText.Substring(length + coin.Key.Length);
                var elements = remainText.Split(splitTexts, StringSplitOptions.RemoveEmptyEntries);
                var price = elements[0];
                if (price.Equals(".") == true)
                {
                    price = "0";
                }

                coin.Value.MarketPrice = decimal.Parse(price, System.Globalization.NumberStyles.Any);
            }
        }

        //public List<KeyValuePair<string, string>> HandleGetCoinPrices()
        //{
        //    var coins = new List<KeyValuePair<string, string>>();
        //    var elementExpenedSelector = driver.SafeFindElement(By.XPath("//*[@id=\"content\"]/div/div[1]/div/i[2]"));
        //    if (elementExpenedSelector.SafeGetAttribute("class").Equals("expand fa fa-fw fa-angle-double-up") == true)
        //    {
        //        var elementCoinPrice = driver.SafeFindElement(By.CssSelector("span.instruments.tickerBarSection"), false);
        //        if (elementCoinPrice != null)
        //        {
        //            int innerCount = 0;
        //            string innerText = string.Empty;
        //            string marketPrice = string.Empty;

        //            var elementCoinPrices = elementCoinPrice.SafeFindElements(By.TagName("span"));
        //            foreach (var elementCoin in elementCoinPrices)
        //            {
        //                switch (innerCount)
        //                {
        //                    case 0://0 = coinname + price + chageper 
        //                        innerText = elementCoin.Text;
        //                        innerCount++;
        //                        break;
        //                    case 1://1 = price
        //                        marketPrice = elementCoin.Text;
        //                        innerCount++;
        //                        break;
        //                    case 2://2 = change per
        //                        var range = innerText.Length - (marketPrice.Length + elementCoin.Text.Length);
        //                        var coinName = innerText.Substring(0, range);
        //                        innerCount = 0;
        //                        if (coinName.Equals(string.Empty) == false)
        //                        {
        //                            coins.Add(new KeyValuePair<string, string>(innerText.Substring(0, range), marketPrice));
        //                        }
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        elementExpenedSelector.Click();
        //    }

        //    //var elementCoinPrice = driver.SafeFindElement(By.CssSelector("span.instruments.tickerBarSection"), false);
        //    //if (elementCoinPrice != null)
        //    //{
        //    //    int innerCount = 0;
        //    //    string innerText = string.Empty;
        //    //    string marketPrice = string.Empty;

        //    //    var elementCoinPrices = elementCoinPrice.SafeFindElements(By.TagName("span"));
        //    //    foreach (var elementCoin in elementCoinPrices)
        //    //    {
        //    //        switch (innerCount)
        //    //        {
        //    //            case 0://0 = coinname + price + chageper 
        //    //                innerText = elementCoin.Text;
        //    //                innerCount++;
        //    //                break;
        //    //            case 1://1 = price
        //    //                marketPrice = elementCoin.Text;
        //    //                innerCount++;
        //    //                break;
        //    //            case 2://2 = change per
        //    //                var range = innerText.Length - (marketPrice.Length + elementCoin.Text.Length);
        //    //                var coinName = innerText.Substring(0, range);
        //    //                innerCount = 0;
        //    //                if (coinName.Equals(string.Empty) == false)
        //    //                {
        //    //                    coins.Add(new KeyValuePair<string, string>(innerText.Substring(0, range), marketPrice));
        //    //                }
        //    //                break;
        //    //        }
        //    //    }

        //    //    //var elementCoinPrices = elementCoinPrice.SafeFindElements(By.ClassName("tickerBarItem "));
        //    //    //foreach (var elementCoin in elementCoinPrices)
        //    //    //{
        //    //    //    var elementCoinName = elementCoin.SafeFindElement(By.TagName("a"), false);
        //    //    //    if (elementCoinName == null)
        //    //    //    {
        //    //    //        continue;
        //    //    //    }

        //    //    //    var coinName = elementCoinName.Text;

        //    //    //    if (coinName.Equals(string.Empty) == true)
        //    //    //    {
        //    //    //        continue;
        //    //    //    }

        //    //    //    var elementMarketPrice = elementCoin.SafeFindElement(By.ClassName("price"), false);
        //    //    //    if (elementMarketPrice == null)
        //    //    //    {
        //    //    //        continue;
        //    //    //    }

        //    //    //    var marketPrice = elementMarketPrice.Text;

        //    //    //    coins.Add(new KeyValuePair<string, string>(coinName, marketPrice));
        //    //    //}
        //    //}

        //    return coins;
        //}
    }
}
