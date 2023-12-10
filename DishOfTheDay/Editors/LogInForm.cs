using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DishOfTheDay.Editors
{
    public partial class LogInForm : Form
    {
        public LogInForm()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Text.Trim();
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Невірний пароль чи пошта", "Kitchen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var found = DataLayer.Instance.VerifyPassword(email, password);

            if (!found)
            {
                var userExist = DataLayer.Instance.UserExist(email);
                if (!userExist)
                {
                    var res = MessageBox.Show("Бажаєте зареєструвати нового користувача?", "Kitchen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(res == DialogResult.Yes)
                    {
                        DataLayer.Instance.RegisterUser(email, password);
                        DialogResult = DialogResult.OK;
                        return;
                    }
                }
                MessageBox.Show("Невірний пароль чи пошта", "Kitchen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
