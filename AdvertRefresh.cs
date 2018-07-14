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
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly String AD_TO_REFRESH_TITLE = "Fish for Sale";
        private static readonly String LOGIN = "christian_yeates@yahoo.co.uk";
        private static readonly String PASS = "Petspassword1";

        private static readonly Scheduler scheduler = new Scheduler();

        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));

            //ChromeDriverService serviceCR = ChromeDriverService.CreateDefaultService();
            //serviceCR.Port = 9510;
            //IWebDriver driver = new ChromeDriver(serviceCR);

            int count = 10;
            Stack<Advert> ads = new Stack<Advert>();

            for (int i = 0; i < count;  i++)
            {
                Advert ad = new Advert(LOGIN, PASS, AD_TO_REFRESH_TITLE, TimeSpan.FromSeconds((i + 1) * 30));
                ad.FirstName = "firstName_" + i;
                ad.LastName = "lastName_" + i;
                ad.Enabled = true;
                ads.Push(ad);
                scheduler.AddTask(ad);
                System.Threading.Thread.Sleep(15000);
            }

            for (int i = 0; i < count / 5; i++)
            {
                Console.WriteLine("Press enter to stop the last added task");
                Console.ReadLine();
                scheduler.RemoveTask(ads.Pop());
            }
            
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

            scheduler.RemoveAllTasks();
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error("UNHANDLED EXCEPTION: " + e.ExceptionObject);
        }

    }
}
