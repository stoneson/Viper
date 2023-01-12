<template>
    <div id="layout11" class="l-layout" style="width: 100%; height: 100%; overflow: hidden;">
        <div style="padding-left:95%"><el-button type="danger" @click="Save()">保存</el-button></div>
        <el-form :model="apiinfo" style="width: 100%; min-width: 250px; margin-top: 10px; ">
            <!--<el-divider content-position="left"><h3>基本信息</h3></el-divider>-->
            <el-collapse value="基本信息" accordion>
                <el-collapse-item name="基本信息">
                    <h3 slot="title">基本信息</h3>

                    <div class="panel-view">
                        <el-row>
                            <el-col :span="12">
                                <el-form-item label="接口名称：" label-width="120px" required="true">
                                    <el-input v-model="apiinfo.Name" autocomplete="on"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :span="12">
                                <el-form-item label="接口标题：" label-width="120px">
                                    <el-input v-model="apiinfo.Title" autocomplete="on"></el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-row>
                            <el-col :span="24">
                                <el-form-item label="接口路径：" label-width="120px" required="true">
                                    <el-container>
                                        <el-select style="width:120px;" v-model="apiinfo.HttpMethod" placeholder="请选择">
                                            <el-option v-for="item in HttpMethods"
                                                       :key="item.value"
                                                       :label="item.Name"
                                                       :value="item.value"
                                                       :disabled="item.disabled">
                                            </el-option>
                                        </el-select>
                                        <span style="margin-left:5px;margin-right:5px;">{{apiinfo.RoutePrefix}}</span>
                                        <el-input v-model="apiinfo.Route" autocomplete="on"></el-input>
                                    </el-container>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-row>
                            <el-col :span="8">
                                <el-form-item label="接口状态：" label-width="120px">
                                    <el-select style="width:100px;" v-model="apiinfo.Stat" placeholder="请选择">
                                        <el-option v-for="item in Stats"
                                                   :key="item.value"
                                                   :label="item.Name"
                                                   :value="item.value"
                                                   :disabled="item.disabled">
                                        </el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :span="8">
                                <el-form-item label-width="120px">
                                    <span slot="label">
                                        多个执行者
                                        <el-tooltip content="是否支持多执行者同时执行" placement="top">
                                            <i class="el-icon-question"></i>
                                        </el-tooltip>：
                                    </span>
                                    <el-switch v-model="apiinfo.MultiExecutor">
                                    </el-switch>
                                </el-form-item>
                            </el-col>
                            <el-col :span="8">
                                <el-form-item label-width="120px">
                                    <span slot="label">
                                        驼峰结果
                                        <el-tooltip content="返回结果是否为驼峰命名" placement="top">
                                            <i class="el-icon-question"></i>
                                        </el-tooltip>：
                                    </span>
                                    <el-switch v-model="apiinfo.IsCamelCaseNames">
                                    </el-switch>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-row>
                            <el-col :span="24">
                                <el-form-item label="备注：" label-width="120px">
                                    <el-input type="textarea" v-model="apiinfo.Remark"></el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </div>
                </el-collapse-item>
                <el-collapse-item name="执行者设置">
                    <template slot="title">
                        <el-tooltip content="添加执行者" placement="top">
                            <i @click.stop.prevent="e => {e.stopPropagation();AddExecutorList()}" class="el-icon-plus" style="width: 24px; cursor: pointer;"></i>
                        </el-tooltip>
                        <h3>  执行者设置</h3>
                    </template>
                    
                    <el-collapse value="executor0" accordion>
                        <!--<div v-for="item,index in apiinfo.ExecutorList">-->
                        <el-collapse-item v-for="item,index in apiinfo.ExecutorList">
                            <span slot="name">executor{{index}}</span>
                            <template slot="title">
                                <el-tooltip content="删除当前执行者" placement="top">
                                    <i @click.stop.prevent="e => {e.stopPropagation();DelExecutorList(item)}" class="el-icon-delete" style="width: 24px; cursor: pointer;"></i>
                                </el-tooltip>
                                <h4>  执行者【{{index+1}}】</h4>
                            </template>
                            <el-api-executor :executorinfo="item" :urlinfodata="urlinfodata" :connectiondata="connectiondata" :disabled="disabled"></el-api-executor>
                        </el-collapse-item>
                        <!--</div>-->
                    </el-collapse>
                </el-collapse-item>
                </el-collapse>
            </el-form>
        </div>
    </template>

    <script>
        module.exports = {
            props: {
                urlinfodata: {
                    type: Array,
                    default: function () {
                        return []
                    }
                },
                connectiondata: {
                    type: Array,
                    default: function () {
                        return []
                    }
                },
                apiinfo: {
                    type: Object,
                    default: function () {
                        return {
                            Name: "GetOneEntity",
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
                                    FullClassName: "",
                                    CSScriptCode: "",
                                    CmdType: "",
                                    Sql: "",
                                    DLLName: "",
                                    Conditions: {
                                        ConditionType: "AND"
                                    },
                                    BaseUri: "BaseUriDemo",
                                    Route: "Health",
                                    Method: "HelloWorld",
                                    NextExecutors: [],
                                    ApiInputInfoList: [{
                                        Name: "count",
                                        SqlName: "count",
                                        TypeName: "int",
                                        DefaultValue: "",
                                        AllowNullable: true,
                                        Title: "count",
                                        Remark: "count",
                                        SqlTypeName: "int",
                                        ValuePath: "count",
                                        SubFieldInfoList: [],
                                        HasSub: false
                                    }],
                                    HasInput: false,
                                    OutputList: []
                                }
                            ],
                            HasExecutor: true,
                            KeyWord: "HCenter_DynamicApi_TestApi_GetOneEntity",
                            RoutePrefix: "api2/HCenter",
                            Namespace: "HCenter.DynamicApi",
                            ControllerName: "TestApi"
                        }
                    }
                },
            },
            data: function () {
                return {
                    HttpMethods: [
                        { Name: 'GET', value: 'GET' },
                        { Name: 'POST', value: 'POST' },
                        //{ Name: 'PUT', value: 'PUT' },
                        //{ Name: 'DELETE', value: 'DELETE' },
                    ],
                    Stats: [
                        { Name: '开启', value: 1 },
                        { Name: '停用', value: 0 },
                    ],
                    InputData: {
                        Name: "GetOneEntity",
                        HttpMethod: "GET",
                        Route: "GetOneEntity",
                        Title: "测试WebServiceDemo",
                        Remark: "无入参返回实体",
                        MultiExecutor: false,
                        IsCamelCaseNames: false,
                        Stat: 1,
                        ExecutorList: "",
                        HasExecutor: true,
                        KeyWord: "HCenter_DynamicApi_TestApi_GetOneEntity",
                        RoutePrefix: "api2/HCenter",
                        Namespace: "HCenter.DynamicApi",
                        ControllerName: "TestApi",
                        uname: "",
                        profile: ""
                    },
                    disabled: false
                };
            },
            created: function () {
                var that = this;

                //用于数据初始化
                document.title = "接口信息";
                //console.log(that.apiinfo);
            },
            mounted: function () {

            },
            methods: {
                
                Save: function () {
                    //console.log(JSON.stringify(this.apiinfo));
                    //alert(JSON.stringify(this.apiinfo));
                    var that = this;
                    var input = anno.getInput();

                    that.InputData.Name = that.apiinfo.Name;
                    that.InputData.HttpMethod = that.apiinfo.HttpMethod;
                    that.InputData.Route = that.apiinfo.Route;
                    that.InputData.Title = that.apiinfo.Title;
                    that.InputData.Remark = that.apiinfo.Remark;
                    that.InputData.MultiExecutor = that.apiinfo.MultiExecutor;
                    that.InputData.IsCamelCaseNames = that.apiinfo.IsCamelCaseNames;
                    that.InputData.Stat = that.apiinfo.Stat;
                    that.InputData.ExecutorList = JSON.stringify(that.apiinfo.ExecutorList);
                    that.InputData.HasExecutor = that.apiinfo.HasExecutor;
                    that.InputData.KeyWord = that.apiinfo.KeyWord;
                    that.InputData.RoutePrefix = that.apiinfo.RoutePrefix;
                    that.InputData.Namespace = that.apiinfo.Namespace;
                    that.InputData.ControllerName = that.apiinfo.ControllerName;
                   
                    SaveApiInfo(that, that.InputData, true, "","保存",function (data) {
                        that.$emit("refresh", data.split('|')[1]);
                    });
                },
                AddExecutorList: function () {
                    let newObject = JSON.parse(JSON.stringify(this.apiinfo.ExecutorList[0]));
                    newObject.ExecType = "SqlTpl";
                    newObject.CmdType = "P";
                    newObject.Name = "SqlTpl_" +getGuid32();
                    this.apiinfo.ExecutorList.push(newObject);
                    //console.log(this.apiinfo.ExecutorList);
                },
                DelExecutorList: function (item) {
                    var that = this;
                    this.$confirm("确定要删除该执行者【" + item.Name + "】", "提示", {
                        confirmButtonText: "确定",
                        cancelButtonText: "取消",
                    }).then(function (rlt) {
                        if (item.NextExecutors.length > 0) {
                            that.$message.error("删除失败,该执行者下包含下级执行者！");
                        }
                        else {
                            that.apiinfo.ExecutorList.splice(item, 1);
                        }
                    });
                }
            },
            components: {
                'el-api-executor': 'url:/component/el-api-executor.vue',
            }
        };
    </script>
    <style scoped>
        .interface-title {
            font-size: 18px;
            clear: both;
            font-weight: normal;
            margin-top: 0.48rem;
            margin-bottom: 0.16rem;
            border-left: 3px solid #2395f1;
            padding-left: 8px;
            /*.tooltip{
                font-size: 13px;
                font-weight: normal;
            }*/
        }

        .panel-view {
            margin-left: 30px;
            font-size: 13px;
            background: rgba(236, 238, 241, 0.67);
            border-radius: 2px;
            margin-top: 2px;
            margin-bottom: 2px;
            border: 0px;
            overflow: hidden;
        }
        .el-collapse-item__wrap {
            will-change: height;
            background-color: #FFF;
            overflow: hidden;
            -webkit-box-sizing: border-box;
            box-sizing: border-box;
            border-bottom: 0px solid #EBEEF5 !important;
        }
    </style>
