using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DishOfTheDay.Editors
{
    public partial class StatisticsForm : Form
    {
        public StatisticsForm()
        {
            InitializeComponent();

            txtInformation.Rtf = DataLayer.Instance.GetStatistics();//.Replace("\n", "\r\n");
        }
    }
}
