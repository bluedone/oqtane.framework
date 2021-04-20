using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Services;
using Oqtane.Shared;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class OqtaneServiceCollectionExtensions
    {
        public static IServiceCollection AddOqtane(this IServiceCollection services, Runtime runtime, string[] supportedCultures)
        {
            LoadAssemblies();
            LoadSatelliteAssemblies(supportedCultures);
            services.AddOqtaneServices(runtime);

            return services;
        }

        public static IServiceCollection AddOqtaneDbContext(this IServiceCollection services)
        {
            services.AddScoped<IDbConfig, DbConfig>();
            services.AddDbContext<MasterDBContext>(options => { });
            services.AddDbContext<TenantDBContext>(options => { });

            return services;
        }

        public static IServiceCollection AddOqtaneAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy(PolicyNames.ViewPage, policy => policy.Requirements.Add(new PermissionRequirement(EntityNames.Page, PermissionNames.View)));
                options.AddPolicy(PolicyNames.EditPage, policy => policy.Requirements.Add(new PermissionRequirement(EntityNames.Page, PermissionNames.Edit)));
                options.AddPolicy(PolicyNames.ViewModule, policy => policy.Requirements.Add(new PermissionRequirement(EntityNames.Module, PermissionNames.View)));
                options.AddPolicy(PolicyNames.EditModule, policy => policy.Requirements.Add(new PermissionRequirement(EntityNames.Module, PermissionNames.Edit)));
                options.AddPolicy(PolicyNames.ViewFolder, policy => policy.Requirements.Add(new PermissionRequirement(EntityNames.Folder, PermissionNames.View)));
                options.AddPolicy(PolicyNames.EditFolder, policy => policy.Requirements.Add(new PermissionRequirement(EntityNames.Folder, PermissionNames.Edit)));
                options.AddPolicy(PolicyNames.ListFolder, policy => policy.Requirements.Add(new PermissionRequirement(EntityNames.Folder, PermissionNames.Browse)));
            });

            return services;
        }

        internal static IServiceCollection AddOqtaneScopedServices(this IServiceCollection services)
        {
            services.AddScoped<SiteState>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IInstallationService, InstallationService>();
            services.AddScoped<IModuleDefinitionService, ModuleDefinitionService>();
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAliasService, AliasService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ISiteService, SiteService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IPageModuleService, PageModuleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobLogService, JobLogService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ISiteTemplateService, SiteTemplateService>();
            services.AddScoped<ISqlService, SqlService>();
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILanguageService, LanguageService>();

            return services;
        }

        internal static IServiceCollection AddOqtaneSingletonServices(this IServiceCollection services)
        {
            services.AddSingleton<IInstallationManager, InstallationManager>();
            services.AddSingleton<ISyncManager, SyncManager>();
            services.AddSingleton<IDatabaseManager, DatabaseManager>();

            return services;
        }

        internal static IServiceCollection AddOqtaneTransientServices(this IServiceCollection services)
        {
            services.AddTransient<IModuleDefinitionRepository, ModuleDefinitionRepository>();
            services.AddTransient<IThemeRepository, ThemeRepository>();
            services.AddTransient<IUserPermissions, UserPermissions>();
            services.AddTransient<ITenantResolver, TenantResolver>();
            services.AddTransient<IAliasRepository, AliasRepository>();
            services.AddTransient<ITenantRepository, TenantRepository>();
            services.AddTransient<ISiteRepository, SiteRepository>();
            services.AddTransient<IPageRepository, PageRepository>();
            services.AddTransient<IModuleRepository, ModuleRepository>();
            services.AddTransient<IPageModuleRepository, PageModuleRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProfileRepository, ProfileRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRoleRepository, UserRoleRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<ILogManager, LogManager>();
            services.AddTransient<ILocalizationManager, LocalizationManager>();
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddTransient<IJobLogRepository, JobLogRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IFolderRepository, FolderRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<ISiteTemplateRepository, SiteTemplateRepository>();
            services.AddTransient<ISqlRepository, SqlRepository>();
            services.AddTransient<IUpgradeManager, UpgradeManager>();
            services.AddTransient<ILanguageRepository, LanguageRepository>();

            return services;
        }

        public static IServiceCollection ConfigureOqtaneCookieOptions(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;

                    return Task.CompletedTask;
                };
            });

            return services;
        }

        public static IServiceCollection ConfigureOqtaneIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;
            });

            return services;
        }

        internal static IServiceCollection TryAddHttpClientWithAuthenticationCookie(this IServiceCollection services)
        {
            if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
            {
                services.AddScoped(s =>
                {
                    // creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                    var navigationManager = s.GetRequiredService<NavigationManager>();
                    var httpContextAccessor = s.GetRequiredService<IHttpContextAccessor>();
                    var authToken = httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
                    var client = new HttpClient(new HttpClientHandler { UseCookies = false });
                    if (authToken != null)
                    {
                        client.DefaultRequestHeaders.Add("Cookie", ".AspNetCore.Identity.Application=" + authToken);
                    }

                    client.BaseAddress = new Uri(navigationManager.Uri);

                    return client;
                });
            }

            return services;
        }

        internal static IServiceCollection TryAddSwagger(this IServiceCollection services, bool useSwagger)
        {
            if (useSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Oqtane", Version = "v1" });
                });
            }

            return services;
        }

        private static IServiceCollection AddOqtaneServices(this IServiceCollection services, Runtime runtime)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var hostedServiceType = typeof(IHostedService);

            var assemblies = AppDomain.CurrentDomain.GetOqtaneAssemblies();
            foreach (var assembly in assemblies)
            {
                // dynamically register module services, contexts, and repository classes
                var implementationTypes = assembly.GetInterfaces<IService>();
                foreach (var implementationType in implementationTypes)
                {
                    if (implementationType.AssemblyQualifiedName != null)
                    {
                        var serviceType = Type.GetType(implementationType.AssemblyQualifiedName.Replace(implementationType.Name, $"I{implementationType.Name}"));
                        services.AddScoped(serviceType ?? implementationType, implementationType);
                    }
                }

                // dynamically register database providers
                var databaseTypes = assembly.GetInterfaces<IOqtaneDatabase>();
                foreach (var databaseType in databaseTypes)
                {
                    if (databaseType.AssemblyQualifiedName != null)
                    {
                        var serviceType = Type.GetType("Oqtane.Interfaces.IOqtaneDatabase, Oqtane.Shared");
                        services.AddScoped(serviceType ?? databaseType, databaseType);
                    }
                }

                // dynamically register hosted services
                var serviceTypes = assembly.GetTypes(hostedServiceType);
                foreach (var serviceType in serviceTypes)
                {
                    if (serviceType.IsSubclassOf(typeof(HostedServiceBase)))
                    {
                        services.AddSingleton(hostedServiceType, serviceType);
                    }
                }

                // register server startup services
                var startUps = assembly.GetInstances<IServerStartup>();
                foreach (var startup in startUps)
                {
                    startup.ConfigureServices(services);
                }

                if (runtime == Runtime.Server)
                {
                    // register client startup services if running on server
                    assembly.GetInstances<IClientStartup>()
                        .ToList()
                        .ForEach(x => x.ConfigureServices(services));
                }
            }
            return services;
        }

        private static void LoadAssemblies()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (assemblyPath == null) return;

            AssemblyLoadContext.Default.Resolving += ResolveDependencies;

            var assembliesFolder = new DirectoryInfo(assemblyPath);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // iterate through Oqtane assemblies in /bin ( filter is narrow to optimize loading process )
            foreach (var dll in assembliesFolder.EnumerateFiles($"*.dll", SearchOption.TopDirectoryOnly).Where(f => f.IsOqtaneAssembly()))
            {
                AssemblyName assemblyName;
                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(dll.FullName);
                }
                catch
                {
                    Console.WriteLine($"Not Assembly : {dll.Name}");
                    continue;
                }

                if (!assemblies.Any(a => AssemblyName.ReferenceMatchesDefinition(assemblyName, a.GetName())))
                {
                    try
                    {
                        var pdb = Path.ChangeExtension(dll.FullName, ".pdb");
                        Assembly assembly = null;

                        // load assembly ( and symbols ) from stream to prevent locking files ( as long as dependencies are in /bin they will load as well )
                        if (File.Exists(pdb))
                        {
                            assembly = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(File.ReadAllBytes(dll.FullName)), new MemoryStream(File.ReadAllBytes(pdb)));
                        }
                        else
                        {
                            assembly = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(File.ReadAllBytes(dll.FullName)));
                        }
                        Console.WriteLine($"Loaded : {assemblyName}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed : {assemblyName}\n{e}");
                    }
                }
            }
        }

        private static void LoadSatelliteAssemblies(string[] supportedCultures)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (assemblyPath == null)
            {
                return;
            }

            AssemblyLoadContext.Default.Resolving += ResolveDependencies;

            foreach (var culture in supportedCultures)
            {
                if (culture == Constants.DefaultCulture)
                {
                    continue;
                }

                var assembliesFolder = new DirectoryInfo(Path.Combine(assemblyPath, culture));
                if (assembliesFolder.Exists)
                {
                    foreach (var assemblyFile in assembliesFolder.EnumerateFiles(Constants.SatelliteAssemblyExtension))
                    {
                        AssemblyName assemblyName;
                        try
                        {
                            assemblyName = AssemblyName.GetAssemblyName(assemblyFile.FullName);
                        }
                        catch
                        {
                            Console.WriteLine($"Not Satellite Assembly : {assemblyFile.Name}");
                            continue;
                        }

                        try
                        {
                            Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(File.ReadAllBytes(assemblyFile.FullName)));
                            Console.WriteLine($"Loaded : {assemblyName}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed : {assemblyName}\n{e}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"The satellite assemblies folder named '{culture}' is not found.");
                }
            }
        }

        private static Assembly ResolveDependencies(AssemblyLoadContext context, AssemblyName name)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) + Path.DirectorySeparatorChar + name.Name + ".dll";
            if (File.Exists(assemblyPath))
            {
                return context.LoadFromStream(new MemoryStream(File.ReadAllBytes(assemblyPath)));
            }
            else
            {
                return null;
            }
        }

    }
}
