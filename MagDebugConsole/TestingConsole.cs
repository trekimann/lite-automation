using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Json;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using HttpWebServices;
using IslServices;
using Magrathea;
using Magrathea.Executors;
using Newtonsoft.Json;
using Selenium.htmlElementManager;
using Selenium.OCR;
using Selenium.testLogger;

namespace MagDebugConsole
{
    class TestingConsole
    {
        static void Main(string[] args)
        {

            //List<TestInformation> testlist = new List<TestInformation>();

            //int port = 26001;

            //var listening = new Thread(() => Listener(port));
            //listening.Start();

            //Console.ReadLine();
            //listener.Stop();
            //listening.Abort();


            PdfOcr reader = new PdfOcr();
            
            string directory = @"C:\Users\MANNN\Desktop\Building using Magrathea.pdf";
            
            Console.ReadLine();
        }

        public static HttpListener listener;

        private static void Listener(int port)
        {
            //listen to port
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:"+port+"/Test/");
            listener.Start();
            
            while (listener.IsListening)
            {

                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new StreamReader(body, encoding);

                Console.WriteLine(request.Url);
                string bodyContent = reader.ReadToEnd();
                //JsonValue bodyAsJson = JsonObject.Parse(bodyContent);
                //JsonObject jsonOb = new JsonObject();
                //var keys = jsonOb.Keys;
                //var type = bodyAsJson.GetType().ToString();
                //if (type.Equals("System.Json.JsonObject"))
                //{
                //    var keys = bodyAsJson
                //}

                ////pull apart json just because
                //foreach (JsonValue json in bodyAsJson)
                //{
                //    Console.WriteLine(json.ToString());
                //}
                Console.WriteLine(bodyContent);
                body.Close();
            }
        }
    }
}
