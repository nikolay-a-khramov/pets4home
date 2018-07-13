using System;
using System.Collections.Generic;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using pets4home.core;
using log4net.Config;
using log4net;

namespace pets4home
{
    class AdvertRefresh
    {
        private static readonly String AD_TO_REFRESH_TITLE = "Fish for Sale";
        private static readonly String LOGIN = "christian_yeates@yahoo.co.uk";
        private static readonly String PASS = "Petspassword1";

        private static readonly Scheduler scheduler = new Scheduler();

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));

            //ChromeDriverService serviceCR = ChromeDriverService.CreateDefaultService();
            //serviceCR.Port = 9510;
            //IWebDriver driver = new ChromeDriver(serviceCR);

            Advert fishAdCarl = new Advert(LOGIN, PASS, AD_TO_REFRESH_TITLE, TimeSpan.FromSeconds(20));
            fishAdCarl.FirstName = "Carl";
            fishAdCarl.LastName = "Smith";
            fishAdCarl.Enabled = true;
            scheduler.AddTask(fishAdCarl);

            Advert fishAdMike = new Advert(LOGIN, PASS, AD_TO_REFRESH_TITLE, TimeSpan.FromSeconds(21));
            fishAdMike.FirstName = "Mike";
            fishAdMike.LastName = "Johnson";
            fishAdMike.Enabled = true;
            scheduler.AddTask(fishAdMike);

            Console.WriteLine("Press enter to stop Mike's task");
            Console.ReadLine();

            scheduler.RemoveTask(fishAdMike);

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

            scheduler.RemoveAllTasks();
        }

    }
}
