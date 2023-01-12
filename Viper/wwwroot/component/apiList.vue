<template>
    <div id="layout1" class="l-layout" style="width: 100%; height: 100%;overflow:hidden;">
        <!--<div class="l-layout-center" style="width:100%; left:1px; top: 1px;"><div>-->

        <el-container style="width: 100%; height: 60px; border: 1px solid #eee; vertical-align: middle; background-color: #D3DCE6;">
            <span class="interface-title" style="height: 100%; font-size: x-large; font-weight: normal; margin-left: 15px; margin-top: 10px; ">{{headInfo}}</span>
            <el-form ref="form" style="width: 300px; margin-left: 15px; margin-top: 10px; ">
                <el-form-item>
                    <!--地址名称：-->
                    <el-input style="width: 200px"
                              prefix-icon="el-icon-search"
                              placeholder="搜索接口"
                              v-model="formData.apiName"></el-input>
                    <el-button type="info" v-on:keyup.enter="onQuery" @click="onQuery">查询</el-button>
                    <!--<el-button icon="el-icon-plus" type="success" @click="openAddApiInfo">增加连接</el-button>-->
                </el-form-item>
            </el-form>
            <div uInfo style="height: 100%; float: right; margin-top: 10px; margin-right: 10px; ">
                <el-button type="primary" @click="addApiInfo">添加接口</el-button>
            </div>
        </el-container>
        <el-table :data="GridData.filter(data => !formData.apiName || data.Name.toLowerCase().includes(formData.apiName.toLowerCase())||data.Title.includes(formData.apiName))"
                  border
                  stripe
                  size="mini"
                  :max-height="window.innerHeight-110"
                  style="width: 100%">
            <el-table-column type="index" fixed :index="indexMethod">
            </el-table-column>
            <el-table-column fixed prop="Name" show-overflow-tooltip label="接口名称" width="180">
            </el-table-column>
            <el-table-column fixed prop="Title" show-overflow-tooltip label="接口标题" width="180">
            </el-table-column>
            <el-table-column show-overflow-tooltip label="接口路径" style="min-width:250px">
                <template slot-scope="scope">
                    <span :style="(scope.row.HttpMethod=='GET')?'color: #095694;' :'color: #00a854;' ">
                        {{ scope.row.HttpMethod}}
                    </span>
                    <span>{{ scope.row.RoutePrefix }}{{ scope.row.Route }}</span>
                </template>
            </el-table-column>

            <el-table-column prop="ControllerName" show-overflow-tooltip label="接口分类" width="130">
            </el-table-column>
            <el-table-column prop="Stat"
                             show-overflow-tooltip="true"
                             label="状态"
                             width="80">
                <template slot-scope="scope">
                    <el-tag :type="scope.row.Stat ? 'success' : 'danger'">
                        {{
            scope.row.Stat ? "开启" : "停用"
                        }}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column fixed="right" align="center" width="300" label="操作">
                <template slot-scope="scope">
                    <el-button @click="DialogApiInfo(scope.row,1)" type="text" size="small">上传接口</el-button>
                    <el-button @click="DialogApiInfo(scope.row,2)" type="text" size="small">下载接口</el-button>
                    <el-button @click="openAddApiInfo(scope.row)" type="text" size="small">编辑</el-button>
                    <el-button @click="deleteApiInfo(scope.row)" type="text" size="small">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <div class="block" style="text-align:center">
            <el-pagination @size-change="handleSizeChange" style="width: 100%;"
                           @current-change="handleCurrentChange"
                           :current-page="currentPage"
                           :page-sizes="pagesizes"
                           :page-size="pagesize"
                           layout="total, sizes, prev, pager, next, jumper"
                           :total="total">
            </el-pagination>
        </div>
        <el-dialogbar :apiinfo="ApiInfo" @refresh="search"></el-dialogbar>
        <el-syncinterface :urlsdata="urlsData"></el-syncinterface>
    </div>
    <!-- </div></div>-->

</template>

