using DishOfTheDay.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;

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
                    btnReview.Enabled = false;
                    CurrentItem = new DishEntity();
                    CurrentIngredients = new List<DishIngredientEntity>();
                }
                else
                {
                    DataLayer.Instance.IncrementUsageNumber(CurrentItem.dish_id);
                    btnReview.Enabled = DataLayer.Instance.CurrentUser.client_id != CurrentItem.owner;
                    buttonOK.Enabled = DataLayer.Instance.CurrentUser.client_id == CurrentItem.owner;
                    btnAdd.Enabled = DataLayer.Instance.CurrentUser.client_id == CurrentItem.owner;
                    btnRemove.Enabled = DataLayer.Instance.CurrentUser.client_id == CurrentItem.owner;

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
                                pictureBox.Image = System.Drawing.Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Щось сталося", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    foreach (var item in CurrentIngredients)
                    {
                        var i = lstIngredients.Items.Add(item.ingredientName);
                        i.SubItems.Add(item.count.ToString());
                        i.SubItems.Add(item.units);
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
                i.SubItems.Add(dlg.CurrentItem.units.ToString());
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
                lstIngredients.SelectedItems[0].SubItems[2].Text = selectedItem.units;
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

        private void btnPDF_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Data files (*.pdf)|*.pdf|All files|*.*";
            dlg.RestoreDirectory = true;
            dlg.DefaultExt = ".pdf";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var fontProgram = FontProgramFactory.CreateFont(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "verdana.ttf"));
                    var font = PdfFontFactory.CreateFont(fontProgram, "Windows-1251");
                    
                    var writer = new PdfWriter(dlg.FileName);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);
                    document.SetFont(font);
                    //header
                    var header = new Paragraph("РЕЦЕПТ " + CurrentItem.name.ToUpper())
                        .SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
                    document.Add(header);
                    
                    // Line separator
                    var ls = new LineSeparator(new SolidLine());
                    document.Add(ls);
                    
                    //subheader
                    var subheader  = new Paragraph("Створено в DishOfTheDay " + DateTime.Now)
                        .SetTextAlignment(TextAlignment.CENTER).SetFontSize(15);
                    document.Add(subheader);

                    // Add image
                    if (CurrentItem.picture != null)
                    {
                        var img = new Image(ImageDataFactory.Create(CurrentItem.picture))
                            // .SetMaxWidth(UnitValue.CreatePercentValue(70))
                            // .SetMaxWidth(200)
                            .SetMaxHeight(200)
                            .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        document.Add(img);
                    }
                    
                    //Information about dish
                    var dishType = new Paragraph("Тип страви: " + CurrentItem.DishTypeName)
                        .SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(dishType);
                    var kitchen = new Paragraph("Кухня, якій належить страва: " + CurrentItem.KitchenName)
                        .SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(kitchen);
                    var cookingTime = new Paragraph("Для приготування  страви потрібно: " + CurrentItem.cooking_time + "хв")
                        .SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(cookingTime);
                    var recipe = new Paragraph("Рецепт: " + CurrentItem.recipe)
                        .SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(recipe);
                    
                    //ingredients
                    var title = new Paragraph("Інгрідієнти для страви")
                        .SetTextAlignment(TextAlignment.LEFT).SetFontSize(12).SetBold();
                    document.Add(title);

                    Table table = new Table(3, false);
                    table.SetWidth(UnitValue.CreatePercentValue(100));
                    Cell cell11 = new Cell(1, 1).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Назва"));
                    Cell cell12 = new Cell(1, 1).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Кількість"));
                    Cell cell13 = new Cell(1, 1).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Міра вимірювання"));
                    table.AddCell(cell11);
                    table.AddCell(cell12);
                    table.AddCell(cell13);

                    foreach (var i in CurrentIngredients)
                    {
                        Cell cell21 = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(i.ingredientName));
                        Cell cell22 = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(i.count.ToString()));
                        Cell cell23 = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(i.units));
                        table.AddCell(cell21);
                        table.AddCell(cell22);
                        table.AddCell(cell23);
                    }
                    document.Add(table);
                  
                    // Page numbers
                    var n = pdf.GetNumberOfPages();
                    for (var i = 1; i <= n; i++)
                    {
                        document.ShowTextAligned(new Paragraph(string.Format("page" + i + " of " + n)),
                            559, 806, i, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);
                    }
                    
                    document.Close();
                    // MessageBox.Show("Данні успішно експортовано", "Information", MessageBoxButtons.OK,
                    //     MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Щось сталося", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReview_Click(object sender, EventArgs e)
        {
            var dlg = new ReviewEditForm();
            dlg.CurrentDishId = CurrentItem.dish_id;
            dlg.ShowDialog();
        }
    }
}
