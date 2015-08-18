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
    public struct ContractStruct
    {
        public ContractStruct(int q, CardTrump t, bool c, bool rc)
        {
            if((q >= 1 && q <= 7 && (int)t >= 1 && (int)t <= 5 && !(c && rc))  ||  (q == 0 && t == CardTrump.NotYetDefined && c == false && rc == false))
            {
                this.quantity = q;
                this.trump = t;
                this.contra = c;
                this.recontra = rc;

                born = true;
            }
            else
            {
                this.quantity = 0;
                this.trump = CardTrump.NotYetDefined;
                this.contra = false;
                this.recontra = false;

                born = false;
            }
        }

        bool born;
        public bool Born
        {
            get
            {
                return born;
            }
            set
            {
                if (value == true)
                {
                    if (!this.Defined)
                        this.Refresh();
                }
                else
                {
                    Empty();
                }
            }
        }

        int quantity;
        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                if (NoContract == false)
                {
                    if (IsQuantityGood(value))
                        quantity = value;
                }
            }
        }
        public bool IsQuantityGood(int q)
        {
            return (q >= 1 && q <= 7);
        }

        CardTrump trump;
        public CardTrump Trump
        {
            get
            {
                return trump;
            }
            set
            {
                if (NoContract == false)
                {
                    if (IsTrumpGood(value))
                        trump = value;
                }
            }
        }
        public bool IsTrumpGood(CardTrump t)
        {
            return ((int)t >= 1 && (int)t <= 5);
        }

        public bool contra;
        public bool Contra
        {
            get
            {
                return contra;
            }
            set
            {
                if (NoContract == false)
                {
                    contra = value;
                    if (contra)
                        recontra = false;
                }
            }
        }

        public bool recontra;
        public bool ReContra
        {
            get
            {
                return recontra;
            }
            set
            {
                if (NoContract == false)
                {
                    recontra = value;
                    if (recontra)
                        contra = false;
                }
            }
        }


        public static bool operator ==(ContractStruct c1, ContractStruct c2)
        {
            return c1.Equals(c2);
        }
        public static bool operator !=(ContractStruct c1, ContractStruct c2)
        {
            return !c1.Equals(c2);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ContractStruct))
                return false;
            ContractStruct cs = (ContractStruct)obj;
            return (this.quantity == cs.quantity && this.trump == cs.trump && this.Contra == cs.Contra && this.ReContra == cs.ReContra && this.Born == cs.Born);
        }

        public bool Defined
        {
            get
            {
                return ((born && IsQuantityGood(this.quantity) && IsTrumpGood(this.trump) && !(Contra == true && ReContra == true)) || NoContract);
            }
        }
        public void Empty()
        {
            quantity = 0;
            trump = CardTrump.NotYetDefined;
            Contra = false;
            ReContra = false;

            born = false;
        }
        public void Refresh()
        {
            quantity = 1;
            trump = CardTrump.NotYetDefined;
            Contra = false;
            ReContra = false;

            born = true;
        }
        public bool NoContract
        {
            get
            {
                return (born == true && quantity == 0 && trump == CardTrump.NotYetDefined && contra == false && recontra == false);
            }
            set
            {
                if (value == true)
                {
                    quantity = 0;
                    trump = CardTrump.NotYetDefined;
                    Contra = false;
                    ReContra = false;

                    born = true;
                }
                else
                {
                    if (NoContract)
                        Refresh();
                }
            }
        }



        public String GetString1()
        {
            /*if (!Defined)
                return "";
            else*/
                return (NoContract ? "-" : quantity.ToString());
        }
        public String GetString2()
        {
            /*if (!Defined)
                return "";
            else*/
            {
                switch (Trump)
                {
                    case CardTrump.Hearts:
                        return "♥";
                    case CardTrump.Diamonds:
                        return "♦";
                    case CardTrump.Clubs:
                        return "♣";
                    case CardTrump.Spades:
                        return "♠";
                    case CardTrump.NT:
                        return "NT";
                    default:
                        return "";
                }
            }
        }
        public String GetString3()
        {
            /*if (!Defined)
                return "";
            else*/
                return (this.ReContra ? "**" : (this.Contra ? "*" : ""));
        }
        public override string ToString()
        {
            return (!Defined ? "" : (GetString1() + GetString2() + GetString3()));
        }
    };



    public class Contract : BaseChangedData, ISQLSerialize
    {
        // Инкапсулируем структуру:
        ContractStruct val;
        public ContractStruct GetStruct()
        {
            return val;
        }

        // Конструкторы:
        public Contract()
        {
            val.Empty();
        }
        public Contract(int q, CardTrump tr, bool co, bool reco)
        {
            val = new ContractStruct(q, tr, co, reco);
        }
        public Contract(ContractStruct cs)
        {
            val = cs;
        }
        public Contract(Contract c)
        {
            val = c.GetStruct();
        }

        // Change
        public void Empty()
        {
            ContractStruct oldc = val;
            val.Empty();
            if (oldc != val && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(oldc, val));
        }
        public void Refresh()
        {
            ContractStruct oldc = val;
            val.Refresh();
            if (oldc != val && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(oldc, val));
        }
        public bool NoContract
        {
            get
            {
                return val.NoContract;
            }
            set
            {
                ContractStruct oldc = val;
                val.NoContract = value;
                if (oldc != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(oldc, val));
            }
        }
        public void ChangeContract(int q, CardTrump tr, bool co, bool reco)
        {
            this.ChangeContract(new ContractStruct(q, tr, co, reco));
        }
        public void ChangeContract(ContractStruct cs)
        {
            ContractStruct oldc = val;
            val = cs;
            if (oldc != val && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(oldc, val));
        }

        // Copy
        public void CopyContract(Contract c)
        {
            ContractStruct oldc = val;
            val = c.GetStruct();
            if (oldc != val && IsChangedHandlers())
                OnChanged(this, new ChangedEventsArgs(oldc, val));
        }

        // Get&Set
        public int Quantity
        {
            get
            {
                return val.Quantity;
            }
            set
            {
                ContractStruct oldc = val;
                val.Quantity = value;
                if (oldc != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(oldc, val));
            }
        }
        public CardTrump Trump
        {
            get
            {
                return val.Trump;
            }
            set
            {
                ContractStruct oldc = val;
                val.Trump = value;
                if (oldc != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(oldc, val));
            }
        }
        public bool Contra
        {
            get
            {
                return val.Contra;
            }
            set
            {
                ContractStruct oldc = val;
                val.Contra = value;
                if (oldc != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(oldc, val));
            }
        }
        public bool ReContra
        {
            get
            {
                return val.ReContra;
            }
            set
            {
                ContractStruct oldc = val;
                val.ReContra = value;
                if (oldc != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(oldc, val));
            }
        }

        // Получение строкового представления контракта
        public String GetString1()
        {
            return val.GetString1();
        }
        public String GetString2()
        {
            return val.GetString2();
        }
        public String GetString3()
        {
            return val.GetString3();
        }
        public override String ToString()
        {
            return (val.ToString());
        }

        // Born & Defined
        public bool Born
        {
            get
            {
                return val.Born;
            }
            set
            {
                ContractStruct oldc = val;
                val.Born = value;
                if (oldc != val && IsChangedHandlers())
                    OnChanged(this, new ChangedEventsArgs(oldc, val));
            }
        }
        override public bool IsDefined()
        {
            return val.Defined;
        }

        // Событие изменения контракта
        /*public class ContractEventsArgs : EventArgs
        {
            public ContractStruct OldContractStruct;
            public ContractStruct NewContractStruct;
            public ContractEventsArgs(ContractStruct oldc, ContractStruct newc)
            {
                OldContractStruct = oldc;
                NewContractStruct = newc;
            }
        }
        public delegate void ContractHandler(object sender, ContractEventsArgs e);
        public event ContractHandler ContractChanged;*/

        // Сериализация SQL
        // (3 бита на Quantity, 3 бита на CardTrump, 1 бит на Contra, 1 бит на ReContra)
        public void _FromDataBase(object v)
        {
            if (v == DBNull.Value)
            {
                this.Empty();
            }
            else
            {
                byte c = (byte) v;

                this.ChangeContract((int)(c & 7), (CardTrump)((c >> 3) & 7), (((c >> 6) & 1) == 1 ? true : false), (((c >> 7) & 1) == 1 ? true : false));
            }
        }
        public object _ToDataBase()
        {
            if (!this.IsDefined())
            {
                //return "NULL";
                return DBNull.Value;
            }
            else
            {
                byte c = 0;
                c |= (byte)(this.Quantity & 7);
                c |= (byte)(((byte)this.Trump & 7) << 3);
                if (this.Contra)
                    c |= (1 << 6);
                if (this.ReContra)
                    c |= (1 << 7);
                //return c.ToString();
                return c;
            }
        }

        // Clear
        public override void Clear()
        {
            this.Empty();
        }
    }




    public partial class ContractSelectControl : BaseSelectControl, IAttachData<Contract>, IDetachData
    {
        Font m_font_for_suits;
        Contract m_contract;
 
        public ContractSelectControl()
        {
            InitializeComponent();
            this.m_contract = null;
        }

        

        public void AttachData(Contract c)
        {
            // Присоединение контракта:
            if (this.m_contract == null)
            {
                this.m_contract = c;
                this.m_contract.Changed += OnDataChanged;

                // Отрисовать:
                this.Invalidate();
            }
        }

        public void DetachData(bool _inv)
        {
            // Отсоединение контракта:
            if (this.m_contract != null)
            {
                this.m_contract.Changed -= OnDataChanged;
                this.m_contract = null;

                // Отрисовать:
                if (_inv)
                    this.Invalidate();
            }
            // Закрыть селектор, если он открыт подо мной:
            if (SelectorOpened)
                CloseSelector(false);
        }

        protected void OnDataChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            // ? Возможно: если изменения извне (загрузка из файла), закрыть (или обновить - сложнее) селектор

            this.Invalidate();
        }

        protected override void OpenSelector()
        {
            if (m_contract == null)
                return;

            ContractSelector.GetInstance().OpenMe(this, this.SelectorCover, this.ParentForm, this.m_contract);

            ContractSelector.GetInstance().Closing += OnSelectorClosing;
            ContractSelector.GetInstance().Closed += OnSelectorClosed;
            this.SelectorOpened = true;
        }

        protected override void CloseSelector(bool saveBeforeClose)
        {
            ContractSelector.GetInstance().CloseMe(saveBeforeClose);
        }

        void OnSelectorClosing(object sender, SelectorClosingEventArgs e)
        {
            //if (!(sender as ContractSelector).IsAttachedToThisControl(this))
            //    return;

            if (e != null && e.bSaveBeforeClose == true && this.m_contract != null && e.dataToSaveBeforeClosed != null)
            {
                this.m_contract.CopyContract(e.dataToSaveBeforeClosed as Contract);
            }
        }

        void OnSelectorClosed(object sender, EventArgs e)
        {
            //if (!(sender as ContractSelector).IsAttachedToThisControl(this))
            //    return;

            ContractSelector.GetInstance().Closing -= OnSelectorClosing;
            ContractSelector.GetInstance().Closed -= OnSelectorClosed;

            this.SelectorOpened = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            // рисуем текст, если контракт определен
            if (this.m_contract != null && this.m_contract.IsDefined())
            {
                if (this.Focused || SelectorOpened)
                    m_rectf_String.Width -= splitter_width;
                bool nospace = false;

                SizeF szf1 = g.MeasureString(m_contract.GetString1(), this.Font);
                SizeF szf2 = g.MeasureString(m_contract.GetString2(), (m_contract.Trump == CardTrump.NT) ? this.Font : m_font_for_suits);
                SizeF szf3 = g.MeasureString(m_contract.GetString3(), this.Font);

                RectangleF rf = new RectangleF(m_rectf_String.X + text_left_offset, m_rectf_String.Y + (m_rectf_String.Height - szf1.Height) / 2, szf1.Width, szf1.Height);
                if (rf.Right > m_rectf_String.Right)
                {
                    nospace = true;
                    rf.Width = m_rectf_String.Right - rf.X;
                }
                g.DrawString(m_contract.GetString1(), this.Font, m_brush_String, rf, new StringFormat(StringFormatFlags.NoWrap));

                if (!nospace)
                {
                    rf = new RectangleF(rf.Right, 1 + (m_rectf_String.Height - szf2.Height) / 2, szf2.Width, szf2.Height);
                    if (rf.Right > m_rectf_String.Right)
                    {
                        nospace = true;
                        rf.Width = m_rectf_String.Right - rf.X;
                    }
                    g.DrawString(m_contract.GetString2(), (m_contract.Trump == CardTrump.NT) ? this.Font : m_font_for_suits, (m_contract.Trump == CardTrump.Hearts || m_contract.Trump == CardTrump.Diamonds) ? m_brush_String_RED : m_brush_String_BLACK, rf, new StringFormat(StringFormatFlags.NoWrap));
                }

                if (!nospace)
                {
                    rf = new RectangleF(rf.Right, 1 + (m_rectf_String.Height - szf3.Height) / 2, szf3.Width, szf3.Height);
                    if (rf.Right > m_rectf_String.Right)
                    {
                        nospace = true;
                        rf.Width = m_rectf_String.Right - rf.X;
                    }
                    g.DrawString(m_contract.GetString3(), this.Font, m_brush_String, rf, new StringFormat(StringFormatFlags.NoWrap));
                }

                if (this.Focused || SelectorOpened)
                    m_rectf_String.Width += splitter_width;
            }
        }
    }
}
