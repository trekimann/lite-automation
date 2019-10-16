using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Json;
using System.Net;
using System.Text;


namespace restClient_0
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    class RestClientPost
    {
        public httpVerb httpMethod { get; set; }

        public RestClientPost()
        {

            httpMethod = httpVerb.POST;
        }

        public static JsonValue makeRequest(string endPoint, Object dataObject)
        {
            var stream = JsonConvert.SerializeObject(dataObject);
            File.WriteAllText(@"C:\Users\MANNN\Desktop\MagTests\json.txt", stream);
            string strResponseValue = string.Empty;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endPoint);
            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Method = httpVerb.POST.ToString();


            
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(dataObject));
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JsonValue responseJson = JsonValue.Parse(result);
                    return responseJson;
                }
            }
           
        }
    }
}
