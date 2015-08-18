namespace BridgeProject
{
    partial class CardSelector
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не меняйте 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CardSelector
            // 
            this.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular);
            this.GotFocus += new System.EventHandler(this.CardSelector_GotFocus);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CardSelector_MouseDown);
            this.LostFocus += new System.EventHandler(this.CardSelector_LostFocus);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CardSelector_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
