using Bidding_Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Bidding_DLL.Common
{
    public static class CommonMethods
    {
        private static Type GetTypeOfProperty<T>(string FieldName)
        {

            PropertyInfo property = typeof(T).GetProperties(BindingFlags.Instance |
                       BindingFlags.Static |
                       BindingFlags.NonPublic |
                       BindingFlags.Public).ToList().Where(p => p.Name == FieldName).FirstOrDefault();
            return property.PropertyType;
        }

        public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, List<DatatableSearchParams> searchParams)
        {
            if (searchParams?.Count > 0)
            {
                string filter = string.Empty;
                for (int i = 0; i < searchParams.Count; i++)
                {
                    if (i == 0)
                    {
                        if (searchParams[i].Value != null)
                        {
                            if (CommonMethods.GetTypeOfProperty<T>(searchParams[i].Name) == typeof(Boolean))
                            {

                                filter += searchParams[i].Name + " == " + Convert.ToBoolean(searchParams[i].Value);


                            }
                            else if (CommonMethods.GetTypeOfProperty<T>(searchParams[i].Name) == typeof(Int32))
                            {

                                filter += searchParams[i].Name + " == " + Convert.ToBoolean(searchParams[i].Value);

                            }
                            else if (CommonMethods.GetTypeOfProperty<T>(searchParams[i].Name) == typeof(String))
                            {

                                filter += searchParams[i].Name + ".Contains(\"" + searchParams[i].Value + "\")";

                            }
                            else if (CommonMethods.GetTypeOfProperty<T>(searchParams[i].Name) == typeof(DateTime))
                            {

                                filter += searchParams[i].Name + " == " + Convert.ToDateTime(searchParams[i].Value);

                            }
                        }

                    }
                    else
                    {
                        if (searchParams[i].Value != null)
                        {
                            if (CommonMethods.GetTypeOfProperty<T>(searchParams[i].Name) == typeof(Boolean))
                            {

                                filter += " && " + searchParams[i].Name + " == " + Convert.ToBoolean(searchParams[i].Value);

                            }
                            else if (CommonMethods.GetTypeOfProperty<T>(searchParams[i].Name) == typeof(Int32))
                            {

                                filter += " && " + searchParams[i].Name + " == " + Convert.ToBoolean(searchParams[i].Value);

                            }
                            else if (CommonMethods.GetTypeOfProperty<T>(searchParams[i].Name) == typeof(String))
                            {

                                filter += " && " + searchParams[i].Name + ".Contains(\"" + searchParams[i].Value + "\")";

                            }
                        }


                    }
                }

                if (!String.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
            }

            return query;
        }

        public static IQueryable<T> ApplySort<T>(IQueryable<T> query, string SortBy, string Direction)
        {
            if (!String.IsNullOrEmpty(SortBy))
            {
                string orderby = SortBy + " " + Direction;
                query = query.OrderBy(orderby);
            }

            return query;
        }

        public static List<string> DeserializeString(string jsonstring)
        {

            try
            {
                List<string> list = JsonConvert.DeserializeObject<List<string>>(jsonstring);
                return list;
            }
            catch
            {
                List <string> list = new List<string>();
               list.Add(jsonstring);
                return list;
            }



        }

        public static string SerializeString(object str)
        {

            try
            {
                if (str != null)
                {
                   return JsonConvert.SerializeObject(str);
                }
             
                return String.Empty;
            }
            catch
            {
                return String.Empty;
            }



        }

        //public static PagedResult<T> ExecuteQuery<T>(this IOrderedQueryable<T> query, ref PagedResultBase model)
        //{

        //    PagedResult<T> result = new PagedResult<T>();
        //    result.CurrentPage = model.CurrentPage;
        //    result.RowCount = query.Count();
        //    var pageCount = (double)result.RowCount / model.PageSize;
        //    result.PageCount = (int)Math.Ceiling(pageCount);


        //    var skip = (model.CurrentPage - 1) * model.PageSize;
        //    result.Results = query.Skip(skip).Take(model.PageSize).ToList();

        //    return result;


        //}
    }
}
