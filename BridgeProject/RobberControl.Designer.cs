namespace BridgeProject
{
    partial class RobberControl
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
            brushHeader.Dispose();
            brushText.Dispose();
            fontHeader.Dispose();

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
            components = new System.ComponentModel.Container();
            //this.Size = new System.Drawing.Size(160, 400);
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Tahoma", 6, System.Drawing.FontStyle.Regular);
        }

        #endregion
    }
}
