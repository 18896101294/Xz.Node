﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xz.Node.Repository;

namespace Xz.Node.Repository.Migrations
{
    [DbContext(typeof(XzDbContext))]
    [Migration("20210806031120_AddTestCode")]
    partial class AddTestCode
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Xz.Node.Framework.Model.SysTable", b =>
                {
                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("TableDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TableName")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("SysTables");
                });

            modelBuilder.Entity("Xz.Node.Framework.Model.SysTableColumn", b =>
                {
                    b.Property<string>("ColumnName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ColumnType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IsDisplay")
                        .HasColumnType("int");

                    b.Property<int?>("IsKey")
                        .HasColumnType("int");

                    b.Property<int?>("IsNull")
                        .HasColumnType("int");

                    b.Property<int?>("MaxLength")
                        .HasColumnType("int");

                    b.ToTable("SysTableColumns");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Auth.Auth_ModuleElementInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Attr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Class")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DomId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Icon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModuleId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Script")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.Property<string>("TypeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auth_ModuleElement");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Auth.Auth_ModuleInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CascadeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HotKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAutoExpand")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLeaf")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSys")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortNo")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Vector")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auth_Module");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Auth.Auth_OrgInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BizCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CascadeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CreateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HotKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAutoExpand")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLeaf")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortNo")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TypeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auth_Org");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Auth.Auth_RelevanceInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExtendInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OperateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("OperatorId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecondId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("ThirdId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auth_Relevance");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Auth.Auth_ResourceInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AppId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AppName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CascadeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreateUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortNo")
                        .HasColumnType("int");

                    b.Property<string>("TypeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auth_Resource");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Auth.Auth_RoleInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CreateId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TypeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auth_Role");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Auth.Auth_UserInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Account")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BizCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TypeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auth_User");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.System.System_ApplicationInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AppSecret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnType("bit");

                    b.Property<string>("Icon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("System_Application");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.System.System_DataPrivilegeRuleInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Enable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("PrivilegeRules")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortNo")
                        .HasColumnType("int");

                    b.Property<string>("SourceCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubSourceCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("System_DataPrivilegeRule");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.System.System_OpenJobInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreateUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cron")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ErrorCount")
                        .HasColumnType("int");

                    b.Property<string>("JobCall")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobCallParams")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("JobType")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastErrorTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastRunTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NextRunTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("OrgId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RunCount")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("System_OpenJob");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.System.System_SysLogInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Application")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Href")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ip")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Result")
                        .HasColumnType("int");

                    b.Property<string>("TypeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("System_SysLog");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_OaInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnName("Disable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TestOpForeignKey")
                        .HasColumnName("TestOpForeignKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TestOpForeignKey")
                        .IsUnique();

                    b.ToTable("Test_Oa");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_ObInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnName("Disable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TestOpForeignKey")
                        .HasColumnName("TestOpForeignKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TestOpForeignKey");

                    b.ToTable("Test_Ob");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_OcInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnName("Disable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TestOpForeignKey")
                        .HasColumnName("TestOpForeignKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TestOpForeignKey")
                        .IsUnique();

                    b.ToTable("Test_Oc");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_OmInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Test_Om");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_OnInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Test_On");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_On_OmInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<Guid>("TestOmKey")
                        .HasColumnName("TestOmKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TestOnKey")
                        .HasColumnName("TestOnKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TestOmKey");

                    b.HasIndex("TestOnKey");

                    b.ToTable("Test_On_Om");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_OpInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AppSecret")
                        .HasColumnName("AppSecret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnName("CreateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Creater")
                        .HasColumnName("Creater")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnName("Disable")
                        .HasColumnType("bit");

                    b.Property<string>("Icon")
                        .HasColumnName("Icon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnName("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnName("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdateUserId")
                        .HasColumnName("UpdateUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Updater")
                        .HasColumnName("Updater")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Test_Op");
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_OaInfo", b =>
                {
                    b.HasOne("Xz.Node.Repository.Domain.Test.Test_OpInfo", "TestOp")
                        .WithOne("TestOa")
                        .HasForeignKey("Xz.Node.Repository.Domain.Test.Test_OaInfo", "TestOpForeignKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_ObInfo", b =>
                {
                    b.HasOne("Xz.Node.Repository.Domain.Test.Test_OpInfo", "TestOp")
                        .WithMany("Test_Obs")
                        .HasForeignKey("TestOpForeignKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_OcInfo", b =>
                {
                    b.HasOne("Xz.Node.Repository.Domain.Test.Test_OpInfo", "TestOp")
                        .WithOne("TestOc")
                        .HasForeignKey("Xz.Node.Repository.Domain.Test.Test_OcInfo", "TestOpForeignKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xz.Node.Repository.Domain.Test.Test_On_OmInfo", b =>
                {
                    b.HasOne("Xz.Node.Repository.Domain.Test.Test_OmInfo", "TestOm")
                        .WithMany("Test_On_Oms")
                        .HasForeignKey("TestOmKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Xz.Node.Repository.Domain.Test.Test_OnInfo", "TestOn")
                        .WithMany("Test_On_Oms")
                        .HasForeignKey("TestOnKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
