namespace BridgeProject
{
    partial class SwitcherControl
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
            m_brush_String.Dispose();
            if (highlightPens != null)
            {
                for (int i = 0; i < highlightPens.Length; i++)
                    highlightPens[i].Dispose();
            }

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
            // Switcher
            // 
            if (this.can_change)
            {
                this.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular);
                this.MouseDown += /*new System.Windows.Forms.MouseEventHandler(..)*/ this.Switcher_MouseDown;
                this.MouseMove += this.Switcher_MouseMove;
                this.MouseUp += this.Switcher_MouseUp;
            }

            this.ResumeLayout(false);

        }

        #endregion
    }
}
