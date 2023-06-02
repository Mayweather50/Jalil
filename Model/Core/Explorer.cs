using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySQLConnect.Core
{
    public static class Explorer
    {
        public static async Task<string[]> OpenExplorer(Window parentWindow, string extensions)
        {
            List<FileDialogFilter> _filters = new List<FileDialogFilter>() { new FileDialogFilter() };
            _filters[0].Extensions.Add(extensions);
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filters = _filters };
            if (openFileDialog.ShowAsync(parentWindow).Result == null) Console.WriteLine("NULL");
            return openFileDialog.ShowAsync(parentWindow).Result;
        }
    }
}