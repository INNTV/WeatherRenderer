using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeatherRender.MyDBTableAdapters;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace WeatherRender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {


            timer1.Enabled = false;

            button1.ForeColor = Color.White;
            button1.Text = "Started";
            button1.BackColor = Color.Red;

            if (richTextBox1.Lines.Length > 500)
            {
                richTextBox1.Text = "";
            }


            richTextBox1.Text += "START: " + DateTime.Now.ToString()+" \n";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            Application.DoEvents();

            CONDUCTORTableAdapter Cond_Ta = new CONDUCTORTableAdapter();
            string NewRowId = Cond_Ta.Insert_Cond("conductor01",
                "AFTER MACHINE",
                "0",
                " پیش بینی آب و هوای شهرهای ایران **** " + string.Format("{0:00}", DateTime.Now.Hour) + ":" + string.Format("{0:00}", DateTime.Now.Minute),
                "پیش بینی آب و هوای شهرهای ایران  - اتوماتیک",
                "1",
                "1",
                System.Configuration.ConfigurationSettings.AppSettings["ConductorId"].Trim(),
                "0",
                "",
                "RENDERSRV").ToString();

          //  string NewRowId = "0";



            string CitiesRoot = System.Configuration.ConfigurationSettings.AppSettings["CitiesRoot"];
            string ConditionsSourceFolder = System.Configuration.ConfigurationSettings.AppSettings["ConditionsSourceFolder"];


            weather_TempTableAdapter W_Ta = new weather_TempTableAdapter();
            MyDB.weather_TempDataTable W_Dt = W_Ta.LoadCities();

            TxtObject TObj = new TxtObject();
            TObj.BrushGradientEndColor = "#ffffff";
            TObj.BrushGradientStartColor = "#ffffff";
            TObj.BrushWidth = 3;
            TObj.FntName = System.Configuration.ConfigurationSettings.AppSettings["FontName"];
            TObj.FntSize = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["FontSize"]);
            TObj.FntStyle = FontStyle.Regular;
            TObj.ForeColorCode = "#ffffff";
            TObj.GradientAngel = 0;
            TObj.ImgFormat = System.Drawing.Imaging.ImageFormat.Png;
            TObj.ImgHeight = 161;
            TObj.ImgWidth = 180;
            TObj.MarginX = 0;
            TObj.MarginY = 0;


            XmlDocument XCitiesDoc = new XmlDocument();
            string CitiesXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Cities.xml";

            if (File.Exists(CitiesXmlPath))
            {

                XCitiesDoc.Load(CitiesXmlPath);
                XmlNodeList CitiesLst = XCitiesDoc.GetElementsByTagName("Location");

                foreach (XmlNode Nd in CitiesLst)
                {
                    string CityId = Nd.Attributes["name"].Value;
                    foreach (DataRow item in W_Dt)
                    {
                        if (item["text"].ToString().Trim() == CityId)
                        {
                            TObj.Text = item["maxt"].ToString().Trim();
                            TObj.CompleteFileName = CitiesRoot + "\\" + Nd.Attributes["CityPath"].Value + "\\C - mAX.png";
                            Utilities.Txt2Img(TObj);


                            TObj.Text = item["mint"].ToString().Trim();
                            TObj.CompleteFileName = CitiesRoot + "\\" + Nd.Attributes["CityPath"].Value + "\\C - mIN.png";
                            Utilities.Txt2Img(TObj);

                            File.Copy(ConditionsSourceFolder + "\\" + item["img"].ToString().Trim() + ".avi",
                                CitiesRoot + "\\" + Nd.Attributes["CityPath"].Value + "\\7000.avi",
                                true);
                        }
                    }
                }

                //Start Render
                RenderObject RndObj = new RenderObject();
                RndObj.AeProjectPath = System.Configuration.ConfigurationSettings.AppSettings["AeProjectFile"];
                RndObj.AeRenderPath = System.Configuration.ConfigurationSettings.AppSettings["AeRenderPath"];
                RndObj.CompositionName = System.Configuration.ConfigurationSettings.AppSettings["Composition"];
                RndObj.DestDirectory = System.Configuration.ConfigurationSettings.AppSettings["OutputPath"];
                RndObj.DestFullFileName = System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"];
                try
                {
                    File.Delete(System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"]);
                }
                catch
                {

                }
              


                StreamReader reader = Utilities.Renderer(RndObj);
                int Lngth = richTextBox1.Text.Length;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    richTextBox1.Text = richTextBox1.Text.Remove(Lngth, richTextBox1.Text.Length - Lngth);
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();


                    richTextBox1.AppendText(line + " >> FROM : 2195 FRAMES");
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();


                }

                richTextBox1.Text = richTextBox1.Text.Remove(Lngth, richTextBox1.Text.Length - Lngth);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();


                string PlayOutFolderDate = string.Format("{0:0000}", DateTime.Now.Year) + string.Format("{0:00}", DateTime.Now.Month) + string.Format("{0:00}", DateTime.Now.Day);
                string PlayOutFolder = System.Configuration.ConfigurationSettings.AppSettings["PlayOutRoot"] + "\\" + PlayOutFolderDate;
                if (!Directory.Exists(PlayOutFolder + "\\THUMB"))
                {
                    Directory.CreateDirectory(PlayOutFolder + "\\THUMB");
                }
                if (!Directory.Exists(PlayOutFolder + "\\VIDEO"))
                {
                    Directory.CreateDirectory(PlayOutFolder + "\\VIDEO");
                }



                string PlyFileName = Cond_Ta.INSERT_PLAYOUT01("playout01",
                    NewRowId,
                    "1",
                    "00:01:26",
                    "S:\\PLAYOUT\\" + PlayOutFolderDate,
                    "1",
                    "1"
                    ).ToString();

                richTextBox1.Text += "COPY: " + PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi" + "\n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
                File.Copy(System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"], PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi");



                richTextBox1.Text += "COPY: " + PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".wav" + "\n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
                File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\w.wav", PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".wav");


                richTextBox1.Text += "COPY: " + PlayOutFolder + "\\THUMB\\" + PlyFileName + ".jpg" + "\n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
                File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\w.jpg", PlayOutFolder + "\\THUMB\\" + PlyFileName + ".jpg");



                Cond_Ta.Update_FileName("playout01", PlyFileName, PlyFileName);
                Cond_Ta.Update_Active("conductor01", NewRowId);


            }

            richTextBox1.Text += "END: " + DateTime.Now.ToString() + " \n";
            richTextBox1.Text += " ======================================== \n";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            Application.DoEvents();

            button1.ForeColor = Color.White;
            button1.Text = "Start";
            button1.BackColor = Color.Navy;

            timer1.Enabled = true;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            RenderTableAdapter Rn_Ta = new RenderTableAdapter();
            MyDB.RenderDataTable Rn_Dt = Rn_Ta.Select_Render_Q();
            if (Rn_Dt.Rows.Count == 1)
            {
                button1_Click(new object(), new EventArgs());
                Rn_Ta.Update_Render(int.Parse(Rn_Dt.Rows[0]["ID"].ToString()));
            }
           

            timer1.Enabled = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult Rs = MessageBox.Show("آیا از برنامه خارج می شوید", "خروج از برنامه",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Rs == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }   
        }


    }
}
