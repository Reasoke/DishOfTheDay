using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DishOfTheDay.Editors;
using DishOfTheDay.Entity;
using Newtonsoft.Json;

namespace DishOfTheDay
{

    public partial class MainForm : Form
    {
        public enum ViewMode
        {
            Dishes,
            Clients,
            Ingredients,
            Kitchens,
            DishTypes,
        }

        private ViewMode currentViewMode = ViewMode.Dishes;
        private Timer filterTimer;
        private List<object> currentData;
        
        public MainForm()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1160, 540);

            filterTimer = new Timer();
            filterTimer.Enabled = false;
            filterTimer.Interval = 300;
            filterTimer.Tick += (s, a) =>
            {
                RefreshData();
                filterTimer.Stop();
            };

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void dichesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentViewMode = ViewMode.Dishes;
            ApplyFilters(sender, e);
        }

        private void clientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentViewMode = ViewMode.Clients;
            ApplyFilters(sender, e);
        }

        private void ingredientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentViewMode = ViewMode.Ingredients;
            ApplyFilters(sender, e);
        }

        private void kitchenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentViewMode = ViewMode.Kitchens;
            ApplyFilters(sender, e);
        }

        private void dishTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentViewMode = ViewMode.DishTypes;
            ApplyFilters(sender, e);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            editToolStripMenuItem.Enabled = lstMain.SelectedItems.Count == 1;
            deleteToolStripMenuItem.Enabled = lstMain.SelectedItems.Count > 0;
        }

        private void RefreshData()
        {
            int selectedIndex = lstMain.SelectedItems.Count == 0 ? -1 : lstMain.SelectedItems[0].Index;
            lstMain.Items.Clear();
            lstMain.Columns.Clear();
            currentData = null;
            //int width = lstMain.Width;

            panelDishes.Visible = currentViewMode == ViewMode.Dishes;
            panelClients.Visible = currentViewMode == ViewMode.Clients;
            panelIngredients.Visible = currentViewMode == ViewMode.Ingredients;
            
            switch (currentViewMode)
            {
                case ViewMode.Dishes:

                    //filter
                    string search = txtSearch.Text;
                    int sortIndex = cmbSort.SelectedIndex;
                    bool sortAsc = btnSort.ImageIndex == 4;
                    int minCookingTime = (int)numMinTime.Value;
                    int maxCookingTime = (int)numMaxTime.Value;
                    int minIngredientCount = (int)numMinIngredient.Value;
                    int maxIngredientCount = (int)numMaxIngredient.Value;
                    int dishTypeId = cmbDishTypes.SelectedValue == null ? -1 : (int)cmbDishTypes.SelectedValue;
                    int kitchenId = cmbKitchens.SelectedValue == null ? -1 : (int)cmbKitchens.SelectedValue;
                    bool? hasPicture = checkBoxPicture.CheckState == CheckState.Unchecked ? (bool?)null : checkBoxPicture.CheckState == CheckState.Checked;

                    var dishes = DataLayer.Instance.GetDishes(search, sortIndex, sortAsc, minCookingTime, maxCookingTime,
                        minIngredientCount, maxIngredientCount, dishTypeId, kitchenId, hasPicture);
                    currentData = new List<object>(dishes);

                    //display
                    var kitchens = DataLayer.Instance.KitchensFilter;
                    var dishTypes = DataLayer.Instance.DishTypesFilter;

                    lstMain.Columns.Add("Назва", 200);
                    lstMain.Columns.Add("Кухня", 140);
                    lstMain.Columns.Add("Тип страви", 120);
                    lstMain.Columns.Add("Час приготування (хв)", 200);

                    cmbSort.Items.Clear();
                    cmbSort.Items.Add("");
                    cmbSort.Items.Add("Назва");
                    cmbSort.Items.Add("Кухня");
                    cmbSort.Items.Add("Тип страви");
                    cmbSort.Items.Add("Час приготування (хв)");
                    cmbSort.SelectedIndex = sortIndex;

                    cmbDishTypes.DisplayMember = "name";
                    cmbDishTypes.ValueMember = "dish_type_id";
                    cmbDishTypes.DataSource = dishTypes;
                    if (dishTypeId != -1)
                        cmbDishTypes.SelectedValue = dishTypeId;
                    
                    cmbKitchens.DisplayMember = "name";
                    cmbKitchens.ValueMember = "kitchen_id";
                    cmbKitchens.DataSource = kitchens;
                    if (kitchenId != -1)
                        cmbKitchens.SelectedValue = kitchenId;

                    foreach (var entity in dishes)
                    {
                        var listViewItem = lstMain.Items.Add(entity.name);
                        listViewItem.SubItems.Add(kitchens.FirstOrDefault(k => k.kitchen_id == entity.kitchen)?.name);
                        listViewItem.SubItems.Add(dishTypes.FirstOrDefault(d => d.dish_type_id == entity.dish_type)?.name);
                        listViewItem.SubItems.Add(entity.cooking_time.ToString());
                        listViewItem.Tag = entity;
                    }
                    header.Text = "Страви";
                    
                    break;
                case ViewMode.Clients:
                    //filter
                    search = txtSearch.Text;
                    sortIndex = cmbSort.SelectedIndex;
                    sortAsc = btnSort.ImageIndex == 4;
                    bool? phone = checkBoxPhone.CheckState == CheckState.Unchecked ? (bool?)null : checkBoxPhone.CheckState == CheckState.Checked;
                    bool? address = checkBoxAddress.CheckState == CheckState.Unchecked ? (bool?)null : checkBoxAddress.CheckState == CheckState.Checked;
                    bool? desc = checkBoxDesc.CheckState == CheckState.Unchecked ? (bool?)null : checkBoxDesc.CheckState == CheckState.Checked;
                    int minDishes = (int)numMinDish.Value;
                    int maxDishes = (int)numMaxDish.Value;

                    var clients = DataLayer.Instance.GetClients(search, sortIndex, sortAsc, phone, address, desc, minDishes, maxDishes);
                    currentData = new List<object>(clients);
                    
                    //display
                    lstMain.Columns.Add("Ім'я", 150);
                    lstMain.Columns.Add("Прізвище", 150);
                    lstMain.Columns.Add("Електронна пошта", 200);
                    lstMain.Columns.Add("Номер телеффону", 200);
                    lstMain.Columns.Add("Адреса", 150);
                    lstMain.Columns.Add("Про себе", 150);
                    
                    cmbSort.Items.Clear();
                    cmbSort.Items.Add("");
                    cmbSort.Items.Add("Ім'я");
                    cmbSort.Items.Add("Прізвище");
                    cmbSort.Items.Add("Електронна пошта");
                    cmbSort.SelectedIndex = sortIndex;

                    foreach (var entity in clients)
                    {
                        var listViewItem = lstMain.Items.Add(entity.first_name);
                        listViewItem.SubItems.Add(entity.last_name);
                        listViewItem.SubItems.Add(entity.email);
                        listViewItem.SubItems.Add(entity.phone);
                        listViewItem.SubItems.Add(entity.address);
                        listViewItem.SubItems.Add(entity.description);
                        listViewItem.Tag = entity;
                    }
                    header.Text = "Користувачі";
                    break;
                case ViewMode.Ingredients:

                    //filter
                    search = txtSearch.Text;
                    sortIndex = cmbSort.SelectedIndex;
                    sortAsc = btnSort.ImageIndex == 4;
                    int minPrice = (int)numMinPrice.Value;
                    int maxPrice = (int)numMaxPrice.Value;
                    string units = cmbUnits.Text;
                    string manufacturer = cmbManufacturer.Text;
                    //int manufacturer = cmbManufacturer.SelectedValue == null ? -1 : (int)cmbManufacturer.SelectedValue;

                    var ingredients = DataLayer.Instance.GetIngredients(search, sortIndex, sortAsc, minPrice, maxPrice, units, manufacturer);
                    currentData = new List<object>(ingredients);

                    //display;
                    lstMain.Columns.Add("Назва", 200);
                    lstMain.Columns.Add("Ціна", 100);
                    lstMain.Columns.Add("Міра вимірювання", 150);
                    lstMain.Columns.Add("Термін придатності (днів)", 100);
                    lstMain.Columns.Add("Виробник", 200);

                    cmbSort.Items.Clear();
                    cmbSort.Items.Add("");
                    cmbSort.Items.Add("Назва");
                    cmbSort.Items.Add("Ціна");
                    cmbSort.Items.Add("Міра вимірювання");
                    cmbSort.Items.Add("Термін придатності (днів)");
                    cmbSort.Items.Add("Виробник");
                    cmbSort.SelectedIndex = sortIndex;

                    cmbUnits.Items.Clear();
                    cmbUnits.Items.Add("");
                    cmbUnits.Items.AddRange(DataLayer.Instance.GetIngredientUnits());
                    cmbUnits.Text = units;

                    cmbManufacturer.Items.Clear();
                    cmbManufacturer.Items.Add("");
                    cmbManufacturer.Items.AddRange(DataLayer.Instance.GetIngredientManufacturers());
                    cmbManufacturer.Text = units;

                    foreach (var entity in ingredients)
                    {
                        var listViewItem = lstMain.Items.Add(entity.name);
                        listViewItem.SubItems.Add(entity.price.ToString());
                        listViewItem.SubItems.Add(entity.units);
                        listViewItem.SubItems.Add(entity.expiration.ToString());
                        listViewItem.SubItems.Add(entity.manufacturer);
                        listViewItem.Tag = entity;
                    }
                    header.Text = "Індгредієнти";
                    break;
                case ViewMode.Kitchens:
                    //filter
                    search = txtSearch.Text;
                    sortIndex = cmbSort.SelectedIndex;
                    sortAsc = btnSort.ImageIndex == 4;

                    var kitchen = DataLayer.Instance.GetKitchens(search, sortIndex, sortAsc);
                    currentData = new List<object>(kitchen);

                    //display
                    lstMain.Columns.Add("Назва", 200);

                    cmbSort.Items.Clear();
                    cmbSort.Items.Add("");
                    cmbSort.Items.Add("Назва");
                    cmbSort.SelectedIndex = sortIndex;

                    foreach (var entity in kitchen)
                    {
                        var listViewItem = lstMain.Items.Add(entity.name);
                        listViewItem.Tag = entity;
                    }
                    header.Text = "Кухні";
                    break;
                case ViewMode.DishTypes:
                    //filter
                    search = txtSearch.Text;
                    sortIndex = cmbSort.SelectedIndex;
                    sortAsc = btnSort.ImageIndex == 4;

                    var dishType = DataLayer.Instance.GetDithTypes(search, sortIndex, sortAsc);
                    currentData = new List<object>(dishType);

                    //display
                    lstMain.Columns.Add("Назва", 200);

                    cmbSort.Items.Clear();
                    cmbSort.Items.Add("");
                    cmbSort.Items.Add("Назва");
                    cmbSort.SelectedIndex = sortIndex;

                    foreach (var entity in dishType)
                    {
                        var listViewItem = lstMain.Items.Add(entity.name);
                        listViewItem.Tag = entity;
                    }
                    header.Text = "Типи страв";
                    break;
            }
            if(selectedIndex != -1 && selectedIndex < lstMain.Items.Count)
                lstMain.SelectedIndices.Add(selectedIndex);

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            switch (currentViewMode)
            {
                case ViewMode.Dishes:
                    {
                        var dlg = new DishEditForm();
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveDish(dlg.CurrentItem, dlg.CurrentIngredients);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case ViewMode.Clients:
                    {
                        var dlg = new ClientEditForm();
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveClient(dlg.CurrentItem);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case ViewMode.Ingredients:
                    {
                        var dlg = new IngredientEditForm();
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveIngredient(dlg.CurrentItem);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case ViewMode.Kitchens:
                    {
                        var dlg = new KitchenEditForm();
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveKitchen(dlg.CurrentItem);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case ViewMode.DishTypes:
                    {
                        var dlg = new DishTypeEditForm();
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveDishType(dlg.CurrentItem);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstMain.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нічого не обрано", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedItem = lstMain.SelectedItems[0].Tag;
            if (selectedItem == null)
            {
                MessageBox.Show("Немає даних", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            switch (selectedItem)
            {
                case DishEntity dishEntity:
                    {
                        var dlg = new DishEditForm();
                        dlg.CurrentItem = dishEntity;
                        dlg.CurrentIngredients = DataLayer.Instance.GetIngredientsByDish(dishEntity.dish_id);
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveDish(dlg.CurrentItem, dlg.CurrentIngredients);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case KitchenEntity kitchenEntity:
                    {
                        var dlg = new KitchenEditForm();
                        dlg.CurrentItem = kitchenEntity;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveKitchen(kitchenEntity);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case DishTypeEntity dishTypeEntity:
                    {
                        var dlg = new DishTypeEditForm();
                        dlg.CurrentItem = dishTypeEntity;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveDishType(dishTypeEntity);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case ClientEntity clientEntity:
                    {
                        var dlg = new ClientEditForm();
                        dlg.CurrentItem = clientEntity;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveClient(clientEntity);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
                case IngredientEntity ingredientEntity:
                    {
                        var dlg = new IngredientEditForm();
                        dlg.CurrentItem = ingredientEntity;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            DataLayer.Instance.SaveIngredient(ingredientEntity);
                            ApplyFilters(sender, e);
                        }
                        break;
                    }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstMain.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нічого не обрано", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Ви впевненні, що бажєте видалити цей запис?", "Увага", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
            {
                return;
            }

            var selectedItem = lstMain.SelectedItems[0].Tag;
            if (selectedItem == null)
            {
                MessageBox.Show("Немає даних", "warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            switch (selectedItem)
            {
                case DishEntity dishEntity:
                    DataLayer.Instance.DeleteDish(dishEntity.dish_id);
                    break;
                case KitchenEntity kitchenEntity:
                    DataLayer.Instance.DeleteKitchen(kitchenEntity.kitchen_id);
                    break;
                case DishTypeEntity dishTypeEntity:
                    DataLayer.Instance.DeleteDishType(dishTypeEntity.dish_type_id);
                    break;
                case ClientEntity clientEntity:
                    DataLayer.Instance.DeleteClient(clientEntity.client_id);
                    break;
                case IngredientEntity ingredientEntity:
                    DataLayer.Instance.DeleteIngredient(ingredientEntity.ingredient_id);
                    break;
            }

            ApplyFilters(sender, e);
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            btnSort.ImageIndex = btnSort.ImageIndex == 4 ? 5 : 4;
            ApplyFilters(sender, e);
        }

        private void ApplyFilters(object sender, EventArgs e)
        {
            filterTimer.Stop();
            filterTimer.Start();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Data files (*.txt, *.json)|*.txt;*.json|All files|*.*";
            dlg.RestoreDirectory = true;
            dlg.DefaultExt = ".json";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(currentData, Formatting.Indented);
                    File.WriteAllText(dlg.FileName, json);
                    MessageBox.Show("Данні успішно експортовано", "Information", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "Щось сталося", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
        }
        
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Data files (*.txt, *.json)|*.txt;*.json|All files|*.*";
            dlg.RestoreDirectory = true;
            dlg.DefaultExt = ".json";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var json = File.ReadAllText(dlg.FileName);
                    switch (currentViewMode)
                    {
                        case ViewMode.Dishes:
                            var items = JsonConvert.DeserializeObject<DishEntity[]>(json);
                            foreach (var item in items)
                            {
                                item.dish_id = -1;
                                DataLayer.Instance.SaveDish(item, null);
                            }
                            ApplyFilters(sender, e);
                            MessageBox.Show("Данні успішно імпортовано", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case ViewMode.Clients:
                            break;
                        case ViewMode.Ingredients:
                            break;
                        case ViewMode.Kitchens:
                            break;
                        case ViewMode.DishTypes:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Щось сталося", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void getStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new StatisticsForm();
            dlg.ShowDialog();
        }
    }
}
