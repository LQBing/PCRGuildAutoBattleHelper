using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCRGuildAutoBattleHelper
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
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
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(tbxApiKey.Text!=""&&tbxSecretKey.Text!="")
            {
                Program.ApiKey = tbxApiKey.Text;
                Program.SecretKey = tbxSecretKey.Text;
                var client = new Baidu.Aip.Ocr.Ocr(Program.ApiKey, Program.SecretKey);
                var result = client.GeneralBasic(BitmapByte(Icon.ToBitmap()), null);
                AccessTokenModel a = JsonConvert.DeserializeObject<AccessTokenModel>(result.ToString());               
                if (a.error_msg!=null)
                {
                    string rz = "验证失败！请输入有效的ApiKey和SecretKey!\n";
                    MessageBox.Show(rz);
                }
                else
                {
                    MessageBox.Show("成功登录，登录id:"+ a.log_id.ToString());
                    this.DialogResult = DialogResult.OK;
                }                   
            }
            else
            {
                MessageBox.Show("请输入值！");
            }
        }
    }
}
