<template>
    <div id="layout1" class="l-layout">
        <el-container>
            <el-container>
                <el-aside v-bind:style="{width:menumWidth}" style="width: 100%; height: 100vh; top: 0px; left: 0px; overflow: hidden; overflow-y: hidden ">
                    <div style="margin:0px">
                        <el-form ref="form" style="width: 100%">
                            <el-form-item style="background-color: #D3DCE6;height:44px;padding:5px">
                                <!--接口名称：-->
                                <el-input style="width: 170px"
                                          prefix-icon="el-icon-search"
                                          placeholder="搜索接口"
                                          v-model="formData.apiName" @input="filterInput"></el-input>
                                <el-button type="primary" v-on:keyup.enter="onQuery" @click="onQuery">添加分类</el-button>
                            </el-form-item>
                        </el-form>
                    </div>
                    <div :class="{pc_menum:!isMobile,mobile_menum:isMobile}" style="margin-top: -22px; ">
                        <el-tree :data="menuroot" :props="defaultProps" style="font-size: small; "
                                 node-key="Id" highlight-current ref="tree" @node-click="handleNodeClick"
                                 :expand-on-click-node="true" :filter-node-method="filterNode" class="tree-scroll">
                            <span class="custom-tree-node" slot-scope="{ node, data }" style="width: 100%;" 
                                  @mouseover="mouseover(data)" @mouseleave="mouseout(data)">
                                <el-tooltip effect="light" :content="data.Title" placement="top-start">
                                    <span>
                                        <!--绑定鼠标移入和移出事件-->
                                        <span slot="title" :style="(data.HttpMethod=='GET')?'color: #095694; background-color: #8eafca':'color: #00a854; background-color: #cfefdf' ">{{data.HttpMethod}}</span>
                                        <!--<span>{{ data.Title }}</span>-->
                                        <span>{{ellipsis(data.Title,25)}}</span>
                                        <span v-if="data.dropdownShow &&data.Level==1" class="btns" style="width: 100%; text-align: right">
                                            <el-tooltip content="上传接口" placement="top">
                                                <i @click.stop="syncInterface(data,1)" class="el-icon-upload2" style="width: 24px"></i>
                                            </el-tooltip>
                                            <el-tooltip content="下载接口" placement="top">
                                                <i @click.stop="syncInterface(data,2)" class="el-icon-download" style="width: 24px"></i>
                                            </el-tooltip>
                                            <el-tooltip content="删除接口" placement="top">
                                                <i @click.stop="syncInterface(data,3)" class="el-icon-delete" style="width: 24px"></i>
                                            </el-tooltip>
                                        </span>
                                        <span v-if="data.dropdownShow && data.Level==0" class="btns" style="width: 100%; text-align: right">
                                            <el-tooltip content="上传分类" placement="top">
                                                <i @click.stop="syncInterface(data,4)" class="el-icon-upload2" style="width: 24px"></i>
                                            </el-tooltip>
                                            <el-tooltip content="下载分类" placement="top">
                                                <i @click.stop="syncInterface(data,5)" class="el-icon-download" style="width: 24px"></i>
                                            </el-tooltip>
                                            <el-tooltip content="添加接口" placement="top">
                                                <i @click.stop="addType(data)" class="el-icon-plus" style="width: 24px"></i>
                                            </el-tooltip>

                                            <el-tooltip content="修改分类" placement="top">
                                                <i @click.stop="editType(data)" class="el-icon-edit" style="width: 24px"></i>
                                            </el-tooltip>
                                            <el-tooltip content="删除分类" placement="top">
                                                <i @click.stop="delType(data)" class="el-icon-delete" style="width: 24px"></i>
                                            </el-tooltip>
                                        </span>
                                    </span>
                                </el-tooltip>
                            </span>

                        </el-tree>
                    </div>
                    <div>
                        <el-dialog :title="currentData.title" :visible.sync="currentData.show">
                            <el-form ref="classData" :model="classData">
                                <el-form-item label="分类名" label-width="120px" prop="Title" :rules="{ required: true, message: '请输入分类名', trigger:['blur','change'] }">
                                    <el-input v-model="classData.Title" autocomplete="off"></el-input>
                                </el-form-item>
                                <!--<el-form-item label="命名空间" label-width="120px">
                                    <el-input v-model="classData.Namespace" autocomplete="off"></el-input>
                                </el-form-item>-->
                                <el-form-item label="控制器名称" label-width="120px" prop="Name" :rules="{ required: true, message: '请输入控制器名称', trigger:['blur','change'] }">
                                    <el-input v-model="classData.Name" autocomplete="off"></el-input>
                                </el-form-item>
                                <el-form-item label="控制器路由" label-width="120px" prop="RoutePrefix" :rules="{ required: true, message: '请输入控制器路由', trigger: ['blur','change'] }">
                                    <el-input v-model="classData.RoutePrefix" autocomplete="off"></el-input>
                                </el-form-item>
                                <el-form-item label="默认数据库" label-width="120px" prop="DefaultDBName" :rules="{ required: true, message: '请选择数据库', trigger:['blur','change'] }">
                                    <el-select style="width:300px;" v-model="classData.DefaultDBName" placeholder="请选择">
                                        <el-option v-for="item in ConnectionData"
                                                   :key="item.Name"
                                                   :label="item.Name"
                                                   :value="item.Name">
                                        </el-option>
                                    </el-select>
                                </el-form-item>
                                <el-form-item label="备注" label-width="120px">
                                    <el-input v-model="classData.Remark" autocomplete="off"></el-input>
                                </el-form-item>
                            </el-form>
                            <div slot="footer" class="dialog-footer">
                                <el-button @click="currentData.show = false">取 消</el-button>
                                <el-button type="primary" @click="addClass('classData')">确 定</el-button>
                            </div>
                        </el-dialog>
                    </div>
                    <el-dialogbar :apiinfo="ApiInfo" @refresh="search"></el-dialogbar>
                    <el-syncinterface :urlsdata="urlsData" @refresh="search"></el-syncinterface>
                </el-aside>
                <el-main style="width: 100%; height: 100vh; top: 0px; left: 0px; margin-top: -8px; margin-left: -8px;">

                    <!--<el-header style=" background-color: #D3DCE6;">-->
                    <!--<div v-on:click="changeisCollapse" style="width: 1px; float: left; cursor: pointer;">
                        <i :class="{'el-icon-s-fold':isCollapse==false,'el-icon-s-unfold':isCollapse}"></i>
                    </div>-->
                    <!--<div style=" float: left; ">
                        <span style="font-size: x-large; font-weight: normal;"> {{headInfo}}</span>
                    </div>
                    <div uInfo style="height: 60px; float: right; margin-right: 10px;">-->
                    <!--<el-avatar size="medium" style="vertical-align: middle;">{{menuroot.Lenght}}</el-avatar>-->
                    <!--<el-button type="primary" v-on:keyup.enter="onQuery" @click="onQuery">添加接口</el-button>
                        </div>
                    </el-header>-->
                    <el-container style="width: 100%;top: 0px; left: 0px;">
                        <div style="width: 100%; height: 100%; top: 0px; left: 0px; margin: 0px">
                            <iframe id="frmMain" name="frmMain" frameborder="0" @load="tabOnLoad(this)" :src="frmMainSrc" style="width: 100%; height: 100%; top: 0px; left: 0px;" />
                        </div>

                        <!--<el-tabs v-model="activeName" style="width:100%">
                            <el-tab-pane v-for="(item, index) in tabs"
                                         :key="item.name"
                                         :label="item.name"
                                         :name="item.name"
                                         :closable="item.closable">
                                <el-container v-loading="item.loading">
                                    <iframe :id="item.id" @load="tabOnLoad" frameborder="0" :src="item.src" style="width: 100%; height: calc(100% - 87px);">
                                    </iframe>
                                </el-container>
                            </el-tab-pane>
                        </el-tabs>-->
                    </el-container>
                </el-main>
            </el-container>
        </el-container>

    </div>
