using MySQLConnect.ViewModel;
using Avalonia.Controls;
using MySQLConnect.Model.Core;

namespace MySQLConnect.View;

public partial class Editor : Window
{
    public Editor()
    {
        InitializeComponent();
        DataContext = new EditorVM();
        EditorHandler.root = Root;
        EditorHandler.win = this;
        PointerPressed += EditorHandler.PointerPressed;
        PointerReleased += EditorHandler.PointerReleased;
        PointerMoved += EditorHandler.PointerMoved;
        this.Opened += (s, e) =>
        {
            EditorHandler.WeekGenerate();
        };
        this.Closed += (s, e) =>
        {
            Tabletime.ChangeTabletime();
            Tabletime.OutputChangedWeek();
        };
    }
}