using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Xml;

namespace BridgeProject
{
    static public class DB
    {
        static public System.Data.SqlServerCe.SqlCeConnection sqlConnection = null;
        static public String connectionString = "";

        static public void InitializeConnectionString(string path, string temp_dir)
        {
            connectionString = ("Data Source='" + path + "'") + (temp_dir.Length == 0 ? "" : ("; Temp File Directory='" + temp_dir + "'"));
        }
        
        static public bool CreateDatabase(string path)
        {
            System.Data.SqlServerCe.SqlCeEngine dbEngine = new System.Data.SqlServerCe.SqlCeEngine("Data Source='" + path + "'");
            try
            {
                dbEngine.CreateDatabase();
                return true;
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                // error #25114 - База уже есть
                MessageBox.Show(e.Message, "db error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }
            finally
            {
                dbEngine.Dispose();
            }
        }

        static public void ReConnect()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new System.Data.SqlServerCe.SqlCeConnection(connectionString);
                sqlConnection.Open();
            }

            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
            else if (sqlConnection.State == ConnectionState.Broken)
            {
                sqlConnection.Close();
                sqlConnection.Open();
            }
        }

        static public void Disconnect()
        {
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }
        }

        static public System.Data.SqlServerCe.SqlCeCommand CreateQuery()
        {
            ReConnect();

            return sqlConnection.CreateCommand();
        }

        static public void SafeTransRollback(System.Data.SqlServerCe.SqlCeTransaction trans)
        {
            try
            {
                trans.Rollback();
            }
            catch (System.InvalidOperationException)
            {
            }
        }


        static public int GetLastInsertId(System.Data.SqlServerCe.SqlCeTransaction trans)
        {
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = CreateQuery();
            sqlQuery.CommandText = "SELECT @@IDENTITY";
            if (trans != null)
            {
                sqlQuery.Transaction = trans;
            }
            object o = ExecuteScalar(sqlQuery);
            if (o == null || o == DBNull.Value)
            {
                return -1;
            }
            else
            {
                return (int)(decimal)o;
            }
        }
        static public int GetLastInsertId()
        {
            return GetLastInsertId(null);
        }


        static public System.Data.SqlServerCe.SqlCeDataReader ExecuteReader(System.Data.SqlServerCe.SqlCeCommand command)
        {
            bool retry = false;

            try
            {
                return command.ExecuteReader();
            }
            catch (System.Data.SqlServerCe.SqlCeException ex)
            {
                if (ex.NativeError == 0)
                {
                    retry = true;
                }
                else
                {
                    throw;
                }
            }

            if (retry)
            {
                command.Connection.Close();
                command.Connection.Open();

                return command.ExecuteReader();
            }
            else
                return null;
        }


        static public object ExecuteScalar(System.Data.SqlServerCe.SqlCeCommand command)
        {
            bool retry = false;

            try
            {
                return command.ExecuteScalar();
            }
            catch (System.Data.SqlServerCe.SqlCeException ex)
            {
                if (ex.NativeError == 0)
                {
                    retry = true;
                }
                else
                {
                    throw;
                }
            }

            if (retry)
            {
                command.Connection.Close();
                command.Connection.Open();

                return command.ExecuteScalar();
            }
            else
                return null;
        }



