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
        if(sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending){
            return true;
        }else{
            await _es.SendEmail(ServiceName, "This service "+ServiceName+" is down now.");
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
                    var counter = 0 ;

                    while(sc.Status == ServiceControllerStatus.Stopped && counter < 3){
                        Thread.Sleep(1000);
                        sc.Refresh();
                        counter++;
                    }
                    if(sc.Status == ServiceControllerStatus.StartPending  || sc.Status == ServiceControllerStatus.Running){
                        await _es.SendEmail(serviceName,"This service "+serviceName+" is up now.");
                    }else{
                        await _es.SendEmail(serviceName, "This service "+serviceName+" is tried restart three time, but it failed. please check the service asap."+
                        "curr status"+ sc.Status);
                    }
                }catch(Exception ex){
                     throw ex;
                }
            }
        }
    }

}