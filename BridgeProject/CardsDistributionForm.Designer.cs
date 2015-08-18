using System.Drawing;
using System.Windows.Forms;
namespace BridgeProject
{
    partial class CardsDistributionForm
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
            this.cardsDistributionSelector1 = new BridgeProject.CardsDistributionSelector();
            this.quarterSelectN = new QuarterSelect(Quarters.N, Color.FromArgb(0, 128, 255));
            this.quarterSelectS = new QuarterSelect(Quarters.S, Color.FromArgb(247, 199, 9));
            this.quarterSelectE = new QuarterSelect(Quarters.E, Color.LightGreen);
            this.quarterSelectW = new QuarterSelect(Quarters.W, Color.MediumPurple);
            this.tbN = new Label();
            this.tbS = new Label();
            this.tbE = new Label();
            this.tbW = new Label();
            btnOK = new Button();
            btnCANCEL = new Button();
            btnAUTO = new Button();
            this.SuspendLayout();
            //
            // btnOK
            //
            this.btnOK.Text = "OK";
            this.btnOK.Location = new Point(172, 6);
            this.btnOK.Size = new Size(60, 20);
            this.btnOK.Click += new System.EventHandler(btnOK_Click);
            //
            // btnCANCEL
            //
            this.btnCANCEL.Text = "Отмена";
            this.btnCANCEL.Location = new Point(172, 29);
            this.btnCANCEL.Size = new Size(60, 20);
            this.btnCANCEL.Click += new System.EventHandler(btnCANCEL_Click);
            //
            // btnAUTO
            //
            this.btnAUTO.Text = "Auto";
            this.btnAUTO.Location = new Point(172, 60);
            this.btnAUTO.Size = new Size(60, 20);
            this.btnAUTO.Click += new System.EventHandler(btnAUTO_Click);
            //
            // quarterSelectN
            //
            this.quarterSelectN.Location = new Point(5, 5);
            this.quarterSelectN.Size = new Size(20, 26);
            //
            // quarterSelectS
            //
            this.quarterSelectS.Location = new Point(30, 5);
            this.quarterSelectS.Size = new Size(20, 26);
            //
            // quarterSelectE
            //
            this.quarterSelectE.Location = new Point(55, 5);
            this.quarterSelectE.Size = new Size(20, 26);
            //
            // quarterSelectW
            //
            this.quarterSelectW.Location = new Point(80, 5);
            this.quarterSelectW.Size = new Size(20, 26);
            //
            // tbN
            //
            this.tbN.Location = new Point(5, 32);
            this.tbN.Size = new Size(20, 20);
            this.tbN.TextAlign = ContentAlignment.TopCenter;
            //
            // tbS
            //
            this.tbS.Location = new Point(30, 32);
            this.tbS.Size = new Size(20, 20);
            this.tbS.TextAlign = ContentAlignment.TopCenter;
            //
            // tbE
            //
            this.tbE.Location = new Point(55, 32);
            this.tbE.Size = new Size(20, 20);
            this.tbE.TextAlign = ContentAlignment.TopCenter;
            //
            // tbW
            //
            this.tbW.Location = new Point(80, 32);
            this.tbW.Size = new Size(20, 20);
            this.tbW.TextAlign = ContentAlignment.TopCenter;
            // 
            // cardsDistributionSelector1
            // 
            this.cardsDistributionSelector1.Location = new System.Drawing.Point(7, 65);
            this.cardsDistributionSelector1.Name = "cardsDistributionSelector1";
            this.cardsDistributionSelector1.Size = new System.Drawing.Size(140, 200);
            this.cardsDistributionSelector1.TabIndex = 0;
            this.cardsDistributionSelector1.Text = "cardsDistributionSelector1";
            this.cardsDistributionSelector1.AttachData(this.cd);
            this.cardsDistributionSelector1.AttachQuarter(this.quarterSelectN);
            this.cardsDistributionSelector1.AttachQuarter(this.quarterSelectS);
            this.cardsDistributionSelector1.AttachQuarter(this.quarterSelectE);
            this.cardsDistributionSelector1.AttachQuarter(this.quarterSelectW);
            this.quarterSelectN.SelectMe();
            //
            // cd
            //
            OnDataChanged(this, null);
            this.cd.Changed += new BaseChangedData.ChangedHandler(OnDataChanged);
            // 
            // CardsDistributionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.cardsDistributionSelector1);
            this.Controls.Add(this.quarterSelectN);
            this.Controls.Add(this.quarterSelectS);
            this.Controls.Add(this.quarterSelectE);
            this.Controls.Add(this.quarterSelectW);
            this.Controls.Add(this.tbN);
            this.Controls.Add(this.tbS);
            this.Controls.Add(this.tbE);
            this.Controls.Add(this.tbW);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCANCEL);
            this.Controls.Add(this.btnAUTO);
            this.Menu = this.mainMenu1;
            this.Name = "CardsDistributionForm";
            this.Text = "Распределение карт (редактор)";
            this.ResumeLayout(false);

        }

        void btnCANCEL_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        void btnOK_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        void btnAUTO_Click(object sender, System.EventArgs e)
        {
            if (q_CompleteMePlease != Quarters.NotDefinedYet)
            {
                this.cd.AutoCompleteMePlease(q_CompleteMePlease);
            }
        }

        Quarters q_CompleteMePlease = Quarters.NotDefinedYet;

        void OnDataChanged(object sender, BaseChangedData.ChangedEventsArgs e)
        {
            this.tbN.Text = this.cd.GetCount(Quarters.N).ToString();
            this.tbS.Text = this.cd.GetCount(Quarters.S).ToString();
            this.tbE.Text = this.cd.GetCount(Quarters.E).ToString();
            this.tbW.Text = this.cd.GetCount(Quarters.W).ToString();

            this.btnAUTO.Enabled = this.cd.CanMeAutocompletePlease(out q_CompleteMePlease);
        }

        #endregion

        private CardsDistributionSelector cardsDistributionSelector1;
        private QuarterSelect quarterSelectN;
        private QuarterSelect quarterSelectS;
        private QuarterSelect quarterSelectE;
        private QuarterSelect quarterSelectW;
        private Label tbN;
        private Label tbS;
        private Label tbE;
        private Label tbW;
        private Button btnOK;
        private Button btnCANCEL;
        private Button btnAUTO;
    }
}