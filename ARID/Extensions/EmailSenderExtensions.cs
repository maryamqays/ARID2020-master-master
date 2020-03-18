using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using System.Text;

namespace ARID.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            StringBuilder header = new StringBuilder();

            header.Append("<center>");
            header.Append("<h3>معرف الباحث العربي - أُريد</h3>");            
            header.Append("<p><font face='Tahoma'>مرحباً بك، </font></P>");
            header.Append("<p><font face='Tahoma'>لقد تم تسجيلك في منصة أريد بنجاح. نرجو منك الضغط على الرابط الموجود في الأسفل لتفعيل حسابك.</font></p>");

            header.Append("<br/>");

            header.Append("<h3>Arab Researcher ID - ARID</h3>");
            header.Append("<p><font face='Tahoma'>Greetings, </font></P>");
            header.Append("<p><font face='Tahoma'>You have been registered successfully at ARID platform, please click on the link below to activate your account.</font></p>");

            header.Append("<br/>");
            header.Append("</center>");

            StringBuilder trailer = new StringBuilder();

            trailer.Append("<center>");
            trailer.Append("<h3><font face='Tahoma'>هل أنت بحاجة للمساعدة؟</font></h3>");
            trailer.Append("<p><font face='Tahoma'>إذا كنت بحاجة للمساعدة نرجوا منك أن تتصل بخدمات الدعم الفني. </font></P>");
            trailer.Append("<p><font face='Tahoma'>مع أطيب التحيات</font></P>");
            trailer.Append("<p><font face='Tahoma'>فريق منصة أريد</font></P>");
            trailer.Append("<p><font face='Tahoma'>arid.my | support@arid.my</font></P>");
            trailer.Append("<p><font face='Tahoma'>لقد وصلك هذا الإيميل لأنك قمت بالتسجيل عضواً في منصة أريد</font></P>");

            trailer.Append("<br/>");

            trailer.Append("<h3><font face='Tahoma'>Need any Help ?</font></h3>");
            trailer.Append("<p><font face='Tahoma'>If you need any help please contact our customer support</font></P>");
            trailer.Append("<p><font face='Tahoma'>Regards,</font></P>");
            trailer.Append("<p><font face='Tahoma'>ARID Team </font></P>");
            trailer.Append("<p><font face='Tahoma'>arid.my | support@arid.my</font></P>");
            trailer.Append("<p><font face='Tahoma'>You got this email as you have registered as new member at ARID platform. </font></P>");
            
            return emailSender.SendEmailAsync(email, "Confirm your Email at ARID - تأكيد بريدك الإلكتروني", header +
                   $"<center><b><a href='{HtmlEncoder.Default.Encode(link)}'>Activation Link | رابط التفعيل </a> </p></b></center> <br/>" + trailer
             );
        }
    }
}
