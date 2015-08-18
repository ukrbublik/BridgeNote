//#define CAN_MODIFY_DB_IN_MENU


using System.Windows.Forms;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BridgeProject
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс  следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.SuspendLayout();
            //
            // table
            //
            //this.table.Location = new System.Drawing.Point(60, 5);
            //this.table.Size = new System.Drawing.Size(400, 480);
            //this.table.AutoScrollMargin = new System.Drawing.Size(100, 480);
            //
            // mainMenu1
            //
            MenuItem mi = new MenuItem();
            mi.Text = "Новая сдача";
            mi.Click += MainMenu_NEW_OnClick;
            this.mainMenu1.MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Text = "Меню";
            this.mainMenu1.MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Click += MainMenu_LOAD_OnClick;
            mi.Text = "Список игр";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Click += MainMenu_FOLDERS_OnClick;
            mi.Text = "Редактор папок";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Text = "-";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Click += MainMenu_UNDO;
            mi.Text = "Отменить сдачу";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Click += MainMenu_CLOSE_OnClick;
            mi.Text = "Закрыть игру";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Click += MainMenu_EXPORT_OnClick;
            mi.Text = "Экспорт...";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);
            mi = new MenuItem();
            mi.Text = "-";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Справка";
            int indexHelp = this.mainMenu1.MenuItems[1].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Очки (роббер)";
            mi.Click += new EventHandler(OnHelp_ScoreRob);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Очки (спорт)";
            mi.Click += new EventHandler(OnHelp_ScoreSport);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Компенсация (московская)";
            mi.Click += new EventHandler(OnHelp_CompensationMoscow);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Компенсация (европейская)";
            mi.Click += new EventHandler(OnHelp_CompensationEurope);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Компенсация (чикагская)";
            mi.Click += new EventHandler(OnHelp_CompensationChicago);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Компенсация (Милтона-Уорка)";
            mi.Click += new EventHandler(OnHelp_CompensationMO);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Очки -> IMP";
            mi.Click += new EventHandler(OnHelp_ScoreToIMP);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "IMP -> VP";
            mi.Click += new EventHandler(OnHelp_IMPToVP);
            this.mainMenu1.MenuItems[1].MenuItems[indexHelp].MenuItems.Add(mi);

#if CAN_MODIFY_DB_IN_MENU
            mi = new MenuItem();
            mi.Text = "-";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Совместимость БД";
            int db_compatible__pos = this.mainMenu1.MenuItems[1].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Click += MainMenu_DB_Create;
            mi.Text = "Создать таблицы";
            this.mainMenu1.MenuItems[1].MenuItems[db_compatible__pos].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Click += MainMenu_DB_Modify;
            mi.Text = "Изменить формат";
            this.mainMenu1.MenuItems[1].MenuItems[db_compatible__pos].MenuItems.Add(mi);