</template>

<script>
    module.exports = {
        data: function () {
            return {
                defaultProps: {
                    children: 'children',
                    label: 'Title',
                },
                formData: {
                    apiName: "",
                },
                menuroot: [],
                menumWidth: "300px",
                isCollapse: false,
                isMobile: false,
                frmMainSrc: "/html/component.html?anno_component_name=connectConfig",
                //headInfo: "接口列表",
                //tabs: [
                //    { id: "132054881103872", name: "服务监控", closable: false, src: "html/welcome.html?appName=WebApi", loading: false }
                //],
                openeds: [],
                currentData: {
                    show: false,
                    title: "增加地址",
                    ID: -1,
                    Name: "",
                    BaseUri: "",
                    Remark: "",
                },
                classData: {
                    Title: "",
                    Namespace: "",
                    Name: "",
                    RoutePrefix: "",
                    DefaultDBName: "",
                    Remark: ""
                },
                ConnectionData: [],
                ApiInfo: {
                    Title: "",
                    Namespace: "",
                    Name: "",
                    RoutePrefix: "",
                    DefaultDBName: "",
                    Remark: "",
                    show: false
                },
                urlsData: {
                    title: "",
                    show: false,
                    KeyWord: "",
                    BaseUri: "",
                    issync: false,
                    Info: "",
                    Name:""
                }
            };
        },
        created: function () {
            var that = this;
            //console.log("apiControllerList")
            that.getControllermenus();
            that.getConnectionData();
        },
        mounted: function () {
            var that = this;
            if (that.menuroot.length > 0) {
                that.handleNodeClick(that.menuroot[0]);
            }
            // 接受iframe的参数
            window.addEventListener('message', this.handleIncomingMessage, false);
            window.search = that.search;// 这里需要暴露给全局，这样的话，子页面才能调用相对应的方法
        },
        methods: {
            ellipsis(value, k) {
                if (!value) return "";
                var str = strCode(value, k);
                return str;
            },
            search(id,Level) { //上面 @refresh调用的方法就是自己定义的
                //console.log("apiControllerList");
                var that = this;
                that.getControllermenus();
                if (Level == 0) {
                    var aee = that.menuroot.find(ele => ele?.Id === id);
                    if (aee != null && aee != undefined) {
                        that.handleNodeClick(aee);
                        return;
                    }
                }
                else{
                    for (var i = 0; i < that.menuroot.length; i++) {
                        var aee = that.menuroot[i].children.find(ele => ele?.Id === id);
                        if (aee != null && aee != undefined) {
                            that.handleNodeClick(aee);
                            break;
                        }
                    }
                }
            },
            getControllermenus: function () {
                var that = this;
              
                GetControllerMenus(that, function (data) {
                    that.menuroot = data;
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            that.openeds.push(data[i].Id);
                        }
                    }
                });
            },
            handleIncomingMessage: function (event) {
                //console.log(event.data);
                this.$refs.tree.store.nodesMap[event.data.ControllerName].expanded = true;
                this.$refs.tree.setCurrentKey(event.data.Id)
            },
            filterNode: function (value, data) {
                if (!value) return true;
                return data.Title.indexOf(value) !== -1;
            },
            filterInput: function () {
                // 这里每当输入的数据有变化就触发原生的过滤节点这个函数
                this.$refs.tree.filter(this.formData.apiName);
            },
            onQuery: function () {
                //this.getGridData();
                //console.log(this.currentData.title);
                //this.classData = { };
                this.currentData.title = "添加分类";
                this.currentData.ID = -1;
                this.currentData.name = "";
                this.currentData.show = true;
                this.$nextTick(() => {
                    this.$refs["classData"].resetFields();
                })
            },
            indexMethod: function (index) {
                return index + 1;
            },
            tabOnLoad: function (iframeObj) {
                //console.log(iframeObj);
                //setTimeout(function () {
                //    if (!iframeObj)
                //        return;
                //    iframeObj.height = 1000;// iframeObj.document.body.offsetHeight;//(iframeObj.Document ? iframeObj.document.body.clientHeight : iframeObj.contentDocument.body.offsetHeight);
                //}, 200)
            },

            openMenuItemApi: function (item) {
                var that = this;
                that.frmMainSrc = "/html/component.html?anno_component_name=apiContent&id=" + item.Id;
                //console.log("openMenuItemApis=" + item.Id);
            },

            mouseover(data) { // 移入
                //if (data.Level == 0) {
                //console.log(data, data.dropdownShow)
                data.dropdownShow = true;
                //}
            },
            mouseout(data) { // 移除
                //if (data.Level == 0) {
                //console.log(data, data.dropdownShow)
                data.dropdownShow = false;
                //}
            },
            handleNodeClick(data) {
                var that = this;
                //console.log(data);
                if (data.Level == 0) {
                    that.headInfo = data.Title + " 共(" + data.children.length + ")个";
                    that.frmMainSrc = "/html/component.html?anno_component_name=apiList&name=" + data.Id + "&headInfo=" + escape(that.headInfo);
                    //console.log(that.headInfo);
                } else {
                    that.openMenuItemApi(data);
                }
            },
            addClass: function (formName) {
                var that = this;
                this.$refs[formName].validate((valid, object) => {
                    if (valid) {
                        this.currentData.show = false;
                        var input = {};
                        input.Title = this.classData.Title;
                        input.Namespace = this.classData.Namespace;
                        input.Name = this.classData.Name;
                        input.RoutePrefix = this.classData.RoutePrefix;
                        input.DefaultDBName = this.classData.DefaultDBName;
                        input.Remark = this.classData.Remark;
                        SaveController(that, input, true, "","保存", function(res) {
                            that.getControllermenus();
                        });
                    } else {
                        that.$message.error("请填写信息完整!");
                        return false;
                    }
                });
            },
            getConnectionData: function () {
                var that = this;

                GetConnectionStrings(that, "", function (data) {
                    that.ConnectionData = data;
                })
            },
            addType(data) {
                //alert('添加')
                this.ApiInfo.Title = data.label;
                this.ApiInfo.Id = data.Id;
                this.ApiInfo.Namespace = "";
                this.ApiInfo.Name = data.Name;
                this.ApiInfo.RoutePrefix = data.RoutePrefix;
                this.ApiInfo.DefaultDBName = data.DefaultDBName;
                this.ApiInfo.Remark = data.Remark;
                this.ApiInfo.show = true;
            },
            editType(data) {
                //alert('修改')
                //console.log(data);
                this.classData.Title = data.label;
                this.classData.Namespace = "";
                this.classData.Name = data.Name;
                this.classData.RoutePrefix = data.RoutePrefix;
                this.classData.DefaultDBName = data.DefaultDBName;
                this.classData.Remark = data.Remark;
                this.currentData.title = "修改分类";
                this.currentData.ID = -1;
                this.currentData.name = "";
                this.currentData.show = true;

            },
            delType(datarow) {
                var that = this;
                this.$confirm("确定要删除【" + datarow.label + "】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    DelController(that, datarow.Name, function (res) {
                        if (res) {
                            const index = that.menuroot.findIndex(ele => ele?.Id === datarow.Id);
                            if (index > -1) {
                                that.menuroot.splice(index, 1);
                            }
                        }
                    });
                }).catch(() => {
                    this.$message({
                        type: 'info',
                        message: '已取消删除'
                    });
                });;
            },
            syncInterface(datarow, state) {
                var that = this;
                if (state == "1") {
                    InitsyncInterface(that, "当前上传接口(" + datarow.Name + ")", datarow.Id, "", true, "上 传");
                }
                else if (state == "2") {
                    InitsyncInterface(that, "当前下载接口(" + datarow.Name + ")", datarow.Id, "", false, "下 载");
                }
                else if (state == "3"){
                    this.$confirm("确定要删除接口【" + datarow.Name + "】", "提示", {
                        confirmButtonText: "确定",
                        cancelButtonText: "取消",
                    }).then(function (rlt) {
                        DelApiInfo(that, datarow.PId, datarow.Name, function (res) {
                            if (res) {
                                for (var i = 0; i < that.menuroot.length; i++) {
                                    const index = that.menuroot[i].children.findIndex(ele => ele?.Id === datarow.Id);
                                    if (index > -1) {
                                        that.menuroot[i].children.splice(index, 1);
                                        that.handleNodeClick(that.menuroot[i]);
                                        break;
                                    }
                                }
                            }
                        });
                    }).catch(() => {
                        this.$message({
                            type: 'info',
                            message: '已取消删除'
                        });
                    });
                }
                else if (state == "4") {
                    InitsyncInterface(that, "当前上传分类(" + datarow.Name + ")", datarow.Id, datarow.Name, true, "上 传");
                }
                else if (state == "5") {
                    InitsyncInterface(that, "当前下载分类(" + datarow.Name + ")", datarow.Id, datarow.Name, false, "下 载");
                }
            }

        },
        components: {//<!--命名全部小写、或者下划线隔开，【中划线或驼峰会报错】  -- >
            'el-dialogbar': 'url:/component/el-dialogBar.vue',
            'el-syncinterface': 'url:/component/el-syncInterface.vue',
        }

    };
