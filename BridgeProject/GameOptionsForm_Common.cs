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
    public struct comboitem_id_name
    {
        int id;
        string name;
        public override string ToString()
        {
            return name;
        }
        public int GetId()
        {
            return id;
        }
        public comboitem_id_name(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    };

    //combos: folder, type, n, s, e, w
    //texts: place, comment

    public partial class GameOptionsForm_Common : Form
    {
        bool edit_mode = false;
        int game_id = -1;

        public GameOptionsForm_Common(bool edit_mode, int game_id)
        {
            InitializeComponent();

            System.Data.SqlServerCe.SqlCeCommand sqlQuery;
            System.Data.SqlServerCe.SqlCeDataReader sqlReader;

            // РЕЖИМ РЕДАКТИРОВАНИЯ:
            string edit_place = "", edit_comment = "";
            int edit_n = -1, edit_s = -1, edit_e = -1, edit_w = -1, edit_folder = -1;
            GameType edit_type = (GameType) (-1);

            this.edit_mode = edit_mode;
            this.game_id = game_id;
            if (edit_mode)
            {
                this.menuItem1.Text = "Сохранить";
                this.comboBox2.Enabled = false;

                sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "SELECT Type, Place, Comment, fk_N, fk_S, fk_E, fk_W, fk_Folder_id FROM Games WHERE id=" + game_id;
                sqlReader = DB.ExecuteReader(sqlQuery);
                if (sqlReader.Read())
                {
                    edit_type = (GameType)sqlReader.GetByte(sqlReader.GetOrdinal("Type"));
                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("Place")))
                        edit_place = sqlReader.GetString(sqlReader.GetOrdinal("Place")).Trim();
                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("Comment")))
                        edit_comment = sqlReader.GetString(sqlReader.GetOrdinal("Comment")).Trim();
                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("fk_N")))
                        edit_n = sqlReader.GetInt32(sqlReader.GetOrdinal("fk_N"));
                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("fk_S")))
                        edit_s = sqlReader.GetInt32(sqlReader.GetOrdinal("fk_S"));
                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("fk_E")))
                        edit_e = sqlReader.GetInt32(sqlReader.GetOrdinal("fk_E"));
                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("fk_W")))
                        edit_w = sqlReader.GetInt32(sqlReader.GetOrdinal("fk_W"));
                    if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("fk_Folder_id")))
                        edit_folder = sqlReader.GetInt32(sqlReader.GetOrdinal("fk_Folder_id"));
                    sqlReader.Close();
                }
                else
                {
                    sqlReader.Close();
                    this.DialogResult = DialogResult.Cancel;
                }
            }

            // РЕЖИМ РЕДАКТИРОВАНИЯ - выбор типа в комбе
            if (edit_mode)
            {
                switch (edit_type)
                {
                    case GameType.Robber:
                        comboBox2.SelectedIndex = 0;
                        break;
                    case GameType.Sport:
                        comboBox2.SelectedIndex = 1;
                        break;
                    case GameType.Compensat:
                        comboBox2.SelectedIndex = 2;
                        break;
                    case GameType.SimpleIMP:
                        comboBox2.SelectedIndex = 3;
                        break;
                }
            }

            // РЕЖИМ РЕДАКТИРОВАНИЯ - запись в текстбокс места и коммента
            if (edit_mode)
            {
                textbox1.Text = edit_place;
                textBox2.Text = edit_comment;
            }

            // Заполнение 4 комб сразу 'N/S/E/W'
            sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT * FROM Players ORDER BY Name";
            sqlReader = DB.ExecuteReader(sqlQuery);
            while (sqlReader.Read())
            {
                int id = sqlReader.GetInt32(sqlReader.GetOrdinal("id"));
                string name = sqlReader.IsDBNull(sqlReader.GetOrdinal("Name")) ? "" : sqlReader.GetString(sqlReader.GetOrdinal("Name")).Trim();
                int index = comboBox3.Items.Add(new comboitem_id_name(id, name));
                comboBox4.Items.Add(new comboitem_id_name(id, name));
                comboBox5.Items.Add(new comboitem_id_name(id, name));
                comboBox6.Items.Add(new comboitem_id_name(id, name));
                if (edit_mode)
                {
                    if (edit_n != -1 && edit_n == id)
                        comboBox3.SelectedIndex = index;
                    if (edit_s != -1 && edit_s == id)
                        comboBox4.SelectedIndex = index;
                    if (edit_e != -1 && edit_e == id)
                        comboBox5.SelectedIndex = index;
                    if (edit_w != -1 && edit_w == id)
                        comboBox6.SelectedIndex = index;
                }
            }
            sqlReader.Close();

            // Заполнение комбы 'Папки'
            sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT * FROM Folders ORDER BY Name";
            sqlReader = DB.ExecuteReader(sqlQuery);
            while (sqlReader.Read())
            {
                int id = sqlReader.GetInt32(sqlReader.GetOrdinal("id"));
                string name = sqlReader.IsDBNull(sqlReader.GetOrdinal("Name")) ? "" : sqlReader.GetString(sqlReader.GetOrdinal("Name")).Trim();
                int index = combobox1.Items.Add(new comboitem_id_name(id, name));
                if (edit_mode)
                {
                    if (edit_folder != -1 && edit_folder == id)
                        combobox1.SelectedIndex = index;
                }
            }
            sqlReader.Close();

        }


        // Далее
        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран тип игры!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }


            // Folder
            if (combobox1.SelectedIndex == -1)
            {
                this.Folder_Id = -1;
                this.Folder_Name = combobox1.Text.Trim();
            }
            else
            {
                this.Folder_Id = ((comboitem_id_name)combobox1.Items[combobox1.SelectedIndex]).GetId();
                this.Folder_Name = "";
            }

            // N
            if (comboBox3.SelectedIndex == -1)
            {
                this.N_Id = -1;
                this.N_Name = comboBox3.Text.Trim();
            }
            else
            {
                this.N_Id = ((comboitem_id_name)comboBox3.Items[comboBox3.SelectedIndex]).GetId();
                this.N_Name = "";
            }

            // S
            if (comboBox4.SelectedIndex == -1)
            {
                this.S_Id = -1;
                this.S_Name = comboBox4.Text.Trim();
            }
            else
            {
                this.S_Id = ((comboitem_id_name)comboBox4.Items[comboBox4.SelectedIndex]).GetId();
                this.S_Name = "";
            }

            // E
            if (comboBox5.SelectedIndex == -1)
            {
                this.E_Id = -1;
                this.E_Name = comboBox5.Text.Trim();
            }
            else
            {
                this.E_Id = ((comboitem_id_name)comboBox5.Items[comboBox5.SelectedIndex]).GetId();
                this.E_Name = "";
            }

            // W
            if (comboBox6.SelectedIndex == -1)
            {
                this.W_Id = -1;
                this.W_Name = comboBox6.Text.Trim();
            }
            else
            {
                this.W_Id = ((comboitem_id_name)comboBox6.Items[comboBox6.SelectedIndex]).GetId();
                this.W_Name = "";
            }

            //Place
            this.Place = textbox1.Text.Trim();

            //Comment
            this.Comment = textBox2.Text.Trim();



            if (edit_mode)
            {
                done2steps = true;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                // Форма настроек игры (зависит от типа)
                if (comboBox2.SelectedIndex == 0)
                {
                    // 0 - Роббер
                    GameOptionsForm_Robber formR = new GameOptionsForm_Robber();
                    if (formR.ShowDialog() == DialogResult.OK && formR.accepted == true)
                    {
                        // скопировать частные опции игры:
                        this.GameTip = (byte)(GameType.Robber);
                        this.GameOptions = formR.GameOptions;
                        this.FirstDealer = (byte)(formR.FirstDealer);
                        this.DealsCount = DBNull.Value;
                        this.ZoneSwims = DBNull.Value;

                        done2steps = true;

                        this.DialogResult = DialogResult.OK;
                    }
                }
                else if (comboBox2.SelectedIndex == 1)
                {
                    // 1 - Дубл
                    GameOptionsForm_Double formD = new GameOptionsForm_Double();
                    if (formD.ShowDialog() == DialogResult.OK && formD.accepted == true)
                    {
                        // скопировать частные опции игры:
                        this.GameTip = (byte)(GameType.Sport);
                        this.GameOptions = DBNull.Value;
                        this.FirstDealer = (byte)(formD.FirstDealer);
                        this.DealsCount = formD.DealsCount;
                        this.ZoneSwims = formD.ZoneSwims;

                        done2steps = true;

                        this.DialogResult = DialogResult.OK;
                    }
                }
                else if (comboBox2.SelectedIndex == 2)
                {
                    // 2 - Комп
                    GameOptionsForm_Comp formC = new GameOptionsForm_Comp();
                    if (formC.ShowDialog() == DialogResult.OK && formC.accepted == true)
                    {
                        // скопировать частные опции игры:
                        this.GameTip = (byte)(GameType.Compensat);
                        this.GameOptions = formC.GameOptions;
                        this.FirstDealer = (byte)(formC.FirstDealer);
                        this.DealsCount = formC.DealsCount;
                        this.ZoneSwims = formC.ZoneSwims;

                        done2steps = true;

                        this.DialogResult = DialogResult.OK;
                    }
                }
                else if (comboBox2.SelectedIndex == 3)
                {
                    // 3 - IMP
                    GameOptionsForm_SimpleIMP formI = new GameOptionsForm_SimpleIMP();
                    if (formI.ShowDialog() == DialogResult.OK && formI.accepted == true)
                    {
                        // скопировать частные опции игры:
                        this.GameTip = (byte)(GameType.SimpleIMP);
                        this.GameOptions = DBNull.Value;
                        this.FirstDealer = (byte)(formI.FirstDealer);
                        this.DealsCount = formI.DealsCount;
                        this.ZoneSwims = formI.ZoneSwims;

                        done2steps = true;

                        this.DialogResult = DialogResult.OK;
                    }
                }
            }
        }

        public bool done2steps = false; //заполнена ли общая и частная (после 'Далее') форма?    (для edit_mode - только эта форма)

        // публичные опции игры
        public object GameOptions;
        public object FirstDealer;
        public object GameTip;
        public object DealsCount;
        public object ZoneSwims;

        // публичные данные об игре
        public int Folder_Id;
        public string Folder_Name;
        public int N_Id;
        public string N_Name;
        public int S_Id;
        public string S_Name;
        public int E_Id;
        public string E_Name;
        public int W_Id;
        public string W_Name;
        public string Place;
        public string Comment;


        // Отмена
        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}