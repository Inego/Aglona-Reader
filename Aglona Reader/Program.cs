﻿using System;
using System.Windows.Forms;

namespace AglonaReader
{
    // ReSharper disable once UnusedType.Global
    internal static class Program
    {
        [STAThread]
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
