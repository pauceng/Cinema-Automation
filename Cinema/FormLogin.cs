using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Cinema
{    //Data Source=OH-MY-PC;Initial Catalog=Cinema;Integrated Security=True
    public partial class FormLogin : Form
    {
        SqlConnection connect = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        public FormLogin()
        {
            InitializeComponent();
            connect.ConnectionString = "Data Source=OH-MY-PC;Initial Catalog=Cinema;Integrated Security=True";
            try
            {
                connect.Open();
            }
            catch (Exception mesaj)
            {

                MessageBox.Show(mesaj.Message);
            }
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            
        }
        //
        //public void sqlGetir(string kayit)
        //{
        //    connect.Open();
        ////    string kayit = "SELECT * from musteriler";
        //    //musteriler tablosundaki tüm kayıtları çekecek olan sql sorgusu.
        //    SqlCommand komut = new SqlCommand(kayit, connect);
        //    //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
        //    SqlDataAdapter da = new SqlDataAdapter(komut);
        //    //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
        //    DataTable dt = new DataTable();
        //    da.Fill(dt);
        //    //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
        //    dataGridView1.DataSource = dt;
        //    //Formumuzdaki DataGridViewin veri kaynağını oluşturduğumuz tablo olarak gösteriyoruz.
        //    connect.Close();
        //}

        private void lnkLabelSifreAdmin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelAdminSifre.Visible = true ;
        }

        private void lnkLabelSifreUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelUserSifre.Visible = true;
        }

        private void lnkLabelUserGeri_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelUserSifre.Visible = false;
        }

        private void lnkLabelAdminGeri_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelAdminSifre.Visible = false;
        }

        private void btnKayitOlPanelAc_Click(object sender, EventArgs e)
        {
            panelKayitOl.Visible = true;
            panelAdminSifre.Visible = false;
            panelUserSifre.Visible = false;

        }

        private void btnKayitIptal_Click(object sender, EventArgs e)
        {
            panelKayitOl.Visible = false;
        }
        //Kullanıcı şifre unutum. şifre kullanıcı adı bilgilerini öğrenmesi
        private void btnUserNewPassword_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = null, kullaniciMail = null, kullaniciParola = null, kullaniciUserName = null;
                SqlParameter klcMail = new SqlParameter("@144", txtBmailGonderUser.Text);
                sql = "SELECT TOP 1000 [klcUserName],[klcParola],[klcMail] FROM [Cinema].[dbo].[tblKullanici]";
                SqlCommand command = new SqlCommand(sql, connect);
                command.Parameters.Add(klcMail);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                foreach (DataRow item in dataTable.Rows)
                {
                    kullaniciMail = item["klcMail"].ToString();
                    kullaniciParola = item["klcParola"].ToString();
                    kullaniciUserName = item["klcUserName"].ToString();

               //MessageBox.Show(kullaniciMail + kullaniciParola + kullaniciUserName); //Gelen değerlerin kontorlü
                    if ((dataTable.Rows.Count > 0) && kullaniciMail == txtBmailGonderUser.Text)
                    {


                        try
                        {
                            SmtpClient Gonderen = new SmtpClient("smtp.gmail.com");
                            Gonderen.Port = 587;
                            Gonderen.EnableSsl = true;
                            Gonderen.Timeout = 100000;
                            Gonderen.DeliveryMethod = SmtpDeliveryMethod.Network;
                            Gonderen.UseDefaultCredentials = false;
                            Gonderen.Credentials = new NetworkCredential("cenglabcinema@gmail.com", "cinema2015");
                            MailMessage yeniMail = new MailMessage();
                            yeniMail.To.Add(txtBmailGonderUser.Text);
                            yeniMail.From = new MailAddress("cenglabcinema@gmail.com", "Ceng LAB Cinema");
                            yeniMail.Subject = "LAB Cinema Yenilenme";
                            yeniMail.Body = "Şifre : " + kullaniciParola + " \nKullanıcı Adı: " + kullaniciUserName;
                            Gonderen.Send(yeniMail);
                            if (MessageBox.Show("Teşekkürler, Başarı ile tamamlanmıştır.", "İşlem Sonuc", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                            {
                                txtBmailGonderUser.Clear();
                                panelUserSifre.Visible = false;
                            }
                            else
                            {
                                txtBmailGonderUser.Clear();
                                MessageBox.Show("Kayıtlı mail adresi bulunamadı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
       //Admin şifre unuttum, şifre ve kullanıcı adı öğrenme
        private void btnAdminNewPassword_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = null, kullaniciMail = null, kullaniciParola = null, kullaniciUserName = null;
                SqlParameter klcMail = new SqlParameter("@144", txtBmailGonderAdmin.Text);
                sql = "SELECT TOP 20 [adminUserName],[adminParola],[adminMail] FROM [Cinema].[dbo].[tblAdmin]";
                SqlCommand command = new SqlCommand(sql, connect);
                command.Parameters.Add(klcMail);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                foreach (DataRow item in dataTable.Rows)
                {
                    kullaniciMail = item["adminMail"].ToString();
                    kullaniciParola = item["adminParola"].ToString();
                    kullaniciUserName = item["adminUserName"].ToString();

               //     MessageBox.Show(kullaniciMail + kullaniciParola + kullaniciUserName); //Gelen değerlerin kontorlü
                    if ((dataTable.Rows.Count > 0) && kullaniciMail == txtBmailGonderAdmin.Text)
                    {
                        try
                        {
                            SmtpClient Gonderen = new SmtpClient("smtp.gmail.com", 587);
                            Gonderen.EnableSsl = true;
                            Gonderen.Timeout = 100000;
                            Gonderen.DeliveryMethod = SmtpDeliveryMethod.Network;
                            Gonderen.UseDefaultCredentials = false;
                            Gonderen.Credentials = new NetworkCredential("cenglabcinema@gmail.com", "cinema2015");
                            MailMessage yeniMail = new MailMessage();
                            yeniMail.To.Add(txtBmailGonderAdmin.Text);
                            yeniMail.From = new MailAddress("cenglabcinema@gmail.com", "Ceng LAB Cinema");
                            yeniMail.Subject = "LAB Cinema Yenilenme";
                            yeniMail.Body = "Şifre : "+ kullaniciParola +" \nKullanıcı Adı: " + kullaniciUserName;
                            Gonderen.Send(yeniMail);
                            if (MessageBox.Show("Teşekkürler, Başarı ile tamamlanmıştır.", "İşlem Sonuc", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                            {
                                txtBmailGonderAdmin.Clear();
                                panelAdminSifre.Visible = false;
                            }
                            else {
                                txtBmailGonderAdmin.Clear();
                                MessageBox.Show("Kayıtlı mail adresi bulunamadı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        
        }

        private void btnAdminLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = null;
                SqlParameter klcAd = new SqlParameter("@141", txtBadminAd.Text);
                SqlParameter klcParola = new SqlParameter("@142", txtBadminPass.Text);
                sql = "SELECT * FROM tblAdmin WHERE adminUserName = @141 and adminParola = @142";
                SqlCommand command = new SqlCommand(sql, connect);
                command.Parameters.Add(klcAd);
                command.Parameters.Add(klcParola);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (txtBadminAd.Text == "" && txtBadminPass.Text == "")
                {
                    MessageBox.Show("Kullanıcı Adı ve Şifre Giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {

                    if (dataTable.Rows.Count > 0)
                    {
                        
                        FormAdmin formAdmin = new FormAdmin();
                        formAdmin.StartPosition = FormStartPosition.CenterScreen;
                        formAdmin.Show();
                        this.Close();
                        formAdmin.TopMost = true;
                        
                    }
                    else
                    {
                        txtBadminAd.Clear();
                        txtBadminPass.Clear();
                        MessageBox.Show("Kullanıcı Adı ve ya Parola Yanlış!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }
        public string Kullanici;
        private void btnKullancLogin_Click(object sender, EventArgs e)
        {
            
            try
            {
                string sql = null;
                SqlParameter klcAd = new SqlParameter("@141", txtBKullancAd.Text);
                SqlParameter klcParola = new SqlParameter("@142", txtBKullancPass.Text);
                sql = "SELECT * FROM tblKullanici WHERE klcUserName = @141 and klcParola = @142";
                SqlCommand command = new SqlCommand(sql, connect);
                command.Parameters.Add(klcAd);
                command.Parameters.Add(klcParola);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                Kullanici = txtBKullancAd.Text;
                if (txtBKullancAd.Text == "" && txtBKullancPass.Text == "")
                {
                    MessageBox.Show("Kullanıcı Adı ve Şifre Giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {
                    
                    if (dataTable.Rows.Count > 0)
                    {
                       // 
                        this.Close();
                        
                        Kullanici = txtBKullancAd.Text;
                        string sifre = txtBKullancPass.Text;

                        FormAL userForm = new FormAL(Kullanici, sifre);
                        userForm.Show();
                        this.Close();
                        userForm.TopMost = true;
                    }
                    else
                    {
                        Kullanici = null;
                        txtBKullancAd.Clear();
                        txtBKullancPass.Clear();
                        MessageBox.Show("Kullanıcı Adı ve ya Parola Yanlış!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }

        private void btnUserKayitOl_Click(object sender, EventArgs e)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                    connect.Open();
                // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                string sqlKayit = "insert into tblKullanici(klcAd,klcUserName,klcParola,klcMail,klcID,klcTel) values(@klcAd,@klcUserName,@klcParola,@klcMail,@klcID,@klcTel)";
                string sqlKlcID = "SELECT MAX(klcID) FROM tblKullanici";
                // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                SqlCommand cmdKayit = new SqlCommand(sqlKayit, connect);
                SqlCommand cmdSonID = new SqlCommand(sqlKlcID, connect);
                int newId = (int)cmdSonID.ExecuteScalar();
                //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
                cmdKayit.Parameters.AddWithValue("@klcAd", txtBKayitAd.Text);
                cmdKayit.Parameters.AddWithValue("@klcUserName", txtBKayitUserName.Text);
                cmdKayit.Parameters.AddWithValue("@klcParola", txtBKayitPasswrd.Text);
                cmdKayit.Parameters.AddWithValue("@klcMail", txtBKayitMail.Text);
                cmdKayit.Parameters.AddWithValue("@klcTel", txtBKayitTel.Text);
                cmdKayit.Parameters.AddWithValue("@klcID", (newId + 1).ToString());
                //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                cmdKayit.ExecuteNonQuery();
                //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                connect.Close();
                panelKayitOl.Visible = false;
                MessageBox.Show("Müşteri Kayıt İşlemi Gerçekleşti.");
                this.Close();
            }
            catch (Exception mesaj)
            {
                MessageBox.Show("İşlem Sırasında Hata Oluştu." + mesaj.Message);
            }
        }
    }
}
