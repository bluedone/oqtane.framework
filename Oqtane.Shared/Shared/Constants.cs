﻿namespace Oqtane.Shared
{
    public class Constants
    {
        public const string PackageId = "Oqtane.Framework";
        public const string Version = "0.0.1";

        public const string PageComponent = "Oqtane.Shared.ThemeBuilder, Oqtane.Client";
        public const string ContainerComponent = "Oqtane.Shared.ContainerBuilder, Oqtane.Client";

        public const string DefaultTheme = "Oqtane.Themes.BlazorTheme.Default, Oqtane.Client";
        public const string DefaultLayout = "";
        public const string DefaultContainer = "Oqtane.Themes.BlazorTheme.Container, Oqtane.Client";
        public const string DefaultAdminContainer = "Oqtane.Themes.AdminContainer, Oqtane.Client";

        public static readonly string[] DefaultModuleActions = new[] { "Settings", "Import", "Export" }; 
        public const string DefaultModuleActionsTemplate = "Oqtane.Modules.Admin.Modules.{Control}, Oqtane.Client";
        public const string PageManagementModule = "Oqtane.Modules.Admin.Pages, Oqtane.Client";
        public const string ModuleMessageControl = "Oqtane.Modules.Controls.ModuleMessage, Oqtane.Client";

        public const string DefaultControl = "Index";
        public const string AdminPane = "Admin";

        public const string AllUsersRole = "All Users";
        public const string HostRole = "Host Users";
        public const string AdminRole = "Administrators";
        public const string RegisteredRole = "Registered Users";
    }
}
