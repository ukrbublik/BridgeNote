namespace BridgeProject
{
    partial class CardsDistributionShowForm
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
            this.cardsDistributionWatcher1 = new BridgeProject.CardsDistributionWatcher();
            this.SuspendLayout();
            // 
            // cardsDistributionWatcher1
            // 
            this.cardsDistributionWatcher1.Location = new System.Drawing.Point(5, 8);
            this.cardsDistributionWatcher1.Name = "cardsDistributionWatcher1";
            this.cardsDistributionWatcher1.Size = new System.Drawing.Size(230, 240);
            this.cardsDistributionWatcher1.TabIndex = 0;
            this.cardsDistributionWatcher1.Text = "cardsDistributionWatcher1";
            this.cardsDistributionWatcher1.AttachData(this.cd);
            // 
            // CardsDistributionShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.cardsDistributionWatcher1);
            this.Menu = this.mainMenu1;
            this.Name = "CardsDistributionShowForm";
            this.Text = "Распределение карт (просмотр)";
            this.ResumeLayout(false);

        }

        #endregion

        private CardsDistributionWatcher cardsDistributionWatcher1;
    }
}