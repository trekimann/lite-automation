using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Json;
using System.Net;
using System.Text;



namespace restClient_0
{

    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    class RESTClient
    {
        public httpVerb httpMethod { get; set; }

        public RESTClient()
        {
           
            httpMethod = httpVerb.GET;
        }

        public static JsonObject makeRequest(string endPoint)
        {
            
            string strResponseValue = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);

            request.Method = httpVerb.GET.ToString();

            HttpWebResponse response = null;
            JsonObject result = null;
            //DataTable dataTable = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();


                //Proecess the resppnse stream... (could be JSON, XML or HTML etc..._

                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            strResponseValue = reader.ReadToEnd();

                            JsonValue responseJson = JsonValue.Parse(strResponseValue);
                            result = responseJson as JsonObject;
                            
                            return result;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                strResponseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            return result;
        }
    }
}
