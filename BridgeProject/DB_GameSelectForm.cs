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
    public partial class DB_GameSelectForm : Form
    {

        int SELECTOR_FOLDER = -1;  // -1 для NULL
        bool SELECTOR_FOLDER__is_enabled = false;





        public DB_GameSelectForm()
        {
            InitializeComponent();

            this.m_listview_Games.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.m_listview_Games.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.m_listview_Games.Columns.Add("Комментарий", -2, HorizontalAlignment.Left);
            this.m_listview_Games.Columns.Add("Дата", -2, HorizontalAlignment.Left);
            this.m_listview_Games.Columns.Add("Тип", -2, HorizontalAlignment.Left);
            this.m_listview_Games.Columns.Add("Опции", -2, HorizontalAlignment.Left);
            this.m_listview_Games.Columns.Add("Место", -2, HorizontalAlignment.Left);
            this.m_listview_Games.Columns.Add("Игроки", -2, HorizontalAlignment.Left);

            this.m_listview_Games.ContextMenu = new ContextMenu();
            this.m_listview_Games.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            this.m_listview_Games.ContextMenu.MenuItems.Add(new MenuItem());
            this.m_listview_Games.ContextMenu.MenuItems.Add(new MenuItem());
            this.m_listview_Games.ContextMenu.MenuItems.Add(new MenuItem());
            this.m_listview_Games.ContextMenu.MenuItems[0].Text = "Редактировать";
            this.m_listview_Games.ContextMenu.MenuItems[0].Click += new EventHandler(ContextMenu_OnClickEdit);
            this.m_listview_Games.ContextMenu.MenuItems[1].Text = "Удалить";
            this.m_listview_Games.ContextMenu.MenuItems[1].Click += new EventHandler(ContextMenu_OnClickDelete);
            this.m_listview_Games.ContextMenu.MenuItems[2].Text = "Клонировать";
            this.m_listview_Games.ContextMenu.MenuItems[2].Click += new EventHandler(ContextMenu_OnClickClone);


            LoadFoldersToCombo(false);

            LoadGamesToList();
        }


        public class MenuItemPlus : MenuItem
        {
            public int id = -1;
            public MenuItemPlus(int id)
            {
                this.id = id;
            }
        }


        void ContextMenu_Popup(object sender, EventArgs e)
        {
            if (this.m_listview_Games.SelectedIndices.Count == 0)
            {
                this.m_listview_Games.ContextMenu.MenuItems[0].Enabled = false;
                this.m_listview_Games.ContextMenu.MenuItems[1].Enabled = false;
                this.m_listview_Games.ContextMenu.MenuItems[2].Enabled = false;
            }
            else
            {
                this.m_listview_Games.ContextMenu.MenuItems[0].Enabled = true;
                this.m_listview_Games.ContextMenu.MenuItems[1].Enabled = true;
                this.m_listview_Games.ContextMenu.MenuItems[2].Enabled = true;
            }
        }




        // Загрузка 1 игры в список
        void Load1Game(ListViewItem lvi, System.Data.SqlServerCe.SqlCeDataReader sqlReader)
        {
            bool update = (lvi != null); //обновить существующий lvi или добавить новый?

            // Комментарий
            String str = "";
            if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("Comment")))
            {
                str = sqlReader.GetString(sqlReader.GetOrdinal("Comment")).Trim();
            }
            if (update)
                lvi.Text = str;
            else
                lvi = m_listview_Games.Items.Add(new ListViewItem(str));

            // Дата
            str = "";
            if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("StartDate")))
            {
                DateTime dt = sqlReader.GetDateTime(sqlReader.GetOrdinal("StartDate"));
                str = dt.ToString("dd.MM.yyyy HH") + "ч";
            }
            if (update)
                lvi.SubItems[1].Text = str;
            else
                lvi.SubItems.Add(str);

            // id (тэг)
            if (!update)
            {
                lvi.Tag = sqlReader.GetInt32(sqlReader.GetOrdinal("id"));
            }

            // Тип
            GameType t = (GameType)sqlReader.GetByte(sqlReader.GetOrdinal("Type"));
            if (!update)
            {
                switch (t)
                {
                    case GameType.Robber:
                        lvi.SubItems.Add("Робб");
                        break;
                    case GameType.Sport:
                        lvi.SubItems.Add("Спорт");
                        break;
                    case GameType.Compensat:
                        lvi.SubItems.Add("Комп");
                        break;
                    case GameType.SimpleIMP:
                        lvi.SubItems.Add("IMP");
                        break;
                }
            }
            
            // Строка опций:
            if (!update)
            {
                ArrayOfString arrOptions = new ArrayOfString();
                if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("DealsInMatch")))
                    arrOptions.Add(sqlReader.GetByte(sqlReader.GetOrdinal("DealsInMatch")).ToString() + "x");
                if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("GameOptions")))
                {
                    byte GOptions = sqlReader.GetByte(sqlReader.GetOrdinal("GameOptions"));
                    if (t == GameType.Robber)
                    {
                        bool BonusForOners = ((GOptions & 1) > 0);
                        bool BonusForWholeRobber = ((GOptions & 2) > 0);
                        arrOptions.Add(BonusForWholeRobber ? "за робб" : "за гейм");
                        if (BonusForOners)
                            arrOptions.Add("за онеры");
                    }
                    if (t == GameType.Compensat)
                    {
                        BridgeGameScoring.TypeOfCompensation CompType = (BridgeGameScoring.TypeOfCompensation)(GOptions & 3); //2 bits
                        bool TenCardsIsTwoFits = ((GOptions & 4) > 0); //3rd bit
                        bool LessCompFor2Fits23PC = ((GOptions & 8) > 0); //4th bit
                        switch (CompType)
                        {
                            case BridgeGameScoring.TypeOfCompensation.Chicago:
                                arrOptions.Add("чикаг");
                                break;
                            case BridgeGameScoring.TypeOfCompensation.Europe:
                                arrOptions.Add("европ");
                                if (TenCardsIsTwoFits)
                                    arrOptions.Add("10 за 2");
                                if (LessCompFor2Fits23PC)
                                    arrOptions.Add("< 23-2");
                                break;
                            case BridgeGameScoring.TypeOfCompensation.Milton_York:
                                arrOptions.Add("М-У");
                                break;
                            case BridgeGameScoring.TypeOfCompensation.Moscow:
                                arrOptions.Add("моск");
                                break;
                        }
                    }
                }

                str = "";
                if (arrOptions.Count != 0)
                {
                    for (int j = 0; j < arrOptions.Count; j++)
                    {
                        if (j > 0)
                            str += ", ";
                        str += arrOptions[j];
                    }
                }
                lvi.SubItems.Add(str);
            }

            // Место
            str = "";
            if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("Place")))
            {
                str = sqlReader.GetString(sqlReader.GetOrdinal("Place")).Trim();
            }
            if(update)
                lvi.SubItems[4].Text = str;
            else
                lvi.SubItems.Add(str);

            // Игроки
            str = "";
            str += "N: ";
            if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("N")))
            {
                str += sqlReader.GetString(sqlReader.GetOrdinal("N")).Trim();
            }
            str += ", S: ";
            if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("S")))
            {
                str += sqlReader.GetString(sqlReader.GetOrdinal("S")).Trim();
            }
            str += ", E: ";
            if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("E")))
            {
                str += sqlReader.GetString(sqlReader.GetOrdinal("E")).Trim();
            }
            str += ", W: ";
            if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("W")))
            {
                str += sqlReader.GetString(sqlReader.GetOrdinal("W")).Trim();
            }
            if (update)
                lvi.SubItems[5].Text = str;
            else
                lvi.SubItems.Add(str);

        }

        //Загрузка списка игр
        void LoadGamesToList()
        {
            // Сначала очистить:
            m_listview_Games.Items.Clear();

            // Если папка не выбрана, не показывать ничего!
            if (!SELECTOR_FOLDER__is_enabled)
                return;

            // Теперь загрузить:
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT g.id, g.StartDate, g.Place, g.Comment, g.Type, g.DealsInMatch, g.GameOptions, n.Name as N, s.Name as S, e.Name as E, w.Name as W FROM Games g LEFT JOIN Players n ON g.fk_N = n.id  LEFT JOIN Players s ON g.fk_S = s.id  LEFT JOIN Players e ON g.fk_E = e.id  LEFT JOIN Players w ON g.fk_W = w.id";
            if (SELECTOR_FOLDER__is_enabled)
            {
                if(SELECTOR_FOLDER == -1)
                    sqlQuery.CommandText += " WHERE g.fk_Folder_id IS NULL";
                else
                    sqlQuery.CommandText += (" WHERE g.fk_Folder_id = " + SELECTOR_FOLDER);
            }
            sqlQuery.CommandText += " ORDER BY g.StartDate DESC, g.id ASC";
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = DB.ExecuteReader(sqlQuery);
            while (sqlReader.Read())
            {
                Load1Game(null, sqlReader);
            }
            sqlReader.Close();


            // Подогнать ширину столбцов
            this.m_listview_Games.Columns[0].Width = -1; //Комментарий
            this.m_listview_Games.Columns[1].Width = -1; //Дата
            this.m_listview_Games.Columns[2].Width = -1; //Тип
            this.m_listview_Games.Columns[3].Width = -1; //Опции
            this.m_listview_Games.Columns[4].Width = -1; //Место
            this.m_listview_Games.Columns[5].Width = -1; //Игроки
        }


        //Загрузка комбы папок
        void LoadFoldersToCombo(bool reload)
        {
            // Сначала очистить:
            m_combo_Folders.Items.Clear();

            // Теперь загрузить:
            //1. NULL
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT COUNT(id) FROM Games WHERE fk_Folder_id is NULL";
            object o = DB.ExecuteScalar(sqlQuery);
            int fid = -1;
            string fname = "- нет папки -";
            if (o != null && o != DBNull.Value)
                fname += " (" + (int)o + ")";

            int index = m_combo_Folders.Items.Add(new comboitem_id_name(fid, fname));

            // Выбрать:
            if (SELECTOR_FOLDER__is_enabled && SELECTOR_FOLDER == fid)
            {
                if(reload) dont_react_on_folder_selection = true;
                m_combo_Folders.SelectedIndex = index;
                if (reload) dont_react_on_folder_selection = false;
            }

            // 2. остальные
            sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT f.id as [fid], f.Name as [fname], COUNT(g.id) as [fcount] FROM Folders f LEFT JOIN Games g ON f.id=g.fk_Folder_id GROUP BY f.id, f.Name ORDER BY [fname]";
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = DB.ExecuteReader(sqlQuery);
            while (sqlReader.Read())
            {
                fid = sqlReader.IsDBNull(sqlReader.GetOrdinal("fid")) ? -1 : sqlReader.GetInt32(sqlReader.GetOrdinal("fid"));
                fname = "";
                if (fid == -1)
                    fname = "- нет папки -";
                if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("fname")))
                    fname = sqlReader.GetString(sqlReader.GetOrdinal("fname"));
                fname += " (" + (sqlReader.IsDBNull(sqlReader.GetOrdinal("fcount")) ? 0 : sqlReader.GetInt32(sqlReader.GetOrdinal("fcount"))) + ")";

                index = m_combo_Folders.Items.Add(new comboitem_id_name(fid, fname));

                // Выбрать:
                if (SELECTOR_FOLDER__is_enabled && SELECTOR_FOLDER == fid)
                {
                    if (reload) dont_react_on_folder_selection = true;
                    m_combo_Folders.SelectedIndex = index;
                    if (reload) dont_react_on_folder_selection = false;
                }
            }
            sqlReader.Close();
        }


        // Выбор игры:
        public int m_selected_GameId = -1;
        void m_listview_Games_ItemActivate(object sender, System.EventArgs e)
        {
            m_selected_GameId = (int)m_listview_Games.FocusedItem.Tag;
            this.DialogResult = DialogResult.OK;
        }



        // Создание новой игры
        private void menuItem1_Click(object sender, EventArgs e)
        {
            //brandnew!
            GameOptionsForm_Common form = new GameOptionsForm_Common(false, -1);
            if (form.ShowDialog() == DialogResult.OK && form.done2steps == true)
            {
                // Конечные id папки и игроков (-1 для NULL, если нет то создать)
                int folder_id = DB.DB_GetAttributeId(form.Folder_Id, form.Folder_Name, "Folders", "id", "Name");
                int n_id = DB.DB_GetAttributeId(form.N_Id, form.N_Name, "Players", "id", "Name");
                int s_id = DB.DB_GetAttributeId(form.S_Id, form.S_Name, "Players", "id", "Name");
                int e_id = DB.DB_GetAttributeId(form.E_Id, form.E_Name, "Players", "id", "Name");
                int w_id = DB.DB_GetAttributeId(form.W_Id, form.W_Name, "Players", "id", "Name");

                // Обрезанные строки места и комментария
                string place__cut = DB.DB_GetCuttedString(form.Place, "Games", "Place");
                string comment__cut = DB.DB_GetCuttedString(form.Comment, "Games", "Comment");


                // Создать новую игру:
                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "INSERT INTO Games(Type, GameOptions, DealsInMatch, FirstDealer, ZoneSwims, fk_Folder_id, fk_N, fk_S, fk_E, fk_W, Place, Comment, StartDate) VALUES(@type, @options, @dealscount, @firstdealer, @zoneswims, @folder, @n, @s, @e, @w, @place, @comment, GETDATE())";
                sqlQuery.Parameters.Add("type", form.GameTip);
                sqlQuery.Parameters.Add("options", form.GameOptions);
                sqlQuery.Parameters.Add("dealscount", form.DealsCount);
                sqlQuery.Parameters.Add("zoneswims", form.ZoneSwims);
                sqlQuery.Parameters.Add("firstdealer", form.FirstDealer);

                sqlQuery.Parameters.Add("folder", (folder_id != -1 ? (object)folder_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("n", (n_id != -1 ? (object)n_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("s", (s_id != -1 ? (object)s_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("e", (e_id != -1 ? (object)e_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("w", (w_id != -1 ? (object)w_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("place", (place__cut.Length > 0 ? (object)place__cut : (object)DBNull.Value));
                sqlQuery.Parameters.Add("comment", (comment__cut.Length > 0 ? (object)comment__cut : (object)DBNull.Value));
                sqlQuery.Prepare();

                int new_game_id;
                while (DB.ExecuteNonQuery(sqlQuery, true, out new_game_id) == 0)
                {
                    if (MessageBox.Show("Новая игра не была добавлена!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    {
                        DB.sqlConnection.Close();
                        DB.sqlConnection.Open();
                    }
                    else
                    {
                        return;
                    }
                }

                // Записать id новой игры:
                this.m_selected_GameId = new_game_id;

                this.DialogResult = DialogResult.OK;

            }
        }


        // Редактирование игры
        void ContextMenu_OnClickEdit(object sender, EventArgs e)
        {
            ListViewItem lvi = m_listview_Games.Items[m_listview_Games.SelectedIndices[0]];
            int gameId = (int)lvi.Tag;


            // Открыть окно редактирования
            GameOptionsForm_Common form = new GameOptionsForm_Common(true, gameId);
            if (form.ShowDialog() == DialogResult.OK && form.done2steps == true)
            {
                // Конечные id папки и игроков (-1 для NULL, если нет то создать)
                int folder_id = DB.DB_GetAttributeId(form.Folder_Id, form.Folder_Name, "Folders", "id", "Name");
                int n_id = DB.DB_GetAttributeId(form.N_Id, form.N_Name, "Players", "id", "Name");
                int s_id = DB.DB_GetAttributeId(form.S_Id, form.S_Name, "Players", "id", "Name");
                int e_id = DB.DB_GetAttributeId(form.E_Id, form.E_Name, "Players", "id", "Name");
                int w_id = DB.DB_GetAttributeId(form.W_Id, form.W_Name, "Players", "id", "Name");

                // Обрезанные строки места и комментария
                string place__cut = DB.DB_GetCuttedString(form.Place, "Games", "Place");
                string comment__cut = DB.DB_GetCuttedString(form.Comment, "Games", "Comment");

                // Редактировать в БД
                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "UPDATE Games SET fk_Folder_id = @folder, fk_N = @n, fk_S = @s, fk_E = @e, fk_W = @w, Place = @place, Comment = @comment WHERE id=" + gameId;
                sqlQuery.Parameters.Add("folder", (folder_id != -1 ? (object)folder_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("n", (n_id != -1 ? (object)n_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("s", (s_id != -1 ? (object)s_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("e", (e_id != -1 ? (object)e_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("w", (w_id != -1 ? (object)w_id : (object)DBNull.Value));
                sqlQuery.Parameters.Add("place", (place__cut.Length > 0 ? (object)place__cut : (object)DBNull.Value));
                sqlQuery.Parameters.Add("comment", (comment__cut.Length > 0 ? (object)comment__cut : (object)DBNull.Value));
                sqlQuery.Prepare();

                while (DB.ExecuteNonQuery(sqlQuery, true) == 0)
                {
                    if (MessageBox.Show("Игра " + gameId + " не была изменена!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    {
                        DB.sqlConnection.Close();
                        DB.sqlConnection.Open();
                    }
                    else
                    {
                        return;
                    }
                }

                // Если изменилась папка, убрать из списка;  иначе обновить запись об игре в списке
                if (SELECTOR_FOLDER__is_enabled && folder_id != SELECTOR_FOLDER)
                {
                    // Убрать из списка
                    m_listview_Games.Items.RemoveAt(m_listview_Games.SelectedIndices[0]);

                    // Перезагрузить список папок
                    LoadFoldersToCombo(true);
                }
                else
                {
                    // Обновить запись об игре в списке
                    sqlQuery = DB.CreateQuery();
                    sqlQuery.CommandText = "SELECT g.id, g.StartDate, g.Place, g.Comment, g.Type, g.DealsInMatch, g.GameOptions, n.Name as N, s.Name as S, e.Name as E, w.Name as W FROM Games g LEFT JOIN Players n ON g.fk_N = n.id  LEFT JOIN Players s ON g.fk_S = s.id  LEFT JOIN Players e ON g.fk_E = e.id  LEFT JOIN Players w ON g.fk_W = w.id WHERE g.id=" + gameId;
                    System.Data.SqlServerCe.SqlCeDataReader sqlReader = DB.ExecuteReader(sqlQuery);
                    if (sqlReader.Read())
                    {
                        Load1Game(lvi, sqlReader);
                    }
                    sqlReader.Close();
                }

            }
        }


        // Удаление игры
        void ContextMenu_OnClickDelete(object sender, EventArgs e)
        {
            int gameId = (int) m_listview_Games.Items[m_listview_Games.SelectedIndices[0]].Tag;

            if (MessageBox.Show("Удалить эту игру?", "Игра #" + gameId, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                DeleteGame(gameId);
            }
        }

        bool DeleteGame(int gameId)
        {
            // Удалить из БД
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "DELETE FROM Games WHERE id=" + gameId;

            while (DB.ExecuteNonQuery(sqlQuery, true) == 0)
            {
                if (MessageBox.Show("Игра " + gameId + " не была удалена!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                {
                    DB.sqlConnection.Close();
                    DB.sqlConnection.Open();
                }
                else
                {
                    return false;
                }
            }

            // Убрать из списка
            m_listview_Games.Items.RemoveAt(m_listview_Games.SelectedIndices[0]);

            // Перезагрузить список папок
            LoadFoldersToCombo(true);

            return true;
        }


        // Клонирование игры
        void ContextMenu_OnClickClone(object sender, EventArgs e)
        {
            int gameId = (int)m_listview_Games.Items[m_listview_Games.SelectedIndices[0]].Tag;

            while (CloneGame(gameId) == false)
            {
                if (MessageBox.Show("Попробовать еще раз, обновив соединение с БД?", "db error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    DB.sqlConnection.Close();
                    DB.sqlConnection.Open();
                }
                else
                {
                    return;
                }
            }
        }

        bool CloneGame(int gameId)
        {
            System.Data.SqlServerCe.SqlCeTransaction trans = null;
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = null;
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = null;
            System.Data.SqlServerCe.SqlCeDataReader sqlReader2 = null;

            try
            {
                int comm_maxlen = DB.DB_GetMaxLength("Games", "Comment");

                // Склонировать (используя транзакцию)
                sqlQuery = DB.CreateQuery();
                trans = DB.sqlConnection.BeginTransaction();
                sqlQuery.Transaction = trans;
                sqlQuery.CommandText = "INSERT INTO Games(Type, GameOptions, DealsInMatch, FirstDealer, ZoneSwims, fk_Folder_id, fk_N, fk_S, fk_E, fk_W, Place, Comment, StartDate) (SELECT Type, GameOptions, DealsInMatch, FirstDealer, ZoneSwims, fk_Folder_id, fk_N, fk_S, fk_E, fk_W, Place, (CASE WHEN Comment is NULL THEN '{клон}' ELSE (SUBSTRING(Comment, 1, (" + comm_maxlen + " - LEN(' {клон}'))) + ' {клон}')  END) as Comment_clon, StartDate FROM Games WHERE id=" + gameId + ")";
                if (sqlQuery.ExecuteNonQuery() == 0)
                {
                    trans.Rollback();

                    MessageBox.Show("Игра #" + gameId + " не была склонирована!", "db error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return false;
                }

                int new_gameId = DB.GetLastInsertId(trans);


                sqlQuery.CommandText = "SELECT id FROM Matches WHERE fk_Game_id=" + gameId + " ORDER BY id";
                sqlReader = sqlQuery.ExecuteReader();

                while (sqlReader.Read())
                {
                    int cur_matchId = sqlReader.GetInt32(0);

                    sqlQuery.CommandText = "INSERT INTO Matches(fk_Game_id, SCORE_NS, SCORE_EW) (SELECT " + new_gameId + " as fk_Game_id__new, SCORE_NS, SCORE_EW FROM Matches WHERE id=" + cur_matchId + ")";
                    if (sqlQuery.ExecuteNonQuery() == 0)
                    {
                        sqlReader.Close();
                        trans.Rollback();

                        MessageBox.Show("Игра #" + gameId + " не была склонирована!\nОшибка клонирования матча #" + cur_matchId + "!", "db error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return false;
                    }

                    int new_cur_matchId = DB.GetLastInsertId(trans);


                    /****int added = 0;
                    sqlQuery.CommandText = "INSERT INTO Deals_Rob(fk_Match_id, Pair, Contract, Oners, Result, CardsDistribution) (SELECT " + new_cur_matchId + " as fk_Match_id__new, Pair, Contract, Oners, Result, CardsDistribution FROM Deals_Rob WHERE fk_Match_id=" + cur_matchId + ")";
                    added = sqlQuery.ExecuteNonQuery();

                    sqlQuery.CommandText = "INSERT INTO Deals_Sport(fk_Match_id, Pair, Contract, Result, CardsDistribution, Figures, Fits, StrongestPair) (SELECT " + new_cur_matchId + " as fk_Match_id__new, Pair, Contract, Result, CardsDistribution, Figures, Fits, StrongestPair FROM Deals_Sport WHERE fk_Match_id=" + cur_matchId + ")";
                    added = sqlQuery.ExecuteNonQuery();

                    sqlQuery.CommandText = "INSERT INTO Deals_Double(fk_Match_id, CardsDistribution, Pair1, Contract1, Result1, Pair2, Contract2, Result2, IsSecondStarted) (SELECT " + new_cur_matchId + " as fk_Match_id__new, CardsDistribution, Pair1, Contract1, Result1, Pair2, Contract2, Result2, IsSecondStarted FROM Deals_Double WHERE fk_Match_id=" + cur_matchId + ")";
                    added = sqlQuery.ExecuteNonQuery();*****/

                    int cur_dealId = -1;

                    sqlQuery.CommandText = "SELECT id FROM Deals_Rob WHERE fk_Match_id=" + cur_matchId + " ORDER BY id";
                    sqlReader2 = sqlQuery.ExecuteReader();
                    while (sqlReader2.Read())
                    {
                        cur_dealId = (int) sqlReader2.GetSqlInt32(0);
                        sqlQuery.CommandText = "INSERT INTO Deals_Rob(fk_Match_id, Pair, Contract, Oners, Result, CardsDistribution) (SELECT " + new_cur_matchId + " as fk_Match_id__new, Pair, Contract, Oners, Result, CardsDistribution FROM Deals_Rob WHERE id=" + cur_dealId + ")";
                        if (sqlQuery.ExecuteNonQuery() == 0)
                        {
                            sqlReader2.Close();
                            sqlReader.Close();
                            trans.Rollback();

                            MessageBox.Show("Игра #" + gameId + " не была склонирована!\nОшибка клонирования сдачи #" + cur_dealId + " матча #" + cur_matchId + "!", "db error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            return false;
                        }
                    }
                    sqlReader2.Close();

                    sqlQuery.CommandText = "SELECT id FROM Deals_Sport WHERE fk_Match_id=" + cur_matchId + " ORDER BY id";
                    sqlReader2 = sqlQuery.ExecuteReader();
                    while (sqlReader2.Read())
                    {
                        cur_dealId = (int) sqlReader2.GetSqlInt32(0);
                        sqlQuery.CommandText = "INSERT INTO Deals_Sport(fk_Match_id, Pair, Contract, Result, CardsDistribution, Figures, Fits, StrongestPair) (SELECT " + new_cur_matchId + " as fk_Match_id__new, Pair, Contract, Result, CardsDistribution, Figures, Fits, StrongestPair FROM Deals_Sport WHERE id=" + cur_dealId + ")";
                        if (sqlQuery.ExecuteNonQuery() == 0)
                        {
                            sqlReader2.Close();
                            sqlReader.Close();
                            trans.Rollback();

                            MessageBox.Show("Игра #" + gameId + " не была склонирована!\nОшибка клонирования сдачи #" + cur_dealId + " матча #" + cur_matchId + "!", "db error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            return false;
                        }
                    }
                    sqlReader2.Close();

                    sqlQuery.CommandText = "SELECT id FROM Deals_Double WHERE fk_Match_id=" + cur_matchId + " ORDER BY id";
                    sqlReader2 = sqlQuery.ExecuteReader();
                    while (sqlReader2.Read())
                    {
                        cur_dealId = (int) sqlReader2.GetSqlInt32(0);
                        sqlQuery.CommandText = "INSERT INTO Deals_Double(fk_Match_id, CardsDistribution, Pair1, Contract1, Result1, Pair2, Contract2, Result2, IsSecondStarted) (SELECT " + new_cur_matchId + " as fk_Match_id__new, CardsDistribution, Pair1, Contract1, Result1, Pair2, Contract2, Result2, IsSecondStarted FROM Deals_Double WHERE id=" + cur_dealId + ")";
                        if (sqlQuery.ExecuteNonQuery() == 0)
                        {
                            sqlReader2.Close();
                            sqlReader.Close();
                            trans.Rollback();

                            MessageBox.Show("Игра #" + gameId + " не была склонирована!\nОшибка клонирования сдачи #" + cur_dealId + " матча #" + cur_matchId + "!", "db error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            return false;
                        }
                    }
                    sqlReader2.Close();
                }
                sqlReader.Close();


                // Добавить в список
                sqlQuery.CommandText = "SELECT g.id, g.StartDate, g.Place, g.Comment, g.Type, g.DealsInMatch, g.GameOptions, n.Name as N, s.Name as S, e.Name as E, w.Name as W FROM Games g LEFT JOIN Players n ON g.fk_N = n.id  LEFT JOIN Players s ON g.fk_S = s.id  LEFT JOIN Players e ON g.fk_E = e.id  LEFT JOIN Players w ON g.fk_W = w.id WHERE g.id=" + new_gameId;
                sqlReader = sqlQuery.ExecuteReader();
                if (sqlReader.Read())
                {
                    Load1Game(null, sqlReader);

                    sqlReader.Close();
                    trans.Commit();

                    LoadFoldersToCombo(true); // перезагрузить список папок

                    MessageBox.Show("Игра #" + gameId + " успешно склонирована в #" + new_gameId, "Клонирование успешно", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    return true;
                }
                else
                {
                    sqlReader.Close();
                    trans.Rollback();

                    MessageBox.Show("Игра #" + gameId + " не была склонирована в #" + new_gameId, "Клонирование неудачно", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            catch (System.Data.SqlServerCe.SqlCeException ex)
            {
                if (trans != null)
                {
                    DB.SafeTransRollback(trans);
                }
                if (sqlReader != null && sqlReader.IsClosed == false)
                {
                    sqlReader.Close();
                }
                if (sqlReader2 != null && sqlReader2.IsClosed == false)
                {
                    sqlReader2.Close();
                }


                if (ex.NativeError == 0)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }


        //отмена
        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


        // Выбрана папка
        bool dont_react_on_folder_selection = false;
        private void m_combo_Folders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dont_react_on_folder_selection)
                return;

            if (m_combo_Folders.SelectedIndex == -1)
            {
                SELECTOR_FOLDER__is_enabled = false;
            }
            else
            {
                SELECTOR_FOLDER = ((comboitem_id_name)m_combo_Folders.Items[m_combo_Folders.SelectedIndex]).GetId();
                SELECTOR_FOLDER__is_enabled = true;
            }

            //Обновить выборку
            LoadGamesToList();
        }
    }
}