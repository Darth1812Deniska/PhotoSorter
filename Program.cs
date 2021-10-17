using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IniParser;
using IniParser.Model;

namespace PhotoSorter
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var parser = new FileIniDataParser();
            string fromFolderPath = string.Empty;
            string toFolderPath = string.Empty;
            string replaceOrCopy = string.Empty;
            if (File.Exists("config.ini"))
            {
                Logger.Info("\"config.ini\" Найден");
                IniData data = parser.ReadFile("config.ini");
                fromFolderPath = data["PhotoSorter"]["FromFolderPath"];
                Logger.Debug(fromFolderPath);
                toFolderPath = data["PhotoSorter"]["ToFolderPath"];
                Logger.Debug(toFolderPath);
                replaceOrCopy = data["PhotoSorter"]["ReplaceOrCopy"];
                Logger.Debug(replaceOrCopy);
            }
            else
            {
                Logger.Info("\"config.ini\" Не найден");
                IniData iniData = new IniData();
                iniData.Sections.AddSection("PhotoSorter");
                iniData["PhotoSorter"].AddKey("FromFolderPath");
                iniData["PhotoSorter"].AddKey("ToFolderPath");
                iniData["PhotoSorter"].AddKey("ReplaceOrCopy");
                iniData["PhotoSorter"]["FromFolderPath"] = string.Empty;
                iniData["PhotoSorter"]["ToFolderPath"] = string.Empty;
                iniData["PhotoSorter"]["ReplaceOrCopy"] = "C";
                parser.WriteFile("config.ini", iniData);
                Logger.Info("\"config.ini\" Создан");
                return;
            }

            bool fromFolderPathIsEmpty = string.IsNullOrEmpty(fromFolderPath);
            bool toFolderPathIsEmpty = string.IsNullOrEmpty(toFolderPath);
            if (fromFolderPathIsEmpty)
            {
                Logger.Error("Не задана папка с изображениями");
                return;
            }

            if (toFolderPathIsEmpty)
            {
                Logger.Error("Не задана папка для сортировки");
                return;
            }

            if (!Directory.Exists(fromFolderPath))
            {
                Logger.Error($"Папки {fromFolderPath} не существует");
                return;
            }
            else
            {
                Logger.Info($"Папка {fromFolderPath} найдена");
            }
            if (!Directory.Exists(toFolderPath))
            {
                Logger.Info($"Папки {toFolderPath} не существует");
                Directory.CreateDirectory(toFolderPath);
                Logger.Info($"Папка {toFolderPath} создана");
            }
            else
            {
                Logger.Info($"Папка {toFolderPath} найдена");
            }
            List<PhotoFileInfo> photoFiles = GetPhotoFiles(fromFolderPath);
            foreach (PhotoFileInfo fileInfo in photoFiles)
            {
                string folderToMove = fileInfo.FolderToMove;
                string fullFolderToMove = $"{toFolderPath}{Path.DirectorySeparatorChar}{folderToMove}";
                Console.WriteLine($"fullFolderToMove = {fullFolderToMove}");
                Directory.CreateDirectory(fullFolderToMove);
                switch(replaceOrCopy)
                {
                    case "C":
                        File.Copy(fileInfo.FileFullPath, $"{toFolderPath}{Path.DirectorySeparatorChar}{fileInfo.FileNameToMove}", true);
                        break;
                    default:
                        File.Copy(fileInfo.FileFullPath, $"{toFolderPath}{Path.DirectorySeparatorChar}{fileInfo.FileNameToMove}", true);
                        break;
                    case "R":
                        File.Replace(fileInfo.FileFullPath, $"{toFolderPath}{Path.DirectorySeparatorChar}{fileInfo.FileNameToMove}",
                            $"{toFolderPath}{Path.DirectorySeparatorChar}{fileInfo.FileNameToMove}.bac");
                        break;
                }
            }

        }
        static private List<PhotoFileInfo> GetPhotoFiles(string fromFolderPath)
        {
            List<PhotoFileInfo> result = new List<PhotoFileInfo>();
            result = Directory.GetFiles(fromFolderPath)
                .Where(f => Path.GetExtension(f).ToLower() == ".jpg")
                .Select(x => new PhotoFileInfo(x)).ToList();
            var folders = Directory.GetDirectories(fromFolderPath);
            foreach (string folderPath in folders)
            {
                List<PhotoFileInfo> photoList = GetPhotoFiles(folderPath);
                result.AddRange(photoList);
            }
            return result;
        }
    }
}
