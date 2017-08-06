using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            string source = @"C:\Users\Daniel\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            string destination = @"C:\OneDrive\Pictures\Windows";
            CloneDirectory(source, destination);
        }

        private static void CloneDirectory(string root, string dest)
        {
            foreach (var directory in System.IO.Directory.GetDirectories(root))
            {
                string dirName = Path.GetFileName(directory);
                if (!System.IO.Directory.Exists(Path.Combine(dest, dirName)))
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(dest, dirName));
                }
                CloneDirectory(directory, Path.Combine(dest, dirName));
            }

            foreach (var file in System.IO.Directory.GetFiles(root))
            {
                if (!File.Exists(Path.Combine(dest, Path.GetFileName(Path.ChangeExtension(file, ".jpg")))))
                {
                    File.Copy(file, Path.Combine(dest, Path.GetFileName(Path.ChangeExtension(file, ".jpg"))));
                }
            }

            foreach (var file in System.IO.Directory.GetFiles(dest))
            {
                try
                {
                    var directories = ImageMetadataReader.ReadMetadata(Path.Combine(dest, Path.GetFileName(file)));
                    foreach (var directory in directories)
                    {
                        foreach (var tag in directory.Tags)
                        {
                            var Name = tag.Name;
                            var Description = tag.Description;

                            if (Name.Contains("Image Height") && !Description.Contains("1080"))
                            {
                                File.Delete(Path.Combine(dest, Path.GetFileName(file)));
                            }
                            else if (Name.Contains("Image Width") && !Description.Contains("1920"))
                            {
                                File.Delete(Path.Combine(dest, Path.GetFileName(file)));
                            }
                        }
                    }
                }
            catch
                {
                }

            }

            foreach (var file in System.IO.Directory.GetFiles(dest))
            {
                FileInfo size = new FileInfo(file);
                if (size.Length < 100000)
                {
                    File.Delete(Path.Combine(dest, Path.GetFileName(file)));
                }
            }
        }
    }
}
