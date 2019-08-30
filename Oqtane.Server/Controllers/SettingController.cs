﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Oqtane.Repository;
using Oqtane.Models;
using Oqtane.Shared;
using Oqtane.Security;

namespace Oqtane.Controllers
{
    [Route("{site}/api/[controller]")]
    public class SettingController : Controller
    {
        private readonly ISettingRepository Settings;
        private readonly IUserPermissions UserPermissions;

        public SettingController(ISettingRepository Settings, IUserPermissions UserPermissions)
        {
            this.Settings = Settings;
            this.UserPermissions = UserPermissions;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Setting> Get(string entityname, int entityid)
        {
            return Settings.GetSettings(entityname, entityid);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Setting Get(int id)
        {
            return Settings.GetSetting(id);
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize]
        public Setting Post([FromBody] Setting Setting)
        {
            if (ModelState.IsValid && IsAuthorized(Setting.EntityName, Setting.EntityId))
            {
                Setting = Settings.AddSetting(Setting);
            }
            return Setting;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize]
        public Setting Put(int id, [FromBody] Setting Setting)
        {
            if (ModelState.IsValid && IsAuthorized(Setting.EntityName, Setting.EntityId))
            {
                Setting = Settings.UpdateSetting(Setting);
            }
            return Setting;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.AdminRole)]
        public void Delete(int id)
        {
            Settings.DeleteSetting(id);
        }

        private bool IsAuthorized(string EntityName, int EntityId)
        {
            bool authorized = false;
            switch (EntityName)
            {
                case "Module":
                    authorized = UserPermissions.IsAuthorized(User, EntityName, EntityId, "Edit");
                    break;
                default:
                    authorized = User.IsInRole(Constants.AdminRole);
                    break;
            }
            return authorized;
        }
    }
}