</script>
<style>
    .pc_menum {
        height: 100%;
        position: relative;
        /*overflow: hidden;*/
        overflow-x: hidden;
        overflow-y: scroll;
        height: calc(100% - 56px);
    }

    .mobile_menum {
        height: 100%;
        position: relative;
        /*overflow: hidden;*/
        overflow-x: hidden;
        overflow-y: scroll;
        height: calc(100% - 55px);
    }

    .el-tabs__header {
        padding-left: 20px;
    }

    .el-tabs__content {
        height: calc(100% - 13px);
    }

    .el-tab-pane {
        height: 100%;
    }

    html, body {
        height: 100%;
        margin: 0px;
        padding: 0px;
        margin: 0px 0px;
    }

    div[uInfo] {
        margin-left: 10px;
    }

    .el-header {
        padding: 0 0px;
        background-color: #fff;
        color: #333;
        left: 20px;
        line-height: 60px;
        border-bottom: 1px solid #e6e6e6;
        /*box-shadow: 0px 5px 2px rgba(0,21,41,.35);*/
    }

    .el-aside {
        background-color: #D3DCE6;
        color: #333;
        /*overflow: hidden;*/
        /*width:200px;*/
        /*box-shadow: 2px 0 6px rgba(0,21,41,.35);*/
        width: 350px !important;
    }

    .el-submenu__icon-arrow {
        right: 40px;
    }

    .el-menu200px {
        border-right: solid 0px #e6e6e6;
        /*width: 220px;*/
        height: 100%;
        position: absolute;
        overflow-x: hidden;
        overflow-y: scroll;
    }

    .el-menu64px {
        border-right: solid 0px #e6e6e6;
        /*width: 220px;*/
        height: 100%;
    }

    .el-container {
        height: 100%;
    }

    .el-main {
        color: #333;
        text-align: center;
        padding: 0 0;
        overflow-y: hidden;
        /*margin-left: 8px;*/
    }

    .el-tabs__header {
        margin: 0px;
    }

    .el-submenu .el-menu-item {
        height: 35px;
        line-height: 35px;
    }

    .tree-wrappper {
        min-height: 500px;
        overflow-y: hidden;
    }

    .btns {
        /*background-color: #eef7fe;*/
        position: absolute;
        /* top: 50%;*/
        right: 0;
        /* transform: translateY(-60%);*/
        /*transition: all 0.2s;*/
    }

    .el-tree-node__content {
        height: 30px;
        border-top: 0px solid #ebebeb;
    }
</style>