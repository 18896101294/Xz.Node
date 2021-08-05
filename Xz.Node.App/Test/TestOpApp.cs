using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.App.Test.Response;
using Xz.Node.Framework.Extensions;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Test;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Test
{
    /// <summary>
    /// 测试App
    /// </summary>
    public class TestOpApp : BaseGuidApp<Test_OpInfo, XzDbContext>
    {
        /// <summary>
        /// 测试App构造函数
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        public TestOpApp(IUnitWork<XzDbContext> unitWork, IRepository<Test_OpInfo, XzDbContext> repository,
            IAuth auth) : base(unitWork, repository, auth)
        {
            //var unitWork = _autofacServiceProvider.GetService<IUnitWork<OpenAuthDBContext>>();
        }

        /// <summary>
        /// 一对一关联查询，Lambda表达式写法
        /// </summary>
        public dynamic TestOneToOneLambdaFun()
        {
            //两表join lamdba写法
            var query = UnitWork.Find<Test_OpInfo>(null).Join(UnitWork.Find<Test_OcInfo>(null), a => a.Id, b => b.TestOpForeignKey, (a, b) => new
            {
                OpName = a.Name,
                OcName = b.Name
            });

            //三表join lamdba写法
            var query1 = UnitWork.Find<Test_OpInfo>(null).Join(UnitWork.Find<Test_OcInfo>(null), a => a.Id, b => b.TestOpForeignKey, (a, b) => new
            {
                OpId = a.Id,
                OpName = a.Name,
                OcName = b.Name
            }).Join(UnitWork.Find<Test_OaInfo>(null), a => a.OpId, b => b.TestOpForeignKey, (a, b) => new
            {
                OpName = a.OpName,
                OcName = a.OcName,
                OaName = b.Name,
            });



            return query;
        }

        /// <summary>
        /// 一对一关联查询，Linq表达式写法
        /// </summary>
        public IList<TestOneToOneLinqFunVM> TestOneToOneLinqFun()
        {
            //简单的多表查询可以使用UnitWork完成
            //如果是复杂的SQL查询，建议使用SQL语句查询，以获得更高的性能。
            var query = from op in UnitWork.Find<Test_OpInfo>(null)
                        join oc in UnitWork.Find<Test_OcInfo>(null) on op.Id equals oc.TestOpForeignKey
                        where op.IsDelete == false && op.Disable == true && oc.Disable == true && oc.IsDelete == false
                        select new TestOneToOneLinqFunVM()
                        {
                            OpName = op.Name,
                            OcName = oc.Name
                        };

            //三表join linq写法
            var query1 = from op in UnitWork.Find<Test_OpInfo>(null)
                         join oc in UnitWork.Find<Test_OcInfo>(null) on op.Id equals oc.TestOpForeignKey
                         where op.IsDelete == false && op.Disable == true && oc.Disable == true && oc.IsDelete == false
                         join oa in UnitWork.Find<Test_OcInfo>(null) on op.Id equals oa.TestOpForeignKey
                         select new
                         {
                             OpName = op.Name,
                             OcName = oc.Name,
                             OaName = oa.Name
                         };
            return query.ToList();
        }

        /// <summary>
        /// 一对一多关联查询，Lambda表达式写法
        /// </summary>
        public dynamic TestOneToMoreLambdaFun()
        {
            /*
             * GroupJoin：  用于查询一对多的关系很方便，所以得数据格式就是一对多的关系
             * SelectMany:  可以解析集合中含有集合的情况(也就是一对多的表现)为单一对象
             */
            var query = UnitWork.Find<Test_OpInfo>(null).GroupJoin(UnitWork.Find<Test_ObInfo>(null), a => a.Id, b => b.TestOpForeignKey, (a, b) => new
            {
                OpName = a.Name,
                parent = b
            }).SelectMany(a => a.parent, (m, n) => new
            {
                OpName = m.OpName,
                OcName = n.Name
            });

            //三表left join lamdba写法,这里也可以先selectmany了在join第三张表
            var query1 = UnitWork.Find<Test_OpInfo>(null).GroupJoin(UnitWork.Find<Test_ObInfo>(null), a => a.Id, b => b.TestOpForeignKey, (a, b) => new
            {
                Opid = a.Id,
                OpName = a.Name,
                Parent = b
            }).GroupJoin(UnitWork.Find<Test_OaInfo>(null), a => a.Opid, b => b.TestOpForeignKey, (m, n) => new
            {
                OpName = m.OpName,
                Opid = m.Opid,
                Score = n,
                Parent = m.Parent
            }).SelectMany(a => a.Parent.DefaultIfEmpty(), (m, n) => new
            {
                OpName = m.OpName,
                ObName = n.Name,
                Score = m.Score
            }).SelectMany(a => a.Score.DefaultIfEmpty(), (m, n) => new
            {
                OpName = m.OpName,
                ObName = m.ObName,
                OaName = n.Name,
            });
            return query;
        }

        /// <summary>
        /// 一对一两表关联查询，Linq表达式写法
        /// </summary>
        public dynamic TestOneToMoreLinqFun()
        {
            //简单的多表查询可以使用UnitWork完成
            //如果是复杂的SQL查询，建议使用SQL语句查询，以获得更高的性能。
            var query = from op in UnitWork.Find<Test_OpInfo>(null)
                        join ob in UnitWork.Find<Test_ObInfo>(null) on op.Id equals ob.TestOpForeignKey into jtemp
                        from leftjoin in jtemp.DefaultIfEmpty()
                        select new
                        {
                            OpName = op.Name,
                            OcName = leftjoin.Name
                        };

            //三表left join linq写法
            var query1 = from op in UnitWork.Find<Test_OpInfo>(null)
                         join ob in UnitWork.Find<Test_ObInfo>(null) on op.Id equals ob.TestOpForeignKey into jtemp
                         join oa in UnitWork.Find<Test_OaInfo>(null) on op.Id equals oa.TestOpForeignKey into stemp
                         from leftp in jtemp.DefaultIfEmpty()
                         from lefts in stemp.DefaultIfEmpty()
                         select new
                         {
                             OpName = op.Name,
                             OcName = leftp.Name,
                             OaName = lefts.Name
                         };
            return query;
        }

        /// <summary>
        /// 单表分组的写法
        /// </summary>
        /// <returns></returns>
        public dynamic OneTableGroupFun()
        {
            /*
             * 这里用 grouptemp.ToList() 直接查询的话会出错：.ToList()' could not be translated.
             * 翻译过来就是无法将tolist解析成sql语句。
             * 所以这种写法不可取，可以先把数据查出来，再在内存中进行分组
             */

            //linq的写法
            var query1 = from op in UnitWork.Find<Test_OpInfo>(null)
                         group op by op.AppSecret into grouptemp
                         //where grouptemp.Sum(a => a.Score1) > 60 //这里可以加上一些条件
                         select new
                         {
                             AppSecret = grouptemp.Key,
                             //List = grouptemp.ToList()
                             //底下可以跟一些统计之类的函数
                             //sum = grouptemp.Sum(a => a.Score1),
                             //max = grouptemp.Max(a => a.Score1),
                             //min = grouptemp.Min(a => a.Score1),
                             //avg = grouptemp.Average(a => a.Score1)
                         };

            //lamdba的写法
            var query = UnitWork.Find<Test_OpInfo>(null).GroupBy(a => a.AppSecret).Select(grouptemp => new
            {
                AppSecret = grouptemp.Key,
                List = grouptemp.ToList()
                //sum = grouptemp.Sum(a => a.Score1),
                //max = grouptemp.Max(a => a.Score1),
                //min = grouptemp.Min(a => a.Score1),
                //avg = grouptemp.Average(a => a.Score1)
            });
            //.Where(a => a.max > 60);
            return query1;
        }

        /// <summary>
        /// 获取指定条件的数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Test_OpInfo> FindByName(string name)
        {
            /*
             * 使用 Include 获取TestOp的导航属性
             * 拓展：ThenInclude 可继续获取下一层级的导航属性
             */
            var opInfo = Repository.Table
                            .Include(o => o.TestOc)
                            .Include(o => o.Test_Obs)
                            //.ThenInclude(o => o.test)
                            .FirstOrDefault(o => o.Name == name);

            var opInfo1 = Repository.Table
                            .Include(o => o.Test_Obs)
                            .FirstOrDefault(o => o.Name == name);

            return Repository.Find(u => u.Name == name).ToList();
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        protected override Dictionary<string, string> _excelColumnNames
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "Name","应用名称"},
                    { "AppSecret","应用密钥"},
                    { "Description","应用描述"},
                    { "Icon","应用图标"},
                    { "Disable","是否可用"}
                };
            }
        }

        /// <summary>
        /// 验证导入的数据
        /// </summary>
        /// <param name="datas"></param>
        protected override void ValidImportDatas(IList<Dictionary<string, string>> datas)
        {
            base.ValidImportDatas(datas);//调用父类方法

            {
                //这里可以进行自定义的验证

                //代码块...
            }

            //此处可以进行导入数据的验证
            for (var i = 0; i < datas.Count; i++)
            {
                //例如：
                //var data = datas[i];
                //ValidExcelDataRequire(_excelColumnNames["Value"], i + 1, data["Value"]);
                //ValidExcelDataMaxLength(_excelColumnNames["Value"], i + 1, data["Value"], 200);
                //ValidExcelDataRequire(_excelColumnNames["Text"], i + 1, data["Text"]);
                //ValidExcelDataMaxLength(_excelColumnNames["Text"], i + 1, data["Text"], 2000);
                //ValidExcelDataRequire(_excelColumnNames["DisplayNo"], i + 1, data["DisplayNo"]);
                //ValidExcelDataIsInt(_excelColumnNames["DisplayNo"], i + 1, data["DisplayNo"]);
                //ValidExcelDataRequire(_excelColumnNames["Category"], i + 1, data["Category"]);
                //ValidExcelDataMaxLength(_excelColumnNames["Category"], i + 1, data["Category"], 50);
            }
        }

    }
}
