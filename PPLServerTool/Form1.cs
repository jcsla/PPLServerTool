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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Json;

namespace PPLServerTool
{
    public partial class Form1 : Form
    {
        List<string> keyList = new List<string>();
        List<string> programList = new List<string>();
        List<string> programEntryList = new List<string>();
        PPLDataList pplDataList = new PPLDataList();
        string fileName;

        const int INSERT = 1;
        const int DISPLAY = 2;
        const int UPDATE = 3;
        const int INQUIRY = 4;

        public Form1()
        {
            InitializeComponent();

            initializeDramaName();
        }

        private void initializeDramaName()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://211.110.33.122/admin/status");
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"password\":\"1q2w3e4r\"" + "}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string response = streamReader.ReadToEnd();

                    processTokenizer(response);

                    addDramaName();
                }
            }
        }

        private void processTokenizer(string response)
        {
            string[] jsonToken1 = response.Split('[');
            string[] jsonToken2 = jsonToken1[1].Split(']');
            string[] jsonLastToken = jsonToken2[0].Split('"');

            for (int i = 1; i < jsonLastToken.Length; i = i + 2)
            {
                MatchEvaluator replacer = m => ((char)int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)).ToString();
                string value = Regex.Replace(jsonLastToken[i], @"\\u([a-fA-F0-9]{4})", replacer);

                keyList.Add(value);
            }

            for (int i = 0; i < keyList.Count; i++)
            {
                string[] programNameToken = keyList[i].Split('_');

                programList.Add(programNameToken[0]);
            }

            programList = programList.AsEnumerable().Distinct().ToList();
        }

        private void addDramaName()
        {
            for (int i = 0; i < programList.Count; i++)
            {
                i_program_name.Items.Add(programList[i]);
                d_program_name.Items.Add(programList[i]);
            }
        }

        private void program_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProgramEntry(INSERT);
        }

        private void d_program_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProgramEntry(DISPLAY);
        }

        private void updateProgramEntry(int status)
        {
            if (status == INSERT)
            {
                string programName = i_program_name.Text;
                i_program_entry.Items.Clear();
                programEntryList.Clear();

                for (int i = 0; i < keyList.Count; i++)
                {
                    if (keyList[i].IndexOf(programName) != -1)
                    {
                        string[] programEntryToken = keyList[i].Split('_');
                        programEntryList.Add(programEntryToken[1]);
                    }
                }

                programEntryList.Sort();

                for (int i = 0; i < programEntryList.Count; i++)
                    i_program_entry.Items.Add(programEntryList[i]);
            }
            else
            {
                string programName = d_program_name.Text;
                d_program_entry.Items.Clear();
                programEntryList.Clear();

                for (int i = 0; i < keyList.Count; i++)
                {
                    if (keyList[i].IndexOf(programName) != -1)
                    {
                        string[] programEntryToken = keyList[i].Split('_');
                        programEntryList.Add(programEntryToken[1]);
                    }
                }

                programEntryList.Sort();

                for (int i = 0; i < programEntryList.Count; i++)
                    d_program_entry.Items.Add(programEntryList[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string uploadFileName = uploadImage(pictureBox1.ImageLocation, fileName);

            submitQuery(uploadFileName);

            //clearAll();
        }

        public string uploadImage(string path, string fileName)
        {
            Chilkat.SFtp sftp = new Chilkat.SFtp();

            bool success;
            success = sftp.UnlockComponent("Anything for 30-day trial");
            if (success != true)
            {
                MessageBox.Show(sftp.LastErrorText);
                return null;
            }

            sftp.ConnectTimeoutMs = 5000;
            sftp.IdleTimeoutMs = 10000;

            success = sftp.Connect("http://kdspykim2.cafe24.com", 22);
            if (success != true)
            {
                MessageBox.Show(sftp.LastErrorText);
                return null;
            }

            success = sftp.AuthenticatePw("image", "dlalwlfmfsjgdjfk");
            if (success != true)
            {
                MessageBox.Show(sftp.LastErrorText);
                return null;
            }

            success = sftp.InitializeSftp();
            if (success != true)
            {
                MessageBox.Show(sftp.LastErrorText);
                return null;
            }

            string uploadFileName = makeFileName(fileName);

            string handle;
            handle = sftp.OpenFile("public_html/" + uploadFileName, "writeOnly", "createTruncate");
            if (handle == null)
            {
                MessageBox.Show(sftp.LastErrorText);
                return null;
            }

            success = sftp.UploadFile(handle, path);
            if (success != true)
            {
                MessageBox.Show(sftp.LastErrorText);
                return null;
            }

            success = sftp.CloseHandle(handle);
            if (success != true)
            {
                MessageBox.Show(sftp.LastErrorText);
                return null;
            }

            return uploadFileName;
        }

        private string makeFileName(string fileName)
        {
            string uploadFileName;

            string year = System.DateTime.Now.Year.ToString();
            string month = System.DateTime.Now.Month.ToString();
            string day = System.DateTime.Now.Day.ToString();
            string hour = System.DateTime.Now.Hour.ToString();
            string minute = System.DateTime.Now.Minute.ToString();
            string second = System.DateTime.Now.Second.ToString();
            string date = year + month + day + hour + minute + second;

            Random r = new Random();
            string randomNumber = r.Next().ToString();

            uploadFileName = date + "_" + randomNumber + "_" + fileName;

            return uploadFileName;
        }

        private void submitQuery(string uploadFileName)
        {
            string address = "http://kdspykim2.cafe24.com:8080/insert_ppl_data?";
            string imageAddress = "http://kdspykim2.cafe24.com/~image/";

            string programName = i_program_name.Text;
            string programEntry = i_program_entry.Text;
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
            string story = story_text.Text;

            int startTime_s = convertTime(startHour, startMinute, startSecond);
            int endTime_s = convertTime(endHour, endMinute, endSecond);

            imageAddress = imageAddress + uploadFileName;

            string query = address + "drama_code=" + programName + "_" + programEntry + "&" + "product_name=" + productName +
                "&" + "product_image=" + imageAddress + "&" + "brand_name=" + brandName + "&" + "store_link=" + link +
                "&" + "price=" + price + "&" + "start_time=" + startTime_s + "&" + "end_time=" + endTime_s + "&" + "story=" + story;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string result = streamReader.ReadToEnd();
            MessageBox.Show(result.ToString());
        }

        public int convertTime(string hour, string min, string sec)
        {
            int i_hour = Convert.ToInt32(hour);
            int i_min = Convert.ToInt32(min);
            int i_sec = Convert.ToInt32(sec);

            return i_hour * 3600 + i_min * 60 + i_sec;
        }

        private void clearAll()
        {
            i_program_name.Text = "";
            i_program_entry.Text = "";
            product_name.Text = "";
            brand_name.Text = "";
            link_name.Text = "";
            price_name.Text = "";
            start_hour.Text = "";
            start_minute.Text = "";
            start_second.Text = "";
            end_hour.Text = "";
            end_minute.Text = "";
            end_second.Text = "";
            story_text.Text = "";

            i_program_entry.Items.Clear();

            listView1.Items.Clear();
            pictureBox1.ImageLocation = null;
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right))
            {
                if (countImage() == 0)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem m1 = new MenuItem();

                    m1.Text = "추가";

                    m1.Click += (senders, es) =>
                    {
                        addImage();
                    };

                    m.MenuItems.Add(m1);

                    m.Show(listView1, new Point(e.X, e.Y));
                }
            }
        }

        private void addImage()
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

            ListViewItem lvi = new ListViewItem(fileDialog.FileName);
            listView1.Items.Add(lvi);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right))
            {
                if (listView1.Items[0].Selected == true)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem m2 = new MenuItem();

                    m2.Text = "삭제";

                    m2.Click += (senders, es) =>
                    {
                        deleteImage();
                    };

                    m.MenuItems.Add(m2);

                    m.Show(listView1, new Point(e.X, e.Y));
                }
            }
            else
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Selected == true)
                    {
                        pictureBox1.ImageLocation = listView1.Items[i].Text;
                    }
                }
            }
        }

        private void deleteImage()
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items.Remove(listView1.SelectedItems[i]);
                listView1.Update();
            }

            pictureBox1.ImageLocation = null;
        }

        private int countImage()
        {
            int count = 0;

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                    count = count + 1;
            }

            return count;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            getPPL(INQUIRY, null, null);
            showPPL();
        }

        public void getPPL(int mode, string programName, string programEntry)
        {
            string address = "http://kdspykim2.cafe24.com:8080/get_ppl_data?";

            if (mode == INQUIRY)
            {
                programName = d_program_name.Text.ToString();
                programEntry = d_program_entry.Text.ToString();
            }

            string programKey = programName + "_" + programEntry;

            string query = address + "drama_code=" + programKey;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string result = streamReader.ReadToEnd();

            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(result);
            JsonArrayCollection items = (JsonArrayCollection)obj;

            pplDataList.clearList();
            foreach (JsonObjectCollection item in items)
            {
                string productCode = item["product_code"].GetValue().ToString();
                string programCode = item["drama_code"].GetValue().ToString();
                string productName = item["product_name"].GetValue().ToString();
                string brandName = item["brand_name"].GetValue().ToString();
                string productImage = item["product_image"].GetValue().ToString();
                string price = item["price"].GetValue().ToString();
                string storeLink = item["store_link"].GetValue().ToString();
                int startTime = Convert.ToInt32(item["start_time"].GetValue());
                int endTime = Convert.ToInt32(item["end_time"].GetValue());

                PPLData pplData = new PPLData(productCode, programCode, productName, brandName, productImage, price, storeLink, startTime, endTime);
                pplDataList.insertPPLData(pplData);
            }
        }

        public void showPPL()
        {
            for (int i = 0; i < pplDataList.getCount(); i++)
            {
                listView2.Items.Add(pplDataList.getPPL(i).getProductName());
                listView2.Items[i].SubItems.Add(pplDataList.getPPL(i).getBrandName());
                listView2.Items[i].SubItems.Add(pplDataList.getPPL(i).getStoreLink());
                listView2.Items[i].SubItems.Add(pplDataList.getPPL(i).getPrice());
                listView2.Items[i].SubItems.Add(convertTime(pplDataList.getPPL(i).getStartTime()));
                listView2.Items[i].SubItems.Add(convertTime(pplDataList.getPPL(i).getEndTime()));
            }
        }

        private string convertTime(int time)
        {
            int hour = time / 3600;
            int minute = (time / 60) % 60;
            int sec = (time % 60);

            return hour.ToString() + " : " + minute.ToString() + " : " + sec.ToString();
        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right))
            {
                ContextMenu m = new ContextMenu();
                MenuItem m1 = new MenuItem();

                m1.Text = "수정";

                m1.Click += (senders, es) =>
                {
                    Form2 form2 = new Form2(this);
                    form2.ShowDialog();
                };

                m.MenuItems.Add(m1);

                m.Show(listView2, new Point(e.X, e.Y));
            }
        }

        public ListView getListView2()
        {
            return listView2;
        }

        public PPLDataList getPPLDataList()
        {
            return pplDataList;
        }

        public void removeListView2()
        {
            listView2.Items.Clear();
        }
    }
}