using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 串口助手终极版
{
    public partial class Form1 : Form
    {
        bool judgeSerialPortState = false;
        Int32 timerNum;
        bool serialPortStateFlag;
        int count = 0;
        public Form1()
        {
            InitializeComponent();
            string flag;
            for (int i = 0; i < 20; i++)
            {
                flag = "COM" + i.ToString();
                try
                {
                    serialPort1.PortName = flag;
                    serialPort1.Open();
                    judgeSerialPortState = true;
                    comboBox1.Items.Add(flag);
                    serialPort1.Close();
                }
                catch (Exception){}
            }
            if (!judgeSerialPortState)
                MessageBox.Show("请打开串口","错误");
     
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //串口中断
        {
            string str;
            if (!radioButton4.Checked)
            {
                str = serialPort1.ReadExisting();
                textBox1.AppendText(str);
            }
            else
            {
                byte dat;
                dat = (byte)serialPort1.ReadByte();
                str = Convert.ToString(dat,16).ToUpper();
                textBox1.AppendText((str.Length==1)?("0X0"+str):("0X"+str));
            }
        }
        private void button1_Click(object sender, EventArgs e)//扫描
        {
            string flag;
            for (int i = 0; i < 20; i++)
            {
                flag = "COM" + i.ToString();
                try
                {
                    serialPort1.PortName = flag;
                    serialPort1.Open();
                    comboBox1.Items.Add(flag);
                    serialPort1.Close();
                }
                catch (Exception) { }
            }   
        }

        private void button2_Click(object sender, EventArgs e)//串口开关
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.Open();
                    button2.BackgroundImage = Properties.Resources.关;
                    serialPortStateFlag = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("串口打开错误", "错误");
                }
            }
            else
            {
                try
                {
                    serialPort1.Close();
                    serialPortStateFlag = false;
                    button2.BackgroundImage = Properties.Resources.开;
                }
                catch (Exception){}
            }
        }

        private void button3_Click(object sender, EventArgs e)//发送数据
        {
            if (!checkBox1.Checked)
                SendDataToSerialPort(serialPort1, textBox2);
            else
            {
                try
                {
                    timerNum = Convert.ToInt32(textBox3.Text);
                    button3.Enabled = false;
                    progressBar1.Maximum = timerNum;
                    timer1.Interval = 1000;
                    timer1.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("请输入纯数字","错误");
                }
               
            }
        }

        private void SendDataToSerialPort(SerialPort myPort, TextBox myTextBox)
        {
            string str;
            if (myPort.IsOpen)
            {
                if (myTextBox.Text != "")
                {
                    if (!radioButton2.Checked)
                    {
                        #region 写入字符
                        try
                        {
                            str = myTextBox.Text;
                            myPort.WriteLine(str);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("数据写入错误", "错误");
                        }
                        #endregion
                    }
                    else
                    {
                        #region  写入数值
                        byte[] buffer = new byte[1];
                        str = myTextBox.Text;
                        for (int i = 0; i < (str.Length - str.Length % 2) / 2; i++)
                        {
                            try
                            {
                                buffer[0] = Convert.ToByte(str.Substring(i * 2, 2));
                                myPort.Write(buffer, 0, 1);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("数据写入错误", "错误");
                            }
                        }
                        #endregion
                    }
                }
                else
                    MessageBox.Show("发送数据不能为空", "错误");
            }
            else
            {
                MessageBox.Show("串口还没有打开","错误");
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            count++;
            progressBar1.Value = count;
            if (count == timerNum)
            {
                count = 0;
                System.Media.SystemSounds.Asterisk.Play();
                try
                {
                    timer1.Stop();
                    SendDataToSerialPort(serialPort1,textBox2);
                    button3.Enabled = true;
                    if (serialPortStateFlag)
                        MessageBox.Show("已发送","提示");
                }
                catch (Exception) { MessageBox.Show("定时器错误","错误"); }    
            }
            
        }

     
    }
}
