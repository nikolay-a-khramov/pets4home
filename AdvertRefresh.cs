using System;
using System.Collections.Generic;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
namespace pets4home
{
    class AdvertRefresh
    {
        private static readonly String[] ADS_TO_REFRESH_TITLES = {"Fish for Sale"};
        private static readonly String LOGIN = "christian_yeates@yahoo.co.uk";
        private static readonly String PASS = "Petspassword1";
        private static readonly String URL_BASE = "https://www.pets4homes.co.uk/login/";
        private static readonly String URL_MANAGE_ADS = "https://www.pets4homes.co.uk/account/manage-adverts/";
        private static readonly By LOCATOR_FLD_LOGIN = By.Id("email");
        private static readonly By LOCATOR_FLD_PASS = By.Id("pass");
        private static readonly By LOCATOR_BTN_LOGIN = By.CssSelector(".btn-lg");
        private static readonly By LOCATOR_DIV_AD = By.CssSelector(".manageadvert");
        private static readonly By LOCATOR_DIV_AD_TITLE = By.CssSelector(".title");
        private static readonly By LOCATOR_ICN_REFRESH = By.CssSelector(".fa-refresh");
        static void Main(string[] args)
        {
            ChromeDriverService serviceCR = ChromeDriverService.CreateDefaultService();
            serviceCR.Port = 9510;
            IWebDriver driver = new ChromeDriver(serviceCR);
            driver.Url = URL_BASE;

            IWebElement fieldLogin = driver.FindElement(LOCATOR_FLD_LOGIN);
            IWebElement fieldPass = driver.FindElement(LOCATOR_FLD_PASS);
            IWebElement btnLogin = driver.FindElement(LOCATOR_BTN_LOGIN);

            fieldLogin.SendKeys(LOGIN);
            fieldPass.SendKeys(PASS);
            btnLogin.Click();

            driver.Url = URL_MANAGE_ADS;
            //TODO: Revert change!
            List<IWebElement> divAds = findAdWithTitle(driver, new List<String>(ADS_TO_REFRESH_TITLES));

            foreach(IWebElement ad in divAds) {
                IWebElement iconRefresh = ad.FindElement(LOCATOR_ICN_REFRESH);
                (new Actions(driver)).MoveToElement(iconRefresh).Click().Perform();
            }
            
            driver.Quit();
        }

        static List<IWebElement> findAdWithTitle(IWebDriver driver, List<String> titlesToFind) {
            List<IWebElement> foundDivs = new List<IWebElement>();
            var allAdDivs = driver.FindElements(LOCATOR_DIV_AD);
            foreach (IWebElement div in allAdDivs) {
                IWebElement divTitle = div.FindElement(LOCATOR_DIV_AD_TITLE);
                Console.WriteLine(divTitle.Text);
                if (titlesToFind.Contains(divTitle.Text)) {
                    foundDivs.Add(div);
                }
            }
            return foundDivs;
        }
    }
}
