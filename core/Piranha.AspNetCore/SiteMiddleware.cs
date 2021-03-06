/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Piranha.Web;
using System;
using System.Threading.Tasks;

namespace Piranha.AspNetCore
{
    public class SiteMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        public SiteMiddleware(RequestDelegate next, ILoggerFactory factory = null) : base(next, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="api">The current api</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context, IApi api) {
            if (!context.Request.Path.Value.StartsWith("/manager/")) {
                // Try to get the requested site by hostname
                var site = api.Sites.GetByHostname(context.Request.Host.Host);

                // If we didn't find the site, get the default site
                if (site == null)
                    site = api.Sites.GetDefault();

                // Store the current site id for the current request
                if (site != null)
                    context.Items[SiteId] = site.Id;
            }
            // Nothing to see here, move along
            await next.Invoke(context);
        }
    }
}
