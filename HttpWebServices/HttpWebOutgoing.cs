using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpWebServices
{
    public class HttpWebOutgoing
    {
        public Object SendStringInBody(int port, string uri, string content, string body, String identifier = null)
        {
                dynamic message = new ExpandoObject();
                message.Content = content;
                if (identifier != null)
                {
                    message.Identifier = identifier;
                }
                message.BodySent = body;
                var json = JsonConvert.SerializeObject(message);

                byte[] byteArray = Encoding.UTF8.GetBytes(json);
                String fullUrl = "http://localhost:" + port + @"/" + uri;
            try
            {
                WebRequest request = WebRequest.Create(fullUrl);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "POST";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }catch(Exception e)
            { message.Error = e.ToString(); }
                return message;
        }

        public Object SendStringInBodyReturnResponse(int port, string uri, string content, string body, String identifier = null)
        {
            dynamic message = new ExpandoObject();
            message.Content = content;
            if (identifier != null)
            {
                message.Identifier = identifier;
            }
            message.BodySent = body;
            var json = JsonConvert.SerializeObject(message);

            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            WebRequest request = WebRequest.Create("http://localhost:" + port + @"/" + uri);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            var reader = new StreamReader(response.GetResponseStream());
            message.WebResponse = reader.ReadToEnd();

            return message;
        }
    }
}
