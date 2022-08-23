using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TermInjectorPlus
{
    public class HelperFunctions
    {
        public static string GetLocalAppDataPath(string restOfPath)
        {
            var termInjectorDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    TermInjectorPlusSettings.Default.TermInjectorDir);
            if (!Directory.Exists(termInjectorDir))
            {
                Directory.CreateDirectory(termInjectorDir);
            }

            if (restOfPath == null)
            {
                return termInjectorDir;
            }
            else
            {
                return Path.Combine(
                    termInjectorDir,
                    restOfPath);
            }
        }

        
    }
}
