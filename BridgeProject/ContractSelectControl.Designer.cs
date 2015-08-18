using System.Drawing;
namespace BridgeProject
{
    partial class ContractSelectControl
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
            m_font_for_suits.Dispose();

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
            m_font_for_suits = new Font("Courier New", 9F, FontStyle.Regular);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
