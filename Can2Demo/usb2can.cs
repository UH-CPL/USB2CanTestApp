using System;
using System.Text;
using System.Runtime.InteropServices;


namespace Can2Tool
{
    public struct CanalMsg
        {
            public uint flags;                // CAN message flags
            public uint obid;                 // Used by driver for channel info etc.
            public uint id;                   // CAN id (11-bit or 29-bit)
            public byte sizeData;             // Data size 0-8
            public byte data0;               // CAN Data	
            public byte data1;               // CAN Data	
            public byte data2;               // CAN Data	
            public byte data3;               // CAN Data	
            public byte data4;               // CAN Data	
            public byte data5;               // CAN Data	
            public byte data6;               // CAN Data	
            public byte data7;               // CAN Data	
            public uint timestamp;            // Relative time stamp for package in microseconds
        } ;

    public struct CanelStat
    {
        public uint RxCount;
        public uint TxCount;
        public uint RxBytes;
        public uint TxBytes;
        public uint Overuns;
        public uint Warnings;
        public uint BusOffs;
    }
    
    
    public class usb2can
    {
        [DllImport("usb2can.dll", EntryPoint = "CanalOpen")]
        public static extern uint CanalOpen(char[] Initstrring, int CANMsgType);

        [DllImport("usb2can.dll", EntryPoint = "CanalSend")]
        public static extern int CanalSend(uint handle, ref CanalMsg pCanalMsg);

        [DllImport("usb2can.dll", EntryPoint = "CanalGetStatistics")]
        public static extern int CanalGetStatistics(uint handle, ref CanelStat Statistic);

        [DllImport("usb2can.dll", EntryPoint = "CanalDataAvailable")]
        public static extern int CanalDataAvailable(uint handle);

        [DllImport("usb2can.dll", EntryPoint = "CanalReceive")]
        public static extern int CanalReceive(uint handle, ref CanalMsg pCanalMsg);

        [DllImport("usb2can.dll", EntryPoint = "CanalClose")]
        public static extern int CanalClose(uint handel);


    }
}