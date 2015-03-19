using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using VDS.Utils;

namespace VDS.Ctrls
{
    public enum themeEnum
    {
        None = 0,
        PIN = 1,
        VIN = 2,
        ESK = 3,
    }
    public partial class TextBoxEx : TextBox
    {

        #region 属性

        private themeEnum theme;
        [Category("自定义"),
        RefreshProperties(RefreshProperties.All),
        DefaultValue(0)
        ]
        public themeEnum Theme
        {
            get { return theme; }
            set
            {
                theme = value;
                this.title = "请输入";
                if (theme == themeEnum.None)
                {
                    this.regExp = "";
                    this.coustomMaxLength = 0;
                }
                else
                {
                    InitByTheme(theme.ToString());
                    this.title += theme;
                }
            }
        }

        private string title = "请输入";
        [Category("自定义")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }


        private string regExp;

        [Category("自定义")]
        [Description("正则表达式验证输入")]
        public string RegExp
        {
            get { return regExp; }
            set { regExp = value; }
        }


        private int coustomMaxLength = 0;

        [Category("自定义")]
        [Description("限制输入的长度")]
        public int CoustomMaxLength
        {
            get { return coustomMaxLength; }
            set { coustomMaxLength = value; }
        }

        //private string value;
        //public string Value
        //{
        //    get { return this.value; }
        //    set { this.value = value; }
        //}

        #endregion
        public TextBoxEx()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private void InitByTheme(string theme)
        {
            switch (theme.ToUpper())
            {
                case "PIN":
                    this.coustomMaxLength = 8;
                    this.regExp = "[0-9A-Fa-f]{8}";
                    break;
                case "VIN":
                    this.coustomMaxLength = 17;
                    this.regExp = "[0-9A-Za-z]{17}";
                    break;
                case "ESK":
                    this.coustomMaxLength = 32;
                    this.regExp = "[0-9A-Fa-f]{32}";
                    break;
                default:
                    this.coustomMaxLength = 0;
                    this.regExp = "";
                    break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_CHAR = 0x0102;
            const int WM_PASTE = 0x0302;

            switch (m.Msg)
            {
                case WM_CHAR:
                    char c = (char)m.WParam;

                    if (CheckLengthFlow(c.ToString()))
                    {
                        break;
                    }

                    base.WndProc(ref m);

                    //  CheckInputData(c.ToString());//"[0-9A-Fa-f]"

                    break;
                case WM_PASTE:
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData.GetDataPresent(DataFormats.Text)) 
                    {
                        string text = (string)iData.GetData(DataFormats.Text);

                        if (CheckLengthFlow(text))
                        {
                            m.Result = (IntPtr)0;
                            break;
                        }
                    }
                    base.WndProc(ref m);

                    //  CheckInputData(c.ToString());//"[0-9A-Fa-f]"

                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private bool CheckLengthFlow(string text)
        {
            if (this.coustomMaxLength == 0)
                return false;

            int len = GetByteLength(text);  
            int tlen = GetByteLength(this.Text);  
            int slen = GetByteLength(this.SelectedText);   

            return (this.coustomMaxLength < (tlen - slen + len));
        }

        private int GetByteLength(string text)
        {
            return System.Text.Encoding.Default.GetBytes(text).Length;
        }

        private void CheckInputData(string input)
        {
            bool bl = Validation(input, this.regExp);
            if (!bl)
            {
                VDSCenter.ShowVDSMessageBox(this.theme.ToString() + "码输入不合法");
            }
        }

        private bool Validation(string validValue, string regExp)
        {
            if (string.IsNullOrEmpty(regExp))
                return true;

            Regex regex;
            try
            {
                regex = new Regex(regExp);
            }
            catch
            {
                return false;
            }

            return regex.IsMatch(validValue);
        }

        public bool Validation(string regExp)
        {
            return Validation(this.Text, regExp);
        }

        public bool Validation()
        {
            bool bl = Validation(this.Text, this.regExp);
            if (!bl)
            {
                VDSCenter.ShowVDSMessageBox(this.theme.ToString() + "码输入不合法");
            }
            return bl;
        }

    }
}
