"use strict";
//const connection = new signalR.HubConnectionBuilder().withUrl("https://watercloud.vip/chatHub")
const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information).build();

connection.serverTimeoutInMilliseconds = 24e4;
connection.keepAliveIntervalInMilliseconds = 12e4;
connection.start().then(function () {
    //传入token值
    connection.invoke("SendLogin", "").catch(err => console.error("发送失败：" + err.toString()));
    console.log("signalr连接成功");
}).catch(function (ex) {
    console.log("signalr连接失败" + ex);
    //SignalR JavaScript 客户端不会自动重新连接，必须编写代码将手动重新连接你的客户端
    setTimeout(start(), 5000);

});


function start() {
    try {
        connection.start();
        console.log("connected");
    } catch (err) {
        console.log(err);
        setTimeout(start(), 5000);
    }
};
connection.on("ReceiveMessage", function (msg) {
    var data = JSON.parse(msg);
    layui.use(['notice','common'], function () {
        var notice = layui.notice;
        var common = layui.common;
        if (data.F_MessageType == 0)  {
            notice.options = {
                positionClass: "toast-bottom-right",//弹出的位置,
                onclick: function () {
                    common.ajax({
                        url: "/InfoManage/Message/ReadMsgForm",
                        data: { keyValue: data.F_Id },
                        type: 'POST',
                        success: function () {
                            var title = '通知---' + data.F_CreatorUserName,
                                noticeTime = data.F_CreatorTime,
                                content = data.F_MessageInfo;
                            var html = '<div style="padding:15px 20px; text-align:justify; line-height: 22px;border-bottom:1px solid #e2e2e2;background-color: #2f4056;color: #ffffff">\n' +
                                '<div style="text-align: center;margin-bottom: 20px;font-weight: bold;border-bottom:1px solid #718fb5;padding-bottom: 5px"><h4 class="text-danger">' + title + '</h4></div>\n' +
                                '<div style="font-size: 12px">' + content + '</div>\n' +
                                '</div>\n';
                            parent.layer.open({
                                type: 1,
                                title: '通知' + '<span style="float: right;right: 1px;font-size: 12px;color: #b1b3b9;margin-top: 1px">' + noticeTime + '</span>',
                                area: '150px;',
                                shade: 0.8,
                                id: 'layuimini-notice',
                                btn: ['确定'],
                                btnAlign: 'c',
                                moveType: 1,
                                content: html
                            });
                        }
                    });
                }
            };
            notice.success(data.F_MessageInfo);
        }
        else if (data.F_MessageType == 1) {
            notice.options = {
                positionClass: "toast-bottom-right",//弹出的位置,
                onclick: function () {
                    common.ajax({
                        url: "/InfoManage/Message/ReadMsgForm",
                        data: { keyValue: data.F_Id },
                        type: 'POST',
                        success: function () {
                            var title = '私信---' + data.F_CreatorUserName,
                                noticeTime = data.F_CreatorTime,
                                content = data.F_MessageInfo;
                            var html = '<div style="padding:15px 20px; text-align:justify; line-height: 22px;border-bottom:1px solid #e2e2e2;background-color: #2f4056;color: #ffffff">\n' +
                                '<div style="text-align: center;margin-bottom: 20px;font-weight: bold;border-bottom:1px solid #718fb5;padding-bottom: 5px"><h4 class="text-danger">' + title + '</h4></div>\n' +
                                '<div style="font-size: 12px">' + content + '</div>\n' +
                                '</div>\n';
                            parent.layer.open({
                                type: 1,
                                title: '私信' + '<span style="float: right;right: 1px;font-size: 12px;color: #b1b3b9;margin-top: 1px">' + noticeTime + '</span>',
                                area: '150px;',
                                shade: 0.8,
                                id: 'layuimini-notice',
                                btn: ['确定'],
                                btnAlign: 'c',
                                moveType: 1,
                                content: html
                            });
                        }
                    });
                },
            };
            notice.warning(data.F_MessageInfo);
        }
        else {
            notice.options = {
                positionClass: "toast-bottom-right",//弹出的位置,
                onclick: function () {
                    common.ajax({
                        url: "/InfoManage/Message/ReadMsgForm",
                        data: { keyValue: data.F_Id },
                        type: 'POST',
                        success: function () {
                            $("[layuimini-href='" + data.F_Href + "']", ".layuimini-menu-left").click();
                        }
                    });
                }
            };
            notice.error(data.F_MessageInfo);
        }
        $("#noticeMarker").html("<span class='layui-badge-dot'></span>");
    })
});
//下面测试断线重连机制 ，
//重连之前调用 （只有在掉线的一瞬间，只进入一次）
connection.onreconnecting(function(error) {
    console.log("重连中...");
});
//(默认4次重连)，任何一次只要回调成功，调用
connection.onreconnected(function(connectionId){
    console.log("重连成功");
});
//(默认4次重连) 全部都失败后，调用
connection.onclose(function(error){
    console.log('重连失败');
});