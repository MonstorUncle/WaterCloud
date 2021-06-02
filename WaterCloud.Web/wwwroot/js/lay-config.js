/**
 * date:2019/08/16
 * author:Mr.Chung
 * description:此处放layui自定义扩展
 */
var version;
window.rootPath = (function (src) {
    src = document.scripts[document.scripts.length - 1].src;
    //获取版本号
    version = src.substring(src.lastIndexOf("?v=") + 3, src.length);
    return src.substring(0, src.lastIndexOf("/") + 1);
})();

layui.config({
    base: rootPath + "lay-module/",
    version: version
}).extend({
    miniAdmin: "layuimini/miniAdmin", // layuimini后台扩展
    miniMenu: "layuimini/miniMenu", // layuimini菜单扩展
    miniTab: "layuimini/miniTab", // layuimini tab扩展
    miniTheme: "layuimini/miniTheme", // layuimini 主题扩展
    miniTongji: "layuimini/miniTongji", // layuimini 统计扩展
    step: 'step-lay/step', // 分步表单扩展
    treeTable: 'treetable-lay/treeTable', //table树形扩展3.x
    tableSelect: 'tableSelect/tableSelect', // table选择扩展
    tableEdit: 'tableSelect/tableEdit', // table编辑扩展
    iconPickerFa: 'iconPicker/iconPickerFa', // fa图标选择扩展
    echarts: 'echarts/echarts', // echarts图表扩展
    wangEditor: 'wangEditor/wangEditor', // wangEditor富文本扩展
    layarea: 'layarea/layarea', //  省市县区三级联动下拉选择器
    common: 'waterCloud/common', //  工具类
    commonTable: 'waterCloud/commonTable', //  工具类
    optimizeSelectOption: 'optimizeSelectOption/optimizeSelectOption', //  下拉框遮挡
    dtree: 'dtree/dtree', //  树形扩展
    xmSelect:'xm-select/xm-select', //select多选扩展
    flowlayout:'flow/flowlayout', //flow流程插件
    waterflow: 'flow/waterflow' ,//flow流程插件
    formDesigner: 'formDesigner/formDesigner', //表单设计器
    formField: 'formDesigner/formField', //表单设计器
    formPreview: 'formDesigner/formPreview', //表单设计器
    notice: 'notice/notice', //消息提醒
    soulTable: 'soulTable/soulTable',//表格插件
    soulTableSlim: 'soulTable/soulTable.slim',//表格插件
    tableChild: 'soulTable/tableChild',//子表
    tableMerge: 'soulTable/tableMerge',//合并表格
    tableFilter: 'soulTable/tableFilter',//表格过滤
    excel: 'soulTable/excel',//表格导出
    cardTable: 'cardTable/cardTable',//卡片表格
    cron: 'cron/cron',//cron表达式
    tabletree: 'tabletree/tabletree',//基于layui的树形扩展
    mobileTable: 'mobileTable/mobileTable',
});