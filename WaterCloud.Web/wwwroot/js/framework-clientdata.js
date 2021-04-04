var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        dataItems: [],
        authorizeButton: [],
        authorizeFields: [],
        moduleFields: [],
    };
    var init = function () {
        $.ajax({
            url: "/ClientsData/GetClientsDataJson?v="+new Date().Format("yyyy-MM-dd hh:mm:ss"),
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) {
                dataJson.dataItems = data.dataItems;
                dataJson.authorizeButton = data.authorizeButton;
                dataJson.authorizeFields = data.authorizeFields;
                dataJson.moduleFields = data.moduleFields;
            }
        });
    }
    init();
    return dataJson;
}