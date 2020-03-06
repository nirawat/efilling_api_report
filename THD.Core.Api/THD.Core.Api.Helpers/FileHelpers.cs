using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace THD.Core.Api.Helpers
{
    public static class ServerDirectorys
    {
        public static string GetDirectorys(string StorePath, FolderDocument Folder)
        {
            string Directorys = $"{StorePath + Folder.ToString()}";
            if (!Directory.Exists(Directorys))
            {
                DirectoryInfo dir = Directory.CreateDirectory(Directorys);
            }
            return Directorys;
        }



        public static bool SaveFileFromBase64(string StorePath, FolderDocument Folder, string FileName, string FileBase64)
        {
            bool result = false;
            string dir = GetDirectorys(StorePath, Folder);
            string create_file = $"{dir + "\\" + FileName.ToString()}";
            if (Directory.Exists(dir))
            {
                if (!string.IsNullOrEmpty(FileBase64))
                {
                    byte[] fileBytes = Convert.FromBase64String(FileBase64);
                    using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                    {
                        File.WriteAllBytes(create_file, fileBytes);
                        result = true;
                    }
                }
            }
            return result;
        }

        public static string ReadFileToBase64(string StorePath, FolderDocument Folder, string FileName)
        {
            string fBase64 = string.Empty;
            string dir = GetDirectorys(StorePath, Folder);
            string read_file = $"{dir + "\\" + FileName.ToString()}";
            if (File.Exists(read_file))
            {
                string readFileByte = Convert.ToBase64String(File.ReadAllBytes(read_file));
                fBase64 = readFileByte;
            }
            return fBase64;
        }

        public static string DownloadFileHome1(string StorePathArchive, string StorePathDocument, FolderDocument Folder, string ProjectNumber, string FileZipName,
                                               string FileName1, string FileName2, string FileName3, string FileName4, string FileName5)
        {
            string dirArchive = $"{StorePathArchive + Folder.ToString() + "\\" + ProjectNumber}";
            if (!Directory.Exists(dirArchive))
            {
                DirectoryInfo dir = Directory.CreateDirectory(dirArchive);
            }
            string dirDocument = GetDirectorys(StorePathDocument, Folder);

            DirectoryInfo from = new DirectoryInfo(dirArchive);

            string archive_file = $"{dirArchive + "\\" + FileZipName.ToString()}";

            using (FileStream zipToOpen = new FileStream(archive_file, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {

                    //File 1
                    string read_file_1 = $"{dirDocument + "\\" + FileName1.ToString()}";
                    if (File.Exists(read_file_1))
                    {
                        string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_1));
                        string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                        string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName1.ToString()).Replace(".", "") + ";base64,", "");

                        string tmp_file = $"{dirArchive + "\\" + "แบบเสนอเพื่อขอรับการพิจารณารับรองด้านความปลอดภัย" + Path.GetExtension(FileName1.ToString())}";
                        byte[] fileBytes = Convert.FromBase64String(modify_base64);
                        using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                        {
                            File.WriteAllBytes(tmp_file, fileBytes);
                        }

                        var relPath = tmp_file.Substring(from.FullName.Length + 1);
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                    }


                    //File 2
                    string read_file_2 = $"{dirDocument + "\\" + FileName2.ToString()}";
                    if (File.Exists(read_file_2))
                    {
                        string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_2));
                        string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                        string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName2.ToString()).Replace(".", "") + ";base64,", "");

                        string tmp_file = $"{dirArchive + "\\" + "โครงการวิจัยฉบับสมบูรณ์" + Path.GetExtension(FileName2.ToString())}";
                        byte[] fileBytes = Convert.FromBase64String(modify_base64);
                        using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                        {
                            File.WriteAllBytes(tmp_file, fileBytes);
                        }

                        var relPath = tmp_file.Substring(from.FullName.Length + 1);
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                    }

                    //File 3
                    string read_file_3 = $"{dirDocument + "\\" + FileName3.ToString()}";
                    if (File.Exists(read_file_3))
                    {
                        string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_3));
                        string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                        string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName3.ToString()).Replace(".", "") + ";base64,", "");

                        string tmp_file = $"{dirArchive + "\\" + "เอกสารชี้แจงรายละเอียดของเชื้อที่ใช้_แบบฟอร์มข้อตกลงการใช้ตัวอย่างชีวภาพ" + Path.GetExtension(FileName3.ToString())}";
                        byte[] fileBytes = Convert.FromBase64String(modify_base64);
                        using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                        {
                            File.WriteAllBytes(tmp_file, fileBytes);
                        }

                        var relPath = tmp_file.Substring(from.FullName.Length + 1);
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                    }

                    //File 4
                    string read_file_4 = $"{dirDocument + "\\" + FileName4.ToString()}";
                    if (File.Exists(read_file_4))
                    {
                        string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_4));
                        string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                        string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName4.ToString()).Replace(".", "") + ";base64,", "");

                        string tmp_file = $"{dirArchive + "\\" + "หนังสือรับรองและอนุมัติให้ใช้สถานะที่" + Path.GetExtension(FileName4.ToString())}";
                        byte[] fileBytes = Convert.FromBase64String(modify_base64);
                        using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                        {
                            File.WriteAllBytes(tmp_file, fileBytes);
                        }

                        var relPath = tmp_file.Substring(from.FullName.Length + 1);
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                    }

                    //File 5
                    string read_file_5 = $"{dirDocument + "\\" + FileName5.ToString()}";
                    if (File.Exists(read_file_5))
                    {
                        string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_5));
                        string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                        string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName5.ToString()).Replace(".", "") + ";base64,", "");

                        string tmp_file = $"{dirArchive + "\\" + "อื่นๆ" + Path.GetExtension(FileName5.ToString())}";
                        byte[] fileBytes = Convert.FromBase64String(modify_base64);
                        using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                        {
                            File.WriteAllBytes(tmp_file, fileBytes);
                        }

                        var relPath = tmp_file.Substring(from.FullName.Length + 1);
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                    }

                }
            }


            if (File.Exists(archive_file))
            {
                string readFileByte = "data:application/zip;base64," + Convert.ToBase64String(File.ReadAllBytes(archive_file));
                return readFileByte;
            }


            return null;

        }

        public static string DownloadFileHome2(string StorePathArchive, string StorePathDocument, FolderDocument Folder, string ProjectNumber, string FileZipName, string FileName1, string FileName2)
        {
            string dirArchive = $"{StorePathArchive + Folder.ToString() + "\\" + ProjectNumber}";
            if (!Directory.Exists(dirArchive))
            {
                DirectoryInfo dir = Directory.CreateDirectory(dirArchive);
            }
            string dirDocument = GetDirectorys(StorePathDocument, Folder);

            if (!string.IsNullOrEmpty(FileName1) && !string.IsNullOrEmpty(FileName2))
            {
                DirectoryInfo from = new DirectoryInfo(dirArchive);

                string archive_file = $"{dirArchive + "\\" + FileZipName.ToString()}";

                using (FileStream zipToOpen = new FileStream(archive_file, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                    {

                        //File 1
                        string read_file_1 = $"{dirDocument + "\\" + FileName1.ToString()}";
                        if (File.Exists(read_file_1))
                        {
                            string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_1));
                            string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                            string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName1.ToString()).Replace(".", "") + ";base64,", "");

                            string tmp_file = $"{dirArchive + "\\" + "แนบไฟล์แบบคำขอ (NUIBC01)" + Path.GetExtension(FileName1.ToString())}";
                            byte[] fileBytes = Convert.FromBase64String(modify_base64);
                            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                            {
                                File.WriteAllBytes(tmp_file, fileBytes);
                            }

                            var relPath = tmp_file.Substring(from.FullName.Length + 1);
                            ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                        }


                        //File 2
                        string read_file_2 = $"{dirDocument + "\\" + FileName2.ToString()}";
                        if (File.Exists(read_file_2))
                        {
                            string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_2));
                            string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                            string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName2.ToString()).Replace(".", "") + ";base64,", "");

                            string tmp_file = $"{dirArchive + "\\" + "แนบไฟล์แบบประเมินเบื้องต้น" + Path.GetExtension(FileName2.ToString())}";
                            byte[] fileBytes = Convert.FromBase64String(modify_base64);
                            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                            {
                                File.WriteAllBytes(tmp_file, fileBytes);
                            }

                            var relPath = tmp_file.Substring(from.FullName.Length + 1);
                            ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                        }

                    }
                }

                if (File.Exists(archive_file))
                {
                    string readFileByte = "data:application/zip;base64," + Convert.ToBase64String(File.ReadAllBytes(archive_file));
                    return readFileByte;
                }
            }

            //File 1
            if (!string.IsNullOrEmpty(FileName1) && string.IsNullOrEmpty(FileName2))
            {
                string read_file = $"{dirDocument + "\\" + FileName1.ToString()}";
                if (File.Exists(read_file))
                {
                    string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file));
                    string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                    return decode_Base64;
                }
            }

            //File 2
            if (!string.IsNullOrEmpty(FileName2) && string.IsNullOrEmpty(FileName1))
            {
                string read_file = $"{dirDocument + "\\" + FileName2.ToString()}";
                if (File.Exists(read_file))
                {
                    string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file));
                    string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                    return decode_Base64;
                }
            }

            return null;

        }

        public static string DownloadFileC3Tab2(string StorePathArchive, string StorePathDocument, FolderDocument Folder, string ProjectNumber, string FileZipName,
                                               string FileName1, string FileName2, string FileName3)
        {
            string dirArchive = $"{StorePathArchive + Folder.ToString() + "\\" + ProjectNumber}";
            if (!Directory.Exists(dirArchive))
            {
                DirectoryInfo dir = Directory.CreateDirectory(dirArchive);
            }
            string dirDocument = GetDirectorys(StorePathDocument, Folder);

            DirectoryInfo from = new DirectoryInfo(dirArchive);

            string archive_file = $"{dirArchive + "\\" + FileZipName.ToString()}";

            using (FileStream zipToOpen = new FileStream(archive_file, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {

                    //File 1
                    if (!string.IsNullOrEmpty(FileName1))
                    {
                        string read_file_1 = $"{dirDocument + "\\" + FileName1.ToString()}";
                        if (File.Exists(read_file_1))
                        {
                            string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_1));
                            string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                            string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName1.ToString()).Replace(".", "") + ";base64,", "");

                            string tmp_file = $"{dirArchive + "\\" + "file_1_" + Path.GetExtension(FileName1.ToString())}";
                            byte[] fileBytes = Convert.FromBase64String(modify_base64);
                            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                            {
                                File.WriteAllBytes(tmp_file, fileBytes);
                            }

                            var relPath = tmp_file.Substring(from.FullName.Length + 1);
                            ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                        }
                    }


                    //File 2
                    if (!string.IsNullOrEmpty(FileName2))
                    {
                        string read_file_2 = $"{dirDocument + "\\" + FileName2.ToString()}";
                        if (File.Exists(read_file_2))
                        {
                            string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_2));
                            string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                            string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName2.ToString()).Replace(".", "") + ";base64,", "");

                            string tmp_file = $"{dirArchive + "\\" + "file_2_" + Path.GetExtension(FileName2.ToString())}";
                            byte[] fileBytes = Convert.FromBase64String(modify_base64);
                            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                            {
                                File.WriteAllBytes(tmp_file, fileBytes);
                            }

                            var relPath = tmp_file.Substring(from.FullName.Length + 1);
                            ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                        }
                    }


                    //File 3
                    if (!string.IsNullOrEmpty(FileName3))
                    {
                        string read_file_3 = $"{dirDocument + "\\" + FileName3.ToString()}";
                        if (File.Exists(read_file_3))
                        {
                            string encode_Base64 = Convert.ToBase64String(File.ReadAllBytes(read_file_3));
                            string decode_Base64 = Encoding.UTF8.GetString(Convert.FromBase64String(encode_Base64));
                            string modify_base64 = decode_Base64.Replace("data:application/" + Path.GetExtension(FileName3.ToString()).Replace(".", "") + ";base64,", "");

                            string tmp_file = $"{dirArchive + "\\" + "file_3_" + Path.GetExtension(FileName3.ToString())}";
                            byte[] fileBytes = Convert.FromBase64String(modify_base64);
                            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                            {
                                File.WriteAllBytes(tmp_file, fileBytes);
                            }

                            var relPath = tmp_file.Substring(from.FullName.Length + 1);
                            ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(tmp_file, relPath);
                        }
                    }
                }
            }


            if (File.Exists(archive_file))
            {
                string readFileByte = "data:application/zip;base64," + Convert.ToBase64String(File.ReadAllBytes(archive_file));
                return readFileByte;
            }


            return null;

        }

        public enum FolderDocument
        {
            menuA1 = 0,
            menuA2 = 1,
            menuA3 = 2,
            menuA4 = 3,
            menuA5 = 4,
            menuA6 = 5,
            menuA7 = 6,
            menuA8 = 7,
            menuC3Tab2 = 8,
            menuC3Tab4 = 9,
        }
    }
}
