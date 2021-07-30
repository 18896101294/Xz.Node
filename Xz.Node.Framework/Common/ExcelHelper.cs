using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xz.Node.Framework.Common
{
    /// <summary>
    /// Excel 帮助类
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// 生成一个只有Header的excel，用于导入模板
        /// </summary>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static byte[] CreateEmptyExcel(params IList<string>[] columnNames)
        {
            if (columnNames == null || columnNames.Length == 0)
                return null;
            var ep = new OfficeOpenXml.ExcelPackage();
            var wb = ep.Workbook;
            for (int i = 0; i < columnNames.Length; i++)
            {
                var ws = wb.Worksheets.Add($"sheet{i + 1}");
                //写数据  
                int colIndex = 1;
                foreach (string columnName in columnNames[i])
                {
                    ws.Cells[1, colIndex++].Value = columnName;
                }
            }
            if (wb.Worksheets.Count == 0)
                wb.Worksheets.Add($"sheet1");
            return ep.GetAsByteArray();
        }

        public static byte[] CreateEmptyExcel(string[] sheetName, params IList<string>[] columnNames)
        {
            if (columnNames == null || columnNames.Length == 0)
                return null;
            var ep = new ExcelPackage();
            var wb = ep.Workbook;
            for (int i = 0; i < columnNames.Length; i++)
            {
                ExcelWorksheet ws = null;
                if (i <= sheetName.Length - 1)
                {
                    ws = wb.Worksheets.Add(sheetName[i]);
                }
                else
                {
                    ws = wb.Worksheets.Add($"sheet{i + 1}");
                }
                //写数据  
                int colIndex = 1;
                foreach (string columnName in columnNames[i])
                {
                    ws.Cells[1, colIndex++].Value = columnName;
                }
            }
            if (wb.Worksheets.Count == 0)
                wb.Worksheets.Add($"sheet1");
            return ep.GetAsByteArray();
        }
        /// <summary>
        /// 创建excel
        /// </summary>
        /// <param name="dics"></param>
        /// <returns></returns>
        public static byte[] CreateExcel(string[] sheetName, params IList<Dictionary<string, string>>[] dics)
        {
            var ep = new ExcelPackage();
            var wb = ep.Workbook;
            for (int i = 0; i < dics.Length; i++)
            {
                if (dics[i] == null || dics[i].Count == 0)
                    continue;

                ExcelWorksheet ws = null;
                if (i <= sheetName.Length - 1)
                {
                    ws = wb.Worksheets.Add(sheetName[i]);
                }
                else
                {
                    ws = wb.Worksheets.Add($"sheet{i + 1}");
                }
                //写数据  
                int colIndex = 1;
                var noName = "序号";

                ws.Cells[1, colIndex++].Value = noName;
                var headerRow = dics[i].First();
                foreach (string key in headerRow.Keys)
                {
                    ws.Cells[1, colIndex++].Value = key;
                }
                //给序号列赋值
                int rowIndex = 2;
                foreach (var dic in dics[i])
                {
                    colIndex = 1;
                    ws.Cells[rowIndex, colIndex++].Value = (rowIndex - 1).ToString();
                    foreach (KeyValuePair<string, string> d in dic)
                    {
                        ws.Cells[rowIndex, colIndex++].Value = d.Value;
                    }
                    rowIndex++;
                }

            }
            if (wb.Worksheets.Count == 0)
                wb.Worksheets.Add($"sheet1");
            return ep.GetAsByteArray();
        }


        public static byte[] CreateExcel(params IList<Dictionary<string, string>>[] dics)
        {
            var ep = new ExcelPackage();
            var wb = ep.Workbook;
            for (int i = 0; i < dics.Length; i++)
            {
                if (dics[i] == null || dics[i].Count == 0)
                    continue;

                var ws = wb.Worksheets.Add($"sheet{i + 1}");

                //写数据  
                int colIndex = 1;
                var noName = "序号";

                ws.Cells[1, colIndex++].Value = noName;
                var headerRow = dics[i].First();
                foreach (string key in headerRow.Keys)
                {
                    ws.Cells[1, colIndex++].Value = key;
                }
                //给序号列赋值
                int rowIndex = 2;
                foreach (var dic in dics[i])
                {
                    colIndex = 1;
                    ws.Cells[rowIndex, colIndex++].Value = (rowIndex - 1).ToString();
                    foreach (KeyValuePair<string, string> d in dic)
                    {
                        ws.Cells[rowIndex, colIndex++].Value = d.Value;
                    }
                    rowIndex++;
                }

            }
            if (wb.Worksheets.Count == 0)
                wb.Worksheets.Add($"sheet1");
            return ep.GetAsByteArray();

        }
        /// <summary>
        /// 将excel转换为字典
        /// </summary>
        /// <param name="columnNames"></param>
        /// <param name="excel"></param>
        /// <returns></returns>
        public static Dictionary<string, IList<Dictionary<string, string>>> GetDicDatasFromExcel(Stream stream, params string[] sheets)
        {
            var datas = new Dictionary<string, IList<Dictionary<string, string>>>();
            using (var excel = new ExcelPackage(stream))
            {
                if (excel.Workbook.Worksheets.Count == 0)
                    throw new Exception("请上传Excel文件");
                foreach (var item in sheets)
                {
                    var sheet = excel.Workbook.Worksheets[item];
                    if (sheet == null)
                        continue;

                    var itemData = new List<Dictionary<string, string>>();
                    var rowCount = sheet.Dimension.Rows;
                    if (sheet == null || rowCount <= 1)
                    {
                        datas.Add(item, itemData);
                        continue;
                    }

                    datas.Add(item, itemData);

                    var columnCount = sheet.Dimension.Columns;
                    var columnDic = new Dictionary<string, int>();
                    for (var colIndex = 1; colIndex <= columnCount; colIndex++)
                    {
                        var columnName = sheet.Cells[1, colIndex].Text?.Trim();
                        if (!string.IsNullOrWhiteSpace(columnName))
                        {
                            columnDic.Add(columnName, colIndex);
                        }
                    }
                    for (var rowIndex = 2; rowIndex <= rowCount; rowIndex++)
                    {
                        var data = new Dictionary<string, string>();
                        foreach (var columnD in columnDic)
                        {
                            var value = sheet.Cells[rowIndex, columnD.Value].Text?.Trim();
                            data[columnD.Key] = value;
                        }
                        itemData.Add(data);
                    }
                }

            }
            return datas;
        }
        /// <summary>
        /// 将excel转换为字典
        /// </summary>
        /// <param name="columnNames"></param>
        /// <param name="excel"></param>
        /// <returns></returns>
        public static IList<Dictionary<string, string>> GetDicDatasFromExcel(Stream stream)
        {
            var datas = new List<Dictionary<string, string>>();
            using (ExcelPackage excel = new ExcelPackage(stream))
            {
                if (excel.Workbook.Worksheets.Count == 0)
                    throw new Exception("请上传Excel文件");

                var sheet = excel.Workbook.Worksheets[0];
                var rowCount = sheet.Dimension.Rows;
                var columnCount = sheet.Dimension.Columns;
                if (rowCount > 1)
                {
                    var columnDic = new Dictionary<string, int>();
                    for (var colIndex = 1; colIndex <= columnCount; colIndex++)
                    {
                        var columnName = sheet.Cells[1, colIndex].Text?.Trim();
                        if (!string.IsNullOrWhiteSpace(columnName))
                        {
                            columnDic.Add(columnName, colIndex);
                        }
                    }
                    for (var rowIndex = 2; rowIndex <= rowCount; rowIndex++)
                    {
                        var data = new Dictionary<string, string>();
                        foreach (var columnD in columnDic)
                        {
                            var value = sheet.Cells[rowIndex, columnD.Value].Text?.Trim();
                            data[columnD.Key] = value;
                        }
                        datas.Add(data);
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// 将数据转换为字段
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static IList<Dictionary<string, string>> GetDicDatasFromModel(dynamic objs)
        {
            var dics = new List<Dictionary<string, string>>();
            if (objs == null || objs.Count == 0)
                return dics;
            var data = objs[0];
            var type = data.GetType();
            foreach (var obj in objs)
            {
                if (type != null)
                {
                    var dic = new Dictionary<string, string>();
                    foreach (var property in type.GetProperties())
                    {
                        var v = "";
                        var value = property.GetValue(obj);
                        if (value != null)
                        {
                            v = value.ToString();
                        }
                        dic.Add(property.Name, v);
                    }

                    dics.Add(dic);
                }
                else
                {
                    var dic = ((IDictionary<string, object>)obj).ToDictionary(o => o.Key,
                        p => (p.Value == null ? null : p.Value.ToString()));
                    dics.Add(dic);
                }
            }
            return dics;
        }

        ///// <summary>
        ///// 通过excel转换成DataSet
        ///// </summary>
        ///// <param name="Path"></param>
        ///// <returns></returns>
        //public DataSet ExcelToDS(string Path)
        //{
        //    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
        //    OleDbConnection conn = new OleDbConnection(strConn);
        //    conn.Open();
        //    string strExcel = "";
        //    OleDbDataAdapter myCommand = null;
        //    DataSet ds = new DataSet();
        //    DataTable TableNams = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);

        //    if (TableNams != null && TableNams.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < TableNams.Rows.Count; i++)
        //        {
        //            strExcel = "select * from [" + TableNams.Rows[i]["TABLE_NAME"] + "]";
        //            myCommand = new OleDbDataAdapter(strExcel, strConn);
        //            myCommand.Fill(ds, "table" + i);
        //        }

        //    }
        //    conn.Close();
        //    return ds;
        //}
    }
}
