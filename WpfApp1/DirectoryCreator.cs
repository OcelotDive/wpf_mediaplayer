using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class DirectoryCreator
    {
       static string previouslyPlayedImagesPath;
       static string previouslyPlayedFileDirectoryPath;



       public static string PreviousImagePath
        {
            get { return previouslyPlayedImagesPath; }
            set { previouslyPlayedImagesPath = value; }
        }

        public static string PreviouslFilePath
        {
            get { return previouslyPlayedFileDirectoryPath; }
            set { previouslyPlayedFileDirectoryPath = value; }
        }

        internal void BootStrapMediaPlayer()
        {
            CreateAppRootDirectory();
            CreateScreenDumpDirectory();
            CreateFileInfoDirectory();
            CreateMediaTitleFile();
        }

        private void CreateAppRootDirectory()
        {
            var pathToRootMediaDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WPFMEDIA");
            var rootDirectory = new System.IO.DirectoryInfo(pathToRootMediaDirectory);
            if (!rootDirectory.Exists)
                rootDirectory.Create();
        }

        private void CreateScreenDumpDirectory()
        {
            var pathToImagesDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WPFMEDIA", "Screendumps");
            previouslyPlayedImagesPath = pathToImagesDirectory.ToString();
            var imageDirectory = new DirectoryInfo(pathToImagesDirectory);
            if (!imageDirectory.Exists)
                imageDirectory.Create();
        }

        private void CreateFileInfoDirectory()
        {
            var pathToInfoFileDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WPFMEDIA", "Info");
            previouslyPlayedFileDirectoryPath = pathToInfoFileDirectory.ToString();
            var infoDirectory = new DirectoryInfo(pathToInfoFileDirectory);

            if (!infoDirectory.Exists)
            {
                infoDirectory.Create();
                string mediaTitleInfoFile = previouslyPlayedFileDirectoryPath + "\\lastPlays.txt";
                File.Create(mediaTitleInfoFile);
            }
        }

        private void CreateMediaTitleFile()
        {
            string mediaTitleInfoFile = previouslyPlayedFileDirectoryPath + "\\lastPlays.txt";
            if (!File.Exists(mediaTitleInfoFile))
                File.Create(mediaTitleInfoFile);
        }
    }
}
