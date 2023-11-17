using DishOfTheDay.Entity;
using System;
using System.Windows.Forms;

namespace DishOfTheDay.Editors
{
    public partial class DishIngredientEditForm : Form
    {
        public DishIngredientEditForm()
        {
            InitializeComponent();

            this.Load += (s, a) =>
            {
                cmbIngredients.DataSource = DataLayer.Instance.Ingredients;
                cmbIngredients.DisplayMember = "name";
                cmbIngredients.ValueMember = "ingredient_id";

                if (CurrentItem == null)
                {
                    CurrentItem = new DishIngredientEntity();
                }
                else
                {
                    cmbIngredients.SelectedValue = CurrentItem.ingredient_id;
                    numCount.Value = CurrentItem.count;
                                      
                }
            };
        }

        public DishIngredientEntity CurrentItem { get; set; }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var ingredient = cmbIngredients.SelectedItem as IngredientEntity;
            if (ingredient == null)
            {
                MessageBox.Show("Не обрано інгредіент", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CurrentItem.ingredient_id = ingredient.ingredient_id;
            CurrentItem.ingredientName = ingredient.name;
            CurrentItem.units = ingredient.units;
            CurrentItem.count = (int)numCount.Value;


            DialogResult = DialogResult.OK;

        }
    }
}
