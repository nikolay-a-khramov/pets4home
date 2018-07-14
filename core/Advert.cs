using System;
using System.Collections.Generic;
using System.Text;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace pets4home.core
{
    class Advert : ISchedulerTask
    {
        private static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly String URL_BASE = "https://www.pets4homes.co.uk/login/";
        private static readonly String URL_MANAGE_ADS = "https://www.pets4homes.co.uk/account/manage-adverts/";
        private static readonly By LOCATOR_FLD_LOGIN = By.Id("email");
        private static readonly By LOCATOR_FLD_PASS = By.Id("pass");
        private static readonly By LOCATOR_BTN_LOGIN = By.CssSelector(".btn-lg");
        private static readonly By LOCATOR_DIV_AD = By.CssSelector(".manageadvert");
        private static readonly By LOCATOR_DIV_AD_TITLE = By.CssSelector(".title");
        private static readonly By LOCATOR_ICN_REFRESH = By.CssSelector(".fa-refresh");

        public String ContactEmail { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Title { get; set; }

        private String Username { get; set; }
        private String Password { get; set; }

        private IWebDriver Driver { get; set; }

        public string Name
        {
            get
            {
                return String.Format("Advert refresh for '{0}'", this);
            }
            set
            {
                Name = value;
            }
        }
        public TimeSpan Interval { get; set; }
        public bool Enabled { get; set; }

        public Advert(String username, String password, String title, TimeSpan interval)
        {
            Username = username;
            Password = password;
            Title = title;
            Interval = interval;
        }

        public void Refresh()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            Driver = new ChromeDriver(options);
            log.Info(String.Format("{0} refreshing ", this));
            ClickRefreshIcon(Login().FindAdByTitle());
            log.Info(String.Format("{0} refreshed successfully ", this));
            Driver.Quit();
        }


        private Advert Login()
        {
            Driver.Url = URL_BASE;

            log.Debug(String.Format("[{0}] Performing login for: {1}", this, Username));
            IWebElement fieldLogin = Driver.FindElement(LOCATOR_FLD_LOGIN);
            IWebElement fieldPass = Driver.FindElement(LOCATOR_FLD_PASS);
            IWebElement btnLogin = Driver.FindElement(LOCATOR_BTN_LOGIN);

            fieldLogin.SendKeys(Username);
            fieldPass.SendKeys(Password);
            btnLogin.Click();

            return this;
        }

        private IWebElement FindAdByTitle()
        {
            Driver.Url = URL_MANAGE_ADS;

            var allAdDivs = Driver.FindElements(LOCATOR_DIV_AD);
            foreach (IWebElement div in allAdDivs)
            {
                IWebElement divTitle = div.FindElement(LOCATOR_DIV_AD_TITLE);
                if (divTitle.Text.Equals(Title))
                {
                    return div;
                }
            }
            string error = "Couldn't find advert with title: " + Title;
            log.Error(error);
            throw new NoSuchElementException("Couldn't find advert with title: " + Title);
        }

        private Advert ClickRefreshIcon(IWebElement advertDiv)
        {
            IWebElement iconRefresh = advertDiv.FindElement(LOCATOR_ICN_REFRESH);
            (new Actions(Driver)).MoveToElement(iconRefresh).Click().Perform();

            return this;
        }

        public override string ToString()
        {
            return String.Format("Advert: '{0}'|{1} {2}", 
                Title, FirstName, LastName);
        }

        void ISchedulerTask.Execute()
        {
            if (!Enabled)
            {
                log.Debug("Execute called on task that is not enabled: " + Name);
                return;
            }

            log.Info("Executing task: " + Name);
            Refresh();
        }
    }
}
