using System.Net.Sockets;
using NModbus;  
using System.Net;

//namespace ModbusClient;
public class ModbusClient
{
    private readonly IModbusFactory _modbusFactory;
    private TcpClient _tcpClient;
    private IModbusMaster _modbusMaster;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ModbusClient()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        _modbusFactory = new ModbusFactory();
    }

    public void CreateMaster(IPAddress ip, int port)
    {
        _tcpClient = new TcpClient();
        _tcpClient.Connect(ip, port);
        _modbusMaster = _modbusFactory.CreateMaster(_tcpClient);
    }

    public void Write(byte unitId, ushort address, ushort[] data)
    {
        _modbusMaster.WriteMultipleRegisters(unitId, address, data);
    }

    public void Dispose()
    {
        _modbusMaster?.Dispose();
        //_modbusMaster = null;
        _tcpClient?.Close();
        //_tcpClient = null;
    }
}
