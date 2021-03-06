﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.HTStudioService;

namespace User.SoftWare.Service
{
    /// <summary>
    /// 为软件提供相应的服务.
    /// </summary>
    public class SoftWareService
    {
        public SoftWareService(Version version, string softWareName)
        {
            Version = version;
            SoftWareName = softWareName;
        }

        public Version Version { get; private set; }
        public string SoftWareName { get; private set; }
        /// <summary>
        /// 软件的根目录.
        /// </summary>
        public string Root { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 服务缓存文件夹.
        /// </summary>
        public string ServiceCache => Root + @"ServiceCache\";
        /// <summary>
        /// 更新文件夹.
        /// </summary>
        public string UpdateCache => ServiceCache + @"Update\";

        bool IsDownloading { get; set; } = false;
        HTStudioService.HTStudioService service = new HTStudioService.HTStudioService();
        DownloadTask[] CurrentTask { get; set; }

        public event CheckUpdateEventHandler CheckUpdateCompleted;
        public event ChannelFreshEventHandler ChannelFreshed;
        public event EventHandler<bool> UpdateCompleted;

        /// <summary>
        /// 检查更新,触发<see cref="CheckUpdateCompleted"/>事件.
        /// </summary>
        public void CheckUpdate()
        {
            try
            {
                if (CheckHasDownload())
                {
                    return;
                }
                service.CheckUpdate(SoftWareName, Version.ToString(), out UpdateType type, out bool m);
                string version = service.GetSoftWareVersion(SoftWareName);
                long size = 0;
                if (type == UpdateType.Download)
                {
                    CurrentTask = service.GetUpdateTask(SoftWareName, Version.ToString());
                    foreach (var item in CurrentTask)
                    {
                        size += item.Size;
                    }
                }
                CheckUpdateCompleted?.Invoke(this, new CheckUpdateEventArgs(ChannelState.Completed, type, version, size));
            }
            catch (Exception)
            {
                CheckUpdateCompleted?.Invoke(this, new CheckUpdateEventArgs(ChannelState.Failed));
            }
        }
        /// <summary>
        /// 下载更新,下载到<see cref="UpdateCache"/>指定的文件夹,并触发<see cref="ChannelFreshed"/>事件,以显示进度.
        /// </summary>
        public void DownloadUpdate()
        {
            if (!IsDownloading)
            {
                IsDownloading = true;
                try
                {
                    if (CheckHasDownload())
                    {
                        return;
                    }
                    long p = 0;
                    long size = 0;
                    DownloadTask[] task = service.GetUpdateTask(SoftWareName, Version.ToString());
                    foreach (var item in task)
                    {
                        size += item.Size;
                    }
                    foreach (var item in task)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(UpdateCache + item.ExtendedPath.Last));
                        using (FileStream fs = new FileStream(UpdateCache + item.ExtendedPath.Last, FileMode.Create))
                        {
                            for (int i = 0; i < item.Num; i++)
                            {
                                fs.Position = i * 1024;
                                foreach (var d in service.Download(item, i, true).Data)
                                {
                                    fs.WriteByte(d);
                                    p++;
                                }
                                ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Doing, p, size));
                            }
                        }
                    }
                    ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Completed));
                    File.WriteAllText(UpdateCache + "UpdateInfo.txt", Version.ToString());
                }
                catch (IOException)
                {
                    ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.FileFailed));
                }
                catch (Exception)
                {
                    ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Failed));
                }
                IsDownloading = false;
            }
        }
        /// <summary>
        /// 正式更新,将<see cref="UpdateCache"/>中的文件复制到<see cref="Root"/>.
        /// </summary>
        public void ApplyUpdate()
        {
            try
            {
                foreach (var item in new DirectoryInfo(UpdateCache).GetFiles())
                {
                    string newpath = Root + Tools.GetRelativePath(item.FullName, UpdateCache);
                    if (Tools.GetRelativePath(item.FullName, UpdateCache) != "UpdateInfo.txt")
                    {
                        File.Copy(item.FullName, newpath, true);
                        File.Delete(item.FullName);
                    }
                }
                File.Delete(UpdateCache + "UpdateInfo.txt");
            }
            catch (Exception)
            {

            }
            UpdateCompleted?.Invoke(this, true);
        }
        public void UploadUpdate(IEnumerable<FileSystemInfo> infos)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (var item in infos)
            {
                if (item is FileInfo e1)
                {
                    files.Add(e1);
                }
                else if(item is DirectoryInfo e2)
                {
                    files.AddRange( e2.GetFiles("*", SearchOption.AllDirectories));
                    
                }
            }
            foreach (var info in files)
            {
                //目标文件路径.
                ExtendedPath epath = new ExtendedPath() { Root = "", Middle = @"SoftWare\" + SoftWareName + @"\Update\" + Version + @"\", Last = Tools.GetRelativePath(info.FullName, Root) };
                
            }
        }

        bool CheckHasDownload()
        {
            if (File.Exists(UpdateCache + "UpdateInfo.txt"))
            {
                string s = File.ReadAllLines(UpdateCache + "UpdateInfo.txt")[0];
                if (Version.TryParse(s, out Version v))
                {
                    if (v >= Version)
                    {
                        CheckUpdateCompleted?.Invoke(this, new CheckUpdateEventArgs(ChannelState.Completed, UpdateType.Download, s, 0));
                        ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Completed));
                        return true;
                    }
                }
            }
            return false;
        }
        public DownloadTask[] GetTask()
        {
            return service.GetUpdateTask(SoftWareName, Version.ToString());
        }
    }

    public delegate void CheckUpdateEventHandler(object sender, CheckUpdateEventArgs e);
    public class CheckUpdateEventArgs : EventArgs
    {
        public CheckUpdateEventArgs(ChannelState channelState)
        {
            ChannelState = channelState;
        }
        public CheckUpdateEventArgs(ChannelState channelState, UpdateType updateType, string version, long length)
        {
            ChannelState = channelState;
            UpdateType = updateType;
            Version = version;
            Length = length;
        }

        public ChannelState ChannelState { get; private set; } = ChannelState.Completed;
        public UpdateType UpdateType { get; private set; }
        public string Version { get; private set; }
        public long Length { get; private set; }
    }
    public delegate void ChannelFreshEventHandler(object sender, ChannelFreshEventArgs e);
    public class ChannelFreshEventArgs : EventArgs
    {
        public ChannelFreshEventArgs(ChannelState channelState)
        {
            ChannelState = channelState;
        }

        public ChannelFreshEventArgs(ChannelState channelState, long location, long length)
        {
            ChannelState = channelState;
            Location = location;
            Length = length;
        }

        public ChannelState ChannelState { get; private set; }
        public long Location { get; set; }
        public long Length { get; private set; }
        public double Percent => (int)((double)Location / Length * 1000) / 10.0;
    }
    public enum ChannelState
    {
        Doing,
        DoingOfUpdate,
        Failed,
        FileFailed,
        Completed,
    }
}
