using Base.Filters;
using Base.Options;
using FluentValidation.AspNetCore;
using MassTransit.Internals;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using Shared.Constants;
using Shared.Extensions;
using System.Text;
using User.Api.Configuration;
using User.Infrastructure;
using User.Infrastructure.Configuration.DataAccess;
using User.Infrastructure.Configuration.Mediation;
using User.Infrastructure.Configuration.Processing;
using User.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder);
var appSetting = InitializeApp(builder.Configuration);

var app = builder.Build();
ConfigureApp(app);

//using (var scope = host.Services.CreateScope())
//{
//    var dbContextOptions = scope.ServiceProvider.GetRequiredService<DbContextOptions<UserDb>>();
//    dbContextOptions.In
//    using (var dbContext = dbContextOptions)
//    {
//        dbContext.InitDb();
//    }
//}

app.Run();

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.AddServiceDefaults();
    //Setting up request size
    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = int.MaxValue; // Set to 50 MB (adjust as needed)
    });
    //Setting FormData size limit
    builder.Services.Configure<FormOptions>(x =>
    {
        x.ValueLengthLimit = int.MaxValue;
        x.MultipartBodyLengthLimit = int.MaxValue;
        x.MultipartHeadersLengthLimit = int.MaxValue;
    });
    builder.Services.AddHttpContextAccessor();
    builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("JwtOptions"));
    var jwtOption = builder.Configuration.GetSection("JwtOptions").Get<JwtOption>();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOption.Secret))
                };
            });
    builder.Services.AddCors(o => o.AddPolicy("corsPolicy", builder =>
    {
        builder.SetIsOriginAllowedToAllowWildcardSubdomains()
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));
    builder.Services.AddFluentValidation(mv => mv.RegisterValidatorsFromAssembly(AppDomain.CurrentDomain.Load("User.Application")));
    builder.Services.AddControllers(config => config.Filters.Add(typeof(ApiResultFilterAttribute)));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    var swagconfig = builder.Configuration.GetSection("SwaggerAuth");
    ConfigSwagger(builder.Services);
    builder.Services.AddLogging(configure =>
    {
        configure.ClearProviders();
        configure.AddJsonConsole(opts =>
        {
            opts.TimestampFormat = "s";
        });
    });
    builder.Services.Configure<ApiBehaviorOptions>(config => config.SuppressModelStateInvalidFilter = false);
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.AddVersionSupport(builder.Configuration.GetSection("Version").Get<VersionSettings>());
    builder.Services.RegisterSessionManager();
    builder.Services.AddSingleton<IUserModule, UserModule>();
    builder.AddSqlServerDbContext<UserDb>("User");
    builder.Services.AddDataAccessModule();
    builder.Services.AddMediatorModule(loggingEnabled: true);
    builder.Services.AddServiceModule();
    builder.Services.AddRepositories();    
}

AppSettings InitializeApp(IConfiguration configuration)
{
    IConfigurationSection appSettings = configuration.GetSection("AppSettings");
    var appSetting = appSettings.Get<AppSettings>();
    // Register service to discovery
    builder.Services.RegisterService(appSetting.ServiceConfig);

    //appSetting.RabitMQConfiguration.UserName = Environment.GetEnvironmentVariable(CommonEnvVariables.RabbitMQUserName) ?? string.Empty;
    //appSetting.RabitMQConfiguration.Password = Environment.GetEnvironmentVariable(CommonEnvVariables.RabbitMQPassword) ?? string.Empty;
    //appSetting.RabitMQConfiguration.HostName = Environment.GetEnvironmentVariable(CommonEnvVariables.RabbitMQHost) ?? string.Empty;
    //appSetting.RabitMQConfiguration.Port = Convert.ToUInt16(Environment.GetEnvironmentVariable(CommonEnvVariables.RabbitMQPort));
    return appSetting;
}
void ConfigSwagger(IServiceCollection serviceCollection)
{
    // Add SwaggerGen for API documentation
    serviceCollection.AddSwaggerGen();
}
void ConfigureApp(WebApplication app)
{
    app.UseCors("corsPolicy");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}