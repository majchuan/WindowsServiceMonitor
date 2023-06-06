using OhfsFileEngineMonitor;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>{

        options.ServiceName = "Monitor_Service";
        
    }).ConfigureServices((context, services) =>{

        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);

        services.AddSingleton<MonitorOhfsFileEngineService>();
        services.AddHostedService<WindowsBackgroundService>();
        
        services.AddLogging(builder =>{
            builder.AddConfiguration(context.Configuration.GetSection("Logging"));
        });
    });

IHost host = builder.Build();
host.Run();
