using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;


    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static DateTime RoundSecond(this DateTime dt)
        {
            return dt.AddMilliseconds(-dt.Millisecond);
        }

        public static TimeSpan RoundSecond(this TimeSpan sp)
        {
            //return sp;
            return TimeSpan.FromSeconds(Math.Floor(sp.TotalSeconds));
        }

        public static string ToCSV(this DataTable dt)
        {
            StringBuilder strbud = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                    strbud.Append(dr[0].ToString() + ",");
                strbud.Append(Environment.NewLine);
            }
            return strbud.ToString();
        }

        public static bool IsBetween<T>(this T t, T lowerBound, T upperBound, bool includeLowerBound = false, bool includeUpperBound = false) where T : IComparable<T>
        {
            if (t == null) throw new ArgumentNullException("t");

            var lowerCompareResult = t.CompareTo(lowerBound);
            var upperCompareResult = t.CompareTo(upperBound);

            return (includeLowerBound && lowerCompareResult == 0) ||
                (includeUpperBound && upperCompareResult == 0) ||
                (lowerCompareResult > 0 && upperCompareResult < 0);
        }
        public static object ExClone(this object obj)
        {
            BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            MemoryStream stream = new MemoryStream();
            Formatter.Serialize(stream, obj);
            stream.Position = 0;
            object clonedObj = Formatter.Deserialize(stream);
            stream.Close();
            return clonedObj;
        }
    }
    public class ListItem
    {
        private string _thisValue = "";
        private string _thisText = "";
        private string _thisState = "";


        public ListItem(string text, string value)
        {
            this.ThisText = text;
            this.ThisValue = value;
        }

        public ListItem(string text, string value, string state)
        {
            this.ThisText = text;
            this.ThisValue = value;
            this.ThisState = state;
        }

        public string ThisText
        {
            get { return _thisText; }
            set { _thisText = value; }
        }

        public string ThisValue
        {
            get { return _thisValue; }
            set { _thisValue = value; }
        }

        public string ThisState
        {
            get { return _thisState; }
            set { _thisState = value; }
        }

        public override string ToString()
        {
            return this.ThisText.ToString();
        }
    }

    public static class LINQHelper
    {

        public static T FirstOrNullObject<T>(this IEnumerable<T> enumerable, Func<T, bool> func, T nullObject)
        {
            var val = enumerable.FirstOrDefault<T>(func);
            if (val == null)
            {
                val = nullObject;
            }

            return val;
        }

        public static T FirstOrNullObject<T>(this IEnumerable<T> enumerable, T nullObject)
        {
            var val = enumerable.FirstOrDefault<T>();
            if (val == null)
            {
                val = nullObject;
            }
            return val;
        }

        public static T FirstOrNew<T>(this IEnumerable<T> enumerable, Func<T, bool> func, T newObject)
        {
            return enumerable.FirstOrNullObject<T>(func, newObject);
        }

        public static T FirstOrNew<T>(this IEnumerable<T> enumerable, T newObject)
        {
            return enumerable.FirstOrNullObject<T>(newObject);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> func, T defaultObject)
        {
            return enumerable.FirstOrNullObject<T>(func, defaultObject);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, T defaultObject)
        {
            return enumerable.FirstOrNullObject<T>(defaultObject);
        }

        public static T LastOrNullObject<T>(this IEnumerable<T> enumerable, Func<T, bool> func, T nullObject)
        {
            var val = enumerable.LastOrDefault<T>(func);
            if (val == null)
            {
                val = nullObject;
            }

            return val;
        }

        public static T LastOrNullObject<T>(this IEnumerable<T> enumerable, T nullObject)
        {
            var val = enumerable.LastOrDefault<T>();
            if (val == null)
            {
                val = nullObject;
            }
            return val;
        }

        public static T LastOrNew<T>(this IEnumerable<T> enumerable, Func<T, bool> func, T newObject)
        {
            return enumerable.LastOrNullObject<T>(func, newObject);
        }

        public static T LastOrNew<T>(this IEnumerable<T> enumerable, T newObject)
        {
            return enumerable.LastOrNullObject<T>(newObject);
        }

        public static T LastOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> func, T defaultObject)
        {
            return enumerable.LastOrNullObject<T>(func, defaultObject);
        }

        public static T LastOrDefault<T>(this IEnumerable<T> enumerable, T defaultObject)
        {
            return enumerable.LastOrNullObject<T>(defaultObject);
        }

    }

