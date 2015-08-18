namespace BridgeProject
{
    partial class GameOptionsForm_Comp
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
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.combo1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.radioButton10 = new System.Windows.Forms.RadioButton();
            this.radioButton9 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton8 = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Создать игру";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Отмена";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton4);
            this.panel1.Controls.Add(this.radioButton3);
            this.panel1.Controls.Add(this.radioButton5);
            this.panel1.Controls.Add(this.radioButton6);
            this.panel1.Location = new System.Drawing.Point(19, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(173, 130);
            // 
            // radioButton4
            // 
            this.radioButton4.Location = new System.Drawing.Point(50, 85);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(68, 40);
            this.radioButton4.TabIndex = 8;
            this.radioButton4.Text = "S";
            // 
            // radioButton3
            // 
            this.radioButton3.Checked = true;
            this.radioButton3.Location = new System.Drawing.Point(50, 3);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(68, 40);
            this.radioButton3.TabIndex = 7;
            this.radioButton3.Text = "N";
            // 
            // radioButton5
            // 
            this.radioButton5.Location = new System.Drawing.Point(3, 45);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(68, 40);
            this.radioButton5.TabIndex = 9;
            this.radioButton5.Text = "W";
            // 
            // radioButton6
            // 
            this.radioButton6.Location = new System.Drawing.Point(98, 45);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(68, 40);
            this.radioButton6.TabIndex = 10;
            this.radioButton6.Text = "E";
            // 
            // combo1
            // 
            this.combo1.Items.Add(8);
            this.combo1.Items.Add(12);
            this.combo1.Items.Add(16);
            this.combo1.Items.Add(20);
            this.combo1.Items.Add(24);
            this.combo1.Items.Add(28);
            this.combo1.Items.Add(32);
            this.combo1.Items.Add(64);
            this.combo1.Location = new System.Drawing.Point(182, 7);
            this.combo1.Name = "combo1";
            this.combo1.Size = new System.Drawing.Size(112, 41);
            this.combo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 33);
            this.label1.Text = "Первый сдающий:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 40);
            this.label2.Text = "Сдач в матче:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(253, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(186, 68);
            this.label3.Text = "Чередование зон/сдающего:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioButton2);
            this.panel2.Controls.Add(this.radioButton7);
            this.panel2.Location = new System.Drawing.Point(214, 123);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(258, 85);
            // 
            // radioButton2
            // 
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(3, 3);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(199, 40);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.Text = "плавает зона";
            // 
            // radioButton7
            // 
            this.radioButton7.Location = new System.Drawing.Point(3, 40);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(250, 40);
            this.radioButton7.TabIndex = 9;
            this.radioButton7.Text = "плавает сдающий";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 252);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(217, 33);
            this.label4.Text = "Тип компенсации:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkBox2);
            this.panel3.Controls.Add(this.radioButton10);
            this.panel3.Controls.Add(this.radioButton9);
            this.panel3.Controls.Add(this.radioButton1);
            this.panel3.Controls.Add(this.radioButton8);
            this.panel3.Controls.Add(this.checkBox1);
            this.panel3.Location = new System.Drawing.Point(2, 288);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(418, 220);
            // 
            // checkBox2
            // 
            this.checkBox2.Enabled = false;
            this.checkBox2.Location = new System.Drawing.Point(205, 83);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(198, 38);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "< при 23 (2)";
            // 
            // radioButton10
            // 
            this.radioButton10.Location = new System.Drawing.Point(3, 173);
            this.radioButton10.Name = "radioButton10";
            this.radioButton10.Size = new System.Drawing.Size(180, 41);
            this.radioButton10.TabIndex = 11;
            this.radioButton10.Text = "чикагская";
            // 
            // radioButton9
            // 
            this.radioButton9.Location = new System.Drawing.Point(3, 127);
            this.radioButton9.Name = "radioButton9";
            this.radioButton9.Size = new System.Drawing.Size(230, 40);
            this.radioButton9.TabIndex = 10;
            this.radioButton9.Text = "Милтона-Уорка";
            // 
            // radioButton1
            // 
            this.radioButton1.Location = new System.Drawing.Point(3, 44);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(199, 40);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = false;
            this.radioButton1.Text = "европейская";
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton8
            // 
            this.radioButton8.Checked = true;
            this.radioButton8.Location = new System.Drawing.Point(3, 3);
            this.radioButton8.Name = "radioButton8";
            this.radioButton8.Size = new System.Drawing.Size(186, 40);
            this.radioButton8.TabIndex = 9;
            this.radioButton8.Text = "московская";
            // 
            // checkBox1
            // 
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(42, 83);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(147, 38);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "10 за 2";
            // 
            // GameOptionsForm_Comp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 536);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.combo1);
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu1;
            this.Name = "GameOptionsForm_Comp";
            this.Text = "Настройки игры (компенс.)";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox combo1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton radioButton10;
        private System.Windows.Forms.RadioButton radioButton9;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton8;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}