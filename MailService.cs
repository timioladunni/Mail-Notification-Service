using MailNotificationService.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Timers;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MailNotificationService
{
    class MailService
    {
        
        private readonly Timer timer;
        IConfiguration _configuration;
        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public MailService()
        {
            var key = ConfigurationManager.AppSettings["timer"];
            var timerKey = Convert.ToInt64(key);
            timer = new System.Timers.Timer(timerKey) { AutoReset = true };
            timer.Elapsed += ExecuteEvent;
            
        }
        public void ExecuteEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                
                timer.Stop();
                var logPath3 = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer3 = new StreamWriter(logPath3, true);
                writer3.WriteLine(string.Concat($"{DateTime.Now} : Service is Excecuting "));
                writer3.Close();
                MailNotificationServiceDBContext context = new MailNotificationServiceDBContext();
                var welcomeMail = MessageType.WelcomeMail.ToString();
                var thresholdNotificationMail = MessageType.BudgetNotificationMail.ToString();
                var sendingStatus = context.Emails.Where(t => t.Status == Status.Pending.ToString());
                var numberOfCount = sendingStatus.ToList().Count;
                foreach (var item in sendingStatus)
                {
                    if (item == null)
                    {
                        timer.Stop();
                        timer.Start();
                    }
                    if (item.MessageType == welcomeMail)
                    {
                        try
                        {
                            UserInfoModel myDeserializedClass = JsonConvert.DeserializeObject<UserInfoModel>(item.Body);
                            var path = ConfigurationManager.AppSettings["PathToWelcomeEmailTemplate"];
                            var result = SendEmailMessageAsync(SubjectType.Welcome.ToString(), myDeserializedClass.Name, myDeserializedClass.Email, myDeserializedClass.Password, path);
                            if (result == true)
                            {
                                item.Status = Status.Sent.ToString();
                                context.Entry(item).State = EntityState.Modified;
                            }
                            else
                            {
                                var logPath2 = ConfigurationManager.AppSettings["logpath"];
                                StreamWriter writer2 = new StreamWriter(logPath2, true);
                                writer2.WriteLine(string.Concat($"{DateTime.Now} : Could not send welcome mail"));
                                writer2.Close();
                            }
                           
                        }
                        catch (Exception ex)
                        {
                            var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                            var logPath2 = ConfigurationManager.AppSettings["logpath"];
                            StreamWriter writer2 = new StreamWriter(logPath2, true);
                            writer2.WriteLine(string.Concat("Error occured in the welcomeMaill ", errMessage));
                            writer2.Close();
                        }

                    }
                    else if (item.MessageType == thresholdNotificationMail)
                    {

                        try
                        {
                            InstitutionInfoModel institutionInfo = JsonConvert.DeserializeObject<InstitutionInfoModel>(item.Body);
                            var path = ConfigurationManager.AppSettings["PathToBudgetNotificationTemplate"];
                            var repaymentAccount = String.Format("{0:n}", institutionInfo.RepaymentAccount);
                            var threshold = String.Format("{0:n}", institutionInfo.Threshold);
                            var currentAccount = String.Format("{0:n}", institutionInfo.CurrentAccount);
                            var result = SendThresholdNotificationMail(SubjectType.ThresholdNotification.ToString(), institutionInfo.InstitutionName, institutionInfo.Email, repaymentAccount, threshold, currentAccount,  path);
                            if (result == true)
                            {
                                item.Status = Status.Sent.ToString();
                                context.Entry(item).State = EntityState.Modified;
                            }
                            else
                            {
                                var logPath1 = ConfigurationManager.AppSettings["logpath"];
                                StreamWriter writer1 = new StreamWriter(logPath1, true);
                                writer1.WriteLine(string.Concat($"{DateTime.Now} : Could not send budget notification mail "));
                                writer1.Close();
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                            var logPath1 = ConfigurationManager.AppSettings["logpath"];
                            StreamWriter writer1 = new StreamWriter(logPath1, true);
                            writer1.WriteLine(string.Concat("Error occured in the ExecuteEvent ", errMessage));
                            writer1.Close();

                        }


                    }


                }
                context.SaveChanges();
                var logPath = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer = new StreamWriter(logPath, true);
                writer.WriteLine(string.Concat($"{DateTime.Now} : {numberOfCount} file[s] execution completed sucessfully "));
                writer.Close();
                timer.Start();
            }
            catch (Exception ex)
            {

                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                var logPath = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer = new StreamWriter(logPath, true);
                writer.WriteLine(string.Concat($"{DateTime.Now}: Error occured in the CreateWelcomeBody ", errMessage));
                writer.Close();
            }
           
        }


        private static string CreateWelcomeBody(string name, string pwd, string email, string pathToMail)
        {
            try
            {
                string body = string.Empty;
                string fileName = pathToMail;
                using (StreamReader reader = new StreamReader(fileName))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{Name}", name);
                body = body.Replace("{Email}", email);
                body = body.Replace("{PWD}", pwd);

                return body;
            }
            catch (Exception ex)
            {

                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                var logPath = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer = new StreamWriter(logPath, true);
                writer.WriteLine(string.Concat($"{DateTime.Now} : Error occured in the CreateWelcomeBody ", errMessage));
                writer.Close();
                return errMessage;
            }
            
        }

        public static bool SendEmailMessageAsync(string subject, string name, string recipientEmail, string pwd, string pathToMail)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    string email = ConfigurationManager.AppSettings["AppEmail"];
                    string password =ConfigurationManager.AppSettings["AppPassword"];
                    var loginInfo = new NetworkCredential(email, password);
                    mail.From = new MailAddress(email);
                    mail.To.Add(new MailAddress(recipientEmail));
                    mail.Subject = subject;
                    mail.Body =  CreateWelcomeBody(name, pwd, recipientEmail, pathToMail);
                    mail.IsBodyHtml = true;
                    try
                    {
                        var smtpClient = new SmtpClient("smtpout.secureserver.net", 587);
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = loginInfo;
                        smtpClient.EnableSsl = true;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.Send(mail);
                        return true;
                    }
                    finally
                    {
                        //dispose the client
                        mail.Dispose();
                    }
                }                
            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                var logPath = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer = new StreamWriter(logPath, true);
                writer.WriteLine(string.Concat($"{DateTime.Now} : Error occured in the SendEmailMessageAsync ", errMessage));
                writer.Close();
                return false;
                
            }
        }

        private static string CreateThresholdNotificationBody(string institutionName, string repaymentAccount, string threshold, string currentBalance, string pathToMail)
        {
            try
            {
                string body = string.Empty;
                string fileName = pathToMail;
                using (StreamReader reader = new StreamReader(fileName))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{{institution_name}}", institutionName);
                body = body.Replace("{{repayment_account}}", repaymentAccount);
                body = body.Replace("{Threshold}", threshold);
                body = body.Replace("{CurrentBalance}", currentBalance);
                return body;
            }
            catch (Exception ex)
            {

                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                var logPath = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer = new StreamWriter(logPath, true);
                writer.WriteLine(string.Concat($"{DateTime.Now} : Error occured in the CreateThresholdNotificationBody ", errMessage));
                writer.Close();
                return errMessage;
            }
            
        }
        public static bool SendThresholdNotificationMail(string subject, string institutionName, string recipientEmail, string repaymentAccount, string threshold, string currentBalance, string pathToMail)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    string email = ConfigurationManager.AppSettings["AppEmail"];
                    string password = ConfigurationManager.AppSettings["AppPassword"];
                    var loginInfo = new NetworkCredential(email, password);
                    mail.From = new MailAddress(email);
                    mail.To.Add(new MailAddress(recipientEmail));
                    mail.Subject = subject;
                    mail.Body = CreateThresholdNotificationBody(institutionName, repaymentAccount, threshold, currentBalance, pathToMail);
                    mail.IsBodyHtml = true;
                    try
                    {
                        var smtpClient = new SmtpClient("smtpout.secureserver.net", 587);
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = loginInfo;
                        smtpClient.EnableSsl = true;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.Send(mail);
                        return true;
                    }
                    finally
                    {
                        //dispose the client
                        mail.Dispose();
                    }
                }

                
            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                var logPath = ConfigurationManager.AppSettings["logpath"];
                StreamWriter writer = new StreamWriter(logPath, true);
                writer.WriteLine(string.Concat($"{DateTime.Now} : Error occured in the SendThresholdNotificationMaily ", errMessage));
                writer.Close();
                return false;

            }
        }
        public void Stop()
        {
            var logPath = ConfigurationManager.AppSettings["logpath"];
            StreamWriter writer = new StreamWriter(logPath, true);
            writer.WriteLine(string.Concat($"{DateTime.Now} : Service has been stopped "));
            writer.Close();
            timer.Stop();
           
        }
        public void Start()
        {
            var logPath = ConfigurationManager.AppSettings["logpath"];
            StreamWriter writer = new StreamWriter(logPath, true);
            writer.WriteLine(string.Concat($"{DateTime.Now} : Service has been Started "));
            writer.Close();
            timer.Start();
            
        }
      
    }
        
}
