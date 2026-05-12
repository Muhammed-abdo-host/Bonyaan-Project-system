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

namespace bonyaan_system
{
    public partial class Employees: Form
    {
        // غير المسار ده بمسار قاعدة البيانات اللي عندك بالظبط
        string connString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";
        public Employees()
        {
            InitializeComponent();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // لاحظ الأسماء بقت هي هي اللي في الـ SQL بالظبط
                string query = "INSERT INTO Employees (FullName, JobTitle, PhoneNumber, Email, Password) " +
                               "VALUES (@fname, @title, @phone, @email, @pass)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@fname", txtFullName.Text);
                cmd.Parameters.AddWithValue("@title", comboBoxRole.Text);
                cmd.Parameters.AddWithValue("@phone", txtNumber.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم الحفظ بنجاح! مبروك يا هندسة، السيستم نطق.");
                    LoadEmployeesData(); // تحديث الجدول
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حصل خطأ بسيط: " + ex.Message);
                }
            }
        }

        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Employees_Load(object sender, EventArgs e)
        {

        }
        private void LoadEmployeesData()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // الأسماء هنا لازم تكون نفس أسماء الـ Script اللي عملنا بيه الجدول
                string query = "SELECT EmployeeID, FullName, JobTitle, PhoneNumber, Email FROM Employees";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt; // تأكد إن ده اسم الـ DataGridView عندك
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في تحميل الموظفين: " + ex.Message);
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // الاستعلام بالأسماء الجديدة
                string query = "UPDATE Employees SET FullName=@fname, JobTitle=@title, PhoneNumber=@phone, Email=@email, Password=@pass " +
                               "WHERE EmployeeID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@fname", txtFullName.Text);
                cmd.Parameters.AddWithValue("@title", comboBoxRole.Text);
                cmd.Parameters.AddWithValue("@phone", txtNumber.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                // تعديل مهم: سحب الـ ID عن طريق رقم العمود (0) بدل الاسم
                if (dataGridView1.CurrentRow != null)
                {
                    // Cells[0] تعني أول عمود في الجدول وهو الـ EmployeeID
                    cmd.Parameters.AddWithValue("@id", dataGridView1.CurrentRow.Cells[0].Value);
                }
                else
                {
                    MessageBox.Show("من فضلك اختر موظفاً من الجدول أولاً.");
                    return;
                }

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم التعديل بنجاح!");
                        LoadEmployeesData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل أنت متأكد من حذف هذا الموظف؟", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "DELETE FROM Employees WHERE Email=@email";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم حذف الموظف بنجاح!");
                        LoadEmployeesData(); // تحديث الجدول
                        txtFullName.Clear();
                        txtEmail.Clear();
                        txtPassword.Clear();
                        txtNumber.Clear();
                        comboBoxRole.SelectedIndex = -1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("خطأ في الحذف: " + ex.Message);
                    }
                }
            }
        }

        private void btnReset1_Click(object sender, EventArgs e)
        {
            txtFullName.Clear();
            
            txtNumber.Clear();
            txtEmail.Clear();
            txtPassword.Clear();

            // إعادة الكومبو بوكس للحالة الأولى (بدون اختيار)
            comboBoxRole.SelectedIndex = -1;

            // (اختياري) إلغاء تحديد أي صف في الجدول
            if (dataGridView1.CurrentRow != null)
            {
                dataGridView1.ClearSelection();
            }

            // (اختياري) لو عايز ترجع المؤشر لأول خانة فوراً
            txtFullName.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // استعلام ذكي: بيبحث بالخانات اللي اتكتب فيها بس ويطنش الفاضي
                string query = "SELECT EmployeeID, FullName, JobTitle, PhoneNumber, Email FROM Employees " +
                               "WHERE (Email LIKE @email OR @email = '%%') " +
                               "AND (PhoneNumber LIKE @phone OR @phone = '%%') " +
                               "AND (JobTitle LIKE @role OR @role = '')";

                SqlCommand cmd = new SqlCommand(query, conn);

                // اربطي كل بارامتر بالخانة بتاعته في groupBox2
                // غيري الأسامي دي (txtSearchEmail، إلخ) لأسامي الأدوات الحقيقية عندك
                cmd.Parameters.AddWithValue("@email", "%" + txtSearchEmail.Text + "%");
                cmd.Parameters.AddWithValue("@phone", "%" + txtSearchNumber.Text + "%");
                cmd.Parameters.AddWithValue("@role", comboBoxSearchRole.Text);

                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("مفيش موظف بالبيانات دي يا هندسة.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في البحث: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // التأكد إن الضغطة مش على رأس الجدول وإن الصف فيه بيانات
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells[0].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // الأسماء هنا مطابقة لجدول الـ SQL الجديد اللي عملناه
                txtFullName.Text = row.Cells["FullName"].Value.ToString();
                comboBoxRole.Text = row.Cells["JobTitle"].Value.ToString(); // غيرنا Role لـ JobTitle
                txtNumber.Text   = row.Cells["PhoneNumber"].Value.ToString(); // غيرنا Number لـ PhoneNumber
                txtEmail.Text    = row.Cells["Email"].Value.ToString();

                // ملاحظة: الباسورد غالباً مش بنعرضه في الـ Grid لأسباب أمنية
                // لو محتاج تجيبه لازم تكون مختاره في جملة الـ Select في ميثود Load
            }
        }

        private void but_Click(object sender, EventArgs e)
        {
            // 1. تفضية الخانات اللي في الجزء بتاع البحث (groupBox2)
            txtSearchEmail.Clear();
            txtSearchNumber.Clear();
            comboBoxSearchRole.SelectedIndex = -1;

            // 2. إعادة تحميل كل الموظفين في الجدول عشان نلغي الفلتر
            LoadEmployeesData();

            // 3. (اختياري) رسالة صغيرة أو Focus على أول خانة
            txtSearchEmail.Focus();
        }
    }
    }

