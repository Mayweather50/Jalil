using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace MySQLConnect.Model
{
    static class CRUDRows
    {
        public static int[] AddRow(Window win, Grid grid, List<ComboBoxItem> subjList)
        {
            double heightWin = win.Height + 50;
            if (heightWin >= Config.rootWindow.Height) return null;
            win.Height = heightWin;
            grid.RowDefinitions.Add(new RowDefinition());
            ComboBox box = new ComboBox() { Items = subjList, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
            grid.Children.Add(box);
            Grid.SetColumn(box, 1);
            Grid.SetRow(box, grid.RowDefinitions.Count - 3);
            int[] arr = new int[2];
            arr[0] = grid.RowDefinitions.Count - 2;
            arr[1] = grid.RowDefinitions.Count - 1;
            return arr;
        }
        public static int[] RemoveRow(Window win, Grid grid)
        {
            double heightWin = win.Height - 50;
            if (heightWin < 450) return null;
            win.Height = heightWin;
            grid.Children.RemoveAt(grid.Children.Count - 1);
            grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 3);
            int[] arr = new int[2];
            arr[0] = grid.RowDefinitions.Count - 2;
            arr[1] = grid.RowDefinitions.Count - 1;
            return arr;
        }
    }
}
