﻿using System;
using System.Collections.Generic;
using Oqtane.Models;

namespace Oqtane.Shared
{
    public class PageState
    {
        public string Alias { get; set; }
        public Site Site { get; set; }
        public List<Page> Pages { get; set; }
        public Page Page { get; set; }
        public User User { get; set; }
        public List<Module> Modules { get; set; }
        public Uri Uri { get; set; }
        public Dictionary<string, string> QueryString { get; set; }
        public int ModuleId { get; set; }
        public string Control { get; set; }
        public string Mode { get; set; }
    }
}
