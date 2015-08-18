using System.Windows.Forms;
namespace BridgeProject
{
    partial class ShporaForm
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
            t = new ShporaControl();
            label = new Label();
            this.SuspendLayout();
            // 
            // ShporaForm
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 536);
            this.Location = new System.Drawing.Point(0, 52);
            this.Controls.Add(t);
            this.Controls.Add(label);
            label.Font = new System.Drawing.Font("Tahoma", 7, System.Drawing.FontStyle.Italic);
            label.Location = new System.Drawing.Point(10, 2);
            label.Width = this.Width - 2 * 10;
            label.Height = 24;
            label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //this.t.Location = new System.Drawing.Point(5, 5);
            //this.t.Size = new System.Drawing.Size(this.Width - 10, this.Height - 10);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "ShporaForm";
            this.Text = "Справка";
            this.ResumeLayout(false);

        }

        #endregion

        public ShporaControl t;
        public Label label;
    }
}