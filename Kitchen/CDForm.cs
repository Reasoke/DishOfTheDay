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
    public partial class CDForm : Form
    {
        public CDForm()
        {
            InitializeComponent();
        }

        private void clientBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.clientBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.kitchenDataSet);

        }

        private void CDForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'kitchenDataSet.ClientDish' table. You can move, or remove it, as needed.
            this.clientDishTableAdapter.Fill(this.kitchenDataSet.ClientDish);
            // TODO: This line of code loads data into the 'kitchenDataSet.Client' table. You can move, or remove it, as needed.
            this.clientTableAdapter.Fill(this.kitchenDataSet.Client);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите подтвердить изменения?", "Изменение данных", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                clientBindingSource.EndEdit();
                clientTableAdapter.Update(kitchenDataSet);
                clientDishTableAdapter.Update(kitchenDataSet);
            }
        }
    }
}
