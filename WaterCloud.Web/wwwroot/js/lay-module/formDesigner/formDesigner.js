layui.define(['layer', 'laytpl', 'element', 'form', 'slider', 'laydate', 'rate', 'colorpicker', 'layedit', 'carousel', 'upload', 'formField', 'formPreview']
    , function (exports) {
        var $ = layui.jquery
            , layer = layui.layer
            , laytpl = layui.laytpl
            , setter = layui.cache
            , element = layui.element
            , formPreview = layui.formPreview
            , slider = layui.slider
            , laydate = layui.laydate
            , rate = layui.rate
            , colorpicker = layui.colorpicker
            , carousel = layui.carousel
            , form = layui.form
            , upload = layui.upload
            , layedit = layui.layedit
            , formField = layui.formField
            , hint = layui.hint
            , guid = function () {
                var d = new Date().getTime();
                if (window.performance && typeof window.performance.now === "function") {
                    d += performance.now(); //use high-precision timer if available
                }
                var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
                });
                return uuid;
            }
            , lang = {
                id: "标识",
                label: "标题",
                index: "序号",
                tag: "表单类型",
                tagIcon: '图标',
                width: '宽度',
                height: "高度",
                span: '网格宽度',
                placeholder: "placeholder",
                defaultValue: "默认值",
                labelWidth: "文本宽度",
                clearable: "是否清楚",
                prepend: "前缀",
                append: "追加",
                prefixIcon: '前缀图标',
                suffixIcon: '后缀图标',
                maxlength: "最大长度",
                showWordLimit: "是否限制字符",
                readonly: "只读",
                disabled: "禁用",
                required: "必填",
                columns: "列数",
                options: "选项",
                switchValue: "默认值",
                maxValue: "最大值",
                minValue: "最小值",
                stepValue: "步长",
                datetype: "日期类型",
                dateformat: "日期格式",
                half: "显示半星",
                theme: "皮肤",
                rateLength: "星星个数",
                interval: "间隔毫秒",
                autoplay: "自动播放",
                startIndex: "开始位置",
                full: "是否全屏",
                arrow: "鼠标样式",
                contents: "内容",
                document: '帮助文档',
                input: "输入框",
                select: "下拉",
                checkbox: "多选组",
                radio: "单选组",
                date: "日期",
                editor: "编辑器",
                slider: "滑块",
                image: "图片",
                grid: "一行多列",
                colorpicker: "颜色选择器",
                textarea: "多行文本",
                rate: "评分控件",
                switch: "开关",
                password: "密码框",
                carousel: "轮播",
                text: "显示文本"

            }
            , expressions = [{ text: '默认', value: '' }
                , { text: '数字', value: 'number' }
                , { text: '邮箱', value: 'email' }
                , { text: '手机', value: 'phone' }
                , { text: '身份证', value: 'identity' }
                , { text: '日期', value: 'date' }
                , { text: '网址', value: 'url' }
                , { text: '密码', value: 'pass' }
            ]
            , renderCommonProperty = function (json) {
                var _html = '';
                for (var key in json) {
                    if (key === 'index') { continue; }
                    if (key === 'tag' || key === 'id') { //只读字段
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        if (key === 'tag') {
                            _html += '    <input type="text" readonly id="{0}" name="{0}" value="{1}" required lay-verify="required" placeholder="请输入字段标识" autocomplete="off" class="layui-input">'
                                .format(key, json[key] == undefined ? '' : json[key]);
                        } else {
                            _html += '    <input type="text" id="{0}" name="{0}" value="{1}" required lay-verify="required" placeholder="请输入字段标识" autocomplete="off" class="layui-input">'
                                .format(key, json[key] == undefined ? '' : json[key]);
                        }

                        _html += '  </div>';
                        _html += '</div>';
                    }
                    else if (key === 'readonly' || key === 'disabled' || key === 'required' || key === 'half' || key === "text" || key === "autoplay"
                        || key === "full") {
                        var yes = "是";
                        var no = "否";
                        if (key === 'readonly') {
                            yes = "只读";
                            no = "可写";
                        }
                        if (key === 'disabled') {
                            yes = "隐藏";
                            no = "显示";
                        }
                        if (key === 'required') {
                            yes = "必填";
                            no = "可选";
                        }

                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '    <input type="checkbox" id="{1}" {0} name="{1}" lay-skin="switch" lay-text="{2}|{3}">'.format(json[key] ? 'checked' : '', key, yes, no);
                        _html += '  </div>';
                        _html += '</div>';
                        if (key === 'required') {
                            _html += '<div class="layui-form-item" >';
                            _html += '  <label class="layui-form-label">验证</label>';
                            _html += '  <div class="layui-input-block">';
                            _html += '<select name="{0}" lay-verify="required">'.format(key);
                            for (var i = 0; i < expressions.length; i++) {
                                if (expressions[i].value === json.expression) {
                                    _html += '<option value="{0}" selected="">{1}</option>'.format(expressions[i].value, expressions[i].text);
                                } else {
                                    _html += '<option value="{0}">{1}</option>'.format(expressions[i].value, expressions[i].text);
                                }
                            }
                            _html += '</select>'
                            _html += '  </div>';
                            _html += '</div>';
                        }
                    }
                    else if (key === 'defaultValue' || key === 'label' || key === 'height' || key === 'placeholder' || key === 'document'
                        || key === 'minValue' || key === 'maxValue' || key === 'stepValue' || key === 'rateLength' || key === 'width') {
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '    <input type="text" id="{0}" name="{0}" value="{1}" required lay-verify="required" placeholder="请输入{2}" autocomplete="off" class="layui-input">'
                            .format(key, json[key] == undefined ? '' : json[key], lang[key]);
                        _html += '  </div>';
                        _html += '</div>';
                    }
                    else if (key === 'switchValue') {
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '     <input type="checkbox" name="{0}" {1} lay-skin="switch" lay-text="ON|OFF">'
                            .format(key, json[key] ? 'checked' : '');
                        _html += '  </div>';
                        _html += '</div>';
                    } else if (key === 'datetype') {
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '<select name="{0}" lay-verify="required">'.format(key);
                        var _datetype = ["year", "month", "date", "datetime"];
                        for (var i = 0; i < _datetype.length; i++) {
                            if (_datetype[i] === json.datetype) {
                                _html += '<option value="{0}" selected="">{0}</option>'.format(_datetype[i]);
                            } else {
                                _html += '<option value="{0}">{0}</option>'.format(_datetype[i]);
                            }
                        }
                        _html += '</select>'
                        _html += '  </div>';
                        _html += '</div>';
                    }
                    else if (key === 'dateformat') {
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '<select name="{0}" lay-verify="required">'.format(key);
                        var _strs = [];
                        switch (json.datetype) {
                            case "date":
                                _strs = ["yyyy年MM月dd日","yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMdd"];
                                break;
                            case "datetime":
                                _strs = ["yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyy年MM月dd日 HH时mm分ss秒"];
                                break;
                            case "year":
                                _strs = ["yyyy", "yyyy年"];
                                break;
                            case "month":
                                _strs = ["yyyy年MM月", "yyyy-MM", "yyyy/MM", "yyyyMM"];
                                break;
                        }
                        for (var i = 0; i < _strs.length; i++) {
                            if (_strs[i] === json.dateformat) {
                                _html += '<option value="{0}" selected="">{0}</option>'.format(_strs[i]);
                            } else {
                                _html += '<option value="{0}">{0}</option>'.format(_strs[i]);
                            }
                        }
                        _html += '</select>'
                        _html += '  </div>';
                        _html += '</div>';

                    }
                    else if (key === 'contents') {
                        //处理
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '    <button type="button" id="select-option-add" class="layui-btn layui-btn-primary layui-btn-sm"><i class="layui-icon layui-icon-addition"></i>增加选项</button>'
                        //_html += '    <button type="button" id="select-option-datasource" class="layui-btn layui-btn-primary layui-btn-sm"><i class="layui-icon layui-icon-addition"></i>设置数据源</button>'
                        _html += '  </div>';
                        _html += '</div>';
                        _html += '<div id="{0}">'.format(json.tag);
                        //选项
                        for (var i = 0; i < json.contents.length; i++) {
                            _html += '<div class="layui-form-item option contents-options" data-index="{0}">'.format(i);

                            _html += '  <div class="layui-inline" style="margin-right: 0px;width:220px; margin-left:30px;">';
                            _html += '   <input type="text" name="{0}-text"  autocomplete="off" value="{1}" class="layui-input">'.format(json.tag, json.contents[i]);
                            _html += '  </div>';
                            _html += '  <div class="layui-inline" style="margin-right: 0px;">';
                            _html += '   <i class="layui-icon layui-icon-slider contents-option-drag" style="color:blue;font-size:20px;"></i>';
                            _html += '   <i class="layui-icon layui-icon-delete contents-option-delete" style="color:red;font-size:20px;"></i>';
                            _html += '  </div>';
                            _html += '</div>';
                            //向 .option .layui-inline 添加drag事件并且必须设在 contents-option-drag 中才能拖动
                        }
                        _html += '</div>';
                    }
                    else if (key === 'options') {
                        //处理
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '    <button type="button" id="select-option-add" class="layui-btn layui-btn-primary layui-btn-sm"><i class="layui-icon layui-icon-addition"></i>增加选项</button>'
                        //_html += '    <button type="button" id="select-option-datasource" class="layui-btn layui-btn-primary layui-btn-sm"><i class="layui-icon layui-icon-addition"></i>设置数据源</button>'
                        _html += '  </div>';
                        _html += '</div>';
                        _html += '<div id="{0}">'.format(json.tag);
                        //选项
                        for (var i = 0; i < json.options.length; i++) {
                            _html += '<div class="layui-form-item option select-options" data-index="{0}">'.format(i);
                            _html += '  <div class="layui-inline" style="width: 30px; margin-right: 0px;">';
                            if (json.tag === 'checkbox') {
                                if (json.options[i].checked) {
                                    _html += '    <input type="checkbox" name="{0}" lay-skin="primary" title="" checked="">'.format(json.tag);
                                } else {
                                    _html += '    <input type="checkbox" name="{0}" lay-skin="primary" title="">'.format(json.tag);
                                }
                            } else {
                                if (json.options[i].checked) {
                                    _html += '   <input type="radio" name="{0}"  checked="">'.format(json.tag);
                                } else {
                                    _html += '   <input type="radio" name="{0}" >'.format(json.tag);
                                }
                            }

                            _html += '  </div>';
                            _html += '  <div class="layui-inline" style="margin-right: 0px;width:110px;">';
                            _html += '   <input type="text" name="{0}-text"  autocomplete="off" value="{1}" class="layui-input">'.format(json.tag, json.options[i].text);
                            _html += '  </div>';
                            _html += '  <div class="layui-inline" style="margin-right: 0px;width:110px;">';
                            _html += '   <input type="text" name="{0}-value"  autocomplete="off" value="{1}" class="layui-input">'.format(json.tag, json.options[i].value);
                            _html += '  </div>';
                            _html += '  <div class="layui-inline" style="margin-right: 0px;">';
                            _html += '   <i class="layui-icon layui-icon-slider select-option-drag" style="color:blue;font-size:20px;"></i>';
                            _html += '   <i class="layui-icon layui-icon-delete select-option-delete" style="color:red;font-size:20px;"></i>';
                            _html += '  </div>';
                            _html += '</div>';
                            //向 .option .layui-inline 添加drag事件并且必须设在 select-option-drag 中才能拖动
                        }
                        _html += '</div>';
                    }
                    else if (key === 'columns') {
                        var columnCount = 2;
                        columnCount = json[key].length;
                        //处理
                        _html += '<div class="layui-form-item" >';
                        _html += '  <label class="layui-form-label">{0}</label>'.format(lang[key]);
                        _html += '  <div class="layui-input-block">';
                        _html += '<select name="{0}" lay-verify="required">'.format(key);
                        for (var i = 2; i <= 12; i++) {
                            if (i === columnCount) {
                                _html += '<option value="{0}" selected="">{0}</option>'.format(i);
                            } else {
                                _html += '<option value="{0}">{0}</option>'.format(i);
                            }
                        }
                        _html += '</select>'
                        _html += '</div>';
                        _html += '</div>';
                    }
                }
                return _html;
            }
            //模块名称常量

            , MOD_NAME = 'formDesigner'
            , TP_MAIN = ['<div class="layui-layout layui-layout-admin">'
                , '<div class="layui-header" style="height:25px">'
                , '<div class="layui-logo" style="line-height:25px">Layui 表单设计器</div>'
                , '<!-- 头部区域（可配合layui已有的水平导航） -->'
                , '<ul class="layui-nav layui-layout-right">'
                , '<li class="layui-nav-item" style="line-height:25px">'
                , '<a id="btnImportJson" href="#" class="importJson">导入数据</a>'
                , '</li>'
                , '<li class="layui-nav-item" style="line-height:25px">'
                , '<a id="btnExportJson" href="#" class="exportJson">导出数据</a>'
                , '</li>'
                , '<li class="layui-nav-item" style="line-height:25px">'
                , '<a id="btnTemplateList" href="#" class="templateList">模板</a>'
                , '</li>'
                , '<li class="layui-nav-item" style="line-height:25px">'
                , '<a href="#" class="previewForm">预览</a>'
                , '</li>'
                , '</ul>'
                , '</div>'
                , '<div class="layui-col-md3 layui-col-sm3">'
                , '<!-- 左侧导航区域（可配合layui已有的垂直导航） -->'
                , '<h3 class="coltitle">组件</h3>'
                , '<div id="generateSetValue" class="layui-hide"></div>'
                , '<div class="components-list" style="position:absolute; height:440px; overflow:auto;border:1px dotted #999" id="components-form-list">'
                , '</div>'
                , '</div>'
                , '<div class="layui-col-md6 layui-col-sm6">'
                , '<h3 class="coltitle">设计</h3>'
                , '<!-- 内容主体区域 -->'
                , '<form class="layui-form">'
                , '<div class="layui-form" id="formDesignerForm" lay-filter="formDesignerForm" style="height:410px; overflow:auto">'
                , '<div class="layui-row layui-empty">'
                , '从左侧拖拽控件到此设计区域来添加字段'
                , '</div>'
                , '</form>'
                , '</div>'
                , '</div>'
                , '<div class="layui-col-md3 layui-col-sm3">'
                , '<h3 class="coltitle">属性</h3>'
                , '<!-- 属性导航 -->'
                , '<form class="layui-form" style="position:absolute; height:440px; overflow:auto;border:1px dotted #999">'
                , '<div id="columnProperty">'
                , '</div>'
                , '</form>'
                , '</div>'
                , '</div>'].join('')
            , TP_HTML_VIEW = ['<div class="htmlcodeview layui-layer-wrap" style="display: none;">'
                , '<textarea class="site-demo-text" id="generate-code-view"></textarea>'
                , '<a href="javascript:;" class="layui-btn layui-btn-normal" style="margin-right:20px;" id="copy-html-code">复制代码</a>'
                , '</div>'].join('')
            , TP_IMPORT_VIEW = ['<div class="importjsoncodeview" layui-layer-wrap" style="display: none;">'
                , '<textarea class="site-demo-text" id="import-json-code-view"></textarea>'
                , '<a href="javascript:;" class="layui-btn layui-btn-normal" style="margin-right:20px;" id="import-json-code">导入数据</a>'
                , '</div>'].join('')
            //外部接口
            , formDesigner = {
                index: layui.formDesigner ? (layui.formDesigner.index + 10000) : 0

                //设置全局项

                , set: function (options) {
                    var that = this;
                    that.config = $.extend({}
                        , that.config
                        , options);
                    return that;
                }

                //事件监听

                , on: function (events
                    , callback) {
                    return layui.onevent.call(this
                        , MOD_NAME
                        , events
                        , callback);
                }
            }
            /**
             * 操作当前实例
             * id 表示当前实例的索引 默认就是内部的 index，如果id存在值 那么就从已经存在的获取
            */

            , thisIns = function () {
                var that = this

                    , options = that.config;
                return {
                    config: options

                    , reload: function (options) {
                        that.reload.call(that
                            , options);
                    }
                }
            }

            , getThisInsConfig = function (id) {
                var config = thisIns.config[id];
                if (!config) { hint.error('The ID option was not found in the table instance'); }
                return config || null;
            }

            , Class = function (options) {
                var that = this;
                that.index = ++formDesigner.index; //增加实例，index 也是要增加
                that.config = $.extend({}
                    , that.config
                    , formDesigner.config
                    , options);
                that.render();

            };

        /* 此方法最后一道 commom.js 中 */
        String.prototype.format = function (args) {
            var result = this;
            if (arguments.length > 0) {
                if (arguments.length == 1 && typeof (args) == "object") {
                    for (var key in args) {
                        if (args[key] != undefined) {
                            var reg = new RegExp("({" + key + "})"
                                , "g");
                            result = result.replace(reg
                                , args[key]);
                        }
                    }
                }
                else {
                    for (var i = 0; i < arguments.length; i++) {
                        if (arguments[i] != undefined) {
                            var reg = new RegExp("({[" + i + "]})"
                                , "g");
                            result = result.replace(reg
                                , arguments[i]);
                        }
                    }
                }
            }
            return result;
        }

        Class.prototype.findJsonItem = function (json, id) {
            var that = this,
                options = that.config;
            for (var i = 0; i < json.length; i++) {
                if (json[i].id === id) {
                    return json[i];
                } else {
                    if (json[i].tag === 'grid') {
                        for (var j = 0; j < json[i].columns.length; j++) {
                            if (json[i].columns[j].list.length > 0) {
                                var _item = that.findJsonItem(json[i].columns[j].list, id);
                                if (_item) {
                                    return _item;
                                }
                            }
                        }
                    }
                }
            }
        }
        /* 删除json中的文件并返回上一个节点*/
        Class.prototype.deleteJsonItem = function (json, id) {
            var that = this,
                options = that.config;
            for (var i = 0; i < json.length; i++) {
                if (json[i].id === id) {
                    json.splice(i, 1);
                    if (i > 0) {
                        return json[i - 1];
                    }
                    break;
                } {
                    if (json[i].tag === 'grid') {
                        for (var j = 0; j < json[i].columns.length; j++) {
                            if (json[i].columns[j].list.length > 0) {
                                that.deleteJsonItem(json[i].columns[j].list, id);
                            }
                        }
                    }
                }


            }
            return undefined;
        };
        Class.prototype.copyJsonAfterItem = function (json, id) {
            var that = this,
                options = that.config;
            for (var i = 0; i < json.length; i++) {
                if (json[i].id === id) {
                    var _newjson = JSON.parse(JSON.stringify(json[i]));
                    _newjson.id = that.autoId(_newjson.tag);
                    json.splice(i + 1, 0, _newjson);
                    return json[i];
                } else {
                    if (json[i].tag === 'grid') {
                        for (var j = 0; j < json[i].columns.length; j++) {
                            if (json[i].columns[j].list.length > 0) {
                                var _item = that.copyJsonAfterItem(json[i].columns[j].list, id);
                                if (_item) {
                                    return _item;
                                }
                            }
                        }
                    }
                }

            }
            return undefined;
        };
        /**
         * data 表示设计区数据
         * dataSource 表示数据源即一个控件的数据来源
         * 
        */
        Class.prototype.config = {
            version: "1.0.0"
            , formName: "表单示例"
            , formId: "id"
            , generateId: 0
            , data: []
            , dataSource: {}
            , selectItem: undefined
            , headerHide: true
            , htmlCode: {
                css: ''
                , html: ''
                , script: ''
            }
        };

        /* 自动生成ID 当前页面自动排序*/
        Class.prototype.autoId = function (tag) {
            var that = this,
                options = that.config;
            options.generateId = options.generateId + 1;
            return tag + '_' + options.generateId;
        }
        /* 组件定义 */
        Class.prototype.components = {
            input: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _readonly = json.readonly ? 'readonly=""' : '';
                    var _required = json.required ? 'required' : '';
                    if (json.expression !== null && json.expression !== undefined && json.expression !== '') {
                        _required = _required + '|' + json.expression;
                    }
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<input name="{0}" value="{1}" placeholder="{3}" class="layui-input" lay-verify="{6}" {4} {5} style="width:{2}">'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _readonly, _disabled, _required);
                    _html += '</div>';
                    // if(selected){
                    // 	_html +='<div class="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';
                    // }
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _readonly = json.readonly ? 'readonly=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    $('#' + json.id + ' .layui-input-block').empty();
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    }
                    else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    var _html = '';
                    //重绘设计区改id下的所有元素
                    _html += '<input name="{0}" value="{1}" placeholder="{3}" class="layui-input" lay-verify="required" {4} {5} {6} style="width:{2}">'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _readonly, _disabled, _required);
                    $('#' + json.id + ' .layui-input-block').append(_html);
                    form.render('input');
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID
                    var _json = JSON.parse(JSON.stringify(formField.input));
                    _json.id = id == undefined ? autoId(_json.tag) : id;
                    _json.index = index;
                    return _json;
                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                }
            },
            password: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _readonly = json.readonly ? 'readonly=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<input type="password" name="{0}" lay-verify="pass" placeholder="{3}" value="{1}" autocomplete="off" style="width:{2}" {4} {5} {6} class="layui-input">'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _readonly, _disabled, _required);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _readonly = json.readonly ? 'readonly=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    $('#' + json.id + ' .layui-input-block').empty();
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    var _html = '';
                    //重绘设计区改id下的所有元素
                    _html += '<input type="password" name="{0}" lay-verify="pass" placeholder="{3}" value="{1}" autocomplete="off" style="width:{2}" {4} {5} {6} class="layui-input">'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _readonly, _disabled, _required);
                    $('#' + json.id + ' .layui-input-block').append(_html);
                    form.render('input');
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.password));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                }
            },
            select: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block" style="width:calc({0} - 110px)">'.format(json.width);
                    _html += '<select name="{0}" lay-verify="required" {1}>'.format(json.id, _disabled);
                    if (json.defaultValue === undefined) {
                        _html += '<option value="{0}" selected="">{1}</option>'.format('', '请选择');
                    }
                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked) {
                            _html += '<option value="{0}" selected="">{1}</option>'.format(json.options[i].value, json.options[i].text);
                        } else {
                            _html += '<option value="{0}">{1}</option>'.format(json.options[i].value, json.options[i].text);
                        }
                    }
                    _html += '</select>'
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    $('#' + json.id + ' .layui-input-block').empty();
                    $('#' + json.id + ' .layui-input-block').css({ width: 'calc({0} - 110px'.format(json.width) });
                    var _html = '';
                    _html += '<select name="{0}" lay-verify="required" {1}>'.format(json.id, _disabled);
                    //重绘设计区改id下的所有元素
                    if (json.defaultValue === undefined) {
                        _html += '<option value="{0}" selected="">{1}</option>'.format('', '请选择');
                    }
                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked) {
                            _html += '<option value="{0}" selected="">{1}</option>'.format(json.options[i].value, json.options[i].text);
                        } else {
                            _html += '<option value="{0}">{1}</option>'.format(json.options[i].value, json.options[i].text);
                        }
                    }
                    _html += '</select>'
                    $('#' + json.id + ' .layui-input-block').append(_html);
                    form.render('select');
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.select));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;
                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                }
            },
            radio: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked) {
                            _html += '<input type="radio" name="{0}" value="{1}" title="{2}" checked="" {3}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        } else {
                            _html += '<input type="radio" name="{0}" value="{1}" title="{2}" {3}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        }
                    }
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    $('#' + json.id + ' .layui-input-block').empty();
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    var _html = '';
                    //重绘设计区改id下的所有元素
                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked) {
                            _html += '<input type="radio" name="{0}" value="{1}" title="{2}" checked="" {3}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        } else {
                            _html += '<input type="radio" name="{0}" value="{1}" title="{2}" {3}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        }
                    }
                    $('#' + json.id + ' .layui-input-block').append(_html);
                    form.render('radio');
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.radio));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                }
            },
            checkbox: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked) {
                            _html += '<input type="checkbox" name="{0}" title="{1}" checked="" {2}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        } else {
                            _html += '<input type="checkbox" name="{0}" title="{1}" {2}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        }
                    }
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    $('#' + json.id + ' .layui-input-block').empty();
                    var _html = '';
                    //重绘设计区改id下的所有元素
                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked) {
                            _html += '<input type="checkbox" name="{0}" title="{1}" checked="" {2}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        } else {
                            _html += '<input type="checkbox" name="{0}" title="{1}" {2}>'.format(json.tag, json.options[i].value, json.options[i].text, _disabled);
                        }
                    }
                    $('#' + json.id + ' .layui-input-block').append(_html);
                    form.render('checkbox');
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.checkbox));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                }
            },
            switch: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    _html += '<input type="checkbox" name="{0}" lay-skin="switch" lay-text="ON|OFF" {1}>'.format(json.tag, _disabled);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    $('#' + json.id + ' .layui-input-block').empty();
                    var _html = '';
                    _html += '<input type="checkbox" name="{0}" lay-skin="switch" lay-text="ON|OFF" {1}>'.format(json.tag, _disabled);
                    $('#' + json.id + ' .layui-input-block').append(_html);
                    form.render('checkbox');
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.switch));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                }
            },
            slider: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block" style="width:calc({0} - 110px);">'.format(json.width);
                    _html += '<div id="{0}" class="widget-slider"></div>'.format(json.tag + json.id);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    $('#' + json.id + ' .layui-input-block').css({ width: 'calc({0} - 110px'.format(json.width) });
                    slider.render({
                        elem: '#' + json.tag + json.id,
                        value: json.defaultValue, //初始值 
                        min: json.minValue,
                        max: json.maxValue,
                        step: json.stepValue,
                        disabled: json.disabled
                    });
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID  
                    var _json = JSON.parse(JSON.stringify(formField.slider));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                }
            },
            date: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block" style="width:calc({0} - 110px);">'.format(json.width);
                    _html += '<div id="{0}" class="layui-input icon-date widget-date"></div>'.format(json.tag + json.id);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    $('#' + json.id + ' .layui-input-block').css({ width: 'calc({0} - 110px'.format(json.width) });
                    laydate.render({
                        elem: '#' + json.tag + json.id,
                        type: json.datetype,
                        range: json.range,
                        format: json.dateformat,
                        value: json.defaultValue,
                        isInitValue: json.isInitValue,
                        min: json.minValue,
                        max: json.maxValue,
                        position: json.position,
                        zindex: json.zindex,
                        theme: json.theme
                    });
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID  
                    var _json = JSON.parse(JSON.stringify(formField.date));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }
                    }
                    $('#columnProperty').append(_html);
                    form.render('select');
                }
            },
            rate: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _readonly = json.readonly ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    _html += '<div id="{0}" class="widget-rate"></div>'.format(json.tag + json.id);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    rate.render({
                        elem: '#' + json.tag + json.id,
                        value: json.defaultValue,
                        text: json.text,
                        length: json.rateLength,
                        half: json.half,
                        readonly: json.readonly
                    });
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.rate));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }

                    }
                    $('#columnProperty').append(_html);
                    form.render('select');
                }
            },
            carousel: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    // _html +='<label class="layui-form-label {0}">{1}:</label>'.format(json.required?'required':'',json.label);
                    // _html +='<div class="layui-input-block">';
                    _html += '<div class="layui-carousel" id="{0}">'.format(json.tag + json.id);
                    _html += '<div carousel-item class="carousel-item">';
                    for (var i = 0; i < json.options.length; i++) {
                        _html += '<div><img src="{0}" /></div>'.format(json.options[i].value);
                    }
                    _html += '</div>';//end for div carousel-item
                    _html += '</div>';//end for class=layui-carousel
                    // _html +='</div>'; 
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    $('#' + json.id).empty();
                    var _html = '';
                    //重绘设计区改id下的所有元素
                    _html += '<div class="layui-carousel" id="{0}">'.format(json.tag + json.id);
                    _html += '<div carousel-item class="carousel-item">';
                    for (var i = 0; i < json.options.length; i++) {
                        _html += '<div><img src="{0}" /></div>'.format(json.options[i].value);
                    }
                    _html += '</div>';//end for div carousel-item
                    _html += '</div>';//end for class=layui-carousel
                    $('#' + json.id).append(_html);
                    carousel.render({
                        elem: '#' + json.tag + json.id,
                        width: json.width,//设置容器宽度
                        arrow: json.arrow, //始终显示箭头
                        //anim: 'updown' //切换动画方式
                    });
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.carousel));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }

                    }
                    $('#columnProperty').append(_html);
                }
            },
            colorpicker: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    _html += '<div id="{0}" class="widget-rate"></div>'.format(json.tag + json.id);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    colorpicker.render({
                        elem: '#' + json.tag + json.id,
                        colorformat: json.colorformat,
                        alpha: json.alpha,
                        done: function (color) {
                            // console.log(color)
                            //譬如你可以在回调中把得到的 color 赋值给表单
                        }
                    });
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.colorpicker));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }

                    }
                    $('#columnProperty').append(_html);
                    form.render('select');
                }
            },
            image: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    _html += '<div class="layui-upload">';
                    _html += '<button type="button" class="layui-btn" id="{0}">图片上传</button>'.format(json.tag + json.id);
                    _html += '<blockquote class="layui-elem-quote layui-quote-nm" style="margin-top: 10px;width: 88%">预览图：';
                    _html += '<div class="layui-upload-list uploader-list" style="overflow: auto;" id="uploader-list-{0}">'.format(json.id);
                    _html += '</div>';
                    _html += '</blockquote>';
                    _html += '</div>';


                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.image));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }

                    }
                    $('#columnProperty').append(_html);
                }
            },
            textarea: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block" style="width:calc({0} - 110px)">'.format(json.width);
                    _html += '<textarea name="{0}" placeholder="{3}" width="{2}" class="layui-textarea" {4} {5}>{1}</textarea>'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _disabled, _required);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    var _disabled = json.disabled ? 'disabled=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                    $('#' + json.id + ' .layui-input-block').empty();
                    $('#' + json.id + ' .layui-input-block').css({ width: '{0}'.format(json.width) });
                    var _html = '';
                    _html += '<textarea name="{0}" placeholder="{3}" width="{2}" class="layui-textarea" {4} {5}>{1}</textarea>'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _disabled, _required);
                    $('#' + json.id + ' .layui-input-block').append(_html);
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.textarea));
                    _json.id = id == undefined ? autoId(_json.tag) : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }

                    }
                    $('#columnProperty').append(_html);
                }
            },
            editor: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _html = '<div id="{0}" class="layui-form-item {2}"  data-id="{0}" data-tag="{1}" data-index="{3}">'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<textarea id="{0}" style="display: none; "></textarea>'.format(json.tag + json.id);
                    _html += '</div>';
                    // if(selected){
                    // 	_html +='<div class="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';
                    // }
                    _html += '</div>';
                    return _html;
                },
                update: function (json) {
                    if (!json.required) {
                        $('#' + json.id).find('.layui-form-label').removeClass('required');
                    } else {
                        $('#' + json.id).find('.layui-form-label').addClass('required');
                    }
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.editor));
                    _json.id = id == undefined ? autoId(_json.tag) : id;
                    _json.index = index;
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }

                    }
                    $('#columnProperty').append(_html);
                }
            },
            grid: {
                /**
                 * 根据json对象生成html对象
                 * @param {object} json 
                 * @param {boolean} selected true 表示选择当前 
                 * */
                render: function (json, selected) {
                    if (selected === undefined) { selected = false; }
                    var _html = '<div id="{0}" class="layui-form-item layui-row grid {2}"  data-id="{0}" data-tag="{1}" data-index="{3}" >'.format(json.id, json.tag, selected ? 'active' : '', json.index);
                    var colClass = 'layui-col-md6 layui-col-sm6';
                    if (json.columns.length == 3) {
                        colClass = 'layui-col-md4 layui-col-sm4';
                    } else if (json.columns.length == 4) {
                        colClass = 'layui-col-md3 layui-col-sm3';
                    } else if (json.columns.length == 6) {
                        colClass = 'layui-col-md2 layui-col-sm2';
                    }
                    for (var i = 0; i < json.columns.length; i++) {
                        _html += '<div class="{2} widget-col-list column{0}" data-index="{0}" data-parentindex="{1}">'.format(i, json.index, colClass);
                        //some html 
                        _html += '</div>';
                    }

                    // if(selected){
                    // 	_html +='<div class="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';
                    // }
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 默认是一个一行两列的布局对象 
                    var _json = JSON.parse(JSON.stringify(formField.grid));
                    _json.id = id == undefined ? autoId(_json.tag) : id;
                    _json.index = index;
                    if (columncount > 2) {
                        var _col = {
                            span: 12,
                            list: [],
                        };
                        for (var i = _json.columns.length; i < columncount; i++) {
                            _json.columns.splice(i, 0, _col);
                        }
                    }
                    return _json;

                },
                /* 根据 json 对象显示对应的属性 */
                property: function (json) {
                    $('#columnProperty').empty();
                    var _html = '';
                    _html = renderCommonProperty(json);//获取通用属性HTML字符串
                    //处理特殊字符串
                    for (var key in json) {
                        if (key === 'index') { continue; }

                    }

                    $('#columnProperty').append(_html);
                    //如果存在 option 那么向 .option .layui-inline 添加drag事件并且必须设在 select-option-drag 中才能拖动
                }
            }

        };


        /* 给字段属性绑定事件 实现双向绑定？*/
        Class.prototype.bindPropertyEvent = function (_json) {
            var that = this
                , options = that.config;
            if (options.data === undefined) { return; }
            if (typeof (options.data) === 'string') {
                options.data = JSON.parse(options.data);
            }
            var _property = $('#columnProperty');
            var _columns = _property.find('input,textarea,select,checkbox');
            var _json = options.selectItem;
            //没有可以选择的
            if (_json === undefined) { return; }
            form.on('checkbox', function (data) {
                //data.elem.closest('.layui-form-item')
                if (_json.tag === 'checkbox') {
                    var _index = parseInt(data.elem.closest('.layui-form-item').dataset.index);
                    for (var i = 0; i < _json.options.length; i++) {
                        if (i === _index) {
                            _json.options[i].checked = data.elem.checked;
                            break;
                        }
                    }
                    that.components[_json.tag].update(_json);
                    that.renderForm();
                }
            });
            form.on('radio', function (data) {
                //data.elem.closest('.layui-form-item')
                if (_json.tag === 'radio') {
                    var _index = parseInt(data.elem.closest('.layui-form-item').dataset.index);
                    for (var i = 0; i < _json.options.length; i++) {
                        if (i === _index) {
                            _json.options[i].checked = data.elem.checked;
                            break;
                        }
                    }
                    that.components[_json.tag].update(_json);
                    that.renderForm();
                }
            });
            form.on('select', function (data) {
                var _key = data.elem.name;
                var _value = parseInt(data.value);
                var _json = options.selectItem;
                if (_key === 'columns') {
                    var columnCount = _json[_key].length;
                    var nullJson = {
                        span: 12,
                        list: []
                    };
                    if (_value > columnCount) {
                        for (var i = columnCount + 1; i <= _value; i++) {
                            _json[_key].splice(i, 0, nullJson);
                        }
                    }

                    that.renderForm();

                } else if (_key === 'dateformat') {
                    //更新 data 数据
                    _json.dateformat = data.value;

                    laydate.render({
                        elem: '#' + _json.tag + _json.id,
                        type: _json.datetype,
                        range: _json.range,
                        format: _json.dateformat,
                        value: _json.defaultValue,
                        isInitValue: _json.isInitValue,
                        min: _json.minValue,
                        max: _json.maxValue,
                        position: _json.position,
                        zindex: _json.zindex,
                        theme: _json.theme
                    });
                    that.renderForm();
                } else if (_key === 'datetype') {
                    //更新 data 数据
                    _json.datetype = data.value;
                    switch (_json.datetype) {
                        case "date":
                            _json.dateformat = "yyyy-MM-dd";
                            break;
                        case "datetime":
                            _json.dateformat = "yyyy-MM-dd HH:mm:ss";
                            break;
                        case "year":
                            _json.dateformat = "yyyy";
                            break;
                        case "month":
                            _json.dateformat = "yyyy-MM";
                            break;
                    }

                    laydate.render({
                        elem: '#' + _json.tag + _json.id,
                        type: _json.datetype,
                        range: _json.range,
                        format: _json.dateformat,
                        value: _json.defaultValue,
                        isInitValue: _json.isInitValue,
                        min: _json.minValue,
                        max: _json.maxValue,
                        position: _json.position,
                        zindex: _json.zindex,
                        theme: _json.theme
                    });
                    that.renderForm();
                } else if (_key === 'expression') {
                    _json.expression = data.value;
                    that.components[_json.tag].update(_json);
                    that.renderForm();
                }
            });
            form.on('switch', function (data) {
                var _key = data.elem.name;
                var _value = data.elem.checked ? true : false;
                if (_key === 'readonly' || _key == 'disabled' || _key === 'required' || _key === 'half' || _key === 'text') {
                    _json[_key] = _value;
                    that.components[_json.tag].update(_json);//局部更新
                    that.renderForm();
                }
            });
            //options 的添加事件
            if (_json.hasOwnProperty('options')) {
                $('#select-option-add').on('click', function () {

                    //添加html
                    _json.options.splice(_json.options.length + 1, 0, { text: 'option', value: 'value', checked: false });

                    var _htmloption = '';
                    _htmloption += '<div class="layui-form-item option select-options" data-index="{0}">'.format(_json.options.length + 1);
                    _htmloption += '  <div class="layui-inline" style="width: 30px; margin-right: 0px;">';
                    if (_json.tag === 'checkbox') {
                        _htmloption += '   <input type="checkbox" name="{0}" lay-skin="primary" title="">'.format(_json.tag);
                    } else {
                        _htmloption += '   <input type="radio" name="{0}" >'.format(_json.tag);
                    }
                    _htmloption += '  </div>';
                    _htmloption += '  <div class="layui-inline" style="margin-right: 0px;width:110px;">';
                    _htmloption += '   <input type="text" name="{0}-text"  autocomplete="off" value="{1}" class="layui-input">'.format(_json.tag, 'option');
                    _htmloption += '  </div>';
                    _htmloption += '  <div class="layui-inline" style="margin-right: 0px;width:110px;">';
                    _htmloption += '   <input type="text" name="{0}-value"  autocomplete="off" value="{1}" class="layui-input">'.format(_json.tag, 'value');
                    _htmloption += '  </div>';
                    _htmloption += '  <div class="layui-inline" style="margin-right: 0px;">';
                    _htmloption += '   <i class="layui-icon layui-icon-slider select-option-drag" style="color:blue;font-size:20px;"></i>';
                    _htmloption += '   <i class="layui-icon layui-icon-delete select-option-delete" style="color:red;font-size:20px;"></i>';
                    _htmloption += '  </div>';
                    _htmloption += '</div>';
                    $('#columnProperty .select-options').last().after(_htmloption);
                    _html = '';
                    if (_json.tag === 'checkbox') {
                        //同步到设计视图checkbox
                        _html += '<input type="checkbox" name="{0}" title="{1}" >'.format(_json.tag, 'option');
                        $('#' + _json.id + ' .layui-input-block').append(_html);
                    } else if (_json.tag === 'radio') {
                        //同步到设计视图radio
                        _html += '<input type="radio" name="{0}" title="{1}" >'.format(_json.tag, 'option');
                        $('#' + _json.id + ' .layui-input-block').append(_html);
                    } else if (_json.tag === 'carousel') {

                    }

                    if (_json.tag === 'checkbox') {
                        form.render('checkbox');
                    } else if (_json.tag === 'radio') {
                        form.render('radio');
                    } else if (_json.tag == 'carousel') {
                        that.components[_json.tag].update(_json);
                        form.render('radio');
                        carousel.render({
                            elem: '#' + item.tag + item.id,
                            width: item.width,//设置容器宽度
                            arrow: item.arrow, //始终显示箭头
                            //anim: 'updown' //切换动画方式
                        });
                    }

                });

                //委托监听先关闭在增加 click
                $(document).off('click', '#columnProperty .select-option-delete').on('click', '#columnProperty .select-option-delete', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    //从数据源 options.data 中删除节点
                    var _index = $(this).closest('.layui-form-item')[0].dataset.index;
                    if (_index !== undefined) {
                        _json.options.splice(_index, 1);//删除此节点
                    }
                    //从html中删除节点 改成全部重绘
                    // $(this).closest('.layui-form-item').remove()
                    $('#' + _json.tag).empty();
                    //删除所有的选项
                    //选项
                    var _html = '';
                    for (var i = 0; i < _json.options.length; i++) {
                        _html += '<div class="layui-form-item option select-options" data-index="{0}">'.format(i);
                        _html += '  <div class="layui-inline" style="width: 30px; margin-right: 0px;">';
                        if (_json.tag === 'checkbox') {
                            _html += '   <input type="checkbox" name="{0}" lay-skin="primary" title="">'.format(_json.tag);
                        } else {
                            _html += '   <input type="radio" name="{0}" >'.format(_json.tag);
                        }
                        _html += '  </div>';
                        _html += '  <div class="layui-inline" style="margin-right: 0px;width:110px;">';
                        _html += '   <input type="text" name="{0}-text"  autocomplete="off" value="{1}" class="layui-input">'.format(_json.tag, _json.options[i].text);
                        _html += '  </div>';
                        _html += '  <div class="layui-inline" style="margin-right: 0px;width:110px;">';
                        _html += '   <input type="text" name="{0}-value"  autocomplete="off" value="{1}" class="layui-input">'.format(_json.tag, _json.options[i].value);
                        _html += '  </div>';
                        _html += '  <div class="layui-inline" style="margin-right: 0px;">';
                        _html += '   <i class="layui-icon layui-icon-slider select-option-drag" style="color:blue;font-size:20px;"></i>';
                        _html += '   <i class="layui-icon layui-icon-delete select-option-delete" style="color:red;font-size:20px;"></i>';
                        _html += '  </div>';
                        _html += '</div>';
                        //向 .option .layui-inline 添加drag事件并且必须设在 select-option-drag 中才能拖动
                    }
                    $('#' + _json.tag).append(_html);
                    //更新设计区节点
                    that.components[_json.tag].update(_json);
                    if (_json.tag === 'checkbox') {
                        form.render('checkbox');
                    } else if (_json.tag === 'radio') {
                        form.render('radio');
                    }


                });
            }

            $(document).off('click', '.select-options input[type=checkbox]').on('click', '.select-options input[type=checkbox]', function (e) {
                // console.log(e);
                //判断是否选中
                //找到 id=key 下的 option值
                var _options = [];
                //遍历options 的值
                $('#columnProperty .select-options').each(function () {
                    _options.push({ text: $(this).find('input[name=select-text]').val(), value: $(this).find('input[name=select-value]').val() });
                    // console.log($(this));
                });

                //_json.options 节点值替换为 _options 值
                _json.options = JSON.parse(JSON.stringify(_options));
            });


            //属性模块的 input keyup
            $(document).off('keyup', '#columnProperty .layui-input').on('keyup', '#columnProperty .layui-input', function () {
                if ($(this).attr("name") !== undefined) {
                    //去改变值 
                    //改变json的值
                    var _key = $(this).attr("name");
                    var _value = $(this).val();
                    var _json = options.selectItem;
                    if (_key === 'id') {
                        return;
                    }
                    if (_key === 'label') {
                        _json[_key] = _value;
                        $('#' + _json.id).find('.layui-form-label').text(_value);
                    }
                    if (_key === 'placeholder') {
                        _json[_key] = _value;
                        $('#' + _json.id).find('.layui-input').attr('placeholder', _value);
                    }
                    if (_key === 'width') {
                        _json[_key] = _value;
                        // $('#' + _json.id).find('.layui-input').css({ "width": _value });
                        that.components[_json.tag].update(_json);//局部更新
                    }
                    if (_key === 'height') {
                        _json[_key] = _value;
                        $('#' + _json.id).find('.layui-input').css({ "height": _value });
                        if (_json.tag === 'editor') {
                            layedit.build(_json.tag + _json.id, {
                                height: _value
                            }); //建立编辑器
                        }
                    }
                    if (_key === 'defaultValue') {
                        _json[_key] = _value;
                        if (_json.tag === 'slider') {
                            slider.render({
                                elem: '#' + _json.tag + _json.id,
                                value: _json.defaultValue, //初始值 
                                min: _json.minValue,
                                max: _json.maxValue,
                                step: _json.stepValue,
                                disabled: _json.disabled
                            });
                        } else if (_json.tag === 'rate') {
                            rate.render({
                                elem: '#' + _json.tag + _json.id,
                                value: _json.defaultValue,
                                text: _json.text,
                                length: _json.rateLength,
                                half: _json.half,
                                readonly: _json.readonly
                            });
                        }
                        else {
                            $('#' + _json.id).find('.layui-input').val(_value);
                        }
                    }
                    if (_key === "minValue" || _key === "maxValue" || _key === "stepValue") {
                        _json[_key] = _value;
                        if (_json.tag === 'slider') {
                            slider.render({
                                elem: '#' + _json.tag + _json.id,
                                value: _json.defaultValue, //初始值 
                                min: _json.minValue,
                                max: _json.maxValue,
                                step: _json.stepValue,
                                disabled: _json.disabled
                            });
                        }
                    }
                    if (_key === 'rateLength') {
                        _json[_key] = _value;
                        if (_json.tag === 'rate') {
                            rate.render({
                                elem: '#' + _json.tag + _json.id,
                                value: _json.defaultValue,
                                text: _json.text,
                                length: _json.rateLength,
                                half: _json.half,
                                readonly: _json.readonly
                            });
                        }
                    }

                    if (_key === 'readonly') {
                        _json[_key] = _value;
                        $('#' + _json.id).find('.layui-input').attr('readonly', _value);
                    }
                    if (_key === 'select-text' || _key === 'select-value') {
                        //找到 id=key 下的 option值
                        var _options = [];
                        //遍历options 的值
                        $('#columnProperty .select-options').each(function () {
                            _options.push({ text: $(this).find('input[name=select-text]').val(), value: $(this).find('input[name=select-value]').val() });
                            // console.log($(this));
                        });
                        //_json.options 节点值替换为 _options 值
                        _json.options = JSON.parse(JSON.stringify(_options));
                        that.components[_json.tag].update(_json);//局部更新
                    }
                    if (_key === 'carousel-text' || _key === 'carousel-value') {
                        //找到 id=key 下的 option值
                        var _options = [];
                        //遍历options 的值
                        $('#columnProperty .select-options').each(function () {
                            _options.push({ text: $(this).find('input[name=carousel-text]').val(), value: $(this).find('input[name=carousel-value]').val() });
                            // console.log($(this));
                        });
                        //_json.options 节点值替换为 _options 值
                        _json.options = JSON.parse(JSON.stringify(_options));
                        that.components[_json.tag].update(_json);//局部更新
                    }


                    if (_key === 'radio-text' || _key === 'radio-value') {
                        //找到 id=key 下的 option值
                        var _options = [];
                        //遍历options 的值
                        $('#columnProperty .select-options').each(function () {
                            _options.push({ text: $(this).find('input[name=radio-text]').val(), value: $(this).find('input[name=radio-value]').val() });
                            // console.log($(this));
                        });
                        //_json.options 节点值替换为 _options 值
                        _json.options = JSON.parse(JSON.stringify(_options));
                        that.components[_json.tag].update(_json);//局部更新
                    }

                    if (_key === 'checkbox-text' || _key === 'checkbox-value') {
                        //找到 id=key 下的 option值
                        var _options = [];
                        //遍历options 的值
                        $('#columnProperty .select-options').each(function () {
                            _options.push({ text: $(this).find('input[name=checkbox-text]').val(), value: $(this).find('input[name=checkbox-value]').val() });
                            // console.log($(this));
                        });
                        //_json.options 节点值替换为 _options 值
                        _json.options = JSON.parse(JSON.stringify(_options));
                        that.components[_json.tag].update(_json);//局部更新
                    }
                }
            });

            $(document).off('blur', '#columnProperty .layui-input').on('blur', '#columnProperty .layui-input', function () {
                if ($(this).attr("name") !== undefined) {
                    //改变json的值
                    var _key = $(this).attr("name");
                    var _value = $(this).val();
                    var _json = options.selectItem;
                    var _oldid = _json.id;
                    if (_key === 'id' && _value !== _oldid) {//标识的更改
                        //检测id是否存在重复
                        var _checkid = that.findJsonItem(options.data, _value);
                        if (_checkid === undefined) {
                            _json[_key] = _value;
                            that.renderForm();
                        } else {
                            //提示层 
                            layer.msg('ID已经存在');
                        }
                    }
                }
            });

            //预览


        }

        /* 如果是grid布局控件 就显示不一样的样式 */
        Class.prototype.addClick = function (evt) {
            var that = this
                , options = that.config;
            $("#formDesignerForm .layui-form-item").on('click', function (e) {
                //当 div 为嵌套关系的时候 阻止事件冒泡
                e.preventDefault();
                e.stopPropagation();
                // console.log("您点击了 formDesignerForm .layui-form-item");
                var index = parseInt($(this)[0].dataset.index);
                var _id = $(this)[0].dataset.id;
                options.selectItem = that.findJsonItem(options.data, _id);
                var tag = $(this)[0].dataset.tag;

                //显示当前的属性
                that.components[tag].property(options.selectItem);
                that.bindPropertyEvent();
                //移除其他元素的 siblings() 方法返回被选元素的所有同级元素
                // $(this).siblings('div').removeClass('active');
                //移除 #formDesignerForm .layui-form-item 下所有的 active 
                $('#formDesignerForm .layui-form-item').removeClass('active');
                //给当前元素增加class
                $(this).addClass('active');
                var _draghtml1 = '<div class="widget-view-action"  id="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag" id="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';

                var len = $(this).children().length;
                if (len <= 12) {

                    //先删除元素
                    $("#formDesignerForm .layui-form-item .widget-view-action").remove();
                    $("#formDesignerForm .layui-form-item .widget-view-drag").remove();

                    // console.log("显示子节点");
                    // console.log($(this).children());
                    if ($('#widget-view-action')) {//已存在
                        $('#widget-view-action').remove();
                    }

                    $(this).children(len - 1).after(_draghtml1);
                }
                /* 向 拷贝 删除 按钮添加 click 动作 */
                that.addCopyDeleteClick();
                //重新渲染
                form.render();
            });

        };


        /* 加入copy选项删除 */
        Class.prototype.addCopyDeleteClick = function () {
            var that = this
                , options = that.config;
            if (options.data === undefined) { return; }
            if (typeof (options.data) === 'string') {
                options.data = JSON.parse(options.data);
            }
            //复制当前节点
            $('#formDesignerForm  .layui-form-item .widget-view-action .layui-icon-file').on('click', function (e) {
                e.stopPropagation();
                //在json中插入
                if (options.data === undefined) { return; }
                if (typeof (options.data) === 'string') {
                    options.data = JSON.parse(options.data);
                }
                var _id = this.parentElement.parentElement.dataset.id;
                if (_id !== undefined) {
                    options.selectItem = that.copyJsonAfterItem(options.data, _id);
                }

                that.renderForm();
            });
            $('#formDesignerForm  .layui-form-item .layui-icon-delete').on('click', function (e) {
                e.stopPropagation();
                //获取当前组件的组件id
                var _id = this.parentElement.parentElement.dataset.id;
                if (_id !== undefined) {
                    options.selectItem = that.deleteJsonItem(options.data, _id);
                }
                //document.elementFromPoint(e.pageX,e.pageY).parentElement.parentElement.parentElement.dataset 获取父grid的信息

                that.renderForm();
                // console.log('click layui-icon layui-icon-delete');
            });
        };



        /* 触发 grid 的 sortablejs 事件*/
        Class.prototype.bindGridSortEvent = function (json) {
            var that = this
                , options = that.config;
            var objs = $('#' + json.id + ' .widget-col-list');
            //遍历他下面的节点
            for (var i = 0; i < objs.length; i++) {
                var gridSortable = Sortable.create(objs[i], {
                    group: {
                        name: 'formgroup'
                    },
                    handle: '.widget-view-drag',
                    ghostClass: "ghost",
                    animation: 150,
                    onAdd: function (evt) {
                        var parentItem = that.findJsonItem(options.data, evt.item.parentElement.parentElement.dataset.id);
                        var index = evt.newIndex;
                        var colIndex = evt.item.parentElement.dataset.index;
                        if (evt.item.dataset.id != undefined) {
                            //表示从其他地方移动过来
                            var _fromItem = JSON.parse(JSON.stringify(that.findJsonItem(options.data, evt.item.dataset.id)));
                            var _oldid = _fromItem.id;
                            _fromItem.id = that.autoId(_fromItem.tag);
                            _fromItem.index = index;
                            parentItem.columns[colIndex].list.splice(index + 1, 0, _fromItem);
                            that.deleteJsonItem(options.data, _oldid);

                        } else {
                            /* 向指定目标放入数据 splice */
                            var tag = evt.item.dataset.tag;
                            _id = that.autoId(tag);
                            var _newitem = that.components[tag].jsonData(_id, evt.newIndex);
                            _newitem.index = index;
                            parentItem.columns[colIndex].list.splice(index + 1, 0, _newitem);
                            options.selectItem = _newitem;
                        }
                        that.renderForm();

                    }
                });
            }

        };

        //渲染视图
        Class.prototype.render = function () {
            var that = this
                , options = that.config;

            options.elem = $(options.elem);
            options.id = options.id || options.elem.attr('id') || that.index;

            //cache 模式

            /* 输入型组件 开始*/
            var _listhtml = ""
            _listhtml += '<div class="components-title">{0} </div>'.format(formField.c1.name);
            _listhtml += '<div class="components-draggable" id="c1">';
            $.each(formField.c1.list
                , function (index
                    , item) {
                    _listhtml += '<div class="components-item" data-tag="{0}"><div class="components-body"><i class="icon iconfont icon-input"></i>{1}</div></div>'.format(item
                        , lang[item]);
                });
            _listhtml += '</div>';
            _listhtml += '<div class="components-title">{0} </div>'.format(formField.c2.name);
            _listhtml += '<div class="components-draggable" id="c2">';

            /* 选择型组件 开始*/
            $.each(formField.c2.list
                , function (index
                    , item) {
                    _listhtml += '<div class="components-item" data-tag="{0}"><div class="components-body"><i class="icon iconfont icon-input"></i>{1}</div></div>'.format(item
                        , lang[item]);
                });
            _listhtml += '</div>';


            _listhtml += '<div class="components-title">{0} </div>'.format(formField.c3.name);
            _listhtml += '<div class="components-draggable" id="c3">';

            /* 布局型组件 开始*/
            $.each(formField.c3.list
                , function (index
                    , item) {
                    _listhtml += '<div class="components-item" data-tag="{0}"><div class="components-body"><i class="icon iconfont icon-input"></i>{1}</div></div>'.format(item
                        , lang[item]);
                });
            _listhtml += '</div>';
            if (options.headerHide == true) {
                TP_MAIN = TP_MAIN.replace('layui-header', 'layui-header layui-hide');
            }
            options.elem.html(TP_MAIN);
            $('#components-form-list').append(_listhtml);
            $('body').append(TP_HTML_VIEW);
            $('body').append(TP_IMPORT_VIEW);//TP_IMPORT_VIEW
            //$('body').append(TP_ABOUT_VIEW);
            //排序事件注册
            var sortable1 = Sortable.create(document.getElementById("c1"), {
                group: {
                    name: 'formgroup',
                    pull: 'clone', //克隆本区域元素到其他区域
                    put: false, //禁止本区域实现拖动或拖入
                },
                ghostClass: "ghost",
                sort: false,
                animation: 150,
                onEnd: function (evt) {
                    // console.log('onEnd.foo:', [evt.item, evt.from]);
                    // console.log(evt.oldIndex);
                    // console.log(evt.newIndex);
                    var itemEl = evt.item;
                    // console.log(itemEl);
                }
            });
            var sortable2 = Sortable.create(document.getElementById("c2"), {
                group: {
                    name: 'formgroup',
                    pull: 'clone',
                    put: false, //禁止本区域实现拖动或拖入
                },
                sort: false,
                animation: 150
            });
            var sortable3 = Sortable.create(document.getElementById("c3"), {
                group: {
                    name: 'formgroup',
                    pull: 'clone',
                    put: false, //禁止本区域实现拖动或拖入
                },
                sort: false,
                animation: 150
            });
            //formDesignerForm
            var formItemSort = Sortable.create(document.getElementById("formDesignerForm"), {
                group: {
                    name: 'formgroup'
                },
                handle: '.widget-view-drag',
                ghostClass: "ghost",
                animation: 200,
                /**
                 * 不同元素之间的拷贝
                 * 如果是从组件区域则是创建一个新的节点
                 * 如果是从设计区则是移动一个节点
                 *  */
                onAdd: function (evt) {
                    // console.log(evt.item.dataset.tag);
                    var columncount = evt.item.dataset.columncount;
                    if (columncount === undefined) { columncount = 2; }
                    if (options.data === undefined) { return; }
                    if (typeof (options.data) === 'string') {
                        options.data = JSON.parse(options.data);
                    }
                    //注意这里的一个bug，newIndex 第一次拖动也是1 第二次拖动也是1
                    if (options.data == null || options.data.length === 0) { evt.newIndex = 0; }

                    if (evt.item.dataset.id !== undefined) {
                        /*根据id的新算法 复制一份副本 删除json中的节点 再插入节点*/
                        var _item = that.findJsonItem(options.data, evt.item.dataset.id);
                        options.selectItem = _item;
                        that.deleteJsonItem(options.data, evt.item.dataset.id);
                        options.data.splice(evt.newIndex + 1, 0, _item);
                    }
                    else {
                        var _id = that.autoId(evt.item.dataset.tag);
                        /* 向现有的表单数据中增加新的数组元素 splice */
                        var _newitem = that.components[evt.item.dataset.tag].jsonData(_id, evt.newIndex, columncount);
                        //如果是 grid 呢，需要知道几列
                        options.selectItem = _newitem;
                        options.data.splice(evt.newIndex, 0, _newitem);//options.data.splice(evt.newIndex + 1, 0, _newitem);

                        // console.log("add new item" + options.data);
                        /* 如果evt.item.dataset.id 有值表示从已经存在的布局中获取了元素，这个时候要在数据源中删除这个元素*/
                    }



                    //局部更新 只要更新 designer 设计区部分
                    that.renderForm();
                },
                onEnd: function (evt) {
                    var itemEl = evt.item;
                    //只有当to的目标容器是 formDesignerForm 才出发次逻辑
                    if (evt.to.id === 'formDesignerForm') {
                        // .moveJsonArrayItem(evt.oldIndex, evt.newIndex);
                    }
                }
            });

            //导出数据
            $('.exportJson').on('click', function () {
                document.getElementById('generate-code-view').value = JSON.stringify(options.data, null, 4);

                layer.open({
                    type: 1
                    , title: 'JSON 数据导出'
                    , id: 'Lay_layer_htmlcodeview'
                    , content: $('.htmlcodeview')
                    , area: ['800px', '600px']
                    , shade: false
                    , resize: false
                    , success: function (layero, index) {
                        layer.style(index, {
                            marginLeft: -220
                        });
                    }
                    , end: function () {
                    }
                });
            });
            //导入数据
            $('.importJson').on('click', function () {
                document.getElementById('import-json-code').value = JSON.stringify(options.data, null, 4);

                layer.open({
                    type: 1
                    , title: 'JSON模板数据导入'
                    , id: 'Lay_layer_importjsoncodeview'
                    , content: $('.importjsoncodeview')
                    , area: ['800px', '600px']
                    , shade: false
                    , resize: false
                    , success: function (layero, index) {
                        layer.style(index, {
                            marginLeft: -220
                        });
                    }
                    , end: function () {
                    }
                });
            });
            $('#copy-html-code').on('click', function () {
                var Url2 = document.getElementById("generate-code-view");
                Url2.select(); // 选择对象
                document.execCommand("Copy"); // 执行
                layer.msg('复制成功');
            });
            $('#generateSetValue').on('click', function () {
                document.getElementById('generate-code-view').value = JSON.stringify(options.data, null, 4);
            });
            $('#import-json-code').on('click', function () {
                var _value = document.getElementById("import-json-code-view").value;
                options.data = JSON.parse(_value);
                that.renderForm();
                layer.closeAll();
                layer.msg('导入成功');
            });
            $('.previewForm').on('click', function () {
                window.localStorage.setItem('layui_form_json', JSON.stringify(options.data));
                //iframe窗

                layer.open({
                    type: 2,
                    title: '表单预览',
                    btn: ['关闭'], //可以无限个按钮
                    btn1: function (index, layero) {
                        layer.close(index);
                    },
                    closeBtn: 1, //不显示关闭按钮
                    shade: [0],
                    area: ['100%', '100%'],
                    offset: 'auto', //右下角弹出 
                    anim: 2,
                    content: ['../../page/preview.html', 'yes'], //iframe的url，no代表不显示滚动条
                    end: function () { //此处用于演示
                        //加载结束
                    }
                });
            });
            $('.templateList').on('click', function () {
                $.ajax({
                    url: document.location.origin + '/json/demo.json',
                    dataType: "json",
                    async: true,
                    type: "GET",
                    success: function (result) {
                        options.data = result;
                        that.renderForm();
                    }
                });
            });
            that.renderForm();
        };

        /* 递归渲染组件 */
        Class.prototype.renderComponents = function (jsondata, elem) {
            var that = this
                , options = that.config;
            $.each(jsondata, function (index, item) {
                item.index = index;//设置index 仅仅为了传递给render对象，如果存在下级子节点那么 子节点的也要变动
                if (options.selectItem === undefined) {
                    elem.append(that.components[item.tag].render(item, false));
                } else {
                    if (options.selectItem.id === item.id) {
                        elem.append(that.components[item.tag].render(item, true));
                        //显示当前的属性
                        that.components[item.tag].property(item);
                        that.bindPropertyEvent(item);
                    } else {
                        elem.append(that.components[item.tag].render(item, false));
                    }
                }
                if (item.tag === 'grid') {
                    that.bindGridSortEvent(item);
                    $.each(item.columns, function (index2, item2) {
                        //获取当前的 DOM 对象
                        var elem2 = $('#' + item.id + ' .widget-col-list').filter('.column' + index2);
                        if (item2.list.length > 0) {
                            that.renderComponents(item2.list, elem2);
                        }
                    });
                } else if (item.tag === 'slider') {
                    //定义初始值
                    slider.render({
                        elem: '#' + item.tag + item.id,
                        value: item.defaultValue, //初始值 
                        min: item.minValue,
                        max: item.maxValue,
                        step: item.stepValue
                    });
                } else if (item.tag === 'date') {
                    laydate.render({
                        elem: '#' + item.tag + item.id,
                        type: item.datetype,
                        range: item.range,
                        format: item.dateformat,
                        value: item.defaultValue,
                        isInitValue: item.isInitValue,
                        min: item.minValue,
                        max: item.maxValue,
                        position: item.position,
                        zindex: item.zindex,
                        theme: item.theme

                    });
                } else if (item.tag === 'rate') {
                    rate.render({
                        elem: '#' + item.tag + item.id,
                        value: item.defaultValue,
                        text: item.text,
                        half: item.half,
                        length: item.rateLength,
                        readonly: item.readonly
                    });
                } else if (item.tag === 'colorpicker') {
                    colorpicker.render({
                        elem: '#' + item.tag + item.id,
                        done: function (color) {
                            // console.log(color)
                            //譬如你可以在回调中把得到的 color 赋值给表单
                        }
                    });
                } else if (item.tag === 'editor') {
                    layedit.build(item.tag + item.id, {
                        height: item.height
                    }); //建立编辑器
                } else if (item.tag === 'carousel') {
                    carousel.render({
                        elem: '#' + item.tag + item.id,
                        width: item.width,//设置容器宽度
                        arrow: item.arrow, //始终显示箭头
                        //anim: 'updown' //切换动画方式
                    });
                } else if (item.tag === 'image') {
                    upload.render({
                        elem: '#' + item.tag + item.id
                        , url: 'https://httpbin.org/post'
                        , multiple: true
                        , before: function (obj) {
                            layer.msg('图片上传中...', {
                                icon: 16,
                                shade: 0.01,
                                time: 0
                            })
                        }
                        , done: function (res) {
                            layer.close(layer.msg());//关闭上传提示窗口
                            //上传完毕
                            $('#uploader-list-' + item.id).append(
                                '<div id="" class="file-iteme">' +
                                '<div class="handle"><i class="layui-icon layui-icon-delete"></i></div>' +
                                '<img style="width: 100px;height: 100px;" src=' + res.data.src + '>' +
                                '<div class="info">' + res.data.title + '</div>' +
                                '</div>'
                            );
                        }
                    });
                }
            });
        };

        /* 重新渲染设计区*/
        Class.prototype.renderForm = function () {
            var that = this
                , options = that.config;
            var elem = $('#formDesignerForm');
            //清空
            elem.empty();
            that.renderComponents(options.data, elem);

            //选中的节点只有一个
            if (options.selectItem !== undefined) {
                var _draghtml1 = '<div class="widget-view-action"  id="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag" id="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';
                var len = $('#' + options.selectItem.id).children().length;
                if ($('#widget-view-action')) {//已存在
                    $('#widget-view-action').remove();
                }
                $('#' + options.selectItem.id).children(len - 1).after(_draghtml1);
                $('#formDesignerForm .layui-form-item').removeClass('active');
                //给当前元素增加class
                $('#' + options.selectItem.id).addClass('active');
                //设置当前选择项目的拷贝删除的事件

                //显示当前的属性
                that.components[options.selectItem.tag].property(options.selectItem);
                that.bindPropertyEvent(options.selectItem);

            }
            that.addClick();

            /* 向 拷贝 删除 按钮添加 click 动作 */
            that.addCopyDeleteClick();
            document.getElementById('generate-code-view').value = JSON.stringify(options.data, null, 4);
            form.render();

        };

        /* 渲染预览框 */
        Class.prototype.renderPreview = function () {
            var that = this
                , options = that.config;
        };

        Class.prototype.reload = function (id
            , options) {
            var that = this;
            options = options || {};//如果是空的话，就赋值 {} 
            that.render();
        }

        //核心入口 初始化一个 formDesigner类
        formDesigner.render = function (options) {
            var ins = new Class(options);
            return thisIns.call(ins);
        };
        exports('formDesigner'
            , formDesigner);
    });