using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections;

namespace PCRGuildAutoBattleHelper
{
    public partial class frmMain : Form
    {
        //========================定义常量===========================
        uint WM_MOUSEMOVE = 0x0200;
        uint WM_LBUTTONDOWN = 0x201;
        uint WM_LBUTTONUP = 0x202;
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern int GetWindowRect(IntPtr hwnd, ref System.Drawing.Rectangle lpRect);
        [DllImport("User32.dll")]//取设备场景
        private static extern IntPtr GetDC(IntPtr hwnd);//返回设备场景句柄
        //结构体布局 本机位置
        [StructLayout(LayoutKind.Sequential)]
        struct NativeRECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(HandleRef hwnd, out NativeRECT rect);
        //========================定义常用句柄===========================
        public static IntPtr hwndCalc = FindWindow(null, "雷电模拟器");
        public static IntPtr hwndChild = FindWindowEx(hwndCalc, IntPtr.Zero, null, "TheRender");
        //========================构造方法===========================
        public frmMain()
        {
            InitializeComponent();
        }
        //========================点击安卓屏幕===========================
        public bool ClickAndroidScreen(int x, int y)
        {
            IntPtr myhandle = hwndChild;

            if (myhandle != IntPtr.Zero)
            {
                for(int i = 0; i < 20; i++)
                {
                    PostMessage(myhandle, WM_LBUTTONDOWN, 0, x + (y << 16));
                    PostMessage(myhandle, WM_LBUTTONUP, 0, x + (y << 16));
                    Thread.Sleep(1);
                }                
                return true;
            }
            return false;
        }
        //========================拖动安卓屏幕===========================
        public bool SlideAndroidScreen(int x, int y, int z)
        {
            IntPtr myhandle = hwndChild;

            if (myhandle != IntPtr.Zero)
            {
                PostMessage(myhandle, WM_LBUTTONDOWN, 0, x + (y << 16));
                PostMessage(myhandle, WM_MOUSEMOVE, 0, x + (z << 16));
                PostMessage(myhandle, WM_LBUTTONUP, 0, x + (z << 16));
                return true;
            }
            return false;
        }
        //========================捕捉安卓屏幕===========================
        public class ScreenCapture
        {
            /// 
            /// Creates an Image object containing a screen shot of the entire desktop?
            /// 
            /// 
            public Image CaptureScreen()
            {
                return CaptureWindow(User32.GetDesktopWindow());
            }
            /// 
            /// Creates an Image object containing a screen shot of a specific window?
            /// 
            /// The handle to the window. (In windows forms, this is obtained by the Handle property)
            /// 
            public Image CaptureWindow(IntPtr handle)
            {
                // get te hDC of the target window
                IntPtr hdcSrc = User32.GetWindowDC(handle);
                // get the size
                User32.RECT windowRect = new User32.RECT();
                User32.GetWindowRect(handle, ref windowRect);
                //int width = windowRect.right - windowRect.left;
                //int height = windowRect.bottom - windowRect.top;
                int width =40;
                int height = 19;
                // create a device context we can copy to
                IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
                // create a bitmap we can copy it to,
                // using GetDeviceCaps to get the width/height
                IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
                // select the bitmap object
                IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
                // bitblt over
                GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 1080, 25, GDI32.SRCCOPY);
                // restore selection
                GDI32.SelectObject(hdcDest, hOld);
                // clean up 
                GDI32.DeleteDC(hdcDest);
                User32.ReleaseDC(handle, hdcSrc);
                // get a .NET image object for it
                Image img = Image.FromHbitmap(hBitmap);
                // free up the Bitmap object
                GDI32.DeleteObject(hBitmap);
                return img;
            }
            /// 
            /// Captures a screen shot of a specific window, and saves it to a file?
            /// 
            /// 
            /// 
            /// 
            public void CaptureWindowToFile(IntPtr handle, string filename, System.Drawing.Imaging.ImageFormat format)
            {
                Image img = CaptureWindow(handle);
                img.Save(filename, format);
            }
            /// 
            /// Captures a screen shot of the entire desktop, and saves it to a file?
            /// 
            /// 
            /// 
            public void CaptureScreenToFile(string filename, System.Drawing.Imaging.ImageFormat format)
            {
                Image img = CaptureScreen();
                img.Save(filename, format);
            }

