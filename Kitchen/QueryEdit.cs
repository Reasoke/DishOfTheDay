using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Kitchen
{
    public partial class QueryEdit : Form
    {
        //const string ConnectionString = @"Data Source=.;Initial Catalog=Kitchen;Integrated Security=True";
        string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Kitchen.Properties.Settings.KitchenConnectionString"].ConnectionString;

        public QueryEdit()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection sqlconn = new SqlConnection(ConnectionString);
                sqlconn.Open();
                SqlDataAdapter oda = new SqlDataAdapter(TestInput.Text, sqlconn);
                DataTable dt = new DataTable();
                oda.Fill(dt);
                dataGridView1.DataSource = dt;
                sqlconn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Щось сталося", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestInput.Clear();
            TestInput.Text = "Select";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
