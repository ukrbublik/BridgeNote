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
    public partial class ResultSelector : Control
    {
        Timer timer;
        int timer_first = 600;
        int timer_interval = 400;

        static Rectangle m_rect_MainBorderBounds;
        static Rectangle m_rect_QuantityDecreaseBounds;
        static Rectangle m_rect_QuantityIncreaseBounds;
        static Size static_size;
        static Font font2;
        static int arrow_offset;
        static public void SetCoordinates()
        {
            static_size = new System.Drawing.Size(134, 50);
            m_rect_MainBorderBounds = new Rectangle(0, 0, static_size.Width, static_size.Height);
            m_rect_QuantityDecreaseBounds = new Rectangle(10, 12, 37, 26);
            m_rect_QuantityIncreaseBounds = new Rectangle(m_rect_MainBorderBounds.Right - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Y, m_rect_QuantityDecreaseBounds.Width, m_rect_QuantityDecreaseBounds.Height);
            arrow_offset = 10;
            font2 = new Font("Tahoma", 7F, FontStyle.Regular);
        }
        static ResultSelector()
        {
            SetCoordinates();
        }

        enum Area { OutOfBounds = -1, None = 0, ArrowLeft, ArrowRight };
        Area CapturedArea, StickArea;


         protected ResultSelector()
        {
            InitializeComponent();

            CloseMe(false);

            CapturedArea = Area.None;
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
        }

        static ResultSelector only_one = null;
        static public ResultSelector GetInstance()
        {
            if (only_one == null)
                only_one = new ResultSelector();
            return only_one;
        }

        // ----------------------------------------------------------------------------------------
        
        // !!!!!!!!
        // !!!!!!!! КАЖДОМУ ВНУТР. ЭЛЕМЕНТУ НАДО ДОБАВЛЯТЬ ТАКОЙ ОБРАБОТЧИК ДЛЯ LostFocus - CloseWhenLostFocus
        // !!!!!!!!

        ResultSelectControl m_selControl;
        Control m_attach_sel, m_attach_form;
        Result m_result;

        public void OpenMe(ResultSelectControl s, Control attach_sel, Control attach_form, Result resultToCopy)
        {
            if (IsOpened())
                CloseMe(true);

            m_result = new Result(resultToCopy);
            m_result.Born = true;
            m_result.Changed += OnResultChanged;
            m_result._AddContractChangedHandler(OnContractChanged);

            m_selControl = s;
            m_attach_sel = attach_sel;
            m_attach_form = attach_form;
            DefineLocation();
            if (!m_attach_form.Controls.Contains(this))
                m_attach_form.Controls.Add(this);
            Visible = true;
            BringToFront();

            this.Focus();
        }

        void CloseWhenLostFocus(object obj, EventArgs e)
        {
            // ЗАКРЫТЬ СЕЛЕКТОР, ЕСЛИ ФОКУС НЕ СОДЕРЖИТ НИ САМ СЕЛЕКТОР, НИ КАКОЙ-ЛИБО ИЗ ЕГО ВНУТРЕННИХ КОНТРОЛОВ
            if (m_selControl != null && m_selControl.Focused)
                return;
            if (this.Focused)
                return;
            for (int i = 0; i < this.Controls.Count; i++)
                if (this.Controls[i].Focused)
                    return;
            this.CloseMe(true);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            CloseWhenLostFocus(this, e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }

        public bool IsAttachedToThisControl(ResultSelectControl rsc)
        {
            return (m_selControl == rsc);
        }
        public bool IsOpened()
        {
            return (m_attach_form != null && m_attach_form.Controls.Contains(this));
        }

        public event SelectorClosingHandler Closing;
        public event EventHandler Closed;

        public void CloseMe(bool saveBeforeClose)
        {
            if (this.m_result != null)
            {
                this.m_result._RemoveContractChangedHandler(OnContractChanged);
                this.m_result.RemoveChangedHandlers();
            }

            if(Closing != null)
                Closing(this, new SelectorClosingEventArgs(m_result, saveBeforeClose));
            m_result = null;

            m_selControl = null;
            //if (m_attach_form != null && m_attach_form.Controls.Contains(this))
            //    m_attach_form.Controls.Remove(this);
            m_attach_sel = null;
            m_attach_form = null;
            this.Size = new Size(0, 0);
            this.Visible = false;

            if (Closed != null)
                Closed(this, null);
        }

        internal void DefineLocation()
        {
            this.Size = static_size; // :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ??? :2 ???

            Point selcontrol_Location = m_attach_sel.Location;
            Control temp = m_attach_sel;
            while (temp.Parent != m_attach_form)
            {
                temp = temp.Parent;
                selcontrol_Location.Offset(temp.Location.X, temp.Location.Y);
            }
            Point loc = selcontrol_Location;
            loc.Offset(0, m_attach_sel.Size.Height + 1);
            // если жмёт справа:
            if ((loc.X + this.Width) > m_attach_form.ClientRectangle.Width)
                loc.X = m_attach_form.ClientRectangle.Width - this.Width;
            // если жмёт снизу:
            if ((loc.Y + this.Height) > m_attach_form.ClientRectangle.Height)
                loc.Y = selcontrol_Location.Y - this.Height - 1;
            // если жмёт слева:
            if (loc.X < 0)
                loc.X = 0;
            // если жмёт сверху:
            if (loc.Y < 0)
                loc.Y = 0;

            this.Location = loc;
        }

        // ----------------------------------------------------------------------------------------

        bool ticked = false;
        void timer_Tick(object sender, EventArgs e)
        {
            ticked = true;
            if (this.timer.Interval == timer_first)
                this.timer.Interval = timer_interval;

            // повторение
            //ResultStruct oldRS = this.m_result.GetStruct();  - не нужно, см. OnResultChanged()
            if (CapturedArea == Area.ArrowLeft)
                this.m_result.Quantity--;
            else
                this.m_result.Quantity++;
            RefreshArea(CapturedArea);
            //RefreshArea(CapturedArea, true, oldRS); - перенес в OnResultChanged()
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            // рамка
            SmallHelper.DrawRect(g, ContractSelector.m_pen_MainBounds2, m_rect_MainBorderBounds);
            m_rect_MainBorderBounds.Inflate(-1, -1);
            SmallHelper.DrawRect(g, ContractSelector.m_pen_MainBounds1, m_rect_MainBorderBounds);
            m_rect_MainBorderBounds.Inflate(1, 1);

            if (m_result == null)
                return;

            if (!this.m_result.IsDefined() || this.m_result.IsContractNo())
            {
                // надпись [контракт не определён] или [контракт не установлен]

                String s1 = "Контракт";
                String s2 = (!this.m_result.IsDefined() ? "не определён!" : "не установлен");
                SizeF sf1 = g.MeasureString(s1, font2);
                SizeF sf2 = g.MeasureString(s2, font2);
                int y_distance = -8;
                g.DrawString(s1, font2, (!this.m_result.IsDefined() ? ContractSelector.m_brush_String_RED : ContractSelector.m_brush_String_BLACK) , m_rect_MainBorderBounds.X + (m_rect_MainBorderBounds.Width - sf1.Width) / 2, m_rect_MainBorderBounds.Y + (m_rect_MainBorderBounds.Height - sf1.Height - sf2.Height - y_distance) / 2);
                g.DrawString(s2, font2, (!this.m_result.IsDefined() ? ContractSelector.m_brush_String_RED : ContractSelector.m_brush_String_BLACK), m_rect_MainBorderBounds.X + (m_rect_MainBorderBounds.Width - sf2.Width) / 2, m_rect_MainBorderBounds.Y + (m_rect_MainBorderBounds.Height - sf1.Height - sf2.Height - y_distance) / 2 + sf1.Height + y_distance);
            }
            else
            {
                // кнопка [<]
                SmallHelper.DrawRect(g, (!this.m_result.IsMin) ? (CapturedArea == Area.ArrowLeft && StickArea == CapturedArea ? ContractSelector.m_pen_Arrows_SELECTED : ContractSelector.m_pen_Arrows_ACTIVE) : ContractSelector.m_pen_Arrows_NONACTIVE, m_rect_QuantityDecreaseBounds);
                SmallHelper.FillRectInside(g, (!this.m_result.IsMin) ? (CapturedArea == Area.ArrowLeft && StickArea == CapturedArea ? ContractSelector.m_brush_Arrows_SELECTED : ContractSelector.m_brush_Arrows_ACTIVE) : ContractSelector.m_brush_Arrows_NONACTIVE, m_rect_QuantityDecreaseBounds);
                SmallHelper.DrawBmpTransp(g, (!this.m_result.IsMin) ? ContractSelector.m_bmp_Arrow_Left : ContractSelector.m_bmp_Arrow_Left_Passive, new Point(m_rect_QuantityDecreaseBounds.Right - arrow_offset - ContractSelector.m_bmp_Arrow_Left.Width, m_rect_QuantityDecreaseBounds.Top + (m_rect_QuantityDecreaseBounds.Height - ContractSelector.m_bmp_Arrow_Left.Height) / 2));

                // кнопка [>]
                SmallHelper.DrawRect(g, (!this.m_result.IsMax) ? (CapturedArea == Area.ArrowRight && StickArea == CapturedArea ? ContractSelector.m_pen_Arrows_SELECTED : ContractSelector.m_pen_Arrows_ACTIVE) : ContractSelector.m_pen_Arrows_NONACTIVE, m_rect_QuantityIncreaseBounds);
                SmallHelper.FillRectInside(g, (!this.m_result.IsMax) ? (CapturedArea == Area.ArrowRight && StickArea == CapturedArea ? ContractSelector.m_brush_Arrows_SELECTED : ContractSelector.m_brush_Arrows_ACTIVE) : ContractSelector.m_brush_Arrows_NONACTIVE, m_rect_QuantityIncreaseBounds);
                SmallHelper.DrawBmpTransp(g, (!this.m_result.IsMax) ? ContractSelector.m_bmp_Arrow_Right : ContractSelector.m_bmp_Arrow_Right_Passive, new Point(m_rect_QuantityIncreaseBounds.Left + arrow_offset, m_rect_QuantityIncreaseBounds.Top + (m_rect_QuantityIncreaseBounds.Height - ContractSelector.m_bmp_Arrow_Right.Height) / 2));

                // цифра
                RectangleF rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Top, m_rect_QuantityIncreaseBounds.Left - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Height), this.m_result.ToString(), this.Font);
                g.DrawString(this.m_result.ToString(), this.Font, this.m_result.Quantity >= 0 ? ContractSelector.m_brush_String_GREEN : ContractSelector.m_brush_String_RED , rf /*m_rect_QuantityNumber.X, m_rect_QuantityNumber.Y*/);
            }
        }


        void OnContractChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            if (((ContractStruct)e._old).Quantity != ((ContractStruct)e._new).Quantity && this.m_result.Born)
            {
                // обнулить и заблокировать результат
                this.m_result.EmptyAndDead();

                //или

                // сдвинуть результат
                //int diff = ((ContractStruct)e._new).Quantity - ((ContractStruct)e._old).Quantity;
                //this.m_result.Quantity -= diff;
            }
        }

        void OnResultChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            if (((ResultStruct)e._new).Born != ((ResultStruct)e._old).Born)
            {
                this.Invalidate();
            }
            else if (((ResultStruct)e._old).Quantity != ((ResultStruct)e._new).Quantity)
            {
                Graphics g = this.CreateGraphics();
                RectangleF rf, rf2;
                rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Top, m_rect_QuantityIncreaseBounds.Left - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Height), ((ResultStruct)e._old).ToString(), this.Font);
                rf2 = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Top, m_rect_QuantityIncreaseBounds.Left - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Height), ((ResultStruct)e._new).ToString(), this.Font);
                this.Invalidate(new Rectangle((int)rf.X, (int)rf.Y, (int)rf.Width, (int)rf.Height));
                this.Invalidate(new Rectangle((int)rf2.X, (int)rf2.Y, (int)rf2.Width, (int)rf2.Height));
                g.Dispose();

                if (((ResultStruct)e._new).Quantity > ((ResultStruct)e._old).Quantity && ((ResultStruct)e._old).IsMin)
                    this.Invalidate(m_rect_QuantityDecreaseBounds);

                if (((ResultStruct)e._new).Quantity < ((ResultStruct)e._old).Quantity && ((ResultStruct)e._old).IsMax)
                    this.Invalidate(m_rect_QuantityIncreaseBounds);
            }
        }

        void RefreshArea(Area capturedArea)
        {
            switch (capturedArea)
            {
                case Area.ArrowLeft:
                    this.Invalidate(m_rect_QuantityDecreaseBounds);
                    break;
                case Area.ArrowRight:
                    this.Invalidate(m_rect_QuantityIncreaseBounds);
                    break;
            }
        }

        private void ResultSelector_MouseDown(object sender, MouseEventArgs e)
        {
            StickArea = GetStickArea(e.X, e.Y);
            CapturedArea = StickArea;
            if (CapturedArea == Area.OutOfBounds)
            {
                //this.m_selControl.ParentForm.Controls.Remove(this); //закрыть серектор!
                return;
            }
            if (CapturedArea != Area.None/* && CapturedArea != Area.OutOfBounds*/)
                this.RefreshArea(CapturedArea);
            if (CapturedArea == Area.ArrowLeft || CapturedArea == Area.ArrowRight)
            {
                ticked = false;
                timer.Interval = timer_first;
                timer.Enabled = true;
            }
        }

        private void ResultSelector_MouseUp(object sender, MouseEventArgs e)
        {
            if (CapturedArea == Area.ArrowLeft || CapturedArea == Area.ArrowRight)
            {
                timer.Enabled = false;
                if (ticked)
                {
                    Area old = CapturedArea;
                    CapturedArea = Area.None;
                    RefreshArea(old);
                    return;
                }
            }

            if (StickArea == Area.OutOfBounds)
            {
                CapturedArea = Area.None;
                //this.m_selControl.ParentForm.Controls.Remove(this); //закрыть серектор!
                return;
            }

            if (CapturedArea == Area.None/* || CapturedArea == Area.OutOfBounds*/)
                return;

            Area oldCapturedArea = CapturedArea;
            //ResultStruct oldResult = this.m_result.GetStruct(); - не нужно, см. OnResultChanged()

            bool choiseDone = (CapturedArea == StickArea);
            if (choiseDone)
            {
                switch (CapturedArea)
                {
                    case Area.ArrowLeft:
                        this.m_result.Quantity--;
                        break;
                    case Area.ArrowRight:
                        this.m_result.Quantity++;
                        break;
                }
            }

            CapturedArea = Area.None;

            if (choiseDone)
            {
                //this.RefreshArea(oldCapturedArea, true, oldResult); - перенес в OnResultChanged()
                this.RefreshArea(oldCapturedArea);
            }
        }

        private Area GetStickArea(int x, int y)
        {
            if (m_rect_QuantityDecreaseBounds.Contains(x, y))
                return Area.ArrowLeft;
            else if (m_rect_QuantityIncreaseBounds.Contains(x, y))
                return Area.ArrowRight;
            else if (m_rect_MainBorderBounds.Contains(x, y))
                return Area.None;
            else
                return Area.OutOfBounds;
        }

        private void ResultSelector_MouseMove(object sender, MouseEventArgs e)
        {
            if (CapturedArea == Area.None || CapturedArea == Area.OutOfBounds)
                return;

            Area oldStickArea = StickArea;
            StickArea = GetStickArea(e.X, e.Y);
            if (StickArea != oldStickArea && (StickArea == CapturedArea || oldStickArea == CapturedArea))
            {
                if (CapturedArea == Area.ArrowLeft || CapturedArea == Area.ArrowRight)
                    this.timer.Enabled = !this.timer.Enabled;

                this.RefreshArea(CapturedArea);
            }
        }
    }
}
