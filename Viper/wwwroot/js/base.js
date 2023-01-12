var anno = {
    input: {
        profile: localStorage.token,
        uname: localStorage.account
    },
    ajaxpara: {
        async: true,
        dataType: "json",
        type: 'post',
        src: window.location.origin === undefined
            ? window.location.protocol + "//" + window.location.host + "/AnnoApi/"
            : window.location.origin + "/AnnoApi/" //兼容老版本IE origin
    },
    process: function (input, url, callback, errorCallBack) {
        window.$.ajax({
            url: anno.ajaxpara.src + url + "?t=" + new Date().getMilliseconds(),
            type: anno.ajaxpara.type,
            async: anno.ajaxpara.async,
            dataType: anno.ajaxpara.dataType,
            data: input,
            success: function (data, status) {
                if (status === "success" && data.status && (data.msg === null || data.msg === "")) {
                    callback(data, status);
                } else if (errorCallBack !== null && errorCallBack !== undefined) {
                    errorCallBack(data, status);
                } else {
                    callback(data, status);
                }
            }
        }
        );
    },
    processGet: function (input, url, callback, errorCallBack) {
        window.$.ajax({
            url: anno.ajaxpara.src + url + "?t=" + new Date().getMilliseconds(),
            type: 'GET',//anno.ajaxpara.type,
            async: anno.ajaxpara.async,
            dataType: anno.ajaxpara.dataType,
            data: input,
            success: function (data, status) {
                if (status === "success" && data.status && (data.msg === null || data.msg === "")) {
                    callback(data, status);
                } else if (errorCallBack !== null && errorCallBack !== undefined) {
                    errorCallBack(data, status);
                } else {
                    callback(data, status);
                }
            }
        }
        );
    },
    processAjaxPost: function (input, baseurl, url, callback, errorCallBack) {
        window.$.ajax({
            url: baseurl + url + "?t=" + new Date().getMilliseconds(),
            type: 'POST',
            async: anno.ajaxpara.async,
            dataType: anno.ajaxpara.dataType,
            //contentType: 'application/json;charset=utf-8',//向后台传送格式
            data: input,
            timeout: 5000,
            success: function (data, status) {
                if (status === "success" && data.status && (data.msg === null || data.msg === "")) {
                    callback(data, status);
                } else if (errorCallBack !== null && errorCallBack !== undefined) {
                    errorCallBack(data, status);
                } else {
                    callback(data, status);
                }
            },
            error: function (data, status) {
                if (errorCallBack !== null && errorCallBack !== undefined) {
                    errorCallBack(status);
                }
            }
        }
        );
    },
    processAjaxGET: function (input, baseurl, url, callback, errorCallBack) {
        window.$.ajax({
            url: baseurl + url,//+ "?t=" + new Date().getMilliseconds(),
            type: 'GET',//anno.ajaxpara.type,
            async: true,
            dataType: anno.ajaxpara.dataType,
            data: input,
            timeout: 4000,
            success: function (data, status) {
                if (status === "success" && data.status && (data.msg === null || data.msg === "")) {
                    callback(data, status);
                } else if (errorCallBack !== null && errorCallBack !== undefined) {
                    errorCallBack(data, status);
                } else {
                    callback(data, status);
                }
            },
            error: function (data, status) {
                if (errorCallBack !== null && errorCallBack !== undefined) {
                    errorCallBack(status);
                }
            }
        }
        );
    },
    watch: function (obj, attr, callback) {
        if (typeof obj.defaultValues === 'undefined') {
            obj.defaultValues = {};
            for (var p in obj) {
                if (typeof obj[p] !== 'object')
                    obj.defaultValues[p] = obj[p];
            }
        }
        if (typeof obj.setAttr === 'undefined') {
            obj.setAttr = function (attr, value) {
                if (this[attr] !== value) {
                    this.defaultValues[attr] = this[attr];
                    this[attr] = value;
                    return callback(this);
                }
                return this;
            };
        }
    },
    observe: function (dom, config, callback) {
        var MutationObserver =
            window.MutationObserver || window.WebKitMutationObserver || window.MozMutationObserver; //浏览器兼容
        var _config = { attributes: true, childList: true }; //配置对象
        if (config !== null) {
            _config = config;
        }
        var observer = new MutationObserver(function (mutations) { //构造函数回调
            mutations.forEach(function (record) {
                callback(record);
            });
        });
        dom.each(function () {
            observer.observe(this, _config);
        });
    },
    inherit: function (p) { //对象继承
        if (p === null) throw TypeError();
        if (Object.create)
            return Object.create(p);
        var t = typeof p;
        if (t !== "object" && t !== "function") throw TypeError();

        function F() { }

        F.prototype = p;
        return new F();
    },
    cloneObj: function (obj) { //对象克隆
        var str, newobj = obj.constructor === Array ? [] : {};
        if (typeof obj !== 'object') {
            return newobj;
        } else if (window.JSON) {
            str = JSON.stringify(obj), //序列化对象
                newobj = JSON.parse(str); //还原
        } else {
            for (var i in obj) {
                newobj[i] = typeof obj[i] === 'object' ? cloneObj(obj[i]) : obj[i];
            }
        }
        return newobj;
    },
    getInput: function () {
        return this.cloneObj(this.input);
    },
    GetUrlParms: function () {
        var args = new Object();
        var query = location.search.substring(1); //获取查询串
        var pairs = query.split("&"); //在逗号处断开
        for (var i = 0; i < pairs.length; i++) {
            var pos = pairs[i].indexOf('='); //查找name=value
            if (pos === -1) continue; //如果没有找到就跳过
            var argname = pairs[i].substring(0, pos); //提取name
            var value = pairs[i].substring(pos + 1); //提取value
            args[argname] = unescape(value); //存为属性
        }
        return args;
    }
};

