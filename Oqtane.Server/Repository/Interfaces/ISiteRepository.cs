﻿using System.Collections.Generic;
using Oqtane.Models;

namespace Oqtane.Repository.Interfaces
{
    public interface ISiteRepository
    {
        IEnumerable<Site> GetSites();
        Site AddSite(Site site);
        Site UpdateSite(Site site);
        Site GetSite(int siteId);
        void DeleteSite(int siteId);
    }
}
