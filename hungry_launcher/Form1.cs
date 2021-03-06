﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Text;
using System.IO.Compression;
using Microsoft.Win32;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace hungry_launcher
{
    public partial class Form1 : Form
    {
        utils ut = new utils();
        private utils.GetRequest response;
        private static double buffer;
        private string javahome;
        private string mdir;
        private string mversion;
        private string alocmem;
        private string uuid;
        private string token;
        private bool fastdown;
        private string[] mver;
        private utils.Version[] downver;
        private bool logged;
        private bool console, autoclose, downloading, license;
        private static long fsize;


        public Form1()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) &&
                    (p.MainModule.FileName == curr.MainModule.FileName))
                {
                    MessageBox.Show("Launcher is already runnig");
                    Thread.Sleep(10);
                    Environment.Exit(0);
                }
            }

            ut.setform(this);

            mdir = Properties.Settings.Default.mdir;

            this.TopMost = true;

            if (mdir == "")
            {
                using (var dialog = new FolderBrowserDialog())
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        this.TopMost = false;
                        mdir = dialog.SelectedPath;
                        Properties.Settings.Default.mdir = mdir;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Thread.Sleep(10);
                        Environment.Exit(0);
                    }
            }

            this.TopMost = true;

            bool fexist = Directory.Exists(mdir);
            if (fexist == false)
                Directory.CreateDirectory(mdir);

            checkBox1.Checked = Properties.Settings.Default.chBox;
            checkBox2.Checked = Properties.Settings.Default.chBox2;
            checkBox3.Checked = Properties.Settings.Default.chBox3;
            checkBox4.Checked = Properties.Settings.Default.chBox4;
            checkBox5.Checked = Properties.Settings.Default.chBox5;
            checkBox6.Checked = Properties.Settings.Default.chBox6;
            checkBox7.Checked = Properties.Settings.Default.chBox7;
            checkBox8.Checked = Properties.Settings.Default.chBox8;
            logged = Properties.Settings.Default.logged;

            if (checkBox6.Checked == false)
            {
                javahome = Properties.Settings.Default.javapath;
            }
            else
            {
                javahome = ut.getjavapath();
            }

            if (checkBox2.Checked == true)
            {
                textBox2.Text = Properties.Settings.Default.Textbox2;
            }
            textBox1.Text = Properties.Settings.Default.Textbox;
            textBox3.Text = Properties.Settings.Default.Textbox3;

            if (checkBox8.Checked == true && checkBox5.Checked == true && logged == true)
            {
                button7.PerformClick();
            }

            if (checkBox5.Checked == true)
            {
                textBox2.Text = Properties.Settings.Default.Textbox2;

                label1.Text = "   Login    ";

                if (logged == true)
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;

                    button7.Visible = false;//Exchane button 7
                    button8.Visible = true; //abd button 8 coords

                    label7.Text = "Hello " + response.selectedProfile.name;
                    label7.Visible = true;

                    button1.Visible = true;
                }
                else
                {
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;

                    button8.Visible = false;//Exchane button 7
                    button7.Visible = true; //abd button 8 coords

                    label7.Text = "Hello ";
                    label7.Visible = false;

                    button1.Visible = false;
                }

            }
            else
            {
                label1.Text = "Nickname ";

                textBox1.Enabled = true;
                textBox2.Enabled = false;

                button7.Visible = false;
                button8.Visible = false;

                button1.Visible = true;

                textBox2.Text = "";
            }

            mver = ut.installedver(mdir);

            if (mver != null)
            {
                foreach (Object i in mver)
                {
                    comboBox1.Items.Add(i);
                }
                if (mver.Contains(Properties.Settings.Default.combobox)) comboBox1.Text = Properties.Settings.Default.combobox;
            }

            downver = ut.getverlist(mdir);

            if (downver != null)
            {
                foreach (var item in downver)
                {
                    comboBox3.Items.Add(item.id);
                }
            }
            else
            {
                button3.Enabled = false;
                comboBox3.Enabled = false;
            }

            this.TopMost = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.combobox = comboBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            string text = comboBox1.Text;
            mver = ut.installedver(mdir);
            comboBox1.Items.Clear();
            if (mver != null)
            {
                if (mver.Length == 0) comboBox1.Items.Add("");
                foreach (Object i in mver)
                {
                    if (downloading == true && i.ToString() == comboBox3.Text)
                    {
                        continue;
                    }
                    else
                    {
                        comboBox1.Items.Add(i);
                    }
                }
                if (mver.Contains(text)) comboBox1.Text = text;
            }
            Properties.Settings.Default.combobox = comboBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {

        }

        private void comboBox3_DropDown(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            if (downver != null)
            {
                if (downver.Length == 0) comboBox3.Items.Add("");
                foreach (var item in downver)
                {
                    comboBox3.Items.Add(item.id);
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Textbox = textBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true && license == true)
            {
                Properties.Settings.Default.Textbox2 = textBox2.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Textbox3 = textBox3.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox2 = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox3 = checkBox3.Checked;
            Properties.Settings.Default.Save();
            autoclose = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox4 = checkBox4.Checked;
            Properties.Settings.Default.Save();
            utils.debug = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox5 = checkBox5.Checked;
            Properties.Settings.Default.Save();
            license = checkBox5.Checked;

            if (checkBox5.Checked == true)
            {
                textBox2.Text = Properties.Settings.Default.Textbox2;

                label1.Text = "   Login    ";

                if (logged == true)
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;

                    button7.Visible = false;//Exchane button 7
                    button8.Visible = true; //abd button 8 coords

                    label7.Text = "Hello " + response.selectedProfile.name;
                    label7.Visible = true;

                    button1.Visible = true;
                }
                else
                {
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;

                    button8.Visible = false;//Exchane button 7
                    button7.Visible = true; //abd button 8 coords

                    label7.Text = "Hello ";
                    label7.Visible = false;

                    button1.Visible = false;
                }

            }
            else
            {
                label1.Text = "Nickname ";

                textBox1.Enabled = true;
                textBox2.Enabled = false;

                button7.Visible = false;
                button8.Visible = false;

                button1.Visible = true;

                textBox2.Text = "";
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                javahome = ut.getjavapath();
                button6.Enabled = false;
            }
            else button6.Enabled = true;
            Properties.Settings.Default.chBox6 = checkBox6.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked == true)
            {
                fastdown = true;
            }
            else fastdown = false;
            Properties.Settings.Default.chBox7 = checkBox7.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.chBox8 = checkBox8.Checked;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long memory = 0;
            if (javahome == null)
            {
                MessageBox.Show("Can't find JAVA");
            }
            else
            {
                if (mdir == null)
                {
                    MessageBox.Show("Can't find installation path");
                }
                else
                {
                    if (string.IsNullOrEmpty(comboBox1.Text))
                    {
                        MessageBox.Show("Version doesn't set");
                    }
                    else
                    {
                        try
                        {
                            memory = Convert.ToInt64(textBox3.Text);
                        }
                        catch
                        {
                            memory = 0;
                        }

                        if (memory < 512)
                        {
                            MessageBox.Show("Not enough memory to launch");
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(textBox1.Text) == true || textBox1.Text.Contains(" "))
                            {
                                MessageBox.Show("Your username shouldn't be empty or contain spaces");
                            }
                            else
                            {
                                memory = 0;
                                console = checkBox1.Checked;
                                mversion = comboBox1.Text;
                                alocmem = textBox3.Text + "M";
                                char a = '"';
                                string memorys = " -Xms512M -Xmx{0}";
                                memorys = string.Format(memorys, alocmem);
                                ut.donwlibs(mversion, mdir);
                                string launch = ut.extractlibs(mversion, mdir);

                                string username;
                                if (license == true)
                                {
                                    username = response.selectedProfile.name;
                                    uuid = response.selectedProfile.id;
                                    token = response.accessToken;
                                }
                                else
                                {
                                    username = textBox1.Text;
                                    uuid = ut.getuuid();
                                }

                                launch = launch.Replace("${auth_player_name}", a + username + a);
                                launch = launch.Replace("${version_name}", a + mversion + a);
                                launch = launch.Replace("${game_directory}", a + mdir + a);
                                if (license == true)
                                {
                                    launch = launch.Replace("${auth_uuid}", a + uuid + a);
                                    launch = launch.Replace("${auth_access_token}", a + token + a);
                                }
                                else
                                {
                                    launch = launch.Replace("${auth_uuid}", a + uuid + a);
                                    launch = launch.Replace("${auth_access_token}", a + uuid + a);
                                }
                                launch = launch.Replace("${user_properties}", "{}");
                                launch = launch.Replace("${user_type}", a + "mojang" + a);

                                launch = memorys + launch;

                                if (console == true)
                                {
                                    Process minecraft = new Process();
                                    try
                                    {
                                        ProcessStartInfo mcstart = new ProcessStartInfo(javahome + "\\bin\\java.exe", launch);
                                        minecraft.StartInfo = mcstart;
                                        minecraft.Start();
                                        int procid = minecraft.Id;
                                        ut.launprof(username, mversion, "Standart", mdir);
                                        if (autoclose == true)
                                        {
                                            while (memory < 200000)
                                            {
                                                System.Diagnostics.Process pr = Process.GetProcessById(procid);
                                                memory = pr.WorkingSet64 / 1024;
                                            }
                                            Thread.Sleep(10);
                                            Environment.Exit(0);
                                        }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Can't start minecraft, something is wrong");
                                    }

                                }
                                else
                                {
                                    Process minecraft = new Process();
                                    try
                                    {
                                        ProcessStartInfo mcstart = new ProcessStartInfo(javahome + "\\bin\\javaw.exe", launch);
                                        minecraft.StartInfo = mcstart;
                                        minecraft.Start();
                                        int procid = minecraft.Id;
                                        ut.launprof(username, mversion, "Standart", mdir);
                                        if (autoclose == true)
                                        {
                                            while (memory < 200000)
                                            {
                                                System.Diagnostics.Process pr = Process.GetProcessById(procid);
                                                memory = pr.WorkingSet64 / 1024;
                                            }
                                            Thread.Sleep(10);
                                            Environment.Exit(0);
                                        }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Can't start minecraft, something is wrong");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = mdir;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    if (path != mdir)
                        comboBox1.Text = null;
                    mdir = path;
                }
                else
                {
                    path = mdir;
                }
            }
            Properties.Settings.Default.mdir = mdir;
            Properties.Settings.Default.Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text != "")
            {
                checkBox7.Enabled = false;
                comboBox3.Enabled = false;
                checkBox3.Enabled = false;
                downloading = true;

                if (comboBox1.Text == comboBox3.Text) comboBox1.Text = null;
                button2.Enabled = false;

                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string combo3text = "";

            MethodInvoker getValues = new MethodInvoker(delegate()
            {
                combo3text = comboBox3.Text;
            });

            if (this.InvokeRequired)
            {
                this.Invoke(getValues);
            }
            else
            {
                getValues();
            }

            mversion = combo3text;

            if (mversion != null)
            {


                if (fastdown == true)
                {
                    Thread th01 = new Thread(delegate() { ut.AS1getsize(mversion, mdir, out fsize); });
                    th01.Start();

                    long filesize = 0;

                    Thread th02 = new Thread(delegate() { ut.AS2getsize(mversion, mdir, out filesize); });
                    th02.Start();

                    do
                    {
                        Thread.Sleep(25);
                        if (th01.IsAlive == false) th01.Join();
                        if (th02.IsAlive == false) th02.Join();
                    } while (th02.IsAlive == true || th01.IsAlive == true);

                    fsize += filesize;

                    Thread th1 = new Thread(delegate() { ut.downloadver(mversion, mdir); });     //Normal
                    th1.Start();

                    Thread th2 = new Thread(delegate() { ut.getassets(mdir); });          // Async
                    th2.Start();

                    Thread th3 = new Thread(delegate() { ut.donwlibs(mversion, mdir); }); // Async
                    th1.Join();

                    th3.Start();

                    th2.Join();
                    th3.Join();

                }
                else
                {
                    fsize = ut.getsize(mversion, mdir);
                    ut.downloadver(mversion, mdir);
                    ut.donwlibs(mversion, mdir);
                    ut.getassets(mdir);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button3.Enabled = true;
            this.comboBox3.Enabled = true;
            this.checkBox7.Enabled = true;
            buffer = 0;
            fsize = 0;
            progressBar1.Invoke(new MethodInvoker(delegate() { progressBar1.Value = Convert.ToInt32(0); }));
            comboBox3.Invoke(new MethodInvoker(delegate() { comboBox3.Text = null; }));
            button2.Invoke(new MethodInvoker(delegate() { button2.Enabled = true; }));
            this.checkBox3.Enabled = true;
            this.downloading = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel2.Show();
            panel1.Hide();
            if (downloading == true) button3.Enabled = false;
            else button3.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Show();
            panel2.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string path;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;
                dialog.SelectedPath = javahome;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    javahome = path;
                }
                else
                {
                    path = javahome;
                }
            }
            Properties.Settings.Default.javapath = javahome;
            Properties.Settings.Default.Save();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            response = ut.sendrequest(textBox1.Text, textBox2.Text);

            if (response != null && response.error == null)
            {
                logged = true;

                Properties.Settings.Default.logged = logged;
                Properties.Settings.Default.Save();

                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button7.Visible = false;//Exchane button 7
                button8.Visible = true; //abd button 8 coords

                label7.Text = "Hello " + response.selectedProfile.name;
                label7.Visible = true;

                button1.Visible = true;
            }
            else
            {
                logged = false;

                Properties.Settings.Default.logged = logged;
                Properties.Settings.Default.Save();

                button1.Visible = false;
                MessageBox.Show("Cant't connect to authentication server or login/password is wrong");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ut.logout(textBox1.Text, textBox2.Text);

            logged = false;

            Properties.Settings.Default.logged = logged;
            Properties.Settings.Default.Save();

            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button8.Visible = false;//Exchane button 7
            button7.Visible = true; //abd button 8 coords

            label7.Text = "Hello ";
            label7.Visible = false;

            button1.Visible = false;
        }

        public void SetProgress(double value)
        {
            buffer = buffer + value;
            double percent = 100 - (100 * (fsize - buffer) / fsize);
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new MethodInvoker(delegate()
                {
                    progressBar1.Value = Convert.ToInt32(percent);
                }));
            }
            else
            {
                progressBar1.Value = Convert.ToInt32(buffer);
            }
        }

        public static void progress(double value, Form1 oForm1)
        {
            oForm1.SetProgress(value);
        }
    }



    /// <summary>
    ///                             UTILITS CLASS
    /// </summary>
    /// 


    public class utils
    {

        private Form1 oForm1;

        private static string assetsversion = "";
        public static bool debug;


        public void setform(Form1 form1)
        {
            oForm1 = form1;
        }

        public string getjavapath()  // Путь установки java
        {
            string javapath = null;
            string jdkKey = "SOFTWARE\\JavaSoft\\Java Development Kit";
            string jreKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
            bool is64bit = System.Environment.Is64BitOperatingSystem;
            RegistryKey basejdk, basejre;

            if (is64bit == true)
            {
                basejdk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(jdkKey);
                basejre = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(jreKey);
            }
            else
            {
                basejdk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(jdkKey);
                basejre = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(jreKey);
            }
            if (basejdk != null)
            {
                using (basejdk)
                {
                    String currentVersion = basejdk.GetValue("CurrentVersion").ToString();
                    using (var homeKey = basejdk.OpenSubKey(currentVersion))
                        javapath = homeKey.GetValue("JavaHome").ToString();
                }
            }
            else if (basejre != null)
            {
                String currentVersion = basejre.GetValue("CurrentVersion").ToString();
                using (var homeKey = basejre.OpenSubKey(currentVersion))
                    javapath = homeKey.GetValue("JavaHome").ToString();
            }
            return javapath;
        }

        public string[] installedver(string mdir)  // Список установленных версий
        {
            string versions = "{0}\\versions\\";
            versions = string.Format(versions, mdir);
            if (Directory.Exists(versions))
            {

                List<string> mver = new List<string>();
                DirectoryInfo verpath = new DirectoryInfo(versions);
                FileInfo[] vers = verpath.GetFiles("*.jar", SearchOption.AllDirectories);
                foreach (FileInfo file in vers)
                {
                    string folname = file.Name.Replace(".jar", "");
                    string jsonname = file.Name.Replace("jar", "json");
                    if ((file.DirectoryName.Equals(versions + folname)) && (File.Exists(versions + folname + "\\" + jsonname)))
                    {
                        mver.Add(folname);
                    }
                }
                return mver.ToArray();
            }
            else return null;
        }

        public Version[] getverlist(string mdir)   //Получить список версий из интернета 
        {
            string jsonurl = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json";

            WebClient client;
            Stream checknet;

            try
            {
                client = new WebClient();
                checknet = client.OpenRead(jsonurl);
                checknet.Close();
            }
            catch
            {
                if (debug == true) MessageBox.Show("Can't get versions list");
                return null;
            }

            bool fexist = File.Exists(mdir + "\\versions.json");
            if (fexist == false)
                client.DownloadFile(jsonurl, mdir + "\\versions.json");


            McVersion mcversions = JsonConvert.DeserializeObject<McVersion>(File.ReadAllText(mdir + "\\versions.json"));
            List<Version> allver = mcversions.versions;
            List<Version> release = new List<Version>();

            foreach (var item in allver)
            {
                if (item.type == "release")
                {
                    release.Add(item);
                }
            }

            release.Reverse();
            return release.ToArray();
        }

        public class Version
        {
            public string id { get; set; }
            public string type { get; set; }
        }

        public class McVersion
        {
            public List<Version> versions { get; set; }
        }

        public void downloadver(string ver, string mdir)   // Скачать jar и json версии, add cache check
        {
            string verjson = "http://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/" + ver + ".json";
            string verget = "http://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/" + ver + ".jar";
            WebClient jsondown = new WebClient();
            WebClient verdown = new WebClient();
            System.IO.Directory.CreateDirectory(mdir + "\\versions\\" + ver);
            bool fexist = File.Exists(mdir + "\\versions\\" + ver);
            if (fexist == true)
            {
                File.Delete(mdir + "\\versions\\" + ver + ".json");
                File.Delete(mdir + "\\versions\\" + ver + ".jar");
            }

            try
            {
                jsondown.DownloadFile(verjson, mdir + "\\versions\\" + ver + "\\" + ver + ".json");
                FileInfo f = new FileInfo(mdir + "\\versions\\" + ver + "\\" + ver + ".json");
                Form1.progress(f.Length, oForm1);
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Can't download " + ver + ".json");
            }

            try
            {
                verdown.DownloadFile(verget, mdir + "\\versions\\" + ver + "\\" + ver + ".jar");
                FileInfo f = new FileInfo(mdir + "\\versions\\" + ver + "\\" + ver + ".jar");
                Form1.progress(f.Length, oForm1);
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Can't download " + ver + ".jar");
            }
        }

        public class Natives
        {
            public string windows { get; set; }
        }
        public class Os
        {
            public string name { get; set; }
        }

        public class Rule
        {
            public string action { get; set; }
            public Os os { get; set; }
        }

        public class Library
        {
            public string name { get; set; }
            public string url { get; set; }
            public List<Rule> rules { get; set; }
            public Natives natives { get; set; }
        }

        public class Libraries
        {
            public string mainClass { get; set; }
            public string minecraftArguments { get; set; }
            public string assets { get; set; }
            public List<Library> libraries { get; set; }
        }

        public void donwlibs(string vers, string mdir)  // Скачать библиотеки, add cache check
        {
            Libraries libs = JsonConvert.DeserializeObject<Libraries>(File.ReadAllText(mdir + "\\versions\\" + vers + "\\" + vers + ".json"));
            mdir = mdir + "\\libraries\\";

            if (Directory.Exists(mdir + "natives"))
            {
                Directory.Delete(mdir + "natives", true);
            }
            else
            {
                Directory.CreateDirectory(mdir + "natives");
            }

            foreach (var item in libs.libraries)
            {
                bool osx = false;
                if (item.rules != null)
                {
                    foreach (var rul in item.rules)
                    {
                        if ((rul.action != null) && (rul.action == "allow"))
                        {
                            if ((rul.os != null) && (rul.os.name == "osx"))
                            {
                                osx = true;
                            }

                        }
                    }
                }
                if (osx == true)
                {
                    continue;
                }

                string libr = "";
                string url = "";
                string fname = "";
                string forge = "";
                string chname = "";
                int j = 0;

                for (int i = 0; i < item.name.Length; i++)
                {
                    if (item.name[i] == ':')
                    {
                        libr = libr + "\\";
                        i++;
                        j = i;
                        break;
                    }
                    if (item.name[i] == '.')
                    {
                        libr = libr + "\\";
                    }
                    else
                    {
                        libr = libr + item.name[i];
                    }
                }

                for (int k = j; k < item.name.Length; k++)
                {
                    if (item.name[k] == ':')
                    {
                        fname = fname + "-";
                        libr = libr + "\\";
                    }
                    else
                    {
                        fname = fname + item.name[k];
                        libr = libr + item.name[k];
                    }
                }

                if ((item.natives != null) && (item.natives.windows != null))
                {
                    fname = fname + "-natives-windows";
                    if (item.natives.windows.Contains("${arch}"))
                    {
                        bool is64bit = System.Environment.Is64BitOperatingSystem;
                        if (is64bit == true)
                        {
                            fname = fname + "-64";
                        }
                        else
                        {
                            fname = fname + "-32";
                        }
                    }
                }
                if (item.name.Contains("forge"))
                {
                    forge = fname + ".jar";
                    fname = fname + "-universal";
                    chname = forge;
                }
                else
                {
                    chname = fname + ".jar";
                }
                fname = fname + ".jar";
                url = libr + '/' + fname;
                url = url.Replace("\\", "/");
                bool fexist = File.Exists(mdir + libr + "\\" + chname);

                if (fexist == false)
                {

                    try
                    {
                        if ((item.natives == null) && (item.url == null))
                        {
                            string getlib = "https://libraries.minecraft.net/" + url;
                            WebClient libdown = new WebClient();
                            System.IO.Directory.CreateDirectory(mdir + libr);
                            libdown.DownloadFile(getlib, mdir + libr + "\\" + Path.GetFileName(getlib));

                            FileInfo f = new FileInfo(mdir + libr + "\\" + Path.GetFileName(getlib));
                            Form1.progress(f.Length, oForm1);
                        }
                        else if ((item.natives != null) && (item.natives.windows != null) && (item.url == null))
                        {
                            string getlib = "https://libraries.minecraft.net/" + url;
                            WebClient libdown = new WebClient();
                            System.IO.Directory.CreateDirectory(mdir + libr);
                            libdown.DownloadFile(getlib, mdir + libr + "\\" + Path.GetFileName(getlib));

                            FileInfo f = new FileInfo(mdir + libr + "\\" + Path.GetFileName(getlib));
                            Form1.progress(f.Length, oForm1);
                        }
                        else if (item.url != null)
                        {
                            string getlib = "";
                            if (item.name.Contains("scala"))
                            {
                                getlib = "http://repo1.maven.org/maven2/" + url;
                            }
                            else
                            {
                                getlib = item.url + url;
                            }
                            WebClient libdown = new WebClient();
                            System.IO.Directory.CreateDirectory(mdir + libr);
                            libdown.DownloadFile(getlib, mdir + libr + "\\" + Path.GetFileName(getlib));

                            FileInfo f = new FileInfo(mdir + libr + "\\" + Path.GetFileName(getlib));
                            Form1.progress(f.Length, oForm1);

                            if (item.name.Contains("forge"))
                            {
                                File.Move(mdir + libr + "\\" + Path.GetFileName(getlib), mdir + libr + "\\" + forge);
                            }
                        }
                    }
                    catch
                    {
                        if (debug == true)
                            MessageBox.Show("Cant download file " + fname);
                    }
                }
            }

        }

        public string extractlibs(string vers, string mdir)  // Извлечть и получить путь библиотек
        {
            Libraries libs = JsonConvert.DeserializeObject<Libraries>(File.ReadAllText(mdir + "\\versions\\" + vers + "\\" + vers + ".json"));
            string cp = "";
            mdir = mdir + "\\libraries\\";

            if (Directory.Exists(mdir + "natives"))
            {
                Directory.Delete(mdir + "natives", true);
            }
            else
            {
                Directory.CreateDirectory(mdir + "natives");
            }

            foreach (var item in libs.libraries)
            {
                bool osx = false;
                if (item.rules != null)
                {
                    foreach (var rul in item.rules)
                    {
                        if ((rul.action != null) && (rul.action == "allow"))
                        {
                            if ((rul.os != null) && (rul.os.name == "osx"))
                            {
                                osx = true;
                            }

                        }
                    }
                }
                if (osx == true)
                {
                    continue;
                }

                string libr = "";
                string fname = "";
                string forge = "";
                int j = 0;

                for (int i = 0; i < item.name.Length; i++)
                {
                    if (item.name[i] == ':')
                    {
                        libr = libr + "\\";
                        i++;
                        j = i;
                        break;
                    }
                    if (item.name[i] == '.')
                    {
                        libr = libr + "\\";
                    }
                    else
                    {
                        libr = libr + item.name[i];
                    }
                }

                for (int k = j; k < item.name.Length; k++)
                {
                    if (item.name[k] == ':')
                    {
                        fname = fname + "-";
                        libr = libr + "\\";
                    }
                    else
                    {
                        fname = fname + item.name[k];
                        libr = libr + item.name[k];
                    }
                }

                if ((item.natives != null) && (item.natives.windows != null))
                {
                    fname = fname + "-natives-windows";
                    if (item.natives.windows.Contains("${arch}"))
                    {
                        bool is64bit = System.Environment.Is64BitOperatingSystem;
                        if (is64bit == true)
                        {
                            fname = fname + "-64";
                        }
                        else
                        {
                            fname = fname + "-32";
                        }
                    }
                }
                if (item.name.Contains("forge"))
                {
                    forge = fname + ".jar";
                    fname = fname + "-universal";
                }
                fname = fname + ".jar";


                if (item.name.Contains("forge"))
                {
                    cp = cp + mdir + libr + "\\" + forge + ";";
                }
                else
                {
                    cp = cp + mdir + libr + "\\" + fname + ";";
                }
                if (((item.name.Contains("org.lwjgl.lwjgl:lwjgl-platform")) && (item.natives.windows != null)) || ((item.name.Contains("net.java.jinput:jinput-platform")) && (item.natives.windows != null)))
                {
                    string zipPath = mdir + libr + "\\" + fname;
                    string extractPath = mdir + "natives";
                    try
                    {
                        ZipFile.ExtractToDirectory(zipPath, extractPath);
                        if (Directory.Exists(extractPath + "\\META-INF"))
                        {
                            Directory.Delete(extractPath + "\\META-INF", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (debug == true)
                            MessageBox.Show(ex.Message);
                    }
                    if (!cp.Contains(" -Djava.library.path="))
                    {
                        cp = " -Djava.library.path=" + extractPath + " -cp " + cp;
                    }
                }
            }
            mdir = mdir.Replace("libraries", "versions");
            cp = cp + mdir + vers + "\\" + vers + ".jar";
            cp = cp + " " + libs.mainClass + " " + libs.minecraftArguments;
            mdir = mdir.Replace("\\versions", "");
            char a = '"';
            if (libs.assets != null)
            {
                cp = cp.Replace("${assets_index_name}", libs.assets);
                assetsversion = libs.assets;
                cp = cp.Replace("${assets_root}", a + mdir + "\\assets" + a);
            }
            else
            {
                assetsversion = "old";
                cp = cp.Replace("${game_assets}", a + mdir + "\\assets\\virtual\\legacy" + a);
            }
            return cp;
        }

        public class Assets
        {
            public Dictionary<string, Objects> objects { get; set; }
        }

        public class Objects
        {
            public string hash { get; set; }
            public int size { get; set; }
        }

        public void getassets(string mdir)    // Скачать звуки и тд.
        {
            if (assetsversion == "old")
            {
                assetsversion = "0";
            }
            string assetsjson = "";
            string format = "";
            string names = "";
            for (int i = 0; i < assetsversion.Length; i++)
                if (assetsversion[i] != '.') assetsjson = assetsjson + assetsversion[i];
            int version = Convert.ToInt32(assetsjson);

            Assets assets;

            if (version < 172)
            {
                format = assetsjson = "legacy.json";
                Directory.CreateDirectory(mdir + "\\assets\\virtual\\legacy\\indexes\\");
                WebClient assetsjsondown = new WebClient();

                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;
                assetsjsondown.DownloadFile(assetsjson, mdir + "\\assets\\virtual\\legacy\\indexes\\" + format);
                assets = JsonConvert.DeserializeObject<Assets>(File.ReadAllText(mdir + "\\assets\\virtual\\legacy\\indexes\\" + "legacy.json"));

                FileInfo f = new FileInfo(mdir + "\\assets\\virtual\\legacy\\indexes\\" + format);
                Form1.progress(f.Length, oForm1);
            }
            else
            {
                format = assetsjson = assetsversion + ".json";
                Directory.CreateDirectory(mdir + "\\assets\\indexes\\");

                WebClient assetsjsondown = new WebClient();

                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;
                assetsjsondown.DownloadFile(assetsjson, mdir + "\\assets\\indexes\\" + format);
                assets = JsonConvert.DeserializeObject<Assets>(File.ReadAllText(mdir + "\\assets\\indexes\\" + format));

                FileInfo f = new FileInfo(mdir + "\\assets\\indexes\\" + format);
                Form1.progress(f.Length, oForm1);
            }

            foreach (KeyValuePair<string, Objects> i in assets.objects)
            {
                string hash = Convert.ToString(i.Value.hash);
                int size = Convert.ToInt32(i.Value.size);
                bool hashok = false;
                bool fexist = false;
                string fSHA1 = "";

                WebClient assetsdown = new WebClient();
                names = i.Key.ToString();
                if (names.Contains("/"))
                {
                    if (names.LastIndexOf("/") > 0)
                        names = names.Substring(0, names.LastIndexOf("/"));
                    names = names.Replace("/", "\\");
                    names = "\\" + names;
                }
                else
                {
                    names = null;
                }

                if (version < 172)
                {
                    fexist = File.Exists(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1)))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }
                else
                {
                    fexist = File.Exists(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }

                if ((fexist == false) || (fexist == true && hashok == false))
                {
                    if (!Directory.Exists(mdir + "\\assets\\virtual\\legacy\\" + names) & version < 172)
                    {
                        Directory.CreateDirectory(mdir + "\\assets\\virtual\\legacy\\" + names);
                    }

                    else
                    {
                        if (!Directory.Exists(mdir + "\\assets\\objects\\" + hash.Substring(0, 2)) & version >= 172)
                            Directory.CreateDirectory(mdir + "\\assets\\objects\\" + hash.Substring(0, 2));
                    }

                    try
                    {
                        if (fexist == true)
                        {
                            if (version < 172)
                                File.Delete(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));
                            else
                                File.Delete(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                        }
                        if (version < 172)
                        {
                            assetsdown.DownloadFile("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash, mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));

                            FileInfo f = new FileInfo(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));
                            Form1.progress(f.Length, oForm1);
                        }
                        else
                        {
                            assetsdown.DownloadFile("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash, mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);

                            FileInfo f = new FileInfo(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                            Form1.progress(f.Length, oForm1);
                        }
                    }
                    catch
                    {
                        if (debug == true)
                            MessageBox.Show("Can't download " + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/")));
                    }
                }
            }

        }

        public long getsize(string vers, string mdir)
        {
            long fsize = 0;

            string verjson = "http://s3.amazonaws.com/Minecraft.Download/versions/" + vers + "/" + vers + ".json";
            string verget = "http://s3.amazonaws.com/Minecraft.Download/versions/" + vers + "/" + vers + ".jar";

            try
            {
                WebRequest jsondown = HttpWebRequest.Create(verjson);
                jsondown.Method = "HEAD";
                using (System.Net.WebResponse resp = jsondown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Can't get size of " + vers + ".json");
            }

            try
            {
                WebRequest verdown = HttpWebRequest.Create(verget);
                verdown.Method = "HEAD";
                using (System.Net.WebResponse resp = verdown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Can't get size of " + vers + ".jar");
            }

            WebClient client = new WebClient();
            StreamReader reader;
            String content = "";

            try
            {
                Stream streams = client.OpenRead(verjson);
                reader = new StreamReader(streams);
                content = reader.ReadToEnd();
                streams.Close();
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Cant get sizes of libraries");
            }

            Libraries libs = JsonConvert.DeserializeObject<Libraries>(content);

            foreach (var item in libs.libraries)
            {
                bool osx = false;
                if (item.rules != null)
                {
                    foreach (var rul in item.rules)
                    {
                        if ((rul.action != null) && (rul.action == "allow"))
                        {
                            if ((rul.os != null) && (rul.os.name == "osx"))
                            {
                                osx = true;
                            }

                        }
                    }
                }
                if (osx == true)
                {
                    continue;
                }

                string libr = "";
                string url = "";
                string fname = "";
                string forge = "";
                string chname = "";
                int j = 0;

                for (int i = 0; i < item.name.Length; i++)
                {
                    if (item.name[i] == ':')
                    {
                        libr = libr + "\\";
                        i++;
                        j = i;
                        break;
                    }
                    if (item.name[i] == '.')
                    {
                        libr = libr + "\\";
                    }
                    else
                    {
                        libr = libr + item.name[i];
                    }
                }

                for (int k = j; k < item.name.Length; k++)
                {
                    if (item.name[k] == ':')
                    {
                        fname = fname + "-";
                        libr = libr + "\\";
                    }
                    else
                    {
                        fname = fname + item.name[k];
                        libr = libr + item.name[k];
                    }
                }

                if ((item.natives != null) && (item.natives.windows != null))
                {
                    fname = fname + "-natives-windows";
                    if (item.natives.windows.Contains("${arch}"))
                    {
                        bool is64bit = System.Environment.Is64BitOperatingSystem;
                        if (is64bit == true)
                        {
                            fname = fname + "-64";
                        }
                        else
                        {
                            fname = fname + "-32";
                        }
                    }
                }
                if (item.name.Contains("forge"))
                {
                    forge = fname + ".jar";
                    fname = fname + "-universal";
                    chname = forge;
                }
                else
                {
                    chname = fname + ".jar";
                }
                fname = fname + ".jar";
                url = libr + '/' + fname;
                url = url.Replace("\\", "/");

                try
                {
                    if ((item.natives == null) && (item.url == null))
                    {
                        string getlib = "https://libraries.minecraft.net/" + url;
                        WebRequest libdown = HttpWebRequest.Create(getlib);
                        libdown.Method = "HEAD";
                        using (System.Net.WebResponse resp = libdown.GetResponse())
                        {
                            long ContentLength;
                            if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                            {
                                fsize += ContentLength;
                            }
                        }
                    }
                    else if ((item.natives != null) && (item.natives.windows != null) && (item.url == null))
                    {
                        string getlib = "https://libraries.minecraft.net/" + url;
                        WebRequest libdown = HttpWebRequest.Create(getlib);
                        libdown.Method = "HEAD";
                        using (System.Net.WebResponse resp = libdown.GetResponse())
                        {
                            long ContentLength;
                            if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                            {
                                fsize += ContentLength;
                            }
                        }
                    }
                    else if (item.url != null)
                    {
                        string getlib = "";
                        if (item.name.Contains("scala"))
                        {
                            getlib = "http://repo1.maven.org/maven2/" + url;
                        }
                        else
                        {
                            getlib = item.url + url;
                        }
                        WebRequest libdown = HttpWebRequest.Create(getlib);
                        libdown.Method = "HEAD";
                        using (System.Net.WebResponse resp = libdown.GetResponse())
                        {
                            long ContentLength;
                            if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                            {
                                fsize += ContentLength;
                            }
                        }
                    }
                    if (libs.assets != null)
                    {
                        assetsversion = libs.assets;
                    }
                    else
                    {
                        assetsversion = "old";
                    }
                }
                catch
                {
                    if (debug == true)
                        MessageBox.Show("Can't get size of " + fname);
                }
            }

            if (assetsversion == "old")
            {
                assetsversion = "0";
            }
            string assetsjson = "";
            string format = "";
            string names = "";
            for (int i = 0; i < assetsversion.Length; i++)
                if (assetsversion[i] != '.') assetsjson = assetsjson + assetsversion[i];
            int version = Convert.ToInt32(assetsjson);


            Assets assets;

            if (version < 172)
            {
                format = assetsjson = "legacy.json";
                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;

                Stream stream = client.OpenRead(assetsjson);
                reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                reader.Close();
                assets = JsonConvert.DeserializeObject<Assets>(content);

                WebRequest assetsjsondown = HttpWebRequest.Create(assetsjson);
                assetsjsondown.Method = "HEAD";
                using (System.Net.WebResponse resp = assetsjsondown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }
            else
            {
                format = assetsjson = assetsversion + ".json";
                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;

                Stream stream = client.OpenRead(assetsjson);
                reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                reader.Close();
                assets = JsonConvert.DeserializeObject<Assets>(content);

                WebRequest assetsjsondown = HttpWebRequest.Create(assetsjson);
                assetsjsondown.Method = "HEAD";
                using (System.Net.WebResponse resp = assetsjsondown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }

            foreach (KeyValuePair<string, Objects> i in assets.objects)
            {
                string hash = Convert.ToString(i.Value.hash);
                int size = Convert.ToInt32(i.Value.size);
                bool hashok = false;
                bool fexist = false;
                string fSHA1 = "";

                names = i.Key.ToString();
                if (names.Contains("/"))
                {
                    if (names.LastIndexOf("/") > 0)
                        names = names.Substring(0, names.LastIndexOf("/"));
                    names = names.Replace("/", "\\");
                    names = "\\" + names;
                }
                else
                {
                    names = null;
                }

                if (version < 172)
                {
                    fexist = File.Exists(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1)))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }
                else
                {
                    fexist = File.Exists(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }

                if ((fexist == false) || (fexist == true && hashok == false))
                {
                    try
                    {
                        if (version < 172)
                        {
                            WebRequest assetdown = HttpWebRequest.Create("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash);
                            assetdown.Method = "HEAD";
                            using (System.Net.WebResponse resp = assetdown.GetResponse())
                            {
                                long ContentLength;
                                if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                                {
                                    fsize += ContentLength;
                                }
                            }
                        }
                        else
                        {
                            WebRequest assetdown = HttpWebRequest.Create("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash);
                            assetdown.Method = "HEAD";
                            using (System.Net.WebResponse resp = assetdown.GetResponse())
                            {
                                long ContentLength;
                                if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                                {
                                    fsize += ContentLength;
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (debug == true)
                            MessageBox.Show("Can't get size of " + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/")));
                    }
                }
            }

            return fsize;
        } //Old Normal

        public void AS1getsize(string vers, string mdir, out long size)
        {
            long fsize = 0;

            string verjson = "http://s3.amazonaws.com/Minecraft.Download/versions/" + vers + "/" + vers + ".json";
            string verget = "http://s3.amazonaws.com/Minecraft.Download/versions/" + vers + "/" + vers + ".jar";

            try
            {
                WebRequest jsondown = HttpWebRequest.Create(verjson);
                jsondown.Method = "HEAD";
                using (System.Net.WebResponse resp = jsondown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Can't get size of " + vers + ".json");
            }

            try
            {
                WebRequest verdown = HttpWebRequest.Create(verget);
                verdown.Method = "HEAD";
                using (System.Net.WebResponse resp = verdown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Can't get size of " + vers + ".jar");
            }

            WebClient client = new WebClient();
            StreamReader reader;
            String content = "";

            try
            {
                Stream streams = client.OpenRead(verjson);
                reader = new StreamReader(streams);
                content = reader.ReadToEnd();
                streams.Close();
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Cant get sizes of libraries");
            }

            Libraries libs = JsonConvert.DeserializeObject<Libraries>(content);

            foreach (var item in libs.libraries)
            {
                bool osx = false;
                if (item.rules != null)
                {
                    foreach (var rul in item.rules)
                    {
                        if ((rul.action != null) && (rul.action == "allow"))
                        {
                            if ((rul.os != null) && (rul.os.name == "osx"))
                            {
                                osx = true;
                            }

                        }
                    }
                }
                if (osx == true)
                {
                    continue;
                }

                string libr = "";
                string url = "";
                string fname = "";
                string forge = "";
                string chname = "";
                int j = 0;

                for (int i = 0; i < item.name.Length; i++)
                {
                    if (item.name[i] == ':')
                    {
                        libr = libr + "\\";
                        i++;
                        j = i;
                        break;
                    }
                    if (item.name[i] == '.')
                    {
                        libr = libr + "\\";
                    }
                    else
                    {
                        libr = libr + item.name[i];
                    }
                }

                for (int k = j; k < item.name.Length; k++)
                {
                    if (item.name[k] == ':')
                    {
                        fname = fname + "-";
                        libr = libr + "\\";
                    }
                    else
                    {
                        fname = fname + item.name[k];
                        libr = libr + item.name[k];
                    }
                }

                if ((item.natives != null) && (item.natives.windows != null))
                {
                    fname = fname + "-natives-windows";
                    if (item.natives.windows.Contains("${arch}"))
                    {
                        bool is64bit = System.Environment.Is64BitOperatingSystem;
                        if (is64bit == true)
                        {
                            fname = fname + "-64";
                        }
                        else
                        {
                            fname = fname + "-32";
                        }
                    }
                }
                if (item.name.Contains("forge"))
                {
                    forge = fname + ".jar";
                    fname = fname + "-universal";
                    chname = forge;
                }
                else
                {
                    chname = fname + ".jar";
                }
                fname = fname + ".jar";
                url = libr + '/' + fname;
                url = url.Replace("\\", "/");

                try
                {
                    if ((item.natives == null) && (item.url == null))
                    {
                        string getlib = "https://libraries.minecraft.net/" + url;
                        WebRequest libdown = HttpWebRequest.Create(getlib);
                        libdown.Method = "HEAD";
                        using (System.Net.WebResponse resp = libdown.GetResponse())
                        {
                            long ContentLength;
                            if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                            {
                                fsize += ContentLength;
                            }
                        }
                    }
                    else if ((item.natives != null) && (item.natives.windows != null) && (item.url == null))
                    {
                        string getlib = "https://libraries.minecraft.net/" + url;
                        WebRequest libdown = HttpWebRequest.Create(getlib);
                        libdown.Method = "HEAD";
                        using (System.Net.WebResponse resp = libdown.GetResponse())
                        {
                            long ContentLength;
                            if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                            {
                                fsize += ContentLength;
                            }
                        }
                    }
                    else if (item.url != null)
                    {
                        string getlib = "";
                        if (item.name.Contains("scala"))
                        {
                            getlib = "http://repo1.maven.org/maven2/" + url;
                        }
                        else
                        {
                            getlib = item.url + url;
                        }
                        WebRequest libdown = HttpWebRequest.Create(getlib);
                        libdown.Method = "HEAD";
                        using (System.Net.WebResponse resp = libdown.GetResponse())
                        {
                            long ContentLength;
                            if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                            {
                                fsize += ContentLength;
                            }
                        }
                    }
                }
                catch
                {
                    if (debug == true)
                        MessageBox.Show("Can't get size of " + fname);
                }
            }


            size = fsize;
        } // New normal, must be async

        public void AS2getsize(string vers, string mdir, out long sizes)
        {
            long fsize = 0;

            string verjson = "http://s3.amazonaws.com/Minecraft.Download/versions/" + vers + "/" + vers + ".json";

            WebClient client = new WebClient();
            StreamReader reader;
            String content = "";

            try
            {
                Stream streams = client.OpenRead(verjson);
                reader = new StreamReader(streams);
                content = reader.ReadToEnd();
                streams.Close();
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Cant get sizes of libraries");
            }

            Libraries libs = JsonConvert.DeserializeObject<Libraries>(content);

            if (libs.assets != null)
            {
                assetsversion = libs.assets;
            }
            else
            {
                assetsversion = "old";
            }


            if (assetsversion == "old")
            {
                assetsversion = "0";
            }
            string assetsjson = "";
            string format = "";
            string names = "";
            for (int i = 0; i < assetsversion.Length; i++)
                if (assetsversion[i] != '.') assetsjson = assetsjson + assetsversion[i];
            int version = Convert.ToInt32(assetsjson);


            Assets assets;

            if (version < 172)
            {
                format = assetsjson = "legacy.json";
                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;

                Stream stream = client.OpenRead(assetsjson);
                reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                reader.Close();
                assets = JsonConvert.DeserializeObject<Assets>(content);

                WebRequest assetsjsondown = HttpWebRequest.Create(assetsjson);
                assetsjsondown.Method = "HEAD";
                using (System.Net.WebResponse resp = assetsjsondown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }
            else
            {
                format = assetsjson = assetsversion + ".json";
                assetsjson = "http://s3.amazonaws.com/Minecraft.Download/indexes/" + assetsjson;

                Stream stream = client.OpenRead(assetsjson);
                reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                reader.Close();
                assets = JsonConvert.DeserializeObject<Assets>(content);

                WebRequest assetsjsondown = HttpWebRequest.Create(assetsjson);
                assetsjsondown.Method = "HEAD";
                using (System.Net.WebResponse resp = assetsjsondown.GetResponse())
                {
                    long ContentLength;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        fsize += ContentLength;
                    }
                }
            }

            foreach (KeyValuePair<string, Objects> i in assets.objects)
            {
                string hash = Convert.ToString(i.Value.hash);
                int size = Convert.ToInt32(i.Value.size);
                bool hashok = false;
                bool fexist = false;
                string fSHA1 = "";

                names = i.Key.ToString();
                if (names.Contains("/"))
                {
                    if (names.LastIndexOf("/") > 0)
                        names = names.Substring(0, names.LastIndexOf("/"));
                    names = names.Replace("/", "\\");
                    names = "\\" + names;
                }
                else
                {
                    names = null;
                }

                if (version < 172)
                {
                    fexist = File.Exists(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1));
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\virtual\\legacy" + names + "\\" + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/") + 1)))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }
                else
                {
                    fexist = File.Exists(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash);
                    if (fexist == true)
                    {
                        using (FileStream stream = File.OpenRead(mdir + "\\assets\\objects\\" + hash.Substring(0, 2) + "\\" + hash))
                        {
                            SHA1Managed sha = new SHA1Managed();
                            byte[] checksum = sha.ComputeHash(stream);
                            fSHA1 = BitConverter.ToString(checksum).Replace("-", string.Empty);
                            fSHA1 = fSHA1.ToLower();
                        }
                        if (fSHA1 == hash) hashok = true;
                    }
                }

                if ((fexist == false) || (fexist == true && hashok == false))
                {
                    try
                    {
                        if (version < 172)
                        {
                            WebRequest assetdown = HttpWebRequest.Create("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash);
                            assetdown.Method = "HEAD";
                            using (System.Net.WebResponse resp = assetdown.GetResponse())
                            {
                                long ContentLength;
                                if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                                {
                                    fsize += ContentLength;
                                }
                            }
                        }
                        else
                        {
                            WebRequest assetdown = HttpWebRequest.Create("http://resources.download.minecraft.net/" + hash.Substring(0, 2) + "/" + hash);
                            assetdown.Method = "HEAD";
                            using (System.Net.WebResponse resp = assetdown.GetResponse())
                            {
                                long ContentLength;
                                if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                                {
                                    fsize += ContentLength;
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (debug == true)
                            MessageBox.Show("Can't get size of " + i.Key.ToString().Substring(i.Key.ToString().LastIndexOf("/")));
                    }
                }
            }

            sizes = fsize;
        } // New normal, must be async

        public class Authentication
        {
            public string username { get; set; }
        }

        public class Profiles
        {
            public Authentication authentication { get; set; }
            public string name { get; set; }
            public string lastVersionId { get; set; }
            public string javaArgs { get; set; }
        }

        public class LaunchProfiles
        {
            public Dictionary<string, Profiles> profiles { get; set; }
            public string selectedProfile { get; set; }
            public string clientToken { get; set; }
        }

        public void launprof(string name, string version, string client, string mdir)  // Make like mojang launcher
        {
            Authentication auth = new Authentication();
            auth.username = name;

            Profiles pr = new Profiles();
            pr.authentication = auth;
            pr.javaArgs = "";
            pr.lastVersionId = client;
            pr.name = client;

            Dictionary<string, Profiles> pro = new Dictionary<string, Profiles>();
            pro.Add(client, pr);

            LaunchProfiles prof = new LaunchProfiles();
            prof.clientToken = "";
            prof.selectedProfile = client;
            prof.profiles = pro;

            try
            {
                string output = JsonConvert.SerializeObject(prof, Formatting.Indented);

                bool fexist = File.Exists(mdir + "\\launcher_profiles.json");
                if (fexist == true)
                    File.Delete(mdir + "\\launcher_profiles.json");

                StreamWriter sr = new StreamWriter(mdir + "\\launcher_profiles.json");
                sr.WriteLine(output);
                sr.Close();
            }
            catch
            {
                if (debug == true)
                    MessageBox.Show("Can't create launcher_profile.json");
            }
        }

        public class Agent
        {
            public string name { get; set; }
            public string version { get; set; }
        }

        public class SendRequest
        {
            public Agent agent { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string clientToken { get; set; }
        }


        public class AvailableProfile
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class SelectedProfile
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class GetRequest
        {
            public string accessToken { get; set; }
            public string clientToken { get; set; }
            public string error { get; set; }
            public string errorMessage { get; set; }
            public List<AvailableProfile> availableProfile { get; set; }
            public SelectedProfile selectedProfile { get; set; }
        }

        public GetRequest sendrequest(string name, string password)
        {
            SendRequest root = new SendRequest();
            Agent ag = new Agent();

            ag.name = "Minecraft";
            ag.version = "1";

            root.username = name;
            root.password = password;
            root.clientToken = getuuid();
            root.agent = ag;

            string send = JsonConvert.SerializeObject(root);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://authserver.mojang.com/authenticate");

            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] postBytes = Encoding.ASCII.GetBytes(send);

            Stream requestStream = request.GetRequestStream();

            Thread.Sleep(10);

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                GetRequest getreq = JsonConvert.DeserializeObject<GetRequest>(responseString);

                return getreq;
            }
            catch
            {
                return null;

            }
        }

        public class Logout
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public void logout(string name, string pass)
        {
            Logout lout = new Logout();
            lout.username = name;
            lout.password = pass;

            string send = JsonConvert.SerializeObject(lout);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://authserver.mojang.com/authenticate");

            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] postBytes = Encoding.ASCII.GetBytes(send);

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();
        }

        public string getuuid()
        {
            Guid g = Guid.NewGuid();

            string uuid = g.ToString();

            return uuid;
        }

    }
}