function LoadScriptToHead(src,callback) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.onload = script.onreadystatechange = function () {
        if (!this.readyState || this.readyState === "loaded" || this.readyState === "complete") {
          if(callback!=undefined&&callback!=null&&(typeof callback)==="function"){
              callback();
          }
        script.onload = script.onreadystatechange = null;
        }
    };
    script.src = src;
    head.appendChild(script);
}

function LoadStyleToHead(src,callback) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('link');
    script.rel = 'stylesheet';
    script.onload = script.onreadystatechange = function () {
        if (!this.readyState || this.readyState === "loaded" || this.readyState === "complete") {
          if(callback!=undefined&&callback!=null&&(typeof callback)==="function"){
              callback();
          }
        script.onload = script.onreadystatechange = null;
        }
    };
    script.href = src;
    head.appendChild(script);
}
/**----------------------------------------------------------------------------------------------- */

function getGuid32() {
    var rt_str = String.fromCharCode(65 + Math.floor(Math.random() * 26));
    for (i = 0; i < 31; ++i) {
        var num = Math.floor(Math.random() * (26 + 26 + 10));
        var ch_str;
        if (num < 10) {
            ch_str = num.toString();
        }
        else if (num < 10 + 26) {
            ch_str = String.fromCharCode(65 + num - 10);
        }
        else {
            ch_str = String.fromCharCode(97 + num - 10 - 26);
        }
        rt_str += ch_str;
    }
    return rt_str;
}

function strCode(str, k) {
    var count = 0;
    var zifu = "";
    if (str) {
        len = str.length;
        for (var i = 0; i < len; i++) {  //遍历字符串，枚举每个字符
            if (str.charCodeAt(i) > 255) {  //字符编码大于255，说明是双字节字符(即是中文)
                count += 2;  //则累加2个
            } else {
                count++;  //否则递加一次
            }
            zifu += str[i];
            if (count > k) {
                return zifu + "...";
            }
        }
        return zifu;
    } else {
        return zifu;
    }
}

/**
* 初始化信息
 * InitsyncInterface
*/
function InitsyncInterface(that, title, keyword, name, issync, info) {
    that.urlsData.show = true;
    that.urlsData.title = title;
    that.urlsData.KeyWord = keyword;
    that.urlsData.Name = name;
    that.urlsData.issync = issync;
    that.urlsData.Info = info;
}
/**
* 数据回调
*/
function dataCallback(that, data, callback) {
    if (data.status === true) {
        callback(data.outputData);
    } else {
        that.$message.error(data.msg);
    }
}
/**
* 获取数据库连接配置
 * Anno.DynamicApi/DynamicApi/GetConnectionStrings
*/
function GetConnectionStrings(that, ConnectionName, callback) {
    var input = anno.getInput();
    input.Name = ConnectionName;
    anno.process(input, "Anno.DynamicApi/DynamicApi/GetConnectionStrings", function (data) {
        dataCallback(that, data, callback);
    });
}

