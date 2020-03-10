﻿using Microsoft.AspNetCore.Components;
using Oqtane.Shared;
using Oqtane.UI;

namespace Oqtane.Themes
{
    public class LayoutBase : ComponentBase, ILayoutControl
    {
        [CascadingParameter]
        protected PageState PageState { get; set; }
        public virtual string Panes { get; set; }

        public string LayoutPath()
        {
            return "Themes/" + GetType().Namespace + "/";
        }

    }
}
