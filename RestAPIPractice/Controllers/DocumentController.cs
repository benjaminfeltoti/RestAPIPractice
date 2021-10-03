using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestAPIPractice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestAPIPractice.Controllers
{
    [Route("api/dokumentumok")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> documentControllerLogger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            documentControllerLogger = logger;
        }

        [HttpGet]
        public string Get()
        {
            var files = Directory.GetFiles(Settings.DocumentsFolderPath);

            FileModel[] fileModels = new FileModel[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                var filePathAsArray = files[i].Split('\\');
                var filename = filePathAsArray[filePathAsArray.Length - 1];

                fileModels[i] = new FileModel
                {
                    Content = null,
                    FileFullPath = files[i],
                    FileName = filename
                };
            }

            return JsonSerializer.Serialize(fileModels);
        }

        [HttpGet("{fileName}")]
        public string Get(string fileName)
        {
            string responseString;

            var targetFilePath = Settings.DocumentsFolderPath + '\\' + fileName;

            if (String.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(targetFilePath))
            {
                //TODO : Respond as an error correctly
                responseString = "ERROR! Given filename is incorrect or file could not be found.";
                return responseString;
            }

            try
            {
                byte[] fileContent = System.IO.File.ReadAllBytes(targetFilePath);
                responseString = Convert.ToBase64String(fileContent);
            }
            catch (Exception)
            {
                //TODO : Handle errors
                responseString = "ERROR! Given filename could not be read!";
            }
            
            return JsonSerializer.Serialize(responseString);
        }

        // POST api/dokumentumok
        [HttpPost]
        public void Post([FromForm] FileModel value)
        {
            if (value == null || String.IsNullOrWhiteSpace(value.FileName) || String.IsNullOrWhiteSpace(value.Content))
            {
                return;
            }

            try
            {
                var bytes = Convert.FromBase64String(value.Content);
                //TODO: Check if file already exists with the name. If yes, add a number to the name.
                System.IO.File.WriteAllBytesAsync(Settings.DocumentsFolderPath + "\\" + value.FileName, bytes);
            }
            catch (Exception)
            { // Suspend exceptions
            }

            // Validate File
        }

    }
}
