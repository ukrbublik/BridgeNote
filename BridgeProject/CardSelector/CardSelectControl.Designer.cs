using System.Windows.Forms;
namespace BridgeProject
{
    partial class CardSelectControl
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
            this.selector = new CardSelector(this);
            this.SuspendLayout();
            //
            // selector
            //
            this.selector.Name = "CardSelector";
            this.selector.TabIndex = 2;
            // 
            // CardSelectControl
            // 
            this.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular);
            this.GotFocus += new System.EventHandler(this.CardSelectControl_GotFocus);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CardSelectControl_MouseDown);
            this.Resize += new System.EventHandler(this.CardSelectControl_Resize);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CardSelectControl_KeyPress);
            this.LostFocus += new System.EventHandler(this.CardSelectControl_LostFocus);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CardSelectControl_MouseUp);
            this.ResumeLayout(false);
        }


        #endregion
    }
}
