using Kitchen.Repository;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Kitchen
{
    public partial class EditForm : Form
    {
        private readonly int id;
        /// true - якщо запис модифікується, false - якщо створюється нова
        readonly bool edit;

        public EditForm()
        {
            InitializeComponent();
            var rep = new CommonRepository();
            comboBoxGender.Items.Clear();
            comboBoxGender.Items.AddRange(rep.GetGenders().ToArray());
        }

        public EditForm(int id, string firstName, string lastName, string email, string password, string phone, string address, string descriprion, DateTime dob, string gender) : this()
        {
            edit = true;
            this.id = id;
            textBoxFirstName.Text = firstName;
            textBoxLastName.Text = lastName;
            textBoxEmail.Text = email;
            textBoxPassword.Text = password;
            textBoxPhone.Text = phone;
            textBoxAddress.Text = address;
            textBoxDescription.Text = descriprion;
            
            dateTimePickerDOB.Value = dob;
            switch (gender)
            {
                case "чоловік":
                    comboBoxGender.SelectedIndex = 1;
                    break;
                case "жінка":
                    comboBoxGender.SelectedIndex = 2;
                    break;
                default:
                    comboBoxGender.SelectedIndex = 0;
                    break;
            }
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'kitchenDataSet.Client' table. You can move, or remove it, as needed.
            this.clientTableAdapter.Fill(this.kitchenDataSet.Client);

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string gender;
            switch (comboBoxGender.SelectedIndex)
            {
                case 1:
                    gender = "чоловік";
                    break;
                case 2:
                    gender = "жінка";
                    break;
                default:
                    gender = null;
                    break;
            }

            if (edit)
            {
                clientTableAdapter.UpdateQuery(textBoxFirstName.Text, textBoxLastName.Text, textBoxEmail.Text, textBoxPassword.Text, 
                    textBoxPhone.Text, textBoxAddress.Text, textBoxDescription.Text, dateTimePickerDOB.Value.ToString("g"), comboBoxGender.Text, id);
            }
            else
            {
                clientTableAdapter.Insert(textBoxFirstName.Text, textBoxLastName.Text, textBoxEmail.Text, textBoxPassword.Text,
                   textBoxPhone.Text, textBoxAddress.Text, textBoxDescription.Text, dateTimePickerDOB.Value, comboBoxGender.Text);
            }
            Close();
        }
    }
}
