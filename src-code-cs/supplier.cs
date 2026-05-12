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
using System.Xml.Linq;

namespace bonyaan_system
{
    public partial class supplier: Form
    {
        string connString = @"Data Source=.\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";
        public supplier()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void supplier_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 1. التأكد أن الحقول ليست فارغة
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("من فضلك اختر المورد أو أدخل الاسم المراد تعديل بياناته", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. رسالة تأكيد قبل التعديل (اختياري ولكن احترافي)
            var confirmResult = MessageBox.Show("هل أنت متأكد من تعديل بيانات هذا المورد؟", "تأكيد التعديل", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connString))
                    {
                        con.Open();

                        // 3. أمر التحديث (سنفترض أن التعديل يتم بناءً على اسم المورد)
                        // نصيحة: الأفضل دايماً التعديل بالـ ID لو متاح عندك
                        string query = "UPDATE Suppliers SET ContactNumber = @phone, CompanyBranch = @address WHERE TRIM(SupplierName) = @name";

                        SqlCommand cmd = new SqlCommand(query, con);

                        // إضافة الباراميترز مع استخدام Trim لتفادي المسافات
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());

                        // 4. تنفيذ الأمر والتحقق من التأثير
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("تم تحديث بيانات المورد بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // تحديث الجدول المعروض (لو عندك دالة بتملأ الـ DataGridView)
                            LoadSuppliersData();

                            // مسح الخانات
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("لم يتم العثور على مورد بهذا الاسم لتعديله!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في قاعدة البيانات: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ClearFields()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            // لو عندك حقول تانية ضيفها هنا بنفس الطريقة
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True"))
            {
                string query = "INSERT INTO Suppliers (SupplierName, CompanyBranch, ContactNumber) VALUES (@name, @branch, @contact)";
                SqlCommand cmd = new SqlCommand(query, conn);

                // بنربط التكست بوكس بالأسماء الجديدة في قاعدة البيانات
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@branch", txtAddress.Text); // العنوان هيروح لـ CompanyBranch
                cmd.Parameters.AddWithValue("@contact", txtPhone.Text);  // التليفون هيروح لـ ContactNumber

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("تم حفظ بيانات المورد بنجاح في نظام بنيان");
                LoadSuppliersData(); // عشان تظهر في الجدول فوراً
            }
        }
        public void LoadSuppliersData()
        {
            try
            {
                string connString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // بنجيب الكل عشان يعرض SupplierName و CompanyBranch و ContactNumber
                    string query = "SELECT * FROM Suppliers";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvSuppliers.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تحديث الجدول: " + ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textName_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True"))
            {
                // بنبحث باستخدام رقم التليفون
                // غيرنا Phone لـ ContactNumber
                string query = "SELECT * FROM Suppliers WHERE ContactNumber LIKE @phone";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                // تأكد أن اسم الباراميتر هنا يطابق اللي في جملة الاستعلام فوق
                da.SelectCommand.Parameters.AddWithValue("@phone", "%" + txtSearchPhone.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSuppliers.DataSource = dt;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل أنت متأكد من حذف هذا المورد؟", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True"))
                {
                    string query = "DELETE FROM Suppliers WHERE SupplierName = @name";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("تم الحذف بنجاح");
                    LoadSuppliersData(); // تحديث الجدول بعد الحذف
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
        
            // مسح النص من خانة البحث
            txtSearchPhone.Clear();

            // إعادة عرض كل الموردين في الجدول (عشان يلغي فلترة البحث)
            LoadSuppliersData();

            // وضع المؤشر جوه الخانة عشان يكتب فوراً
            txtSearchPhone.Focus();
        
        }

        private void dgvSuppliers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Suppliers_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

            // تفريغ خانات النصوص
            txtName.Clear();
            txtAddress.Clear(); // أو txtBranch حسب تسميتك
            txtPhone.Clear();

            // لو عندك خانة بحث برضه صفرها
            txtSearchPhone.Clear();

            // تركيز الماوس على أول خانة للاستعداد لإدخال جديد
            txtName.Focus();
        }
    }
}
