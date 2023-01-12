<template>
    <div id="layout1" class="l-layout" style="height: 100%;">
        <div class="l-layout-center" style="width:60%; left:1px; top: 1px;">

            <div>
                <el-form ref="form" size="mini">
                    <el-form-item>
                        <!--地址名称：-->
                        <el-input style="width: 20%"
                                  prefix-icon="el-icon-search"
                                  placeholder="地址名称"
                                  v-model="formData.urlName"></el-input>
                        <el-button type="info" v-on:keyup.enter="onQuery" @click="onQuery">查询</el-button>
                        <!--<el-button icon="el-icon-plus" type="success" @click="openAddUriInfo">增加连接</el-button>-->
                    </el-form-item>
                </el-form>
                <el-container style="height: 100%; border: 1px solid #eee">
                    <el-table :data="GridData"
                              border
                              stripe
                              @row-click="handleCurrentChange"
                              size="mini"
                              :max-height="window.innerHeight-68"
                              style="width: 100%">
                        <el-table-column type="index" fixed :index="indexMethod">
                        </el-table-column>
                        <el-table-column fixed prop="Name" show-overflow-tooltip label="地址名称" width="130">
                        </el-table-column>

                        <el-table-column fixed prop="Remark" show-overflow-tooltip label="备注" width="160">
                        </el-table-column>

                        <el-table-column prop="BaseUri" show-overflow-tooltip label="环境地址">
                        </el-table-column>

                        <el-table-column fixed="right" align="center" width="300" label="操作">
                            <template slot-scope="scope">
                                <!--<el-button @click="restart(scope.row)" type="text" size="small">重启服务</el-button>-->
                                <el-button @click="deleteUriInfo(scope.row)" type="text" size="small">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-container>
            </div>

        </div>
        <div class="l-layout-right" style="width: 40%;min-width:250px; top: 1px;right:1px; position: fixed;">

            <el-container style="height: 100%; width: 98%; border: 1px solid #eee; margin-top: 55px; ">
                <el-form :model="currentData" style="width: 90%; min-width: 250px; margin-top: 10px; ">
                    <el-form-item label="地址名称：" label-width="120px">
                        <el-input v-model="currentData.Name" autocomplete="on" style="width:200px;"></el-input>
                    </el-form-item>
                    <el-form-item label="环境地址：" label-width="120px">
                        <el-input  v-model="currentData.BaseUri"  autocomplete="off"></el-input>
                    </el-form-item>
                    <el-form-item label="备注：" label-width="120px">
                        <el-input type="textarea" v-model="currentData.Remark" autocomplete="on"></el-input>
                    </el-form-item>
                    <el-form-item label="     " label-width="120px">
                        <el-button type="success" @click="addUriInfo">保存</el-button>
                    </el-form-item>
                </el-form>
            </el-container>

        </div>
    </div>
</template>

<script>
    module.exports = {
        data: function () {
            return {
                GridData: [],
                formData: {
                    urlName: "",
                },
                currentData: {
                    show: true,
                    title: "增加地址",
                    ID: -1,
                    Name: "",
                    BaseUri: "",
                    Remark: "",
                },
                //total: 0,
                //currentPage: 1,
                //pagesize: 20,
                //pagesizes: [10, 15, 20, 30, 40],

            };
        },
        created: function () {
            var that = this;
            //console.log("环境");
            //用于数据初始化
            document.title = "外部地址列表";
            this.keyupAnno();
        },
        mounted: function () {
            this.getGridData();
        },
        methods: {
            deleteUriInfo: function (row) {
                var that = this;
                this.$confirm("确定要删除【" + row.Name+"】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    DelSetingUriInfo(that, row.Name, function (res) {
                        if (res) {
                            that.onQuery();
                        }
                    });
                });
            },
            onQuery: function () {
                this.getGridData();
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
            getGridData: function () {
                var that = this;
                GetSetingUriInfo(that, that.formData.urlName, function (data) {
                    that.GridData = data;
                });
            },
            handleCurrentChange: function (val) {
                //this.currentRow = val;
                this.currentData.Name = val.Name;
                this.currentData.BaseUri = val.BaseUri;
                this.currentData.Remark = val.Remark;
            },
            openAddUriInfo: function () {
                this.currentData.title = "增加连接";
                this.currentData.ID = -1;
                this.currentData.Name = "";
                this.currentData.BaseUri = "";
                this.currentData.Remark = "";
                this.currentData.show = true;
            },
            addUriInfo: function () {
                this.currentData.show = false;
                var that = this;
                SaveSetingUriInfo(that, that.currentData.Name, that.currentData.BaseUri, that.currentData.Remark, function (res) {
                    if (res) {
                        that.onQuery();
                    }
                });
            },
            restart: function (row) {
                var that = this;
                that.$prompt('请输入KEY', '重启服务', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    inputPattern: /^.+$/,
                    inputErrorMessage: '请输入KEY'
                }).then(({ value }) => {
                    ReStartService(that, row.BaseUri, value, function (res) {
                    })
                }).catch(() => {
                    this.$message({
                        type: 'info',
                        message: '取消输入'
                    });
                });
                
            }
        },
    };
</script>
<style scoped>
</style>