/**
* 获取第三方地址
 * Anno.DynamicApi/DynamicApi/GetUriInfo
*/
function GetUriInfo(that, urlName, callback) {
    var input = anno.getInput();
    input.Name = urlName;
    anno.process(input, "Anno.DynamicApi/DynamicApi/GetUriInfo", function (data) {
        dataCallback(that, data, callback);
    });
}

/**
* 获取环境地址
 * Anno.DynamicApi/DynamicApi/GetSetingUriInfo
*/
function GetSetingUriInfo(that, urlName, callback) {
    var input = anno.getInput();
    input.Name = urlName;
    anno.process(input, "Anno.DynamicApi/DynamicApi/GetSetingUriInfo", function (data) {
        dataCallback(that, data, callback);
    });
}
/**
* 删除环境地址
 * Anno.DynamicApi/DynamicApi/DelSetingUriInfo
*/
function DelSetingUriInfo(that, Name, callback) {
    var input = anno.getInput();
    input.Name = Name;
    anno.process(input, "Anno.DynamicApi/DynamicApi/DelSetingUriInfo", function (data) {
        if (data.status === true) {
            that.$message({ message: '删除成功', type: 'success' });
            callback(true);
        } else {
            that.$message.error(data.msg);
        }
    });
}
/**
* 保存环境地址
 * Anno.DynamicApi/DynamicApi/SaveSetingUriInfo
*/
function SaveSetingUriInfo(that, Name, BaseUri, Remark, callback) {
    var input = anno.getInput();
    input.Name = Name;
    input.BaseUri = BaseUri;
    input.Remark = Remark;
    anno.process(input, "Anno.DynamicApi/DynamicApi/SaveSetingUriInfo", function (data) {
        if (data.status === true) {
            that.$message({ message: '更新成功', type: 'success' });
            callback(true);
        } else {
            that.$message.error(data.msg);
        }
    });
}

/**
* 获取获取控制器菜单
 * Anno.DynamicApi/DynamicApi/GetControllerMenus
*/
function GetControllerMenus(that, callback) {
    var input = anno.getInput();
    anno.ajaxpara.async = false;
    anno.process(input, "Anno.DynamicApi/DynamicApi/GetControllerMenus", function (data) {
        dataCallback(that, data, callback);
    });
    anno.ajaxpara.async = true;
}

/**
* 获取控制器（项目）
 * Anno.DynamicApi/DynamicApi/GetController
*/
function GetControllers(that, Name, issync, BaseUri, callback) {
    var input = anno.getInput();
    input.Name = Name;
    anno.ajaxpara.async = false;
    BaseUri = issync ? anno.ajaxpara.src : BaseUri + "/AnnoApi/";
    anno.processAjaxPost(input, BaseUri, "Anno.DynamicApi/DynamicApi/GetControllers", function (data) {
        callback(data.outputData);
    }, function (error) {
        that.$message.error("获取分类信息失败，请稍后重试！");
    });
    anno.ajaxpara.async = true;
}

/**
* 删除控制器（项目）
 * Anno.DynamicApi/DynamicApi/DelController
*/
function DelController(that, Name, callback) {
    var input = anno.getInput();
    input.Name = Name;
    anno.process(input, "Anno.DynamicApi/DynamicApi/DelController", function (data) {
        if (data.status === true) {
            that.$message({ message: '删除成功', type: 'success' });
            callback(true);
        } else {
            that.$message.error(data.msg);
        }
    });
}

/**
* 保存控制器（项目）
 * Anno.DynamicApi/DynamicApi/SaveController
*/
function SaveController(that, menuroot, issync, BaseUri, info, callback) {
    var input = anno.getInput();
    var menudata = menuroot;
    menudata.uname = input.uname;
    menudata.profile = input.profile;
    anno.ajaxpara.async = false;
    BaseUri = issync ? anno.ajaxpara.src : BaseUri + "/AnnoApi/";
    anno.processAjaxPost(menudata, BaseUri, "Anno.DynamicApi/DynamicApi/SaveController", function (data) {
        if (data.Data.indexOf("成功") > -1) {
            that.$message({ message: info + '分类成功', type: 'success' });
            callback(data.outputData);
        } else {
            that.$message.error(info + "分类失败：" + data.msg);
        }
    }, function (error) {
        that.$message.error(info + "分类(" + menudata.Title + ")失败，请稍后重试！");
    });
    anno.ajaxpara.async = true;
}


