using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Imaging;

namespace BridgeProject
{
    public partial class ContractSelector : Control
    {
        Timer timer;
        int timer_first = 600;
        int timer_interval = 400;

        // Графич. ресурсы:
        static public Bitmap m_bmp_Arrow_Left;
        static public Bitmap m_bmp_Arrow_Left_Passive;
        static public Bitmap m_bmp_Arrow_Right;
        static public Bitmap m_bmp_Arrow_Right_Passive;
        static public Bitmap m_bmp_CuteBtn;
        static public Bitmap m_bmp_CuteBtn_Selected;
        static public Bitmap m_bmp_CuteBtn_Twice;
        static public Bitmap m_bmp_CuteBtn_Twice_Selected;
        static public Bitmap m_bmp_CuteBtn_Long;
        static public Bitmap m_bmp_CuteBtn_Long_Selected;
        static public Bitmap m_bmp_Check_False;
        static public Bitmap m_bmp_Check_True;
        static public Bitmap m_bmp_Check_Half;

        static public Pen m_pen_MainBounds1;
        static public Pen m_pen_MainBounds2;
        static public Pen m_pen_Arrows_NONACTIVE;
        static public Pen m_pen_Arrows_ACTIVE;
        static public Pen m_pen_Arrows_SELECTED;
        static public Pen m_pen_Arrows_REPEAT;
        static public SolidBrush m_brush_Arrows_NONACTIVE;
        static public SolidBrush m_brush_Arrows_ACTIVE;
        static public SolidBrush m_brush_Arrows_SELECTED;
        static public SolidBrush m_brush_Arrows_REPEAT;
        static public SolidBrush m_brush_String;
        static public SolidBrush m_brush_String_RED;
        static public SolidBrush m_brush_String_BLACK;
        static public SolidBrush m_brush_String_BLUE;
        static public SolidBrush m_brush_String_GREEN;
        static public SolidBrush m_brush_String_ANTIRED;
        static public SolidBrush m_brush_String_ANTIBLACK;
        static public SolidBrush m_brush_String_ANTIBLUE;
        static public SolidBrush m_brush_String_ANTIGREEN;
        static public Font quantityNum_font;
        static public Font suits_font;
        static public Font nt_font;
        static public Font re_contra_font;
        static public Font NO_font;

