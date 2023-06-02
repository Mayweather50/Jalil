using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace MySQLConnect.Model.Core
{
    public static class EditorHandler
    {
        public static Window win;
        public static Canvas root;
        public static List<List<Border>> posBusy = new List<List<Border>>(); // Всевозможные позиции для предметов
        public static BrushConverter converter = new BrushConverter(); // Конвертер цвета
        public static List<List<Lesson>> thisWeek = new List<List<Lesson>>();

        public static void WeekGenerate() // Генерация недели в окне редактирования
        {
            posBusy.Clear();
            double width = win.Width / 6;
            double height = win.Height / 6;
            for (int day = 0; day < 6; day++)
            {
                posBusy.Add(new List<Border>());
                for (int para = 0; para < thisWeek[day].Count; para++)
                {
                    Border border = new Border()
                    {
                        Background = converter.ConvertFromString("White") as IBrush,
                        BorderBrush = converter.ConvertFromString("black") as IBrush,
                        BorderThickness = new Thickness(2, 2),
                        Width = width,
                        Height = height
                    };
                    Label label = new Label()
                    {
                        Content = $"{thisWeek[day][para].Subject}",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    };
                    border.Child = label;
                    border.ZIndex = 1;
                    border.Margin = new Thickness(width * day, height * para);
                    root.Children.Add(border);
                    posBusy[day].Add(border);
                }
            }
        }
        #region Drag&Drop
        private static double modifyX; //Модификатор для позиции объекта при перемещении
        private static double modifyY; //Модификатор для позиции объекта при перемещении
        private static Border capturedObj; // Захваченный объект
        private static Thickness prevPos; // Предыдущая позиция взятого объекта

        public static void PointerPressed(object? sender, PointerPressedEventArgs e) // Когда кнопка мыши нажата
        {
            var point = e.GetCurrentPoint(root);
            if (point.Properties.IsLeftButtonPressed && e.Source is Border) // Если кнопка мыши нажата и объект соответствует типу
            {
                capturedObj = (Border)e.Source; // Присваивание объекта для "глобальной" переменной
                capturedObj.Background = converter.ConvertFromString("#4aabff") as IBrush;
                prevPos = capturedObj.Margin;
                capturedObj.ZIndex = 2; // Возвышение объекта поверх других
                modifyX = point.Position.X - capturedObj.Bounds.Position.X; // Чтоб текст не "прилипал" левой частью к курсору
                modifyY = point.Position.Y - capturedObj.Bounds.Position.Y; // Чтоб текст не "прилипал" верхней частью к курсору
            }
        }
        public static void PointerMoved(object? sender, PointerEventArgs e) // Когда курсор передвигается
        {
            if (capturedObj != null)
            {
                Point point = e.GetPosition(root);
                double x = point.X;
                double y = point.Y;
                capturedObj.Margin = new Thickness(x - modifyX, y - modifyY, 0, 0);
            }
        }
        public static void PointerReleased(object? sender, PointerReleasedEventArgs e) // Когда кнопка мыши отпущена
        {
            if (capturedObj != null)
            {
                Border newPos = new Border();
                float prevDistance = float.MaxValue; // редактировать стандартное значение
                int _para = 0, _day = 0;
                int _thisDay = 0, _thisPara = 0;
                for(int day = 0; day < 6; day++)
                {
                    for (int para = 0; para < posBusy[day].Count; para++)
                    {
                        float distance = MathF.Sqrt(MathF.Pow((float)(capturedObj.Margin.Left - posBusy[day][para].Margin.Left), 2)
                            + MathF.Pow((float)(capturedObj.Margin.Top - posBusy[day][para].Margin.Top), 2)); // Вычисление расстояния между двумя точками
                        if (distance < prevDistance && posBusy[day][para] != capturedObj) // Проверка меньше ли расстояние
                        {
                            prevDistance = distance;
                            newPos = posBusy[day][para];
                            _para = para;
                            _day = day;
                        }
                        else if(posBusy[day][para] == capturedObj)
                        {
                            _thisPara = para;
                            _thisDay = day;
                        }
                    }
                }
                capturedObj.Background = converter.ConvertFromString("white") as IBrush;
                // Замена в основном массиве текущей недели
                Lesson oldPara = thisWeek[_day][_para];
                Lesson selecPara = thisWeek[_thisDay][_thisPara];
                selecPara.Day += _day - selecPara.Day;
                oldPara.Day += _thisDay - oldPara.Day;
                thisWeek[_day][_para] = selecPara; // Выбранная пара переставляется на новое место
                thisWeek[_thisDay][_thisPara] = oldPara; // Старая пара переставляется на старое место предыдущей пары

                capturedObj.Margin = newPos.Margin; // Установка border'a из старой позиции в новую
                newPos.Margin = prevPos; // Установка border'a из новой позиции в старую
                // Замена в массиве border'ов
                posBusy[_day][_para] = capturedObj;
                posBusy[_thisDay][_thisPara] = newPos;
                capturedObj.ZIndex = 1; // Возвращение объекта на стандартный слой
                capturedObj = null;
            }
        }
        #endregion
    }
}
