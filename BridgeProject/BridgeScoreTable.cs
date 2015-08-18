using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Data.SqlServerCe;
using System.IO;

namespace BridgeProject
{
    public partial class BridgeScoreTable : BaseTable
    {
        public void DisposeMe()
        {

        }

        public void CloseMe()
        {
            // >>> Очистить заголовки, линии и робберы
            this._Close_new();


            // Если остались контролы... ???
            this.Controls.Clear();


            // Извлечь комбо выбора матча
            m_form.Controls.Remove(PAGE__ComboBox);
            
            // Извлечь надпись total-score
            TS_SCore.RemoveChangedHandlers();
            TS_SCore.Clear();
            TS_Label.DetachData(false);
            m_form.Controls.Remove(TS_Label);


        }

        public void _Close_new()
        {
            // Очистить заголовки:
            while (HeadlinesScores.Count > 0)
            {
                DeleteHeadline(HeadlinesScores.Count - 1);
            }

            // Очистить линии
            while (VALUES.Count > 0)
            {
                DeleteLine(VALUES.Count - 1);
            }
            
            // Очистить робберы:
            if (CONTROL_ROBBER != null)
            {
                CONTROL_ROBBER.DetachData();
                m_form.Controls.Remove(CONTROL_ROBBER);
                CONTROL_ROBBER.Dispose();
            }
            if (ROBBERS != null)
            {
                ROBBERS.Clear();
            }
        }

        public void _Close_old()
        {
            if (CONTROLS_COVERS != null)
            {
                for (int i = 0; i < CONTROLS_COVERS.Count; i++)
                {
                    for (int j = 0; j < CONTROLS_COVERS[i].Count; j++)
                    {
                        if(CONTROLS_COVERS[i, j] != null)
                        {
                            (CONTROLS_COVERS[i, j] as ControlCover).DetachControl();
                            CONTROLS_COVERS[i, j].Parent.Controls.Remove(CONTROLS_COVERS[i, j]);
                            CONTROLS_COVERS[i, j].Dispose();
                        }
                    }
                }
            }

            if (CONTROLS != null)
            {
                for (int i = 0; i < CONTROLS.Count; i++)
                {
                    for (int j = 0; j < CONTROLS[i].Count; j++)
                    {
                        if (CONTROLS[i, j] != null)
                        {
                            if (CONTROLS[i, j].Parent != null)
                            {
                                CONTROLS[i, j].Parent.Controls.Remove(CONTROLS[i, j]);
                            }
                            CONTROLS[i, j].Dispose();
                        }
                    }
                }
            }

            if (VALUES != null)
            {
                VALUES.Clear();
            }

            if (CoVa_Dependences != null)
            {
                CoVa_Dependences.Clear();
            }

            // Очистить робберы:
            if(CONTROL_ROBBER != null)
            {
                CONTROL_ROBBER.DetachData();
                m_form.Controls.Remove(CONTROL_ROBBER);
                CONTROL_ROBBER.Dispose();
            }
            if (ROBBERS != null)
            {
                ROBBERS.Clear();
            }

            // Очистить заголовки:
            if (HeadlinesScores_CoverControls != null) //тут была ошибка
            {
                for (int i = 0; i < HeadlinesScores_CoverControls.Count; i++)
                {
                    (HeadlinesScores_CoverControls[i] as ControlCover).DetachControl();
                    if (HeadlinesScores_CoverControls[i].Parent != null)
                    {
                        HeadlinesScores_CoverControls[i].Parent.Controls.Remove(HeadlinesScores_CoverControls[i]);
                    }
                    HeadlinesScores_CoverControls[i].Dispose();
                }
            }
            if (HeadlinesScores_Controls != null)
            {
                for (int i = 0; i < HeadlinesScores_Controls.Count; i++)
                {
                    if (HeadlinesScores_Controls[i].Parent != null)
                    {
                        HeadlinesScores_Controls[i].Parent.Controls.Remove(HeadlinesScores_Controls[i]);
                    }
                    HeadlinesScores_Controls[i].Dispose();
                }
            }
            if (HeadlinesScores != null)
            {
                HeadlinesScores.Clear();
            }
        }


        // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ CSV $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

        public bool CanExport()
        {
            return (PAGE__VIEW && PAGE__NO != -1);
        }

