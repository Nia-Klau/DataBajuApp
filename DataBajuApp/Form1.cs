using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DataBajuApp
{
    public partial class Form1 : Form
    {
        private List<Baju> daftarBaju = new List<Baju>();
        private int selectedIndex = -1;

        private string connectionString = "server=localhost;database=databaju;uid=root;pwd=;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CekKoneksiDatabase();
            LoadDataFromDatabase();
        }

        private void CekKoneksiDatabase()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Koneksi ke database berhasil.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal koneksi ke database:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadDataFromDatabase()
        {
            daftarBaju.Clear();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Baju";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    daftarBaju.Add(new Baju
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nama = reader["Nama"].ToString(),
                        Ukuran = reader["Ukuran"].ToString(),
                        Warna = reader["Warna"].ToString(),
                        Harga = Convert.ToDecimal(reader["Harga"])
                    });
                }

                conn.Close();
            }

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dgvBaju.AutoGenerateColumns = false;
            dgvBaju.Columns.Clear();

            dgvBaju.DataSource = null;
            dgvBaju.DataSource = daftarBaju;

            // Kolom manual untuk menghindari bentrok property
            dgvBaju.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", ReadOnly = true });
            dgvBaju.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nama", HeaderText = "Nama" });
            dgvBaju.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Ukuran", HeaderText = "Ukuran" });
            dgvBaju.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Warna", HeaderText = "Warna" });
            dgvBaju.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Harga", HeaderText = "Harga" });
        }

        private void ClearForm()
        {
            txtNama.Text = "";
            txtUkuran.Text = "";
            txtWarna.Text = "";
            txtHarga.Text = "";
            selectedIndex = -1;
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNama.Text) ||
                string.IsNullOrWhiteSpace(txtUkuran.Text) ||
                string.IsNullOrWhiteSpace(txtWarna.Text) ||
                string.IsNullOrWhiteSpace(txtHarga.Text))
            {
                MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal harga = decimal.TryParse(txtHarga.Text, out decimal h) ? h : 0;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO Baju (Nama, Ukuran, Warna, Harga) VALUES (@Nama, @Ukuran, @Warna, @Harga)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@Ukuran", txtUkuran.Text);
                cmd.Parameters.AddWithValue("@Warna", txtWarna.Text);
                cmd.Parameters.AddWithValue("@Harga", harga);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadDataFromDatabase();
            ClearForm();
            MessageBox.Show("Data berhasil ditambahkan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0)
            {
                Baju selected = daftarBaju[selectedIndex];

                decimal harga = decimal.TryParse(txtHarga.Text, out decimal h) ? h : 0;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = "UPDATE Baju SET Nama=@Nama, Ukuran=@Ukuran, Warna=@Warna, Harga=@Harga WHERE Id=@Id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@Ukuran", txtUkuran.Text);
                    cmd.Parameters.AddWithValue("@Warna", txtWarna.Text);
                    cmd.Parameters.AddWithValue("@Harga", harga);
                    cmd.Parameters.AddWithValue("@Id", selected.Id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadDataFromDatabase();
                ClearForm();
                MessageBox.Show("Data berhasil diperbarui.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0)
            {
                Baju selected = daftarBaju[selectedIndex];

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = "DELETE FROM Baju WHERE Id=@Id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", selected.Id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadDataFromDatabase();
                ClearForm();
                MessageBox.Show("Data berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void dgvBaju_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedIndex = e.RowIndex;
                var baju = daftarBaju[selectedIndex];

                txtNama.Text = baju.Nama;
                txtUkuran.Text = baju.Ukuran;
                txtWarna.Text = baju.Warna;
                txtHarga.Text = baju.Harga.ToString();
            }
        }
    }

    public class Baju
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public string Ukuran { get; set; }
        public string Warna { get; set; }
        public decimal Harga { get; set; }
    }
}
