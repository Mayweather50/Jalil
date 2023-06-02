using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MySQLConnect.Model.Core
{
    public static class WordReader
    {
        static private Table table;
        static private string title;
        static public List<string> Read(string path)
        {
            List<string> data = new List<string>();
            object miss = Missing.Value;
            Application app = new Application();
            Document doc = app.Documents.Open(path, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss,
                ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            foreach (Table item in doc.Content.Tables)
            {
                if (item.Cell(1, 1).Range.Text.Contains("Наименование"))
                {
                    table = item;
                    break;
                }
            }
            if (table == null) return new List<string>(); // DEBUG
            for (int r = 3; r <= table.Rows.Count - 1; r++)
            {
                string inputRow = "";
                bool finalRow = false;
                for (int c = 1; c < table.Columns.Count && !finalRow; c++)
                {
                    try { title = table.Cell(r, c).Range.Text.Replace("\r\a", "").Trim(); }
                    catch (COMException ex)
                    {
                        Console.WriteLine($"ROW: {r} " + ex.Message);
                        continue;
                    }
                    finally
                    {
                        if (title.StartsWith("Прак") || title.StartsWith("Содерж") || title.StartsWith("Лабора") || title.StartsWith("Дифф")) inputRow += title;
                        else if (inputRow != "" && int.TryParse(title, out int a))
                        {
                            inputRow += $" = {a}";
                            finalRow = true; // Необходимо, вместо continue
                        }
                    }
                }
                if (inputRow != "") data.Add(inputRow);
            }
            try { doc.Close(); } catch (COMException ex) { Console.WriteLine(ex.Message); }
            app.Quit();
            SaveData(data);
            return data;
        }
        private async static void SaveData(List<string> data)
        {
            using (FileStream fs = new FileStream("programs.json", FileMode.OpenOrCreate))
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                };
                await JsonSerializer.SerializeAsync(fs, data, options);
                fs.Dispose();
            }
        }
    }
}