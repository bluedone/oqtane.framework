﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Oqtane.Repository;
using Oqtane.Models;

namespace Oqtane.Controllers
{
    [Route("{site}/api/[controller]")]
    public class AliasController : Controller
    {
        private readonly IAliasRepository Aliases;

        public AliasController(IAliasRepository Aliases)
        {
            this.Aliases = Aliases;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Alias> Get()
        {
            return Aliases.GetAliases();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Alias Get(int id)
        {
            return Aliases.GetAlias(id);
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize]
        public Alias Post([FromBody] Alias Alias)
        {
            if (ModelState.IsValid)
            {
                Alias = Aliases.AddAlias(Alias);
            }
            return Alias;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize]
        public Alias Put(int id, [FromBody] Alias Alias)
        {
            if (ModelState.IsValid)
            {
                Alias = Aliases.UpdateAlias(Alias);
            }
            return Alias;
        }

        // DELETE api/<controller>/5
        [Authorize]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Aliases.DeleteAlias(id);
        }
    }
}