            /// 
            /// Helper class containing Gdi32 API functions
            /// 
            private class GDI32
            {

                public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
                [DllImport("gdi32.dll")]
                public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                    int nWidth, int nHeight, IntPtr hObjectSource,
                    int nXSrc, int nYSrc, int dwRop);
                [DllImport("gdi32.dll")]
                public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                    int nHeight);
                [DllImport("gdi32.dll")]
                public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
                [DllImport("gdi32.dll")]
                public static extern bool DeleteDC(IntPtr hDC);
                [DllImport("gdi32.dll")]
                public static extern bool DeleteObject(IntPtr hObject);
                [DllImport("gdi32.dll")]
                public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
            }

            /// 
            /// Helper class containing User32 API functions
            /// 
            private class User32
            {
                [StructLayout(LayoutKind.Sequential)]
                public struct RECT
                {
                    public int left;
                    public int top;
                    public int right;
                    public int bottom;
                }
                [DllImport("user32.dll")]
                public static extern IntPtr GetDesktopWindow();
                [DllImport("user32.dll")]
                public static extern IntPtr GetWindowDC(IntPtr hWnd);
                [DllImport("user32.dll")]
                public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
                [DllImport("user32.dll")]
                public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            }
        }

        //========================自己的方法===========================  
        public void AddTextToBoard(string str)
        {
            tbxEditor.AppendText(str);
            tbxEditor.AppendText(Environment.NewLine);
            tbxEditor.ScrollToCaret();
        }
        public string ConvertIntToTime(int seconds)
        {
            string min = (seconds / 60).ToString();
            string colon = ":";
            string sec = (seconds % 60).ToString();
            if (Convert.ToInt32(sec) / 10 == 0)
            {
                sec = "0" + sec;
            }
            string UB_Time = min + colon + sec;

            return UB_Time;
        }

        public int ConvertCharacterNameToNumber(string name)
        {
            string charaName = name;
            int num;
            if (listCharacter.SingleOrDefault(x => x.characterName == charaName)!=null)
            {
                 num = listCharacter.SingleOrDefault(x => x.characterName == charaName).characterNum;
            }
            else
            {
                num = 0;                
            }
            return num;
        }
    
        public void ClickCharacterByUBTime(string Now,string UBtime,int num)
        {
            if (Now == UBtime)
            {
                ClickCharacter(num);
            }            
        }
        public void CleanScript()
        {
            listCharacter = null;
            listUB = null;
        }
        public static byte[] BitmapByte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }
        private string ApiKey = Program.ApiKey;
        private string SecretKey = Program.SecretKey;
        public void ClickCharacter(int num)
        {
            if(num==1)
            ClickAndroidScreen(960, 600);
            else if (num == 2)
                ClickAndroidScreen(800, 600);
            else if(num==3)
                ClickAndroidScreen(632, 600);
            else if(num==4)
                ClickAndroidScreen(480, 600);
            else if (num==5)
                ClickAndroidScreen(320, 600);
        }

        List<CharacterNumModel> listCharacter = new List<CharacterNumModel>();

        List<CharacterUBModel> listUB = new List<CharacterUBModel>();
        private void btnReadScript_Click(object sender, EventArgs e)
        {
            string str=tbxEditor.Text;
            //先看存不存在定位
            if (str.Contains("定位") && str.Contains(";") && str.Contains(",") && str.Contains("(")&&str.Contains(")"))
            {
                //以中文分号进行分割
                string[] strArr = str.Split(';');
                //获得小括号里面的值
                string team = Regex.Replace(strArr[0], @"(.*\()(.*)(\).*)", "$2");
                //分割获得单个角色名
                string[] teamArr = team.Split(',');
                //先判断五个参数全不全
                if (teamArr.Count() == 5)
                {
                    //录入名称和参数的对应关系             
                    for (int i = 0; i < 5; i++)
                    {
                        CharacterNumModel ubModel = new CharacterNumModel();
                        ubModel.characterName = teamArr[i];
                        ubModel.characterNum = i + 1;
                        listCharacter.Add(ubModel);
                    }
                    //有分号就肯定有第二部分，而且可能是空字符串，非null
                    if (strArr[1] != "")
                    {
                        if (strArr.LastOrDefault() == "")
                        {
                            //剔除第一组
                            ArrayList al = new ArrayList(strArr);
                            al.RemoveAt(0);
                            //重新生成StrArr(只包含int，string)
                            strArr = (string[])al.ToArray(typeof(string));
                            //确定执行的次数，-1是为了去除末尾的""
                            for (int i = 0; i < strArr.Count() - 1; i++)
                            {
                                //UBCharacter==UBTime+CharacterName
                                string[] UBcharacter = strArr[i].Split(',');
                                //Get int time
                                string time = UBcharacter[0];
                                //Get string Name
                                string chara = UBcharacter[1];
                                //name,int
                                CharacterUBModel ubModel = new CharacterUBModel();
                                //string类型的时间转化为int类型
                                int seconds = Convert.ToInt32(time);
                                if (seconds >= 0)
                                {
                                    //时间，对象
                                    ubModel.ubTime = ConvertIntToTime(seconds);
                                    ubModel.clickWho = ConvertCharacterNameToNumber(chara);
                                    listUB.Add(ubModel);
                                }
                                else
                                {
                                    MessageBox.Show("请不要输入负数！");
                                    //将之前录入的脚本清空
                                    CleanScript();
                                }

                            }
                            bool isCorrect = true;
                            foreach (var item in listUB)
                            {
                                if (item.clickWho == 0)
                                {
                                    isCorrect = false;
                                }
                            }
                            if (isCorrect == true)
                            {
                                MessageBox.Show("脚本读取成功！");
                                this.tbxEditor.Enabled = false;
                                this.btnReadScript.Enabled = false;
                            }
                            else
                            {
                                MessageBox.Show("变量名冲突，请检查！");
                                //将之前录入的脚本清空
                                CleanScript();
                            }
                        }
                        else
                        {
                            MessageBox.Show("请填写完整的脚本！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("请填写第二部分的脚本！");
                    }
                }
                //5个参数不全
                else
                {
                    MessageBox.Show("请填写5个参数,目前只有"+teamArr.Count().ToString()+"个参数！");
                }               
            }
            else
            {
                MessageBox.Show("请按照语法填写！样例：定位（布丁，空花，狗拳，亚莉莎，xcw）；\n");
            }
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            tmReadSeconds.Start();                     
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tmReadSeconds.Stop();
            //将之前录入的脚本清空
            CleanScript();
            tbxEditor.Enabled = true;
        }

        //========================计时器事件===========================
        private void tmReadSeconds_Tick(object sender, EventArgs e)
        {
            Rectangle rect = new System.Drawing.Rectangle();
            IntPtr mainHandle = hwndChild;
            GetWindowRect(mainHandle, ref rect);
            ScreenCapture t = new ScreenCapture();
            Image img = t.CaptureWindow(mainHandle);

            Bitmap bit = new Bitmap(img);

            pictureBox1.Image = bit;

            var client = new Baidu.Aip.Ocr.Ocr(ApiKey, SecretKey);
            // 通用文字识别
            var result = client.GeneralBasic(BitmapByte(bit), null);
            AccessTokenModel ocrResult = JsonConvert.DeserializeObject<AccessTokenModel>(result.ToString());
            string rz = "";
            if (ocrResult.error_msg != null)
            {
                rz = "QPS已达到峰值！";
            }
            else
            {
                if (ocrResult.words_result.HasValues)
                {
                    //rz = result["words_result"].First.SelectToken("words").ToString();
                    rz = ocrResult.words_result.First.SelectToken("words").ToString();
                    //=======================从这里开始写轴===========================
                    for (int i = 0; i < listUB.Count; i++)
                    {
                        ClickCharacterByUBTime(rz, listUB[i].ubTime, listUB[i].clickWho);
                    }
                }
                else
                {
                    rz = "无法识别";
                }
            }
            AddTextToBoard(rz);


        }

        //分辨率1280*720(dpi 240)
        //雷电模拟器4.0.26
        //窗口标题是“雷电模拟器”
        //没有多开
        //320*600 5
        //480*600 4
        //632*600 3
        //800*600 2
        //960*600 1
    }
}
