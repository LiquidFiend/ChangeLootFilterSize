using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeLootFilterSize
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.AllowDrop = true;
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;
        }

        void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                e.Effect = DragDropEffects.Copy;
        }

        void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0 && Path.GetExtension(files[0]) == "filter")
                SetFileName(files[0]);
        }

       

        private const int MINSIZE = 18;
        private const int MAXSIZE = 45;

        private void btnCreateNewFile_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Filter doesn't exist");
                return;
            }

            if (String.IsNullOrEmpty(txtNewFilterName.Text) || txtNewFilterName.Text == Path.GetFileNameWithoutExtension(txtFilePath.Text))
            {
                MessageBox.Show("File must have a new name, and cannot be blank");
                return;
            }

            string file = txtFilePath.Text;
            string path = Path.GetDirectoryName(file);

            StringBuilder modifiedFilter = new StringBuilder();

            using (StreamReader read = new StreamReader(file))
            {
                while (!read.EndOfStream)
                {

                    string line = read.ReadLine();
                    string trimmedLine = line.Trim();

                    if (trimmedLine.StartsWith("SetFontSize"))
                    {
                        int fontSize = Convert.ToInt16(trimmedLine.Substring(trimmedLine.Length - 2));
                        int modifiedSize = fontSize;
                        modifiedSize += (int)numericUpDown1.Value;

                        modifiedSize = modifiedSize > MAXSIZE ? MAXSIZE : modifiedSize;
                        modifiedSize = modifiedSize < MINSIZE ? MINSIZE : modifiedSize;

                        line = line.Replace(fontSize.ToString(), modifiedSize.ToString());
                    }

                    modifiedFilter.AppendLine(line);
                }

                read.Close();
            }


            using (StreamWriter writer = new StreamWriter(Path.Combine(path, txtNewFilterName.Text + ".filter")))
            {
                writer.Write(modifiedFilter.ToString());
                writer.Close();
            }
        }

        private void btnPickFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "POE Filters (*.filter)|*.filter";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SetFileName(dialog.FileName);
        }

        private void SetFileName(string fileName)
        {
            txtFilePath.Text = fileName;
            txtNewFilterName.Text = Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
