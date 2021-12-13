namespace RoomBuilder
{
    partial class RoomBuilder
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
            this.btn_loadImage = new System.Windows.Forms.Button();
            this.btnCreateNewRoom = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNumTilesWide = new System.Windows.Forms.TextBox();
            this.tbNumTilesHigh = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBoxMap = new System.Windows.Forms.PictureBox();
            this.lblMouseDownPoint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_loadImage
            // 
            this.btn_loadImage.Location = new System.Drawing.Point(44, 59);
            this.btn_loadImage.Name = "btn_loadImage";
            this.btn_loadImage.Size = new System.Drawing.Size(120, 23);
            this.btn_loadImage.TabIndex = 0;
            this.btn_loadImage.Text = "Load Sprite Sheet";
            this.btn_loadImage.UseVisualStyleBackColor = true;
            this.btn_loadImage.Click += new System.EventHandler(this.btn_loadImage_Click);
            // 
            // btnCreateNewRoom
            // 
            this.btnCreateNewRoom.Location = new System.Drawing.Point(46, 185);
            this.btnCreateNewRoom.Name = "btnCreateNewRoom";
            this.btnCreateNewRoom.Size = new System.Drawing.Size(126, 23);
            this.btnCreateNewRoom.TabIndex = 1;
            this.btnCreateNewRoom.Text = "Create New Room";
            this.btnCreateNewRoom.UseVisualStyleBackColor = true;
            this.btnCreateNewRoom.Click += new System.EventHandler(this.btnCreateNewRoom_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Num Tiles Wide";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Num Tiles High";
            // 
            // tbNumTilesWide
            // 
            this.tbNumTilesWide.Location = new System.Drawing.Point(131, 112);
            this.tbNumTilesWide.Name = "tbNumTilesWide";
            this.tbNumTilesWide.Size = new System.Drawing.Size(100, 20);
            this.tbNumTilesWide.TabIndex = 4;
            // 
            // tbNumTilesHigh
            // 
            this.tbNumTilesHigh.Location = new System.Drawing.Point(131, 142);
            this.tbNumTilesHigh.Name = "tbNumTilesHigh";
            this.tbNumTilesHigh.Size = new System.Drawing.Size(100, 20);
            this.tbNumTilesHigh.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(12, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblMouseDownPoint);
            this.splitContainer1.Panel1.Controls.Add(this.btn_loadImage);
            this.splitContainer1.Panel1.Controls.Add(this.tbNumTilesHigh);
            this.splitContainer1.Panel1.Controls.Add(this.btnCreateNewRoom);
            this.splitContainer1.Panel1.Controls.Add(this.tbNumTilesWide);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.AutoScrollMargin = new System.Drawing.Size(10, 10);
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(2804, 957);
            this.splitContainer1.SplitterDistance = 307;
            this.splitContainer1.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2493, 0);
            this.panel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.flowLayoutPanel1.Controls.Add(this.pictureBoxMap);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(54, 29);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2354, 894);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // pictureBoxMap
            // 
            this.pictureBoxMap.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pictureBoxMap.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxMap.Name = "pictureBoxMap";
            this.pictureBoxMap.Size = new System.Drawing.Size(2272, 710);
            this.pictureBoxMap.TabIndex = 0;
            this.pictureBoxMap.TabStop = false;
            // 
            // lblMouseDownPoint
            // 
            this.lblMouseDownPoint.AutoSize = true;
            this.lblMouseDownPoint.Location = new System.Drawing.Point(41, 267);
            this.lblMouseDownPoint.Name = "lblMouseDownPoint";
            this.lblMouseDownPoint.Size = new System.Drawing.Size(35, 13);
            this.lblMouseDownPoint.TabIndex = 6;
            this.lblMouseDownPoint.Text = "label3";
            // 
            // RoomBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2892, 994);
            this.Controls.Add(this.splitContainer1);
            this.Name = "RoomBuilder";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_loadImage;
        private System.Windows.Forms.Button btnCreateNewRoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNumTilesWide;
        private System.Windows.Forms.TextBox tbNumTilesHigh;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxMap;
        private System.Windows.Forms.Label lblMouseDownPoint;
    }
}

