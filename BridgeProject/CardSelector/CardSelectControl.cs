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
    public enum CardValue { Unknown = 0, Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace };
    public enum CardSuit { Unknown = 0, Hearts, Diamonds, Clubs, Spades };
    public struct Card
    {
        public CardValue value;
        public CardSuit suit;
        public Card(CardValue v, CardSuit s)
        {
            value = v;
            suit = s;
        }
        public String GetValueString()
        {
            String s = "";
            switch (value)
            {
                case CardValue.Two:
                case CardValue.Three:
                case CardValue.Four:
                case CardValue.Five:
                case CardValue.Six:
                case CardValue.Seven:
                case CardValue.Eight:
                case CardValue.Nine:
                case CardValue.Ten:
                    s += ((int)value).ToString();
                    break;
                case CardValue.Jack:
                    s += "J";
                    break;
                case CardValue.Queen:
                    s += "Q";
                    break;
                case CardValue.King:
                    s += "K";
                    break;
                case CardValue.Ace:
                    s += "A";
                    break;
                case CardValue.Unknown:
                    break;
                default:
                    s += "?";
                    break;
            }
            return s;
        }
        public String GetSuitString()
        {
            String s = "";
            switch (suit)
            {
                case CardSuit.Hearts:
                    s += "♥";
                    break;
                case CardSuit.Diamonds:
                    s += "♦";
                    break;
                case CardSuit.Clubs:
                    s += "♣";
                    break;
                case CardSuit.Spades:
                    s += "♠";
                    break;
                case CardSuit.Unknown:
                    break;
                default:
                    s += "?";
                    break;
            }
            return s;
        }
    };

    public partial class CardSelectControl : Control
    {
        Pen m_pen_Bounds;
        SolidBrush m_brush_String;
        SolidBrush m_brush_String_RED;
        SolidBrush m_brush_String_BLACK;
        SolidBrush m_brush_String_FOCUS;
        SolidBrush m_brush_Arrow;
        SolidBrush m_brush_ArrowRect;
        Pen m_pen_ArrowRectBounds;
        SolidBrush m_brush_FocusRect;

        Rectangle m_rect_BorderBounds;
        Point[] m_points_Arrow;
        Rectangle m_rect_ArrowRect;
        Rectangle m_rect_ArrowRectBounds;
        Rectangle m_rect_FocusRect;
        RectangleF m_rectf_String;

        public Card m_card;
        CardSelector selector;
        bool SelectorOpened;

        public CardSelectControl()
        {
            InitializeComponent();
            SelectorOpened = false;

            m_pen_Bounds = new Pen(SystemColors.WindowFrame);
            m_brush_String = new SolidBrush(SystemColors.ControlText);
            m_brush_String_FOCUS = new SolidBrush(Color.White);
            m_brush_String_RED = new SolidBrush(Color.Red);
            m_brush_String_BLACK = new SolidBrush(Color.Black);
            m_brush_Arrow = new SolidBrush(SystemColors.WindowText);
            m_brush_ArrowRect = new SolidBrush(SystemColors.ControlDark);
            m_pen_ArrowRectBounds = new Pen(this.BackColor);
            m_brush_FocusRect = new SolidBrush(SystemColors.ActiveCaption);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            // рамка
            g.DrawRectangle(m_pen_Bounds, m_rect_BorderBounds);

            // стрелка
            if (this.SelectorOpened)
            {
                g.FillRectangle(m_brush_ArrowRect, m_rect_ArrowRect);
                g.DrawRectangle(m_pen_ArrowRectBounds, m_rect_ArrowRectBounds);
            }
            g.FillPolygon(m_brush_Arrow, m_points_Arrow);

            // фокус
            if (this.Focused && !this.SelectorOpened)
            {
                g.FillRectangle(m_brush_FocusRect, m_rect_FocusRect);
            }

            // текст
            g.DrawString(m_card.GetValueString(), this.Font, this.Focused && !this.SelectorOpened ? m_brush_String_FOCUS : m_brush_String, m_rectf_String, new StringFormat(StringFormatFlags.NoWrap));
            SizeF sizeValStr = g.MeasureString(m_card.GetValueString(), this.Font);
            RectangleF rectfSuitStr = m_rectf_String;
            rectfSuitStr.X += m_card.GetValueString().Length * 10;  // sizeValStr.Width неточен, а других вариантов в .NET CF нет
            g.DrawString(m_card.GetSuitString(), this.Font, this.Focused && !this.SelectorOpened ? m_brush_String_FOCUS : (m_card.suit == CardSuit.Hearts || m_card.suit == CardSuit.Diamonds) ? m_brush_String_RED : m_brush_String_BLACK, rectfSuitStr, new StringFormat(StringFormatFlags.NoWrap));
        }

        private void CardSelectControl_Resize(object sender, EventArgs e)
        {
            m_rect_BorderBounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            int triangle_h = 4;  //высота треугольника
            int triangle_out = 3;  //отступ справа, не считая обводку
            int triangle_y1 = (int)Math.Ceiling((double)(this.Height - triangle_h) / 2);
            int triangle_y2 = triangle_y1 + triangle_h;
            int triangle_x2 = this.Width - 1 - triangle_out;
            int triangle_x1 = triangle_x2 - (triangle_h * 2 - 1);
            int triangle_x3 = triangle_x1 + (triangle_h - 1);
            m_points_Arrow = new Point[] { new Point(triangle_x1, triangle_y1), new Point(triangle_x2, triangle_y1), new Point(triangle_x3, triangle_y2) };
            int triangleP_width = (triangle_h * 2 - 1) + 2 * (triangle_out);
            int triangleP_height = this.Height - 2;
            int triangleP_x = this.Width - 1 - triangleP_width;
            int triangleP_y = 1;
            m_rect_ArrowRect = new Rectangle(triangleP_x, triangleP_y, triangleP_width, triangleP_height);
            m_rect_ArrowRectBounds = new Rectangle(triangleP_x, triangleP_y, triangleP_width - 1, triangleP_height - 1);

            int focus_width = this.Width - 2 - triangleP_width - 3;
            int focus_height = this.Height - 2 - 2;
            int focus_x = 3;
            int focus_y = 2;
            m_rect_FocusRect = new Rectangle(focus_x, focus_y, focus_width, focus_height);

            m_rectf_String = m_rect_FocusRect;
            m_rectf_String.Y -= 1;

        }

        bool WasMouseDown;

        private void CardSelectControl_MouseDown(object sender, MouseEventArgs e)
        {
            WasMouseDown = true;
            //this.Focus();
        }

        private void CardSelectControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (!WasMouseDown)
                return;
            else
                WasMouseDown = false;

            this.Focus();

            if (!this.Parent.Controls.Contains(this.selector))
            {
                this.selector.DefineLocation();
                this.Parent.Controls.Add(this.selector);
                this.selector.BringToFront();
                //this.selector.Capture = true;

                this.Parent.MouseDown += new MouseEventHandler(ParentForm_MouseDown); // + слежка за клацаньем по путому месту формы
                
                // !!!!!!
                // Можно сделать Control[], за которыми будет слежка
                // !!!!!!
                
                this.SelectorOpened = true;
                this.Invalidate();
            }
            else
            {
                this.Parent.Controls.Remove(this.selector);

                this.Parent.MouseDown -= new MouseEventHandler(ParentForm_MouseDown); // - слежка за клацаньем по путому месту формы

                this.SelectorOpened = false;
                this.Invalidate();
            }
        }

        void ParentForm_MouseDown(object sender, MouseEventArgs e)
        {
            Program.MainForm.Text = "form  " + e.X.ToString() + " , " + e.Y.ToString();

            if (this.Parent.Controls.Contains(this.selector))
                this.Parent.Controls.Remove(this.selector);

            this.Parent.MouseDown -= new MouseEventHandler(ParentForm_MouseDown); // - слежка за клацаньем по путому месту формы
        }

        private void CardSelectControl_KeyPress(object sender, KeyPressEventArgs e)
        { }

        private void CardSelectControl_GotFocus(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void CardSelectControl_LostFocus(object sender, EventArgs e)
        {
            if (this.Parent.Controls.Contains(this.selector))
                this.Parent.Controls.Remove(this.selector);

            this.Parent.MouseDown -= new MouseEventHandler(ParentForm_MouseDown); // - слежка за клацаньем по путому месту формы

            SelectorOpened = false;
            this.Refresh();
        }
    }
}
