using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinema
{
    public partial class FormAL : Form
    {
        string KullaniciAD, KullaniciPass, klcID, bltID, drum;
        SqlConnection connect = new SqlConnection();
        public FormAL()
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
        }
        public FormAL(string isim, string pass)
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
            KullaniciAD = isim;
            KullaniciPass = pass;

        }
        private void FormAL_Load(object sender, EventArgs e)
        {
            FormLogin girisFM = new FormLogin();

            //MessageBox.Show(KullaniciAD+ KullaniciPass);
            try
            {
                if (connect.State == ConnectionState.Closed)
                    connect.Open();
                string sql = null;
                SqlParameter klcAd = new SqlParameter("@141", KullaniciAD);
                SqlParameter klcParola = new SqlParameter("@142", KullaniciPass);
                sql = "SELECT * FROM tblKullanici WHERE klcUserName = @141 and klcParola = @142";
                SqlCommand command = new SqlCommand(sql, connect);
                command.Parameters.Add(klcAd);
                command.Parameters.Add(klcParola);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                connect.Close();
                if (connect.State == ConnectionState.Closed)
                    connect.Open();
                string sqlASK = "SELECT * FROM tblKullanici WHERE klcUserName='" + KullaniciAD + "' and klcParola='" + KullaniciPass + "'";
                SqlDataAdapter adpSeans = new SqlDataAdapter(sqlASK, connect);
                DataSet dtSeans = new DataSet();
                adpSeans.Fill(dtSeans);

                foreach (DataRow item in dtSeans.Tables[0].Rows)
                {
                    textBoxAD.Text = item["klcAd"].ToString();
                    txtUserNameInfo.Text = item["klcAd"].ToString();
                    textBoxUserName.Text = item["klcUserName"].ToString();
                    textBoxPass.Text = item["klcParola"].ToString();
                    textBoxMail.Text = item["klcMail"].ToString();
                    textBoxTel.Text = item["klcTel"].ToString();
                    klcID = item["klcID"].ToString();
                }
                connect.Close();
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);

            }

        }

        private void btnKlcGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                    connect.Open();
                string sqlQuery = "UPDATE tblKullanici SET klcAd='" + textBoxAD.Text + "',klcUserName='" + textBoxUserName.Text + "',klcParola='" + textBoxPass.Text + "',klcMail='" + textBoxMail.Text + "',klcTel='" + textBoxTel.Text + "' where klcID=@klcID";
                SqlCommand cmdUpdate = new SqlCommand(sqlQuery, connect);
                cmdUpdate.Parameters.AddWithValue("@klcID", klcID);
                cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Bilgileriniz Güncellenmiştir!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connect.Close();
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }

        private void btnBiletIptal_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bileti İptal Etmekten Emin misiiniz?", "Kontrol", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (connect.State == ConnectionState.Closed)
                        connect.Open();
                    string sqlQuery = "Delete from tblBilet Where bltID=@ID";
                    SqlCommand cmdDelete = new SqlCommand(sqlQuery, connect);
                    cmdDelete.Parameters.AddWithValue("@ID", bltID);
                    cmdDelete.ExecuteNonQuery();
                    connect.Close();
                    MessageBox.Show("Bilet iptal Edildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textFilmAD.Clear();
                    textSalonNo.Clear();
                    textKoltukNo.Clear();
                    textSaat.Clear();
                    textTipi.Clear();
                }
                connect.Close();
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);
            }
        }
        string telefon, ucret,salonNo;
        int salon;
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (connect.State == ConnectionState.Closed)
                    connect.Open();
                string sqlBilet = "SELECT * FROM tblBilet WHERE bltMusteri='" + txtUserNameInfo.Text + "'";
                // SqlDataAdapter adpBlt = new SqlDataAdapter(sqlBilet, connect);
                DataSet dtd = new DataSet();
                SqlCommand cmd = new SqlCommand(sqlBilet, connect);
                SqlDataReader sqlReader = cmd.ExecuteReader();
                // adpBlt.Fill(dtd);


                //  foreach (DataRow item1 in adpBlt.Tables[0].Rows)
                // {
                while (sqlReader.Read())
                {
                    textFilmAD.Text = sqlReader["bltMusteri"].ToString();
                    textSalonNo.Text = sqlReader["bltSalon"].ToString();
                    textKoltukNo.Text = sqlReader["bltKltNo"].ToString();
                    textSaat.Text = sqlReader["bltSaat"].ToString();
                    textTipi.Text = sqlReader["bltTip"].ToString();
                    drum = sqlReader["bltdrum"].ToString();
                    telefon = sqlReader["bltTel"].ToString();
                    bltID = sqlReader["bltID"].ToString();
                    ucret = sqlReader["bltUcret"].ToString();
                   // salonNo = sqlReader["bltSalon"].ToString();
                }
                connect.Close();
                if (drum == "rezervasyon")
                {
                    btnBiletAL.Visible = true;
                }
                if (drum == "satıldı")
                {
                    btnBiletAL.Enabled = false;
                }
            }
            catch (Exception mesaj)
            {
                MessageBox.Show(mesaj.Message);

            }
        }

        private void btnBiletAL_Click(object sender, EventArgs e)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                    connect.Open();
                // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                string sqlKayit = "insert into tblBilet(bltID,bltMusteri,bltKltNo,bltTip,bltUcret,bltSaat,bltDrum,bltTel,bltSalon) values(@bltID,@bltMusteri,@bltKltNo,@bltTip,@bltUcret,@bltSaat,@bltDrum,@bltTel,@bltSalon)";
                string sqlBiletID = "SELECT MAX(bltID) FROM tblBilet";
                // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                SqlCommand cmdBilet = new SqlCommand(sqlKayit, connect);
                SqlCommand cmdSonID = new SqlCommand(sqlBiletID, connect);
                int sonID = (int)cmdSonID.ExecuteScalar();
                cmdBilet.Parameters.AddWithValue("@bltID", (sonID + 1).ToString());
                cmdBilet.Parameters.AddWithValue("@bltMusteri", textFilmAD.Text);
                cmdBilet.Parameters.AddWithValue("@bltKltNo", textKoltukNo.Text);
                cmdBilet.Parameters.AddWithValue("@bltTip", textTipi.Text);
                cmdBilet.Parameters.AddWithValue("@bltUcret", ucret);
                cmdBilet.Parameters.AddWithValue("@bltSaat", textSaat.Text);
                cmdBilet.Parameters.AddWithValue("@bltDrum", "satıldı");
                cmdBilet.Parameters.AddWithValue("@bltTel", telefon);
                cmdBilet.Parameters.AddWithValue("@bltSalon", Convert.ToInt32(textSalonNo.Text));
                //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                cmdBilet.ExecuteNonQuery();
                //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                connect.Close();
                MessageBox.Show("Bileti Aldınız!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception mesaj)
            {
                MessageBox.Show("Sql ile bağlı hata!: " + mesaj.Message);
            }
        }
    }
}
