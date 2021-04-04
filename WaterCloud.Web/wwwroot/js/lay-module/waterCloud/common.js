/**
 * date:2020/02/27
 * author:Mr.Q
 * version:1.6
 * description:watercloud 主体框架扩展
 */
layui.define(["jquery", "layer", 'table', 'treeTable', 'xmSelect', 'miniTab'], function (exports) {
    var $ = layui.jquery,
        miniTab = layui.miniTab,
        layer = layui.layer,
        treeTable = layui.treeTable,
        xmSelect = layui.xmSelect;

    var obj = {
        //tabletree渲染封装里面有字段权限
        rendertreetable: function (options) {
            //样式不协调，先不加
            var defaults = {
                elem: '#currentTableId',//主键
                toolbar: '#toolbarDemo',//工具栏
                defaultToolbar: ['filter', 'exports', 'print'],//默认工具栏
                search: true,//搜索按钮
                loading: false,
                checkOther:false,//关闭复选框联动
                tree: {
                    iconIndex: 0,           // 折叠图标显示在第几列  多选等记得修改
                    isPidData: true,        // 是否是id、pid形式数据
                    idName: 'F_Id',  // id字段名称
                    pidName: 'F_ParentId',     // pid字段名称
                    arrowType: 'arrow2',
                    getIcon: 'ew-tree-icon-style2',
                },
                height: 'full-60',
                method: 'get',//请求方法
                cellMinWidth: 60,//最小宽度
                authorizeFields: true, // 字段权限开关
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
                options.defaultToolbar = !options.defaultToolbar? [] : options.defaultToolbar;
                options.defaultToolbar.push({
                    title: '搜索',
                    layEvent: 'TABLE_SEARCH',
                    icon: 'layui-icon-search'
                });
            }   
            //ie缓存问题
            options.url = obj.urlAddTime(options.url);
            //字段权限
            if (options.authorizeFields) {
                options.cols = obj.tableAuthorizeFields(options.cols);
            }
            options.done = function (res, curr, count) {
                //关闭加载
                //layer.closeAll('loading');
                if (doneCallback) {
                    doneCallback(res, curr, count);
                }
            };
            return treeTable.render(options);
        },
        //treetable刷新
        reloadtreetable: function (tree, options) {
            var loading = layer.load(0, { shade: false });
            var defaults = {
                where: {}
            };
            var options = $.extend(defaults, options);
            options.where.time = new Date().Format("yyyy-MM-dd hh:mm:ss");
            //执行搜索重载
            tree.reload({
                where: options.where
            });
            //关闭加载
            layer.closeAll('loading');
        },
        //msg
        modalMsg: function (content, type) {
            if (type != undefined) {
                var icon = 0;
                if (type == 'success') {
                    icon = 1;
                }
                else if (type == 'error') {
                    icon = 2;
                }
                else if (type == 'warning') {
                    icon = 7;
                }
                else {
                    icon = 5;
                }
                top.layer.msg(content, {
                    icon: icon, time: 1000, shift: 5,
                });
            } else {
                top.layer.msg(content);
            }
        },
        //alert
        modalAlert: function (content, type) {
            var icon = 0;
            if (type == 'success') {
                icon = 1;
            }
            if (type == 'error') {
                icon = 2;
            }
            if (type == 'warning') {
                icon = 7;
            }
            top.layer.alert(content, {
                icon: icon,
                title: "系统提示",
                btn: ['确认'],
                btnclass: ['btn btn-primary'],
                yes: function (index) {
                    top.layer.close(index);
                }
            });
        },
        //界面关闭
        modalClose: function () {
            var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
            var $IsdialogClose = top.$("#layui-layer" + index).find('.layui-layer-btn').find("#IsdialogClose");
            var IsClose = $IsdialogClose.is(":checked");
            if ($IsdialogClose.length == 0) {
                IsClose = true;
            }
            if (IsClose) {
                parent.layer.close(index);
            } else {
                location.reload();
            }
        },
        //创建界面
        modalOpen: function (options) {
            var defaults = {
                title: '系统窗口',
                width: "100px",
                height: "100px",
                anim: 0,//动画
                isOutAnim: true,//关闭动画
                maxmin: true, //开启最大化最小化按钮
                url: '',
                shade: 0.3,
                data: null,
                btn: ['确认', '关闭'],
                btnclass: ['layui-btn', 'layui-btn-primary'],
                isMax: false,//最大化属性 默认不是
                callBack: null,
                end: null,
                yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'submit'
                        , submit = layero.find('iframe').contents().find('#' + submitID);
                    submit.trigger('click');
                }
            };
            var doneCallback = options.success;
            var options = $.extend(defaults, options);
            options.success = function (layero, index) {
                //子界面定义一个窗口方法initForm，里面调用common.val给参数赋值
                if (!!options.data && layero.find('iframe')[0].contentWindow.initForm) {
                    layero.find('iframe')[0].contentWindow.initForm(options.data);
                }
                $(layero).addClass("scroll-wrapper");//苹果 iframe 滚动条失效解决方式
                if (doneCallback) {
                    doneCallback(layero, index);
                }
            };
            //ie缓存问题
            options.url = obj.urlAddTime(options.url);
            //options.url = obj.urlAddTime(options.url);
            //高度宽度超出就适应屏幕
            var _width = document.body.clientWidth > parseInt(options.width.replace('px', '')) ? options.width : document.body.clientWidth - 20 + 'px';
            var _height = document.body.clientHeight > parseInt(options.height.replace('px', '')) ? options.height : document.body.clientHeight - 20 + 'px';
            if (obj.currentWindow()) {
                _width = obj.currentWindow().document.body.clientWidth > parseInt(options.width.replace('px', '')) ? options.width : obj.currentWindow().document.body.clientWidth - 20 + 'px';
                _height = obj.currentWindow().document.body.clientHeight > parseInt(options.height.replace('px', '')) ? options.height : obj.currentWindow().document.body.clientHeight - 20 + 'px';
            }
            var index = top.layer.open({
                type: 2,
                shade: options.shade,
                title: options.title,
                isOutAnim: options.isOutAnim,//关闭动画
                maxmin: options.maxmin, //开启最大化最小化按钮
                fix: false,
                area: [_width, _height],
                content: options.url,
                btn: options.btn,
                success: function (layero, index) {
                    if (!!options.success) {
                        options.success(layero, index);
                    }
                },
                //btnclass: options.btnclass,
                yes: function (index, layero) {
                    if (!!options.yes) {
                        options.yes(index, layero);
                    }
                }, cancel: function () {
                    return true;
                },
                end: function () {
                    if (!!options.end) {
                        options.end();
                    }
                }
            });
            if (options.isMax) {
                top.layer.full(index);
            }
            return index;
        },
        //表单提交
        submitForm: function (options) {
            var defaults = {
                url: "",
                param: [],
                success: null,
                close: true,
                preventReuse:'.site-demo-active'//防止重复的参数
            };
            var options = $.extend(defaults, options);
            //ie缓存问题
            options.url = obj.urlAddTime(options.url);
            // 单击之后提交按钮不可选,防止重复提交
            if (!!options.preventReuse) {
                $(options.preventReuse).addClass('layui-btn-disabled');
                $(options.preventReuse).attr('disabled', 'disabled');
            }
            window.setTimeout(function () {
                if ($('[name=__RequestVerificationToken]').length > 0) {
                    var csrfToken = $('[name=__RequestVerificationToken]').val();
                }
                var index = parent.layer.load(0, {
                    shade: [0.5, '#000'], //0.1透明度的背景
                });
                $.ajax({
                    url: options.url,
                    data: options.param,
                    type: "post",
                    dataType: "json",
                    headers: {
                        "X-CSRF-TOKEN": csrfToken
                    },
                    success: function (data) {
                        if (data.state == "success") {
                            options.success(data);
                            obj.modalMsg(data.message, data.state);
                            if (options.close) {
                                try {
                                    obj.modalClose();
                                }
                                catch (err) {
                                    parent.layer.close(index);
                                    miniTab.deleteCurrentByIframe();
                                }

                            }
                        } else {
                            obj.modalAlert(data.message, data.state);
                        }
                        if (!!options.preventReuse) {
                            $(options.preventReuse).removeClass('layui-btn-disabled');
                            $(options.preventReuse).removeAttr('disabled');
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        parent.layer.close(index);
                        obj.modalAlert(errorThrown, "error");
                        if (!!options.preventReuse) {
                            $(options.preventReuse).removeClass('layui-btn-disabled');
                            $(options.preventReuse).removeAttr('disabled');
                        }
                    },
                    beforeSend: function () {
                    },
                    complete: function () {
                        parent.layer.close(index);
                    }
                });
                return false;
            }, 500);
        },
        //二次确认框
        modalConfirm: function (content, callBack) {
           var index= top.layer.confirm(content, {
                icon: "fa-exclamation-circle",
                title: "系统提示",
                btn: ['确认', '取消'],
                btnclass: ['btn btn-primary', 'btn btn-danger'],
            }, function () {
                top.layer.close(index);
                callBack(true);
            }, function () {
                callBack(false)
            });
        },
        //文档弹窗
        modalPrompt: function (type, title, callBack) {
            if (type < 0 || type > 3) {
                type = 0;
            }
            top.layer.prompt({
                formType: type,
                value: '',
                title: title,
                btn: ['确认', '取消'],
                btnclass: ['btn btn-primary', 'btn btn-danger'],
            }, function (value, index, elem) {
                top.layer.close(index);
                callBack(true, value);
            });
        },
        //删除
        deleteForm: function (options) {
            var defaults = {
                prompt: "注：您确定要删除选中数据吗？",
                url: "",
                param: [],
                success: null,
                close: false
            };
            var options = $.extend(defaults, options);
            //ie缓存问题
            options.url = obj.urlAddTime(options.url);
            if ($('[name=__RequestVerificationToken]').length > 0) {
                var csrfToken = $('[name=__RequestVerificationToken]').val();
            }
            obj.modalConfirm(options.prompt, function (r) {
                if (r) {
                    var index = parent.layer.load(0, {
                        shade: [0.5, '#000'], //0.1透明度的背景
                    });
                    window.setTimeout(function () {
                        $.ajax({
                            url: options.url,
                            data: options.param,
                            type: "post",
                            dataType: "json",
                            headers: {
                                "X-CSRF-TOKEN": csrfToken
                            },
                            success: function (data) {
                                if (data.state == "success") {
                                    options.success(data);
                                    obj.modalMsg(data.message, data.state);
                                    if (options.close) {
                                        obj.modalClose();
                                    }
                                } else {
                                    obj.modalAlert(data.message, data.state);
                                }
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                parent.layer.close(index);
                                obj.modalMsg(errorThrown, "error");
                            },
                            beforeSend: function () {
                            },
                            complete: function () {
                                parent.layer.close(index);
                            }
                        });
                    }, 500);
                }
            });

        },
        //post提交
        submitPost: function (options) {
            var defaults = {
                prompt: options.title,
                url: "",
                param: [],
                success: null,
                close: false
            };
            var options = $.extend(defaults, options);
            //ie缓存问题
            options.url = obj.urlAddTime(options.url);
            if ($('[name=__RequestVerificationToken]').length > 0) {
                var csrfToken = $('[name=__RequestVerificationToken]').val();
            }
            obj.modalConfirm(options.prompt, function (r) {
                if (r) {
                    var index = parent.layer.load(0, {
                        shade: [0.5, '#000'], //0.1透明度的背景
                    });
                    window.setTimeout(function () {
                        $.ajax({
                            url: options.url,
                            data: options.param,
                            type: "post",
                            dataType: "json",
                            headers: {
                                "X-CSRF-TOKEN": csrfToken
                            },
                            success: function (data) {
                                if (data.state == "success") {
                                    options.success(data);
                                    obj.modalMsg(data.message, data.state);
                                    if (options.close) {
                                        obj.modalClose();
                                    }
                                } else {
                                    obj.modalAlert(data.message, data.state);
                                }

                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                parent.layer.close(index);
                                obj.modalMsg(errorThrown, "error");
                            },
                            beforeSend: function () {
                            },
                            complete: function () {
                                parent.layer.close(index);
                            }
                        });
                    }, 500);
                }
            });

        },
        //Form序列化方法
        val: function (filter, formdate) {
            var element = $('div[lay-filter=' + filter + ']');
            if (!!formdate) {
                for (var key in formdate) {
                    var $id = element.find('#' + key);
                    var value = $.trim(formdate[key]).replace(/&nbsp;/g, '');
                    var type = $id.attr('type');
                    if ($id.hasClass("select2-hidden-accessible")) {
                        type = "select";
                    }
                    switch (type) {
                        case "checkbox":
                            if (value == "true") {
                                $id.attr("checked", 'checked');
                            } else {
                                $id.removeAttr("checked");
                            }
                            break;
                        case "select":
                            $id.val(value).trigger("change");
                            break;
                        default:
                            $id.val(value);
                            break;
                    }
                };
                return false;
            }
            var postdata = {};
            element.find('input,select,textarea').each(function (r) {
                var $this = $(this);
                var id = $this.attr('id');
                var type = $this.attr('type');
                switch (type) {
                    case "checkbox":
                        postdata[id] = $this.is(":checked");
                        break;
                    case "radio":
                        postdata[id] = $this.is(":checked");
                        break;
                    default:
                        var value = $this.val() == "" ? "&nbsp;" : $this.val();
                        if (!$.request("keyValue")) {
                            value = value.replace(/&nbsp;/g, '');
                        }
                        postdata[id] = value;
                        break;
                }
            });
            return postdata;
        },
        //父窗体刷新（按钮刷新）
        parentreload: function (filter) {
            var iframes = top.$(".layui-show>iframe");
            if (iframes.length > 0) {
                iframes[0].contentWindow.$('button[lay-filter="' + filter + '"]').click();
            }
        },
        currentWindow: function () {
            var iframes = top.$(".layui-show>iframe");
            if (iframes.length > 0) {
                return iframes[0].contentWindow;
            }
        },
        //当前页刷新（按钮刷新）
        reload: function (filter) {
            $('button[lay-filter="' + filter + '"]').click();//按钮刷新
        },
        //下载方法
        download: function (url, data, method) {
            if (url && data) {
                data = typeof data == 'string' ? data : jQuery.param(data);
                var inputs = '';
                $.each(data.split('&'), function () {
                    var pair = this.split('=');
                    inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
                });
                $('<form action="' + url + '" method="' + (method || 'post') + '">' + inputs + '</form>').appendTo('body').submit().remove();
            };
        },
        //设置Form只读
        setReadOnly: function (filter) {
            var readForm = layui.$('[lay-filter="' + filter + '"]');
            readForm.find('input,textarea,select').prop('disabled', true);
            readForm.find('input,textarea,select').removeAttr('lay-verify');
            readForm.find('.layui-layedit iframe').contents().find('body').prop('contenteditable', false);
        },
        //按钮权限(控制js模板,格式必须严格,新)
        authorizeButtonNew: function (innerHTML) {
            //行操作权限控制
            var moduleId = top.$(".layui-tab-title>.layui-this").attr("lay-id");
            var returnhtml = '';
            //没有就全清
            if (!top.clients || !top.clients.authorizeButton) {
                return returnhtml;
            }
            var dataJson = top.clients.authorizeButton[moduleId.split("?")[0]];
            if (innerHTML.indexOf('</button>') > -1) {
                var tempList = innerHTML.split('</button>');
                for (var i = 0; i < tempList.length; i++) {
                    if (tempList[i].indexOf('<button ') > -1) {
                        var itemList = tempList[i].split('<button ');
                        returnhtml = returnhtml + itemList[0];
                        if (itemList[1].indexOf(' authorize') == -1) {
                            returnhtml = returnhtml + '<button ' + itemList[1] + '</button>';
                        }
                        else if (dataJson != undefined) {
                            $.each(dataJson, function (i) {
                                if (itemList[1].indexOf('id="' + dataJson[i].F_EnCode + '"') > -1) {
                                    returnhtml = returnhtml + '<button ' + itemList[1] + '</button>';
                                    return false;
                                }
                            });
                        }
                        if (itemList.length>2) {
                            returnhtml = returnhtml + itemList[2];
                        }
                    }
                    else {
                        returnhtml = returnhtml + tempList[i];
                    }
                }
            }
            else if (innerHTML.indexOf('</a>') > -1){
                var tempList = innerHTML.split('</a>');
                for (var i = 0; i < tempList.length; i++) {
                    if (tempList[i].indexOf('<a ') > -1) {
                        var itemList = tempList[i].split('<a ');
                        returnhtml = returnhtml + itemList[0];
                        if (itemList[1].indexOf(' authorize') == -1) {
                            returnhtml = returnhtml + '<a ' + itemList[1] + '</a>';
                        }
                        else if (dataJson != undefined) {
                            $.each(dataJson, function (i) {
                                if (itemList[1].indexOf('id="' + dataJson[i].F_EnCode + '"') > -1) {
                                    returnhtml = returnhtml + '<a ' + itemList[1] + '</a>';
                                    return false;
                                }
                            });
                        }
                        if (itemList.length > 2) {
                            returnhtml = returnhtml + itemList[2];
                        }
                    }
                    else {
                        returnhtml = returnhtml + tempList[i];
                    }
                }
            }
            //去除隐藏
            //returnhtml = returnhtml.replace(/ layui-hide/g, '');
            return returnhtml;
        },
        //权限按钮(控制dom,只控制button,旧)
        authorizeButton: function (id) {
            var moduleId = top.$(".layui-tab-title>.layui-this").attr("lay-id");
            //没有就全清
            if (!top.clients || !top.clients.authorizeButton) {
                $element.find('button[authorize=yes]').attr('authorize', 'no');
                $element.find("[authorize=no]").parents('button').prev('.split').remove();
                $element.find("[authorize=no]").parents('button').remove();
                $element.find('[authorize=no]').remove();
                return false;
            }
            var dataJson = top.clients.authorizeButton[moduleId.split("?")[0]];
            var $element = $('#' + id);
            $element.find('button[authorize=yes]').attr('authorize', 'no');
            if (dataJson != undefined) {
                $.each(dataJson, function (i) {
                    $element.find("#" + dataJson[i].F_EnCode).attr('authorize', 'yes');
                    //去除隐藏
                    //$element.find("#" + dataJson[i].F_EnCode).removeClass('layui-hide');
                });
            }
            $element.find("[authorize=no]").parents('button').prev('.split').remove();
            $element.find("[authorize=no]").parents('button').remove();
            $element.find('[authorize=no]').remove();
        },
        //权限控制单个按钮，返回是否
        authorizeButtonOne: function (id) {
            var moduleId = top.$(".layui-tab-title>.layui-this").attr("lay-id");
            var isOk = false;
            if (!top.clients || !top.clients.authorizeButton || !top.clients.authorizeButton[moduleId.split("?")[0]]) {
                isOk = false;
            }
            else {
                var dataJson = top.clients.authorizeButton[moduleId.split("?")[0]];

                for (var i = 0; i < dataJson.length; i++) {
                    if (dataJson[i].F_EnCode == id) {
                        isOk = true;
                    }
                }
            }
            if (isOk) {
                $("#" + id).parent().removeClass('layui-hide');
            }
            else {
                $("#" + id).remove();;
                $("#" + id).parent().remove();;
            }
            return isOk;
        },
        //表单权限字段
        authorizeFields: function (filter) {
            var moduleId = top.$(".layui-tab-title>.layui-this").attr("lay-id");
            var element = $('div[lay-filter=' + filter + ']');
            //没有就全清
            if (!top.clients || !top.clients.moduleFields) {
                element.find('input,select,textarea').each(function (r) {
                    $this.parent().parent().remove();
                });
                return false;
            }
            if (!!top.clients.moduleFields[moduleId.split("?")[0]] && top.clients.moduleFields[moduleId.split("?")[0]] == true) {
                var dataJson = top.clients.authorizeFields[moduleId.split("?")[0]];
                element.find('input,select,textarea').each(function (r) {
                    var $this = $(this);
                    var id = $this.attr('id');
                    $this.addClass('layui-hide');
                    $this.attr('authorize', 'no');
                    if (dataJson != undefined) {
                        for (var i = 0; i < dataJson.length; i++) {
                            if (id == dataJson[i].F_EnCode) {
                                $this.parent().parent().removeClass('layui-hide');
                                $this.removeClass('layui-hide');
                                $this.attr('authorize', 'yes');
                                break;
                            }
                        }
                        //dataJson.find(item => {
                        //    if (id == item.F_EnCode) {
                        //        $this.parent().parent().removeClass('layui-hide');
                        //        $this.removeClass('layui-hide');
                        //    }
                        //});
                    }
                });
                element.find('[authorize=no]').parent().parent().remove();
            }
            else {
                element.find('input,select,textarea').each(function (r) {
                    var $this = $(this);
                    $this.parent().parent().removeClass('layui-hide');
                });
            }
        },
        //iframe定时器方法
        iframeInterval:function(func, time){
            console.log("启动定时器", func, time);
            //点击iframe对应的标签则直接执行定时器方法。不过注意，这里我默认只执行最后一个定时器方法，如果有多个定时器请自行更改。
            top.$("li[lay-id='" + $(self.frameElement).attr("src") + "']").unbind('click').click(function () {
                console.log(func, "方法调用");
                func.call();
            });
            return setInterval(function () {
                console.log("定时器调用");
                if ($(self.frameElement.parentElement).hasClass("layui-show")) {
                    //判断所在的页面是否显示
                    console.log(func, "方法调用");
                    console.log("定时器间隔" + time);
                    func.call();
                }
            }, time);
        },
        //xmselect多选
        multipleSelectRender: function (options) {
            var defaults = {
                filterable: true,//搜索模式
                autoRow: true,//自动换行
                empty: '没有数据!',//空数据提示
                toolbar: { show: true },//工具栏
                remoteSearch: true,//远程搜索
                paging: true,//分页
                pageSize: 4,//分页大小
                direction: 'auto',//下拉方向
                data: [],
                prop: {
                    name: 'text',
                    value: 'id',
                },
                remoteMethod: function (val, cb, show) {
                    //远程数据方法
                    //val 搜索参数
                    //cb data的值
                    //show 执行完显示值
                }
            };
            var options = $.extend(defaults, options);
            return xmSelect.render(options);
        },
        //xmselect单选
        radioSelectRender: function (options) {
            var defaults = {
                filterable: true,//搜索模式
                toolbar: { show: true, list: ["CLEAR"] },//工具栏
                empty: '没有数据!',//空数据提示
                remoteSearch: true,//远程搜索
                paging: true,//分页
                pageSize: 4,//分页大小
                data: [],
                radio: true,//单选
                clickClose: true,//选择关闭
                direction: 'auto',//下拉方向
                prop: {
                    name: 'text',
                    value: 'id',
                },
                model: {
                    icon: 'hidden',//图标隐藏
                    label: {
                        type: 'text'//显示
                    }
                },
                remoteMethod: function (val, cb, show) {
                    //远程数据方法
                    //val 搜索参数
                    //cb data的值
                    //show 执行完显示值
                }
            };
            var options = $.extend(defaults, options);
            return xmSelect.render(options);
        },
        //ajax封装
        ajax: function (options) {
            var defaults = {
                dataType: "json",
                async: true,
                type: "GET"
            };
            var options = $.extend(defaults, options);
            //ie缓存问题
            options.url = obj.urlAddTime(options.url);
            return $.ajax(options);
        },
        //打开新Tab页签
        openNewTabByIframe: function (options) {
            var defaults = {
                title: "",
                href: "",
                checkOpen: true
            };
            var options = $.extend(defaults, options);
            if (options.checkOpen && miniTab.check(options.href.split("?")[0], true)) {
                obj.modalAlert("界面已打开,请关闭后重试", "warning");
            }
            miniTab.openNewTabByIframe({
                title: options.title,
                href: options.href
            });
        },
        //刷新tab Iframe
        reloadIframe: function (src, filter) {
            var iframes = top.$('.layui-tab-item>iframe[src="' + src + '"]');
            if (iframes.length>0) {
                iframes[0].contentWindow.$('button[lay-filter="' + filter + '"]').click();
            }
        },
        //url参数注入(//ie缓存问题)
        urlAddTime: function (url) {
            if (!!url) {
                if (url.indexOf("?") >= 0) {
                    url = url+ '&v=' + new Date().Format("yyyy-MM-dd hh:mm:ss");
                }
                else {
                    url = url + '?v=' + new Date().Format("yyyy-MM-dd hh:mm:ss");
                }
                return url;
            }
            else {
                return null;
            }
        },
        //表格权限字段(过滤cols)
        tableAuthorizeFields: function (cols) {
            var moduleId = top.$(".layui-tab-title>.layui-this").attr("lay-id");
            //没有权限就返回无
            if (!top.clients||!top.clients.moduleFields) {
                return [];
            }
            if (!!top.clients.moduleFields[moduleId.split("?")[0]] && top.clients.moduleFields[moduleId.split("?")[0]] == true) {
                var dataJson = top.clients.authorizeFields[moduleId.split("?")[0]];
                var array = [];
                $.each(cols[0], function (i) {
                    //添加非常规列
                    if (!!cols[0][i].type && cols[0][i].type != 'normal') {
                        array.push(cols[0][i]);
                    } else if (!cols[0][i].field && cols[0][i].title == "操作") {
                        array.push(cols[0][i]);
                    }
                    if (!!dataJson) {
                        for (var j = 0; j < dataJson.length; j++) {
                            if (cols[0][i].field == dataJson[j].F_EnCode) {
                                array.push(cols[0][i]);
                                break;
                            }
                        }
                    }
                });
                cols[0] = array;
            };
            return cols;
        },
        //treetable行点击事件及按钮显示控制
        treeTableRowClick: function (type, rendertree ,tableId, oneList, moreList) {
            var oneList = !!oneList ? oneList : [];
            var moreList = !!moreList ? moreList : [];
            treeTable.on('row(' + tableId + ')', function (obj) {
                obj.tr.addClass("layui-table-click").siblings().removeClass("layui-table-click");
                obj.tr.find("div.layui-unselect.layui-form-" + type)[0].click();
                if (type =="radio") {
                    for (var i = 0; i < oneList.length; i++) {
                        $('[name="' + oneList[i] + '"]').removeClass("layui-hide");
                    }
                    for (var i = 0; i < moreList.length; i++) {
                        $('[name="' + moreList[i] + '"]').removeClass("layui-hide");
                    }
                }
            })
            //多选框监听
            treeTable.on(type + '(' + tableId + ')', function (obj) {
                //控制按钮
                var data = rendertree.checkStatus(false);
                if (obj.type == "all") {
                    if (obj.checked && rendertree.options.data.length != 0) {
                        if (rendertree.options.data.length > 1) {
                            for (var i = 0; i < oneList.length; i++) {
                                $('[name="' + oneList[i] + '"]').addClass("layui-hide");
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
            });

        },
        //form参数过滤方法，值不存在直接删除
        removeEmpty: function (filter, formdate) {
            var element = $('div[lay-filter=' + filter + ']');
            if (!!formdate) {
                for (var key in formdate) {
                    var $id = element.find('#' + key);
                    if (!!$id && formdate[key] != 0 && formdate[key]!=false && !formdate[key]) {
                        $id.parent().parent().remove();
                    }
                };
                return false;
            }
        },
        //表格单元格自动列宽
        //tableResize: function (id) {
        //    //动态监听表头高度变化，冻结行跟着改变高度
        //    $("div [lay-id='" + id + "'] .layui-table-header tr").resize(function () {
        //        $("div [lay-id='" + id + "'] .layui-table-header tr").each(function (index, val) {
        //            $($("div [lay-id='" + id + "'] .layui-table-fixed .layui-table-header table tr")[index]).height($(val).height());
        //        });
        //    });
        //    //初始化高度，使得冻结行表头高度一致
        //    $("div [lay-id='" + id + "'] .layui-table-header  tr").each(function (index, val) {
        //        $($("div [lay-id='" + id + "'] .layui-table-fixed .layui-table-header table tr")[index]).height($(val).height());
        //    });
        //    //动态监听表体高度变化，冻结行跟着改变高度
        //    $("div [lay-id='" + id + "'] .layui-table-main tr").resize(function () {
        //        $("div [lay-id='" + id + "'] .layui-table-body tr").each(function (index, val) {
        //            $($("div [lay-id='" + id + "'] .layui-table-fixed .layui-table-body table tr")[index]).height($(val).height());
        //        });
        //    });
        //    //初始化高度，使得冻结行表体高度一致
        //    $("div [lay-id='" + id + "'] .layui-table-main tr").each(function (index, val) {
        //        $($("div [lay-id='" + id + "'] .layui-table-fixed .layui-table-body table tr")[index]).height($(val).height());
        //    });

        //    //初始化滚动条
        //    $("div [lay-id='" + id + "'] .layui-table-fixed .layui-table-body").animate({ scrollTop: $("div [lay-id='" + id + "'] .layui-table-main").scrollTop() }, 0); 
        //},
    }
    exports("common", obj);
});