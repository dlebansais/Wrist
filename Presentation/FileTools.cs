using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

namespace Presentation
{
    public class FileTools
    {
        #region File Dialog
        public static bool OpenTextFile(out string fileName, out string content)
        {
            fileName = null;
            content = null;
            return false;
        }

        /*
        private void OnFileOpened(object sender, CSHTML5.Extensions.FileOpenDialog.FileOpenedEventArgs e)
        {
            MessageBox.Show(e.Text);
            using (StringReader sr = new StringReader(e.Text))
            {
                string Cotnent = sr.ReadToEnd();
            }
        }*/

        public static void SaveTextFile(string fileName, string content)
        {
            //Task task = FileSaveDialog.SaveTextToFile(fileName, content);
            //task.Wait();
        }
        #endregion

        #region File
        public static bool FileExists(string fileName)
        {
            return Bookkeeping.FileExists(fileName);
        }

        public static string LoadTextFile(string fileName, FileMode mode)
        {
            byte[] Bytes = LoadFromIsolatedStorageFile(ToISFileName(fileName), mode);
            if (Bytes == null)
                return null;

            return Encoding.UTF8.GetString(Bytes);
        }

        public static bool CommitTextFile(string fileName, string content)
        {
            byte[] bytes = (content == null) ? null : Encoding.UTF8.GetBytes(content);

            bool Success = SaveToIsolatedStorageFile(ToISFileName(fileName), bytes);
            if (Success)
                Bookkeeping.CreateFile(fileName);

            return Success;
        }

        public static byte[] LoadBinaryFile(string fileName, FileMode mode)
        {
            return LoadFromIsolatedStorageFile(ToISFileName(fileName), mode);
        }

        public static bool CommitBinaryFile(string fileName, byte[] content)
        {
            bool Success = SaveToIsolatedStorageFile(ToISFileName(fileName), content);
            if (Success)
                Bookkeeping.CreateFile(fileName);

            return Success;
        }

        public static void CopyFile(string sourceFileName, string destinationFileName)
        {
            Bookkeeping.CreateFile(destinationFileName);

            string Source = ToISFileName(sourceFileName);
            string Destination = ToISFileName(destinationFileName);

            if (IsolatedStorageFileExists(Source))
                CopyIsolatedStorageFile(Source, Destination, out bool success);
        }
        #endregion

        #region Directory
        public static bool DirectoryExists(string directoryName)
        {
            return Bookkeeping.DirectoryExists(directoryName);
        }

        public static void CreateDirectory(string directoryName)
        {
            Bookkeeping.CreateDirectory(directoryName);
        }

        public static void DeleteDirectory(string directoryName)
        {
            Bookkeeping.DeleteDirectory(directoryName);
        }

        public static string[] DirectoryFolders(string directoryName)
        {
            return Bookkeeping.DirectoryFolders(directoryName);
        }

        public static string[] DirectoryFiles(string directoryName, string extension)
        {
            return Bookkeeping.DirectoryFiles(directoryName, extension);
        }
        #endregion

        #region Bitmap
        public static FileBitmapImage LoadBitmap(string fileName)
        {
            string Source = ToISFileName(fileName);
            if (!IsolatedStorageFileExists(Source))
                return null;

            if (BitmapCache.ContainsKey(Source))
                return BitmapCache[Source];
            else
            {
                FileBitmapImage Result = LoadFromIsolatedStorageBitmap(Source);
                if (Result != null)
                    BitmapCache.Add(Source, Result);

                return Result;
            }
        }

        private static Dictionary<string, FileBitmapImage> BitmapCache = new Dictionary<string, FileBitmapImage>();
        #endregion

        #region Isolated Storage
        private static string ToISFileName(string fileName)
        {
            fileName = fileName.Replace('\\', '_');
            fileName = fileName.Replace('/', '_');

            return fileName;
        }

        private static string ToISDirectoryName(string directoryName)
        {
            if (directoryName.EndsWith("/") || directoryName.EndsWith("\\"))
                return ToISFileName(directoryName);
            else
                return ToISFileName(directoryName + "/");
        }

        private static IsolatedStorageFile GetStorageRoot()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();
            return storage;
        }

        private static bool IsolatedStorageFileExists(string fileName)
        {
            try
            {
                using (IsolatedStorageFile storage = GetStorageRoot())
                {
                    return storage.FileExists(fileName);
                }
            }
            catch (Exception e)
            {
                PrintException(e, "File Exists", fileName);
            }

            return false;
        }

        private static byte[] LoadFromIsolatedStorageFile(string fileName, FileMode mode)
        {
            byte[] Content = null;

            try
            {
                using (IsolatedStorageFile storage = GetStorageRoot())
                {
                    using (IsolatedStorageFileStream fs = storage.OpenFile(fileName, mode))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            Content = br.ReadBytes((int)fs.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PrintException(e, "Load File", fileName);
            }

            return Content;
        }

        private static bool SaveToIsolatedStorageFile(string fileName, byte[] content)
        {
            //Debug.WriteLine($"Writing {content.Length} bytes in file {fileName}");

            try
            {
                using (IsolatedStorageFile storage = GetStorageRoot())
                {
                    using (IsolatedStorageFileStream fs = storage.CreateFile(fileName))
                    {
                        try
                        {
                            fs.Write(content, 0, content.Length);
                            fs.Close();
                            return true;
                        }
                        catch (Exception e)
                        {
                            PrintException(e, "Write File Data", fileName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PrintException(e, "Save File", fileName);
            }

            return false;
        }

        private static void DeleteIsolatedStorageFile(string fileName)
        {
            try
            {
                using (IsolatedStorageFile storage = GetStorageRoot())
                {
                    //storage.DeleteFile(fileName);
                }
            }
            catch (Exception e)
            {
                PrintException(e, "Delete File", fileName);
            }
        }

        private static void CopyIsolatedStorageFile(string sourceFileName, string destinationFileName, out bool success)
        {
            success = false;

            try
            {
                using (IsolatedStorageFile storage = GetStorageRoot())
                {
                    using (IsolatedStorageFileStream src = storage.OpenFile(sourceFileName, FileMode.Open))
                    {
                        using (BinaryReader br = new BinaryReader(src))
                        {
                            byte[] Content = br.ReadBytes((int)src.Length);

                            using (IsolatedStorageFileStream dst = storage.CreateFile(destinationFileName))
                            {
                                dst.Write(Content, 0, Content.Length);
                                dst.Close();

                                success = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PrintException(e, "Copy File", sourceFileName + "/" + destinationFileName);
            }
        }

        private static FileBitmapImage LoadFromIsolatedStorageBitmap(string fileName)
        {
            FileBitmapImage Result = null;

            try
            {
                IsolatedStorageFile storage = GetStorageRoot();
                IsolatedStorageFileStream fs = storage.OpenFile(fileName, FileMode.Open);
                Result = new FileBitmapImage(storage, fs);
            }
            catch (Exception e)
            {
                PrintException(e, "Load Bitmap", fileName);
            }

            return Result;
        }

        private static void PrintException(Exception e, string Operation, string fileName)
        {
            //Debug.WriteLine("Isolated Storage Error, Operation=" + Operation + ", Filename=" + fileName + ": " + e.Message);
        }
        #endregion
    }
}