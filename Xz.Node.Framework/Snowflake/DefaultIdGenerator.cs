using System;
using System.Threading;
using Xz.Node.Framework.Snowflake.Contract;
using Xz.Node.Framework.Snowflake.Core;

namespace Xz.Node.Framework.Snowflake
{
    /// <summary>
    /// 默认实现
    /// </summary>
    public class DefaultIdGenerator : IIdGenerator
    {
        private ISnowWorker _SnowWorker { get; set; }

        public Action<OverCostActionArg> GenIdActionAsync
        {
            get => _SnowWorker.GenAction;
            set => _SnowWorker.GenAction = value;
        }

        public DefaultIdGenerator(IdGeneratorOptions options)
        {
            if (options == null)
            {
                throw new ApplicationException("options error.");
            }

            if (options.BaseTime < DateTime.Now.AddYears(-50) || options.BaseTime > DateTime.Now)
            {
                throw new ApplicationException("BaseTime error.");
            }

            if (options.WorkerIdBitLength <= 0)
            {
                throw new ApplicationException("WorkerIdBitLength error.(range:[1, 21])");
            }
            if (options.SeqBitLength + options.WorkerIdBitLength > 22)
            {
                throw new ApplicationException("error：WorkerIdBitLength + SeqBitLength <= 22");
            }

            var maxWorkerIdNumber = (1 << options.WorkerIdBitLength) - 1;
            if (options.WorkerId < 0 || options.WorkerId > maxWorkerIdNumber)
            {
                throw new ApplicationException("WorkerId error. (range:[0, " + (maxWorkerIdNumber > 0 ? maxWorkerIdNumber : 63) + "]");
            }

            if (options.SeqBitLength < 2 || options.SeqBitLength > 21)
            {
                throw new ApplicationException("SeqBitLength error. (range:[2, 21])");
            }

            var maxSeqNumber = (1 << options.SeqBitLength) - 1;
            if (options.MaxSeqNumber < 0 || options.MaxSeqNumber > maxSeqNumber)
            {
                throw new ApplicationException("MaxSeqNumber error. (range:[1, " + maxSeqNumber + "]");
            }

            var maxValue = maxSeqNumber;
            if (options.MinSeqNumber < 1 || options.MinSeqNumber > maxValue)
            {
                throw new ApplicationException("MinSeqNumber error. (range:[1, " + maxValue + "]");
            }

            switch (options.Method)
            {
                case 1:
                    _SnowWorker = new SnowWorkerM1(options);
                    break;
                case 2:
                    _SnowWorker = new SnowWorkerM2(options);
                    break;
                default:
                    _SnowWorker = new SnowWorkerM1(options);
                    break;
            }

            if (options.Method == 1)
            {
                Thread.Sleep(500);
            }
        }

        public long NewLong()
        {
            return _SnowWorker.NextId();
        }
    }
}
