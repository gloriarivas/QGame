/*
 * Progammed by: Gloria Rivas-Bonilla 7618325
 * Revision History:
 *      16-Oct-2022:    Started program, control panel buttons completed
 *                      DesignForm design completed
 *      17-Oct-2022:    Completed "generate" button
 *      23-Oct-2022:    selected picture box in tool box can add that image to the generated grid
 *                      Save dialog works, still need to implement what will be saved
 *      25-Oct-2022:    Completed
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRivasBonillaQGame
{
    /// <summary>
    /// design form, here the user can create a new game and save it as a text file
    /// </summary>
    public partial class DesignForm : Shared
    {
        private Shared shared = new Shared();

        //ints to take input from user
        public int columns = 0;
        public int rows = 0;
        public string fileToSave;

        //list to store picture box when generated
        List<PictureBox> boxOrder = new List<PictureBox>();
        //variables to keep track of how many doors, boxes, and walls there are in the save
        int numberOfWalls = 0;
        int numberOfDoors = 0;
        int numberOfBoxes = 0;

        //constant variables for picture box placement
        public const int STARTX = 220;
        public const int STARTY = 80;
        public const int SIZE = 50;
        public const int GAP = 2;


        public DesignForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// generates a grid of blank picture boxes depending on user input within pnlGameBoard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //error handling, checking if user input is int
            int checkParse;

            if (int.TryParse(txtRowAmount.Text, out checkParse) == false || int.TryParse(txtColumnAmount.Text, out checkParse) == false)
            {
                MessageBox.Show("Please provide valid date for rows and columns: Both must be integers", "QGame");
            }
            else
            {
                rows = int.Parse(txtRowAmount.Text);
                columns = int.Parse(txtColumnAmount.Text);

                int xPlane = STARTX;
                int yPlane = STARTY;
                //adding rows
                for (int k = 0; k < columns; k++)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        PictureBox box = new PictureBox();
                        box.Image = null;
                        //location of picture boxes
                        box.Left = xPlane;
                        box.Top = yPlane;
                        //size of picture box
                        box.Height = SIZE;
                        box.Width = SIZE;
                        //next instance will be height + gap lower than initial box
                        yPlane += SIZE + GAP;
                        box.BorderStyle = BorderStyle.FixedSingle;
                        this.Controls.Add(box);
                        //add event handler
                        box.Click += Click_Box;
                        //picture is stretched to fit each picture box
                        box.SizeMode = PictureBoxSizeMode.StretchImage;
                        boxOrder.Add(box);
                    }
                    xPlane += SIZE + GAP;
                    yPlane = STARTY;
                }
            }
        }

        /// <summary>
        /// all click events from the generated picture boxes will be directed to this method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Box(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            pictureBox.Image = shared.toolboxItem;
        }

        /// <summary>
        /// selects an item from the toolbox, that item will be used to place an image in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selected_Item(object sender, EventArgs e)
        {
            PictureBox selected = (PictureBox)sender;

            if (selected.Image == pboxWall.Image)
            {
                shared.toolboxItem = shared.wall;
            }
            else if (selected.Image == pBoxRedDoor.Image)
            {
                shared.toolboxItem = shared.redDoor;
            }
            else if (selected.Image == pboxGreenDoor.Image)
            {
                shared.toolboxItem = shared.greenDoor;
            }
            else if (selected.Image == pboxRedBox.Image)
            {
                shared.toolboxItem = shared.redBox;
            }
            else if (selected.Image == pboxGreenBox.Image)
            {
                shared.toolboxItem = shared.greenBox;
            }
            else
            {
                shared.toolboxItem = null;
            }
        }

        /// <summary>
        /// closes design form, control panel and all other instances stay open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// creates format that will be used when saving to a text file
        /// </summary>
        private void savePattern()
        {
            //add rows and columns to file
            fileToSave = $"{rows}\n{columns}\n";
            //check list content by the index
            int listIndex = 0;
            //add each picture boxes row/column and content
            for (int k = 0; k < rows; k++)
            {
                for (int i = 0; i < columns; i++)
                {
                    //content
                    fileToSave += $"{k}\n{i}\n";
                    if (boxOrder[listIndex].Image == null)
                    {
                        fileToSave += $"{saveNone}\n";
                    }
                    else if (boxOrder[listIndex].Image == shared.wall)
                    {
                        numberOfWalls++;
                        fileToSave += $"{saveWall}\n";
                    }
                    else if (boxOrder[listIndex].Image == shared.greenDoor || boxOrder[listIndex].Image == shared.redDoor)
                    {
                        numberOfDoors++;
                        if (boxOrder[listIndex].Image == shared.greenDoor)
                        {
                            fileToSave += $"{saveGreenDoor}\n";
                        }
                        else
                        {
                            fileToSave += $"{saveRedDoor}\n";
                        }
                    }
                    else if (boxOrder[listIndex].Image == shared.greenBox || boxOrder[listIndex].Image == shared.redBox)
                    {
                        numberOfBoxes++;
                        if (boxOrder[listIndex].Image == shared.greenBox)
                        {
                            fileToSave += $"{saveGreenBox}\n";
                        }
                        else
                        {
                            fileToSave += $"{saveRedBox}\n";
                        }
                    }
                    listIndex++;
                }
            }
        }

        /// <summary>
        /// saves design as a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = dlgSave.ShowDialog();
            switch (result)
            {
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    try
                    {
                        //count how many of each item is on the generated pboxes
                        string fileName = dlgSave.FileName;
                        StreamWriter write = new StreamWriter(fileName);
                        savePattern();
                        write.WriteLine(fileToSave);
                        write.Close();
                        MessageBox.Show($"File Saved Successfully:\nTotal number of walls: {numberOfWalls}\nTotal number of doors: {numberOfDoors}\nTotal number of boxes: {numberOfBoxes}", "QGame");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: could not save file" + ex.Message);
                    }
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
    }
}
