namespace OhfsFileEngineMonitor;
using System;
using System.ServiceProcess;
using System.Linq;
using System.Collections.Generic;

public sealed class MonitorOhfsFileEngineService
{

    public bool ServiceExists(string ServiceName)
    {
        return ServiceController.GetServices().Any(x => x.ServiceName == ServiceName);
    }

    public bool ServiceIsRunning(string ServiceName)
    {
        ServiceController sc = new ServiceController(ServiceName);
        if(sc.Status == ServiceControllerStatus.Running){
            return true;
        }else{
            return false;
        }
    }

    public void CheckServices(IList<string> serviceNames)
    {
        foreach(var serviceName in serviceNames)
        {
            if(ServiceExists(serviceName) && ServiceIsRunning(serviceName) == false){
                try{
                    ServiceController sc = new ServiceController(serviceName);
                    
                    sc.Start();
                    while(sc.Status == ServiceControllerStatus.Stopped){
                        Thread.Sleep(1000);
                        sc.Refresh();
                    }
                }catch(Exception ex){
                     throw ex;
                }
            }
        }
    }

}