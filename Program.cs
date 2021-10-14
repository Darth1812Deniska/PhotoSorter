﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExifLib;

namespace PhotoSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            string fromFolderPath = @"/Users/admin/Pictures/Сохранение 20180505";
            if (!Directory.Exists(fromFolderPath))
            {
                Console.WriteLine($"Папки {fromFolderPath} не существует");
                return;
            }
            else
            {
                Console.WriteLine($"Папка {fromFolderPath} найдена");
            }
            string toFolderPath = @"/Users/admin/Pictures/test";
            if (!Directory.Exists(toFolderPath))
            {
                Console.WriteLine($"Папки {toFolderPath} не существует");
                return;
            }
            else
            {
                Console.WriteLine($"Папка {toFolderPath} найдена");
            }
            List<PhotoFileInfo> photoFiles = GetPhotoFiles(fromFolderPath);
            foreach (PhotoFileInfo fileInfo in photoFiles)
            {
                string folderToMove = fileInfo.FolderToMove;
                string fullFolderToMove = $"{toFolderPath}{Path.DirectorySeparatorChar}{folderToMove}";
                Console.WriteLine($"fullFolderToMove = {fullFolderToMove}");
                Directory.CreateDirectory(fullFolderToMove);
                File.Copy(fileInfo.FileFullPath, $"{toFolderPath}{Path.DirectorySeparatorChar}{fileInfo.FileNameToMove}");
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