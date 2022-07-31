using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared;
using Staff.Data;
using Staff.ViewModel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OptionsJWT>(builder.Configuration.GetSection("jwt"));
builder.Services.Configure<ConsulServiceSetting>(builder.Configuration.GetSection("ConsulServiceSetting"));

ConsulServiceSetting consulService = new ConsulServiceSetting { };
builder.Configuration.Bind("ConsulServiceSetting", consulService);

builder.Services.AddConsulToApp(consulService.DiscoveryAddress);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Assessment",
        Description = "API for staff",
        Contact = new OpenApiContact
        {
            Name = "Safwan Osama",
            Email = "sfwan07@gmail.com",
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.",
    });


    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

});
builder.Services.AddDBSqlServer(builder.Configuration.GetConnectionString("db"),typeof(Program).Assembly.FullName);
builder.Services.AddAutoMapper(typeof(Program));


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

builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddCors();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(options =>
{
    options.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        if (httpReq.HttpContext.Request.Headers.ContainsKey("Referer"))
        {
            var refererUrl = new Uri(httpReq.HttpContext.Request.Headers["Referer"]);
            swagger.Servers.Add(new OpenApiServer { Url = $"{refererUrl.Scheme}://{refererUrl.Host}:{refererUrl.Port}/api/staff" });
        }
        var serverUrl = $"{httpReq.Scheme}://{httpReq.Host}";
        swagger.Servers.Add(new OpenApiServer { Url = serverUrl });
    }
                );
});
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

app.UseEndpoints(endpoints => { 
    endpoints.MapControllers().RequireAuthorization();
});

app.UseConsul(app.Lifetime);

app.Run();
