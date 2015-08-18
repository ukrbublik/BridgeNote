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
    public struct SwitcherStruct
    {
        bool born;
        String[] vars;
        int choise;

        public void SetVariants(String[] v)
        {
            if(vars == null)
                vars = (String[])v.Clone();
        }
        public int Choise
        {
            get
            {
                return choise;
            }
            set
            {
                if(IsVariantsLoaded() && IsChoiseOK(value))
                {
                    this.choise = value;
                    this.born = true;
                }
            }
        }
        public bool Defined
        {
            get
            {
                return (IsVariantsLoaded() && born && IsChoiseOK());
            }
        }
        bool IsChoiseOK(int c)
        {
            return (c >= 0 && c < this.vars.Length);
        }
        bool IsChoiseOK()
        {
            return IsChoiseOK(this.choise);
        }
        bool IsVariantsLoaded()
        {
            return (this.vars != null && this.vars.Length != 0);
        }

        public bool Born
        {
            get
            {
                return this.born;
            }
            set
            {
                if(value == true)
                {
                    if (IsVariantsLoaded())
                    {
                        if(!this.born || !IsChoiseOK())
                            this.choise = 0;
                        this.born = true;
                    }
                }
                else
                {
                    this.born = false;
                }
            }
        }

        public void Clear()
        {
            this.choise = 0;
            this.born = false;
        }

        public void Switch()
        {
            if (IsVariantsLoaded())
            {
                if (!Born)
                {
                    Born = true;
                }
                else
                {
                    choise++;
                    if (!IsChoiseOK())
                        choise = 0;
                }
            }
        }


        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;
            if (this.vars == null || ((SwitcherStruct)obj).vars == null)
                return false;
            bool vars_equals = (this.vars.Length == ((SwitcherStruct)obj).vars.Length);
            if (vars_equals)
            {
                for (int i = 0; i < this.vars.Length; i++)
                {
                    if (this.vars[i] != ((SwitcherStruct)obj).vars[i])
                        vars_equals = false;
                }
            }

            return (this.born == ((SwitcherStruct)obj).born && vars_equals && this.choise == ((SwitcherStruct)obj).choise);
        }
        public static bool operator ==(SwitcherStruct sw1, SwitcherStruct sw2)
        {
            return sw1.Equals(sw2);
        }
        public static bool operator !=(SwitcherStruct sw1, SwitcherStruct sw2)
        {
            return !sw1.Equals(sw2);
        }

        public override String ToString()
        {
            if (this.Defined)
            {
                return (vars[choise]);
            }
            else
            {
                return "";
            }
        }
    }

    public class Switcher : BaseChangedData, ISQLSerialize
    {
        // Инкапсулируем структуру:
        protected SwitcherStruct val;
        public SwitcherStruct GetStruct()
        {
            return val;
        }

        // Конструкторы:
        public Switcher()
        {
        }
        public Switcher(String[] vars)
        {
            val.SetVariants(vars);
        }

        // Иммитация кострукторов:
        public void Constructor(String[] vars)
        {
            val.SetVariants(vars);
        }

        // Get&Set
        virtual public int Choise
        {
            get
            {
                return val.Choise;
            }
            set
            {
                SwitcherStruct olds = val;
                val.Choise = value;
                if (olds != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(olds, val));
            }
        }
        public bool Born
        {
            get
            {
                return val.Born;
            }
            set
            {
                SwitcherStruct olds = val;
                val.Born = value;
                if (olds != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(olds, val));
            }
        }

        override public bool IsDefined()
        {
            return val.Defined;
        }

        // Ф-ия Переключить
        virtual public void Switch()
        {
            SwitcherStruct olds = val;
            val.Switch();
            if (olds != val && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(olds, val));
        }

        // Получение строкового представления переключателя
        public override String ToString()
        {
            return val.ToString();
        }

        // Сериализация SQL
        public void _FromDataBase(object v)
        {
            if (v == DBNull.Value)
                this.Born = false;
            else
                if(v.GetType() == typeof(System.Boolean))
                    this.Choise = ((bool)v == true ? 1 : 0);
                else if (v.GetType() == typeof(System.Byte))
                    this.Choise = (int)(byte)v;
                else
                    this.Choise = (int)v;
        }
        public object _ToDataBase()
        {
            if(this.IsDefined())
                return this.Choise;
            else
                return DBNull.Value;
        }

        // Clear()
        public override void Clear()
        {
            SwitcherStruct olds = val;

            val.Clear();
            
            if (olds != val && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(olds, val));
        }
    }

    public partial class SwitcherControl : BaseControlInTable, IAttachData<Switcher>, IDetachData
    {
        Switcher m_switcher;

        bool can_change;
        public bool CanChange
        {
            get
            {
                return can_change;
            }
            set
            {
                if (can_change != value)
                {
                    if (value == true) //on
                    {
                        this.MouseDown += this.Switcher_MouseDown;
                        this.MouseMove += this.Switcher_MouseMove;
                        this.MouseUp += this.Switcher_MouseUp;
                    }
                    else //off
                    {
                        this.MouseDown -= this.Switcher_MouseDown;
                        this.MouseMove -= this.Switcher_MouseMove;
                        this.MouseUp -= this.Switcher_MouseUp;
                    }
                }
            }
        }

        public SwitcherControl()
        {
            this.m_switcher = null;
            this.can_change = true;
            InitializeComponent();

            bMousePressed = false;
            m_brush_String = new SolidBrush(SystemColors.ControlText);
        }
        public SwitcherControl(bool CanChange)
        {
            this.m_switcher = null;
            this.can_change = CanChange;
            InitializeComponent();

            bMousePressed = false;
            m_brush_String = new SolidBrush(SystemColors.ControlText);
        }

        public void AttachData(Switcher sw)
        {
            if (m_switcher == null)
            {
                m_switcher = sw;
                m_switcher.Changed += OnSwitcherChanged;

                OnSwitcherChanged(this, null);
            }
        }

        public void DetachData(bool _inv)
        {
            if (m_switcher != null)
            {
                m_switcher.Changed -= OnSwitcherChanged;
                m_switcher = null;

                if(_inv)
                    OnSwitcherChanged(this, null);
            }
        }

        void OnSwitcherChanged(object sender, Switcher.ChangedEventsArgs e)
        {
            this.Invalidate();
        }

        bool bMousePressed;
        bool bMouseOver;

        Pen[] highlightPens;
        public void DefineHighlight(int count, Color highlightColor)
        {
            highlightPens = new Pen[count];
            for (int i = 0; i < count; i++)
            {
                highlightPens[i] = new Pen(Color.FromArgb(highlightColor.R + (this.BackColor.R - highlightColor.R) / (count + 2) * (i + 2), highlightColor.G + (this.BackColor.G - highlightColor.G) / (count + 2) * (i + 2), highlightColor.B + (this.BackColor.B - highlightColor.B) / (count + 2) * (i + 2)));
            }
        }

        Rectangle m_rect_BorderBounds;
        SolidBrush m_brush_String;
        RectangleF m_rf_String;
        int offset_x = 1;
        protected StringAlignment align_x = StringAlignment.Near;

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            // Обводка, если нажата мышка
            if (bMousePressed && bMouseOver && !_lock_)
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
            if (m_switcher != null && m_switcher.IsDefined())
            {
                SmallHelper.DrawMultiString(g, m_rect_BorderBounds, m_switcher.ToString(), this.Font, this.m_brush_String, align_x, StringAlignment.Near, StringAlignment.Center, offset_x, 0, 0);


                //m_rf_String = SmallHelper.StringInMiddle(g, m_rect_BorderBounds, m_switcher.ToString(), this.Font);
                //g.DrawString(this.m_switcher.ToString(), this.Font, this.m_brush_String, offset_x, m_rf_String.Y);
            }
        }

        private void Switcher_MouseDown(object sender, MouseEventArgs e)
        {
            if (_lock_)
            {
                this.Focus();
                return;
            }

            bMousePressed = true;
            bMouseOver = true;
            this.Focus();

            this.Invalidate();
        }

        private void Switcher_MouseMove(object sender, MouseEventArgs e)
        {
            if (_lock_)
                return;

            if (!bMousePressed)
                return;

            bool old_bMouseOver = bMouseOver;
            bMouseOver = m_rect_BorderBounds.Contains(e.X, e.Y);
            if (old_bMouseOver != bMouseOver)
            {
                this.Invalidate();
            }
        }

        private void Switcher_MouseUp(object sender, MouseEventArgs e)
        {
            if (_lock_)
                return;

            if (!bMousePressed)
                return;
            else
                bMousePressed = false;

            if (m_rect_BorderBounds.Contains(e.X, e.Y))
            {
                if (m_switcher != null)
                    m_switcher.Switch();
            }
            this.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            m_rect_BorderBounds = new Rectangle(0, 0, this.Width, this.Height);
        }


        // ILock
        public override void OnLocked()
        {
            
        }

        public override void OnUnlocked()
        {
            
        }
    }



    // ----------------- Моды --------------------------

    public class PairSwitcher : Switcher
    {
        public PairSwitcher()
            : base(new String[] { "NS", "EW" })
        {
        }

        public Pairs Pair
        {
            get
            {
                if (!this.Born)
                    return Pairs.NotDefinedYet;
                else
                    return (Pairs) (this.Choise + 1);
            }
            set
            {
                if (value == Pairs.NotDefinedYet)
                    this.Born = false;
                else
                    this.Choise = (int) (value - 1);
            }
        }
    }

    public class ZoneSwitcher : Switcher
    {
        public ZoneSwitcher()
            : base(new String[] { "-", "NS", "EW", "NS,EW" })
        {
        }

        public Zones Zone
        {
            get
            {
                if (!this.Born)
                    return Zones.NotDefinedYet;
                else
                    return (Zones)(this.Choise + 1);
            }
            set
            {
                if (value == Zones.NotDefinedYet)
                    this.Born = false;
                else
                    this.Choise = (int)(value - 1);
            }
        }
        public static bool IsPairInZone(Pairs pair, Zones zone)
        {
            if (pair == Pairs.NotDefinedYet || zone == Zones.NotDefinedYet)
                return false;
            else if (zone == Zones.Both || pair == Pairs.EW && zone == Zones.EW || pair == Pairs.NS && zone == Zones.NS)
                return true;
            else
                return false;
        }
    }

    public class QuarterSwitcher : Switcher
    {
        public QuarterSwitcher()
            : base(new String[] { "N", "S", "E", "W" })
        {
        }

        public Quarters Quarter
        {
            get
            {
                if (!this.Born)
                    return Quarters.NotDefinedYet;
                else
                    return (Quarters)(this.Choise + 1);
            }
            set
            {
                if (value == Quarters.NotDefinedYet)
                    this.Born = false;
                else
                    this.Choise = (int)(value - 1);
            }
        }
    }

    public class SwitcherControl_Orange : SwitcherControl
    {
        public SwitcherControl_Orange()
            : base()
        {
            DefineHighlight(5, Color.Orange);
        }
    }

    public class SwitcherControl_Orange_Center : SwitcherControl_Orange
    {
        public SwitcherControl_Orange_Center()
            : base()
        {
            align_x = StringAlignment.Center;
        }
    }

    public class SwitcherControl_NoChange : SwitcherControl
    {
        public SwitcherControl_NoChange()
            : base(false)
        {

        }
    }

    public class OnersSwitcher : Switcher
    {
        public OnersSwitcher()
            : base(new String[] { "-", "NS 4o", "NS 5o", "NS 4A", "EW 4o", "EW 5o", "EW 4A" })
        {
            //Born = true;
        }

        public CardTrump trump = CardTrump.NotYetDefined;

        override public int Choise
        {
            get
            {
                return base.Choise;
            }
            set
            {
                switch (trump)
                {
                    case CardTrump.NotYetDefined:
                        if (value == 0) // -
                            base.Choise = value;
                        break;
                    case CardTrump.NT:
                        if(value == 0 || value == 3 || value == 6) // -, NS 4A, EW 4A
                            base.Choise = value;
                        break;
                    case CardTrump.Clubs: case CardTrump.Diamonds: case CardTrump.Hearts: case CardTrump.Spades:
                        if(value == 0 || value == 1 || value == 2 || value == 4 || value == 5) // -, NS 4o, NS 5o, EW 4o, EW 5o
                            base.Choise = value;
                        break;
                }
            }
        }

        override public void Switch()
        {
            SwitcherStruct olds = val;

            do
            {
                val.Switch();
            }
            while (IsNormal() == false);


            if (olds != val && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(olds, val));
        }

        private bool IsNormal()
        {
            switch (trump)
            {
                case CardTrump.NT:
                    return (Choise == 0 || Choise == 3 || Choise == 6); // -, NS 4A, EW 4A
                case CardTrump.Clubs:
                case CardTrump.Diamonds:
                case CardTrump.Hearts:
                case CardTrump.Spades:
                    return (Choise == 0 || Choise == 1 || Choise == 2 || Choise == 4 || Choise == 5); // -, NS 4o, NS 5o, EW 4o, EW 5o
                case CardTrump.NotYetDefined:
                    return (Choise == 0); //-
                default:
                    return (Choise == 0); //-
            }
        }

        override public bool IsDefined()
        {
            return (val.Defined && IsNormal());
        }
    }

    public class FitsSwitcher : Switcher
    {
        public FitsSwitcher()
            : base(new String[] { "0", "1", "2" })
        {
        }
    }
}
