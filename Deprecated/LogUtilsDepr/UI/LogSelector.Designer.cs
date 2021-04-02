namespace LogUtils.UI
{
    partial class LogSelector
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
            this.list_category = new System.Windows.Forms.ListBox();
            this.cb_show = new System.Windows.Forms.CheckBox();
            this.panel_textColor = new System.Windows.Forms.Panel();
            this.panel_backColor = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_level = new System.Windows.Forms.ComboBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // list_category
            // 
            this.list_category.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.list_category.FormattingEnabled = true;
            this.list_category.ItemHeight = 16;
            this.list_category.Location = new System.Drawing.Point(12, 12);
            this.list_category.Name = "list_category";
            this.list_category.Size = new System.Drawing.Size(243, 180);
            this.list_category.TabIndex = 0;
            this.list_category.SelectedIndexChanged += new System.EventHandler(this.Category_SelectedIndexChanged);
            // 
            // cb_show
            // 
            this.cb_show.AutoSize = true;
            this.cb_show.Location = new System.Drawing.Point(293, 38);
            this.cb_show.Name = "cb_show";
            this.cb_show.Size = new System.Drawing.Size(62, 21);
            this.cb_show.TabIndex = 1;
            this.cb_show.Text = "show";
            this.cb_show.UseVisualStyleBackColor = true;
            this.cb_show.CheckedChanged += new System.EventHandler(this.Show_CheckedChanged);
            // 
            // panel_textColor
            // 
            this.panel_textColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_textColor.Location = new System.Drawing.Point(266, 65);
            this.panel_textColor.Name = "panel_textColor";
            this.panel_textColor.Size = new System.Drawing.Size(30, 30);
            this.panel_textColor.TabIndex = 2;
            // 
            // panel_backColor
            // 
            this.panel_backColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_backColor.Location = new System.Drawing.Point(266, 101);
            this.panel_backColor.Name = "panel_backColor";
            this.panel_backColor.Size = new System.Drawing.Size(30, 30);
            this.panel_backColor.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(304, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "text color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(305, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "back color";
            // 
            // cb_level
            // 
            this.cb_level.FormattingEnabled = true;
            this.cb_level.Items.AddRange(new object[] {
            "Trace",
            "Debug",
            "Info",
            "Warning",
            "Error",
            "Fatal"});
            this.cb_level.Location = new System.Drawing.Point(266, 137);
            this.cb_level.Name = "cb_level";
            this.cb_level.Size = new System.Drawing.Size(121, 24);
            this.cb_level.TabIndex = 4;
            this.cb_level.Text = "Info";
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(293, 167);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 23);
            this.button_ok.TabIndex = 5;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // LogSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(396, 204);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.cb_level);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel_backColor);
            this.Controls.Add(this.panel_textColor);
            this.Controls.Add(this.cb_show);
            this.Controls.Add(this.list_category);
            this.Name = "LogSelector";
            this.Text = "LogSelector";
            this.Load += new System.EventHandler(this.LogSelector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox list_category;
        private System.Windows.Forms.CheckBox cb_show;
        private System.Windows.Forms.Panel panel_textColor;
        private System.Windows.Forms.Panel panel_backColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cb_level;
        private System.Windows.Forms.Button button_ok;
    }
}