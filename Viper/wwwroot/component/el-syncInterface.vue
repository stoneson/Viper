<template>
    <div>
         <el-dialog :title="urlsdata.title" :visible.sync="urlsdata.show" width="30%" id="tongbu">
             <el-form ref="urlsdata" :model="urlsdata" label-position="top">
                 <el-form-item label="接口" label-width="50%" prop="KeyWord" v-show="false">
                     <el-input v-model="urlsdata.KeyWord" autocomplete="off"></el-input>
                 </el-form-item>
                 <el-form-item label="控制器" label-width="50%" prop="Name" v-show="false">
                     <el-input v-model="urlsdata.Name" autocomplete="off"></el-input>
                 </el-form-item>
                 <el-form-item label="" label-width="120px" prop="BaseUri">
                     <el-select style="width:100%" v-model="urlsdata.BaseUri" placeholder="请选择对应环境地址">
                         <el-option v-for="item in GridSetingData"
                                    :key="item.BaseUri"
                                    :label="item.BaseUri"
                                    :value="item.BaseUri">
                             <span style="float: left">{{ item.Name }}</span>
                             <span style="float: right; color: #8492a6; font-size: 15px">{{ item.BaseUri }}</span>
                         </el-option>
                     </el-select>
                 </el-form-item>
             </el-form>
            <div slot="footer" class="dialog-footer">
                <el-button @click="urlsdata.show = false">取 消</el-button>
                <el-button type="primary" @click="SaveSyncApiInfo('urlsdata')">{{urlsdata.Info}}</el-button>
            </div>
        </el-dialog>
    </div>
</template>
<script>
    module.exports = {
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
                Info:"",
                SyncApiInfo: {},
                GridSetingData: [],
                menuroot:[]
            };
        },
        props: {
            urlsdata: {
                type: Object,
                default: function () {
                    return {
                        title: "",
                        show: false,
                        KeyWord: "",
                        BaseUri: "",
                        issync: false,
                        Info: "",
                        Name: ""
                    }
                }
            },
        },
        created: function () {
            //console.log("el-syncinterface");
        },
        mounted: function () {
            var that = this;
            that.loadSetingData();
        },
        methods: {
            loadSetingData: function () {
                var that = this;
                GetSetingUriInfo(that, "", function (data) {
                    that.GridSetingData = data;
                });
            },
            GetapiInfo: function (that) {
                var result = false;

                if (that.urlsdata.Name != "") {
                    GetControllers(that, that.urlsdata.Name, that.urlsdata.issync, that.urlsdata.BaseUri, function (data) {
                        if (data.length > 0) {
                            that.menuroot = data;
                            result = true;
                        }
                        else {
                            that.$message.error("未获取到分类[" + that.urlsdata.Name + "]信息!");
                        }
                    })
                }
                else {
                    GetApiInfo(that, 1, 20, that.urlsdata.KeyWord, that.urlsdata.issync, that.urlsdata.BaseUri, function (data) {
                        if (data.Rows.length > 0) {
                            that.SyncApiInfo = data.Rows[0];
                            result = true;
                        }
                        else {
                            that.$message.error("未获取到接口[" + that.urlsdata.KeyWord + "]信息!");
                        }
                    });
                }
                return result;
            },
            SaveapiInfo: function (that) {
                var input = anno.getInput();

                that.InputData.Name = that.SyncApiInfo.Name;
                that.InputData.HttpMethod = that.SyncApiInfo.HttpMethod;
                that.InputData.Route = that.SyncApiInfo.Route;
                that.InputData.Title = that.SyncApiInfo.Title;
                that.InputData.Remark = that.SyncApiInfo.Remark;
                that.InputData.MultiExecutor = that.SyncApiInfo.MultiExecutor;
                that.InputData.IsCamelCaseNames = that.SyncApiInfo.IsCamelCaseNames;
                that.InputData.Stat = that.SyncApiInfo.Stat;
                that.InputData.ExecutorList = JSON.stringify(that.SyncApiInfo.ExecutorList);
                that.InputData.HasExecutor = that.SyncApiInfo.HasExecutor;
                that.InputData.KeyWord = that.SyncApiInfo.KeyWord;
                that.InputData.RoutePrefix = that.SyncApiInfo.RoutePrefix;
                that.InputData.Namespace = that.SyncApiInfo.Namespace;
                that.InputData.ControllerName = that.SyncApiInfo.ControllerName;
                that.InputData.uname = input.uname;
                that.InputData.profile = input.profile;
                
                var str = that.urlsdata.issync ? "上传" : "下载";
                if (that.urlsdata.Name != "") {
                    if (that.menuroot != undefined && that.menuroot != null && that.menuroot.length > 0) {
                        var menudata = that.menuroot[0];
                        SaveController(that, menudata, !that.urlsdata.issync, that.urlsdata.BaseUri, str, function (data) {
                            that.urlsdata.show = false;
                            that.$emit("refresh", data.split('|')[1],0);
                        })
                    }
                }
                else {
                    SaveApiInfo(that, that.InputData, !that.urlsdata.issync, that.urlsdata.BaseUri, str, function (data) {
                        that.urlsdata.show = false;
                        that.$emit("refresh", data.split('|')[1]);
                    });
                }
               
            },
            SaveSyncApiInfo: function () {
                var that = this;
                if (that.urlsdata.BaseUri!="") {
                    if (that.GetapiInfo(that)) {
                        that.SaveapiInfo(that);
                    }
                } else {
                    var str = that.urlsdata.issync ? "上传地址必填!" : "下载地址必填!";
                    that.$message.error(str);
                    return false;
                }
            },
        }
    };
</script>
