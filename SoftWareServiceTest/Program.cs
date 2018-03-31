using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.SoftWare.Service;

namespace SoftWareServiceTest
{
    class Program
    {
        static string code = "";
        static void Main(string[] args)
        {
            SoftWareService softWareService = new SoftWareService(Version.Parse("1.0.0.0"), "Edit_Community");
            softWareService.CheckUpdateCompleted += SoftWareService_CheckUpdateCompleted;
            softWareService.ChannelFreshed += SoftWareService_ChannelFreshed;
            while (!IsExitCode(Console.ReadLine()))
            {
                if (code=="check")
                {
                    softWareService.CheckUpdate();
                }
                else if (code =="download")
                {
                    softWareService.DownloadUpdate();
                }
            }
        }

        private static void SoftWareService_ChannelFreshed(object sender, ChannelFreshEventArgs e)
        {
            Console.WriteLine("channel:{0},{1}%",e.ChannelState,e.Percent);
        }

        static bool IsExitCode(string e)
        {
            code = e.ToLower();
            if (code == "exit")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void SoftWareService_CheckUpdateCompleted(object sender, CheckUpdateEventArgs e)
        {
            Console.WriteLine("check:{0},{1},{2}", e.ChannelState,e.UpdateType,e.Length);
        }
    }
}
