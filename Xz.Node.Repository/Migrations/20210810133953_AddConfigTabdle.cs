using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Xz.Node.Repository.Migrations
{
    public partial class AddConfigTabdle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auth_Module",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CascadeId = table.Column<string>(nullable: true, comment: "节点语义ID"),
                    Name = table.Column<string>(nullable: true, comment: "名称"),
                    ParentId = table.Column<string>(nullable: true, comment: "父节点流水号"),
                    ParentName = table.Column<string>(nullable: true, comment: "父节点名称"),
                    Url = table.Column<string>(nullable: true, comment: "主页面URL"),
                    HotKey = table.Column<string>(nullable: true, comment: "热键"),
                    IsLeaf = table.Column<bool>(nullable: false, comment: "是否叶子节点"),
                    IsAutoExpand = table.Column<bool>(nullable: false, comment: "是否自动展开"),
                    IconName = table.Column<string>(nullable: true, comment: "节点图标文件名称"),
                    Status = table.Column<int>(nullable: false, comment: "当前状态"),
                    Vector = table.Column<string>(nullable: true, comment: "矢量图标"),
                    SortNo = table.Column<int>(nullable: false, comment: "排序号"),
                    Code = table.Column<string>(nullable: true, comment: "模块标识"),
                    IsSys = table.Column<bool>(nullable: false, comment: "是否系统模块")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Module", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_ModuleElement",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DomId = table.Column<string>(nullable: true, comment: "DOM ID"),
                    Name = table.Column<string>(nullable: true, comment: "名称"),
                    Attr = table.Column<string>(nullable: true, comment: "元素附加属性"),
                    Script = table.Column<string>(nullable: true, comment: "元素调用脚本"),
                    Icon = table.Column<string>(nullable: true, comment: "元素图标"),
                    Class = table.Column<string>(nullable: true, comment: "元素样式"),
                    Remark = table.Column<string>(nullable: true, comment: "备注"),
                    Sort = table.Column<int>(nullable: false, comment: "排序字段"),
                    ModuleId = table.Column<string>(nullable: true, comment: "功能模块Id"),
                    TypeName = table.Column<string>(nullable: true, comment: "分类名称"),
                    TypeId = table.Column<string>(nullable: true, comment: "分类ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_ModuleElement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Org",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CascadeId = table.Column<string>(nullable: true, comment: "节点语义ID"),
                    Name = table.Column<string>(nullable: true, comment: "名称"),
                    ParentId = table.Column<string>(nullable: true, comment: "父节点流水号"),
                    ParentName = table.Column<string>(nullable: true, comment: "父节点名称"),
                    HotKey = table.Column<string>(nullable: true, comment: "热键"),
                    IsLeaf = table.Column<bool>(nullable: false, comment: "是否叶子节点"),
                    IsAutoExpand = table.Column<bool>(nullable: false, comment: "是否自动展开"),
                    IconName = table.Column<string>(nullable: true, comment: "节点图标文件名称"),
                    Status = table.Column<int>(nullable: false, comment: "当前状态"),
                    BizCode = table.Column<string>(nullable: true, comment: "业务对照码"),
                    CustomCode = table.Column<string>(nullable: true, comment: "自定义扩展码"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    CreateId = table.Column<int>(nullable: false, comment: "创建人ID"),
                    SortNo = table.Column<int>(nullable: false, comment: "排序号"),
                    TypeName = table.Column<string>(nullable: true, comment: "分类名称"),
                    TypeId = table.Column<string>(nullable: true, comment: "分类ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Org", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Relevance",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true, comment: "描述"),
                    Key = table.Column<string>(nullable: true, comment: "映射标识"),
                    Status = table.Column<int>(nullable: false, comment: "状态"),
                    OperateTime = table.Column<DateTime>(nullable: false, comment: "授权时间"),
                    OperatorId = table.Column<string>(nullable: true, comment: "授权人"),
                    FirstId = table.Column<string>(nullable: true, comment: "第一个表主键ID"),
                    SecondId = table.Column<string>(nullable: true, comment: "第二个表主键ID"),
                    ThirdId = table.Column<string>(nullable: true, comment: "第三个主键"),
                    ExtendInfo = table.Column<string>(nullable: true, comment: "扩展信息")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Relevance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Resource",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CascadeId = table.Column<string>(nullable: true, comment: "节点语义ID"),
                    Name = table.Column<string>(nullable: true, comment: "名称"),
                    ParentId = table.Column<string>(nullable: true, comment: "父节点流水号"),
                    ParentName = table.Column<string>(nullable: true, comment: "父节点名称"),
                    SortNo = table.Column<int>(nullable: false, comment: "排序号"),
                    Description = table.Column<string>(nullable: true, comment: "描述"),
                    AppId = table.Column<string>(nullable: true, comment: "资源所属应用ID"),
                    AppName = table.Column<string>(nullable: true, comment: "所属应用名称"),
                    TypeName = table.Column<string>(nullable: true, comment: "分类名称"),
                    TypeId = table.Column<string>(nullable: true, comment: "分类ID"),
                    Disable = table.Column<bool>(nullable: false, comment: "是否可用"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<string>(nullable: true, comment: "创建人ID"),
                    CreateUserName = table.Column<string>(nullable: true, comment: "创建人"),
                    UpdateTime = table.Column<DateTime>(nullable: true, comment: "最后更新时间"),
                    UpdateUserId = table.Column<string>(nullable: true, comment: "最后更新人ID"),
                    UpdateUserName = table.Column<string>(nullable: true, comment: "最后更新人")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Resource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Role",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true, comment: "角色名称"),
                    Status = table.Column<int>(nullable: false, comment: "当前状态"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    CreateId = table.Column<string>(nullable: true, comment: "创建人ID"),
                    TypeName = table.Column<string>(nullable: true, comment: "分类名称"),
                    TypeId = table.Column<string>(nullable: true, comment: "分类ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Account = table.Column<string>(nullable: true, comment: "用户登录帐号"),
                    Password = table.Column<string>(nullable: true, comment: "密码"),
                    Name = table.Column<string>(nullable: true, comment: "用户姓名"),
                    Sex = table.Column<int>(nullable: false, comment: "性别"),
                    Status = table.Column<int>(nullable: false, comment: "用户状态"),
                    BizCode = table.Column<string>(nullable: true, comment: "业务对照码"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "经办时间"),
                    CreateId = table.Column<string>(nullable: true, comment: "创建人"),
                    TypeName = table.Column<string>(nullable: true, comment: "分类名称"),
                    TypeId = table.Column<string>(nullable: true, comment: "分类ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysTableColumns",
                columns: table => new
                {
                    ColumnName = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ColumnType = table.Column<string>(nullable: true),
                    MaxLength = table.Column<int>(nullable: true),
                    IsNull = table.Column<int>(nullable: true),
                    IsDisplay = table.Column<int>(nullable: true),
                    IsKey = table.Column<int>(nullable: true),
                    EntityType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SysTables",
                columns: table => new
                {
                    TableName = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    TableDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "System_Application",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true, comment: "应用名称"),
                    AppSecret = table.Column<string>(nullable: true, comment: "应用密钥"),
                    Description = table.Column<string>(nullable: true, comment: "应用描述"),
                    Icon = table.Column<string>(nullable: true, comment: "应用图标"),
                    Disable = table.Column<bool>(nullable: false, comment: "是否可用"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建日期"),
                    CreateUser = table.Column<string>(nullable: true, comment: "创建人")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "System_Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    Value = table.Column<string>(nullable: true, comment: "值"),
                    Text = table.Column<string>(nullable: true, comment: "值显示值"),
                    DisplayNo = table.Column<int>(nullable: false, comment: "顺序号"),
                    Category = table.Column<string>(nullable: true, comment: "分类"),
                    Remark = table.Column<string>(nullable: true, comment: "备注"),
                    IsHide = table.Column<bool>(nullable: false, comment: "是否隐藏")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "System_DataPrivilegeRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    SourceCode = table.Column<string>(nullable: true, comment: "模块编号"),
                    SubSourceCode = table.Column<string>(nullable: true, comment: "二级资源标识"),
                    Description = table.Column<string>(nullable: true, comment: "权限描述"),
                    SortNo = table.Column<int>(nullable: false, comment: "排序号"),
                    PrivilegeRules = table.Column<string>(nullable: true, comment: "权限规则"),
                    Enable = table.Column<bool>(nullable: false, comment: "是否可用")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_DataPrivilegeRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "System_OpenJob",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    JobName = table.Column<string>(nullable: true, comment: "任务名称"),
                    RunCount = table.Column<int>(nullable: false, comment: "任务执行次数"),
                    ErrorCount = table.Column<int>(nullable: false, comment: "异常次数"),
                    NextRunTime = table.Column<DateTime>(nullable: true, comment: "下次执行时间"),
                    LastRunTime = table.Column<DateTime>(nullable: true, comment: "最后一次执行时间"),
                    LastErrorTime = table.Column<DateTime>(nullable: true, comment: "最后一次失败时间"),
                    JobType = table.Column<int>(nullable: false, comment: "任务执行方式0：本地任务；1：外部接口任务"),
                    JobCall = table.Column<string>(nullable: true, comment: "任务地址"),
                    JobCallParams = table.Column<string>(nullable: true, comment: "任务参数，JSON格式"),
                    Cron = table.Column<string>(nullable: true, comment: "CRON表达式"),
                    Status = table.Column<int>(nullable: false, comment: "任务运行状态（0：停止，1：正在运行，2：暂停）"),
                    Remark = table.Column<string>(nullable: true, comment: "备注"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<string>(nullable: true, comment: "创建人ID"),
                    CreateUserName = table.Column<string>(nullable: true, comment: "创建人"),
                    UpdateTime = table.Column<DateTime>(nullable: true, comment: "最后更新时间"),
                    UpdateUserId = table.Column<string>(nullable: true, comment: "最后更新人ID"),
                    UpdateUserName = table.Column<string>(nullable: true, comment: "最后更新人"),
                    OrgId = table.Column<string>(nullable: true, comment: "所属部门")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_OpenJob", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "System_SysLog",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true, comment: "日志内容"),
                    TypeName = table.Column<string>(nullable: true, comment: "分类名称"),
                    TypeId = table.Column<string>(nullable: true, comment: "分类ID"),
                    Href = table.Column<string>(nullable: true, comment: "操作所属模块地址"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "记录时间"),
                    CreateId = table.Column<string>(nullable: true, comment: "操作人ID"),
                    CreateName = table.Column<string>(nullable: true, comment: "操作人"),
                    Ip = table.Column<string>(nullable: true, comment: "操作机器的IP地址"),
                    Result = table.Column<int>(nullable: false, comment: "操作的结果：0：成功；1：失败；"),
                    Application = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_SysLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Test_Code",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    Column1 = table.Column<string>(nullable: true, comment: "字段1"),
                    Column2 = table.Column<string>(nullable: true, comment: "字段2"),
                    Column3 = table.Column<string>(nullable: true, comment: "字段3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Code", x => x.Id);
                },
                comment: "测试类");

            migrationBuilder.CreateTable(
                name: "Test_Om",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    Name = table.Column<string>(nullable: true, comment: "应用名称")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Om", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Test_On",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    Name = table.Column<string>(nullable: true, comment: "应用名称")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_On", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Test_Op",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    Name = table.Column<string>(nullable: true, comment: "应用名称"),
                    AppSecret = table.Column<string>(nullable: true, comment: "应用密钥"),
                    Description = table.Column<string>(nullable: true, comment: "应用描述"),
                    Icon = table.Column<string>(nullable: true, comment: "应用图标"),
                    Disable = table.Column<bool>(nullable: false, comment: "是否可用")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Op", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Test_On_Om",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    TestOnKey = table.Column<Guid>(nullable: false, comment: "TestOn主键"),
                    TestOmKey = table.Column<Guid>(nullable: false, comment: "TestOm主键")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_On_Om", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_On_Om_Test_Om_TestOmKey",
                        column: x => x.TestOmKey,
                        principalTable: "Test_Om",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Test_On_Om_Test_On_TestOnKey",
                        column: x => x.TestOnKey,
                        principalTable: "Test_On",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Test_Oa",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    TestOpForeignKey = table.Column<Guid>(nullable: false, comment: "TestOpForeignKey"),
                    Name = table.Column<string>(nullable: true, comment: "应用名称"),
                    Description = table.Column<string>(nullable: true, comment: "应用描述"),
                    Disable = table.Column<bool>(nullable: false, comment: "是否可用")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Oa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_Oa_Test_Op_TestOpForeignKey",
                        column: x => x.TestOpForeignKey,
                        principalTable: "Test_Op",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Test_Ob",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    TestOpForeignKey = table.Column<Guid>(nullable: false, comment: "TestOpForeignKey"),
                    Name = table.Column<string>(nullable: true, comment: "应用名称"),
                    Description = table.Column<string>(nullable: true, comment: "应用描述"),
                    Disable = table.Column<bool>(nullable: false, comment: "是否可用")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Ob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_Ob_Test_Op_TestOpForeignKey",
                        column: x => x.TestOpForeignKey,
                        principalTable: "Test_Op",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Test_Oc",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, comment: "Id"),
                    IsDelete = table.Column<bool>(nullable: false, comment: "是否物理删除"),
                    Creater = table.Column<string>(nullable: true, comment: "创建用户"),
                    CreateUserId = table.Column<Guid>(nullable: false, comment: "创建用户Id"),
                    CreateTime = table.Column<DateTime>(nullable: false, comment: "创建时间"),
                    Updater = table.Column<string>(nullable: true, comment: "更新用户"),
                    UpdateUserId = table.Column<Guid>(nullable: false, comment: "更新用户Id"),
                    UpdateTime = table.Column<DateTime>(nullable: false, comment: "更新时间"),
                    TestOpForeignKey = table.Column<Guid>(nullable: false, comment: "TestOpForeignKey"),
                    Name = table.Column<string>(nullable: true, comment: "应用名称"),
                    Description = table.Column<string>(nullable: true, comment: "应用描述"),
                    Disable = table.Column<bool>(nullable: false, comment: "是否可用")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Oc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_Oc_Test_Op_TestOpForeignKey",
                        column: x => x.TestOpForeignKey,
                        principalTable: "Test_Op",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Test_Oa_TestOpForeignKey",
                table: "Test_Oa",
                column: "TestOpForeignKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Test_Ob_TestOpForeignKey",
                table: "Test_Ob",
                column: "TestOpForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_Test_Oc_TestOpForeignKey",
                table: "Test_Oc",
                column: "TestOpForeignKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Test_On_Om_TestOmKey",
                table: "Test_On_Om",
                column: "TestOmKey");

            migrationBuilder.CreateIndex(
                name: "IX_Test_On_Om_TestOnKey",
                table: "Test_On_Om",
                column: "TestOnKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auth_Module");

            migrationBuilder.DropTable(
                name: "Auth_ModuleElement");

            migrationBuilder.DropTable(
                name: "Auth_Org");

            migrationBuilder.DropTable(
                name: "Auth_Relevance");

            migrationBuilder.DropTable(
                name: "Auth_Resource");

            migrationBuilder.DropTable(
                name: "Auth_Role");

            migrationBuilder.DropTable(
                name: "Auth_User");

            migrationBuilder.DropTable(
                name: "SysTableColumns");

            migrationBuilder.DropTable(
                name: "SysTables");

            migrationBuilder.DropTable(
                name: "System_Application");

            migrationBuilder.DropTable(
                name: "System_Configuration");

            migrationBuilder.DropTable(
                name: "System_DataPrivilegeRule");

            migrationBuilder.DropTable(
                name: "System_OpenJob");

            migrationBuilder.DropTable(
                name: "System_SysLog");

            migrationBuilder.DropTable(
                name: "Test_Code");

            migrationBuilder.DropTable(
                name: "Test_Oa");

            migrationBuilder.DropTable(
                name: "Test_Ob");

            migrationBuilder.DropTable(
                name: "Test_Oc");

            migrationBuilder.DropTable(
                name: "Test_On_Om");

            migrationBuilder.DropTable(
                name: "Test_Op");

            migrationBuilder.DropTable(
                name: "Test_Om");

            migrationBuilder.DropTable(
                name: "Test_On");
        }
    }
}
