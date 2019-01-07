using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Individual_Project
{
    class Program
    {
        /// <summary>
        /// The main function of the application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            App messenger = new App();
            messenger.Menu();
        }
    }
}


