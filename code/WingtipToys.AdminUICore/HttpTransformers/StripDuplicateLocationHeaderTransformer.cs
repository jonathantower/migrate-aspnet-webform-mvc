using Yarp.ReverseProxy.Forwarder;

namespace WingtipToys.AdminUICore.HttpTransformers
{
    public class StripDuplicateLocationHeaderTransformer : HttpTransformer
    {
        public override async ValueTask<bool> TransformResponseAsync(
            HttpContext context,
            HttpResponseMessage proxyResponse,
            CancellationToken cancellation)
        {
            // If the backend sent a redirect, capture it
            var backendLocation = proxyResponse.Headers.Location?.ToString();

            // Remove any Location header already on the ASP.NET response
            if (context.Response.Headers.ContainsKey("Location"))
            {
                context.Response.Headers.Remove("Location");
            }

            // If the backend had one, add it exactly once
            if (!string.IsNullOrEmpty(backendLocation))
            {
                context.Response.Headers["Location"] = backendLocation;
            }

            return await base.TransformResponseAsync(context, proxyResponse, cancellation);
        }
    }
}
