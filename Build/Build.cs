using Nuke.Common;
using Nuke.Common.Execution;
using ricaun.Nuke;

class Build : NukeBuild, IPublishPack, ITest
{
    public static int Main() => Execute<Build>(x => x.From<IPublishPack>().Build);
}
