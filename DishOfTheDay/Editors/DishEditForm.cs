using DishOfTheDay.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Dapper.SqlMapper;

namespace DishOfTheDay.Editors
{
    public partial class DishEditForm : Form
    {
        private bool imageChanged;

        public DishEditForm()
        {
            InitializeComponent();

            this.Load += (s, a) =>
            {
                cmbKitchen.DataSource = DataLayer.Instance.Kitchens;
                cmbKitchen.DisplayMember = "name";
                cmbKitchen.ValueMember = "kitchen_id";
                cmbDishType.DataSource = DataLayer.Instance.DishTypes;
                cmbDishType.DisplayMember = "name";
                cmbDishType.ValueMember = "dish_type_id";

                if (CurrentItem == null)
                {
                    CurrentItem = new DishEntity();
                    CurrentIngredients = new List<DishIngredientEntity>();
                }
                else
                {
                    txtName.Text = CurrentItem.name;
                    cmbKitchen.SelectedValue = CurrentItem.kitchen;
                    cmbDishType.SelectedValue = CurrentItem.dish_type;
                    numTime.Value = CurrentItem.cooking_time;
                    txtRecipe.Text = CurrentItem.recipe;
                    if (CurrentItem.picture != null && CurrentItem.picture.Length > 0)
                    {
                        try
                        {
                            using (var ms = new MemoryStream(CurrentItem.picture))
                            {
                                pictureBox.Image = Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    foreach (var item in CurrentIngredients)
                    {
                        var i = lstIngredients.Items.Add(item.ingredientName);
                        i.SubItems.Add(item.count.ToString());
                        i.Tag = item;
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

        public DishEntity CurrentItem { get; set; }
        public List<DishIngredientEntity> CurrentIngredients { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var kitchen = cmbKitchen.SelectedItem as KitchenEntity;
            if (kitchen == null)
            {
                MessageBox.Show("Не обрано кухню страви", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var dishType = cmbDishType.SelectedItem as DishTypeEntity;
            if (dishType == null)
            {
                MessageBox.Show("Не обрано тип страви", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Не введено назву страви", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtRecipe.Text))
            {
                MessageBox.Show("Не введено рецепт страви", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CurrentItem.name = txtName.Text;
            CurrentItem.kitchen = kitchen.kitchen_id;
            CurrentItem.dish_type = dishType.dish_type_id;
            CurrentItem.cooking_time = (int)numTime.Value;
            CurrentItem.recipe = txtRecipe.Text;
            if(imageChanged)
                CurrentItem.picture = File.ReadAllBytes(pictureBox.ImageLocation);


            DialogResult = DialogResult.OK;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var dlg = new DishIngredientEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var i = lstIngredients.Items.Add(dlg.CurrentItem.ingredientName);
                i.SubItems.Add(dlg.CurrentItem.count.ToString());
                i.Tag = dlg.CurrentItem;
                dlg.CurrentItem.dish_id = CurrentItem.dish_id;
                CurrentIngredients.Add(dlg.CurrentItem);
            }

        }

        private void lstIngredients_DoubleClick(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нічого не обрано", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedItem = lstIngredients.SelectedItems[0].Tag as DishIngredientEntity;
            if (selectedItem == null)
            {
                MessageBox.Show("Немає даних", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var dlg = new DishIngredientEditForm();
            dlg.CurrentItem = selectedItem;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                lstIngredients.SelectedItems[0].Text = selectedItem.ingredientName;
                lstIngredients.SelectedItems[0].SubItems[1].Text = selectedItem.count.ToString();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нічого не обрано", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedItem = lstIngredients.SelectedItems[0].Tag as DishIngredientEntity;
            if (selectedItem == null)
            {
                MessageBox.Show("Немає даних", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lstIngredients.Items.Remove(lstIngredients.SelectedItems[0]);
            CurrentIngredients.Remove(selectedItem);

        }
    }
}
