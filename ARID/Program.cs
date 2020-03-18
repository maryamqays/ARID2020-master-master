using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace ARID
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args)
            //Workaround:
            //After configuring with Application Insights, a JavaScript snippet is added to the file Views/Shared/_Layout.cshtml.There are two workarounds,
            //depending on whether you want Application Insights to collect page views from your app:
            //Collect page views - Add ".UseApplicationInsights()" to the WebHostBuilder in Program.cs.
            //Don't collect page views - Delete the following lines from Views/Shared/_Layout.cshtml:
            //@inject Microsoft.ApplicationInsights.AspNetCore.JavaScriptSnippet JavaScriptSnippet.
            //@Html.Raw(JavaScriptSnippet.FullScript).
            .UseApplicationInsights()

            .UseSetting("detailedErrors", "true")
            .CaptureStartupErrors(true)
            .UseStartup<Startup>()
            .Build();
    }
}
