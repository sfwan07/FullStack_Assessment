using Consul;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Shared;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureAppConfiguration(webBuilder => {
    webBuilder.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();
    //webBuilder.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
    //    webBuilder.AddJsonFile($"configuration.{builder.Environment.EnvironmentName}.json", true, true);
});

builder.Services.Configure<ConsulServiceSetting>(builder.Configuration.GetSection("ConsulServiceSetting"));

ConsulServiceSetting consulService = new ConsulServiceSetting { };
builder.Configuration.Bind("ConsulServiceSetting", consulService);

builder.Services.AddConsulToApp(consulService.DiscoveryAddress);
builder.Services.AddOcelot().AddConsul();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    builder.Configuration.GetSection("APIs").Get<string[]>().ToList().ForEach(a =>
    {
        c.SwaggerEndpoint($"/api/{a}/swagger/v1/swagger.json", $"{a} v1");
    });
    c.EnableDeepLinking();
});
//}

app.UseConsul(app.Lifetime);
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
