namespace OhfsFileEngineMonitor;

public class WindowsBackgroundService : BackgroundService
{
    private readonly MonitorOhfsFileEngineService _monitorOfhsService;
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly TimeSpan _period;
    //private readonly TimeSpan _period = TimeSpan.FromSeconds(10);
    private readonly IConfiguration _configuration;
    private readonly EmailService _es ;
    private readonly String _serviceName;
    public WindowsBackgroundService(MonitorOhfsFileEngineService mOhfsService, ILogger<WindowsBackgroundService> logger, IConfiguration configuration, EmailService es)
    {
        _monitorOfhsService = mOhfsService;
        _logger = logger;
        _configuration = configuration; 
        _period = TimeSpan.FromMinutes(_configuration.GetValue<int>("PeriodTimeMinutes"));
        _es = es ;
        _serviceName = configuration.GetValue<string>("CurrentServiceName");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try{
            // Build up schedule job to monitor the ohfs service. 
            using PeriodicTimer timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    //Obtain service name from config 
                    IList<string> serviceNames = _configuration.GetSection("OhfsService:ServiceNames").Get<List<string>>();
                    await _monitorOfhsService.CheckServices(serviceNames);

                    _logger.LogInformation("Worker running at: {time} for service {service name}", DateTimeOffset.Now,serviceNames);

                }catch(Exception ex)
                {
                    _logger.LogError(ex, "{Message}", ex.Message);
                }
            }
            Environment.Exit(1);
        }catch(TaskCanceledException)
        {}
        catch(Exception ex){
            _logger.LogError(ex, "{Message}", ex.Message);
            await _es.SendEmail(_serviceName, _serviceName + " is down now. error message is " + ex.Message) ; 
            Environment.Exit(1);
        }
    
    }
}
