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
    public struct ResultStruct
    {
        public ResultStruct(Contract c)
        {
            this.contract = c;
            this.contract_quantity__for_correct = (this.contract.IsDefined() ? this.contract.Quantity : 0);
            if (this.contract.NoContract)
                this.quantity = 100;
            else
                this.quantity = 0;
            this.born = false;
        }

        Contract contract;
        public int contract_quantity__for_correct;

        bool born;
        int quantity;
        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                if (IsContractOK() && !IsContractNO() && IsQuantityGood(value))
                {
                    this.quantity = value;
                    this.born = true;
                }
            }
        }

        // Взятки
        public int GetTricks()
        {
            if (IsContractNO())
                return 100; //нет рез-та, т.к. нет контракта
            else if (IsContractOK() && !IsContractNO() && IsQuantityGood(this.quantity))
                return (6 + this.contract.Quantity + this.quantity);
            else
                return 255; //не определен
        }
        public void SetFromTricks(int tricks)
        {
            if (IsContractNO() || tricks == 100)
                this.DontCare();
            else if (tricks == 255)
                this.EmptyAndDead();
            else if (IsContractOK() && !IsContractNO())
            {
                this.Quantity = (tricks - 6 - this.contract.Quantity);
            }
        }

        bool IsQuantityGood(int q)
        {
            return ((6 + this.contract.Quantity + q) >= 0 && (6 + this.contract.Quantity + q) <= 13);
        }
        public int WhatQuantityWillBeGood()
        {
            if ((6 + this.contract.Quantity + quantity) < 0)
            {
                return (-this.contract.Quantity - 6 + 0);
            }
            else if ((6 + this.contract.Quantity + quantity) > 13)
            {
                return (-this.contract.Quantity - 6 + 13);
            }
            else
            {
                return quantity;
            }
        }
        bool IsQuantityGood()
        {
            return IsQuantityGood(this.quantity);
        }
        public bool IsContractOK()
        {
            return (contract != null && contract.IsDefined());
        }
        public bool IsContractNO()
        {
            return (contract != null && contract.NoContract == true);
        }
        public bool IsMin
        {
            get
            {
                return ((6 + contract.Quantity + this.quantity) == 0);
            }
        }
        public bool IsMax
        {
            get
            {
                return ((6 + contract.Quantity + this.quantity) == 13);
            }
        }

        public bool Defined
        {
            get
            {
                return ((IsContractOK() && IsQuantityGood() && this.born) || IsContractNO());
            }
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
                    if (IsContractOK() && !IsContractNO())
                    {
                        if (!IsQuantityGood())
                            this.quantity = 0;
                        this.born = true;
                    }
                }
                else
                {
                    this.born = false;
                }
            }
        }

        public void EmptyAndDead()
        {
            quantity = 0;
            born = false;
        }

        public void DontCare()
        {
            quantity = 100;
            born = false;
        }

        public static bool operator ==(ResultStruct r1, ResultStruct r2)
        {
            return r1.Equals(r2);
        }
        public static bool operator !=(ResultStruct r1, ResultStruct r2)
        {
            return !r1.Equals(r2);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ResultStruct))
                return false;
            ResultStruct rs = (ResultStruct) obj;
            return (this.contract == rs.contract && this.quantity == rs.quantity && this.born == rs.born);
        }

        public override String ToString()
        {
            if (this.Defined)
            {
                return (IsContractNO() ? "-" : (Quantity == 0 ? "=" : (Quantity > 0 ? "+" : "") + Quantity.ToString()));
            }
            else
            {
                return "";
            }
        }

 
        /*public bool ContractPtrEquals(ResultStruct rs)
        {
            return (this.contract == rs.contract);
        }*/

        public void _AddContractChangedHandler(BaseChangedData.ChangedHandler h)
        {
            if (contract != null)
                contract.Changed += h;
        }
        public void _RemoveContractChangedHandler(BaseChangedData.ChangedHandler h)
        {
            if (contract != null)
                contract.Changed -= h;
        }
    };


    public class Result : BaseChangedData, ISQLSerialize
    {
        // stuff
        public void _AddContractChangedHandler(BaseChangedData.ChangedHandler h)
        {
            if(val != null)
                val._AddContractChangedHandler(h);
        }
        public void _RemoveContractChangedHandler(BaseChangedData.ChangedHandler h)
        {
            if(val != null)
                val._RemoveContractChangedHandler(h);
        }

        // Инкапсулируем структуру:
        ResultStruct val;
        ResultStruct GetStruct()
        {
            return val;
        }

        // Конструкторы:
        public Result()
        {
        }
        public void Constructor(Contract c)
        {
            val = new ResultStruct(c);
        }
        public Result(Contract c)
        {
            val = new ResultStruct(c);
        }
        public Result(Result r)
        {
            val = r.GetStruct();
        }

        // Copy
        public void CopyResult(Result r)
        {
            ResultStruct oldr = val;
            val = r.GetStruct();
            /*if (!val.ContractPtrEquals(oldr))
            {
                oldr.ClearContractChangedHandler(Contract_Changed);
                val.AddContractChangedHandler(Contract_Changed);
            }*/
            if (oldr != val && IsChangedHandlers())
                OnChanged(this, new BaseChangedData.ChangedEventsArgs(oldr, val));
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
                ResultStruct oldr = val;
                val.Quantity = value;
                if (oldr != val && IsChangedHandlers())
                    OnChanged(this, new BaseChangedData.ChangedEventsArgs(oldr, val));
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
                ResultStruct oldr = val;
                val.Born = value;
                if (oldr != val && IsChangedHandlers())
                    OnChanged(this, new BaseChangedData.ChangedEventsArgs(oldr, val));
            }
        }
        public void EmptyAndDead()
        {
            ResultStruct oldr = val;
            val.EmptyAndDead();
            if (oldr != val && IsChangedHandlers())
                OnChanged(this, new BaseChangedData.ChangedEventsArgs(oldr, val));
        }
        public void DontCare()
        {
            ResultStruct oldr = val;
            val.DontCare();
            if (oldr != val && IsChangedHandlers())
                OnChanged(this, new BaseChangedData.ChangedEventsArgs(oldr, val));
        }
        public int RegisteredContractQuantity
        {
            get
            {
                return this.val.contract_quantity__for_correct;
            }
            set
            {
                this.val.contract_quantity__for_correct = value;
            }
        }

        override public bool IsDefined()
        {
            return val.Defined;
        }
        public bool IsContractNo()
        {
            return val.IsContractNO();
        }

        public bool IsMin
        {
            get
            {
                return val.IsMin;
            }
        }
        public bool IsMax
        {
            get
            {
                return val.IsMax;
            }
        }

        // Взятки
        public int GetTricks()
        {
            return val.GetTricks();
        }
        public void SetFromTricks(int tricks)
        {
            ResultStruct oldr = val;
            val.SetFromTricks(tricks);
            if (oldr != val && IsChangedHandlers())
                OnChanged(this, new BaseChangedData.ChangedEventsArgs(oldr, val));
        }

        // Получение строкового представления результата
        public override String ToString()
        {
            return val.ToString();
        }

        // Обработка события изменения контракта
        public enum CorrectMethods { MakeNotDefined = 0, ShiftResult, NoChanges };
        public void CorrectWhenContractChanged(Contract c, CorrectMethods met)
        {
            if(!c.IsDefined())
            {
                this.EmptyAndDead();
            }
            else if (c.NoContract)
            {
                this.DontCare();
            }
            else if (!this.Born)
            {
                this.EmptyAndDead();
            }
            else if (this.Born && this.RegisteredContractQuantity != c.Quantity && this.RegisteredContractQuantity != 0)
            {
                switch (met)
                {
                    case CorrectMethods.MakeNotDefined:
                        this.EmptyAndDead();
                        break;
                    case CorrectMethods.ShiftResult:
                        int diff = c.Quantity - this.RegisteredContractQuantity;
                        this.Quantity -= diff;
                        break;
                    case CorrectMethods.NoChanges:
                        if (!this.IsDefined())
                            this.Quantity = val.WhatQuantityWillBeGood();
                        break;
                }
            }


            // Сохранить текущее значение контракта:
            this.RegisteredContractQuantity = c.IsDefined() ? c.Quantity : 0;
         }


        // Сериализация SQL
        public void _FromDataBase(object v)
        {
            if (v == DBNull.Value)
                this.EmptyAndDead();
            else
                this.SetFromTricks((int)(byte)v);
         }
        public object _ToDataBase()
        {
            //return (this.IsDefined() ? this.Quantity.ToString() : "NULL");

            if(this.IsDefined())
                return this.GetTricks();
            else
                return DBNull.Value;
        }

        // Clear
        public override void Clear()
        {
            ResultStruct oldr = val;
            
            val.EmptyAndDead();
            val.contract_quantity__for_correct = 0;

            if (oldr != val && IsChangedHandlers())
                OnChanged(this, new BaseChangedData.ChangedEventsArgs(oldr, val));
        }
    }




    public partial class ResultSelectControl : BaseSelectControl, IAttachData<Result>, IDetachData
    {
        Result m_result;
        ResultSelector selector;

        public ResultSelectControl()
        {
            InitializeComponent();
            this.m_result = null;
        }

        public void AttachData(Result r)
        {
            // Присоединение результата:
            if (this.m_result == null)
            {
                this.m_result = r;
                this.m_result.Changed += OnResultChanged;

                // Отрисовать:
                this.Invalidate();
            }
        }

        public void DetachData(bool _inv)
        {
            // Отсоединение результата:
            if (this.m_result != null)
            {
                this.m_result.Changed -= OnResultChanged;
                this.m_result = null;

                // Отрисовать:
                if(_inv)
                    this.Invalidate();
            }
            // Закрыть селектор, если он открыт подо мной:
            if (SelectorOpened)
                CloseSelector(false);
        }

        void OnResultChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            // ? Возможно: если изменения извне (загрузка из файла или изменнеие контракта), закрыть (или обновить - сложнее) селектор

            this.Invalidate();
        }


        protected override void OpenSelector()
        {
            if (m_result == null)
                return;

            ResultSelector.GetInstance().OpenMe(this, this.SelectorCover, this.ParentForm, this.m_result);

            ResultSelector.GetInstance().Closing += OnSelectorClosing;
            ResultSelector.GetInstance().Closed += OnSelectorClosed;
            this.SelectorOpened = true;
        }

        protected override void CloseSelector(bool saveBeforeClose)
        {
            ResultSelector.GetInstance().CloseMe(saveBeforeClose);
        }

        void OnSelectorClosing(object sender, SelectorClosingEventArgs e)
        {
            //if (!(sender as ResultSelector).IsAttachedToThisControl(this))
            //    return;

            if (e != null && e.bSaveBeforeClose == true && this.m_result != null && e.dataToSaveBeforeClosed != null)
            {
                this.m_result.CopyResult(e.dataToSaveBeforeClosed as Result);
            }
        }

        void OnSelectorClosed(object sender, EventArgs e)
        {
            //if (!(sender as ResultSelector).IsAttachedToThisControl(this))
            //    return;

            ResultSelector.GetInstance().Closing -= OnSelectorClosing;
            ResultSelector.GetInstance().Closed -= OnSelectorClosed;

            this.SelectorOpened = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            // рисуем текст, если результат определен
            if (this.m_result!=null && this.m_result.IsDefined())
            {
                if (this.Focused || SelectorOpened)
                    m_rectf_String.Width -= splitter_width;

                SizeF szf1 = g.MeasureString(m_result.ToString(), this.Font);
                RectangleF rf = new RectangleF(m_rectf_String.X + text_left_offset, m_rectf_String.Y + (m_rectf_String.Height - szf1.Height) / 2, szf1.Width, szf1.Height);
                if (rf.Right > m_rectf_String.Right)
                    rf.Width = m_rectf_String.Right - rf.X;
                g.DrawString(m_result.ToString(), this.Font, (m_result.IsContractNo() ? m_brush_String_BLACK : (m_result.Quantity >= 0 ? m_brush_String_GREEN : m_brush_String_RED)), rf, new StringFormat(StringFormatFlags.NoWrap));

                if (this.Focused || SelectorOpened)
                    m_rectf_String.Width += splitter_width;
            }
        }
    }
}
