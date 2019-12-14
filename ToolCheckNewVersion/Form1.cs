using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolCheckNewVersion
{   
    public partial class MainForm : Form
    {
        string CactruyenDangQuet="", TruyenDangQuet="", ChuongDangQuet="", LinkBia = "";
        string TienTrinhQuetBia = "Chưa chạy";

        int TruyenDie = 0;
        int SoLuongAnhKiemTra = 0;
        int SoChuongDie = 0;
        int TienTrinh = 1;
        int Model = 0;
        

        public MainForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            if (File.Exists("./Setting.txt"))
            {
                StreamReader objReader;

                string filename = "./Setting.txt";

                objReader = new StreamReader(filename);

                do
                {
                    if (objReader.ReadLine() == "ThongBao") { cbNhanThongBao.Checked = true; }
                    if (objReader.ReadLine() == "GhiNho") { cbGhiNho.Checked = true; }
                    if (objReader.ReadLine() == "DaTab") { cbMultiple.Checked = true; }
                    trModel.Value = int.Parse(objReader.ReadLine());
                    if (objReader.ReadLine() == "MoTrangSuaChuong") { cbOpenLink.Checked = true; }
                    if (objReader.ReadLine() == "HienChrome") { cbHienChrome.Checked = true; }
                }
                while ((objReader.Peek() != -1));
                objReader.Close();

                if (cbGhiNho.Checked == true)
                {
                    txtTaiKhoan.Text = ToolCheckNewVersion.Properties.Settings.Default.TaiKhoan;
                    txtMatKhau.Text = ToolCheckNewVersion.Properties.Settings.Default.MatKhau;
                }

            }
            else
            { }

            if (File.Exists("./History.txt") == true)
            {
                StreamReader objReader;

                string filename = "./History.txt";

                objReader = new StreamReader(filename);

                do
                {
                    rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + objReader.ReadLine() + "\n";
                }
                while ((objReader.Peek() != -1));
                objReader.Close();

            }
            else
            {
                File.CreateText("./History.txt");
            }

            //txt1.Text = "dfsd";

            //txt1.Items.Add("d");

            //txt1.Text = txt1.Items[0].ToString();

            Process[] processes = Process.GetProcessesByName("COM Surrogate");
            foreach (var process in processes)
            {
                process.Kill();
            }

            tipModel.SetToolTip(trModel, "Độ chính xác đạt " + (90 + trModel.Value) + "%");
        }

        void KiemTraBia()
        {
            Int32 ID1 = Int32.Parse(txtID1.Text);
            Int32 ID2 = Int32.Parse(txtID2.Text);
            CactruyenDangQuet = "Đang quét các truyện ID từ " + txtID1.Text + " đến " + txtID2.Text; //Trang thai
            Status();

            while (ID1 <= ID2)
            {
                if (TienTrinhQuetBia == "Chưa chạy")
                {
                    break;
                }

                TruyenDie = 0;
                TruyenDangQuet = "Đang Quét tới truyện ID: " + ID1.ToString(); //Trang thai
                Status();

                //Lấy ra trang html của truyện              
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(@"https://blogtruyen.vn/" + ID1.ToString());

                #region Hearder
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":authority", "blogtruyen.vn");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":method", "GET");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":path", @"https://blogtruyen.vn/" + ID1.ToString());
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":scheme", "https");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-encoding", "gzip, deflate, br");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-language", "vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "max-age=0");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("cookie", "BTHiddenSidebarWidget=; BTHiddenSidebarWidget=; __cfduid=d4008b0ffe03aa2d327a08955272d519e1552905463; _ga=GA1.2.1971512359.1552871828; _gid=GA1.2.2059036427.1552871828; BT_ID=Dbw7wzBy3xd1J1TGvg5v; RdBsw44wJZ=45EABB5031A14C7939896F4FAE1728B2; BTHiddenSidebarWidget=; btpop4=Popunder; btpop5=Popunder; btpop1=Popunder; btpop2=Popunder; btpop3=Popunder; bannerpreload=1");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("upgrade-insecure-requests", "1");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", @"https://blogtruyen.vn/" + ID1.ToString());
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
                #endregion

                string htmlTruyen = httpClient.GetStringAsync("").Result;

                try
                {
                    string AnhBia1 = @"<div class=""thumbnail"">(.*?)</div>";
                    var Bia1 = Regex.Matches(htmlTruyen, AnhBia1, RegexOptions.Singleline);

                    string AnhBia2 = @"""(.*?)""";
                    var Bia2 = Regex.Matches(Bia1[0].ToString(), AnhBia2, RegexOptions.Singleline);

                    LinkBia = Bia2[1].ToString().Replace(@"""", "");                   
                }
                catch
                {
                    LinkBia = "Rỗng";
                }
                
                if(LinkBia != "Rỗng")
                {
                    try { LinkBia = LinkBia.Replace(@"///", @"//"); }
                    catch { }

                    try
                    {
                        WebClient request = new WebClient();
                        request.Headers.Set("Referer", @"https://blogtruyen.vn/" + ID1.ToString());
                        request.DownloadFile(LinkBia, "Check.png");
                    }
                    catch
                    {
                        rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "[lỗi bìa] " + "https://blogtruyen.vn/admin/ManageManga/UpdateWithPermission/" + ID1.ToString() + "\n"; //Trường hợp muốn lấy link thay vì tên

                        SoChuongDie++;
                        txtCount.Text = "Số lượng truyện lỗi bìa: " + SoChuongDie; //Trang Thai
                    }
                }
                

                //rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "\n\n";
                ID1++;

                System.GC.Collect();
            }
        }

        void LayIDChuong(string LinkChuong)
        {
            string ID = LinkChuong.Split(new string[] { "/c" }, StringSplitOptions.None)[1];
            ID = ID.Split(new string[] { "/" }, StringSplitOptions.None)[0];
            ListLink.Items.Add(ID);
            ListLink.TopIndex = ListLink.Items.Count - 1;
        }

        void Status()
        {
            try
            {
                rtbStatus.Text = "Đang quét các truyện từ ID " + txtID1.Text + " tới ID " + txtID2.Text + "\n"
                            + TruyenDangQuet + "\n"
                            + ChuongDangQuet;
            }
            catch { }
            
        }

        void KetThuc()
        {
            txtID1.ReadOnly = false;
            txtID2.ReadOnly = false;
                    
            btnQuet.Text = "Quét";
            btnQuet.Enabled = true;

            btnQuetUpdate.Text = "Quét + update";
            btnQuetUpdate.Enabled = true;

            btnAutoUpdate.Text = "Cập nhật tự động";
            btnAutoUpdate.Enabled = true;

            rtbStatus.Text = "";
            CactruyenDangQuet = ""; TruyenDangQuet = ""; ChuongDangQuet = "";

            rtbKetQuaQuet.ReadOnly = false;

            prQuetAnh.Value = 0;
            prQuetChuong.Value = 0;

            trModel.Enabled = true;
            Model = trModel.Value;

            btnQuetBia.Enabled = true;
            TienTrinhQuetBia = "Chưa chạy";
            //txtCount.Text = "";
        }

        void GiaiPhongBoNho()
        {
            if(cbMultiple.Checked == false)
            {
                try
                {
                    Process[] processes = Process.GetProcessesByName("chromedriver");
                    foreach (var process in processes)
                    {
                        process.Kill();
                    }
                }
                catch { }
            }
            
        }
            
        void LayChuong()
        {
            ListLink.Items.Clear();

            Int32 ID1 = Int32.Parse(txtID1.Text);
            Int32 ID2 = Int32.Parse(txtID2.Text);
            CactruyenDangQuet = "Đang quét các truyện ID từ " + txtID1.Text + " đến " + txtID2.Text; //Trang thai
            Status();

            while (ID1 <= ID2)
            {
                if (btnQuet.Text != "Dừng")
                {
                    break;
                }

                TruyenDie = 0;
                TruyenDangQuet = "Đang Quét tới truyện ID: " + ID1.ToString(); //Trang thai
                Status();

                //Lấy ra trang html của truyện              
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(@"https://blogtruyen.vn/" + ID1.ToString());

                #region Hearder
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":authority", "blogtruyen.vn");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":method", "GET");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":path", @"https://blogtruyen.vn/" + ID1.ToString());
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":scheme", "https");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-encoding", "gzip, deflate, br");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-language", "vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "max-age=0");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("cookie", "BTHiddenSidebarWidget=; BTHiddenSidebarWidget=; __cfduid=d4008b0ffe03aa2d327a08955272d519e1552905463; _ga=GA1.2.1971512359.1552871828; _gid=GA1.2.2059036427.1552871828; BT_ID=Dbw7wzBy3xd1J1TGvg5v; RdBsw44wJZ=45EABB5031A14C7939896F4FAE1728B2; BTHiddenSidebarWidget=; btpop4=Popunder; btpop5=Popunder; btpop1=Popunder; btpop2=Popunder; btpop3=Popunder; bannerpreload=1");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("upgrade-insecure-requests", "1");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", @"https://blogtruyen.vn/" + ID1.ToString());
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
                #endregion

                string htmlDanhSachChuong = httpClient.GetStringAsync("").Result;

                string DanhSachChuongPartem = @"<p id=""ch(.*?)</p>";
                var DanhSachChuong = Regex.Matches(htmlDanhSachChuong, DanhSachChuongPartem, RegexOptions.Singleline);

                string TenChuong;
                string LinkChuong;

                int i = DanhSachChuong.Count - 1;
                double DaQuet = 0;
                while (i >= 0)
                {
                    DaQuet++;
                    prQuetChuong.Value = Convert.ToInt32((DaQuet / DanhSachChuong.Count) * 100);

                    if (btnQuet.Text != "Dừng")
                    {
                        break;
                    }

                    SoLuongAnhKiemTra = 0;

                    var NameChap = Regex.Matches(DanhSachChuong[i].ToString(), @"title=""(.*?)""", RegexOptions.Singleline);
                    TenChuong = NameChap[0].ToString().Replace(@"title=""", "");
                    TenChuong = TenChuong.Replace(@"""", "");

                    var LinkChap = Regex.Matches(DanhSachChuong[i].ToString(), @"href=""(.*?)""", RegexOptions.Singleline);
                    LinkChuong = LinkChap[0].ToString().Replace(@"href=""", "");
                    LinkChuong = LinkChuong.Replace(@"""", "");


                    LayDanhSachAnh(LinkChuong + "|" + TenChuong);
                    System.GC.Collect();

                    i--;
                }

                //rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "\n\n";
                ID1++;

                System.GC.Collect();
            }
        }

        void LayDanhSachAnh(object LinkTen)
        {
            string LinkChuong = LinkTen.ToString().Split('|')[0];
            string TenChuong = LinkTen.ToString().Split('|')[1];

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(@"https://blogtruyen.vn" + LinkChuong);

            #region Header
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":authority", "blogtruyen.vn");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":method", "GET");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":path", LinkChuong);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(":scheme", "https");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-language", "vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "max-age=0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("cookie", "BTHiddenSidebarWidget=; BTHiddenSidebarWidget=; __cfduid=d4008b0ffe03aa2d327a08955272d519e1552905463; _ga=GA1.2.1971512359.1552871828; _gid=GA1.2.2059036427.1552871828; BT_ID=Dbw7wzBy3xd1J1TGvg5v; RdBsw44wJZ=45EABB5031A14C7939896F4FAE1728B2; BTHiddenSidebarWidget=; btpop4=Popunder; btpop5=Popunder; btpop1=Popunder; btpop2=Popunder; btpop3=Popunder; bannerpreload=1");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", @"https://blogtruyen.vn" + LinkChuong);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("upgrade-insecure-requests", "1");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
            #endregion

            string htmlDanhSachAnh = httpClient.GetStringAsync("").Result;

            string DanhSachAnhPartem1 = @"<article id=""content"">(.*?)</article>";
            var DanhSachAnh1 = Regex.Matches(htmlDanhSachAnh, DanhSachAnhPartem1, RegexOptions.Singleline);

            int TruongHop = 0;
            string DanhSachAnhPartem2 = @"<img src=""(.*?)""";
            var DanhSachAnh2 = Regex.Matches(DanhSachAnh1[0].ToString(), DanhSachAnhPartem2, RegexOptions.Singleline);

            if (DanhSachAnh2.Count == 0)
            {
                TruongHop = 1;
                DanhSachAnhPartem2 = @"{""url"":""(.*?)""";             
                DanhSachAnh2 = Regex.Matches(DanhSachAnh1[0].ToString(), DanhSachAnhPartem2, RegexOptions.Singleline);
            }

            /* //Tạo cửa sổ Chrome Cách 2
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true; //ẩn cửa sổ command

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless"); //ẩn cửa sổ chrome
            options.AddUserProfilePreference("disable-popup-blocking", "true");
            options.AddArguments("--disable-extensions"); // to disable extension
            options.AddArguments("--disable-notifications"); // to disable notification
            options.AddArguments("--disable-application-cache"); // to disable cache

            ChromeDriver chromeDriver = new ChromeDriver(service, options);

            chromeDriver.Url = @"https://blogtruyen.vn" + LinkChuong;

            chromeDriver.Navigate();
            // */
            //int SoThuTuAnh = 1;//số thứ tự của element ảnh Cách 2

            //Kiểm tra link ảnh
            string Link = "";
            double DaQuet = 0;
            prQuetAnh.Value = 0;

            //double test = Math.Round((DanhSachAnh2.Count * trModel.Value) / 10.0);

            foreach (var anh in DanhSachAnh2)
            {
                DaQuet++;
                prQuetAnh.Value = Convert.ToInt32((DaQuet / DanhSachAnh2.Count) * 100);

                if (btnQuet.Text != "Dừng")
                {
                    break;
                }

                if(TruongHop == 0)
                {
                    Link = anh.ToString().Replace(@"<img src=""", "");
                    Link = Link.Replace(@"""", "");
                }
                else
                {
                    Link = anh.ToString().Replace(@"{""url"":""", "");
                    Link = Link.Replace(@"""", "");
                }
                

                ChuongDangQuet = "Đang quét tới chương: " + TenChuong; //Trang thai
                Status();

                if (Link.IndexOf("i.blogtruyen.com") == -1)//Giữ nguyên
                {
                    if (TruyenDie < 1)
                    {
                        TruyenDie = 1;
                        rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "\n" + "Truyện : " + "https://blogtruyen.vn/" + TruyenDangQuet.Replace("Đang Quét tới truyện ID: ", "") + "\n";
                    }

                    rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "https://blogtruyen.vn" + LinkChuong + "\n"; //Trường hợp muốn lấy link thay vì tên

                    SoChuongDie++;
                    txtCount.Text = "Số lượng chương lỗi: " + SoChuongDie; //Trang thái
                    LayIDChuong(LinkChuong);
                    break;
                }
                else 
                {
                    if (Model == 1 && SoLuongAnhKiemTra < 1) //Chỉ kiểm tra 1 ảnh đầu
                    {
                        SoLuongAnhKiemTra++; 

                        try
                        {
                            WebClient request = new WebClient();
                            request.Headers.Set("Referer", @"https://blogtruyen.vn" + LinkChuong);
                            request.DownloadFile(Link, "Check.png");
                        }
                        catch
                        {
                            if (TruyenDie < 1)
                            {
                                TruyenDie = 1;
                                rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "\n" + "Truyện : " + "https://blogtruyen.vn/" + TruyenDangQuet.Replace("Đang Quét tới truyện ID: ", "") + "\n";
                            }

                            rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "[cần re-up] " + "https://blogtruyen.vn" + LinkChuong + "\n"; //Trường hợp muốn lấy link thay vì tên

                            SoChuongDie++;
                            txtCount.Text = "Số lượng chương lỗi: " + SoChuongDie; //Trang Thai
                            LayIDChuong(LinkChuong);
                            break;
                        }
                       
                    }

                    else if (Model > 1 && trModel.Value < 10 && SoLuongAnhKiemTra <= Math.Round((DanhSachAnh2.Count * Model) / 10.0)) //kiểm tra ảnh theo giá trị % từ trModel
                    {
                        SoLuongAnhKiemTra++; 

                        try
                        {
                            WebClient request = new WebClient();
                            request.Headers.Set("Referer", @"https://blogtruyen.vn" + LinkChuong);
                            request.DownloadFile(Link, "Check.png");
                        }
                        catch
                        {
                            if (TruyenDie < 1)
                            {
                                TruyenDie = 1;
                                rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "\n" + "Truyện : " + "https://blogtruyen.vn/" + TruyenDangQuet.Replace("Đang Quét tới truyện ID: ", "") + "\n";
                            }

                            rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "[cần re-up] " + "https://blogtruyen.vn" + LinkChuong + "\n"; //Trường hợp muốn lấy link thay vì tên

                            SoChuongDie++;
                            txtCount.Text = "Số lượng chương lỗi: " + SoChuongDie; //Trang Thai
                            LayIDChuong(LinkChuong);
                            break;
                        }

                    }

                    else if (Model == 10)
                    {
                        //Tìm element ảnh Cách 2
                        //var image = chromeDriver.FindElementByXPath(@"//*[@id=""content""]/img[" + SoThuTuAnh + "]");

                        try
                        {
                            WebClient request = new WebClient();
                            request.Headers.Set("Referer", @"https://blogtruyen.vn" + LinkChuong);
                            request.DownloadFile(Link, "Check.png");
                        }
                        catch
                        {
                            if (TruyenDie < 1)
                            {
                                TruyenDie = 1;
                                rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "\n" + "Truyện : " + "https://blogtruyen.vn/" + TruyenDangQuet.Replace("Đang Quét tới truyện ID: ", "") + "\n";
                            }

                            rtbKetQuaQuet.Text = rtbKetQuaQuet.Text + "[cần re-up] " + "https://blogtruyen.vn" + LinkChuong + "\n"; //Trường hợp muốn lấy link thay vì tên

                            SoChuongDie++;
                            txtCount.Text = "Số lượng chương lỗi: " + SoChuongDie; //Trang Thai
                            LayIDChuong(LinkChuong);
                            break;
                        }

                    }
                    
                }
                
                //SoThuTuAnh++; //Cách 2
                System.GC.Collect();
                                            
            }

            System.GC.Collect();
            //chromeDriver.Close(); //Cách 2
        }

        void CapNhatTuDong()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true; //ẩn cửa sổ command

            ChromeOptions options = new ChromeOptions();
            
            if(cbHienChrome.Checked != true)
            {
                options.AddArgument("headless"); //ẩn cửa sổ chrome
            }

            ChromeDriver chromeDriver = new ChromeDriver(service, options);
            //ChromeDriver chromeDriver = new ChromeDriver();


            chromeDriver.Url = "https://id.blogtruyen.vn/dang-nhap?returnUrl=https://blogtruyen.vn/admin/";
            chromeDriver.Navigate();

            var username = chromeDriver.FindElementById("UserName");
            username.SendKeys(txtTaiKhoan.Text);

            var password = chromeDriver.FindElementById("Password");
            password.SendKeys(txtMatKhau.Text);

            var dangnhap = chromeDriver.FindElementByClassName("btn-raised");
            dangnhap.Click();

            int i = 1;
            int sum = ListLink.Items.Count;

            while (i <= sum)
            {
                if (btnAutoUpdate.Text != "Dừng")
                {
                    break;
                }

                try
                {
                    ListLink.SelectedIndex = i - 1;
                    rtbStatus.Text = "Đang tiến hành cập nhật tự động\n" + "Đang cập nhật chương: " + i + "/" + sum;

                    chromeDriver.Url = "https://blogtruyen.vn/admin/cap-nhat-chuong/" + ListLink.Items[i - 1].ToString();
                    chromeDriver.Navigate();

                    var update = chromeDriver.FindElementByClassName("btnUpdateEditor");
                    update.Click();
                }
                catch
                {
                 
                    MessageBox.Show("Không thể thao tác.\n\nCó thể bạn đã nhập sai mật khẩu, tài khoản hoặc trình duyệt bị tắt đột ngột.", "Lỗi");
                    break;
                }

                i++;
            }

            try
            { chromeDriver.Close(); }
            catch { }

            GiaiPhongBoNho();        
        }

        private void btnQuetUpdate_Click(object sender, EventArgs e)
        {
            CactruyenDangQuet = ""; TruyenDangQuet = ""; ChuongDangQuet = "";
            TienTrinh = 1;           

            int test = 1;

            try
            {
                Int32 ID1 = Int32.Parse(txtID1.Text);
            }
            catch
            { test = 0; }

            if(txtID2.Text == "" || txtID2.Text == null)
            {
                txtID2.Text = txtID1.Text;
            }

            if(txtID1.Text == "" || txtID2.Text == null)
            {
                MessageBox.Show("Vui lòng điền ID truyện bạn muốn quét", "Lỗi");
            }
            else if (txtMatKhau.Text == "" || txtMatKhau.Text == null || txtTaiKhoan.Text == "" || txtTaiKhoan.Text == null)
            {
                MessageBox.Show("Vui lòng điền mật khẩu và tài khoản", "Lỗi");
            }
            else if (test == 0)
            { MessageBox.Show("ID truyện không hợp lệ", "Lỗi");}
            else
            {
                new Thread(() =>
                {
                    ThreadStart thread1 = new ThreadStart(LayChuong);
                    Thread thrd1 = new Thread(thread1);
                    thrd1.IsBackground = true;

                    ThreadStart thread2 = new ThreadStart(CapNhatTuDong);
                    Thread thrd2 = new Thread(thread2);
                    thrd2.IsBackground = true;

                    ThreadStart thread3 = new ThreadStart(LayChuong);
                    Thread thrd3 = new Thread(thread3);
                    thrd3.IsBackground = true;

                    if (btnQuetUpdate.Text == "Quét + update")
                    {                      
                        while (TienTrinh < 4)
                        {
                            if (TienTrinh == 1)
                            {
                                Model = 0;
                                trModel.Enabled = false;

                                SoChuongDie = 0;
                                txtCount.Text = "Số lượng chương lỗi: 0";

                                btnQuetUpdate.Text = "Dừng";
                                btnQuet.Text = "Dừng";
                                btnQuetBia.Enabled = false;
                                btnQuet.Enabled = false;
                                rtbKetQuaQuet.ReadOnly = true;
                                txtID1.ReadOnly = true;
                                txtID2.ReadOnly = true;

                                thrd1.Start();
                                thrd1.Join();
                                KetThuc();
                            }

                            else if (TienTrinh == 2)
                            {
                                if(ListLink.Items.Count > 0)
                                {
                                    rtbStatus.Text = "Đang tiến hành cập nhật tự động";

                                    btnQuetUpdate.Text = "Dừng";
                                    btnAutoUpdate.Text = "Dừng";
                                    btnQuetBia.Enabled = false;
                                    btnQuet.Enabled = false;
                                    btnAutoUpdate.Enabled = false;
                                    rtbKetQuaQuet.ReadOnly = true;
                                    txtID1.ReadOnly = true;
                                    txtID2.ReadOnly = true;

                                    thrd2.Start();
                                    thrd2.Join();
                                    KetThuc();
                                    txtCount.Text = "";
                                }
                                else
                                {                                  
                                    trModel.Enabled = true;

                                    rtbStatus.Text = "Đã xong";

                                    if (cbNhanThongBao.Checked == true)
                                    {
                                        MessageBox.Show("Đã quét và update xong", "Thông báo");
                                    }
                                    break;
                                }
                            }

                            else if (TienTrinh == 3)
                            {
                                Model = trModel.Value;
                                trModel.Enabled = true;

                                SoChuongDie = 0;
                                txtCount.Text = "Số lượng chương lỗi: 0";

                                btnQuetUpdate.Text = "Dừng";
                                btnQuet.Text = "Dừng";
                                btnQuetBia.Enabled = false;
                                btnQuet.Enabled = false;
                                rtbKetQuaQuet.ReadOnly = true;
                                rtbKetQuaQuet.Text = "";
                                txtID1.ReadOnly = true;
                                txtID2.ReadOnly = true;

                                thrd3.Start();
                                thrd3.Join();
                                KetThuc();

                                rtbStatus.Text = "Quét và update hoàn tất";

                                if (cbNhanThongBao.Checked == true)
                                {
                                    MessageBox.Show("Đã quét và update xong", "Thông báo");
                                }                            
                            }

                            TienTrinh++;
                            
                        }

                    }
                    else
                    {
                        KetThuc();
                        TienTrinh = 4;

                        try
                        {
                            try
                            {
                                thrd1.Abort();
                            }
                            catch
                            {
                                thrd2.Abort();

                                GiaiPhongBoNho();
                            }

                        }
                        catch
                        { thrd3.Abort(); }
                    }
                   
                })
                { IsBackground = true }.Start();
            }
           
            rtbStatus.Focus();
        }      

        private void btnQuet_Click(object sender, EventArgs e)
        {
            CactruyenDangQuet = ""; TruyenDangQuet = ""; ChuongDangQuet = "";

            int test = 1;
            Model = trModel.Value;

            try
            {
                Int32 ID1 = Int32.Parse(txtID1.Text);
            }
            catch
            { test = 0; }

            if (txtID2.Text == "" || txtID2.Text == null)
            {
                txtID2.Text = txtID1.Text;
            }

            new Thread(() =>
            {
                if(test == 0)
                { MessageBox.Show("ID truyện không hợp lệ", "Lỗi"); }
                else if (txtID1.Text != "" && txtID1.Text != null && txtID2.Text != "" && txtID2.Text != null)
                {
                    ThreadStart thread = new ThreadStart(LayChuong);
                    Thread thrd = new Thread(thread);
                    thrd.IsBackground = true;

                    if (btnQuet.Text == "Quét")
                    {
                        thrd.Start();

                        ListLink.Items.Clear();

                        btnQuet.Text = "Dừng";
                        SoChuongDie = 0;
                        txtCount.Text = "Số lượng chương lỗi: 0";

                        if (btnQuetUpdate.Text != "Dừng quét")
                        {
                            btnQuetUpdate.Enabled = false;
                        }

                        btnQuetBia.Enabled = false;
                        rtbKetQuaQuet.ReadOnly = true;
                        txtID1.ReadOnly = true;
                        txtID2.ReadOnly = true;
                    }
                    else if (btnQuet.Text == "Dừng")
                    {
                        btnQuet.Text = "Quét";
                        thrd.Abort();
                    }

                    try
                    { thrd.Join(); }
                    catch
                    { }

                    KetThuc();

                    rtbStatus.Text = "Đã xong";
                    if (cbNhanThongBao.Checked == true)
                    {
                        MessageBox.Show("Update tự động hoàn tất", "Thông báo");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng điền đủ ID1 và ID2");
                }

            })
            { IsBackground = true }.Start();

            rtbStatus.Focus();
        }
        
        private void btnAutoUpdate_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {

                if (btnQuet.Text == "Dừng")
                {
                    MessageBox.Show("Chưa Quét xong chương lỗi nên không thể tự động cập nhật\n\nVui lòng dừng tiến trình Quét chương lỗi hoặc đợi tiến trình hoàn tất", "Lỗi");
                }
                else if (btnQuet.Text == "Quét")
                {

                    if (txtMatKhau.Text != null && txtMatKhau.Text != "" && txtMatKhau.Text != " " && txtTaiKhoan.Text != null && txtTaiKhoan.Text != "" && txtTaiKhoan.Text != " ")
                    {
                        ThreadStart thread = new ThreadStart(CapNhatTuDong);
                        Thread thrd = new Thread(thread);
                        thrd.IsBackground = true;

                        if (btnAutoUpdate.Text == "Cập nhật tự động")
                        {
                            thrd.Start();
                            btnQuetBia.Enabled = false;
                            btnAutoUpdate.Text = "Dừng";

                            rtbStatus.Text = "Đang tiến hành cập nhật tự động";
                        }
                        else if (btnAutoUpdate.Text == "Dừng")
                        {
                            thrd.Abort();
                        }

                        try
                        { thrd.Join(); }
                        catch
                        { }

                        KetThuc();

                        GiaiPhongBoNho();

                        rtbStatus.Text = "Cập nhật tự động hoàn tất";
                        if (cbNhanThongBao.Checked == true)
                        {
                            MessageBox.Show("Đã cập nhật xong", "Thông báo");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng điền mật khẩu và tài khoản");
                    }

                }

            })
            { IsBackground = true }.Start();

            rtbStatus.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText("./History.txt", "");

            if (File.Exists("./Setting.txt"))
            {
                File.Delete("./Setting.txt");
            }

            using (StreamWriter sw = File.CreateText("Setting.txt"))
            {
                if (cbNhanThongBao.Checked == true) { sw.WriteLine("ThongBao"); }
                else { sw.WriteLine("KhongThongBao"); }

                if (cbGhiNho.Checked == true) { sw.WriteLine("GhiNho"); }
                else { sw.WriteLine("KhongGhiNho"); }

                if (cbMultiple.Checked == true) { sw.WriteLine("DaTab"); }
                else { sw.WriteLine("KhongDaTab"); }

                sw.WriteLine(trModel.Value.ToString());

                if (cbOpenLink.Checked == true) { sw.WriteLine("MoTrangSuaChuong"); }
                else { sw.WriteLine("KhongMoTrangSuaChuong"); }

                if (cbHienChrome.Checked == true) { sw.WriteLine("HienChrome"); }
                else { sw.WriteLine("KhongHienChrome"); }
            }

            if (cbGhiNho.Checked == true)
            {
                ToolCheckNewVersion.Properties.Settings.Default.TaiKhoan = txtTaiKhoan.Text;
                ToolCheckNewVersion.Properties.Settings.Default.MatKhau = txtMatKhau.Text;
                ToolCheckNewVersion.Properties.Settings.Default.Save();
            }
            else
            {
                ToolCheckNewVersion.Properties.Settings.Default.TaiKhoan = "";
                ToolCheckNewVersion.Properties.Settings.Default.MatKhau = "";
                ToolCheckNewVersion.Properties.Settings.Default.Save();
            }

            GiaiPhongBoNho();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbKetQuaQuet.Focus();
            SendKeys.Send("^a{BS}");

            //ListLink.Items.Clear();
        }
      
        private void btnSave_Click(object sender, EventArgs e)
        {
            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            var fullFileName = System.IO.Path.Combine(desktopFolder, "List " + txtID1.Text + "-" + txtID2.Text + ".txt");

            if (rtbKetQuaQuet.Text != null && rtbKetQuaQuet.Text != "" && rtbKetQuaQuet.Text != "\n" && rtbKetQuaQuet.Text != " ")
            {
                if (File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }

                if (MessageBox.Show("Bạn có muốn lưu danh sách quét từ ID " + txtID1.Text + " tới ID " + txtID2.Text + " không ?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (StreamWriter sw = File.CreateText(fullFileName))
                    {
                        sw.WriteLine(rtbKetQuaQuet.Text);
                    }

                }
            }
        }
       
        private void rtbKetQuaQuet_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (cbOpenLink.Checked == true)
            {
                try
                {
                    string ID = e.LinkText.Split(new string[] { "/c" }, StringSplitOptions.None)[1];
                    ID = ID.Split(new string[] { "/" }, StringSplitOptions.None)[0];
                    System.Diagnostics.Process.Start("https://blogtruyen.vn/admin/cap-nhat-chuong/" + ID);
                }
                catch
                {
                    System.Diagnostics.Process.Start(e.LinkText);
                }
            }
            else
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
        }
      
        private void ListLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnAutoUpdate.Text == "Cập nhật tự động")
            {
                System.Diagnostics.Process.Start("https://blogtruyen.vn/admin/cap-nhat-chuong/" + ListLink.Items[ListLink.SelectedIndex].ToString());
            }
        }

        private void ListLink_MouseEnter(object sender, EventArgs e)
        {
            if(ListLink.Items.Count > 0)
            {
                tipListLink.SetToolTip(ListLink,(ListLink.SelectedIndex + 1).ToString() + "/" + ListLink.Items.Count.ToString());
            }
            
        }

        private void trModel_ValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(trModel.Value.ToString());    
            Model = trModel.Value;
            tipModel.SetToolTip(trModel, "Độ chính xác đạt " + (90 + trModel.Value) + "%");

        }
      
        private void btnTopMost_Click(object sender, EventArgs e)
        {
            if (TopMost == false)
            {
                TopMost = true;
                btnTopMost.Text = "Bỏ ghim màn hình";
                
                this.Width = 485;
                this.Height = 360;
            }
            else
            {
                TopMost = false;
                btnTopMost.Text = "Ghim lên màn hình";

                this.Width = 875;
                this.Height = 428;
            }
        }

        private void btnQuetBia_Click(object sender, EventArgs e)
        {
            int test = 1;
            Model = trModel.Value;

            try
            {
                Int32 ID1 = Int32.Parse(txtID1.Text);
            }
            catch
            { test = 0; }

            if (txtID2.Text == "" || txtID2.Text == null)
            {
                txtID2.Text = txtID1.Text;
            }

            new Thread(() =>
            {
                if (test == 0)
                { MessageBox.Show("ID truyện không hợp lệ", "Lỗi"); }
                else if (txtID1.Text != "" && txtID1.Text != null && txtID2.Text != "" && txtID2.Text != null)
                {
                    ThreadStart thread = new ThreadStart(KiemTraBia);
                    Thread thrd = new Thread(thread);
                    thrd.IsBackground = true;

                    if (TienTrinhQuetBia == "Chưa chạy")
                    {
                        thrd.Start();

                        ListLink.Items.Clear();

                        TienTrinhQuetBia = "Đang chạy";
                        SoChuongDie = 0;
                        txtCount.Text = "Số lượng truyện lỗi bìa: 0";

                        btnAutoUpdate.Enabled = false;
                        btnQuet.Enabled = false;
                        btnQuetUpdate.Enabled = false;

                        rtbKetQuaQuet.ReadOnly = true;
                        txtID1.ReadOnly = true;
                        txtID2.ReadOnly = true;
                    }
                    else if (TienTrinhQuetBia == "Đang chạy")
                    {
                        TienTrinhQuetBia = "Chưa chạy";
                        thrd.Abort();
                    }

                    try
                    { thrd.Join(); }
                    catch
                    { }

                    KetThuc();

                    rtbStatus.Text = "Đã xong";
                    if (cbNhanThongBao.Checked == true)
                    {
                        MessageBox.Show("Đã quet xong ảnh bìa", "Thông báo");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng điền đủ ID1 và ID2");
                }

            })
            { IsBackground = true }.Start();

            rtbStatus.Focus();
        }

        private void toolDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            Process.Start(".\\ToolDownload.lnk");
        }

        private void rtbKetQuaQuet_TextChanged(object sender, EventArgs e)
        {
            if(cbMultiple.Checked == false)
            {
                File.WriteAllText("./History.txt", rtbKetQuaQuet.Text);
            }           
        }

    }
}
