using System;
using System.IO;
using ExifLib;
namespace PhotoSorter
{
    public class PhotoFileInfo
    {
        private string _fileFullPath;
        private string _fileName;
        private DateTime _createDatetime;
        private int _createYear;
        private int _createMonth;

        public string FileName { get => _fileName; set => _fileName = value; }
        public string FileFullPath { get => _fileFullPath; set => _fileFullPath = value; }
        public DateTime CreateDatetime { get => _createDatetime; set => _createDatetime = value; }
        public int CreateYear { get => _createYear; set => _createYear = value; }
        public int CreateMonth { get => _createMonth; set => _createMonth = value; }
        public string FolderToMove { get => GetFolderToMove(); }
        public string FileNameToMove { get => GetFileNameToMove(); }

        public PhotoFileInfo(string fileFullPath)
        {
            _fileFullPath = fileFullPath;
            Console.WriteLine($"Найден файл {_fileFullPath}");
            try
            {
                using (ExifReader reader = new ExifReader(FileFullPath))
                {
                    reader.GetTagValue(ExifTags.DateTime, out _createDatetime);
                }
            }
            catch
            {
                _createDatetime = DateTime.MinValue;
            }
            
            //_createDatetime = createDateTime;
            _fileName = Path.GetFileName(FileFullPath);
            Console.WriteLine($"Имя файла = {_fileName}, " +
                $"расширение файла = {Path.GetExtension(FileFullPath)}");
            _createYear = CreateDatetime.Year;
            _createMonth = CreateDatetime.Month;
            Console.WriteLine($"Дата создания = {CreateDatetime}, год = {CreateYear}" +
                $", месяц = {CreateMonth}");
        }

        public string GetFolderToMove()
        {
            string result = $"{CreateYear}{Path.DirectorySeparatorChar}{CreateMonth}";
            return result;
        }

        public string GetFileNameToMove()
        {
            string result = string.Empty;
            string folderToMove = GetFolderToMove();
            result = $"{folderToMove}{Path.DirectorySeparatorChar}{FileName}";
            return result;
        }

    }
}