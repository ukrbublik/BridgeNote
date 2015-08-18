using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace BridgeProject
{
    class NumTextBox : TextBox
    {
        public bool TextFilter_AllowMinus = true;
        public bool TextFilter_AllowDot = true;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            Char c = e.KeyChar;
            String strAllow = "\b0123456789"; //27 - esc, \r - enter
            if (TextFilter_AllowMinus)
                strAllow += "-";
            if (TextFilter_AllowDot)
                strAllow += ".";
            if (strAllow.Contains(c))
                base.OnKeyPress(e);
            else
                e.Handled = true;
        }

    }


    class TextBoxInTable : UserControl, ITotalFocus, IControlInTable, IAttachData<BaseChangedData>, IDetachData, ILock
    {
        NumTextBox m_TextBox;
        //Button m_BtnClose;
        Control m_crlFake;

        Color mem_backColor;

        public TextBoxInTable()
            : base()
        {

            this.BackColor = Color.White;
            mem_backColor = this.BackColor;

            m_TextBox = new NumTextBox();
            //m_BtnClose = new Button();
            m_crlFake = new Control();

            m_TextBox.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular);
            m_TextBox.BorderStyle = BorderStyle.None;
            m_TextBox.HideSelection = true;
            m_TextBox.Multiline = false;
            m_TextBox.MaxLength = 2;
            m_TextBox.Size = new System.Drawing.Size(this.Width - 2, this.Height);
            m_TextBox.Location = new System.Drawing.Point(1, (this.Height - m_TextBox.Height) / 2);
            m_TextBox.BackColor = this.BackColor;
            m_TextBox.GotFocus += new EventHandler(m_TextBox_GotFocus);
            m_TextBox.LostFocus += new EventHandler(m_TextBox_LostFocus);
            m_TextBox.TextChanged += new EventHandler(m_TextBox_TextChanged);
            m_TextBox.KeyDown += TextBox_OnKeyDown;

            m_TextBox.ReadOnly = true;

            //m_BtnClose.Size = new System.Drawing.Size(10, 10);

            m_crlFake.Location = new Point(0, 0);
            m_crlFake.Size = new Size(0, 0);


            this.GotFocus += OnGotFocus;
            this.LostFocus += OnLostFocus;
            this.Controls.Add(m_TextBox);
            m_TextBox.GotFocus += OnGotFocus;
            m_TextBox.LostFocus += OnLostFocus;
            //this.Controls.Add(m_BtnClose);
            //m_BtnClose.GotFocus += OnGotFocus;
            //m_BtnClose.LostFocus += OnLostFocus;
            this.Controls.Add(m_crlFake);
            m_crlFake.GotFocus += OnGotFocus;
            m_crlFake.LostFocus += OnLostFocus;

            this.LostTotalFocus += OnLostTotalFocus;
        }

        void m_TextBox_TextChanged(object sender, EventArgs e)
        {
            // *********** Добавить кнопку [отмена] ***************
            // *********** При нажатии она вызывает Synchronize(false) и исчезает.
        }

        void OnLostTotalFocus(object sender, EventArgs e)
        {
            Synchronize(true); //принять изменения
        }

        protected void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            Keys k = e.KeyData;
            if (k == Keys.Escape)
            {
                this.m_crlFake.Focus();
                Synchronize(false); //вернуть как было
            }
            if (k == Keys.Enter)
            {
                this.m_crlFake.Focus();
                Synchronize(true); //принять изменения
            }

            base.OnKeyDown(e);
        }

        void m_TextBox_GotFocus(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
            m_TextBox.BackColor = this.BackColor;
        }
        
        void m_TextBox_LostFocus(object sender, EventArgs e)
        {
            this.BackColor = mem_backColor;
            m_TextBox.BackColor = this.BackColor;

            // ************* Если есть кнопка [отмена], убрать ее ************
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            m_TextBox.Size = new System.Drawing.Size(this.Width - 2, this.Height);
            m_TextBox.Location = new System.Drawing.Point(1, (this.Height - m_TextBox.Height) / 2);

            //m_BtnClose.Location = new Point(this.Width - 1 - m_BtnClose.Width - 1, (this.Height - m_BtnClose.Height) / 2);
        }

        //
        // Синхронизация
        //

        void Synchronize(bool saveToData)
        {
            if (m_value == null)
            {
                this.m_TextBox.Text = "";
                return;
            }

            // текст бокс -> данные
            if (saveToData == true)
            {
                //if ((m_value as IFromString).FromString(this.m_TextBox.Text) == false) //попытаться пропарсить
                //    this.m_TextBox.Text = m_value.ToString(); //вернуть как было

                (m_value as IFromString).FromString(this.m_TextBox.Text); //попытаться пропарсить
                this.m_TextBox.Text = m_value.ToString(); //загрузить данные в текст-бокс
            }
            // данные -> текст бокс
            else
            {
                this.m_TextBox.Text = m_value.ToString();
            }
        }

        //
        // IAttachData
        //

        BaseChangedData m_value;

        public void AttachData(BaseChangedData data)
        {
            if (m_value == null)
            {
                m_value = data;
                m_value.Changed += OnDataChanged;

                OnDataChanged(this, null);

                // Включить текстовый фильтр для типа int
                if (data.GetType() == typeof(IntData))
                {
                    m_TextBox.TextFilter_AllowMinus = true;
                    m_TextBox.TextFilter_AllowDot = false;
                }

                if (!data.GetType().GetInterfaces().Contains(typeof(IFromString)))
                    throw new Exception("Тип данных, подключаемый к TextBoxInTable, не поддерживает интерфейс IFromString");
            }
        }
        public void DetachData(bool _inv)
        {
            if (m_value != null)
            {
                m_value.Changed -= OnDataChanged;
                m_value = null;

                if(_inv)
                    OnDataChanged(this, null);
            }
        }

        public void OnDataChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            Synchronize(false);
        }


        //
        // ITotalFocus
        //

        public event EventHandler GotTotalFocus;
        public event EventHandler LostTotalFocus;
        bool bTotalFocus;
        public bool HaveTotalFocus()
        {
            return bTotalFocus;
        }

        void OnGotFocus(object sender, EventArgs e)
        {
            if (!bTotalFocus)
            {
                bTotalFocus = true;
                if (GotTotalFocus != null)
                    GotTotalFocus(this, null);
            }
        }

        void OnLostFocus(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Controls.Count; i++)
                if (this.Controls[i].Focused)
                    return;

            bTotalFocus = false;
            if (LostTotalFocus != null)
                LostTotalFocus(this, null);
        }

        //
        // IControlInTable
        //

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


        //
        // ILock
        //
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

        public virtual void OnLocked()
        {
            m_TextBox.ReadOnly = true;
        }

        public virtual void OnUnlocked()
        {
            m_TextBox.ReadOnly = false;          
        }

        #endregion
    }
}
