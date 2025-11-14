using System.Linq;
using System.Threading;
using Iot.Device.CpuTemperature;
using DevicePerformance = DeviceMonitoring.Models.DevicePerformance;

//namespace PiDevicePerformanceInfo;
public class PiDevicePerformanceInfo
{
    


    private readonly CpuTemperature _cpuTemperature;
    public PiDevicePerformanceInfo()
    {
        _cpuTemperature = new CpuTemperature();
    }

    public DevicePerformance GetPerformanceInfo()
    {
        
        return new DevicePerformance()
        {
            CpuUsage = GetCpuUsage(),
            MemoryUsage = GetMemoryUsage(),
            CpuHeat = GetCpuTemperature()
        };
          
    }
    
    private float GetMemoryUsage()
    {
        var memoryInfoString = ReadMemory();
        var (memTotal, memAvialable) = ParseMemoryInfoString(memoryInfoString);
        float ramUsage = ((float)(memTotal - memAvialable) / memTotal) * 100;
        return ramUsage;
    }

    private float GetCpuUsage()
    {
        var cpuUsage = GetCpuUsagePercentage();
        return cpuUsage; 
    }

    private float GetCpuTemperature()
    {
        double temperature = 0;
        if(_cpuTemperature.IsAvailable)
        {
            temperature = _cpuTemperature.Temperature.DegreesCelsius;
        }
        return (float) temperature;
    }

    private float GetCpuUsagePercentage()
    {
        var oldValue = ReadCpu();
        Thread.Sleep(1000);
        var newValue = ReadCpu();
        var oldArray = ParseCpuString(oldValue);
        var newArray = ParseCpuString(newValue);
        return CalculateCpuUsage(oldArray, newArray);
    }
    private string ReadCpu()
    {
        using(FileStream filrStream = new FileStream("/proc/stat", FileMode.Open, FileAccess.Read))
        {
            using(StreamReader reader = new StreamReader(filrStream))
            {
                return reader.ReadLine() ?? string.Empty;
            }

        }
        //return System.IO.File.ReadLines("/proc/stat").First();
    }

    private List<double> ParseCpuString(string cpuString)
    {
        var cpuValues = cpuString.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Skip(1).ToArray();
        return cpuValues.Select(s => double.Parse(s)).ToList();
        //var splitted = cpuString.Split(' ').ToList();
        //splitted.RemoveRange(0,2);
        //return splitted.Select(s => double.Parse(s)).ToList();
        //var cpuValues = splitted.Select(x => Convert.ToDouble(x)).ToList();
        //return cpuValues;
    }

    private float CalculateCpuUsage(List<double> oldArray, List<double> newArray)
    {
        double oldIdle = oldArray[3] + oldArray[4];
        double newIdle = newArray[3] + newArray[4];

        double oldTotal = oldArray.Sum();
        double newTotal = newArray.Sum();

        double totald = newTotal - oldTotal;
        double idled = newIdle - oldIdle;

        float cpuUsage = (float)((totald - idled) / totald * 100);
        return cpuUsage;
    }

    private string ReadMemory()
    {
        using(FileStream filrStream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read))
        {
            using(StreamReader reader = new StreamReader(filrStream))
            {
                return reader.ReadToEnd();
            }

        }
        //return System.IO.File.ReadAllText("/proc/meminfo");
    }

    private (long memTotal, long memAvailable) ParseMemoryInfoString(string memoryInfoString)
    {
        memoryInfoString = String.Concat(memoryInfoString.Where(c => !Char.IsWhiteSpace(c)));
        var infoLines = memoryInfoString.Split("kB").ToList();
        var memTotal = Convert.ToInt64(infoLines[0].Split(':')[1]);
        var memAvailable = Convert.ToInt64(infoLines[2].Split(':')[1]);
        return (memTotal, memAvailable);
    }


}