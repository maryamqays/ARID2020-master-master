using ARID.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARID.Services
{
    public class EmailLogger : IEmailLogger
    {
        //private readonly IOptions<SmtpDetails> _smtpDetails;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailLogger(
            //IOptions<SmtpDetails> smtpDetails,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            //_smtpDetails = smtpDetails;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            if (exception != null)
            {
                // Add the details of the Http Request if present
                var stringBuilder = new StringBuilder();
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var request = httpContext.Request;
                    stringBuilder.Append("User: ").Append(_userManager.GetUserName(httpContext.User)).Append("<br/>")
                        .Append("Address: ").Append($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}").Append("<br/>")
                        .Append("Local IP address: ").Append(httpContext.Connection.LocalIpAddress).Append("<br/>")
                        .Append("Remote IP address: ").Append(httpContext.Connection.RemoteIpAddress).Append("<br/>");
                }
                EmailException(exception, stringBuilder);
            }
        }

        private void EmailException(Exception exception, StringBuilder stringBuilder)
        {
            BuildExceptionText(stringBuilder, "<h1>Error </h1>", exception);

            //_emailSender.SendEmailAsync(
            //   info@filspay.com, new List<string> { _configuration["ErrorDeliveryEmailAddress"] },
            //    "System Error", stringBuilder.ToString()).ConfigureAwait(false);

            _emailSender.SendEmailAsync("info@filspay.com", "Error", stringBuilder.ToString());
        }

        private StringBuilder BuildExceptionText(StringBuilder stringBuilder, string title, Exception exception)
        {
            stringBuilder.Append(title).Append("<h2>").Append(exception.Message).Append("</h2><br/>")
               .Append(exception.Source ?? "").Append("<hr/>");
            if (exception.StackTrace != null)
            {
                stringBuilder.Append("<h3>Stack trace: </h3><br/>").Append(exception.StackTrace.Replace(Environment.NewLine, "<br/>"));
            }

            if (exception.InnerException != null)
            {
                BuildExceptionText(stringBuilder, "<h2>Inner exception </h2>", exception.InnerException);
            }

            return stringBuilder;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoDispose();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
    }

    [Serializable]
    internal class NoDispose : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
