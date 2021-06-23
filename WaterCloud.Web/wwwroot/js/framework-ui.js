$(function () {
    //document.body.className = localStorage.getItem('config-skin');
    //$("[data-toggle='tooltip']").tooltip();
})
//获取参数
$.request = function (name) {
    var search = location.search.slice(1);
    var arr = search.split("&");
    for (var i = 0; i < arr.length; i++) {
        var ar = arr[i].split("=");
        if (ar[0] == name) {
            if (unescape(ar[1]) == 'undefined') {
                return "";
            } else {
                return unescape(ar[1]);
            }
        }
    }
    return "";
}
//json拼接
$.jsonWhere = function (data, action) {
    if (action == null) return;
    var reval = new Array();
    $(data).each(function (i, v) {
        if (action(v)) {
            reval.push(v);
        }
    })
    return reval;
}
//select绑定
$.fn.bindSelect = function (options) {
    var defaults = {
        id: "id",
        text: "text",
        search: false,
        url: "",
        param: [],
        change: null,
        data: null,
        checked: 0,
    };
    var options = $.extend(defaults, options);
    var $element = $(this);
    if (options.data != null) {
        $.each(options.data, function (i) {
            if (options.id == "") {
                var temp = $("<option></option>").val(i).html(options.data[i]);
                //字典
                if (i == options.checked) {
                    temp.prop("selected", true);
                }
                $element.append(temp);
            }
            else {
                var temp = $("<option></option>").val(options.data[i][options.id]).html(options.data[i][options.text]);
                //list
                if (options.data[i][options.id] == options.checked) {
                    temp.prop("selected", true);
                }
                $element.append(temp);
            }
        });
        $element.on("change", function (e) {
            if (options.change != null) {
                options.change(options.data[$(this).find("option:selected").index()]);
            }
        });
    }
    else if (options.url != "") {
        //ie缓存问题
        if (options.url.indexOf("?") >= 0) {
            options.url = options.url + '&v=' + new Date().Format("yyyy-MM-dd hh:mm:ss");
        }
        else {
            options.url = options.url + '?v=' + new Date().Format("yyyy-MM-dd hh:mm:ss");
        }
        $.ajax({
            url: options.url,
            data: options.param,
            dataType: "json",
            async: false,
            success: function (data) {
                $.each(data, function (i) {
                    if (options.id == "") {
                        var temp = $("<option></option>").val(data[i]).html(data[i]);
                        //字典
                        if (i == options.checked) {
                            temp.prop("selected", true);
                        }
                        $element.append(temp);
                    }
                    else {
                        var temp = $("<option></option>").val(data[i][options.id]).html(data[i][options.text]);
                        //list
                        if (data[i][options.id] == options.checked) {
                            temp.prop("selected", true);
                        }
                        $element.append(temp);
                    }
                    $element.on("change", function (e) {
                        if (options.change != null) {
                            options.change(options.data[$(this).find("option:selected").index()]);
                        }
                    });
                })
            }
        });
    }
}
//radio绑定
$.fn.bindRadio = function (options) {
    var defaults = {
        id: "id",
        text: "text",
        search: false,
        url: "",
        param: [],
        change: null,
        data: null,
        checked: 0,
        filterName: ""
    };
    var options = $.extend(defaults, options);
    var $element = $(this);
    if (options.data != null) {
        $.each(options.data, function (i) {
            if (!options.id) {
                var temp = $("<input>").val(i).attr("title", options.data[i]).attr("type", "radio").attr("name", $element.attr("id"));
                //字典
                if (!options.filterName) {
                    temp.attr("lay-filter", $element.attr("id"));
                }
                else {
                    temp.attr("lay-filter", filterName);
                }
                if (i == options.checked) {
                    temp.prop("checked", "true");
                }

                $element.append(temp);
            }
            else {
                var temp = $("<input>").val(options.data[i][options.id]).attr("title", options.data[i][options.text]).attr("type", "radio").attr("name", options.id);
                //list
                if (!options.filterName) {
                    temp.attr("lay-filter", $element.attr("id"));
                }
                else {
                    temp.attr("lay-filter", filterName);
                }
                if (options.data[i][options.id] == options.checked) {
                    temp.prop("checked", "true");
                }
                $element.append();
            }
        });
    }
    else if (options.url != "") {
        //ie缓存问题
        if (options.url.indexOf("?") >= 0) {
            options.url = options.url + '&v=' + new Date().Format("yyyy-MM-dd hh:mm:ss");
        }
        else {
            options.url = options.url + '?v=' + new Date().Format("yyyy-MM-dd hh:mm:ss");
        }
        $.ajax({
            url: options.url,
            data: options.param,
            dataType: "json",
            async: false,
            success: function (data) {
                $.each(data, function (i) {
                    if (!options.id) {
                        var temp = $("<input>").val(data[i]).attr("title", data[i]).attr("type", "radio").attr("name", $element.attr("id"));
                        //字典
                        if (!options.filterName) {
                            temp.attr("lay-filter", $element.attr("id"));
                        }
                        else {
                            temp.attr("lay-filter", filterName);
                        }
                        if (i == options.checked) {
                            temp.prop("checked", "true");
                        }
                        $element.append(temp);
                    }
                    else {
                        var temp = $("<input>").val(data[i][options.id]).attr("title", data[i][options.text]).attr("type", "radio").attr("name", $element.attr("id"));
                        //list
                        if (!options.filterName) {
                            temp.attr("lay-filter", $element.attr("id"));
                        }
                        else {
                            temp.attr("lay-filter", filterName);
                        }
                        if (data[i][options.id] == options.checked) {
                            temp.prop("checked", "true");
                        }
                        $element.append();
                    }
                });
            }
        });
    }
}
// 时间格式方法
Date.prototype.Format = function (fmt) {
    var o = {

        "M+": this.getMonth() + 1, //月份 

        "d+": this.getDate(), //日 

        "h+": this.getHours(), //小时 

        "m+": this.getMinutes(), //分 

        "s+": this.getSeconds(), //秒 

        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 

        "S": this.getMilliseconds() //毫秒 

    };

    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));

    for (var k in o)

        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
Date.prototype.formatDate = function () { //author: meizz   
    var myyear = this.getFullYear();
    var mymonth = this.getMonth() + 1;
    var myweekday = this.getDate();
    var myhour = this.getHours();
    var myminute = this.getMinutes();
    var second = this.getSeconds();
    if (mymonth < 10) {
        mymonth = "0" + mymonth;
    }
    if (myweekday < 10) {
        myweekday = "0" + myweekday;
    }
    if (myhour < 10) {
        myhour = "0" + myhour;
    }
    if (myminute < 10) {
        myminute = "0" + myminute;
    }
    if (second < 10) {
        second = "0" + second;
    }
    return (myyear.toString() + mymonth.toString() + myweekday.toString() + myhour.toString() + myminute.toString() + second.toString());
};
//集合id
function uuid() {
    var s = [];
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < 36; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    s[14] = "4"; // bits 12-15 of the time_hi_and_version field to 0010
    s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1); // bits 6-7 of the clock_seq_hi_and_reserved to 01
    s[8] = s[13] = s[18] = s[23] = "-";

    var uuid = s.join("");
    return uuid;
}