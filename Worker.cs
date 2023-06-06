namespace OhfsFileEngineMonitor;

public class WindowsBackgroundService : BackgroundService
{
    private readonly MonitorOhfsFileEngineService _monitorOfhsService;
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly TimeSpan _period;
    //private readonly TimeSpan _period = TimeSpan.FromSeconds(10);

    private readonly IConfiguration _configuration;

    public WindowsBackgroundService(MonitorOhfsFileEngineService mOhfsService, ILogger<WindowsBackgroundService> logger, IConfiguration configuration)
    {
        _monitorOfhsService = mOhfsService;
        _logger = logger;
        _configuration = configuration; 
        _period = TimeSpan.FromMinutes(_configuration.GetValue<int>("PeriodTimeMinutes"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try{
            using PeriodicTimer timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Build up schedule job to monitor the ohfs service. 
                try
                {
                    //Obtain service name from config 
                    IList<string> serviceNames = _configuration.GetSection("OhfsService:ServiceNames").Get<List<string>>();
                    _monitorOfhsService.CheckServices(serviceNames);

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
            Environment.Exit(1);
        }
    
    }
}
