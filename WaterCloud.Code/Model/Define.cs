﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WaterCloud.Code
{
    public static class Define
    {
        public const string DBTYPE_SQLSERVER = "System.Data.SqlClient";    //sql server
        public const string DBTYPE_MYSQL = "MySql.Data.MySqlClient";    //sql server
        public const string DBTYPE_ORACLE = "Oracle.ManagedDataAccess.Client";    //oracle

        
        public const string SYSTEM_USERNAME = "admin";    //超级管理员
        public const string SYSTEM_USERPWD = "0000";
        public const string SYSTEM_MASTERPROJECT = "d69fd66a-6a77-4011-8a25-53a79bdf5001";

        public const string PROVIDER_COOKIE = "Cookie";
        public const string PROVIDER_SESSION = "Session";
        public const string PROVIDER_WEBAPI = "WebApi";

        public const string CACHEPROVIDER_REDIS = "Redis";
        public const string CACHEPROVIDER_MEMORY = "Memory";

        public const string DATAPRIVILEGE_LOGINUSER = "{loginUser}";  //数据权限配置中，当前登录用户的key
        public const string DATAPRIVILEGE_LOGINROLE = "{loginRole}";  //数据权限配置中，当前登录用户角色的key
        public const string DATAPRIVILEGE_LOGINORG = "{loginOrg}";  //数据权限配置中，当前登录用户部门的key
    }
}
