using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestExecution
{
    class TestExecutorListener
    {
        public int port { get; private set; }
        //public int id { get; private set; }
        public RichTextBox LogOutPut { get; private set; }
        public Form1 form { get; private set; }
        private Thread listeningThread;

        public TestExecutorListener(int port, RichTextBox LogOutPut, Form1 form)
        {
            this.port = port;
            //this.id = id;
            this.LogOutPut = LogOutPut;
            this.form = form;
        }

        public void InitialiseListener()
        {
            //int port = 26001;
            Console.WriteLine("Starting Test Listener on port: "+port);
            listeningThread = new Thread(() => Listener(port));
            listeningThread.Start();
        }

        public void startAbort()
        {
            try
            {
                listener.Stop();
                listener.Close();
                listeningThread.Abort();
            }
            catch (Exception e)
            {

            }
        }

        public HttpListener listener;

        private void Listener(int port)
        {
            //listen to port
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:" + port + "/");// + id 

            try
            {
                listener.Start();
                while (listener.IsListening)
                {
                    try
                    {
                        HttpListenerContext context = listener.GetContext();

                        HttpListenerRequest request = context.Request;
                        Stream body = request.InputStream;
                        Encoding encoding = request.ContentEncoding;
                        StreamReader reader = new StreamReader(body, encoding);

                        string bodyContent = reader.ReadToEnd();
                        body.Close();
                        var asJson = JsonValue.Parse(bodyContent);
                        //var message = (string)asJson["BodySent"];
                        //message = message + "\n "+request.Url;

                        form.BeginInvoke(new Action(delegate
                        {
                            form.RecieveMessage(asJson);//updateTextBox(LogOutPut, message);

                    }));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Listner crashed");
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            catch (Exception listenerException)
            {
                Console.WriteLine(listenerException.ToString());
            }
        }
    }
}
