using VOXO.GISSM.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace VOXO.GISSM
{
    public class GISSMPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            //var moduleGroup = context.AddGroup(GISSMPermissions.GroupName, L("Permission:GISSM"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<GISSMResource>(name);
        }
    }
}