namespace BridgeProject
{
    partial class ResultSelector
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
            // ContractSelector
            // 
            this.Name = "ResultSelector";
            this.Size = static_size;
            this.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResultSelector_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ResultSelector_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResultSelector_MouseUp);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