        public void Export_CSV()
        {
            // Код матча: датаначала_типигры_idигры_#матча_idматча
            String matchcode = "";

            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT StartDate FROM Games WHERE id=" + DB_Game_Id;
            object o = DB.ExecuteScalar(sqlQuery);
            if (o != null && o != DBNull.Value)
            {
                matchcode += ((DateTime)o).ToString("dd-MM-yy") + "_";
            }

            switch (GameSettings_GameType)
            {
                case GameType.Compensat:
                    switch (GameSettings_Comp_Type)
                    {
                        case BridgeGameScoring.TypeOfCompensation.Chicago:
                            matchcode += "cch";
                            break;
                        case BridgeGameScoring.TypeOfCompensation.Europe:
                            matchcode += "ceu";
                            break;
                        case BridgeGameScoring.TypeOfCompensation.Milton_York:
                            matchcode += "cmu";
                            break;
                        case BridgeGameScoring.TypeOfCompensation.Moscow:
                            matchcode += "cmo";
                            break;
                    }
                    break;
                case GameType.Robber:
                    matchcode += "rob";
                    break;
                case GameType.SimpleIMP:
                    matchcode += "imp";
                    break;
                case GameType.Sport:
                    matchcode += "spo";
                    break;
            }
            if(!isRobber)
            {
                matchcode += GameSettings_DealsInMatch;
            }
            matchcode += "_" + (PAGE__NO + 1) + "__" + DB_Game_Id + "_" + DB_Matches_Ids_ALL[PAGE__NO];

            String strCSV = "";
            int columns_count = arrHeadersNames.Count + (hide_dealer_in_menu ? 1 : 0);

            for (int i = 0; i < arrHeadersNames.Count; i++)
            {
                if (i > 0)
                    strCSV += ";";

                // Для столбцов "Очки", "IMP", "Компенсация", "Разница"
                bool is_score_col = false;
                if (isSplit && (bool)SUBDEALS_CONTROLS_ISSPLIT[i])
                {
                    if (columnsDataClasses[CoVa_Dependences_SPLIT[i, 0]] == typeof(SimpleScore))
                        is_score_col = true;
                }
                else
                {
                    if (columnsDataClasses[CoVa_Dependences[i, 0]] == typeof(SimpleScore))
                        is_score_col = true;
                }

                if (is_score_col)
                {
                    strCSV += (arrHeadersNames_FULL[i].IndexOf(';') == -1 ? (arrHeadersNames_FULL[i] + " (NS)") : ("\"" + arrHeadersNames_FULL[i] + " (NS)" + "\""));
                    strCSV += ";";
                    strCSV += (arrHeadersNames_FULL[i].IndexOf(';') == -1 ? (arrHeadersNames_FULL[i] + " (EW)") : ("\"" + arrHeadersNames_FULL[i] + " (EW)" + "\""));
                }
                else
                {
                    strCSV += (arrHeadersNames_FULL[i].IndexOf(';') == -1 ? arrHeadersNames_FULL[i] : ("\"" + arrHeadersNames_FULL[i] + "\""));
                }

                if(hide_dealer_in_menu && i == 0)
                    strCSV += ";" + "Сдающий";
            }
            strCSV += "\n";


            for (int i = 0; i < VALUES.Count; i++)
            {
                for(int sub = 0 ; sub < (isSplit ? SUBDEALS.Count : 1) ; sub++)
                {
                    for(int j = 0 ; j <arrHeadersNames.Count ; j++)
                    {
                        if (j > 0)
                            strCSV += ";";
                        if (isSplit && (bool)SUBDEALS_CONTROLS_ISSPLIT[j])
                        {
                            if(VALUES[i, CoVa_Dependences_SPLIT[j, sub]] == null)
                            {
                                if (columnsDataClasses[CoVa_Dependences_SPLIT[j, sub]] == typeof(SimpleScore))
                                {
                                    strCSV += ";";
                                }
                                else
                                {
                                    strCSV += "";
                                }
                            }
                            else if (VALUES[i, CoVa_Dependences_SPLIT[j, sub]].GetType() == typeof(PairSwitcher) && VALUES[i, ContractColumns[sub]] != null && (VALUES[i, ContractColumns[sub]] as Contract).NoContract)
                            {
                                strCSV += "-";
                            }
                            else if (VALUES[i, CoVa_Dependences_SPLIT[j, sub]].GetType() == typeof(Contract))
                            {
                                string temp = (VALUES[i, CoVa_Dependences_SPLIT[j, sub]]).ToString();
                                temp = temp.Replace("♣", "C");
                                temp = temp.Replace("♠", "S");
                                temp = temp.Replace("♥", "H");
                                temp = temp.Replace("♦", "D");
                                strCSV += temp;
                            }
                            else if (VALUES[i, CoVa_Dependences_SPLIT[j, sub]].GetType() == typeof(SimpleScore))
                            {
                                strCSV += (VALUES[i, CoVa_Dependences_SPLIT[j, sub]]).ToString().Replace(" : ", ";");
                            }
                            else
                            {
                                strCSV += (VALUES[i, CoVa_Dependences_SPLIT[j, sub]]).ToString();
                            }
                        }
                        else
                        {
                            if (VALUES[i, CoVa_Dependences[j, 0]] == null)
                            {
                                if (columnsDataClasses[CoVa_Dependences[j, 0]] == typeof(SimpleScore))
                                {
                                    strCSV += ";";
                                }
                                else
                                {
                                    strCSV += "";
                                }
                            }
                            else if (VALUES[i, CoVa_Dependences[j, 0]].GetType() == typeof(PairSwitcher) && VALUES[i, ContractColumns[sub]] != null && (VALUES[i, ContractColumns[sub]] as Contract).NoContract)
                            {
                                strCSV += "-";
                            }
                            else if (VALUES[i, CoVa_Dependences[j, 0]].GetType() == typeof(Contract))
                            {
                                string temp = (VALUES[i, CoVa_Dependences[j, 0]]).ToString();
                                temp = temp.Replace("♣", "C");
                                temp = temp.Replace("♠", "S");
                                temp = temp.Replace("♥", "H");
                                temp = temp.Replace("♦", "D");
                                strCSV += temp;
                            }
                            else if (VALUES[i, CoVa_Dependences[j, 0]].GetType() == typeof(SimpleScore))
                            {
                                if (sub == 0)
                                {
                                    string temp = (VALUES[i, CoVa_Dependences[j, 0]]).ToString();
                                    temp = temp.Replace(" : ", ";");
                                    strCSV += temp;
                                }
                            }
                            else
                            {
                                strCSV += (VALUES[i, CoVa_Dependences[j, 0]]).ToString();
                            }
                        }
                        if (CoVa_Dependences[j].Count == 3) //сдающий
                        {
                            strCSV += ";" + (VALUES[i, CoVa_Dependences[j, 2]]).ToString();
                        }
                    }
                    strCSV += "\n";
                }
            }


            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = Program.ExeDir;
            dlg.FileName = matchcode + ".csv";
            dlg.Filter = "CSV (*.csv)|*.csv";
            dlg.FilterIndex = 0;
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(dlg.FileName, false, Encoding.GetEncoding(1251));
                    sw.Write(strCSV);
                    sw.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                }
            }

            
        }

        // --------------------------------------------------------------------------------------------------------------------------

        // Постраничное отображение:
        bool PAGE__VIEW = true;
        int PAGE__NO = -1;
        int PAGE__LINES_BEFORE = 0; //сколько сдач было до матча PAGE_NO
        ComboBox PAGE__ComboBox = null;
        /*Label*/ ShowTextControl TS_Label = null;
        DealScore_CLASS TS_SCore;
        
        // Последняя созданная сдача
        public int CUR_line = -1;
        public int CUR_level = 0;

        // Высота строки меню на форме (также высота заголовка):
        const int MENUHEIGHT = 52;

        // Улучшение автоскролла
        Control fakeControlForAutoScroll;
        public void AutoScroll_Optimize()
        {
            fakeControlForAutoScroll.Location = new Point(this.AutoScrollPosition.X + GetComfortWidth(), this.AutoScrollPosition.Y + GetComfortHeight() + 1);
        }

        // Данные и контролы для обычных элементов таблицы
        ArrayOfArrayOfControl CONTROLS;
        ArrayOfArrayOfControl CONTROLS_COVERS;
        ArrayOfArrayOfObject VALUES;
        ArrayOfArrayOfInt CoVa_Dependences;

        // Может ли контракт быть "нет контракта"
        bool canbeNoContract = true;
        public bool CanBeNoContract
        {
            get
            {
                return canbeNoContract;
            }
        }

        // Данные и контролы для робберных элементов таблицы
        bool isRobber;
        public bool IsRobber
        {
            get
            {
                return isRobber;
            }
        }
        ArrayOfRobbers ROBBERS;
        RobberControl CONTROL_ROBBER;
        ArrayList HeadlinesScores;
        ArrayOfControl HeadlinesScores_Controls;
        ArrayOfControl HeadlinesScores_CoverControls;

        // Деление сдачи на подсдачи за разными столами
        bool isSplit;
        public bool IsSplit
        {
            get
            {
                return isSplit;
            }
        }
        ArrayOfArrayOfInt SUBDEALS;
        ArrayList SUBDEALS_CONTROLS_ISSPLIT;
        ArrayOfArrayOfInt CoVa_Dependences_SPLIT;

        // REFLECT: зависимости между данными
        Dictionary<int, string[]> REFLECT_Constructors;
        Dictionary<int, int[]> REFLECT_Depends; //зависимость от -1 значит инициализировать при нажатии [NEW], т.е. в начале новой сдачи
        Dictionary<int, int[]> REFLECT_Recounts;
        struct FuncInfo
        {
            // в параметрах указываются id данных
            // константные id данных : см. enum CONST
            // переменные, определяемые динамически в ф-ии REFLEFTOR : ROBBER_NO (№ текущего роббера), ROBBER (текущий роббер), RDEAL_NO (№ сдачи внутри роббера), LINE (глобальный № сдачи-линии)
            // остальные указываются в XML в теге <data> в свойстве "id"
            public string fname;
            public string[] fparams;
            public FuncInfo(string n, string[] parms)
            {
                fname = n;
                fparams = parms;
            }
        };
        public enum CONST { LINE_NO = 100, RDEAL_NO = 101, RDEAL_FULLSCORE = 102, MDEAL_NO = 201, /*SUBDEAL_NO = 103,*/ ROBBER = 1000, ROBBER_NO = 1001, ROBBER_TOTAL_SCORE = 1002, ROBBER_FIRSTLINE = 1003, MATCH_NO = 2001, MATCH_TOTAL_SCORE = 2002, MATCH_FIRSTLINE = 2003, FIRST_DEALER = 3000, LOADING_FROM_DB = 4000, LINE_NO__REAL = 5000, ROBBER_NO__REAL = 5001, MATCH_NO__REAL = 5002 };
        Dictionary<int, string> ID_NAMES;
        Dictionary<int, FuncInfo[]> REFLECT_Functions;
        ArrayOfString REFLECT_InsideFunctionsList;
        ArrayOfInt NotNecessaryValues; //список необязательных данных: распределение карт
        ArrayOfArrayOfInt NotNecessaryValues_NOCONTRACT; //список необязательных данных при "нет контракта"
        ArrayOfInt ContractColumns; //№ колонок с контрактом

        // Классы для данных и контролов:
        ArrayOfTypes columnsDataClasses;
        ArrayOfTypes columnsControlsClasses;

        // Настройки интерфейса:
        ArrayOfString arrHeadersNames;
        ArrayOfString arrHeadersNames_FULL;
        Font fontHeader;
        ArrayOfInt arrHeadersWidths;
        int headerHeight = 34, elementHeight = 20, headlinescoreHeight = 34;
        int offset = 1;
        int offset_sub = -1;
        public Size size_CONTROL_ROBBER = new Size(100 - 7, 400);


        //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        #region Game Settings 1-2-3-4
        void LOAD_GAME_SETTINGS___1_Robber()
        {
            isRobber = true;
            isSplit = false;
            canbeNoContract = true;

            // ОПЦИИ
            GameSettings_Rob_BonusForOners = ((GameSettings_Options & 1) > 0);
            GameSettings_Rob_BonusForWholeRobber = ((GameSettings_Options & 2) > 0);


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! DB !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            DB_Deals_TableName = "Deals_Rob";
            DB_Deals_ColumnsNames.Add(6, "CardsDistribution");
            DB_Deals_ColumnsNames.Add(1, "Pair");
            DB_Deals_ColumnsNames.Add(2, "Contract");
            DB_Deals_ColumnsNames.Add(3, "Result");
            if (GameSettings_Rob_BonusForOners)
                DB_Deals_ColumnsNames.Add(8, "Oners");

            columnsDataClasses = new ArrayOfTypes();
            columnsDataClasses.Add(typeof(ZoneSwitcher));
            columnsDataClasses.Add(typeof(PairSwitcher));
            columnsDataClasses.Add(typeof(Contract));
            columnsDataClasses.Add(typeof(Result));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(IntData));
            columnsDataClasses.Add(typeof(CardsDistribution));
            columnsDataClasses.Add(typeof(QuarterSwitcher));
            if (GameSettings_Rob_BonusForOners)
                columnsDataClasses.Add(typeof(OnersSwitcher));

            ID_NAMES.Add(0, "ZONE");
            ID_NAMES.Add(1, "PAIR");
            ID_NAMES.Add(2, "CONTRACT");
            ID_NAMES.Add(3, "RESULT");
            ID_NAMES.Add(4, "RDEAL_LIGHTSCORE");
            ID_NAMES.Add(5, "NO");
            ID_NAMES.Add(6, "DISTRIBUTION");
            ID_NAMES.Add(7, "DEALER");
            if (GameSettings_Rob_BonusForOners)
                ID_NAMES.Add(8, "ONERS");

            REFLECT_Functions.Add(0, new FuncInfo[] { new FuncInfo("DefineRobberZone", new string[] { "ZONE", "ROBBER", "RDEAL_NO" }) });
            REFLECT_Depends.Add(0, new int[] { -1 });
            REFLECT_Functions.Add(1, new FuncInfo[] { new FuncInfo("NoPairIfNoContract", new string[] { "PAIR", "CONTRACT" }) });
            REFLECT_Depends.Add(1, new int[] { 1, 2 });
            REFLECT_Functions.Add(3, new FuncInfo[] { new FuncInfo("CorrectResultAccordingToContract", new string[] { "RESULT", "CONTRACT", "0" }) });
            REFLECT_Depends.Add(3, new int[] { 2 });
            REFLECT_Constructors.Add(3, new string[] { "CONTRACT" });
            if (GameSettings_Rob_BonusForOners)
            {
                REFLECT_Functions.Add((int)CONST.RDEAL_FULLSCORE, new FuncInfo[] { new FuncInfo("RobberScore", new string[] { "RDEAL_FULLSCORE", "PAIR", "ZONE", "CONTRACT", "RESULT", (GameSettings_Rob_BonusForWholeRobber ? "TRUE" : "FALSE"), "ROBBER", "RDEAL_NO", "ONERS" }) });
                REFLECT_Depends.Add((int)CONST.RDEAL_FULLSCORE, new int[] { 0, 1, 2, 3, 8 });
            }
            else
            {
                REFLECT_Functions.Add((int)CONST.RDEAL_FULLSCORE, new FuncInfo[] { new FuncInfo("RobberScore", new string[] { "RDEAL_FULLSCORE", "PAIR", "ZONE", "CONTRACT", "RESULT", (GameSettings_Rob_BonusForWholeRobber ? "TRUE" : "FALSE"), "ROBBER", "RDEAL_NO" }) });
                REFLECT_Depends.Add((int)CONST.RDEAL_FULLSCORE, new int[] { 0, 1, 2, 3 });
            }
            REFLECT_Functions.Add(4, new FuncInfo[] { new FuncInfo("RobberScoreLight", new string[] { "RDEAL_LIGHTSCORE", "RDEAL_FULLSCORE" }) });
            REFLECT_Depends.Add(4, new int[] { (int)CONST.RDEAL_FULLSCORE });
            REFLECT_Functions.Add(5, new FuncInfo[] { new FuncInfo("IntData_Equal", new string[] { "NO", "RDEAL_NO" }) });
            REFLECT_Constructors.Add(5, new string[] { "true" });
            REFLECT_Depends.Add(5, new int[] { -1 });
            REFLECT_Functions.Add((int)CONST.ROBBER, new FuncInfo[] { new FuncInfo("CleanUnnecessaryRDealsInRobber", new string[] { "ROBBER", "ROBBER_FIRSTLINE" }), new FuncInfo("InvalidateRobber", new string[] { "ROBBER" }) });
            REFLECT_Depends.Add((int)CONST.ROBBER, new int[] { (int)CONST.RDEAL_FULLSCORE });
            REFLECT_Recounts.Add((int)CONST.ROBBER, new int[] { 0, (int)CONST.RDEAL_FULLSCORE });
            REFLECT_Functions.Add((int)CONST.ROBBER_TOTAL_SCORE, new FuncInfo[] { new FuncInfo("SetRobberScore", new string[] { "ROBBER", "ROBBER_TOTAL_SCORE" }), new FuncInfo("TotalScore_RobberMode", new string[] { "ROBBER_TOTAL_SCORE", "ROBBER_NO__REAL" }) });
            REFLECT_Depends.Add((int)CONST.ROBBER_TOTAL_SCORE, new int[] { (int)CONST.ROBBER });
            REFLECT_InsideFunctionsList.Add("CleanUnnecessaryRDealsInRobber");
            REFLECT_InsideFunctionsList.Add("InvalidateRobber");
            REFLECT_InsideFunctionsList.Add("TotalScore_RobberMode");
            REFLECT_Functions.Add(7, new FuncInfo[] { new FuncInfo("DefineRobberDealer", new string[] { "DEALER", "LINE_NO__REAL", "ROBBER_NO__REAL", "RDEAL_NO", "FIRST_DEALER" }) });
            REFLECT_Depends.Add(7, new int[] { -1 });

            if (GameSettings_Rob_BonusForOners)
            {
                REFLECT_Functions.Add(8, new FuncInfo[] { new FuncInfo("GetOners", new string[] { "ONERS", "CONTRACT", "DISTRIBUTION" }) });
                REFLECT_Depends.Add(8, new int[] { 8, 2, 6 });
            }

            NotNecessaryValues.Add(6);
            ContractColumns.Add(new int[] { 2 });
            NotNecessaryValues_NOCONTRACT.Add(new ArrayOfInt());
            NotNecessaryValues_NOCONTRACT[0].Add(new int[] { 1 });
            if (GameSettings_Rob_BonusForOners)
                NotNecessaryValues_NOCONTRACT[0].Add(new int[] { 8 });

            columnsControlsClasses = new ArrayOfTypes();
            columnsControlsClasses.Add(typeof(DealInfoControl));
            columnsControlsClasses.Add(typeof(ShowTextControl_Center));
            columnsControlsClasses.Add(typeof(ShowTextControl));
            columnsControlsClasses.Add(typeof(SwitcherControl_Orange_Center));
            columnsControlsClasses.Add(typeof(ContractSelectControl));
            if (GameSettings_Rob_BonusForOners)
                columnsControlsClasses.Add(typeof(SwitcherControl_Orange));
            columnsControlsClasses.Add(typeof(ResultSelectControl));
            columnsControlsClasses.Add(typeof(ShowSimpleScore));

            arrHeadersNames = new ArrayOfString();
            arrHeadersNames.Add("#");
            arrHeadersNames.Add("Сда\nющ.");
            arrHeadersNames.Add("Зона");
            arrHeadersNames.Add("Па\nра");
            arrHeadersNames.Add("Контракт");
            if (GameSettings_Rob_BonusForOners)
                arrHeadersNames.Add("Онеры");
            arrHeadersNames.Add("Рез-т");
            arrHeadersNames.Add("Очки");

            arrHeadersNames_FULL = new ArrayOfString();
            arrHeadersNames_FULL.Add("#");
            arrHeadersNames_FULL.Add("Сдающий");
            arrHeadersNames_FULL.Add("Зона");
            arrHeadersNames_FULL.Add("Пара");
            arrHeadersNames_FULL.Add("Контракт");
            if (GameSettings_Rob_BonusForOners)
                arrHeadersNames_FULL.Add("Онеры");
            arrHeadersNames_FULL.Add("Рез-т");
            arrHeadersNames_FULL.Add("Очки");


            arrHeadersWidths = new ArrayOfInt();
            if (GameSettings_Rob_BonusForOners)
            {
                arrHeadersWidths.Add(new int[] { 22, 33, 51, 26, 69, /*oners*/51, 45, 45 });
            }
            else
            {
                arrHeadersWidths.Add(new int[] { 22+8, 33, 51+9, 26+9, 69+8, 45+9, 45+9 });
            }

            CoVa_Dependences = new ArrayOfArrayOfInt();
            for (int i = 0; i < arrHeadersNames.Count; i++)
                CoVa_Dependences.Add(new ArrayOfInt());
            CoVa_Dependences[0].Add(new int[] { 5, 6 });
            CoVa_Dependences[1].Add(7);
            CoVa_Dependences[2].Add(0);
            CoVa_Dependences[3].Add(1);
            CoVa_Dependences[4].Add(2);
            if (GameSettings_Rob_BonusForOners)
            {
                CoVa_Dependences[5].Add(8);
                CoVa_Dependences[6].Add(3);
                CoVa_Dependences[7].Add(new int[] { 4 }); //относительно static
            }
            else
            {
                CoVa_Dependences[5].Add(3);
                CoVa_Dependences[6].Add(new int[] { 4 }); //относительно static
            }
        }
        

        void LOAD_GAME_SETTINGS___2_Sport()
        {
            isRobber = false;
            isSplit = true;
            canbeNoContract = true;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! DB !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            DB_Deals_TableName = "Deals_Double";
            DB_Deals_ColumnsNames.Add(1, "CardsDistribution");
            DB_Deals_ColumnsNames.Add(5, "Pair1");
            DB_Deals_ColumnsNames.Add(6, "Contract1");
            DB_Deals_ColumnsNames.Add(7, "Result1");
            DB_Deals_ColumnsNames.Add(9, "Pair2");
            DB_Deals_ColumnsNames.Add(10, "Contract2");
            DB_Deals_ColumnsNames.Add(11, "Result2");

            columnsDataClasses = new ArrayOfTypes();

            columnsDataClasses.Add(typeof(IntData));
            columnsDataClasses.Add(typeof(CardsDistribution));
            columnsDataClasses.Add(typeof(IntData));
            columnsDataClasses.Add(typeof(IntData));
            columnsDataClasses.Add(typeof(ZoneSwitcher));
            columnsDataClasses.Add(typeof(PairSwitcher));
            columnsDataClasses.Add(typeof(Contract));
            columnsDataClasses.Add(typeof(Result));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(PairSwitcher));
            columnsDataClasses.Add(typeof(Contract));
            columnsDataClasses.Add(typeof(Result));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(QuarterSwitcher));


            SUBDEALS = new ArrayOfArrayOfInt();
            SUBDEALS.Add(new ArrayOfInt());
            SUBDEALS[0].Add(new int[] { 2, 5, 6, 7, 8 });
            SUBDEALS.Add(new ArrayOfInt());
            SUBDEALS[1].Add(new int[] { 3, 9, 10, 11, 12 });

            ID_NAMES.Add(0, "NO");
            ID_NAMES.Add(1, "DISTRIBUTION");
            ID_NAMES.Add(2, "SUBNO1");
            ID_NAMES.Add(3, "SUBNO2");
            ID_NAMES.Add(4, "ZONE");
            ID_NAMES.Add(5, "PAIR1");
            ID_NAMES.Add(6, "CONTRACT1");
            ID_NAMES.Add(7, "RESULT1");
            ID_NAMES.Add(8, "SCORE1");
            ID_NAMES.Add(9, "PAIR2");
            ID_NAMES.Add(10, "CONTRACT2");
            ID_NAMES.Add(11, "RESULT2");
            ID_NAMES.Add(12, "SCORE2");
            ID_NAMES.Add(13, "DIFF");
            ID_NAMES.Add(14, "IMP");
            ID_NAMES.Add(15, "DEALER");

            REFLECT_Functions.Add(5, new FuncInfo[] { new FuncInfo("NoPairIfNoContract", new string[] { "PAIR1", "CONTRACT1" }) });
            REFLECT_Depends.Add(5, new int[] { 5, 6 });
            REFLECT_Functions.Add(9, new FuncInfo[] { new FuncInfo("NoPairIfNoContract", new string[] { "PAIR2", "CONTRACT2" }) });
            REFLECT_Depends.Add(9, new int[] { 9, 10 });
            REFLECT_Functions.Add(0, new FuncInfo[] { new FuncInfo("IntData_Equal", new string[] { "NO", "MDEAL_NO" }) });
            REFLECT_Constructors.Add(0, new string[] { "true" });
            REFLECT_Depends.Add(0, new int[] { -1 });
            REFLECT_Functions.Add(2, new FuncInfo[] { new FuncInfo("IntData_Equal", new string[] { "SUBNO1", "0" }) });
            REFLECT_Constructors.Add(2, new string[] { "true" });
            REFLECT_Depends.Add(2, new int[] { -1 });
            REFLECT_Functions.Add(3, new FuncInfo[] { new FuncInfo("IntData_Equal", new string[] { "SUBNO2", "1" }) });
            REFLECT_Constructors.Add(3, new string[] { "true" });
            REFLECT_Depends.Add(3, new int[] { -2 });
            REFLECT_Functions.Add(4, new FuncInfo[] { new FuncInfo("DefineSportZone", new string[] { "ZONE", "LINE_NO__REAL", "MATCH_NO__REAL", "MDEAL_NO", GameSettings_3Sports_ZoneSwims.ToString() }) });
            REFLECT_Depends.Add(4, new int[] { -1 });
            REFLECT_Functions.Add(7, new FuncInfo[] { new FuncInfo("CorrectResultAccordingToContract", new string[] { "RESULT1", "CONTRACT1", "0" }) });
            REFLECT_Depends.Add(7, new int[] { 6 });
            REFLECT_Constructors.Add(7, new string[] { "CONTRACT1" });
            REFLECT_Functions.Add(8, new FuncInfo[] { new FuncInfo("SportScore", new string[] { "SCORE1", "PAIR1", "ZONE", "CONTRACT1", "RESULT1" }) });
            REFLECT_Depends.Add(8, new int[] { 4, 5, 6, 7 });
            REFLECT_Functions.Add(11, new FuncInfo[] { new FuncInfo("CorrectResultAccordingToContract", new string[] { "RESULT2", "CONTRACT2", "0" }) });
            REFLECT_Depends.Add(11, new int[] { 10 });
            REFLECT_Constructors.Add(11, new string[] { "CONTRACT2" });
            REFLECT_Functions.Add(12, new FuncInfo[] { new FuncInfo("SportScoreInvert", new string[] { "SCORE2", "PAIR2", "ZONE", "CONTRACT2", "RESULT2" }) });
            REFLECT_Depends.Add(12, new int[] { 4, 9, 10, 11 });
            REFLECT_Functions.Add(13, new FuncInfo[] { new FuncInfo("ScoreSumm", new string[] { "DIFF", "SCORE1", "SCORE2" }) });
            REFLECT_Depends.Add(13, new int[] { 8, 12 });
            REFLECT_Functions.Add(14, new FuncInfo[] { new FuncInfo("ConvertToIMPs", new string[] { "IMP", "DIFF" }) });
            REFLECT_Depends.Add(14, new int[] { 13 });
            REFLECT_Functions.Add(15, new FuncInfo[] { new FuncInfo("DefineSportDealer", new string[] { "DEALER", "LINE_NO__REAL", "MATCH_NO__REAL", "MDEAL_NO", "FIRST_DEALER", GameSettings_3Sports_ZoneSwims.ToString() }) });
            REFLECT_Depends.Add(15, new int[] { -1 });

            //new for match:
            REFLECT_Functions.Add((int)CONST.MATCH_TOTAL_SCORE, new FuncInfo[] { new FuncInfo("SetMatchScore", new string[] { "MATCH_TOTAL_SCORE", "MATCH_NO", "14" }), new FuncInfo("TotalScore_MatchMode", new string[] { "MATCH_TOTAL_SCORE", "MATCH_NO__REAL" }) });
            REFLECT_Depends.Add((int)CONST.MATCH_TOTAL_SCORE, new int[] { 14 });
            REFLECT_InsideFunctionsList.Add("SetMatchScore");
            REFLECT_InsideFunctionsList.Add("TotalScore_MatchMode");

            NotNecessaryValues.Add(new int[] { 1, 13, 14 });
            ContractColumns.Add(new int[] { 6, 10 });
            NotNecessaryValues_NOCONTRACT.Add(new ArrayOfInt());
            NotNecessaryValues_NOCONTRACT.Add(new ArrayOfInt());
            NotNecessaryValues_NOCONTRACT[0].Add(new int[] { 5 });
            NotNecessaryValues_NOCONTRACT[1].Add(new int[] { 9 });

            columnsControlsClasses = new ArrayOfTypes();
            columnsControlsClasses.Add(typeof(DealInfoControl_split));
            columnsControlsClasses.Add(typeof(ShowTextControl_Center));
            columnsControlsClasses.Add(typeof(ShowTextControl_Center));
            columnsControlsClasses.Add(typeof(ShowTextControl));
            columnsControlsClasses.Add(typeof(SwitcherControl_Orange_Center));
            columnsControlsClasses.Add(typeof(ContractSelectControl));
            columnsControlsClasses.Add(typeof(ResultSelectControl));
            columnsControlsClasses.Add(typeof(ShowSimpleScore));
            columnsControlsClasses.Add(typeof(ShowSimpleScore));
            columnsControlsClasses.Add(typeof(ShowSimpleScore));

            arrHeadersNames = new ArrayOfString();
            arrHeadersNames.Add("#");
            arrHeadersNames.Add("Ст\nол");
            arrHeadersNames.Add("Сда\nющ.");
            arrHeadersNames.Add("Зона");
            arrHeadersNames.Add("Па\nра");
            arrHeadersNames.Add("Контракт");
            arrHeadersNames.Add("Рез-т");
            arrHeadersNames.Add("Очки");
            arrHeadersNames.Add("Раз-\nница");
            arrHeadersNames.Add("IMP");

            arrHeadersNames_FULL = new ArrayOfString();
            arrHeadersNames_FULL.Add("#");
            arrHeadersNames_FULL.Add("Стол");
            arrHeadersNames_FULL.Add("Сдающий");
            arrHeadersNames_FULL.Add("Зона");
            arrHeadersNames_FULL.Add("Пара");
            arrHeadersNames_FULL.Add("Контракт");
            arrHeadersNames_FULL.Add("Рез-т");
            arrHeadersNames_FULL.Add("Очки");
            arrHeadersNames_FULL.Add("Разница");
            arrHeadersNames_FULL.Add("IMP");

            // Какие контролы разделены?
            SUBDEALS_CONTROLS_ISSPLIT = new ArrayList();
            SUBDEALS_CONTROLS_ISSPLIT.Add(false);
            SUBDEALS_CONTROLS_ISSPLIT.Add(true);
            SUBDEALS_CONTROLS_ISSPLIT.Add(false);
            SUBDEALS_CONTROLS_ISSPLIT.Add(false);
            SUBDEALS_CONTROLS_ISSPLIT.Add(true);
            SUBDEALS_CONTROLS_ISSPLIT.Add(true);
            SUBDEALS_CONTROLS_ISSPLIT.Add(true);
            SUBDEALS_CONTROLS_ISSPLIT.Add(true);
            SUBDEALS_CONTROLS_ISSPLIT.Add(false);
            SUBDEALS_CONTROLS_ISSPLIT.Add(false);

            arrHeadersWidths = new ArrayOfInt();
            arrHeadersWidths.Add(new int[] { 22+4, 22, 33, 51+7, 26+7, 69+6, 45+6, 45+7, 45+7, 28+6 });

            CoVa_Dependences = new ArrayOfArrayOfInt();
            for (int i = 0; i < arrHeadersNames.Count; i++)
                CoVa_Dependences.Add(new ArrayOfInt());
            CoVa_Dependences[0].Add(new int[] { 0, 1 });
            CoVa_Dependences[2].Add(15);
            CoVa_Dependences[3].Add(4);
            CoVa_Dependences[8].Add(new int[] { 13 });  //относительно static
            CoVa_Dependences[9].Add(new int[] { 14 });  //относительно static

            CoVa_Dependences_SPLIT = new ArrayOfArrayOfInt();
            for (int i = 0; i < arrHeadersNames.Count; i++)
                CoVa_Dependences_SPLIT.Add(new ArrayOfInt());
            CoVa_Dependences_SPLIT[1].Add(new int[] { 2, 3 });
            CoVa_Dependences_SPLIT[4].Add(new int[] { 5, 9 });
            CoVa_Dependences_SPLIT[5].Add(new int[] { 6, 10 });
            CoVa_Dependences_SPLIT[6].Add(new int[] { 7, 11 });
            CoVa_Dependences_SPLIT[7].Add(new int[] { 8, 12 });   //относительно static (было 5,9,8,12)
        }


        bool hide_dealer_in_menu = false;
        void LOAD_GAME_SETTINGS___3_Compensat()
        {
            isRobber = false;
            isSplit = false;
            canbeNoContract = true;

            // ОПЦИИ
            GameSettings_Comp_Type = (BridgeGameScoring.TypeOfCompensation)(GameSettings_Options & 3); //2 bits
            GameSettings_Comp_10CardsIs2Fits = ((GameSettings_Options & 4) > 0); //3rd bit
            GameSettings_Comp_LessCompFor2Fits23PC = ((GameSettings_Options & 8) > 0); //4th bit

            // Скрыть ли сдающего в меню?
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
                hide_dealer_in_menu = true;
            else
                hide_dealer_in_menu = false;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! DB !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            DB_Deals_TableName = "Deals_Sport";
            DB_Deals_ColumnsNames.Add(1, "CardsDistribution");
            DB_Deals_ColumnsNames.Add(3, "Pair");
            DB_Deals_ColumnsNames.Add(4, "Contract");
            DB_Deals_ColumnsNames.Add(5, "Result");
            DB_Deals_ColumnsNames.Add(7, "Figures");
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                DB_Deals_ColumnsNames.Add(12, "Fits");
                DB_Deals_ColumnsNames.Add(13, "StrongestPair");
            }

            columnsDataClasses = new ArrayOfTypes();
            columnsDataClasses.Add(typeof(IntData));
            columnsDataClasses.Add(typeof(CardsDistribution));
            columnsDataClasses.Add(typeof(ZoneSwitcher));
            columnsDataClasses.Add(typeof(PairSwitcher));
            columnsDataClasses.Add(typeof(Contract));
            columnsDataClasses.Add(typeof(Result));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(IntData));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(QuarterSwitcher));
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                columnsDataClasses.Add(typeof(FitsSwitcher));
                columnsDataClasses.Add(typeof(BoolData));
            }

            ID_NAMES.Add(0, "NO");
            ID_NAMES.Add(1, "DISTRIBUTION");
            ID_NAMES.Add(2, "ZONE");
            ID_NAMES.Add(3, "PAIR");
            ID_NAMES.Add(4, "CONTRACT");
            ID_NAMES.Add(5, "RESULT");
            ID_NAMES.Add(6, "SCORE");
            ID_NAMES.Add(7, "FIG");
            ID_NAMES.Add(8, "COMPENS");
            ID_NAMES.Add(9, "DIFF");
            ID_NAMES.Add(10, "IMP");
            ID_NAMES.Add(11, "DEALER");
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                ID_NAMES.Add(12, "FITS");
                ID_NAMES.Add(13, "STRONGEST");
            }

            REFLECT_Functions.Add(3, new FuncInfo[] { new FuncInfo("NoPairIfNoContract", new string[] { "PAIR", "CONTRACT" }) });
            REFLECT_Depends.Add(3, new int[] { 3, 4 });
            REFLECT_Functions.Add(0, new FuncInfo[] { new FuncInfo("IntData_Equal", new string[] { "NO", "MDEAL_NO" }) });
            REFLECT_Constructors.Add(0, new string[] { "true" });
            REFLECT_Depends.Add(0, new int[] { -1 });
            REFLECT_Functions.Add(2, new FuncInfo[] { new FuncInfo("DefineSportZone", new string[] { "ZONE", "LINE_NO__REAL", "MATCH_NO__REAL", "MDEAL_NO", GameSettings_3Sports_ZoneSwims.ToString() }) });
            REFLECT_Depends.Add(2, new int[] { -1 });
            REFLECT_Functions.Add(5, new FuncInfo[] { new FuncInfo("CorrectResultAccordingToContract", new string[] { "RESULT", "CONTRACT", "0" }) });
            REFLECT_Depends.Add(5, new int[] { 4 });
            REFLECT_Constructors.Add(5, new string[] { "CONTRACT" });
            REFLECT_Functions.Add(6, new FuncInfo[] { new FuncInfo("SportScore", new string[] { "SCORE", "PAIR", "ZONE", "CONTRACT", "RESULT" }) });
            REFLECT_Depends.Add(6, new int[] { 2, 3, 4, 5 });

            //fig
            REFLECT_Functions.Add(7, new FuncInfo[] { new FuncInfo("FiguresPoints", new string[] { "FIG", "DISTRIBUTION", "TRUE", /*not use*/ "PAIR" }) });
            REFLECT_Depends.Add(7, new int[] { 7, 1 });
            REFLECT_Constructors.Add(7, new string[] { "true", "0", "true", "40" });

            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                // strongest
                REFLECT_Functions.Add(13, new FuncInfo[] { new FuncInfo("WhoIsStrongest20", new string[] { "STRONGEST", "FIG", "DISTRIBUTION", "LOADING_FROM_DB" }) });
                REFLECT_Depends.Add(13, new int[] { 1, 7 });

                //fits
                REFLECT_Functions.Add(12, new FuncInfo[] { new FuncInfo("FitsPoints", new string[] { "FITS", "DISTRIBUTION", (GameSettings_Comp_10CardsIs2Fits ? "TRUE" : "FALSE") }) });
                REFLECT_Depends.Add(12, new int[] { 12, 1 });
            }

            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                //comp with fits
                REFLECT_Functions.Add(8, new FuncInfo[] { new FuncInfo("GetCompensation_WithFits", new string[] { "COMPENS", "DISTRIBUTION", "FIG", "ZONE", "TRUE", /*not use*/"PAIR", /* ||||| */ "FITS", "STRONGEST", (GameSettings_Comp_10CardsIs2Fits ? "TRUE" : "FALSE"), (GameSettings_Comp_LessCompFor2Fits23PC ? "TRUE" : "FALSE") }) });
                REFLECT_Depends.Add(8, new int[] { 7, 2, 1, 12, 13 });
            }
            else
            {
                //comp
                REFLECT_Functions.Add(8, new FuncInfo[] { new FuncInfo("GetCompensation", new string[] { "COMPENS", "DISTRIBUTION", "FIG", "ZONE", "TRUE", /*not use*/"PAIR", ((int)GameSettings_Comp_Type).ToString() }) });
                REFLECT_Depends.Add(8, new int[] { 7, 2, 1 });
            }
            REFLECT_Functions.Add(9, new FuncInfo[] { new FuncInfo("ScoreSumm", new string[] { "DIFF", "SCORE", "COMPENS" }) });
            REFLECT_Depends.Add(9, new int[] { 6, 8 });
            REFLECT_Functions.Add(10, new FuncInfo[] { new FuncInfo("ConvertToIMPs", new string[] { "IMP", "DIFF" }) });
            REFLECT_Depends.Add(10, new int[] { 9 });
            REFLECT_Functions.Add(11, new FuncInfo[] { new FuncInfo("DefineSportDealer", new string[] { "DEALER", "LINE_NO__REAL", "MATCH_NO__REAL", "MDEAL_NO", "FIRST_DEALER", GameSettings_3Sports_ZoneSwims.ToString() }) });
            REFLECT_Depends.Add(11, new int[] { -1 });

            //new for match:
            REFLECT_Functions.Add((int)CONST.MATCH_TOTAL_SCORE, new FuncInfo[] { new FuncInfo("SetMatchScore", new string[] { "MATCH_TOTAL_SCORE", "MATCH_NO", "10" }), new FuncInfo("TotalScore_MatchMode", new string[] { "MATCH_TOTAL_SCORE", "MATCH_NO__REAL" }) });
            REFLECT_Depends.Add((int)CONST.MATCH_TOTAL_SCORE, new int[] { 10 });
            REFLECT_InsideFunctionsList.Add("SetMatchScore");
            REFLECT_InsideFunctionsList.Add("TotalScore_MatchMode");


            NotNecessaryValues.Add(new int[] { 1 });
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                NotNecessaryValues.Add(new int[] { 13 }); //!!!!!!!!!!! указать сильную сторону все-таки обязательно, если фигур=20 !!!!!!!!!!!!
            }
            ContractColumns.Add(new int[] { 4 });
            NotNecessaryValues_NOCONTRACT.Add(new ArrayOfInt());
            NotNecessaryValues_NOCONTRACT[0].Add(new int[] { 3 });

            columnsControlsClasses = new ArrayOfTypes();
            columnsControlsClasses.Add(typeof(DealInfoControl));
            if (!hide_dealer_in_menu)
            {
                columnsControlsClasses.Add(typeof(ShowTextControl_Center)); //1...
            }
            columnsControlsClasses.Add(typeof(ShowTextControl));
            columnsControlsClasses.Add(typeof(SwitcherControl_Orange));
            columnsControlsClasses.Add(typeof(ContractSelectControl));
            columnsControlsClasses.Add(typeof(ResultSelectControl));
            columnsControlsClasses.Add(typeof(ShowSimpleScore));
            columnsControlsClasses.Add(typeof(TextBoxInTable)); //6
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                columnsControlsClasses.Add(typeof(SwitcherControl_Orange)); //7
            }
            columnsControlsClasses.Add(typeof(ShowSimpleScore)); //7-8
            columnsControlsClasses.Add(typeof(ShowSimpleScore)); //8-9
            columnsControlsClasses.Add(typeof(ShowSimpleScore)); //9-10

            arrHeadersNames = new ArrayOfString();
            arrHeadersNames.Add("#");
            if (!hide_dealer_in_menu)
            {
                arrHeadersNames.Add("Сда\nющ.");
            }
            arrHeadersNames.Add("Зона");
            arrHeadersNames.Add("Па\nра");
            arrHeadersNames.Add("Контракт");
            arrHeadersNames.Add("Рез-т");
            arrHeadersNames.Add("Очки");
            arrHeadersNames.Add("Фи\nгур"); //6
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                arrHeadersNames.Add("Фи\nты"); //7
            }
            arrHeadersNames.Add("Комп."); //7-8
            arrHeadersNames.Add("Раз-\nница"); //8-9
            arrHeadersNames.Add("IMP"); //9-10

            arrHeadersNames_FULL = new ArrayOfString();
            arrHeadersNames_FULL.Add("#");
            if (!hide_dealer_in_menu)
            {
                arrHeadersNames_FULL.Add("Сдающий");
            }
            arrHeadersNames_FULL.Add("Зона");
            arrHeadersNames_FULL.Add("Пара");
            arrHeadersNames_FULL.Add("Контракт");
            arrHeadersNames_FULL.Add("Рез-т");
            arrHeadersNames_FULL.Add("Очки");
            arrHeadersNames_FULL.Add("Фигуры (NS)"); //6
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                arrHeadersNames_FULL.Add("Фиты"); //7
            }
            arrHeadersNames_FULL.Add("Компенсация"); //7-8
            arrHeadersNames_FULL.Add("Разница"); //8-9
            arrHeadersNames_FULL.Add("IMP"); //9-10

            arrHeadersWidths = new ArrayOfInt();
            arrHeadersWidths.Add(22);
            if (!hide_dealer_in_menu)
            {
                arrHeadersWidths.Add(33);
            }
            arrHeadersWidths.Add(new int[] { 51, 26, 69, 45, 45, 26 });
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
                arrHeadersWidths.Add(25);
            arrHeadersWidths.Add(new int[] { 45, 45, 28 });

            int hidden_dealer_offset = (!hide_dealer_in_menu) ? 1 : 0; //сдвиг индексов на 1 (после #), если сдающего все-таки показывать
            CoVa_Dependences = new ArrayOfArrayOfInt();
            for (int i = 0; i < arrHeadersNames.Count; i++)
                CoVa_Dependences.Add(new ArrayOfInt());
            if (hide_dealer_in_menu)
            {
                CoVa_Dependences[0].Add(new int[] { 0, 1, 11 });
            }
            else
            {
                CoVa_Dependences[0].Add(new int[] { 0, 1 });
                CoVa_Dependences[1].Add(11);
            }
            CoVa_Dependences[1 + hidden_dealer_offset].Add(2);
            CoVa_Dependences[2 + hidden_dealer_offset].Add(3);
            CoVa_Dependences[3 + hidden_dealer_offset].Add(4);
            CoVa_Dependences[4 + hidden_dealer_offset].Add(5);
            CoVa_Dependences[5 + hidden_dealer_offset].Add(new int[] { 6 });  //относительно static
            CoVa_Dependences[6 + hidden_dealer_offset].Add(7);
            if (GameSettings_Comp_Type == BridgeGameScoring.TypeOfCompensation.Europe)
            {
                CoVa_Dependences[7 + hidden_dealer_offset].Add(new int[] { 12 });  // относительно СИЛЬНОЙ пары, так что ПОХУЙ //
                CoVa_Dependences[8 + hidden_dealer_offset].Add(new int[] { 8 });  //относительно static
                CoVa_Dependences[9 + hidden_dealer_offset].Add(new int[] { 9 });  //относительно static
                CoVa_Dependences[10 + hidden_dealer_offset].Add(new int[] { 10 });  //относительно static
            }
            else
            {
                CoVa_Dependences[7 + hidden_dealer_offset].Add(new int[] { 8 });  //относительно static
                CoVa_Dependences[8 + hidden_dealer_offset].Add(new int[] { 9 });  //относительно static
                CoVa_Dependences[9 + hidden_dealer_offset].Add(new int[] { 10 });  //относительно static
            }
        }


        void LOAD_GAME_SETTINGS___4_SimpleIMP()
        {
            isRobber = false;
            isSplit = false;
            canbeNoContract = true;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! DB !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            DB_Deals_TableName = "Deals_Sport";
            DB_Deals_ColumnsNames.Add(1, "CardsDistribution");
            DB_Deals_ColumnsNames.Add(3, "Pair");
            DB_Deals_ColumnsNames.Add(4, "Contract");
            DB_Deals_ColumnsNames.Add(5, "Result");

            columnsDataClasses = new ArrayOfTypes();
            columnsDataClasses.Add(typeof(IntData));
            columnsDataClasses.Add(typeof(CardsDistribution));
            columnsDataClasses.Add(typeof(ZoneSwitcher));
            columnsDataClasses.Add(typeof(PairSwitcher));
            columnsDataClasses.Add(typeof(Contract));
            columnsDataClasses.Add(typeof(Result));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(SimpleScore));
            columnsDataClasses.Add(typeof(QuarterSwitcher));

            ID_NAMES.Add(0, "NO");
            ID_NAMES.Add(1, "DISTRIBUTION");
            ID_NAMES.Add(2, "ZONE");
            ID_NAMES.Add(3, "PAIR");
            ID_NAMES.Add(4, "CONTRACT");
            ID_NAMES.Add(5, "RESULT");
            ID_NAMES.Add(6, "SCORE");
            ID_NAMES.Add(7, "IMP");
            ID_NAMES.Add(8, "DEALER");

            REFLECT_Functions.Add(3, new FuncInfo[] { new FuncInfo("NoPairIfNoContract", new string[] { "PAIR", "CONTRACT" }) });
            REFLECT_Depends.Add(3, new int[] { 3, 4 });
            REFLECT_Functions.Add(0, new FuncInfo[] { new FuncInfo("IntData_Equal", new string[] { "NO", "MDEAL_NO" }) });
            REFLECT_Constructors.Add(0, new string[] { "true" });
            REFLECT_Depends.Add(0, new int[] { -1 });
            REFLECT_Functions.Add(2, new FuncInfo[] { new FuncInfo("DefineSportZone", new string[] { "ZONE", "LINE_NO__REAL", "MATCH_NO__REAL", "MDEAL_NO", GameSettings_3Sports_ZoneSwims.ToString() }) });
            REFLECT_Depends.Add(2, new int[] { -1 });
            REFLECT_Functions.Add(5, new FuncInfo[] { new FuncInfo("CorrectResultAccordingToContract", new string[] { "RESULT", "CONTRACT", "0" }) });
            REFLECT_Depends.Add(5, new int[] { 4 });
            REFLECT_Constructors.Add(5, new string[] { "CONTRACT" });
            REFLECT_Functions.Add(6, new FuncInfo[] { new FuncInfo("SportScore", new string[] { "SCORE", "PAIR", "ZONE", "CONTRACT", "RESULT" }) });
            REFLECT_Depends.Add(6, new int[] { 2, 3, 4, 5 });
            REFLECT_Functions.Add(7, new FuncInfo[] { new FuncInfo("ConvertToIMPs", new string[] { "IMP", "SCORE" }) });
            REFLECT_Depends.Add(7, new int[] { 6 });
            REFLECT_Functions.Add(8, new FuncInfo[] { new FuncInfo("DefineSportDealer", new string[] { "DEALER", "LINE_NO__REAL", "MATCH_NO__REAL", "MDEAL_NO", "FIRST_DEALER", GameSettings_3Sports_ZoneSwims.ToString() }) });
            REFLECT_Depends.Add(8, new int[] { -1 });
            //new for match:
            REFLECT_Functions.Add((int)CONST.MATCH_TOTAL_SCORE, new FuncInfo[] { new FuncInfo("SetMatchScore", new string[] { "MATCH_TOTAL_SCORE", "MATCH_NO", "7" }), new FuncInfo("TotalScore_MatchMode", new string[] { "MATCH_TOTAL_SCORE", "MATCH_NO__REAL" }) });
            REFLECT_Depends.Add((int)CONST.MATCH_TOTAL_SCORE, new int[] { 7 });
            REFLECT_InsideFunctionsList.Add("SetMatchScore");
            REFLECT_InsideFunctionsList.Add("TotalScore_MatchMode");

            NotNecessaryValues.Add(new int[] { 1 });
            ContractColumns.Add(new int[] { 4 });
            NotNecessaryValues_NOCONTRACT.Add(new ArrayOfInt());
            NotNecessaryValues_NOCONTRACT[0].Add(new int[] { 3 });


            columnsControlsClasses = new ArrayOfTypes();
            columnsControlsClasses.Add(typeof(DealInfoControl));
            columnsControlsClasses.Add(typeof(ShowTextControl_Center));
            columnsControlsClasses.Add(typeof(ShowTextControl));
            columnsControlsClasses.Add(typeof(SwitcherControl_Orange_Center));
            columnsControlsClasses.Add(typeof(ContractSelectControl));
            columnsControlsClasses.Add(typeof(ResultSelectControl));
            columnsControlsClasses.Add(typeof(ShowSimpleScore));
            columnsControlsClasses.Add(typeof(ShowSimpleScore));

            arrHeadersNames = new ArrayOfString();
            arrHeadersNames.Add("#");
            arrHeadersNames.Add("Сда-\nющий");
            arrHeadersNames.Add("Зона");
            arrHeadersNames.Add("Па\nра");
            arrHeadersNames.Add("Контракт");
            arrHeadersNames.Add("Рез-т");
            arrHeadersNames.Add("Очки");
            arrHeadersNames.Add("IMP");

            arrHeadersNames_FULL = new ArrayOfString();
            arrHeadersNames_FULL.Add("#");
            arrHeadersNames_FULL.Add("Сдающий");
            arrHeadersNames_FULL.Add("Зона");
            arrHeadersNames_FULL.Add("Пара");
            arrHeadersNames_FULL.Add("Контракт");
            arrHeadersNames_FULL.Add("Рез-т");
            arrHeadersNames_FULL.Add("Очки");
            arrHeadersNames_FULL.Add("IMP");

            arrHeadersWidths = new ArrayOfInt();
            arrHeadersWidths.Add(new int[] { 22+8, 42+3, 55+14, 28+14, 71+14, 47+14, 47+14, 30+14 });

            CoVa_Dependences = new ArrayOfArrayOfInt();
            for (int i = 0; i < arrHeadersNames.Count; i++)
                CoVa_Dependences.Add(new ArrayOfInt());
            CoVa_Dependences[0].Add(new int[] { 0, 1 });
            CoVa_Dependences[1].Add(8);
            CoVa_Dependences[2].Add(2);
            CoVa_Dependences[3].Add(3);
            CoVa_Dependences[4].Add(4);
            CoVa_Dependences[5].Add(5);
            CoVa_Dependences[6].Add(new int[] { 6 });  //относительно заданной пары
            CoVa_Dependences[7].Add(new int[] { 7 });  //относительно заданной пары
        }
        #endregion
        //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$


        // -------------------------------------------------- INITIALIZE --------------------------------------------------------

        Form m_form = null;
        public BridgeScoreTable(Form f, int GameId)
        {
            m_form = f;
            InitializeComponent();

            // Для нормального авто-скролла
            fakeControlForAutoScroll = new Control();
            fakeControlForAutoScroll.Size = new Size(0, 0);
            fakeControlForAutoScroll.Location = new Point(0, 0);
            this.Controls.Add(fakeControlForAutoScroll);

            // Создать селектор матчей:
            if (PAGE__VIEW)
            {
                PAGE__ComboBox = new ComboBox();
                PAGE__ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                PAGE__ComboBox.Location = new Point(4, 2);
                PAGE__ComboBox.Font = new Font("Tahoma", 6, FontStyle.Regular);
                PAGE__ComboBox.Size = new Size(250, 30);
                m_form.Controls.Add(PAGE__ComboBox);
                PAGE__ComboBox.SelectedIndexChanged += new EventHandler(PAGE_ComboBox_SelectedIndexChanged);
            }


            // Создать надпись total-score
            TS_SCore = new DealScore_CLASS();

            TS_Label = new ShowTextControl();
            TS_Label.Location = new Point(270, 2);
            TS_Label.Size = new Size(180, (PAGE__VIEW ? PAGE__ComboBox.Height : 30));
            TS_Label.SetTextFormat(StringAlignment.Center, StringAlignment.Center, 0, 0);
            TS_Label.SetTextFonts(new Font[] { new Font("Tahoma", 7, FontStyle.Bold) });
            //TS_Label.BackColor = Color.Aqua;
            TS_Label.AttachData(TS_SCore);
            m_form.Controls.Add(TS_Label);

         

            // Конструкторы для БД
            DB_Game_Id = GameId;
            DB_Matches_Ids = new ArrayOfInt();
            DB_Deals_Ids = new ArrayOfArrayOfInt();
            DB_Deals_ColumnsNames = new Dictionary<int, string>();


            // ************** DB - загрузить параметры, важно сначала это сделать ****************
            DB_LoadGameParameters();


            // Указать !НАШУ ПАРУ!, чтобы очки писались относительно нее (взыть из БД)
            ShowSimpleScore.RelativePair = Pairs.NS;
            BridgeGameScoring.RelativePair = Pairs.NS;

            // Конструкторы массивов для reflect:
            REFLECT_Functions = new Dictionary<int, FuncInfo[]>();
            REFLECT_Depends = new Dictionary<int, int[]>();
            REFLECT_Constructors = new Dictionary<int, string[]>();
            REFLECT_Recounts = new Dictionary<int, int[]>();
            REFLECT_InsideFunctionsList = new ArrayOfString();
            NotNecessaryValues = new ArrayOfInt();
            ContractColumns = new ArrayOfInt();
            NotNecessaryValues_NOCONTRACT = new ArrayOfArrayOfInt();

            // Общие ID для ф-ий reflect
            ID_NAMES = new Dictionary<int, string>();
            ID_NAMES.Add((int)CONST.LINE_NO, "LINE_NO");
            //for robber:
            ID_NAMES.Add((int)CONST.ROBBER_NO, "ROBBER_NO");
            ID_NAMES.Add((int)CONST.RDEAL_NO, "RDEAL_NO");
            ID_NAMES.Add((int)CONST.ROBBER, "ROBBER");
            ID_NAMES.Add((int)CONST.ROBBER_TOTAL_SCORE, "ROBBER_TOTAL_SCORE");
            ID_NAMES.Add((int)CONST.RDEAL_FULLSCORE, "RDEAL_FULLSCORE");
            ID_NAMES.Add((int)CONST.ROBBER_FIRSTLINE, "ROBBER_FIRSTLINE");
            //new for match:
            ID_NAMES.Add((int)CONST.MATCH_NO, "MATCH_NO");
            ID_NAMES.Add((int)CONST.MDEAL_NO, "MDEAL_NO");
            ID_NAMES.Add((int)CONST.MATCH_TOTAL_SCORE, "MATCH_TOTAL_SCORE");
            ID_NAMES.Add((int)CONST.MATCH_FIRSTLINE, "MATCH_FIRSTLINE");
            //first dealer:
            ID_NAMES.Add((int)CONST.FIRST_DEALER, "FIRST_DEALER");
            //loading:
            ID_NAMES.Add((int)CONST.LOADING_FROM_DB, "LOADING_FROM_DB");
            //for page mode:
            ID_NAMES.Add((int)CONST.LINE_NO__REAL, "LINE_NO__REAL");
            ID_NAMES.Add((int)CONST.ROBBER_NO__REAL, "ROBBER_NO__REAL");
            ID_NAMES.Add((int)CONST.MATCH_NO__REAL, "MATCH_NO__REAL");


            // Загрузка всех необходимых настроек для данного типа игры:
            switch (GameSettings_GameType)
            {
                case GameType.Robber:
                    LOAD_GAME_SETTINGS___1_Robber();
                    break;
                case GameType.Sport:
                    LOAD_GAME_SETTINGS___2_Sport();
                    break;
                case GameType.Compensat:
                    LOAD_GAME_SETTINGS___3_Compensat();
                    break;
                case GameType.SimpleIMP:
                    LOAD_GAME_SETTINGS___4_SimpleIMP();
                    break;
            }
            // В зависимости от того, может ли быть "нет контракта", спрятать или показать кнопку [НЕТ КОНТРАКТА] на селекторе
            ContractSelector.ExtendedWidth(canbeNoContract);


            // Координаты этой таблицы + контрол роббера:
            int page_combo_height = 0;
            if (PAGE__VIEW)
                page_combo_height = PAGE__ComboBox.Height + 3;

            if (isRobber)
            {
                CONTROL_ROBBER = new RobberControl();
                CONTROL_ROBBER.Location = new Point(2, 2 + page_combo_height); //промежуток должен быть
                CONTROL_ROBBER.Size = new Size(size_CONTROL_ROBBER.Width, m_form.Height - 2 * 2 - page_combo_height);
                m_form.Controls.Add(CONTROL_ROBBER);

                this.Location = new Point(2 + size_CONTROL_ROBBER.Width + 3, 2 + page_combo_height); //промежуток должен быть = 3
                this.Width = f.Width - 2 * 2 - (size_CONTROL_ROBBER.Width + 3);
            }
            else
            {
                this.Location = new Point(2, 2 + page_combo_height);
                this.Width = f.Width - 2 * 2;
            }
            this.Height = f.Height - 2 * 2 - page_combo_height;


            // Обработка передвижений оранжевого курсора по таблице
            this.ActiveElementChanged += OnActiveElementChanged;


            // Конструкторы данных и контролов:
            VALUES = new ArrayOfArrayOfObject();
            CONTROLS = new ArrayOfArrayOfControl();
            CONTROLS_COVERS = new ArrayOfArrayOfControl();
            if (isRobber) // для робберного бриджа:
            {
                // Создание массива робберов
                ROBBERS = new ArrayOfRobbers();
            }
            // Создание массива заголовков с totalscore-ми (вверху каждого роббера/матча) и контролов для них
            HeadlinesScores = new ArrayList();
            HeadlinesScores_Controls = new ArrayOfControl();
            HeadlinesScores_CoverControls = new ArrayOfControl();


            // ************** DB - построить таблицу ****************
            DB_Matches_TotalScores = new ArrayList();
            if (PAGE__VIEW)
                DB_LoadIDs_BuildTable_v3();
            else
                DB_LoadIDs_BuildTable_v2();
        }

        bool PAGE_ComboBox_manualselect = false;
        void PAGE_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!PAGE_ComboBox_manualselect)
            {
                DB_OpenPage(((match_in_combo)PAGE__ComboBox.Items[PAGE__ComboBox.SelectedIndex]).no, true);
            }
        }



        // --------------------------------------------------- DB ------------------------------------------------------
        // Настройки игры
        Quarters GameSettings_FirstDealer;
        byte GameSettings_DealsInMatch;
        GameType GameSettings_GameType;
        byte GameSettings_Options; //набор битов, считанных из БД - определяет настройки игры
        //для роббера
        bool GameSettings_Rob_BonusForOners;
        bool GameSettings_Rob_BonusForWholeRobber;
        //для 3 спортивных типов:
        bool GameSettings_3Sports_ZoneSwims;
        //для компенсаций
        BridgeGameScoring.TypeOfCompensation GameSettings_Comp_Type;
        bool GameSettings_Comp_10CardsIs2Fits;
        bool GameSettings_Comp_LessCompFor2Fits23PC;


        // Индексы игры-робберов(матчей)-раздач:
        int DB_Game_Id;
        ArrayOfInt DB_Matches_Ids;
        ArrayOfArrayOfInt DB_Deals_Ids;

        // Название таблицы сдач: Deals_Rob / Deals_Double / Deals_Sport
        string DB_Deals_TableName;

        // Названия полей, которые нужноо загрузить (и соотвествие индексам в VALUES[])
        Dictionary<int, string> DB_Deals_ColumnsNames;

        void DB_LoadGameParameters()
        {
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT * FROM Games WHERE id=" + DB_Game_Id + "";
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = DB.ExecuteReader(sqlQuery);
            sqlReader.Read();

            GameSettings_GameType = (GameType)sqlReader.GetByte(sqlReader.GetOrdinal("Type"));

            if (sqlReader.IsDBNull(sqlReader.GetOrdinal("FirstDealer")))
                GameSettings_FirstDealer = Quarters.NotDefinedYet;
            else
                GameSettings_FirstDealer = (Quarters)(sqlReader.GetByte(sqlReader.GetOrdinal("FirstDealer")));
            if (GameSettings_FirstDealer == Quarters.NotDefinedYet)
                GameSettings_FirstDealer = Quarters.N;

            if (sqlReader.IsDBNull(sqlReader.GetOrdinal("ZoneSwims")))
                GameSettings_3Sports_ZoneSwims = true;
            else
                GameSettings_3Sports_ZoneSwims = sqlReader.GetBoolean(sqlReader.GetOrdinal("ZoneSwims"));

            if (sqlReader.IsDBNull(sqlReader.GetOrdinal("GameOptions")))
                GameSettings_Options = 0;
            else
                GameSettings_Options = sqlReader.GetByte(sqlReader.GetOrdinal("GameOptions"));

            if (sqlReader.IsDBNull(sqlReader.GetOrdinal("DealsInMatch")))
                GameSettings_DealsInMatch = 0;
            else
                GameSettings_DealsInMatch = sqlReader.GetByte(sqlReader.GetOrdinal("DealsInMatch"));

            sqlReader.Close();
        }


