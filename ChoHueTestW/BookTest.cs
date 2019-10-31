﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChoHoeBV;
using ChoHoe;
namespace ChoHueTestW
{
    [TestClass]
    public class BookTest
    {



        
        [TestMethod]
        public void Load20Book()
        {
            var bk = new Book();
            bk.Load(@"J:\development\EPUB vertical sample file\2.0.epub",@"C:\");
            
        }
        [TestMethod]
        public void Load30Book()
        {
            var bk = new Book();
            bk.Load(@"J:\development\EPUB vertical sample file\Fei Dan De Ya Li Ya  27 - Mo Zhe.epub");
           
        }
        [TestMethod]
        public void Convert20Book()
        {
            var bk = new Book();
            bk.Load(@"J:\development\EPUB vertical sample file\2.0.epub", @"C:\");
            
            bool dotranslate = false;
            bool totriditional = true;
            bool pageDirection = false;
            bool convertMobi = false;
            bool fontemb = false;
            bool replacepicture = true;
            int debugcount= ChoHueTestW.Properties.Settings.Default.debugCount;
            string debugtext= $"偵錯用 2.0 書本 編號 #{debugcount} 「{bk.GetTitle()}」";
            ChoHueTestW.Properties.Settings.Default.debugCount++;
            ChoHueTestW.Properties.Settings.Default.Save();


            bk.Convert(dotranslate, totriditional, pageDirection, convertMobi, fontemb, replacepicture, bk.GetAuthor(), debugtext);
        }
        [TestMethod]
        public void Convert30Book()
        {
            var bk = new Book();
            bk.Load(@"J:\development\EPUB vertical sample file\Fei Dan De Ya Li Ya  27 - Mo Zhe.epub");
            
            bool dotranslate = false;
            bool totriditional = true;
            bool pageDirection = false;
            bool convertMobi = false;
            bool fontemb = false;
            bool replacepicture = true;

            int debugcount = ChoHueTestW.Properties.Settings.Default.debugCount;
            string debugtext = $"偵錯用 3.0 書本 編號 #{debugcount} 「{bk.GetTitle()}」";
            ChoHueTestW.Properties.Settings.Default.debugCount++;
            ChoHueTestW.Properties.Settings.Default.Save();


            bk.Convert(dotranslate, totriditional, pageDirection, convertMobi, fontemb, replacepicture, bk.GetAuthor(), debugtext);
        }





        [TestMethod]
        public async void CheckNewVersionTrue()
        {
            var cv =new NewVersionCheck();
            var testResult = await cv.HasnewAsync();
            var expected = true;

            Assert.AreEqual(expected, testResult);

        }
        [TestMethod]
        public async void CheckNewVersionFalse()
        {
            var cv = new NewVersionCheck();
            var testResult =await cv.HasnewAsync();
            var expected = false;

            Assert.AreEqual(expected, testResult);

        }
    }
}

