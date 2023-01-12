<template>
    <div>
        <el-dialog title="添加接口" :visible.sync="apiinfo.show" @close ='resetForms()'>
            <el-form ref="ApiData" :model="ApiData">
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="分类名" label-width="120px" prop="Title">
                            <el-input v-model="apiinfo.Title" autocomplete="off" :disabled="true"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="接口名称" label-width="120px" prop="Name" :rules="{ required: true, message: '请输入接口名称', trigger:['blur','change'] }">
                            <el-input v-model="ApiData.Name" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="接口标题" label-width="120px" prop="Title" :rules="{ required: true, message: '请输入接口标题', trigger:['blur','change'] }">
                            <el-input v-model="ApiData.Title" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row :gutter="18">
                    <el-col :span="8">
                        <el-form-item label="接口路径" label-width="120px" prop="HttpMethod" :rules="{ required: true, message: '请选择接口路径', trigger:['blur','change'] }">
                            <el-select size="small" v-model="ApiData.HttpMethod" placeholder="请选择">
                                <el-option label="GET" value="GET"> </el-option>
                                <el-option label="POST" value="POST"> </el-option>
                                <!--<el-option label="PUT" value="PUT"> </el-option>
                                <el-option label="DELETE" value="DELETE"> </el-option>-->
                            </el-select>
                        </el-form-item>
                    </el-col>
                    <el-col :span="6">
                        <el-form-item prop="RoutePrefix">
                            <el-input v-model="apiinfo.RoutePrefix" autocomplete="off" :disabled="true"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="6">
                        <el-form-item prop="Route" :rules="{ required: true, message: '请输入路由名称', trigger:['blur','change'] }">
                            <el-input v-model="ApiData.Route" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="接口类型" label-width="120px" prop="ExecType" :rules="{ required: true, message: '请选择接口类型', trigger: ['blur','change'] }">
                            <el-select size="small" v-model="ApiData.ExecType" placeholder="请选择" @change="DataChange()">
                                <el-option label="脚本" value="SqlTpl"> </el-option>
                                <el-option label="WebApi" value="RemoteApi"> </el-option>
                                <el-option label="WCF/WS" value="RemoteWS"> </el-option>
                                <el-option label="DLL库" value="LocalDLL"> </el-option>
                                <el-option label="CS脚本" value="CSScript"> </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="地址" v-if="ApiData.ExecType==='RemoteApi'||ApiData.ExecType==='RemoteWS'" label-width="120px" prop="BaseUri" :rules="{ required: true, message: '请选择地址', trigger: ['blur','change'] }">
                            <el-select style="width:300px;" v-model="ApiData.BaseUri" placeholder="请选择">
                                <el-option v-for="item in UriInfoData"
                                           :key="item.Name"
                                           :label="item.Name"
                                           :value="item.Name">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="类名" v-if="ApiData.ExecType === 'LocalDLL'" label-width="120px" prop="FullClassName" :rules="{ required: true, message: '请输入类名', trigger: ['blur','change'] }">
                            <el-input v-model="ApiData.FullClassName" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="脚本" v-if="ApiData.ExecType === 'CSScript'" label-width="120px" prop="CSScriptCode" :rules="{ required: true, message: '请输入脚本', trigger: ['blur','change'] }">
                            <el-input v-model="ApiData.CSScriptCode" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="脚本类型" v-if="ApiData.ExecType === 'SqlTpl'" label-width="120px" prop="CmdType" :rules="{ required: true, message: '请输入脚本类型', trigger: ['blur','change'] }">
                            <el-select size="small" v-model="ApiData.CmdType" placeholder="请选择">
                                <el-option label="SQL脚本" value="T"> </el-option>
                                <el-option label="存储过程" value="P"> </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="Sql" v-if="ApiData.ExecType === 'SqlTpl'" label-width="120px" prop="Sql" :rules="{ required: true, message: '请输入出参名称', trigger: ['blur','change'] }">
                            <el-input v-model="ApiData.Sql" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="出参名称" v-if="ApiData.ExecType === 'SqlTpl'" label-width="120px" prop="OutputName" :rules="{ required: true, message: '请输入出参名称', trigger: ['blur','change'] }">
                            <el-input v-model="ApiData.OutputName" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="20">
                        <el-form-item label="备注" label-width="120px">
                            <el-input v-model="ApiData.Remark" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <div slot="footer" class="dialog-footer">
                <el-button @click="apiinfo.show = false">取 消</el-button>
                <el-button type="primary" @click="addClass('ApiData')">确 定</el-button>
            </div>
        </el-dialog>
    </div>
</template>
<script>
    module.exports = {
        data: function () {
            return {
                ApicurrentData: {
                    show: false,
                    title: "增加地址",
                    ID: -1,
                    Name: "",
                    BaseUri: "",
                    Remark: "",
                },
                ApiData: {
                    Title: "",
                    Name: "",
                    ExecType: "",
                    BaseUri: "",
                    HttpMethod: "",
                    Route: "",
                    FullClassName: "",
                    CSScriptCode: "",
                    CmdType: "",
                    OutputName: "",
                    Sql: "",
                    Remark: ""
                },
                ConnectionData: [],
                UriInfoData: [],
                ApiInfo: {
                    Name: "GetOneEntity",
                    HttpMethod: "GET",
                    Route: "GetOneEntity",
                    Title: "测试WebServiceDemo",
                    Remark: "无入参返回实体",
                    FullClassName: "",
                    ExecutorList: "",
                    //OutputList:"",
                    RoutePrefix: "api2/HCenter",
                    ControllerName: "TestApi"
                }
            };
        },
        props: {
            apiinfo: {
                type: Object,
                default: function () {
                    return {
                        Title: "",
                        Namespace: "",
                        Name: "",
                        RoutePrefix: "",
                        DefaultDBName: "",
                        Remark: "",
                        show: false
                    }
                }
            },
        },
        //watch: {
        //    'apiinfo.Title': {
        //        handler(newV, oldV) {
        //            console.log("watch", newV, oldV)
        //            this.ApiData.Title = newV;
        //        },
        //        immediate: true
        //    }
        //},
        created: function () {
            var that = this;
            /* that.ApiData.Title = that.apiinfo.Title;*/
            //用于数据初始化
            document.title = "接口信息";
            //console.log(that.apiinfo);
            that.getUriInfoData();
        },
        mounted: function () {
        },
        methods: {
            resetForms() {
                this.$refs['ApiData'].resetFields();   // form是绑定数据,一定记得加prop参数，不加的话无法重置弹框表单数据
            },
            getUriInfoData: function () {
                var that = this;
                GetUriInfo(that, "", function (data) {
                    that.UriInfoData = data;
                });
            },
            DataChange: function () {
                var that = this;
                that.ApiData.BaseUri = "";
                that.ApiData.FullClassName = "";
                that.ApiData.CSScriptCode = "";
                that.ApiData.CmdType = "";
                that.ApiData.OutputName = "";
            },
            addClass: function (formName) {
                var that = this;
                this.$refs[formName].validate((valid, object) => {
                    if (valid) {
                        //var input = anno.getInput();
                        var input = {};
                        input.Title = that.ApiData.Title;
                        input.Name = that.ApiData.Name;
                        input.HttpMethod = that.ApiData.HttpMethod;
                        input.Route = that.ApiData.Route;
                        input.Remark = that.ApiData.Remark;
                        input.RoutePrefix = that.apiinfo.RoutePrefix;
                        input.ControllerName = that.apiinfo.Name;
                        input.ExecutorList = '[{"ExecType":"' + that.ApiData.ExecType + '","BaseUri":"' + that.ApiData.BaseUri + '","FullClassName":"' + that.ApiData.FullClassName + '","CSScriptCode":"' + that.ApiData.CSScriptCode + '","CmdType":"' + that.ApiData.CmdType + '","Sql":"' + that.ApiData.Sql + '","OutputList":[{"OutputName":"' + that.ApiData.OutputName + '"}]}]';
                        ////input.ApiInfo.OutputList = '[{"OutputName":"' + that.ApiData.OutputName + '"}]';
                        SaveApiInfo(that, input, true, "","保存", function (data) {
                            that.$emit("refresh", data.split('|')[1]);
                            that.apiinfo.show = false;//最后关闭弹窗子页面
                        });
                    } else {
                        that.$message.error("请填写信息完整!");
                        return false;
                    }
                });
            }
        },
        //components: {
        //    'el-apiview': httpVueLoader('/component/el-apiview.vue')
        //}
    };
</script>
