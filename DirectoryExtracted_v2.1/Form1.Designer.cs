namespace DirectoryExtracted_v2._1
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxSaveDirPath = new System.Windows.Forms.TextBox();
            this.buttonSetSaveDir = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.listBoxDir = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxAll = new System.Windows.Forms.CheckBox();
            this.checkBoxFasad = new System.Windows.Forms.CheckBox();
            this.checkBoxOtdelka = new System.Windows.Forms.CheckBox();
            this.checkBoxPlanning = new System.Windows.Forms.CheckBox();
            this.checkBoxKvart = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.listBoxDownloadStat = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbMax = new System.Windows.Forms.RadioButton();
            this.rbMedium = new System.Windows.Forms.RadioButton();
            this.rbLow = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSaveDirPath
            // 
            this.textBoxSaveDirPath.Location = new System.Drawing.Point(12, 48);
            this.textBoxSaveDirPath.Name = "textBoxSaveDirPath";
            this.textBoxSaveDirPath.Size = new System.Drawing.Size(304, 20);
            this.textBoxSaveDirPath.TabIndex = 0;
            // 
            // buttonSetSaveDir
            // 
            this.buttonSetSaveDir.Location = new System.Drawing.Point(322, 46);
            this.buttonSetSaveDir.Name = "buttonSetSaveDir";
            this.buttonSetSaveDir.Size = new System.Drawing.Size(75, 23);
            this.buttonSetSaveDir.TabIndex = 1;
            this.buttonSetSaveDir.Text = "...";
            this.buttonSetSaveDir.UseVisualStyleBackColor = true;
            this.buttonSetSaveDir.Click += new System.EventHandler(this.buttonSetSaveDir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Папка для сохранения";
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(322, 92);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 25);
            this.buttonLoad.TabIndex = 3;
            this.buttonLoad.Text = "Загрузить";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // listBoxDir
            // 
            this.listBoxDir.FormattingEnabled = true;
            this.listBoxDir.HorizontalScrollbar = true;
            this.listBoxDir.Location = new System.Drawing.Point(15, 133);
            this.listBoxDir.Name = "listBoxDir";
            this.listBoxDir.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBoxDir.Size = new System.Drawing.Size(243, 134);
            this.listBoxDir.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxAll);
            this.groupBox1.Controls.Add(this.checkBoxFasad);
            this.groupBox1.Controls.Add(this.checkBoxOtdelka);
            this.groupBox1.Controls.Add(this.checkBoxPlanning);
            this.groupBox1.Controls.Add(this.checkBoxKvart);
            this.groupBox1.Location = new System.Drawing.Point(273, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(124, 141);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Что скачать";
            // 
            // checkBoxAll
            // 
            this.checkBoxAll.AutoSize = true;
            this.checkBoxAll.Location = new System.Drawing.Point(7, 112);
            this.checkBoxAll.Name = "checkBoxAll";
            this.checkBoxAll.Size = new System.Drawing.Size(45, 17);
            this.checkBoxAll.TabIndex = 4;
            this.checkBoxAll.Text = "Все";
            this.checkBoxAll.UseVisualStyleBackColor = true;
            this.checkBoxAll.CheckedChanged += new System.EventHandler(this.checkBoxAll_CheckedChanged);
            // 
            // checkBoxFasad
            // 
            this.checkBoxFasad.AutoSize = true;
            this.checkBoxFasad.Location = new System.Drawing.Point(7, 89);
            this.checkBoxFasad.Name = "checkBoxFasad";
            this.checkBoxFasad.Size = new System.Drawing.Size(107, 17);
            this.checkBoxFasad.TabIndex = 3;
            this.checkBoxFasad.Text = "Фасады и виды";
            this.checkBoxFasad.UseVisualStyleBackColor = true;
            // 
            // checkBoxOtdelka
            // 
            this.checkBoxOtdelka.AutoSize = true;
            this.checkBoxOtdelka.Location = new System.Drawing.Point(7, 66);
            this.checkBoxOtdelka.Name = "checkBoxOtdelka";
            this.checkBoxOtdelka.Size = new System.Drawing.Size(98, 17);
            this.checkBoxOtdelka.TabIndex = 2;
            this.checkBoxOtdelka.Text = "Фото отделки";
            this.checkBoxOtdelka.UseVisualStyleBackColor = true;
            // 
            // checkBoxPlanning
            // 
            this.checkBoxPlanning.AutoSize = true;
            this.checkBoxPlanning.Location = new System.Drawing.Point(7, 43);
            this.checkBoxPlanning.Name = "checkBoxPlanning";
            this.checkBoxPlanning.Size = new System.Drawing.Size(88, 17);
            this.checkBoxPlanning.TabIndex = 1;
            this.checkBoxPlanning.Text = "Планировки";
            this.checkBoxPlanning.UseVisualStyleBackColor = true;
            // 
            // checkBoxKvart
            // 
            this.checkBoxKvart.AutoSize = true;
            this.checkBoxKvart.Location = new System.Drawing.Point(7, 20);
            this.checkBoxKvart.Name = "checkBoxKvart";
            this.checkBoxKvart.Size = new System.Drawing.Size(111, 17);
            this.checkBoxKvart.TabIndex = 0;
            this.checkBoxKvart.Text = "Квартирографии";
            this.checkBoxKvart.UseVisualStyleBackColor = true;
            // 
            // listBoxDownloadStat
            // 
            this.listBoxDownloadStat.FormattingEnabled = true;
            this.listBoxDownloadStat.HorizontalScrollbar = true;
            this.listBoxDownloadStat.Location = new System.Drawing.Point(15, 288);
            this.listBoxDownloadStat.Name = "listBoxDownloadStat";
            this.listBoxDownloadStat.Size = new System.Drawing.Size(382, 69);
            this.listBoxDownloadStat.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 272);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Статистика загрузки";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbMax);
            this.groupBox2.Controls.Add(this.rbMedium);
            this.groupBox2.Controls.Add(this.rbLow);
            this.groupBox2.Location = new System.Drawing.Point(15, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 43);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Кол-во соединений";
            // 
            // rbMax
            // 
            this.rbMax.AutoSize = true;
            this.rbMax.Location = new System.Drawing.Point(164, 19);
            this.rbMax.Name = "rbMax";
            this.rbMax.Size = new System.Drawing.Size(55, 17);
            this.rbMax.TabIndex = 2;
            this.rbMax.Text = "Макс.";
            this.rbMax.UseVisualStyleBackColor = true;
            this.rbMax.CheckedChanged += new System.EventHandler(this.rbMax_CheckedChanged);
            // 
            // rbMedium
            // 
            this.rbMedium.AutoSize = true;
            this.rbMedium.Checked = true;
            this.rbMedium.Location = new System.Drawing.Point(86, 18);
            this.rbMedium.Name = "rbMedium";
            this.rbMedium.Size = new System.Drawing.Size(53, 17);
            this.rbMedium.TabIndex = 1;
            this.rbMedium.TabStop = true;
            this.rbMedium.Text = "Сред.";
            this.rbMedium.UseVisualStyleBackColor = true;
            this.rbMedium.CheckedChanged += new System.EventHandler(this.rbMedium_CheckedChanged);
            // 
            // rbLow
            // 
            this.rbLow.AutoSize = true;
            this.rbLow.Location = new System.Drawing.Point(6, 18);
            this.rbLow.Name = "rbLow";
            this.rbLow.Size = new System.Drawing.Size(49, 17);
            this.rbLow.TabIndex = 0;
            this.rbLow.Text = "Мин.";
            this.rbLow.UseVisualStyleBackColor = true;
            this.rbLow.CheckedChanged += new System.EventHandler(this.rbLow_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 369);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxDownloadStat);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBoxDir);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSetSaveDir);
            this.Controls.Add(this.textBoxSaveDirPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSaveDirPath;
        private System.Windows.Forms.Button buttonSetSaveDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.ListBox listBoxDir;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxPlanning;
        private System.Windows.Forms.CheckBox checkBoxKvart;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ListBox listBoxDownloadStat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxAll;
        private System.Windows.Forms.CheckBox checkBoxFasad;
        private System.Windows.Forms.CheckBox checkBoxOtdelka;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbMax;
        private System.Windows.Forms.RadioButton rbMedium;
        private System.Windows.Forms.RadioButton rbLow;
    }
}

