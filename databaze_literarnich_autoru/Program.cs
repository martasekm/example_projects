namespace FinalProject
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var loginForm = new LoginForm();
            Application.Run(loginForm);
            if(loginForm.SelectedUserRole == null)
            {
                return;
            }
            var dataLoadDialog = new DataLoadDialog();
            Application.Run(dataLoadDialog);
            if(!dataLoadDialog.LoadedSuccessfully)
            {
                return;
            }
            var mainWindow = new MainWindow(dataLoadDialog.LoadedAuthors, loginForm.SelectedUserRole, dataLoadDialog.Fetcher);
            Application.Run(mainWindow);
            if(mainWindow.EditsMade && mainWindow.OutputJsonPath != null)
            {
                ExportAuthors(mainWindow.AuthorsList, mainWindow.OutputJsonPath);
            }
        }

        static void ExportAuthors(List<DataClasses.Author> authors, string filePath)
        {
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(authors);
            File.WriteAllText(filePath, serialized);
        }
    }

}