using System;
using pets4home.core;
using log4net.Config;
using log4net;
using System.IO;
using System.Text;

namespace pets4home
{
    class AdvertRefresh
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static readonly Scheduler scheduler = new Scheduler();

        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            CsvReader reader = new CsvReader("adverts.csv");
            while (reader.hasMoreRows())
            {
                var row = reader.getNextRow();
                scheduler.AddTask(Advert.fromProperties(row));
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
