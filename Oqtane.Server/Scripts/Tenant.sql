/*  

Create tables

*/

CREATE TABLE [dbo].[Site](
	[SiteId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Logo] [nvarchar](50) NOT NULL,
  CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED 
  (
	[SiteId] ASC
  )
)
GO

CREATE TABLE [dbo].[Page](
	[PageId] [int] IDENTITY(1,1) NOT NULL,
	[SiteId] [int] NOT NULL,
	[Path] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ThemeType] [nvarchar](200) NULL,
	[Icon] [nvarchar](50) NOT NULL,
	[Panes] [nvarchar](50) NOT NULL,
	[ViewPermissions] [nvarchar](500) NOT NULL,
	[EditPermissions] [nvarchar](500) NOT NULL,
	[ParentId] [int] NULL,
	[Order] [int] NOT NULL,
	[IsNavigation] [bit] NOT NULL,
	[LayoutType] [nvarchar](200) NOT NULL,
  CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED 
  (
	[PageId] ASC
  )
)
GO

CREATE TABLE [dbo].[Module](
	[ModuleId] [int] IDENTITY(1,1) NOT NULL,
	[SiteId] [int] NOT NULL,
	[ModuleDefinitionName] [nvarchar](200) NOT NULL,
	[ViewPermissions] [nvarchar](500) NOT NULL,
	[EditPermissions] [nvarchar](500) NOT NULL,
  CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
  (
	[ModuleId] ASC
  )
)
GO

CREATE TABLE [dbo].[PageModule](
	[PageModuleId] [int] IDENTITY(1,1) NOT NULL,
	[PageId] [int] NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Pane] [nvarchar](50) NOT NULL,
	[Order] [int] NOT NULL,
	[ContainerType] [nvarchar](200) NOT NULL,
  CONSTRAINT [PK_PageModule] PRIMARY KEY CLUSTERED 
  (
	[PageModuleId] ASC
  )
)
GO

CREATE TABLE [dbo].[HtmlText](
	[HtmlTextId] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
  CONSTRAINT [PK_HtmlText] PRIMARY KEY CLUSTERED 
  (
	[HtmlTextId] ASC
  )
)
GO

CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Roles] [nvarchar](50) NOT NULL,
	[IsSuperUser] [bit] NOT NULL,
  CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
  (
	[UserId] ASC
  )
)
GO

