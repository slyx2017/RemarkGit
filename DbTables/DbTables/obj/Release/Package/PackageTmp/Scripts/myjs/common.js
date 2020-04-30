function GetQueryString(name) { //取URL地址参数
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}
var formatDate = function (v, format) {
    if (!v) return "";
    var d = v;
    if (typeof v === 'string') {
        if (v.indexOf("/Date(") > -1)
            d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
        else
            d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));//.split(".")[0] 用来处理出现毫秒的情况，截取掉.xxx，否则会出错
    }
    var o = {
        "M+": d.getMonth() + 1,  //month
        "d+": d.getDate(),       //day
        "h+": d.getHours(),      //hour
        "m+": d.getMinutes(),    //minute
        "s+": d.getSeconds(),    //second
        "q+": Math.floor((d.getMonth() + 3) / 3),  //quarter
        "S": d.getMilliseconds() //millisecond
    };
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
};
//form表单赋值
$.fn.SetWebControls = function (data) {
    var $id = $(this)
    for (var key in data) {
        var id = $id.find('#' + key);
        if (id.attr('id')) {
            var type = id.attr('type');
            if (id.attr("input-date")) {
                type = "date";
            }
            var value = $.trim(data[key]).replace(/&nbsp;/g, '');
            switch (type) {
                case "checkbox":
                    if (value == "true") {
                        id.attr("checked", 'checked');
                    } else {
                        id.removeAttr("checked");
                    }
                    break;
                case "date":
                    id.val(formatDate(value, 'yyyy-MM-dd'));
                    break;
                default:
                    id.val(value);
                    break;
            }
        }
    }
}

$.fn.GetWebControls = function (keyValue) {
    var reVal = {};
    $(this).find('input').each(function (r) {
        var id = $(this).attr('id');
        var type = $(this).attr('type');
        if (id) {
            switch (type) {
                case "checkbox":
                    if ($("#" + id).is(":checked")) {
                        reVal[id] = true;
                    } else {
                        reVal[id] = false;
                    }
                    break;
                default:
                    var value = $("#" + id).val();
                    if (value == "") {
                        value = "";
                    }
                    reVal[id] = $.trim(value)
                    break;
            }
        }

    });

    var postdata = reVal
    return postdata;
};

$.fn.CurrentIframe = function () {
    var index = parent.layer.getFrameIndex(window.name);
    var iframeWindow = window['layui-layer-iframe' + index];
    return iframeWindow;
}
var dialogClose = function () {
    var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
    parent.layer.close(index);
    return index;
}
var dialogMsg = function (msg) {
    var layer = window.parent.layer;
    layer.msg(msg);
}
var dialogAlert = function (msg) {
    var layer = window.parent.layer;
    layer.alert(msg);
}