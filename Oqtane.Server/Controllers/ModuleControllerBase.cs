using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Oqtane.Infrastructure;
using System.Collections.Generic;
using System;

namespace Oqtane.Controllers
{
    public class ModuleControllerBase : Controller
    {
        protected readonly ILogManager _logger;
        // querystring parameters for policy authorization and validation
        protected Dictionary<string, int> _authEntityId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        protected int _entityId = -1; // deprecated

        public ModuleControllerBase(ILogManager logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            int value;
            foreach (var param in accessor.HttpContext.Request.Query)
            {
                if (param.Key.StartsWith("auth") && param.Key.EndsWith("id") && int.TryParse(param.Value, out value))
                {
                    _authEntityId.Add(param.Key.Substring(4, param.Key.Length - 6), int.Parse(param.Value));
                }
            }
            // entityid is deprecated
            if (accessor.HttpContext.Request.Query.ContainsKey("entityid"))
            {
                _entityId = int.Parse(accessor.HttpContext.Request.Query["entityid"]);
            }
        }

    }
}
