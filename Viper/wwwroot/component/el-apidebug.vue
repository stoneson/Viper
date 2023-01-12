<template>
    <div id="layout11" class="l-layout" style="width: 100%; height: 100%; overflow: hidden;" v-loading="loading" element-loading-text="运行中..." element-loading-spinner="el-icon-loading"
    element-loading-background="rgba(255,255,255,0)">
        <el-form :model="apiinfo" style="width: 100%; min-width: 250px; margin-top: 10px; ">
            <el-row>
                <el-col :span="1.5">
                    <el-select style="width:120px;" v-model="apiinfo.HttpMethod" placeholder="请选择" :disabled="true">
                        <el-option v-for="item in HttpMethods"
                                   :key="item.value"
                                   :label="item.Name"
                                   :value="item.value"
                                   :disabled="item.disabled">
                        </el-option>
                    </el-select>
                </el-col>
                <el-col :span="8">
                    <!--<el-input v-model="address" autocomplete="on"></el-input>-->
                    <el-select style="width:100%;" v-model="address" placeholder="请选择">
                        <el-option v-for="item in GridData"
                                   :key="item.BaseUri"
                                   :label="item.BaseUri"
                                   :value="item.BaseUri">
                        </el-option>
                    </el-select>
                </el-col>
                <el-col :span="8">
                    <el-input v-model="path" autocomplete="on" :disabled="true"></el-input>
                </el-col>
                <el-col :span="6">
                    <el-button type="danger" @click="Save()" style="margin-left:20px" :disabled="apiinfo.Stat==0">运行</el-button>
                </el-col>
            </el-row>
        </el-form>
        <h2 class="interface-title" style="margin-top: 20px; margin-bottom: 20px">
            请求头部
        </h2>
        <div style="margin-bottom: 10px">
            <el-table :data="Headers"
                      style="width: 100%"
                      :row-style="{height:0+'px'}"
                      :cell-style="{padding:0+'px'}"
                      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
                      default-expand-all indent="16">
                <el-table-column width="10%">
                    <template slot="header">
                        <el-tooltip content="添加Header参数" placement="top">
                            <i @click="addHeadRow()" class="el-icon-plus" style="width: 24px; cursor: pointer;"></i>
                        </el-tooltip>
                    </template>
                    <template v-slot="scope">
                        <el-tooltip content="删除节点" placement="top" v-if="scope.row.Name!=='ContentType'">
                            <i @click="e => {e.stopPropagation();delHeadRow(scope.row);}" class="el-icon-delete" style="width: 24px; cursor: pointer;"></i>
                        </el-tooltip>
                    </template>
                </el-table-column>
                <el-table-column label="参数" width="20%">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.Name"  autocomplete="on" placeholder="输入参数" :disabled="scope.row.Name==='ContentType'"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="值" width="70%">
                    <template slot-scope="scope">
                        <el-select style="width:100%" size="small" v-model="scope.row.value" placeholder="请选择" v-if="scope.row.Name==='ContentType'">
                            <el-option v-for="item in ContentType"
                                       :key="item"
                                       :label="item"
                                       :value="item">
                            </el-option>
                        </el-select>
                        <el-input style="width:100%;" v-model="scope.row.value" v-if="scope.row.Name!=='ContentType'"  autocomplete="on" placeholder="输入值"></el-input>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <h2 class="interface-title" style="margin-top: 20px; margin-bottom: 20px">
            请求参数
        </h2>
        <div v-if="apiinfo.HttpMethod==='GET'">
            <el-table :data="requestdata"
                      style="width: 100%"
                      :row-style="{height:0+'px'}"
                      :cell-style="{padding:0+'px'}"
                      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
                      default-expand-all indent="16">
                <el-table-column label="参数" width="20%">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.name" :disabled="true" autocomplete="on" placeholder="输入参数"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="数据类型" width="10%">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.type" :disabled="true" autocomplete="on" placeholder="输入数据类型"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="值" width="30%">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.value" autocomplete="on" placeholder="输入值"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="描述" width="40%">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.description" :disabled="true" autocomplete="on" placeholder="输入描述"></el-input>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <el-input type="textarea" :rows="15" v-model="request" v-if="apiinfo.HttpMethod==='POST'"></el-input>
        <h2 class="interface-title" style="margin-top: 20px; margin-bottom: 20px">
            返回数据
            <span style="padding-left:80%;" id="time_id">执行时间:<span style="color:red">{{time}}</span></span>
        </h2>
        <el-input type="textarea" :rows="17" v-model="ResponseParameters"></el-input>
    </div>
