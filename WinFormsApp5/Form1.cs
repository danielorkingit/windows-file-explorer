namespace WinFormsApp5
{
    public partial class Form1 : Form
    {
        SplashScreen splashScreen = new SplashScreen();

        DriveInfo[] drives = DriveInfo.GetDrives();
        DirectoryInfo pathInfo;
        DirectoryInfo currentFolder;


        TreeNode filesN;
        TreeNode dirN;
        TreeNode currentNode;
        TreeNode tn;

        static ImageList _imageList;

        string path;

        // IMAGELIST
        public static ImageList ImageList
        {
            get
            {
                if (_imageList == null)
                {
                    _imageList = new ImageList();
                    _imageList.Images.Add("folder", Image.FromFile(@"...THE FULL PATH TO THE ICONS FOLDER WinFormsApp5\WinFormsApp5\icons\folder.ico"));
                    _imageList.Images.Add("file", Image.FromFile(@"...THE FULL PATH TO THE ICONS FOLDER WinFormsApp5\WinFormsApp5\icons\folder.ico"));
                }
                return _imageList;
            }
        }

        // FORM 1 ROOT

        public Form1()
        {
            InitializeComponent();

            splashScreen.Show();

            // ADD IMAGE LIST

            treeView1.ImageList = ImageList;

            listView1.SmallImageList = ImageList;

            // ADD ROOT NODE -> DEFAULT: MAIN (C:) DRIVE

            tn = treeView1.Nodes.Add(drives[0].Name);

            currentFolder = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)); // DEFAULT: CURRENT USER (OPTIONAL: CHANGE TO DISK DIRECTORY)

            textBox1.Text = currentFolder.FullName;

            catchData(currentFolder, tn); // LOAD ALL FOLDERS (OPTIONAL: CHANGE TO LOAD TREENODE BY EXPANDING)

            this.Show(); // SHOW MAIN SCREEN
            splashScreen.Hide(); // HIDE SPLASHSCREEN
        }


        public void catchData(DirectoryInfo currentF, TreeNode tn)
        {
            try
            {
                Application.DoEvents(); // OPTIONAL: DISABLE SPLASHSCREEN -> DRAWS WINDOW WHILE PROCESSING
                FileInfo[] fileArray = currentF.GetFiles();
                foreach (FileInfo fileInfo in fileArray)
                {
                    filesN = new TreeNode(fileInfo.Name);
                    filesN.Name = fileInfo.FullName;
                    filesN.ImageKey = "file";
                    tn.Nodes.Add(filesN);
                }
                try
                {
                    DirectoryInfo[] dirArray = currentF.GetDirectories();
                    foreach (DirectoryInfo directoryInfo in dirArray)
                    {
                        dirN = new TreeNode(directoryInfo.Name);
                        dirN.Name = directoryInfo.FullName;
                        dirN.ImageKey = "folder";
                        tn.Nodes.Add(dirN);

                        catchData(directoryInfo, dirN); // RECURSIVE FUNCTIONING
                    }
                }
                catch { }
            }
            catch { }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                listView1.Clear();

                listView1.BeginUpdate();

                currentNode = e.Node;
                if (currentNode.Nodes.Count > 0) // CREATE LISTVIEW ITEMS
                {
                    foreach (TreeNode subN in currentNode.Nodes)
                    {
                        ListViewItem item = listView1.Items.Add(subN.Text, subN.ImageKey);
                        item.Name = subN.Name;
                    }
                }

                listView1.EndUpdate();
            }
            catch { }


        }

        // OPEN TEXT-FILES IN RICHBOX

        private void treeView1_NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            path = e.Node.Name;
            pathInfo = new DirectoryInfo(path);
            if (path != null && path != "")
            {
                if (File.Exists(pathInfo.FullName))
                {
                    richTextBox1.Show();
                    button3.Show();
                    string[] lines = System.IO.File.ReadAllLines(pathInfo.FullName);
                    richTextBox1.Lines = lines;
                    button3.Show();
                }

                else
                {
                    MessageBox.Show("File not found.");
                }
            }
        }

        // ENABLES ROUTING THROUGH LISTVIEW

        private void buildNewList(string path)
        {
            try
            {
                listView1.Clear();

                pathInfo = new DirectoryInfo(path);

                foreach (FileInfo fileInfo in pathInfo.GetFiles())
                {
                    ListViewItem fileLV = new ListViewItem(fileInfo.Name);
                    fileLV.ImageKey = "file";
                    listView1.Items.Add(fileLV.Text);
                }
                try
                {
                    foreach (DirectoryInfo directoryInfo in pathInfo.GetDirectories())
                    {
                        ListViewItem directoryListView = new ListViewItem(directoryInfo.Name);
                        directoryListView.ImageKey = "folder";
                        listView1.Items.Add(directoryListView.Text);
                    }
                }
                catch { }
            }
            catch { }
        }

        // OPEN TEXT-FILES IN RICHBOX FROM LISTVIEW

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            path = listView1.SelectedItems[0].Name;

            if (path != null && path != "")
            {
                if (listView1.FocusedItem.ImageKey == "folder")
                {
                    buildNewList(path);
                    listView1.SelectedItems.Clear();
                }
                else if (listView1.FocusedItem.ImageKey == "file")
                {
                    pathInfo = new DirectoryInfo(path);
                    if (File.Exists(pathInfo.FullName))
                    {
                        richTextBox1.Show();
                        button3.Show();
                        string[] lines = System.IO.File.ReadAllLines(textBox1.Text);
                        richTextBox1.Lines = lines;
                        button3.Show();
                    }

                    else
                    {
                        MessageBox.Show("File not found.");
                    }
                }
            }

        }

        // UPDATE TEXTBOX

        private void updateText(TreeNodeMouseHoverEventArgs e)
        {
            string nodePath = e.Node.Name;
            textBox1.Text = nodePath;
        }

        private void updateTextList(ListViewItemMouseHoverEventArgs e)
        {
            string nodePath = e.Item.Name;
            textBox1.Text = nodePath;
        }

        // MOUSEHOVER -> UPDATE PATH

        private void treeView1_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            updateText(e);
        }

        private void listView1_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            updateTextList(e);
        }

        // OPEN CONTEXT MENUE

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = listView1.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        // DELETE PECIFIC FILE

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            path = listView1.FocusedItem.Name;

            if (path != null && path != "")
            {
                File.Delete(path);

                DirectoryInfo pathI = new DirectoryInfo(currentNode.Name);

                listView1.BeginUpdate();

                listView1.FocusedItem.Remove();

                listView1.EndUpdate();

                refresh(pathI, currentNode); // REFRESH CURRENT NODE ONLY 
            }
        }

        // REFRESH ONLY CURRENT NODE

        public void refresh(DirectoryInfo currentF, TreeNode tn)
        {
            try
            {
                currentNode.Nodes.Clear();
                FileInfo[] fileArray = currentF.GetFiles();
                foreach (FileInfo fileInfo in fileArray)
                {
                    filesN = new TreeNode(fileInfo.Name);
                    filesN.Name = fileInfo.FullName;
                    filesN.ImageKey = "file";
                    tn.Nodes.Add(filesN);
                }
                try
                {
                    DirectoryInfo[] dirArray = currentF.GetDirectories();
                    foreach (DirectoryInfo directoryInfo in dirArray)
                    {
                        dirN = new TreeNode(directoryInfo.Name);
                        dirN.Name = directoryInfo.FullName;
                        dirN.ImageKey = "folder";
                        tn.Nodes.Add(dirN);

                        refresh(directoryInfo, dirN);
                    }
                }
                catch { }
            }
            catch { }
        }

        // REFRESH-BUTTON

        private void button1_Click(object sender, EventArgs e)
        {
            path = currentNode.Name;

            if (path != null && path != "")
            {
                DirectoryInfo pathI = new DirectoryInfo(path);
                refresh(pathI, currentNode); // ONLY REFRESH CURRENT NODE
                buildNewList(path);
            }
        }

        // SEARCH BUTTON

        private void button2_Click(object sender, EventArgs e)
        {
            string listPath = textBox1.Text;
            buildNewList(listPath);
        }

        // CLOSE RICHBOX

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Hide();
            button3.Hide();
        }

        // SAVE .TXT FILE

        private void button4_Click(object sender, EventArgs e)
        {

            File.WriteAllText(currentNode.Name + "\\test.txt", richTextBox1.Text); // CHANGE FILENAME OR MAKE A INPUT DIALOGUE

            richTextBox1.Hide();
            button4.Hide();
            button3.Hide();
        }

        // ADD A .TXT FILE

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Show();
            button3.Show();
            button4.Show();
            richTextBox1.Text = "input text.";
        }
    }
}