<script>
    module.exports = {
        data: function () {
            return {
                GridData: [],
                formData: {
                    apiName: "",
                },
                headInfo: "接口列表",
                menuroot: [],
                total: 0,
                currentPage: 1,
                pagesize: 20,
                pagesizes: [10, 15, 20, 30, 40],
                ApiInfo: {
                    Title: "",
                    Namespace: "",
                    Name: "",
                    RoutePrefix: "",
                    DefaultDBName: "",
                    Remark: "",
                    show: false
                },
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
                urlsData: {
                    title: "",
                    show: false,
                    KeyWord: "",
                    BaseUri: "",
                    issync: false,
                    Info: "",
                    Name: ""
                }
            };
        },
        created: function () {
            var that = this;

            //用于数据初始化
            document.title = "接口列表";
            this.keyupAnno();
            this.getControllermenus();
        },
        mounted: function () {
            var that = this;
            //console.log("apilist");
            var args = anno.GetUrlParms();
            //console.log(args);
            that.headInfo = unescape(args["headInfo"]);
            this.loadData(1, 20);
            window.onQuery = that.onQuery;
        },
        methods: {
            search(id) { //上面 @refresh调用的方法就是自己定义的
                window.parent.search(id);
            },
            getControllermenus: function () {
                var that = this;
                var args = anno.GetUrlParms();
                GetControllers(that, args["name"], true, "", function (data) {
                    that.menuroot = data;
                });
            },
            deleteApiInfo: function (row) {
                var that = this;
                this.$confirm("确定要删除接口【" + row.Name+"】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    DelApiInfo(that, row.ControllerName, row.Name, function (res) {
                        if (res) {
                            that.onQuery();
                        }
                    });
                }).catch(() => {
                    this.$message({
                        type: 'info',
                        message: '已取消删除'
                    });
                });;
            },
            onQuery: function () {
                var that = this;
                //alert("121");
                //console.log(that.currentPage);
                that.loadData(that.currentPage, that.pagesize);
            },
            indexMethod: function (index) {
                return index + 1;
            },
            keyupAnno: function () {
                var that = this;
                document.onkeydown = function (e) {
                    var _key = window.event.keyCode;
                    if (_key === 13) {
                        that.onQuery();
                    }
                };
            },
            handleSizeChange: function (val) {
                var that = this;
                that.pagesize = val;
                that.loadData(that.currentPage, val);
            },
            handleCurrentChange: function (val) {
                var that = this;
                that.currentPage = val;
                that.loadData(val, that.pagesize);
            },
            loadData: function (page, pagesize) {
                var that = this;
                GetApiInfo(that, page, pagesize, "", true, "", function (data) {
                    that.GridData = data.Rows;
                    that.total = parseInt(data.Total);
                });
            },
            openAddApiInfo: function (row) {
                var data = new Object();
                data.Id = row.KeyWord;
                this.openMenuItemApi(data);
            },
            openMenuItemApi: function (item) {
                //console.log(item.Id);
                var that = this;
                window.location = "/html/component.html?anno_component_name=apiContent&id=" + escape(item.Id);
                //console.log("openMenuItemApis=" + item.Id);
            },
            addApiInfo: function () {
                var input = anno.getInput();
                var args = anno.GetUrlParms();
                input.ControllerName = args["name"];
                //alert(input.ControllerName);
                //console.log(input.ControllerName);
                this.ApiInfo.Title = this.menuroot[0].Title;
                this.ApiInfo.Namespace = "";
                this.ApiInfo.Name = this.menuroot[0].Name;
                this.ApiInfo.RoutePrefix = this.menuroot[0].RoutePrefix;
                this.ApiInfo.DefaultDBName = this.menuroot[0].DefaultDBName;
                this.ApiInfo.Remark = this.menuroot[0].Remark;
                this.ApiInfo.show = true;
            },
            DialogApiInfo: function (row,state) {
                var that = this;
                if (state == 1) {
                    InitsyncInterface(that, "当前同步接口(" + row.Name + ")", row.KeyWord, "", true, "同 步");
                }
                else {
                    InitsyncInterface(that, "当前下载接口(" + row.Name + ")", row.KeyWord, "", false, "下 载");
                }
                
            },
        },
        components: {//<!--命名全部小写、或者下划线隔开，【中划线或驼峰会报错】  -- >
            'el-dialogbar': 'url:/component/el-dialogBar.vue',
            'el-syncinterface': 'url:/component/el-syncInterface.vue',
        }
    };
</script>

<style scoped>
    .interface-title {
        clear: both;
        font-weight: normal;
        margin-top: 0.48rem;
        margin-bottom: 0.16rem;
        border-left: 3px solid #2395f1;
        padding-left: 8px;
       

    }
    .tooltip {
        font-size: 13px;
        font-weight: normal;
    }
    #tongbu .el-dialog__body {
        height: 30px;
        margin-top: -10px;
    }
    #tongbu .el-dialog {
    margin-top:30vh !important;
    }
</style>