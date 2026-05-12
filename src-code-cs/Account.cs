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
    public partial class Account: Form
    {
        string connString = @"Data Source=DESKTOP-OBP6063\SQLEXPRESS01;Initial Catalog=bonyaan;Integrated Security=True";

        public Account()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("من فضلك أدخل اسم المستخدم وكافة البيانات", "تنبيه");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();

                    //  نبحث عن الباسورد الخاص بالمستخدم المكتوب اسمه في txtUsername
                    string checkQuery = "SELECT Password FROM Users WHERE Username = @user";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@user", txtUsername.Text.Trim());

                    object result = checkCmd.ExecuteScalar();

                    if (result != null)
                    {
                        string dbPassword = result.ToString().Trim();

                        // التأكد أن الباسورد القديم صح
                        if (dbPassword == txtCurrentPassword.Text.Trim())
                        {
                            // التأكد أن الباسورد الجديد متطابق مع خانة التأكيد
                            if (txtNewPassword.Text == txtConfirmPassword.Text)
                            {
                                //  تنفيذ التعديل للمستخدم المحدد
                                string updateQuery = "UPDATE Users SET Password = @newPass WHERE Username = @user";
                                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                                updateCmd.Parameters.AddWithValue("@newPass", txtNewPassword.Text.Trim());
                                updateCmd.Parameters.AddWithValue("@user", txtUsername.Text.Trim());

                                updateCmd.ExecuteNonQuery();
                                MessageBox.Show($"تم تغيير باسورد المستخدم ({txtUsername.Text}) بنجاح!");

                                // تنظيف الخانات
                                txtUsername.Clear();
                                txtCurrentPassword.Clear();
                                txtNewPassword.Clear();
                                txtConfirmPassword.Clear();
                            }
                            else
                            {
                                MessageBox.Show("الباسورد الجديد غير متطابق مع التأكيد");
                            }
                        }
                        else
                        {
                            MessageBox.Show("كلمة المرور الحالية غير صحيحة لهذا المستخدم");
                        }
                    }
                    else
                    {
                        MessageBox.Show("اسم المستخدم هذا غير موجود في قاعدة البيانات");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }
        }
    }
}
