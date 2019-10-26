﻿using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ionic.Zip;
using System.Xml;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System.Xml.XPath;
using HtmlAgilityPack;
using System.Collections.Generic;
using ChoHoe;
using System.ComponentModel;

namespace ChoHoeBV
{

    public partial class Form1 : MetroFramework.Forms.MetroForm
    {


        BackgroundWorker bw = new BackgroundWorker();
        BackgroundWorker bwConvert = new BackgroundWorker();

        string UserSelectedFilePath;
        //單本專用ㄉ變數R
        Book abook = new Book();
        //多本專用ㄉ變數
        List<Book> batchBookList = new List<Book>();

        //ResourceManager Rm = new ResourceManager("作夥直排_Csharp_ver.Strings", Assembly.GetExecutingAssembly());
 
        bool ToTradictional =true ;
        ArrayList xhtml = new ArrayList();
      
       
        
       
        public Form1()
        {
            InitializeComponent();
            Make_Btn.Enabled = false;
            Logger.logger.Info("🦄//////////////////🦄 - App Started - 🧛///////////////////////🧛");
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(IfDoModifyPageDirection_Chkbox, "預設會強制指定為由右而左，直排小說的翻頁方向。");
            SetInitialValue();
           
            //呼叫語言func

            LanguageChanging(Convert.ToString(languagecombobox.SelectedValue));
            this.StyleManager = metroStyleManager1;


            NewVersionCheck versionCheck=new NewVersionCheck();
            if (versionCheck.Hasnew())
            {

            } 
         
        }

        private void SetInitialValue()
        {
            IfDoToChineseChkbox.Checked = ChoHoe.Properties.Settings.Default.ChineseConvert;
            IfDoModifyPageDirection_Chkbox.Checked = ChoHoe.Properties.Settings.Default.IfChangePageDirection;
            PageRTL_Radio.Checked = ChoHoe.Properties.Settings.Default.PageDirection;
            PageLTR_Radio.Checked = !ChoHoe.Properties.Settings.Default.PageDirection;
            IfReplacePicture_Chkbox.Checked = ChoHoe.Properties.Settings.Default.ReplaceImg;
            IfConvertMobi_Chkbox.Checked = ChoHoe.Properties.Settings.Default.ConvertMobi;
            IfEmbdedFont_Chkbox.Checked = ChoHoe.Properties.Settings.Default.EmbedFont;
            ToTraditionValue_Cmd.Text= ChoHoe.Properties.Settings.Default.ToTriditional ? ">" : "<";
            ToTradictional = ChoHoe.Properties.Settings.Default.ToTriditional;

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

        }
            
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        private void ToTraditionValue_Cmd_Click(object sender, EventArgs e)
        {
            
            if (ToTraditionValue_Cmd.Text == ">")
            {
                ToTraditionValue_Cmd.Text = "<";

                ChoHoe.Properties.Settings.Default.ToTriditional = ToTradictional = false;
            }
            else
            {
                ToTraditionValue_Cmd.Text = ">";
                ChoHoe.Properties.Settings.Default.ToTriditional = ToTradictional = true;
            }
            ChoHoe.Properties.Settings.Default.Save();
        }

        public void LanguageChanging(string langCode)
        {
                       
        }
      

        private void Language_Convert_TCT_Switcher_Btn_Batch_Click(object sender, EventArgs e)
        {
            int TCT_Status_Batch = 0;
            if (Language_Convert_TCT_Switcher_Btn_Batch.Text == ">")
            {
                Language_Convert_TCT_Switcher_Btn_Batch.Text = "<";
                TCT_Status_Batch = 1;
            }
            else
            {
                Language_Convert_TCT_Switcher_Btn_Batch.Text = ">";
                TCT_Status_Batch = 0;
            }
        }

        private void OpenBtn_Single_Click(object sender, EventArgs e)
        {
            Make_Btn.Enabled = false;
            Logger.logger.Info(System.Environment.NewLine+$"///////////////Open File///////////////"+System.Environment.NewLine+"////////////////////////////////");
            Logger.logger.Debug("開啟檔案");
            OpenFileDialog Import_File = new OpenFileDialog();
            Import_File.Filter = "EPUB檔案|*.epub";
            Import_File.Title = "請選擇一個epub檔案";
            if (Import_File.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                // bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(loadBack_RunWorkerCompleted) ;


                inprogressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
                inprogressBar.MarqueeAnimationSpeed = 30;
                abook = new Book();
                Logger.logger.Trace($"{Import_File.SafeFileName}");
                bw.DoWork += new DoWorkEventHandler(loadBackgroundworker_DoWork);
                Logger.logger.Trace("開始讀取檔案");

                bw.RunWorkerAsync(argument: Import_File.FileName);
                //abook.Load(Import_File.FileName);
                //fileimport.load( Import_File.FileName);
               
                //g_unzipDirectory = fileimport.g_tempuncompressedpath;
            }


        }


        
        

        private void TabPage_Single_Click(object sender, EventArgs e)
        {

        }
        private void Make_Btn_Click(object sender, EventArgs e)
        {
            
            Make_Btn.Enabled = false;
            StatusLabel.Text = "轉檔中...";
            bwConvert.RunWorkerCompleted += new RunWorkerCompletedEventHandler(convertBack_RunWorkerCompleted);


            inprogressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            inprogressBar.MarqueeAnimationSpeed = 30;
            

            //Logger.logger.Trace($"{}");
            bwConvert.DoWork += new DoWorkEventHandler(convertBackgroundworker_DoWork);
            Logger.logger.Trace("開始轉檔");

            Make_Btn.Enabled = false;
            bwConvert.WorkerSupportsCancellation = true;
            bwConvert.RunWorkerAsync(argument: IfDoModifyPageDirection_Chkbox.Checked);


        }
       

