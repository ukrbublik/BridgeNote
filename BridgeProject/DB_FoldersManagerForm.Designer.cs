using System.Windows.Forms;
namespace BridgeProject
{
    partial class DB_FoldersManagerForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс  следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.m_combo1 = new System.Windows.Forms.ComboBox();
            this.m_combo2 = new System.Windows.Forms.ComboBox();
            this.m_textbox1 = new System.Windows.Forms.TextBox();
            this.m_textbox2 = new System.Windows.Forms.TextBox();
            this.m_btn1 = new System.Windows.Forms.Button();
            this.m_btn2 = new System.Windows.Forms.Button();
            this.m_btn3 = new System.Windows.Forms.Button();
            this.m_btn4 = new System.Windows.Forms.Button();
            this.m_btn5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_combo1
            // 
            this.m_combo1.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.m_combo1.Location = new System.Drawing.Point(9, 153);
            this.m_combo1.Name = "m_combo1";
            this.m_combo1.Size = new System.Drawing.Size(459, 35);
            this.m_combo1.TabIndex = 0;
            this.m_combo1.SelectedIndexChanged += new System.EventHandler(this.m_combo1_SelectedIndexChanged);
            // 
            // m_combo2
            // 
            this.m_combo2.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.m_combo2.Location = new System.Drawing.Point(9, 304);
            this.m_combo2.Name = "m_combo2";
            this.m_combo2.Size = new System.Drawing.Size(283, 35);
            this.m_combo2.TabIndex = 1;
            this.m_combo2.SelectedIndexChanged += new System.EventHandler(this.m_combo2_SelectedIndexChanged);
            // 
            // m_textbox1
            // 
            this.m_textbox1.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.m_textbox1.Location = new System.Drawing.Point(9, 51);
            this.m_textbox1.Name = "m_textbox1";
            this.m_textbox1.Size = new System.Drawing.Size(283, 35);
            this.m_textbox1.TabIndex = 2;
            // 
            // m_textbox2
            // 
            this.m_textbox2.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.m_textbox2.Location = new System.Drawing.Point(9, 227);
            this.m_textbox2.Name = "m_textbox2";
            this.m_textbox2.Size = new System.Drawing.Size(283, 35);
            this.m_textbox2.TabIndex = 3;
            // 
            // m_btn1
            // 
            this.m_btn1.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            this.m_btn1.Location = new System.Drawing.Point(298, 51);
            this.m_btn1.Name = "m_btn1";
            this.m_btn1.Size = new System.Drawing.Size(170, 35);
            this.m_btn1.TabIndex = 4;
            this.m_btn1.Text = "Добавить";
            this.m_btn1.Click += new System.EventHandler(this.m_btn1_Click);
            // 
            // m_btn2
            // 
            this.m_btn2.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            this.m_btn2.Location = new System.Drawing.Point(298, 304);
            this.m_btn2.Name = "m_btn2";
            this.m_btn2.Size = new System.Drawing.Size(170, 35);
            this.m_btn2.TabIndex = 5;
            this.m_btn2.Text = "Переместить";
            this.m_btn2.Click += new System.EventHandler(this.m_btn2_Click);
            // 
            // m_btn3
            // 
            this.m_btn3.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            this.m_btn3.Location = new System.Drawing.Point(298, 227);
            this.m_btn3.Name = "m_btn3";
            this.m_btn3.Size = new System.Drawing.Size(170, 35);
            this.m_btn3.TabIndex = 6;
            this.m_btn3.Text = "Переименовать";
            this.m_btn3.Click += new System.EventHandler(this.m_btn3_Click);
            // 
            // m_btn4
            // 
            this.m_btn4.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            this.m_btn4.Location = new System.Drawing.Point(298, 359);
            this.m_btn4.Name = "m_btn4";
            this.m_btn4.Size = new System.Drawing.Size(170, 35);
            this.m_btn4.TabIndex = 7;
            this.m_btn4.Text = "Очистить";
            this.m_btn4.Click += new System.EventHandler(this.m_btn4_Click);
            // 
            // m_btn5
            // 
            this.m_btn5.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            this.m_btn5.Location = new System.Drawing.Point(298, 411);
            this.m_btn5.Name = "m_btn5";
            this.m_btn5.Size = new System.Drawing.Size(170, 35);
            this.m_btn5.TabIndex = 8;
            this.m_btn5.Text = "Удалить";
            this.m_btn5.Click += new System.EventHandler(this.m_btn5_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 26);
            this.label1.Text = "Добавить новую папку:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label2.Location = new System.Drawing.Point(9, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 26);
            this.label2.Text = "Выберите папку:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label3.Location = new System.Drawing.Point(9, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(230, 26);
            this.label3.Text = "Переименовать папку:";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label4.Location = new System.Drawing.Point(9, 275);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(325, 26);
            this.label4.Text = "Переместить игры в другую папку:";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label5.Location = new System.Drawing.Point(9, 363);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 26);
            this.label5.Text = "Удалить все игры:";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label6.Location = new System.Drawing.Point(9, 416);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(251, 26);
            this.label6.Text = "Удалить папку и все игры:";
            // 
            // DB_FoldersManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 536);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_combo1);
            this.Controls.Add(this.m_combo2);
            this.Controls.Add(this.m_textbox1);
            this.Controls.Add(this.m_textbox2);
            this.Controls.Add(this.m_btn1);
            this.Controls.Add(this.m_btn2);
            this.Controls.Add(this.m_btn3);
            this.Controls.Add(this.m_btn4);
            this.Controls.Add(this.m_btn5);
            this.Controls.Add(this.label1);
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "DB_FoldersManagerForm";
            this.Text = "Папки";
            this.ResumeLayout(false);

        }

        #endregion

        private ComboBox m_combo1;
        private ComboBox m_combo2;
        private TextBox m_textbox1;
        private TextBox m_textbox2;
        private Button m_btn1;
        private Button m_btn2;
        private Button m_btn3;
        private Button m_btn4;
        private Button m_btn5;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}