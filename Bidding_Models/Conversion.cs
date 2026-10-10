
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
namespace Bidding_Models
{
    public static class Conversion
    {

        /*DataTable to List conversion*/
        // List<Student> newStudents = studentTbl.ToList<Student>();
        // List<User> UserList = JsonConvert.DeserializeObject<List<User>>(jsonString);
        // Student newStudents = studentTbl.GetItem<Student>(dr);
        public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new
                                 {
                                     Name = aProp.Name,
                                     Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                                 }).ToList();
            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new
                                     {
                                         Name = aHeader.ColumnName,
                                         Type = aHeader.DataType
                                     }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfo = aTSource.GetType().GetProperty(aField.Name);

                    if ((propertyInfo.PropertyType == typeof(Int16) || propertyInfo.PropertyType == typeof(Int16?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Int16() : ToInt16(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Int32) || propertyInfo.PropertyType == typeof(Int32?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Int32() : ToInt32(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Int64) || propertyInfo.PropertyType == typeof(Int64?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Int64() : ToInt64(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Decimal) || propertyInfo.PropertyType == typeof(Decimal?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Decimal() : ToDecimal(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Double) || propertyInfo.PropertyType == typeof(Double?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Double() : ToDouble(dataRow[aField.Name]);

                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?)))
                    {
                        var value = dataRow[aField.Name] ?? ToDateTime(dataRow[aField.Name]);
                        value = value == DBNull.Value ? null : value;
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Boolean) || propertyInfo.PropertyType == typeof(Boolean?)))
                    {
                        var value = dataRow[aField.Name] ?? ToBool(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(byte[]))
                    {
                        var value = dataRow[aField.Name] ?? ToByteArray(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);

                    }
                    else
                    {

                        var value = dataRow[aField.Name] ?? ToString(dataRow[aField.Name]);
                        value = value == DBNull.Value ? String.Empty : value;
                        propertyInfo.SetValue(aTSource, value, null);
                    }

                }
                dataList.Add(aTSource);
            }
            return dataList;
        }
        public static List<TSource> ToList<TSource>(this string jsonString)
        {
            var dataList = JsonConvert.DeserializeObject<List<TSource>>(jsonString);
            return dataList;
        }
        public static TSource ToObject<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new
                                 {
                                     Name = aProp.Name,
                                     Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? (aProp.PropertyType.IsEnum ? aProp.PropertyType.GetEnumUnderlyingType() : aProp.PropertyType)
                                 }).ToList();
            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new
                                     {
                                         Name = aHeader.ColumnName,
                                         Type = aHeader.DataType
                                     }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();


            var aTSource = new TSource();
            if (dataTable.Rows.Count > 0)
            {
                DataRow dataRow = dataTable.Rows[0];
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfo = aTSource.GetType().GetProperty(aField.Name);

                    if ((propertyInfo.PropertyType == typeof(Int16) || propertyInfo.PropertyType == typeof(Int16?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Int16() : ToInt16(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Int32) || propertyInfo.PropertyType == typeof(Int32?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Int32() : ToInt32(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Int64) || propertyInfo.PropertyType == typeof(Int64?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Int64() : ToInt64(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Decimal) || propertyInfo.PropertyType == typeof(Decimal?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Decimal() : ToDecimal(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Double) || propertyInfo.PropertyType == typeof(Double?)))
                    {
                        var value = (dataRow[aField.Name] == DBNull.Value) ? new Double() : ToDouble(dataRow[aField.Name]);

                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?)))
                    {
                        var value = dataRow[aField.Name] ?? ToDateTime(dataRow[aField.Name]);
                        value = value == DBNull.Value ? null : value;
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(Boolean) || propertyInfo.PropertyType == typeof(Boolean?)))
                    {
                        var value = dataRow[aField.Name] ?? ToBool(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(byte[]))
                    {
                        var value = dataRow[aField.Name] ?? ToByteArray(dataRow[aField.Name]);
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                    else
                    {
                        var value = dataRow[aField.Name] ?? ToString(dataRow[aField.Name]);
                        value = value == DBNull.Value ? String.Empty : value;
                        propertyInfo.SetValue(aTSource, value, null);
                    }
                }
            }
            else
            {
                return default(TSource);
            }
            return aTSource;
        }
        public static DataTable ToDataTable<TSource>(this IList<TSource> data)
        {
            DataTable dataTable = new DataTable(typeof(TSource).Name);
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                    prop.PropertyType);
            }

            foreach (TSource item in data)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public static DataTable ToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }
        public static DataSet ToDataSet(string jsonString)
        {
            return JsonConvert.DeserializeObject<DataSet>(jsonString);
        }
        //public static TSource ToObject<TSource>(this DataTable dataTable) where TSource : new()
        //{
        //    var aTSource = new TSource();

        //    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        //    var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
        //                         select new
        //                         {
        //                             Name = aProp.Name
        //                             //Type = Nullable.GetUnderlyingType(aProp.PropertyType) ??
        //                             // aProp.PropertyType
        //                         }).ToList();
        //    var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
        //                             select new
        //                             {
        //                                 Name = aHeader.ColumnName
        //                                 //Type = aHeader.DataType
        //                             }).ToList();
        //    var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();
        //    if (dataTable.Rows.Count > 0)
        //    {
        //        DataRow dataRow = dataTable.Rows[0];
        //        aTSource = new TSource();
        //        foreach (var aField in commonFields)
        //        {
        //            PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.Name);

        //            var value = (dataRow[aField.Name] == DBNull.Value) ?
        //            null : dataRow[aField.Name].ToString(); //if database field is nullable


        //            propertyInfos.SetValue(aTSource, value, null);
        //        }
        //    }

        //    return aTSource;
        //}
        public static T ToObject<T>(string jsonString)
        {
            Type temp = typeof(T);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        public static string ToJson(Object o)
        {
            return JsonConvert.SerializeObject(o);


        }
        public static string ToJson(DataSet ds)
        {
            return JsonConvert.SerializeObject(ds, Formatting.Indented);
        }
        public static string ToJson(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }
        public static DataTable ToPivotTable<T, TColumn, TRow, TData>(
                                this IEnumerable<T> source,
                                Func<T, TColumn> columnSelector,
                                Expression<Func<T, TRow>> rowSelector,
                                Func<IEnumerable<T>, TData> dataSelector)
        {
            DataTable table = new DataTable();
            var rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            table.Columns.Add(new DataColumn(rowName));
            var columns = source.Select(columnSelector).Distinct();

            foreach (var column in columns)
                table.Columns.Add(new DataColumn(column.ToString()));

            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             });

            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                var items = row.Values.Cast<object>().ToList();
                items.Insert(0, row.Key);
                dataRow.ItemArray = items.ToArray();
                table.Rows.Add(dataRow);
            }

            return table;
        }
        public static bool ToBool(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToBoolean(value);
                else
                    return false;
            }
            catch (Exception)
            {
                try
                {
                    if (value != null && value != DBNull.Value && value != string.Empty)
                        return Convert.ToBoolean(Convert.ToInt32(value));
                    else
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }
        public static Guid ToGuid(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Guid.Parse(Convert.ToString(value));
                else
                    return Guid.Empty;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }

        }
        public static string ToString(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToString(value);
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        public static TimeSpan ToTimeSpan(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return (TimeSpan)value;
                else
                    return new TimeSpan(0, 0, 0);
            }
            catch (Exception)
            {
                return new TimeSpan(0, 0, 0);
            }

        }
        public static int ToInt(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToInt32(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static Int16 ToInt16(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToInt16(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static Int32 ToInt32(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToInt32(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static short ToShort(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToInt16(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static Int64 ToInt64(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToInt64(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static Decimal ToDecimal(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToDecimal(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static double ToDouble(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToDouble(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static float ToSingle(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToSingle(value);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static DateTime ToDateTime(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Convert.ToDateTime(value);
                else

                    return default(DateTime);

            }
            catch (Exception)
            {
                return default(DateTime);
            }

        }
        public static DateTime ToDateTimeFromUnix(object unixTimeStamp)
        {
            if (unixTimeStamp != null && unixTimeStamp != DBNull.Value && unixTimeStamp != string.Empty)
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddMilliseconds(Convert.ToDouble(unixTimeStamp));
                return dtDateTime.ToLocalTime();
            }
            else
            {
                return default(DateTime);
            }
            // Unix timestamp is seconds past epoch

        }
        public static string ToHash(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return value.ToString();
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        public static Byte[] ToByteArray(object value)
        {
            try
            {
                if (value != null && value != DBNull.Value && value != string.Empty)
                    return Encoding.ASCII.GetBytes(value.ToString());
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //public static Byte[] ToByteArray(object value)
        //{
        //    try
        //    {
        //        if (value != null && value != DBNull.Value && value != string.Empty)
        //            return (Byte[])(value);
        //        else
        //            return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}
        public static string OppositeDateTimeConvert(string yyyymmdd)
        {
            try
            {
                if (yyyymmdd.Length >= 14)
                {
                    yyyymmdd = yyyymmdd.Substring(4, 2) + "/" + yyyymmdd.Substring(6, 2) + "/" + yyyymmdd.Substring(0, 4) + " " + yyyymmdd.Substring(8, 2) + ":" + yyyymmdd.Substring(10, 2) + ":" + yyyymmdd.Substring(12, 2);
                }
                else if (yyyymmdd.Length >= 12)
                {
                    yyyymmdd = yyyymmdd.Substring(4, 2) + "/" + yyyymmdd.Substring(6, 2) + "/" + yyyymmdd.Substring(0, 4) + " " + yyyymmdd.Substring(8, 2) + ":" + yyyymmdd.Substring(10, 2);//+ ":" + yyyymmdd.Substring(12, 2);
                }
                else if (yyyymmdd.Length == 8)
                {
                    yyyymmdd = yyyymmdd.Substring(4, 2) + "/" + yyyymmdd.Substring(6, 2) + "/" + yyyymmdd.Substring(0, 4);
                }

                return yyyymmdd;
            }
            catch (Exception ex)
            {
                return "";
            }

            return yyyymmdd;
        }
        public static string DateDifference(DateTime? ToDate, DateTime? FromDate)
        {
            string diff = String.Empty;
            if (ToDate != null && ToDate != DateTime.MinValue && FromDate != null && FromDate != DateTime.MinValue)
            {
                TimeSpan difference = Conversion.ToDateTime(ToDate) - Conversion.ToDateTime(FromDate);
                diff = difference.Days + " day(s) " + difference.Hours + " hr(s) " + difference.Minutes + " min(s)";
            }

            return diff;

        }
    }
}
