/**
 * date:2020/09/29
 * author:Mr.Q
 * version:1.6
 * description:watercloud 主体框架扩展
 */
layui.define(["jquery", "layer", 'table', 'soulTable', 'common', 'tabletree'], function (exports) {
    var $ = layui.jquery,
        layer = layui.layer,     
        soulTable = layui.soulTable,       
        tabletree = layui.tabletree,       
        common = layui.common,       
        table = layui.table;

    var obj = {
        //table渲染封装里面有字段权限
        rendertable: function (options) {
            var defaults = {
                elem: '#currentTableId',//主键
                toolbar: '#toolbarDemo',//工具栏
                defaultToolbar: ['filter', 'exports', 'print'],//默认工具栏
                search: true,//搜索按钮
                method: 'get',//请求方法
                cellMinWidth: 100,//最小宽度
                limit: 10,//每页数据 默认
                limits: [10, 20, 30, 40, 50],
                id:'currentTableId',
                height: 'full-60',
                loading: false,
                page: { //支持传入 laypage 组件的所有参数（某些参数除外，如：jump/elem） - 详见文档
                    layout: ['skip', 'prev', 'page', 'next', 'limit', 'count'] //自定义分页布局
                    ,curr: 1 //设定初始在第 1 页
                    , groups: 3 //只显示 1 个连续页码
                    , first: false //不显示首页
                    , last: false //不显示尾页
                },
                authorizeFields: true, // 字段权限开关
                autoColumnWidth: true,
                overflow: { // 默认所有表格都超出
                    type: 'tips'
                    , hoverTime: 300 // 悬停时间，单位ms, 悬停 hoverTime 后才会显示，默认为 0
                    , color: 'black' // 字体颜色
                    , bgColor: 'white' // 背景色
                    , header: true, // 表头支持 overflow
                    total: true // 合计行支持 overflow
                },
                contextmenu: {
                    header: [
                        {
                            name: '复制',
                            icon: 'layui-icon layui-icon-template',
                            click: function (obj) {
                                soulTable.copy(obj.text)
                                layer.msg('复制成功！')
                            }
                        },
                        {
                            name: '导出excel',
                            icon: 'layui-icon layui-icon-download-circle',
                            click: function () {
                                soulTable.export(this.id)
                            }
                        },
                        {
                            name: '重载表格',
                            icon: 'layui-icon layui-icon-refresh-1',
                            click: function () {
                                table.reload(this.id)
                            }
                        }
                    ],
                    // 表格内容右键菜单配置
                    body: [
                        {
                            name: '复制',
                            icon: 'layui-icon layui-icon-template',
                            click: function (obj) {
                                soulTable.copy(obj.text)
                                layer.msg('复制成功！')
                            }
                        }
                    ],
                    // 合计栏右键菜单配置
                    total: [{
                        name: '复制',
                        icon: 'layui-icon layui-icon-template',
                        click: function (obj) {
                            soulTable.copy(obj.text)
                            layer.msg('复制成功！')
                        }
                    }]
                },
                excel: {
                    filename: '表格信息' + new Date().formatDate() + '.xlsx',
                },
                request: {
                    pageName: 'page' //页码的参数名称，默认：page
                    , limitName: 'rows' //每页数据量的参数名，默认：limit
                },
                parseData: function (res) { //res 即为原始返回的数据
                    return {
                        "code": res.state, //解析接口状态
                        "msg": res.message, //解析提示文本
                        "count": res.count, //解析数据长度
                        "data": res.data //解析数据列表
                    };
                }
            };
            var doneCallback = options.done;
            var options = $.extend(defaults, options);
            if (document.body.clientWidth < 500 && !!options.defaultToolbar) {
                for (var i = 0; i < options.defaultToolbar.length; i++) {
                    if (options.defaultToolbar[i] == "print") {
                        options.defaultToolbar.splice(i, 1);
                    }
                }
            }
            if (options.search) {
                options.defaultToolbar = !options.defaultToolbar ? [] : options.defaultToolbar;
                options.defaultToolbar.push({
                    title: '搜索',
                    layEvent: 'TABLE_SEARCH',
                    icon: 'layui-icon-search'
                });
            }  
            //ie缓存问题
            options.url = common.urlAddTime(options.url);
            //字段权限
            if (options.authorizeFields) {
                options.cols = common.tableAuthorizeFields(options.cols);
            }
            options.done = function (res, curr, count) {
                //关闭加载
                //layer.closeAll('loading');
                //固定列引发的问题
                //table.resize(options.id);
                //obj.tableResize(options.id);
                if (doneCallback) {
                    doneCallback(res, curr, count);
                }
                soulTable.render(this);
                table.resize();
            };
            return table.render(options);
        },
        //table刷新
        reloadtable: function (options) {
            layer.load(0, { shade: false });
            var defaults = {
                elem: 'currentTableId',//主键
                page: true,//分页参数
                curr: 1,//当前页
                where: {}
            };
            var options = $.extend(defaults, options);
            options.where.time = new Date().Format("yyyy-MM-dd hh:mm:ss");
            if (options.page) {
                if (options.page == true) {
                    table.reload(options.elem, {
                        page: { //支持传入 laypage 组件的所有参数（某些参数除外，如：jump/elem） - 详见文档
                            layout: ['skip', 'prev', 'page', 'next', 'limit', 'count'] //自定义分页布局
                            , groups: 3 //只显示 1 个连续页码
                            , first: false //不显示首页
                            , last: false, //不显示尾页
                            curr: options.curr
                        }
                        , where: options.where
                    }, 'data');
                }
                else {
                    table.reload(options.elem, {
                        page: options.page
                        , where: options.where
                    }, 'data');
                }
            }
            else {
                //执行搜索重载
                table.reload(options.elem, {
                    where: options.where
                }, 'data');
            }
            //关闭加载
            layer.closeAll('loading');
        },
        //tabletree渲染封装里面有字段权限
        rendertreetable: function (options) {
            //样式不协调，先不加
            var defaults = {
                elem: '#currentTableId',//主键
                toolbar: '#toolbarDemo',//工具栏
                defaultToolbar: ['filter', 'exports', 'print'],//默认工具栏
                search: true,//搜索按钮
                loading: false,
                treeIdName: 'F_Id',  // id字段名称
                treePidName: 'F_ParentId',     // pid字段名称
                filter:true,
                treeColIndex: 1,
                treeSpid: 0,
                limit: 99999,//每页数据 默认
                page: { //支持传入 laypage 组件的所有参数（某些参数除外，如：jump/elem） - 详见文档
                    layout: ['count'] //自定义分页布局
                    , first: false //不显示首页
                    , last: false //不显示尾页
                },
                height: 'full-60',
                method: 'get',//请求方法
                cellMinWidth: 60,//最小宽度
                authorizeFields: true, // 字段权限开关
                overflow: { // 默认所有表格都超出
                    type: 'tips'
                    , hoverTime: 300 // 悬停时间，单位ms, 悬停 hoverTime 后才会显示，默认为 0
                    , color: 'black' // 字体颜色
                    , bgColor: 'white' // 背景色
                    , header: true, // 表头支持 overflow
                    total: true // 合计行支持 overflow
                },
                contextmenu: {
                    header: [
                        {
                            name: '复制',
                            icon: 'layui-icon layui-icon-template',
                            click: function (obj) {
                                soulTable.copy(obj.text)
                                layer.msg('复制成功！')
                            }
                        },
                        {
                            name: '导出excel',
                            icon: 'layui-icon layui-icon-download-circle',
                            click: function () {
                                soulTable.export(this.id)
                            }
                        },
                        {
                            name: '重载表格',
                            icon: 'layui-icon layui-icon-refresh-1',
                            click: function () {
                                table.reload(this.id)
                            }
                        }
                    ],
                    // 表格内容右键菜单配置
                    body: [
                        {
                            name: '复制',
                            icon: 'layui-icon layui-icon-template',
                            click: function (obj) {
                                soulTable.copy(obj.text)
                                layer.msg('复制成功！')
                            }
                        }
                    ],
                    // 合计栏右键菜单配置
                    total: [{
                        name: '复制',
                        icon: 'layui-icon layui-icon-template',
                        click: function (obj) {
                            soulTable.copy(obj.text)
                            layer.msg('复制成功！')
                        }
                    }]
                },
                excel: {
                    filename: '表格信息' + new Date().formatDate() + '.xlsx',
                },
                parseData: function (res) { //res 即为原始返回的数据
                    return {
                        "code": res.state, //解析接口状态
                        "msg": res.message, //解析提示文本
                        "count": res.count, //解析数据长度
                        "data": res.data //解析数据列表
                    };
                }
            };
            var doneCallback = options.done;
            var options = $.extend(defaults, options);
            if (document.body.clientWidth < 500 && !!options.defaultToolbar) {
                for (var i = 0; i < options.defaultToolbar.length; i++) {
                    if (options.defaultToolbar[i] == "print") {
                        options.defaultToolbar.splice(i, 1);
                    }
                }
            }
            //搜索框按钮
            if (options.search) {
                options.defaultToolbar = !options.defaultToolbar ? [] : options.defaultToolbar;
                options.defaultToolbar.push({
                    title: '搜索',
                    layEvent: 'TABLE_SEARCH',
                    icon: 'layui-icon-search'
                });
            }
            //ie缓存问题
            options.url = common.urlAddTime(options.url);
            //字段权限
            if (options.authorizeFields) {
                options.cols = common.tableAuthorizeFields(options.cols);
            }
            options.done = function (res, curr, count) {
                //关闭加载
                //layer.closeAll('loading');
                if (doneCallback) {
                    doneCallback(res, curr, count);
                }
                table.resize();
            };
            tabletree.render(options);
            return options;
        },
        //table行点击事件及按钮显示控制
        tableRowClick: function (type, tableFilter, tableId, oneList, moreList) {
            var oneList = !!oneList ? oneList : [];
            var moreList = !!moreList ? moreList : [];
            //type是checkbox或者radio
            $(document).on("click", ".layui-table-body table.layui-table tbody tr", function () {
                var obj = event ? event.target : event.srcElement;
                // 获取事件名称
                var tag = obj.tagName;
                var index = $(this).attr('data-index');
                var tableBox = $(this).parents('.layui-table-box');
                //存在固定列
                if (tableBox.find(".layui-table-fixed.layui-table-fixed-l").length > 0) {
                    tableDiv = tableBox.find(".layui-table-fixed.layui-table-fixed-l");
                } else {
                    tableDiv = tableBox.find(".layui-table-body.layui-table-main");
                }
                var checkCell = tableDiv.find("tr[data-index=" + index + "]").find("td div.laytable-cell-" + type + " div.layui-form-" + type + " I");
                if (checkCell.length > 0) {
                    //div和td生效
                    if (tag == 'DIV' || tag == "TD") {
                        checkCell.click();
                    }
                }
            });
            //对td的单击事件进行拦截停止，防止事件冒泡再次触发上述的单击事件（Table的单击行事件不会拦截，依然有效）
            $(document).on("click", "td div.laytable-cell-" + type + " div.layui-form-" + type + "", function (e) {
                e.stopPropagation();
            });
            table.on('row(' + tableFilter+')', function (obj) {
                obj.tr.addClass('layui-table-click').siblings().removeClass('layui-table-click');
            })
            //多选框监听
            table.on(type+'(' + tableFilter + ')', function (obj) {
                //控制按钮
                var data = table.checkStatus(tableId).data;
                if (obj.type == "all") {
                    if (obj.checked && table.cache[tableId].length != 0) {
                        if (table.cache[tableId].length > 1) {
                            for (var i = 0; i < oneList.length; i++) {
                                $('[name="' + oneList[i]+'"]').addClass("layui-hide");
                            }
                            for (var i = 0; i < moreList.length; i++) {
                                $('[name="' + moreList[i] + '"]').removeClass("layui-hide");
                            }
                        }
                        else {
                            for (var i = 0; i < oneList.length; i++) {
                                $('[name="' + oneList[i] + '"]').removeClass("layui-hide");
                            }
                            for (var i = 0; i < moreList.length; i++) {
                                $('[name="' + moreList[i] + '"]').removeClass("layui-hide");
                            }
                        }
                    }
                    else {
                        for (var i = 0; i < oneList.length; i++) {
                            $('[name="' + oneList[i] + '"]').addClass("layui-hide");
                        }
                        for (var i = 0; i < moreList.length; i++) {
                            $('[name="' + moreList[i] + '"]').addClass("layui-hide");
                        }
                    }
                }
                else {
                    if (data.length > 1) {
                        for (var i = 0; i < oneList.length; i++) {
                            $('[name="' + oneList[i] + '"]').addClass("layui-hide");
                        }
                        for (var i = 0; i < moreList.length; i++) {
                            $('[name="' + moreList[i] + '"]').removeClass("layui-hide");
                        }
                    }
                    else if (data.length == 1) {
                        for (var i = 0; i < oneList.length; i++) {
                            $('[name="' + oneList[i] + '"]').removeClass("layui-hide");
                        }
                        for (var i = 0; i < moreList.length; i++) {
                            $('[name="' + moreList[i] + '"]').removeClass("layui-hide");
                        }
                    }
                    else {
                        for (var i = 0; i < oneList.length; i++) {
                            $('[name="' + oneList[i] + '"]').addClass("layui-hide");
                        }
                        for (var i = 0; i < moreList.length; i++) {
                            $('[name="' + moreList[i] + '"]').addClass("layui-hide");
                        }
                    }
                }
                table.resize();
            });
        },
    }
    exports("commonTable", obj);
});