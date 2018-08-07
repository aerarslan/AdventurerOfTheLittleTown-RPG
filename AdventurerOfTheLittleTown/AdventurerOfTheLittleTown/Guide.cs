using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdventurerOfTheLittleTown
{
    public partial class Guide : Form
    {
        private int pictureCounter = 0;
              
        public Guide()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            pictureCounter++;
            PictureDesigner();
            lblPage.Text = pictureCounter+1 + "/5";
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            pictureCounter--;
            PictureDesigner();
            lblPage.Text = pictureCounter+1 + "/5";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            pictureCounter = 0;
            Close();
        }

        private void PictureDesigner()
        {           
            if (pictureCounter == 0)
                pbGuide.Image = Properties.Resources.guide1;
            else if (pictureCounter == 1)
                pbGuide.Image = Properties.Resources.guide2;
            else if (pictureCounter == 2)
                pbGuide.Image = Properties.Resources.guide3;
            else if (pictureCounter == 3)
                pbGuide.Image = Properties.Resources.guide4;
            else if (pictureCounter == 4)
                pbGuide.Image = Properties.Resources.guide5;
            if (pictureCounter <= -1)
                pictureCounter = 0;
            else if (pictureCounter >= 5)
                pictureCounter = 4;
        }
    }
}
