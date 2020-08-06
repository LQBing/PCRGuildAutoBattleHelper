using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCRGuildAutoBattleHelper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //实例化APIKey、SecretKey的输入窗体
            frmLogin objFrmLogin = new frmLogin();
            DialogResult result = objFrmLogin.ShowDialog();
            if (result == DialogResult.OK)
            {
                Application.Run(new frmMain());
            }
            //Application.Run(new frmMain());
        }
        //因为不是由login窗体生成的Main，所以无法进行窗体之间构造方法的传值，这里选择定义全局变量
        public static string ApiKey;
        public static string SecretKey;
    }
}
