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
    public partial class CardsDistributionShowForm : Form
    {
        CardsDistribution cd;
        public CardsDistributionShowForm(CardsDistribution cd)
        {
            this.cd = cd;
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
            this.cardsDistributionWatcher1.DetachData(false);
        }
    }
}