using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kitchen
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'kitchenDataSet.Dish' table. You can move, or remove it, as needed.
            this.dishTableAdapter.Fill(this.kitchenDataSet.Dish);
            // TODO: This line of code loads data into the 'kitchenDataSet.ClientDish' table. You can move, or remove it, as needed.
            this.clientDishTableAdapter.Fill(this.kitchenDataSet.ClientDish);
            // TODO: This line of code loads data into the 'kitchenDataSet.Client' table. You can move, or remove it, as needed.
            this.clientTableAdapter.Fill(this.kitchenDataSet.Client);
            dataGridView1.AutoGenerateColumns = true;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientTableAdapter.Update(kitchenDataSet);
            clientDishTableAdapter.Update(kitchenDataSet);
            dishTableAdapter.Update(kitchenDataSet);
        }

        private void clientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = clientBindingSource;
            dataGridView1.DataSource = clientBindingSource;
            label1.Text = "Clients";
        }

        private void clientDishesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = clientDishBindingSource;
            dataGridView1.DataSource = clientDishBindingSource;
            label1.Text = "ClientDishes";
        }

        private void dishesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = dishBindingSource;
            dataGridView1.DataSource = dishBindingSource;
            label1.Text = "Dishes";
        }

        private void clientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rs = new CDForm();
            if (rs.ShowDialog() == DialogResult.OK)
            {
                dishTableAdapter.Fill(kitchenDataSet.Dish);
                clientDishTableAdapter.Fill(kitchenDataSet.ClientDish);
                clientTableAdapter.Fill(kitchenDataSet.Client);
            }
        }

    }
}
