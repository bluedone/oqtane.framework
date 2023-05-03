# Latest Release

[3.4.2](https://github.com/oqtane/oqtane.framework/releases/tag/v3.4.2) was released on Mar 29, 2023 and is primarily focused on performance, as the permissions system has been overhauled to avoid unnecessary encoding and parsing of custom access control strings. This release also includes enhancements to connection string management, numerous stabilization and user experience improvements, and the ability to dynamically generate an XML sitemap for seach engine indexing. This release includes 62 pull requests by 6 different contributors, pushing the total number of project commits all-time over 3300. The Oqtane framework continues to evolve at a rapid pace to meet the needs of .NET developers.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Foqtane%2Foqtane.framework%2Fmaster%2Fazuredeploy.json)

# Oqtane Framework

![Oqtane](https://github.com/oqtane/framework/blob/master/oqtane.png?raw=true "Oqtane")

Oqtane is a Modular Application Framework. It leverages Blazor, an open source and cross-platform web UI framework for building single-page apps using .NET and C# instead of JavaScript. Blazor apps are composed of reusable web UI components implemented using C#, HTML, and CSS. Both client and server code is written in C#, allowing you to share code and libraries.

Oqtane is being developed based on some fundamental principles which are outlined in the [Oqtane Philosophy](https://www.oqtane.org/blog/!/20/oqtane-philosophy).

Please note that this project is owned by the .NET Foundation and is governed by the **[.NET Foundation Contributor Covenant Code of Conduct](https://dotnetfoundation.org/code-of-conduct)**

# Getting Started

**Using Version 3:**

- Install **[.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)**.
   
- Install the latest edition (v17.0 or higher) of [Visual Studio 2022](https://visualstudio.microsoft.com/vs/preview/#download-preview) with the **ASP.NET and web development** workload enabled. Oqtane works with ALL editions of Visual Studio from Community to Enterprise. If you wish to use LocalDB for development ( not a requirement as Oqtane supports SQLite, mySQL, and PostgreSQL ) you must also install the **Data storage and processing**.  

- clone the Oqtane dev branch source code to your local system. Open the **Oqtane.sln** solution file and Build the solution. Make sure you specify Oqtane.Server as the Startup Project and then Run the application.

**Installing an official release:**

- A detailed set of instructions for installing Oqtane on IIS is located here: [Installing Oqtane on IIS](https://www.oqtane.org/Resources/Blog/PostId/542/installing-oqtane-on-iis)
- Instructions for upgrading Oqtane are located here: [Upgrading Oqtane](https://www.oqtane.org/Resources/Blog/PostId/543/upgrading-oqtane)

**Additional Instructions**

- If you have already installed a previous version of Oqtane and you wish to do a clean database install, simply reset the DefaultConnection value in the Oqtane.Server\appsettings.json file to "". This will trigger a re-install when you run the application which will execute the database installation.
   
- If you want to submit pull requests make sure you install the [Github Extension For Visual Studio](https://visualstudio.github.com/). It is recommended you ignore any local changes you have made to the appsettings.json file before you submit a pull request. To automate this activity, open a command prompt and navigate to the /Oqtane.Server/ folder and enter the command "git update-index --skip-worktree appsettings.json" 

**Video Series**

- If you are getting started with Oqtane, a [series of videos](https://www.youtube.com/watch?v=JPfUZPlRRCE&list=PLYhXmd7yV0elLNLfQwZBUlM7ZSMYPTZ_f) are available which explain how to install the product, interact with the user interface, and develop custom modules.

# Documentation
There is a separate [Documentation repository](https://github.com/oqtane/oqtane.docs) which contains a variety of types of documentation for Oqtane, including API documentation that is auto generated using Docfx. The contents of the repository is published to Githib Pages and is available at [https://docs.oqtane.org](https://docs.oqtane.org/)

# Roadmap
This project is open source, and therefore is a work in progress...

4.0.0 ( Q2 2023 )
- [ ] Migration to .NET 7
- [ ] File / New Project experience
- [ ] Folder Providers

[3.4.2](https://github.com/oqtane/oqtane.framework/releases/tag/v3.4.2) ( Mar 29, 2023 )
- [x] Stabilization improvements 

[3.4.1](https://github.com/oqtane/oqtane.framework/releases/tag/v3.4.1) ( Mar 13, 2023 )
- [x] Stabilization improvements 

[3.4.0](https://github.com/oqtane/oqtane.framework/releases/tag/v3.4.0) ( Mar 12, 2023 )
- [x] Permissions performance optimization
- [x] Connection string management improvements
- [x] XML site map generator
- [x] OIDC integration with User Profiles

[3.3.1](https://github.com/oqtane/oqtane.framework/releases/tag/v3.3.1) ( Jan 14, 2023 )
- [x] Stabilization improvements 

[3.3.0](https://github.com/oqtane/oqtane.framework/releases/tag/v3.3.0) ( Jan 12, 2023 )
- [x] Dynamic Authorization Policies
- [x] Entity-Level Permissions
- [x] Extended Module Permissions

[3.2.1](https://github.com/oqtane/oqtane.framework/releases/tag/v3.2.1) ( Oct 17, 2022 )
- [x] Stabilization improvements  
- [x] Server Event System

[3.2.0](https://github.com/oqtane/oqtane.framework/releases/tag/v3.2.0) ( Sep 13, 2022 )
- [x] .NET MAUI / Blazor Hybrid support 
- [x] Upgrade to Bootstrap 5.2

[3.1.3](https://github.com/oqtane/oqtane.framework/releases/tag/v3.1.3) ( Jun 27, 2022 )
- [x] Stabilization improvements 

[3.1.2](https://github.com/oqtane/oqtane.framework/releases/tag/v3.1.2) ( May 14, 2022 )
- [x] Stabilization improvements 

[3.1.1](https://github.com/oqtane/oqtane.framework/releases/tag/v3.1.1) ( May 3, 2022 )
- [x] Stabilization improvements 

[3.1.0](https://github.com/oqtane/oqtane.framework/releases/tag/v3.1.0) ( Apr 5, 2022 )
- [x] User account lockout support
- [x] Two factor authentication support
- [x] Per-site configuration of password complexity, lockout criteria
- [x] External login support via OAuth2 / OpenID Connect
- [x] Support for Single Sign On (SSO) via OpenID Connect
- [x] External client support via Jwt tokens
- [x] Downstream API support via Jwt tokens
- [x] CSS resource hierarchy support
- [x] Site structure/content migration
- [x] Event log notifications  
- [x] 404 page handling
- [x] Property change component notifications
- [x] Support for ES6 JavaScript modules

[3.0.3](https://github.com/oqtane/oqtane.framework/releases/tag/v3.0.3) ( Feb 15, 2022 )
- [x] Url fragment and anchor navigation support
- [x] Meta tag support in page head
- [x] Html/Text content versioning support

[3.0.2](https://github.com/oqtane/oqtane.framework/releases/tag/v3.0.2) ( Jan 16, 2022 )
- [x] Default alias specification, auto alias registration, redirect logic
- [x] Improvements to visitor tracking and url mapping
- [x] Scheduler enhancements for stop/start, weekly and one-time jobs
- [x] Purge job for daily housekeeping of event log and visitors 
- [x] Granular security filtering for Settings 

[3.0.1](https://github.com/oqtane/oqtane.framework/releases/tag/v3.0.1) ( Dec 12, 2021 )
- [x] Url mapping for broken links, content migration
- [x] Visitor tracking for usage insights, personalization
- [x] User experience improvements in Page and Module management

[3.0.0](https://github.com/oqtane/oqtane.framework/releases/tag/v3.0.0) ( Nov 11, 2021 )
- [x] Migration to .NET 6
- [x] Blazor hosting model flexibility per site
- [x] Blazor WebAssembly prerendering support

[2.3.1](https://github.com/oqtane/oqtane.framework/releases/tag/v2.3.1) ( Sep 27, 2021 )
- [x] Complete UI migration to Bootstrap 5 and HTML5 form validation
- [x] Improve module/theme installation and add support for commercial extensions
- [x] Replace System.Drawing with ImageSharp
- [x] Image resizing service

[2.2.0](https://github.com/oqtane/oqtane.framework/releases/tag/v2.2.0) ( Jul 6, 2021 )
- [x] Bootstrap 5 Upgrade
- [x] Package Service integration
- [x] Default and Shared Resource File inclusion
- [x] Startup Error logging
- [x] API Controller Validation and Logging

[2.1.0](https://github.com/oqtane/oqtane.framework/releases/tag/v2.1.0) ( Jun 4, 2021 )
- [x] Cross Platform Database Support ( ie. LocalDB, SQL Server, SQLite, MySQL, PostgreSQL ) - see [#964](https://github.com/oqtane/oqtane.framework/discussions/964)
- [x] Utilize EF Core Migrations - see [#964](https://github.com/oqtane/oqtane.framework/discussions/964)
- [x] Public Content Folder support
- [x] Multi-tenant Infrastructure improvements
- [x] User Authorization optimization
- [x] Consolidation of Package Management
- [x] Blazor Server Pre-rendering
- [x] Translation Package installation support

[2.0.2](https://github.com/oqtane/oqtane.framework/releases/tag/v2.0.2) ( Apr 19, 2021 )
- [x] Assorted fixes and user experience improvements

[2.0.1](https://github.com/oqtane/oqtane.framework/releases/tag/v2.0.1) ( Feb 27, 2021 )
- [x] Complete Static Localization of Admin UI

[2.0.0](https://github.com/oqtane/oqtane.framework/releases/tag/v2.0.0) ( Nov 11, 2020 - released in conjunction with .NET 5 )
- [x] Migration to .NET 5
- [x] Static Localization ( ie. labels, help text, etc.. )
- [x] Improved JavaScript Reference Support
- [x] Performance Optimizations
- [x] Developer Productivity Enhancements

[1.0.0](https://github.com/oqtane/oqtane.framework/releases/tag/v1.0.0) ( May 19, 2020 - released in conjunction with .NET Core 3.2 )
- [x] Multi-Tenant ( Shared Database & Isolated Database ) 
- [x] Modular Architecture
- [x] Headless API with Swagger Support
- [x] Dynamic Page Compositing Model / Site & Page Management
- [x] Authentication / User Management / Profile Management
- [x] Authorization / Roles Management / Granular Permissions
- [x] Dynamic Routing
- [x] Extensibility via Custom Modules
- [x] Extensibility via Custom Themes
- [x] Event Logging / Audit Trail
- [x] Folder / File Management
- [x] Recycle Bin
- [x] Scheduled Jobs ( Background Processing )
- [x] Notifications / Email Delivery
- [x] Seamless Upgrade Experience
- [x] Progressive Web Application Support
- [x] JavaScript Lazy Loading
- [x] Dynamic CSS/Lazy Loading

# Background
Oqtane was created by [Shaun Walker](https://www.linkedin.com/in/shaunbrucewalker/) and is inspired by the DotNetNuke web application framework. Initially created as a proof of concept, Oqtane is a native Blazor application written from the ground up using modern .NET Core technology and a Single Page Application (SPA) architecture. It is a modular application framework offering a fully dynamic page compositing model, multi-site support, designer friendly themes, and extensibility via third party modules.

# Release Announcements

[Oqtane 3.3](https://www.oqtane.org/blog/!/54/oqtane-3-3-0-released)

[Oqtane 3.2](https://www.oqtane.org/blog/!/50/oqtane-3-2-for-net-maui-blazor-hybrid)

[Oqtane 3.1](https://www.oqtane.org/blog/!/41/oqtane-3-1-released)

[Oqtane 3.0](https://www.oqtane.org/Resources/Blog/PostId/551/announcing-oqtane-30-for-net-6)

[Oqtane 2.2](https://www.oqtane.org/Resources/Blog/PostId/549/oqtane-22-upgrades-to-bootstrap-5)

[Oqtane 2.1](https://www.oqtane.org/Resources/Blog/PostId/548/oqtane-21-now-supports-multiple-databases)

[Oqtane 2.0](https://www.oqtane.org/Resources/Blog/PostId/544/announcing-oqtane-20-for-net-5)

[Oqtane 1.0](https://www.oqtane.org/Resources/Blog/PostId/540/announcing-oqtane-10-a-modular-application-framework-for-blazor)

[Oqtane POC](https://www.oqtane.org/Resources/Blog/PostId/520/announcing-oqtane-a-modular-application-framework-for-blazor)

# Reference Implementations

[Built On Blazor!](https://builtonblazor.net) - a showcase of sites built on Blazor

[.NET Foundation Project Trends](https://www.dnfprojects.com) - tracks the most active .NET Foundation open source projects based on GitHub activity

# Architecture

The following diagram visualizes the client and server components in the Oqtane architecture.

![Architecture](https://github.com/oqtane/framework/blob/master/screenshots/Architecture.png?raw=true "Oqtane Architecture")

# Databases

As of version 2.1, Oqtane supports multiple relational database providers.

![Databases](https://github.com/oqtane/framework/blob/dev/screenshots/databases.png?raw=true "Oqtane Databases")

# Example Screenshots

Install Wizard:

![Installer](https://github.com/oqtane/framework/blob/master/screenshots/Installer.png?raw=true "Installer")

Default view after installation:

![Home](https://github.com/oqtane/framework/blob/master/screenshots/screenshot0.png?raw=true "Home")

A seamless login flow utilizing .NET Core Identity services:

![Login](https://github.com/oqtane/framework/blob/master/screenshots/screenshot1.png?raw=true "Login")

Main view for authorized users, allowing full management of modules and content:

![Admin View](https://github.com/oqtane/framework/blob/master/screenshots/screenshot2.png?raw=true "Admin View")

Content editing user experience using modal dialog:

![Edit Content](https://github.com/oqtane/framework/blob/master/screenshots/screenshot3.png?raw=true "Edit Content")

Context menu for managing specific module on page:

![Manage Module](https://github.com/oqtane/framework/blob/master/screenshots/screenshot4.png?raw=true "Manage Module")

Control panel for adding, editing, and deleting pages as well as adding new modules to a page:

![Manage Page](https://github.com/oqtane/framework/blob/master/screenshots/screenshot5.png?raw=true "Manage Page")

Admin dashboard for accessing the various administrative features of the framework:

![Admin Dashboard](https://github.com/oqtane/framework/blob/master/screenshots/screenshot6.png?raw=true "Admin Dashboard")

Responsive design mobile view:

![Mobile View](https://github.com/oqtane/framework/blob/master/screenshots/screenshot7.png?raw=true "Mobile View")
