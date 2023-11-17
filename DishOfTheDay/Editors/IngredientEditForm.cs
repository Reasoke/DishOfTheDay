using DishOfTheDay.Entity;
using System;
using System.Windows.Forms;

namespace DishOfTheDay.Editors
{
    public partial class IngredientEditForm : Form
    {
        public IngredientEditForm()
        {
            InitializeComponent();

            this.Load += (s, a) =>
            {
                if (CurrentItem == null)
                {
                    CurrentItem = new IngredientEntity();
                }
                else
                {
                    txtName.Text = CurrentItem.name;
                    numPrice.Value = (decimal)CurrentItem.price;
                    txtUnits.Text = CurrentItem.units;
                    numExpiration.Value = CurrentItem.expiration;
                    txtManufacturer.Text = CurrentItem.manufacturer;
                }
            };
        }

        public IngredientEntity CurrentItem { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {          
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Не введено назву інгредієнта", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtUnits.Text))
            {
                MessageBox.Show("Не введено міру вимірювання", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (numPrice.Value == 0)
            {
                MessageBox.Show("Не введено ціну", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CurrentItem.name = txtName.Text;
            CurrentItem.price = (float)numPrice.Value;
            CurrentItem.units = txtUnits.Text;
            CurrentItem.expiration = numExpiration.Value;
            CurrentItem.manufacturer = txtManufacturer.Text;

            DialogResult = DialogResult.OK;
        }
    }
}