/**
* 获取接口集合
 * Anno.DynamicApi/DynamicApi/GetApiInfo
*/
function GetApiInfo(that, page, pagesize, KeyWord, issync, BaseUri, callback) {
    var input = anno.getInput();
    var args = anno.GetUrlParms();
    input.ControllerName = args["name"];
    anno.ajaxpara.async = false;
    if (page !== null && page !== undefined) {
        input.page = page;
    } else {
        input.page = 1;
    }
    if (pagesize !== null && pagesize !== undefined) {
        input.pagesize = pagesize;
    } else {
        input.pagesize = 20;
    }
    input.KeyWord = KeyWord;
    BaseUri = issync ? anno.ajaxpara.src : BaseUri + "/AnnoApi/";
    anno.processAjaxPost(input, BaseUri, "Anno.DynamicApi/DynamicApi/GetApiInfo", function (data) {
        dataCallback(that, data, callback);
    }, function (error) {
        that.$message.error("获取接口信息失败，请稍后重试！");
    });
    anno.ajaxpara.async = true;
}

/**
*删除接口
 * Anno.DynamicApi/DynamicApi/DelApiInfo
*/
function DelApiInfo(that, ControllerName, Name, callback) {
    var input = anno.getInput();
    input.ControllerName = ControllerName;
    input.ApiName = Name;
    anno.process(input, "Anno.DynamicApi/DynamicApi/DelApiInfo", function (data) {
        if (data.status === true) {
            that.$message({ message: '删除成功', type: 'success' });
            callback(true);
        } else {
            that.$message.error(data.msg);
        }
    });
}

/**
* 保存接口
 * Anno.DynamicApi/DynamicApi/SaveApiInfo
*/
function SaveApiInfo(that, InputInfo, issync, BaseUri, info, callback) {
    var input = anno.getInput();
    var InputData = InputInfo;
    InputData.uname = input.uname;
    InputData.profile = input.profile;
    BaseUri = issync ? anno.ajaxpara.src : BaseUri + "/AnnoApi/";
    anno.processAjaxPost(InputData, BaseUri, "Anno.DynamicApi/DynamicApi/SaveApiInfo", function (data) {
        if (data.status === true) {
            that.$message({ message: "接口(" + InputData.Name + ")" + info + "成功!", type: 'success' });
            callback(data.outputData);
        } else {
            that.$message.error(data.msg);
        }
    }, function (error) {
        that.$message.error(info + "接口(" + InputData.Name + ")失败，请稍后重试！");
    });
}


/**
* 获取接口请求数据
 * Anno.DynamicApi/DynamicApi/GetSwageeInfo
*/
function GetSwageeInfo(that, path, callback) {
    var input = anno.getInput();
    input.api = path;
    anno.process(input, "Anno.DynamicApi/DynamicApi/GetSwageeInfo", function (data) {
        //callback(data);
        dataCallback(that, data, callback);
    });
}

/**
* 调用接口
 * Anno.DynamicApi/DynamicApi/HttpRequest
*/
function HttpRequest(that, HttpMethod, Headers, requestdata, request, address, path, callback) {
    var input = anno.getInput();
    input.HttpMethod = HttpMethod;
    input.Headers = JSON.stringify(Headers);
    if (HttpMethod == "GET") {
        input.data = JSON.stringify(requestdata);
    }
    if (HttpMethod == "POST") {
        input.data = request;
    }
    input.url = address + path;
    anno.process(input, "Anno.DynamicApi/DynamicApi/HttpRequest", function (data) {
        //callback(data);
        if (data.status === true) {
            callback(data.outputData);
        } else {
            that.$message.error(data.msg);
        }
    }, function (data, status) {
        that.loading = false;
        callback(status);
        //that.$message.error(data.msg);
    });
}