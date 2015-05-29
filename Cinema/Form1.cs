using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinema
{
    public partial class Form1 : Form
    {
        SqlConnection sqlBaglan = new SqlConnection();
        public Form1()
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
        }
        int[] seciliKOltuk = new int[80];
        private void Form1_Load(object sender, EventArgs e)
        {
            //// TODO: This line of code loads data into the 'frm1SeansDataSet.tblSeans' table. You can move, or remove it, as needed.
            //this.tblSeansTableAdapter.Fill(this.frm1SeansDataSet.tblSeans);
            //// TODO: This line of code loads data into the 'frm1FilmDataSet.tblFilm' table. You can move, or remove it, as needed.
            //this.tblFilmTableAdapter.Fill(this.frm1FilmDataSet.tblFilm);
            dataGridViewHomeFilm.AllowUserToAddRows = false;
            comboBxHmAraTuru.SelectedIndex = 0;
            FormAdmin m = new FormAdmin();
          //  m.ShowDialog();
        }

        private void Temizlik()
        {
            txtHomeSearch.Clear();
            dataGridViewHomeFilm.Visible = false;
            comboBoxHomeSeans.Visible = false;
            btnBiletAl.Visible = false;
            lblSlogan.Visible = true;
            lblFilmSeans.Visible = false;
            pictureBxHomePoster.Visible = false;
            btnBiletAl.Visible = false;
            comboBoxHomeSeans.Items.Clear();
            tabControlbiletAl.Visible = false;
            btnGirisMenu.Enabled = true;
          //  checkBox3D.Enabled = true;
           // tabControlbiletAl.SelectTab(0);
        }
        private void btnGirisMenu_Click(object sender, EventArgs e)
        {
            Temizlik();
            FormLogin fmLogin = new FormLogin();
            fmLogin.ShowDialog();
        }
        //Gönderilen komuta göre film getiren metot
        private void FilmTypeGetir(string sqlAsk)
        {
            if (sqlBaglan.State == ConnectionState.Closed)
                sqlBaglan.Open();
            SqlCommand cmdFilmgetir = new SqlCommand(sqlAsk, sqlBaglan);
            //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
            SqlDataAdapter da = new SqlDataAdapter(cmdFilmgetir);
            //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
            DataTable dt = new DataTable();
            da.Fill(dt);
            //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
            dataGridViewHomeFilm.DataSource = dt;
            //Formumuzdaki DataGridViewin veri kaynağını oluşturduğumuz tablo olarak gösteriyoruz.
            sqlBaglan.Close();
        }

        private void SenasAtama(string sqlAsk)
        {
            comboBoxHomeSeans.Items.Clear();
            if (sqlBaglan.State == ConnectionState.Closed)
                sqlBaglan.Open();
            SqlCommand cmdFilmgetir = new SqlCommand(sqlAsk, sqlBaglan);
            SqlDataReader sqlReader = cmdFilmgetir.ExecuteReader();
            while (sqlReader.Read())
            {
                comboBoxHomeSeans.Items.Add(sqlReader["snsSalonNo"].ToString() + "   -   " + sqlReader["snsSaat"].ToString() + "   -   " + sqlReader["snsFilmAd"].ToString());
            }
            sqlBaglan.Close();
        }

        public int sqlSayi(string sqlAsk, string Sutun)
        {
            int sonuc = 0;
            if (sqlBaglan.State == ConnectionState.Closed)
                sqlBaglan.Open();
            SqlCommand cmdFilmgetir = new SqlCommand(sqlAsk, sqlBaglan);
            SqlDataReader sqlReader = cmdFilmgetir.ExecuteReader();
            while (sqlReader.Read())
            {
                sonuc = Convert.ToInt32(sqlReader[Sutun].ToString());
            }
            sqlBaglan.Close();
            return sonuc;
        }


        private void btnHomeSearch_Click(object sender, EventArgs e)
        {
            dataGridViewHomeFilm.Visible = true;
            comboBoxHomeSeans.Visible = true;
            btnBiletAl.Visible = true;
            lblSlogan.Visible = false;
            lblFilmSeans.Visible = true;
            FilmTypeGetir("select * from tblFilm");
            if (comboBxHmAraTuru.SelectedIndex == 0)
            {
                FilmTypeGetir("select * from tblFilm");
                txtHomeSearch.Clear();
            }
            if (comboBxHmAraTuru.SelectedIndex == 1)
            {
                FilmTypeGetir("select flmAd from tblFilm where flmAd='" + txtHomeSearch.Text + "'");
            }
            if (comboBxHmAraTuru.SelectedIndex == 2)
            {
                FilmTypeGetir("select flmOyuncu from tblFilm where flmOyuncu='" + txtHomeSearch.Text + "'");
            }
            if (comboBxHmAraTuru.SelectedIndex == 3)
            {
                FilmTypeGetir("select flmYonetmen from tblFilm where flmYonetmen='" + txtHomeSearch.Text + "'");
            }
            if (comboBxHmAraTuru.SelectedIndex == 4)
            {
                FilmTypeGetir("select flmTuru from tblFilm where flmTuru='" + txtHomeSearch.Text + "'");
            }
            if (comboBxHmAraTuru.SelectedIndex == 5)
            {
                FilmTypeGetir("select flmDil from tblFilm where flmDil='" + txtHomeSearch + "'");
            }
        }

        private void dataGridViewHomeFilm_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                pictureBxHomePoster.Visible = true;
                MemoryStream ms = new MemoryStream((byte[])dataGridViewHomeFilm.CurrentRow.Cells[8].Value);
                pictureBxHomePoster.Image = Image.FromStream(ms);

            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message);
            }
            string SelectedText = dataGridViewHomeFilm.CurrentRow.Cells[0].Value.ToString();
            string komut = "select snsFilmAd,snsSalonNo,snsSaat from tblSeans where snsFilmAd='"+SelectedText+"'";
            SenasAtama(komut);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            Temizlik();
        }

        Button[,] btn = new Button[70, 70];
        int p = 0;
        private void btnBiletAl_Click(object sender, EventArgs e)
        {
            btnGirisMenu.Enabled = false;
            try
            {
                tabControlbiletAl.Visible = true;
                pictureBxHomePoster.Visible = false;
                SqlDataAdapter adp = new SqlDataAdapter("select snsSalonNo from tblSeans", sqlBaglan);
                DataSet dt = new DataSet();
                string SalonNo = null;
                adp.Fill(dt);
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    SalonNo = item["snsSalonNo"].ToString();
                }
                int slnID = Convert.ToInt32(SalonNo);
                string cmdKoltukSayi = "select slnKoltukSayi from tblSalon where slnNo='" + slnID + "'";
                string cmdSiraSayi = "select slnSiraSayi from tblSalon where slnNo='" + slnID + "'";
                int KoltukSayi = sqlSayi(cmdKoltukSayi, "slnKoltukSayi");
                int SiraSayi = sqlSayi(cmdSiraSayi, "slnSiraSayi");
                //
                string cmdBiletKoltukNo = "select bltKltNo from tblBilet";
                //
                int txt = 0, toplamKoltuk = KoltukSayi * SiraSayi;
                string[] Koltuk = new string[toplamKoltuk];
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
                        btn[i, j].Name = "btn"+txt.ToString();
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
            catch (Exception)
            {
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            int satir, sutun;
            sutun = ((Button)sender).Location.X / 35;
            satir = (((Button)sender).Location.Y - 15) / 35;// n location(Column)
          //  MessageBox.Show("satır:" + (satir + 1) + " sütun:" + (sutun + 1));
            btn[satir, sutun].Image = Image.FromFile(@"C:\Users\Mehemmed\Documents\Visual Studio 2013\Projects\Cinema\img\kltSatildi.png");
            btnNext1.Enabled = true;
            lblKoltukNo.Text = btn[satir, sutun].Text;
        }       

        private void btnNext1_Click(object sender, EventArgs e)
        {
            txtToplamUcret.Text = "0";
            try
            {
                string sonuc = null;
                tabControlbiletAl.SelectTab(1);
                txtSatisSeansInfo.Text = comboBoxHomeSeans.SelectedItem.ToString();
                string text = comboBoxHomeSeans.SelectedItem.ToString().Substring(31);
                if (sqlBaglan.State == ConnectionState.Closed)
                { sqlBaglan.Open(); }
                SqlCommand cmdFilmgetir = new SqlCommand("select flmTuru from tblFilm where flmAd='" + text + "'", sqlBaglan);
                SqlDataReader sqlReader = cmdFilmgetir.ExecuteReader();
                while (sqlReader.Read())
                {
                    sonuc = sqlReader["flmTuru"].ToString().Substring(0, 6);
                }
                sqlBaglan.Close();
                // MessageBox.Show(sonuc);
                if (sonuc.Equals("Action"))
                {
                    if (MessageBox.Show("Bu film 3D gözlük gerektirir, Gözlük ile temin edilmek için \'Evet\' seciniz, Ücret 4TL", "Bilgiledirme", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        labelOgrenciUcret.Text = "10";
                        labelNormalUcret.Text = "14";
                        checkBox3D.Checked = true;
                       checkBox3D.Enabled = true;
                    }
                }
                else
                {
                    checkBox3D.Enabled = false;
                }
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message);
            }
        }

        private void btnSatisBiletiAl_Click(object sender, EventArgs e)
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
                    cmdBilet.Parameters.AddWithValue("@bltSalon", "7");
                    if (rdOgrenci.Checked == true)
                    { cmdBilet.Parameters.AddWithValue("@bltTip", rdOgrenci.Text); }
                    else
                    { cmdBilet.Parameters.AddWithValue("@bltTip", rdNormal.Text); }

                    cmdBilet.Parameters.AddWithValue("@bltUcret", txtToplamUcret.Text);
                    cmdBilet.Parameters.AddWithValue("@bltSaat", DateTime.Now.ToString("h:mm:ss"));
                    if (rdRezervasyon.Checked == true) { cmdBilet.Parameters.AddWithValue("@bltDrum", rdRezervasyon.Text); }
                    else { cmdBilet.Parameters.AddWithValue("@bltDrum", "satıldı"); }
                    cmdBilet.Parameters.AddWithValue("@bltTel", txtSatisTel.Text);
                    //   sqlBaglan.Open();
                    //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                    cmdBilet.ExecuteNonQuery();
                    //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                    sqlBaglan.Close();
                    if (rdSatis.Checked == true)
                    {
                        if (MessageBox.Show("Biletiniz Rezerve Edildi! Teşekkürler.", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            Temizlik();
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("İyi seyirler Dileriz!", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            Temizlik();
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show("Sql ile bağlı hata!: " + mesaj.Message);
            }

        }

        private void rdNormal_CheckedChanged(object sender, EventArgs e)
        {   
            if (rdNormal.Checked == true)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) + Convert.ToInt32(labelNormalUcret.Text)).ToString();
            }
            if(rdNormal.Checked == false)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) - Convert.ToInt32(labelNormalUcret.Text)).ToString();
            }
            
        }

        private void checkBox3D_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3D.Checked == true)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) + 4).ToString();
            }
            if (checkBox3D.Checked == false)
            {
                txtToplamUcret.Text = (Convert.ToInt32(txtToplamUcret.Text) - 4).ToString();
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


    }
}
