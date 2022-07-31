using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Identity.Data;
using Identity.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Net.Http.Headers;
using Identity.Repository;
using Microsoft.Extensions.Options;
using Shared;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("db");

builder.Services.Configure<OptionsJWT>(builder.Configuration.GetSection("jwt"));
builder.Services.Configure<SeedData>(builder.Configuration.GetSection("SeedData"));
builder.Services.Configure<ConsulServiceSetting>(builder.Configuration.GetSection("ConsulServiceSetting"));

ConsulServiceSetting consulService = new ConsulServiceSetting { };
builder.Configuration.Bind("ConsulServiceSetting", consulService);

builder.Services.AddConsulToApp(consulService.DiscoveryAddress);

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(
                options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}
            )
            .AddJwtBearer(jwt =>
            {

                OptionsJWT jwtOption = new OptionsJWT { };
                builder.Configuration.Bind("jwt",jwtOption);
                var tokenValidation = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireAudience = true,
                    RequireExpirationTime = true,
                    RoleClaimType = ClaimTypeCustom.Roles.ToString(),
                    NameClaimType = ClaimTypeCustom.SID.ToString(),
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOption.ValidIssuer,
                    ValidAudience = jwtOption.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.Secret))
                };
                jwt.TokenValidationParameters = tokenValidation;
                jwt.SaveToken = true;
                jwt.RequireHttpsMetadata = false;
                jwt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var access_token = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(access_token))
                        {
                            context.Token = access_token;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

builder.Services.AddCors();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddRouting();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    IServiceProvider service = scope.ServiceProvider;

    IdentityContext dataContext = service.GetRequiredService<IdentityContext>();
    dataContext.Database.Migrate();

    SeedData seedData= new SeedData { } ;
    builder.Configuration.Bind("SeedData", seedData);

    var roleManager= service.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var userManager= service.GetRequiredService<UserManager<IdentityUser<Guid>>>();
    seedData.User.ForEach((user) =>
    {
        if (!userManager.Users.AnyAsync(p=> p.UserName == user.UserName).GetAwaiter().GetResult())
        {
            var result = userManager.CreateAsync(new IdentityUser<Guid>
            {
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = true
            }, user.Password).GetAwaiter().GetResult();
            if (result.Errors.Any())
                throw new Exception();
        }
    });

    seedData.Role.ForEach((role) =>
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            var result = roleManager.CreateAsync(new IdentityRole<Guid>
            {
                Name = role,
                NormalizedName = role.ToUpper(),
            }).GetAwaiter().GetResult();
            if (result.Errors.Any())
                throw new Exception();
        }
    });

    seedData.UserRole.ForEach((userRole) =>
    {
        var user = userManager.FindByNameAsync(userRole.User).GetAwaiter().GetResult();
        if(!userManager.IsInRoleAsync(user,userRole.Role).GetAwaiter().GetResult())
            userManager.AddToRoleAsync(user,userRole.Role).GetAwaiter().GetResult();
    });

}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(options =>
{
    options.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        if (httpReq.HttpContext.Request.Headers.ContainsKey("Referer"))
        {
            var refererUrl = new Uri(httpReq.HttpContext.Request.Headers["Referer"]);
            swagger.Servers.Add(new OpenApiServer { Url = $"{refererUrl.Scheme}://{refererUrl.Host}:{refererUrl.Port}/api/identity" });
        }
        var serverUrl = $"{httpReq.Scheme}://{httpReq.Host}";
        swagger.Servers.Add(new OpenApiServer { Url = serverUrl });
    }
                );
});
app.UseSwaggerUI();
//}

app.UseCors((c) =>
{
    c.AllowAnyOrigin();
    c.AllowAnyMethod();
    c.AllowAnyHeader();
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoint =>
{
    endpoint.MapControllers();
});

app.UseConsul(app.Lifetime);

app.Run();