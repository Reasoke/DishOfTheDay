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
    public partial class ReviewEditForm : Form
    {
        public ReviewEditForm()
        {
            InitializeComponent();

            this.Load += ReviewEditForm_Load;
        }

        public int CurrentDishId { get; set; }
        public int CurrentRating { get; set; }

        private void ReviewEditForm_Load(object sender, EventArgs e)
        {
            var currentReview = DataLayer.Instance.GetReview(CurrentDishId);
            if (currentReview == null)
                return;

            CurrentRating = currentReview.rating;
            txtReview.Text = currentReview.review;
            RefreshStarsRating();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pic && pic.Tag is string ratingStr && int.TryParse(ratingStr, out var rating))
            {
                CurrentRating = rating;
                RefreshStarsRating();
            }
        }

        private void RefreshStarsRating()
        {
            pictureBox1.Image = CurrentRating > 0 ? Properties.Resources.star : Properties.Resources.starInactive;
            pictureBox2.Image = CurrentRating > 1 ? Properties.Resources.star : Properties.Resources.starInactive;
            pictureBox3.Image = CurrentRating > 2 ? Properties.Resources.star : Properties.Resources.starInactive;
            pictureBox4.Image = CurrentRating > 3 ? Properties.Resources.star : Properties.Resources.starInactive;
            pictureBox5.Image = CurrentRating > 4 ? Properties.Resources.star : Properties.Resources.starInactive;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DataLayer.Instance.SetReview(CurrentDishId, CurrentRating, txtReview.Text);
            DialogResult = DialogResult.OK;
        }
    }
}
