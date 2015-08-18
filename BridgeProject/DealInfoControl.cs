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
    public class CardsDistribution : BaseChangedData, ISQLSerialize
    {
        byte[] d;
        public CardsDistribution()
        {
            this.d = new byte[20];
        }
        public CardsDistribution(CardsDistribution cd)
        {
            this.d = (byte[]) cd.d.Clone();
        }
        public void CopyFrom(CardsDistribution cd)
        {
            this.d = (byte[]) cd.d.Clone();

            if(IsChangedHandlers())
                this.OnChanged(this, null);
        }
        override public void Clear()
        {
            this.d = new byte[20];

            if (IsChangedHandlers())
                this.OnChanged(this, null);
        }

        private bool getbyte(int i)
        {
            return (d[i / (8 * sizeof(byte))] & (1 << (i % (8 * sizeof(byte))))) != 0;
        }
        private void setbyte(int i, bool val)
        {
            if(val == true)
                d[i / (8 * sizeof(byte))] |= (byte)(1 << (i % (8 * sizeof(byte))));
            else
                d[i / (8 * sizeof(byte))] &= (byte)~(1 << (i % (8 * sizeof(byte))));
        }

        public Quarters Get(CardValue v, CardSuit s)
        {
            int cardNo = ((int)v - 2) * 4 + (s == CardSuit.Hearts ? 0 : (s == CardSuit.Diamonds ? 1 : (s == CardSuit.Clubs ? 2 : 3))); // от 0 до 51
            if (getbyte(cardNo) == false)
            {
                return Quarters.NotDefinedYet;
            }
            else if (getbyte(52 + cardNo) == true)
            {
                if(getbyte(52 + 52 + cardNo) == true)
                    return Quarters.N;
                else
                    return Quarters.S;
            }
            else
            {
                if(getbyte(52 + 52 + cardNo) == true)
                    return Quarters.E;
                else
                    return Quarters.W;
            }
        }

        public void Set(CardValue v, CardSuit s, Quarters val)
        {
            Quarters old = Get(v, s);

            int cardNo = ((int)v - 2) * 4 + (s == CardSuit.Hearts ? 0 : (s == CardSuit.Diamonds ? 1 : (s == CardSuit.Clubs ? 2 : 3))); // от 0 до 51
            if (val == Quarters.NotDefinedYet)
            {
                setbyte(cardNo, false);
            }
            else if (GetCount(val) < 13)
            {
                setbyte(cardNo, true);
                setbyte(52 + cardNo, (val == Quarters.N || val == Quarters.S));
                setbyte(52 + 52 + cardNo, (val == Quarters.N || val == Quarters.E));
            }

            if (old != val && IsOneCardDistrChangedHandlers())
                this.OnOneCardDistrChanged(this, new OneCardDistrChangedEventsArgs(new Card(v, s), old, val));

            if(old != val && IsChangedHandlers())
                this.OnChanged(this, null);
        }

        public int GetCount(Quarters q)
        {
            int count = 0;
            for (CardValue v = CardValue.Two; v <= CardValue.Ace; v++)
            {
                for (CardSuit s = CardSuit.Hearts; s <= CardSuit.Spades; s++)
                {
                    if (Get(v, s) == q)
                        count++;
                }
            }
            return count;
        }

        public bool CanMeAutocompletePlease(out Quarters q)
        {
            bool complN = (GetCount(Quarters.N) == 13);
            bool complS = (GetCount(Quarters.S) == 13);
            bool complE = (GetCount(Quarters.E) == 13);
            bool complW = (GetCount(Quarters.W) == 13);

            int compl = (complN ? 1 : 0) + (complS ? 1 : 0) + (complE ? 1 : 0) + (complW ? 1 : 0);

            if (compl == 3)
            {
                if (complN == false)
                    q = Quarters.N;
                else if (complS == false)
                    q = Quarters.S;
                else if (complE == false)
                    q = Quarters.E;
                else
                    q = Quarters.W;
                return true;
            }
            else
            {
                q = Quarters.NotDefinedYet;
                return false;
            }
        }

        public void AutoCompleteMePlease(Quarters q)
        {
            for (CardValue v = CardValue.Two; v <= CardValue.Ace; v++)
            {
                for (CardSuit s = CardSuit.Hearts; s <= CardSuit.Spades; s++)
                {
                    if (Get(v, s) == Quarters.NotDefinedYet)
                    {
                        Set(v, s, q);
                    }
                }
            }
        }

        public override bool IsDefined()
        {
            for (int i = 0; i < 52; i++)
            {
                if (getbyte(i) == false)
                    return false;
            }
            if (GetCount(Quarters.N) != 13 || GetCount(Quarters.S) != 13 || GetCount(Quarters.E) != 13 || GetCount(Quarters.W) != 13)
            {
                return false;
            }
            return true;
        }


        // Сериализация SQL
/*        public void _FromDataBase(object v)
        {
            if (v == DBNull.Value)
            {
                this.Clear();
            }
            else
            {
                char[] arr = ((string)v).ToCharArray();

                
                string s = "from db: ";
                for (int i = 0; i < 10; i++)
                {
                    s += "[" + (short)arr[i] + "] ";
                }
                MessageBox.Show(s);



                this.d = new byte[20];
                for (int i = 0; i < arr.Length && i < 10; i++)
                {
                    this.d[i * 2] = (byte)arr[i];
                    this.d[i * 2 + 1] = (byte)(arr[i] >> 8);
                }

                if (IsChangedHandlers())
                    this.OnChanged(this, null);
            }
        }
        public object _ToDataBase()
        {
            if (!this.IsDefined())
                return DBNull.Value;
            else
            {
                char[] v = new char[10];
                for (int i = 0; i < v.Length; i++)
                    v[i] = (char)(this.d[i * 2] + (this.d[i * 2 + 1] << 8));


                string s = "to db: ";
                for (int i = 0; i < 10; i++)
                {
                    s += "[" + (short)v[i] + "] ";
                }
                MessageBox.Show(s);



                return new string(v);
            }
        }*/




        public void _FromDataBase(object v)
        {
            if (v == DBNull.Value)
            {
                this.Clear();
            }
            else
            {
                this.d = (byte[]) ((byte[])v).Clone();

                if (IsChangedHandlers())
                    this.OnChanged(this, null);
            }
        }
        public object _ToDataBase()
        {
            /*if (!this.IsDefined())
            {
                return DBNull.Value;
            }
            else*/
            {
                return this.d.Clone();
            }
        }



        // event OneCardChanged:
        public class OneCardDistrChangedEventsArgs : EventArgs
        {
            public Card card;
            public Quarters old_q;
            public Quarters new_q;
            public OneCardDistrChangedEventsArgs(Card card, Quarters old_q, Quarters new_q)
            {
                this.card = card;
                this.old_q = old_q;
                this.new_q = new_q;
            }
        }
        public delegate void OneCardDistrChangedHandler(object sender, OneCardDistrChangedEventsArgs e);
        public event OneCardDistrChangedHandler OneCardDistrChanged;
        public bool IsOneCardDistrChangedHandlers()
        {
            return (OneCardDistrChanged != null);
        }
        public void RemoveOneCardDistrChangedHandlers()
        {
            OneCardDistrChanged = null;
        }
        public void OnOneCardDistrChanged(object sender, OneCardDistrChangedEventsArgs e)
        {
            if (OneCardDistrChanged != null)
                OneCardDistrChanged(sender, e);
        }
    }

    // ---------------------------------------------------------------------------------------------------

    public partial class DealInfoControl : BaseControlInTable, IDetachData
    {
        protected bool m_bSplit;
        bool m_bSub2;
        bool m_bLock1;
        bool m_bLock2;

        IntData m_dealNo;
        CardsDistribution m_cardsDistribution;
        QuarterSwitcher m_quarterDealer;
        public void AttachData(IntData dealNo, CardsDistribution cardsDistribution)
        {
            if (m_dealNo == null)
            {
                m_dealNo = dealNo;
                m_dealNo.Changed += OnValue1Changed;

                OnValue1Changed(this, null);
            }
            if (m_cardsDistribution == null)
            {
                m_cardsDistribution = cardsDistribution;
                m_cardsDistribution.Changed += OnValue2Changed;

                OnValue2Changed(this, null);
            }

            ContextMenuOff();
            ContextMenuOn();
        }
        public void AttachData(IntData dealNo, CardsDistribution cardsDistribution, QuarterSwitcher qDealer)
        {
            if (m_dealNo == null)
            {
                m_dealNo = dealNo;
                m_dealNo.Changed += OnValue1Changed;

                OnValue1Changed(this, null);
            }
            if (m_cardsDistribution == null)
            {
                m_cardsDistribution = cardsDistribution;
                m_cardsDistribution.Changed += OnValue2Changed;

                OnValue2Changed(this, null);
            }
            if (m_quarterDealer == null)
            {
                m_quarterDealer = qDealer;
                m_quarterDealer.Changed += OnValue3Changed;

                OnValue3Changed(this, null);
            }

            ContextMenuOff();
            ContextMenuOn();
        }
        public void DetachData(bool _inv)
        {
            if (m_dealNo != null)
            {
                m_dealNo.Changed -= OnValue1Changed;
                m_dealNo = null;

                if(_inv)
                    OnValue1Changed(this, null);
            }
            if (m_cardsDistribution != null)
            {
                m_cardsDistribution.Changed -= OnValue2Changed;
                m_cardsDistribution = null;

                if(_inv)
                    OnValue2Changed(this, null);
            }
            if (m_quarterDealer != null)
            {
                m_quarterDealer.Changed -= OnValue3Changed;
                m_quarterDealer = null;

                if(_inv)
                    OnValue3Changed(this, null);
            }

            ContextMenuOff();
        }

        void ContextMenuOn()
        {
            if (m_cardsDistribution == null && m_quarterDealer == null)
                return;

            this.ContextMenu = new ContextMenu();
            MenuItem mi;
            if (m_quarterDealer != null)
            {
                mi = new MenuItem();
                mi.Text = "Сдающий: " + m_quarterDealer.ToString();
                this.ContextMenu.MenuItems.Add(mi);
            }
            if (m_quarterDealer != null && m_cardsDistribution != null)
            {
                mi = new MenuItem();
                mi.Text = "-";
                this.ContextMenu.MenuItems.Add(mi);
            }
            if (m_cardsDistribution != null)
            {
                mi = new MenuItem();
                mi.Text = "Сдача";
                mi.Click += OnMenu0;
                this.ContextMenu.MenuItems.Add(mi);
                mi = new MenuItem();
                mi.Text = "Изменить сдачу";
                mi.Click += OnMenu1;
                this.ContextMenu.MenuItems.Add(mi);
                mi = new MenuItem();
                mi.Text = "Удалить сдачу";
                mi.Click += OnMenu2;
                this.ContextMenu.MenuItems.Add(mi);

                //this.ContextMenu.MenuItems[m_quarterDealer == null ? 0 : 2].Click += OnMenu0;
                //this.ContextMenu.MenuItems[m_quarterDealer == null ? 1 : 3].Click += OnMenu1;
                //this.ContextMenu.MenuItems[m_quarterDealer == null ? 2 : 4].Click += OnMenu2;
            }
            if (m_quarterDealer != null || m_cardsDistribution != null)
            {
                mi = new MenuItem();
                mi.Text = "-";
                this.ContextMenu.MenuItems.Add(mi);
            }

            //mi for lock
            {
                mi = new MenuItem();
                mi.Text = (!m_bSplit ? "Lock" : "Lock 1");
                mi.Checked = m_bLock1;
                mi.Click += OnMenu_Lock1;
                this.ContextMenu.MenuItems.Add(mi);

                if(m_bSplit && m_bSub2)
                {
                    mi = new MenuItem();
                    mi.Text = "Lock 2";
                    mi.Checked = m_bLock2;
                    mi.Click += OnMenu_Lock2;
                    this.ContextMenu.MenuItems.Add(mi);
                }
            }
            this.ContextMenu.Popup += OnMenuPopup;
        }

        void OnMenuPopup(object sender, EventArgs e)
        {
            if (m_cardsDistribution != null)
            {
                this.ContextMenu.MenuItems[m_quarterDealer == null ? 2 : 4].Enabled = m_cardsDistribution.IsDefined();
            }
        }

        void ContextMenuOff()
        {
            if (this.ContextMenu != null)
            {
                if (this.ContextMenu.MenuItems != null)
                {
                    for (int i = 0; i < this.ContextMenu.MenuItems.Count; i++)
                    {
                        this.ContextMenu.MenuItems[i].Dispose();
                    }
                }
                this.ContextMenu.MenuItems.Clear();
                this.ContextMenu.Dispose();
                this.ContextMenu = null;
            }
        }
        public void OnValue1Changed(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            this.Invalidate();
        }
        public void OnValue2Changed(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            this.Invalidate();
        }
        public void OnValue3Changed(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            //изменить в меню сдающего:
            if(this.ContextMenu != null && this.ContextMenu.MenuItems.Count != 0)
                this.ContextMenu.MenuItems[0].Text = "Сдающий: " + (m_quarterDealer != null && m_quarterDealer.IsDefined() ? m_quarterDealer.ToString() : "?");
        }

        Rectangle m_rect_BorderBounds;
        int offset_x = 3;
        StringAlignment align_x = StringAlignment.Center;
        StringAlignment align_x2 = StringAlignment.Center;
        SolidBrush m_brush_String = new SolidBrush(SystemColors.ControlText);
        SolidBrush m_brush_String_Def = new SolidBrush(Color.Blue);
        SolidBrush m_brush_UNLOCKED = new SolidBrush(Color.LightGreen);
        Font FontBold;


        public DealInfoControl()
        {
            InitializeComponent();

            m_bSplit = false; //todo: version for 2-split
            m_bSub2 = false; //sub2 is not started by default
            m_bLock1 = true;
            m_bLock2 = true;

            m_dealNo = null;
            m_cardsDistribution = null;
            m_quarterDealer = null;

            bMousePressed = false;
            this.FontBold = new Font(this.Font.Name, this.Font.Size, FontStyle.Bold);
            DefineHighlight(5, Color.Orange);
        }

        Pen[] highlightPens;
        public void DefineHighlight(int count, Color highlightColor)
        {
            highlightPens = new Pen[count];
            for (int i = 0; i < count; i++)
            {
                highlightPens[i] = new Pen(Color.FromArgb(highlightColor.R + (this.BackColor.R - highlightColor.R) / (count + 2) * (i + 2), highlightColor.G + (this.BackColor.G - highlightColor.G) / (count + 2) * (i + 2), highlightColor.B + (this.BackColor.B - highlightColor.B) / (count + 2) * (i + 2)));
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            m_rect_BorderBounds = new Rectangle(0, 0, this.Width, this.Height);
        }
        bool bMousePressed;
        bool bMouseOver;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            bMousePressed = true;
            bMouseOver = true;
            this.Focus();
            this.Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!bMousePressed)
                return;

            bool old_bMouseOver = bMouseOver;
            bMouseOver = m_rect_BorderBounds.Contains(e.X, e.Y);
            if (old_bMouseOver != bMouseOver)
            {
                this.Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!bMousePressed)
                return;
            else
                bMousePressed = false;

            this.Invalidate();
            if (m_rect_BorderBounds.Contains(e.X, e.Y))
            {
                if (this.ContextMenu != null)
                {
                    this.ContextMenu.Show(this, new Point(0, this.Height));
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            // Закраска для обозначения блокировки
            if(!m_bLock1)
            {
                if (!m_bSplit)
                {
                    SmallHelper.FillRectInside(g, m_brush_UNLOCKED, m_rect_BorderBounds);
                }
                else
                {
                    SmallHelper.FillRectInside(g, m_brush_UNLOCKED, new Rectangle(0, 0, m_rect_BorderBounds.Width, (m_rect_BorderBounds.Height % 2 == 0 ? m_rect_BorderBounds.Height + 1 : m_rect_BorderBounds.Height) / 2 + 1));
                }
            }
            if (m_bSplit && m_bSub2 && !m_bLock2)
            {
                SmallHelper.FillRectInside(g, m_brush_UNLOCKED, new Rectangle(0, (m_rect_BorderBounds.Height % 2 == 0 ? m_rect_BorderBounds.Height + 1 : m_rect_BorderBounds.Height) / 2, m_rect_BorderBounds.Width, (m_rect_BorderBounds.Height % 2 == 0 ? m_rect_BorderBounds.Height + 1 : m_rect_BorderBounds.Height) / 2 + (m_rect_BorderBounds.Height % 2 == 0 ? 0 : 1)));
            }


            // Обводка, если нажата мышка
            if (bMousePressed && bMouseOver)
            {
                Rectangle r = m_rect_BorderBounds;
                for (int i = 0; i < highlightPens.Length; i++)
                {
                    SmallHelper.DrawRect(g, highlightPens[i], r);
                    r.X++;
                    r.Y++;
                    r.Width -= 2;
                    r.Height -= 2;
                }
            }

            // Текст
            if (m_dealNo != null && m_dealNo.IsDefined())
            {
                bool distributionDefined = (this.m_cardsDistribution != null && this.m_cardsDistribution.IsDefined());
                Rectangle m_rect_BorderBounds = new Rectangle(0, 0, this.Width, this.Height);
                SmallHelper.DrawMultiString(g, m_rect_BorderBounds, this.m_dealNo.ToString(), distributionDefined ? this.FontBold : this.Font, distributionDefined ? this.m_brush_String_Def : this.m_brush_String, align_x, align_x2, StringAlignment.Center, offset_x, 0, 0);
            }
        }


        void OpenCardsDistributionForm()
        {
            if (this.m_cardsDistribution == null)
                return;

            CardsDistributionForm form = new CardsDistributionForm(this.m_cardsDistribution);
            DialogResult dres = form.ShowDialog();
            if (dres == DialogResult.OK)
            {
                this.m_cardsDistribution.CopyFrom(form.GetCD());
            }
            form.Dispose();
        }

        void OpenCardsDistributionShowForm()
        {
            if (this.m_cardsDistribution == null)
                return;

            CardsDistributionShowForm form = new CardsDistributionShowForm(this.m_cardsDistribution);
            form.ShowDialog();
        }

        void ClearCardsDistribution()
        {
            if (this.m_cardsDistribution == null)
                return;

            if (this.m_cardsDistribution.IsDefined())
            {
                if (MessageBox.Show("Удалить распределение карт?", "Подтвердите", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.m_cardsDistribution.Clear();
                }
            }
        }

        void OnMenu0(object sender, EventArgs e)
        {
            OpenCardsDistributionShowForm();           
        }

        void OnMenu1(object sender, EventArgs e)
        {
            OpenCardsDistributionForm();
        }

        void OnMenu2(object sender, EventArgs e)
        {
            ClearCardsDistribution();
        }



        // for lock

        public void SetSub2(bool is2started)
        {
            if (m_bSplit == false || m_bSub2 == is2started)
                return;

            m_bSub2 = is2started;
            m_bLock2 = true;

            if (this.ContextMenu != null)
            {
                if (m_bSub2)
                {
                    MenuItem mi;
                    mi = new MenuItem();
                    mi.Text = "Lock 2";
                    mi.Checked = m_bLock2;
                    mi.Click += OnMenu_Lock2;
                    this.ContextMenu.MenuItems.Add(mi);
                }
                else
                {
                    this.ContextMenu.MenuItems.RemoveAt(this.ContextMenu.MenuItems.Count - 1);
                }
            }


            this.Invalidate();
        }

        public void SetLockView(int level, bool val)
        {
            bool changed = false;
            if (level == 0)
            {
                if (m_bLock1 != val)
                    changed = true;
                m_bLock1 = val;
                if (changed)
                {
                    Invalidate();
                    if (this.ContextMenu != null)
                    {
                        this.ContextMenu.MenuItems[this.ContextMenu.MenuItems.Count - 1 - (m_bSplit && m_bSub2 ? 1 : 0)].Checked = m_bLock1;
                    }
                }
            }
            else if (level == 1)
            {
                if (m_bLock2 != val)
                    changed = true;
                m_bLock2 = val;
                if (changed)
                {
                    Invalidate();
                    if (this.ContextMenu != null)
                    {
                        if (m_bSplit && m_bSub2)
                            this.ContextMenu.MenuItems[this.ContextMenu.MenuItems.Count - 1].Checked = m_bLock2;
                    }
                }
            }
        }

        void OnMenu_Lock1(object sender, EventArgs e)
        {
            MenuItem mi = this.ContextMenu.MenuItems[this.ContextMenu.MenuItems.Count - 1 - (m_bSplit && m_bSub2 ? 1 : 0)];
            int lev = 0;
            bool locking = !mi.Checked;

            // ---------- запрос блокировки линии в BridgeScoreTable -------------
            if(LockingLine != null)
                LockingLine(this, new LockLineEventArgs(lev, locking));
        }

        void OnMenu_Lock2(object sender, EventArgs e)
        {
            MenuItem mi = this.ContextMenu.MenuItems[this.ContextMenu.MenuItems.Count - 1];
            int lev = 1;
            bool locking = !mi.Checked;

            // ---------- запрос блокировки линии в BridgeScoreTable -------------
            if (LockingLine != null)
                LockingLine(this, new LockLineEventArgs(lev, locking));
        }


        // event LockingLine
        public class LockLineEventArgs : EventArgs
        {
            //public int line;
            public int level;
            public bool locking;

            public LockLineEventArgs(/*int line,*/ int level, bool locking)
            {
                //this.line = line;
                this.level = level;
                this.locking = locking;
            }
        }
        public delegate void LockLineHandler(object sender, LockLineEventArgs e);
        public event LockLineHandler LockingLine;

        public void RemoveLockingLineEvent()
        {
            LockingLine = null;
        }
    }



    public class DealInfoControl_split : DealInfoControl
    {
        public DealInfoControl_split()
            : base()
        {
            m_bSplit = true;
        }
    }

    // ---------------------------------------------------------------------------------------------------


    public class CardsDistributionSelector : Control, IAttachData<CardsDistribution>, IDetachData
    {
        Quarters m_Q;
        CardsDistribution m_CD;
        public void AttachData(CardsDistribution cd)
        {
            if (m_CD == null)
            {
                m_CD = cd;
                m_CD.Changed += OnDataFullChanged;
                m_CD.OneCardDistrChanged += OnDataSingleChanged;

                OnDataFullChanged(this, null);
            }
        }

        public void DetachData(bool _inv)
        {
            if (m_CD != null)
            {
                m_CD.Changed -= OnDataFullChanged;
                m_CD.OneCardDistrChanged -= OnDataSingleChanged;
                m_CD = null;

                if(_inv)
                    OnDataFullChanged(this, null);
            }
        }

        SolidBrush[] brushesQ;
        Color[] colorsQ;
        SolidBrush brush_Black, brush_Red;
        Pen pen_Black;
        Font font_for_suits;

        Size size1;
        Point ptOffset;
        Size szDistance;

        public CardsDistributionSelector()
        {
            brush_Black = new SolidBrush(Color.Black);
            brush_Red = new SolidBrush(Color.Red);
            pen_Black = new Pen(Color.Black);
            font_for_suits = new Font("Courier New", 11F, FontStyle.Regular);

            ptOffset = new Point(1, 1);
            szDistance = new Size(1, 1);

            CapturedArea = new Card();
            m_Q = Quarters.NotDefinedYet;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            size1.Width = (this.Width - 2 * ptOffset.X - 3 * szDistance.Width) / 4;
            size1.Height = (this.Height - 2 * ptOffset.Y - 12 * szDistance.Height) / 13;

            this.Width = 2 * ptOffset.X + 3 * szDistance.Width + 4 * size1.Width;
            this.Height = 2 * ptOffset.Y + 12 * szDistance.Height + 13 * size1.Height;
        }
        
        public void SetOffsets(Point ptOff, Size szDist)
        {
            ptOffset = ptOff;
            szDistance = szDist;

            size1.Width = (this.Width - 2 * ptOffset.X - 3 * szDistance.Width) / 4;
            size1.Height = (this.Height - 2 * ptOffset.Y - 12 * szDistance.Height) / 13;
        }

        public void AttachQuarter(QuarterSelect qs)
        {
            if (colorsQ == null || colorsQ.Length != 5)
            {
                colorsQ = new Color[5];
                colorsQ[(int)Quarters.NotDefinedYet] = Color.White;
            }
            if (brushesQ != null && brushesQ.Length != 5)
            {
                for (int i = 0; i < brushesQ.Length; i++)
                    if (brushesQ[i] != null)
                        brushesQ[i].Dispose();
                brushesQ = null;
            }
            if (brushesQ == null || brushesQ.Length != 5)
            {
                brushesQ = new SolidBrush[5];
                brushesQ[(int)Quarters.NotDefinedYet] = new SolidBrush(Color.White);
            }

            colorsQ[(int)qs.GetQuater()] = qs.GetColor();
            brushesQ[(int)qs.GetQuater()] = new SolidBrush(qs.GetColor());
            qs.Selected += OnSelected;
        }

        QuarterSelect m_QS = null;

        void OnSelected(object sender, EventArgs e)
        {
            QuarterSelect old_QS = m_QS;
            m_QS = (QuarterSelect)sender;
            m_Q = m_QS.GetQuater();

            if (old_QS != m_QS)
            {
                if (old_QS != null)
                    old_QS.Select = false;
                if (m_QS != null)
                    m_QS.Select = true;
            }
        }

        int rings = 5;                // толщина тени
        int str_dist = -1;            // расстояние между достоинством карты и мастью
        int str_suit_correct_y = 1;   // сдвиг по Y символа масти
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle r = new Rectangle(0, 0, size1.Width, size1.Height);
            for (int i = 0; i < 4; i++)
            {
                CardSuit s = (i == 0 ? CardSuit.Clubs : (i == 1 ? CardSuit.Diamonds : (i == 2 ? CardSuit.Hearts : CardSuit.Spades))); //очередность мастей
                
                for(int j = 0 ; j < 13 ; j++)
                {
                    CardValue v = (CardValue)(j + 2);

                    r.X = ptOffset.X + i * (size1.Width + szDistance.Width);
                    r.Y = ptOffset.Y + j * (size1.Height + szDistance.Height);
                    // Обводка
                    SmallHelper.DrawRect(g, pen_Black, r);
                    // Закрасить текущим цветом
                    if (brushesQ != null && brushesQ.Length == 5)
                    {
                        SmallHelper.FillRectInside(g, brushesQ[(int)m_CD.Get(v, s)], r);
                    }
                    // Тень нового цвета
                    if (CapturedArea.IsDefined() && (CapturedArea.value == v) && (CapturedArea.suit == s) && StickArea.IsDefined() && (StickArea.value == v) && (StickArea.suit == s))
                    {
                        bool ok = false;
                        Quarters q = m_Q;
                        if (m_Q == Quarters.NotDefinedYet || m_Q == m_CD.Get(CapturedArea.value, CapturedArea.suit))
                        {
                            ok = true;
                            q = Quarters.NotDefinedYet;
                        }
                        else if (m_CD.GetCount(m_Q) < 13)
                        {
                            ok = true;
                            q = m_Q;
                        }
                        if (ok)
                        {
                            Rectangle r2 = r;
                            Color c1 = colorsQ[(int)m_CD.Get(CapturedArea.value, CapturedArea.suit)];
                            Color c2 = colorsQ[(int)q];
                            for (int k = 0; k < rings; k++)
                            {
                                r2.X++;
                                r2.Y++;
                                r2.Width -= 2;
                                r2.Height -= 2;
                                Color col = Color.FromArgb(c2.R + (c1.R - c2.R) / rings * k, c2.G + (c1.G - c2.G) / rings * k, c2.B + (c1.B - c2.B) / rings * k);
                                Pen pen = new Pen(col);
                                SmallHelper.DrawRect(g, pen, r2);
                            }
                        }
                    }
                    // Номер карты
                    Card card = new Card(v, s);
                    if (card.IsDefined())
                    {
                        SizeF sf1 = g.MeasureString(card.GetValueString(), this.Font);
                        SizeF sf2 = g.MeasureString(card.GetSuitString(), this.font_for_suits);
                        g.DrawString(card.GetValueString(), this.Font, brush_Black, r.X + (r.Width - sf1.Width - sf2.Width - str_dist) / 2, r.Y + (r.Height - sf1.Height) / 2);
                        g.DrawString(card.GetSuitString(), this.font_for_suits, (card.suit == CardSuit.Hearts || card.suit == CardSuit.Diamonds) ? brush_Red : brush_Black, r.X + (r.Width - sf1.Width - sf2.Width - str_dist) / 2 + sf1.Width + str_dist, r.Y + (r.Height - sf2.Height) / 2 + str_suit_correct_y);
                    }
                }
            }
        }

        Card CapturedArea, StickArea;
        Card GetArea(int x, int y)
        {
            Rectangle r = new Rectangle(0, 0, size1.Width, size1.Height);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    r.X = ptOffset.X + i * (size1.Width + szDistance.Width);
                    r.Y = ptOffset.Y + j * (size1.Height + szDistance.Height);
                    if (r.Contains(new Point(x, y)))
                        return new Card((CardValue)(j + 2), (i == 0 ? CardSuit.Clubs : (i == 1 ? CardSuit.Diamonds : (i == 2 ? CardSuit.Hearts : CardSuit.Spades)))); //очередность мастей
                }
            }
            return new Card();
        }
        void RefreshArea(Card area)
        {
            if (area.IsDefined())
            {
                int suitPos = (area.suit == CardSuit.Clubs ? 0 : (area.suit == CardSuit.Diamonds ? 1 : (area.suit == CardSuit.Hearts ? 2 : 3))); //очередность мастей
                int valPos = (int)(area.value - 2);
                Rectangle r = new Rectangle(ptOffset.X + suitPos * (size1.Width + szDistance.Width), ptOffset.Y + valPos * (size1.Height + szDistance.Height), size1.Width, size1.Height);
                this.Invalidate(r);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            StickArea = GetArea(e.X, e.Y);
            CapturedArea = StickArea;
            RefreshArea(CapturedArea);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!CapturedArea.IsDefined())
                return;

            Card oldStickArea = StickArea;
            StickArea = GetArea(e.X, e.Y);
            if (StickArea != oldStickArea && (StickArea == CapturedArea || oldStickArea == CapturedArea))
            {
                RefreshArea(CapturedArea);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!StickArea.IsDefined())
            {
                CapturedArea.def = false;
                return;
            }
            if (!CapturedArea.IsDefined())
            {
                return;
            }

            Card oldCapturedArea = CapturedArea;
            bool choiseDone = (CapturedArea == StickArea);
            if (choiseDone && CapturedArea.IsDefined())
            {
                if (m_Q == Quarters.NotDefinedYet || m_Q == m_CD.Get(CapturedArea.value, CapturedArea.suit))
                {
                    m_CD.Set(CapturedArea.value, CapturedArea.suit, Quarters.NotDefinedYet);
                }
                else if (m_CD.GetCount(m_Q) < 13)
                {
                    m_CD.Set(CapturedArea.value, CapturedArea.suit, m_Q);
                }
            }
            CapturedArea.def = false;
            if (choiseDone)
            {
                RefreshArea(oldCapturedArea);
            }
        }

        bool single_changed = false;
        void OnDataSingleChanged(object sender, CardsDistribution.OneCardDistrChangedEventsArgs e)
        {
            // Предотвратить полную перерисовку:
            single_changed = true;

            RefreshArea(e.card);
        }
        void OnDataFullChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            // Предотвращение полной перерисовки:
            if (single_changed)
            {
                single_changed = false;
                return;
            }

            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            font_for_suits.Dispose();
            brush_Red.Dispose();
            brush_Black.Dispose();
            pen_Black.Dispose();
            if (brushesQ != null)
            {
                for (int i = 0; i < brushesQ.Length; i++)
                    if (brushesQ[i] != null)
                        brushesQ[i].Dispose();
            }

            base.Dispose(disposing);
        }


    }

    // ---------------------------------------------------------------------------------------------------

    public class QuarterSelect : Control
    {
        int rings = 5;
        Color m_color;
        Quarters m_quarter;
        public Color GetColor() { return m_color; }
        public Quarters GetQuater() { return m_quarter; }

        Rectangle m_rect_BorderBounds;

        public QuarterSelect(Quarters q, Color c)
        {
            m_quarter = q;
            m_color = c;
        }

        bool m_selected;
        public bool Select
        {
            get
            {
                return m_selected;
            }
            set
            {
                bool old = m_selected;
                m_selected = value;
                if (m_selected != old)
                    this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Pen pen;

            // Обводка
            Rectangle r = m_rect_BorderBounds;
            for (int i = 0; i < rings; i++)
            {
                if (bMousePressed && bMouseOver)
                {
                    Color col = Color.FromArgb(this.BackColor.R + (Color.Black.R - this.BackColor.R) / (rings * 2 + 1) * (i + 1), this.BackColor.G + (Color.Black.G - this.BackColor.G) / (rings * 2 + 1) * (i + 1), this.BackColor.B + (Color.Black.B - this.BackColor.B) / (rings * 2 + 1) * (i + 1));
                    pen = new Pen(col);
                    SmallHelper.DrawRect(g, pen, r);
                    pen.Dispose();
                }
                else if (this.m_selected)
                {
                    Color col = Color.FromArgb(this.BackColor.R + (Color.Black.R - this.BackColor.R) / (rings + 1) * (i + 1), this.BackColor.G + (Color.Black.G - this.BackColor.G) / (rings + 1) * (i + 1), this.BackColor.B + (Color.Black.B - this.BackColor.B) / (rings + 1) * (i + 1));
                    pen = new Pen(col);
                    SmallHelper.DrawRect(g, pen, r);
                    pen.Dispose();
                }
                r.Inflate(-1, -1);
            }

            // Граница
            pen = new Pen(Color.Black);
            SmallHelper.DrawRect(g, pen, r);
            pen.Dispose();

            // Заливка цветом данной масти
            Brush brush = new SolidBrush(m_color);
            SmallHelper.FillRectInside(g, brush, r);
            brush.Dispose();

            // Символ игрока (N/S/E/W)
            brush = new SolidBrush(Color.Black);
            String str = (this.m_quarter == Quarters.N ? "N" : (this.m_quarter == Quarters.S ? "S" : this.m_quarter == Quarters.E ? "E" : this.m_quarter == Quarters.W ? "W" : ""));
            SmallHelper.DrawMultiString(g, r, str, this.Font, brush, StringAlignment.Center, StringAlignment.Center, StringAlignment.Center, 0, 0, 0);
            brush.Dispose();
        }

        bool bMousePressed = false;
        bool bMouseOver = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            bMousePressed = true;
            bMouseOver = true;
            this.Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!bMousePressed)
                return;

            bool old_bMouseOver = bMouseOver;
            bMouseOver = m_rect_BorderBounds.Contains(e.X, e.Y);
            if (old_bMouseOver != bMouseOver)
            {
                this.Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!bMousePressed)
                return;
            else
                bMousePressed = false;

            if (m_rect_BorderBounds.Contains(e.X, e.Y))
            {
                this.SelectMe();
            }
            this.Invalidate();
        }

        public event EventHandler Selected;
        public void SelectMe()
        {
            if(Selected != null)
                this.Selected(this, null);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            m_rect_BorderBounds = new Rectangle(0, 0, this.Width, this.Height);
        }
    }

    // ---------------------------------------------------------------------------------------------------

    public class CardsDistributionWatcher : Control, IAttachData<CardsDistribution>, IDetachData
    {
        CardsDistribution m_CD;
        public void AttachData(CardsDistribution cd)
        {
            if (m_CD == null)
            {
                m_CD = cd;
                m_CD.Changed += OnDataChanged;

                OnDataChanged(this, null);
            }
        }
        public void DetachData(bool _inv)
        {
            if (m_CD != null)
            {
                m_CD.Changed -= OnDataChanged;
                m_CD = null;

                if(_inv)
                    OnDataChanged(this, null);
            }
        }

        SolidBrush brush_Black, brush_Red;
        Pen pen_Black;
        Font font_for_suits;

        public CardsDistributionWatcher()
        {
            brush_Black = new SolidBrush(Color.Black);
            brush_Red = new SolidBrush(Color.Red);
            pen_Black = new Pen(Color.Black);
            font_for_suits = new Font("Courier New", 11F, FontStyle.Regular);
        }

        void OnDataChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            font_for_suits.Dispose();
            brush_Red.Dispose();
            brush_Black.Dispose();
            pen_Black.Dispose();

            base.Dispose(disposing);
        }


        int square_width = 100;
        int wrap_width_for_N_S = 200;
        int additional_squareoff_for_N_S = 15;
        int square_in = 1;
        int square_off = 2;
        int main_offset = 1;
        int suit_offsetX = 1;
        int suit_offsetY = -1;
        int str_intervalY_in = -2;
        int str_intervalY = 0;
        Rectangle m_rect_BorderBounds;
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //SmallHelper.FillRectInside(g, new SolidBrush(Color.Gray), m_rect_BorderBounds);

            // Нарисовать квадрат со сторонами света посередине
            Rectangle rectCenterQuarter = new Rectangle((m_rect_BorderBounds.Width - square_width) / 2, (m_rect_BorderBounds.Height - square_width) / 2, square_width, square_width);
            SmallHelper.DrawRect(g, pen_Black, rectCenterQuarter);

            SmallHelper.DrawMultiString(g, rectCenterQuarter, "W", this.Font, brush_Black, StringAlignment.Near, StringAlignment.Near, StringAlignment.Center, square_in, 0, 0);
            SmallHelper.DrawMultiString(g, rectCenterQuarter, "E", this.Font, brush_Black, StringAlignment.Far, StringAlignment.Far, StringAlignment.Center, square_in, 0, 0);
            SmallHelper.DrawMultiString(g, rectCenterQuarter, "N", this.Font, brush_Black, StringAlignment.Center, StringAlignment.Center, StringAlignment.Near, 0, square_in, 0);
            SmallHelper.DrawMultiString(g, rectCenterQuarter, "S", this.Font, brush_Black, StringAlignment.Center, StringAlignment.Center, StringAlignment.Far, 0, square_in, 0);

            if (m_CD == null/* || !m_CD.IsDefined()*/)
                return;

            // Порядок, в котором строки мастей записываются по вертикали
            CardSuit[] suitsFollowArr = new CardSuit[] { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades };
            // Массив сторон света
            Quarters[] quarterFollowArr = new Quarters[] { Quarters.W, Quarters.E, Quarters.N, Quarters.S };

            // Найти ширину полосы, в которой будут символы мастей (эта полоса слева от полосы дистоинств)
            float suit_width = 0;
            SizeF ssf;
            for (int i = 0; i < 4; i++)
            {
                ssf = g.MeasureString(SmallHelper.GetCardSuitString(suitsFollowArr[i]), font_for_suits);
                if (ssf.Width > suit_width)
                    suit_width = ssf.Width;
            }

            // Найти прямоугольники для каждого куска текста по стороам света
            Rectangle[] rQArr = new Rectangle[4];
            for(int i = 0 ; i < 4 ; i++)
            {
                switch(quarterFollowArr[i])
                {
                    case Quarters.W:
                        rQArr[i] = new Rectangle(main_offset, main_offset, (m_rect_BorderBounds.Width - 2 * (main_offset + square_off) - square_width) / 2, this.Height - 2 * main_offset);
                        break;
                    case Quarters.E:
                        rQArr[i] = new Rectangle(rectCenterQuarter.Right + square_off, main_offset, (m_rect_BorderBounds.Width - 2 * (main_offset + square_off) - square_width) / 2, this.Height - 2 * main_offset);
                        break;
                    case Quarters.N:
                        rQArr[i] = new Rectangle(main_offset, main_offset, this.Width - 2 * main_offset, (m_rect_BorderBounds.Height - 2 * (main_offset + square_off + additional_squareoff_for_N_S) - square_width) / 2);
                        break;
                    case Quarters.S:
                        rQArr[i] = new Rectangle(main_offset, rectCenterQuarter.Bottom + square_off + additional_squareoff_for_N_S, this.Width - 2 * main_offset, (m_rect_BorderBounds.Height - 2 * (main_offset + square_off + additional_squareoff_for_N_S) - square_width) / 2);
                        break;
                }
            }

            // Забить все 4x4 строки распределения карт (сторона света x масть)
            String[,] strArr = new String[4, 4] { { "", "", "", "" }, { "", "", "", "" }, { "", "", "", "" }, { "", "", "", "" } };
            for (CardValue v = CardValue.Ace; v >= CardValue.Two; v--)
            {
                for (int j = 0 ; j < 4 ; j++)
                {
                    CardSuit s = suitsFollowArr[j]; //очередность мастей
                    Quarters q = m_CD.Get(v, s);
                    if (q != Quarters.NotDefinedYet)
                    {
                        int quarterIndex = -1;
                        for(int i = 0 ; i < quarterFollowArr.Length ; i++)
                        {
                            if(quarterFollowArr[i] == q)
                            {
                                quarterIndex = i;
                                break;
                            }
                        }
                        strArr[quarterIndex, j] += " " + SmallHelper.GetCardValueString(v);
                    }
                 }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    strArr[i, j] = strArr[i, j].Trim();
                    if (strArr[i, j].Length == 0)
                        strArr[i, j] = "-";
                }
            }

            // А теперь главное - нарисовать:
            for (int i = 0; i < 4; i++)
            {
                // Разделить строки на мультистроки, учитывая макс. ширину wrap_width
                int wrap_width = (int)(rQArr[i].Width - suit_width - suit_offsetX);
                if ((quarterFollowArr[i] == Quarters.N || quarterFollowArr[i] == Quarters.S) && wrap_width_for_N_S > 0 && wrap_width_for_N_S < wrap_width)
                    wrap_width = wrap_width_for_N_S;
                for (int j = 0; j < 4; j++)
                {
                    strArr[i, j] = SmallHelper.WrapString(g, this.Font, strArr[i, j], wrap_width);
                }

                // Посчитать размеры 4 мультистрок для каждой масти, и найти точный прямоугольник для текста текущей стороны света (учитывая символы мастей)
                SizeF[] szArr = new SizeF[4];
                RectangleF rfQuarterText = new RectangleF(0, 0, 0, 0);
                for (int j = 0; j < 4; j++)
                {
                    szArr[j] = SmallHelper.MeasureMultistring(g, this.Font, strArr[i, j], str_intervalY_in);
                    rfQuarterText.Height += szArr[j].Height;
                    if (j > 0)
                        rfQuarterText.Height += str_intervalY;
                    if (szArr[j].Width > rfQuarterText.Width)
                        rfQuarterText.Width = szArr[j].Width;
                }
                rfQuarterText.Width += (suit_width + suit_offsetX);
                rfQuarterText.X = rQArr[i].X + (rQArr[i].Width - rfQuarterText.Width) / 2;
                rfQuarterText.Y = rQArr[i].Y + (rQArr[i].Height - rfQuarterText.Height) / 2;

                // Нарисовать:
                float drawX = rfQuarterText.X, drawY = rfQuarterText.Y;
                for (int j = 0; j < 4; j++)
                {
                    g.DrawString(SmallHelper.GetCardSuitString(suitsFollowArr[j]), font_for_suits, (suitsFollowArr[j] == CardSuit.Hearts || suitsFollowArr[j] == CardSuit.Diamonds) ? brush_Red : brush_Black, drawX, drawY + suit_offsetY);
                    SmallHelper.DrawMultiString(g, new RectangleF(drawX + suit_width + suit_offsetX, drawY, 0, 0), strArr[i, j], this.Font, brush_Black, StringAlignment.Near, StringAlignment.Near, StringAlignment.Near, 0, 0, str_intervalY_in);
                    drawY += (szArr[j].Height + str_intervalY);
                }

                //SmallHelper.DrawRect(g, pen_Black, new Rectangle((int)rfQuarterText.X, (int)rfQuarterText.Y, (int)rfQuarterText.Width, (int)rfQuarterText.Height)); //delete me!
            }
        }

        protected override void OnResize(EventArgs e)
        {
            m_rect_BorderBounds = new Rectangle(0, 0, this.Width, this.Height);
        }
    }
}
