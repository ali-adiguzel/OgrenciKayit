using System;
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


namespace OgrKayit
{
    public partial class Form1 : Form
    {
        string resimUrl = null;
        
        SqlConnection conn = new SqlConnection(OgrKayit.Properties.Settings.Default.OgrenciDBConnectionString);

        void baglantiAc()
        {
            conn.Open();
        }

        void baglantiKapat()
        {
            conn.Close();
        }

        public Form1()
        {
            InitializeComponent();          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ogrencilerTableAdapter.Fill(this.ogrenciDBDataSet.Ogrenciler);
            
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            Image img = pictureBox1.Image;
            byte[] arr;

            ImageConverter converter = new ImageConverter();
            arr = (byte[])converter.ConvertTo(img, typeof(byte[]));

            try
            {
                baglantiAc();
                string insertKmt = "INSERT into Ogrenciler(OgrAd,OgrSoyad,OgrTc,OgrDogum,OgrFoto) VALUES(@Ad, @Soyad, @Tc, @Dogum, @Foto)";
                SqlCommand cmd = new SqlCommand(insertKmt, conn);

                cmd.Parameters.AddWithValue("@Ad", txtAd.Text);
                cmd.Parameters.AddWithValue("@Soyad", txtSoyad.Text);

                if (txtTc.Text.Length == 11)
                {
                    cmd.Parameters.AddWithValue("@Tc", txtTc.Text);
                }
                else
                {
                    MessageBox.Show("Tc No 11 hane olmalı!", "Tc No Yanlış!");
                }
                
                if (arr == null)
                {
                    int resimYok = 0;
                    arr = (byte[])converter.ConvertTo(resimYok, typeof(byte[]));
                }
                cmd.Parameters.AddWithValue("@Dogum", dogumTarihSec.Value);
                cmd.Parameters.AddWithValue("@Foto", arr);

                cmd.ExecuteNonQuery();
                
                MessageBox.Show("Öğrenci kayıt edildi.");

                this.ogrencilerTableAdapter.Fill(this.ogrenciDBDataSet.Ogrenciler);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baglantiKapat();
            }
        }

        private void btnSec_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog fotoSec = new OpenFileDialog())
            {
                if (fotoSec.ShowDialog()==DialogResult.OK)
                {
                    resimUrl = fotoSec.FileName;
                    pictureBox1.Image = Image.FromFile(fotoSec.FileName);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            
        }

        public Image resimGetir(byte[] arr)
        {
            using (MemoryStream hafiza = new MemoryStream(arr))
            {
                return Image.FromStream(hafiza);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                
                txtAd.Text = (string)row.Cells[0].Value;
                txtSoyad.Text = (string)row.Cells[1].Value;
                txtTc.Text = row.Cells[2].Value.ToString();
                dogumTarihSec.Value = (DateTime)row.Cells[3].Value;

                pictureBox1.Image = resimGetir((byte[])row.Cells[4].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                baglantiAc();
                string insertKmt = "DELETE from Ogrenciler WHERE OgrID = @id";
                SqlCommand cmd = new SqlCommand(insertKmt, conn);

                cmd.Parameters.AddWithValue("@id", dataGridView1.SelectedRows[0].Cells[5].Value);
                cmd.ExecuteNonQuery();

                this.ogrencilerTableAdapter.Fill(this.ogrenciDBDataSet.Ogrenciler);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baglantiKapat();
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                baglantiAc();
                string insertKmt = "UPDATE Ogrenciler SET OgrAd = @ad, OgrSoyad = @soyad, OgrTc = @tc, OgrDogum = @dogum, OgrFoto = @foto WHERE OgrId = @id";
                SqlCommand cmd = new SqlCommand(insertKmt, conn);

                Image img = pictureBox1.Image;
                byte[] arr;

                ImageConverter converter = new ImageConverter();
                arr = (byte[])converter.ConvertTo(img, typeof(byte[]));

                cmd.Parameters.AddWithValue("@ad", txtAd.Text);
                cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                cmd.Parameters.AddWithValue("@tc", txtTc.Text);
                cmd.Parameters.AddWithValue("@dogum", dogumTarihSec.Value);
                cmd.Parameters.AddWithValue("@foto", arr);
                cmd.Parameters.AddWithValue("@id", dataGridView1.SelectedRows[0].Cells[5].Value);
                cmd.ExecuteNonQuery();

                this.ogrencilerTableAdapter.Fill(this.ogrenciDBDataSet.Ogrenciler);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baglantiKapat();
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = dataGridView1.DataSource;
            bs.Filter = "OgrAd" + " like '%" + aratxt.Text + "%'";
            dataGridView1.DataSource = bs;
        }
    }
}
