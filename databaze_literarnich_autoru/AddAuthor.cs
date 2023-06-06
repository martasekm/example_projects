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
    public partial class AddAuthor : Form
    {
        public Author Author {get; private set; }
        public bool SavePressed { get; private set; } = false;
        public AddAuthor()
        {
            InitializeComponent();
            this.Author = new Author();
        }

        public AddAuthor(Author author)
        {
            InitializeComponent();
            this.Author = author;
            textBox1.Text = author.FirstName;
            textBox2.Text = author.LastName;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Text = author.DateOfBirth.ToShortDateString();
            checkBox1.Checked = author.DateOfDeath.HasValue;
            if (author.DateOfDeath.HasValue)
            {
                textBox4.Text = author.DateOfDeath.Value.ToShortDateString();
            }
            richTextBox1.Text = author.Description;
        }

        private void AddAuthor_Load(object sender, EventArgs e)
        {

        }

        private void UpdateAuthor()
        {
            Author.FirstName = textBox1.Text;
            Author.LastName = textBox2.Text;
            var validBirth = DateTime.TryParse(textBox3.Text, out DateTime parsedBirth);
            var validDeath = DateTime.TryParse(textBox4.Text, out DateTime parsedDeath);
            Author.DateOfBirth = validBirth ? parsedBirth : new DateTime(1980, 1, 1);
            Author.DateOfDeath = checkBox1.Checked && validDeath ? parsedDeath : null;
            Author.Description = richTextBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SavePressed = true;
            UpdateAuthor();
            this.Close();
        }

        private void AddAuthor_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled = ((CheckBox)sender).Checked;
        }
    }
}
