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
    public partial class GameOptionsForm_Robber : Form
    {
        public GameOptionsForm_Robber()
        {
            InitializeComponent();

            // По умолчанию:
            this.radioButton3.Checked = true; //N
            this.radioButton1.Checked = true; //for robber, not for game
            this.checkBox1.Checked = false; //oners - no
        }

        //game create
        private void menuItem1_Click(object sender, EventArgs e)
        {
            // Первый сдающий
            FirstDealer = Quarters.NotDefinedYet;
            if (radioButton3.Checked)
                FirstDealer = Quarters.N;
            else if (radioButton5.Checked)
                FirstDealer = Quarters.W;
            else if (radioButton6.Checked)
                FirstDealer = Quarters.E;
            else if (radioButton4.Checked)
                FirstDealer = Quarters.S;

            // Премия за онеры
            bool BonusForOners = checkBox1.Checked;

            // Премия за геймы сразу, или после роббера
            bool BonusForWholeRobber = radioButton1.Checked;

            // Создать GameOptions
            GameOptions = 0;
            if(BonusForOners)
                GameOptions |= 1;
            if(BonusForWholeRobber)
                GameOptions |= 2;


            accepted = true;
            this.DialogResult = DialogResult.OK;
        }

        public bool accepted = false;

        // публичные опции
        public byte GameOptions;
        public Quarters FirstDealer;


        //cancel
        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }
}