using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebApi.Models;

namespace WebApi.Handlers
{
    public class ResultWrapperHandler : DelegatingHandler
    {
        private readonly MaskSettings maskSettings;

        public ResultWrapperHandler(MaskSettings maskSettings)
        {
            this.maskSettings = maskSettings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage result = await base.SendAsync(request, cancellationToken);
            this.WrapResultIfNeeded(request, result);
            return result;
        }

        protected virtual void WrapResultIfNeeded(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            if (this.IsIgnoredUrl(request.RequestUri))
            {
                return;
            }

            if (this.IsMaskUrl(request.RequestUri))
            {
                if (this.MaskFor.TryGetValue(request.RequestUri.AbsolutePath, out MaskPath maskPath))
                {
                    this.Wrap(response, maskPath);
                    return;
                }
            }
        }

        private void Wrap(HttpResponseMessage response, MaskPath maskPath)
        {
            if (!response.TryGetContentValue(out object resultObject) || resultObject == null)
            {
                response.StatusCode = HttpStatusCode.OK;
                return;
            }

            JToken jtokens = JToken.FromObject(resultObject);
            if (jtokens is JValue)
            {
                return;
            }

            Mask(jtokens, maskPath);

            response.Content = new StringContent(jtokens.ToString(), Encoding.UTF8, "application/json");
        }

        private static void Mask(JToken jtokens, MaskPath maskPath)
        {
            foreach (JToken token in jtokens)
            {
                if (token is JProperty property)
                {
                    Mask(property, maskPath);
                }
                else if (token is JObject)
                {
                    Mask(token, maskPath);
                }
            }
        }

        private static void Mask(JProperty property, MaskPath maskPath)
        {
            if (maskPath.Properties.Contains(property.Name))
            {
                property.Value = maskPath.MaskString ?? "xxxxxxxx";
            }
        }

        private bool IsIgnoredUrl(Uri uri)
        {
            if (uri == null || string.IsNullOrEmpty(uri.AbsolutePath))
            {
                return false;
            }

            return this.maskSettings.IgnoredPaths.Contains(uri.AbsolutePath);
        }

        private bool IsMaskUrl(Uri uri)
        {
            if (uri == null || string.IsNullOrEmpty(uri.AbsolutePath))
            {
                return false;
            }

            return this.maskSettings.Masks.SelectMany(x => x.Paths).Contains(uri.AbsolutePath);
        }

        private IDictionary<string, MaskPath> MaskFor =>
            this.maskSettings.Masks.SelectMany(x => x.Paths, (p, c) => new MaskPath(p.Properties, c, p.MaskString))
                                    .ToDictionary(x => x.Path, x => new MaskPath(x.Properties, x.Path, x.MaskString));
    }
}
