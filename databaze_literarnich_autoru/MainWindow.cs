using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FinalProject.DataClasses;

namespace FinalProject
{
    public partial class MainWindow : Form
    {
        private List<DataClasses.Author> authorsList;
        private UserRole userRole;
        public List<DataClasses.Author> AuthorsList
        {
            get { return authorsList; }
            private set { authorsList = value; }
        }
        public bool EditsMade { get; private set; }
        public string? OutputJsonPath { get; private set; } = null;
        internal MainWindow(List<DataClasses.Author> loadedAuthors, UserRole userRole, DataFetcher fetcher)
        {
            InitializeComponent();
            this.userRole = userRole;
            SetPermissions();
            comboBox2.Items.Clear();
            loadedAuthors.ForEach(x => x.SetAuthorAttributeForBooks());
            authorsList = loadedAuthors;
            comboBox2.Items.AddRange(authorsList.ToArray());
        }

        private void SetPermissions()
        {
            buttonEdit.Enabled = userRole.HasEditPermissions;
            buttonDelete.Enabled = userRole.HasDeletePermissions;
            buttonNew.Enabled = userRole.HasCreatePermissions;
        }

        private void UpdateEverything()
        {
            var authorComboBox = comboBox2;
            var authorRichText = richTextBox1;
            var authorNameLabel = label1;
            var authorDatesLabel = label2;
            var bookComboBox = comboBox1;
            var bookRichText = richTextBox2;
            bookComboBox.Items.Clear();
            if (authorComboBox.SelectedItem == null)
            {
                authorRichText.Text = "";
                authorNameLabel.Text = "";
                authorDatesLabel.Text = "";
            }
            else
            {
                Author selected = (Author)authorComboBox.SelectedItem;
                authorRichText.Text = selected.Description;
                authorNameLabel.Text = selected.FullName;
                authorDatesLabel.Text = selected.FormattedDates;
                bookComboBox.Items.AddRange(selected.Books.ToArray());
            }
            UpdateBookElements();
        }

        private void UpdateBookElements()
        {
            var bookComboBox = comboBox1;
            var bookRichText = richTextBox2;
            if(bookComboBox.Items.Count == 0 || bookComboBox.SelectedItem == null)
            {
                if(bookComboBox.Items.Count == 0)
                {
                    bookComboBox.Text = "";
                }
                bookRichText.Text = "";
                return;
            }
            bookRichText.Text = ((Book)bookComboBox.SelectedItem).Description;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEverything();
            comboBox1.Text = "";
        }

        private void comboBox2_TextUpdate(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateBookElements();
        }

        private void button2_Click(object sender, EventArgs e)
        {   
            if(comboBox2.SelectedItem == null || comboBox2.Text == "")
            {
                return;
            }
            saveFileHtml.ShowDialog();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if(comboBox2.SelectedItem == null)
            {
                return;
            }
            var result = MessageBox.Show("For real?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if(result == DialogResult.No)
            {
                return;
            }
            EditsMade = true;
            var cmbBoxItems = comboBox2.Items;
            comboBox2.Items.Remove(comboBox2.SelectedItem);
            comboBox2.Text = "";
            UpdateEverything();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if(comboBox2.SelectedItem == null)
            {
                return;
            }
            var editWindow = new AddAuthor((Author)comboBox2.SelectedItem);
            editWindow.ShowDialog();
            if(editWindow.SavePressed)
            {
                EditsMade = true;
                UpdateEverything();
            }
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            var addWindow = new AddAuthor();
            addWindow.ShowDialog();
            if (!addWindow.SavePressed)
            {
                return;
            }
            EditsMade = true;
            comboBox2.Items.Add(addWindow.Author);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!EditsMade)
            {
                return;
            }
            this.AuthorsList = new List<Author>();
            foreach(object author in comboBox2.Items)
            {
                AuthorsList.Add((Author)author);
            }
            var result = MessageBox.Show("Soubor byl upraven. Uložit změny?", "Neuložené změny", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if(result == DialogResult.Yes)
            {
                saveFileJson.ShowDialog();
            }
        }

        private void saveFileJson_FileOk(object sender, CancelEventArgs e)
        {
            OutputJsonPath = ((SaveFileDialog)sender).FileName;
        }

        private async void saveFileHtml_FileOk(object sender, CancelEventArgs e)
        {
            var author = (Author)comboBox2.SelectedItem;
            var export = new Exporters.HtmlExporter(saveFileHtml.FileName).ExportAuthorAsync(author);
            await export;
            await Task.Delay(3000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var searchWindow = new Search(comboBox2.Items.Cast<Author>().ToList());
            searchWindow.ShowDialog();
            if (searchWindow.Selected && searchWindow.SelectedAuthor != null)
            {
                comboBox2.SelectedItem = searchWindow.SelectedAuthor;
                UpdateEverything();
            }
        }
    }
}
