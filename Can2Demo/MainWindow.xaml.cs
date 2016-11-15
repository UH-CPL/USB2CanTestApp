using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;

namespace Can2Tool
{
    /// <summary>
    /// </summary>
    public partial class MainWindow : Window
    {
        uint CanIF = 0;
        char[] Initstring = "ED000200 ; 500".ToCharArray() ;
        string data = null;

        public static CanalMsg TxMsg = new CanalMsg();
        public static CanalMsg RxMsg = new CanalMsg();

        Timer timer;
        
        public MainWindow()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimer);
            timer.Interval = 1000;
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            GetCANData();
        }

        private void TaStart_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "Start";
            if ((CanIF = usb2can.CanalOpen(Initstring, 0)) <= 0)
            {
                Display.AppendText(Environment.NewLine);
                Display.AppendText("Failed to open USB2CAN interface.");
                CanIF = 0;
            }
            else
            {
                Display.AppendText(Environment.NewLine);
                Display.AppendText("Init O.K. :" + CanIF.ToString());
                Display.AppendText(Environment.NewLine);

            }

        }

        private void TaSend_Click(object sender, RoutedEventArgs e)
        {
            TxMsg.flags = 0x00000001;
            TxMsg.id = 0x1234560A;
            TxMsg.sizeData = 8;
            TxMsg.data0 = 0x55;
            TxMsg.data1 = 0x01;
            TxMsg.data2 = 0x02;
            TxMsg.data3 = 0x03;
            TxMsg.data4 = 0x04;
            TxMsg.data5 = 0x05;
            TxMsg.data6 = 0x06;
            TxMsg.data7 = 0x07;

            int Err = usb2can.CanalSend(CanIF, ref TxMsg);
            Display.AppendText(Environment.NewLine);
            Display.AppendText("Send :" + Err.ToString());
        }

        private void TaStatistic_Click(object sender, RoutedEventArgs e)
        {
            CanelStat Statistic = new CanelStat();
            int Err = usb2can.CanalGetStatistics(CanIF, ref  Statistic);
            Display.AppendText(Environment.NewLine);
            Display.AppendText("Statistic :" + Err.ToString());
            Display.AppendText(Environment.NewLine);
            Display.AppendText("RxCount: " + Statistic.RxCount.ToString());
            Display.AppendText(Environment.NewLine);
            Display.AppendText("TxCount: " + Statistic.TxCount.ToString());

        }

        public void GetCANData()
        {
            int Err = usb2can.CanalDataAvailable(CanIF);
            // Display.AppendText(Environment.NewLine);
            string rxData = "Rx Count :" + Err.ToString();
            //Display.Text = rxData;
            data += rxData;
            data += Environment.NewLine;
            if (Err > 0)
            {
                while (usb2can.CanalDataAvailable(CanIF) > 0)
                {
                    usb2can.CanalReceive(CanIF, ref RxMsg);
                    //Display.AppendText(Environment.NewLine);
                    //Display.AppendText("ID: " + RxMsg.id.ToString());
                    data += RxMsg.id.ToString();

                    //Display.AppendText(" Size: " + RxMsg.sizeData.ToString());

                    string strData = " Data: ";
                    strData += string.Format(" {0:X2}", RxMsg.data0);
                    strData += string.Format(" {0:X2}", RxMsg.data1);
                    strData += string.Format(" {0:X2}", RxMsg.data2);
                    strData += string.Format(" {0:X2}", RxMsg.data3);
                    strData += string.Format(" {0:X2}", RxMsg.data4);
                    strData += string.Format(" {0:X2}", RxMsg.data5);
                    strData += string.Format(" {0:X2}", RxMsg.data6);
                    strData += string.Format(" {0:X2}", RxMsg.data7);
                    data += strData;
                    data += Environment.NewLine;
                    //Display.AppendText(strData);
                    //Display.AppendText(" RxTime: " + RxMsg.timestamp.ToString());
                }
            }
        }
        private void TaRx_Click(object sender, RoutedEventArgs e)
        {
            Display.AppendText("Fetching data...");
            data = "";
            timer.Start();
            TaRx.IsEnabled = false;
        }

        private void Can2Tool_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CanIF > 0) usb2can.CanalClose(CanIF);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string fileName = null;
            if(textBoxFilename.Text != null && textBoxFilename.Text != "")
            {
                fileName = textBoxFilename.Text;
            }
            else
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            File.WriteAllText(fileName + ".txt", data);
            MessageBox.Show("Saved Successfully!");
        }

        private void StopReceive_Click(object sender, RoutedEventArgs e)
        {
            Display.AppendText("Stopped.");
            Display.AppendText(Environment.NewLine);
            timer.Stop();
            TaRx.IsEnabled = true;
            Display.AppendText(data);
            Display.AppendText(Environment.NewLine);
        }
    }
}