        static private int ExecuteNonQuery(System.Data.SqlServerCe.SqlCeCommand command, bool useTransaction, out int out__rowid, bool findoutLastInsertId)
        {
            bool retry = false;
            bool trans_started = false;
            System.Data.SqlServerCe.SqlCeTransaction trans = null;


            try
            {
                if (useTransaction)
                {
                    trans = command.Connection.BeginTransaction();
                    trans_started = true;
                    command.Transaction = trans;
                }
                int rows_aff = command.ExecuteNonQuery();
                int row_id = -1;
                if (findoutLastInsertId)
                {
                    if (useTransaction)
                        row_id = GetLastInsertId(trans);
                    else
                        row_id = GetLastInsertId();
                }
                if (useTransaction)
                {
                    trans.Commit(System.Data.SqlServerCe.CommitMode.Immediate);
                    trans_started = false;
                    command.Transaction = null;
                }

                out__rowid = row_id;
                return rows_aff;
            }
            catch (System.Data.SqlServerCe.SqlCeException ex)
            {
                ////////MessageBox.Show(ex.Message + "\n" + "HRES = " + ex.HResult + "\n" + "ERRNO = " + ex.NativeError);

                if (useTransaction && trans_started)
                {
                    SafeTransRollback(trans);
                    trans.Dispose();
                    trans = null;
                    command.Transaction = null;
                    trans_started = false;
                }

                if (ex.NativeError == 0)
                {
                    retry = true;
                }
                else
                {
                    throw;
                }
            }



            if (retry)
            {
                command.Connection.Close();
                command.Connection.Open();

                if (useTransaction)
                {
                    trans = command.Connection.BeginTransaction();
                    trans_started = true;
                    command.Transaction = trans;
                }
                int rows_aff = command.ExecuteNonQuery();
                int row_id = -1;
                if (findoutLastInsertId)
                {
                    if (useTransaction)
                        row_id = GetLastInsertId(trans);
                    else
                        row_id = GetLastInsertId();
                }
                if (useTransaction)
                {
                    trans.Commit(System.Data.SqlServerCe.CommitMode.Immediate);
                    trans_started = false;
                    command.Transaction = null;
                }

                out__rowid = row_id;
                return rows_aff;
            }
            else
            {
                out__rowid = -1;
                return 0;
            }
        }

        static public int ExecuteNonQuery(System.Data.SqlServerCe.SqlCeCommand command, bool useTransaction)
        {
            int fake;
            return ExecuteNonQuery(command, useTransaction, out fake, false);
        }

        static public int ExecuteNonQuery(System.Data.SqlServerCe.SqlCeCommand command, bool useTransaction, out int out__rowid)
        {
            return ExecuteNonQuery(command, useTransaction, out out__rowid, true);
        }








        //-------------------------------------------------------- create/modify db 'bridge' ---------------------------------

        /*static public bool CreateBridgeDB()
        {
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = CreateQuery();

            sqlQuery.CommandText = "CREATE DATABASE Bridge";
            try
            {
                sqlQuery.ExecuteNonQuery();
                return true;
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                // error #25114 - База уже есть
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }
        }*/



        static public bool CreateTables(bool use_trans)
        {
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = CreateQuery();
            System.Data.SqlServerCe.SqlCeTransaction trans = null;

            if (use_trans)
            {
                trans = sqlConnection.BeginTransaction();
                sqlQuery.Transaction = trans;
            }

            sqlQuery.CommandText = "CREATE TABLE Folders (id INT IDENTITY PRIMARY KEY, Name NVARCHAR(30))";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }


            /*sqlQuery.CommandText = "alter table folders alter column Name nvarchar(30)";
            sqlQuery.ExecuteNonQuery();
            sqlQuery.CommandText = "alter table players alter column Name nvarchar(50)";
            sqlQuery.ExecuteNonQuery();*/


