﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Shared;
using Oqtane.Models;
using System.Threading.Tasks;

namespace Oqtane.Modules
{
    public class ModuleBase : ComponentBase, IModuleControl
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        protected PageState PageState { get; set; }

        [CascadingParameter]
        protected Module ModuleState { get; set; }

        [CascadingParameter] 
        protected ModuleInstance ModuleInstance { get; set; }

        public virtual SecurityAccessLevel SecurityAccessLevel { get { return SecurityAccessLevel.View; } set { } } // default security

        public virtual string Title { get { return ""; } }

        public virtual string Actions { get { return ""; } }

        public virtual bool UseAdminContainer { get { return true; } }

        public string ModulePath()
        {
            return "Modules/" + this.GetType().Namespace + "/";
        }

        public async Task AddCSS(string Url)
        {
            if (!Url.StartsWith("http"))
            {
                Url = ModulePath() + Url;
            }
            var interop = new Interop(JSRuntime);
            await interop.AddCSS("Module:" + Utilities.CreateIdFromUrl(Url), Url);
        }

        public string NavigateUrl()
        {
            return NavigateUrl(PageState.Page.Path);
        }

        public string NavigateUrl(Reload reload)
        {
            return NavigateUrl(PageState.Page.Path, reload);
        }

        public string NavigateUrl(string path)
        {
            return NavigateUrl(path, "", Reload.None);
        }

        public string NavigateUrl(string path, Reload reload)
        {
            return NavigateUrl(path, "", reload);
        }

        public string NavigateUrl(string path, string parameters)
        {
            return Utilities.NavigateUrl(PageState.Alias.Path, path, parameters, Reload.None);
        }

        public string NavigateUrl(string path, string parameters, Reload reload)
        {
            return Utilities.NavigateUrl(PageState.Alias.Path, path, parameters, reload);
        }

        public string EditUrl(string action)
        {
            return EditUrl(ModuleState.ModuleId, action);
        }

        public string EditUrl(string action, string parameters)
        {
            return EditUrl(ModuleState.ModuleId, action, parameters);
        }

        public string EditUrl(int moduleid, string action)
        {
            return EditUrl(moduleid, action, "");
        }

        public string EditUrl(int moduleid, string action, string parameters)
        {
            return EditUrl(PageState.Page.Path, moduleid, action, parameters);
        }

        public string EditUrl(string path, int moduleid, string action, string parameters)
        {
            return Utilities.EditUrl(PageState.Alias.Path, path, moduleid, action, parameters);
        }
    }
}
