using DishOfTheDay.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

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
            CurrentItem.count = (int)numCount.Value;


            DialogResult = DialogResult.OK;

        }
    }
}
