using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BridgeProject
{
    public partial class ControlCover : UserControl
    {
        public ControlCover()
        {
            InitializeComponent();

            m_bBorder = false;
            m_lock = true;
            m_iBorderWidth = 2;
            pens_Border = new Pen[] { new Pen(Color.FromArgb(213, 150, 0)), new Pen(Color.FromArgb(255, 184, 51)) };
            //pens_Border_UNLOCKED = new Pen[] { new Pen(Color.FromArgb(6, 111, 37)), new Pen(Color.FromArgb(23, 210, 172)) };
            pens_Border_LOCKED = new Pen[] { new Pen(Color.FromArgb(253, 53, 53)), new Pen(Color.FromArgb(253, 122, 122)) };
        }

        int m_iBorderWidth;
        bool m_bBorder;
        bool m_lock;
        Pen[] pens_Border;
        Pen[] pens_Border_UNLOCKED;
        Pen[] pens_Border_LOCKED;

        public void AttachControl(Control c)
        {
            if(this.Controls.Count > 0)
                return;

            // Определить состояние и цвет (lock/unlock) обводки
            if (c.GetType().GetInterfaces().Contains(typeof(ILock)))
                m_lock = (c as ILock).Lock;
            else
                m_lock = true;

            if (c.GetType().GetInterfaces().Contains(typeof(IControlInTable)))
                m_bBorder = (c as IControlInTable).IsActiveInTable();
            else
                m_bBorder = false;

            // Установить координаты:
            if (m_bBorder)
            {
                this.Location = new Point(c.Location.X - m_iBorderWidth, c.Location.Y - m_iBorderWidth);
                this.Size = new Size(c.Size.Width + 2 * m_iBorderWidth, c.Size.Height + 2 * m_iBorderWidth);
                c.Location = new Point(m_iBorderWidth, m_iBorderWidth);
            }
            else
            {
                this.Location = c.Location;
                this.Size = c.Size;
                c.Location = new Point(0, 0);
            }
            this.Controls.Add(c);
            if (m_bBorder)
            {
                this.Invalidate();
            }

            // Добавить обработчики:
            c.Resize += OnControlResize;
            if (c.GetType().GetInterfaces().Contains(typeof(ILock)))
            {
                (c as ILock).Locked += AttachedControl_Locked;
                (c as ILock).Unlocked += AttachedControl_Unlocked;
            }
            if (c.GetType().GetInterfaces().Contains(typeof(IControlInTable)))
            {
                (c as IControlInTable).GotActive += AttachedControl_GotActive;
                (c as IControlInTable).LostActive += AttachedControl_LostActive;
            }


            //------------------------------------------------------ old -------------------------------------------
            /*
            this.Location = c.Location;
            c.Location = new Point(0, 0);
            this.Controls.Add(c);
            this.Size = this.Controls[0].Size;
            c.Resize += OnControlResize;

            m_lock = true;
            if (c.GetType().GetInterfaces().Contains(typeof(ILock)))
            {
                m_lock = (c as ILock).Lock;

                (c as ILock).Locked += AttachedControl_Locked;
                (c as ILock).Unlocked += AttachedControl_Unlocked;
            }

            m_bBorder = false;
            if (c.GetType().GetInterfaces().Contains(typeof(IControlInTable)))
            {
                if ((c as IControlInTable).IsActiveInTable == true)
                {
                    AttachedControl_GotActive(null, null);
                }

                (c as IControlInTable).GotActive += AttachedControl_GotActive;
                (c as IControlInTable).LostActive += AttachedControl_LostActive;
            }
            */
        }

        void OnControlResize(object sender, EventArgs e)
        {
            if(m_bBorder)
                this.Size = new Size(this.Controls[0].Size.Width + 2 * m_iBorderWidth, this.Controls[0].Size.Height + 2 * m_iBorderWidth);
            else
                this.Size = this.Controls[0].Size;
        }

        public void DetachControl()
        {
            if(this.Controls.Count > 0)
            {
                if (this.Controls[this.Controls.Count - 1].GetType().GetInterfaces().Contains(typeof(IControlInTable)))
                {
                    (this.Controls[this.Controls.Count - 1] as IControlInTable).GotActive -= AttachedControl_GotActive;
                    (this.Controls[this.Controls.Count - 1] as IControlInTable).LostActive -= AttachedControl_LostActive;
                }
                if (this.Controls[this.Controls.Count - 1].GetType().GetInterfaces().Contains(typeof(ILock)))
                {
                    (this.Controls[this.Controls.Count - 1] as ILock).Locked -= AttachedControl_Locked;
                    (this.Controls[this.Controls.Count - 1] as ILock).Unlocked -= AttachedControl_Unlocked;
                }
                this.Controls[this.Controls.Count - 1].Resize -= OnControlResize;
                this.Controls.Clear();
            }
        }
        
        public void Move(Point pt)
        {
            if (this.Controls.Count == 0)
                return;

            if (!m_bBorder)
                this.Location = pt;
            else
                this.Location = new Point(pt.X - m_iBorderWidth, pt.Y - m_iBorderWidth);
        }

        void AttachedControl_GotActive(object sender, EventArgs e)
        {
            m_bBorder = true;
            this.BringToFront();
            this.Location = new Point(this.Location.X - m_iBorderWidth, this.Location.Y - m_iBorderWidth);
            this.Size = new Size(this.Size.Width + 2*m_iBorderWidth, this.Size.Height + 2*m_iBorderWidth);
            this.Controls[0].Location = new Point(m_iBorderWidth, m_iBorderWidth);
        }

        void AttachedControl_LostActive(object sender, EventArgs e)
        {
            m_bBorder = false;
            this.Size = new Size(this.Size.Width - 2 * m_iBorderWidth, this.Size.Height - 2 * m_iBorderWidth);
            this.Location = new Point(this.Location.X + m_iBorderWidth, this.Location.Y + m_iBorderWidth);
            this.Controls[0].Location = new Point(0, 0);
        }


        void AttachedControl_Locked(object sender, EventArgs e)
        {
            m_lock = true;
            if (m_bBorder)
            {
                this.Invalidate();
            }
        }

        void AttachedControl_Unlocked(object sender, EventArgs e)
        {
            m_lock = false;
            if (m_bBorder)
            {
                this.Invalidate();
            }
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            if (m_bBorder)
            {
                Pen[] pens = (m_lock ? pens_Border_LOCKED : pens_Border);
                if (this.Controls[0].GetType() == typeof(BridgeProject.RobberScoreControl)  ||  this.Controls[0].GetType().IsSubclassOf(typeof(BridgeProject.RobberScoreControl)))
                    pens = pens_Border;
                if (this.Controls[0].GetType() == typeof(BridgeProject.DealInfoControl) || this.Controls[0].GetType().IsSubclassOf(typeof(BridgeProject.DealInfoControl)))
                    pens = pens_Border;

                for (int i = 0; i < m_iBorderWidth; i++)
                {
                    SmallHelper.DrawRect(g, pens[m_iBorderWidth - 1 - i], new Rectangle(i, i, this.Width - 2 * i, this.Height - 2 * i));
                }
            }
        }
    }
}
