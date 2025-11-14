using NModbus;
using System.Net;
using System.Net.Sockets;

//namespace ModbusServer;
public class ModbusServer
{
    private readonly IModbusFactory _modbusFactory;
    private TcpListener? _tcpListener;
    private IModbusSlaveNetwork? _slaveNetwork;

    public ModbusServer()
    {
        _modbusFactory = new ModbusFactory();
    }

    public void CreateServer(IPAddress ip, int port)
    {
        _tcpListener = new TcpListener(ip, port);
        var slave = _modbusFactory.CreateSlave(1);
        _slaveNetwork = _modbusFactory.CreateSlaveNetwork(_tcpListener);
        _slaveNetwork.AddSlave(slave);
    }
    public void StartServer()
    {
        if (_slaveNetwork == null)
        {
            throw new InvalidOperationException("Server not created. Call CreateServer first.");
        }
        _slaveNetwork.ListenAsync();
    }
    public void Dispose()
    {
        _slaveNetwork?.Dispose();
        _slaveNetwork = null;
        _tcpListener?.Stop();
        _tcpListener = null;
    }    
}