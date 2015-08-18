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
    public struct IntDataStruct
    {
        public bool bMin, bMax;
        public int iMin, iMax;
        public bool IsInInterval(int v)
        {
            if (bMin && v < iMin)
                return false;
            if (bMax && v > iMax)
                return false;
            return true;
        }

        int iVal;
        public int IntVal
        {
            get
            {
                return this.iVal;
            }
            set
            {
                // Проверить [iMin;iMax]
                if (IsInInterval(value))
                {
                    this.iVal = value;
                    this.born = true;
                }
            }
        }
        bool born;
        public bool Born
        {
            get
            {
                return this.born;
            }
            set
            {
                if (value == false)
                    this.born = value;
            }
        }
        public override String ToString()
        {
            return (born ? iVal.ToString() : "");
        }
        public static bool operator ==(IntDataStruct i1, IntDataStruct i2)
        {
            return i1.Equals(i2);
        }
        public static bool operator !=(IntDataStruct i1, IntDataStruct i2)
        {
            return !i1.Equals(i2);
        }
        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;
            IntDataStruct d = (IntDataStruct) obj;
            return (this.iVal == d.iVal && this.born == d.born);
        }
    }
    
    public class IntData : BaseChangedData, IFromString, ISQLSerialize
    {
        IntDataStruct value;

        public IntData()
        {
        }

        // Сериализация SQL
        public void _FromDataBase(object v)
        {
            if (v == DBNull.Value)
                this.Born = false;
            else
                if (v.GetType() == typeof(System.Boolean))
                    this.Value = ((bool)v == true ? 1 : 0);
                else if (v.GetType() == typeof(System.Byte))
                    this.Value = (int)(byte)v;
                else
                    this.Value = (int)v;
        }
        public object _ToDataBase()
        {
            if (this.IsDefined())
                return this.Value;
            else
                return DBNull.Value;
        }

        // Clear
        public override void Clear()
        {
            IntDataStruct old = this.value;

            this.value.IntVal = 0;
            this.value.Born = false;

            if (old != this.value && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(old, this.value));
        }


        // Min & Max & +1
        bool plus1;
        public void Constructor(bool plus1)
        {
            this.plus1 = plus1;
        }
        public void Constructor(bool bmin, int min, bool bmax, int max)
        {
            value.iMin = min;
            value.iMax = max;
            value.bMin = bmin;
            value.bMax = bmax;
        }
        public void Constructor(bool plus1, bool bmin, int min, bool bmax, int max)
        {
            value.iMin = min;
            value.iMax = max;
            value.bMin = bmin;
            value.bMax = bmax;

            this.plus1 = plus1;
        }

        public bool FromString(String str)
        {
            try
            {
                int newVal = Int32.Parse(str);
                IntDataStruct old = this.value;
                Value = newVal;
                if (old.IntVal != newVal && old == this.value) //если значение не изменилось, хотя должно было (т.е. не попало в интервал [min;max])
                    return false;
                else
                    return true;
            }
            catch (FormatException e)
            {
                //Born = false; //----????????
                return false;
            }
        }

        public int Value
        {
            get
            {
                return this.value.IntVal;
            }
            set
            {
                IntDataStruct old = this.value;

                this.value.IntVal = value;
                //this.value.Born = true;

                if(old != this.value && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(old, this.value));
            }
        }

        public bool Born
        {
            get
            {
                return this.value.Born;
            }
            set
            {
                IntDataStruct old = this.value;

                this.value.Born = value;

                if (old != this.value && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(old, this.value));
            }
        }

        public override bool IsDefined()
        {
            return this.value.Born;
        }

        public override String ToString()
        {
            //return this.value.ToString();
            return (this.Born ? (plus1 ? (Value + 1).ToString() : Value.ToString()) : "");
        }
    }



    public partial class ShowTextControl : BaseControlInTable, IAttachData<BaseChangedData>, IDetachData
    {
        protected BaseChangedData m_value;

        SolidBrush m_brush_String = new SolidBrush(SystemColors.ControlText);

        int offset_x = 1;
        int interval_y = 0;
        StringAlignment align_x = StringAlignment.Near;
        StringAlignment align_x2 = StringAlignment.Near;
        Font[] strFonts;
        Brush[] strBrushes;

        public void SetTextFonts(Font[] fonts)
        {
            if (strFonts != null)
            {
                for (int i = 0; i < strFonts.Length; i++)
                    strFonts[i].Dispose();
            }

            if (fonts != null)
            {
                for (int i = 0; i < fonts.Length; i++)
                {
                    if (fonts[i] == null)
                        fonts[i] = this.Font;
                }
            }
            strFonts = fonts;
        }
        public void SetTextBrushes(Brush[] brushes)
        {
            if (strBrushes != null)
            {
                for (int i = 0; i < strBrushes.Length; i++)
                    strBrushes[i].Dispose();
            }

            if (brushes != null)
            {
                for (int i = 0; i < brushes.Length; i++)
                {
                    if (brushes[i] == null)
                        brushes[i] = this.m_brush_String;
                }
            }
            strBrushes = brushes;
        }

        public void SetTextFormat(StringAlignment align_x, StringAlignment align_x2, int offset_x, int interval_y)
        {
            this.align_x = align_x;
            this.align_x2 = align_x2;
            this.offset_x = offset_x;
            this.interval_y = interval_y;

            this.Invalidate();
        }

        public ShowTextControl()
        {
            m_value = null;
            InitializeComponent();
        }

        /*Bitmap bmpbuf;
        protected override void OnResize(EventArgs e)
        {
            if(bmpbuf != null)
                bmpbuf.Dispose();
            bmpbuf = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
        }*/

        public void AttachData(BaseChangedData obj)
        {
            if (m_value == null)
            {
                m_value = obj;
                m_value.Changed += OnValueChanged;

                OnValueChanged(this, null);
            }
        }
        public void DetachData(bool _inv)
        {
            if (m_value != null)
            {
                m_value.Changed -= OnValueChanged;
                m_value = null;

                if(_inv)
                    OnValueChanged(this, null);
            }
        }

        public void OnValueChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
             this.Invalidate();
        }

        protected bool bUseFonts = true;
        protected bool bUseBrushes = true;
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;
            /*Graphics gbuf = Graphics.FromImage(bmpbuf);
            gbuf.Clear(this.BackColor);*/

            // Текст
            if (m_value != null && m_value.IsDefined())
            {
                Rectangle m_rect_BorderBounds = new Rectangle(0, 0, this.Width, this.Height);
                if(strFonts == null && strBrushes == null)
                    SmallHelper.DrawMultiString(g, m_rect_BorderBounds, this.m_value.ToString(), this.Font, this.m_brush_String, align_x, align_x2, StringAlignment.Center, offset_x, 0, interval_y);
                else
                    SmallHelper.DrawMultiString(g, m_rect_BorderBounds, this.m_value.ToString(), (this.strFonts == null || !bUseFonts) ? (new Font[] { this.Font }) : this.strFonts, (this.strBrushes == null || !bUseBrushes) ? (new Brush[] { this.m_brush_String }) : this.strBrushes, align_x, align_x2, StringAlignment.Center, offset_x, 0, interval_y);
            }

            /*gbuf.Dispose();
            g.DrawImage(bmpbuf, 0, 0);
            g.Dispose();*/

        }

        private void ShowTextControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
        }
    }

    public class ShowTextControl_Center : ShowTextControl
    {
        public ShowTextControl_Center()
            : base()
        {
            this.SetTextFormat(StringAlignment.Center, StringAlignment.Near, 1, 0);
        }
    }

    public class RobberScoreControl : ShowTextControl
    {
        public RobberScoreControl()
            : base()
        {
            this.SetTextFormat(StringAlignment.Center, StringAlignment.Center, 0, -4);
            this.SetTextFonts(new Font[] { new Font("Tahoma", 6F, FontStyle.Bold), null });
            this.SetTextBrushes(new Brush[] { new SolidBrush(Color.Blue), null });
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            bUseBrushes = (m_value as IMatchCompleted).IsCompleted();

            base.OnPaint(pe);
        }
    }


    // -------- bool data ------

    public struct BoolDataStruct
    {
        bool bVal;
        public bool BoolVal
        {
            get
            {
                return this.bVal;
            }
            set
            {
                this.bVal = value;
                this.born = true;
            }
        }

        bool born;
        public bool Born
        {
            get
            {
                return this.born;
            }
            set
            {
                if (value == false)
                    this.born = false;
            }
        }

        public static bool operator ==(BoolDataStruct b1, BoolDataStruct b2)
        {
            return b1.Equals(b2);
        }
        public static bool operator !=(BoolDataStruct b1, BoolDataStruct b2)
        {
            return !b1.Equals(b2);
        }
        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;
            BoolDataStruct b = (BoolDataStruct)obj;
            return (this.bVal == b.bVal && this.born == b.born);
        }
    }

    public class BoolData : BaseChangedData, ISQLSerialize
    {
        BoolDataStruct value;
        public BoolData()
        {
        }

        public bool Value
        {
            get
            {
                return this.value.BoolVal;
            }
            set
            {
                BoolDataStruct old = this.value;

                this.value.BoolVal = value;

                if (old != this.value && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(old, this.value));
            }
        }

        public bool Born
        {
            get
            {
                return this.value.Born;
            }
            set
            {
                BoolDataStruct old = this.value;

                this.value.Born = value;

                if (old != this.value && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(old, this.value));
            }
        }

        public override bool IsDefined()
        {
            return this.value.Born;
        }

        // Сериализация SQL
        public void _FromDataBase(object v)
        {
            if (v == DBNull.Value)
                this.Born = false;
            else
                this.Value = (bool)v;
        }
        public object _ToDataBase()
        {
            if (this.IsDefined())
                return this.Value;
            else
                return DBNull.Value;
        }

        // Clear
        public override void Clear()
        {
            BoolDataStruct old = this.value;

            this.value.BoolVal = false;
            this.value.Born = false;

            if (old != this.value && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(old, this.value));
        }
    }

}
