using System;

namespace Xz.Node.Framework.Snowflake.Contract
{
    internal interface ISnowWorker
    {
        Action<OverCostActionArg> GenAction { get; set; }

        long NextId();
    }
}
