using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;
using System.Windows.Forms;

using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace Server_end
{
    public partial class Form1 : Form
    {
        private List<float> data1 = new List<float>();  // 数据源
        private List<float> data2 = new List<float>();  // 数据源

        // 创建两个Series，一个用于温度数据，一个用于湿度数据
        //private Series seriesTemperature = new Series { Name = "温度", Color = Color.Green };
        //private Series seriesHumidity = new Series { Name = "湿度", Color = Color.Red };
        private System.Timers.Timer timer1 = new System.Timers.Timer();

        public Form1()
        {
            InitializeComponent();
            this.txtIP.Text = "127.0.0.1";
            this.txtPort.Text = "123";
            this.txtName.Text = "ZZY";

            // 初始化Chart控件
            chart1.Titles.Add("实时温湿度折线");
            chart1.ChartAreas[0].AxisX.Title = "时间";
            chart1.ChartAreas[0].AxisY.Title = "温度/湿度";
            // 创建第一个系列，用于显示温度数据
            chart1.Series.Add("温度");
            chart1.Series["温度"].ChartType = SeriesChartType.Line;
            // 创建第二个系列，用于显示湿度数据
            chart1.Series.Add("湿度");
            chart1.Series["湿度"].ChartType = SeriesChartType.Line;

            // 设置图表
            //chart1.Series.Clear();
            //chart1.Series.Add(seriesTemperature);
            //chart1.Series.Add(seriesHumidity);

            // 设置图表的X轴为时间
            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            //chart1.ChartAreas[0].AxisX.Interval = 1;

            // 设置定时器，每秒触发一次
            timer1.Interval = 1000;
            timer1.Elapsed += Timer_Elapsed;
            timer1.Start();

            // 明确指定使用System.Windows.Forms.Timer
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 每秒更新一次
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        string XINGXI;

        Socket sockClient = null;
        Thread thrClient = null;
        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStartConnect_Click(object sender, EventArgs e)
        {
            IPAddress address = IPAddress.Parse(this.txtIP.Text.Trim());
            IPEndPoint ipe = new IPEndPoint(address, int.Parse(this.txtPort.Text.Trim()));

            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //this.txtReceive.AppendText("正在与服务器连接中......" + "\r\n");
                sockClient.Connect(ipe);
            }
            catch (Exception ee)
            {
                MessageBox.Show("连接失败" + ee.ToString());
            }

            //this.txtReceive.AppendText("连接成功");
            MessageBox.Show("连接成功");
            thrClient = new Thread(ReceiveMsg);
            thrClient.IsBackground = true;
            thrClient.Start();
        }

        private void ReceiveMsg()
        {
            while (true)
            {
                byte[] arrMsgRec = new byte[1024];
                int length = sockClient.Receive(arrMsgRec);

                if (length == 0)//代表客户端已经断开了
                {
                    //Invoke(new Action(() => this.txtReceive.AppendText("断开连接")));
                    break;
                }
                else
                {
                    string strMsg = Encoding.UTF8.GetString(arrMsgRec, 0, length);
                    //string msg = "[接收]" + strMsg + Environment.NewLine;
                    //Invoke(new Action(() => this.txtReceive.AppendText(strMsg)));
                    XINGXI = strMsg;
                }
                // 切分
                string[] parts = XINGXI.Split(',');
                if (parts.Length >= 9) // 确保数组有足够的元素
                {
                    string var1 = parts[0]; // 第一个%c
                    string var2 = parts[1]; // 第一个%f
                    string var3 = parts[2]; // 第二个%f
                    string var4 = parts[3]; // 第三个%f
                    string var5 = parts[4]; // 第四个%f
                    string var6 = parts[5]; // 第一个%d
                    string var7 = parts[6]; // 第二个%d
                    string var8 = parts[7]; // 第三个%d
                    string var9 = parts[8]; // 第二个%c

                    // 使用Invoke来更新UI控件
                    Invoke(new Action(() =>
                    {
                        textBox1.Clear();
                        textBox1.AppendText(var2 + Environment.NewLine);

                        textBox2.Clear();
                        textBox2.AppendText(var3 + Environment.NewLine);

                        textBox3.Clear();
                        textBox3.AppendText(var4 + Environment.NewLine);

                        textBox4.Clear();
                        textBox4.AppendText(var5 + Environment.NewLine);

                        textBox7.Clear();
                        textBox7.AppendText(var6 + Environment.NewLine);

                        textBox10.Clear();
                        textBox10.AppendText(XINGXI + Environment.NewLine);

                        int var7Int = int.Parse(var7);
                        if (var7Int == 1)
                        {
                            textBox8.BackColor = Color.Red;
                        }
                        else
                        {
                            textBox8.BackColor = Color.Lime;
                        }

                        int var8Int = int.Parse(var8);
                        if (var8Int == 1)
                        {
                            textBox9.BackColor = Color.Red;
                        }
                        else
                        {
                            textBox9.BackColor = Color.Lime;
                        }
                        
                        textBox8.Clear();
                        textBox8.AppendText(var7 + Environment.NewLine);

                        textBox9.Clear();
                        textBox9.AppendText(var8 + Environment.NewLine);
                                                                   
                        
                    }));

                }
                else
                {
                    // 处理数组长度不符合预期的情况
                    Console.WriteLine("数据格式不正确，期望9个值，实际得到: " + parts.Length);
                }


            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string strMsg = "1";
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            sockClient.Send(arrMsg);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strMsg = "2";
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            sockClient.Send(arrMsg);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string strMsg = "3";
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            sockClient.Send(arrMsg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strMsg = "4";
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            sockClient.Send(arrMsg);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string strMsg = "6";
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            sockClient.Send(arrMsg);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string strMsg = "5";
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            sockClient.Send(arrMsg);
        }

        string xingxi;

        private void Timer_Tick(object sender, EventArgs e)
        {
            xingxi = this.textBox10.Text;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string textToSave = $"时间戳：{timestamp}\n数据：{xingxi}\n";

            SaveToFile(textToSave);

            //在UI线程上执行方法
            Invoke(new MethodInvoker(delegate
            {
                CheckTemperatureAndHumidity();
            }));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string textToSave = $"时间戳：{timestamp}\n数据：{xingxi}\n";

            SaveToFile(textToSave);
            
            MessageBox.Show("数据已保存到文件。");
        }

        private void SaveToFile(string text)
        {
            string filePath = "E:\\..B办公文件\\（新）读研期间\\b.课程\\C计算机网络（4学分）\\实践模拟题目（5.8）\\数据日志.txt"; // 指定保存文件的路径

            // 使用using语句确保文件流被正确关闭
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(text);
            }
            
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void CheckTemperatureAndHumidity()
        {
            float temperature1, temperature2, humidity1, humidity2, temperatureLimit, humidityLimit;

            // 从textBox1和textBox3中读取温度值
            if (float.TryParse(textBox1.Text, out temperature1) && float.TryParse(textBox3.Text, out temperature2))
            {
                // 从comboBox3中读取温度上限
                if (float.TryParse(comboBox3.SelectedItem.ToString(), out temperatureLimit))
                {
                    // 检查温度是否超过上限
                    if (temperature1 > temperatureLimit || temperature2 > temperatureLimit)
                    {
                        textBox5.BackColor = Color.Red;
                    }
                    else
                    {
                        textBox5.BackColor = Color.Lime;
                    }
                }
                else
                {
                    // 处理温度上限转换失败的情况
                }
            }
            else
            {
                // 处理温度值转换失败的情况
            }

            // 从textBox2和textBox4中读取湿度值
            if (float.TryParse(textBox2.Text, out humidity1) && float.TryParse(textBox4.Text, out humidity2))
            {
                // 从comboBox4中读取湿度上限
                if (float.TryParse(comboBox4.SelectedItem.ToString(), out humidityLimit))
                {
                    // 检查湿度是否超过上限
                    if (humidity1 > humidityLimit || humidity2 > humidityLimit)
                    {
                        textBox6.BackColor = Color.Red;
                    }
                    else
                    {
                        textBox6.BackColor = Color.Lime;
                    }
                }
                else
                {
                    // 处理湿度上限转换失败的情况
                }
            }
            else
            {
                // 处理湿度值转换失败的情况
            }
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 读取textBox3和textBox4中的数据
            string temperatureText = textBox3.Text;
            string humidityText = textBox4.Text;

            float value1;
            float value2;

            // 使用float.Parse方法尝试转换文本为float
            if (float.TryParse(textBox3.Text, out value1))
            {
                // 转换成功，可以继续使用value变量
                // 例如：
                // Console.WriteLine(value);
                // 或在其他地方使用value变量
            }
            else
            {
                // 转换失败，value变量保持为默认值0
                // 可以显示错误消息或进行其他处理
                //MessageBox.Show("文本格式不正确，无法转换为浮点数。");
            }
            if (float.TryParse(textBox4.Text, out value2))
            {
                // 转换成功，可以继续使用value变量
                // 例如：
                // Console.WriteLine(value);
                // 或在其他地方使用value变量
            }
            else
            {
                // 转换失败，value变量保持为默认值0
                // 可以显示错误消息或进行其他处理
                //MessageBox.Show("文本格式不正确，无法转换为浮点数。");
            }

            // 添加到数据源中
            data1.Add(value1);
            data2.Add(value2);

            // 如果数据源超过了一定长度，就删除最前面的数据
            if (data1.Count > 20)
            {
                data1.RemoveAt(0);
            }
            if (data2.Count > 20)
            {
                data2.RemoveAt(0);
            }

            // 绑定数据并刷新Chart控件
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    chart1.Series["温度"].Points.DataBindY(data1);
                    chart1.Series["湿度"].Points.DataBindY(data2);
                    chart1.Refresh();
                }));
            }
            else
            {
                chart1.Series["温度"].Points.DataBindY(data1);
                chart1.Series["湿度"].Points.DataBindY(data2);
                chart1.Refresh();
            }

        }
        
    }
}
