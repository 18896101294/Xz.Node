using System;
using System.Collections.Generic;
using System.Text;
using Xz.Node.App.Base;
using Xz.Node.App.CodeGeneration.Request;
using Xz.Node.Framework.Model;
using Xz.Node.Repository;

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
        /// 生成代码
        /// </summary>
        /// <param name="req"></param>
        public void CreateCode(CreateCodeReq req)
        {
            //根据数据库表名获取表结构
            var resultData = _dbExtension.GetDbTableStructure(req.TableName);


        }
    }
}