        public static void LoadGraphicsResources()
        {
            /*m_bmp_Arrow_Left = new Bitmap(Program.ExeDir + "Resources\\" + "arrow_left.bmp");
            m_bmp_Arrow_Left_Passive = new Bitmap(Program.ExeDir + "Resources\\" + "arrow_left_passive.bmp");
            m_bmp_Arrow_Right = new Bitmap(Program.ExeDir + "Resources\\" + "arrow_right.bmp");
            m_bmp_Arrow_Right_Passive = new Bitmap(Program.ExeDir + "Resources\\" + "arrow_right_passive.bmp");
            m_bmp_CuteBtn = new Bitmap(Program.ExeDir + "Resources\\" + "cute_button.bmp");
            m_bmp_CuteBtn_Selected = new Bitmap(Program.ExeDir + "Resources\\" + "cute_button_selected.bmp");
            m_bmp_CuteBtn_Twice = new Bitmap(Program.ExeDir + "Resources\\" + "cute_big_button.bmp");
            m_bmp_CuteBtn_Twice_Selected = new Bitmap(Program.ExeDir + "Resources\\" + "cute_big_button_selected.bmp");
            m_bmp_CuteBtn_Long = new Bitmap(Program.ExeDir + "Resources\\" + "cute_long_button.bmp");
            m_bmp_CuteBtn_Long_Selected = new Bitmap(Program.ExeDir + "Resources\\" + "cute_long_button_selected.bmp");
            m_bmp_Check_False = new Bitmap(Program.ExeDir + "Resources\\" + "check_false.bmp");
            m_bmp_Check_True = new Bitmap(Program.ExeDir + "Resources\\" + "check_true.bmp");
            m_bmp_Check_Half = new Bitmap(Program.ExeDir + "Resources\\" + "check_hz.bmp");*/


            m_bmp_Arrow_Left = BridgeProject.Properties.Resources.arrow_left;
            m_bmp_Arrow_Left_Passive = BridgeProject.Properties.Resources.arrow_left_passive;
            m_bmp_Arrow_Right = BridgeProject.Properties.Resources.arrow_right;
            m_bmp_Arrow_Right_Passive = BridgeProject.Properties.Resources.arrow_right_passive;
            m_bmp_CuteBtn = BridgeProject.Properties.Resources.cute_button;
            m_bmp_CuteBtn_Selected = BridgeProject.Properties.Resources.cute_button_selected;
            m_bmp_CuteBtn_Twice = BridgeProject.Properties.Resources.cute_big_button;
            m_bmp_CuteBtn_Twice_Selected = BridgeProject.Properties.Resources.cute_big_button_selected;
            m_bmp_CuteBtn_Long = BridgeProject.Properties.Resources.cute_long_button;
            m_bmp_CuteBtn_Long_Selected = BridgeProject.Properties.Resources.cute_long_button_selected;
            m_bmp_Check_False = BridgeProject.Properties.Resources.check_false;
            m_bmp_Check_True = BridgeProject.Properties.Resources.check_true;
            m_bmp_Check_Half = BridgeProject.Properties.Resources.check_hz;

            m_pen_MainBounds1 = new Pen(Color.FromArgb(213, 150, 0));
            m_pen_MainBounds2 = new Pen(Color.FromArgb(255, 184, 51));
            m_pen_Arrows_NONACTIVE = new Pen(Color.Transparent);
            m_pen_Arrows_ACTIVE = new Pen(Color.Gray);
            m_pen_Arrows_SELECTED = new Pen(Color.Gray);
            m_pen_Arrows_REPEAT = new Pen(Color.Gray);

            m_brush_Arrows_NONACTIVE = new SolidBrush(Color.Transparent);
            m_brush_Arrows_ACTIVE = new SolidBrush(Color.White);
            m_brush_Arrows_SELECTED = new SolidBrush(Color.DarkGray);
            m_brush_Arrows_REPEAT = new SolidBrush(Color.DarkRed);
            m_brush_String = new SolidBrush(SystemColors.ControlText);
            m_brush_String_RED = new SolidBrush(Color.Red);
            m_brush_String_BLACK = new SolidBrush(Color.Black);
            m_brush_String_BLUE = new SolidBrush(Color.Blue);
            m_brush_String_GREEN = new SolidBrush(Color.Green);
            m_brush_String_ANTIRED = new SolidBrush(SmallHelper.AntiColor(Color.Red));
            m_brush_String_ANTIBLACK = new SolidBrush(SmallHelper.AntiColor(Color.Black));
            m_brush_String_ANTIBLUE = new SolidBrush(SmallHelper.AntiColor(Color.Blue));
            m_brush_String_ANTIGREEN = new SolidBrush(SmallHelper.AntiColor(Color.Green));

            quantityNum_font = new Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            suits_font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular);
            nt_font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            re_contra_font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            NO_font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
        }



        // Статические координаты:
        static Rectangle m_rect_QuantityDecreaseBounds;
        static Rectangle m_rect_QuantityIncreaseBounds;
        static Rectangle m_rect_MainBorderBounds;
        static Rectangle m_rect_HeartsBounds;
        static Rectangle m_rect_DiamondsBounds;
        static Rectangle m_rect_ClubsBounds;
        static Rectangle m_rect_SpadesBounds;
        static Rectangle m_rect_NTBounds;
        static Rectangle m_rect_ContraBounds;
        static Rectangle m_rect_ReContraBounds;
        static Rectangle m_rect_NOCONTRACT;

        static Size static_size;
        static int arrow_offset;
        static Point trumps_pt;
        static float n_t_distance;
        static Point check_offset_img;
        static Point check_offset_text;
        static int voffset_NOCONTRACT;

        public static void ExtendedWidth(bool ext)
        {
            if (ext)
                static_size.Height = 192;
            else
                static_size.Height = 155;
            m_rect_MainBorderBounds = new Rectangle(0, 0, static_size.Width, static_size.Height);
        }

