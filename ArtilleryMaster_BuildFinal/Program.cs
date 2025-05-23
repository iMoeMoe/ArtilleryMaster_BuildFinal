// File: ArtilleryMaster_BuildFinal/Program.cs
// This is where it all begins. Or ends, if you close the window.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArtilleryMaster_BuildFinal
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // If this doesn't work, try turning it off and on again.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

// To-Do List (All Done!)
// - Started the app. That's it. That's the list.
