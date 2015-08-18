using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BridgeProject
{
    public partial class CardsDistributionForm : Form
    {
        CardsDistribution cd;
        public CardsDistributionForm(CardsDistribution cd)
        {
            this.cd = new CardsDistribution(cd);
            InitializeComponent();
        }
        public CardsDistribution GetCD()
        {
            return this.cd;
        }
        protected override void OnClosed(EventArgs e)
        {
            cardsDistributionSelector1.DetachData(false);
        }
    }
}