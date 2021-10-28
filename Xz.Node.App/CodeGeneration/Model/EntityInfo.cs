using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xz.Node.App.CodeGeneration.Model
{
    /// <summary>
    /// 数据库表实体相关信息
    /// </summary>
    public class EntityModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 字段属性
        /// </summary>
        public IList<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();
    }

    public class EntityInfo
    {
        public EntityInfo()
        {
            IsDal = true;
            IsBll = true;
            IsWeb = true;
            Properties = new List<PropertyInfo>();

        }
        public string CreateUser { get; set; }
        public string Output
        {
            get
            {
                if (string.IsNullOrEmpty(NameSpace))
                {
                    throw new Exception("请输入命名空间");
                }
                var path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\";
                return path;
            }
        }
        public string Name { get; set; }

        //用于创建基础增删改查
        public StringBuilder IBllTxt { get; set; } = new StringBuilder();

        public StringBuilder BllTxt { get; set; } = new StringBuilder();

        public StringBuilder ControllerTxt { get; set; } = new StringBuilder();

        //用于创建dto 和 viewmolde时使用，返回类名 begin
        public string GetSelectDtoName => $"Get{this.Name}DataDto";

        public string GetSelectPageDtoName => $"Get{this.Name}PageDataDto";

        public string GetSaveDtoName => $"Save{this.Name}DataDto";

        public string GetDelDtoName => $"Del{this.Name}DataDto";

        public string GetViewModelName => $"Get{this.Name}ViewModel";

        public string DtoName { get; set; }

        //用于创建dto 和 viewmolde时使用，返回类名 end

        /// <summary>
        /// 前端用的类名
        /// </summary>
        public string FrontName
        {
            get
            {
                var names = Name.Split('_').Where(o => !string.IsNullOrWhiteSpace(o))
                    .Select(o => o.ToLower()).ToList();
                return string.Join("-", names);
            }
        }


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
        public string FLNameAll
        {
            get
            {
                var value = "";
                if (!string.IsNullOrWhiteSpace(FUName))
                {
                    value = FUName.ToLower();
                }
                return value;
            }
        }
        public string NameSpace { get; set; }
        public string NameSpacel
        {
            get
            {

                return NameSpace.ToLower();
            }
        }
        /// <summary>
        /// 前端用的命名空间
        /// </summary>
        public string FrontNameSpace
        {
            get
            {
                var np = NameSpace.Replace("EQuality.MES", "").Replace("EQuality", "");
                return string.Join("-", np.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.ToLower()));
            }
        }
        public String FrontFUNameSpace
        {
            get
            {
                var np = NameSpace.Replace("EQuality.MES", "").Replace("EQuality", "");
                return string.Join("", np.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Substring(0, 1).ToUpper() + o.Substring(1).ToLower()));
            }
        }
        public String FrontFLNameSpace
        {
            get
            {
                var np = NameSpace.Replace("EQuality.MES", "").Replace("EQuality", "");
                return string.Join("", np.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Substring(0, 1).ToLower() + o.Substring(1).ToLower()));
            }
        }
        public string Description { get; set; }
        public IList<PropertyInfo> Properties { get; set; }

        public IList<PropertyInfo> CreateProperties
        {
            get
            {
                var properties = new List<PropertyInfo>();
                foreach (var item in Properties)
                {
                    if (this.BaseEntityStr == "BaseEntity")
                    {
                        if (!BaseCreateProperties.Contains(item.Name))
                        {
                            properties.Add(item);
                        }
                    }
                    else
                    {
                        if (!RootCreateProperties.Contains(item.Name))
                        {
                            properties.Add(item);
                        }
                    }
                }
                return properties;
            }
        }
        public IList<string> BaseCreateProperties
        {
            get
            {
                IList<string> columns = new List<string>() {
                    "Id","IsDeleted","CreateId","CreateBy","CreateTime","ModifyId","ModifyBy","ModifyTime"
                };
                return columns;
            }
        }

        public IList<string> RootCreateProperties
        {
            get
            {
                IList<string> columns = new List<string>() {
                    "Id"
                };
                return columns;
            }
        }


        public bool IsDal { get; set; }
        public bool IsBll { get; set; }
        public bool IsWeb { get; set; }
        public bool IsModel { get; set; }
        public bool IsClinetWeb { get; set; }
        public bool IsckBasics { get; set; }

        public bool IsFront { get; set; }
        public string FrontPath { get; set; }
        public string BaseEntityStr { get; set; } = "BaseEntity";

        public IList<string> BaseProperties
        {
            get
            {
                IList<string> columns = new List<string>() {
                    "ID","IS_DELETE","CREATE_DT","CREATE_USER_ID","UPDATE_DT","UPDATE_USER_ID","CREATER","UPDATER"
                };
                return columns;
            }
        }
    }
}
