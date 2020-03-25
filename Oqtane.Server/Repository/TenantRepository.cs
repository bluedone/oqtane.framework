﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Oqtane.Models;

namespace Oqtane.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private MasterDBContext _db;
        private readonly IMemoryCache _cache;

        public TenantRepository(MasterDBContext context, IMemoryCache cache)
        {
            _db = context;
            _cache = cache;
        }

        public IEnumerable<Tenant> GetTenants()
        {
            return _cache.GetOrCreate("tenants", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return _db.Tenant.ToList();
            });
        }

        public Tenant AddTenant(Tenant tenant)
        {
            _db.Tenant.Add(tenant);
            _db.SaveChanges();
            _cache.Remove("tenants");
            return tenant;
        }

        public Tenant UpdateTenant(Tenant tenant)
        {
            _db.Entry(tenant).State = EntityState.Modified;
            _db.SaveChanges();
            _cache.Remove("tenants");
            return tenant;
        }

        public Tenant GetTenant(int tenantId)
        {
            return _db.Tenant.Find(tenantId);
        }

        public void DeleteTenant(int tenantId)
        {
            Tenant tenant = _db.Tenant.Find(tenantId);
            if (tenant != null)
            {
                _db.Tenant.Remove(tenant);
                _db.SaveChanges();
            }

            _cache.Remove("tenants");
        }
    }
}
