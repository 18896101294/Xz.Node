using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xz.Node.App.CodeGeneration.Model;
using Xz.Node.App.CodeGeneration.Request;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;

namespace Xz.Node.App.CodeGeneration
{
    /// <summary>
    /// 代码生成
    /// </summary>
    public class CodeGenerationApp
    {
        private readonly DbExtension _dbExtension;

        /// <summary>
        /// 代码生成构造函数
        /// </summary>
        public CodeGenerationApp(DbExtension dbExtension)
        {
            _dbExtension = dbExtension;
        }

        /// <summary>
        /// 获取数据库所有表
        /// </summary>
        /// <returns></returns>
        public IList<SysTable> GetTables()
        {
            var resultData = _dbExtension.GetDbTables();
            return resultData;
        }

        /// <summary>
        /// 根据数据库表名获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public IList<SysTableColumn> GetDbTableStructure(string tableName)
        {
            var resultData = _dbExtension.GetDbTableStructure(tableName);
            return resultData;
        }

        /// <summary>
        /// 根据表名获取实体详情
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public EntityModel GetEntities(string tableName)
        {
            var entity = new EntityModel();
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new InfoException("请填写表名");
            }
            var tableColumns = _dbExtension.GetDbTableStructure(tableName);
            foreach (var column in tableColumns)
            {
                var property = new PropertyInfo();
                property.DataType = column.ColumnType;
                property.Length = column.MaxLength;
                property.IsNullable = column.IsNull.HasValue ? true : false;
                property.Name = column.ColumnName;
                property.Description = column.Comment;
                entity.Properties.Add(property);
            }
            var idProperty = entity.Properties.Where(o => o.Name.ToUpper() == "ID" || o.Name.ToUpper() == "UID").SingleOrDefault();
            if (idProperty == null)
            {
                throw new InfoException("表不符合规范，没有ID字段");
            }
            if (idProperty.CSharpTypeName != "Guid")
            {
                throw new InfoException("表不符合规范，ID的类型必须是GUID");
            }
            if (!entity.Properties.Where(o => o.Name.ToUpper() == "IsDelete".ToUpper()).Any())
            {
                throw new InfoException("表不符合规范，没有IsDelete字段");
            }
            if (!entity.Properties.Where(o => o.Name.ToUpper() == "CreateTime".ToUpper()).Any())
            {
                throw new InfoException("表不符合规范，没有CreateTime字段");
            }
            if (!entity.Properties.Where(o => o.Name.ToUpper() == "CreateUserId".ToUpper()).Any())
            {
                throw new InfoException("表不符合规范，没有CreateUserId字段");
            }
            if (!entity.Properties.Where(o => o.Name.ToUpper() == "UpdateTime".ToUpper()).Any())
            {
                throw new InfoException("表不符合规范，没有UpdateTime字段");
            }
            if (!entity.Properties.Where(o => o.Name.ToUpper() == "UpdateUserId".ToUpper()).Any())
            {
                throw new InfoException("表不符合规范，没有UpdateUserId字段");
            }
            return entity;
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="req"></param>
        public void CreateCode(CreateCodeReq req)
        {
            if(req.Properties == null || req.Properties.Count() == 0)
            {
                throw new InfoException("字段属性为空");
            }
            if (string.IsNullOrWhiteSpace(req.NameSpace))
            {
                throw new InfoException("请填写命名空间");
            }
            if (string.IsNullOrWhiteSpace(req.TableName))
            {
                throw new InfoException("请填写表名");
            }
            if (string.IsNullOrWhiteSpace(req.NameSpace))
            {
                throw new InfoException("请填写命名空间");
            }
            if (string.IsNullOrEmpty(req.Description))
            {
                throw new InfoException("请填写表描述");
            }

            var entity = new EntityInfo();

            var filePath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\";
            //同级目录，非同级目录时，注释掉这一行
            //filePath = Path.Combine(filePath, "xz", "Node");
            //Model
            if (req.IsModel)
            {
                var modelFolder = Path.Combine(filePath, "Xz.Node.Repository", "Domain", req.NameSpace);
                if (!Directory.Exists(modelFolder))
                {
                    Directory.CreateDirectory(modelFolder);
                }
                if (!File.Exists(Path.Combine(modelFolder, req.TableName + "Info.cs")))
                {
                    //File.WriteAllText(Path.Combine(modelFolder, req.TableName + "Info.cs"), new Template.Model(entity).TransformText());
                }
            }
        }
    }
}
