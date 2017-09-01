using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Win32;

namespace WindowsSpotlightBackground
{
    class Program
    {
        static void Main(string[] args)
        {
            string UserName = Environment.UserName;
            string SourceDirectory = $@"C:\Users\{UserName}\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            string DestinationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\Windows Spotlight Images";
            CreateDirectory(SourceDirectory, DestinationDirectory);
            CopyFiles(SourceDirectory, DestinationDirectory);
            DeleteUnwantedFiles(SourceDirectory, DestinationDirectory);
            //SetDesktopBackground(UserName, DestinationDirectory);
        }

        private static void CreateDirectory(string SourceDirectory, string DestinationDirectory)
        {
            if (!System.IO.Directory.Exists(DestinationDirectory))
            {
                System.IO.Directory.CreateDirectory(DestinationDirectory);
            }
        }

        private static void CopyFiles(string SourceDirectory, string DestinationDirectory)
        {
            foreach (var Directory in System.IO.Directory.GetDirectories(SourceDirectory))
            {
                string DirectoryName = Path.GetFileName(Directory);
                if (!System.IO.Directory.Exists(Path.Combine(DestinationDirectory, DirectoryName)))
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(DestinationDirectory, DirectoryName));
                }
                CopyFiles(Directory, Path.Combine(DestinationDirectory, DirectoryName));
            }

            foreach (var File in System.IO.Directory.GetFiles(SourceDirectory))
            {
                if (!System.IO.File.Exists(Path.Combine(DestinationDirectory, Path.GetFileName(Path.ChangeExtension(File, ".jpg")))))
                {
                    System.IO.File.Copy(File, Path.Combine(DestinationDirectory, Path.GetFileName(Path.ChangeExtension(File, ".jpg"))));
                }
            }
        }

        private static void DeleteUnwantedFiles(string SourceDirectory, string DestinationDirectory)
        {
            foreach (var File in System.IO.Directory.GetFiles(DestinationDirectory))
            {
                try
                {
                    var FileMetaData = ImageMetadataReader.ReadMetadata(Path.Combine(DestinationDirectory, Path.GetFileName(File)));
                    foreach (var MetaDataDirectory in FileMetaData)
                    {
                        foreach (var MetaDataTag in MetaDataDirectory.Tags)
                        {
                            var Name = MetaDataTag.Name;
                            var Description = MetaDataTag.Description;

                            if (Name.Contains("Image Height") && !Description.Contains("1080"))
                            {
                                System.IO.File.Delete(Path.Combine(DestinationDirectory, Path.GetFileName(File)));
                            }
                            else if (Name.Contains("Image Width") && !Description.Contains("1920"))
                            {
                                System.IO.File.Delete(Path.Combine(DestinationDirectory, Path.GetFileName(File)));
                            }
                        }
                    }
                }
                catch
                {
                    System.IO.File.Delete(Path.Combine(DestinationDirectory, Path.GetFileName(File)));
                }
            }
        }
    }
}

