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
    public partial class PasswordEditForm : Form
    {
        public PasswordEditForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var old = txtOld.Text;
            var newone = txtNew.Text;
            var confirm = txtConfirm.Text;

            if(!string.Equals(newone, confirm))
            {
                MessageBox.Show("Паролі не співпадають", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!DataLayer.Instance.ChangePassword(old, newone))
            {
                MessageBox.Show("Старий пароль введено з помилкою", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
