using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Linq;
using DevicePerformance = PiDevicePerformanceInfo.DevicePerformance;




namespace worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ModbusServer _modbusServer;
    private readonly PiDevicePerformanceInfo _performanceInfo;
    private readonly MBCliennt _modbusClient;
    private byte _SlaveId = 1;


    public Worker(ILogger<Worker> logger, ModbusServer modbusServer,
        ModbusClient modbusClient, PiDevicePerformanceInfo performanceInfo)
    {
        _modbusServer = modbusServer;
        _modbusClient = modbusClient;
        _performanceInfo = performanceInfo;
        _logger = logger;
}


    private static ushort[] FloatToRegisters(float value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return new ushort[]
        {
            BitConverter.ToUInt16(bytes, 0),
            BitConverter.ToUInt16(bytes, 2)
};
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?? IPAddress.Parse("127.0.0.1");
        var port = 10502;
        _modbusServer.CreateServer(ipAddress, port);
        _modbusServer.StartServer();

        _modbusClient.CreateMaster(ipAddress, port);

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _modbusClient.Dispose();
        _modbusServer.Dispose();
        return base.StopAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var performanceData = _performanceInfo.GetPerformanceInfo();
            WriteData(performanceData);
            _logger.LogInformation($"Cpu Usage:{performanceData.CpuUsage} -- Cpu Temperature:{performanceData.CpuHeat} -- Ram Usage:{performanceData.MemoryUsage} -- TimeStamp:{DateTime.Now}");
            await Task.Delay(5000, stoppingToken);
        }
    }

    private void WriteData(DevicePerformance performanceData)
    {
        var props = typeof(DevicePerformance).GetProperties();
        ushort address = 18000;
        foreach (var prop in props)
        {
            var value = Convert.ToSingle(prop.GetValue(performanceData));
            var convertedValue = FloatToRegisters(value);
            _modbusClient.Write(_SlaveId, address, convertedValue);
            address+=1;
        }
        /*    var value = Convert.ToSingle(prop.GetValue(performanceData));
            var convertedValue = value.ToBytes().ToUnsignedShortArray();
            _modbusClient.Write(_SlaveId, address, convertedValue);
            address+=1;
        }
        ushort[] data = new ushort[3];
        data[0] = (ushort)(performanceData.CpuUsage * 100);
        data[1] = (ushort)(performanceData.MemoryUsage * 100);
        data[2] = (ushort)(performanceData.CpuTemperature * 100);
        _modbusClient.Write(_SlaveId, 0, data);*/
    }

 
}