/*  

Create foreign key relationships

*/
ALTER TABLE [dbo].[HtmlText]  WITH CHECK ADD  CONSTRAINT [FK_HtmlText_Module] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Module] ([ModuleId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Module]  WITH CHECK ADD  CONSTRAINT [FK_Module_Site] FOREIGN KEY([SiteId])
REFERENCES [dbo].[Site] ([SiteId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Page]  WITH CHECK ADD  CONSTRAINT [FK_Page_Site] FOREIGN KEY([SiteId])
REFERENCES [dbo].[Site] ([SiteId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PageModule]  WITH CHECK ADD  CONSTRAINT [FK_PageModule_Module] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Module] ([ModuleId])
GO

ALTER TABLE [dbo].[PageModule]  WITH CHECK ADD  CONSTRAINT [FK_PageModule_Page] FOREIGN KEY([PageId])
REFERENCES [dbo].[Page] ([PageId])
ON DELETE CASCADE
GO

/*  

Create seed data

*/

SET IDENTITY_INSERT [dbo].[Site] ON 
GO
INSERT [dbo].[Site] ([SiteId], [Name], [Logo]) 
VALUES (1, N'Site1', N'oqtane.png')
GO
INSERT [dbo].[Site] ([SiteId], [Name], [Logo]) 
VALUES (2, N'Site2', N'oqtane.png')
GO
SET IDENTITY_INSERT [dbo].[Site] OFF
GO

SET IDENTITY_INSERT [dbo].[Page] ON 
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (1, 1, N'Page1', N'', N'Oqtane.Client.Themes.Theme1.Theme1, Oqtane.Client', N'oi-home', N'Left;Right', N'All Users', N'Administrators', NULL, 1, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (2, 1, N'Page2', N'page2', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'oi-plus', N'Top;Bottom', N'Administrators;Editors;', N'Administrators', NULL, 3, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (3, 1, N'Page3', N'page3', N'Oqtane.Client.Themes.Theme3.Theme3, Oqtane.Client', N'oi-list-rich', N'Left;Right', N'All Users', N'Administrators', NULL, 3, 1, N'Oqtane.Client.Themes.Theme3.HorizontalLayout, Oqtane.Client')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (4, 1, N'Admin', N'admin', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'oi-home', N'Top;Bottom', N'Administrators', N'Administrators', NULL, 7, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (5, 1, N'Page Management', N'admin/pages', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'', N'Top;Bottom', N'Administrators', N'Administrators', 4, 1, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (6, 1, N'Login', N'login', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'', N'Top;Bottom', N'All Users', N'Administrators', NULL, 1, 0, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (7, 1, N'Register', N'register', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'', N'Top;Bottom', N'All Users', N'Administrators', NULL, 1, 0, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (8, 1, N'Site Management', N'admin/sites', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'', N'Top;Bottom', N'Administrators', N'Administrators', 4, 0, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (9, 1, N'User Management', N'admin/users', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'', N'Top;Bottom', N'Administrators', N'Administrators', 4, 2, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (10, 1, N'Module Management', N'admin/modules', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'', N'Top;Bottom', N'Administrators', N'Administrators', 4, 3, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (11, 1, N'Theme Management', N'admin/themes', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'', N'Top;Bottom', N'Administrators', N'Administrators', 4, 4, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (12, 2, N'Page1', N'', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'oi-home', N'Top;Bottom', N'All Users', N'Administrators', NULL, 1, 1, N'')
GO
INSERT [dbo].[Page] ([PageId], [SiteId], [Name], [Path], [ThemeType], [Icon], [Panes], [ViewPermissions], [EditPermissions], [ParentId], [Order], [IsNavigation], [LayoutType]) 
VALUES (13, 2, N'Page2', N'page2', N'Oqtane.Client.Themes.Theme2.Theme2, Oqtane.Client', N'oi-home', N'Top;Bottom', N'All Users', N'Administrators', NULL, 1, 1, N'')
GO
SET IDENTITY_INSERT [dbo].[Page] OFF 
GO

SET IDENTITY_INSERT [dbo].[Module] ON 
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (1, 1, N'Oqtane.Client.Modules.Weather, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (2, 1, N'Oqtane.Client.Modules.Counter, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (3, 1, N'Oqtane.Client.Modules.HtmlText, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (4, 1, N'Oqtane.Client.Modules.Weather, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (5, 1, N'Oqtane.Client.Modules.HtmlText, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (6, 1, N'Oqtane.Client.Modules.HtmlText, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (7, 1, N'Oqtane.Client.Modules.HtmlText, Oqtane.Client', N'Administrators;Editors;Members;', N'Administrators;Editors;')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (8, 1, N'Oqtane.Client.Modules.Admin.Pages, Oqtane.Client', N'Administrators', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (9, 1, N'Oqtane.Client.Modules.Admin.Login, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (10, 1, N'Oqtane.Client.Modules.Admin.Register, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (11, 1, N'Oqtane.Client.Modules.Admin.Admin, Oqtane.Client', N'Administrators', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (12, 1, N'Oqtane.Client.Modules.Admin.Sites, Oqtane.Client', N'Administrators', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (13, 1, N'Oqtane.Client.Modules.Admin.Users, Oqtane.Client', N'Administrators', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (14, 1, N'Oqtane.Client.Modules.Admin.ModuleDefinitions, Oqtane.Client', N'Administrators', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (15, 1, N'Oqtane.Client.Modules.Admin.Themes, Oqtane.Client', N'Administrators', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (16, 2, N'Oqtane.Client.Modules.HtmlText, Oqtane.Client', N'All Users', N'Administrators')
GO
INSERT [dbo].[Module] ([ModuleId], [SiteId], [ModuleDefinitionName], [ViewPermissions], [EditPermissions]) 
VALUES (17, 2, N'Oqtane.Client.Modules.HtmlText, Oqtane.Client', N'All Users', N'Administrators')
GO
SET IDENTITY_INSERT [dbo].[Module] OFF 
GO

SET IDENTITY_INSERT [dbo].[PageModule] ON 
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (1, 1, 1, N'Weather', N'Right', 0, N'Oqtane.Client.Themes.Theme1.Container1, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (2, 1, 2, N'Counter', N'Left', 1, N'Oqtane.Client.Themes.Theme1.Container1, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (3, 1, 3, N'Text', N'Left', 0, N'Oqtane.Client.Themes.Theme1.Container1, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (4, 2, 4, N'Weather', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (5, 2, 5, N'Text', N'Top', 1, N'Oqtane.Client.Themes.Theme1.Container1, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (6, 3, 6, N'Text', N'Left', 0, N'Oqtane.Client.Themes.Theme1.Container1, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (7, 3, 7, N'Text', N'Right', 0, N'Oqtane.Client.Themes.Theme1.Container1, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (8, 5, 8, N'Page Management', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (9, 6, 9, N'Login', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (10, 7, 10, N'Register', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (11, 4, 11, N'Administration', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (12, 8, 12, N'Site Management', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (13, 9, 13, N'User Management', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (14, 10, 14, N'Module Management', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (15, 11, 15, N'Theme Management', N'Top', 0, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (16, 12, 16, N'Text', N'Top', 1, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
INSERT [dbo].[PageModule] ([PageModuleId], [PageId], [ModuleId], [Title], [Pane], [Order], [ContainerType]) 
VALUES (17, 13, 17, N'Text', N'Top', 1, N'Oqtane.Client.Themes.Theme2.Container2, Oqtane.Client')
GO
SET IDENTITY_INSERT [dbo].[PageModule] OFF 
GO

SET IDENTITY_INSERT [dbo].[HtmlText] ON 
GO
INSERT [dbo].[HtmlText] ([HtmlTextId], [ModuleId], [Content]) 
VALUES (1, 3, N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. <br /><br /><a href="http://localhost:44357/site2/">Go To Site2</a>')
GO
INSERT [dbo].[HtmlText] ([HtmlTextId], [ModuleId], [Content]) 
VALUES (2, 5, N'Enim sed faucibus turpis in eu mi bibendum neque egestas. Quis hendrerit dolor magna eget est lorem. Dui faucibus in ornare quam viverra orci sagittis. Integer eget aliquet nibh praesent tristique magna sit. Nunc aliquet bibendum enim facilisis gravida neque convallis a cras. Tortor id aliquet lectus proin. Diam volutpat commodo sed egestas egestas fringilla. Posuere sollicitudin aliquam ultrices sagittis orci. Viverra mauris in aliquam sem fringilla ut morbi tincidunt. Eget gravida cum sociis natoque penatibus et. Sagittis orci a scelerisque purus semper. Eget velit aliquet sagittis id consectetur purus. Volutpat blandit aliquam etiam erat. Et tortor consequat id porta nibh venenatis cras. Volutpat odio facilisis mauris sit amet. Varius duis at consectetur lorem.')
GO
INSERT [dbo].[HtmlText] ([HtmlTextId], [ModuleId], [Content]) 
VALUES (3, 6, N'Id consectetur purus ut faucibus pulvinar elementum integer. Bibendum neque egestas congue quisque egestas diam in arcu. Eget nullam non nisi est sit amet facilisis. Sit amet consectetur adipiscing elit pellentesque. Id aliquet risus feugiat in. Enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Commodo odio aenean sed adipiscing. Pharetra massa massa ultricies mi quis hendrerit dolor magna. Aliquet enim tortor at auctor urna nunc. Nulla pellentesque dignissim enim sit amet. Suscipit adipiscing bibendum est ultricies integer quis auctor. Lacinia quis vel eros donec ac odio tempor. Aliquam vestibulum morbi blandit cursus risus at.')
GO
INSERT [dbo].[HtmlText] ([HtmlTextId], [ModuleId], [Content]) 
VALUES (4, 7, N'Ornare arcu dui vivamus arcu felis bibendum ut. Tortor vitae purus faucibus ornare. Lectus sit amet est placerat in egestas erat imperdiet sed. Aliquam sem et tortor consequat id. Fermentum iaculis eu non diam phasellus vestibulum. Ultricies integer quis auctor elit sed. Fermentum odio eu feugiat pretium nibh ipsum. Ut consequat semper viverra nam libero. Blandit aliquam etiam erat velit scelerisque in dictum non consectetur. At risus viverra adipiscing at in tellus. Facilisi nullam vehicula ipsum a arcu cursus vitae congue. At varius vel pharetra vel turpis nunc eget lorem dolor. Morbi non arcu risus quis varius. Turpis massa sed elementum tempus egestas.')
GO
INSERT [dbo].[HtmlText] ([HtmlTextId], [ModuleId], [Content]) 
VALUES (5, 16, N'Id consectetur purus ut faucibus pulvinar elementum integer. Bibendum neque egestas congue quisque egestas diam in arcu. Eget nullam non nisi est sit amet facilisis. Sit amet consectetur adipiscing elit pellentesque. Id aliquet risus feugiat in. Enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Commodo odio aenean sed adipiscing. Pharetra massa massa ultricies mi quis hendrerit dolor magna. Aliquet enim tortor at auctor urna nunc. Nulla pellentesque dignissim enim sit amet. Suscipit adipiscing bibendum est ultricies integer quis auctor. Lacinia quis vel eros donec ac odio tempor. Aliquam vestibulum morbi blandit cursus risus at.  <br /><br /><a href="http://localhost:44357/">Go To Site1</a>')
GO
INSERT [dbo].[HtmlText] ([HtmlTextId], [ModuleId], [Content]) 
VALUES (6, 17, N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.')
GO
SET IDENTITY_INSERT [dbo].[HtmlText] OFF 
GO

SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([UserId], [Username], [DisplayName], [Roles], [IsSuperUser]) 
VALUES (1, N'host', N'Host', N'', 1)
GO
INSERT [dbo].[User] ([UserId], [Username], [DisplayName], [Roles], [IsSuperUser]) 
VALUES (2, N'admin', N'Administrator', N'Administrators;', 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [DisplayName], [Roles], [IsSuperUser]) 
VALUES (3, N'editor', N'Editor', N'Editors;', 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [DisplayName], [Roles], [IsSuperUser]) 
VALUES (4, N'member', N'Member', N'Members;', 0)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO


