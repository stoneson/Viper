<template>
    <div id="layout1" class="l-layout" style="width: 100%; height: 100%; overflow: hidden; margin-left: 1px ">
        <el-tabs v-model="activeName" type="border-card" style="width:100%">
            <el-tab-pane key="预览" label="预览" name="预览">
                <!--<span slot="label">
                    <span class="span-box">
                        <span>"预览"</span>
                        <el-tooltip class="item"
                                    effect="dark"
                                    content="提示 "
                                    placement="bottom-start">
                            <i class="el-icon-question"></i>
                        </el-tooltip>
                    </span>
                </span>-->
                <el-apiview :apiinfo="ApiInfo" :request="RequestParameters" :requestdata="GetInfoData"></el-apiview>
            </el-tab-pane>
            <el-tab-pane key="编辑" label="编辑" name="编辑">
                <el-apiedit :apiinfo="ApiInfo" :urlinfodata="UriInfoData" :connectiondata="ConnectionData" @refresh="search"></el-apiedit>
            </el-tab-pane>
            <el-tab-pane key="调试" label="调试" name="调试">
                <el-apidebug :apiinfo="ApiInfo" :path="path" :address="address" :request="RequestParameters" :requestdata="GetInfoData"></el-apidebug>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
    module.exports = {
        data: function () {
            return {
                activeName: "预览",
                KeyWord: "",
                ApiInfo: {
                    Name: "GetOneEntity2",
                    HttpMethod: "GET",
                    Route: "GetOneEntity",
                    Title: "测试WebServiceDemo",
                    Remark: "无入参返回实体",
                    MultiExecutor: false,
                    IsCamelCaseNames: false,
                    Stat: 1,
                    ExecutorList: [
                        {
                            Name: "RemoteWS_ffbe4755cacd4bd99e8fb6b6a92e7cc4",
                            ExecType: "RemoteWS",
                            CodeField: "ResponseCode",
                            CodeSucceedValue: "AA",
                            MsgField: "ResponseDesc",
                            DataField: "data",
                            UseExecutorLog: true,
                            Route: "Health",
                            FullClassName: "",
                            CSScriptCode: "",
                            CmdType: "",
                            Sql: "",
                            DLLName: "",
                            Conditions: {
                                ConditionType: "AND"
                            },
                            BaseUri: "BaseUriDemo",
                            Method: "HelloWorld",
                            NextExecutors: [],
                            ApiInputInfoList: [],
                            HasInput: false,
                            OutputList: []
                        }
                    ],
                    HasExecutor: true,
                    KeyWord: "HCenter_DynamicApi_TestApi_GetOneEntity",
                    RoutePrefix: "api2/HCenter",
                    Namespace: "HCenter.DynamicApi",
                    ControllerName: "TestApi"
                },
                UriInfoData: [],
                ConnectionData: [],
                path: "",
                address:"",
                disabled: false,
                RequestParameters: "",
                GetInfoData: [],
            };
        },
        created: function () {
            var that = this;

            //用于数据初始化
            document.title = "接口信息";
            
        },
        mounted: function () {
            var that = this;
            var args = anno.GetUrlParms();
            var protocol = location.protocol;
            var hostname = location.hostname;
            var port = location.port;
            var basePath = protocol + "//" + hostname + ":" + port +"/AnnoApi";
            that.address = basePath;
            //console.log(args);
            that.KeyWord = unescape(args["id"]);
            //console.log("KeyWord",that.KeyWord);
            that.loadData();
            that.getUriInfoData();
            that.getConnectionData();
            
        },
        methods: {
            search(id) {
                this.loadData();
                window.parent.search(id);
            },
            getUriInfoData: function () {
                var that = this;
                GetUriInfo(that, "", function (data) {
                    that.UriInfoData = data;
                });
            },
            getConnectionData: function () {
                var that = this;
                GetConnectionStrings(that, "", function (data) {
                    that.ConnectionData = data;
                });
            },
            loadData: function () {
                var that = this;
                GetApiInfo(that, 1, 20, that.KeyWord, true, "", function (data) {
                    that.GridData = data.Rows;
                    that.total = parseInt(data.Total);
                    if (data.Rows.length > 0) {
                        that.ApiInfo = data.Rows[0];
                        for (var i = 0; i < that.ApiInfo.ExecutorList.length; i++) {
                            if (!that.ApiInfo.ExecutorList[i].hasOwnProperty('OutputList')) {
                                that.ApiInfo.ExecutorList[i].OutputList = [];
                            }
                        }
                        that.path = that.ApiInfo.RoutePrefix + that.ApiInfo.Route;//"/" +
                        that.getSwaggerData();
                        window.parent.postMessage({ Id: that.KeyWord, ControllerName: that.ApiInfo.ControllerName }, '*')
                    }
                });
            },
            getSwaggerData: function () {
                var that = this;
                GetSwageeInfo(that, that.ApiInfo.KeyWord, function (data) {
                    if (data != null) {
                        if (that.ApiInfo.HttpMethod == "GET" && data.Parameters != undefined && data.Parameters != null) {
                            for (var i = 0; i < data.Parameters.length; i++) {
                                var da = { name: data.Parameters[i].name, value: '', description: data.Parameters[i].description, type: data.Parameters[i].type };
                                that.GetInfoData.push(da);
                            }
                        }
                        if (that.ApiInfo.HttpMethod == "POST") {
                            that.RequestParameters = JSON.stringify(data.Properties, null, 4);
                        }
                    }
                })
            }
        },
        components: {//<!--命名全部小写、或者下划线隔开，【中划线或驼峰会报错】  -- >
            'el-apiview': 'url:/component/el-apiview.vue',//httpVueLoader('/component/el-apiview.vue'),
            'el-apiedit': 'url:/component/el-apiedit.vue',//httpVueLoader('/component/el-apiedit.vue'),
            'el-apidebug': 'url:/component/el-apidebug.vue',//httpVueLoader('/component/el-apidebug.vue'),
        }
    };
</script>
<style scoped>
    .icon-close {
        border-radius: 50%;
        text-align: center;
        -webkit-transition: all .3s cubic-bezier(.645,.045,.355,1);
        transition: all .3s cubic-bezier(.645,.045,.355,1);
        margin-left: 5px;
    }
    .el-tabs--border-card {
        background: #FFF;
         border: 0px solid #DCDFE6; 
        -webkit-box-shadow: 0 2px 4px 0 rgb(0 0 0 / 12%), 0 0 6px 0 rgb(0 0 0 / 4%);
        box-shadow: 0 2px 4px 0 rgb(0 0 0 / 12%), 0 0 6px 0 rgb(0 0 0 / 4%);
    }
</style>