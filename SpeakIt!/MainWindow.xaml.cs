using NAudio.Wave;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace SpeakIt_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] arrGiong = { "Nữ miền Bắc", "Nam miền Bắc", "Nam miền Nam", "Nữ miền Nam" };
        int[] arrGiongmini = { 2,4, 3, 1 };
        string[] arrVanBan;
        int filexong = -1;
        int soluongcau=0;
        Thread DowThread;
        Thread DocThread;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _run_Click(object sender, RoutedEventArgs e)
        {
            string text = _text.Text;
            int gender = arrGiongmini[Array.IndexOf(arrGiong, _nguoidoc.Text)];
            string speed = Between(_tocdo.Text,"(",")");
            DowThread = new Thread(() => Taifilethread(text, gender, speed));
            DowThread.IsBackground = true;
            DowThread.Start();
        }


        public string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }
        private void threadplay()
        {
            int dem = 0;

            while (true)
            {
                if (filexong >=dem)
                {
                    PlayMp3FromUrl("audio/"+dem.ToString()+".wav");
                    dem += 1;
                }
                Thread.Sleep(1000);
            }
        }
        private void clear()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo("audio");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            Thread.Sleep(1000);
        }
        private void Taifilethread(string text, int gender, string speed)
        {
            if (text.Length > 2000)
            {
                filexong = -1;
                DocThread = new Thread(() => threadplay());
                DocThread.IsBackground = true;
                DocThread.Start();
                string[] tokens = text.Split(new[] { "." }, StringSplitOptions.None);
                arrVanBan = tokens;
                soluongcau = tokens.Length;
                int dem = 0;
                foreach (string item in tokens)
                {

                    IRestResponse response;
                    while (true)
                    {
                        try
                        {
                            var client = new RestClient("https://api.zalo.ai/v1/tts/synthesize?apikey=9YX4lUKZXTV3a9EJ0suhFDzVNaaN6ODq");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            request.AddParameter("input", item);
                            request.AddParameter("speaker_id", gender.ToString());
                            request.AddParameter("speed", speed);
                            response = client.Execute(request);
                            Console.WriteLine(response.Content);
                            break;
                        }
                        catch { }
                    }
                    JObject stuff = JObject.Parse(response.Content);
                    DownFileByUrl(stuff["data"]["url"].ToString(), "audio/" + dem.ToString() + ".wav");
                    Thread.Sleep(1000);
                    filexong = dem;
                    dem += 1;
                }
            }
            else
            {

                filexong = -1;
                DocThread = new Thread(() => threadplay());
                DocThread.IsBackground = true;
                DocThread.Start();

                int dem = 0;
                    IRestResponse response;
                    while (true)
                    {
                        try
                        {
                            var client = new RestClient("https://api.zalo.ai/v1/tts/synthesize?apikey=9YX4lUKZXTV3a9EJ0suhFDzVNaaN6ODq");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            request.AddParameter("input", text);
                            request.AddParameter("speaker_id", gender.ToString());
                            request.AddParameter("speed", speed);
                            response = client.Execute(request);
                            Console.WriteLine(response.Content);
                            break;
                        }
                        catch { }
                    }
                    JObject stuff = JObject.Parse(response.Content);
                    DownFileByUrl(stuff["data"]["url"].ToString(), "audio/" + dem.ToString() + ".wav");
                    Thread.Sleep(1000);
                    filexong = dem;
                    dem += 1;
            
        }




        }
        //private void Taifilethread(string text, string gender = "lannhi", string speed = "-1")
        //{
        //        if (text.Length > 497)
        //        {
        //        filexong = -1;
        //        DocThread = new Thread(() => threadplay());
        //        DocThread.IsBackground = true;
        //        DocThread.Start();
        //        string[] tokens = text.Split(new[] { "." }, StringSplitOptions.None);
        //            arrVanBan = tokens;
        //               soluongcau = tokens.Length;
        //            int dem = 0;
        //            foreach (string item in tokens)
        //            {

        //                byte[] response;
        //                while (true)
        //                {
        //                    try
        //                    {
        //                    var wb = new WebClient();
        //                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //                        var data = new NameValueCollection();
        //                        data["text"] = item;
        //                        data["voice"] = "commercial";
        //                        data["gender"] = gender;
        //                        data["speed"] = speed;
        //                        response = wb.UploadValues("https://speech.openfpt.vn/speech", "POST", data);
        //                        break;
        //                    }
        //                    catch { }
        //                }
        //                    string responseInString = Encoding.UTF8.GetString(response);
        //                    JObject stuff = JObject.Parse(responseInString);
        //                    DownFileByUrl(stuff["Url"].ToString(), "audio/" + dem.ToString() + ".mp3");
        //            Thread.Sleep(1000);
        //            filexong = dem;
        //            dem += 1;
        //            }

        //        }
        //        else
        //        {
        //            using (var wb = new WebClient())
        //            {
        //                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //                var data = new NameValueCollection();
        //                data["text"] = text;
        //                data["voice"] = "commercial";
        //                data["gender"] = gender;
        //                data["speed"] = speed;
        //                var response = wb.UploadValues("https://speech.openfpt.vn/speech", "POST", data);
        //                string responseInString = Encoding.UTF8.GetString(response);
        //                JObject stuff = JObject.Parse(responseInString);
        //                DownFileByUrl(stuff["Url"].ToString(), "audio/1.mp3");
        //            Thread.Sleep(1000);
        //                PlayMp3FromUrl("audio/1.mp3");
        //            }
        //        }


        //}
        private void DownFileByUrl(string url, string filename)
        {

                while (true)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(url, filename);
                            break;
                        }
                    }
                    catch { }
                }
            
        }

        private void PlayMp3FromUrl(string url)
        {

                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new WaveFileReader(url))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
        }

        private void _stop_Click(object sender, RoutedEventArgs e)
        {
            if (DocThread != null)
            {
                if (DocThread.IsAlive)
                {
                    DocThread.Abort();
                }
            }
            if (DowThread != null)
            {
                if (DowThread.IsAlive)
                {
                    DowThread.Abort();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void _minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void _info_Click(object sender, RoutedEventArgs e)
        {

            System.Diagnostics.Process.Start("https://raw.githubusercontent.com/phatjkk/DonateMe/master/Information");
        }

        private void Grid_MouseLeftButtonDown(object sender, TouchEventArgs e)
        {
            var move = sender as System.Windows.Controls.Grid;
            var win = Window.GetWindow(move);
            win.DragMove();
        }
    }
}
