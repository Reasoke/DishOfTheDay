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

        private void queryEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var qe = new QueryEdit();
            qe.Show();
        }

        private bool edit;
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            edit = false;
            var edt = new EditForm();
            edt.ShowDialog();
            clientTableAdapter.Fill(kitchenDataSet.Client);
            kitchenDataSet.AcceptChanges();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            edit = true;
            var st = new KitchenDataSet.ClientDataTable();
            clientTableAdapter.FillByID(st,Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value));

            object[] row = st.Rows[0].ItemArray;
            var edt = new EditForm(
                Convert.ToInt32(row[0]),
                row[1].ToString(),
                row[2].ToString(),
                row[3].ToString(),
                row[4].ToString(),
                row[5].ToString(),
                row[6].ToString(),
                row[7].ToString(),
                Convert.ToDateTime(row[8]),
                row[9].ToString()
            );
            edt.ShowDialog();
            clientTableAdapter.Fill(kitchenDataSet.Client);
            kitchenDataSet.AcceptChanges();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clientTableAdapter.DeleteQuery(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value));
            clientTableAdapter.Fill(kitchenDataSet.Client);
            kitchenDataSet.AcceptChanges();
        }

        
    }
}
