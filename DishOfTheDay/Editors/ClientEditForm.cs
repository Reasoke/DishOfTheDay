using DishOfTheDay.Entity;
using System;
using System.Windows.Forms;

namespace DishOfTheDay.Editors
{
    public partial class ClientEditForm : Form
    {
        public ClientEditForm()
        {
            InitializeComponent();

            this.Load += (s, a) =>
            {              
                if (CurrentItem == null)
                {
                    CurrentItem = new ClientEntity();
                }
                else
                {
                    txtFirstName.Text = CurrentItem.first_name;                   
                    txtLastName.Text = CurrentItem.last_name;                   
                    txtEmail.Text = CurrentItem.email;                   
                    txtPhone.Text = CurrentItem.phone;                   
                    txtAddress.Text = CurrentItem.address;                   
                    txtDesc.Text = CurrentItem.description;                   
                }
            };
        }

        public ClientEntity CurrentItem { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {          
            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                MessageBox.Show("Не введено назву ім'я", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Не введено прізвище", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Не введено електронну пошту", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CurrentItem.first_name = txtFirstName.Text;
            CurrentItem.last_name = txtLastName.Text;
            CurrentItem.email = txtEmail.Text;
            CurrentItem.phone = txtPhone.Text;
            CurrentItem.address = txtAddress.Text;
            CurrentItem.description = txtDesc.Text;
            
            DialogResult = DialogResult.OK;
        }
    }
}
