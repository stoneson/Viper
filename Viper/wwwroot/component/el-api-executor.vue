<template>

    <el-collapse value="ei{{executorinfo.Name}}" accordion>
        <el-collapse-item title=">>>基本信息" name="ei{{executorinfo.Name}}">
            <el-row>
                <el-col :span="8">
                    <el-form-item label="类型：" label-width="120px" required="true">
                        <el-select style="width:120px;" v-model="executorinfo.ExecType" placeholder="请选择" :disabled="disabled">
                            <el-option v-for="item in ExecTypes"
                                       :key="item.value"
                                       :label="item.Name"
                                       :value="item.value"
                                       :disabled="item.disabled">
                            </el-option>
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="名称：" label-width="120px" required="true">
                        <el-input v-model="executorinfo.Name" autocomplete="on" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label-width="120px">
                        <span slot="label">
                            日志跟踪
                            <el-tooltip content="是否开启日志跟踪" placement="top">
                                <i class="el-icon-question"></i>
                            </el-tooltip>：
                        </span>
                        <el-switch v-model="executorinfo.UseExecutorLog" :disabled="disabled">
                        </el-switch>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="8">
                    <el-form-item label="地址名称:" v-if="executorinfo.ExecType==='RemoteApi'||executorinfo.ExecType==='RemoteWS'" label-width="120px" required="true">
                        <el-select v-model="executorinfo.BaseUri" placeholder="请选择" :disabled="disabled">
                            <el-option v-for="item in urlinfodata"
                                       :key="item.Name"
                                       :label="item.Name"
                                       :value="item.Name">
                            </el-option>
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="调用路由:" v-if="executorinfo.ExecType==='RemoteApi'" label-width="120px" required="true">
                        <el-input v-model="executorinfo.Route" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="服务方法名:" v-if="executorinfo.ExecType==='RemoteWS'" label-width="120px" required="true">
                        <el-input v-model="executorinfo.Method" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="8">
                    <el-form-item label="DLL控件名称:" v-if="executorinfo.ExecType === 'LocalDLL'" label-width="120px" required="true">
                        <el-input v-model="executorinfo.DLLName" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="类名:" v-if="executorinfo.ExecType === 'LocalDLL'" label-width="120px" required="true">
                        <el-input v-model="executorinfo.FullClassName" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="方法名:" v-if="executorinfo.ExecType === 'LocalDLL'" label-width="120px" required="true">
                        <el-input v-model="executorinfo.Method" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col>
                    <el-form-item label="CS脚本:" v-if="executorinfo.ExecType === 'CSScript'" label-width="120px" required="true">
                        <!--<el-input v-model="executorinfo.CSScriptCode" autocomplete="off" :disabled="disabled"></el-input>-->
                        <el-input type="textarea" :rows="17" v-model="executorinfo.CSScriptCode" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="8">
                    <el-form-item label="脚本类型:" v-if="executorinfo.ExecType === 'SqlTpl'" label-width="120px" required="true">
                        <el-select size="small" v-model="executorinfo.CmdType" placeholder="请选择" :disabled="disabled">
                            <el-option label="SQL脚本" value="T"> </el-option>
                            <el-option label="存储过程" value="P"> </el-option>
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="数据库连接:" v-if="executorinfo.ExecType === 'SqlTpl'" label-width="120px" required="true">
                        <el-select style="width:300px;" v-model="executorinfo.DBName" placeholder="请选择" :disabled="disabled">
                            <el-option v-for="item in connectiondata"
                                       :key="item.Name"
                                       :label="item.Name"
                                       :value="item.Name">
                            </el-option>
                        </el-select>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="8">
                    <el-form-item label="存储名称:" v-if="executorinfo.ExecType === 'SqlTpl'&&executorinfo.CmdType==='P'" label-width="120px" required="true">
                        <el-input v-model="executorinfo.Sql" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col>
                    <el-form-item label="Sql脚本:" v-if="executorinfo.ExecType === 'SqlTpl'&&executorinfo.CmdType==='T'" label-width="120px" required="true">
                        <el-input type="textarea" :rows="17" v-model="executorinfo.Sql" autocomplete="off" :disabled="disabled"></el-input>
                    </el-form-item>

                </el-col>
            </el-row>
            <el-row>
                <div v-if="executorinfo.ExecType === 'SqlTpl'" style="margin-left:120px">
                    <el-table :data="executorinfo.OutputList"
                              style="width: 100%"
                              :row-style="{height:0+'px'}"
                              :cell-style="{padding:0+'px'}"
                              :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
                              default-expand-all indent="16">
                        <el-table-column width="100px" scoped-slot v-if="!disabled">
                            <template slot="header">
                                <el-tooltip content="添加出参" placement="top">
                                    <i @click="addOutRow()" class="el-icon-plus" style="width: 24px; cursor: pointer;"></i>
                                </el-tooltip>
                            </template>
                            <template v-slot="scope">
                                <el-tooltip content="删除出参" placement="top">
                                    <i @click="e => {e.stopPropagation();delOutRow(scope.row);}" class="el-icon-delete" style="width: 24px; cursor: pointer;"></i>
                                </el-tooltip>
                            </template>
                        </el-table-column>
                        <el-table-column label="出参名称" width="200px">
                            <template slot-scope="scope">
                                <el-input style="width:100%;" v-model="scope.row.OutputName" :disabled="disabled" autocomplete="on" placeholder="输入出参名称"></el-input>
                            </template>
                        </el-table-column>
                        <el-table-column label="是否返回数组" width="200px">
                            <template slot-scope="scope">
                                <el-select size="small" v-model="scope.row.IsArray" placeholder="请选择" :disabled="disabled">
                                    <el-option label="true" value="true"> </el-option>
                                    <el-option label="false" value="false"> </el-option>
                                </el-select>
                            </template>
                        </el-table-column>
                        <el-table-column label="数据源下标" width="200px">
                            <template slot-scope="scope">
                                <el-input style="width:100%;" v-model="scope.row.DataSoureIndex" :disabled="disabled" autocomplete="on" placeholder="输入数据源下标"></el-input>
                            </template>
                        </el-table-column>
                    </el-table>
                </div>
            </el-row>
        </el-collapse-item>
        <el-collapse-item title=">>>入参设置" name="21">
            <el-table :data="executorinfo.ApiInputInfoList"
                      style="width: 100%"
                      :row-style="{height:0+'px'}"
                      :cell-style="{padding:0+'px'}"
                      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
                      row-key="ID"
                      default-expand-all indent="16"
                      :tree-props="{children: 'SubFieldInfoList', hasChildren: 'hasChildren'}">

                <el-table-column width="200px" scoped-slot>
                    <template slot="header">
                        <el-tooltip content="添加入参" placement="top" v-if="!disabled">
                            <i @click="addRow(null)" class="el-icon-plus" style="width: 24px; cursor: pointer;"></i>
                        </el-tooltip>
                    </template>
                    <template v-slot="scope" v-if="!disabled">
                        <el-tooltip content="删除入参" placement="top">
                            <i @click="e => {e.stopPropagation();deleteInputInfo(scope.row);}" class="el-icon-delete" style="width: 24px; cursor: pointer;"></i>
                        </el-tooltip>
                        <el-tooltip v-if="scope.row.TypeName=='object'||scope.row.TypeName=='array'" content="添加下级入参" placement="top">
                            <i @click="addRow(scope.row)" class="el-icon-circle-plus-outline" style="width: 24px; cursor: pointer;"></i>
                        </el-tooltip>
                    </template>
                </el-table-column>
                <el-table-column label="入参名称" width="150px">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.Name" autocomplete="on" :disabled="disabled" placeholder="输入入参名称"></el-inputstyle="width:100%;">
                    </template>
                </el-table-column>
                <el-table-column label="入参类型" width="130px">
                    <template slot-scope="scope">
                        <el-select style="width:100%;" v-model="scope.row.TypeName" placeholder="请选择" :disabled="disabled">
                            <el-option v-for="item in CStypes"
                                       :key="item.value"
                                       :label="item.Name"
                                       :value="item.value"
                                       :disabled="item.disabled">
                            </el-option>
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column label="字段名称" width="130px">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.SqlName" :disabled="disabled" autocomplete="on" placeholder="输入对应数据库字段名称"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="匹配路径" width="130px">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.ValuePath" :disabled="disabled" autocomplete="on" placeholder="输入匹配路径"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="默认值" width="130px">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.DefaultValue" :disabled="disabled" autocomplete="on" placeholder="输入默认值"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="标题" width="130px">
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.Title" :disabled="disabled" autocomplete="on" placeholder="输入标题"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="备注">
                    <!--<template slot="header">
                <div class="cell" style="float: left; margin-top: 10px; ">备注</div>
                <el-button type="primary" icon="el-icon-edit" circle style="float:right;"></el-button>
            </template>-->
                    <template slot-scope="scope">
                        <el-input style="width:100%;" v-model="scope.row.Remark" :disabled="disabled" autocomplete="on" placeholder="输入备注"></el-input>
                    </template>
                </el-table-column>
                <!--<el-table-column fixed="right" align="left" width="110px" label="操作">
            <template slot-scope="scope">
                <el-tooltip content="删除节点" placement="top">
                    <i @click="e => {e.stopPropagation();deleteInputInfo(scope.row);}" class="el-icon-delete" style="width: 24px; cursor: pointer;"></i>
                </el-tooltip>
                <el-tooltip content="添加兄弟节点" placement="top">
                    <i @click="e => {e.stopPropagation();alert('添加兄弟节点')}" class="el-icon-plus" style="width: 24px; cursor: pointer;"></i>
                </el-tooltip>
                <el-tooltip v-if="scope.row.TypeName=='object'||scope.row.TypeName=='array'" content="添加子节点" placement="top">
                    <i @click="e => {e.stopPropagation();alert('添加子节点')}" class="el-icon-circle-plus-outline" style="width: 24px; cursor: pointer;"></i>
                </el-tooltip>
            </template>
        </el-table-column>-->
            </el-table>
        </el-collapse-item>
        <el-collapse-item title="下级执行者设置" name="12">
            <template slot="title">
                <el-tooltip content="添加下级执行者" placement="top" v-if="!disabled">
                    <i @click.stop.prevent="e => {e.stopPropagation();addNextExecutors()}" class="el-icon-plus" style="width: 24px; cursor: pointer;"></i>
                </el-tooltip>
                <span>  下级执行者设置</span>
            </template>
            <div style="padding-left:16px">
                <el-collapse value="nextexecutor0" accordion>
                    <el-collapse-item v-for="item,index in executorinfo.NextExecutors">
                        <span slot="name">nextexecutor{{index}}</span>
                        <template slot="title">
                            <el-tooltip content="删除当前下级执行者" placement="top" v-if="!disabled">
                                <i @click.stop.prevent="e => {e.stopPropagation();DelNextExecutors(item)}" class="el-icon-delete" style="width: 24px; cursor: pointer;"></i>
                            </el-tooltip>
                            <h4>   下级执行者【{{index+1}}】</h4>
                        </template>
                        <el-api-executor :executorinfo="item" :urlinfodata="urlinfodata" :connectiondata="connectiondata" :disabled="disabled"></el-api-executor>
                    </el-collapse-item>
                </el-collapse>
            </div>
        </el-collapse-item>
    </el-collapse>
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
            disabled: {
                type: Boolean,
                default: function () {
                    return false
                }
            },
            executorinfo: {
                type: Object,
                default: function () {
                    return {
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
                            ID:"",
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
                        OutputList: [
                            {
                                OutputName: "data",
                                DataSoureIndex: 0,
                                IsArray: true,
                                ApiInputInfoList: [],
                                HasOutput: false
                            }
                        ]
                    }
                }
            },
        },
        data: function () {
            return {
                ExecTypes: [
                    { Name: 'Sql脚本', value: 'SqlTpl' },
                    { Name: 'WebApi', value: 'RemoteApi' },
                    { Name: 'WCF/WS', value: 'RemoteWS' },
                    { Name: 'DLL库', value: 'LocalDLL' },
                    { Name: 'CS脚本', value: 'CSScript' },
                ],
                CmdTypes: [
                    { Name: 'SQL脚本', value: 'T' },
                    { Name: '存储过程', value: 'P' },
                ],
                CStypes: [
                    { Name: 'String', value: 'string' },
                    { Name: 'Int', value: 'int' },
                    { Name: 'Double', value: 'double' },
                    { Name: 'Decimal', value: 'decimal' },
                    { Name: 'Long', value: 'long' },
                    { Name: 'Float', value: 'float' },
                    { Name: 'Bool', value: 'bool' },
                    { Name: 'DateTime', value: 'datetime' },
                    { Name: 'Object', value: 'object' },
                    { Name: 'Array', value: 'array' },
                ],
                ApiInfo: {
                    Name: "",//字段名称），同级的不能重名
                    SqlName: "", //字段名称
                    TypeName: "string",//入参类型
                    DefaultValue: "",//默认值
                    AllowNullable: true,
                    Title: "",//标题
                    Remark: "",//备注
                    SqlTypeName: "string",
                    ValuePath: "",//匹配路径
                    SubFieldInfoList: [],
                    HasSub: false
                },
                NextExecutorsInfo: {
                    Name: "",
                    ExecType: "SqlTpl",
                    CodeField: "",
                    CodeSucceedValue: "",
                    MsgField: "",
                    DataField: "",
                    UseExecutorLog: true,
                    Route: "",
                    FullClassName: "",
                    CSScriptCode: "",
                    CmdType: "P",
                    Sql: "",
                    DLLName: "",
                    Conditions: {
                        ConditionType: "AND"
                    },
                    BaseUri: "",
                    Method: "",
                    NextExecutors: [],
                    ApiInputInfoList: [],
                    HasInput: false,
                    OutputList: []
                },
                OutputInfo:
                {
                    OutputName: "data",
                    DataSoureIndex: 0,
                    IsArray: true,
                    ApiInputInfoList: [],
                    HasOutput: false
                }
            };
        },
        created: function () {
            var that = this;

            //用于数据初始化
            document.title = "接口信息";
            //console.log(that.apiinfo);
            //console.log("el-api-executor");
        },
        mounted: function () {

        },
        methods: {
            deleteNextExecutorsChild: function (row, obj) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].Name == row.Name) {
                        const index = obj.findIndex(x => x.Name === row.Name);
                        obj.splice(index, 1);
                    }
                    else {
                        if (obj[i].NextExecutors.length > 0) {
                            this.deleteNextExecutorsChild(row, obj[i].NextExecutors)
                        }
                    }
                }
            },
            deleteInputInfoChild: function (row, obj) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].Name == row.Name) {
                        const index = obj.findIndex(x => x.Name === row.Name);
                        obj.splice(index, 1);
                    }
                    else {
                        if (obj[i].SubFieldInfoList.length > 0) {
                            this.deleteInputInfoChild(row, obj[i].SubFieldInfoList)
                        }
                    }
                }
            },
            deleteInputInfo: function (row) {
                var that = this;
                this.$confirm("确定要删除该节点【" + row.Name + "】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    //var input = anno.getInput();
                    //input.Name = row.Name;
                    //anno.process(input, "Platform/DelConnectionString", function (data) {
                    //    if (data.Head.ResponseCode === 'AA' && (data.Head.ResponseDesc === null || data.Head.ResponseDesc === "")) {
                    //        that.onQuery();

                    //        that.$message({ message: '连接删除成功', type: 'success' });
                    //    } else {
                    //        that.$message.error(data.Head.ResponseDesc);
                    //    }
                    //});
                    if (row.SubFieldInfoList.length > 0) {
                        that.$message.error("删除失败,该节点下包含子节点！");
                    }
                    else {
                        that.deleteInputInfoChild(row, that.executorinfo.ApiInputInfoList);
                    }
                });
            },
            addRow(row) {
                let newObject = JSON.parse(JSON.stringify(this.ApiInfo));
                newObject.ID =getGuid32();
                if (row != undefined) {
                    newObject.Name = row.Name+"_Name_" + row.SubFieldInfoList.length;
                    row.SubFieldInfoList.push(newObject);
                } else {
                    newObject.Name = "Name_" + this.executorinfo.ApiInputInfoList.length;
                    this.executorinfo.ApiInputInfoList.push(newObject);
                }
            },
            addOutRow() {
                let newObject = JSON.parse(JSON.stringify(this.OutputInfo));
                newObject.OutputName = "data" + this.executorinfo.OutputList.length;
                newObject.IsArray = false;
                newObject.DataSoureIndex = 0;
                this.executorinfo.OutputList.push(newObject);
            },
            delOutRow(item) {
                var that = this;
                this.$confirm("确定要删除该节点【" + item.OutputName + "】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    const index = that.executorinfo.OutputList.findIndex(x => x.OutputName === item.OutputName);
                    that.executorinfo.OutputList.splice(index, 1);
                });
            },
            addNextExecutors: function (item) {
                let newObject = JSON.parse(JSON.stringify(this.NextExecutorsInfo));
                newObject.ExecType = "RemoteApi";
                newObject.Name = "RemoteApi_" +getGuid32();
                if (item != undefined) {
                    item.NextExecutors.push(newObject);
                }
                else {
                    this.executorinfo.NextExecutors.push(newObject);
                }
            },
            DelNextExecutors: function (item) {
                var that = this;
                this.$confirm("确定要删除该下级执行者【" + item.Name + "】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    if (item.NextExecutors.length > 0) {
                        that.$message.error("删除失败,该下级执行者下包含下级执行者！");
                    }
                    else {
                        that.deleteNextExecutorsChild(item, that.executorinfo.NextExecutors);
                    }
                });
            }
        },
        components: {
            'el-api-executor': 'url:/component/el-api-executor.vue',//httpVueLoader('/component/el-api-executor.vue')
        }
    };
</script>
<style scoped>
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
    .el-collapse-item__wrap {
        will-change: height;
        background-color: #FFF;
        overflow: hidden;
        -webkit-box-sizing: border-box;
        box-sizing: border-box;
        border-bottom: 0px solid #EBEEF5 !important;
    }
</style>
