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
    public partial class GameOptionsForm_Comp : Form
    {
        public GameOptionsForm_Comp()
        {
            InitializeComponent();

            // По умолчанию:
            this.radioButton3.Checked = true; //N
            this.radioButton2.Checked = true; //zone swims
            this.combo1.SelectedIndex = 0; //8
            this.radioButton8.Checked = true; //moscow
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
            }
            else
            {
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
            }
        }

        // create game
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

            // Кол-во сдач в каждом матче
            DealsCount = (byte)(int) combo1.Items[combo1.SelectedIndex];

            // Плавает зона?
            ZoneSwims = radioButton2.Checked;
            
            // Опции
            BridgeGameScoring.TypeOfCompensation CompType = BridgeGameScoring.TypeOfCompensation.Moscow;
            if (radioButton8.Checked)
                CompType = BridgeGameScoring.TypeOfCompensation.Moscow;
            else if (radioButton1.Checked)
                CompType = BridgeGameScoring.TypeOfCompensation.Europe;
            else if (radioButton9.Checked)
                CompType = BridgeGameScoring.TypeOfCompensation.Milton_York;
            else if (radioButton10.Checked)
                CompType = BridgeGameScoring.TypeOfCompensation.Chicago;
            bool Europe_10_is_2 = checkBox1.Checked;
            bool Europe_less_23_2 = checkBox2.Checked;
            
            // Создать GameOptions
            GameOptions = 0;
            GameOptions |= (byte)CompType; //2 bits
            if (Europe_10_is_2)
                GameOptions |= 4; //3rd bit
            if (Europe_less_23_2)
                GameOptions |= 8; //4th bit


            accepted = true;
            this.DialogResult = DialogResult.OK;
        }

        public bool accepted = false;

        // публичные опции
        public byte GameOptions;
        public Quarters FirstDealer;
        public byte DealsCount;
        public bool ZoneSwims;


        // cancel
        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }
}