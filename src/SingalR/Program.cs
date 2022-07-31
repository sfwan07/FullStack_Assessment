using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared;
using SingalR.API;
using SingalR.API.Data;
using SingalR.API.Model;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OptionsJWT>(builder.Configuration.GetSection("jwt"));
builder.Services.Configure<ConsulServiceSetting>(builder.Configuration.GetSection("ConsulServiceSetting"));

ConsulServiceSetting consulService = new ConsulServiceSetting { };
builder.Configuration.Bind("ConsulServiceSetting", consulService);

builder.Services.AddConsulToApp(consulService.DiscoveryAddress);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

builder.Services.AddRouting();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("db"));
});


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
                builder.Configuration.Bind("jwt", jwtOption);
                var tokenValidation = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireAudience = true,
                    RequireExpirationTime = true,
                    RoleClaimType = "Roles",
                    NameClaimType = "SID",
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
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseRouting();
app.UseCors((c) =>
{
    c.AllowAnyOrigin();
    c.AllowAnyMethod();
    c.AllowAnyHeader();
});
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<AppHub>("/hub", config =>
    {
    }).RequireAuthorization();
});

app.UseConsul(app.Lifetime);

app.Run();