        private void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
          
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox1 aboutboxxx = new AboutBox1();
            Setting settingForm = new Setting();
            settingForm.Show();
            
            //aboutboxxx.Show();
        }

        private void OpenBtn_Batch_Click(object sender, EventArgs e)
        {
            OpenFileDialog Import_File = new OpenFileDialog();
            Import_File.Filter = "EPUB檔案|*.epub";
            Import_File.Title = "請選擇一個epub檔案";
            Import_File.Multiselect = true;
         

            if (Import_File.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {

                return;
                
             

            }

           



            foreach (string name in Import_File.FileNames)
            {
                Batch_grid.Rows.Add(new object[] { System.IO.Path.GetFileNameWithoutExtension(name) });
                Book abooks = new Book();
                abooks.Load(name);
                batchBookList.Add(abook);
            }
        }

        private void Make_Batch_Click(object sender, EventArgs e)
        {
       
        }

        private void PageRTL_Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (PageRTL_Radio.Checked)
            {
                ChoHoe.Properties.Settings.Default.PageDirection= PageRTL_Radio.Checked;
                ChoHoe.Properties.Settings.Default.Save();
            }
        }

        private void PageLTR_Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (PageLTR_Radio.Checked)
            {
                ChoHoe.Properties.Settings.Default.PageDirection = PageLTR_Radio.Checked;
                ChoHoe.Properties.Settings.Default.Save();
            }
        }

        private void IfDoToTriditionChkbox_CheckChanged(object sender, EventArgs e)
        {
            ChoHoe.Properties.Settings.Default.ChineseConvert = IfDoToChineseChkbox.Checked;
            ChoHoe.Properties.Settings.Default.Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ChoHoe.Properties.Settings.Default.Save();
        }

        private void IfDoModifyPageDirection_Chkbox_CheckedChanged(object sender, EventArgs e)
        {
            ChoHoe.Properties.Settings.Default.IfChangePageDirection = IfDoModifyPageDirection_Chkbox.Checked;
            ChoHoe.Properties.Settings.Default.Save();
        }

        private void IfReplacePicture_Chkbox_CheckedChanged(object sender, EventArgs e)
        {
            ChoHoe.Properties.Settings.Default.ReplaceImg = IfReplacePicture_Chkbox.Checked;
            ChoHoe.Properties.Settings.Default.Save();
        }

        private void IfConvertMobi_Chkbox_CheckedChanged(object sender, EventArgs e)
        {
            ChoHoe.Properties.Settings.Default.ConvertMobi = IfConvertMobi_Chkbox.Checked;
            ChoHoe.Properties.Settings.Default.Save();
        }

        private void IfEmbdedFont_Chkbox_CheckedChanged(object sender, EventArgs e)
        {
            ChoHoe.Properties.Settings.Default.EmbedFont = IfEmbdedFont_Chkbox.Checked;
            ChoHoe.Properties.Settings.Default.Save();
        }
        private void loadBackgroundworker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            string path = (string)e.Argument;
         
            abook.Load(path);
        }
        private void convertBackgroundworker_DoWork(object sender, DoWorkEventArgs e)
        {

            bool Modifypage = (bool)e.Argument;

           

            if (Modifypage)
            {
                abook.Convert(IfDoToChineseChkbox.Checked, ToTradictional, PageRTL_Radio.Checked, IfConvertMobi_Chkbox.Checked, IfEmbdedFont_Chkbox.Checked, IfReplacePicture_Chkbox.Checked, Author_Imported_TextBox.Text, Title_Imported_TextBox.Text);
            }
            else
            {
                abook.Convert(IfDoToChineseChkbox.Checked, ToTradictional, true, IfConvertMobi_Chkbox.Checked, IfEmbdedFont_Chkbox.Checked, IfReplacePicture_Chkbox.Checked, Author_Imported_TextBox.Text, Title_Imported_TextBox.Text);
            }


            e.Cancel = true;
            return;

           
        }
        private void loadBack_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Author_Imported_TextBox.Text = abook.GetAuthor();
            Title_Imported_TextBox.Text = abook.GetTitle();
            Make_Btn.Enabled = true;
            int index =ChoHoe.Properties.Settings.Default.DebugBookIndex;
            string debugstring = $"偵錯用書本 編號 #{index}";
            ChoHoe.Properties.Settings.Default.DebugBookIndex = ++index;
            ChoHoe.Properties.Settings.Default.Save();

           // Title_Imported_TextBox.Text = debugstring;
            StatusLabel.Text = "讀取完畢。";
            inprogressBar.MarqueeAnimationSpeed = 0;
            inprogressBar.Value = 0;
        }
        private void convertBack_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Author_Imported_TextBox.Text = "";
            Title_Imported_TextBox.Text = "";
            Make_Btn.Enabled = false;
 
            StatusLabel.Text = "轉檔完畢。";
            ClearDirectory("temp");
            inprogressBar.MarqueeAnimationSpeed = 0;
            inprogressBar.Value = 0;
        }
        private void SetCustomizeLocalizationWord_cmd_Click(object sender, EventArgs e)
        {

        }

        private void SettingCmd_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutboxxx = new AboutBox1();
            Setting settingForm = new Setting();
            settingForm.Show();
        }
    }

}
