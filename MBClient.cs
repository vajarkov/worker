using System;
using NModbus;
using System.Net;
using System.Net.Sockets;

public class MBClient
{
    private readonly IModbusFactory _modbusFactory;
    private IModbusMaster? _master;
    private TcpClient? _tcpClient;
    public MBClient()
    {
        _modbusFactory = new ModbusFactory();
    }

    public void CreateMaster(IPAddress ip, int port)
    {
        _tcpClient = new TcpClient();
        _tcpClient.Connect(ip, port);
        _master = _modbusFactory.CreateMaster(_tcpClient);
    }
    public void Write(byte unitId, ushort address, ushort[] data)
    {
        if (_master == null) throw new InvalidOperationException("Master not created. Call CreateMaster first.");
        _master.WriteMultipleRegisters(unitId, address, data);
    }
    
    public void Dispose()
    {
        _master?.Dispose();
        _master = null;
        _tcpClient?.Close();
        _tcpClient=null;
    }

   
}