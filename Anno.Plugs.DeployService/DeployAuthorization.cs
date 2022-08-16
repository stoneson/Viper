namespace Anno.Plugs.DeployService
{
    public class DeployAuthorization : EngineData.Filters.AuthorizationFilterAttribute
    {
        // Token: 0x06000021 RID: 33 RVA: 0x00002E54 File Offset: 0x00001054
        public override void OnAuthorization(EngineData.BaseModule context)
        {
            if (context.RequestString("deploySecret") != null && !context.RequestString("deploySecret").Equals(Const.CustomConfiguration.Settings["deploySecret"]))
            {
                context.Authorized = false;
                context.ActionResult = new EngineData.ActionResult(false, null, null, "部署口令错误！");
                return;
            }
            context.Authorized = true;
        }
    }
}
