using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin.Cors;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Data.Entity;
using Data;
using Repository;
using Contracts;
using Entities;

namespace ApiEFAutofac
{
    /// <summary>
    /// Represents a class that encapsulates several Web Api configurations: CORS, routing, formatters, exception handling, dependency injection
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposing HttpConfiguration will break Web API")]
    public sealed class ApiConfig
    {
        private readonly HttpConfiguration _configuration = new HttpConfiguration();
        private readonly IAppBuilder _app;

        public ApiConfig(IAppBuilder app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
        }

        /// <summary>
        /// Initializes and configures <see cref="CorsOptions"/> instance.
        /// </summary>
        /// <param name="origins">String of allowed origins delimited by: ';'</param>
        public ApiConfig ConfigureCorsMiddleware(string origins)
        {
            var corsOption = CorsOptions.AllowAll;

            if (string.IsNullOrWhiteSpace(origins))
                _app.UseCors(corsOption);

            var corsPolicy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true
            };

            // StringSplitOptions.RemoveEmptyEntries doesn't remove whitespaces.
            origins.Split(';')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList()
                .ForEach(origin => corsPolicy.Origins.Add(origin));

            if (!corsPolicy.Origins.Any())
                _app.UseCors(corsOption);

            var corsOptions = new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(corsPolicy)
                }
            };

            _app.UseCors(corsOption);

            return this;
        }

        /// <summary>
        /// Configures formatter to use JSON only.
        /// </summary>
        public ApiConfig ConfigureFormatters()
        {
            // Delete 2 lines below if you need support for formatters other than json
            _configuration.Formatters.Clear();
            _configuration.Formatters.Add(new JsonMediaTypeFormatter());

            _configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                = new CamelCasePropertyNamesContractResolver();
            _configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            _configuration.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            _configuration.Formatters.JsonFormatter.SupportedMediaTypes.Clear();

            // Delete the line below if you nee more content type. E.g. application/text
            _configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            return this;
        }

        /// <summary>
        /// Configures Web API routes.
        /// </summary>
        public ApiConfig ConfigureRoutes()
        {
            _configuration.MapHttpAttributeRoutes();
            _configuration.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            return this;
        }

        /// <summary>
        /// Configures custom implementations for: <see cref="IExceptionHandler"/> and <see cref="IExceptionLogger"/>.
        /// </summary>
        public ApiConfig ConfigureExceptionHandling()
        {
            _configuration.Services.Replace(typeof(IExceptionHandler), new ApiExceptionHandler());
            _configuration.Services.Add(typeof(IExceptionLogger), new ApiExceptionLogger());

            return this;
        }

        /// <summary>
        /// Initializes and configures instance of <see cref="IContainer"/>.
        /// </summary>
        public ApiConfig ConfigureAufacMiddleware()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WebApiDbContext>()
                  .As<DbContext>()
                  .InstancePerRequest();

            builder.RegisterType<DbFactory>()
                   .As<IDbFactory>()
                   .InstancePerRequest();

            builder.RegisterGeneric(typeof(GenericRepository<>))
                  .As(typeof(IGenericRepository<>))
                  .InstancePerRequest();

            builder.RegisterType<StandardRepository>().As<IStandardRepository<Standard>>();
            builder.RegisterType<StudentRepository>().As<IStudentRepository<Student>>();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();

            _configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            _app.UseAutofacMiddleware(container);
            return this;
        }
        public void UseWebApi()
        {
            _app.UseWebApi(_configuration);
        }
    }
}