        public static void SetCoordinates()
        {
            static_size = new System.Drawing.Size(122, 192); //было 155

            m_rect_MainBorderBounds = new Rectangle(0, 0, static_size.Width, static_size.Height);
            m_rect_QuantityDecreaseBounds = new Rectangle(10, 6, 37, 26);
            m_rect_QuantityIncreaseBounds = new Rectangle(m_rect_MainBorderBounds.Right - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Y, m_rect_QuantityDecreaseBounds.Width, m_rect_QuantityDecreaseBounds.Height);
            arrow_offset = 10;
            trumps_pt = new Point(6, 37);
            n_t_distance = -10;
            m_rect_ContraBounds = new Rectangle(5, 103, 112, 21);
            m_rect_ReContraBounds = new Rectangle(5, 129, 112, 21);
            check_offset_img = new Point(0, -3);
            check_offset_text = new Point(-2, 0); // по х - по отношению к рисунку!
            voffset_NOCONTRACT = 5;

            // области козырей
            int suits_interval = m_bmp_CuteBtn_Twice.Height - 2 * m_bmp_CuteBtn.Height;
            m_rect_ClubsBounds = new Rectangle(trumps_pt.X, trumps_pt.Y, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height);
            m_rect_DiamondsBounds = new Rectangle(trumps_pt.X + m_bmp_CuteBtn.Width + suits_interval, trumps_pt.Y, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height);
            m_rect_HeartsBounds = new Rectangle(trumps_pt.X, trumps_pt.Y + m_bmp_CuteBtn.Height + suits_interval, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height);
            m_rect_SpadesBounds = new Rectangle(trumps_pt.X + m_bmp_CuteBtn.Width + suits_interval, trumps_pt.Y + m_bmp_CuteBtn.Height + suits_interval, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height);
            m_rect_NTBounds = new Rectangle(trumps_pt.X + 2 * m_bmp_CuteBtn.Width + 2 * suits_interval, trumps_pt.Y, m_bmp_CuteBtn_Twice.Width, m_bmp_CuteBtn_Twice.Height);

            // область "неконракта"
            m_rect_NOCONTRACT = new Rectangle((static_size.Width - m_bmp_CuteBtn_Long.Width) / 2, m_rect_ReContraBounds.Bottom + voffset_NOCONTRACT, m_bmp_CuteBtn_Long.Width, m_bmp_CuteBtn_Long.Height);
        }

        static ContractSelector()
        {
            LoadGraphicsResources();
            SetCoordinates();
        }

        enum Area { OutOfBounds = -1, None = 0, ArrowLeft, ArrowRight, Clubs, Spades, Hearts, Diamonds, NT, Contra, ReContra, NOCONTRACT };
        Area CapturedArea, StickArea;

        protected ContractSelector()
        {
            InitializeComponent();

            CloseMe(false);

            CapturedArea = Area.None;
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
        }

        static ContractSelector only_one = null;
        static public ContractSelector GetInstance()
        {
            if (only_one == null)
                only_one = new ContractSelector();
            return only_one;
        }

        // ----------------------------------------------------------------------------------------

        // !!!!!!!!
        // !!!!!!!! КАЖДОМУ ВНУТР. ЭЛЕМЕНТУ НАДО ДОБАВЛЯТЬ ТАКОЙ ОБРАБОТЧИК ДЛЯ LostFocus - CloseWhenLostFocus
        // !!!!!!!!

        Contract m_contract;
        ContractSelectControl m_selControl;
        Control m_attach_sel, m_attach_form;

