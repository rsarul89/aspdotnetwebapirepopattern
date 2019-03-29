using log4net;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace ApiEFAutofac
{
    /// <summary>
    /// Represents implementation of <see cref="ExceptionLogger"/>.
    /// </summary>
    public class ApiExceptionLogger : ExceptionLogger
    {
        private const string CorrelationIdHeaderName = "CorrelationId";
        private static ILog _logger = null;
        public ApiExceptionLogger()
        {
            // Gets directory path of the calling application  
            // RelativeSearchPath is null if the executing assembly i.e. calling assembly is a  
            // stand alone exe file (Console, WinForm, etc).   
            // RelativeSearchPath is not null if the calling assembly is a web hosted application i.e. a web site  
            //var log4NetConfigDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var log4NetConfigDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var log4NetConfigFilePath = Path.Combine(log4NetConfigDirectory, "App_Start", "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4NetConfigFilePath));
        }
        //public override void Log(ExceptionLoggerContext context)
        //{
        //    _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //    //_logger.Error(context.Exception.ToString() + Environment.NewLine);
        //    _logger.Error(Environment.NewLine + " Excetion Time: " + System.DateTime.Now + Environment.NewLine
        //        + " Exception Message: " + context.Exception.Message.ToString() + Environment.NewLine
        //        + " Exception File Path: " + context.ExceptionContext.ControllerContext.Controller.ToString() + "/" + context.ExceptionContext.ControllerContext.RouteData.Values["action"] + Environment.NewLine);
        //}
        //public void Log(string ex)
        //{
        //    _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //    _logger.Error(ex);
        //    //_logger.Error(Environment.NewLine +" Excetion Time: " + System.DateTime.Now + Environment.NewLine  
        //    //    + " Exception Message: " + context.Exception.Message.ToString() + Environment.NewLine  
        //    //    + " Exception File Path: " + context.ExceptionContext.ControllerContext.Controller.ToString() + "/" + context.ExceptionContext.ControllerContext.RouteData.Values["action"] + Environment.NewLine);   
        //}
        /// <summary>
        /// Overrides <see cref="ExceptionLogger.LogAsync"/> method with custom logger implementations.
        /// </summary>
        /// <param name="context">Instance of <see cref="ExceptionLoggerContext"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public override async Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            GetOrSetCorrelationId(context.Request);
            var request = await CreateRequest(context.Request);
            // Use a logger of your choice to log a request.
            _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Error(Environment.NewLine + "=============================================================================================================================" + Environment.NewLine
                + " Excetion Time: " + System.DateTime.Now + Environment.NewLine
                + " CorrelationId: " + request.CorrelationId + Environment.NewLine
                + " Exception Message: " + context.Exception.StackTrace + Environment.NewLine
                + " Exception File Path: " + request.Path + Environment.NewLine
                + " Exception File Path Base: " + request.PathBase + Environment.NewLine
                + "=============================================================================================================================" + Environment.NewLine);

            void GetOrSetCorrelationId(HttpRequestMessage message)
            {
                var correlationId = Guid.NewGuid().ToString();

                if (!message.Headers.TryGetValues(CorrelationIdHeaderName, out var correlations))
                {
                    message.Headers.Add(CorrelationIdHeaderName, correlationId);
                }
                else if (Guid.TryParse(correlations.First(), out var id))
                {
                    message.Headers.Add(CorrelationIdHeaderName, id.ToString());
                }
                else
                {
                    message.Headers.Add(CorrelationIdHeaderName, correlationId);
                }
            }
        }

        private static async Task<dynamic> CreateRequest(HttpRequestMessage message)
        {
            var request = new
            {
                CorrelationId = message.Headers.GetValues(CorrelationIdHeaderName).First(),
                Body = await ReadContent(message.Content).ConfigureAwait(false),
                Method = message.Method.Method,
                Scheme = message.RequestUri.Scheme,
                Host = message.RequestUri.Host,
                Protocol = string.Empty,
                PathBase = message.RequestUri.PathAndQuery,
                Path = message.RequestUri.AbsoluteUri,
                QueryString = message.RequestUri.Query
            };

            return request;

            async Task<string> ReadContent(HttpContent content)
            {
                using (content)
                {
                    string body;
                    try
                    {
                        body = await content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        body = $"Failed to read body. Error: {e}";
                    }

                    return body;
                }
            }
        }
    }
}