using System;
using System.Collections.Generic;
using System.Globalization;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    internal static class BlogPostMetadataProvider
    {
        private static readonly DateTimeFormatInfo InvariantFormat = CultureInfo.InvariantCulture.DateTimeFormat;
        private static readonly char[] PathSeparators = { '/', '\\' };
        private static readonly string[] TimeFormats = { "HH:mm:ss", "HH:mm" };
        private static readonly string[] DateFormats = { "yyyyMMdd", "yyyy-MM-dd", "yyyy/MM/dd" };

        public static bool TryParse(IPage page, BlogCompilationReport report, out DateTime date, out string slug)
        {
            date = default(DateTime);
            slug = null;

            // Expected page path in one of the following formats:
            // /YYYY/MM/DD
            // /YYYY/MM/DD/SLUG
            // /YYYY/MMM/DD
            // /YYYY/MMM/DD/SLUG

            var parts = page.Id.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3 || parts.Length > 4)
            {
                report.AddWarning($"Page \"{page.Id}\" skipped - wrong path format");
                return false;
            }

            if (!int.TryParse(parts[0], out var year) || year < 0)
            {
                report.AddWarning($"Page \"{page.Id}\" skipped - \"{parts[0]}\" is not a valid year");
                return false;
            }

            if (!int.TryParse(parts[1], out var month))
            {
                IEnumerable<string[]> EnumerateMonthNames()
                {
                    yield return InvariantFormat.AbbreviatedMonthNames;
                    yield return InvariantFormat.MonthNames;
                    yield return InvariantFormat.AbbreviatedMonthGenitiveNames;
                    yield return InvariantFormat.MonthGenitiveNames;
                }

                foreach (var array in EnumerateMonthNames())
                {
                    month = Array.FindIndex(array, s => StringComparer.OrdinalIgnoreCase.Equals(s, parts[1]));
                    if (month >= 0)
                    {
                        break;
                    }
                }

                if (month >= 0)
                {
                    month++;
                }
            }

            if (month <= 0 || month > 12)
            {
                report.AddWarning($"Page \"{page.Id}\" skipped - \"{parts[1]}\" is not a valid month");
                return false;
            }

            if (int.TryParse(parts[2], out var day))
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);
                if (day > 0 && day <= daysInMonth)
                {
                    var timeStr = page.Metadata.GetString("time");
                    if (string.IsNullOrEmpty(timeStr) ||
                       !DateTime.TryParseExact(timeStr, TimeFormats, InvariantFormat, DateTimeStyles.None, out var time))
                    {
                        time = new DateTime(1970, 1, 1, 0, 0, 0);
                    }
                    
                    slug = page.Metadata.GetString("slug");
                    if (string.IsNullOrEmpty(slug))
                    {
                        slug = parts.Length > 3 ? parts[3] : "";
                    }

                    var dateStr = page.Metadata.GetString("date");
                    if (!string.IsNullOrEmpty(dateStr) &&
                        DateTime.TryParseExact(dateStr, DateFormats, InvariantFormat, DateTimeStyles.None, out var dateOverride))
                    {
                        year = dateOverride.Year;
                        month = dateOverride.Month;
                        day = dateOverride.Day;
                    }

                    date = new DateTime(year, month, day, time.Hour, time.Minute, time.Second, DateTimeKind.Utc);
                    return true;
                }
            }

            report.AddWarning($"Page \"{page.Id}\" skipped - \"{parts[2]}\" is not a valid day of {year}/{month}");
            return false;
        }
    }
}