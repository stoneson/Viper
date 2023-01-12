<template>
    <div id="layout1" class="l-layout" style="height: 100%;">
        <div class="l-layout-center" style="width:60%; left:1px; top: 1px;">
            
                <div>
                    <el-form ref="form" size="mini">
                        <el-form-item>
                            <!--连接名称：-->
                            <el-input style="width: 20%"
                                      prefix-icon="el-icon-search"
                                      placeholder="连接名称"
                                      v-model="formData.ConnectionName"></el-input>
                            <el-button type="info" v-on:keyup.enter="onQuery" @click="onQuery">查询</el-button>
                            <!--<el-button icon="el-icon-plus" type="success" @click="openAddConnection">增加连接</el-button>-->
                        </el-form-item>
                    </el-form>
                    <el-container style="height: 100%; border: 1px solid #eee">
                        <el-table :data="ConnectionData"
                                  border
                                  stripe
                                  @row-click="handleCurrentChange"
                                  size="mini"
                                  :max-height="window.innerHeight-68"
                                  style="width: 100%">
                            <el-table-column type="index" fixed :index="indexMethod">
                            </el-table-column>
                            <el-table-column fixed prop="Name" show-overflow-tooltip label="连接名称" width="130">
                            </el-table-column>

                            <el-table-column fixed prop="ProviderName" show-overflow-tooltip label="连接驱动" width="160">
                            </el-table-column>

                            <el-table-column prop="ConnectionString" show-overflow-tooltip label="连接字符串">
                            </el-table-column>
                            <el-table-column prop="IsDefault"
                                             show-overflow-tooltip="true"
                                             label="默认连接"
                                             width="80">
                                <template slot-scope="scope">
                                    <el-tag :type="scope.row.IsDefault ? 'success' : 'danger'">
                                        {{
            scope.row.IsDefault ? "是" : "否"
                                        }}
                                    </el-tag>
                                </template>
                            </el-table-column>
                            <el-table-column fixed="right" align="center" width="100" label="操作">
                                <template slot-scope="scope">
                                    <el-button @click="deleteConnection(scope.row)" type="text" size="small">删除</el-button>
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-container>
                </div>
           
        </div>
        <div class="l-layout-right" style="width: 40%;min-width:250px; top: 1px;right:1px; position: fixed;">
           
            <el-container style="height: 100%; width: 98%; border: 1px solid #eee; margin-top: 55px; ">
                <el-form  :model="currentData" style="width: 90%; min-width: 250px; margin-top: 10px; ">
                    <el-form-item label="连接名称：" label-width="120px" >
                        <el-input v-model="currentData.Name" autocomplete="on" style="width:200px;"></el-input>
                    </el-form-item>
                    <el-form-item label="连接驱动：" label-width="120px">
                        <!--<el-input v-model="currentData.ProviderName" autocomplete="on"></el-input>-->
                        <el-select style="width:200px;" v-model="currentData.ProviderName" placeholder="请选择">
                            <el-option v-for="item in providerNames"
                                       :key="item.value"
                                       :label="item.Name"
                                       :value="item.value"
                                       :disabled="item.disabled">
                            </el-option>
                        </el-select>
                    </el-form-item>

                    <el-form-item label="连接字符串：" label-width="120px">
                        <el-input type="textarea" v-model="currentData.ConnectionString" autosize autocomplete="off"></el-input>
                    </el-form-item>
                    <el-form-item label="     "  label-width="120px">
                        <el-button type="success" @click="addConnection">保存</el-button>
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
                ConnectionData: [],
                formData: {
                    ConnectionName: "",
                },
                currentData: {
                    show: true,
                    title: "增加连接",
                    ID: -1,
                    Name: "",
                    ConnectionString: "",
                    ProviderName: "SqlServer",
                    IsDefault:false,
                },
                //total: 0,
                //currentPage: 1,
                //pagesize: 20,
                //pagesizes: [10, 15, 20, 30, 40],

                providerNames: [
                    { Name: 'SqlServer', value: 'SqlServer' },
                    { Name: 'Oracle', value: 'Oracle' },
                    { Name: 'MySql', value: 'MySql', disabled: true},
                ],
            };
        },
        created: function () {
            var that = this;

            //用于数据初始化
            document.title = "连接列表";
            this.keyupAnno();
        },
        mounted: function () {
            this.getConnectionData();
        },
        methods: {
            deleteConnection: function (row) {
                var that = this;
                this.$confirm("确定要删除连接【" + row.Name+"】", "提示", {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                }).then(function (rlt) {
                    var input = anno.getInput();
                    input.Name = row.Name;
                    anno.process(input, "Anno.DynamicApi/DynamicApi/DelConnectionString", function (data) {
                        if (data.status === true) {
                            that.onQuery();

                            that.$message({ message: '连接删除成功', type: 'success' });
                        } else {
                            that.$message.error(data.msg);
                        }
                    });
                });
            },
            onQuery: function () {
                this.getConnectionData();
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
            getConnectionData: function () {
                var that = this;
                //var page = that.currentPage;
                //var pagesize = that.pagesize;

                var input = anno.getInput();
                input.Name = that.formData.ConnectionName;
                //if (page !== null && page !== undefined) {
                //    input.page = page;
                //} else {
                //    input.page = 1;
                //}
                //if (pagesize !== null && pagesize !== undefined) {
                //    input.pagesize = pagesize;
                //} else {
                //    input.pagesize = 20;
                //}
                anno.process(input, "Anno.DynamicApi/DynamicApi/GetConnectionStrings", function (data) {
                    if (data.status === true) {
                        that.ConnectionData = data.outputData;
                        //that.total = data.outputData.Length;
                    } else {
                        that.$message.error(data.msg);
                    }
                });
            },
            handleCurrentChange: function (val) {
                //this.currentRow = val;
                this.currentData.Name = val.Name;
                this.currentData.ConnectionString = val.ConnectionString;
                this.currentData.ProviderName = val.ProviderName;
                this.currentData.IsDefault = val.IsDefault;
            },
            openAddConnection: function () {
                this.currentData.title = "增加连接";
                this.currentData.ID = -1;
                this.currentData.Name = "";
                this.currentData.ConnectionString = "";
                this.currentData.ProviderName = "SqlServer";
                this.currentData.IsDefault = false;

                this.currentData.show = true;
            },
            addConnection: function () {
                this.currentData.show = false;
                var that = this;
                var input = anno.getInput();
                input.Name = this.currentData.Name;
                input.ConnectionString = this.currentData.ConnectionString;
                input.ProviderName = this.currentData.ProviderName;
                input.IsDefault = this.currentData.IsDefault;
                anno.process(input, "Anno.DynamicApi/DynamicApi/SaveConnectionString", function (data) {
                    if (data.status === true) {
                        that.onQuery();

                        that.$message({ message: '连接更新成功', type: 'success' });
                    } else {
                        that.$message.error(data.msg);
                    }
                });
            },
        },
    };
</script>
<style scoped>
</style>