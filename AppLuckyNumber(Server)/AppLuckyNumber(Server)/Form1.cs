using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;
using Label = System.Windows.Forms.Label;

namespace AppLuckyNumber_Server_
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();
            InitializeComponent1();
            int timeCycle = 60;
            WaitAndPost(timeCycle);
        }
        private void InitializeComponent1()
        {

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // Cập nhật mỗi giây
            timer.Tick += timer1_Tick;
            timer.Start();
        }
        private async Task WaitAndPost(int timeCycle)
        {
            try
            {
                xNet.HttpRequest http1 = new xNet.HttpRequest();
                xNet.HttpRequest http2 = new xNet.HttpRequest();
                Thread t = new Thread(async () =>
                {
                    while (true)
                    {
                        Random random = new Random();
                        int luckyNumber = random.Next(0, 9);
                        int minute1 = DateTime.Now.Minute;
                        //if (minute1 == 0 || minute1 == 1 || minute1 == 2 )//Đôi khi thao tác chậm, nên cho khoảng thời gian này 
                        //{
                            List<string> list = new List<string>(); 
                            int a = int.Parse( http1.Post("http://tahoang111-001-site1.btempurl.com/api/Check/PostLuckyNumber?luckyNumber=" + luckyNumber).ToString());
                            if (a != -1)
                            {
                                //Đọc tất cả file của người chơi để thưởng coin
                                string allFilePlayer = http2.Get("http://tahoang111-001-site1.btempurl.com/api/Check/AllFilePlayer?code=amkjdfiuhfd").ToString();
                                 if (allFilePlayer.Contains(".txt"))
                                {
                                    var arr = allFilePlayer.Split(',');
                                    foreach(string item in arr)
                                    {
                                        var arrItem = item.Split('\\');
                                        string item1 = arrItem[arrItem.Length-1];
                                        var arrItemChild= item1.Split('_');
                                        if(arrItemChild[1].Contains(luckyNumber.ToString()))
                                        {
                                            list.Add(arrItemChild[0]);
                                        }
                                    }   
                                }
                                foreach(string item in list) //danh sách IDVi
                                { 
                                string resultGetCoin = http2.Post("http://tahoang111-001-site1.btempurl.com/api/Walletes/GetCoin?id="+ item).ToString();
                                }
                                //Xóa file txt của người chơi đã cược 
                                string deleteFilePlayer = http2.Post("http://tahoang111-001-site1.btempurl.com/api/Check/DeleteFile?code=amkjdfiuhfd").ToString();
                            }
                        //}
                        int minute2 = DateTime.Now.Minute;
                        //Thread.Sleep((2) * 60 * 1000);
                        Thread.Sleep((timeCycle- minute2) * 60 * 1000);
                        //Thread.Sleep(timeCycle * 60 * 1000);
                    }
                });
                t.Start();
            }
            catch
            {

            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                lblTime.Text = DateTime.Now.ToString("HH:mm:ss, dd-MM-yyyy"); // Định dạng ngày giờ đầy đủ
            }
            catch
            {

            }
        }
        //load số may mắn lên giao diện
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(UpdateLblTotalPlayer);
            thread1.Start();
            Thread thread2 = new Thread(UpdateLblLKNumber);
            thread2.Start();
        }
        private void UpdateLblLKNumber()
        {
            try
            {
                xNet.HttpRequest http = new xNet.HttpRequest();
                while (true)
                {
                    int minute = DateTime.Now.Minute;
                    int result = int.Parse(http.Get("http://tahoang111-001-site1.btempurl.com/api/Check/LuckyNumber").ToString());
                    UpdateLblLKNumberValue(result.ToString());
                    //Thread.Sleep((60 - minute) * 60 * 1000);
                    Thread.Sleep(10000);//10 giây gửi yêu cầu 1 lần
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateLblTotalPlayer()
        {
            try
            {
                xNet.HttpRequest http = new xNet.HttpRequest();
                while (true)
                {
                    int result = int.Parse(http.Get("http://tahoang111-001-site1.btempurl.com/api/Check/CountPlayerCurrent").ToString());
                    UpdateLblTotalPlayerValue(result.ToString());
                    Thread.Sleep(10000);//10 giây gửi yêu cầu 1 lần
                }
            }
            catch (Exception ex)
            {

            }
        }
        //gắn tổng người cược vào lable
        private void UpdateLblTotalPlayerValue(string value)
        {
            if (lblTotalPlayer.InvokeRequired)
            {
                // Truy cập và cập nhật Label thông qua UI thread
                lblTotalPlayer.Invoke((MethodInvoker)(() => lblTotalPlayer.Text = value));
            }
            else
            {
                lblTotalPlayer.Text = value;
            }
        }     
        //gắn số may mắn vào lable
        private void UpdateLblLKNumberValue(string value)
        {
            if (lblLuckyNumber.InvokeRequired)
            {
                // Truy cập và cập nhật Label thông qua UI thread
                lblLuckyNumber.Invoke((MethodInvoker)(() => lblLuckyNumber.Text = value));
            }
            else
            {
                lblLuckyNumber.Text = value;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //try
            //{
            //    // Kiểm tra xem người dùng có đang tắt chương trình không
            //    if (e.CloseReason == CloseReason.UserClosing)
            //    {
            //        // Lấy danh sách tất cả các quá trình đang chạy
            //        Process[] processes = Process.GetProcesses();

            //        // Lặp qua tất cả các quá trình
            //        foreach (Process process in processes)
            //        {
            //            // Đóng tất cả các luồng của quá trình
            //            process.Kill();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }
    }
}
