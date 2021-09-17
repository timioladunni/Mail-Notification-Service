using System;
using System.Configuration;
using System.IO;
using Topshelf;

namespace MailNotificationService
{
    class Program
    {
        
        static void Main(string[] args)
        {

            try
            {
                HostFactory.Run(hostConfig =>
                {
                    hostConfig.Service<MailService>(serviceConfig =>
                    {
                        serviceConfig.ConstructUsing(() => new MailService());

                        serviceConfig.WhenStarted(s => s.Start());
                        serviceConfig.WhenStopped(s => s.Stop());

                    });
                    hostConfig.RunAsLocalSystem();
                    hostConfig.SetServiceName("CENotificationService");
                    hostConfig.SetDisplayName("CElNotificationService");
                    hostConfig.SetDescription("This service automatically sends pending messages to the receipent");
                });
            }
            catch (Exception ex)
            {

                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                var logPath = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer = new StreamWriter(logPath, true);
                writer.WriteLine(string.Concat("Error occured in the Main ", errMessage));
                writer.Close();
            }
        }
    }
}
