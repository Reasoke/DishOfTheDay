using DishOfTheDay.Entity;
using System;
using System.Windows.Forms;

namespace DishOfTheDay.Editors
{
    public partial class DishTypeEditForm : Form
    {
        public DishTypeEditForm()
        {
            InitializeComponent();

            this.Load += (s, a) =>
            {

                if (CurrentItem == null)
                {
                    CurrentItem = new DishTypeEntity();
                }
                else
                {
                    txtName.Text = CurrentItem.name;
                }
            };
        }

        public DishTypeEntity CurrentItem { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Не введено назву типу страви", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
            CurrentItem.name = txtName.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
