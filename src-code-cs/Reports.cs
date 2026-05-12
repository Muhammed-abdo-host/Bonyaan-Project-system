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

namespace bonyaan_system
{
    public partial class Reports: Form
    {
        string connString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";

        public Reports()
        {
            InitializeComponent();
            FillProjectsCombo();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // ركز هنا: غيرنا Budget لـ TotalBudget عشان تطابق جدولك
                string query = @"SELECT p.ProjectName, p.Location, p.TotalBudget, 
                        ISNULL(SUM(o.TotalAmount), 0) as Expenses,
                        (p.TotalBudget - ISNULL(SUM(o.TotalAmount), 0)) as ProfitLoss
                        FROM Projects p
                        LEFT JOIN Orders o ON p.ProjectID = o.ProjectID
                        GROUP BY p.ProjectName, p.Location, p.TotalBudget";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("الجدول فاضي، مفيش بيانات مشاريع حالياً.");
                    }
                }
                catch (Exception ex)
                {
                    // لو طلع إيرور تاني هيقولك مكانه بالظبط
                    MessageBox.Show("إيرور في الداتابيز: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ProfitLoss"].Value != null)
                    total += Convert.ToDecimal(row.Cells["ProfitLoss"].Value);
            }
            MessageBox.Show($"إجمالي أرباح مشاريع بنيان حالياً هو: {total:C}");
        }
        private void StyleGrid()
        {
            decimal totalProfit = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ProfitLoss"].Value != null)
                {
                    // بنشيل علامة + لو موجودة قبل التحويل
                    string valStr = row.Cells["ProfitLoss"].Value.ToString().Replace("+", "");
                    totalProfit += Convert.ToDecimal(valStr);
                }
            }

            string status = totalProfit >= 0 ? "أرباح" : "خسائر";
            MessageBox.Show($"إجمالي {status} الشركة في هذه الفترة: {totalProfit:N2}", "التقرير المالي الإجمالي");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 1. تحديد مكان حفظ الملف
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt";
            sfd.FileName = "Bonyan_Report_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        // 2. تصميم شكل الفاتورة (Header)
                        sw.WriteLine("================================================");
                        sw.WriteLine("            نظام بنيان لإدارة المشاريع            ");
                        sw.WriteLine("            Financial Summary Report            ");
                        sw.WriteLine("================================================");
                        sw.WriteLine($"تاريخ التقرير: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}");
                        sw.WriteLine($"الفترة من: {dtpFrom.Value.ToShortDateString()} إلى: {dtpTo.Value.ToShortDateString()}");
                        sw.WriteLine("------------------------------------------------");

                        // رؤوس الأعمدة
                        sw.WriteLine(string.Format("{0,-20} | {1,-10} | {2,-10}", "Project Name", "Budget", "Profit/Loss"));
                        sw.WriteLine("------------------------------------------------");

                        // 3. قراءة البيانات من الـ DataGridView
                        decimal totalProfit = 0;
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["ProjectName"].Value != null)
                            {
                                string name = row.Cells["ProjectName"].Value.ToString();
                                string budget = row.Cells["Budget"].Value.ToString();
                                string profit = row.Cells["ProfitLoss"].Value.ToString();

                                sw.WriteLine(string.Format("{0,-20} | {1,-10} | {2,-10}", name, budget, profit));

                                // حساب الإجمالي (بنشيل علامة + لو موجودة)
                                totalProfit += Convert.ToDecimal(profit.Replace("+", ""));
                            }
                        }

                        // 4. تذييل الفاتورة (Footer)
                        sw.WriteLine("------------------------------------------------");
                        sw.WriteLine($"إجمالي الربح/الخسارة للفترة: {totalProfit:N2}");
                        sw.WriteLine("================================================");
                        sw.WriteLine("           تم الاستخراج بواسطة سيستم بنيان          ");
                    }

                    MessageBox.Show("تمت طباعة التقرير بنجاح يا هندسة!", "تم التصدير", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // فتح الملف تلقائياً بعد الحفظ (اختياري)
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حصلت مشكلة وأنا بطلع الملف: " + ex.Message);
                }
            }
        }
        private void FillProjectsCombo()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // استعلام بيجيب اسم المشروع والـ ID بتاعه
                string query = "SELECT ProjectID, ProjectName FROM Projects";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    // تنظيف الكومبو بوكس الأول
                    comboBoxSelectProject.Items.Clear();

                    // إضافة اختيار "كل المشاريع" كاختيار افتراضي
                    comboBoxSelectProject.Items.Add("All Projects");

                    while (dr.Read())
                    {
                        // إضافة اسم المشروع للكومبو بوكس
                        comboBoxSelectProject.Items.Add(dr["ProjectName"].ToString());
                    }

                    // خليه يختار "All Projects" أول ما يفتح
                    comboBoxSelectProject.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في تحميل المشاريع: " + ex.Message);
                }
            }
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {

        }
    }

}
