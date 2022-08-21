namespace EverStats.Data
{
    public class ChainData
    {
        public int entry { get; set; }
        public DateOnly dateOnly { get; set; }
        public DateTime date { get; set; }
        public int chainId { get; set; }
        public decimal reservesCoinBalance { get; set; }
        public decimal reservesTokenBalance { get; set; }
        public decimal liquidityToken { get; set; }
        public decimal liquidityCoin { get; set; }
        public decimal veAmount { get; set; }
        public decimal staked { get; set; }
        public decimal aveMultiplier { get; set; }
        public decimal rewards { get; set; }
        public decimal volumeBuy { get; set; }
        public decimal volumeSell { get; set; }
        public decimal volumeTrade { get; set; }
        public decimal bridgeVault { get; set; }
        public decimal tokenPriceCoin { get; set; }
        public decimal coinPriceStable { get; set; }
        public decimal tokenPriceStable { get; set; }
        public decimal marketCap { get; set; }
        public decimal blockNumber { get; set; }
        public decimal holders { get; set; }
        public decimal burn { get; set; }
        public decimal burnPercent { get; set; }
        public decimal totalSupply { get; set; }
        public decimal everSwap { get; set; }
        public decimal usdReservesCoinBalance { get; set; }
        public decimal usdReservesTokenBalance { get; set; }
        public decimal usdReservesBalance { get; set; }
        public decimal usdLiquidityToken { get; set; }
        public decimal usdLiquidityCoin { get; set; }
        public decimal usdStaked { get; set; }
        public decimal usdRewards { get; set; }
        public decimal usdVolumeBuy { get; set; }
        public decimal usdVolumeSell { get; set; }
        public decimal usdVolumeTrade { get; set; }
        public decimal usdEverSwap { get; set; }
        public decimal supplyOnChainPercent { get; set; }
        public decimal stakedOfTotalSupplyPercent { get; set; }
        public decimal stakedOfOnChainPercent { get; set; }
        public decimal stakedOfTotalStakedPercent { get; set; }
        public decimal veRiseOnChainPercent { get; set; }
    }
}
