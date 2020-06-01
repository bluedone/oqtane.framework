﻿using Microsoft.AspNetCore.Components;
using Oqtane.Shared;
using Oqtane.UI;

namespace Oqtane.Themes
{
    public abstract class LayoutBase : ComponentBase, ILayoutControl
    {
        [CascadingParameter]
        protected PageState PageState { get; set; }
        public virtual string Name { get; set; }
        public virtual string Thumbnail { get; set; }
        public virtual string Panes { get; set; }

        public string LayoutPath()
        {
            return "Themes/" + GetType().Namespace + "/";
        }

    }
}
