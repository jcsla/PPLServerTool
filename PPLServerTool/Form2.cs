using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace PPLServerTool
{
    public partial class Form2 : Form
    {
        Form1 form1;
        string fileName = "";
        int index;

        string productCode;

        const int UPDATE = 3;

        public Form2(Form1 form1)
        {
            InitializeComponent();

            this.form1 = form1;

            findIndex();
            initializeForm();
        }

        private void findIndex()
        {
            for (int i = 0; i < form1.getListView2().Items.Count; i++)
            {
                if (form1.getListView2().Items[i].Selected == true)
                    index = i;
            }
        }

        private void initializeForm()
        {
            product_name.Text = form1.getPPLDataList().getPPL(index).getProductName();
            brand_name.Text = form1.getPPLDataList().getPPL(index).getBrandName();
            link_name.Text = form1.getPPLDataList().getPPL(index).getStoreLink();
            price_name.Text = form1.getPPLDataList().getPPL(index).getPrice();
            int startTime = form1.getPPLDataList().getPPL(index).getStartTime();
            string startHour = getStartHour(startTime);
            string startMinute = getStartMinute(startTime);
            string startSecond = getStartSecond(startTime);
            start_hour.Text = startHour;
            start_minute.Text = startMinute;
            start_second.Text = startSecond;
            int endTime = form1.getPPLDataList().getPPL(index).getEndTime();
            string endHour = getStartHour(endTime);
            string endMinute = getStartMinute(endTime);
            string endSecond = getStartSecond(endTime);
            end_hour.Text = endHour;
            end_minute.Text = endMinute;
            end_second.Text = endSecond;
            pictureBox1.ImageLocation = form1.getPPLDataList().getPPL(index).getProductImage();

            productCode = form1.getPPLDataList().getPPL(index).getProductCode();
        }

        private string getStartHour(int time)
        {
            int hour = time / 3600;

            return hour.ToString();
        }

        private string getStartMinute(int time)
        {
            int minute = (time / 60) % 60;

            return minute.ToString();
        }

        private string getStartSecond(int time)
        {
            int sec = (time % 60);

            return sec.ToString();
        }

        // 취소
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 수정
        private void button1_Click(object sender, EventArgs e)
        {
            bool mImage = false;
            string uploadFileName = null;

            if (form1.getPPLDataList().getPPL(index).getProductImage().Contains(fileName) == false)
            {
                mImage = true;
                uploadFileName = form1.uploadImage(pictureBox1.ImageLocation, fileName);
            }

            submitQuery(mImage, uploadFileName);

            this.Close();

            form1.removeListView2();

            string[] program = form1.getPPLDataList().getPPL(index).getProgramCode().Split('_');
            string programName = program[0];
            string programEntry = program[1];
            form1.getPPL(UPDATE, programName, programEntry);
            form1.showPPL();
        }

        private void submitQuery(bool mImage, string uploadFileName)
        {
            string address = "http://kdspykim2.cafe24.com:8080/modify_ppl_data?";
            string query = null;

            query = query + address;
            query = query + "product_code=" + productCode + "&";

            if (mImage == true)
            {
                string imageAddress = "http://kdspykim2.cafe24.com/~image/";
                imageAddress = imageAddress + uploadFileName;

                query = query + "product_image=" + imageAddress + "&";
            }

            string productName = product_name.Text;
            string brandName = brand_name.Text;
            string link = link_name.Text;
            string price = price_name.Text;
            string startHour = start_hour.Text;
            string startMinute = start_minute.Text;
            string startSecond = start_second.Text;
            string endHour = end_hour.Text;
            string endMinute = end_minute.Text;
            string endSecond = end_second.Text;

            int startTime_s = form1.convertTime(startHour, startMinute, startSecond);
            int endTime_s = form1.convertTime(endHour, endMinute, endSecond);

            query = query + "product_name=" + productName + "&" + "brand_name=" + brandName + "&" + "store_link=" + link + "&" +
                "price=" + price + "&" + "start_time=" + startTime_s + "&" + "end_time=" + endTime_s;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string result = streamReader.ReadToEnd();
            MessageBox.Show(result.ToString());
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right))
            {
                ContextMenu m = new ContextMenu();
                MenuItem m1 = new MenuItem();

                m1.Text = "수정";

                m1.Click += (senders, es) =>
                {
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.InitialDirectory = "c:\\";
                    fileDialog.Filter = "(*.jpg)|*.jpg|(*.gif)|*.gif|(*.png)|*.png";

                    // 경로 fileDialog.FileName
                    // 파일명 fileDialog.SafeFileName
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox1.ImageLocation = fileDialog.FileName;
                        fileName = fileDialog.SafeFileName;
                    }
                };

                m.MenuItems.Add(m1);

                m.Show(pictureBox1, new Point(e.X, e.Y));
            }
        }
    }
}
