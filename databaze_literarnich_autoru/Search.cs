using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class Search : Form
    {
        private List<DataClasses.Author> authors;
        public DataClasses.Author? SelectedAuthor { get; private set; } = null;
        public bool Selected { get; private set; } = false;
        public Search(List<DataClasses.Author> authors)
        {
            InitializeComponent();
            this.authors = authors;
            this.authors.Sort((a, b) => a.LastName.CompareTo(b.LastName));
            comboBox1.Items.AddRange(this.authors.ToArray());
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            var selStart = comboBox1.SelectionStart;
            var selLen = comboBox1.SelectionLength;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(authors.Where(x => x.FullName.ToLower().Contains(comboBox1.Text)).ToArray());
            comboBox1.SelectionStart = selStart;
            comboBox1.SelectionLength = selLen;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Selected = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Search_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(comboBox1.SelectedItem != null)
            {
                SelectedAuthor = (DataClasses.Author)comboBox1.SelectedItem;
            }
        }
    }
}
