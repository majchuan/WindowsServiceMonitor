namespace OhfsFileEngineMonitor;
using System;
using System.ServiceProcess;
using System.Linq;
using System.Collections.Generic;
using SendGrid.Helpers.Mail;

public sealed class MonitorOhfsFileEngineService
{
    private readonly EmailService _es;
    public MonitorOhfsFileEngineService(EmailService es) => _es = es;

    public bool ServiceExists(string ServiceName)
    {
        return ServiceController.GetServices().Any(x => x.ServiceName == ServiceName);
    }

    public async Task<bool> ServiceIsRunning(string ServiceName)
    {
        ServiceController sc = new ServiceController(ServiceName);
        if(sc.Status == ServiceControllerStatus.Running){
            return true;
        }else{
            await _es.SendEmail(ServiceName, "This service is down now.");
            return false;
        }
    }

    public async Task CheckServices(IList<string> serviceNames)
    {
        foreach(var serviceName in serviceNames)
        {
            if(ServiceExists(serviceName) && ServiceIsRunning(serviceName).Result == false){
                try{
                    ServiceController sc = new ServiceController(serviceName);
                    sc.Start();

                    while(sc.Status == ServiceControllerStatus.Stopped){
                        Thread.Sleep(1000);
                        sc.Refresh();
                    }

                    await _es.SendEmail(serviceName , "This service is up now.");

                }catch(Exception ex){
                     throw ex;
                }
            }
        }
    }

}