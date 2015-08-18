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
    public partial class GameOptionsForm_SimpleIMP : Form
    {
        public GameOptionsForm_SimpleIMP()
        {
            InitializeComponent();

            // По умолчанию:
            this.radioButton3.Checked = true; //N
            this.radioButton2.Checked = true; //zone swims
            this.combo1.SelectedIndex = 0; //8
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
            DealsCount = (byte)(int)combo1.Items[combo1.SelectedIndex];

            // Плавает зона?
            ZoneSwims = radioButton2.Checked;

            // Опций нет


            accepted = true;
            this.DialogResult = DialogResult.OK;
        }

        public bool accepted = false;

        // публичные опции
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