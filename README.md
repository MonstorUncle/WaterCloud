<p></p>
<p></p>

<p align="center">
<img src="https://watercloud.vip/icon/favicon.ico" height="80"/>
</p>
<div align="center">

[![star](https://gitee.com/qian_wei_hong/WaterCloud/badge/star.svg)](https://gitee.com/qian_wei_hong/WaterCloud/stargazers) 
[![fork](https://gitee.com/qian_wei_hong/WaterCloud/badge/fork.svg)](https://gitee.com/qian_wei_hong/WaterCloud/members)
![](https://img.shields.io/badge/release-2.7.0-red)

![](https://img.shields.io/badge/.net%20core-3.1.0-blue)
![](https://img.shields.io/badge/Chloe-4.8.1-blue)
![](https://img.shields.io/badge/layui-2.5.7-blue)

![](https://img.shields.io/badge/Quartz-3.2.4-brightgreen)
![](https://img.shields.io/badge/Autofac-6.1.0-brightgreen)
![](https://img.shields.io/badge/SignalR-4.2.2-brightgreen)
</div>

----
# WaterCloud

#### 介绍

- 请勿用于违反我国法律的web平台、如诈骗等非法平台网站。
- WaterCloud是一套基于ASP.NET Core 3.1  MVC+Chloe+LayUI的框架，源代码完全开源，可以帮助你解决C#.NET项目的重复工作！
- 采用主流框架，容易上手，简单易学，学习成本低。
- 可完全实现二次开发让开发更多关注业务逻辑。既能快速提高开发效率，帮助公司节省人力成本，同时又不失灵活性。
- 支持SQLServer、MySQL、Oracle 等多数据库类型。模块化设计，层次结构清晰。内置一系列企业信息管理的基础功能。
- 操作权限基于RBAC，权限控制精密细致，对所有管理链接都进行权限验证，可控制到导航菜单、功能按钮，控制到行级，列表级，表单字段级，不同人对同一个页面的操作不同。
- 数据权限,精细化数据权限控制，实现不同人看不同数据。
- 代码生成功能，简单前后端代码生成。
- 表单设计器，提供多种方式设计表单，动态表单拖拉式设计以及自定义表单。
- 流程设计器，动态设计流程，节点及连线条件设计。
- 内容管理，已配置好UEditor编辑器，可以使用。
- 文件管理，提供文件上传及下载功能。
- 提高开发效率及质量。常用类封装，日志、缓存、验证、字典、文件、邮件、Excel。等等。
- 页面为响应式设计，支持电脑、平板、智能手机等设备，微信浏览器以及各种常见浏览器。
- 适用范围：可以开发OA、ERP、BPM、CRM、WMS、TMS、MIS、BI、电商平台后台、物流管理系统、快递管理系统、教务管理系统等各类管理软件。
- 租户管理(未实现，理论阶段)


- .net4.5版地址：https://gitee.com/qian_wei_hong/WaterCloud/tree/net4.5
- .net5版地址：https://gitee.com/qian_wei_hong/WaterCloud/tree/net5
- 项目演示地址：http://106.75.169.226:5000/  （账号：admin 密码：0000，数据库2个小时还原一次）
- 文档地址：https://gitee.com/qian_wei_hong/WaterCloud/wikis/pages

#### 前端以及后端使用技术介绍

1、前端技术

- js框架：jquery-3.4.1、LayUI、LayUI mini（开源）。
- 图标：Font Awesome 4.7.0。
- 客户端验证：LayUI verify。
- 富文本编辑器：开源wangEditor、百度UEditor、layui edit。
- 上传文件：开源zyupload、layui upoload。
- 动态页签：LayUI mini miniTab。
- 数据表格：LayUI table、LayUI 开源 TalbePlug。
- 下拉选择框：LayUI select、LayUI 开源 TalbePlug(optimizeSelectOption)、xmselect。
- 树结构控件：LayUI 开源 dtree。
- 树状表格：LayUI 开源 treetable-lay。
- 穿梭框：LayUI transfer。
- 页面布局：LayUI、LayUI mini。
- 图表插件：echarts
- 日期控件：LayUI laydate
- 图标选择：LayUI 开源 IconPicker
- 颜色选择：paigusu
- 省市区选择：LayUI 开源 layarea

2、后端技术

- 核心框架：ASP.NET Core 3.1、WEB API
- 定时任务：QuartZ，实现web控制(WaterCloud.Service/AutoJob/Job下新建job，web端创建定时任务)
- 持久层框架：Chloe（支持多种数据库，复杂查询操作）
- 安全支持：过滤器、Sql注入、请求伪造
- 服务端验证：实体模型验证
- 缓存框架：Redis/Memory（单点登录控制）
- 日志管理：Log、登录日志、操作日志
- 工具类：NPOI、Newtonsoft.Json、验证码、丰富公共类似
- 其他：AutoFac、Swagger


#### 环境要求

1. VS2019及以上版本；
2. Asp.net Core 3.1；
3. Mysql或者SQLSERVER2005及以上版本，database文件夹下有sql文件可执行；
4. 请使用VS2019及以上版本打开解决方案。

#### 使用说明

1. 前端参考Layui 官方文档：https://www.layui.com/doc/
2. Layui框架参考Layuimini 码云地址：https://gitee.com/zhongshaofa/layuimini
3. Chole.ORM 文档地址：http://www.52chloe.com/Wiki/Document
4. WaterCloud讨论交流QQ群（1065447456）
5. 多租户可以使用Furion框架实现，码云地址：https://gitee.com/monksoul/Furion

#### 捐赠支持

开源项目不易，若此项目能得到你的青睐，可以捐赠支持作者持续开发与维护，感谢所有支持开源的朋友。


![输入图片说明](https://images.gitee.com/uploads/images/2020/0331/144842_7cf04ad6_7353672.jpeg "1585637076201.jpg")          ![输入图片说明](https://images.gitee.com/uploads/images/2020/0331/144852_8b26c8cb_7353672.png "mm_facetoface_collect_qrcode_1585637044089.png")