using System.IO;

namespace RestAPIPractice
{
    public class SettingsReader
    {
        private const string configurationFilePath = "Resources\\configuration.txt";

        public string GetPath()
        {
            string path;

            using (StreamReader streamReader = new StreamReader(configurationFilePath))
            {
                path = streamReader.ReadLine();
            }

            //TODO : Validate path/file format

            return path;
        }
    }
}