        public void OpenMe(ContractSelectControl s, Control attach_sel, Control attach_form, Contract contractToCopy)
        {
            if (IsOpened())
                CloseMe(true);

            m_contract = new Contract(contractToCopy);
            m_contract.Born = true;
            m_contract.Changed += OnContractChanged;

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
            // !!!ВНИМАНИЕ!!!  ЗАКРЫТЬ СЕЛЕКТОР, ЕСЛИ ФОКУС НЕ СОДЕРЖИТ НИ САМ СЕЛЕКТОР, НИ КАКОЙ-ЛИБО ИЗ ЕГО ВНУТРЕННИХ КОНТРОЛОВ
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

        public bool IsAttachedToThisControl(ContractSelectControl csc)
        {
            return (m_selControl == csc);
        }
        public bool IsOpened()
        {
            return (m_attach_form != null && m_attach_form.Controls.Contains(this));
        }

        public event SelectorClosingHandler Closing;
        public event EventHandler Closed;

        public void CloseMe(bool saveBeforeClose)
        {
            if (this.m_contract != null)
            {
                this.m_contract.RemoveChangedHandlers();
            }

            if (Closing != null)
                Closing(this, new SelectorClosingEventArgs(m_contract, saveBeforeClose));
            m_contract = null;


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
            //ContractStruct oldCS = this.m_contract.GetStruct(); - не нужно, см. OnContractChanged()
            if (CapturedArea == Area.ArrowLeft)
                this.m_contract.Quantity--;
            else
                this.m_contract.Quantity++;
            RefreshArea(CapturedArea);
            //RefreshArea(CapturedArea, true, oldCS); - перенес в OnContractChanged()
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;
            RectangleF rf, rf2;

            // рамка
            SmallHelper.DrawRect(g, m_pen_MainBounds2, m_rect_MainBorderBounds);
            m_rect_MainBorderBounds.Inflate(-1, -1);
            SmallHelper.DrawRect(g, m_pen_MainBounds1, m_rect_MainBorderBounds);
            m_rect_MainBorderBounds.Inflate(1, 1);

            if (this.m_contract == null)
                return;

            // кнопка [<]
            SmallHelper.DrawRect(g, (this.m_contract.Quantity > 1) ? (CapturedArea == Area.ArrowLeft && StickArea == CapturedArea ? m_pen_Arrows_SELECTED : m_pen_Arrows_ACTIVE) : m_pen_Arrows_NONACTIVE, m_rect_QuantityDecreaseBounds);
            SmallHelper.FillRectInside(g, (this.m_contract.Quantity > 1) ? (CapturedArea == Area.ArrowLeft && StickArea == CapturedArea ? m_brush_Arrows_SELECTED : m_brush_Arrows_ACTIVE) : m_brush_Arrows_NONACTIVE, m_rect_QuantityDecreaseBounds);
            SmallHelper.DrawBmpTransp(g, (this.m_contract.Quantity > 1) ? m_bmp_Arrow_Left : m_bmp_Arrow_Left_Passive, new Point(m_rect_QuantityDecreaseBounds.Right - arrow_offset - m_bmp_Arrow_Left.Width, m_rect_QuantityDecreaseBounds.Top + (m_rect_QuantityDecreaseBounds.Height - m_bmp_Arrow_Left.Height) / 2));

            // кнопка [>]
            SmallHelper.DrawRect(g, (this.m_contract.Quantity < 7 && this.m_contract.Quantity != 0) ? (CapturedArea == Area.ArrowRight && StickArea == CapturedArea ? m_pen_Arrows_SELECTED : m_pen_Arrows_ACTIVE) : m_pen_Arrows_NONACTIVE, m_rect_QuantityIncreaseBounds);
            SmallHelper.FillRectInside(g, (this.m_contract.Quantity < 7 && this.m_contract.Quantity != 0) ? (CapturedArea == Area.ArrowRight && StickArea == CapturedArea ? m_brush_Arrows_SELECTED : m_brush_Arrows_ACTIVE) : m_brush_Arrows_NONACTIVE, m_rect_QuantityIncreaseBounds);
            SmallHelper.DrawBmpTransp(g, (this.m_contract.Quantity < 7 && this.m_contract.Quantity != 0) ? m_bmp_Arrow_Right : m_bmp_Arrow_Right_Passive, new Point(m_rect_QuantityIncreaseBounds.Left + arrow_offset, m_rect_QuantityIncreaseBounds.Top + (m_rect_QuantityIncreaseBounds.Height - m_bmp_Arrow_Right.Height) / 2));

            // цифра
            rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Top, m_rect_QuantityIncreaseBounds.Left - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Height), this.m_contract.GetString1(), quantityNum_font);
            g.DrawString(this.m_contract.GetString1(), quantityNum_font, m_brush_String, rf /*m_rect_QuantityNumber.X, m_rect_QuantityNumber.Y*/);

