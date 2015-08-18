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
    public partial class ScoreHelpForm : Form
    {
        public ScoreHelpForm(bool isrobber)
        {
            this.IsRobber = isrobber;

            InitializeComponent();

            if (!IsRobber)
            {
                this.label1.Text = this.label1.Text.Replace("робберный", "спортивный");
                this.Controls.Remove(this.label5);
                this.Controls.Remove(this.label7);
                this.Controls.Remove(this.label8);
                this.Controls.Remove(this.label9);
                this.Controls.Remove(this.label10);
            }
        }

        bool IsRobber;

        // contract (1-7)
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CountScore();
        }

        // contract (suit)
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CountScore();
        }

        // zone
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            CountScore();
        }

        // contract (double)
        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked == true)
                checkBox3.Checked = false;
            CountScore();
        }

        // contract (redouble)
        private void checkBox3_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
                checkBox2.Checked = false;
            CountScore();
        }

        // result
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CountScore();
        }

        void CountScore()
        {
            bool ready = true;

            bool inzone = checkBox1.Checked;
            int contract_quantity = 0;
            if (comboBox1.SelectedIndex == -1)
                ready = false;
            else
                contract_quantity = int.Parse(comboBox1.Items[comboBox1.SelectedIndex] as string);
            CardTrump contract_trump = CardTrump.NotYetDefined;
            if (comboBox2.SelectedIndex == -1)
            {
                ready = false;
            }
            else
            {
                string trump_str = (string)comboBox2.Items[comboBox2.SelectedIndex];
                switch (trump_str)
                {
                    case "Hearts":
                        contract_trump = CardTrump.Hearts;
                        break;
                    case "Diamonds":
                        contract_trump = CardTrump.Diamonds;
                        break;
                    case "Clubs":
                        contract_trump = CardTrump.Clubs;
                        break;
                    case "Spades":
                        contract_trump = CardTrump.Spades;
                        break;
                    case "NT":
                        contract_trump = CardTrump.NT;
                        break;
                }
            }
            bool contract_double = checkBox2.Checked;
            bool contract_redouble = checkBox3.Checked;
            int result = -1;
            if (comboBox3.SelectedIndex == -1)
                ready = false;
            else
                result = int.Parse(comboBox3.Items[comboBox3.SelectedIndex] as string);
            

            //---------------------------------
            int score = 0;
            int bonus = 0;

            if (ready)
            {
                int result_diff = result - (6 + contract_quantity);
                // ----------------------------- robber ---------------------------
                if (IsRobber)
                {
                    // --- Если контракт сыгран ---
                    if (result_diff >= 0)
                    {
                        // Очки за взятки:
                        score = contract_quantity * (SmallHelper.WhatTrumpType(contract_trump) == TrumpType.Minor ? 20 : 30) + (contract_trump == CardTrump.NT ? 10 : 0);
                        if (contract_double)
                            score *= 2;
                        else if (contract_redouble)
                            score *= 4;
                        // Премия за превышение взяток:
                        ArrayOfInt bonuses = new ArrayOfInt();
                        if (result_diff > 0)
                        {
                            if (contract_double)
                                bonuses.Add(result_diff * (inzone ? 200 : 100));
                            else if (contract_redouble)
                                bonuses.Add(result_diff * (inzone ? 400 : 200));
                            else
                                bonuses.Add(result_diff * (SmallHelper.WhatTrumpType(contract_trump) == TrumpType.Minor ? 20 : 30));
                        }
                        if (contract_quantity == 6) //малый шлем
                        {
                            bonuses.Add(inzone ? 750 : 500);
                        }
                        else if (contract_quantity == 7) //большой шлем
                        {
                            bonuses.Add(inzone ? 1500 : 1000);
                        }
                        // Дополнительная премия за оскорбление:
                        if (contract_double)
                            bonuses.Add(50);
                        else if (contract_redouble)
                            bonuses.Add(100);

                        // Посчитать сумму бонусов
                        for (int i = 0; i < bonuses.Count; i++)
                            bonus += bonuses[i];
                    }
                    // --- Если контракт проигран ---
                    else
                    {
                        int enemy_score = 0;
                        for (int i = -1; i >= result_diff; i--)
                        {
                            if (i == -1)
                            {
                                if (contract_double)
                                    enemy_score += (inzone ? 200 : 100);
                                else if (contract_redouble)
                                    enemy_score += (inzone ? 400 : 200);
                                else
                                    enemy_score += (inzone ? 100 : 50);
                            }
                            else
                            {
                                if (contract_double)
                                    enemy_score += (inzone ? 300 : 200);
                                else if (contract_redouble)
                                    enemy_score += (inzone ? 600 : 400);
                                else
                                    enemy_score += (inzone ? 100 : 50);
                            }
                        }

                        bonus = -enemy_score;
                    }
                }
                // ----------------------------- sport ---------------------------
                else
                {
                    // --- Если контракт сыгран ---
                    if (result_diff >= 0)
                    {
                        // Очки за взятки:
                        score = contract_quantity * (SmallHelper.WhatTrumpType(contract_trump) == TrumpType.Minor ? 20 : 30) + (contract_trump == CardTrump.NT ? 10 : 0);
                        if (contract_double)
                            score *= 2;
                        else if (contract_redouble)
                            score *= 4;
                        // Премия за превышение взяток:
                        if (result_diff > 0)
                        {
                            if (contract_double)
                                bonus += result_diff * (inzone ? 200 : 100);
                            else if (contract_redouble)
                                bonus += result_diff * (inzone ? 400 : 200);
                            else
                                bonus += result_diff * (SmallHelper.WhatTrumpType(contract_trump) == TrumpType.Minor ? 20 : 30);
                        }
                        if (contract_quantity == 6) //малый шлем
                        {
                            bonus += (inzone ? 750 : 500);
                        }
                        if (contract_quantity == 7) //большой шлем
                        {
                            bonus += (inzone ? 1500 : 1000);
                        }
                        if (score >= 100) //за гейм
                        {
                            bonus += (inzone ? 500 : 300);
                        }
                        else //за частичную запись
                        {
                            bonus += 50;
                        }
                        // Дополнительная премия:
                        if (contract_double)
                            bonus += 50;
                        else if (contract_redouble)
                            bonus += 100;
                    }
                    // --- Если контракт проигран ---
                    else
                    {
                        int enemy_score = 0;
                        for (int i = -1; i >= result_diff; i--)
                        {
                            if (i == -1)
                            {
                                if (contract_double)
                                    enemy_score += (inzone ? 200 : 100);
                                else if (contract_redouble)
                                    enemy_score += (inzone ? 400 : 200);
                                else
                                    enemy_score += (inzone ? 100 : 50);
                            }
                            else if (i == -2 || i == -3)
                            {
                                if (contract_double)
                                    enemy_score += (inzone ? 300 : 200);
                                else if (contract_redouble)
                                    enemy_score += (inzone ? 600 : 400);
                                else
                                    enemy_score += (inzone ? 100 : 50);
                            }
                            else
                            {
                                if (contract_double)
                                    enemy_score += (inzone ? 300 : 300);
                                else if (contract_redouble)
                                    enemy_score += (inzone ? 600 : 600);
                                else
                                    enemy_score += (inzone ? 100 : 50);
                            }
                        }

                        bonus = -enemy_score;
                    }
                }
            }
            //---------------------------------



            if (IsRobber)
            {
                label6.Text = score.ToString();
                label7.Text = bonus.ToString();
            }
            else
            {
                label6.Text = (score + bonus).ToString();
            }
        }

        // label6 - score
        // label7 - premium
        // label 1 - заголовок
        // label 8,9,10 - примечание
    }
}