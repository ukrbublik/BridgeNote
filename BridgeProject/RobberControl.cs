using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace BridgeProject
{
    public class DealScoreExt
    {
        // Если все пас, то счет = 0, и это нормально
        public bool all_players_passed = false;

        public void DealWasNotPlayed()
        {
            NS_down = 0;
            NS_up = null;
            EW_down = 0;
            EW_up = null;
            all_players_passed = true;
        }

        public int[] NS_up;
        public int NS_down;
        public int[] EW_up;
        public int EW_down;

        public void Clear()
        {
            NS_down = 0;
            NS_up = null;
            EW_down = 0;
            EW_up = null;
            all_players_passed = false;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DealScoreExt))
                return false;
            DealScoreExt ds2 = obj as DealScoreExt;

            if (this.all_players_passed != ds2.all_players_passed)
                return false;

            if (this.NS_down != ds2.NS_down || this.EW_down != ds2.EW_down)
                return false;
            if ((this.EW_up == null || this.EW_up.Length == 0) != (ds2.EW_up == null || ds2.EW_up.Length == 0))
                return false;
            if ((this.NS_up == null || this.NS_up.Length == 0) != (ds2.NS_up == null || ds2.NS_up.Length == 0))
                return false;

            if (this.EW_up != null && this.EW_up.Length != 0)
            {
                if (this.EW_up.Length != ds2.EW_up.Length)
                    return false;
                for (int i = 0; i < this.EW_up.Length; i++)
                {
                    if (this.EW_up[i] != ds2.EW_up[i])
                        return false;
                }
            }
            if (this.NS_up != null && this.NS_up.Length != 0)
            {
                if (this.NS_up.Length != ds2.NS_up.Length)
                    return false;
                for (int i = 0; i < this.NS_up.Length; i++)
                {
                    if (this.NS_up[i] != ds2.NS_up[i])
                        return false;
                }
            }
            return true;
        }

        public bool IsEmpty()
        {
            int ns_bonuses = 0, ew_bonuses = 0;
            if (NS_up != null)
                for (int i = 0; i < NS_up.Length; i++)
                    ns_bonuses += NS_up[i];
            if (EW_up != null)
                for (int i = 0; i < EW_up.Length; i++)
                    ew_bonuses += EW_up[i];


            if (NS_down == 0 && EW_down == 0 && ns_bonuses == 0 && ew_bonuses == 0 && !all_players_passed)
                return true;
            else
                return false;
        }
    }

    public class Robber
    {
        public ArrayOfDeals ScoreArray;
        public Robber()
        {
            this.ScoreArray = new ArrayOfDeals();
        }

        public void SetTotalScore(RobberScore rs)
        {
            bool bNS = false, bEW = false;
            int iNS = 0, iEW = 0;
            int iNS_total = 0, iEW_total = 0;
            for (int i = 0; i < ScoreArray.Count; i++)
            {
                // если сдача незавершена
                if (ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                {
                    rs.SetScore(iNS_total, iEW_total, false);
                    return;
                }

                iNS += ScoreArray[i].NS_down;
                iEW += ScoreArray[i].EW_down;
                iNS_total += ScoreArray[i].NS_down;
                if (ScoreArray[i].NS_up != null)
                    for (int j = 0; j < ScoreArray[i].NS_up.Length; j++)
                        iNS_total += ScoreArray[i].NS_up[j];
                iEW_total += ScoreArray[i].EW_down;
                if (ScoreArray[i].EW_up != null)
                    for (int j = 0; j < ScoreArray[i].EW_up.Length; j++)
                        iEW_total += ScoreArray[i].EW_up[j];

                if (iEW >= 100)
                {
                    if (bEW)
                    {
                        // Роббер завершен.
                        rs.SetScore(iNS_total, iEW_total, true);
                        return;
                    }
                    bEW = true;
                    iNS = iEW = 0;
                }
                else if (iNS >= 100)
                {
                    if (bNS)
                    {
                        // Роббер завершен.
                        rs.SetScore(iNS_total, iEW_total, true);
                        return;
                    }
                    bNS = true;
                    iNS = iEW = 0;
                }
            }

            // Роббер не завершен
            rs.SetScore(iNS_total, iEW_total, false);
        }

        public bool IsCompleted()
        {
            bool bNS = false, bEW = false;
            int iNS = 0, iEW = 0;
            for (int i = 0; i < ScoreArray.Count; i++)
            {
                // если сдача незавершена
                if(ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                    return false;

                iNS += ScoreArray[i].NS_down;
                iEW += ScoreArray[i].EW_down;
                if (iEW >= 100)
                {
                    if (bEW)
                    {
                        // Роббер завершен. Надо удалить нижние сдачи, если есть!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        return true;
                    }
                    bEW = true;
                    iNS = iEW = 0;
                }
                else if (iNS >= 100)
                {
                    if (bNS)
                    {
                        // Роббер завершен. Надо удалить нижние сдачи, если есть!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        return true;
                    }
                    bNS = true;
                    iNS = iEW = 0;
                }
            }
            return false;
        }

        public int WhereCompleted()
        {
            bool bNS = false, bEW = false;
            int iNS = 0, iEW = 0;
            for (int i = 0; i < ScoreArray.Count; i++)
            {
                // если сдача незавершена
                if (ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                    return -1;

                iNS += ScoreArray[i].NS_down;
                iEW += ScoreArray[i].EW_down;
                if (iEW >= 100)
                {
                    if (bEW)
                    {
                        return i;
                    }
                    bEW = true;
                    iNS = iEW = 0;
                }
                else if (iNS >= 100)
                {
                    if (bNS)
                    {
                        return i;
                    }
                    bNS = true;
                    iNS = iEW = 0;
                }
            }
            return -1;
        }

        public bool IsClearAfterGame(int robdealNo) //произошел ли гейм до этой сдачи, а после под чертой было пусто?
        {
            bool bNS = false, bEW = false;
            int iNS = 0, iEW = 0;
            for (int i = 0; i <= robdealNo; i++)
            {
                // если сдача незавершена
                if (ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                    return false;

                iNS += ScoreArray[i].NS_down;
                iEW += ScoreArray[i].EW_down;
                if (iEW >= 100)
                {
                    iNS = iEW = 0;
                    if (bEW)
                        break;
                    bEW = true;
                }
                else if (iNS >= 100)
                {
                    iNS = iEW = 0;
                    if (bNS)
                        break;
                    bNS = true;
                }
            }
            return ((bNS || bEW) && iNS == 0 && iEW == 0);
        }

        public bool DoesDealMakeGame(int robdealNo) //приносит ли эта сдача гейм?
        {
            bool bNS = false, bEW = false;
            int iNS = 0, iEW = 0;
            for (int i = 0; i <= robdealNo; i++)
            {
                // если сдача незавершена
                if (ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                    return false;

                iNS += ScoreArray[i].NS_down;
                iEW += ScoreArray[i].EW_down;
                if (iEW >= 100)
                {
                    if (i == robdealNo)
                        return true;
                    if (bEW)
                        break;
                    bEW = true;
                    iNS = iEW = 0;
                }
                else if (iNS >= 100)
                {
                    if (i == robdealNo)
                        return true;
                    if (bNS)
                        break;
                    bNS = true;
                    iNS = iEW = 0;
                }
            }
            return false;
        }

        public int GetValidDealsCount()
        {
            int valid_deals = 0;
            for (int i = 0; i < this.ScoreArray.Count; i++)
            {
                // если сдача незавершена?
                if (ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                    break;
                else
                    valid_deals++;
            }
            return valid_deals;
        }

        public int HowManyValidGames() //сколько полных геймов?
        {
            int games = 0;
            bool bNS = false, bEW = false;
            int iNS = 0, iEW = 0;
            for (int i = 0; i < this.ScoreArray.Count; i++)
            {
                // если сдача незавершена?
                if (ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                    break;

                iNS += ScoreArray[i].NS_down;
                iEW += ScoreArray[i].EW_down;
                if (iEW >= 100)
                {
                    games++;
                    if (bEW)
                        break;
                    bEW = true;
                    iNS = iEW = 0;
                }
                else if (iNS >= 100)
                {
                    games++;
                    if (bNS)
                        break;
                    bNS = true;
                    iNS = iEW = 0;
                }
            }
            return games;
        }

        public Zones WhatZone(int robdealNo)
        {
            if (robdealNo < 0 || robdealNo >= ScoreArray.Count)
                return Zones.NotDefinedYet;

            bool bNS = false, bEW = false;
            int iNS = 0, iEW = 0;
            for (int i = 0; i < robdealNo; i++)
            {
                // если сдача незавершена
                if (ScoreArray[i] == null || ScoreArray[i].IsEmpty())
                    return Zones.NotDefinedYet;

                iNS += ScoreArray[i].NS_down;
                iEW += ScoreArray[i].EW_down;
                if (iEW >= 100)
                {
                    if (bEW)
                    {
                        return Zones.NotDefinedYet; // - роббер завершен (EW 2 гейма)
                    }
                    bEW = true;
                    iNS = iEW = 0;
                }
                else if (iNS >= 100)
                {
                    if (bNS)
                    {
                        return Zones.NotDefinedYet; // - роббер завершен (NS 2 гейма)
                    }
                    bNS = true;
                    iNS = iEW = 0;
                }
            }

            if (bNS && bEW)
                return Zones.Both;
            else if (bNS)
                return Zones.NS;
            else if (bEW)
                return Zones.EW;
            else
                return Zones.None;
        }

        // Event 'Update'
        public class UpdateEventArgs : EventArgs
        {
            public int robNo;
            public UpdateEventArgs(int robNo)
            {
                this.robNo = robNo;
            }
        }
        public delegate void UpdateHandler(object sender, UpdateEventArgs e);
        public event UpdateHandler UpdateMe;
        public bool IsUpdateHandlers()
        {
            return (UpdateMe != null);
        }
        public void OnUpdate(object sender, UpdateEventArgs e)
        {
            if (UpdateMe != null)
                UpdateMe(sender, e);
        }

    }

    // Массив робберов
    public class ArrayOfRobbers : CollectionBase
    {
        public Robber this[int index]
        {
            get
            {
                return ((Robber)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(Robber value)
        {
            return (List.Add(value));
        }

        public int IndexOf(Robber value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, Robber value)
        {
            List.Insert(index, value);
        }

        public void Remove(Robber value)
        {
            List.Remove(value);
        }

        public bool Contains(Robber value)
        {
            return (List.Contains(value));
        }

        protected override void OnInsert(int index, Object value)
        { }

        protected override void OnRemove(int index, Object value)
        { }

        protected override void OnSet(int index, Object oldValue, Object newValue)
        { }

        protected override void OnValidate(Object value)
        {
            if (value.GetType() != typeof(Robber))
                throw new ArgumentException("value must be of type Robber", "value");
        }

        //........

        // Перевод общ#сдачи -> #роббера и #сдачи в этом роббере
        public int GetRobberNo(int line)
        {
            int deals = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (line < (deals + this[i].ScoreArray.Count))
                {
                    return i;
                }
                deals += this[i].ScoreArray.Count;
            }
            return -1;
        }

        public int GetRobDealNo(int line)
        {
            int deals = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (line < (deals + this[i].ScoreArray.Count))
                {
                    return (line - deals);
                }
                deals += this[i].ScoreArray.Count;
            }
            return -1;
        }

        // С какого общ#сдачи начался роббер
        public int GetFirstLineOfRobber(int robNo)
        {
            int deals = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (i == robNo)
                    return deals;
                deals += this[i].ScoreArray.Count;
            }
            return -1;
        }

        // Определение зоны
        public Zones WhatZone(int line)
        {
            if (GetRobberNo(line) != -1)
            {
                return this[GetRobberNo(line)].WhatZone(GetRobDealNo(line));
            }
            else
            {
                return Zones.NotDefinedYet;
            }
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------

    public partial class RobberControl : Control
    {
        Robber m_robber;

        Size offset;
        Size space;
        int height_Header;
        int spaceV_Header;
        int spaceV_Boundary;
        SizeF size1;
        SolidBrush brushText;
        SolidBrush brushHeader; 
        Font fontHeader;

        public RobberControl()
        {
            InitializeComponent();
            this.brushText = new SolidBrush(Color.Black);
            this.brushHeader = new SolidBrush(Color.Red);
            this.fontHeader = new Font(this.Font.Name, this.Font.Size, FontStyle.Bold);

            m_robber = null;

            offset = new Size(0, 0); // отступ от клиентской области
            space = new Size(1, -1);  // отступ между линиями по Y и расстояние до верт.разделителя по X
            spaceV_Boundary = 2; // расстояние до гориз.черты (отступ через черту = 2 * spaceV_Boundary + 1)
            size1 = this.CreateGraphics().MeasureString("00000", this.Font);
            size1.Width = (this.Width - offset.Width * 2 - space.Width * 2 - 1) / 2;
            height_Header = (int) this.CreateGraphics().MeasureString("NS", this.fontHeader).Height;
            spaceV_Header = 4; // расстояние между заголовком и началом линий счета
        }

        public void AttachData(Robber r)
        {
            if(m_robber == r)
                return;

            // detach old
            if(m_robber != null)
            {
                m_robber.UpdateMe -= OnRobberUpdate;
                m_robber = null;
            }

            // attach new
            m_robber = r;
            m_robber.UpdateMe += OnRobberUpdate;

            this.Invalidate();
        }

        public void DetachData()
        {
            if (m_robber != null)
            {
                m_robber.UpdateMe -= OnRobberUpdate;
                m_robber = null;

                this.Invalidate();
            }
        }

        void OnRobberUpdate(object sender, Robber.UpdateEventArgs e)
        {
            //if(e.robNo == m_robber.) ????????????????? 
                this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (m_robber == null)
                return;

            Graphics g = pe.Graphics;
            Rectangle rf1 = new Rectangle();
            RectangleF rf2 = new RectangleF();
            Pen pen_Black = new Pen(Color.Black);
            Pen pen_BorderBetween = new Pen(Color.Blue);

            int ValidDealsCount = m_robber.GetValidDealsCount();
            int CompletedDealsCount = m_robber.WhereCompleted() + 1;
            if (m_robber.WhereCompleted() != -1 && CompletedDealsCount < ValidDealsCount)
                ValidDealsCount = CompletedDealsCount;

            // V Разделитель
            int bonus_lines = 0, bonus_lines_ns = 0, bonus_lines_ew = 0;
            for (int i = 0; i < ValidDealsCount; i++)
            {
                if (m_robber.ScoreArray[i].NS_up != null && m_robber.ScoreArray[i].NS_up.Length != 0)
                    bonus_lines_ns += m_robber.ScoreArray[i].NS_up.Length;
                if (m_robber.ScoreArray[i].EW_up != null && m_robber.ScoreArray[i].EW_up.Length != 0)
                    bonus_lines_ew += m_robber.ScoreArray[i].EW_up.Length;
            }
            bonus_lines += (bonus_lines_ns > bonus_lines_ew ? bonus_lines_ns : bonus_lines_ew);

            int usual_lines = 0, usual_lines_ns = 0, usual_lines_ew = 0;
            int iNS = 0, iEW = 0;
            for (int i = 0; i < ValidDealsCount; i++)
            {
                if (m_robber.ScoreArray[i].EW_down != 0)
                {
                    usual_lines_ew++;
                    iEW += m_robber.ScoreArray[i].EW_down;
                }
                else if (m_robber.ScoreArray[i].NS_down != 0)
                {
                    usual_lines_ns++;
                    iNS += m_robber.ScoreArray[i].NS_down;
                }

                if (iNS >= 100 || iEW >= 100)
                {
                    usual_lines += (usual_lines_ns > usual_lines_ew ? usual_lines_ns : usual_lines_ew);
                    usual_lines_ew = usual_lines_ns = 0;
                    iNS = iEW = 0;
                }
            }
            usual_lines += (usual_lines_ns > usual_lines_ew ? usual_lines_ns : usual_lines_ew);

            int total_lines = bonus_lines + usual_lines;
            int separators = 1 + m_robber.HowManyValidGames();
            bool is_main_separator_half = (usual_lines == 0 || bonus_lines == 0);
            bool is_closing_separator_half = (ValidDealsCount > 0 ? m_robber.IsClearAfterGame(ValidDealsCount - 1) : false);
            int total_height = total_lines * (int)size1.Height + separators * (1 + 2 * spaceV_Boundary);
            if (m_robber.IsCompleted())
                total_height -= spaceV_Boundary;

            if (total_lines > 0)
                total_height += (total_lines - 1 - separators + (is_main_separator_half ? 1 : 0) + (is_closing_separator_half ? 1 : 0)) * space.Height;

            // Посчитать, влезают ли все очки по вертикали:
            bool use_compact_bonuses = false; // сжать бонусы?
            int robber_line_full_height = offset.Height * 2 + (height_Header + spaceV_Header) + total_height; //полная высота
            if (robber_line_full_height > ClientSize.Height)
                use_compact_bonuses = true;
            if (bonus_lines == 0)
                use_compact_bonuses = false;

            // Сжать бонусы
            if (use_compact_bonuses)
            {
                bonus_lines = 1;
                total_lines = bonus_lines + usual_lines;
                total_height = total_lines * (int)size1.Height + separators * (1 + 2 * spaceV_Boundary);
                if (m_robber.IsCompleted())
                    total_height -= spaceV_Boundary;
                if (total_lines > 0)
                    total_height += (total_lines - 1 - separators + (is_main_separator_half ? 1 : 0) + (is_closing_separator_half ? 1 : 0)) * space.Height;
            }
            
            g.DrawLine(pen_Black, offset.Width + (int)size1.Width + 1 * space.Width, offset.Height + (height_Header + spaceV_Header), offset.Width + (int)size1.Width + 1 * space.Width, offset.Height + (height_Header + spaceV_Header) + total_height - 1); //добавить +1 за толстую черту
            

            // заголовок: NS и EW
            rf1.X = offset.Width;
            rf1.Y = offset.Height;
            rf1.Width = (int)size1.Width;
            rf1.Height = height_Header;
            rf2 = SmallHelper.StringInMiddle(g, rf1, "NS", this.fontHeader);
            g.DrawString("NS", this.fontHeader, this.brushHeader, rf2);

            rf1.X = offset.Width + (int)size1.Width + 2 * space.Width + 1;
            rf2 = SmallHelper.StringInMiddle(g, rf1, "EW", this.fontHeader);
            g.DrawString("EW", this.fontHeader, this.brushHeader, rf2);


            
            // Бонусы
            if (!use_compact_bonuses)
            {
                bonus_lines_ns = 0;
                bonus_lines_ew = 0;
                rf1.Width = (int)size1.Width;
                rf1.Height = (int)size1.Height;
                for (int i = 0; i < ValidDealsCount; i++)
                {
                    if (m_robber.ScoreArray[i].NS_up != null && m_robber.ScoreArray[i].NS_up.Length != 0)
                    {
                        for (int j = 0; j < m_robber.ScoreArray[i].NS_up.Length; j++)
                        {
                            bonus_lines_ns++;
                            rf1.X = offset.Width;
                            rf1.Y = offset.Height + (height_Header + spaceV_Header) + (bonus_lines - bonus_lines_ns) * ((int)size1.Height + space.Height);
                            rf2 = SmallHelper.StringInMiddle(g, rf1, m_robber.ScoreArray[i].NS_up[j].ToString(), this.Font);
                            g.DrawString(m_robber.ScoreArray[i].NS_up[j].ToString(), this.Font, this.brushText, rf2);
                        }
                    }
                    if (m_robber.ScoreArray[i].EW_up != null && m_robber.ScoreArray[i].EW_up.Length != 0)
                    {
                        for (int j = 0; j < m_robber.ScoreArray[i].EW_up.Length; j++)
                        {
                            bonus_lines_ew++;
                            rf1.X = offset.Width + (int)size1.Width + 2 * space.Width + 1;
                            rf1.Y = offset.Height + (height_Header + spaceV_Header) + (bonus_lines - bonus_lines_ew) * ((int)size1.Height + space.Height);
                            rf2 = SmallHelper.StringInMiddle(g, rf1, m_robber.ScoreArray[i].EW_up[j].ToString(), this.Font);
                            g.DrawString(m_robber.ScoreArray[i].EW_up[j].ToString(), this.Font, this.brushText, rf2);
                        }
                    }
                }
            }
            else
            {
                int total_bonuses_NS = 0, total_bonuses_EW = 0;
                for (int i = 0; i < ValidDealsCount; i++)
                {
                    if (m_robber.ScoreArray[i].NS_up != null && m_robber.ScoreArray[i].NS_up.Length != 0)
                    {
                        for (int j = 0; j < m_robber.ScoreArray[i].NS_up.Length; j++)
                        {
                            total_bonuses_NS += m_robber.ScoreArray[i].NS_up[j];
                        }
                    }
                    if (m_robber.ScoreArray[i].EW_up != null && m_robber.ScoreArray[i].EW_up.Length != 0)
                    {
                        for (int j = 0; j < m_robber.ScoreArray[i].EW_up.Length; j++)
                        {
                            total_bonuses_EW += m_robber.ScoreArray[i].EW_up[j];
                        }
                    }
                }

                rf1.Width = (int)size1.Width;
                rf1.Height = (int)size1.Height;
                rf1.X = offset.Width;
                rf1.Y = offset.Height + (height_Header + spaceV_Header);
                rf2 = SmallHelper.StringInMiddle(g, rf1, total_bonuses_NS.ToString(), this.Font);
                g.DrawString(total_bonuses_NS.ToString(), this.Font, this.brushText, rf2);
                rf1.X = offset.Width + (int)size1.Width + 2 * space.Width + 1;
                rf2 = SmallHelper.StringInMiddle(g, rf1, total_bonuses_EW.ToString(), this.Font);
                g.DrawString(total_bonuses_EW.ToString(), this.Font, this.brushText, rf2);

            }


            // Черта между бонусами и основным счетом:
            int yBorder = offset.Height + (height_Header + spaceV_Header) + spaceV_Boundary;
            if(bonus_lines > 0)
                yBorder += bonus_lines * (int)size1.Height + (bonus_lines - 1) * space.Height;
            g.DrawLine(pen_BorderBetween, offset.Width, yBorder, this.Width - offset.Width - 1, yBorder);
            //g.DrawLine(pen_Black, offset.Width, yBorder + 1, this.Width - offset.Width - 1, yBorder + 1); //тостая черта

            // Основные очки
            usual_lines = 0;
            usual_lines_ns = 0;
            usual_lines_ew = 0;
            iNS = 0;
            iEW = 0;
            separators = 1;
            for (int i = 0; i < ValidDealsCount; i++)
            {
                if (m_robber.ScoreArray[i].NS_down != 0)
                {
                    iNS += m_robber.ScoreArray[i].NS_down;
                    usual_lines_ns++;

                    rf1.X = offset.Width;
                    rf1.Y = offset.Height + (height_Header + spaceV_Header) + (bonus_lines + usual_lines + usual_lines_ns - 1) * (int)size1.Height + separators * (1 + 2 * spaceV_Boundary);
                    //rf1.Y++; // для толстой черты
                    if (bonus_lines + usual_lines + usual_lines_ns > 0)
                        rf1.Y += (bonus_lines + usual_lines + usual_lines_ns - 1 - separators + (is_main_separator_half ? 1 : 0)) * space.Height;
                    rf2 = SmallHelper.StringInMiddle(g, rf1, m_robber.ScoreArray[i].NS_down.ToString(), this.Font);
                    g.DrawString(m_robber.ScoreArray[i].NS_down.ToString(), this.Font, this.brushText, rf2);
                }
                else if (m_robber.ScoreArray[i].EW_down != 0)
                {
                    iEW += m_robber.ScoreArray[i].EW_down;
                    usual_lines_ew++;

                    rf1.X = offset.Width + (int)size1.Width + 2 * space.Width + 1;
                    rf1.Y = offset.Height + (height_Header + spaceV_Header) + (bonus_lines + usual_lines + usual_lines_ew - 1) * (int)size1.Height + separators * (1 + 2 * spaceV_Boundary);
                    //rf1.Y++; // для толстой черты
                    if (bonus_lines + usual_lines + usual_lines_ew > 0)
                        rf1.Y += (bonus_lines + usual_lines + usual_lines_ew - 1 - separators + (is_main_separator_half ? 1 : 0)) * space.Height;
                    rf2 = SmallHelper.StringInMiddle(g, rf1, m_robber.ScoreArray[i].EW_down.ToString(), this.Font);
                    g.DrawString(m_robber.ScoreArray[i].EW_down.ToString(), this.Font, this.brushText, rf2);
                }



                if (iEW >= 100 || iNS >= 100)
                {
                    usual_lines += (usual_lines_ns > usual_lines_ew ? usual_lines_ns : usual_lines_ew);
                    usual_lines_ew = usual_lines_ns = 0;
                    iNS = iEW = 0;
                   
                    // Черта после гейма:
                    yBorder = offset.Height + (height_Header + spaceV_Header) + (bonus_lines + usual_lines) * (int)size1.Height + separators * (1 + 2 * spaceV_Boundary) + spaceV_Boundary;
                    //yBorder++; // для толстой черты
                    if (bonus_lines + usual_lines > 0)
                        yBorder += (bonus_lines + usual_lines - 1 - separators + (is_main_separator_half ? 1 : 0)) * space.Height;
                    g.DrawLine(pen_Black, offset.Width, yBorder, this.Width - offset.Width - 1, yBorder);

                    separators++;
                }
            }

            pen_Black.Dispose();

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            size1.Width = (this.Width - offset.Width * 2 - space.Width * 2 - 1) / 2;
        }

    }

    // --------------------------------------------------------------------------------------------------------------------------------------------

    public struct DealScore
    {
        public DealScore(int ns, int ew)
        {
            this.NS = ns;
            this.EW = ew;
        }
        public int NS;
        public int EW;

        public bool IsEmpty()
        {
            return (NS == 0 && EW == 0);
        }
        public void MakeEmpty()
        {
            NS = 0;
            EW = 0;
        }
        public Pairs Winner
        {
            get
            {
                if (IsEmpty() || (NS == EW))
                    return Pairs.NotDefinedYet;
                else if (NS > EW)
                    return Pairs.NS;
                else
                    return Pairs.EW;
            }
        }

        public override string ToString()
        {
            return (NS + " : " + EW);
        }
    }

    public interface IMatchCompleted
    {
        bool IsCompleted();
    }

    public interface IMatchGetScore
    {
        DealScore GetScore();
    }


    public class DealScore_CLASS : BaseChangedData
    {
        protected DealScore val;

        public override string ToString()
        {
            return val.ToString();
        }

        public override bool IsDefined()
        {
            return true;
            //return (!val.IsEmpty());
        }

        public override void Clear()
        {
            val.MakeEmpty();

            if (IsChangedHandlers())
                OnChanged(null, null);
        }

        public void SetScore(int ns, int ew)
        {
            val.NS = ns;
            val.EW = ew;

            if (IsChangedHandlers())
                OnChanged(null, null);
        }

    }


    public class RobberScore : BaseChangedData, IMatchCompleted, IMatchGetScore
    {
        int robNo;
        public RobberScore(int robNo)
        {
            this.robNo = robNo;
        }

        DealScore score;
        bool robCompleted;
        public void SetScore(int iNS, int iEW, bool bCompl)
        {
            DealScore old = score;
            bool oldCompleted = robCompleted;

            this.score.NS = iNS;
            this.score.EW = iEW;
            this.robCompleted = bCompl;

            if ((!old.Equals(this.score) || !oldCompleted.Equals(this.robCompleted)) && IsChangedHandlers())
            {
                OnChanged(this, null);
                //OnChanged(this, new ChangedEventsArgs(old, this.score));
                //OnChanged(this, new ChangedEventsArgs(oldCompleted, this.robCompleted));
            }
        }
        public bool IsCompleted()
        {
            return robCompleted;
        }

        public override String ToString()
        {
            String s = "Роббер №" + (robNo + 1).ToString();
            //if(robCompleted && score.Winner != Pairs.NotDefinedYet)
                s += "\n" + score.NS.ToString() + " : " + score.EW.ToString();
            return s;
        }

        public override bool IsDefined()
        {
            return (robNo >= 0);
        }

        // Clear
        public override void Clear()
        {
            SetScore(0, 0, false);
        }

        public DealScore GetScore()
        {
            if (!IsCompleted())
                return new DealScore(0, 0);
            else
                return score;
        }

    }


    public class MatchScore : BaseChangedData, IMatchCompleted, IMatchGetScore
    {
        int matchNo;
        int dealsInMatch;
        public MatchScore(int matchNo, int dealsInMatch)
        {
            this.matchNo = matchNo;
            this.dealsInMatch = dealsInMatch;
        }

        DealScore score;
        DealScore score_VP;
        bool matchCompleted;
        public void SetScore(int iNS, int iEW, bool bCompl)
        {
            DealScore old = score;
            bool oldCompleted = matchCompleted;

            this.score.NS = iNS;
            this.score.EW = iEW;
            this.matchCompleted = bCompl;
            if (matchCompleted)
            {
                score_VP = BridgeGameScoring.IMPtoVP(score, dealsInMatch);
            }
            else
            {
                score_VP = new DealScore(0, 0);
            }

            if ((!old.Equals(this.score) || !oldCompleted.Equals(this.matchCompleted)) && IsChangedHandlers())
            {
                OnChanged(this, null);
                //OnChanged(this, new ChangedEventsArgs(old, this.score));
                //OnChanged(this, new ChangedEventsArgs(oldCompleted, this.matchCompleted));
            }
        }
        public bool IsCompleted()
        {
            return matchCompleted;
        }

        public override String ToString()
        {
            String s = "Матч №" + (matchNo + 1).ToString();
            if(matchCompleted && !score_VP.IsEmpty())
                s += "\nVP " + score_VP.NS.ToString() + " : " + score_VP.EW.ToString() + "  (" + score.NS.ToString() + " : " + score.EW.ToString() + ")";
            else
                s += "\n" + score.NS.ToString() + " : " + score.EW.ToString();
            return s;
        }

        public override bool IsDefined()
        {
            return (matchNo >= 0);
        }

        // Clear
        public override void Clear()
        {
            SetScore(0, 0, false);
        }


        public DealScore GetScore()
        {
            if (!IsCompleted())
                return new DealScore(0, 0);
            else
                return score_VP;
        }


    }

}



