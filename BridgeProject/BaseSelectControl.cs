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
    public partial class BaseSelectControl : BaseControlInTable
    {
        // Координаты:
        protected Rectangle m_rect_BorderBounds;
        protected RectangleF m_rectf_String;
        protected Point[] m_points_Arrow;
        protected Rectangle m_rect_ArrowRect;

        // Параметры:
        protected int triangle_h = 4;        //высота треугольника
        protected int triangle_out = 4;      //горизонтальный отступ от обводки
        protected int triangle_bounds = 0;   //обводка квадрата стрелки
        protected int splitter_width = 2;    //ширина разделителя (текста слева и стрелки справа - только при фокусе!!!)
        protected int text_left_offset = 0;  // отступ текста слева (не считая 1 со всех сторон)
        protected int text_right_offset = 0; // отступ текста справа (не считая 1 со всех сторон)

        // Графич. ресурсы:
        static protected Pen m_pen_BorderBounds;
        static protected SolidBrush m_brush_String;
        static protected SolidBrush m_brush_String_BLACK;
        static protected SolidBrush m_brush_String_RED;
        static protected SolidBrush m_brush_String_GREEN;
        static protected SolidBrush m_brush_String_BLUE;
        static protected SolidBrush m_brush_Arrow;
        static protected SolidBrush m_brush_Arrow_Pressed;
        static protected SolidBrush m_brush_ArrowRect;
        static protected SolidBrush m_brush_ArrowRect_Pressed;
        static protected Pen m_pen_ArrowRectBounds;
        static protected Pen m_pen_ArrowRectBounds_Pressed;
        static protected Pen[] m_pens_Splitter;

        static BaseSelectControl()
        {
            LoadGraphicResources();
        }

        static public void LoadGraphicResources()
        {
            m_pen_BorderBounds = new Pen(SystemColors.WindowFrame);
            m_brush_String = new SolidBrush(SystemColors.ControlText);
            m_brush_String_RED = new SolidBrush(Color.Red);
            m_brush_String_BLACK = new SolidBrush(Color.Black);
            m_brush_String_GREEN = new SolidBrush(Color.Green);
            m_brush_String_BLUE = new SolidBrush(Color.Blue);
            m_brush_Arrow = new SolidBrush(Color.Black);
            m_brush_Arrow_Pressed = new SolidBrush(Color.White);
            m_brush_ArrowRect = new SolidBrush(Color.DarkGray);
            m_brush_ArrowRect_Pressed = new SolidBrush(Color.Gray);
            m_pen_ArrowRectBounds = new Pen(Color.Red);
            m_pen_ArrowRectBounds_Pressed = new Pen(Color.Blue);
            m_pens_Splitter = new Pen[] { new Pen(Color.FromArgb(213, 150, 0)), new Pen(Color.FromArgb(255, 184, 51)) }; //справа налево
        }



        protected Control ParentForm;
        protected Control SelectorCover;
        virtual public void AttachStuffForSelector(Control f, Control cov)
        {
            this.ParentForm = f;
            this.SelectorCover = cov;
        }
        public void DetachStuffForSelector()
        {
            this.ParentForm = null;
            this.SelectorCover = null;

            // Закрыть селектор, если он открыт подо мной:
            if (SelectorOpened)
                CloseSelector(false);
        }

        public BaseSelectControl()
        {
            InitializeComponent();
            SelectorCover = this;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            // рамка
            SmallHelper.DrawRect(g, m_pen_BorderBounds, m_rect_BorderBounds);

            // стрелка
            if (!_lock_)
            {
                if (triangle_bounds > 0)
                {
                    if (this.Focused || SelectorOpened)
                        SmallHelper.DrawRect(g, SelectorOpened ? m_pen_ArrowRectBounds_Pressed : m_pen_ArrowRectBounds, m_rect_ArrowRect);
                    SmallHelper.FillRectInside(g, SelectorOpened ? m_brush_ArrowRect_Pressed : m_brush_ArrowRect, m_rect_ArrowRect);
                }
                else
                {
                    if (this.Focused || SelectorOpened)
                    {
                        for (int i = 0; i < splitter_width; i++)
                            g.DrawLine(m_pens_Splitter[i >= m_pens_Splitter.Length ? m_pens_Splitter.Length - 1 : i], m_rect_ArrowRect.X - 1 - i, m_rect_BorderBounds.Top, m_rect_ArrowRect.X - 1 - i, m_rect_BorderBounds.Bottom - 1);
                    }
                    g.FillRectangle(SelectorOpened ? m_brush_ArrowRect_Pressed : m_brush_ArrowRect, m_rect_ArrowRect);
                }
                g.FillPolygon(SelectorOpened ? m_brush_Arrow_Pressed : m_brush_Arrow, m_points_Arrow);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            m_rect_BorderBounds = new Rectangle(-1, -1, this.Width + 2, this.Height + 2);

            int triangleP_width = (triangle_h * 2 - 1) + 2 * (triangle_out + triangle_bounds);
            int triangleP_height = m_rect_BorderBounds.Height - 2;
            int triangleP_x = (m_rect_BorderBounds.Right - 1) - triangleP_width;
            int triangleP_y = m_rect_BorderBounds.Top + 1;
            m_rect_ArrowRect = new Rectangle(triangleP_x, triangleP_y, triangleP_width, triangleP_height);

            int triangle_y1 = triangleP_y + (int)Math.Ceiling((double)(triangleP_height - triangle_h) / 2);
            int triangle_y2 = triangle_y1 + triangle_h;
            int triangle_x2 = (triangleP_x + triangleP_width) - (triangle_out + triangle_bounds);
            int triangle_x1 = triangle_x2 - (triangle_h * 2 - 1);
            int triangle_x3 = triangle_x1 + (triangle_h - 1);
            m_points_Arrow = new Point[] { new Point(triangle_x1, triangle_y1), new Point(triangle_x2, triangle_y1), new Point(triangle_x3, triangle_y2) };

            m_rectf_String = new RectangleF(1 + text_left_offset, 1, m_rect_ArrowRect.X - (1 + text_right_offset) - (1 + text_left_offset), this.Height - 2);
        }

        protected bool WasMouseDown;
        protected bool bSelectorOpened = false;
        public bool SelectorOpened
        {
            get
            {
                return bSelectorOpened;
            }
            set
            {
                bool old = bSelectorOpened;
                bSelectorOpened = value;
                if (old != value)
                {
                    this.Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();

            if (_lock_)
                return;

            WasMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_lock_)
                return;

            if (!WasMouseDown)
                return;
            else
                WasMouseDown = false;
            if (!new Rectangle(0, 0, this.Width, this.Height).Contains(e.X, e.Y))
                return;

            if (SelectorOpened == false)
                this.OpenSelector();
            else
                this.CloseSelector(true);
        }

        protected virtual void OpenSelector()
        { }

        protected virtual void CloseSelector(bool saveBeforeClose)
        { }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        /*private void BaseSelectControl_GotFocus(object sender, EventArgs e)
        {
        }

        private void BaseSelectControl_LostFocus(object sender, EventArgs e)
        {
            if (SelectorOpened == true)
                this.CloseSelector(true);
        }*/


        // ILock
        public override void OnLocked()
        {
            Invalidate();
        }

        public override void OnUnlocked()
        {
            Invalidate();
        }
    }



    //--------------------------------



        // SelectorClosingEvent >>>>>
        public class SelectorClosingEventArgs : EventArgs
        {
            public bool bSaveBeforeClose;
            public Object dataToSaveBeforeClosed;
            public SelectorClosingEventArgs(Object data, bool save)
            {
                this.dataToSaveBeforeClosed = data;
                this.bSaveBeforeClose = save;
            }
        }
        public delegate void SelectorClosingHandler(object sender, SelectorClosingEventArgs e);
        // <<<<<<<
}
