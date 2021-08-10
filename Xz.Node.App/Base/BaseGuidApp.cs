using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Xz.Node.App.AuthStrategies;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Core;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Base
{
    /// <summary>
    /// Guid业务层基类，UnitWork用于事务操作，Repository用于普通的数据库操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class BaseGuidApp<T, TDbContext> : BaseApp<T, TDbContext>, IBaseGuidApp<T> where T : GuidEntity, new() where TDbContext : DbContext
    {
        private readonly AuthStrategyContext user;
        /// <summary>
        /// Guid业务层基类
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        public BaseGuidApp(IUnitWork<TDbContext> unitWork, IRepository<T, TDbContext> repository, IAuth auth) : base(unitWork, repository, auth)
        {
            user = _auth.GetCurrentUser();
        }

        #region 查询
        /// <summary>
        /// 获取实例化对象
        /// </summary>
        public virtual IQueryable<T> Table => Repository.Table;

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public T Get(Guid id, bool? isDelete = false)
        {
            return Repository.FirstOrDefault(u => u.Id == id && u.IsDelete == isDelete);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public virtual IList<T> GetDatas(IList<ConditionInfo> conditions = null, IList<SortInfo> sorts = null,
          bool? isDelete = false)
        {
            var datas = Repository.GetDatas(conditions, sorts, isDelete);
            return datas;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public virtual PageInfo<T> GetPageDatas(IList<ConditionInfo> conditions, IList<SortInfo> sorts,
         int pageIndex, int pageSize, bool? isDelete = false)
        {
            PageInfo<T> page = new PageInfo<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            Repository.GetPageDatas(conditions, sorts, page, isDelete);
            return page;
        }

        /// <summary>
        /// 获取top数据
        /// </summary>
        /// <param name="top"></param>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public virtual IList<T> GetTopDatas(int top = 10, IList<ConditionInfo> conditions = null, IList<SortInfo> sorts = null, bool? isDelete = false)
        {
            return Repository.GetTopDatas(top, conditions, sorts, isDelete);
        }

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public int GetCount(IList<ConditionInfo> conditions = null, bool? isDelete = false)
        {
            return Repository.GetCount(conditions, isDelete);
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="model"></param>
        public virtual void Insert(T model)
        {
            this.BeforeInsert(model);

            //model.Id = SequentialGuidGenerator.GenerateGuid();
            model.GenerateDefaultKeyVal();
            model.UpdateTime = DateTime.Now;
            model.UpdateUserId = Guid.Parse(user.User.Id);
            model.Updater = $"{ user.User.Name}";

            model.CreateTime = DateTime.Now;
            model.CreateUserId = Guid.Parse(user.User.Id);
            model.Creater = $"{user.User.Name}";
            Repository.Add(model);

            this.AfterInsert(model);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="models"></param>
        public virtual void Insert(IList<T> models)
        {
            foreach (var model in models)
            {
                model.GenerateDefaultKeyVal();
                model.UpdateTime = DateTime.Now;
                model.UpdateUserId = Guid.Parse(user.User.Id);
                model.Updater = $"{ user.User.Name}";

                model.CreateTime = DateTime.Now;
                model.CreateUserId = Guid.Parse(user.User.Id);
                model.Creater = $"{user.User.Name}";

                this.BeforeInsert(model);
                Repository.Add(model);
            }
        }

        /// <summary>
        /// 添加之前调用
        /// </summary>
        /// <param name="model"></param>
        protected virtual void BeforeInsert(T model)
        {

        }

        /// <summary>
        /// 添加之后调用
        /// </summary>
        /// <param name="model"></param>
        protected virtual void AfterInsert(T model) { }
        #endregion

        #region 修改
        /// <summary>
        /// 修改方法
        /// </summary>
        /// <param name="model"></param>
        public virtual void Update(T model)
        {
            this.BeforeUpdate(model);

            model.UpdateTime = DateTime.Now;
            model.UpdateUserId = Guid.Parse(user.User.Id);
            model.Updater = $"{ user.User.Name}";
            Repository.Update(model);

            this.AfterUpdate(model);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="models"></param>
        public virtual void Update(IList<T> models)
        {
            foreach (var model in models)
            {
                BeforeUpdate(model);
            }
            Repository.Update(models);
            foreach (var model in models)
            {
                AfterUpdate(model);
            }
        }

        /// <summary>
        /// 修改之前调用
        /// </summary>
        /// <param name="model"></param>
        protected virtual void BeforeUpdate(T model) { }

        /// <summary>
        /// 修改之后调用
        /// </summary>
        /// <param name="model"></param>
        protected virtual void AfterUpdate(T model) { }
        #endregion

        #region 删除
        /// <summary>
        /// 按id批量删除
        /// </summary>
        /// <param name="ids"></param>
        public virtual void Delete(IList<Guid> ids)
        {
            this.BeforeDelete(ids);

            Repository.Delete(u => ids.Contains(u.Id));

            this.AfterDelete(ids);
        }

        /// <summary>
        /// 删除前要进行的操作
        /// </summary>
        /// <param name="ids"></param>
        protected virtual void BeforeDelete(IList<Guid> ids) { }
       
        /// <summary>
        /// 删除后要进行的操作
        /// </summary>
        /// <param name="ids"></param>
        protected virtual void AfterDelete(IList<Guid> ids) { }
        
        #endregion

        #region 导入
        /// <summary>
        /// 获取导入模板
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetImportTemplate()
        {
            if (_excelColumnNames == null || _excelColumnNames.Count() == 0)
            {
                throw new Exception("导入模板未添加！");
            }
            return ExcelHelper.CreateEmptyExcel(_excelColumnNames.Select(o => o.Value).ToList());
        }

        /// <summary>
        /// 获取excel列模板
        /// </summary>
        protected virtual Dictionary<string, string> _excelColumnNames
        {
            get { return this.ExcelColumnNames; }
        }

        /// <summary>
        /// excel列
        /// </summary>
        public Dictionary<string, string> ExcelColumnNames
        {
            get
            {
                var dic = new Dictionary<string, string>();
                foreach (var propInfo in typeof(T).GetProperties())
                {
                    string column = string.Empty;
                    string description = string.Empty;
                    object[] columnAttrs = propInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
                    if (columnAttrs.Length > 0)
                    {
                        var columnAttr = columnAttrs[0] as ColumnAttribute;
                        if (columnAttr != null)
                        {
                            column = columnAttr.Name;
                        }
                    }
                    object[] descriptionAttrs = propInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (descriptionAttrs.Length > 0)
                    {
                        var descriptionAttr = descriptionAttrs[0] as DescriptionAttribute;
                        if (descriptionAttr != null)
                        {
                            description = descriptionAttr.Description;
                        }
                    }
                    if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(description))
                    {
                        dic.Add(column, description);
                    }
                }
                return dic;
            }
        }

        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="file"></param>
        public virtual void Import(Stream file)
        {
            var datas = ExcelHelper.GetDicDatasFromExcel(file);
            ValidImportDatas(datas);
            var entities = ConvertExcelDataToEntity(datas);
            SaveExcelData(entities);
        }

        /// <summary>
        /// 保存导入的数据
        /// </summary>
        /// <param name="entities"></param>
        protected virtual void SaveExcelData(IList<T> entities)
        {
            // var dt = EntityHelper.ToDataTable(entities);
            foreach (var item in entities)
            {
                BeforeInsert(item);
            }

            Repository.BatchAdd(entities.ToArray());
        }

        /// <summary>
        /// 将导入数据转换为List数据
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        protected virtual IList<T> ConvertExcelDataToEntity(IList<Dictionary<string, string>> datas)
        {
            var type = typeof(T);
            IList<T> models = new List<T>();
            foreach (var data in datas)
            {
                T model = new T();
                model.Id = Guid.NewGuid();
                model.UpdateUserId = Guid.Parse(user.User.Id);
                model.UpdateTime = DateTime.Now;
                model.CreateUserId = Guid.Parse(user.User.Id);
                model.CreateTime = DateTime.Now;
                model.Creater = user.User.Name;
                model.Updater = user.User.Name;
                foreach (var property in type.GetProperties())
                {
                    if (data.ContainsKey(property.Name))
                    {
                        var propertyType = property.PropertyType;
                        var value = data[property.Name];
                        if (!string.IsNullOrEmpty(value))
                        {
                            object obj = null;
                            if (propertyType.IsGenericType &&
                                propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) //如果是可空类型
                            {
                                NullableConverter nullableConverter = new NullableConverter(propertyType);
                                obj = Convert.ChangeType(value, nullableConverter.UnderlyingType);
                            }
                            else
                            {
                                if (propertyType.BaseType.Name == "Enum")
                                {
                                    obj = Enum.Parse(propertyType, value);
                                }
                                else if (propertyType.Name == "Guid")
                                {
                                    obj = Guid.Parse(value);
                                }
                                else
                                {
                                    obj = Convert.ChangeType(value, propertyType);
                                }
                            }

                            property.SetValue(model, obj);
                        }
                    }
                }

                models.Add(model);
            }

            return models;
        }

        #region 导入数据验证
        /// <summary>
        /// 验证导入的数据
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void ValidImportDatas(IList<Dictionary<string, string>> datas)
        {
            if (datas == null || datas.Count == 0)
                throw new Exception("没有需要导入的数据");
            //判断模板是否符合规范
            var firstData = datas.First();
            var excelColunms = firstData.Keys;
            var columnNames = _excelColumnNames.Select(o => o.Value).ToList();
            //var notExistColumnNames = excelColunms.Except(columnNames).ToList();
            //if (notExistColumnNames.Count > 0)
            //{
            //    throw new InfoException(T("模板不符合规范，模板中的字段（{0}）不需要", string.Join(",", notExistColumnNames)));
            //}
            var notExistColumnNames = columnNames.Except(excelColunms).ToList();
            if (notExistColumnNames.Count > 0)
            {
                string error = string.Format("模板不符合规范，字段（{0}）在模板中不存在", string.Join(",", notExistColumnNames));
                throw new Exception(error);
            }

            //转换dic的key，用实体的属性名称，替换excel的title，方便后面的操作
            for (var i = 0; i < datas.Count; i++)
            {
                var data = datas[i];
                Dictionary<string, string> newData = new Dictionary<string, string>();
                foreach (var d in data)
                {
                    if (columnNames.Contains(d.Key))
                    {
                        var columnName = _excelColumnNames.Where(o => o.Value == d.Key).Select(o => o.Key)
                            .FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(columnName))
                            newData[columnName] = d.Value;
                    }
                }

                datas[i] = newData;
            }
        }

        /// <summary>
        /// 验证EXCEL数据--必填
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataRequire(string columnName, int rowIndex, string value, string msg = "")
        {
            rowIndex++;
            if (string.IsNullOrWhiteSpace(value))
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = Format("第“{0}”行，“{1}”列：不能为空", rowIndex.ToString(), columnName);
                }
                else
                {
                    msg = Format(msg, rowIndex.ToString(), columnName);
                }
                throw new InfoException(msg);
            }
        }

        /// <summary>
        /// 验证EXCEL数据--最大长度
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="length">最大长度</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataMaxLength(string columnName, int rowIndex, string value, int length, string msg = "")
        {
            rowIndex++;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Length > length)
                {
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        msg = Format("第“{0}”行，“{1}”列：最多{2}个字符", rowIndex.ToString(), columnName, length.ToString());
                    }
                    else
                    {
                        msg = Format(msg, rowIndex.ToString(), columnName, length.ToString());
                    }
                    throw new InfoException(msg);
                }
            }
        }

        /// <summary>
        /// 验证EXCEL数据--最小长度
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="length">最小长度</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataMinLength(string columnName, int rowIndex, string value, int length, string msg = "")
        {
            rowIndex++;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Length < length)
                {
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        msg = Format("第“{0}”行，“{1}”列：最少{2}个字符", rowIndex.ToString(), columnName, length.ToString());
                    }
                    else
                    {
                        msg = Format(msg, rowIndex.ToString(), columnName, length.ToString());
                    }
                    throw new InfoException(msg);
                }
            }
        }

        /// <summary>
        /// 验证EXCEL数据--是否是电话
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataIsPhone(string columnName, int rowIndex, string value, string msg = "")
        {
            rowIndex++;
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "第“{0}”行，“{1}”列：请输入正确的电话号码";
            }
            ValidExcelDataByRegex(columnName, rowIndex, value
                , @"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)"
                , msg);
        }

        /// <summary>
        /// 验证EXCEL数据--是否是移动电话
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataIsMobilePhone(string columnName, int rowIndex, string value, string msg = "")
        {
            rowIndex++;
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "第“{0}”行，“{1}”列：请输入正确的电话号码";
            }
            ValidExcelDataByRegex(columnName, rowIndex, value
                , @"^(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$"
                , msg);
        }

        /// <summary>
        /// 验证EXCEL数据--是否是邮箱
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataIsEmail(string columnName, int rowIndex, string value, string msg = "")
        {
            rowIndex++;
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "第“{0}”行，“{1}”列：请输入正确的邮箱";
            }
            ValidExcelDataByRegex(columnName, rowIndex, value
                , @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$"
                , msg);
        }

        /// <summary>
        /// 验证EXCEL数据--是否是整数
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataIsInt(string columnName, int rowIndex, string value, string msg = "")
        {
            rowIndex++;
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "第“{0}”行，“{1}”列：请输入正确的数字";
            }
            ValidExcelDataByRegex(columnName, rowIndex, value
                , @"^[-+]?\d+$"
                , msg);
        }

        /// <summary>
        /// 验证EXCEL数据--是否是浮点数
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataIsFloat(string columnName, int rowIndex, string value, string msg = "")
        {
            rowIndex++;
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "第“{0}”行，“{1}”列：请输入正确的浮点数";
            }
            ValidExcelDataByRegex(columnName, rowIndex, value
                , @"^[-+]?\d+|[-+]?\d+\.\d+$"
                , msg);
        }

        /// <summary>
        /// 验证EXCEL数据--是否是时间
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataIsDateTime(string columnName, int rowIndex, string value, string msg = "")
        {
            rowIndex++;
            DateTime dt = DateTime.Now;
            if (!DateTime.TryParse(value, out dt))
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = Format("第“{0}”行，“{1}”列：请输入正确的日期", rowIndex.ToString(), columnName);
                }
                else
                {
                    msg = Format(msg, rowIndex.ToString(), columnName);
                }
                throw new InfoException(msg);
            }
        }

        /// <summary>
        /// 验证EXCEL数据--正则
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataByRegex(string columnName, int rowIndex, string value, string regex, string msg = "")
        {
            rowIndex++;
            if (!string.IsNullOrEmpty(value))
            {
                var reg = new Regex(regex);
                if (!reg.IsMatch(value))
                {
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        msg = Format("第“{0}”行，“{1}”列：输入不符合规范", rowIndex.ToString(), columnName);
                    }
                    else
                    {
                        msg = Format(msg, rowIndex.ToString(), columnName);
                    }
                    throw new InfoException(msg);
                }
            }
        }

        /// <summary>
        /// 判断是否具有重复数据
        /// </summary>
        /// <param name="datas">数据</param>
        /// <param name="key">键值</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataUnique(IList<Dictionary<string, string>> datas, string key, string msg = "")
        {
            var repetitionDatas = datas.GroupBy(o => o[key]).Select(o => new
            {
                Key = o.Key,
                Count = o.Count()
            }).Where(o => o.Count > 1).Select(o => o.Key).ToList();
            if (repetitionDatas.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = Format("“{1}”列数据重复：{1}", key, string.Join(",", repetitionDatas));
                }
                else
                {
                    msg = Format(msg, key, string.Join(",", repetitionDatas));
                }
                throw new InfoException(msg);
            }

        }

        /// <summary>
        /// 判断数据是否再指定的容器范围内
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="value">值</param>
        /// <param name="container">指定容器</param>
        /// <param name="msg">自定义格式的提示消息</param>
        protected void ValidExcelDataInContainer(string columnName, int rowIndex, string value, IList<string> container, string msg = "")
        {
            rowIndex++;
            if (container != null && container.Count > 0 && !container.Contains(value))
            {
                string containerStr = string.Join(",", container);
                if (containerStr.Length > 110)
                {
                    containerStr = containerStr.Substring(0, 100) + "...";
                }

                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = Format("第“{0}”行，“{1}”列：输入不符合规范，允许的输入是：{2}", rowIndex.ToString(), columnName, containerStr);
                }
                else
                {
                    msg = Format(msg, rowIndex.ToString(), columnName, containerStr);
                }
                throw new InfoException(msg);
            }
        }
        #endregion

        #endregion

        #region 导出
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="conditionInfos"></param>
        /// <param name="sortInfos"></param>
        /// <returns></returns>
        public virtual byte[] Export(IList<ConditionInfo> conditionInfos, IList<SortInfo> sortInfos = null)
        {
            var datas = GetExportDatas(conditionInfos, sortInfos);
            if (datas.Count != 0)
            {
                IList<Dictionary<string, string>> excelDatas = ExcelHelper.GetDicDatasFromModel(datas);
                DealExcelDatas(excelDatas);
                DealExcelColumnNames(ref excelDatas);
                return ExcelHelper.CreateExcel(excelDatas);
            }
            else
            {
                return GetImportTemplate();
            }
        }

        /// <summary>
        /// 获取需要导出的数据
        /// </summary>
        /// <param name="conditionInfos"></param>
        /// <returns></returns>
        protected virtual dynamic GetExportDatas(IList<ConditionInfo> conditionInfos)
        {
            var datas = this.GetExportDatas(conditionInfos, null);
            return datas;
        }

        /// <summary>
        /// 获取导出数据
        /// </summary>
        /// <param name="conditionInfos"></param>
        /// <param name="sortInfos"></param>
        /// <returns></returns>
        public virtual dynamic GetExportDatas(IList<ConditionInfo> conditionInfos, IList<SortInfo> sortInfos)
        {
            var datas = Repository.GetDatas(conditionInfos, sortInfos);
            return datas;
        }

        /// <summary>
        /// 处理即将导出的数据
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void DealExcelDatas(IList<Dictionary<string, string>> datas) { }

        /// <summary>
        /// 处理EXCEL的列头和列顺序
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="columnNameDics"></param>
        protected virtual void DealExcelColumnNames(ref IList<Dictionary<string, string>> datas,
            Dictionary<string, string> columnNameDics = null)
        {
            if (columnNameDics == null || columnNameDics.Count == 0)
            {
                columnNameDics = _excelColumnNames;
            }

            IList<Dictionary<string, string>> newDatas = new List<Dictionary<string, string>>();
            foreach (var data in datas)
            {
                Dictionary<string, string> newData = new Dictionary<string, string>();

                foreach (var columnName in columnNameDics)
                {
                    newData[columnName.Value] = data[columnName.Key];
                }

                newDatas.Add(newData);
            }

            datas = newDatas;
        }
        #endregion

        #region Common
        /// <summary>
        /// 提交
        /// </summary>
        public virtual void SaveChange()
        {
            Repository.Save();
        }

        /// <summary>
        /// 国际化
        /// </summary>
        /// <param name="content"></param>
        /// <param name="agrs"></param>
        /// <returns></returns>
        protected string Format(string content, params string[] agrs)
        {
            return string.Format(content, agrs);
        }

        /// <summary>
        /// 验证字段是否唯一
        /// 调用方式：ValidDataUnique(model.Id, "Code", model.Code, "已经存在代码为{0}的数据");
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="columnName">验证的列名</param>
        /// <param name="value">验证的值</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="parentColunName">同行字段名</param>
        /// <param name="parentValue">同行字段值</param>
        protected void ValidDataUnique(Guid id, string columnName, string value, string errMsg, string parentColunName = null, string parentValue = null)
        {
            var codeConditions = new List<ConditionInfo>() {
                new ConditionInfo(){
                    ColumnName="Id",
                    Operator = ConditionOperEnum.Unequal,
                    Value = id.ToString()
                },
                new ConditionInfo(){
                    ColumnName=columnName,
                    Value = value
                }
            };
            if (!string.IsNullOrEmpty(parentColunName))
            {
                codeConditions.Add(new ConditionInfo()
                {
                    ColumnName = parentColunName,
                    Value = parentValue
                });
            }
            var codeCount = this.GetCount(codeConditions, false);
            if (codeCount > 0)
            {
                throw new InfoException(Format(errMsg, value));
            }
        }
        #endregion
    }
}