#endif

            mi = new MenuItem();
            mi.Text = "-";
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);

            mi = new MenuItem();
            mi.Text = "Выход";
            mi.Click += MainMenu_EXIT_OnClick;
            this.mainMenu1.MenuItems[1].MenuItems.Add(mi);
            this.mainMenu1.MenuItems[0].Enabled = false;

            this.mainMenu1.MenuItems[1].Popup += OnMainMenu1Popup;



            // 
            // Form1
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 640);
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = true;
            this.Name = "MainForm";
            this.Text = "BridgeNote";
            this.ResumeLayout(false);

        }

        void OnMainMenu1Popup(object sender, EventArgs e)
        {
            this.mainMenu1.MenuItems[1].MenuItems[3].Enabled = (table != null && table.CanUndoLastDeal()); //Отменить сдачу
            this.mainMenu1.MenuItems[1].MenuItems[4].Enabled = (table != null); //Закрыть игру

            this.mainMenu1.MenuItems[1].MenuItems[5].Enabled = (table != null && table.CanExport()); //Экспорт
        }



        void OnHelp_ScoreRob(object sender, EventArgs e)
        {
            ScoreHelpForm form = new ScoreHelpForm(true);
            form.ShowDialog();
        }

        void OnHelp_ScoreSport(object sender, EventArgs e)
        {
            ScoreHelpForm form = new ScoreHelpForm(false);
            form.ShowDialog();
        }

        void OnHelp_ScoreToIMP(object sender, EventArgs e)
        {
            // Подготовить данные:
            ArrayList arr_data = new ArrayList();
            arr_data.Add(new ArrayList());
            (arr_data[0] as ArrayList).Add("Очки");
            (arr_data[0] as ArrayList).Add("IMP");

            int index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("20-40");
            (arr_data[index] as ArrayList).Add(1);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("50-80");
            (arr_data[index] as ArrayList).Add(2);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("90-120");
            (arr_data[index] as ArrayList).Add(3);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("130-160");
            (arr_data[index] as ArrayList).Add(4);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("170-210");
            (arr_data[index] as ArrayList).Add(5);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("220-260");
            (arr_data[index] as ArrayList).Add(6);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("270-310");
            (arr_data[index] as ArrayList).Add(7);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("320-360");
            (arr_data[index] as ArrayList).Add(8);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("370-420");
            (arr_data[index] as ArrayList).Add(9);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("430-490");
            (arr_data[index] as ArrayList).Add(10);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("500-590");
            (arr_data[index] as ArrayList).Add(11);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("600-740");
            (arr_data[index] as ArrayList).Add(12);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("750-890");
            (arr_data[index] as ArrayList).Add(13);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("900-1090");
            (arr_data[index] as ArrayList).Add(14);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("1100-1290");
            (arr_data[index] as ArrayList).Add(15);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("1300-1490");
            (arr_data[index] as ArrayList).Add(16);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("1500-1740");
            (arr_data[index] as ArrayList).Add(17);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("1750-1990");
            (arr_data[index] as ArrayList).Add(18);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("2000-2240");
            (arr_data[index] as ArrayList).Add(19);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("2250-2490");
            (arr_data[index] as ArrayList).Add(20);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("2500-2990");
            (arr_data[index] as ArrayList).Add(21);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("3000-3490");
            (arr_data[index] as ArrayList).Add(22);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("3500-3990");
            (arr_data[index] as ArrayList).Add(23);

            index = arr_data.Add(new ArrayList());
            (arr_data[index] as ArrayList).Add("4000+");
            (arr_data[index] as ArrayList).Add(24);

            ArrayList arr_joints = new ArrayList();

            ArrayOfInt widths = new ArrayOfInt();
            widths.Add(new int[] { 100, 50 });
            ArrayOfInt heights = new ArrayOfInt();
            heights.Add(new int[] { 20 });

            // Показать форму:
            ShporaForm f = new ShporaForm();
            f.t.Font = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Regular);
            f.t.FontBold = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Bold);
            f.t.__LoadDataInTable__(arr_data, 2, arr_joints, widths, heights, 1, 0, 16);
            f.t.Location = new System.Drawing.Point((f.Width - f.t.Width) / 2, f.label.Bottom + 5);
            f.label.Text = "Перевод очков в IMP";
            f.ShowDialog();
        }

        void OnHelp_IMPToVP(object sender, EventArgs e)
        {
            // Подготовить данные:
            ArrayList arr_data = new ArrayList();
            arr_data.Add(new ArrayList());
            (arr_data[0] as ArrayList).Add("VP");
            (arr_data[0] as ArrayList).Add("IMP");
            arr_data.Add(new ArrayList());
            (arr_data[1] as ArrayList).Add(null);

            Dictionary<int, ArrayOfInt>.KeyCollection.Enumerator keys_en = BridgeGameScoring.VP_Table.Keys.GetEnumerator();
            while(keys_en.MoveNext())
            {
                (arr_data[1] as ArrayList).Add(keys_en.Current + "x");
            }

            for (int i = 0; i < BridgeGameScoring.vp_total / 2 ; i++)
            {
                arr_data.Add(new ArrayList());
                (arr_data[i + 2] as ArrayList).Add((BridgeGameScoring.vp_total / 2 - i) + ":" + ((BridgeGameScoring.vp_total / 2 + i) > BridgeGameScoring.vp_max ? BridgeGameScoring.vp_max : (BridgeGameScoring.vp_total / 2 + i)));
            }

            keys_en = BridgeGameScoring.VP_Table.Keys.GetEnumerator();
            while (keys_en.MoveNext())
            {
                for (int j = 0; j < BridgeGameScoring.vp_total / 2 ; j++)
                {
                    (arr_data[j + 2] as ArrayList).Add((j == 0 ? 0 : (BridgeGameScoring.VP_Table[keys_en.Current])[j-1] + 1) + "-" + (BridgeGameScoring.VP_Table[keys_en.Current])[j]);
                }
            }


            ArrayList arr_joints = new ArrayList();
            arr_joints.Add(new ShporaControl.table_joint(0, 0, 1, 2));
            arr_joints.Add(new ShporaControl.table_joint(1, 0, BridgeGameScoring.VP_Table.Keys.Count, 1));

            ArrayOfInt widths = new ArrayOfInt();
            widths.Add(50);
            for (int i = 0; i < BridgeGameScoring.VP_Table.Keys.Count; i++)
                widths.Add(46);
            widths[widths.Count - 1] = 54;
            ArrayOfInt heights = new ArrayOfInt();
            heights.Add(new int[] { 24 });

            // Показать форму:
            ShporaForm f = new ShporaForm();
            f.t.Font = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Regular);
            f.t.FontBold = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Bold);
            f.t.__LoadDataInTable__(arr_data, BridgeGameScoring.VP_Table.Keys.Count + 1, arr_joints, widths, heights, 2, 0, 20);
            f.t.Location = new System.Drawing.Point((f.Width - f.t.Width) / 2, f.label.Bottom + 5);
            f.label.Text = "Перевод IMP в VP";
            f.ShowDialog();
        }

        void OnHelp_CompensationChicago(object sender, EventArgs e)
        {
            // Подготовить данные:
            ArrayList arr_data = new ArrayList();
            arr_data.Add(new ArrayList());
            (arr_data[0] as ArrayList).Add("PC");
            (arr_data[0] as ArrayList).Add("до зоны");
            (arr_data[0] as ArrayList).Add("в зоне");
            for (int i = 20; i <= 37; i++)
            {
                int index = arr_data.Add(new ArrayList());
                if (i == 37)
                    (arr_data[index] as ArrayList).Add("37+");
                else
                    (arr_data[index] as ArrayList).Add(i);
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Chicago(i, false));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Chicago(i, true));
            }

            ArrayList arr_joints = new ArrayList();

            ArrayOfInt widths = new ArrayOfInt();
            widths.Add(new int[] { 40, 72, 70 });
            ArrayOfInt heights = new ArrayOfInt();
            heights.Add(new int[] { 24 });

            // Показать форму:
            ShporaForm f = new ShporaForm();
            f.t.Font = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Regular);
            f.t.FontBold = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Bold);
            f.t.__LoadDataInTable__(arr_data, 3, arr_joints, widths, heights, 1, 0, 20);
            f.t.Location = new System.Drawing.Point((f.Width - f.t.Width) / 2, f.label.Bottom + 5);
            f.label.Text = "Чикагская компенсация";
            f.ShowDialog();
        }

        void OnHelp_CompensationMoscow(object sender, EventArgs e)
        {
            // Подготовить данные:
            ArrayList arr_data = new ArrayList();
            arr_data.Add(new ArrayList());
            (arr_data[0] as ArrayList).Add("PC");
            (arr_data[0] as ArrayList).Add("до зоны");
            (arr_data[0] as ArrayList).Add("в зоне");
            for (int i = 20; i <= 36; i++)
            {
                int index = arr_data.Add(new ArrayList());
                if (i == 36)
                    (arr_data[index] as ArrayList).Add("36+");
                else
                    (arr_data[index] as ArrayList).Add(i);
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Moscow(i, false));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Moscow(i, true));
            }

            ArrayList arr_joints = new ArrayList();

            ArrayOfInt widths = new ArrayOfInt();
            widths.Add(new int[] { 40, 72, 70 });
            ArrayOfInt heights = new ArrayOfInt();
            heights.Add(new int[] { 24 });

            // Показать форму:
            ShporaForm f = new ShporaForm();
            f.t.Font = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Regular);
            f.t.FontBold = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Bold);
            f.t.__LoadDataInTable__(arr_data, 3, arr_joints, widths, heights, 1, 0, 20);
            f.t.Location = new System.Drawing.Point((f.Width - f.t.Width) / 2, f.label.Bottom + 5);
            f.label.Text = "Московская компенсация";
            f.ShowDialog();
        }

        void OnHelp_CompensationMO(object sender, EventArgs e)
        {
            // Подготовить данные:
            ArrayList arr_data = new ArrayList();
            arr_data.Add(new ArrayList());
            (arr_data[0] as ArrayList).Add("PC");
            (arr_data[0] as ArrayList).Add("до зоны");
            (arr_data[0] as ArrayList).Add("в зоне");
            for (int i = 20; i <= 40; i++)
            {
                int index = arr_data.Add(new ArrayList());
                (arr_data[index] as ArrayList).Add(i);
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_MiltonYork(i, false));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_MiltonYork(i, true));
            }

            ArrayList arr_joints = new ArrayList();

            ArrayOfInt widths = new ArrayOfInt();
            widths.Add(new int[] { 40, 72, 70 });
            ArrayOfInt heights = new ArrayOfInt();
            heights.Add(new int[] { 20 });

            // Показать форму:
            ShporaForm f = new ShporaForm();
            f.t.Font = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Regular);
            f.t.FontBold = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Bold);
            f.t.__LoadDataInTable__(arr_data, 3, arr_joints, widths, heights, 1, 0, 18);
            f.t.Location = new System.Drawing.Point((f.Width - f.t.Width) / 2, f.label.Bottom + 5);
            f.label.Text = "Компенсация Милтона-Уорка";
            f.ShowDialog();
        }

        void OnHelp_CompensationEurope(object sender, EventArgs e)
        {
            // Подготовить данные:
            ArrayList arr_data = new ArrayList();
            arr_data.Add(new ArrayList());
            (arr_data[0] as ArrayList).Add("PC");
            (arr_data[0] as ArrayList).Add("до зоны");
            (arr_data[0] as ArrayList).Add(null);
            (arr_data[0] as ArrayList).Add(null);
            (arr_data[0] as ArrayList).Add("в зоне");
            (arr_data[0] as ArrayList).Add(null);
            (arr_data[0] as ArrayList).Add(null);
            arr_data.Add(new ArrayList());
            (arr_data[1] as ArrayList).Add(null);
            (arr_data[1] as ArrayList).Add("без\nфита");
            (arr_data[1] as ArrayList).Add("1\nфит");
            (arr_data[1] as ArrayList).Add("2\nфита");
            (arr_data[1] as ArrayList).Add("без\nфита");
            (arr_data[1] as ArrayList).Add("1\nфит");
            (arr_data[1] as ArrayList).Add("2\nфита");
            for (int i = 20; i <= 37; i++)
            {
                int index = arr_data.Add(new ArrayList());
                if(i == 37)
                    (arr_data[index] as ArrayList).Add("36+");
                else if(i == 23)
                    (arr_data[index] as ArrayList).Add("23 (<)");
                else if(i < 23)
                    (arr_data[index] as ArrayList).Add(i);
                else
                    (arr_data[index] as ArrayList).Add(i-1);
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Europe((i <= 23 ? i : i - 1), 0, false, i == 23));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Europe((i <= 23 ? i : i - 1), 1, false, i == 23));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Europe((i <= 23 ? i : i - 1), 2, false, i == 23));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Europe((i <= 23 ? i : i - 1), 0, true, i == 23));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Europe((i <= 23 ? i : i - 1), 1, true, i == 23));
                (arr_data[index] as ArrayList).Add(BridgeGameScoring.Compens_Europe((i <= 23 ? i : i - 1), 2, true, i == 23));
            }

            ArrayList arr_joints = new ArrayList();
            arr_joints.Add(new ShporaControl.table_joint(1, 0, 3, 1));
            arr_joints.Add(new ShporaControl.table_joint(4, 0, 3, 1));
            arr_joints.Add(new ShporaControl.table_joint(0, 0, 1, 2));

            ArrayOfInt widths = new ArrayOfInt();
            widths.Add(new int[] { 54, 46, 46, 46, 46, 46, 46 });
            ArrayOfInt heights = new ArrayOfInt();
            heights.Add(new int[] { 22, 34 });
            
            // Показать форму:
            ShporaForm f = new ShporaForm();
            f.t.Font = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Regular);
            f.t.FontBold = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Bold);
            f.t.__LoadDataInTable__(arr_data, 7, arr_joints, widths, heights, 2, 0, 20);
            f.t.Location = new System.Drawing.Point((f.Width - f.t.Width) / 2, f.label.Bottom + 5);
            f.label.Text = "Европейская компенсация";
            f.ShowDialog();
        }

        void MainMenu_NEW_OnClick(object sender, EventArgs e)
        {
            if (table != null)
            {
                table.AddNewDeal(sender as MenuItem);
            }
        }

        void MainMenu_EXPORT_OnClick(object sender, EventArgs e)
        {
            if (table != null)
            {
                table.Export_CSV();
            }
        }

        void MainMenu_EXIT_OnClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void MainMenu_CLOSE_OnClick(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            KillTable();

            Cursor.Current = Cursors.Default;
        }

        void MainMenu_UNDO(object sender, EventArgs e)
        {
            if (table != null && table.CanUndoLastDeal())
            {
                if (MessageBox.Show("Отменить последнюю сдачу (" + (table.CUR_line + 1) + (table.IsSplit ? "-" + (table.CUR_level + 1) : "") + ")?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    table.UndoLastDeal();
                }
            }
        }

        void MainMenu_FOLDERS_OnClick(object sender, EventArgs e)
        {
            DB_FoldersManagerForm form = new DB_FoldersManagerForm();
            form.ShowDialog();
        }

        void MainMenu_LOAD_OnClick(object sender, EventArgs e)
        {
            DB_GameSelectForm form = new DB_GameSelectForm();
            if (form.ShowDialog() == DialogResult.OK && form.m_selected_GameId != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                KillTable();
                this.table = new BridgeScoreTable(this, form.m_selected_GameId);
                this.Controls.Add(this.table);
                this.mainMenu1.MenuItems[0].Enabled = true;

                Cursor.Current = Cursors.Default;
            }
        }

        void KillTable()
        {
            if (table != null)
            {
                this.mainMenu1.MenuItems[0].Enabled = false;

                this.table.CloseMe();
                this.Controls.Remove(this.table);
                this.table.Dispose();
                this.table = null;
                GC.SuppressFinalize(this);
            }
        }



        void MainMenu_DB_Create(object sender, EventArgs e)
        {
            DB.CreateTables(false);

            DB.Disconnect();
            DB.ReConnect();
        }

        void MainMenu_DB_Modify(object sender, EventArgs e)
        {
            DB.ModifyTable();

            DB.Disconnect();
            DB.ReConnect();
        }

        #endregion
    }
}

