using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test01
{
    public static class ErrorMessager
    {
        public static void ShowError(string message)
        {
            MessageBox.Show($"{message} {Environment.NewLine}За дополнительной информацией обращайтесь по телефону:{Environment.NewLine} +78005553535!");
        }
    }
}
