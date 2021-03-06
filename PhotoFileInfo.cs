using System;
using System.IO;
using ExifLib;
namespace PhotoSorter
{
    public class PhotoFileInfo
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _fileFullPath;
        private string _fileName;
        private DateTime _createDatetime;
        private int _createYear;
        private int _createMonth;
        private int _createDay;

        public string FileName { get => _fileName; set => _fileName = value; }
        public string FileFullPath { get => _fileFullPath; set => _fileFullPath = value; }
        public DateTime CreateDatetime { get => _createDatetime; set => _createDatetime = value; }
        public int CreateYear { get => _createYear; set => _createYear = value; }
        public int CreateMonth { get => _createMonth; set => _createMonth = value; }
        public int CreateDay { get => _createDay; set => _createDay = value; }
        public string FolderToMove { get => GetFolderToMove(); }
        public string FileNameToMove { get => GetFileNameToMove(); }
        public string DatePrefix { get => GetDatePrefix(); }
        public string NewFileName { get => GetNewFileName(); }


        public PhotoFileInfo(string fileFullPath)
        {
            _fileFullPath = fileFullPath;
            Logger.Info($"Найден файл {_fileFullPath}");
            try
            {
                using (ExifReader reader = new ExifReader(FileFullPath))
                {
                    reader.GetTagValue(ExifTags.DateTime, out _createDatetime);
                    if (_createDatetime  == DateTime.MinValue)
                    {
                        _createDatetime = File.GetCreationTime(fileFullPath);
                    }
                    Logger.Debug($"Debug ExifCreationTime = {_createDatetime}");
                    Logger.Debug($"Debug CreationTime = {File.GetCreationTime(fileFullPath)}");
                }
            }
            catch
            {
                Logger.Debug($"Debug CreationTime = {File.GetCreationTime(fileFullPath)}");
                _createDatetime = File.GetCreationTime(fileFullPath);
            }

            _fileName = Path.GetFileName(FileFullPath);
            Logger.Info($"Имя файла = {_fileName}, " +
                $"расширение файла = {Path.GetExtension(FileFullPath)}");
            _createYear = CreateDatetime.Year;
            _createMonth = CreateDatetime.Month;
            _createDay = CreateDatetime.Day;
            Logger.Info($"Дата создания = {CreateDatetime}, год = {CreateYear}" +
                $", месяц = {CreateMonth}");
        }

        private string GetFolderToMove()
        {
            string result = $"{CreateYear:D4}{Path.DirectorySeparatorChar}{CreateMonth:D2}";
            return result;
        }

        private string GetFileNameToMove()
        {
            string folderToMove = GetFolderToMove();
            string result = $"{folderToMove}{Path.DirectorySeparatorChar}{NewFileName}";
            return result;
        }

        private string GetDatePrefix()
        {
            return $"{CreateYear:D4}{CreateMonth:D2}{CreateDay:D2}";
        }
        private string GetNewFileName()
        {
            return $"{DatePrefix}_{FileName}";
        }

    }
}