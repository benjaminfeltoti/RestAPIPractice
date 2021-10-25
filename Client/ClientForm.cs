using RestAPIPractice.Models;
using System;
using System.Collections.Generic;
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

        private async void uploadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;

            var fileModel = new FileModel();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileModel.FileFullPath = openFileDialog.FileName;
                var filePathAsArray = openFileDialog.FileName.Split('\\');
                
                fileModel.FileName = filePathAsArray[filePathAsArray.Length - 1];

                var fileContent = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                fileModel.Content = Convert.ToBase64String(fileContent);
            }
                        
            var postData = new List<KeyValuePair<string, string>>();

            postData.Add(new KeyValuePair<string, string>("FileName", fileModel.FileName));
            postData.Add(new KeyValuePair<string, string>("FileFullPath", fileModel.FileFullPath));
            postData.Add(new KeyValuePair<string, string>("Content", fileModel.Content));
            
            // POST
            // TODO: Handle timeouts
            try
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                HttpResponseMessage response = await client.PostAsync("api/dokumentumok", content);
                responseTextBox.Text = response.StatusCode.ToString();
            }
            catch (UriFormatException uriException)
            {
                responseTextBox.Text = "Given filepath was too long / file is too big!";
            }
            catch (Exception)
            {
                //Suspend
            }
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            if (filesListView.SelectedItems.Count != 1)
            {
                responseTextBox.Text = "Select an item to download!";
                return;
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
                filesListView.Items.Clear();

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
