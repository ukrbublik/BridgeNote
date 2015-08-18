using System.Drawing;
namespace BridgeProject
{
    partial class BridgeScoreTable
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс  следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            DisposeMe();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BridgeScoreTable
            // 
            this.Name = "BridgeScoreTable";
            this.Font = new Font(this.Font.Name, 6, FontStyle.Regular);

            this.fontHeader = new Font("Tahoma", 5, FontStyle.Bold);
            this.BackColor = Color.White;

            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

this.AutoScroll = true; //o_O

            this.ResumeLayout(false);
        }

        #endregion
    }
}
