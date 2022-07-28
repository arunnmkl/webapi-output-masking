using System.IO;
using System.Web.Http;
using Newtonsoft.Json;
using WebApi.Handlers;
using WebApi.Models;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            MaskSettings maskSettings = LoadJson();
            config.MessageHandlers.Add(new ResultWrapperHandler(maskSettings));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }

        public static MaskSettings LoadJson()
        {
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "/App_Data/appSettings.json";
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string json = streamReader.ReadToEnd();
                Root jobj = JsonConvert.DeserializeObject<Root>(json);
                return jobj.MaskSettings;
            }
        }
    }
}
