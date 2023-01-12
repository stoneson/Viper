<template>
    <div id="layout11" class="l-layout" style="width: 100%; height: 100%; overflow: hidden;">
        <el-form :model="apiinfo" style="width: 100%; min-width: 250px; margin-top: 10px; ">
            <!--<el-collapse value="1" accordion>-->
            <!--<el-collapse-item name="1">-->
            <!--<h3 slot="title">基本信息</h3>-->
            <h2 class="interface-title" style="margin-top: 20px;margin-bottom:20px">
                基本信息
            </h2>
            <div class="panel-view">
                <el-row>
                    <el-col :span="12">
                        <el-form-item label="接口名称：" label-width="120px">
                            <span class="zt">{{apiinfo.Name}}</span>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="接口标题：" label-width="120px">
                            <span class="zt">{{apiinfo.Title}}</span>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="12">
                        <el-form-item label="接口路径：" label-width="120px">
                            <el-container style="height:36px">
                                <!--<span slot="title" :style="apiinfo.HttpMethod=='GET'?'color: #095694; background-color: #8eafca':'color: #00a854; background-color: #cfefdf' ">{{apiinfo.HttpMethod}}</span>-->
                                <span :style="apiinfo.HttpMethod=='GET'?'color: #095694; background-color: #8eafca':'color: #00a854; background-color: #cfefdf'" class="meth zt">{{apiinfo.HttpMethod}}</span>
                                <span style="margin-left:20px;margin-right:5px;font-size:15px" class="zt">
                                    {{apiinfo.RoutePrefix}}{{apiinfo.Route}}
                                    <el-tooltip content="复制路径" placement="top">
                                        <i style="margin-left: 10px; width: 24px; cursor: pointer;" class="el-icon-document-copy" @click="clickCopy()"></i>
                                    </el-tooltip>
                                </span>
                            </el-container>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label-width="120px">
                            <span slot="label" class="zt">
                                多个执行者
                                <el-tooltip content="是否支持多执行者同时执行" placement="top">
                                    <i class="el-icon-question"></i>
                                </el-tooltip>:
                            </span>
                            <span class="undone zt" v-if="!apiinfo.MultiExecutor">{{apiinfo.MultiExecutor?"是":"否"}}</span>
                            <span class="done zt" v-if="apiinfo.MultiExecutor">{{apiinfo.MultiExecutor?"是":"否"}}</span>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="12">
                        <el-form-item label="接口状态：" label-width="120px">
                            <span class="undone zt" v-if="apiinfo.Stat=='0'">{{apiinfo.Stat=="1"?"开启":"停用"}}</span>
                            <span class="done zt" v-if="apiinfo.Stat=='1'">{{apiinfo.Stat=="1"?"开启":"停用"}}</span>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label-width="120px">
                            <span slot="label" class="zt">
                                驼峰结果
                                <el-tooltip content="返回结果是否为驼峰命名" placement="top">
                                    <i class="el-icon-question"></i>
                                </el-tooltip>:
                            </span>
                            <span class="undone zt" v-if="!apiinfo.IsCamelCaseNames">{{apiinfo.IsCamelCaseNames?"是":"否"}}</span>
                            <span class="done zt" v-if="apiinfo.IsCamelCaseNames">{{apiinfo.IsCamelCaseNames?"是":"否"}}</span>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="备注：" label-width="120px">
                            <span class="zt">{{apiinfo.Remark}}</span>
                        </el-form-item>
                    </el-col>
                </el-row>
            </div>
            <h2 class="interface-title" style="margin-top: 20px; margin-bottom: 20px ">
                请求头部
            </h2>
            <div style="margin-bottom: 10px">
                <el-table :data="Headers"
                          style="width: 100%"
                          :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
                          default-expand-all indent="16" border>
                    <el-table-column label="参数" width="200px" prop="Name">
                    </el-table-column>
                    <el-table-column label="值"  prop="value">
                    </el-table-column>
                </el-table>
            </div>
            <h2 class="interface-title" style="margin-top: 20px; margin-bottom: 20px">
                请求参数
            </h2>
            <div v-if="apiinfo.HttpMethod==='GET'">
                <el-table :data="requestdata"
                          style="width: 100%"
                          :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
                          default-expand-all indent="16" border>
                    <el-table-column label="参数" width="200px" prop="name">
                    </el-table-column>
                    <el-table-column label="数据类型" width="200px" prop="type">
                    </el-table-column>
                    <el-table-column label="值" width="200px" prop="value">
                    </el-table-column>
                    <el-table-column label="描述"  prop="description">
                    </el-table-column>

                </el-table>
            </div>
            <el-input type="textarea" :rows="15" v-model="request" v-if="apiinfo.HttpMethod==='POST'" :disabled="true"></el-input>
            <h2 class="interface-title" style="margin-top: 20px; margin-bottom: 20px">
                返回数据
            </h2>
            <el-input type="textarea" :rows="17" v-model="ResponseParameters" :disabled="true"></el-input>
            <!--</el-collapse>-->
        </el-form>
    </div>
</template>

<script>
    module.exports = {
        props: {
            request: {
                type: String,
                default: function () {
                    return ""
                }
            },
            requestdata: {
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
                                    ID: "",
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
                disabled: true,
                ResponseParameters: "",
                questtype: "application/json",
                ContentType: ["application/json", "text/json", "application/xml", "text/xml", "application/x-www-form-urlencoded"],
                Headers: [{ ID: "1", Name: "ContentType", value: "application/json" }]
            };
        },
        created: function () {
            var that = this;
            that.disabled = true;
            //用于数据初始化
            document.title = "接口信息";
            //console.log(that.apiinfo);
        },
        mounted: function () {
            var that = this;
            var val = {
                "msg": "string",
                "status": "bool",
                "outputData": "{}"
            };
            that.ResponseParameters = JSON.stringify(val, null, 4);
        },
        methods: {
            clickCopy() {
                var that = this;
                const save = function (e) {
                    var msg = that.apiinfo.RoutePrefix + that.apiinfo.Route;
                    e.clipboardData.setData('text/plain', msg)
                    e.preventDefault() // 阻止默认行为
                }
                const once = {
                    once: true
                }
                document.addEventListener('copy', save, once) // 添加一个copy事件,当触发时执行一次,执行完删除
                document.execCommand('copy') // 执行copy方法
                that.$message({ message: '已经成功复制到剪切板', type: 'success' })
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
    /* .el-form-item__label {
      font-size:15px;
    }*/
    .meth {
        padding: 0px 6px;
        margin-right: 8px;
        border-radius: 4px;
        vertical-align: middle;
    }

    .undone:before {
        background-color: #ff561b;
        content: "";
        display: inline-block;
        margin-right: 6px;
        width: 10px;
        height: 10px;
        border-radius: 50%;
        position: relative;
        bottom: 1px;
    }

    .done:before {
        background-color: #57cf27;
        content: "";
        display: inline-block;
        margin-right: 6px;
        width: 10px;
        height: 10px;
        border-radius: 50%;
        position: relative;
        bottom: 1px;
    }

    .panel-view {
        /*margin-left: 30px;*/
        font-size: 13px;
        /*background: rgba(236, 238, 241, 0.67);*/
        border-radius: 2px;
        margin-top: 2px;
        margin-bottom: 2px;
        border: 0px;
        overflow: hidden;
    }

    .el-input.is-disabled .el-input__inner {
        background-color: #F5F7FA;
        border-color: #E4E7ED;
        color: #000000;
        cursor: not-allowed;
    }

    .el-textarea.is-disabled .el-textarea__inner {
        background-color: #F5F7FA;
        border-color: #E4E7ED;
        color: #000000;
        cursor: not-allowed;
    }

    .zt {
        font-weight: revert;
    }

    .panel-view .el-form-item {
        margin-bottom: 10px;
    }

    .panel-view .el-form-item__label {
        height: 36px;
        margin-right: 20px;
    }
    .el-table .cell {
        -webkit-box-sizing: border-box;
        box-sizing: border-box;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: normal;
        word-break: break-all;
        line-height: 23px;
        padding-left: 10px;
        padding-right: 10px;
        font-size:16px;
    }
</style>