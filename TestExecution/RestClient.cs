using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Json;
using System.Net;
using System.Text;


namespace TestExecution
{
    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    class RestClient
    {
        public static JsonValue makeRequest(TestExecution.httpVerb verb, string endPoint, Object dataObject)
        {
            var stream = JsonConvert.SerializeObject(dataObject);
            HttpWebRequest httpWebRequest = setUpHttpRequest(verb, endPoint);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(dataObject));
                streamWriter.Flush();
                streamWriter.Close();
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    JsonValue responseJson = parseJsonResponse(streamReader);
                    return responseJson;
                }
            }

        }

        public static JsonValue makeRequest(TestExecution.httpVerb verb, string endPoint)
        {
            HttpWebRequest httpWebRequest = setUpHttpRequest(verb, endPoint);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
          using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                JsonValue responseJson = parseJsonResponse(streamReader);
                return responseJson;
            }
        }

        private static HttpWebRequest setUpHttpRequest(httpVerb verb, string endPoint)
        {
            string strResponseValue = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endPoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = verb.ToString();
            return httpWebRequest;
        }

        private static JsonValue parseJsonResponse(StreamReader streamReader)
        {
            var result = streamReader.ReadToEnd();
            JsonValue responseJson = JsonValue.Parse(result);
            return responseJson;
        }
    }
}
