using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xz.Node.Framework.Enums;

namespace Xz.Node.Framework.Model
{
    public class BaseDto
    {
        /// <summary>
        /// 查询条件模型
        /// </summary>
        public class ConditionModel : BasicServiceModel<ConditionInfo>
        {
            /// <summary>
            /// 字段名称
            /// </summary>
            [Display(Name = "字段名称")]
            public string ColumnName
            {
                get { return Service.ColumnName; }
                set { Service.ColumnName = value; }
            }
            /// <summary>
            /// 操作类型
            /// </summary>
            [Display(Name = "操作类型")]
            public ConditionOperEnum Operator
            {
                get { return Service.Operator; }
                set { Service.Operator = value; }
            }
            /// <summary>
            /// 操作关系
            /// </summary>
            [Display(Name = "操作关系")]
            public ConditionRelationEnum Relation
            {
                get { return Service.Relation; }
                set { Service.Relation = value; }
            }
            /// <summary>
            /// 字段的类型
            /// </summary>
            [Display(Name = "字段的类型")]
            public DataTypeEnum DataType
            {
                get { return Service.DataType; }
                set { Service.DataType = value; }
            }
            /// <summary>
            /// 分组
            /// </summary>
            [Display(Name = "分组")]
            public string Group
            {
                get { return Service.Group; }
                set { Service.Group = value; }
            }
            /// <summary>
            /// 分组间的关系
            /// </summary>
            [Display(Name = "分组间的关系")]
            public ConditionRelationEnum GroupRelation
            {
                get { return Service.GroupRelation; }
                set { Service.GroupRelation = value; }
            }
            /// <summary>
            /// 表别名
            /// </summary>
            [Display(Name = "表别名")]
            public string Alias
            {
                get { return Service.Alias; }
                set { Service.Alias = value; }
            }
            /// <summary>
            /// 比较值
            /// </summary>
            [Display(Name = "比较值")]
            public string Value
            {
                get { return Service.Value; }
                set { Service.Value = value; }
            }
            /// <summary>
            /// 索引号
            /// </summary>
            [Display(Name = "索引号")]
            public int Index
            {
                get { return Service.Index; }
                set { Service.Index = value; }
            }
            /// <summary>
            /// 是否查询
            /// </summary>
            [Display(Name = "是否查询")]
            public bool IsQuery
            {
                get { return Service.IsQuery; }
                set { Service.IsQuery = value; }
            }
        }
        /// <summary>
        /// 排序模型
        /// </summary>
        public class SortModel : BasicServiceModel<SortInfo>
        {
            /// <summary>
            /// 表的字段名
            /// </summary>
            [Display(Name = "表的字段名")]
            public string ColumnName
            {
                get { return Service.ColumnName; }
                set { Service.ColumnName = value; }
            }
            /// <summary>
            /// 排序方向
            /// </summary>
            [Display(Name = "排序方向")]
            public ConditionDirectionEnum Direction
            {
                get { return Service.Direction; }
                set { Service.Direction = value; }
            }
            /// <summary>
            /// 表别名
            /// </summary>
            [Display(Name = "表别名")]
            public string Alias
            {
                get { return Service.Alias; }
                set { Service.Alias = value; }
            }
        }
        /// <summary>
        /// 条件集合模型
        /// </summary>
        public class ConditionsModel
        {
            /// <summary>
            /// 查询条件
            /// </summary>
            [Display(Name = "查询条件")]
            public IList<ConditionModel> Conditions { get; set; }
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        public class DataModel : ConditionsModel
        {
            /// <summary>
            /// 排序条件
            /// </summary>
            [Display(Name = "排序")]
            public IList<SortModel> Sorts { get; set; }
        }
        /// <summary>
        /// 返回最多数量模型
        /// </summary>
        public class TopDataModel : DataModel
        {
            /// <summary>
            /// 最多返回数量条数
            /// </summary>
            [Display(Name = "最多返回数量条数")]
            public int Top { get; set; }
        }

        /// <summary>
        /// 分页查询模型
        /// </summary>
        public class PageDataModel : DataModel
        {
            /// <summary>
            /// 每页数量
            /// </summary>
            [Display(Name = "每页数量")]
            public int? PageSize { get; set; }
            /// <summary>
            /// 第几页
            /// </summary>
            [Display(Name = "第几页")]
            public int? PageIndex { get; set; }
        }
        /// <summary>
        /// 导出文件模型
        /// </summary>
        public class ExportModel : DataModel
        {
            /// <summary>
            /// 导出文件名称
            /// </summary>
            [Display(Name = "导出文件名称")]
            public string FileName { get; set; }
        }

        public class ExportByDcModel : DataModel
        {
            /// <summary>
            /// 导出文件名称
            /// </summary>
            [Display(Name = "导出文件名称")]
            public string FileName { get; set; }

            [Display(Name = "导出文件字段")]
            public Dictionary<string, string> Field { get; set; }
        }
        public class TemplateModel
        {
            /// <summary>
            /// 导出文件名称
            /// </summary>
            [Display(Name = "模板文件名称")]
            public string FileName { get; set; }

            [Display(Name = "导出文件字段")]
            public Dictionary<string, string> Field { get; set; }
        }

        /// <summary>
        /// id集合模型
        /// </summary>
        public class IdsModel
        {
            /// <summary>
            /// 主键集合
            /// </summary>
            [Display(Name = "主键集合")]
            public IList<Guid> Ids { get; set; }
        }
        /// <summary>
        /// 审核/修改状态集合模型
        /// </summary>
        public class StatusModel
        {
            /// <summary>
            /// 主键集合
            /// </summary>
            [Display(Name = "主键集合")]
            public IList<Guid> Ids { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            [Display(Name = "状态")]
            public int Status { get; set; }
        }
    }

    public static class BaseDtoExtensions
    {
        public static IList<ConditionInfo> ToConditions(this IList<BaseDto.ConditionModel> _this)
        {
            var conditionInfos = new List<ConditionInfo>();
            if (_this?.Count > 0)
            {
                foreach (var item in _this)
                {
                    conditionInfos.Add(item.Service);
                }
            }
            return conditionInfos;
        }

        public static IList<SortInfo> ToSorts(this IList<BaseDto.SortModel> _this)
        {
            var sortInfos = new List<SortInfo>();
            if (_this?.Count > 0)
            {
                foreach (var item in _this)
                {
                    sortInfos.Add(item.Service);
                }
            }
            return sortInfos;
        }
    }
}
