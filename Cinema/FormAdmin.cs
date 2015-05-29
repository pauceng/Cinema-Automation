using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Cinema
{
    public partial class FormAdmin : Form
    {
        SqlConnection sqlBaglan = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        public FormAdmin()
        {
            InitializeComponent();
            sqlBaglan.ConnectionString = "Data Source=OH-MY-PC;Initial Catalog=Cinema;Integrated Security=True";
            try
            {
                sqlBaglan.Open();
            }
            catch (Exception mesaj)
            {

                MessageBox.Show(mesaj.Message);
            }
            numericUpDKoltuk.Maximum = 15;
            numericUpDKoltuk.Minimum = 7;
            numericUpDSira.Maximum = 7;
            numericUpDSira.Minimum = 3;
        }
        //Tüm Nesneleri temizleyen metot
        public void TemizlikImandandir()
        {//işlem bittikten sonra nesne temizliği bu olsa gerek
            //Film Ekle kısmı
            txtBfilmEkleAd.Clear(); 
            txtBfilmEkleBoyutlu.Clear(); 
            txtBfilmEkleOyuncu.Clear(); 
            txtBfilmEkleDil.Clear(); 
            txtBfilmEkleYonetmen.Clear(); 
            txtBfilmEkleTürü.Clear(); 
            txtBfilmEkleYil.Clear();
            txtBUpdateID.Clear();
            //Kullanıcı Ekle kısmı
            txtBadminKayitAd.Clear();
            txtBadminKayitMail.Clear();
            txtBadminKayitPasswrd.Clear();
            txtBPassGuncelleme.Clear();
            txtBadminKayitTel.Clear();
            txtBadminKayitUserName.Clear();
        }

        private void btnAdminKayitEt_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlBaglan.State == ConnectionState.Closed)
                    sqlBaglan.Open();
                if (string.IsNullOrEmpty(txtBadminKayitAd.Text) || string.IsNullOrEmpty(txtBadminKayitMail.Text) || string.IsNullOrEmpty(txtBadminKayitPasswrd.Text) || string.IsNullOrEmpty(txtBadminKayitUserName.Text)  || string.IsNullOrEmpty(txtBadminKayitMail.Text) )
                {
                    MessageBox.Show("Eksik Bilgi Girdiniz!","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                }
                else
                {
                    // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                    string sqlKayit = "insert into tblKullanici(klcAd,klcUserName,klcParola,klcMail,klcTel,klcID) values(@klcAd,@klcUserName,@klcParola,@klcMail,@klcTel,@klcID)";
                    string sqlKlcID = "SELECT MAX(klcID) FROM tblKullanici";
                    // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                    SqlCommand cmdKayit = new SqlCommand(sqlKayit, sqlBaglan);
                    SqlCommand cmdSonID = new SqlCommand(sqlKlcID, sqlBaglan);
                    int sonID = (int)cmdSonID.ExecuteScalar();
                    //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
                    cmdKayit.Parameters.AddWithValue("@klcAd", txtBadminKayitAd.Text);
                    cmdKayit.Parameters.AddWithValue("@klcUserName", txtBadminKayitUserName.Text);
                    cmdKayit.Parameters.AddWithValue("@klcParola", txtBadminKayitPasswrd.Text);
                    cmdKayit.Parameters.AddWithValue("@klcMail", txtBadminKayitMail.Text);
                    cmdKayit.Parameters.AddWithValue("@klcTel", txtBadminKayitTel.Text);
                    cmdKayit.Parameters.AddWithValue("@klcID", (sonID + 1).ToString());
                    //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                    cmdKayit.ExecuteNonQuery();
                    //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                    sqlBaglan.Close();
                    MessageBox.Show("Kullanıcı Başarı ile Sisteme Kayıt Edildi!","Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    KullanıcıListele();
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show("Sql ile bağlı hata! :" + mesaj.Message);
            }
        }

        private void FormAdmin_Click(object sender, EventArgs e)
        {
            //Film data grid temizliği
            this.dataGridViewFilm.DataSource = null;
            this.dataGridViewFilm.Rows.Clear();
            //Kullanıcı data grid temizliği
            this.dGridViewKlc.DataSource = null;
            this.dGridViewKlc.Rows.Clear();
            //Kullanıcı güncelleme için kullanılacak buton gizleme
            btnKlcGuncelle.Visible = false;
            btnAdminKayitEt.Visible = true;
            txtBAdminSearch.Clear();
            //Kullanıcı combobox sıfırlama
            comboBoxSearch.SelectedIndex = 0;
            //Film combobox sıfırlama
            comboBoxFilmSearch.SelectedIndex = 0;
            TemizlikImandandir();
            txtBPassGuncelleme.Visible = false;
            //Kullanıcı işlem menu label değişikliği
            lblIslem.Text = "işlem";
        }
        
        private void btnAdminFilmEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlBaglan.State == ConnectionState.Closed)
                    sqlBaglan.Open();
                if (string.IsNullOrEmpty(txtBfilmEkleAd.Text) || string.IsNullOrEmpty(txtBfilmEkleYonetmen.Text) || string.IsNullOrEmpty(txtBfilmEkleOyuncu.Text) || string.IsNullOrEmpty(txtBfilmEkleDil.Text) || string.IsNullOrEmpty(txtBfilmEkleTürü.Text))
                {
                    MessageBox.Show("Eksik Bilgi Girdiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                    string sqlKayit = "insert into tblFilm(flmID,flmAd,flmYonetmen,flmOyuncu,flmDil,flmBoyut,flmTuru,flmTarih,flmImg) values(@flmID,@flmAd,@flmYonetmen,@flmOyuncu,@flmDil,@flmBoyut,@flmTuru,@flmTarih,@flmImg)";
                    string sqlflmID = "SELECT MAX(flmID) FROM tblFilm";
                    // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                    SqlCommand cmdFilmEkle = new SqlCommand(sqlKayit, sqlBaglan);
                    SqlCommand cmdSonID = new SqlCommand(sqlflmID, sqlBaglan);
                    int sonID = (int)cmdSonID.ExecuteScalar();
                    MemoryStream stream = new MemoryStream();
                    pictureBoxImdb.Image.Save(stream, pictureBoxImdb.Image.RawFormat);
                    byte[] pic = stream.GetBuffer();
                    //
                    cmdFilmEkle.Parameters.AddWithValue("@flmID", (sonID + 1).ToString());
                    cmdFilmEkle.Parameters.AddWithValue("@flmAd", txtBfilmEkleAd.Text);
                    cmdFilmEkle.Parameters.AddWithValue("@flmYonetmen", txtBfilmEkleYonetmen.Text);
                    cmdFilmEkle.Parameters.AddWithValue("@flmOyuncu", txtBfilmEkleOyuncu.Text);
                    cmdFilmEkle.Parameters.AddWithValue("@flmDil", txtBfilmEkleDil.Text);
                    cmdFilmEkle.Parameters.AddWithValue("@flmBoyut", txtBfilmEkleBoyutlu.Text);
                    cmdFilmEkle.Parameters.AddWithValue("@flmTuru", txtBfilmEkleTürü.Text);
                    cmdFilmEkle.Parameters.AddWithValue("@flmTarih", txtBfilmEkleYil.Text);
                    cmdFilmEkle.Parameters.AddWithValue("@flmImg", pic);
                 //   sqlBaglan.Open();
                    //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                    cmdFilmEkle.ExecuteNonQuery();
                    //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                    sqlBaglan.Close();
                    TemizlikImandandir();
                    comboBoxImdnName.ResetText();
                    comboBoxImdbYil.ResetText();
                    MessageBox.Show("Film Eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show("Sql ile bağlı hata!: " + mesaj.Message);
            }

        }

        private void btnSalonSalonEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlBaglan.State == ConnectionState.Closed)
                    sqlBaglan.Open();

                // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                string sqlKayit = "insert into tblSalon(slnNo,slnSiraSayi,slnKoltukSayi,slnToplam) values(@slnNo,@slnSiraSayi,@slnKoltukSayi,@slnToplam)";
                string sqlslnID = "SELECT MAX(slnNo) FROM tblSalon";
                // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                SqlCommand cmdSalonEkle = new SqlCommand(sqlKayit, sqlBaglan);
                SqlCommand cmdSalonNo = new SqlCommand(sqlslnID, sqlBaglan);
                int SalonNo = (int)cmdSalonNo.ExecuteScalar();
                //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
                cmdSalonEkle.Parameters.AddWithValue("@slnNo", (SalonNo + 1).ToString());
                cmdSalonEkle.Parameters.AddWithValue("@slnSiraSayi", numericUpDSira.Value.ToString());
                cmdSalonEkle.Parameters.AddWithValue("@slnKoltukSayi", numericUpDKoltuk.Value.ToString());
                cmdSalonEkle.Parameters.AddWithValue("@slnToplam", (numericUpDKoltuk.Value * numericUpDKoltuk.Value).ToString());
                //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                cmdSalonEkle.ExecuteNonQuery();
                //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                sqlBaglan.Close();
                MessageBox.Show("Salon Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);SeansListele();
                SalonListele();
                comboUpdate();
            }
            catch (Exception mesaj)
            {
                MessageBox.Show("Sql ile bağlı hata!: " + mesaj.Message);
            }
        }

        private void KullaniciKayitGetir(string sqlaLL)
        {
        //    sqlBaglan.Open();
            SqlCommand cmdKLCgetir = new SqlCommand(sqlaLL, sqlBaglan);
            //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
            SqlDataAdapter da = new SqlDataAdapter(cmdKLCgetir);
            //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
            DataTable dt = new DataTable();
            da.Fill(dt);
            //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
            dGridViewKlc.DataSource = dt;
            //Formumuzdaki DataGridViewin veri kaynağını oluşturduğumuz tablo olarak gösteriyoruz.
            sqlBaglan.Close();
        }

        private void FilmKayitGetir(string sqlAsk)
        {
            SqlCommand cmdFilmgetir = new SqlCommand(sqlAsk, sqlBaglan);
            //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
            SqlDataAdapter da = new SqlDataAdapter(cmdFilmgetir);
            //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
            DataTable dt = new DataTable();
            da.Fill(dt);
            //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
            dataGridViewFilm.DataSource = dt;
            //Formumuzdaki DataGridViewin veri kaynağını oluşturduğumuz tablo olarak gösteriyoruz.
            sqlBaglan.Close();
        }
        //Veritabanındakı Filmleri tekrar listelemek için
        private void FilmListele()
        {
            DataTable tablo = new DataTable();  // tablo isiminde bir Datatable tanımladık.
            tablo.Clear(); //tabloyu temizledik
            SqlDataAdapter adtr = new SqlDataAdapter("Select * From tblFilm", sqlBaglan);
            adtr.Fill(tablo); //adaptördeki verileri tablonun içine doldurduk.
            dataGridViewFilm.DataSource = tablo; //tablodaki verileri datagridview'e aktardık.
        }

        private void BiletListele()
        {
            DataTable tablo = new DataTable();  // tablo isiminde bir Datatable tanımladık.
            tablo.Clear(); //tabloyu temizledik
            SqlDataAdapter adtr = new SqlDataAdapter("Select * From tblBilet", sqlBaglan);
            adtr.Fill(tablo); //adaptördeki verileri tablonun içine doldurduk.
            dataGridView1.DataSource = tablo; //tablodaki verileri datagridview'e aktardık.
        }
        //Veritabanındakı Kullanıcıları tekrar listelemek için
        private void KullanıcıListele()
        {
            DataTable tablo = new DataTable();  // tablo isiminde bir Datatable tanımladık.
            tablo.Clear(); //tabloyu temizledik
            SqlDataAdapter adtr = new SqlDataAdapter("Select * From tblKullanici", sqlBaglan);
            adtr.Fill(tablo); //adaptördeki verileri tablonun içine doldurduk.
            dGridViewKlc.DataSource = tablo; //tablodaki verileri datagridview'e aktardık.
        }
        //Veritabanındakı salonları tekrar listelemek için
        private void SalonListele()
        {
            DataTable tablo = new DataTable();  // tablo isiminde bir Datatable tanımladık.
            tablo.Clear(); //tabloyu temizledik
            SqlDataAdapter adtr = new SqlDataAdapter("Select * From tblSalon", sqlBaglan);
            adtr.Fill(tablo); //adaptördeki verileri tablonun içine doldurduk.
            dataGridViewSalon.DataSource = tablo; //tablodaki verileri datagridview'e aktardık.
        }
        //Veritabanındakı Seansları tekrar listelemek için
        private void SeansListele()
        {
            DataTable tablo = new DataTable();  // tablo isiminde bir Datatable tanımladık.
            tablo.Clear(); //tabloyu temizledik
            SqlDataAdapter adtr = new SqlDataAdapter("Select * From tblSeans", sqlBaglan);
            adtr.Fill(tablo); //adaptördeki verileri tablonun içine doldurduk.
            dataGridViewSeans.DataSource = tablo; //tablodaki verileri datagridview'e aktardık.
        }

        private void btnAdminBul_Click(object sender, EventArgs e)
        {
            KullaniciKayitGetir("select * from tblKullanici");
            if (comboBoxSearch.SelectedIndex == 1)
            {
                //Kullanıcı ismine göre arama gercekleştirilecek
                KullaniciKayitGetir("select * from tblKullanici where klcAd='"+txtBAdminSearch.Text+"'");
            }
            if (comboBoxSearch.SelectedIndex == 2)
            {
                //Kullanıcı kullanıcı adına göre arama işlemi gercekleştirilecektir
                KullaniciKayitGetir("select * from tblKullanici where klcUserName='" + txtBAdminSearch.Text + "'");
            }
            if (comboBoxSearch.SelectedIndex == 3)
            {
                //Kullanıcı kullanıcı mailine göre arama işlemi gercekleştirilecektir
                KullaniciKayitGetir("select * from tblKullanici where klcMail='" + txtBAdminSearch.Text + "'");
            }
            if (comboBoxSearch.SelectedIndex == 4)
            {
                ////Kullanıcı kullanıcı telefonuna göre arama işlemi gercekleştirilecektir
                KullaniciKayitGetir("select * from tblKullanici where klcTel='" + txtBAdminSearch.Text + "'");
            }
        }

        private void FormAdmin_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'biletDataSet.tblBilet' table. You can move, or remove it, as needed.
            this.tblBiletTableAdapter.Fill(this.biletDataSet.tblBilet);
            // TODO: This line of code loads data into the 'dataSetSeans.tblSeans' table. You can move, or remove it, as needed.
            this.tblSeansTableAdapter.Fill(this.dataSetSeans.tblSeans);
            // TODO: This line of code loads data into the 'cinemaDataSet.tblSalon' table. You can move, or remove it, as needed.
            this.tblSalonTableAdapter.Fill(this.cinemaDataSet.tblSalon);
            // TODO: This line of code loads data into the 'dateSetFilm.tblFilm' table. You can move, or remove it, as needed.
            //this.tblFilmTableAdapter.Fill(this.dateSetFilm.tblFilm);
            //dataGridlerde gözüken son satırı göstermiyor
            dGridViewKlc.AllowUserToAddRows = false;
            dataGridViewFilm.AllowUserToAddRows = false;
            dataGridViewSalon.AllowUserToAddRows = false;
            dataGridViewSeans.AllowUserToAddRows = false;

            //Salon No sıra ile arttığından dolayı, yeni salon eklerken otomotik son salon no dan sonrakı salon no sunu verir
            string sqlSalonID = "SELECT MAX(slnNo) FROM tblSalon";
            SqlCommand cmdSalonNo = new SqlCommand(sqlSalonID, sqlBaglan);
            int SalonNo = (int)cmdSalonNo.ExecuteScalar();
            txtBSalonNoEkle.Text = Convert.ToString(SalonNo + 1);
            //Kullanıcı data grid temizliği
            this.dGridViewKlc.DataSource = null;
            this.dGridViewKlc.Rows.Clear();
            //FormAdmin Acıldığında veritabanından salon no ve film isimlerini cobobaxlara yükler.
            comboUpdate();
        }

        public void comboUpdate()
        {
            
            string sqlSalonNo = "select slnNo from tblSalon";
            string sqlFilmAd = "select flmAd from tblFilm";
            string sqlSeans = "select snsSalonNo, snsFilmAd from tblSeans";
            SqlDataAdapter adpSalonNo = new SqlDataAdapter(sqlSalonNo, sqlBaglan);
            SqlDataAdapter adpFilmAd = new SqlDataAdapter(sqlFilmAd, sqlBaglan);
            SqlDataAdapter adpSeans = new SqlDataAdapter(sqlSeans, sqlBaglan);
            DataSet dtSalonNo = new DataSet();
            DataSet dtFilmAd = new DataSet();
            DataSet dtSeans = new DataSet();
            adpSalonNo.Fill(dtSalonNo);
            foreach (DataRow item in dtSalonNo.Tables[0].Rows)
            {
                comboBoxSalon.Items.Add(item["slnNo"].ToString());
            }
            adpFilmAd.Fill(dtFilmAd);
            foreach (DataRow item in dtFilmAd.Tables[0].Rows)
            {
                comboBoxSalonFlm.Items.Add(item["flmAd"].ToString());
            }
            adpSeans.Fill(dtSeans);
            foreach (DataRow item in dtSeans.Tables[0].Rows)
            {
                comboBoxSeansInfo.Items.Add(item["snsSalonNo"].ToString() + "  =  " + item["snsFilmAd"].ToString());
            }
        }

        private void btnSalonSeansEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlBaglan.State == ConnectionState.Closed)
                    sqlBaglan.Open();

                // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                string sqlSeansKayit = "insert into tblSeans(snsID, snsSalonNo,snsFilmAd,snsTarih,snsSaat) values(@snsID,@snsSalonNo,@snsFilmAd,@snsTarih,@snsSaat)";
                string sqlsnsID = "SELECT MAX(snsID) FROM tblSeans";
                // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                SqlCommand cmdSeansEkle = new SqlCommand(sqlSeansKayit, sqlBaglan);
                SqlCommand cmdsnsID = new SqlCommand(sqlsnsID, sqlBaglan);
                int snsID = (int)cmdsnsID.ExecuteScalar();
                //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
                cmdSeansEkle.Parameters.AddWithValue("@snsID", (snsID+1).ToString());
                cmdSeansEkle.Parameters.AddWithValue("@snsSalonNo", comboBoxSalon.SelectedItem.ToString());
                cmdSeansEkle.Parameters.AddWithValue("@snsFilmAd", comboBoxSalonFlm.GetItemText(comboBoxSalonFlm.SelectedItem).ToString());
                cmdSeansEkle.Parameters.AddWithValue("@snsTarih", dateTimePickerSeans.Value);
                cmdSeansEkle.Parameters.AddWithValue("@snsSaat", comboBoxSeansSaat.GetItemText(comboBoxSeansSaat.SelectedItem).ToString());
                //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                cmdSeansEkle.ExecuteNonQuery();
                //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                sqlBaglan.Close();
                MessageBox.Show("Seans Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SeansListele();
                comboUpdate();
            }
            catch (Exception mesaj)
            {
                MessageBox.Show("Sql ile bağlı hata!: " + mesaj.Message);
            }
        }
        //Film Al Butonuna tıklandığunda IDMDB' den film bilgilerini ceken metod
        private void btnIMDBAra_Click(object sender, EventArgs e)
        {
            string url = String.Format("http://imdbapi.com/?t={0}&y={1}&r=xml", txtFilmAra.Text, comboBoxImdbYil.SelectedItem.ToString());
            // XML nesnesini yarat ve url'den yükle
            XmlDocument xml = new XmlDocument();
            xml.Load(url);
            // Bulunan bilgileri form elemanlarına aktar
            foreach (XmlElement item in xml.SelectNodes(@"root/movie"))
            {
                pictureBoxImdb.ImageLocation = item.GetAttribute("poster");
                txtBfilmEkleAd.Text = item.GetAttribute("title");
                txtBfilmEkleOyuncu.Text = item.GetAttribute("actors");
                txtBfilmEkleYonetmen.Text = item.GetAttribute("director");
                txtBfilmEkleTürü.Text = item.GetAttribute("genre");
                txtBfilmEkleBoyutlu.Text = item.GetAttribute("country");
                txtBfilmEkleDil.Text = item.GetAttribute("language");
                txtBfilmEkleYil.Text = comboBoxImdbYil.SelectedItem.ToString();
            }
        }
        //Film Al kısmında comboxdan secilecek filimin yılın otomotik sağ tarafta gözükmesi için idexlerini eşitledim
        private void comboBoxImdnName_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxImdbYil.SelectedIndex = comboBoxImdnName.SelectedIndex; 
        }

        private void txtBAdminSearch_Click(object sender, EventArgs e)
        {
            txtBAdminSearch.Clear();
            //Kullanıcı data grid temizliği
            this.dGridViewKlc.DataSource = null;
            this.dGridViewKlc.Rows.Clear();
        }

        private void btnKlcDuzenle_Click(object sender, EventArgs e)
        {
            //FormAdmin_Click(sender,e);
            btnKlcGuncelle.Visible = true;
            btnAdminKayitEt.Visible = false;
            txtBPassGuncelleme.Visible = true;
            lblIslem.Text = "düzenle";
            try
            {
                if (sqlBaglan.State == ConnectionState.Closed)
                    sqlBaglan.Open();
                DataGridViewCell cell = null;
                foreach (DataGridViewCell selectedCell in dGridViewKlc.SelectedCells)
                {
                    cell = selectedCell;
                    break;
                }
                if (cell != null)
                {
                    dGridViewKlc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    DataGridViewRow row = cell.OwningRow;
                    txtBadminKayitAd.Text = row.Cells[0].Value.ToString();
                    txtBadminKayitUserName.Text = row.Cells[1].Value.ToString();
                    txtBPassGuncelleme.Text = row.Cells[2].Value.ToString();
                    txtBadminKayitMail.Text = row.Cells[3].Value.ToString();
                    txtBadminKayitTel.Text = row.Cells[4].Value.ToString();
                    txtBUpdateID.Text = row.Cells[5].Value.ToString();
                    
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show( mesaj.Message);
            }
        }

        private void btnKlcGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlBaglan.State == ConnectionState.Closed)
                    sqlBaglan.Open();
                string sqlQuery = "UPDATE tblKullanici SET klcAd='" + txtBadminKayitAd.Text + "',klcUserName='" + txtBadminKayitUserName.Text + "',klcParola='" + txtBPassGuncelleme.Text + "',klcMail='" + txtBadminKayitMail.Text + "',klcTel='" + txtBadminKayitTel.Text + "' where klcID=@klcID";
                SqlCommand cmdUpdate = new SqlCommand(sqlQuery, sqlBaglan);
                cmdUpdate.Parameters.AddWithValue("@klcID", txtBUpdateID.Text);
                cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Kullanıcı Bilgileriniz Güncellenmiştir!","Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
                this.tblFilmTableAdapter.Fill(this.dateSetFilm.tblFilm);
                sqlBaglan.Close();
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }

        private void btnFilmSearchBul_Click(object sender, EventArgs e)
        {
            FilmKayitGetir("select * from tblFilm");
            if (comboBoxFilmSearch.SelectedIndex == 1)
            {
                FilmKayitGetir("select * from tblFilm where flmAd='" + txtBfilmSearch + "'");
            }
            if (comboBoxFilmSearch.SelectedIndex == 2)
            {
                FilmKayitGetir("select * from tblFilm where flmOyuncu='" + txtBfilmSearch + "'");
            }
            if (comboBoxFilmSearch.SelectedIndex == 3)
            {
                FilmKayitGetir("select * from tblFilm where flmYonetmen='" + txtBfilmSearch + "'");
            }
            if (comboBoxFilmSearch.SelectedIndex == 4)
            {
                FilmKayitGetir("select * from tblFilm where flmTuru='" + txtBfilmSearch + "'");
            }
        }

        private void txtBfilmSearch_TextChanged(object sender, EventArgs e)
        {
            //Film data grid temizliği
            this.dataGridViewFilm.DataSource = null;
            this.dataGridViewFilm.Rows.Clear();
        }

        private void btnFilmSİL_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Film'i Silmekten Emin misiiniz?", "Kontrol", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (sqlBaglan.State == ConnectionState.Closed)
                        sqlBaglan.Open();
                    string sqlQuery = "Delete from tblFilm Where flmID=@flmID";
                    SqlCommand cmdDelete = new SqlCommand(sqlQuery, sqlBaglan);
                    string seciliID = dataGridViewFilm.CurrentRow.Cells[0].Value.ToString();
                    cmdDelete.Parameters.AddWithValue("@flmID", seciliID);
                    cmdDelete.ExecuteNonQuery();
                    sqlBaglan.Close();
                    MessageBox.Show("Film Başarı ile Silindi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FilmListele();
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }

        private void btnKlcSil_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Kullanıcı'yı Silmekten Emin misiiniz?", "Kontrol", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (sqlBaglan.State == ConnectionState.Closed)
                        sqlBaglan.Open();
                    string sqlQuery = "Delete from tblKullanici Where klcID=@klcID";
                    SqlCommand cmdDelete = new SqlCommand(sqlQuery, sqlBaglan);
                    string seciliID = dGridViewKlc.CurrentRow.Cells[5].Value.ToString();
                    cmdDelete.Parameters.AddWithValue("@klcID", seciliID);
                    cmdDelete.ExecuteNonQuery();
                    sqlBaglan.Close();
                    MessageBox.Show("Kullanıcı Başarı ile Silindi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    KullanıcıListele();
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }

        private void btnSalonSil_Click(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("Salonu'yı Silmekten Emin misiiniz?", "Kontrol", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (sqlBaglan.State == ConnectionState.Closed)
                        sqlBaglan.Open();
                    string seciliID = dataGridViewSalon.CurrentRow.Cells[0].Value.ToString();
                    string sqlQuery = "Delete from tblSalon Where slnNo=@ID";
                    string control = "select snsSalonNo from tblSeans where snsSalonNo='" + seciliID + "'";
                    SqlDataAdapter adp = new SqlDataAdapter(control, sqlBaglan);
                    DataSet dt = new DataSet();
                    string SalonNo = null;
                    adp.Fill(dt);
                    foreach (DataRow item in dt.Tables[0].Rows)
                    {
                        SalonNo = item["snsSalonNo"].ToString();
                    }
                    SqlCommand cmdDelete = new SqlCommand(sqlQuery, sqlBaglan);
                    if (SalonNo == seciliID)
                    {
                        MessageBox.Show("Bu Salonu silemezsiniz, Bu Salonda Seans Mevcut", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                        cmdDelete.Parameters.AddWithValue("@ID", seciliID);
                        cmdDelete.ExecuteNonQuery();
                        MessageBox.Show("Salon Başarı ile Silindi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SalonListele();
                        comboUpdate();
                    }
                    sqlBaglan.Close();
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }

        private void btnSeansSil_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Seansı'yı Silmekten Emin misiiniz?", "Kontrol", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (sqlBaglan.State == ConnectionState.Closed)
                        sqlBaglan.Open();
                    string sqlQuery = "Delete from tblSeans Where snsID=@ID";
                    SqlCommand cmdDelete = new SqlCommand(sqlQuery, sqlBaglan);
                    string seciliID = dataGridViewSeans.CurrentRow.Cells[0].Value.ToString();
                    cmdDelete.Parameters.AddWithValue("@ID", seciliID);
                    cmdDelete.ExecuteNonQuery();
                    sqlBaglan.Close();
                    MessageBox.Show("Seans Başarı ile Silindi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SeansListele();
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }
        Button[,] btn = new Button[70, 70];
        private void btnAdminRezerv_Click(object sender, EventArgs e)
        {
           
        }


        private void btnAdminRezervET_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlBaglan.State == ConnectionState.Closed)
                    sqlBaglan.Open();
                if (string.IsNullOrEmpty(txtSatisIsim.Text) || string.IsNullOrEmpty(txtSatisTel.Text))
                {
                    MessageBox.Show("Eksik Bilgi Girdiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                    string sqlKayit = "insert into tblBilet(bltID,bltMusteri,bltKltNo,bltTip,bltUcret,bltSaat,bltDrum,bltTel,bltSalon) values(@bltID,@bltMusteri,@bltKltNo,@bltTip,@bltUcret,@bltSaat,@bltDrum,@bltTel,@bltSalon)";
                    string sqlBiletID = "SELECT MAX(bltID) FROM tblBilet";
                    // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                    SqlCommand cmdBilet = new SqlCommand(sqlKayit, sqlBaglan);
                    SqlCommand cmdSonID = new SqlCommand(sqlBiletID, sqlBaglan);

                    int sonID = (int)cmdSonID.ExecuteScalar();
                    cmdBilet.Parameters.AddWithValue("@bltID", (sonID + 1).ToString());
                    cmdBilet.Parameters.AddWithValue("@bltMusteri", txtSatisIsim.Text);
                    cmdBilet.Parameters.AddWithValue("@bltKltNo", lblKoltukNo.Text);
                    if (rdOgrenci.Checked == true)
                    { cmdBilet.Parameters.AddWithValue("@bltTip", rdOgrenci.Text); }
                    else
                    { cmdBilet.Parameters.AddWithValue("@bltTip", rdNormal.Text); }

                    cmdBilet.Parameters.AddWithValue("@bltUcret", txtToplamUcret.Text);
                    cmdBilet.Parameters.AddWithValue("@bltSaat", DateTime.Now.ToString("h:mm:ss"));
                    cmdBilet.Parameters.AddWithValue("@bltDrum", "rezervasyon");
                    cmdBilet.Parameters.AddWithValue("@bltTel", txtSatisTel.Text);
                    string seciliID = dataGridViewSeans.CurrentRow.Cells[1].Value.ToString();
                    cmdBilet.Parameters.AddWithValue("@bltSalon", seciliID);
                    //   sqlBaglan.Open();
                    //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                    cmdBilet.ExecuteNonQuery();
                    //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                    sqlBaglan.Close();
                    if (MessageBox.Show("Bilet Rezerve Edildi!", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        //TemizlikImandandir();
                    }
                    else
                    {
                        //  Application.Exit();
                    }

                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show("Sql ile bağlı hata!: " + mesaj.Message);
            }
        }

        private void btnAdminNext1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(6);
            txtToplamUcret.Text = "0";
            string sqlSeans = "select snsSalonNo, snsFilmAd, snsSaat from tblSeans";
            SqlDataAdapter adpSeans = new SqlDataAdapter(sqlSeans, sqlBaglan);
            DataSet dtSeans = new DataSet();

            adpSeans.Fill(dtSeans);
            foreach (DataRow item in dtSeans.Tables[0].Rows)
            {
             //   comboBoxRezervInfo.Items.Add(item["snsSalonNo"].ToString() + " - " + item["snsFilmAd"].ToString() + " - " + item["snsSaat"].ToString());
            }
            try
            {
                //string sonuc = null;
                //txtSatisSeansInfo.Text = fm1.comboBoxHomeSeans.SelectedItem.ToString();
                //string text = fm1.comboBoxHomeSeans.SelectedItem.ToString().Substring(31);
                //if (sqlBaglan.State == ConnectionState.Closed)
                //{ sqlBaglan.Open(); }
                //SqlCommand cmdFilmgetir = new SqlCommand("select flmTuru from tblFilm where flmAd='" + text + "'", sqlBaglan);
                //SqlDataReader sqlReader = cmdFilmgetir.ExecuteReader();
                //while (sqlReader.Read())
                //{
                //    sonuc = sqlReader["flmTuru"].ToString().Substring(0, 6);
                //}
                //sqlBaglan.Close();

            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message);
            }
        }


        private void btnAdminRezerv_Click_2(object sender, EventArgs e)
        {
            tabControl1.SelectTab(5);
            Form1 fm1 = new Form1();
            try
            {

                string salonNo = dataGridViewSeans.CurrentRow.Cells[1].Value.ToString();

                SqlDataAdapter adp = new SqlDataAdapter("select snsSalonNo from tblSeans", sqlBaglan);
                DataSet dt = new DataSet();
                string SalonNo = null;
                adp.Fill(dt);
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    SalonNo = item["snsSalonNo"].ToString();
                }
                int slnID = Convert.ToInt32(salonNo);
                string cmdKoltukSayi = "select slnKoltukSayi from tblSalon where slnNo='" + slnID + "'";
                string cmdSiraSayi = "select slnSiraSayi from tblSalon where slnNo='" + slnID + "'";
                int KoltukSayi = fm1.sqlSayi(cmdKoltukSayi, "slnKoltukSayi");
                int SiraSayi = fm1.sqlSayi(cmdSiraSayi, "slnSiraSayi");
                string cmdBiletKoltukNo = "select bltKltNo from tblBilet";
                int txt = 0, toplamKoltuk = KoltukSayi * SiraSayi;
                int[] Koltuk = new int[toplamKoltuk];
                ArrayList list = new ArrayList();


                SqlDataAdapter adpSeans = new SqlDataAdapter(cmdBiletKoltukNo, sqlBaglan);
                DataSet dtSeans = new DataSet();
                adpSeans.Fill(dtSeans);
                foreach (DataRow item in dtSeans.Tables[0].Rows)
                {
                    list.Add(item["bltKltNo"].ToString());
                }

                for (int i = 0; i < SiraSayi; i++)
                {
                    for (int j = 0; j < KoltukSayi; j++)
                    {
                        txt++;
                        btn[i, j] = new Button();
                        btn[i, j].Location = new Point(j * 35, i * 35 + 15);
                        btn[i, j].Size = new Size(35, 35);
                        btn[i, j].UseVisualStyleBackColor = true;
                        btn[i, j].Name = "btn" + txt.ToString();
                        btn[i, j].FlatStyle = FlatStyle.Popup;
                        btn[i, j].Text = txt.ToString();
                        for (int k = 0; k < list.Count; k++)
                        {
                            if (btn[i, j].Text == list[k].ToString())
                            {
                                btn[i, j].Image = Image.FromFile(@"C:\Users\Mehemmed\Documents\Visual Studio 2013\Projects\Cinema\img\kltSenin.png");
                            }
                            else
                            {
                                btn[i, j].Image = Image.FromFile(@"C:\Users\Mehemmed\Documents\Visual Studio 2013\Projects\Cinema\img\kltSec.png");

                            }
                        }
                        btn[i, j].Click += new EventHandler(this.button2_Click);
                        groupBoxSalonDesing.Controls.Add(btn[i, j]);

                    }
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int satir, sutun;
            sutun = ((Button)sender).Location.X / 35;
            satir = (((Button)sender).Location.Y - 15) / 35;// n location(Column)
            // MessageBox.Show("satır:" + (satir + 1) + " sütun:" + (sutun + 1));
            btn[satir, sutun].Image = Image.FromFile(@"C:\Users\Mehemmed\Documents\Visual Studio 2013\Projects\Cinema\img\kltSatildi.png");
            //    seciliKOltuk[i++] = Convert.ToInt32( btn[satir, sutun].Text);
            lblKoltukNo.Text= btn[satir, sutun].Text;
            Button clickedButton = sender as Button;
        }

        private void rdNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (rdNormal.Checked == true)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) + Convert.ToInt32(labelNormalUcret.Text)).ToString();
            }
            if (rdNormal.Checked == false)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) - Convert.ToInt32(labelNormalUcret.Text)).ToString();
            }
        }

        private void rdOgrenci_CheckedChanged(object sender, EventArgs e)
        {
            if (rdOgrenci.Checked == true)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) + Convert.ToInt32(labelOgrenciUcret.Text)).ToString();
            }
            if (rdOgrenci.Checked == false)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) - Convert.ToInt32(labelOgrenciUcret.Text)).ToString();
            }
        }

        private void btnBiletIptal_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bileti İptal Etmekten Emin misiiniz?", "Kontrol", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (sqlBaglan.State == ConnectionState.Closed)
                        sqlBaglan.Open();
                    string sqlQuery = "Delete from tblBilet Where bltID=@ID";
                    SqlCommand cmdDelete = new SqlCommand(sqlQuery, sqlBaglan);
                    string seciliID = dataGridView1.CurrentRow.Cells[8].Value.ToString();
                    cmdDelete.Parameters.AddWithValue("@ID", seciliID);
                    cmdDelete.ExecuteNonQuery();
                    sqlBaglan.Close();
                    MessageBox.Show("Bilet iptal Edildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BiletListele();
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }



    }
}
