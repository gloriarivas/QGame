/*
 * Progammed by: Gloria Rivas-Bonilla 7618325
 * Revision History:
 *      15-Nov-2022:    Begin PlayForm form
 *      19-Nov-2022:    Content loads onto playform
 *      22-Nov-2022:    Fix the load, add click events to boxes so they can be selected
 *                      Also add click events for control buttons (up, down, left, right)
 *      25-Nov-2022:    Completed
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRivasBonillaQGame
{
    /// <summary>
    /// form for user to play previously saved games
    /// </summary>
    public partial class PlayForm : Shared
    {
        private Shared shared = new Shared(); //instance of design form

        //constant variables for picture box placement
        private const int STARTX = 5;
        private const int STARTY = 5;
        private const int SIZE = 50;
        private const int GAP = 1;

        private int remainingBoxes = 0;
        private int numberOfMoves = 0;
        private PictureBox selected = null;
        private int rows = 0;
        private int columns = 0;
        private string boxName;

        public PlayForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// enables controller buttons (left, right, up, down)
        /// </summary>
        private void EnableController()
        {
            btnDown.Enabled = true;
            btnRight.Enabled = true;
            btnLeft.Enabled = true;
            btnUp.Enabled = true;
        }

        /// <summary>
        /// disables controller buttons (left, right, up, down)
        /// </summary>
        private void DisableController()
        {
            btnDown.Enabled = false;
            btnRight.Enabled = false;
            btnLeft.Enabled = false;
            btnUp.Enabled = false;
        }

        /// <summary>
        /// load game from a saved txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpen.Filter = "Text file|*.txt|All files|*.*";

            DialogResult result = dlgOpen.ShowDialog();
            switch (result)
            {
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    string filename = dlgOpen.FileName;
                    LoadContent(filename); //send to method "LoadContent"
                    break;
                case DialogResult.Cancel:
                    break;
                case DialogResult.Abort:
                    break;
                case DialogResult.Retry:
                    break;
                case DialogResult.Ignore:
                    break;
                case DialogResult.Yes:
                    break;
                case DialogResult.No:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// takes loaded file from loadtoolstrip and loads it onto the playform
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadContent(string fileName)
        {
            //first, clear any existing content and reset text boxes
            pnlGameScreen.Controls.Clear();
            remainingBoxes = 0;
            numberOfMoves = 0;
            //list of the loaded txt file strings
            List<string> textLines = new List<string>();
            int listIndex = 4; //starts at 4, since that's the index of the first image
            int goToImage = 3; // since in the txt file it's loaded as row,col,image, we go to image instead of reading the row and col numbers

            //load the file
            using (StreamReader reader = new StreamReader(fileName))
            {
                string textLine;
                while ((textLine = reader.ReadLine()) != null)
                {
                    textLines.Add(textLine);
                }
            }

            //grid dimension for the game
            rows = int.Parse(textLines[0]);
            columns = int.Parse(textLines[1]);

            //initially set these values to the first X and Y available
            int xPlane = STARTX;
            int yPlane = STARTY;
            int count = 0;

            for (int k = 0; k < columns; k++)
            {
                for (int i = 0; i < rows; i++)
                {
                    //starts at 3rd line of txt file
                    //check what number is stored on the line
                    PictureBox box = new PictureBox
                    {
                        //location and size of picture boxes
                        Left = xPlane,
                        Top = yPlane,
                        Height = SIZE,
                        Width = SIZE
                    };
                    //next instance will be height + gap lower than initial box
                    yPlane += SIZE + GAP;

                    if (textLines[listIndex] == saveNone.ToString())
                    {
                        box.Image = null;
                    }
                    else if (textLines[listIndex] == saveWall.ToString())
                    {
                        box.Image = shared.wall;
                        box.Tag = "wall";
                    }
                    else if (textLines[listIndex] == saveRedDoor.ToString())
                    {
                        box.Image = shared.redDoor;
                        box.Tag = "r|door";
                    }
                    else if (textLines[listIndex] == saveGreenDoor.ToString())
                    {
                        box.Image = shared.greenDoor;
                        box.Tag = "g|door";
                    }
                    else if (textLines[listIndex] == saveRedBox.ToString())
                    {
                        box.Image = shared.redBox;
                        box.Click += Move_Box;
                        remainingBoxes++;
                        box.Tag = "r";
                    }
                    else if (textLines[listIndex] == saveGreenBox.ToString())
                    {
                        box.Image = shared.greenBox;
                        box.Click += Move_Box;
                        remainingBoxes++;
                        box.Tag = "g";
                    }

                    //picture is stretched to fit each picture box
                    box.SizeMode = PictureBoxSizeMode.StretchImage;
                    box.Name = count.ToString(); //add a name so I can find the box when using the controller
                    count++;
                    if (box.Image != null)//only adds a control if the box has an image
                    {
                        pnlGameScreen.Controls.Add(box);
                    }
                    listIndex += goToImage;
                }
                //adds the size and gap so the next box is to the right
                xPlane += SIZE + GAP;
                yPlane = STARTY;
            }
            //populate gameforms text fields
            txtRemainingBoxes.Text = remainingBoxes.ToString();
            txtMoves.Text = numberOfMoves.ToString();
            EnableController();//enable the controller once load is complete
        }

        /// <summary>
        /// click event to move selected box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Move_Box(object sender, EventArgs e)
        {
            if (selected != null)//unselects previous selection
            {
                selected.BorderStyle = BorderStyle.None;
            }
            selected = (PictureBox)sender;
            selected.BorderStyle = BorderStyle.Fixed3D;
            boxName = selected.Name;
        }

        /// <summary>
        /// if no box is selected before pressing a controller, then show a message
        /// </summary>
        private void BoxSelection()
        {
            if (selected is null)
            {
                MessageBox.Show("Click to select", "QGame", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// add up all the moves from the gamepad
        /// </summary>
        private void Moves()
        {
            numberOfMoves++;
            txtMoves.Text = numberOfMoves.ToString();
        }

        /// <summary>
        /// button for up click, sends button up, stops when it hits another box with an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            BoxSelection();
            string boxXVal = "";
            string boxYVal = "";
            string closestYValue = "0";
            bool canMove = true;
            bool isDoorOfSameColour = false;
            char doorValue;
            Control moveBox = new Control();

            foreach (Control box in pnlGameScreen.Controls) //find and label the box to be moved
            {
                if (box.Name == boxName)
                {
                    boxXVal = box.Left.ToString();
                    boxYVal = box.Top.ToString();
                    moveBox = box;
                    break;
                }
            }
            for (int i = pnlGameScreen.Controls.Count - 1; i > -1; i--)
            {
                PictureBox boxCheck = (PictureBox)pnlGameScreen.Controls[i];
                if (boxCheck.Left.ToString() == boxXVal && boxCheck.Top.ToString() != boxYVal)
                {
                    

                    if (int.Parse(boxCheck.Top.ToString()) == moveBox.Top - SIZE - GAP)//if there's a box right in front of move box, then stay in same position
                    {
                        //if there's a box right in front of move box but is a door of the same colour, then go into door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                            break;
                        }
                        canMove = false;
                        break;
                    }
                    else if (int.Parse(boxCheck.Top.ToString()) < int.Parse(moveBox.Top.ToString()) && int.Parse(boxCheck.Top.ToString()) > int.Parse(closestYValue))
                    {
                        closestYValue = boxCheck.Top.ToString();
                        //check is moveBox is a door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                        }
                    }
                }
            }
            if (isDoorOfSameColour)//remove box when door is the same colour
            {
                pnlGameScreen.Controls.Remove(moveBox);
                txtRemainingBoxes.Text = (int.Parse(txtRemainingBoxes.Text) - 1).ToString();
            }
            else if (int.Parse(closestYValue) < moveBox.Top && canMove == true)
            {
                moveBox.Top = int.Parse(closestYValue) + SIZE + GAP;
                moveBox.BringToFront();
            }
            Moves(); //update number of moves
            WinGame(); //check for a win
        }

        /// <summary>
        /// button for left click, sends box to the left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, EventArgs e)
        {
            BoxSelection();
            string boxXVal = "";
            string boxYVal = "";
            string closestXValue = "0";
            bool canMove = true;
            bool isDoorOfSameColour = false;
            char doorValue;
            Control moveBox = new Control();

            foreach (Control box in pnlGameScreen.Controls)
            {
                if (box.Name == boxName)
                {
                    boxXVal = box.Left.ToString();
                    boxYVal = box.Top.ToString();
                    moveBox = box;
                    break;
                }
            }
            for (int i = pnlGameScreen.Controls.Count - 1; i > -1; i--)
            {
                PictureBox boxCheck = (PictureBox)pnlGameScreen.Controls[i];
                if (boxCheck.Top.ToString() == boxYVal && boxCheck.Left.ToString() != boxXVal)
                {
                    if (int.Parse(boxCheck.Left.ToString()) == moveBox.Left - SIZE - GAP)
                    {
                        //if there's a box right in front of move box but is a door of the same colour, then go into door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                            break;
                        }
                        canMove = false;
                        break;
                    }
                    else if (int.Parse(boxCheck.Left.ToString()) < int.Parse(moveBox.Left.ToString()) && int.Parse(boxCheck.Left.ToString()) > int.Parse(closestXValue))
                    {
                        closestXValue = boxCheck.Left.ToString();
                        //check is moveBox is a door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                        }
                    }
                }
            }
            //remove box when door is the same colour
            if (isDoorOfSameColour)
            {
                pnlGameScreen.Controls.Remove(moveBox);
                txtRemainingBoxes.Text = (int.Parse(txtRemainingBoxes.Text) - 1).ToString();
            }
            else if (int.Parse(closestXValue) < moveBox.Left && canMove == true)
            {
                moveBox.Left = int.Parse(closestXValue) + SIZE + GAP;
                moveBox.BringToFront();
            }
            Moves();
            WinGame();
        }

        /// <summary>
        /// button for right click, sends box to the right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            BoxSelection();
            string boxXVal = "";
            string boxYVal = "";
            string closestXValue = "1000"; //a number that is bigger than the lowest control box
            bool canMove = true;
            bool isDoorOfSameColour = false;
            char doorValue;
            Control moveBox = new Control();

            foreach (Control box in pnlGameScreen.Controls)
            {
                if (box.Name == boxName)
                {
                    boxXVal = box.Left.ToString();
                    boxYVal = box.Top.ToString();
                    moveBox = box;
                    break;
                }
            }
            for (int i = pnlGameScreen.Controls.Count - 1; i > -1; i--)
            {
                PictureBox boxCheck = (PictureBox)pnlGameScreen.Controls[i];
                if (boxCheck.Top.ToString() == boxYVal && boxCheck.Left.ToString() != boxXVal)
                {
                    if (int.Parse(boxCheck.Left.ToString()) == moveBox.Left + SIZE + GAP)
                    {
                        //if there's a box right in front of move box but is a door of the same colour, then go into door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                            break;
                        }
                        canMove = false;
                        break;
                    }
                    if (int.Parse(boxCheck.Left.ToString()) > int.Parse(moveBox.Left.ToString()) && int.Parse(boxCheck.Left.ToString()) < int.Parse(closestXValue))
                    {
                        closestXValue = boxCheck.Left.ToString();
                        //check is moveBox is a door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                        }
                    }
                }
            }
            //remove box when door is the same colour
            if (isDoorOfSameColour)
            {
                pnlGameScreen.Controls.Remove(moveBox);
                txtRemainingBoxes.Text = (int.Parse(txtRemainingBoxes.Text) - 1).ToString();
            }
            else if (int.Parse(closestXValue) > moveBox.Left && canMove == true)
            {
                moveBox.Left = int.Parse(closestXValue) - SIZE - GAP;
                moveBox.BringToFront();
            }
            Moves();
            WinGame();
        }

        /// <summary>
        /// button for down click, sends box down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            BoxSelection();
            string boxXVal = "";
            string boxYVal = "";
            string closestYValue = "1000"; //a number that is bigger than the lowest control box
            bool canMove = true;
            bool isDoorOfSameColour = false;
            char doorValue;
            Control moveBox = new Control();

            foreach (Control box in pnlGameScreen.Controls)
            {
                if (box.Name == boxName)
                {
                    boxXVal = box.Left.ToString();
                    boxYVal = box.Top.ToString();
                    moveBox = box;
                    break;
                }
            }
            for (int i = pnlGameScreen.Controls.Count - 1; i > -1; i--)
            {
                PictureBox boxCheck = (PictureBox)pnlGameScreen.Controls[i];
                if (boxCheck.Left.ToString() == boxXVal && boxCheck.Top.ToString() != boxYVal)
                {
                    if (int.Parse(boxCheck.Top.ToString()) == moveBox.Top + SIZE + GAP)
                    {
                        //if there's a box right in front of move box but is a door of the same colour, then go into door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                            break;
                        }
                        canMove = false;
                        break;
                    }
                    if (int.Parse(boxCheck.Top.ToString()) > int.Parse(moveBox.Top.ToString()) && int.Parse(boxCheck.Top.ToString()) < int.Parse(closestYValue))
                    {
                        closestYValue = boxCheck.Top.ToString();
                        //check is moveBox is a door
                        if (boxCheck.Tag.ToString() == "r|door" || boxCheck.Tag.ToString() == "g|door")
                        {
                            doorValue = boxCheck.Tag.ToString()[0];
                            //check if moveBox is the same colour as door
                            if (moveBox.Tag.ToString() == doorValue.ToString())
                            {
                                isDoorOfSameColour = true;
                            }
                        }
                    }
                }
            }
            //remove box when door is the same colour
            if (isDoorOfSameColour)
            {
                pnlGameScreen.Controls.Remove(moveBox);
                txtRemainingBoxes.Text = (int.Parse(txtRemainingBoxes.Text) - 1).ToString();
            }
            else if (int.Parse(closestYValue) > moveBox.Top && canMove == true)
            {
                moveBox.Top = int.Parse(closestYValue) - SIZE - GAP;
                moveBox.BringToFront();
            }
            Moves();
            WinGame();
        }

        /// <summary>
        /// when game ends, clear the board and reset page (diable controller, clear text boxes)
        /// </summary>
        private void WinGame()
        {
            if (txtRemainingBoxes.Text == "0")
            {
                MessageBox.Show("Congratulations \n Game End", "QGame", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                pnlGameScreen.Controls.Clear();
                txtMoves.Text = "0";
                txtRemainingBoxes.Text = "0";
                DisableController();
            }
        }
    }
}
