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
    public partial class DB_FoldersManagerForm : Form
    {
        public DB_FoldersManagerForm()
        {
            InitializeComponent();

            LoadFoldersToCombo(m_combo1);
            LoadFoldersToCombo(m_combo2);
        }

        int SELECTOR_FOLDER = -1;  // -1 для NULL
        bool SELECTOR_FOLDER__is_enabled = false;

        int SELECTOR_FOLDER2 = -1;  // -1 для NULL
        bool SELECTOR_FOLDER2__is_enabled = false;


        // Выбрана папка:
        private void m_combo1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Заполнить название в поле "Переименовать"
            if (m_combo1.SelectedIndex == -1)
            {
                SELECTOR_FOLDER__is_enabled = false;
                m_textbox2.Text = "";
            }
            else
            {
                int id = ((comboitem_id_name)m_combo1.Items[m_combo1.SelectedIndex]).GetId();
                SELECTOR_FOLDER = id;
                SELECTOR_FOLDER__is_enabled = true;
                if (id == -1)
                {
                    m_textbox2.Text = ""; // т.к. NULL
                }
                else
                {
                    System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                    sqlQuery.CommandText = "SELECT Name FROM Folders WHERE id=" + id;
                    object o = DB.ExecuteScalar(sqlQuery);
                    if (o == null || o == DBNull.Value)
                        m_textbox2.Text = "?";
                    else
                        m_textbox2.Text = ((string)o).Trim();
                }
            }
        }


        // Выбрана папка "to":
        private void m_combo2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_combo2.SelectedIndex == -1)
            {
                SELECTOR_FOLDER2__is_enabled = false;
            }
            else
            {
                int id = ((comboitem_id_name)m_combo2.Items[m_combo2.SelectedIndex]).GetId();
                SELECTOR_FOLDER2 = id;
                SELECTOR_FOLDER2__is_enabled = true;
            }
        }


        // Добавить папку:
        private void m_btn1_Click(object sender, EventArgs e)
        {
            string name = m_textbox1.Text.Trim();

            if (name.Length == 0)
            {
                MessageBox.Show("Название не введено!");
                return;
            }
            else
            {
                string name__cut = DB.DB_GetCuttedString(name, "Folders", "Name");

                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "SELECT id FROM Folders WHERE Name='" + name__cut + "'";
                object o = DB.ExecuteScalar(sqlQuery);
                if (o != null && o != DBNull.Value)
                {
                    MessageBox.Show("Папка с таким именем уже есть!");
                    return;
                }
                else
                {
                    sqlQuery.CommandText = "INSERT INTO Folders(Name) VALUES('" + name__cut + "')";
                    int new_folder_id;

                    while (DB.ExecuteNonQuery(sqlQuery, true, out new_folder_id) == 0)
                    {
                        if (MessageBox.Show("Папка [" + name__cut + "] не была добавлена!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                        {
                            DB.sqlConnection.Close();
                            DB.sqlConnection.Open();
                        }
                        else
                        {
                            return;
                        }
                    }

                    // ОБНОВИТЬ КОМБЫ
                    LoadFoldersToCombo(m_combo1);
                    LoadFoldersToCombo(m_combo2);

                    MessageBox.Show("Папка [" + GetFolderName(new_folder_id) + "] добавлена!");
                    return;
                }
            }
        }

        // Переименовать папку:
        private void m_btn3_Click(object sender, EventArgs e)
        {
            if(m_combo1.SelectedIndex == -1)
            {
                MessageBox.Show("Папка не выбрана!");
                return;
            }

            string name = m_textbox2.Text.Trim();
            int id = ((comboitem_id_name)m_combo1.Items[m_combo1.SelectedIndex]).GetId();

            if (name.Length == 0)
            {
                MessageBox.Show("Название не введено!");
                return;
            }
            else if(id == -1) //NULL
            {
                MessageBox.Show("Выберите другую папку!");
                return;
            }
            else
            {
                string name__cut = DB.DB_GetCuttedString(name, "Folders", "Name");

                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "SELECT id FROM Folders WHERE Name='" + name__cut + "'";
                object o = DB.ExecuteScalar(sqlQuery);
                if (o != null && o != DBNull.Value)
                {
                    MessageBox.Show("Папка с таким именем уже есть!");
                    return;
                }
                else
                {
                    sqlQuery.CommandText = "UPDATE Folders SET Name='" + name__cut + "' WHERE id=" + id;

                    while (DB.ExecuteNonQuery(sqlQuery, true) == 0)
                    {
                        if (MessageBox.Show("Папка не была переименована!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                        {
                            DB.sqlConnection.Close();
                            DB.sqlConnection.Open();
                        }
                        else
                        {
                            return;
                        }
                    }

                    // ОБНОВИТЬ КОМБЫ
                    LoadFoldersToCombo(m_combo1);
                    LoadFoldersToCombo(m_combo2);

                    MessageBox.Show("Папка переименована в [" + GetFolderName(id) + "]!");
                    return;
                }
            }
        }

        // Переместить игры в другую папку:
        private void m_btn2_Click(object sender, EventArgs e)
        {
            if (m_combo1.SelectedIndex == -1)
            {
                MessageBox.Show("Начальная папка не выбрана!");
                return;
            }
            if (m_combo2.SelectedIndex == -1)
            {
                MessageBox.Show("Конечная папка не выбрана!");
                return;
            }

            int id1 = ((comboitem_id_name)m_combo1.Items[m_combo1.SelectedIndex]).GetId();
            int id2 = ((comboitem_id_name)m_combo2.Items[m_combo2.SelectedIndex]).GetId();
            if (id1 == id2)
            {
                MessageBox.Show("Начальная и конечная папки совпадают!");
                return;
            }


            if (MessageBox.Show("Переместить игры из [" + GetFolderName(id1) + "] в [" + GetFolderName(id2) + "]?", "Уверены?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;


            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "UPDATE Games SET fk_Folder_id = " + (id2 == -1 ? "NULL" : id2.ToString()) + " WHERE fk_Folder_id" + (id1 == -1 ? " IS NULL" : "=" + id1);
            int rows_affected = DB.ExecuteNonQuery(sqlQuery, true);

            // ОБНОВИТЬ КОМБЫ
            LoadFoldersToCombo(m_combo1);
            LoadFoldersToCombo(m_combo2);

            MessageBox.Show("Перемещено " + rows_affected + " игр из [" + GetFolderName(id1) + "] в [" + GetFolderName(id2) + "]!");
            return;

        }

        // Очистить:
        private void m_btn4_Click(object sender, EventArgs e)
        {
            if (m_combo1.SelectedIndex == -1)
            {
                MessageBox.Show("Папка не выбрана!");
                return;
            }

            int id = ((comboitem_id_name)m_combo1.Items[m_combo1.SelectedIndex]).GetId();

            if (MessageBox.Show("Очистить папку [" + GetFolderName(id) + "]?", "Уверены?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;
            
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "DELETE FROM Games WHERE fk_Folder_id" + (id == -1 ? " IS NULL" : "=" + id);
            int rows_affected = DB.ExecuteNonQuery(sqlQuery, true);


            // ОБНОВИТЬ КОМБЫ
            LoadFoldersToCombo(m_combo1);
            LoadFoldersToCombo(m_combo2);

            MessageBox.Show("Папка [" + GetFolderName(id) + "] очищена!\nБыло удалено " + rows_affected + " игр.");
            return;
        }

        // Удалить:
        private void m_btn5_Click(object sender, EventArgs e)
        {
            if (m_combo1.SelectedIndex == -1)
            {
                MessageBox.Show("Папка не выбрана!");
                return;
            }

            int id = ((comboitem_id_name)m_combo1.Items[m_combo1.SelectedIndex]).GetId();
            string name = GetFolderName(id);

            if (id == -1)
            {
                MessageBox.Show("Папку [NULL] нельзя удалить!");
                return;
            }

            if (MessageBox.Show("Удалить папку [" + name + "]?", "Уверены?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;
            
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "DELETE FROM Games WHERE fk_Folder_id=" + id;
            int rows_affected = DB.ExecuteNonQuery(sqlQuery, true);
            sqlQuery.CommandText = "DELETE FROM Folders WHERE id=" + id;
            int rows_affected_f = DB.ExecuteNonQuery(sqlQuery, true);


            if (rows_affected_f == 0)
            {
                MessageBox.Show("Папка [" + name + "] не была удалена!\nИз нее удалено " + rows_affected + " игр.", "db error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                // ОБНОВИТЬ КОМБЫ
                LoadFoldersToCombo(m_combo1);
                LoadFoldersToCombo(m_combo2);

                MessageBox.Show("Папка [" + name + "] удалена!\nБыло удалено " + rows_affected + " игр.");
                return;
            }
        }

        // **** контролы ****
        //m_textbox1  добавить
        //m_textbox2  переименовать
        //m_combo1    source
        //m_combo2    destination


        //----------------------------------------------------------------------------------------------------



        //Загрузка комбы папок
        void LoadFoldersToCombo(ComboBox combo)
        {
            bool is_from = (combo == m_combo1);
            bool is_to = (combo == m_combo2);

            // Сохранить SELECTOR_FOLDER...
            int saved__SELECTOR_FOLDER = SELECTOR_FOLDER;
            int saved__SELECTOR_FOLDER2 = SELECTOR_FOLDER2;
            bool saved__SELECTOR_FOLDER__is_enabled = SELECTOR_FOLDER__is_enabled;
            bool saved__SELECTOR_FOLDER2__is_enabled = SELECTOR_FOLDER2__is_enabled;

            // Сначала очистить:
            combo.Items.Clear();
            combo.SelectedIndex = -1;

            // Теперь загрузить:
            //1. NULL
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT COUNT(id) FROM Games WHERE fk_Folder_id is NULL";
            object o = DB.ExecuteScalar(sqlQuery);
            int fid = -1;
            string fname = "- нет папки -";
            if (o != null && o != DBNull.Value)
                fname += " (" + (int)o + ")";

            int index = combo.Items.Add(new comboitem_id_name(fid, fname));

            // Выбрать:
            if (is_from && saved__SELECTOR_FOLDER__is_enabled && saved__SELECTOR_FOLDER == fid || is_to && saved__SELECTOR_FOLDER2__is_enabled && saved__SELECTOR_FOLDER2 == fid)
            {
                combo.SelectedIndex = index;
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

                index = combo.Items.Add(new comboitem_id_name(fid, fname));

                // Выбрать:
                if (is_from && saved__SELECTOR_FOLDER__is_enabled && saved__SELECTOR_FOLDER == fid || is_to && saved__SELECTOR_FOLDER2__is_enabled && saved__SELECTOR_FOLDER2 == fid)
                {
                    combo.SelectedIndex = index;
                }
            }
            sqlReader.Close();
        }


        //-----------------------------------------

        // Имя папки?
        string GetFolderName(int id)
        {
            if (id == -1)
            {
                return "NULL";
            }
            else
            {
                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "SELECT Name FROM Folders WHERE id=" + id;
                object o = DB.ExecuteScalar(sqlQuery);
                if (o == null || o == DBNull.Value)
                    return "?";
                else
                    return ((string)o).Trim();
            }
        }

    }
}