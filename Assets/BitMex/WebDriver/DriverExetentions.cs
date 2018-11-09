using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace Assets.BitMex.WebDriver
{
    public static class DriverExetentions
    {
        public static IWebElement SafeFindElement(this IWebDriver driver, By by, bool isThrow = true)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
                return null;
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
                return null;
            }
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
                return null;
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
                return null;
            }
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
                return null;
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
                return null;
            }
        }

        public static ReadOnlyCollection<IWebElement> SafeFindElements(this IWebElement driver, By by, bool isThrow = true)
        {
            try
            {
                return driver.FindElements(by);
            }
            catch (NoSuchElementException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
                return null;
            }
            catch (StaleElementReferenceException)
            {
                if (isThrow == true) throw new BitMexDriverServiceException();
                return null;
            }
        }
    }
}
