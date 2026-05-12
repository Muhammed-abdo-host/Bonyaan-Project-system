using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bonyaan_system
{

    public partial class Mainform: Form
    {
        public Mainform()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void panelMenue_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // لو إنت شغال بنظام الـ Panels عشان الصفحة تفتح "جوه" المين فورم
            loadform(new Employees());

            // ملاحظة: لو مكنتش لسه عملت ميثود الـ loadform، استخدم الكود اللي تحت ده:
            /*
            Employees empForm = new Employees();
            empForm.TopLevel = false;
            empForm.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Clear(); // تأكد إن mainPanel هو اسم البانل الفاضية عندك
            this.mainPanel.Controls.Add(empForm);
            empForm.Show();
            */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // بنادي الميثود اللي بتعرض الفورم جوه الـ mainPanel
            loadform(new Projects());
        }

        private void button7_Click(object sender, EventArgs e)
        {

            // 1. تعريف نسخة من صفحة الـ Account
            loadform(new Account());

        }

        private void button6_Click(object sender, EventArgs e)
        {

            // 1. إنشاء نسخة من شاشة التقارير
            loadform(new orders());

          
        }
        public void loadform(object Form)
        {
            // بنمسح أي حاجة موجودة في الـ Panel حالياً عشان نحط الشاشة الجديدة
            if (this.mainPanel.Controls.Count > 0)
                this.mainPanel.Controls.RemoveAt(0);

            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Add(f);
            this.mainPanel.Tag = f;
            f.Show();
        }
        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void Mainform_Load(object sender, EventArgs e)
        {

        }

        private void Mainform_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 1. نمسح أي حاجة موجودة جوه البانل دلوقتي
            mainPanel.Controls.Clear();

            // 2. ننشئ نسخة من فورم الموردين
            supplier frm = new supplier(); // تأكد من اسم الكلاس عندك

            // 3. نخليها تظهر كجزء من الواجهة مش فورم مستقلة
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            // 4. نضيفها للبانل ونعرضها
            mainPanel.Controls.Add(frm);
            frm.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // بنادي ميثود الـ loadform اللي عملناها قبل كدة عشان تفتح التقارير في البانل الأساسية
            loadform(new Reports());
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }
    }
}
