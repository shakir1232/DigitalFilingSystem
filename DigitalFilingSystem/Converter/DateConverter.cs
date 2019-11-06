using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalFilingSystem.Converter
{
    public static class DateConverter
    {
        /// <summary>
        /// Get Local ZoneDate
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLocalZoneDate(DateTime date)
        {
            TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(date, BdZone);
            return localDateTime;
        }
    }
}