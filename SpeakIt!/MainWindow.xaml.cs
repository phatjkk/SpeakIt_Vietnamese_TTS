using NAudio.Wave;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
        string path = Directory.GetCurrentDirectory();
        string[] arrGiong = { "Nữ miền Nam","Nữ miền Bắc", "Nam miền Nam", "Nam miền Bắc" };
        int[] arrGiongmini = {1, 2, 3,4 };
        int filexong = -1;
        string version = "3.0.1";
        Thread ThreadBackround;
        Thread DocThread, ThreadUpdateUI;
        Process ffmpeg;
        XuLyAmThanh MainXuLy;
        string keylone = "";
        public MainWindow()
        {
            InitializeComponent();
            ThreadUpdateUI = new Thread(() => UpdateUI());
            ThreadUpdateUI.IsBackground = true;
            ThreadUpdateUI.Start();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as System.Windows.Controls.Grid;
            var win = Window.GetWindow(move);
            win.DragMove();
        }

        private void UpdateUI()
        {
            string textPre = "";
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {

                    if (_text.Text != textPre)
                    {
                        _kytu.Content = "Ký tự đã nhập: " + _text.Text.Length.ToString();
                        textPre = _text.Text;
                        
                    }
                });
                Thread.Sleep(200);
            }
        }
        private bool CheckKey(string key)
        {
            if (keylone == key)
            {
                return true;
            }
            else
            {
                _tientring.Content = "Đang kiểm tra API KEY";
                Thread.Sleep(500);
                var client = new RestClient("https://api.zalo.ai/v1/tts/synthesize");
                var request = new RestRequest(Method.POST);
                request.AddHeader("apikey", key);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                var response = client.Execute(request);
                if ((int)response.StatusCode == 401)
                {
                    _tientring.Content = "Chưa khởi động.";
                    return false;
                }
                _tientring.Content = "Đang xử lý";
                keylone = key;
                return true;
            }

            
        }
        private void _run_Click(object sender, RoutedEventArgs e)
        {
            if (CheckKey(_apikey.Text)==false)
            {
                MessageBox.Show("API KEY không đúng, bạn vui lòng kiểm tra lại key của mình rồi thử lại nhé!","Lỗi");
            }
            else
            {
                File.WriteAllText("APIKey.txt", _apikey.Text);
                string text = _text.Text;
                int gender = arrGiongmini[Array.IndexOf(arrGiong, _nguoidoc.Text)];
                string speed = StringBetween(_tocdo.Text, "(", ")");
                string apikey = _apikey.Text;
                MainXuLy = new XuLyAmThanh(text, gender, speed, apikey);
                MainXuLy.mainRun();
                ThreadBackround = new Thread(() => Backround());
                ThreadBackround.IsBackground = true;
                ThreadBackround.Start();
            }

        }
        private void _stop_Click(object sender, RoutedEventArgs e)
        {
            MainXuLy.StopRead();
            MainXuLy.StopDown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ThreadBackround!=null)
            {
                ThreadBackround.Abort();
            }
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

        public string StringBetween(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        private void _download_Click(object sender, RoutedEventArgs e)
        {
            if (CheckKey(_apikey.Text) == false)
            {
                MessageBox.Show("API KEY không đúng, bạn vui lòng kiểm tra lại key của mình rồi thử lại nhé!", "Lỗi");
            }
            else
            {
                File.WriteAllText("APIKey.txt", _apikey.Text);
                string text = _text.Text;
                int gender = arrGiongmini[Array.IndexOf(arrGiong, _nguoidoc.Text)];
                string speed = StringBetween(_tocdo.Text, "(", ")");
                string apikey = _apikey.Text;
                MainXuLy = new XuLyAmThanh(text, gender, speed, apikey);
                MainXuLy.mainDown();
                ThreadBackround = new Thread(() => Backround());
                ThreadBackround.IsBackground = true;
                ThreadBackround.Start();
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            var client = new RestClient("https://raw.githubusercontent.com/phatjkk/Tool/master/DragonSpeak.txt");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (!response.Content.Contains(version)) { 
                MessageBox.Show("Đã có phiên bản mới. Hãy cập nhật nhé!");
                System.Diagnostics.Process.Start("https://github.com/phatjkk/SpeakIt_Vietnamese_TTS/releases");
                System.Environment.Exit(1);
            }
            else
            {

                _apikey.Text = System.IO.File.ReadAllText("APIKey.txt");
            }
            
        }

        private void _back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
        }

        private void Backround()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() => {
                    _tientring.Content = MainXuLy.getProcessMes();
                    _process.Value = MainXuLy.getProcessNow();
               
                });
                
                Thread.Sleep(2000);
            }
        }
        //private void clear()
        //{
        //    System.IO.DirectoryInfo di = new DirectoryInfo("audio");

        //    foreach (FileInfo file in di.GetFiles())
        //    {
        //        file.Delete();
        //    }
        //    foreach (DirectoryInfo dir in di.GetDirectories())
        //    {
        //        dir.Delete(true);
        //    }
        //    Thread.Sleep(1000);
        //}




        //    private void DownFileByUrl(string url, string filename)
        //    {

        //            while (true)
        //            {
        //                try
        //                {
        //                    using (var client = new WebClient())
        //                    {
        //                        client.DownloadFile(url, filename);
        //                        break;
        //                    }
        //                }
        //                catch { }
        //            }

        //    }

        //    private void PlayMp3FromUrl(string url)
        //    {

        //            using (WaveStream blockAlignedStream =
        //                new BlockAlignReductionStream(
        //                    WaveFormatConversionStream.CreatePcmStream(
        //                        new WaveFileReader(url))))
        //            {
        //                using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
        //                {
        //                    waveOut.Init(blockAlignedStream);
        //                    waveOut.Play();
        //                    while (waveOut.PlaybackState == PlaybackState.Playing)
        //                    {
        //                        System.Threading.Thread.Sleep(100);
        //                    }
        //                }
        //            }
        //    }


        //    private static string Execute(string exePath, string parameters)
        //    {
        //        string result = String.Empty;

        //        using (Process p = new Process())
        //        {
        //            p.StartInfo.UseShellExecute = false;
        //            p.StartInfo.CreateNoWindow = true;
        //            p.StartInfo.RedirectStandardOutput = true;
        //            p.StartInfo.FileName = exePath;
        //            p.StartInfo.Arguments = parameters;
        //            p.Start();
        //            p.WaitForExit();

        //            result = p.StandardOutput.ReadToEnd();
        //        }

        //        return result;
        //    }


    }
}
public class XuLyAmThanh
{
    List<string> linksOfM3u8 = new List<string>();
    List<string> outputTexts = new List<string>();
    List<string> final_input_cutted = new List<string>();
    string[] outputTexts2;
    private int gender = 0;
    private string apikey="";
    private string speed = "1.0";
    private string text = "";
    private string processMes = "";
    private int processNow = 0;
    private int processFull = 0;
    private Process ffplay, ffmpeg;
    private int maxLenghtText = 500;
    private Thread ReadingThread, DownloadingThread;
    string path = Directory.GetCurrentDirectory();
    public XuLyAmThanh(string _text, int _gender = 1, string _speed = "",string _apikey="")
    {
        this.gender = _gender;
        this.text = _text;
        this.speed = _speed;
        this.apikey = _apikey;
    }
    public void mainRun()
    {
        ReadingThread = new Thread(() => Read());
        ReadingThread.Start();
    }
    public void mainDown()
    {

        DownloadingThread = new Thread(() => Down());
        DownloadingThread.Start();
    }
    public void Down()
    {
        this.processMes = "Đang khởi động...";
        DeleteAllFile(path + "\\audio");
        this.processNow = 0;
        string fname;
        if (text.Length > maxLenghtText)
        {

            linksOfM3u8.Clear();
            Thread getLink = new Thread(() => GetDataM3u8());
            getLink.Start();
            while (!(linksOfM3u8.Count > 0))
            {
                Thread.Sleep(2000);
            }
            int maxdown = outputTexts.Count;
            for (int i = 0; i < maxdown; i++)
            {
                fname = i.ToString();
                this.processNow = (i + 1) * 100/ (maxdown + 1);
                this.processMes = "Đang tải file -> " + fname + ".mp3...";
                Thread.Sleep(2000);
                DownFileM3U8toMP3(linksOfM3u8.ElementAt(i), fname + ".mp3");
                this.processMes = "Đã tải xong file -> " + fname + ".mp3";
                
            }
            this.processMes = "Done";
            this.processNow = 100;
            MessageBox.Show("Đã tải xong\nVui lòng check thư mục audio");
        }
        else
        {
            //fname = DateTime.Now.ToString("yyMMddHHmmss");
            fname = "output";
            this.processMes = "Đang tải file -> "+ fname + ".mp3...";
            DownFileM3U8toMP3(getTTS_URL(text), fname + ".mp3");
            this.processNow = 100;
            this.processMes = "Đã tải xong file -> "+ fname + ".mp3";
            MessageBox.Show("Đã tải xong\nVui lòng check thư mục audio");
        }
    }
    private void DeleteAllFile(string folderPath)
    {
        System.IO.DirectoryInfo di = new DirectoryInfo(folderPath);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }

    }
    public void Read()
    {
        this.processMes = "Đang khởi động...";
        if (text.Length > maxLenghtText)
        {
            linksOfM3u8.Clear();

            Thread getLink = new Thread(() => GetDataM3u8());
            getLink.Start();
            while (!(linksOfM3u8.Count > 0))
            {
                Thread.Sleep(1000);
            }
            for (int i = 0; i < outputTexts.Count; i++)
            {
                this.processMes = "Đang chạy trình phát...";
                PlayM3U8FromUrl(linksOfM3u8.ElementAt(i));
            }

        }
        else
        {
            this.processMes = "Đang chạy trình phát...";
            PlayM3U8FromUrl(getTTS_URL(text));
        }
        this.processMes = "Đã xong!";
    }
    public void GetDataM3u8()
    {
        int index = 0;
        while (text.Contains(".."))
        {
            text = text.Replace("..", ".");
        }
        outputTexts = text.Split(new[] { "." }, StringSplitOptions.None).OfType<string>().ToList();


        string doanDuoi2000rollback = "";
        string doanDuoi2000 = "";
        while (index < outputTexts.Count)
        {
            if ((doanDuoi2000.Length + outputTexts.ElementAt(index).Length) < 2000)
            {
                
                doanDuoi2000 += outputTexts.ElementAt(index) + ".";
                index += 1;
            }
            else if ((doanDuoi2000.Length + outputTexts.ElementAt(index).Length) > 2000)
            {
                final_input_cutted.Add(doanDuoi2000);
                doanDuoi2000 = "";
            }
        }
        if (doanDuoi2000.Length > 0)
        {
            final_input_cutted.Add(doanDuoi2000);
        }
        linksOfM3u8.Clear();
        outputTexts.Clear();
        outputTexts = final_input_cutted.ToList();

        foreach (string itemText in final_input_cutted)
        {
            linksOfM3u8.Add(getTTS_URL(itemText));
            Thread.Sleep(2000);
        }
    }
    public void StopDown()
    {
        try
        {
            if (ffmpeg != null)
            {
                ffmpeg.Kill();
            }

        }
        catch { }
        try
        {
            if (DownloadingThread != null)
            {
                DownloadingThread.Abort();
            }


        }
        catch { }
        this.processNow = 100;
        this.processMes = "Đã dừng tiến trình!";
        MessageBox.Show("Đã dừng tiến trình!");
    }
    public void StopRead()
    {
        try
        {
            if (ffplay != null)
            {
                ffplay.Kill();
            }
            
        }
        catch { }
        try
        {
            if (ReadingThread !=null)
            {
                ReadingThread.Abort();
            }
            
            
        }
        catch { }
    }
    public bool CheckReadDone()
    {
        foreach (Process clsProcess in Process.GetProcesses())
        {
            if (clsProcess.ProcessName.Contains("ffplay"))
            {
                return true;
            }
        }
        return false;
    }

    private void PlayM3U8FromUrl(string url)
    {
        string cml = @" -autoexit -nodisp """ + url + @"""";
        ffplay = new Process
        {
            StartInfo = {
             FileName = path+"\\ffplay.exe",
        Arguments = cml,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true,
        WorkingDirectory = path+"\\audio"
    }
        };

        ffplay.EnableRaisingEvents = true;
        ffplay.OutputDataReceived += (s, e) => Debug.WriteLine(e.Data);
        ffplay.ErrorDataReceived += (s, e) => Debug.WriteLine($@"Error: {e.Data}");
        ffplay.Start();
        ffplay.BeginOutputReadLine();
        ffplay.BeginErrorReadLine();
        ffplay.WaitForExit();
    }
    private string getTTS_URL(string _text)
    {
        File.WriteAllText(path + "\\zalo_tts\\output.txt", "");
        File.WriteAllText(path + "\\zalo_tts\\text.txt", _text);
        File.WriteAllText(path + "\\zalo_tts\\setting.txt", gender + "|" + speed+"|"+apikey);
        //var process = Process.Start(path + "\\zalo_tts\\zalo_tts.exe");
        string appPath = path + "\\zalo_tts\\zalo_tts.exe";
        Process ffmpeg = new Process
        {
            StartInfo = {
                    FileName = appPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = path+"\\zalo_tts"
                    }
        };

        ffmpeg.EnableRaisingEvents = true;
        ffmpeg.OutputDataReceived += (s, e) => Debug.WriteLine(e.Data);
        ffmpeg.ErrorDataReceived += (s, e) => Debug.WriteLine($@"Error: {e.Data}");
        ffmpeg.Start();
        ffmpeg.WaitForExit();
        string output = System.IO.File.ReadAllText(path + "\\zalo_tts\\output.txt");
        if (output.Contains("API rate limit exceeded"))
        {
            MessageBox.Show("Bạn đã hết giới hạn sử dụng API Key, vui lòng đổi key khác hoặc liên hệ ZaloAI để được hỗ trợ.\nMã lỗi: "+output,"Lỗi");
            System.Environment.Exit(1);
        }
        var stuff = JObject.Parse(output);
        if (stuff["data"]["url"].ToString().Contains("chunk"))
        {
            return stuff["data"]["url"].ToString();
        }
        else
        {
            return output;
        }
        
    }
    private void DownFileM3U8toMP3(string url, string saveName = "audio.mp3")
    {
        string cml = @" -i """ + url + @""" -ab 256k """ + saveName + @"""";
        Console.WriteLine(cml);
        ffmpeg = new Process
        {
            StartInfo = {
            FileName = path+"\\ffmpeg.exe",
            Arguments = cml,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = path+"\\audio"
        }
        };

        ffmpeg.EnableRaisingEvents = true;
        ffmpeg.OutputDataReceived += (s, e) => Debug.WriteLine(e.Data);
        ffmpeg.ErrorDataReceived += (s, e) => Debug.WriteLine($@"Error: {e.Data}");
        ffmpeg.Start();
        ffmpeg.BeginOutputReadLine();
        ffmpeg.BeginErrorReadLine();
        ffmpeg.WaitForExit();
    }

    public int getProcessNow()
    {
        return this.processNow;
    }
    public string getProcessMes()
    {
        return processMes;
    }
    string getText()
    {
        return this.text;
    }
    void setText(string _text)
    {
        this.text = _text;
    }
    string getSpeed()
    {
        return this.speed;
    }
    void setSpeed(string _speed)
    {
        this.speed = _speed;
    }
    
    public List<string> SplitStringEveryNth(string input, int chunkSize)
    {
        var output = new List<string>();
        var flag = chunkSize;
        var tempString = string.Empty;
        var lenght = input.Length;

        for (var i = 0; i < lenght; i++)
        {
            if (Int32.Equals(flag, 0))
            {
                output.Add(tempString);
                tempString = string.Empty;
                flag = chunkSize;
            }
            else
            {
                tempString += input[i];
                flag--;
            }

            if ((input.Length - 1) == i && flag != 0)
            {
                tempString += input[i];
                output.Add(tempString);
            }
        }
        return output;
    }
}