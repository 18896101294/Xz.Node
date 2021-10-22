using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xz.Node.Framework.Enums;

namespace Xz.Node.App.CodeGeneration.Model
{
    /// <summary>
    /// 字段 属性信息
    /// </summary>
    public class PropertyInfo
    {
        public PropertyInfo()
        {
            IsQuery = true;
            IsShow = true;
            IsEdit = true;
            QueryOper = ConditionOperEnum.Equal;
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        public string FUName
        {
            get
            {
                var names = Name.Split('_').Where(o => !string.IsNullOrWhiteSpace(o))
                   .Select(o => o.Substring(0, 1).ToUpper() + o.Substring(1).ToLower()).ToList();
                return string.Join("", names);
            }
        }
        public string FLName
        {
            get
            {
                var value = "";
                if (!string.IsNullOrWhiteSpace(FUName))
                {
                    value = FUName.Substring(0, 1).ToLower() + FUName.Substring(1);
                }
                return value;
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// 有效位数
        /// </summary>
        public int Prec { get; set; }
        /// <summary>
        /// 小树位数
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        public string CSharpTypeName
        {
            get
            {
                var typeName = "";

                switch (DataType)
                {
                    case "Uniqueidentifier":
                        if (IsNullable)
                        {
                            typeName = "Guid?";
                        }
                        else
                        {
                            typeName = "Guid";
                        }
                        break;
                    case "Int":
                        if (IsNullable)
                        {
                            typeName = "int?";
                        }
                        else
                        {
                            typeName = "int";
                        }
                        break;
                    case "String":
                        typeName = "string";
                        break;
                    case "Decimal":
                        if (IsNullable)
                        {
                            typeName = "decimal?";
                        }
                        else
                        {
                            typeName = "decimal";
                        }
                        break;
                    case "DateTime":
                        if (IsNullable)
                        {
                            typeName = "DateTime?";
                        }
                        else
                        {
                            typeName = "DateTime";
                        }
                        break;
                    case "Boolean":
                        if (IsNullable)
                        {
                            typeName = "bool?";
                        }
                        else
                        {
                            typeName = "bool";
                        }
                        break;
                    default:
                        typeName = DataType;
                        break;
                }
                return typeName;
            }
        }

        public string DatabaseType
        {
            get
            {
                var typeName = "";

                switch (DataType)
                {
                    case "Uniqueidentifier":
                        typeName = "uniqueidentifier";
                        break;
                    case "Int":
                        typeName = "int";
                        break;
                    case "String":
                        typeName = "nvarchar";
                        break;
                    case "Datetime":
                        typeName = "datetime";
                        break;
                    case "Decimal":
                        typeName = "numeric";
                        break;
                    case "Boolean":
                        typeName = "bit";
                        break;
                    default:
                        typeName = DataType;
                        break;
                }
                return typeName;
            }
        }
        /// <summary>
        /// 是否在列表中显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 是否查询
        /// </summary>
        public bool IsQuery { get; set; }
        /// <summary>
        /// 释放可以编辑（在form表单中显示）
        /// </summary>
        public bool IsEdit { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public ConditionOperEnum QueryOper { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefValue
        {
            get
            {
                string value = "";
                switch (DataType)
                {
                    case "Uniqueidentifier":
                        value = "'00000000-0000-0000-0000-000000000000'";
                        break;
                    case "Int":
                        value = "0";
                        break;
                    case "String":
                        value = "''";
                        break;
                    case "DateTime":
                        value = "dayjs(Date()).format('YYYY-MM-DD HH:mm:ss')";
                        break;
                    case "Decimal":
                        value = "0";
                        break;
                    case "Boolean":
                        value = "false";
                        break;
                }
                return value;
            }
        }
        /// <summary>
        /// 表单验证
        /// </summary>
        public string FrontValidText
        {
            get
            {
                IList<string> valids = new List<string>();
                if (!IsNullable)
                {
                    valids.Add("Validators.required");
                }
                if (DataType == "String")
                {
                    valids.Add("Validators.maxLength(" + Length + ")");
                }
                return string.Join(",", valids);
            }
        }
        /// <summary>
        /// 表格单元格管道命令
        /// </summary>
        public string FrontCellValue
        {
            get
            {
                string value = "";
                switch (DataType)
                {
                    case "Uniqueidentifier":
                        value = "rowNode[col.Name]";
                        break;
                    case "Int":
                        value = "rowNode[col.Name]";
                        break;
                    case "String":
                        value = "rowNode[col.Name]";
                        break;
                    case "DateTime":
                        value = "rowNode[col.Name] | date:'yyyy-MM-dd HH:mm:ss'";
                        break;
                    case "Decimal":
                        value = "rowNode[col.Name] | number:'1.0-2'";
                        break;
                    case "Boolean":
                        value = "rowNode[col.Name]?'√':'X' ";
                        break;
                }
                return value;
            }
        }


    }
}
