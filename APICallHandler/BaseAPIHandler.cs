using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Authentication;

namespace APICallHandler
{
    public static class BaseAPIHandler
    {
        public static object ProcessGeneric(APIAction action, string path, string body = "", string itemId = "", AuthenticationToken userToken = null)
        {
            return new KeyValuePair<string, string>("result", "Hello, world!"); // "{'result': 'Hello, world!'}";
        }

        public static string ProcessGenericToXML(APIAction action, string path, string body = "", string itemId = "", AuthenticationToken userToken = null)
        {
            return "<note><to>Self</to><from>Justin</from><heading>Reminder</heading><body> Don't forget return an actual XML serialization of useful data!</body></note>";
        }        
    }
}
