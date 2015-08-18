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
    public partial class ShporaControl : Control
    {
        public void __LoadDataInTable__(ArrayList arr_data, int data_columns, ArrayList arr_joints, ArrayOfInt widths, ArrayOfInt heights, int headerLinesUp, int headerLinesLeft, int data_height)
        {
            // Таблица
            DATA__arr = arr_data;
            DATA__columns = data_columns;

            // Объединения
            DATA__joints = arr_joints;

            // Отображение таблицы
            PAINT__columns_widths = widths;
            PAINT__columns_heights = heights;
            PAINT__data_height = data_height;
            PAINT__headerLinesUp = headerLinesUp;
            PAINT__headerLinesLeft = headerLinesLeft;


            // Определение размеров
            total_width = 0;
            for (int i = 0; i < PAINT__columns_widths.Count; i++)
                total_width += (PAINT__columns_widths[i] + 2 * 1);
            total_width += (DATA__columns - 1) * PAINT__border_delay + 2 * (PAINT__border_delay + 1);
            total_height = 0;
            for (int i = 0; i < DATA__arr.Count; i++)
                total_height += (i < PAINT__columns_heights.Count ? PAINT__columns_heights[i] : PAINT__data_height) + 2 * 1;
            total_height += (DATA__arr.Count - 1) * PAINT__border_delay + 2 * (PAINT__border_delay + 1);

            // Новые размеры:
            this.Width = total_width;
            this.Height = total_height;
        }




        public ShporaControl()
        {
            InitializeComponent();

            // Граф. объекты
            m_pen_Black = new Pen(Color.Black, 1);
            m_brush = new SolidBrush(Color.FromArgb(230, 230, 230));
            m_brush1 = new SolidBrush(Color.FromArgb(196, 226, 255));
            m_brush2 = new SolidBrush(Color.FromArgb(196, 204, 255));
            m_brushH = new SolidBrush(Color.LightPink);
            m_brush_Font = new SolidBrush(Color.Black);
            FontBold = new Font(this.Font.Name, this.Font.Size, FontStyle.Bold);
        }

        ArrayList DATA__arr;
        int DATA__columns;

        public Font FontBold;
        ArrayOfInt PAINT__columns_widths;
        ArrayOfInt PAINT__columns_heights;
        int PAINT__data_height;
        int PAINT__border_delay = 1;
        int PAINT__headerLinesUp;
        int PAINT__headerLinesLeft;

        int total_width = 0;
        int total_height = 0;


        //Объединения
        public struct table_joint
        {
            public int x;
            public int y;
            public int width;
            public int height;
            public table_joint(int x, int y, int width, int height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }
        };
        ArrayList DATA__joints;
        bool GetJoinInfo(int x, int y, out bool join_first, out int join_width, out int join_height)
        {
            bool simple = true;
            join_first = false;
            join_width = 1;
            join_height = 1;
            for (int i = 0; i < DATA__joints.Count; i++)
            {
                if (x >= ((table_joint)DATA__joints[i]).x && x < (((table_joint)DATA__joints[i]).x + ((table_joint)DATA__joints[i]).width) && y >= ((table_joint)DATA__joints[i]).y && y < (((table_joint)DATA__joints[i]).y + ((table_joint)DATA__joints[i]).height))
                {
                    simple = false;
                    join_width = ((table_joint)DATA__joints[i]).width;
                    join_height = ((table_joint)DATA__joints[i]).height;
                    if (x == ((table_joint)DATA__joints[i]).x && y == ((table_joint)DATA__joints[i]).y)
                        join_first = true;
                    break;
                }
            }
            return (!simple);
        }



        Pen m_pen_Black;
        SolidBrush m_brush_Font;
        SolidBrush m_brush;
        SolidBrush m_brush1;
        SolidBrush m_brush2;
        SolidBrush m_brushH;


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;

            // [delme!!!] Заполнить чем-то всю область
            g.FillRectangle(new SolidBrush(Color.Gray), this.ClientRectangle);

            // Граница
            SmallHelper.DrawRect(g, m_pen_Black, new Rectangle(0, 0, total_width, total_height));
            SmallHelper.FillRectInside(g, m_brush, new Rectangle(0, 0, total_width, total_height));

            // Элементы
            Rectangle r = new Rectangle();
            RectangleF rf;
            int jw = 1, jh = 1;
            bool jf = false;
            object o;
            for (int i = 0; i < DATA__arr.Count; i++)
            {
                for (int j = 0; j < DATA__columns; j++)
                {
                    if (GetJoinInfo(j, i, out jf, out jw, out jh) == true)
                    {
                        if (!jf)
                        {
                            continue;
                        }
                    }

                    r.Width = (jw - 1) * PAINT__border_delay + 2 * jw;
                    for (int x = j; x < (j + jw); x++)
                        r.Width += PAINT__columns_widths[x];
                    r.Height = (jh - 1) * PAINT__border_delay + 2 * jh;
                    for (int x = i; x < (i + jh); x++)
                        r.Height += (x < PAINT__columns_heights.Count ? PAINT__columns_heights[x] : PAINT__data_height);
                    r.X = (1 + PAINT__border_delay);
                    for (int x = 0; x < j; x++)
                        r.X += (PAINT__columns_widths[x] + 1 + PAINT__border_delay + 1);
                    r.Y = (1 + PAINT__border_delay);
                    for (int x = 0; x < i; x++)
                        r.Y += (x < PAINT__columns_heights.Count ? PAINT__columns_heights[x] : PAINT__data_height) + 1 + PAINT__border_delay + 1;
                    SmallHelper.DrawRect(g, m_pen_Black, r);
                    bool isHeader = (i < PAINT__headerLinesUp) || (j < PAINT__headerLinesLeft);
                    SmallHelper.FillRectInside(g, (isHeader ? m_brushH : ((i - PAINT__headerLinesUp) % 2 == 0 ? m_brush1 : m_brush2)), r);
                    o = ((ArrayList)DATA__arr[i])[j];
                    rf = SmallHelper.StringInMiddle(g, r, (o == null ? "" : o.ToString()), (isHeader ? this.FontBold : this.Font));
                    SmallHelper.DrawMultiString(g, r, (o == null ? "" : o.ToString()), (isHeader ? this.FontBold : this.Font), m_brush_Font, StringAlignment.Center, StringAlignment.Center, StringAlignment.Center, 0, 0, -5);
                    //g.DrawString((o == null ? "" : o.ToString()), (isHeader ? this.FontBold : this.Font), m_brush_Font, rf);
                }
            }

            g.Dispose();
        }
    }
}
