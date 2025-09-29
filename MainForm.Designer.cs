namespace AirQualityMonitoring
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
            components = new System.ComponentModel.Container();
            toolPanel = new Panel();
            districtComboBox = new ComboBox();
            refreshButton = new Button();
            titleLabel = new Label();
            mapPictureBox = new PictureBox();
            legendPanel = new Panel();
            legendTitle = new Label();
            statusLabel = new Label();
            updateTimer = new System.Windows.Forms.Timer(components);
            toolPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mapPictureBox).BeginInit();
            legendPanel.SuspendLayout();
            SuspendLayout();
            // 
            // toolPanel
            // 
            toolPanel.BackColor = Color.FromArgb(240, 240, 240);
            toolPanel.Controls.Add(districtComboBox);
            toolPanel.Controls.Add(refreshButton);
            toolPanel.Controls.Add(titleLabel);
            toolPanel.Dock = DockStyle.Top;
            toolPanel.Location = new Point(0, 0);
            toolPanel.Margin = new Padding(3, 2, 3, 2);
            toolPanel.Name = "toolPanel";
            toolPanel.Size = new Size(709, 60);
            toolPanel.TabIndex = 0;
            // 
            // districtComboBox
            // 
            districtComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            districtComboBox.Font = new Font("Segoe UI", 10F);
            districtComboBox.FormattingEnabled = true;
            districtComboBox.Location = new Point(521, 18);
            districtComboBox.Margin = new Padding(3, 2, 3, 2);
            districtComboBox.Name = "districtComboBox";
            districtComboBox.Size = new Size(176, 25);
            districtComboBox.TabIndex = 2;
            districtComboBox.SelectedIndexChanged += DistrictComboBox_SelectedIndexChanged;
            // 
            // refreshButton
            // 
            refreshButton.BackColor = Color.FromArgb(52, 152, 219);
            refreshButton.FlatStyle = FlatStyle.Flat;
            refreshButton.Font = new Font("Segoe UI", 10F);
            refreshButton.ForeColor = Color.White;
            refreshButton.Location = new Point(374, 11);
            refreshButton.Margin = new Padding(3, 2, 3, 2);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(131, 36);
            refreshButton.TabIndex = 1;
            refreshButton.Text = "🔄 Оновити дані";
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += RefreshButton_Click;
            // 
            // titleLabel
            // 
            titleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(50, 50, 50);
            titleLabel.Location = new Point(18, 11);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(350, 36);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "🏙️ Моніторинг якості повітря в м. Київ";
            // 
            // mapPictureBox
            // 
            mapPictureBox.BackColor = Color.White;
            mapPictureBox.BorderStyle = BorderStyle.FixedSingle;
            mapPictureBox.Dock = DockStyle.Fill;
            mapPictureBox.Location = new Point(0, 60);
            mapPictureBox.Margin = new Padding(3, 2, 3, 2);
            mapPictureBox.Name = "mapPictureBox";
            mapPictureBox.Size = new Size(476, 468);
            mapPictureBox.TabIndex = 1;
            mapPictureBox.TabStop = false;
            mapPictureBox.Paint += MapPictureBox_Paint;
            mapPictureBox.MouseClick += MapPictureBox_MouseClick;
            mapPictureBox.MouseMove += MapPictureBox_MouseMove;
            // 
            // legendPanel
            // 
            legendPanel.BackColor = Color.FromArgb(250, 250, 250);
            legendPanel.Controls.Add(legendTitle);
            legendPanel.Dock = DockStyle.Right;
            legendPanel.Location = new Point(476, 60);
            legendPanel.Margin = new Padding(3, 2, 3, 2);
            legendPanel.Name = "legendPanel";
            legendPanel.Size = new Size(233, 468);
            legendPanel.TabIndex = 2;
            // 
            // legendTitle
            // 
            legendTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            legendTitle.Location = new Point(9, 8);
            legendTitle.Name = "legendTitle";
            legendTitle.Size = new Size(201, 19);
            legendTitle.TabIndex = 0;
            legendTitle.Text = "Легенда AQI";
            // 
            // statusLabel
            // 
            statusLabel.BackColor = Color.FromArgb(230, 230, 230);
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Location = new Point(0, 528);
            statusLabel.Name = "statusLabel";
            statusLabel.Padding = new Padding(4, 0, 0, 0);
            statusLabel.Size = new Size(709, 19);
            statusLabel.TabIndex = 3;
            statusLabel.Text = "Готово до роботи";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // updateTimer
            // 
            updateTimer.Interval = 60000;
            updateTimer.Tick += UpdateTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 547);
            Controls.Add(mapPictureBox);
            Controls.Add(legendPanel);
            Controls.Add(statusLabel);
            Controls.Add(toolPanel);
            Margin = new Padding(3, 2, 3, 2);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Система моніторингу якості повітря - Київ";
            Load += MainForm_Load;
            toolPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mapPictureBox).EndInit();
            legendPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel toolPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ComboBox districtComboBox;
        private System.Windows.Forms.PictureBox mapPictureBox;
        private System.Windows.Forms.Panel legendPanel;
        private System.Windows.Forms.Label legendTitle;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Timer updateTimer;
    }
}