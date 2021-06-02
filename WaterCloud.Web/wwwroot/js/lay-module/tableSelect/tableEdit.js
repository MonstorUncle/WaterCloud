layui.define(["laydate","laytpl","table","layer"],function(exports) {
    "use strict";
    var moduleName = 'tableEdit',_layui = layui,laytpl = _layui.laytpl
        , $ = _layui.$,
        laydate = _layui.laydate,
        table = _layui.table,
        layer = _layui.layer
        ,selectTpl = [ //单选下拉框模板
            '<div class="layui-tableEdit-div" style="{{d.style}}">'
                ,'<ul class="layui-tableEdit-ul">'
                    ,'{{# d.data = typeof(d.data) === "function"? d.data(d):d.data; if(d.data){ }}'
                        ,'{{# d.data.forEach(function(item){ }}'
                            ,'{{# var selectedClass = d.callbackFn(item) }}'
                            ,'<li class="{{ selectedClass }}" data-name="{{ item.name }}" data-value="{{ item.value }}">'
                                ,'<div class="layui-unselect layui-form-checkbox" lay-skin="primary"><span>{{ item.value }}</span></div>'
                            ,'</li>'
                        ,'{{# }); }}'
                        ,'{{# } else { }}'
                            ,'<li>无数据</li>'
                    ,'{{# } }}'
                ,'</ul>'
            , '</div>'
        ].join('')
        ,selectMoreTpl = [ //多选下拉框模板
            '<div class="layui-tableEdit-div" style="{{d.style}}">'
                ,'<div class="layui-tableEdit-tpl">'
                    ,'<ul>'
                    ,'{{# if(d.data){ }}'
                        ,'{{# d.data.forEach(function(item){ }}'
                            ,'{{# var selectedClass = d.callbackFn(item) }}'
                            ,'<li class="{{ selectedClass }}" data-name="{{ item.name }}" data-value="{{ item.value }}">'
                                ,'<div class="layui-unselect layui-form-checkbox" lay-skin="primary"><span>{{ item.value }}</span><i class="layui-icon layui-icon-ok"></i></div>'
                            ,'</li>'
                        ,'{{# }); }}'
                    ,'{{# } else { }}'
                        ,'<li>无数据</li>'
                    ,'{{# } }}'
                    ,'</ul>'
                ,'</div>'
                ,'<div style="line-height: 36px;">'
                    ,'<div style="float: left">'
                        ,'<button type="button" event-type="close" class="layui-btn layui-btn-sm layui-btn-primary">关闭</button>'
                    ,'</div>'
                    ,'<div style="text-align: right">'
                        ,'<button event-type="confirm" type="button" class="layui-btn layui-btn-sm layui-btn-primary">确定</button>'
                    ,'</div>'
                ,'</div>'
            ,'</div>'
        ].join('');
    //组件用到的css样式
    var thisCss = [];
    thisCss.push('.layui-tableEdit-div{position:absolute;background-color:#fff;font-size:14px;border:1px solid #d2d2d2;z-index:19910908445;max-height: 252px;}');
    thisCss.push('.layui-tableEdit-tpl{max-height:216px;overflow-y:auto;}');
    thisCss.push('.layui-tableEdit-div li{line-height:36px;padding-left:5px;}');
    thisCss.push('.layui-tableEdit-div li:hover{background-color:#f2f2f2;}');
    thisCss.push('.layui-tableEdit-selected{background-color:#5FB878;}');
    thisCss.push('.layui-tableEdit-checked i{background-color:#60b979!important;}');
    thisCss.push('.layui-tableEdit-ul div{padding-left:0px!important;}');
    thisCss.push('.layui-tableEdit-input{text-align:center;position:absolute;left:0;bottom:0;width:100%;height:38px;z-index: 19910908;}');
    thisCss.push('.layui-tableEdit-add{position: absolute;right: 3px;top: 21px;margin-top: -15px;z-index: 199109084;}')
    thisCss.push('.layui-tableEdit-sub{position: absolute;left: 3px;top: 21px;margin-top: -15px;z-index: 199109084;}')
    var thisStyle = document.createElement('style');
    thisStyle.innerHTML = thisCss.join('\n'),document.getElementsByTagName('head')[0].appendChild(thisStyle);

    var configs = {
        callbacks:{}
        ,
        verify: {
            required: [
                /[\S]+/
                ,'必填项不能为空'
            ]
            ,phone: [
                /^1[34578]\d{9}$/
                ,'请输入正确的手机号'
            ]
            ,email: [
                /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/
                ,'邮箱格式不正确'
            ]
            ,url: [
                /(^#)|(^http(s*):\/\/[^\s]+\.[^\s]+)/
                ,'链接格式不正确'
            ]
            ,number:[
                /(^[-+]?\d+$)|(^[-+]?\d+\.\d+$)/
                ,'只能填写数字'
            ]
            ,date: [
                /^(\d{4})[-\/](\d{1}|0\d{1}|1[0-2])([-\/](\d{1}|0\d{1}|[1-2][0-9]|3[0-1]))*$/
                ,'日期格式不正确'
            ]
            ,identity: [
                /(^\d{15}$)|(^\d{17}(x|X|\d)$)/
                ,'请输入正确的身份证号'
            ]
        }
    };

    var Class = function () { //单列模式  也就是只能new一个对象。
        var instance;
        Class = function Class() {
            return instance;
        };
        Class.prototype = this; //保留原型属性
        instance = new Class();
        instance.constructor = Class; //重置构造函数指针
        return instance
    }; //构造器
    var singleInstance = new Class();
    var inFunc = function () {singleInstance.leaveStat = false;},outFunc = function () {singleInstance.leaveStat = true;};
    document.onclick = function () {if(singleInstance.leaveStat)singleInstance.deleteAll();};

    //日期选择框
    Class.prototype.date = function(options){
        var othis = this;
        othis.callback = options.callback,othis.element = options.element,othis.dateType = options.dateType;
        othis.dateType = othis.isEmpty(othis.dateType) ? "datetime":othis.dateType;
        var that = options.element;
        if ($(that).find('input').length>0)return;
        othis.deleteAll(),othis.leaveStat = false;
        var input = $('<input class="layui-input layui-tableEdit-input" type="text">');
        (39 - that.offsetHeight > 3) && input.css('height','30px');
        (that.offsetHeight - 39 > 3) && input.css('height','50px');
        $(that).append(input),input.focus();
        //日期时间选择器 (show: true 表示直接显示)
        laydate.render({elem: input[0],type: othis.dateType,show: true,done:function (value, date) {
            othis.deleteAll();
            if(othis.callback)othis.callback.call(that,value);
        }});
        $('div.layui-laydate').hover(inFunc,outFunc),$(that).hover(inFunc,outFunc);
        _layui.stope();
    };

    //输入框
    Class.prototype.input = function(options){
        var othis = this;
        othis.callback = options.callback,othis.element = options.element;
        othis.oldValue = options.oldValue;
        othis.oldValue = othis.oldValue ? othis.oldValue : '';
        var that = options.element;
        if ($(that).find('input').length>0)return;
        othis.deleteAll(),othis.leaveStat = false;
        var input = $('<input class="layui-input layui-tableEdit-input" style="z-index: 99999999999"  type="text">');
        (39 - that.offsetHeight > 3) && input.css('height','30px');
        (that.offsetHeight - 39 > 3) && input.css('height','50px');
        input.val(othis.oldValue);
        $(that).append(input),input.focus();
        input.click(function (e) {
            _layui.stope(e);
        });
        input.bind('change', function(e){othis.callback.call(othis.element,this.value)});
        $(that).hover(inFunc,outFunc);
        _layui.stope();
    };

    //带加号和减号的输入框(只支持输入数字)
    Class.prototype.signedInput = function(options){
        var othis = this;
        othis.callback = options.callback,othis.element = options.element;
        othis.oldValue = options.oldValue;
        othis.oldValue = othis.oldValue ? othis.oldValue : '';
        var that = options.element;
        if ($(that).find('input').length>0)return;
        othis.deleteAll(),othis.leaveStat = false;
        var thisWidth = that.offsetWidth-49;
        var input = $('<input class="layui-input layui-tableEdit-input" style="left: 24px;width: '+thisWidth+'px" type="text">');//
        var leftBtn = $('<button type="button" class="layui-btn layui-btn-sm layui-tableEdit-sub"><i class="layui-icon layui-icon-subtraction" style="margin-top:-14px!important;position: absolute;left:2px!important"></i></button>');
        var rightBtn = $('<button type="button" class="layui-btn layui-btn-sm layui-tableEdit-add"><i class="layui-icon layui-icon-addition" style="margin-top:-14px!important;position: absolute;right:-1px!important"></i></button>');
        if(39 - that.offsetHeight > 3){
            input.css('height','30px');leftBtn.css('top','16px');rightBtn.css('top','16px');
        }
        if(that.offsetHeight - 39 > 3){
            input.css('height','50px');leftBtn.css('top','25px');rightBtn.css('top','25px');
        }
        input.val(othis.oldValue);
        $(that).append(leftBtn);leftBtn.find('i').html('');
        $(that).append(input),input.focus();$(that).append(rightBtn);rightBtn.find('i').html('');
        input.click(function (e) {
            _layui.stope(e);
        });
        input.bind('change', function(e){othis.callback.call(othis.element,this.value)});
        $(that).hover(inFunc,outFunc);
        _layui.stope();
        $(that).find('button.layui-tableEdit-sub,button.layui-tableEdit-add').bind('click',function () {
            var input = $(that).find('input.layui-tableEdit-input');
            var val = input.val();
            if(!val || val.length<=0)val=0;
            val = parseInt(val);
            if($(this).hasClass('layui-tableEdit-add')){
                ++val;input.val(val);
                othis.callback.call(othis.element,val)
            }else{
                --val;input.val(val);
                othis.callback.call(othis.element,val)
            }
        })
    };

    //判断是否为空函数
    Class.prototype.isEmpty = function(dataStr){return typeof dataStr === 'undefined' || dataStr === null || dataStr.length <= 0;};

    //生成下拉框函数入口
    Class.prototype.register = function(options){
        var othis = this;
        othis.enabled = options.enabled,othis.callback = options.callback;
        othis.data = options.data,othis.element = options.element;
        othis.selectedData = options.selectedData;
        var that = othis.element;
        if($(that).find('input.layui-tableEdit-input')[0]) return;
        othis.deleteAll(),othis.leaveStat = false;
        var input = $('<input class="layui-input layui-tableEdit-input" placeholder="请选择">')
            ,tableEdit = $('<div class="layui-tableEdit"></div>')
            ,tableBody = $(that).parents('div.layui-table-body')
            ,tablePage = $(that).parents('div.layui-table-box').eq(0).next();
        (39 - that.offsetHeight > 3) && input.css('height','30px');
        (that.offsetHeight - 39 > 3) && input.css('height','50px');
        tableEdit.append(input),$(that).append(tableEdit),input.focus();
        var thisY = input[0].getBoundingClientRect().top //输入框y坐标
            ,thisHeight = ((39 - that.offsetHeight > 3) ? 30 : input[0].offsetHeight) //输入框高度
            ,thisHeight = ((that.offsetHeight - 39 > 3) ? 50 :thisHeight)
            ,thisWidth = input[0].offsetWidth //输入框宽度
            ,elemY = that.getBoundingClientRect().top //输入框y坐标
            ,tableBodyY = tableBody[0].getBoundingClientRect().top
            ,pageY = tablePage[0].getBoundingClientRect().top
            ,tableBodyHeight = tableBody.height() //表格高度
            ,isType = thisY-tableBodyY > 0.8*tableBodyHeight
            ,type = isType ? 'top: auto;bottom: '+(thisHeight+2)+'px;' : 'bottom: auto;top: '+(thisHeight+2)+'px;';
        if(elemY<tableBodyY)tableBody[0].scrollTop = that.offsetTop; //调整滚动条位置
        var style = type+'width: '+thisWidth+'px;left: 0px;'+(othis.enabled ? '':'overflow-y: auto;');
        var getClassFn = function(item){
            if(othis.isEmpty(othis.selectedData) || othis.isEmpty(item.name))return "";
            var selectedClass;
            if(typeof othis.selectedData === 'string' || Object.prototype.toString.call(othis.selectedData) === '[object Number]'){
                selectedClass = (item.name+"" === othis.selectedData+"") || (item.value+"" === othis.selectedData+"") ? "layui-tableEdit-selected"+(othis.enabled ? " layui-tableEdit-checked":'') : "";
            }
            if(typeof othis.selectedData === 'object'){
                selectedClass = (item.name+"" === othis.selectedData.name+"") ? "layui-tableEdit-selected"+(othis.enabled ? " layui-tableEdit-checked":'') : "";
            }
            if(Array.isArray(othis.selectedData)){
                for(var i=0;i<othis.selectedData.length;i++){
                    selectedClass = (item.name+"" === othis.selectedData[i].name+"") ? "layui-tableEdit-selected layui-tableEdit-checked" : "";
                    if(!othis.isEmpty(selectedClass)) break;
                }
            }
            return selectedClass;
        };
        tableEdit.append(laytpl(othis.enabled ? selectMoreTpl : selectTpl).render({data: othis.data,style: style,callbackFn:getClassFn}));
        var $tableEdit = $('div.layui-tableEdit-div')[0];
        (thisY+$tableEdit.offsetHeight+thisHeight > pageY) && !isType && (tableBody[0].scrollTop = that.offsetTop);//调整滚动条位置
        othis.events();
    };

    //删除所有下拉框和时间选择框
    Class.prototype.deleteAll = function(){
        $('div.layui-tableEdit-div,div.layui-tableEdit,div.layui-laydate,input.layui-tableEdit-input,button.layui-tableEdit-sub,button.layui-tableEdit-add').remove();
        delete this.leaveStat;//清除（离开状态属性）
    };

    //注册事件
    Class.prototype.events = function(){
        var othis = this;
        var searchFunc = function(val){ //关键字搜索
            $('div.layui-tableEdit-div li').each(function () {
                othis.isEmpty(val) || $(this).data('value').toString().indexOf(val) > -1 ? $(this).show() : $(this).hide();
            });
        },liClickFunc = function(){ //给li元素注册点击事件
            var liArr = $('div.layui-tableEdit-div li');
            liArr.unbind('click'),liArr.bind('click',function (e) {
                _layui.stope(e);
                if(othis.enabled){//多选
                    $(this).hasClass("layui-tableEdit-checked") ? ($(this).removeClass("layui-tableEdit-checked"),
                          $(this).removeClass("layui-tableEdit-selected"))
                        : $(this).addClass("layui-tableEdit-checked")
                }else {//单选
                    othis.deleteAll();
                    if(othis.callback)othis.callback.call(othis.element,{name:$(this).data("name"),value:$(this).data("value")});
                }
            });
        },btnClickFunc = function (){ //给button按钮注册点击事件
            $("div.layui-tableEdit-div button").bind('click',function () {
                var eventType = $(this).attr("event-type"), btn = this,dataList = new Array();
                if(eventType === 'close') singleInstance.deleteAll(); //“关闭”按钮
                if(eventType === 'confirm') { //“确定”按钮
                    $('div.layui-tableEdit-div li').each(function (e) {
                        if(!$(this).hasClass("layui-tableEdit-checked"))return;
                        dataList.push({name:$(this).data("name"),value:$(this).data("value")});
                    });
                    othis.deleteAll();
                    if(othis.callback)othis.callback.call(othis.element,dataList);
                }
            });
        };
        //事件注册
        $(othis.element).find('input.layui-tableEdit-input').bind('input propertychange', function(){searchFunc(this.value)});
        othis.enabled ? (liClickFunc(),btnClickFunc()) : liClickFunc();
        $(othis.element).hover(inFunc,outFunc);
    };

    var AopEvent = function(cols){this.config = {colsConfig:{}};this.parseCols(cols)};//aop构造器
    /**
     * 解析出tableEdit组件所需要的配置信息
     * @param cols
     */
    AopEvent.prototype.parseCols = function(cols){
        var that = this;
        cols.forEach(function (ite) {
            ite.forEach(function (item) {
                if(!item.config)return;
                that.config.colsConfig[item.field] = item.config;
            });
        });
    };
    /**
     * aop代理event
     * @param event 事件名称
     * @param callback 回调函数
     */
    AopEvent.prototype.on = function(event,callback){
        var othis = this;othis.config.event = event,othis.config.callback = callback;
        table.on(othis.config.event,function (obj) {
            var zthis = this,field = $(zthis).data('field'),config = othis.config.colsConfig[field];
            if(!config){
                othis.config.callback.call(zthis,obj);return;
            }
            obj.field = field;
            var callbackFn = function (res) {
                if(config.verify && !othis.verify(res,config.verify,this))return; //验证为空
                obj.value = Array.isArray(res) ? (res.length>0 ? res : [{name:'',value:''}]) : res;
                othis.config.callback.call(zthis,obj);
                if(!singleInstance.isEmpty(config.cascadeSelectField)){
                    var csElement = $(this.parentNode).find("td[data-field='"+config.cascadeSelectField+"']");
                    $(csElement).attr("cascadeSelect-data",JSON.stringify({data:res,field:field}));
                }
            };
            var csd = $(this).attr("cascadeSelect-data");//联动数据
            if(singleInstance.isEmpty(csd)){ //非联动事件
                if(config.type === 'select'){
                    singleInstance.register({data:config.data,element:zthis,enabled:config.enabled,selectedData:obj.data[field],callback:callbackFn});
                }else if(config.type === 'date'){
                    singleInstance.date({dateType:config.dateType,element:zthis,callback:callbackFn});
                }else if(config.type === 'input'){
                    singleInstance.input({element:zthis,oldValue:obj.data[field],callback:callbackFn});
                }else if(config.type === 'signedInput'){
                    singleInstance.signedInput({element:zthis,oldValue:obj.data[field],callback:callbackFn});
                }else othis.config.callback.call(zthis,obj);
            } else {//联动事件
                if(config.type === 'date') return;
                //获取当前单元格的table表格的lay-filter属性值
                var filter = $(zthis).parents('div.layui-table-view').eq(0).prev().attr('lay-filter')
                    ,rs = active.callbackFn.call(zthis,'clickBefore('+filter+')',JSON.parse(csd));
                    //异步操作,由使用者调用。
                    //判断条件为rs为空时。
                    if(singleInstance.isEmpty(rs)){
                        active.on("async("+filter+")",function (result) {
                            singleInstance.register({data:result.data,element:zthis,enabled:result.enabled,selectedData:obj.data[field],callback:callbackFn});
                        })
                    }else {
                        singleInstance.register({data:rs.data,element:zthis,enabled:rs.enabled,selectedData:obj.data[field],callback:callbackFn});
                    }
            }

        });
    };

    /**
     * 验证数据是否符合要求
     * @param data 被验证数据
     * @param verify 正则参数
     * @param td 当前单元格
     * @returns {boolean} true验证通过 false验证未通过
     */
    AopEvent.prototype.verify = function (data,verify,td) {
        var verifyObj = configs.verify[verify.type];
        var verifyMsg = verify.msg;
        verifyMsg = verifyMsg ? verifyMsg : (verifyObj ? verifyObj[1] : '必填项不能为空');
        if(singleInstance.isEmpty(data)){
            layer.tips(verifyMsg, td,{tipsMore:true});
            return false;
        }
        if (Array.isArray(data)) {
            if (data.length <= 0) {
                layer.tips(verifyMsg, td, { tipsMore: true });
                return false;
            }
        }
        else if ((typeof data === 'object' && singleInstance.isEmpty(data.name))
            || data.name === 'undefined'){
            layer.tips(verifyMsg, td,{tipsMore:true});
            return false;
        }
        if(!verifyObj && !verify.regx){
            return true;
        }
        if(verify.regx){ //自定义正则判断
            if(typeof verify.regx === "function"){//为函数时
                if(verify.regx(data))return true;
                layer.tips(verifyMsg, td,{tipsMore:true});
                return false;
            }
            if(typeof verify.regx === "string"){ //为字符串正则时
                var regx = new RegExp(verify.regx);
                if(regx.test(data))return true;
                layer.tips(verifyMsg, td,{tipsMore:true});
                return false;
            }
            if(verify.regx.test(data))return true; //为正则时
            layer.tips(verifyMsg, td,{tipsMore:true});
            return false;
        }
        if(!verifyObj[0].test(data)){
            layer.tips(verifyMsg, td,{tipsMore:true});
            return false;
        }
        return true;
    };

    /**
     * 提交数据的验证
     * @param options {elem:'#test',data:[],verifyKey:'id'}
     * elem: 表格id,带井号  data: 验证数据,数组类型。
     * verifyKey: data中的元素的唯一值字段，且必须在表格中有此字段的单元格。
     * @returns {*}
     */
    AopEvent.prototype.submitValidate = function (options) {
        var that = this,failedTds = [];
        if(!options || singleInstance.isEmpty(options.verifyKey)
            || singleInstance.isEmpty(options.data)
            || singleInstance.isEmpty(options.elem))return failedTds;
        var body = $(options.elem).next().find('div.layui-table-box div.layui-table-body tr');
        options.data.forEach(function (item) {
            for(var field in item){
                var config = that.config.colsConfig[field];
                if(!config || !config.verify)continue;
                var verify = config.verify;
                var tds = body.find('td[data-field="'+options.verifyKey+'"]');
                var thisTd;
                tds.each(function () {
                    var text = $(this).find('div.layui-table-cell').text();
                    if(text+'' === item[options.verifyKey]+''){
                        thisTd = $(this);
                    }
                });
                if(!thisTd)continue;
                var td = thisTd.parent().children('td[data-field="'+field+'"]');
                if(!that.verify(item[field],verify,td))failedTds.push(td[0]);
            }
        });
        return failedTds;
    };

    var active = {
        aopObj:function(cols){return new AopEvent(cols);},
        on:function (event,callback) {
            var filter = event.match(/\((.*)\)$/),eventName = (filter ? (event.replace(filter[0],'')+'_'+ filter[1]) : event);
            configs.callbacks[moduleName+'_'+eventName]=callback;
        },
        callbackFn:function (event,params) {
            var filter = event.match(/\((.*)\)$/),eventName = (filter ? (event.replace(filter[0],'')+'_'+ filter[1]) : event);
            var key = moduleName+'_'+eventName,func = configs.callbacks[key];
            if(!func) return;
            return func.call(this,params);
        }
    };

    active.on("tableEdit(getEntity)",function () {
        return singleInstance;
    });

    exports(moduleName, active);
});