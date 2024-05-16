using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;//头文件
using System.Net.Sockets;
using System.Threading;
using System.Net;

using System.Timers;

namespace UART_Remote_end
{
    delegate void AddUserInfoDel(string userInfo, bool isRemove);
    delegate void AddReceiveInfoDel(string str);
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            myDel += AddUserInfoFunction;
            recvDel += RecMsg;
            this.txtIP.Text = "127.0.0.1";
            this.txtPort.Text = "123";

            serialPort1 = new System.IO.Ports.SerialPort();

            CheckForIllegalCrossThreadCalls = false;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            serialPort1.Encoding = Encoding.GetEncoding("GB2312");
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void RecMsg(string str)
        {
            this.txtReceive.AppendText(str + "\r\n");
        }

        private void AddUserInfoFunction(string strInfo, bool isRemove)
        {
            if (isRemove == true)
            {
                this.IbxUserInfo.Items.Remove(strInfo);
            }
            else
            {
                this.IbxUserInfo.Items.Add(strInfo);
            }
        }

        AddUserInfoDel myDel;
        AddReceiveInfoDel recvDel;
        Socket _socket;
        Thread th;
        Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)//接收函数  SerialDataReceivedEventArgs写错，导致错误，谨记
        {
            try
            {
                string recive_data;
                recive_data = serialPort1.ReadExisting();
                textBox13.AppendText(recive_data);
                
                // 去除前导和尾随的空白字符
                recive_data = recive_data.Trim();
                // 切分
                string[] parts = recive_data.Split(',');
                string var1 = parts[0]; // 第一个%c
                string var2 = parts[1]; // 第一个%f
                string var3 = parts[2]; // 第二个%f
                string var4 = parts[3]; // 第三个%f
                string var5 = parts[4]; // 第四个%f
                string var6 = parts[5]; // 第一个%d
                string var7 = parts[6]; // 第二个%d
                string var8 = parts[7]; // 第三个%d
                string var9 = parts[8]; // 第二个%c

                textBox1.Clear();
                textBox1.AppendText(var2); // DHT11
                textBox2.Clear();
                textBox2.AppendText(var3); // DHT11
                textBox3.Clear();
                textBox3.AppendText(var4); // AHT20
                textBox4.Clear();
                textBox4.AppendText(var5); // AHT20
                textBox7.Clear();
                textBox7.AppendText(var6); // BMP280
                textBox8.Clear();
                textBox8.AppendText(var7); // 震动
                textBox9.Clear();
                textBox9.AppendText(var8); // 火焰


                txtSend.AppendText(recive_data);
                string textSend = this.txtSend.Text;
                byte[] arrTxt = Encoding.UTF8.GetBytes(textSend);

                foreach (string item in dicSocket.Keys)
                {
                    dicSocket[item].Send(arrTxt);
                    string str = item + textSend;
                    Invoke(recvDel, str);
                }

                txtSend.Clear();
                /*
                // 确保切分后的数组有足够的元素
                if (parts.Length == 7)
                {
                    string var1 = parts[0]; // 第一个%f
                    string var2 = parts[1]; // 第二个%f
                    string var3 = parts[2]; // 第三个%f
                    string var4 = parts[3]; // 第四个%f
                    string var5 = parts[4]; // 第一个%d
                    string var6 = parts[5]; // 第二个%d
                    string var7 = parts[6]; // 第三个%d

                    textBox1.AppendText(var1); // DHT11
                    textBox2.AppendText(var2); // DHT11
                    textBox3.AppendText(var3); // AHT20
                    textBox4.AppendText(var4); // AHT20
                    textBox7.AppendText(var5); // BMP280
                    textBox8.AppendText(var6); // 震动
                    textBox9.AppendText(var7); // 火焰
                }
                else
                {
                    // 处理数组长度不符合预期的情况
                    Console.WriteLine("数据格式不正确，期望7个值，实际得到: " + parts.Length);
                }
                */
            }
            catch { }
        }


        /// //////////////////////////////////////////////////////////////////////搜索串口部分
        private void button1_Click_1(object sender, EventArgs e)
        {
            SearchAnAddSerialToComboBox(serialPort1, comboBox1);
        }

        private void SearchAnAddSerialToComboBox(SerialPort MyPort, ComboBox MyBox)//搜索串口函数
        { //将可用的串口号添加到ComboBox
            string[] NmberOfport = new string[20];//最多容纳20个，太多会卡，影响效率
            string MidString1;//中间数组，用于缓存
            MyBox.Items.Clear();//清空combobox的内容
            for (int i = 1; i < 20; i++)
            {
                try //核心是靠try和catch 完成遍历
                {
                    MidString1 = "COM" + i.ToString();  //把串口名字赋给MidString1
                    MyPort.PortName = MidString1;       //把MidString1赋给 MyPort.PortName 
                    MyPort.Open();                      //如果失败，后面代码不执行？？
                    NmberOfport[i - 1] = MidString1;    //依次把MidString1的字符赋给NmberOfport
                    MyBox.Items.Add(MidString1);        //打开成功，添加到下列列表
                    MyPort.Close();                     //关闭
                    MyBox.Text = NmberOfport[i - 1];    //显示最后扫描成功那个串口
                }
                catch { };
            }
        }

        /// ////////////////////////////////////////////////////////////////////////////////打开串口部分
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (button2.Text == "打开串口")//为0时，表示关闭，此时可以进行打开操作
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;//获取端口号
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);//设置波特率
                    serialPort1.Open();//打开串口
                    button2.Text = " 关闭串口";
                }
                catch
                {
                    MessageBox.Show("串口打开错误");
                }
            }
            else  //为1时，表示开启，此时可以进行关闭操作
            {
                try
                {
                    serialPort1.Close();//关闭串口
                    button2.Text = "打开串口";//置位为0，表示状态为关闭
                }
                catch { }
            }
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse(this.txtIP.Text.Trim());
            IPEndPoint endPoint = new IPEndPoint(address, Convert.ToInt32(this.txtPort.Text.Trim()));

            try
            {
                _socket.Bind(endPoint);
                MessageBox.Show("成功");
            }
            catch
            {
                MessageBox.Show("失败");
            }

            _socket.Listen(10);

            th = new Thread(ListenFunction);
            th.Start();
            btnStartService.Enabled = false;
        }

        private void ListenFunction()
        {
            while (true)
            {
                Socket socketClient = _socket.Accept();

                string info = socketClient.RemoteEndPoint.ToString();

                dicSocket.Add(info, socketClient);
                Invoke(myDel, info, false);

                Thread th = new Thread(ReceiveInfo);
                th.IsBackground = true;
                th.Start(socketClient);
            }

        }

            private void ReceiveInfo(object obj)
            {
                Socket sckClient = obj as Socket;
                if (sckClient != null)
                {
                    while (true)
                    {
                        byte[] arrReceive = new byte[1024];
                        int length = -1;
                        length = sckClient.Receive(arrReceive);

                        if (length == 0)
                        {
                            string str = sckClient.RemoteEndPoint.ToString();
                            Invoke(myDel, str, true);
                            break;
                        }
                        else
                        {
                            string str = Encoding.UTF8.GetString(arrReceive, 0, length);
                            serialPort1.Write(str);//接收控制指令
                                                        
                            int STR = int.Parse(str);
                            if (STR == 1)
                            {
                            textBox11.Clear();
                            textBox11.AppendText("开");
                            }
                            else if(STR == 2)
                            {
                            textBox11.Clear();
                            textBox11.AppendText("关");
                            }
                            else if (STR == 3)
                            {
                            textBox10.Clear();
                            textBox10.AppendText("开");
                            }
                            else if (STR == 4)
                            {
                            textBox10.Clear();
                            textBox10.AppendText("关");
                            }
                            else if (STR == 6)
                            {
                            textBox12.Clear();
                            textBox12.AppendText("开");
                            }
                            else if (STR == 5)
                            {
                            textBox12.Clear();
                            textBox12.AppendText("关");
                            }

                        string msg = "[接收]" + sckClient.RemoteEndPoint.ToString() + " " + str;
                            Invoke(recvDel, msg);
                        }
                    }
                }
            }
        
       
        private void btnSendToAll_Click(object sender, EventArgs e)
        {
            //string textSend = serialPort1.ReadExisting();
            string textSend = this.txtSend.Text;
            byte[] arrTxt = Encoding.UTF8.GetBytes(textSend);
            
            foreach (string item in dicSocket.Keys)
            {
            dicSocket[item].Send(arrTxt);
            string str = item + textSend;
            Invoke(recvDel, str);
            }
     
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
