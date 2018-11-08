using System;
using System.Configuration;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace Isn.Common.LogHelper
{
    public static class Log
    {
        static Log()
        {            
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget
            {
                Name = "FileTarget",
                FileName = "${basedir}/Logs/${shortdate}.log",
                Layout =
                    @"${longdate} ::: Exception Type: ${exception:format=Type} ::: Message: ${message} ::: ${exception:format=tostring,Data:maxInnerExceptionLevel=10}${newline}"
            };

            config.AddTarget("File", fileTarget);

            DatabaseTarget dataBaseTarget = null;

            if (ConfigurationManager.ConnectionStrings["Updater"] != null)
            {
                dataBaseTarget = new DatabaseTarget
                {
                    Name = "DatabaseTarget",
                    ConnectionString = ConfigurationManager.ConnectionStrings["Updater"].ConnectionString,
                    CommandText = @"INSERT INTO [UnhandledException] 
                                (Id, Date, Thread, Level, Logger, Message, Exception, Stack, ExceptionType) 
                                VALUES
                                ((SELECT NEWID()), (SELECT GETDATE()), @thread, @level, @logger, @message, @exception, @stack, @exceptionType)"
                };

                dataBaseTarget.Parameters.Add(new DatabaseParameterInfo("@thread", new SimpleLayout("${threadid}")));
                dataBaseTarget.Parameters.Add(new DatabaseParameterInfo("@level", new SimpleLayout("${level}")));
                dataBaseTarget.Parameters.Add(new DatabaseParameterInfo("@logger", new SimpleLayout("${logger}")));
                dataBaseTarget.Parameters.Add(new DatabaseParameterInfo("@message", new SimpleLayout("${message}")));
                dataBaseTarget.Parameters.Add(new DatabaseParameterInfo("@exception", new SimpleLayout("${exception}")));
                dataBaseTarget.Parameters.Add(new DatabaseParameterInfo("@stack",
                    new SimpleLayout("${exception:format=tostring,Data:maxInnerExceptionLevel=10}${newline}")));
                dataBaseTarget.Parameters.Add(new DatabaseParameterInfo("@exceptionType",
                    new SimpleLayout("${exception:format=Type}")));

                config.AddTarget("DataBase", dataBaseTarget);
            }            

            var mailTarget = new MailTarget
            {
                Name = "MailTarget",
                Html = true,
                Body =
                    "${longdate} ::: Exception Type: ${exception:format=Type} ::: Message: ${message} ::: ${exception:format=tostring,Data:maxInnerExceptionLevel=10}${newline}",
                SmtpServer = ConfigurationManager.AppSettings["SmtpServer"],
                From = ConfigurationManager.AppSettings["EmailSender"],
                Encoding = Encoding.UTF8,
                To = ConfigurationManager.AppSettings["EmailReceiver"],
                EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]),
                SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SendingPort"]),
                SmtpUserName = ConfigurationManager.AppSettings["EmailSender"],
                SmtpPassword = ConfigurationManager.AppSettings["PasswordSender"],
                SmtpAuthentication = SmtpAuthenticationMode.Basic,
                Subject = "Updater ${level}"
            };

            config.AddTarget("Mailing", mailTarget);

            var eventLogTarget = new EventLogTarget
            {
                Name = "EventLogTarget",
                Layout =
                    @"${longdate} ::: Exception Type: ${exception:format=Type} ::: Message: ${message} ::: ${exception:format=tostring,Data:maxInnerExceptionLevel=10}${newline}",
                Log = "Application",
                Source = "Updater"
            };

            config.AddTarget("EventLog", eventLogTarget);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));

            if (dataBaseTarget != null)
            {
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, dataBaseTarget));
            }
            
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, mailTarget));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, eventLogTarget));

            LogManager.Configuration = config;
            Instance = LogManager.GetCurrentClassLogger();
        }
        
        public static Logger Instance { get; set; }
    }
}