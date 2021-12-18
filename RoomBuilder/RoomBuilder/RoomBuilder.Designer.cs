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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoomBuilder));
            this.btnCreateNewRoom = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNumTilesWide = new System.Windows.Forms.TextBox();
            this.tbNumTilesHigh = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tbAddColumn = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbAddDoorRow = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAddDoor = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.tbRoomName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBoxMap = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnTileEditMode = new System.Windows.Forms.Button();
            this.btnDoorEditMode = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCreateNewRoom
            // 
            this.btnCreateNewRoom.Location = new System.Drawing.Point(9, 193);
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
            this.label1.Location = new System.Drawing.Point(6, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Num Tiles Wide";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 153);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Num Tiles High";
            // 
            // tbNumTilesWide
            // 
            this.tbNumTilesWide.Location = new System.Drawing.Point(94, 120);
            this.tbNumTilesWide.Name = "tbNumTilesWide";
            this.tbNumTilesWide.Size = new System.Drawing.Size(100, 20);
            this.tbNumTilesWide.TabIndex = 4;
            // 
            // tbNumTilesHigh
            // 
            this.tbNumTilesHigh.Location = new System.Drawing.Point(94, 150);
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
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.tbAddColumn);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.tbAddDoorRow);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.btnAddDoor);
            this.splitContainer1.Panel1.Controls.Add(this.btnLoadFile);
            this.splitContainer1.Panel1.Controls.Add(this.btnSaveFile);
            this.splitContainer1.Panel1.Controls.Add(this.tbRoomName);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
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
            // tbAddColumn
            // 
            this.tbAddColumn.Location = new System.Drawing.Point(112, 646);
            this.tbAddColumn.Name = "tbAddColumn";
            this.tbAddColumn.Size = new System.Drawing.Size(100, 20);
            this.tbAddColumn.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(114, 616);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "column";
            // 
            // tbAddDoorRow
            // 
            this.tbAddDoorRow.Location = new System.Drawing.Point(6, 646);
            this.tbAddDoorRow.Name = "tbAddDoorRow";
            this.tbAddDoorRow.Size = new System.Drawing.Size(100, 20);
            this.tbAddDoorRow.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 617);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "row";
            // 
            // btnAddDoor
            // 
            this.btnAddDoor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAddDoor.BackgroundImage")));
            this.btnAddDoor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAddDoor.Location = new System.Drawing.Point(17, 493);
            this.btnAddDoor.Name = "btnAddDoor";
            this.btnAddDoor.Size = new System.Drawing.Size(50, 50);
            this.btnAddDoor.TabIndex = 11;
            this.btnAddDoor.UseVisualStyleBackColor = true;
            this.btnAddDoor.Click += new System.EventHandler(this.btnAddDoor_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLoadFile.BackgroundImage")));
            this.btnLoadFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLoadFile.Location = new System.Drawing.Point(62, 313);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(50, 50);
            this.btnLoadFile.TabIndex = 10;
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSaveFile.BackgroundImage")));
            this.btnSaveFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSaveFile.Location = new System.Drawing.Point(6, 313);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(50, 50);
            this.btnSaveFile.TabIndex = 9;
            this.btnSaveFile.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.TextChanged += new System.EventHandler(this.tbRoomName_TextChanged);
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // tbRoomName
            // 
            this.tbRoomName.Location = new System.Drawing.Point(3, 251);
            this.tbRoomName.Name = "tbRoomName";
            this.tbRoomName.Size = new System.Drawing.Size(282, 20);
            this.tbRoomName.TabIndex = 8;
            this.tbRoomName.TextChanged += new System.EventHandler(this.tbRoomName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 235);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Room name";
            this.label3.TextChanged += new System.EventHandler(this.tbRoomName_TextChanged);
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
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnTileEditMode
            // 
            this.btnTileEditMode.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnTileEditMode.Location = new System.Drawing.Point(14, 19);
            this.btnTileEditMode.Name = "btnTileEditMode";
            this.btnTileEditMode.Size = new System.Drawing.Size(75, 43);
            this.btnTileEditMode.TabIndex = 16;
            this.btnTileEditMode.Text = "Tile Edit Mode";
            this.btnTileEditMode.UseVisualStyleBackColor = false;
            // 
            // btnDoorEditMode
            // 
            this.btnDoorEditMode.Location = new System.Drawing.Point(95, 19);
            this.btnDoorEditMode.Name = "btnDoorEditMode";
            this.btnDoorEditMode.Size = new System.Drawing.Size(75, 43);
            this.btnDoorEditMode.TabIndex = 17;
            this.btnDoorEditMode.Text = "Door Edit Mode";
            this.btnDoorEditMode.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox1.Controls.Add(this.btnTileEditMode);
            this.groupBox1.Controls.Add(this.btnDoorEditMode);
            this.groupBox1.Location = new System.Drawing.Point(9, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 84);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Mode";
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
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCreateNewRoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNumTilesWide;
        private System.Windows.Forms.TextBox tbNumTilesHigh;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxMap;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.TextBox tbRoomName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tbAddDoorRow;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAddDoor;
        private System.Windows.Forms.TextBox tbAddColumn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTileEditMode;
        private System.Windows.Forms.Button btnDoorEditMode;
    }
}

