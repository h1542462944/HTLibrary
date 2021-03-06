﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace User.IO
{
    public static class UserIO
    {
        /// <summary>
        /// 基于遍历的复制文件夹
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        public static void CopyFolder(string sourcePath, string destPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                //获得源文件下所有文件
                try
                {
                    List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                    files.ForEach(c =>
                    {
                        string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                        try
                        {
                            File.Copy(c, destFile, true);//覆盖模式
                        }
                        catch (Exception)
                        {
                        }

                    });
                    //获得源文件下所有目录文件
                    List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
                    foreach (var item in folders)
                    {
                        Console.WriteLine(item);
                    }
                    folders.ForEach(c =>
                    {
                        string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                        //采用递归的方法实现
                        try
                        {
                            CopyFolder(c, destDir);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            //throw;
                        }

                    });
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                throw new DirectoryNotFoundException("源目录不存在！");
            }
        }

        /// <summary>
        /// 安全的复制
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destFilePath"></param>
        /// <param name="waitTime">每次尝试复制以后等待,毫秒</param>
        /// <param name="n">次数</param>
        /// <returns></returns>
        public static async Task<bool> SafeCopy(string sourceFilePath, string destFilePath, int waitTime = 200, int n = 3)
        {
            if (!File.Exists(sourceFilePath))
            {
#if DEBUG
                throw new ArgumentException();
#endif
                return false;
            }
            bool isSuccess = false;
            await Task.Run(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        File.Copy(sourceFilePath, destFilePath, true);
                        isSuccess = true;
                        break;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(waitTime);
                    }
                    if (i == n - 1)
                    {
                        isSuccess = false;
                    }
                }


            });
            return isSuccess;
        }

        public static async Task<bool> SafeDelete(string path, int waitTime = 200, int n = 3)
        {
            if (!File.Exists(path))
            {
#if DEBUG
                throw new ArgumentException();
#endif
                return false;
            }
            bool isSuccess = false;
            await Task.Run(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        File.Delete(path);
                        isSuccess = true;
                        break;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(waitTime);
                    }
                    if (i == n - 1)
                    {
                        isSuccess = false;
                    }
                }
            });
            return isSuccess;
        }
    }
}
