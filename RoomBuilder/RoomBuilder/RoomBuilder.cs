using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoomBuilder
{
    public partial class RoomBuilder : Form
    {
        BuilderHelper builderHelper;
        private bool _isMouseDown = false;
        private int _lastTileFlipped = -1;
        

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
        }

        private void btn_loadImage_Click(object sender, EventArgs e)
        {

            
        }
        private void LoadSpriteSheet()
        {
            string path = @"E:\Unity Projects\SpiderPocGit\Assets\Sprites\FloorAndWallTiless\WorldBuilderSet\WorldBuilderTilemap48x48.png";
            
            builderHelper.LoadImage(path);
        }
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            //if (builderHelper.image == null) LoadSpriteSheet();

            builderHelper.DrawRoom(e);
            

        }

        private void btnCreateNewRoom_Click(object sender, EventArgs e)
        {
            int width = int.Parse(tbNumTilesWide.Text);
            int height = int.Parse(tbNumTilesHigh.Text);
            builderHelper.SetRoomDimensions(width, height);
            int picBoxWidth = width * builderHelper.tileWidth;
            int picBoxHeight = height * builderHelper.tileHeight;
            flowLayoutPanel1.Size = new Size(picBoxWidth + 25, picBoxHeight + 25);
            pictureBoxMap.Size = new Size(picBoxWidth, picBoxHeight);
            pictureBoxMap.Refresh();
        }
        private void pictureBoxMap_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _isMouseDown = true;

            Point mouseDownLocation = new Point(e.X, e.Y);
            int tileNum = builderHelper.GetTileNumbFromMousePosition(mouseDownLocation);
            lblMouseDownPoint.Text = string.Format("tile num: {0}", tileNum);
            builderHelper.ReverseTile(tileNum);
            _lastTileFlipped = tileNum;
            pictureBoxMap.Refresh();
        }
        private void pictureBoxMap_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _isMouseDown = false;
            _lastTileFlipped = -1;
        }
        private void pictureBoxMap_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(_isMouseDown)
            {
                Point mouseLocation = new Point(e.X, e.Y);
                int tileNum = builderHelper.GetTileNumbFromMousePosition(mouseLocation);
                if(tileNum != _lastTileFlipped)
                {
                    builderHelper.ReverseTile(tileNum);
                    _lastTileFlipped = tileNum;
                    pictureBoxMap.Refresh();
                }
            }
        }
    }
}
