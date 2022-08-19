using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Contracts.Contracts.EverRise.ContractDefinition
{
    public partial class ApprovalChecks : ApprovalChecksBase { }

    public class ApprovalChecksBase 
    {
        [Parameter("uint64", "nonce", 1)]
        public virtual ulong Nonce { get; set; }
        [Parameter("uint32", "nftCheck", 2)]
        public virtual uint NftCheck { get; set; }
        [Parameter("uint32", "tokenCheck", 3)]
        public virtual uint TokenCheck { get; set; }
        [Parameter("uint16", "autoRevokeNftHours", 4)]
        public virtual ushort AutoRevokeNftHours { get; set; }
        [Parameter("uint16", "autoRevokeTokenHours", 5)]
        public virtual ushort AutoRevokeTokenHours { get; set; }
        [Parameter("uint48", "unlockTimestamp", 6)]
        public virtual ulong UnlockTimestamp { get; set; }
    }
}