//$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

        ArrayList DB_Matches_TotalScores; //список результтатов матчей

        bool loadingFromDB = false;
        void DB_LoadIDs_BuildTable_v2()
        {
            loadingFromDB = true;
            Cursor.Current = Cursors.WaitCursor;

            // Сколько было сдач до текущей страницы?
            PAGE__LINES_BEFORE = 0;

            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "SELECT Matches.id as mid, Matches.SCORE_NS, Matches.SCORE_EW, " + DB_Deals_TableName + ".* FROM Matches INNER JOIN " + DB_Deals_TableName + " ON (Matches.id = " + DB_Deals_TableName + ".fk_Match_id) WHERE (Matches.fk_Game_id=" + DB_Game_Id + ") ORDER BY Matches.id, " + DB_Deals_TableName + ".id";
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = DB.ExecuteReader(sqlQuery);
            int id_match, id_match_prev = -1, id_deal;
            bool first = true, id_deal_isnull;
            while(sqlReader.Read())
            {
                id_match = sqlReader.GetInt32(sqlReader.GetOrdinal("mid"));
                id_deal_isnull = sqlReader.IsDBNull(sqlReader.GetOrdinal("id"));
                if (id_deal_isnull)
                {
                    id_deal = -1;
                }
                else
                {
                    id_deal = sqlReader.GetInt32(sqlReader.GetOrdinal("id"));
                }

                if(first || id_match != id_match_prev)
                {
                    // Взять total-score из БД:
                    if (sqlReader.IsDBNull(sqlReader.GetOrdinal("SCORE_NS")) || sqlReader.IsDBNull(sqlReader.GetOrdinal("SCORE_EW")))
                    {
                        DB_Matches_TotalScores.Add(new DealScore(0, 0));
                    }
                    else
                    {
                        DB_Matches_TotalScores.Add(new DealScore(sqlReader.GetInt32(sqlReader.GetOrdinal("SCORE_NS")), sqlReader.GetInt32(sqlReader.GetOrdinal("SCORE_EW"))));
                    }

                    // ids:
                    DB_Matches_Ids.Add(id_match);
                    DB_Deals_Ids.Add(new ArrayOfInt());
                    AddNewMatch(true);
                }
                id_match_prev = id_match;
                if(first == true) first = false;

                if (!id_deal_isnull)
                {
                    DB_Deals_Ids[DB_Matches_Ids.Count - 1].Add(id_deal);
                    int newdeal = AddNewEmptyDeal(DB_Matches_Ids.Count - 1, 0, -666);
                    ActivateNewDeal(newdeal, 0, true, sqlReader);

                    if (isSplit)
                    {
                        bool isSubdeal2 = !sqlReader.IsDBNull(sqlReader.GetOrdinal("IsSecondStarted")) && sqlReader.GetBoolean(sqlReader.GetOrdinal("IsSecondStarted"));
                        if (isSubdeal2)
                        {
                            int newsubdeal = AddNewEmptyDeal(-666, 1, MATCHES_GetLine(DB_Matches_Ids.Count - 1, DB_Deals_Ids[DB_Matches_Ids.Count - 1].Count - 1));
                            ActivateNewDeal(newsubdeal, 1, true, sqlReader);
                        }
                    }
                }
            }
            sqlReader.Close();


            TS_SHOW();

            // Обновить таблицу
            AutoScroll_Optimize();
            this.Invalidate();

            // Обновить total-score из заголовков:
            DealScore ts_score, ts_score_OLD;
            for (int i = 0; i < HeadlinesScores.Count; i++)
            {
                ts_score = (HeadlinesScores[i] as IMatchGetScore).GetScore();
                ts_score_OLD = (DealScore) DB_Matches_TotalScores[i];

                if (!ts_score.Equals(ts_score_OLD))
                {
                    //MessageBox.Show("v2 [" + i + "]  " + ts_score_OLD + " => " + ts_score); //delme

                    DB_SaveMatchScore(i, ts_score);
                }
            }

            Cursor.Current = Cursors.Default;
            loadingFromDB = false;
        }


        void DB_LoadIDs_BuildTable_v3()
        {
            //loadingFromDB = true;
            //Cursor.Current = Cursors.WaitCursor;

            DB_OpenGame();

            if (DB_Matches_Ids_ALL.Count != 0)
            {
                DB_OpenPage(DB_Matches_Ids_ALL.Count - 1);
            }

            //Cursor.Current = Cursors.Default;
            //loadingFromDB = false;
        }

        //-----------------------------------------------------

        ArrayOfInt DB_Matches_Ids_ALL;
        // Загрузить список матчей игры
        void DB_OpenGame()
        {
            loadingFromDB = true;
            Cursor.Current = Cursors.WaitCursor;

            // Загрузить ids матчей в DB_Matches_Ids_ALL
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            
            //sql_optimize
            //sqlQuery.CommandText = "SELECT Matches.id as mid, Matches.SCORE_NS, Matches.SCORE_EW, COUNT(" + DB_Deals_TableName + ".id) FROM Matches INNER JOIN " + DB_Deals_TableName + " ON (Matches.id = " + DB_Deals_TableName + ".fk_Match_id) WHERE (Matches.fk_Game_id=" + DB_Game_Id + ") GROUP BY Matches.id, Matches.SCORE_NS, Matches.SCORE_EW ORDER BY mid";
            //sqlQuery.CommandText = "SELECT m.id as mid, Matches.SCORE_NS, Matches.SCORE_EW FROM (SELECT DISTINCT Matches.id FROM Matches INNER JOIN " + DB_Deals_TableName + " ON (Matches.id = " + DB_Deals_TableName + ".fk_Match_id) WHERE (Matches.fk_Game_id=" + DB_Game_Id + ")) m INNER JOIN Matches ON (m.id = Matches.id) ORDER BY mid";
            sqlQuery.CommandText = "SELECT id as mid, SCORE_NS, SCORE_EW FROM Matches m WHERE fk_Game_id = " + DB_Game_Id + " AND EXISTS (SELECT id FROM " + DB_Deals_TableName + " WHERE fk_Match_id = m.id) ORDER BY mid";
            
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = DB.ExecuteReader(sqlQuery);
            DB_Matches_Ids_ALL = new ArrayOfInt();
            while (sqlReader.Read())
            {
                // Взять total-score из БД:
                if (sqlReader.IsDBNull(sqlReader.GetOrdinal("SCORE_NS")) || sqlReader.IsDBNull(sqlReader.GetOrdinal("SCORE_EW")))
                {
                    DB_Matches_TotalScores.Add(new DealScore(0, 0));
                }
                else
                {
                    DB_Matches_TotalScores.Add(new DealScore(sqlReader.GetInt32(sqlReader.GetOrdinal("SCORE_NS")), sqlReader.GetInt32(sqlReader.GetOrdinal("SCORE_EW"))));
                }

                // ids:
                DB_Matches_Ids_ALL.Add(sqlReader.GetInt32(sqlReader.GetOrdinal("mid")));
            }
            sqlReader.Close();

            // Загрузить ids матчей в селектор
            match_in_combo m = new match_in_combo();
            m.isrob = isRobber;
            for (int i = 0; i < DB_Matches_Ids_ALL.Count; i++)
            {
                m.no = i;
                m.id = DB_Matches_Ids_ALL[i];
                m.score_NS = ((DealScore)DB_Matches_TotalScores[i]).NS;
                m.score_EW = ((DealScore)DB_Matches_TotalScores[i]).EW;
                PAGE__ComboBox.Items.Add(m);
            }

            TS_SHOW();

            Cursor.Current = Cursors.Default;
            loadingFromDB = false;
        }

        struct match_in_combo
        {
            public bool isrob;
            public int no;
            public int id;
            public int score_NS;
            public int score_EW;

            public override string ToString()
            {
                return (isrob ? "Роббер #" : "Матч #") + (no+1) + ( (score_NS == 0 && score_EW == 0) ? "" : (" (" + score_NS + " : " + score_EW + ")") );
            }
            
            // +++ счет +++
        };



        //--------------------------------------------------------


        // Окрыть страницу с нужным матчем
        void DB_OpenPage(int par_PageNo, bool fromSelector)
        {
            loadingFromDB = true;
            Cursor.Current = Cursors.WaitCursor;


            PAGE__NO = par_PageNo;
            if (!fromSelector)
            {
                PAGE_ComboBox_manualselect = true;
                PAGE__ComboBox.SelectedIndex = PAGE__NO;
                PAGE_ComboBox_manualselect = false;
            }
            System.Data.SqlServerCe.SqlCeDataReader sqlReader = null;

            // Сколько было сдач до текущей страницы?
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            //sql_optimize
            //sqlQuery.CommandText = "SELECT COUNT(*) FROM Matches INNER JOIN " + DB_Deals_TableName + " ON (Matches.id = " + DB_Deals_TableName + ".fk_Match_id) WHERE (Matches.fk_Game_id = " + DB_Game_Id + " AND Matches.id < " + DB_Matches_Ids_ALL[PAGE__NO] + ")";
            //int PAGE__LINES_BEFORE_v1 = (int)DB.ExecuteScalar(sqlQuery);
            sqlQuery.CommandText = "SELECT COUNT(*) FROM " + DB_Deals_TableName + " WHERE fk_Match_id IN (SELECT id FROM Matches WHERE fk_Game_id = " + DB_Game_Id + " AND id < " + DB_Matches_Ids_ALL[PAGE__NO] + ")";
            PAGE__LINES_BEFORE = (int)DB.ExecuteScalar(sqlQuery);

            // db ids
            DB_Matches_Ids.Clear();
            DB_Deals_Ids.Clear();

            DB_Matches_Ids.Add(DB_Matches_Ids_ALL[PAGE__NO]); //id матча на текущей (PAGE_NO) странице
            DB_Deals_Ids.Add(new ArrayOfInt());

            // >>> Сколько в матче сдач?
            sqlQuery.CommandText = "SELECT COUNT(*) FROM " + DB_Deals_TableName + " WHERE (fk_Match_id = " + DB_Matches_Ids[0] + ")";
            int new_vals_count = (int) DB.ExecuteScalar(sqlQuery);

            // >>> Сколько в каждой сдаче уровней?
            ArrayList arrSecondLevels = null;
            ArrayList arrSecondLevels_OLD = null;
            if (isSplit)
            {
                arrSecondLevels = new ArrayList();
                sqlQuery.CommandText = "SELECT IsSecondStarted FROM " + DB_Deals_TableName + " WHERE (fk_Match_id = " + DB_Matches_Ids[0] + ")";
                sqlReader = DB.ExecuteReader(sqlQuery);
                bool is_lev_2;
                while (sqlReader.Read())
                {
                    is_lev_2 = !sqlReader.IsDBNull(sqlReader.GetOrdinal("IsSecondStarted")) && sqlReader.GetBoolean(sqlReader.GetOrdinal("IsSecondStarted"));
                    arrSecondLevels.Add(is_lev_2);
                }
                sqlReader.Close();
            }



            // ======== ЗАГОЛОВОК ========
            // оставить только 0 заголовок (если его нет, то создать), значение удалить!
            if (HeadlinesScores.Count == 0)
            {
                HeadlinesScores_Controls.Add(new RobberScoreControl());
                HeadlinesScores_CoverControls.Add(new ControlCover());
                SetCoordinatesForControl(HeadlinesScores_Controls[0], 0);
                (HeadlinesScores_Controls[0] as RobberScoreControl).SetParentTable(this);
                (HeadlinesScores_CoverControls[0] as ControlCover).AttachControl(HeadlinesScores_Controls[0]);
                this.Controls.Add(HeadlinesScores_CoverControls[0]);
            }
            else
            {
                while (HeadlinesScores.Count > 1)
                {
                    DeleteHeadline(HeadlinesScores.Count - 1);
                }
                DeactivateHeadline(0);
                HeadlinesScores.Clear();
            }
            // создать значение и присоединить к контролу
            if (isRobber)
            {
                HeadlinesScores.Add(new RobberScore(PAGE__NO));
            }
            else
            {
                HeadlinesScores.Add(new MatchScore(PAGE__NO, GameSettings_DealsInMatch));
            }
            (HeadlinesScores_Controls[0] as RobberScoreControl).AttachData(HeadlinesScores[0] as BaseChangedData);
            // ======== ~ ЗАГОЛОВОК ~ ========


            // robbers[0] - если нет, создать
            if (isRobber)
            {
                if (ROBBERS.Count == 0)
                    ROBBERS.Add(new Robber());
            }

            // подогнать кол-во линий
            while(VALUES.Count > new_vals_count)
            {
                DeleteLine(VALUES.Count - 1);
            }
            while(VALUES.Count < new_vals_count)
            {
                AddNewEmptyDeal(0, 0, VALUES.Count);
            }

            // robbers[0] - удалить лишнее
            if (isRobber)
            {
                while (ROBBERS.Count > 1)
                    ROBBERS.RemoveAt(ROBBERS.Count - 1);
            }


            // уров.2
            if (isSplit)
            {
                arrSecondLevels_OLD = new ArrayList();
                for (int i = 0; i < VALUES.Count; i++)
                {
                    arrSecondLevels_OLD.Add((VALUES[i, SUBDEALS[1, 0]] == null) ? false : true);
                }
            }

            // подогнать уров.2
            if (isSplit)
            {
                for (int i = 0; i < VALUES.Count; i++)
                {
                    if ((bool)arrSecondLevels[i] != (bool)arrSecondLevels_OLD[i])
                    {
                        if ((bool)arrSecondLevels[i] == true)
                        {
                            AddNewEmptyDeal(0, 1, i);
                        }
                        else
                        {
                            DeleteSubLine(i, 1);
                        }
                    }
                }
            }

            
            // Обновить таблицу
            AutoScroll_Optimize();
            this.Invalidate();


            // деактивировать линии и уров.2
            for(int i = 0 ; i < new_vals_count ; i++)
            {
                DeactivateDeal(i, 0);
                if (isSplit && (bool)arrSecondLevels[i])
                {
                    DeactivateDeal(i, 1);
                }
            }

            //заблокировать линии и уров.2
            LockAllLines();


            // загрузить линии из БД
            sqlQuery.CommandText = "SELECT * FROM " + DB_Deals_TableName + " WHERE (" + DB_Deals_TableName + ".fk_Match_id = " + DB_Matches_Ids[0] + ") ORDER BY id";
            sqlReader = DB.ExecuteReader(sqlQuery);
            int no_deal = 0;
            while (sqlReader.Read())
            {
                int id_deal = sqlReader.GetInt32(sqlReader.GetOrdinal("id"));
                DB_Deals_Ids[0].Add(id_deal);
                ActivateNewDeal(no_deal, 0, true, sqlReader);

                if (isSplit && (bool)arrSecondLevels[no_deal])
                {
                    ActivateNewDeal(no_deal, 1, true, sqlReader);
                }

                no_deal++;
            }
            sqlReader.Close();



            // Обновить total-score из заголовка [0]:
            DealScore ts_score, ts_score_OLD;
            ts_score = (HeadlinesScores[0] as IMatchGetScore).GetScore();
            ts_score_OLD = (DealScore)DB_Matches_TotalScores[PAGE__NO];

            if (!ts_score.Equals(ts_score_OLD))
            {
                //MessageBox.Show("v3 [" + PAGE__NO + "]  " + ts_score_OLD + " => " + ts_score); //delme

                DB_SaveMatchScore(PAGE__NO, ts_score);
            }
            

            // Фокус на заголовке
            FocusOnHeader(0);
            if (isRobber)
            {
                ShowRobber(0);
            }

            // Отменить последнюю нельзя
            CUR_line = -1;
            CUR_level = 0;

            Cursor.Current = Cursors.Default;
            loadingFromDB = false;
        }

        void OnLockingLine(object sender, DealInfoControl.LockLineEventArgs e)
        {
            int lineno = -1;
            for (int i = 0; i < CONTROLS.Count; i++)
            {
                if (CONTROLS[i, 0] == sender)
                {
                    lineno = i;
                    break;
                }
            }

            if (lineno != -1)
            {
                LockLine(lineno, e.level, e.locking);
            }
        }

        void LockLine(int line, int level, bool locking)
        {
            // 1. Изменить отображение (и меню) DealInfoControl
            (CONTROLS[line, 0] as DealInfoControl).SetLockView(level, locking);


            // 2. Заблокировать/разблокировать контролы на линии (и уровне)
            int startControlIndex, endControlIndex;
            if (level == 0)
            {
                startControlIndex = 1;
                endControlIndex = columnsControlsClasses.Count - 1;
            }
            else
            {
                startControlIndex = columnsControlsClasses.Count + (level - 1) * SUBDEALS_CONTROLS_GetSpliColumnsCount();
                endControlIndex = startControlIndex + SUBDEALS_CONTROLS_GetSpliColumnsCount() - 1;
            }

            for (int i = startControlIndex; i <= endControlIndex; i++)
            {
                (CONTROLS[line, i] as ILock).Lock = locking;
            }
        }

        void LockAllLines()
        {
            for (int i = 0; i < CONTROLS.Count; i++)
            {
                LockLine(i, 0, true);

                if (isSplit)
                {
                    // VALUES[i, SUBDEALS[1, 0]] != null  ... '1' is sublevel

                    if( CONTROLS[i].Count > columnsControlsClasses.Count )
                        LockLine(i, 1, true);
                }
            }
        }

        void DB_OpenPage(int par_PageNo)
        {
            DB_OpenPage(par_PageNo, false);
        }

        void DB_SaveMatchScore(int matchNo_real, DealScore ds)
        {
            // Узнать id матча в БД
            int matchId = (PAGE__VIEW ? DB_Matches_Ids_ALL[matchNo_real] : DB_Matches_Ids[matchNo_real]);

            // Сохранить счет матча в БД
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "UPDATE Matches SET SCORE_NS='" + ds.NS + "', SCORE_EW='" + ds.EW + "' WHERE id=" + matchId + "";

            while (DB.ExecuteNonQuery(sqlQuery, true) == 0)
            {
                if (MessageBox.Show("Счет матча не был сохранен!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                {
                    DB.sqlConnection.Close();
                    DB.sqlConnection.Open();
                }
                else
                {
                    break;
                }
            }

            // Сохранить счет матча в []
            DB_Matches_TotalScores[matchNo_real] = ds;

            // обновить селектор матчей
            TS_SHOW_UPDATE(matchNo_real);
        }

//$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

        // Показать total-score на лэйбле над этой таблицей
        void TS_SHOW()
        {
            // обновить только текст total-score
            int ns = 0, ew = 0;
            for(int i = 0 ; i < DB_Matches_TotalScores.Count ; i++)
            {
                ns += ((DealScore)DB_Matches_TotalScores[i]).NS;
                ew += ((DealScore)DB_Matches_TotalScores[i]).EW;
            }
            //TS_Label.Text = ns + " : " + ew;
            //TS_SCore.NS = ns;
            //TS_SCore.EW = ew;
            TS_SCore.SetScore(ns, ew);
            //TS_Label.OnValueChanged(null, null);
            //TS_Label.AttachData(new DealScore(0, 0));
        }

        // обновить селектор матчей
        void TS_SHOW_UPDATE(int matchNo_real)
        {
            if(PAGE__VIEW)
            {
                PAGE_ComboBox_manualselect = true;

                match_in_combo item = (match_in_combo) PAGE__ComboBox.Items[matchNo_real];
                item.score_NS = ((DealScore)DB_Matches_TotalScores[matchNo_real]).NS;
                item.score_EW = ((DealScore)DB_Matches_TotalScores[matchNo_real]).EW;
                PAGE__ComboBox.Items[matchNo_real] = item;
                PAGE__ComboBox.SelectedIndex = matchNo_real;

                PAGE_ComboBox_manualselect = false;
            }

            TS_SHOW();
        }

//$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$


        void DB_Save(int line, int pos)
        {
            if(DB_Deals_ColumnsNames.Keys.Contains(pos))
            {
                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                int id = (isRobber ? DB_Deals_Ids[ROBBERS.GetRobberNo(line), ROBBERS.GetRobDealNo(line)] : DB_Deals_Ids[MATCHES_GetMatchNo(line), MATCHES_GetMDealNo(line)]);
                sqlQuery.CommandText = "UPDATE " + DB_Deals_TableName + " SET " + DB_Deals_ColumnsNames[pos] + "=@value WHERE id=" + id + "";
                sqlQuery.Parameters.Add("value", (object)((VALUES[line, pos] as ISQLSerialize)._ToDataBase()));
                if (((VALUES[line, pos] as ISQLSerialize).GetType() == typeof(CardsDistribution)))
                {
                    sqlQuery.Parameters[0].SqlDbType = SqlDbType.Binary;
                    sqlQuery.Parameters[0].Size = 20;
                }
                sqlQuery.Prepare();

                while (DB.ExecuteNonQuery(sqlQuery, true) == 0)
                {
                    if (MessageBox.Show("Ячейка " + DB_Deals_ColumnsNames[pos] + " в сдаче " + id + " не была обновлена!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    {
                        DB.sqlConnection.Close();
                        DB.sqlConnection.Open();
                    }
                    else
                    {
                        return;
                    }
                }

                /*
                 * проверить select-ом, все ли записалось
                 * 
                    sqlQuery.CommandText = "SELECT " + DB_Deals_ColumnsNames[pos] + " FROM " + DB_Deals_TableName + " WHERE id=" + id;
                    object o = DB.ExecuteScalar(sqlQuery);
                    if (o == null || o == DBNull.Value)
                        MessageBox.Show(DB_Deals_ColumnsNames[pos] + " is NULL !!!!!!!");
                    //else
                        //MessageBox.Show("Ячейка " + DB_Deals_ColumnsNames[pos] + " в сдаче " + id + " успешно обновлена", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                */
            }
        }

        void OnInputChanged_SaveToDataBase(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            for (int i = 0; i < VALUES.Count; i++)
            {
                if (VALUES[i].Contains(sender))
                {
                    //MessageBox.Show("before save " + sender.GetType().ToString()); //анука удолил
                    DB_Save(i, VALUES[i].IndexOf(sender));
                    //MessageBox.Show("after save " + sender.GetType().ToString()); //анука удолил
                    return;
                }
            }
        }
        // ------------------------------------------------- ~ DB ~ -----------------------------------------------------

        #region stuff (все правильно)
        int MATCHES_GetMatchNo(int line)
        {
            return (line / GameSettings_DealsInMatch);
        }

        int MATCHES_GetMDealNo(int line)
        {
            return (line % GameSettings_DealsInMatch);
        }

        int MATCHES_GetLine(int MatchNo, int MDealNo)
        {
            return (MatchNo * GameSettings_DealsInMatch + MDealNo);
        }

        int MATCHES_GetFirstLine(int MatchNo)
        {
            return (MatchNo * GameSettings_DealsInMatch);
        }

        int MATCHES_GetDealsCount(int MatchNo)
        {
            if((MatchNo * GameSettings_DealsInMatch + GameSettings_DealsInMatch) <= VALUES.Count)
            {
                return GameSettings_DealsInMatch;
            }
            else
            {
                return (GameSettings_DealsInMatch + VALUES.Count - (MatchNo * GameSettings_DealsInMatch + GameSettings_DealsInMatch));
            }
            //    012 345 67   3+8-9 = 2
            //    012 345 6    3+7-9 = 1
            //    012 345      3+6-9 = 0
        }

        int MATCHES_GetCount()
        {
            return ((VALUES.Count / GameSettings_DealsInMatch) + (VALUES.Count % GameSettings_DealsInMatch > 0 ? 1 : 0));
        }

        bool MATCHES_NeedToStartNew()
        {
            return (VALUES.Count % GameSettings_DealsInMatch == 0); //!!!!!!!! если первая сдача удалена, но остался заголовок, то не надо начанать новый матч???
        }

        int GetComfortWidth()
        {
            int offset4marker = (offset < 0 ? 1 : 0);

            int totalWidth = 0;
            for (int i = 0; i < arrHeadersWidths.Count; i++)
            {
                totalWidth += arrHeadersWidths[i];
                if (i > 0)
                    totalWidth += offset;
            }
            totalWidth += 2 * (offset + 1);
            totalWidth += 2 * offset4marker;
            return totalWidth;
        }

        int GetComfortHeight()
        {
            int elementMultiHeight = elementHeight; //мульти-высота
            if (this.isSplit)
            {
                elementMultiHeight += (offset_sub + elementHeight) * (SUBDEALS.Count - 1);
            }

            int offset4marker = (offset < 0 ? 1 : 0);

            int totalHeight = 0;
            totalHeight += headerHeight;
            if (this.isRobber)
            {
                for (int i = 0; i < ROBBERS.Count; i++)
                {
                    totalHeight += 1 * (offset + headlinescoreHeight) + ROBBERS[i].ScoreArray.Count * (offset + elementMultiHeight);
                }
            }
            else
            {
                totalHeight += (offset + headlinescoreHeight) * MATCHES_GetCount();
                totalHeight += (offset + elementMultiHeight) * VALUES.Count;
            }
            totalHeight += 2 * (offset + 1);
            totalHeight += 2 * offset4marker;
            return totalHeight;
        }


        void OnInputChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            for (int i = 0; i < VALUES.Count; i++)
            {
                if (VALUES[i].Contains(sender))
                {
                    //MessageBox.Show("before reflect " + sender.GetType().ToString()); //анука удолил
                    REFLECT_DEPENDENCES(i, VALUES[i].IndexOf(sender));
                    //MessageBox.Show("after reflect " + sender.GetType().ToString()); //анука удолил
                    return;
                }
            }
        }

        void OnActiveElementChanged(object sender, BaseTable.ChangedEventsArgs e)
        {
            Control c = (Control)e._new;

            // Для робберного бриджа:
            if (this.isRobber)
            {
                // Определить № роббера:
                int robNo = -1;
                if (HeadlinesScores_Controls.Contains(c))
                {
                    robNo = HeadlinesScores_Controls.IndexOf(c);
                }
                else
                {
                    for (int i = 0; i < CONTROLS.Count; i++)
                    {
                        if (CONTROLS[i].Contains(c))
                        {
                            robNo = ROBBERS.GetRobberNo(i);
                            break;
                        }
                    }
                }

                // Показать роббер:
                if (robNo == -1)
                    CONTROL_ROBBER.DetachData();
                else
                    CONTROL_ROBBER.AttachData(ROBBERS[robNo]);
            }
        }
        #endregion

        // ------------------------------------------------------------- Управление сдачами: add/remove/move ---------------------------------------------------------------
        #region Управление сдачами: add/remove/move

        public bool CanUndoLastDeal()
        {
            return (CUR_line != -1);
        }

        public void UndoLastDeal()
        {
            if (CUR_line != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                // Step 1
                if (isSplit && CUR_level > 0)
                {
                    if (DB_DeleteSubLine(CUR_line, CUR_level) == false)
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }
                    DeleteSubLine(CUR_line, CUR_level);
                }
                else
                {
                    if (DB_DeleteLine(CUR_line) == false)
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }
                    DeleteLine(CUR_line);
                }

                // Step 2
                if (CONTROLS.Count == 0)
                {
                    // Если матч стал пустым - удалить его
                    System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                    sqlQuery.CommandText = "DELETE FROM Matches WHERE (fk_Game_id = " + DB_Game_Id + " AND id = " + DB_Matches_Ids_ALL[PAGE__NO] + ")";
                    DB.ExecuteNonQuery(sqlQuery, true); //ладно уже, без проверки на rows_affected == 0

                    DB_Matches_TotalScores.RemoveAt(PAGE__NO);
                    TS_SHOW();

                    DB_Matches_Ids_ALL.RemoveAt(PAGE__NO);
                    PAGE__ComboBox.Items.RemoveAt(PAGE__NO);

                    PAGE__NO = -1;

                    // Открыть последнюю страницу
                    if (DB_Matches_Ids_ALL.Count > 0)
                    {
                        DB_OpenPage(DB_Matches_Ids_ALL.Count - 1);
                    }
                    else
                    {
                        // Если эта была последняя, то очистить:
                        while (HeadlinesScores.Count > 0)
                        {
                            DeleteHeadline(HeadlinesScores.Count - 1);
                        }

                        if (isRobber)
                        {
                            ShowRobber(-1);
                            ROBBERS.Clear();
                        }

                        // Обновить таблицу
                        AutoScroll_Optimize();
                        this.Invalidate();
                    }
                }
                else
                {
                    // Подправить total-score
                    if (isRobber)
                    {
                        REFLECT_CALL_FUNCTION(0, (int)CONST.ROBBER);
                        REFLECT_CALL_FUNCTION(0, (int)CONST.ROBBER_TOTAL_SCORE);
                    }
                    else
                    {
                        REFLECT_CALL_FUNCTION(0, (int)CONST.MATCH_TOTAL_SCORE);
                    }

                    // Обновить таблицу
                    if (!isSplit || isSplit && CUR_level == 0)
                    {
                        AutoScroll_Optimize();
                        this.Invalidate();
                    }
                }

                Cursor.Current = Cursors.Default;
            }


            // Теперь нет текущей
            CUR_line = -1;
            CUR_level = 0;
        }



        void DeleteHeadline(int line)
        {
            // Удалить значение
            (HeadlinesScores[line] as BaseChangedData).RemoveChangedHandlers();
            HeadlinesScores[line] = null;
            HeadlinesScores.RemoveAt(line);

            // Удалить контрол
            if (HeadlinesScores_Controls[line].GetType().GetInterfaces().Contains(typeof(IDetachData)))
                (HeadlinesScores_Controls[line] as IDetachData).DetachData(false);
            if (HeadlinesScores_Controls[line].GetType().GetInterfaces().Contains(typeof(IControlInTable)))
            {
                (HeadlinesScores_Controls[line] as IControlInTable).UnsetParentTable();
                (HeadlinesScores_Controls[line] as IControlInTable).RemoveGotActiveHandlers();
                (HeadlinesScores_Controls[line] as IControlInTable).RemoveLostActiveHandlers();
            }
            (HeadlinesScores_CoverControls[line] as ControlCover).DetachControl();
            HeadlinesScores_Controls[line].Dispose();
            HeadlinesScores_Controls[line] = null;
            this.Controls.Remove(HeadlinesScores_CoverControls[line]);
            HeadlinesScores_CoverControls[line].Dispose();
            HeadlinesScores_CoverControls[line] = null;
            HeadlinesScores_CoverControls.RemoveAt(line);
            HeadlinesScores_Controls.RemoveAt(line);
        }

        void DeactivateHeadline(int line)
        {
            (HeadlinesScores[line] as BaseChangedData).RemoveChangedHandlers();
            (HeadlinesScores[line] as BaseChangedData).Clear();
            if (HeadlinesScores_Controls[line].GetType().GetInterfaces().Contains(typeof(IDetachData)))
                (HeadlinesScores_Controls[line] as IDetachData).DetachData(true);
        }




        // Передвинуть те элементы, что ниже данной линии:
        void ReMoveUnderLine(int line)
        {
            // Переместить обычные элементы
            for (int i = line + 1; i < CONTROLS.Count; i++)
            {
                for (int j = 0; j < CONTROLS[i].Count; j++)
                {
                    if (CONTROLS_COVERS[i, j] != null)
                        MoveControlCover(CONTROLS_COVERS[i, j] as ControlCover, i, j);
                }
            }

            // Переместить заголовки
            if (this.isRobber)
            {
                for (int i = ROBBERS.GetRobberNo(line) + 1; i < ROBBERS.Count; i++)
                {
                    MoveControlCover(HeadlinesScores_CoverControls[i] as ControlCover, i);
                }
            }
            else
            {
                for(int i = MATCHES_GetMatchNo(line) + 1 ; i < MATCHES_GetCount() ; i++)
                {
                    MoveControlCover(HeadlinesScores_CoverControls[i] as ControlCover, i);
                }
            }
        }




        bool DB_DeleteLine(int line)
        {
            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            int id = (isRobber ? DB_Deals_Ids[ROBBERS.GetRobberNo(line), ROBBERS.GetRobDealNo(line)] : DB_Deals_Ids[MATCHES_GetMatchNo(line), MATCHES_GetMDealNo(line)]);
            sqlQuery.CommandText = "DELETE FROM " + DB_Deals_TableName + " WHERE id=" + id;

            while (DB.ExecuteNonQuery(sqlQuery, true) == 0)
            {
                if (MessageBox.Show("Сдача " + id + " не была удалена!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                {
                    DB.sqlConnection.Close();
                    DB.sqlConnection.Open();
                }
                else
                {
                    return false;
                }
            }

            // Удалить id сдачи из []
            if (isRobber)
                DB_Deals_Ids[ROBBERS.GetRobberNo(line)].RemoveAt(ROBBERS.GetRobDealNo(line));
            else
                DB_Deals_Ids[MATCHES_GetMatchNo(line)].RemoveAt(MATCHES_GetMDealNo(line));

            return true;
        }

        bool DB_DeleteSubLine(int line, int level)
        {
            if(!isSplit)
                return false;
            if(level == 0)
                return false;  //нельзя удалить 0 уровень, а 1 оставить!!! 

            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            int id = DB_Deals_Ids[MATCHES_GetMatchNo(line), MATCHES_GetMDealNo(line)];
            sqlQuery.CommandText = "UPDATE " + DB_Deals_TableName + " SET IsSecondStarted='FALSE'";
            for (int i = 0; i < DB_Deals_ColumnsNames.Keys.Count; i++)
            {
                int colPos = DB_Deals_ColumnsNames.Keys.ElementAt(i); //VALUES[newline, colPos]
                if (SUBDEALS_WhatLevel(colPos) == level)
                {
                    sqlQuery.CommandText += ", " + DB_Deals_ColumnsNames[colPos] + "=NULL";
                }
            } 
            sqlQuery.CommandText += " WHERE id=" + id;

            while (DB.ExecuteNonQuery(sqlQuery, true) == 0)
            {
                if (MessageBox.Show("Сдача " + id + " за 2 столом не была удалена!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                {
                    DB.sqlConnection.Close();
                    DB.sqlConnection.Open();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        void DeleteLine(int line)
        {
            /*
            // Удалить id сдачи из []
            if (isRobber)
                DB_Deals_Ids[ROBBERS.GetRobberNo(line)].RemoveAt(ROBBERS.GetRobDealNo(line));
            else
                DB_Deals_Ids[MATCHES_GetMatchNo(line)].RemoveAt(MATCHES_GetMDealNo(line));
            */


            // Удаление записи в роббере
            if (isRobber)
            {
                ROBBERS[ROBBERS.GetRobberNo(line)].ScoreArray.RemoveAt(ROBBERS.GetRobDealNo(line));
            }

            // Удаление значений
            for (int i = 0; i < VALUES[line].Count; i++)
            {
                if (VALUES[line, i] == null)
                    continue;

                (VALUES[line, i] as BaseChangedData).RemoveChangedHandlers();
                VALUES[line, i] = null;
            }
            VALUES[line].Clear();
            VALUES.RemoveAt(line);

            // Удаление контролов
            if (CONTROLS[line, 0] != null)
                (CONTROLS[line, 0] as DealInfoControl).RemoveLockingLineEvent();
            for (int i = 0; i < CONTROLS[line].Count; i++)
            {
                if (CONTROLS[line, i] == null)
                    continue;

                if (CONTROLS[line, i].GetType().GetInterfaces().Contains(typeof(IDetachData)))
                    (CONTROLS[line, i] as IDetachData).DetachData(false);
                if (CONTROLS[line, i].GetType().IsSubclassOf(typeof(BaseSelectControl)))
                    (CONTROLS[line, i] as BaseSelectControl).DetachStuffForSelector();
                if (CONTROLS[line, i].GetType().GetInterfaces().Contains(typeof(IControlInTable)))
                {
                    (CONTROLS[line, i] as IControlInTable).UnsetParentTable();
                    (CONTROLS[line, i] as IControlInTable).RemoveGotActiveHandlers();
                    (CONTROLS[line, i] as IControlInTable).RemoveLostActiveHandlers();
                }
                (CONTROLS_COVERS[line, i] as ControlCover).DetachControl();
                CONTROLS[line, i].Dispose();
                CONTROLS[line, i] = null;
                this.Controls.Remove(CONTROLS_COVERS[line, i]);
                CONTROLS_COVERS[line, i].Dispose();
                CONTROLS_COVERS[line, i] = null;
            }
            CONTROLS[line].Clear();
            CONTROLS.RemoveAt(line);
            CONTROLS_COVERS[line].Clear();
            CONTROLS_COVERS.RemoveAt(line);
        }

        void DeleteSubLine(int line, int level)
        {
            if (!this.isSplit)
                return;

            if (level == 0)
            {
                // нельзя удалить 0 уровень, а 1 оставить!!! 
                return;
            }

            // Удаление данных 2 уровня:
            for (int i = 0; i < columnsDataClasses.Count; i++)
            {
                if (SUBDEALS_WhatLevel(i) == level)
                {
                    //ok
                }
                else
                    continue;

                if (VALUES[line, i] == null)
                    continue;

                (VALUES[line, i] as BaseChangedData).RemoveChangedHandlers();
                VALUES[line, i] = null;
            }

            // Удаление контролов 2 уровня:
            int startControlIndex = columnsControlsClasses.Count + (level - 1) * SUBDEALS_CONTROLS_GetSpliColumnsCount();
            int endControlIndex = startControlIndex + SUBDEALS_CONTROLS_GetSpliColumnsCount() - 1;
            for (int i = startControlIndex; i <= endControlIndex; i++)
            {
                if (CONTROLS[line, i] == null)
                    continue;

                if (CONTROLS[line, i].GetType().GetInterfaces().Contains(typeof(IDetachData)))
                    (CONTROLS[line, i] as IDetachData).DetachData(false);
                if (CONTROLS[line, i].GetType().IsSubclassOf(typeof(BaseSelectControl)))
                    (CONTROLS[line, i] as BaseSelectControl).DetachStuffForSelector();
                if (CONTROLS[line, i].GetType().GetInterfaces().Contains(typeof(IControlInTable)))
                {
                    (CONTROLS[line, i] as IControlInTable).UnsetParentTable();
                    (CONTROLS[line, i] as IControlInTable).RemoveGotActiveHandlers();
                    (CONTROLS[line, i] as IControlInTable).RemoveLostActiveHandlers();
                }
                (CONTROLS_COVERS[line, i] as ControlCover).DetachControl();
                CONTROLS[line, i].Dispose();
                CONTROLS[line, i] = null;
                this.Controls.Remove(CONTROLS_COVERS[line, i]);
                CONTROLS_COVERS[line, i].Dispose();
                CONTROLS_COVERS[line, i] = null;
            }

            for (int i = endControlIndex; i >= startControlIndex; i--)
            {
                CONTROLS[line].RemoveAt(i);
                CONTROLS_COVERS[line].RemoveAt(i);
            }

            // Убрать возможность блокироки 2 ур.
            if (this.isSplit && level > 0)
            {
                (CONTROLS[line, 0] as DealInfoControl).SetSub2(false);
            }
        }





        void ActivateNewDeal(int newline, int level, bool loadFromDB, SqlCeDataReader sqlReader)
        {
            // ----------- 1. Создание сдачи в БД / загрузка сдачи из БД  +  REFLECT --------------
            
            if (!loadFromDB)
            {
                // !!! Выполнить инициализацию данных, помеченных в REFLECT_Depends со значением {-1}
                // !!! Обычно это зона и порядковые номера
                // !!! При sub-deal для 1 стола -1, для 2 стола -2 и т.д.
                REFLECT_DEPENDENCES(newline, (this.isSplit ? -(level + 1) : -1));
            }
            else
            {
                ArrayOfInt poss = new ArrayOfInt(); //колонки, в которые будут загружены данные; список всегда начинается с -1 (-2)
                poss.Add((this.isSplit ? -(level + 1) : -1));
                for (int i = 0; i < DB_Deals_ColumnsNames.Keys.Count; i++)
                {
                    int colPos = DB_Deals_ColumnsNames.Keys.ElementAt(i); //VALUES[newline, colPos]

                    if (this.isSplit)
                    {
                        if (SUBDEALS_WhatLevel(colPos) == level || SUBDEALS_WhatLevel(colPos) == -1 && level == 0)
                        {
                            //ok
                        }
                        else
                            continue;
                    }

                    // *** Загрузить из БД *** 
                    poss.Add(colPos);
                    object val = sqlReader.GetValue(sqlReader.GetOrdinal(DB_Deals_ColumnsNames[colPos]));
                    (VALUES[newline, colPos] as ISQLSerialize)._FromDataBase(val);
                }

                // Одним махом REFLECT для poss[] - т.е. для начала (-1, -2 для subdeal) и для загруженных данных
                REFLECT_DEPENDENCES(newline, poss);
            }


            // -------------- 2. Активация изменений данных: Changed -> SAVE, REFLECT  ----------------
            for (int i = 0; i < columnsDataClasses.Count; i++)
            {
                if (this.isSplit)
                {
                    if (SUBDEALS_WhatLevel(i) == level || SUBDEALS_WhatLevel(i) == -1 && level == 0)
                    {
                        //ok
                    }
                    else
                        continue;
                }

                // >>>>>>>>> Сначала SAVE, потом REFLECT <<<<<<<<<<<<<<
                // delme!!!

                if (DB_Deals_ColumnsNames.Keys.Contains(i))
                    (VALUES[newline, i] as BaseChangedData).Changed += OnInputChanged_SaveToDataBase;

                (VALUES[newline, i] as BaseChangedData).Changed += OnInputChanged;
            }

            // ----------------- 3. Присоединение данных к контролам -----------------
            int startControlIndex;
            int endControlIndex;
            if (!this.isSplit || this.isSplit && level == 0)
            {
                startControlIndex = 0;
                endControlIndex = columnsControlsClasses.Count - 1;
            }
            else
            {
                startControlIndex = columnsControlsClasses.Count + (level - 1) * SUBDEALS_CONTROLS_GetSpliColumnsCount();
                endControlIndex = startControlIndex + SUBDEALS_CONTROLS_GetSpliColumnsCount() - 1;
            }


            for (int i = startControlIndex; i <= endControlIndex; i++)
            {
                int columnIndex = (!this.isSplit ? i : SUBDEALS_CONTROLS_Get_Split_Coordinates(i).column);

                MethodInfo method;
                Type[] types;
                object[] parameters;
                int params_count = 0; //для sub-deal
                if (!this.isSplit)
                {
                    types = new Type[CoVa_Dependences[columnIndex].Count];
                    for (int j = 0; j < CoVa_Dependences[columnIndex].Count; j++)
                        types[j] = VALUES[newline, CoVa_Dependences[columnIndex, j]].GetType();
                }
                else
                {
                    params_count = CoVa_Dependences[columnIndex].Count + (CoVa_Dependences_SPLIT[columnIndex].Count / SUBDEALS.Count);
                    types = new Type[params_count];
                    int ind = 0;
                    for (int j = 0; j < CoVa_Dependences[columnIndex].Count; j++)
                    {
                        types[ind++] = VALUES[newline, CoVa_Dependences[columnIndex, j]].GetType();
                    }
                    for (int j = 0; j < CoVa_Dependences_SPLIT[columnIndex].Count; j++)
                    {
                        int l = SUBDEALS_WhatLevel(CoVa_Dependences_SPLIT[columnIndex, j]);
                        if (l == level)
                            types[ind++] = VALUES[newline, CoVa_Dependences_SPLIT[columnIndex, j]].GetType();
                    }
                }
                method = CONTROLS[newline, i].GetType().GetMethod("AttachData", types);
                if (method != null)
                {
                    if (!this.isSplit)
                    {
                        parameters = new object[CoVa_Dependences[columnIndex].Count];
                        for (int j = 0; j < CoVa_Dependences[columnIndex].Count; j++)
                            parameters[j] = VALUES[newline, CoVa_Dependences[columnIndex, j]];
                    }
                    else
                    {
                        parameters = new object[params_count];
                        int ind = 0;
                        for (int j = 0; j < CoVa_Dependences[columnIndex].Count; j++)
                        {
                            parameters[ind++] = VALUES[newline, CoVa_Dependences[columnIndex, j]];
                        }
                        for (int j = 0; j < CoVa_Dependences_SPLIT[columnIndex].Count; j++)
                        {
                            int l = SUBDEALS_WhatLevel(CoVa_Dependences_SPLIT[columnIndex, j]);
                            if (l == level)
                                parameters[ind++] = VALUES[newline, CoVa_Dependences_SPLIT[columnIndex, j]];
                        }
                    }
                    method.Invoke(CONTROLS[newline, i], parameters);
                }
            }
        }


        void DeactivateDeal(int line, int level)
        {
           
            // -------------- 0 -----------
            if (isRobber)
            {
                ROBBERS[ROBBERS.GetRobberNo(line)].ScoreArray[ROBBERS.GetRobDealNo(line)].Clear();
            }


            // ----------- 1 ----------
            for (int i = 0; i < columnsDataClasses.Count; i++)
            {
                if (this.isSplit)
                {
                    if (SUBDEALS_WhatLevel(i) == level || SUBDEALS_WhatLevel(i) == -1 && level == 0)
                    {
                        //ok
                    }
                    else
                        continue;
                }

                (VALUES[line, i] as BaseChangedData).RemoveChangedHandlers();
            }

            // ------------- 2 ------------
            for (int i = 0; i < columnsDataClasses.Count; i++)
            {
                if (this.isSplit)
                {
                    if (SUBDEALS_WhatLevel(i) == level || SUBDEALS_WhatLevel(i) == -1 && level == 0)
                    {
                        //ok
                    }
                    else
                        continue;
                }

                (VALUES[line, i] as BaseChangedData).Clear();
            }

            // ------------- 3 --------------
            int startControlIndex;
            int endControlIndex;
            if (!this.isSplit || this.isSplit && level == 0)
            {
                startControlIndex = 0;
                endControlIndex = columnsControlsClasses.Count - 1;
            }
            else
            {
                startControlIndex = columnsControlsClasses.Count + (level - 1) * SUBDEALS_CONTROLS_GetSpliColumnsCount();
                endControlIndex = startControlIndex + SUBDEALS_CONTROLS_GetSpliColumnsCount() - 1;
            }


            for (int i = startControlIndex; i <= endControlIndex; i++)
            {
                if (CONTROLS[line, i].GetType().GetInterfaces().Contains(typeof(IDetachData)))
                    (CONTROLS[line, i] as IDetachData).DetachData(true);
            }
        }


        bool PrepareNewEmptyDeal(int robNo, int level, int line)
        {
            // добавить пустую запись в БД
            if (!isSplit || isSplit && level == 0)
            {
                int iMatchNo = (isRobber ? robNo : MATCHES_GetMatchNo(VALUES.Count));
                int db_match_id = DB_Matches_Ids[iMatchNo];

                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "INSERT INTO " + DB_Deals_TableName + "(fk_Match_id) VALUES(" + db_match_id + ")";  //остальные столбцы в DB_Deals_ColumnsNames будут NULL
                int newDealId = -1;

                while(DB.ExecuteNonQuery(sqlQuery, true, out newDealId) == 0)
                {
                    if (MessageBox.Show("Новая сдача не добавлена в матч " + db_match_id + "", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    {
                        DB.sqlConnection.Close();
                        DB.sqlConnection.Open();
                    }
                    else
                    {
                        return false;
                    }
                }

                DB_Deals_Ids[iMatchNo].Add(newDealId);
                return true;
            }
            
            // если сплит и level == 1, то установить значение IsSecondStarted=1
            else
            {
                int id = DB_Deals_Ids[MATCHES_GetMatchNo(line), MATCHES_GetMDealNo(line)];

                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "UPDATE " + DB_Deals_TableName + " SET IsSecondStarted='TRUE' WHERE id=" + id;

                while(DB.ExecuteNonQuery(sqlQuery, true) == 0)
                {
                    if (MessageBox.Show("Сдача " + id + " за 2 столом не добавлена", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    {
                        DB.sqlConnection.Close();
                        DB.sqlConnection.Open();
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
        }


        int AddNewEmptyDeal(int robNo, int level, int line) // если не робберный бридж, то robNo игнорить; level и line - только для добавления sub-deal
        {
            // № создаваемой сдачи (или уже созданной, если добавляется sub-deal для стола > 0)
            int newline;
            if (this.isSplit && level > 0)
            {
                newline = line;
            }
            else
            {
                if (this.isRobber)
                {
                    // Добавить сдачу в роббер:
                    ROBBERS[robNo].ScoreArray.Add(new DealScoreExt());

                    // глобальный № новой сдачи
                    newline = ROBBERS.GetFirstLineOfRobber(robNo) + (ROBBERS[robNo].ScoreArray.Count - 1);
                }
                else
                {
                    newline = VALUES.Count;
                }
            }


            // Динамическое создание значений для новой сдачи:
            if (!this.isSplit || this.isSplit && level == 0)
            {
                VALUES.Insert(newline, new ArrayList());
                for (int i = 0; i < columnsDataClasses.Count; i++)
                {
                    VALUES[newline].Add(null);
                }
            }
            for (int i = 0; i < columnsDataClasses.Count; i++)
            {
                if (this.isSplit)
                {
                    if (SUBDEALS_WhatLevel(i) == level || SUBDEALS_WhatLevel(i) == -1 && level == 0)
                    {
                        //ok
                    }
                    else
                        continue;
                }

                // Динамическое создание объекта:
                VALUES[newline, i] = ClassBuilder.CreateInstance(columnsDataClasses[i]);
            }

            // Конструкторы:
            for (int i = 0; i < columnsDataClasses.Count; i++)
            {
                if (this.isSplit)
                {
                    if (SUBDEALS_WhatLevel(i) == level || SUBDEALS_WhatLevel(i) == -1 && level == 0)
                    {
                        //ok
                    }
                    else
                        continue;
                }

                MethodInfo method;
                Type[] types;
                object[] parameters;
                if (REFLECT_Constructors.ContainsKey(i))
                {
                    types = new Type[REFLECT_Constructors[i].Length];
                    for (int j = 0; j < REFLECT_Constructors[i].Length; j++)
                        types[j] = GetTypeOfParam("Constructor", (REFLECT_Constructors[i])[j], newline);
                    method = VALUES[newline, i].GetType().GetMethod("Constructor", types);
                    if (method == null)
                    {
                        String strExcMsg = "Не могу найти ф-ию Constructor(";
                        for (int j = 0; j < types.Length; j++)
                        {
                            strExcMsg += (j > 0 ? ", " : "") + types[j].Name;
                        }
                        strExcMsg += ") для " + debug_dataname(newline, i);
                        throw new Exception(strExcMsg);
                    }
                    {
                        parameters = new object[REFLECT_Constructors[i].Length];
                        for (int j = 0; j < REFLECT_Constructors[i].Length; j++)
                            parameters[j] = GetParam("Constructor", (REFLECT_Constructors[i])[j], newline);
                        method.Invoke(VALUES[newline, i], parameters);
                    }
                }
            }


            // Динамическое создание контролов:
            if (!this.isSplit || this.isSplit && level == 0)
            {
                CONTROLS.Insert(newline, new ArrayOfControl());
                CONTROLS_COVERS.Insert(newline, new ArrayOfControl());
            }
            int startControlIndex;
            int endControlIndex;
            if (!this.isSplit || this.isSplit && level == 0)
            {
                startControlIndex = 0;
                endControlIndex = columnsControlsClasses.Count - 1;
            }
            else
            {
                startControlIndex = columnsControlsClasses.Count + (level - 1) * SUBDEALS_CONTROLS_GetSpliColumnsCount();
                endControlIndex = startControlIndex + SUBDEALS_CONTROLS_GetSpliColumnsCount() - 1;
            }


            for (int i = startControlIndex; i <= endControlIndex; i++)
            {
                int columnIndex = ( !this.isSplit ? i : SUBDEALS_CONTROLS_Get_Split_Coordinates(i).column );

                // Динамическое создание объекта:
                object obj = ClassBuilder.CreateInstance(columnsControlsClasses[columnIndex]);

                SetCoordinatesForControl(obj as Control, newline, i);

                int temp = CONTROLS[newline].Add(obj as Control);
                CONTROLS_COVERS[newline].Add(new ControlCover());





                //MessageBox.Show("line=" + newline + ", index = " + i + "\n" + ((obj == null) ? "obj is null" : "obj is fine") + "\n" + CONTROLS[newline].Count + "  add=" + temp);
                    

                if (CONTROLS[newline, i] == null)
                {
                    MessageBox.Show("line=" + newline + ", index = " + i + "\n" + ((obj == null) ? "obj is null" : "obj is fine") + "\n" + CONTROLS[newline].Count + "  add=" + temp);
                    //0 10 | fine | 16 add=15
                }
                




                (CONTROLS_COVERS[newline, i] as ControlCover).AttachControl(CONTROLS[newline, i] as Control);

                this.Controls.Add(CONTROLS_COVERS[newline, i]);

                if (CONTROLS[newline, i].GetType().GetInterfaces().Contains(typeof(IControlInTable)))
                {
                    (CONTROLS[newline, i] as IControlInTable).SetParentTable(this);
                }

                if (CONTROLS[newline, i].GetType().IsSubclassOf(typeof(BaseSelectControl)))
                {
                    (CONTROLS[newline, i] as BaseSelectControl).AttachStuffForSelector(m_form, CONTROLS_COVERS[newline, i]);
                }
            }


            // Добавить поддержку блокировки
            if (!this.isSplit || this.isSplit && level == 0)
            {
                (CONTROLS[newline, 0] as DealInfoControl).LockingLine += OnLockingLine;
            }
            // Добавить возможность блокироки 2 ур.
            if (this.isSplit && level > 0)
            {
                (CONTROLS[newline, 0] as DealInfoControl).SetSub2(true);
            }

            
            return newline;
        }

        // Фокус:
        void FocusOnLine(int line, int level)
        {
            int startControlIndex;
            if (!this.isSplit || this.isSplit && level == 0)
            {
                startControlIndex = 0;
            }
            else
            {
                startControlIndex = columnsControlsClasses.Count + (level - 1) * SUBDEALS_CONTROLS_GetSpliColumnsCount();
            }

            CONTROLS[line][startControlIndex].Focus();
        }
        void FocusOnHeader(int matchNo)
        {
            HeadlinesScores_Controls[matchNo].Focus();
        }
        void ShowRobber(int robNo)
        {
            if (CONTROL_ROBBER != null)
            {
                CONTROL_ROBBER.DetachData();
                if (robNo != -1)
                {
                    CONTROL_ROBBER.AttachData(ROBBERS[robNo]);
                }
            }
        }

        #endregion


        bool AddNewMatch(bool loadFromDB)
        {
            // >>> Проверить, есть ли незаконченные матчи??? <<<
            if (!loadFromDB && PAGE__VIEW)
            {
                ArrayOfInt arrNotCompletedMatches = new ArrayOfInt();
                for (int i = 0; i < DB_Matches_Ids_ALL.Count; i++)
                {
                    if (((DealScore)DB_Matches_TotalScores[i]).IsEmpty())
                        arrNotCompletedMatches.Add(i);
                }
                if (arrNotCompletedMatches.Count > 0)
                {
                    string s = (isRobber ? "Роббер" : "Матч");
                    if (arrNotCompletedMatches.Count > 1)
                        s += (isRobber ? "ы" : "и");
                    s += " ";
                    for (int i = 0; i < arrNotCompletedMatches.Count; i++)
                    {
                        if (i > 0)
                            s += ", ";
                        s += "#" + (arrNotCompletedMatches[i] + 1);
                    }
                    s += (" еще не закончен" + (arrNotCompletedMatches.Count > 1 ? "ы" : "") + "!\nВсе равно начать новый?");

                    if (MessageBox.Show(s, "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        return false;
                }
            }

            if (PAGE__VIEW)
            {
                return AddNewMatch_PAGE_MODE(loadFromDB);
            }
            else
            {
                return AddNewMatch_SIMPLE_MODE(loadFromDB);
            }
        }

        bool AddNewMatch_SIMPLE_MODE(bool loadFromDB)
        {
            if (!loadFromDB) //сохранить в БД
            {
                System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
                sqlQuery.CommandText = "INSERT INTO Matches(fk_Game_id) VALUES(" + DB_Game_Id + ")";
                int new_match_id;

                while (DB.ExecuteNonQuery(sqlQuery, true, out new_match_id) == 0)
                {
                    if (MessageBox.Show("Матч не был создан!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    {
                        DB.sqlConnection.Close();
                        DB.sqlConnection.Open();
                    }
                    else
                    {
                        return false;
                    }
                }

                DB_Matches_Ids.Add(new_match_id);
                DB_Deals_Ids.Add(new ArrayOfInt());

                DB_Matches_TotalScores.Add(new DealScore(0, 0));
            }

            int newMatchNo;
            if (isRobber)
                newMatchNo = ROBBERS.Add(new Robber()); // добавить новый роббер в массив робберов:
            else
                newMatchNo = MATCHES_GetCount(); //

            // Добавить заголовок с рез-том роббера:
            if (isRobber)
            {
                HeadlinesScores.Add(new RobberScore(newMatchNo));
            }
            else
            {
                HeadlinesScores.Add(new MatchScore(newMatchNo, GameSettings_DealsInMatch));
            }
            HeadlinesScores_Controls.Add(new RobberScoreControl());
            HeadlinesScores_CoverControls.Add(new ControlCover());
            (HeadlinesScores_Controls[newMatchNo] as RobberScoreControl).AttachData(HeadlinesScores[newMatchNo] as BaseChangedData);
            SetCoordinatesForControl(HeadlinesScores_Controls[newMatchNo], newMatchNo);
            (HeadlinesScores_Controls[newMatchNo] as RobberScoreControl).SetParentTable(this);
            (HeadlinesScores_CoverControls[newMatchNo] as ControlCover).AttachControl(HeadlinesScores_Controls[newMatchNo]);
            this.Controls.Add(HeadlinesScores_CoverControls[newMatchNo]);

            if (loadFromDB) //загрузить из БД
            {
                //пока не надо
            }

            
            // Фокус:
            FocusOnHeader(newMatchNo);

            return true;
        }

        bool AddNewMatch_PAGE_MODE(bool loadFromDB)
        {
            if (loadFromDB == true) //wtf??? use openpage()!
                return false;

            System.Data.SqlServerCe.SqlCeCommand sqlQuery = DB.CreateQuery();
            sqlQuery.CommandText = "INSERT INTO Matches(fk_Game_id) VALUES(" + DB_Game_Id + ")";
            int new_match_id;

            while (DB.ExecuteNonQuery(sqlQuery, true, out new_match_id) == 0)
            {
                if (MessageBox.Show("Матч не был создан!", "db error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                {
                    DB.sqlConnection.Close();
                    DB.sqlConnection.Open();
                }
                else
                {
                    return false;
                }
            }


            DB_Matches_Ids_ALL.Add(new_match_id);

            DB_Matches_TotalScores.Add(new DealScore(0, 0));

            // + в combo
            match_in_combo m = new match_in_combo();
            m.isrob = isRobber;
            m.no = DB_Matches_Ids_ALL.Count - 1;
            m.id = new_match_id;
            PAGE__ComboBox.Items.Add(m);

            DB_OpenPage(DB_Matches_Ids_ALL.Count - 1);

            return true;
        }


        public void AddNewDeal(MenuItem _mi_)
        {
            //_mi_.MenuItems.Clear();

            // Сначала просканировать линии на наличие незаконченных и неопределенных сдач
            ArrayOfInt NotDefinedLines = new ArrayOfInt();
            for (int i = 0; i < VALUES.Count; i++)
            {
                for(int j = 0 ; j < VALUES[i].Count ; j++)
                {
                    if(VALUES[i, j] != null && !NotNecessaryValues.Contains(j) && VALUES[i, j].GetType().IsSubclassOf(typeof(BaseChangedData)) && (VALUES[i, j] as BaseChangedData).IsDefined() == false)
                    {
                        // пропускать пустые значения из-за "нет контракта"
                        bool skip_if_no_contract = false;
                        for (int k = 0; k < ContractColumns.Count; k++)
                        {
                            if (VALUES[i, ContractColumns[k]] != null && (VALUES[i, ContractColumns[k]] as Contract).NoContract)
                                if (NotNecessaryValues_NOCONTRACT[k].Contains(j))
                                    skip_if_no_contract = true;
                        }
                        if (skip_if_no_contract)
                            continue;

                        // если нет в списке пропущенных столбцов, значит линия НЕЗАПОЛНЕНА
                        if (!NotDefinedLines.Contains(i))
                            NotDefinedLines.Add(i);
                    }
                }
            }
            if(NotDefinedLines.Count != 0)
            {
                String s = "Сначала введите все данные в сдачи:\n";
                for(int i = 0 ; i < NotDefinedLines.Count ; i++)
                {
                    if(i > 0)
                        s += ", ";
                    if (this.isRobber)
                        s += (PAGE__VIEW ? "" : (ROBBERS.GetRobberNo(NotDefinedLines[i]) + 1 + (PAGE__VIEW ? PAGE__NO : 0)).ToString() + ".") + (ROBBERS.GetRobDealNo(NotDefinedLines[i]) + 1).ToString();
                    else
                        s += (PAGE__VIEW ? "" : (MATCHES_GetMatchNo(NotDefinedLines[i]) + 1 + (PAGE__VIEW ? PAGE__NO : 0)).ToString() + ".") + (MATCHES_GetMDealNo(NotDefinedLines[i]) + 1).ToString();
                }
                MessageBox.Show(s);
                return;
            }


            if (this.isRobber)
            {
                ArrayOfInt arrNotCompletedRobbers = new ArrayOfInt();
                if (ROBBERS.Count > 1)
                {
                    for (int i = 0; i < (ROBBERS.Count - 1); i++)
                        if (ROBBERS[i].IsCompleted() == false)
                            arrNotCompletedRobbers.Add(i);
                }

                if (arrNotCompletedRobbers.Count == 0) //нет незаконченных робберов
                {
                    NewDealMenu_NewRDeal(null, null);
                }
                else //в меню возможность закончить робберы
                {
                    ContextMenu ContMenu = new ContextMenu();
                    for (int i = 0; i < arrNotCompletedRobbers.Count; i++)
                    {
                        MenuItem_RDeal mi = new MenuItem_RDeal();
                        mi.robNo = arrNotCompletedRobbers[i];
                        mi.Text = "Продолжить роббер " + (mi.robNo + 1 + (PAGE__VIEW ? PAGE__NO : 0)).ToString();
                        mi.Click += NewDealMenu_InsertRDeal;
                        ContMenu.MenuItems.Add(mi);
                    }
                    MenuItem mi2 = new MenuItem();
                    mi2.Text = (ROBBERS.Count == 0 || ROBBERS[ROBBERS.Count - 1].IsCompleted()) ? "Новый роббер" : "Текущий роббер";
                    mi2.Click += NewDealMenu_NewRDeal;
                    ContMenu.MenuItems.Add(mi2);
                    ContMenu.Show(Program.MainForm, new Point(0, Program.MainForm.Height - MENUHEIGHT - 1));

                    // Удалить конт.меню
                    ContMenu.MenuItems.Clear();
                    ContMenu.Dispose();
                    ContMenu = null;
                }
            }
            else if (this.isSplit)
            {
                ArrayList arrNotCompletedDeals = new ArrayList();
                for (int i = 0; i < VALUES.Count; i++)
                {
                    for (int j = 0; j < SUBDEALS.Count; j++)
                    {
                        if (VALUES[i, SUBDEALS[j, 0]] == null)
                        {
                            arrNotCompletedDeals.Add(new subdeal(i, j));
                        }
                    }
                }

                if (arrNotCompletedDeals.Count == 0) //нет незаконченных сдач
                {
                    NewDealMenu_NewMDeal(null, null);
                }
                else //в меню возможность закончить сдачу под всеми столами
                {
                    ContextMenu ContMenu = new ContextMenu();
                    for (int i = 0; i < arrNotCompletedDeals.Count; i++)
                    {
                        MenuItem_SubDeal mi = new MenuItem_SubDeal();
                        mi.subdeal = (subdeal)arrNotCompletedDeals[i];
                        mi.Text = "Сдача ";
                        if (this.isRobber)
                            mi.Text += (PAGE__VIEW ? "" : (ROBBERS.GetRobberNo(mi.subdeal.line) + 1 + (PAGE__VIEW ? PAGE__NO : 0)).ToString() + ".") + (ROBBERS.GetRobDealNo(mi.subdeal.line) + 1).ToString();
                        else
                            mi.Text += (PAGE__VIEW ? "" : (MATCHES_GetMatchNo(mi.subdeal.line) + 1 + (PAGE__VIEW ? PAGE__NO : 0)).ToString() + ".") + (MATCHES_GetMDealNo(mi.subdeal.line) + 1).ToString();
                        mi.Text += " за столом " + (mi.subdeal.level + 1).ToString();
                        mi.Click += NewDealMenu_NewSubDeal;
                        ContMenu.MenuItems.Add(mi);
                    }
                    MenuItem mi2 = new MenuItem();
                    mi2.Text = "Новая сдача";
                    mi2.Click += NewDealMenu_NewMDeal;
                    ContMenu.MenuItems.Add(mi2);
                    ContMenu.Show(Program.MainForm, new Point(0, Program.MainForm.Height - MENUHEIGHT - 1));

                    // Удалить конт.меню
                    ContMenu.MenuItems.Clear();
                    ContMenu.Dispose();
                    ContMenu = null;
                }
            }
            else
            {
                NewDealMenu_NewMDeal(null, null);
            }

        }

        public struct subdeal
        {
            public int line;
            public int level;
            public subdeal(int line, int level)
            {
                this.line = line;
                this.level = level;
            }
        }

        public class MenuItem_RDeal : MenuItem
        {
            public int robNo;
        }
        public class MenuItem_SubDeal : MenuItem
        {
            public subdeal subdeal;
        }

        void NewDealMenu_NewMDeal(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Добавить новый матч, если нужно
            bool started_new_page = false;
            if (MATCHES_NeedToStartNew())
            {
                if (AddNewMatch(false) == false)
                {
                    Cursor.Current = Cursors.Default;
                    return;
                }
                started_new_page = true;
            }

            // Добавить новую сдачу
            bool created_deal_in_db = PrepareNewEmptyDeal(-666, 0, -666);
            if (created_deal_in_db == false)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            if(!started_new_page)
                LockAllLines();
            int newline = AddNewEmptyDeal(/*false, null,*/ -666, 0, -666);
            ActivateNewDeal(newline, 0, false, null);

            // Разблокировать новую сдачу
            LockLine(newline, 0, false);

            // Отметить её текущей
            CUR_line = newline;
            CUR_level = 0;

            // Обновить таблицу
            AutoScroll_Optimize();
            this.Invalidate();

            // Фокус на новодобавленную строку:
            FocusOnLine(newline, 0);

            Cursor.Current = Cursors.Default;
        }

        void NewDealMenu_NewRDeal(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Добавить новый роббер, если нужно
            bool started_new_page = false;
            if (ROBBERS.Count == 0 || ROBBERS[ROBBERS.Count - 1].IsCompleted())
            {
                if (AddNewMatch(false) == false)
                {
                    Cursor.Current = Cursors.Default;
                    return;
                }
                started_new_page = true;
            }

            // Добавить новую сдачу
            bool created_deal_in_db = PrepareNewEmptyDeal(ROBBERS.Count - 1, 0, -666);
            if (created_deal_in_db == false)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            if (!started_new_page)
                LockAllLines();
            int newline = AddNewEmptyDeal(ROBBERS.Count - 1, 0, -666);
            ActivateNewDeal(newline, 0, false, null);

            // Разблокировать новую сдачу
            LockLine(newline, 0, false);

            // Отметить её текущей
            CUR_line = newline;
            CUR_level = 0;

            // Обновить таблицу
            AutoScroll_Optimize();
            this.Invalidate();

            // Фокус на новодобавленную строку:
            FocusOnLine(newline, 0);

            Cursor.Current = Cursors.Default;
        }

        void NewDealMenu_InsertRDeal(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            MenuItem_RDeal mi = (MenuItem_RDeal)sender;

            // Добавить новую сдачу
            bool created_deal_in_db = PrepareNewEmptyDeal(mi.robNo, 0, -666);
            if (created_deal_in_db == false)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            int newline = AddNewEmptyDeal(mi.robNo, 0, -666);
            ActivateNewDeal(newline, 0, false, null);

            // Передвинуть нижние строки
            ReMoveUnderLine(newline);

            // Обновить таблицу
            AutoScroll_Optimize();
            this.Invalidate();

            // Фокус на новодобавленную строку:
            FocusOnLine(newline, 0);

            Cursor.Current = Cursors.Default;
        }

        void NewDealMenu_NewSubDeal(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            MenuItem_SubDeal mi = (MenuItem_SubDeal) sender;

            // Добавить новую под-сдачу
            bool created_subdeal_in_db = PrepareNewEmptyDeal(-666, mi.subdeal.level, mi.subdeal.line);
            if (created_subdeal_in_db == false)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            LockAllLines();
            int newline = AddNewEmptyDeal(-666, mi.subdeal.level, mi.subdeal.line);
            ActivateNewDeal(newline, mi.subdeal.level, false, null);

            // Разблокировать новую под-сдачу
            LockLine(newline, mi.subdeal.level, false);

            // Отметить её текущей
            CUR_line = newline;
            CUR_level = 1;

            // Обновить таблицу - не надо!

            // Фокус на новодобавленную строку:
            FocusOnLine(newline, mi.subdeal.level);

            Cursor.Current = Cursors.Default;
        }

        // ----------------------------------------------------------  Координаты всех элементов  ------------------------------------------------------------------
        #region Установка координат для элементов
        void SetCoordinatesForControl(Control c, int line, int pos) // - для обычных элементов таблицы
        {
            int columnIndex = (this.isSplit ? SUBDEALS_CONTROLS_Get_Split_Coordinates(pos).column : pos);

            int elementMultiHeight = elementHeight; //мульти-высота
            if (this.isSplit)
            {
                elementMultiHeight += (offset_sub + elementHeight) * (SUBDEALS.Count - 1);
            }

            Rectangle r = new Rectangle(0, 0, 0, 0);
            int offset4marker = (offset < 0 ? 1 : 0);
            r.Y = offset4marker + offset + 1;

r.Y += this.AutoScrollPosition.Y;

            r.Y += headerHeight + offset;
            if (this.isRobber)
            {
                int robNo = ROBBERS.GetRobberNo(line);
                int robDeal = ROBBERS.GetRobDealNo(line);
                for (int i = 0; i < robNo; i++)
                {
                    r.Y += (offset + headlinescoreHeight) + ROBBERS[i].ScoreArray.Count * (elementMultiHeight + offset);
                }
                r.Y += (offset + headlinescoreHeight) + robDeal * (elementMultiHeight + offset);
            }
            else
            {
                r.Y += (offset + elementMultiHeight) * line;
                r.Y += (offset + headlinescoreHeight) * (MATCHES_GetMatchNo(line) + 1);
            }
            if (this.isSplit)
            {
                r.Y += SUBDEALS_CONTROLS_Get_Split_Coordinates(pos).level * (offset_sub + elementHeight);
            }

            r.X = offset4marker + offset + 1;

r.X += this.AutoScrollPosition.X;

            for (int i = 0; i < columnIndex; i++)
            {
                r.X += (offset + arrHeadersWidths[i]);
            }
            r.Width = arrHeadersWidths[columnIndex];
            r.Height = elementHeight;
            if (this.isSplit)
            {
                if ((bool)SUBDEALS_CONTROLS_ISSPLIT[columnIndex] == false)
                    r.Height = elementMultiHeight;
            }


            r.Inflate(-1, -1);
            c.Location = new Point(r.X, r.Y);
            c.Size = new Size(r.Width, r.Height);
        }

        void MoveControlCover(ControlCover cc, int line, int pos) // - для обычных элементов таблицы
        {
            int columnIndex = (this.isSplit ? SUBDEALS_CONTROLS_Get_Split_Coordinates(pos).column : pos);

            int elementMultiHeight = elementHeight; //мульти-высота
            if (this.isSplit)
            {
                elementMultiHeight += (offset_sub + elementHeight) * (SUBDEALS.Count - 1);
            }

            int X = 0, Y = 0;
            int offset4marker = (offset < 0 ? 1 : 0);
            Y = offset4marker + offset + 1;
Y += this.AutoScrollPosition.Y;
            Y += headerHeight + offset;
            if (this.isRobber)
            {
                int robNo = ROBBERS.GetRobberNo(line);
                int robDeal = ROBBERS.GetRobDealNo(line);
                for (int i = 0; i < robNo; i++)
                {
                    Y += (offset + headlinescoreHeight) + ROBBERS[i].ScoreArray.Count * (elementMultiHeight + offset);
                }
                Y += (offset + headlinescoreHeight) + robDeal * (elementMultiHeight + offset);
            }
            else
            {
                Y += (offset + elementMultiHeight) * line;
                Y += (offset + headlinescoreHeight) * (MATCHES_GetMatchNo(line) + 1);
            }
            if (this.isSplit)
            {
                Y += SUBDEALS_CONTROLS_Get_Split_Coordinates(pos).level * (offset_sub + elementHeight);
            }

            X = offset4marker + offset + 1;
X += this.AutoScrollPosition.X;
            for (int i = 0; i < columnIndex; i++)
            {
                X += (offset + arrHeadersWidths[i]);
            }

            X++;
            Y++;
            cc.Move(new Point(X, Y));
        }

        void SetCoordinatesForControl(Control c, int matchNo) // - для HeadlinesScores_Controls[]
        {
            int elementMultiHeight = elementHeight; //мульти-высота
            if (this.isSplit)
            {
                elementMultiHeight += (offset_sub + elementHeight) * (SUBDEALS.Count - 1);
            }

            int offset4marker = (offset < 0 ? 1 : 0);

            Rectangle r = new Rectangle(0, 0, 0, 0);

            r.X = offset4marker + offset + 1;
            r.Y = offset4marker + offset + 1;
r.X += this.AutoScrollPosition.X;
r.Y += this.AutoScrollPosition.Y;
            r.Y += headerHeight + offset;
            
            if (isRobber)
            {
                for (int i = 0; i < matchNo; i++)
                {
                    r.Y += (offset + headlinescoreHeight) + ROBBERS[i].ScoreArray.Count * (elementMultiHeight + offset);
                }
            }
            else
            {
                for(int i = 0; i < matchNo; i++)
                {
                    r.Y += (offset + headlinescoreHeight) + GameSettings_DealsInMatch * (elementMultiHeight + offset);
                }
            }

            int totalWidth = 0;
            for (int i = 0; i < arrHeadersWidths.Count; i++)
            {
                totalWidth += arrHeadersWidths[i];
                if (i > 0)
                    totalWidth += offset;
            }
            r.Width = totalWidth;
            r.Height = headlinescoreHeight;


            r.Inflate(-1, -1);
            c.Location = new Point(r.X, r.Y);
            c.Size = new Size(r.Width, r.Height);
        }

        void MoveControlCover(ControlCover cc, int matchNo) // - для HeadlinesScores_Controls[]
        {
            int elementMultiHeight = elementHeight; //мульти-высота
            if (this.isSplit)
            {
                elementMultiHeight += (offset_sub + elementHeight) * (SUBDEALS.Count - 1);
            }

            int offset4marker = (offset < 0 ? 1 : 0);

            int X = 0, Y = 0;

            X = offset4marker + offset + 1;
            Y = offset4marker + offset + 1;
X += this.AutoScrollPosition.X;
Y += this.AutoScrollPosition.Y;
            Y += headerHeight + offset;

            if (isRobber)
            {
                for (int i = 0; i < matchNo; i++)
                {
                    Y += (offset + headlinescoreHeight) + ROBBERS[i].ScoreArray.Count * (elementMultiHeight + offset);
                }
            }
            else
            {
                for (int i = 0; i < matchNo; i++)
                {
                    Y += (offset + headlinescoreHeight) + GameSettings_DealsInMatch * (elementMultiHeight + offset);
                }
            }


            X++;
            Y++;
            cc.Move(new Point(X, Y));
        }
        #endregion

        // -------------------------------------------------------------  Отрисовка  ----------------------------------------------------------
        #region Отрисовка
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;

            Pen pen_Black = new Pen(Color.Black);
            SolidBrush brush_LightBlue = new SolidBrush(Color.FromArgb(66, 160, 255));
            SolidBrush brush_Black = new SolidBrush(Color.Black);

            int elementMultiHeight = elementHeight; //мульти-высота
            if (this.isSplit)
            {
                elementMultiHeight += (offset_sub + elementHeight) * (SUBDEALS.Count - 1);
            }

            int offset4marker = (offset < 0 ? 1 : 0);

            // Общая граница
            int totalWidth = 0, totalHeight = 0;
            for (int i = 0; i < arrHeadersWidths.Count; i++)
            {
                totalWidth += arrHeadersWidths[i];
                if (i > 0)
                    totalWidth += offset;
            }
            totalHeight += headerHeight;
            if (this.isRobber)
            {
                for (int i = 0; i < ROBBERS.Count; i++)
                {
                    totalHeight += (offset + headlinescoreHeight) + ROBBERS[i].ScoreArray.Count * (elementMultiHeight + offset);
                }
            }
            else
            {
                totalHeight += (offset + elementMultiHeight) * VALUES.Count;
                totalHeight += (offset + headlinescoreHeight) * MATCHES_GetCount();
            }

            Rectangle rect = new Rectangle(this.AutoScrollPosition.X + offset4marker, this.AutoScrollPosition.Y + offset4marker, totalWidth + 2 * (offset + 1), totalHeight + 2 * (offset + 1));
            SmallHelper.DrawRect(g, pen_Black, rect);

            // Заголовки
            rect.X = this.AutoScrollPosition.X + offset4marker + offset + 1;
            rect.Y = this.AutoScrollPosition.Y + offset4marker + offset + 1;
            rect.Height = headerHeight;
            for (int i = 0; i < arrHeadersWidths.Count; i++)
            {
                rect.Width = arrHeadersWidths[i];
                SmallHelper.DrawRect(g, pen_Black, rect);
                SmallHelper.FillRectInside(g, brush_LightBlue, rect);
                SmallHelper.DrawMultiString(g, rect, arrHeadersNames[i], fontHeader, brush_Black, StringAlignment.Center, StringAlignment.Near, StringAlignment.Center, 0, 0, -3);
                rect.X += (arrHeadersWidths[i] + offset);
            }
            rect.Y += (headerHeight + offset);


            /*
            if (this.isRobber)
            {
                for (int i = 0; i < ROBBERS.Count; i++)
                {
                    rect.X = this.AutoScrollPosition.X + offset4marker + offset + 1;
                    rect.Width = totalWidth;
                    rect.Height = headlinescoreHeight;
                    SmallHelper.DrawRect(g, pen_Black, rect);
                    rect.Y += (headlinescoreHeight + offset);

                    for (int j = 0; j < ROBBERS[i].ScoreArray.Count; j++)
                    {
                        rect.X = this.AutoScrollPosition.X + offset4marker + offset + 1;
                        int temp = rect.Y;
                        for (int k = 0; k < arrHeadersWidths.Count; k++)
                        {
                            rect.Y = temp;
                            rect.Width = arrHeadersWidths[k];
                            if(!this.isSplit || this.isSplit && (bool)SUBDEALS_CONTROLS_ISSPLIT[k] == false)
                            {
                                rect.Height = elementMultiHeight;
                                SmallHelper.DrawRect(g, pen_Black, rect);
                            }
                            else
                            {
                                for(int x = 0 ; x < SUBDEALS.Count ; x++)
                                {
                                    rect.Height = elementHeight;
                                    if(x > 0)
                                        rect.Y += (elementHeight + offset_sub);
                                    SmallHelper.DrawRect(g, pen_Black, rect);
                                }
                            }
                            rect.X += (arrHeadersWidths[k] + offset);
                        }
                        rect.Y = temp + (elementMultiHeight + offset);
                    }
                }
            }
            else
            {
                for (int i = 0; i < VALUES.Count; i++)
                {
                    rect.X = this.AutoScrollPosition.X + offset4marker + offset + 1;
                    int temp = rect.Y;
                    for (int k = 0; k < arrHeadersWidths.Count; k++)
                    {
                        rect.Y = temp;
                        rect.Width = arrHeadersWidths[k];
                        if (!this.isSplit || this.isSplit && (bool)SUBDEALS_CONTROLS_ISSPLIT[k] == false)
                        {
                            rect.Height = elementMultiHeight;
                            SmallHelper.DrawRect(g, pen_Black, rect);
                        }
                        else
                        {
                            for (int x = 0; x < SUBDEALS.Count; x++)
                            {
                                rect.Height = elementHeight;
                                if (x > 0)
                                    rect.Y += (elementHeight + offset_sub);
                                SmallHelper.DrawRect(g, pen_Black, rect);
                            }
                        }
                        rect.X += (arrHeadersWidths[k] + offset);
                    }
                    rect.Y = temp + (elementMultiHeight + offset);
                }
            }*/


            // Отрисовка сетки
            for (int i = 0; i < (isRobber ? ROBBERS.Count : MATCHES_GetCount()); i++)
            {
                rect.X = this.AutoScrollPosition.X + offset4marker + offset + 1;
                rect.Width = totalWidth;
                rect.Height = headlinescoreHeight;
                SmallHelper.DrawRect(g, pen_Black, rect);
                rect.Y += (headlinescoreHeight + offset);

                for (int j = 0; j < (isRobber ? ROBBERS[i].ScoreArray.Count : MATCHES_GetDealsCount(i)); j++)
                {
                    rect.X = this.AutoScrollPosition.X + offset4marker + offset + 1;
                    int temp = rect.Y;
                    for (int k = 0; k < arrHeadersWidths.Count; k++)
                    {
                        rect.Y = temp;
                        rect.Width = arrHeadersWidths[k];
                        if (!this.isSplit || this.isSplit && (bool)SUBDEALS_CONTROLS_ISSPLIT[k] == false)
                        {
                            rect.Height = elementMultiHeight;
                            SmallHelper.DrawRect(g, pen_Black, rect);
                        }
                        else
                        {
                            for (int x = 0; x < SUBDEALS.Count; x++)
                            {
                                rect.Height = elementHeight;
                                if (x > 0)
                                    rect.Y += (elementHeight + offset_sub);
                                SmallHelper.DrawRect(g, pen_Black, rect);
                            }
                        }
                        rect.X += (arrHeadersWidths[k] + offset);
                    }
                    rect.Y = temp + (elementMultiHeight + offset);
                }
            }
        }
        #endregion
        
        // -------------------------------------------------------------  Стратегия рассчётов  ---------------------------------------------------------------
        #region Стратегия рассчётов
        Type GetTypeOfParam(string fname_str, string param_str, int line)
        {
            if (ID_NAMES.ContainsValue(param_str)) //переменная
            {
                int id = -1;
                for (int k = 0; k < ID_NAMES.Keys.Count; k++)
                {
                    if (ID_NAMES[ID_NAMES.Keys.ElementAt(k)] == param_str)
                    {
                        id = ID_NAMES.Keys.ElementAt(k);
                        break;
                    }
                }
                if (id >= 0 && id < 100) // параметр - обычные данные в таблице
                {
                    if (id < columnsDataClasses.Count)
                        return columnsDataClasses[id];
                    else
                    {
                        throw new Exception("Параметр " + param_str + " ф-ии " + fname_str + ": не могу определить тип, т.к. для id=" + id + " не определен тип!");
                        return null;
                    }
                }
                else // параметр - роббер или еще что-то нестандартное (но заранее определенное)
                {
                    if (id == (int)CONST.LINE_NO || id == (int)CONST.RDEAL_NO || id == (int)CONST.ROBBER_NO || id == (int)CONST.ROBBER_FIRSTLINE || id == (int)CONST.MATCH_NO || id == (int)CONST.MATCH_FIRSTLINE || id == (int)CONST.MDEAL_NO || id == (int)CONST.ROBBER_NO__REAL || id == (int)CONST.MATCH_NO__REAL || id == (int)CONST.LINE_NO__REAL)
                        return typeof(int);
                    else if(id == (int)CONST.LOADING_FROM_DB)
                        return typeof(bool);
                    else if(id == (int)CONST.FIRST_DEALER)
                        return typeof(Quarters);
                    else if (id == (int)CONST.ROBBER)
                        return typeof(Robber);
                    else if (id == (int)CONST.ROBBER_TOTAL_SCORE)
                        return typeof(RobberScore);
                    else if (id == (int)CONST.MATCH_TOTAL_SCORE)
                        return typeof(MatchScore);
                    else if (id == (int)CONST.RDEAL_FULLSCORE)
                        return typeof(DealScoreExt);
                    else
                    {
                        throw new Exception("Параметр " + param_str + " ф-ии " + fname_str + ": не могу определить тип, т.к. id=" + id + " неизвестно!");
                        return null;
                    }
                }
            }
            else
            {
                if (param_str.ToUpper() == "TRUE" || param_str.ToUpper() == "FALSE")
                {
                    return typeof(bool);
                }
                else
                {
                    try
                    {
                        int.Parse(param_str);
                        return typeof(int);
                    }
                    catch (FormatException e_int)
                    {
                        try
                        {
                            float.Parse(param_str);
                            return typeof(float);
                        }
                        catch (FormatException e_float)
                        {
                            throw new Exception("Параметр " + param_str + " ф-ии " + fname_str + ": не могу определить тип!");
                            return null;
                        }
                    }
                }
            }
        }

        object GetParam(string fname_str, string param_str, int line)
        {
            if (ID_NAMES.ContainsValue(param_str)) //переменная
            {
                int id = -1;
                for (int k = 0; k < ID_NAMES.Keys.Count; k++)
                {
                    if (ID_NAMES[ID_NAMES.Keys.ElementAt(k)] == param_str)
                    {
                        id = ID_NAMES.Keys.ElementAt(k);
                        break;
                    }
                }
                if (id >= 0 && id < 100) // параметр - обычные данные в таблице
                {
                    if (id < columnsDataClasses.Count)
                        return VALUES[line, id];
                    else
                    {
                        throw new Exception("Параметр " + param_str + " ф-ии " + fname_str + ": не могу определить этот параметр, т.к. для id=" + id + " не определен столбец в таблице!");
                        return null;
                    }
                }
                else // параметр - роббер или еще что-то нестандартное (но заранее определенное)
                {
                    if (id == (int)CONST.FIRST_DEALER)
                        return GameSettings_FirstDealer;
                    else if (id == (int)CONST.LOADING_FROM_DB)
                        return loadingFromDB;
                    else if (id == (int)CONST.LINE_NO)
                        return line;
                    else if (id == (int)CONST.LINE_NO__REAL)
                        return (!PAGE__VIEW ? line : (line + PAGE__LINES_BEFORE));
                    else if (id == (int)CONST.RDEAL_NO)
                        return ROBBERS.GetRobDealNo(line);
                    else if (id == (int)CONST.MDEAL_NO)
                        return MATCHES_GetMDealNo(line);
                    else if (id == (int)CONST.ROBBER_NO)
                        return ROBBERS.GetRobberNo(line);
                    else if (id == (int)CONST.ROBBER_NO__REAL)
                        return (!PAGE__VIEW ? ROBBERS.GetRobberNo(line) : (ROBBERS.GetRobberNo(line) + PAGE__NO));
                    else if (id == (int)CONST.MATCH_NO)
                        return MATCHES_GetMatchNo(line);
                    else if (id == (int)CONST.MATCH_NO__REAL)
                        return (!PAGE__VIEW ? MATCHES_GetMatchNo(line) : (MATCHES_GetMatchNo(line) + PAGE__NO));
                    else if (id == (int)CONST.ROBBER_FIRSTLINE)
                        return ROBBERS.GetFirstLineOfRobber(ROBBERS.GetRobberNo(line));
                    else if (id == (int)CONST.MATCH_FIRSTLINE)
                        return MATCHES_GetFirstLine(MATCHES_GetMatchNo(line));
                    else if (id == (int)CONST.ROBBER)
                        return ROBBERS[ROBBERS.GetRobberNo(line)];
                    else if (id == (int)CONST.ROBBER_TOTAL_SCORE)
                        return HeadlinesScores[ROBBERS.GetRobberNo(line)];
                    else if (id == (int)CONST.MATCH_TOTAL_SCORE)
                        return HeadlinesScores[MATCHES_GetMatchNo(line)];
                    else if (id == (int)CONST.RDEAL_FULLSCORE)
                        return ROBBERS[ROBBERS.GetRobberNo(line)].ScoreArray[ROBBERS.GetRobDealNo(line)];
                    else
                    {
                        throw new Exception("Параметр " + param_str + " ф-ии " + fname_str + ": не могу определить этот параметр, т.к. id=" + id + " неизвестно!");
                        return null;
                    }
                }
            }
            else
            {
                if (param_str.ToUpper() == "TRUE")
                {
                    return true;
                }
                else if (param_str.ToUpper() == "FALSE")
                {
                    return false;
                }
                else
                {
                    try
                    {
                        return int.Parse(param_str);
                    }
                    catch (FormatException e_int)
                    {
                        try
                        {
                            return float.Parse(param_str);
                        }
                        catch (FormatException e_float)
                        {
                            throw new Exception("Параметр " + param_str + " ф-ии " + fname_str + ": не могу определить этот параметр!");
                            return null;
                        }
                    }
                }
            }
        }



        void REFLECT_CALL_FUNCTION(int line, int pos)
        {
            if (REFLECT_Functions.ContainsKey(pos))
            {
                FuncInfo[] fis = REFLECT_Functions[pos];
                MethodInfo method;
                Type[] types;
                object[] parameters;
                for (int i = 0; i < fis.Length; i++)
                {
                    if (fis[i].fparams != null && fis[i].fparams.Length != 0)
                    {
                        types = new Type[fis[i].fparams.Length];
                        for (int j = 0; j < fis[i].fparams.Length; j++)
                        {
                            types[j] = GetTypeOfParam(fis[i].fname, fis[i].fparams[j].Trim(), line);
                        }
                    }
                    else
                    {
                        types = new Type[] { };
                    }

                    if (REFLECT_InsideFunctionsList.Contains(fis[i].fname)) //встроенная ф-ия (в этом классе)
                        method = this.GetType().GetMethod(fis[i].fname, types);
                    else
                        method = typeof(BridgeGameScoring).GetMethod(fis[i].fname, types);

                    if (method == null)
                    {
                        String strExcMsg = "Не могу найти ф-ию " + fis[i].fname + "(";
                        for (int j = 0; j < types.Length; j++)
                        {
                            strExcMsg += (j > 0 ? ", " : "") + types[j].Name;
                        }
                        strExcMsg += ")";
                        throw new Exception(strExcMsg);
                    }
                    else
                    {
                        if (fis[i].fparams != null && fis[i].fparams.Length != 0)
                        {
                            parameters = new object[fis[i].fparams.Length];
                            for (int j = 0; j < fis[i].fparams.Length; j++)
                            {
                                parameters[j] = GetParam(fis[i].fname, fis[i].fparams[j].Trim(), line);
                            }
                        }
                        else
                        {
                            parameters = new object[]{};
                        }

                        try
                        {
                            if (REFLECT_InsideFunctionsList.Contains(fis[i].fname)) //встроенная ф-ия (в этом классе)
                                method.Invoke(this, parameters);
                            else
                                method.Invoke(null, parameters);
                        }
                        catch (Exception e)
                        {
                            String strExcMsg = "Ошибка при выполнении ф-ии " + fis[i].fname + "(";
                            for (int j = 0; j < types.Length; j++)
                            {
                                strExcMsg += (j > 0 ? ", " : "") + types[j].Name;
                            }
                            strExcMsg += ")";
                            MessageBox.Show(strExcMsg);
                            throw;
                        }
                    }
                }
            }
        }

        struct coord
        {
            public int line;
            public int pos;
            public coord(int line, int pos)
            {
                this.line = line;
                this.pos = pos;
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() != this.GetType())
                    return false;
                coord c2 = (coord)obj;
                return (this.line == c2.line && this.pos == c2.pos);
            }
            public static bool operator ==(coord c1, coord c2)
            {
                return c1.Equals(c2);
            }
            public static bool operator !=(coord c1, coord c2)
            {
                return !c1.Equals(c2);
            }
        }

        bool Chain_IsHighPriority(ArrayList ChainOfInfluences, coord coo)
        {
            if (coo.pos == (int)CONST.ROBBER || coo.pos == (int)CONST.ROBBER_NO || coo.pos == (int)CONST.ROBBER_NO__REAL || coo.pos == (int)CONST.ROBBER_TOTAL_SCORE)
                return true;
            else
                return false;
        }

        void LinkAfterLink(ArrayList ChainOfInfluences, coord coo1, coord coo2)
        {
            // Робберные элементы имеют в качестве line номер !ПЕРВОЙ! линии роббера:
            if (coo1.pos == (int)CONST.ROBBER || coo1.pos == (int)CONST.ROBBER_NO || coo1.pos == (int)CONST.ROBBER_NO__REAL || coo1.pos == (int)CONST.ROBBER_TOTAL_SCORE)
                coo1.line = ROBBERS.GetFirstLineOfRobber(ROBBERS.GetRobberNo(coo1.line));
            if (coo2.pos == (int)CONST.ROBBER || coo2.pos == (int)CONST.ROBBER_NO || coo2.pos == (int)CONST.ROBBER_NO__REAL || coo2.pos == (int)CONST.ROBBER_TOTAL_SCORE)
                coo2.line = ROBBERS.GetFirstLineOfRobber(ROBBERS.GetRobberNo(coo2.line));

            if (ChainOfInfluences.Contains(coo2))
            {
                //передвинуть coo2 после coo1 (можно в конец)
                if (ChainOfInfluences.Contains(coo1) && ChainOfInfluences.IndexOf(coo2) < ChainOfInfluences.IndexOf(coo1))
                {
                    ChainOfInfluences.RemoveAt(ChainOfInfluences.IndexOf(coo2));
                    if (Chain_IsHighPriority(ChainOfInfluences, coo2))
                        ChainOfInfluences.Add(coo2);
                    else
                        ChainOfInfluences.Insert(ChainOfInfluences.IndexOf(coo1) + 1, coo2);
                }
            }
            else
            {
                if (ChainOfInfluences.Contains(coo1))  //вставить coo2 сразу после coo1 (можно в конец)
                {
                    if (Chain_IsHighPriority(ChainOfInfluences, coo2))
                        ChainOfInfluences.Add(coo2);
                    else
                        ChainOfInfluences.Insert(ChainOfInfluences.IndexOf(coo1) + 1, coo2);
                }
                else  //вставить coo2 куда-нибудь (лучше в начало)
                {
                    if (Chain_IsHighPriority(ChainOfInfluences, coo2))
                        ChainOfInfluences.Add(coo2);
                    else
                        ChainOfInfluences.Insert(0, coo2);
                }
            }
        }

        void BuildChainForLink(ArrayList ChainOfInfluences, coord coo1)
        {
            for (int i = 0; i < REFLECT_Depends.Keys.Count; i++)
            {
                if (REFLECT_Depends[REFLECT_Depends.Keys.ElementAt(i)].Contains(coo1.pos))
                {
                    coord coo2 = new coord(coo1.line, REFLECT_Depends.Keys.ElementAt(i));
                    LinkAfterLink(ChainOfInfluences, coo1, coo2);
                    if(coo1 != coo2)
                        BuildChainForLink(ChainOfInfluences, coo2);  // recurcy
                }
            }
        }

        void BuildChainInVertical(ArrayList ChainOfInfluences, int line)
        {
            if (!isRobber)
                return;

            bool v_re = false;
            int v_re_after_line = line; //линия, после которой надо пересчитать
            int v_re_pos = 0; //позиция роббера - 1000
            for (int i = 0; i < ChainOfInfluences.Count; i++)
            {
                if (REFLECT_Recounts.ContainsKey(((coord)ChainOfInfluences[i]).pos) && REFLECT_Recounts[((coord)ChainOfInfluences[i]).pos] != null && REFLECT_Recounts[((coord)ChainOfInfluences[i]).pos].Length != 0)
                {
                    v_re = true;
                    v_re_pos = ((coord)ChainOfInfluences[i]).pos;
                    break;
                }
            }

            if (v_re)
            {
                int robNo = ROBBERS.GetRobberNo(v_re_after_line);
                int robdealNo = ROBBERS.GetRobDealNo(v_re_after_line);
                for (int i = robdealNo + 1; i < ROBBERS[robNo].ScoreArray.Count; i++)
                {
                    int nextline = v_re_after_line + (i - robdealNo);
                    for (int j = 0; j < REFLECT_Recounts[v_re_pos].Length; j++)
                    {
                        coord coo2 = new coord(/*!!!*/ nextline /*!!!*/, (REFLECT_Recounts[v_re_pos])[j]);
                        LinkAfterLink(ChainOfInfluences, new coord(v_re_after_line, v_re_pos), coo2);
                        BuildChainForLink(ChainOfInfluences, coo2);
                    }
                }
            }
        }

        bool REFLECT_DEPENDENCES_BUSY = false; // перемычка, чтобы не вызывались копии ф-ии, когда она уже работает
        void REFLECT_DEPENDENCES(int line, int pos) // {line, -1} для расчета при нажатии [NEW] для новой линии line
        {
            REFLECT_DEPENDENCES(line, new ArrayOfInt(pos));
        }

        void REFLECT_DEPENDENCES(int line, ArrayOfInt poss)
        {
            if (REFLECT_DEPENDENCES_BUSY)
                return;

            REFLECT_DEPENDENCES_BUSY = true;

            // Создание цепочки:
            ArrayList ChainOfInfluences = new ArrayList();
            for (int i = 0; i < poss.Count; i++)
            {
                BuildChainForLink(ChainOfInfluences, new coord(line, poss[i])); //цепочка по горизонтали
            }
            if (isRobber)
                BuildChainInVertical(ChainOfInfluences, line); //и по вертикали

            if (reflect_debug_msg)
            {
                string s = "REFLECT from ";
                for (int i = 0; i < poss.Count; i++)
                    s += debug_dataname(line, poss[i]);
                s += ":\n";
                for (int j = 0; j < ChainOfInfluences.Count; j++)
                    s += (j > 0 ? " => " : "") + debug_dataname(((coord)ChainOfInfluences[j]).line, ((coord)ChainOfInfluences[j]).pos);
                MessageBox.Show(s);
            }

            // Ввызов всех ф-ий по цепочке:
            for (int i = 0; i < ChainOfInfluences.Count; i++)
            {
                try
                {
                    REFLECT_CALL_FUNCTION(((coord)ChainOfInfluences[i]).line, ((coord)ChainOfInfluences[i]).pos);
                }
                catch (Exception e)
                {
                    // REFLECT DEBUG MESSAGEBOX
                    string s = "ОШИБКА при изменении данных по цепочке от ";
                    for (int j = 0; j < poss.Count; j++)
                    {
                        if (j > 0)
                            s += ", ";
                        s += debug_dataname(line, poss[j]);
                    }
                    s += ":\n";
                    for (int j = 0; j < ChainOfInfluences.Count; j++)
                        s += (j > 0 ? " => " : "") + debug_dataname(((coord)ChainOfInfluences[j]).line, ((coord)ChainOfInfluences[j]).pos);
                    s += "\nОШИБКА на этапе выполнения Ф-ий для " + debug_dataname(((coord)ChainOfInfluences[i]).line, ((coord)ChainOfInfluences[i]).pos);
                    MessageBox.Show(s);
                    throw;
                }
            }

            REFLECT_DEPENDENCES_BUSY = false;
            ChainOfInfluences.Clear();
        }

        string debug_dataname(int line, int pos)
        { return ( (pos == -1 ? "начало" : (ID_NAMES.ContainsKey(pos) ? ID_NAMES[pos] : pos.ToString())) + "[" + line + "]" ); }


        public void CleanUnnecessaryRDealsInRobber(Robber rob, int robFirstLine)
        {
            // при загрузке данных из БД не очищать роббер!
            if (loadingFromDB)
                return;


            // Найти последнюю сдачу:
            int robdealNo_last = -1;
            for (int i = 0; i < rob.ScoreArray.Count; i++)
            {
                if (rob.WhereCompleted() != -1 && rob.WhereCompleted() == i)
                {
                    robdealNo_last = i;
                    break;
                }
            }

            // Удаление лишних сдач в текущем роббере:
            if (robdealNo_last != -1)
            {
                int dealNo_delete = robdealNo_last + 1;
                while ((rob.ScoreArray.Count - dealNo_delete) > 0)
                {
                    DB_DeleteLine(robFirstLine + dealNo_delete); //даже если неудачно - продолжить цикл
                    DeleteLine(robFirstLine + dealNo_delete);
                }

                // Передвинуть нижние строки
                ReMoveUnderLine(robFirstLine + robdealNo_last);

                // Обновить таблицу
                AutoScroll_Optimize();
                this.Invalidate();
            }
        }

        public void InvalidateRobber(Robber rob)
        {
            //rob.OnUpdate(null, new Robber.UpdateEventArgs(robNo));
            rob.OnUpdate(null, null);
        }

        // Match Stuff:
        public void SetMatchScore(MatchScore TotalScore, int matchNo, int colScore)
        {
            int iNS = 0, iEW = 0;
            bool compl = true;
            if(MATCHES_GetDealsCount(matchNo) < GameSettings_DealsInMatch)
                compl = false;

            SimpleScore dscore;
            for (int i = MATCHES_GetFirstLine(matchNo); i < (MATCHES_GetFirstLine(matchNo) + MATCHES_GetDealsCount(matchNo)); i++)
            {
                dscore = (SimpleScore)VALUES[i, colScore];
                if (dscore == null || !dscore.IsDefined())
                {
                    compl = false;
                }
                else
                {
                    iNS += dscore.Score.NS;
                    iEW += dscore.Score.EW;
                }
            }

            TotalScore.SetScore(iNS, iEW, compl);
        }

        // *** TOTAL SCORE ***
        public void TotalScore_RobberMode(RobberScore robScore, int robNo)
        {
            if (loadingFromDB)
                return;

            DealScore ts_score, ts_score_OLD;
            ts_score = (robScore as IMatchGetScore).GetScore();
            ts_score_OLD = (DealScore) DB_Matches_TotalScores[robNo];

            if (!ts_score.Equals(ts_score_OLD))
            {
                //MessageBox.Show("saver [" + robNo + "]  " + ts_score_OLD + " => " + ts_score); //delme

                DB_SaveMatchScore(robNo, ts_score);
            }
        }

        // *** TOTAL SCORE ***
        public void TotalScore_MatchMode(MatchScore matchScore, int matchNo)
        {
            if (loadingFromDB)
                return;

            DealScore ts_score, ts_score_OLD;
            ts_score = (matchScore as IMatchGetScore).GetScore();
            ts_score_OLD = (DealScore) DB_Matches_TotalScores[matchNo];

            if (!ts_score.Equals(ts_score_OLD))
            {
                //MessageBox.Show("savem [" + matchNo + "]  " + ts_score_OLD + " => " + ts_score); //delme

                DB_SaveMatchScore(matchNo, ts_score);
            }
        }

        // Subdeals Stuff:
        int SUBDEALS_WhatLevel(int dataIndex)
        {
            for (int i = 0; i < SUBDEALS.Count; i++)
            {
                if (SUBDEALS[i].Contains(dataIndex))
                    return i;
            }
            return -1;
        }

        int Convert_SplitColumnNo_To_ColumnNo(int splitCol)
        {
            int count = 0;
            int i;
            for (i = 0; i < SUBDEALS_CONTROLS_ISSPLIT.Count; i++)
            {
                if (((bool)SUBDEALS_CONTROLS_ISSPLIT[i]) == true)
                    count++;
                if (count == (splitCol + 1))
                    break;
            }
            return i;
        }
        int SUBDEALS_CONTROLS_GetSpliColumnsCount()
        {
            int columns_split = 0;
            for (int i = 0; i < SUBDEALS_CONTROLS_ISSPLIT.Count; i++)
                if (((bool)SUBDEALS_CONTROLS_ISSPLIT[i]) == true)
                    columns_split++;
            return columns_split;
        }
        struct SPLIT_COORD
        {
            public int column;
            public int level;
            public SPLIT_COORD(int col, int lev)
            {
                column = col;
                level = lev;
            }
        };
        SPLIT_COORD SUBDEALS_CONTROLS_Get_Split_Coordinates(int index)
        {
            int columns_total = columnsControlsClasses.Count;
            int columns_split = SUBDEALS_CONTROLS_GetSpliColumnsCount();

            if (index < columns_total)
            {
                return new SPLIT_COORD(index, 0);
            }
            else
            {
                int level = 1 + (index - columns_total) / columns_split;
                int split_column = (index - columns_total - (level - 1) * columns_split);
                int column = Convert_SplitColumnNo_To_ColumnNo(split_column);
                return new SPLIT_COORD(column, level);
            }
        }

        #endregion


        bool reflect_debug_msg = false;
    }


}
