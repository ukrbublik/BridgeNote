using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Windows.Forms;

namespace BridgeProject
{
    public enum CardValue { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace };
    public enum CardSuit { Hearts, Diamonds, Clubs, Spades };
    public enum CardTrump { NotYetDefined = 0, NT, Hearts, Diamonds, Clubs, Spades };
    public enum TrumpType { Minor, Major, NT };
    public enum Pairs { NotDefinedYet = 0, NS, EW };
    public enum Zones { NotDefinedYet = 0, None, NS, EW, Both };
    public enum Quarters { NotDefinedYet = 0, N, S, E, W };
    public enum GameType { Robber = 0, Sport, Compensat, SimpleIMP };


    public struct Card
    {
        public CardValue value;
        public CardSuit suit;
        public bool def;

        public Card(CardValue v, CardSuit s)
        {
            value = v;
            suit = s;
            def = true;
        }

        public bool IsDefined()
        {
            return def;
        }

        public override bool Equals(object obj)
        {
            if(!obj.GetType().Equals(typeof(Card)))
                return false;
            Card c = (Card)obj;
            return (this.def == c.def && this.value == c.value && this.suit == c.suit);
        }
        public static bool operator ==(Card c1, Card c2)
        {
            return c1.Equals(c2);
        }
        public static bool operator !=(Card c1, Card c2)
        {
            return !c1.Equals(c2);
        }

        public String GetValueString()
        {
            return SmallHelper.GetCardValueString(this.value);
        }
        public String GetSuitString()
        {
            return SmallHelper.GetCardSuitString(this.suit);
        }
        public override string ToString()
        {
            if (def == false)
                return "";
            else
                return GetValueString() + GetSuitString();
        }
    }

    class SmallHelper
    {
        // Сравнение мастей: ♣ < ♦ < ♥ < ♠ < БК
        public static int GetSuitPower(CardSuit s)
        {
            switch (s)
            {
                case CardSuit.Clubs:
                    return 1;
                case CardSuit.Diamonds:
                    return 2;
                case CardSuit.Hearts:
                    return 3;
                case CardSuit.Spades:
                    return 4;
                default:
                    return 0;
            }
        }
        public static int CompareSuits(CardSuit s1, CardSuit s2) //0 - равны, 1 - первая масть выше, -1 - вторая масть выше
        {
            if (GetSuitPower(s1) > GetSuitPower(s2))
                return 1;
            else if (GetSuitPower(s1) < GetSuitPower(s2))
                return -1;
            else
                return 0;
        }

        public static CardSuit TrumpToSuit(CardTrump trump)
        {
            switch (trump)
            {
                case CardTrump.NT:
                case CardTrump.NotYetDefined:
                    throw new Exception(trump == CardTrump.NotYetDefined ? "Приведение TrumpToSuit невозможно: козырная масть не определена!" : "Приведение TrumpToSuit невозможно: нет козыря!");
                case CardTrump.Clubs:
                    return CardSuit.Clubs;
                case CardTrump.Diamonds:
                    return CardSuit.Diamonds;
                case CardTrump.Hearts:
                    return CardSuit.Hearts;
                case CardTrump.Spades:
                    return CardSuit.Spades;
                default:
                    throw new Exception("Приведение TrumpToSuit невозможно");
            }
        }

