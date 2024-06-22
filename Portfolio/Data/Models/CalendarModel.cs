using System;
using System.Collections.Generic;

namespace Portfolio.Models
{
    public class CalendarModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<DateTime> Days { get; set; }

        public CalendarModel(int year, int month)
        {
            Year = year;
            Month = month;
            Days = new List<DateTime>();

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Заполнение предыдущих дней для полного отображения недель
            int daysBefore = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;
            for (int i = 0; i < daysBefore; i++)
            {
                Days.Add(firstDayOfMonth.AddDays(-daysBefore + i));
            }

            // Заполнение дней текущего месяца
            for (int i = 0; i < daysInMonth; i++)
            {
                Days.Add(firstDayOfMonth.AddDays(i));
            }

            // Заполнение следующих дней для полного отображения недель
            int daysAfter = 7 - (Days.Count % 7);
            if (daysAfter < 7)
            {
                for (int i = 0; i < daysAfter; i++)
                {
                    Days.Add(firstDayOfMonth.AddDays(daysInMonth + i));
                }
            }
        }
    }
}
