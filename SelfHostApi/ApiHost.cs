﻿using System;
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
            // FindOpenPort();
            bool startServer = false;
            Console.WriteLine("Enter Port Number. Defaults to 26000. Put ? to find an open port");
            string portNumber = Console.ReadLine();
            if(portNumber != "")
            {
                int portNum = -1;
                while (portNum == -1 && portNumber != "")
                {
                    try
                    {
                        portNum = Int32.Parse(portNumber);
                        if(portNum<0 || portNum>65535)
                        {
                            Console.WriteLine("Ports should be between 1 and 65535. Please try again");
                            portNum = -1;
                            portNumber = Console.ReadLine();                            
                        }
                    }
                    catch
                    {
                        Console.WriteLine("input not recognised. Please try again");
                        portNumber = Console.ReadLine();
                    }

                }
                startServer = true;
            }
            if (portNumber == "")
            {
                portNumber = "26000";
                startServer = true;
            }
            else if (portNumber == "?")
            {
                portNumber = FindOpenPort();
                if (portNumber == null)
                {
                    Console.WriteLine("No open port was found. Contact your system admin to open a port for use");
                }
                else
                {
                    Console.WriteLine("The open port found was: " + portNumber + ". Use this in the test executor");
                    startServer = true;
                }
            }
            if (startServer)
            {
                string baseAddress = "http://localhost:" + portNumber;
                var config = new HttpSelfHostConfiguration(baseAddress);
                config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "{controller}",
                    defaults: new { id = RouteParameter.Optional }
                );

                config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                config.Formatters.Add(new TextMediaTypeFormatter());

                int pn = Int32.Parse(portNumber);
                try
                {
                    using (HttpSelfHostServer server = new HttpSelfHostServer(config))
                    {
                        Console.WriteLine("Starting server");
                        server.OpenAsync().Wait();
                        Console.WriteLine("Server open on " + baseAddress);
                        Console.WriteLine("Listen for test output messages on :" + (pn));
                        Console.WriteLine("Press enter to close");
                        Console.ReadLine();
                        server.CloseAsync();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Somethings gone wrong...");
                    Console.WriteLine("--------------------------------------------------------");
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("--------------------------------------------------------");
                    Console.WriteLine("Press Enter to close");
                    Console.ReadLine();
                }
            }
            Console.ReadLine();
        }

        static string FindOpenPort()
        {
            List<Int32> portsToSkip = new List<Int32>() { 8180, 8580, 8080, 8280 };
            // loop though ports with try catch to open a port

            for (int i = 1; i < 65535; i++)
            {
                if (!portsToSkip.Contains(i))
                {
                    string portNumber = i.ToString();
                    string baseAddress = "http://localhost:" + portNumber;
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
                            Console.WriteLine("Starting server on " + baseAddress);
                            server.OpenAsync().Wait();
                            Console.WriteLine("Server open on " + baseAddress);
                            Console.WriteLine("Port " + i + " is open");
                            server.CloseAsync().Wait();                            
                            return i.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(i + " Is not availible, trying next port");
                    }
                }
            }
            return null;
        }
    }
}