</template>

<script>
    module.exports = {
        props: {
            path: {
                type: String,
                default: function () {
                    return ""
                }
            },
            address: {
                type: String,
                default: function () {
                    return ""
                }
            },
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
                    { Name: 'PUT', value: 'PUT' },
                    { Name: 'DELETE', value: 'DELETE' },
                ],
                Stats: [
                    { Name: '开启', value: 1 },
                    { Name: '停用', value: 0 },
                ],
                Address: "",
                ResponseParameters: "",
                questtype:"application/json",
                ContentType: ["application/json", "text/json", "application/xml", "text/xml","application/x-www-form-urlencoded"],
                Headers: [{ ID: "1", Name: "ContentType", value: "application/json" }],
                loading: false,
                time: "",
                GridData: [],
            };
        },
        created: function () {
            var that = this;
            //console.log("el-apidebug");
            //that.Path = "/" + that.apiinfo.RoutePrefix + "/" + that.apiinfo.Route;
            // js 获取项目地址
           
            //that.RequestParameters = JSON.stringify(that.apiinfo, null,4);
            //用于数据初始化
            document.title = "接口信息";
            //console.log(that.apiinfo);
            //that.getSwaggerData();
            
        },
        mounted: function () {
            this.getGridData()
        },
        methods: {
            Save: function () {
                //console.log(JSON.stringify(this.apiinfo));
                //alert(JSON.stringify(this.apiinfo));
                var that = this;
                if (that.apiinfo.HttpMethod=="GET"&&that.requestdata!=null&&that.requestdata.length > 0) {
                    for (var i = 0; i < that.requestdata.length; i++) {
                        if (that.requestdata[i].value == '') {
                            that.$message({
                                message: that.requestdata[i].name+'--请求参数必填',
                                type: 'warning'
                            });
                            return;
                        }
                    }
                }
                var date1 = new Date();  //开始时间
                that.loading = true;
                
                HttpRequest(that, that.apiinfo.HttpMethod, that.Headers, that.requestdata, that.request, that.address, that.path, function (data) {
                    that.loading = false;
                    //alert(JSON.stringify(data));
                    var json = data;// JSON.parse(data);
                    that.ResponseParameters = JSON.stringify(json, null, 4);
                    var date2 = new Date();    //结束时间
                    var date3 = date2.getTime() - date1.getTime()  //时间差的毫秒数
                    that.time = date3 + "ms";
                });
            },
            addHeadRow() {
                let newObject = { ID: "ID_" + getGuid32(), Name: "", value: "" };
                this.Headers.push(newObject);
            },
            delHeadRow(item) {
                var that = this;
                this.$confirm("确定要删除该参数【" + item.ID + "】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    const index = that.Headers.findIndex(x => x.ID === item.ID);
                    that.Headers.splice(index, 1);
                });
            },
            getGridData: function () {
                var that = this;

                GetSetingUriInfo(that, "", function (data) {
                    that.GridData = data;
                    //if (data.length > 0)
                     //   that.address = data[0];
                });
            }
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
    .el-input.is-disabled .el-input__inner {
        background-color: #F5F7FA;
        border-color: #E4E7ED;
        color: #000000;
        cursor: not-allowed;
    }
    .el-textarea__inner {
        display: block;
        resize: vertical;
        padding: 5px 15px;
        line-height: 1.5;
        box-sizing: border-box;
        width: 100%;
        font-size: inherit;
        color: #000000;
        background-color: #FFF;
        border: 1px solid #DCDFE6;
        border-radius: 4px;
        transition: border-color .2s cubic-bezier(.645,.045,.355,1);
    }
    .el-input--small .el-input__inner {
        height: 40px;
        line-height: 40px;
    }
    .el-table .cell {
        -webkit-box-sizing: border-box;
        box-sizing: border-box;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: normal;
        word-break: break-all;
        line-height: 23px;
        padding-left: 0px;
        padding-right: 10px;
    }
    .el-table__header  {
        width: 100% !important;
    }
    .el-table__body {
        width: 100% !important;
    }
    .el-table__empty-block {
        width: 100% !important;
    }
</style>