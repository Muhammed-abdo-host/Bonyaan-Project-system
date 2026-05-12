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
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace bonyaan_system
{
    public partial class orders : Form
    {
        public orders()
        {
            InitializeComponent();
        }

        private void numQuantity_ValueChanged(object sender, EventArgs e)
        {
            // تحديد أقل قيمة وأكبر قيمة للكمية
            numQuantity.Minimum = 0;
            numQuantity.Maximum = 1000;
            numQuantity.Value = 1; // القيمة الافتراضية أول ما تفتح
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Reports_Load(object sender, EventArgs e)
        {
            FillMaterials(); // دالة سحب البيانات من SQL

            // تظبيط العداد هنا أحسن
            numQuantity.Minimum = 1;
            numQuantity.Maximum = 1000;

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numQuantity_ValueChanged_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string searchText = cmbProducts.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                // بنشوف النص ده موجود في القائمة ولا لأ
                int index = cmbProducts.FindString(searchText);

                if (index != -1)
                {
                    // لو لقاه، بنثبت الاختيار عليه
                    cmbProducts.SelectedIndex = index;
                    MessageBox.Show("تم إيجاد الصنف واختياره");
                }
                else
                {
                    MessageBox.Show("عفواً، هذا الصنف غير موجود في القائمة");
                }
            }
            else
            {
                MessageBox.Show("من فضلك اكتب اسم الصنف أولاً داخل الخانة");
            }
        }

        private void cmbProducts_SelectedIndexChanged(object sender, EventArgs e)
        {


           

            // تظبيط العداد
            numQuantity.Minimum = 1;
            numQuantity.Maximum = 1000;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null) return;

            string materialName = cmbProducts.Text;
            int requestedQty = (int)numQuantity.Value;
            string connString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT CurrentQuantity, Price FROM Materials WHERE MaterialName = @name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", materialName);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int stockQty = reader["CurrentQuantity"] != DBNull.Value ? Convert.ToInt32(reader["CurrentQuantity"]) : 0;
                    double price = reader["Price"] != DBNull.Value ? Convert.ToDouble(reader["Price"]) : 0.0;

                    if (stockQty >= requestedQty)
                    {
                        double total = requestedQty * price;

                        dgvOrderItems.Rows.Add(materialName, requestedQty, price, total);

                        CalculateFinalAmount();

                        reader.Close(); 
                        string updateQuery = "UPDATE Materials SET CurrentQuantity = CurrentQuantity - @req WHERE MaterialName = @name";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@req", requestedQty);
                        updateCmd.Parameters.AddWithValue("@name", materialName);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show($"عفواً، الكمية المتاحة هي ({stockQty}) فقط!");
                    }
                }
                conn.Close();
            }
        }
        private void CalculateFinalAmount()
        {
            double sum = 0;
            foreach (DataGridViewRow row in dgvOrderItems.Rows)
            {
                // بنجمع القيم اللي في العمود الرابع (Total) رقمه في الكود 3
                if (row.Cells[3].Value != null)
                {
                    sum += Convert.ToDouble(row.Cells[3].Value);
                }
            }
            // تأكد إن lblTotal هو اسم الليبل اللي عندك
            lblTotal.Text = sum.ToString("N2");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 1. التأكد إن الجدول مش فاضي
            if (dgvOrderItems.Rows.Count == 0 || dgvOrderItems.Rows[0].IsNewRow)
            {
                MessageBox.Show("!!الجدول فاضي");
                return;
            }

            // 2. التأكد إن المستخدم اختار طريقة دفع
            if (cmbPaymentMethod.SelectedIndex == -1)
            {
                MessageBox.Show("من فضلك اختر طريقة الدفع أولاً");
                return;
            }

            string connString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";

            // تجهيز نص الفاتورة (للعرض أو الطباعة لاحقاً)
            string invoiceContent = "--------------------------------------\n";
            invoiceContent += "        نظام بنيان لإدارة المشروعات        \n";
            invoiceContent += "--------------------------------------\n";
            invoiceContent += $"التاريخ: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}\n";
            invoiceContent += $"العميل: {txtCustomerName.Text}\n";
            invoiceContent += $"الهاتف: {txtCustomerPhone.Text}\n";
            invoiceContent += $"طريقة الدفع: {cmbPaymentMethod.SelectedItem.ToString()}\n"; // إضافة طريقة الدفع للفاتورة
            invoiceContent += "--------------------------------------\n";
            invoiceContent += " المنتج | الكمية | السعر | الإجمالي \n";
            invoiceContent += "--------------------------------------\n";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // استخدام Transaction لضمان إن لو حصل مشكلة في سطر، الكل يتلغي (أمان أكتر)
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    foreach (DataGridViewRow row in dgvOrderItems.Rows)
                    {
                        if (row.Cells[0].Value != null)
                        {
                            string name = row.Cells[0].Value.ToString();
                            string qty = row.Cells[1].Value.ToString();
                            string price = row.Cells[2].Value.ToString();
                            string total = row.Cells[3].Value.ToString();

                            // إضافة السطر لنص الفاتورة
                            invoiceContent += $"{name.PadRight(15)} | {qty.PadRight(5)} | {price.PadRight(5)} | {total}\n";

                            // تحديث المخزن في SQL
                            string updateQuery = "UPDATE Materials SET CurrentQuantity = CurrentQuantity - @qty WHERE MaterialName = @name";
                            SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction);
                            cmd.Parameters.AddWithValue("@qty", qty);
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 4. إضافة المجموع النهائي أسفل الفاتورة
                    invoiceContent += "------------------------------------------\n";
                    invoiceContent += $"إجمالي الفاتورة: {lblTotal.Text} جنيه مصري\n";
                    invoiceContent += "------------------------------------------\n";
                    invoiceContent += "       شكراً لتعاملكم مع شركة بنيان       \n";

                    transaction.Commit(); // تثبيت الحفظ في القاعدة

                    // 5. إظهار الفاتورة فوراً في Notepad
                    string fileName = "Invoice_Last.txt";
                    System.IO.File.WriteAllText(fileName, invoiceContent);
                    System.Diagnostics.Process.Start("notepad.exe", fileName);

                    MessageBox.Show("تم تأكيد الفاتورة وتحديث المخزن بنجاح");

                    // 6. تصفير الشاشة لبدء عملية جديدة
                    dgvOrderItems.Rows.Clear();
                    lblTotal.Text = "0.00";
                    txtCustomerName.Clear();
                    txtCustomerPhone.Clear();
                    cmbPaymentMethod.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // لو حصل إيرور يرجع كل حاجة زي ما كانت
                    MessageBox.Show("حدث خطأ أثناء التأكيد: " + ex.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dgvOrderItems.Rows.Clear(); 
            lblTotal.Text = "0.00"; 
            txtCustomerName.Clear();
            txtCustomerPhone.Clear();
                                    

        }
        private void FillMaterials()
        {

            string connString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT MaterialName FROM Materials";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable(); // تعريف الـ dt هنا
                da.Fill(dt);
                // 1. حدد الأعمدة أولاً والـ DataSource فاضي
                cmbProducts.DataSource = null;
                cmbProducts.DisplayMember = "MaterialName";
                cmbProducts.ValueMember = "MaterialName"; // أو ID لو عندك

                // 2. اربط الداتا في آخر خطوة
                cmbProducts.DataSource = dt;

                // 3. امنع الاختيار التلقائي لأول عنصر
                cmbProducts.SelectedIndex = -1;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // التأكد إن المستخدم اختار سطر بالفعل وموش سطر فاضي
            if (dgvOrderItems.SelectedRows.Count > 0 && !dgvOrderItems.SelectedRows[0].IsNewRow)
            {
                // مسح السطر المختار
                dgvOrderItems.Rows.RemoveAt(dgvOrderItems.SelectedRows[0].Index);

                // أهم خطوة: إعادة حساب الإجمالي الكلي للفاتورة بعد الحذف
                CalculateFinalAmount();
            }
            else
            {
                MessageBox.Show("يرجى تحديد سطر كامل من الجدول لمسحه"); 
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}