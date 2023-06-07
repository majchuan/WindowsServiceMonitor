using OhfsFileEngineMonitor;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>{

        options.ServiceName = "Ohfs_Monitor_Service";
        
    }).ConfigureServices((context, services) =>{

        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);

        services.AddSingleton<EmailService>();
        services.AddSingleton<MonitorOhfsFileEngineService>();
        services.AddHostedService<WindowsBackgroundService>();
        
        services.AddLogging(builder =>{
            builder.AddConfiguration(context.Configuration.GetSection("Logging"));
        });

        services.AddSendGrid(options =>{
            options.ApiKey = context.Configuration.GetValue<String>("SendGrid_API_Key");
        });
    });

IHost host = builder.Build();
host.Run();
