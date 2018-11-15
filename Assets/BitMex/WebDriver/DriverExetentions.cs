using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace Assets.BitMex.WebDriver
{
    public static class DriverExetentions
    {
        public static string SafeGetAttribute(this IWebElement element, string attributeName, bool isThrow = true)
        {
            try
            {
                return element.GetAttribute(attributeName);
            }
            catch (NoSuchElementException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }

            return string.Empty;
        }

        public static IWebElement SafeFindElement(this IWebDriver driver, By by, bool isThrow = true)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            return null;
        }

        public static ReadOnlyCollection<IWebElement> SafeFindElements(this IWebDriver driver, By by, bool isThrow = true)
        {
            try
            {
                return driver.FindElements(by);
            }
            catch (NoSuchElementException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            return null;
        }

        public static IWebElement SafeFindElement(this IWebElement element, By by, bool isThrow = true)
        {
            try
            {
                return element.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            return null;
        }

        public static ReadOnlyCollection<IWebElement> SafeFindElements(this IWebElement element, By by, bool isThrow = true)
        {
            try
            {
                return element.FindElements(by);
            }
            catch (NoSuchElementException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
            }
            return null;
        }

        public static bool SafeChainingClick(this IWebElement element, Func<bool> complete, bool isThrow = true)
        {
            element.Click();
            for (int i = 0; i < 3; i++)
            {
                if (complete() == true)
                {
                    return true;
                }
            }

            if (isThrow == true)
            {
                throw new BitMexDriverServiceException();
            }

            return false;
        }
    }
}
