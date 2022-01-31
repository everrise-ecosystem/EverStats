// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class BlockchainSample
{
    public string date { get; set; } = DateTime.UtcNow.ToString("s");

    public decimal reservesBalanceValue;
    public decimal liquidityTokenValue;
    public decimal liquidityCoinValue;
    public decimal stakedValue;
    public decimal aveMultiplierValue;
    public decimal rewardsValue;
    public decimal volumeTransfersValue;
    public decimal volumeBuyValue;
    public decimal volumeSellValue;
    public decimal volumeTradeValue;
    public decimal bridgeVaultValue;
    public decimal tokenPriceCoinValue;
    public decimal coinPriceStableValue;
    public decimal tokenPriceStableValue;
    public decimal marketCapValue;
    public decimal blockNumberValue;
    public decimal holdersValue;

    public decimal usdReservesBalanceValue;
    public decimal usdLiquidityTokenValue;
    public decimal usdLiquidityCoinValue;
    public decimal usdStakedValue;
    public decimal usdRewardsValue;
    public decimal usdVolumeTransfersValue;
    public decimal usdVolumeBuyValue;
    public decimal usdVolumeSellValue;
    public decimal usdVolumeTradeValue;

    public decimal supplyOnChainPercentValue;
    public decimal stakedOfTotalSupplyPercentValue;
    public decimal stakedOfOnChainPercentValue;
    public decimal stakedOfTotalStakedPercentValue;

#pragma warning disable IDE1006 // Naming Styles
#nullable disable
    public string reservesBalance { get; set; }
    public string liquidityToken { get; set; }
    public string liquidityCoin { get; set; }
    public string staked { get; set; }
    public string aveMultiplier { get; set; }
    public string rewards { get; set; }
    public string volumeTransfers { get; set; }
    public string volumeBuy { get; set; }
    public string volumeSell { get; set; }
    public string volumeTrade { get; set; }
    public string bridgeVault { get; set; }
    public string tokenPriceCoin { get; set; }
    public string coinPriceStable { get; set; }
    public string tokenPriceStable { get; set; }
    public string marketCap { get; set; }
    public string blockNumber { get; set; }
    public string holders { get; set; }

    public string usdReservesBalance { get; set; }
    public string usdLiquidityToken { get; set; }
    public string usdLiquidityCoin { get; set; }
    public string usdStaked { get; set; }
    public string usdRewards { get; set; }
    public string usdVolumeTransfers { get; set; }
    public string usdVolumeBuy { get; set; }
    public string usdVolumeSell { get; set; }
    public string usdVolumeTrade { get; set; }

    public string supplyOnChainPercent { get; set; }
    public string stakedOfTotalSupplyPercent { get; set; }
    public string stakedOfOnChainPercent { get; set; }
    public string stakedOfTotalStakedPercent { get; set; }
#nullable enable
#pragma warning restore IDE1006 // Naming Styles


    public void CreateStringRepresentations()
    {
        reservesBalance = reservesBalanceValue.ToString(reservesBalanceValue >= 0 ? "0.00000000" : "0");
        liquidityToken = liquidityTokenValue.ToString("0.00000000");
        liquidityCoin = liquidityCoinValue.ToString(liquidityCoinValue >= 0 ? "0.00000000" : "0");
        staked = stakedValue.ToString("0.00000000");
        aveMultiplier = aveMultiplierValue.ToString("0.00000000");
        rewards = rewardsValue.ToString("0.00000000");
        volumeTransfers = volumeTransfersValue.ToString("0.00000000");
        volumeBuy = volumeBuyValue.ToString("0.00000000");
        volumeSell = volumeSellValue.ToString("0.00000000");
        volumeTrade = volumeTradeValue.ToString("0.00000000");
        bridgeVault = bridgeVaultValue.ToString(bridgeVaultValue >= 0 ? "0.00000000" : "0");
        tokenPriceCoin = tokenPriceCoinValue.ToString(tokenPriceCoinValue >= 0 ? "0.0000000000000000" : "0");
        coinPriceStable = coinPriceStableValue.ToString(coinPriceStableValue >= 0 ? "0.00000000" : "0");
        tokenPriceStable = tokenPriceStableValue.ToString("0.00000000");
        marketCap = marketCapValue.ToString("0.00000000");
        blockNumber = blockNumberValue.ToString("0");
        holders = holdersValue.ToString("0");

        usdReservesBalance = usdReservesBalanceValue.ToString("0.00000000");
        usdLiquidityToken = usdLiquidityTokenValue.ToString("0.00000000");
        usdLiquidityCoin = usdLiquidityCoinValue.ToString("0.00000000");
        usdStaked = usdStakedValue.ToString("0.00000000");
        usdRewards = usdRewardsValue.ToString("0.00000000");
        usdVolumeTransfers = usdVolumeTransfersValue.ToString("0.00000000");
        usdVolumeBuy = usdVolumeBuyValue.ToString("0.00000000");
        usdVolumeSell = usdVolumeSellValue.ToString("0.00000000");
        usdVolumeTrade = usdVolumeTradeValue.ToString("0.00000000");

        supplyOnChainPercent = supplyOnChainPercentValue.ToString("0.00000000");
        stakedOfTotalSupplyPercent = stakedOfTotalSupplyPercentValue.ToString("0.00000000");
        stakedOfOnChainPercent = stakedOfOnChainPercentValue.ToString("0.00000000");
        stakedOfTotalStakedPercent = stakedOfTotalStakedPercentValue.ToString("0.00000000");
    }
}
