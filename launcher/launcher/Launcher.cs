using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace launcher
{
    class Launcher
    {
        //Add analytics to this - eventually

        public static void launch_synthesis()
        {
            try
            {
                Process.Start("Synthesis/Synthesis.exe");
            }catch(Exception e)
            {
                //Add popup and print e or friendly ux version
            }
        }

        public static void launch_emulator()
        {
            
        }

        public static void open_website()
        {
            Process.Start("http://bxd.autodesk.com/");
        }

        public static void open_tutorials()
        {
            Process.Start("http://bxd.autodesk.com/tutorials.html");
        }
    }
}
