using DishOfTheDay.Entity;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DishOfTheDay.Editors
{
    public partial class KitchenEditForm : Form
    {

        private bool imageChanged;

        public KitchenEditForm()
        {
            InitializeComponent();

            this.Load += (s, a) =>
            {
                if (CurrentItem == null)
                {
                    CurrentItem = new KitchenEntity();
                }
                else
                {
                    txtName.Text = CurrentItem.name;
                    if (CurrentItem.country_flag != null && CurrentItem.country_flag.Length > 0)
                    {
                        try
                        {
                            using (var ms = new MemoryStream(CurrentItem.country_flag))
                            {
                                pictureBox.Image = Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Щось сталося", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };
        }
        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.png, *.jpg, *.jpeg, *.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*";
            dlg.RestoreDirectory = true;
            dlg.DefaultExt = ".png";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox.ImageLocation = dlg.FileName;
                imageChanged = true;
            }
        }

        public KitchenEntity CurrentItem { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Не введено назву кухні", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CurrentItem.name = txtName.Text;
            if (imageChanged)
                CurrentItem.country_flag = File.ReadAllBytes(pictureBox.ImageLocation);

            DialogResult = DialogResult.OK;
        }
    }
}
