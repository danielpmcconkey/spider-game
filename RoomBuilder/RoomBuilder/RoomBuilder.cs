﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoomBuilder
{
    public enum EditMode { TILE, DOOR }
    public partial class RoomBuilder : Form
    {
        BuilderHelper builderHelper;
        private bool _isMouseDown = false;
        private int _lastTileFlipped = -1;
        private bool _hasUnsavedChanges;
        private EditMode _editMode;
        

        public RoomBuilder()
        {
            InitializeComponent();
            builderHelper = new BuilderHelper();
            //this.Paint += this.OnPaint;
            
            pictureBoxMap.Paint += new PaintEventHandler(PictureBox_Paint);
            LoadSpriteSheet();

            pictureBoxMap.MouseDown += new MouseEventHandler(pictureBoxMap_MouseDown);
            pictureBoxMap.MouseUp += new MouseEventHandler(pictureBoxMap_MouseUp);
            pictureBoxMap.MouseMove += new MouseEventHandler(pictureBoxMap_MouseMove);

            _editMode = EditMode.TILE;
            _hasUnsavedChanges = false;
        }

        
        private void LoadSpriteSheet()
        {
            string pathToSpriteSheet = @"E:\Unity Projects\SpiderPocGit\Assets\Sprites\FloorAndWallTiless\WorldBuilderSet\WorldBuilderTilemap48x48.png";
            string pathToDoorImage = @"E:\Unity Projects\SpiderPocGit\Assets\Sprites\FloorAndWallTiless\WorldBuilderSet\WorldBuilderDoor.png";
            builderHelper.LoadSpriteSheetImage(pathToSpriteSheet);
            builderHelper.LoadDoorImage(pathToDoorImage);
        }
        private void ResizeUi()
        {
            int picBoxWidth = (builderHelper.roomWidth + 2) * builderHelper.tileWidth;
            int picBoxHeight = (builderHelper.roomHeight + 2) * builderHelper.tileHeight;
            flowLayoutPanel1.Size = new Size(picBoxWidth + 25, picBoxHeight + 25);
            pictureBoxMap.Size = new Size(picBoxWidth, picBoxHeight);
        }
        
        #region event handlers
        private void btnAddDoor_Click(object sender, EventArgs e)
        {
            builderHelper.AddDoor(int.Parse(tbAddDoorRow.Text), int.Parse(tbAddColumn.Text));
            pictureBoxMap.Refresh();
        }
        private void btnCreateNewRoom_Click(object sender, EventArgs e)
        {
            int width = int.Parse(tbNumTilesWide.Text);
            int height = int.Parse(tbNumTilesHigh.Text);
            builderHelper.SetRoomDimensions(width, height);
            ResizeUi();
            pictureBoxMap.Refresh();

            _hasUnsavedChanges = true;
        }
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"E:\Unity Projects\SpiderPocGit\Assets\RoomTemplates";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "text files (*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string json = File.ReadAllText(openFileDialog1.FileName);
                builderHelper.DeSerializeRoom(json);
                tbNumTilesHigh.Text = builderHelper.roomHeight.ToString();
                tbNumTilesWide.Text = builderHelper.roomWidth.ToString();
                tbRoomName.Text = builderHelper.roomName;
                ResizeUi();
                pictureBoxMap.Refresh();
            }
            _hasUnsavedChanges = false;
        }
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            string json = builderHelper.SerializeRoom();

            saveFileDialog1.Filter = "text files (*.txt)|*.txt";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, json);
            }
            _hasUnsavedChanges = false;
        }
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            builderHelper.DrawRoom(e, _editMode);
        }
        private void pictureBoxMap_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _isMouseDown = true;

            Point mouseDownLocation = new Point(e.X, e.Y);
            int tileNum = builderHelper.GetTileNumbFromMousePosition(mouseDownLocation);
            builderHelper.ReverseTile(tileNum);
            _lastTileFlipped = tileNum;
            pictureBoxMap.Refresh();
            _hasUnsavedChanges = true;
        }
        private void pictureBoxMap_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                Point mouseLocation = new Point(e.X, e.Y);
                int tileNum = builderHelper.GetTileNumbFromMousePosition(mouseLocation);
                if (tileNum != _lastTileFlipped)
                {
                    builderHelper.ReverseTile(tileNum);
                    _lastTileFlipped = tileNum;
                    pictureBoxMap.Refresh();
                    _hasUnsavedChanges = true;
                }
            }
        }
        private void pictureBoxMap_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _isMouseDown = false;
            _lastTileFlipped = -1;
        }
        private void tbRoomName_TextChanged(object sender, EventArgs e)
        {
            builderHelper.roomName = tbRoomName.Text;
            _hasUnsavedChanges = true;
        }
        #endregion
    }
}
