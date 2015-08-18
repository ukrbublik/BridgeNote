namespace BridgeProject
{
    partial class ShowTextControl
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
            if (strFonts != null)
            {
                for (int i = 0; i < strFonts.Length; i++)
                    strFonts[i].Dispose();
            }
            if (strBrushes != null)
            {
                for (int i = 0; i < strBrushes.Length; i++)
                    strBrushes[i].Dispose();
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
            // ShowTextControl
            // 
            this.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ShowTextControl_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
