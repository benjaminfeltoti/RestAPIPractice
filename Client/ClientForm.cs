using RestAPIPractice.Models;
using System;

using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        FileModel[] fileModels;

        HttpClient client;

        public ClientForm()
        {
            InitializeComponent();
            InitializeClient();
        }

        private void InitializeClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/");
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            //await client.PostAsync("api/dokumentumok", ); FileDialog
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            if (filesListView.SelectedItems.Count != 1)
            {
                responseTextBox.Text = "Select an item to download!";
            }

            var fileName = filesListView.SelectedItems[0].Text;

            var response = await client.GetAsync("api/dokumentumok" + '/' + fileName);

            string responseJson = await response.Content.ReadAsStringAsync();
            // TODO : Exception handling
            var fileBase64 = JsonSerializer.Deserialize<string>(responseJson);

            byte[] fileBytes = Convert.FromBase64String(fileBase64);

            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    // TODO : Choose unique filename if already taken.
                    System.IO.File.WriteAllBytes(folderBrowserDialog.SelectedPath + "\\" + fileName, fileBytes);
                    responseTextBox.Text = "Download was successfull at : " + folderBrowserDialog.SelectedPath;
                }
            }
        }

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                var response = await client.GetAsync("api/dokumentumok");

                string responseJson = await response.Content.ReadAsStringAsync();

                fileModels = JsonSerializer.Deserialize<FileModel[]>(responseJson);

                foreach (var file in fileModels)
                {
                    filesListView.Items.Add(new ListViewItem(file.FileName));
                }

                responseTextBox.Text = "";
            }
            catch (HttpRequestException)
            {
                responseTextBox.Text = "Connection failure!";
            }
            catch (Exception)
            {
                // Suspend exception
            }
        }
    }
}
