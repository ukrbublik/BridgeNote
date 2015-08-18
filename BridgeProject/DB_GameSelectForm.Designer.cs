using System.Windows.Forms;
namespace BridgeProject
{
    partial class DB_GameSelectForm
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
            this.m_listview_Games = new System.Windows.Forms.ListView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.m_combo_Folders = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_listview_Games
            // 
            this.m_listview_Games.FullRowSelect = true;
            this.m_listview_Games.Location = new System.Drawing.Point(6, 53);
            this.m_listview_Games.Name = "m_listview_Games";
            this.m_listview_Games.Size = new System.Drawing.Size(468, 477);
            this.m_listview_Games.TabIndex = 0;
            this.m_listview_Games.View = System.Windows.Forms.View.Details;
            this.m_listview_Games.ItemActivate += new System.EventHandler(this.m_listview_Games_ItemActivate);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Новая игра";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Отмена";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // m_combo_Folders
            // 
            this.m_combo_Folders.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.m_combo_Folders.Location = new System.Drawing.Point(89, 9);
            this.m_combo_Folders.Name = "m_combo_Folders";
            this.m_combo_Folders.Size = new System.Drawing.Size(385, 35);
            this.m_combo_Folders.TabIndex = 1;
            this.m_combo_Folders.SelectedIndexChanged += new System.EventHandler(this.m_combo_Folders_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 29);
            this.label1.Text = "Папка:";
            // 
            // DB_GameSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 536);
            this.Controls.Add(this.m_listview_Games);
            this.Controls.Add(this.m_combo_Folders);
            this.Controls.Add(this.label1);
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu1;
            this.Name = "DB_GameSelectForm";
            this.Text = "Выбор игры";
            this.ResumeLayout(false);

        }

        #endregion
        private MenuItem menuItem1;
        private MenuItem menuItem2;
        private ListView m_listview_Games;
        private ComboBox m_combo_Folders;
        private Label label1;
    }
}