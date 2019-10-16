using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace SelfHostApi
{
    public class TextMediaTypeFormatter : MediaTypeFormatter
    {
        public TextMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            try
            {
                var memoryStream = new MemoryStream();
                readStream.CopyTo(memoryStream);
                var s = Encoding.UTF8.GetString(memoryStream.ToArray());
                taskCompletionSource.SetResult(s);
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
            return taskCompletionSource.Task;
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }
    }
    
    public class ApiHost
    {
        static void Main(string[] args)
        {
            string portNumber = "26000";
            string baseAddress = "http://localhost:"+portNumber;
            var config = new HttpSelfHostConfiguration(baseAddress);
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.Add(new TextMediaTypeFormatter());

            try
            {
                using (HttpSelfHostServer server = new HttpSelfHostServer(config))
                {
                    Console.WriteLine("Starting server");
                    server.OpenAsync().Wait();
                    Console.WriteLine("Server open on " + baseAddress);
                    Console.WriteLine("Listen for test output messages on :26001");
                    Console.WriteLine("Press enter to close");
                    Console.ReadLine();
                    server.CloseAsync();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Somethings gone wrong...");
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine(e.ToString());
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Press Enter to close");
            }
        }
    }
}
