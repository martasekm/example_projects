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
    public partial class DataLoadDialog : Form
    {
        private DataFetcher fetcher;
        public DataFetcher Fetcher { get { return fetcher; } private set { fetcher = value; } }
        public List<DataClasses.Author> LoadedAuthors { get; internal set; }
        public bool LoadedSuccessfully { get; internal set; }
        public DataLoadDialog()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            labelFileName.Text = openFileDialog1.FileName;
            fetcher = new JsonDataFetcher(openFileDialog1.FileName);
        }

        private async void buttonLoad_Click(object sender, EventArgs e)
        {
            string resultMsgBoxCaption, resultMsgBoxText;
            MessageBoxIcon resultMsgBoxIcon;
            MessageBoxButtons resultMsgBoxButtons = MessageBoxButtons.OK;
            var loadingBar = new LoadingBar();
            try
            {
                this.Hide();
                loadingBar.Show();
                var loadedAuthors = Task.Run(fetcher.GetAuthors);
                this.LoadedAuthors = await loadedAuthors;
                this.LoadedSuccessfully = true;
                resultMsgBoxCaption = "Uspech";
                resultMsgBoxText = "Data byla uspesne nactena.";
                resultMsgBoxIcon = MessageBoxIcon.Asterisk;
            }
            catch (Exception ex)
            {
                this.LoadedSuccessfully = false;
                resultMsgBoxCaption = "Chyba";
                resultMsgBoxText = "Vstupni soubor nema korektni format. Nactete prosim jiny.";
                resultMsgBoxIcon = MessageBoxIcon.Exclamation;
            }
            //await Task.Delay(3000);
            loadingBar.Close();
            MessageBox.Show(resultMsgBoxText, resultMsgBoxCaption, resultMsgBoxButtons, resultMsgBoxIcon);
            if (this.LoadedSuccessfully)
            {
                this.Close();
            }
            else
            {
                this.Show();
            }
        }

        private void DataLoadDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
