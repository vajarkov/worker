public static class DataConvert
{
    public static ushort[] FloatToRegisters(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return new ushort[]
        {
            BitConverter.ToUInt16(bytes, 0),
            BitConverter.ToUInt16(bytes, 2)
        };
    }

    public static float RegistersToFloat(ushort[] registers)
    {
        if (registers.Length != 2)
        {
            throw new ArgumentException("Registers array must contain exactly 2 elements.");
        }
        byte[] bytes = new byte[4];
        Array.Copy(BitConverter.GetBytes(registers[0]), 0, bytes, 0, 2);
        Array.Copy(BitConverter.GetBytes(registers[1]), 0, bytes, 2, 2);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return BitConverter.ToSingle(bytes, 0);
    }
    public static float ToFloat(this ushort[] args)
     {

         if (args.Length != 2)
             throw new ArgumentException("Input Array length invalid - Array langth must be '2'");

         var highValue = BitConverter.IsLittleEndian ? args[1] : args[0];
         var lowValue = BitConverter.IsLittleEndian ? args[0] : args[1];


         byte[] highRegisterBytes = BitConverter.GetBytes(highValue);
         byte[] lowRegisterBytes = BitConverter.GetBytes(lowValue);

         byte[] doubleBytes = {
                         highRegisterBytes[0],
                         highRegisterBytes[1],
                         lowRegisterBytes[0],
                         lowRegisterBytes[1],
         };

         return BitConverter.ToSingle(doubleBytes, 0);
     }
	 
     public static ushort[] ToUnsignedShortArray(this float fValue)
     {
         var value = BitConverter.SingleToInt32Bits(fValue);
         ushort low = (ushort)(value & 0x0000ffff);
         ushort high = (ushort)((value & 0xffff0000) >> 16);

         var highRegister = BitConverter.IsLittleEndian ? low : high;
         var lowRegister = BitConverter.IsLittleEndian ? high : low;

         var registers = new ushort[2] { lowRegister, highRegister };
         return registers;
     }
}