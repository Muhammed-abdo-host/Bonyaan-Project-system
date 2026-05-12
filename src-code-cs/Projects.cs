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
using System.Windows.Forms.VisualStyles;

namespace bonyaan_system
{
    public partial class Projects: Form
    {
        string connString = @"Data Source=.\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";
        public Projects()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Projects_Load(object sender, EventArgs e)
        {
            comboBoxStatus.Items.Clear();

            // إضافة الخيارات
            comboBoxStatus.Items.Add("تحت الإنشاء");
            comboBoxStatus.Items.Add("مكتمل");
            comboBoxStatus.Items.Add("متوقف مؤقتاً");

            // جعل أول خيار هو الظاهر افتراضياً
            comboBoxStatus.SelectedIndex = 0;
            LoadProjectsData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. التأكد من إدخال رقم عميل
            if (string.IsNullOrEmpty(txtCustomerID.Text))
            {
                MessageBox.Show("من فضلك أدخل رقم العميل أولاً");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // 1. أولاً: نضيف العميل في جدول Customers عشان SQL ما يرفضش
                string addCustomerQuery = "IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerID = @custID) " +
                                          "INSERT INTO Customers (CustomerID, FullName) VALUES (@custID, 'عميل مؤقت')";

                SqlCommand cmdCust = new SqlCommand(addCustomerQuery, conn);
                cmdCust.Parameters.AddWithValue("@custID", txtCustomerID.Text);
                cmdCust.ExecuteNonQuery();

                // 2. ثانياً: نضيف المشروع عادي
                string query = "INSERT INTO Projects (ProjectName, Location, ProjectStatus, StartDate, EndDate, TotalBudget, CustomerID) " +
                               "VALUES (@name, @loc, @status, @start, @end, @budget, @custID)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", txtProjectName.Text);
                cmd.Parameters.AddWithValue("@loc", txtLocation.Text);
                cmd.Parameters.AddWithValue("@status", comboBoxStatus.Text);
                cmd.Parameters.AddWithValue("@start", dateTimePickerStart.Value);
                cmd.Parameters.AddWithValue("@end", dateTimePickerEnd.Value);
                cmd.Parameters.AddWithValue("@budget", numericBudget.Value);
                cmd.Parameters.AddWithValue("@custID", txtCustomerID.Text);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم إضافة المشروع والعميل بنجاح!");
                    LoadProjectsData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }
        private void LoadProjectsData()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string query = "SELECT ProjectID, ProjectName, Location, ProjectStatus, TotalBudget FROM Projects";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    // رسالة تنبيه لو فيه مشكلة في الاتصال
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtProjectName.Clear();
            txtLocation.Clear();
            txtCustomerID.Clear();
            comboBoxStatus.SelectedIndex = -1; // بيرجع الكومبو بوكس فاضي
            dateTimePickerStart.Value = DateTime.Now;
            dateTimePickerEnd.Value = DateTime.Now;
            numericBudget.Value = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // بنحدث البيانات بناءً على الاسم (تأكد من مطابقة أسماء الأعمدة لجدول SQL)
                string query = "UPDATE Projects SET Location=@loc, ProjectStatus=@status, TotalBudget=@budget, StartDate=@start, EndDate=@end WHERE ProjectName=@name";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", txtProjectName.Text);
                cmd.Parameters.AddWithValue("@loc", txtLocation.Text);
                cmd.Parameters.AddWithValue("@status", comboBoxStatus.Text);
                cmd.Parameters.AddWithValue("@budget", numericBudget.Value);
                cmd.Parameters.AddWithValue("@start", dateTimePickerStart.Value);
                cmd.Parameters.AddWithValue("@end", dateTimePickerEnd.Value);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("تم تحديث بيانات المشروع بنجاح!");
                        LoadProjectsData(); // تحديث الجدول
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في التعديل: " + ex.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("هل أنت متأكد من حذف هذا المشروع؟", "تأكيد الحذف", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "DELETE  FROM Projects WHERE ProjectName=@name";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtProjectName.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم حذف المشروع بنجاح");
                    LoadProjectsData(); // تحديث الجدول
                    ClearFields();
                }
            }
        }

        private void ClearFields()
        {
            txtProjectName.Clear();
            txtLocation.Clear();
            txtCustomerID.Clear();
            // تأكد من أسماء الأدوات عندك في الـ Design
            comboBoxStatus.SelectedIndex = -1;
            dateTimePickerStart.Value = DateTime.Now;
            dateTimePickerEnd.Value = DateTime.Now;
            numericBudget.Value = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // البحث بجزء من اسم المشروع
                string query = "SELECT ProjectID, ProjectName, Location, ProjectStatus, TotalBudget FROM Projects WHERE ProjectName LIKE @search + '%'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@search", txtSearch.Text); // تأكد من اسم الـ TextBox
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtProjectName.Text = row.Cells["ProjectName"].Value.ToString();
                txtLocation.Text = row.Cells["Location"].Value.ToString();
                comboBoxStatus.Text = row.Cells["Status"].Value.ToString();
                numericBudget.Value = Convert.ToDecimal(row.Cells["Budget"].Value);
                txtCustomerID.Text = row.Cells["CustomerID"].Value.ToString();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    // بنجيب البيانات بناءً على الحالة المختارة في الفلتر
                    string query = "SELECT ProjectID, ProjectName, Location, ProjectStatus, TotalBudget FROM Projects WHERE ProjectStatus = @status";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@status", comboBoxFilterStatus.Text);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في الفلترة: " + ex.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBoxFilterStatus.SelectedIndex = -1; // يرجع القائمة فاضية
            LoadProjectsData();
            txtSearch.Clear();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