        public static String GetCardValueString(CardValue value)
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
                default:
                    s += "?";
                    break;
            }
            return s;
        }

        public static String GetCardSuitString(CardSuit suit)
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
                default:
                    s += "?";
                    break;
            }
            return s;
        }

        // Минор или мажор или БК
        public static TrumpType WhatTrumpType(CardTrump t)
        {
            switch (t)
            {
                case CardTrump.Clubs: case CardTrump.Diamonds:
                    return TrumpType.Minor;
                case CardTrump.Hearts: case CardTrump.Spades:
                    return TrumpType.Major;
                case CardTrump.NT:
                    return TrumpType.NT;
                default:
                    throw new Exception("Масть не определена!");
            }
        }

        // Графич. функции
        public static Color AntiColor(Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }
        public static void DrawRect(Graphics gr, Pen pen, Rectangle rect)
        {
            gr.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
        public static void FillRectInside(Graphics gr, Brush brush, Rectangle rect)
        {
            //gr.FillRectangle(brush, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
            FillRectInside2(gr, brush, rect, 1);
        }
        public static void FillRectInside2(Graphics gr, Brush brush, Rectangle rect, int border_width)
        {
            gr.FillRectangle(brush, rect.X + border_width, rect.Y + border_width, rect.Width - border_width * 2, rect.Height - border_width * 2);
        }
        public static void DrawBmpTransp(Graphics gr, Bitmap bmp, Point pt)
        {
            ImageAttributes imgAttr = new ImageAttributes();
            imgAttr.SetColorKey(Color.FromArgb(255, 0, 255), Color.FromArgb(255, 0, 255));
            gr.DrawImage(bmp, new Rectangle(pt.X, pt.Y, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgAttr);
        }
        public static RectangleF StringInMiddle(Graphics gr, Rectangle rect, String str, Font font)
        {
            SizeF sz = gr.MeasureString(str, font);
            RectangleF r = new RectangleF(rect.X + (rect.Width - sz.Width) / 2, rect.Y + (rect.Height - sz.Height) / 2, sz.Width, sz.Height);
            return r;
        }
        public static RectangleF StringInRight(Graphics gr, Rectangle rect, String str, Font font)
        {
            SizeF sz = gr.MeasureString(str, font);
            RectangleF r = new RectangleF(rect.X + (rect.Width - sz.Width), rect.Y + (rect.Height - sz.Height) / 2, sz.Width, sz.Height);
            return r;
        }
        public static RectangleF StringInLeft(Graphics gr, Rectangle rect, String str, Font font)
        {
            SizeF sz = gr.MeasureString(str, font);
            RectangleF r = new RectangleF(rect.X, rect.Y + (rect.Height - sz.Height) / 2, sz.Width, sz.Height);
            return r;
        }

        // Многострочный текст
        public static void DrawMultiString(Graphics g, RectangleF rect, String str, Font font, Brush brush, StringAlignment alignX, StringAlignment alignX2, StringAlignment alignY, int offsetX, int offsetY, int intervalY)
        {
            str.Replace("\r\n", "\n");
            str.Replace('\r', '\n');
            String[] strArr = str.Split(new char[] { '\n' });
            SizeF szTotal = new SizeF(0, 0);
            SizeF[] szArr = new SizeF[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                szArr[i] = g.MeasureString(strArr[i], font);
                if(i > 0)
                    szTotal.Height += intervalY;
                szTotal.Height += szArr[i].Height;
                if (szArr[i].Width > szTotal.Width)
                    szTotal.Width = szArr[i].Width;
            }
            RectangleF rectMultiString = new RectangleF(alignX == StringAlignment.Center ? (rect.X + (rect.Width - szTotal.Width) / 2) : (alignX == StringAlignment.Near ? rect.X + offsetX : rect.X + (rect.Width - szTotal.Width) - offsetX), alignY == StringAlignment.Center ? (rect.Y + (rect.Height - szTotal.Height) / 2) : (alignY == StringAlignment.Near ? rect.Y + offsetY : rect.Y + (rect.Height - szTotal.Height) - offsetY), szTotal.Width, szTotal.Height);
            RectangleF rfI = new RectangleF(rectMultiString.X, rectMultiString.Y, rectMultiString.Width, 0);
            for (int i = 0; i < strArr.Length; i++)
            {
                rfI.Height = szArr[i].Height;
                g.DrawString(strArr[i], font, brush, (float)(rfI.X + (rfI.Width - szArr[i].Width) * (alignX2 == StringAlignment.Near ? 0 : (alignX2 == StringAlignment.Center ? 0.5 : 1))), rfI.Y);
                rfI.Y += intervalY;
                rfI.Y += szArr[i].Height;
            }
        }

        public static void DrawMultiString(Graphics g, RectangleF rect, String str, Font[] fonts, Brush[] brushes, StringAlignment alignX, StringAlignment alignX2, StringAlignment alignY, int offsetX, int offsetY, int intervalY)
        {
            str.Replace("\r\n", "\n");
            str.Replace('\r', '\n');
            String[] strArr = str.Split(new char[] { '\n' });
            SizeF szTotal = new SizeF(0, 0);
            SizeF[] szArr = new SizeF[strArr.Length];

            for (int i = 0; i < strArr.Length; i++)
            {
                szArr[i] = g.MeasureString(strArr[i], (i >= fonts.Length ? fonts[fonts.Length - 1] : fonts[i]));
                if (i > 0)
                    szTotal.Height += intervalY;
                szTotal.Height += szArr[i].Height;
                if (szArr[i].Width > szTotal.Width)
                    szTotal.Width = szArr[i].Width;
            }

            RectangleF rectMultiString = new RectangleF(alignX == StringAlignment.Center ? (rect.X + (rect.Width - szTotal.Width) / 2) : (alignX == StringAlignment.Near ? rect.X + offsetX : rect.X + (rect.Width - szTotal.Width) - offsetX), alignY == StringAlignment.Center ? (rect.Y + (rect.Height - szTotal.Height) / 2) : (alignY == StringAlignment.Near ? rect.Y + offsetY : rect.Y + (rect.Height - szTotal.Height) - offsetY), szTotal.Width, szTotal.Height);
            RectangleF rfI = new RectangleF(rectMultiString.X, rectMultiString.Y, rectMultiString.Width, 0);
            
            for (int i = 0; i < strArr.Length; i++)
            {
                rfI.Height = szArr[i].Height;
                g.DrawString(strArr[i], (i >= fonts.Length ? fonts[fonts.Length - 1] : fonts[i]), (i >= brushes.Length ? brushes[brushes.Length - 1] : brushes[i]), (float)(rfI.X + (rfI.Width - szArr[i].Width) * (alignX2 == StringAlignment.Near ? 0 : (alignX2 == StringAlignment.Far ? 1 : 0.5))), rfI.Y);
                rfI.Y += intervalY;
                rfI.Y += szArr[i].Height;
            }
        }

        public static SizeF MeasureMultistring(Graphics g, Font font, String str, int intervalY)
        {
            str.Replace("\r\n", "\n");
            str.Replace('\r', '\n');
            String[] strArr = str.Split(new char[] { '\n' });
            SizeF szTotal = new SizeF(0, 0);
            SizeF szI = new SizeF();
            for (int i = 0; i < strArr.Length; i++)
            {
                szI = g.MeasureString(strArr[i], font);
                if (i > 0)
                    szTotal.Height += intervalY;
                szTotal.Height += szI.Height;
                if (szI.Width > szTotal.Width)
                    szTotal.Width = szI.Width;
            }
            return szTotal;
        }

        public static String WrapString(Graphics g, Font f, String s, int width)
        {
            SizeF ssf = g.MeasureString(s, f);
            if (ssf.Width <= width)
            {
                return s;
            }
            else
            {
                int ggg = (int)(width / ssf.Width * s.Length);
                ssf = g.MeasureString(s.Substring(0, ggg), f);
                if (ssf.Width <= width)
                {
                    for (ggg++; ggg <= s.Length; ggg++)
                    {
                        ssf = g.MeasureString(s.Substring(0, ggg), f);
                        if (ssf.Width > width)
                            break;
                    }
                    ggg--;
                }
                else
                {
                    for (ggg--; ggg >= 0; ggg--)
                    {
                        ssf = g.MeasureString(s.Substring(0, ggg), f);
                        if (ssf.Width <= width)
                            break;
                    }
                }
                // Теперь ggg - число влезающих символов
                int ggg_space = -1;
                for (int i = ggg - 1; i >= 0; i--)
                {
                    if (ggg_space == -1 && s[i] == ' ')
                    {
                        ggg_space = i;
                    }
                    else if (ggg_space != -1 && s[i] != ' ')
                    {
                        break;
                    }
                }
                // Теперь ggg_space - индекс крайнего пробела слева
                if (ggg_space != -1)
                {
                    return s.Substring(0, ggg_space) + "\n" + WrapString(g, f, s.Substring(ggg_space).TrimStart(new char[] { ' ' }), width);
                }
                else
                {
                    for (int i = ggg; i < s.Length; i++)
                    {
                        if (ggg_space == -1 && s[i] == ' ')
                        {
                            ggg_space = i;
                        }
                        else if (ggg_space != -1 && s[i] != ' ')
                        {
                            break;
                        }
                    }
                    // Теперь ggg_space - индекс крайнего пробела справа
                    if (ggg_space != -1)
                        return s.Substring(0, ggg_space) + "\n" + WrapString(g, f, s.Substring(ggg_space).TrimStart(new char[] { ' ' }), width);
                    else
                        return s;
                }
            }
        }

        // Решение зависимостей
        public static ArrayOfInt SolveDependences(ArrayOfArrayOfInt a)
        {
            ArrayOfInt b = new ArrayOfInt();
            while (true)
            {
                int solved = 0;
                for (int i = 0; i < a.Count; i++)
                {
                    if (!b.Contains(i) && IsFreeOfDependences(i, a, b))
                    {
                        solved++;
                        b.Add(i);
                    }
                }
                if (a.Count == b.Count)
                    break;
                else if (solved == 0)
                {
                    throw new Exception("Невозможно распутать зависимости!");
                }
            }
            return b;
        }
        public static bool IsFreeOfDependences(int x, ArrayOfArrayOfInt a, ArrayOfInt b)
        {
            int free = 0;
            for (int j = 0; j < a[x].Count; j++)
            {
                if (b.Contains(a[x, j]))
                    free++;
            }
            return (a[x].Count == free);
        }

        
    }

    // Базовый класс для данных
    public abstract class BaseChangedData
    {
        // 1. event Changed:
        public class ChangedEventsArgs : EventArgs
        {
            public Object _old;
            public Object _new;
            public ChangedEventsArgs(Object _old, Object _new)
            {
                this._old = _old;
                this._new = _new;
            }
        }
        public delegate void ChangedHandler(object sender, ChangedEventsArgs e);
        public event ChangedHandler Changed;
        public bool IsChangedHandlers()
        {
            return (Changed != null);
        }
        public void RemoveChangedHandlers()
        {
            Changed = null;
        }
        public void OnChanged(object sender, ChangedEventsArgs e)
        {
            if(Changed != null)
                Changed(sender, e);
        }


        // 2. bool Defined:
        abstract public bool IsDefined();


        // 3. Clear:
        abstract public void Clear();
    }

    // Интерфейс для данных, поддерживающий загрузку из строки
    public interface IFromString
    {
        bool FromString(String s);
    }

    // Интерфейс для данных, поддерживающий загрузку из DB через SQL
    public interface ISQLSerialize
    {
        void _FromDataBase(object v);
        object _ToDataBase();
    }

    // Интерфейс связи контролов с данными
    public interface IAttachData<T>
    {
        void AttachData(T obj);
    }
    public interface IDetachData
    {
        void DetachData(bool invalidate);
    }

    // Интерфейс для блокировки контролов
    public interface ILock
    {
        bool Lock
        {
            get;
            set;
        }

        event EventHandler Locked;
        event EventHandler Unlocked;

        void OnLocked();
        void OnUnlocked();
    }

    // Интерфейс для контролов в таблице
    public interface IControlInTable
    {
        // принадлежность таблице
        void SetParentTable(BaseTable t);
        void UnsetParentTable();
        bool IsActiveInTable();

        // событие 'GotActive'
        event EventHandler GotActive;
        bool IsGotActiveHandlers();
        void RemoveGotActiveHandlers();
        void Raise_GotActive();

        // событие 'LostActive'
        event EventHandler LostActive;
        bool IsLostActiveHandlers();
        void RemoveLostActiveHandlers();
        void Raise_LostActive();
    }

    // Базовый класс для контролов в таблице
    public abstract class BaseControlInTable : Control, IControlInTable, ILock
    {
        // указатель на таблицу
        protected BaseTable m_Table;
        public void SetParentTable(BaseTable t)
        {
            if (m_Table == null)
            {
                m_Table = t;
                m_Table.FollowElementFocus(this);
            }
        }
        public void UnsetParentTable()
        {
            if (m_Table != null)
            {
                m_Table.UnfollowElementFocus(this);
                m_Table = null;
            }
        }
        public bool IsActiveInTable()
        {
            if (m_Table == null)
                return false;
            else
                return (m_Table.ActiveElement == this);
        }

        // 2 events 'GotActive' & 'LostActive'
        public event EventHandler GotActive;
        public bool IsGotActiveHandlers()
        {
            return (GotActive != null);
        }
        public void RemoveGotActiveHandlers()
        {
            GotActive = null;
        }
        public void Raise_GotActive()
        {
            if (GotActive != null)
                GotActive(this, null);
        }

        public event EventHandler LostActive;
        public bool IsLostActiveHandlers()
        {
            return (LostActive != null);
        }
        public void RemoveLostActiveHandlers()
        {
            LostActive = null;
        }
        public void Raise_LostActive()
        {
            if (LostActive != null)
                LostActive(this, null);
        }


        #region Члены ILock

        protected bool _lock_ = true;
        public bool Lock
        {
            get
            {
                return _lock_;
            }
            set
            {
                bool old = _lock_;
                _lock_ = value;
                if (old != _lock_)
                {
                    if (_lock_ == true)
                    {
                        OnLocked();
                        if (Locked != null)
                            Locked(this, null);
                    }
                    else if (_lock_ == false)
                    {
                        OnUnlocked();
                        if (Unlocked != null)
                            Unlocked(this, null);
                    }
                }
            }
        }

        public event EventHandler Locked;
        public event EventHandler Unlocked;

        public virtual void OnLocked() { }
        public virtual void OnUnlocked() { }

        #endregion
    }

    public interface ITotalFocus
    {
        event EventHandler GotTotalFocus;
        event EventHandler LostTotalFocus;
        bool HaveTotalFocus();
    }

    public abstract class BaseTable : UserControl
    {
        // "взлом" передачи фокуса внутр. элементам
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            
            // Тут фокус должен передаться на первый внутр.элемент, НО я передаю на активный
            // !!!Примечание!!!  При переходе по <TAB> не срабатывает! Но всем похуй!
            if(m_Active != null)
                m_Active.Focus();
        }

        // активный контрол в таблице
        protected Control m_Active;
        public Control ActiveElement
        {
            get
            {
                return m_Active;
            }
        }
        public void FollowElementFocus(Control c)
        {
            if(c.GetType().GetInterfaces().Contains(typeof(ITotalFocus)))
                (c as ITotalFocus).GotTotalFocus += OnElementGotFocus;
            else
                c.GotFocus += OnElementGotFocus;
        }
        public void UnfollowElementFocus(Control c)
        {
            if (c.GetType().GetInterfaces().Contains(typeof(ITotalFocus)))
                (c as ITotalFocus).GotTotalFocus -= OnElementGotFocus;
            else
                c.GotFocus -= OnElementGotFocus;
        }

        void OnElementGotFocus(object sender, EventArgs e)
        {
            Control oldActive = m_Active;

            m_Active = (Control) sender;

            if (oldActive != null && oldActive.GetType().GetInterfaces().Contains(typeof(IControlInTable)))
                (oldActive as IControlInTable).Raise_LostActive();

            if (m_Active != null && m_Active.GetType().GetInterfaces().Contains(typeof(IControlInTable)))
                (m_Active as IControlInTable).Raise_GotActive();
 
            if (m_Active != oldActive && IsActiveElementChangedHandlers())
                OnActiveElementChanged(this, new ChangedEventsArgs(oldActive, this.m_Active));
        }

        // событие ActiveChanged
        public class ChangedEventsArgs : EventArgs
        {
            public Object _old;
            public Object _new;
            public ChangedEventsArgs(Object _old, Object _new)
            {
                this._old = _old;
                this._new = _new;
            }
        }
        public delegate void ChangedHandler(object sender, ChangedEventsArgs e);
        public event ChangedHandler ActiveElementChanged;
        public bool IsActiveElementChangedHandlers()
        {
            return (ActiveElementChanged != null);
        }
        public void RemoveActiveElementChangedHandlers()
        {
            ActiveElementChanged = null;
        }
        public void OnActiveElementChanged(object sender, ChangedEventsArgs e)
        {
            if (ActiveElementChanged != null)
                ActiveElementChanged(sender, e);
        }
    }

    
    // Счёт
    public struct SimpleScoreStruct
    {
        public DealScore score;
        public bool born;
    }

    public class SimpleScore : BaseChangedData
    {
        SimpleScoreStruct val;
        public DealScore Score
        {
            get
            {
                return val.score;
            }
        }
        public void SetScore(int iNS, int iEW)
        {
            SimpleScoreStruct old = val;

            this.val.score.NS = iNS;
            this.val.score.EW = iEW;
            this.val.born = true;

            if (!old.Equals(this.val) && IsChangedHandlers())
                OnChanged(this, null);
        }

        public bool Born
        {
            get
            {
                return val.born;
            }
            set
            {
                SimpleScoreStruct old = val;

                if(value == false)
                    this.val.born = value;

                if (!old.Equals(this.val) && IsChangedHandlers())
                    OnChanged(this, null);
            }
        }

        public String ToString(Pairs pair)
        {
            if (!this.IsDefined())
            {
                return "";
            }
            else
            {
                switch (pair)
                {
                    case Pairs.NotDefinedYet:
                        return (val.score.NS.ToString() + " : " + val.score.EW.ToString()); //относительно никого
                    case Pairs.NS:
                        return (val.score.NS - val.score.EW).ToString(); //относительно NS
                    case Pairs.EW:
                        return (val.score.EW - val.score.NS).ToString(); //относительно EW
                    default:
                        return "";
                }
            }
        }

        public override String ToString()
        {
            return ToString(Pairs.NotDefinedYet);
        }

        public override bool IsDefined()
        {
            return Born;
        }

        // Clear()
        public override void Clear()
        {
            SimpleScoreStruct old = val;

            this.val.score.MakeEmpty();
            this.val.born = false;

            if (!old.Equals(this.val) && IsChangedHandlers())
                OnChanged(this, null);
        }
    }

    public class ShowSimpleScore : BaseControlInTable, IDetachData
    {
        public static Pairs RelativePair = Pairs.NS;

        SimpleScore m_score;
        PairSwitcher m_pair;

        public void AttachData(SimpleScore s)
        {
            if (m_score == null)
            {
                m_score = s;
                m_score.Changed += OnValueChanged;
            }
            OnValueChanged(this, null);
        }
        public void AttachData(PairSwitcher p, SimpleScore s)
        {
            if (m_score == null)
            {
                m_score = s;
                m_score.Changed += OnValueChanged;
            }
            if (m_pair == null)
            {
                m_pair = p;
                m_pair.Changed += OnValueChanged;
            }
            OnValueChanged(this, null);
        }
        public void DetachData(bool _inv)
        {
            if (m_score != null)
            {
                m_score.Changed -= OnValueChanged;
                m_score = null;
            }
            if (m_pair != null)
            {
                m_pair.Changed -= OnValueChanged;
                m_pair = null;
            }

            if(_inv)
                OnValueChanged(this, null);
        }

        SolidBrush m_brush_String = new SolidBrush(SystemColors.ControlText);
        int offset_x = 1;
        StringAlignment align_x = StringAlignment.Near;
        StringAlignment align_x2 = StringAlignment.Near;

        public void OnValueChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
        }

        public ShowSimpleScore()
        {
            this.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            // Текст
            String str = "";
            if (m_score != null && m_score.IsDefined())
            {
                if (m_pair != null && m_pair.IsDefined())
                    str = m_score.ToString(m_pair.Pair);
                else
                    str = m_score.ToString(RelativePair);
            }
            Rectangle m_rect_BorderBounds = new Rectangle(0, 0, this.Width, this.Height);
            SmallHelper.DrawMultiString(g, m_rect_BorderBounds, str, this.Font, this.m_brush_String, align_x, align_x2, StringAlignment.Center, offset_x, 0, 0);
        }
    }




    // Массив строк
    public class ArrayOfString : CollectionBase
    {
        public String this[int index]
        {
            get
            {
                return ((String)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(String value)
        {
            return (List.Add(value));
        }

        public int IndexOf(String value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, String value)
        {
            List.Insert(index, value);
        }

        public void Remove(String value)
        {
            List.Remove(value);
        }

        public bool Contains(String value)
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
            if (value.GetType() != typeof(System.String))
                throw new ArgumentException("value must be of type String", "value");
        }
    }

    // Массив типов
    public class ArrayOfTypes : CollectionBase
    {
        public Type this[int index]
        {
            get
            {
                return ((Type)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(Type value)
        {
            return (List.Add(value));
        }

        public int IndexOf(Type value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, Type value)
        {
            List.Insert(index, value);
        }

        public void Remove(Type value)
        {
            List.Remove(value);
        }

        public bool Contains(Type value)
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

        }
    }

    // Массив интов
    public class ArrayOfInt : CollectionBase
    {
        public ArrayOfInt()
        {
        }
        public ArrayOfInt(int value)
        {
            Add(value);
        }
        public ArrayOfInt(int[] values)
        {
            Add(values);
        }
        public int this[int index]
        {
            get
            {
                return ((int)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(int value)
        {
            return (List.Add(value));
        }
        public void Add(int[] values)
        {
            for (int i = 0; i < values.Length; i++)
                List.Add(values[i]);
        }
        public int IndexOf(int value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, int value)
        {
            List.Insert(index, value);
        }
        public void Insert(int index, int[] values)
        {
            for (int i = 0; i < values.Length; i++)
                List.Insert(index+i, values[i]);
        }
        public void Remove(int value)
        {
            List.Remove(value);
        }

        public bool Contains(int value)
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
            if (value.GetType() != typeof(int))
                throw new ArgumentException("value must be of type int", "value");
        }
    }

    //Массив массивов интов
    public class ArrayOfArrayOfInt : CollectionBase
    {
        public ArrayOfInt this[int index]
        {
            get
            {
                return ((ArrayOfInt)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int this[int index, int i2]
        {
            get
            {
                return (((ArrayOfInt)List[index])[i2]);
            }
            set
            {
                (List[index] as ArrayOfInt)[i2] = value;
            }
        }

        public int Add(ArrayOfInt value)
        {
            return (List.Add(value));
        }
        public void Add(ArrayOfInt[] values)
        {
            for (int i = 0; i < values.Length; i++)
                List.Add(values[i]);
        }

        public int IndexOf(ArrayOfInt value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, ArrayOfInt value)
        {
            List.Insert(index, value);
        }
        public void Insert(int index, ArrayOfInt[] values)
        {
            for (int i = 0; i < values.Length; i++)
                List.Insert(index + i, values[i]);
        }

        public void Remove(ArrayOfInt value)
        {
            List.Remove(value);
        }

        public bool Contains(ArrayOfInt value)
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
            if (value.GetType() != typeof(ArrayOfInt))
                throw new ArgumentException("value must be of type ArrayOfInt", "value");
        }
    }

    //Массив массивов контролов
    public class ArrayOfArrayOfControl : CollectionBase
    {
        public ArrayOfControl this[int index]
        {
            get
            {
                return ((ArrayOfControl)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public Control this[int index, int i2]
        {
            get
            {
                return (((ArrayOfControl)List[index])[i2]);
            }
            set
            {
                (List[index] as ArrayOfControl)[i2] = value;
            }
        }

        public int Add(ArrayOfControl value)
        {
            return (List.Add(value));
        }

        public int IndexOf(ArrayOfControl value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, ArrayOfControl value)
        {
            List.Insert(index, value);
        }

        public void Remove(ArrayOfControl value)
        {
            List.Remove(value);
        }

        public bool Contains(ArrayOfControl value)
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
            if (value.GetType() != typeof(ArrayOfControl))
                throw new ArgumentException("value must be of type ArrayOfControl", "value");
        }
    }

    public class ArrayOfControl : CollectionBase
    {
        public Control this[int index]
        {
            get
            {
                return ((Control)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(Control value)
        {
            return (List.Add(value));
        }

        public int IndexOf(Control value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, Control value)
        {
            List.Insert(index, value);
        }

        public void Remove(Control value)
        {
            List.Remove(value);
        }

        public bool Contains(Control value)
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
            if (value != null && !value.GetType().IsSubclassOf(typeof(Control)))
                throw new ArgumentException("value must be of type Control or derived from Control", "value");
        }
    }

    // Массив массивов объектов
    public class ArrayOfArrayOfObject : CollectionBase
    {
        public ArrayList this[int index]
        {
            get
            {
                return ((ArrayList)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public object this[int index, int i2]
        {
            get
            {
                return (((ArrayList)List[index])[i2]);
            }
            set
            {
                (List[index] as ArrayList)[i2] = value;
            }
        }

        public int Add(ArrayList value)
        {
            return (List.Add(value));
        }

        public int IndexOf(ArrayList value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, ArrayList value)
        {
            List.Insert(index, value);
        }

        public void Remove(ArrayList value)
        {
            List.Remove(value);
        }

        public bool Contains(ArrayList value)
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
            if (value.GetType() != typeof(ArrayList))
                throw new ArgumentException("value must be of type ArrayList", "value");
        }
    }

    // Массив сделок = роббер
    public class ArrayOfDeals : CollectionBase
    {
        public DealScoreExt this[int index]
        {
            get
            {
                return ((DealScoreExt)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(DealScoreExt value)
        {
            return (List.Add(value));
        }

        public int IndexOf(DealScoreExt value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, DealScoreExt value)
        {
            List.Insert(index, value);
        }

        public void Remove(DealScoreExt value)
        {
            List.Remove(value);
        }

        public bool Contains(DealScoreExt value)
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
            if (value.GetType() != typeof(DealScoreExt))
                throw new ArgumentException("value must be of type DealScore", "value");
        }
    }

}