            // козыри
            rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_ClubsBounds.X, m_rect_ClubsBounds.Y, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height), "♣", suits_font);
            SmallHelper.DrawBmpTransp(g, (this.m_contract.Trump == CardTrump.Clubs || CapturedArea == Area.Clubs && StickArea == CapturedArea) ? m_bmp_CuteBtn_Selected : m_bmp_CuteBtn, m_rect_ClubsBounds.Location);
            g.DrawString("♣", suits_font, (this.m_contract.Trump == CardTrump.Clubs || CapturedArea == Area.Clubs && StickArea == CapturedArea) ? m_brush_String_ANTIBLACK : m_brush_String_BLACK, rf.X, rf.Y);

            rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_SpadesBounds.X, m_rect_SpadesBounds.Y, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height), "♠", suits_font);
            SmallHelper.DrawBmpTransp(g, (this.m_contract.Trump == CardTrump.Spades || CapturedArea == Area.Spades && StickArea == CapturedArea) ? m_bmp_CuteBtn_Selected : m_bmp_CuteBtn, m_rect_SpadesBounds.Location);
            g.DrawString("♠", suits_font, (this.m_contract.Trump == CardTrump.Spades || CapturedArea == Area.Spades && StickArea == CapturedArea) ? m_brush_String_ANTIBLACK : m_brush_String_BLACK, rf.X, rf.Y);

            rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_HeartsBounds.X, m_rect_HeartsBounds.Y, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height), "♥", suits_font);
            SmallHelper.DrawBmpTransp(g, (this.m_contract.Trump == CardTrump.Hearts || CapturedArea == Area.Hearts && StickArea == CapturedArea) ? m_bmp_CuteBtn_Selected : m_bmp_CuteBtn, m_rect_HeartsBounds.Location);
            g.DrawString("♥", suits_font, (this.m_contract.Trump == CardTrump.Hearts || CapturedArea == Area.Hearts && StickArea == CapturedArea) ? m_brush_String_ANTIRED : m_brush_String_RED, rf.X, rf.Y);

            rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_DiamondsBounds.X, m_rect_DiamondsBounds.Y, m_bmp_CuteBtn.Width, m_bmp_CuteBtn.Height), "♦", suits_font);
            SmallHelper.DrawBmpTransp(g, (this.m_contract.Trump == CardTrump.Diamonds || CapturedArea == Area.Diamonds && StickArea == CapturedArea) ? m_bmp_CuteBtn_Selected : m_bmp_CuteBtn, m_rect_DiamondsBounds.Location);
            g.DrawString("♦", suits_font, (this.m_contract.Trump == CardTrump.Diamonds || CapturedArea == Area.Diamonds && StickArea == CapturedArea) ? m_brush_String_ANTIRED : m_brush_String_RED, rf.X, rf.Y);

            rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_NTBounds.X, m_rect_NTBounds.Y, m_bmp_CuteBtn_Twice.Width, m_bmp_CuteBtn_Twice.Height), "N", nt_font);
            rf2 = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_NTBounds.X, m_rect_NTBounds.Y, m_bmp_CuteBtn_Twice.Width, m_bmp_CuteBtn_Twice.Height), "T", nt_font);
            float nt_height = rf.Height + n_t_distance + rf2.Height;
            float n_y = m_rect_NTBounds.Y + (m_bmp_CuteBtn_Twice.Height - nt_height) / 2;
            float t_y = n_y + rf.Height + n_t_distance;
            SmallHelper.DrawBmpTransp(g, (this.m_contract.Trump == CardTrump.NT || CapturedArea == Area.NT && StickArea == CapturedArea) ? m_bmp_CuteBtn_Twice_Selected : m_bmp_CuteBtn_Twice, m_rect_NTBounds.Location);
            g.DrawString("N", nt_font, (this.m_contract.Trump == CardTrump.NT || CapturedArea == Area.NT && StickArea == CapturedArea) ? m_brush_String_ANTIBLACK : m_brush_String_BLACK, rf.X, n_y /*m_rect_NTBounds.X + nt_offset[0].X, m_rect_NTBounds.Y + nt_offset[0].Y*/);
            g.DrawString("T", nt_font, (this.m_contract.Trump == CardTrump.NT || CapturedArea == Area.NT && StickArea == CapturedArea) ? m_brush_String_ANTIBLACK : m_brush_String_BLACK, rf2.X, t_y /*m_rect_NTBounds.X + nt_offset[1].X, m_rect_NTBounds.Y + nt_offset[1].Y*/);


            // контра
            SmallHelper.DrawBmpTransp(g, (CapturedArea == Area.Contra && StickArea == CapturedArea) ? m_bmp_Check_Half : (this.m_contract.Contra ? m_bmp_Check_True : m_bmp_Check_False), new Point(m_rect_ContraBounds.Left + check_offset_img.X, m_rect_ContraBounds.Top + check_offset_img.Y));
            g.DrawString("Double", re_contra_font, m_brush_String, m_rect_ContraBounds.Left + check_offset_img.X + m_bmp_Check_False.Width + check_offset_text.X, m_rect_ContraBounds.Top + check_offset_text.Y);

            // реконтра
            SmallHelper.DrawBmpTransp(g, (CapturedArea == Area.ReContra && StickArea == CapturedArea) ? m_bmp_Check_Half : (this.m_contract.ReContra ? m_bmp_Check_True : m_bmp_Check_False), new Point(m_rect_ReContraBounds.Left + check_offset_img.X, m_rect_ReContraBounds.Top + check_offset_img.Y));
            g.DrawString("Redouble", re_contra_font, m_brush_String, m_rect_ReContraBounds.Left + check_offset_img.X + m_bmp_Check_False.Width + check_offset_text.X, m_rect_ReContraBounds.Top + check_offset_text.Y);

            // неконтракт
            rf = SmallHelper.StringInMiddle(g, m_rect_NOCONTRACT, "ВСЕ ПАС", NO_font);
            SmallHelper.DrawBmpTransp(g, (CapturedArea == Area.NOCONTRACT && StickArea == CapturedArea) ? (this.m_contract.NoContract == true ? m_bmp_CuteBtn_Long : m_bmp_CuteBtn_Long_Selected) : (this.m_contract.NoContract == true ? m_bmp_CuteBtn_Long_Selected : m_bmp_CuteBtn_Long), m_rect_NOCONTRACT.Location);
            g.DrawString("ВСЕ ПАС", NO_font, (CapturedArea == Area.NOCONTRACT && StickArea == CapturedArea) ? (this.m_contract.NoContract == true ? m_brush_String_RED : m_brush_String_ANTIRED) : (this.m_contract.NoContract == true ? m_brush_String_ANTIRED : m_brush_String_RED), rf.X, rf.Y);

        }


        void OnContractChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            if (((ContractStruct)e._old).Quantity != ((ContractStruct)e._new).Quantity)
            {
                Graphics g = this.CreateGraphics();
                RectangleF rf, rf2;
                rf = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Top, m_rect_QuantityIncreaseBounds.Left - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Height), ((ContractStruct)e._old).GetString1(), quantityNum_font);
                rf2 = SmallHelper.StringInMiddle(g, new Rectangle(m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Top, m_rect_QuantityIncreaseBounds.Left - m_rect_QuantityDecreaseBounds.Right, m_rect_QuantityDecreaseBounds.Height), ((ContractStruct)e._new).GetString1(), quantityNum_font);
                this.Invalidate(new Rectangle((int)rf.X, (int)rf.Y, (int)rf.Width, (int)rf.Height));
                this.Invalidate(new Rectangle((int)rf2.X, (int)rf2.Y, (int)rf2.Width, (int)rf2.Height));
                g.Dispose();

                if (((ContractStruct)e._new).Quantity == 0 || ((ContractStruct)e._old).Quantity == 0)
                {
                    this.Invalidate(m_rect_QuantityDecreaseBounds);
                    this.Invalidate(m_rect_QuantityIncreaseBounds);
                }
                else
                {
                    if (((ContractStruct)e._new).Quantity > ((ContractStruct)e._old).Quantity && ((ContractStruct)e._old).Quantity == 1)
                        this.Invalidate(m_rect_QuantityDecreaseBounds);
                    if (((ContractStruct)e._new).Quantity < ((ContractStruct)e._old).Quantity && ((ContractStruct)e._old).Quantity == 7)
                        this.Invalidate(m_rect_QuantityIncreaseBounds);
                }
            }

            if (((ContractStruct)e._old).Trump != ((ContractStruct)e._new).Trump)
            {
                switch (((ContractStruct)e._old).Trump)
                {
                    case CardTrump.Clubs:
                        this.Invalidate(m_rect_ClubsBounds);
                        break;
                    case CardTrump.Diamonds:
                        this.Invalidate(m_rect_DiamondsBounds);
                        break;
                    case CardTrump.Hearts:
                        this.Invalidate(m_rect_HeartsBounds);
                        break;
                    case CardTrump.Spades:
                        this.Invalidate(m_rect_SpadesBounds);
                        break;
                    case CardTrump.NT:
                        this.Invalidate(m_rect_NTBounds);
                        break;
                }
            }

            if (((ContractStruct)e._old).Contra != ((ContractStruct)e._new).Contra)
            {
                this.Invalidate(new Rectangle(m_rect_ContraBounds.Left + check_offset_img.X, m_rect_ContraBounds.Top + check_offset_img.Y, m_bmp_Check_True.Width, m_bmp_Check_True.Height));
            }

            if (((ContractStruct)e._old).ReContra != ((ContractStruct)e._new).ReContra)
            {
                this.Invalidate(new Rectangle(m_rect_ReContraBounds.Left + check_offset_img.X, m_rect_ReContraBounds.Top + check_offset_img.Y, m_bmp_Check_True.Width, m_bmp_Check_True.Height));
            }

            if (((ContractStruct)e._old).NoContract != ((ContractStruct)e._new).NoContract)
            {
                this.Invalidate(m_rect_NOCONTRACT);
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
                case Area.Clubs:
                    this.Invalidate(m_rect_ClubsBounds);
                    break;
                case Area.Diamonds:
                    this.Invalidate(m_rect_DiamondsBounds);
                    break;
                case Area.Hearts:
                    this.Invalidate(m_rect_HeartsBounds);
                    break;
                case Area.Spades:
                    this.Invalidate(m_rect_SpadesBounds);
                    break;
                case Area.NT:
                    this.Invalidate(m_rect_NTBounds);
                    break;
                case Area.Contra:
                    this.Invalidate(new Rectangle(m_rect_ContraBounds.Left + check_offset_img.X, m_rect_ContraBounds.Top + check_offset_img.Y, m_bmp_Check_True.Width, m_bmp_Check_True.Height));
                    break;
                case Area.ReContra:
                    this.Invalidate(new Rectangle(m_rect_ReContraBounds.Left + check_offset_img.X, m_rect_ReContraBounds.Top + check_offset_img.Y, m_bmp_Check_True.Width, m_bmp_Check_True.Height));
                    break;
                case Area.NOCONTRACT:
                    this.Invalidate(m_rect_NOCONTRACT);
                    break;
            }
        }

        private void ContractSelector_MouseDown(object sender, MouseEventArgs e)
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

        private void ContractSelector_MouseUp(object sender, MouseEventArgs e)
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
            //ContractStruct oldContract = this.m_contract.GetStruct(); - не нужно, см. OnContractChanged()

            bool choiseDone = (CapturedArea == StickArea);
            if (choiseDone)
            {
                switch (CapturedArea)
                {
                    case Area.ArrowLeft:
                        this.m_contract.Quantity--;
                        break;
                    case Area.ArrowRight:
                        this.m_contract.Quantity++;
                        break;
                    case Area.Clubs:
                        this.m_contract.Trump = CardTrump.Clubs;
                        break;
                    case Area.Diamonds:
                        this.m_contract.Trump = CardTrump.Diamonds;
                        break;
                    case Area.Hearts:
                        this.m_contract.Trump = CardTrump.Hearts;
                        break;
                    case Area.Spades:
                        this.m_contract.Trump = CardTrump.Spades;
                        break;
                    case Area.NT:
                        this.m_contract.Trump = CardTrump.NT;
                        break;
                    case Area.Contra:
                        this.m_contract.Contra = !this.m_contract.Contra;
                        break;
                    case Area.ReContra:
                        this.m_contract.ReContra = !this.m_contract.ReContra;
                        break;
                    case Area.NOCONTRACT:
                        this.m_contract.NoContract = !this.m_contract.NoContract;
                        break;
                }
            }

            CapturedArea = Area.None;

            if (choiseDone)
            {
                this.RefreshArea(oldCapturedArea);
            }
        }

        private Area GetStickArea(int x, int y)
        {
            if (m_rect_QuantityDecreaseBounds.Contains(x, y))
                return Area.ArrowLeft;
            else if (m_rect_QuantityIncreaseBounds.Contains(x, y))
                return Area.ArrowRight;
            else if (m_rect_HeartsBounds.Contains(x, y))
                return Area.Hearts;
            else if (m_rect_DiamondsBounds.Contains(x, y))
                return Area.Diamonds;
            else if (m_rect_ClubsBounds.Contains(x, y))
                return Area.Clubs;
            else if (m_rect_SpadesBounds.Contains(x, y))
                return Area.Spades;
            else if (m_rect_NTBounds.Contains(x, y))
                return Area.NT;
            else if (m_rect_ContraBounds.Contains(x, y))
                return Area.Contra;
            else if (m_rect_ReContraBounds.Contains(x, y))
                return Area.ReContra;
            else if (m_rect_NOCONTRACT.Contains(x, y))
                return Area.NOCONTRACT;
            else if (m_rect_MainBorderBounds.Contains(x, y))
                return Area.None;
            else
                return Area.OutOfBounds;
        }

        private void ContractSelector_MouseMove(object sender, MouseEventArgs e)
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
