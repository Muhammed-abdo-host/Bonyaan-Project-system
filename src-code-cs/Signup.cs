using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace bonyaan_system
{
    public partial class Signup: Form
    {
        public Signup()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            try
            {
                string connectionString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Users (Username, Password, Email, Phone) VALUES (@user, @pass, @email, @phone)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", txtUser.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txtPass.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text); 
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery(); 
                    conn.Close();

                    MessageBox.Show("تم إنشاء الحساب بنجاح! تقدر تدخل دلوقتي.");
                    this.Close(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حصلت مشكلة: " + ex.Message);
            }
        }

        private void Signup_Load(object sender, EventArgs e)
        {

        }
    }
}
