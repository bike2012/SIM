using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//using PylonC.NET;


namespace SIMproject
{
    static class Program
    {
        [DllImport("SIMprojectDLL.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")] 
        public static extern void test(int width, int height, int size, [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=3)] byte[] data, ref int resultNum, 
            [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=5)] int[] resultX, [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=5)] int[] resultY,
            [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=5)] int[] resultRX, [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=5)] int[] resultRY,
            int maxr,int minr,int step,int thr);





        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new form());
        }
    }
}
