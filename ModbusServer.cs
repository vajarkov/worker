using NModbus;
using System.Net;
using System.Net.Sockets;


public class ModbusServer
{
    private readonly IModbusFactory _modbusFactory;
    private TcpListener _tcpListener;
    private IModbusSlaveNetwork _slaveNetwork;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ModbusServer()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        _modbusFactory = new ModbusFactory();
    }

    public void CreateServer(IPAddress ip, int port)
    {
        _tcpListener = new TcpListener(ip, port);
        _tcpListener.Start();
        var slave = _modbusFactory.CreateSlave(1);
        _slaveNetwork = _modbusFactory.CreateSlaveNetwork(_tcpListener);
        //_slaveNetwork.AddSlave(slave);
        //_slaveNetwork.ListenAsync();
    }

    public void AddSlave(byte slaveId)
    {
        var slave = _modbusFactory.CreateSlave(slaveId);
        _slaveNetwork.AddSlave(slave);
    }

    public void StartServer()
    {
        _slaveNetwork.ListenAsync();
    }

    public void Dispose()
    {
        //_slaveNetwork.Dispose();
        //_tcpListener.Stop();
        //_tcpListener.Dispose();

        _slaveNetwork?.Dispose();
        //_slaveNetwork = null;
        _tcpListener?.Stop();
        //_tcpListener = null;
    }
    
}