            sqlQuery.CommandText = "CREATE TABLE Players (id INT IDENTITY PRIMARY KEY, Name NVARCHAR(50))";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }




            sqlQuery.CommandText = "CREATE TABLE Games (id INT IDENTITY PRIMARY KEY, Type TINYINT NOT NULL, GameOptions TINYINT, DealsInMatch TINYINT, FirstDealer TINYINT, ZoneSwims BIT, fk_Folder_id INT, CONSTRAINT FK__Games__no1__Folders__id FOREIGN KEY(fk_Folder_id) REFERENCES Folders(id), fk_N INT, CONSTRAINT FK__Games__N__Players__id FOREIGN KEY(fk_N) REFERENCES Players(id), fk_S INT, CONSTRAINT FK__Games__S__Players__id FOREIGN KEY(fk_S) REFERENCES Players(id), fk_E INT, CONSTRAINT FK__Games__E__Players__id FOREIGN KEY(fk_E) REFERENCES Players(id), fk_W INT, CONSTRAINT FK__Games__W__Players__id FOREIGN KEY(fk_W) REFERENCES Players(id), Place NVARCHAR(30), Comment NVARCHAR(60), StartDate DATETIME)";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

            sqlQuery.CommandText = "CREATE TABLE Matches (id INT IDENTITY PRIMARY KEY, fk_Game_id INT, CONSTRAINT FK__Matches__no1__Games__id FOREIGN KEY(fk_Game_id) REFERENCES Games(id) ON UPDATE CASCADE ON DELETE CASCADE, SCORE_NS INT, SCORE_EW INT)";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }


            sqlQuery.CommandText = "CREATE TABLE Deals_Rob (id INT IDENTITY PRIMARY KEY, fk_Match_id INT, CONSTRAINT FK__Deals_Rob__no1__Matches__id FOREIGN KEY(fk_Match_id) REFERENCES Matches(id) ON UPDATE CASCADE ON DELETE CASCADE, Pair BIT, Contract TINYINT, Oners TINYINT, Result TINYINT, CardsDistribution BINARY(20))";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

            sqlQuery.CommandText = "CREATE TABLE Deals_Sport (id INT IDENTITY PRIMARY KEY, fk_Match_id INT, CONSTRAINT FK__Deals_Sport__no1__Matches__id FOREIGN KEY(fk_Match_id) REFERENCES Matches(id) ON UPDATE CASCADE ON DELETE CASCADE, Pair BIT, Contract TINYINT, Result TINYINT, CardsDistribution BINARY(20), Figures TINYINT, Fits TINYINT, StrongestPair BIT)";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

            sqlQuery.CommandText = "CREATE TABLE Deals_Double (id INT IDENTITY PRIMARY KEY, fk_Match_id INT, CONSTRAINT FK__Deals_Double__no1__Matches__id FOREIGN KEY(fk_Match_id) REFERENCES Matches(id) ON UPDATE CASCADE ON DELETE CASCADE, CardsDistribution BINARY(20), Pair1 BIT, Contract1 TINYINT, Result1 TINYINT, Pair2 BIT, Contract2 TINYINT, Result2 TINYINT, IsSecondStarted BIT)";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }


            if (use_trans)
            {
                try
                {
                    trans.Commit(System.Data.SqlServerCe.CommitMode.Immediate);
                    return true;
                }
                catch (System.Data.SqlServerCe.SqlCeException e)
                {
                    SafeTransRollback(trans);
                    MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        static public void ModifyTable()
        {
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = CreateQuery();
            System.Data.SqlServerCe.SqlCeDataReader dr;



            //--------- total score
            sqlQuery.CommandText = "ALTER TABLE Matches ADD SCORE_NS INT, SCORE_EW INT";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }


            //----------- on delete/update для констраинтов матчей и сдач
            sqlQuery.CommandText = "select CONSTRAINT_NAME from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where (CONSTRAINT_TABLE_NAME = 'Matches' AND UNIQUE_CONSTRAINT_TABLE_NAME = 'Games')";
            object o = sqlQuery.ExecuteScalar();
            if (o != null && o != DBNull.Value)
            {
                sqlQuery.CommandText = "ALTER TABLE Matches DROP CONSTRAINT " + o;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "ALTER TABLE Matches ADD FOREIGN KEY(fk_Game_id) REFERENCES Games(id) ON UPDATE CASCADE ON DELETE CASCADE";
                sqlQuery.ExecuteNonQuery();
            }

            sqlQuery.CommandText = "select CONSTRAINT_NAME from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where (CONSTRAINT_TABLE_NAME = 'Deals_Rob' AND UNIQUE_CONSTRAINT_TABLE_NAME = 'Matches')";
            o = sqlQuery.ExecuteScalar();
            if (o != null && o != DBNull.Value)
            {
                sqlQuery.CommandText = "ALTER TABLE Deals_Rob DROP CONSTRAINT " + o;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "ALTER TABLE Deals_Rob ADD FOREIGN KEY(fk_Match_id) REFERENCES Matches(id) ON UPDATE CASCADE ON DELETE CASCADE";
                sqlQuery.ExecuteNonQuery();
            }

            sqlQuery.CommandText = "select CONSTRAINT_NAME from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where (CONSTRAINT_TABLE_NAME = 'Deals_Sport' AND UNIQUE_CONSTRAINT_TABLE_NAME = 'Matches')";
            o = sqlQuery.ExecuteScalar();
            if (o != null && o != DBNull.Value)
            {
                sqlQuery.CommandText = "ALTER TABLE Deals_Sport DROP CONSTRAINT " + o;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "ALTER TABLE Deals_Sport ADD FOREIGN KEY(fk_Match_id) REFERENCES Matches(id) ON UPDATE CASCADE ON DELETE CASCADE";
                sqlQuery.ExecuteNonQuery();
            }

            sqlQuery.CommandText = "select CONSTRAINT_NAME from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where (CONSTRAINT_TABLE_NAME = 'Deals_Double' AND UNIQUE_CONSTRAINT_TABLE_NAME = 'Matches')";
            o = sqlQuery.ExecuteScalar();
            if (o != null && o != DBNull.Value)
            {
                sqlQuery.CommandText = "ALTER TABLE Deals_Double DROP CONSTRAINT " + o;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "ALTER TABLE Deals_Double ADD FOREIGN KEY(fk_Match_id) REFERENCES Matches(id) ON UPDATE CASCADE ON DELETE CASCADE";
                sqlQuery.ExecuteNonQuery();
            }







            //------------- удалить констраинты из Games
            sqlQuery.CommandText = "select CONSTRAINT_NAME from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where (CONSTRAINT_TABLE_NAME = 'Games' AND UNIQUE_CONSTRAINT_TABLE_NAME = 'Folders')";
            dr = sqlQuery.ExecuteReader();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                {
                    sqlQuery.CommandText = "ALTER TABLE Games DROP CONSTRAINT " + dr.GetString(0);
                    sqlQuery.ExecuteNonQuery();
                }
            }
            dr.Close();



            //----------- folder
            sqlQuery.CommandText = "ALTER TABLE Games ADD fk_Folder_id INT";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

            sqlQuery.CommandText = "ALTER TABLE Games ADD FOREIGN KEY(fk_Folder_id) REFERENCES Folders(id)";
            sqlQuery.ExecuteNonQuery();


            //----------- n s e w
            sqlQuery.CommandText = "ALTER TABLE Games ADD fk_N INT, fk_S INT, fk_E INT, fk_W INT";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

            sqlQuery.CommandText = "ALTER TABLE Games ADD FOREIGN KEY(fk_N) REFERENCES Players(id), FOREIGN KEY(fk_S) REFERENCES Players(id), FOREIGN KEY(fk_E) REFERENCES Players(id), FOREIGN KEY(fk_W) REFERENCES Players(id)";
            sqlQuery.ExecuteNonQuery();



            //---------- place & comment & sdate
            /*sqlQuery.CommandText = "ALTER TABLE Games DROP COLUMN Place";
            sqlQuery.ExecuteNonQuery();
            sqlQuery.CommandText = "ALTER TABLE Games DROP COLUMN Comment";
            sqlQuery.ExecuteNonQuery();
            sqlQuery.CommandText = "ALTER TABLE Games DROP COLUMN StartDate";
            sqlQuery.ExecuteNonQuery();*/

            sqlQuery.CommandText = "ALTER TABLE Games ADD Place NVARCHAR(30), Comment NVARCHAR(60), StartDate DATETIME";
            try
            {
                sqlQuery.ExecuteNonQuery();
            }
            catch (System.Data.SqlServerCe.SqlCeException e)
            {
                MessageBox.Show(e.Message, "Error #" + e.NativeError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }


            //--------- cd
            if (MessageBox.Show("Изменить формат для распределения колоды?\nЭто удалит все сущ. колоды!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                sqlQuery.CommandText = "ALTER TABLE Deals_Sport DROP COLUMN CardsDistribution";
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "ALTER TABLE Deals_Sport ADD CardsDistribution BINARY(20)";
                sqlQuery.ExecuteNonQuery();
            }



            /*sqlQuery.CommandText = "insert into folders (Name) values('тест')";
            sqlQuery.ExecuteNonQuery();
            sqlQuery.CommandText = "insert into folders (Name) values('ааа')";
            sqlQuery.ExecuteNonQuery();
            */




            /*** справка по constaints  ***
            sqlQuery.CommandText = "select * from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS";
            dr = sqlQuery.ExecuteReader();
            while (dr.Read())
            {
                //dr.Read();
                string s = "";
                for (int i = 0; i < dr.FieldCount; i++)
                    s += i + ">" + dr.GetName(i) + " ^ " + dr.GetValue(i).ToString() + "\n"; //MessageBox.Show(dr.GetName(i));
                MessageBox.Show(s);
            }
            dr.Close();
            */
        }



        //  ----------- Функции -------------
        public static int DB_GetMaxLength(string table, string column)
        {
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "select DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + table + "' and COLUMN_NAME='" + column + "'";
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = ExecuteReader(sqlQuery);
            int len = 0;
            if (sqlReader.Read())
                len = (int)sqlReader.GetSqlInt32(sqlReader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH"));
            sqlReader.Close();
            return len;
        }

        public static string DB_GetCuttedString(string str, string table, string column)
        {
            int column_len = DB_GetMaxLength(table, column);
            return (column_len != 0 && str.Length > column_len ? str.Substring(0, column_len) : str);
        }

        public static int DB_GetAttributeId(int selected_id, string selected_name, string table, string column_id, string column_name)
        {
            if (selected_id != -1)
            {
                return selected_id;
            }
            else
            {
                if (selected_name.Length == 0)
                {
                    return -1; //т.е. NULL
                }
                else
                {
                    string selected_name__cut = DB_GetCuttedString(selected_name, table, column_name);

                    System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                    sqlQuery.CommandText = "SELECT " + column_id + " FROM " + table + " WHERE " + column_name + "='" + selected_name__cut + "'";
                    object o = ExecuteScalar(sqlQuery);
                    if (o != null && o != DBNull.Value)
                    {
                        return (int)o;
                    }
                    else
                    {
                        sqlQuery.CommandText = "INSERT INTO " + table + "(" + column_name + ") VALUES('" + selected_name__cut + "')";
                        int inserted_id;
                        ExecuteNonQuery(sqlQuery, true, out inserted_id);
                        return inserted_id;
                    }
                }
            }
        }
    }


    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            String strExePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            ExeDir = strExePath.Substring(0, strExePath.LastIndexOf('\\') + 1);


            //ContractSelector.LoadGraphicsResources();
            //ContractSelector.SetCoordinates();



            // ------------------ start: XML ------------------
            if (System.IO.File.Exists(Program.ExeDir + "BridgeNote.xml") == false)
            {
                MessageBox.Show("Не найден файл BridgeNote.xml!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            String strDBPath = "";
            String strTempDir = "";
            String strDBPath_final = "";
            String strTempDir_final = "";
            bool absDBPath = true;
            bool absTempDir = true;
            bool useTempDir = false;
            XmlReaderSettings xml_settings = new XmlReaderSettings();
            xml_settings.ConformanceLevel = ConformanceLevel.Fragment;
            xml_settings.IgnoreWhitespace = true;
            xml_settings.IgnoreComments = true;
            XmlParserContext xml_context = new XmlParserContext(null, null, null, XmlSpace.Default, System.Text.Encoding.Default);
            XmlReader xml_reader = null;
            try
            {
                xml_reader = XmlReader.Create(Program.ExeDir + "BridgeNote.xml", xml_settings, xml_context);
                xml_reader.Read();
                xml_reader.ReadStartElement("Settings");
                xml_reader.ReadStartElement("Database");
                if (xml_reader.IsStartElement("File"))
                {
                    strDBPath = xml_reader.GetAttribute("path");
                    if (strDBPath == null || strDBPath.Length == 0)
                    {
                        xml_reader.Close();
                        MessageBox.Show("Не указан путь к файлу БД!", "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    try
                    {
                        absDBPath = bool.Parse(xml_reader.GetAttribute("absolute"));
                    }
                    catch (Exception e)
                    {
                        xml_reader.Close();
                        MessageBox.Show("Ошибка в Settings.Database.File.absolute:\n" + e.Message, "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
                else
                {
                    xml_reader.Close();
                    MessageBox.Show("Не найден тег File!", "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return;
                }
                xml_reader.Skip();

                if (xml_reader.IsStartElement("TempDir"))
                {
                    try
                    {
                        useTempDir = bool.Parse(xml_reader.GetAttribute("use"));
                    }
                    catch (Exception e)
                    {
                        xml_reader.Close();
                        MessageBox.Show("Ошибка в Settings.Database.TempDir.use:\n" + e.Message, "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    try
                    {
                        absTempDir = bool.Parse(xml_reader.GetAttribute("absolute"));
                    }
                    catch (Exception e)
                    {
                        xml_reader.Close();
                        MessageBox.Show("Ошибка в Settings.Database.TempDir.absolute:\n" + e.Message, "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    strTempDir = xml_reader.GetAttribute("path");
                    if (useTempDir && (strTempDir == null || absTempDir && strTempDir.Length == 0))
                    {
                        xml_reader.Close();
                        MessageBox.Show("Не указана временная папка для БД!", "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
                else
                {
                    xml_reader.Close();
                    MessageBox.Show("Не найден тег TempDir!", "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return;
                }
                xml_reader.Skip();
                xml_reader.ReadEndElement();
                xml_reader.ReadEndElement();
            }
            catch(XmlException e)
            {
                if (xml_reader != null)
                    xml_reader.Close();
                MessageBox.Show(e.Message, "BridgeNote.xml", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }

            xml_reader.Close();
            // ------------------ end: XML ------------------



            strDBPath_final = (absDBPath ? strDBPath : (Program.ExeDir + strDBPath));
            strTempDir_final = (useTempDir ? (absTempDir ? strTempDir : (Program.ExeDir + strTempDir)) : "");

            // Проверить временную папку
            if (useTempDir && System.IO.Directory.Exists(strTempDir_final) == false)
            {
                MessageBox.Show("Неправильно указана временная папка для БД!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
          
            // Инициализировать строку подключения
            DB.InitializeConnectionString(strDBPath_final, strTempDir_final);

            // Создать БД, если нужно
            if (System.IO.File.Exists(strDBPath_final) == false)
            {
                if (MessageBox.Show("База данных не найдена!\nСоздать новую БД и проинициализировать ее?", "db create", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    if (DB.CreateDatabase(strDBPath_final) == false)
                    {
                        return;
                    }
                    DB.CreateTables(false);
                    DB.Disconnect();
                }
                else
                {
                    return;
                }
            }

            // Подключиться к базе данных
            DB.ReConnect();

            // Следить за подключением
            DB.sqlConnection.StateChange += new StateChangeEventHandler(sqlConnection_StateChange);


            // Запуск главной формы, большой try-catch если DEBUG
#if DEBUG
            try
            {
                Application.Run(MainForm = new Form1());
            }
            catch (Exception e)
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(Program.ExeDir + "debug.txt");
                sw.Write(DateTime.Now.ToString() + "\n");
                sw.Write(e.Message + "\n");
                sw.Write(e.StackTrace + "\n");
                if (e.GetType() == typeof(System.Data.SqlServerCe.SqlCeException))
                {
                    System.Data.SqlServerCe.SqlCeException ex = (System.Data.SqlServerCe.SqlCeException) e;
                    sw.Write("[SQL] NativeError = " + ex.NativeError + "\n");
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        sw.Write("[SQL_err_" + i + "] " + ex.Errors[i].Message + " * " + ex.Errors[i].NativeError + "\n");
                    }
                }
                sw.Write("-------------" + "\n");
                sw.Close();
                throw;
            }
#else
            Application.Run(MainForm = new Form1());
#endif
        }


        static void sqlConnection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (MainForm != null)
            {
                switch (e.CurrentState)
                {
                    case ConnectionState.Open:
                        MainForm.Text = "BridgeNote";
                        break;
                    case ConnectionState.Closed:
                        MainForm.Text = "BridgeNote (c)";
                        break;
                    case ConnectionState.Broken:
                        MainForm.Text = "BridgeNote (b)";
                        break;
                }
            }
        }

        public static Form MainForm;
        public static String ExeDir;

    }
}