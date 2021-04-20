layui.define(['layer', 'laytpl', 'element', 'form', 'slider', 'laydate', 'rate', 'colorpicker', 'layedit', 'carousel', 'upload', 'formField', 'common']
    , function (exports) {
        var $ = layui.jquery
            , layer = layui.layer
            , laytpl = layui.laytpl
            , setter = layui.cache
            , element = layui.element
            , slider = layui.slider
            , laydate = layui.laydate
            , rate = layui.rate
            , colorpicker = layui.colorpicker
            , carousel = layui.carousel
            , form = layui.form
            , upload = layui.upload
            , layedit = layui.layedit
            , formField = layui.formField
            , common = layui.common
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
                append: "",
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
                carousel: "轮播"

            }
            , MOD_NAME = 'formPreview'
            , TPL_SUBMIT = ['<div class="layui-form-item layui-hide">'
                , '<div class="layui-input-block">'
                , '<button type="submit" class="layui-btn" lay-submit="" lay-filter="formPreviewSubmit">立即提交</button>'
                , '<button type="reset" class="layui-btn layui-btn-primary">重置</button>'
                , '</div>'
                , '</div>'].join('')
            //外部接口

            , formPreview = {
                index: layui.formPreview ? (layui.formPreview.index + 10000) : 0

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
                that.index = ++formPreview.index; //增加实例，index 也是要增加
                that.config = $.extend({}
                    , that.config
                    , formPreview.config
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

        Class.prototype.config = {
            version: "1.0.0"
            , formName: "表单示例"
            , formId: "id"
            , generateId: 0
            , data: []
            , selectItem: undefined
        };

        /* 自动生成ID 当前页面自动排序*/
        Class.prototype.autoId = function (tag) {
            var that = this,
                options = that.config;
            options.generateId = options.generateId + 1;
            return tag + '_' + options.generateId;
        };

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
                    var _html = '<div class="layui-form-item" data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<input name="{0}" value="{1}" placeholder="{3}" autocomplete="off" class="layui-input" lay-verify="{6}" {4} {5} style="width:{2}">'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _readonly, _disabled, _required);
                    _html += '</div>';
                    // if(selected){
                    // 	_html +='<div class="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';
                    // }
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID
                    var _json = JSON.parse(JSON.stringify(formField.input));
                    _json.id = id == undefined ? autoId(_json.tag) : id;
                    _json.index = index;
                    return _json;
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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<input type="password" name="{0}" lay-verify="pass" placeholder="{1}" autocomplete="off" value="{2}" autocomplete="off" style="width:{3}" {4} {5} {6} class="layui-input">'
                        .format(json.id, json.placeholder, json.defaultValue ? json.defaultValue : '', json.width, _readonly, _disabled, _required);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.password));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<select name="{0}" id="{0}" lay-verify="required" style="width:{1}">'.format(json.id, json.width);
                    if (json.defaultValue === undefined) {
                        _html += '<option value="{0}" checked>{1}</option>'.format('', '请选择');
                    }
                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked || (!!json.defaultValue && json.defaultValue == json.options[i].value)) {
                            _html += '<option value="{0}" checked>{1}</option>'.format(json.options[i].value, json.options[i].text);
                        } else {
                            _html += '<option value="{0}">{1}</option>'.format(json.options[i].value, json.options[i].text);
                        }
                    }
                    _html += '</select>'
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.select));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;
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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked || (!!json.defaultValue && json.defaultValue == json.options[i].value)) {
                            _html += '<input type="radio" name="{0}" value="{1}" title="{2}" checked="">'.format(json.id, json.options[i].value, json.options[i].text);
                        } else {
                            _html += '<input type="radio" name="{0}" value="{1}" title="{2}">'.format(json.id, json.options[i].value, json.options[i].text);
                        }
                    }
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.radio));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    for (var i = 0; i < json.options.length; i++) {
                        if (json.options[i].checked) {
                            _html += '<input type="checkbox" name="{0}" title="{1}" checked="">'.format(json.id, json.options[i].value, json.options[i].text);
                        } else {
                            _html += '<input type="checkbox" name="{0}" title="{1}" >'.format(json.id, json.options[i].value, json.options[i].text);
                        }
                    }
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.checkbox));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    _html += '<input type="checkbox" name="{0}" id="{0}" lay-skin="switch" lay-text="ON|OFF">'.format(json.id);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.switch));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<input class="layui-hide" hidden type="text" id="{0}" lay-verify="{2}" name="{0}" value="{1}">'.format(json.id, json.defaultValue ? json.defaultValue : '', json.required ? 'required' : '');
                    _html += '<div id="{0}" class="widget-slider"></div>'.format(json.id + "_" + json.tag);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID  
                    var _json = JSON.parse(JSON.stringify(formField.slider));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    _html += '<input id="{0}" name="{0}" class="layui-input icon-date widget-date"  autocomplete="off" lay-verify="{1}" value="{2}"></input>'.format(json.id, json.required ? 'required' : '', json.defaultValue ? json.defaultValue : '');
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID  
                    var _json = JSON.parse(JSON.stringify(formField.date));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<div id="{0}" class="widget-rate"></div>'.format(json.id + "_" + json.tag);
                    _html += '<input class="layui-hide" hidden type="text" id="{0}" lay-verify="{2}" name="{0}" value="{1}">'.format(json.id, json.defaultValue ? json.defaultValue : '', json.required ? 'required' : '');
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.rate));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    // _html +='<label class="layui-form-label {0}">{1}:</label>'.format(json.required?'required':'',json.label);
                    // _html +='<div class="layui-input-block">';
                    _html += '<div class="layui-carousel" id="{0}">'.format(json.id + "_" + json.tag);
                    _html += '<div carousel-item>';
                    for (var i = 0; i < json.options.length; i++) {
                        _html += '<div><img src="{0}" /></div>'.format(json.options[i].value);
                    }
                    _html += '</div>';//end for div carousel-item
                    _html += '</div>';//end for class=layui-carousel
                    // _html +='</div>'; 
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.carousel));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<input class="layui-hide" hidden type="text" id="{0}" lay-verify="{2}" name="{0}" value="{1}">'.format(json.id, json.defaultValue ? json.defaultValue : '', json.required ? 'required' : '');
                    _html += '<div id="{0}" class="widget-rate"></div>'.format(json.id + "_" + json.tag);
                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.colorpicker));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';

                    _html += '<div class="layui-upload">';
                    _html += '<button type="button" class="layui-btn" id="{0}">图片上传</button>'.format(json.id + '_' + json.tag);
                    _html += '<input class="layui-hide" hidden type="text" id="{0}" lay-verify="{2}" name="{0}" value="{1}">'.format(json.id, json.defaultValue ? json.defaultValue : '', json.required ? 'required' : '');
                    _html += '<blockquote class="layui-elem-quote layui-quote-nm" style="margin-top: 10px;width: 88%">预览图：';
                    _html += '<div class="layui-upload-list uploader-list" style="overflow: auto;" id="uploader-list-{0}">'.format(json.id);
                    _html += '<div class="file-iteme"><img class="layui-upload-img" src="{0}"><div class="info"></div>{1}</div>'.format(json.defaultValue ? document.location.origin + json.defaultValue : "", json.defaultValue ? json.defaultValue : "");
                    _html += '<p id="{0}"></p>'.format(json.id + "_text");
                    _html += '</div>';
                    _html += '</blockquote>';
                    _html += '</div>';


                    _html += '</div>';
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.image));
                    _json.id = id == undefined ? guid() : id;
                    _json.index = index;
                    return _json;

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
                    var _readonly = json.readonly ? 'readonly=""' : '';
                    var _required = json.required ? 'required=""' : '';
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<textarea name="{0}" id="{0}" placeholder="{3}" width="{2}" class="layui-textarea" {4} {5} {6}>{1}</textarea>'
                        .format(json.id, json.defaultValue ? json.defaultValue : '', json.width, json.placeholder, _readonly, _disabled, _required);
                    _html += '</div>';
                    // if(selected){
                    // 	_html +='<div class="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';
                    // }
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.textarea));
                    _json.id = id == undefined ? autoId(_json.tag) : id;
                    _json.index = index;
                    return _json;

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
                    var _html = '<div class="layui-form-item"  data-id="{0}" data-tag="{1}" data-index="{2}">'.format(json.id, json.tag, json.index);
                    _html += '<label class="layui-form-label {0}">{1}:</label>'.format(json.required ? 'required' : '', json.label);
                    _html += '<div class="layui-input-block">';
                    _html += '<textarea id="{0}" name="{0}" style="display: none; "></textarea>'.format(json.id);
                    _html += '</div>';
                    // if(selected){
                    // 	_html +='<div class="widget-view-action"><i class="layui-icon layui-icon-file"></i><i class="layui-icon layui-icon-delete"></i></div><div class="widget-view-drag"><i class="layui-icon layui-icon-screen-full"></i></div>';
                    // }
                    _html += '</div>';
                    return _html;
                },
                /* 获取对象 */
                jsonData: function (id, index, columncount) {
                    //分配一个新的ID 
                    var _json = JSON.parse(JSON.stringify(formField.editor));
                    _json.id = id == undefined ? autoId(_json.tag) : id;
                    _json.index = index;
                    return _json;

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

                }
            }

        };


        //渲染视图
        Class.prototype.render = function () {
            var that = this
                , options = that.config;

            options.elem = $(options.elem);
            options.id = options.id || options.elem.attr('id') || that.index;


            options.elem.html('<div class="layui-form" style="height:100%;" id="formPreviewForm" lay-filter="previewForm"></div>');
            that.renderForm();
            if (options.readonly) {
                var readForm = $('#formPreviewForm');
                readForm.find('input,textarea,select').prop('disabled', true);
                readForm.find('input,textarea,select').removeAttr('lay-verify');
                readForm.find('.layui-layedit iframe').contents().find('body').prop('contenteditable', false);
            }
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
                    } else {
                        elem.append(that.components[item.tag].render(item, false));
                    }
                }
                if (item.tag === 'grid') {
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
                        elem: '#' + item.id + "_" + item.tag,
                        value: item.defaultValue, //初始值 
                        min: item.minValue,
                        max: item.maxValue,
                        step: item.stepValue
                        , change: function (value) {
                            $('#' + item.id).val(value);
                        }
                    });
                } else if (item.tag === 'date') {
                    laydate.render({
                        elem: '#' + item.id,
                        type: item.datetype
                        , trigger: 'click'
                    });
                } else if (item.tag === 'rate') {
                    rate.render({
                        elem: '#' + item.id + "_" + item.tag,
                        value: item.defaultValue,
                        text: item.text,
                        half: item.half,
                        choose: function (value) {
                            $('#' + item.id).val(value);
                        }
                    });
                } else if (item.tag === 'colorpicker') {
                    colorpicker.render({
                        elem: '#' + item.id + "_" + item.tag,
                        done: function (color) {
                            $('#' + item.id).val(color);
                            //譬如你可以在回调中把得到的 color 赋值给表单
                        }
                        , change: function (color) {
                            $('#' + item.id).val(color);
                        }
                    });
                } else if (item.tag === 'editor') {
                    //图片上传暂时不可用，图片src不能直接查看
                    layedit.build(item.id, {
                        height: item.height
                    }); //建立编辑器
                    if (!!item.defaultValue) {
                        layedit.setContent(layedit.index, item.defaultValue, true);
                    }
                } else if (item.tag === 'carousel') {
                    carousel.render({
                        elem: '#' + item.id + "_" + item.tag,
                        width: item.width,//设置容器宽度
                        arrow: item.arrow, //始终显示箭头
                        //anim: 'updown' //切换动画方式
                    });
                } else if (item.tag === 'image') {
                    var uploadInst = upload.render({
                        elem: '#' + item.id + '_' + item.tag
                        , url: options.uploadUrl //需要替换成自己的接口--todo
                        , data: options.uploadData
                        , multiple: false
                        , before: function (obj) {
                            layer.msg('图片上传中...', {
                                icon: 16,
                                shade: 0.01,
                                time: 0
                            })
                        }, choose: function (obj) {
                            //预读本地文件示例，不支持ie8
                            obj.preview(function (index, file, result) {
                                $('#uploader-list-' + item.id).html(
                                    '<div class="file-iteme">' +
                                    //'<div class="handle"><i class="layui-icon layui-icon-delete"></i></div>' +
                                    '<img class="layui-upload-img" src=' + result + '>' +
                                    '<div class="info">' + file + '</div>' +
                                    '<p id="' + item.id + "_text" + '"></p>' +
                                    '</div>'
                                );
                            });
                        }
                        , done: function (res) {
                            //如果上传失败
                            if (res.code > 0) {
                                //失败状态，并实现重传
                                var demoText = $('#' + item.id + '_text');
                                demoText.html('<span style="color: #FF5722;">上传失败</span> <a class="layui-btn layui-btn-xs updata-reload">重试</a>');
                                demoText.find('.updata-reload').on('click', function () {
                                    uploadInst.upload();
                                });
                                common.modalMsg(res.msg, "warning");
                                return false;
                            }
                            $('#' + item.id).val(res.data[0].src);
                            layer.close(layer.msg());//关闭上传提示窗口
                            //上传完毕
                            $('#uploader-list-' + item.id).html(
                                '<div class="file-iteme">' +
                                //'<div class="handle"><i class="layui-icon layui-icon-delete"></i></div>' +
                                '<img class="layui-upload-img" src=' + document.location.origin + "/file/" + res.data.src + '>' +
                                '<div class="info">' + res.data.title + '</div>' +
                                '</div>' +
                                '<p id="' + item.id + "_text" + '"></p>'
                            );

                        }
                        , error: function () {
                            //演示失败状态，并实现重传
                            var demoText = $('#' + item.id + '_text');
                            demoText.html('<span style="color: #FF5722;">上传失败</span> <a class="layui-btn layui-btn-xs updata-reload">重试</a>');
                            demoText.find('.updata-reload').on('click', function () {
                                uploadInst.upload();
                            });
                        }
                    });
                }
            });

        };

        /* 重新渲染设计区*/
        Class.prototype.renderForm = function () {
            var that = this
                , options = that.config;
            var elem = $('#formPreviewForm');
            //清空
            elem.empty();
            that.renderComponents(options.data, elem);
            elem.append(TPL_SUBMIT);
            form.render();//一次性渲染表单
        };




        //核心入口 初始化一个 regionSelect 类
        formPreview.render = function (options) {
            var defaults = {
                readonly: true,//主键
                uploadUrl: '/FileManage/Uploadfile/Upload',
                uploadData: { filetype: 1, fileby: '流程表单' }
            };
            options = $.extend(defaults, options);
            var ins = new Class(options);
            return thisIns.call(ins);
        };

        exports('formPreview'
            , formPreview);


    });