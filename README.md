# Oqtane Framework
Oqtane is a Modular Application Framework for Blazor

![Oqtane](https://github.com/oqtane/framework/blob/master/oqtane.png?raw=true "Oqtane")

Oqtane uses Blazor, a new web framework for .NET Core that lets you build interactive web UIs using C# instead of JavaScript. Blazor apps are composed of reusable web UI components implemented using C#, HTML, and CSS. Both client and server code is written in C#, allowing you to share code and libraries.

**To get started with Oqtane:**

   1.&nbsp;Oqtane is currently compatible with **[.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)**.
   
   2.&nbsp;Install the latest edition of [Visual Studio 2019](https://visualstudio.com/vs/) with the **ASP.NET and web development** workload. Installing the latest edition will also install the latest version of .NET Core 3.0.

   3.&nbsp;Download or Clone the Oqtane source code to your local system. Open the **Oqtane.sln** solution file. If you want to develop using **server-side** Blazor ( which includes a full debugging experience in Visual Studio ) you should choose to Build the solution using the default Debug configuration. If you want to develop using **client-side** Blazor ( WebAssembly ) you should first choose the "Wasm" configuration option in the Visual Studio toolbar and then Build.
   
   NOTE: If you have already installed a previous version of Oqtane and you wish to install a newer version, there is currently no upgrade path from one version to the next. The recommended upgrade approach is to get the latest code and build it, and then reset the DefaultConnection value to "" in the appsettings.json file in the Oqtane.server project. This will trigger a re-install when you run the application which will execute the latest database scripts.

# Roadmap
This project is a work in progress and the schedule for implementing the items identified below is dependent upon the availability of community members who are able to assist.

Security
- Logging
- Need support for JwT tokens for external client access to API

Design
- Need modern Admin UI theme
- Need to cleanly separate site.css

Admin
- Need ability to soft delete core entities
- Drag and Drop modules

Upgrade
- Need ability to upgrade application seamlessly
- integrated store/catalog of extensions
- auto update - provide url to check for updates, perhaps even download in background - core and extensions

Database
- Need ability to run on SQLite

# Background
Oqtane was created by [Shaun Walker](https://www.linkedin.com/in/shaunbrucewalker/) and is inspired by the DotNetNuke web application framework. Initially created as a proof of concept, Oqtane is a native Blazor application written from the ground up using modern .NET Core technology. It is a modular framework offering a fully dynamic page compositing model, multi-site support, designer friendly templates ( skins ), and extensibility via third party modules.

At this point Oqtane offers a minimum of desired functionality and is not recommended for production usage. The expectation is that Oqtane will rapidly evolve as a community driven open source project. At this point in time we do not promise any upgrade path from one version to the next, and developers should expect breaking changes as the framework stabilizes.

# Release Announcement

[Announcing Oqtane... a Modular Application Framework for Blazor!](https://www.oqtane.org/Resources/Blog/PostId/520/announcing-oqtane-a-modular-application-framework-for-blazor)

# Example Screenshots

A seamless login flow utilizing .NET Core Identity services:

![Login](https://github.com/oqtane/framework/blob/master/screenshot1.png?raw=true "Login")

Main view for authorized users, allowing full management of modules and content:

![Admin View](https://github.com/oqtane/framework/blob/master/screenshot2.png?raw=true "Admin View")

Content editing user experience using modal dialog:

![Edit Content](https://github.com/oqtane/framework/blob/master/screenshot3.png?raw=true "Edit Content")

Context menu for managing specific module on page:

![Manage Module](https://github.com/oqtane/framework/blob/master/screenshot4.png?raw=true "Manage Module")

Control panel for adding, editing, and deleting pages as well as adding new modules to a page:

![Manage Page](https://github.com/oqtane/framework/blob/master/screenshot5.png?raw=true "Manage Page")

