using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRivasBonillaQGame
{
    /// <summary>
    /// class to share images, so only need to be loaded once
    /// </summary>
    public class Shared : Form
    {
        //selected toolbox item
        public Image toolboxItem = null;

        //load all images into form initially
        //made public so PlayForm and DesignForm can use
        public Image greenBox = GRivasBonillaQGame.Properties.Resources.greenBox;
        public Image greenDoor = GRivasBonillaQGame.Properties.Resources.greenDoor;
        public Image wall = GRivasBonillaQGame.Properties.Resources.wall;
        public Image redBox = GRivasBonillaQGame.Properties.Resources.redBox;
        public Image redDoor = GRivasBonillaQGame.Properties.Resources.redDoor;
        
        //assigning number to each type of item from the toolbox
        public int saveNone = 0;
        public int saveWall = 1;
        public int saveRedDoor = 2;
        public int saveGreenDoor = 3;
        public int saveRedBox = 4;
        public int saveGreenBox = 5;
    }
}
