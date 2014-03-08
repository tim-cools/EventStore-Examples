using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class ApplicationController : ApiController
    {
        public HttpResponseMessage Get(string folder = null, string fileName = "App.html")
        {
            var contentStream = GetFile(folder, fileName);
            if (contentStream == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var streamContent = new StreamContent(contentStream);
            streamContent.Headers.ContentType = GetContentType(fileName);

            return new HttpResponseMessage {Content = streamContent};
        }

        private static MediaTypeHeaderValue GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return new MediaTypeHeaderValue(GetMediaType(extension));
        }

        private static string GetMediaType(string extension)
        {
            switch (extension)
            {
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                default:
                    return "text/plain";
            }
        }

        private Stream GetFile(string folder, string fileName)
        {
            var steamName = GetFileSteamName(folder, fileName);

            return GetType()
                .Assembly
                .GetManifestResourceStream(steamName);
        }

        private static string GetFileSteamName(string folder, string fileName)
        {
            return !string.IsNullOrWhiteSpace(folder) 
                ? string.Format("Soloco.EventStore.ECommerce.App.{0}.{1}", folder, fileName) 
                : string.Format("Soloco.EventStore.ECommerce.App.{0}", fileName);
        }
    }
}