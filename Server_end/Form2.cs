using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_end
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            // 这里的用户名和密码是硬编码的，实际应用中应从安全的地方获取
            string correctUsername = "admin";
            string correctPassword = "123456789";

            if (username == correctUsername && password == correctPassword)
            {
                // 密码正确，跳转到Form1
                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();
            }
            else
            {
                // 密码错误，显示错误信息
                MessageBox.Show("用户名或密码错误");
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
