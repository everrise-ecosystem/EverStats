﻿// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class BlockchainSample
{
    public BlockchainSample() { }
    public BlockchainSample(ChainData data) {
        date = data.date.ToString("yyyy-MM-ddTHH:mm:ss");
        lastStored = date;

        reservesTokenBalanceValue = data.reservesTokenBalance;
        reservesCoinBalanceValue = data.reservesCoinBalance;
        liquidityTokenValue = data.liquidityToken;
        liquidityCoinValue = data.liquidityCoin;
        veAmountValue = data.veAmount;
        stakedValue = data.staked;
        aveMultiplierValue = data.aveMultiplier;
        rewardsValue = data.rewards;
        volumeBuyValue = data.volumeBuy;
        volumeSellValue = data.volumeSell;
        volumeTradeValue = data.volumeTrade;
        bridgeVaultValue = data.bridgeVault;
        tokenPriceCoinValue = data.tokenPriceCoin;
        coinPriceStableValue = data.coinPriceStable;
        tokenPriceStableValue = data.tokenPriceStable;
        marketCapValue = data.marketCap;
        blockNumberValue = data.blockNumber;
        holdersValue = data.holders;
        burnValue = data.burn;
        burnPercentValue = data.burnPercent;
        totalSupplyValue = data.totalSupply;
        everSwapValue = data.everSwap;

        usdReservesTokenBalanceValue = data.usdReservesTokenBalance;
        usdReservesCoinBalanceValue = data.usdReservesCoinBalance;
        usdReservesBalanceValue = data.usdReservesBalance;
        usdLiquidityTokenValue = data.usdLiquidityToken;
        usdLiquidityCoinValue = data.usdLiquidityCoin;
        usdStakedValue = data.usdStaked;
        usdRewardsValue = data.usdRewards;
        usdVolumeBuyValue = data.usdVolumeBuy;
        usdVolumeSellValue = data.usdVolumeSell;
        usdVolumeTradeValue = data.usdVolumeTrade;
        usdEverSwapValue = data.usdEverSwap;

        supplyOnChainPercentValue = data.supplyOnChainPercent;
        stakedOfTotalSupplyPercentValue = data.stakedOfTotalSupplyPercent;
        stakedOfOnChainPercentValue = data.stakedOfOnChainPercent;
        stakedOfTotalStakedPercentValue = data.stakedOfTotalStakedPercent;
        veRiseOnChainPercentValue = data.veRiseOnChainPercent;
        unclaimedTokenBalanceValue = data.unclaimedTokenBalance;
        usdUnclaimedTokenBalanceValue = data.usdUnclaimedTokenBalance;
        stakesCountValue = data.stakesCount;
        mementosCountValue = data.mementosCount;
    }

    public string? date { get; set; }
    public string? lastStored = null;

    public decimal reservesTokenBalanceValue;
    public decimal reservesCoinBalanceValue;
    public decimal liquidityTokenValue;
    public decimal liquidityCoinValue;
    public decimal veAmountValue;
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
    public decimal timeStampValue;
    public decimal holdersValue;
    public decimal burnValue;
    public decimal burnPercentValue;
    public decimal totalSupplyValue;
    public decimal everSwapValue;

    public decimal usdReservesTokenBalanceValue;
    public decimal usdReservesCoinBalanceValue;
    public decimal usdReservesBalanceValue;
    public decimal usdLiquidityTokenValue;
    public decimal usdLiquidityCoinValue;
    public decimal usdStakedValue;
    public decimal usdRewardsValue;
    public decimal usdVolumeTransfersValue;
    public decimal usdVolumeBuyValue;
    public decimal usdVolumeSellValue;
    public decimal usdVolumeTradeValue;
    public decimal usdEverSwapValue;

    public decimal supplyOnChainPercentValue;
    public decimal stakedOfTotalSupplyPercentValue;
    public decimal stakedOfOnChainPercentValue;
    public decimal stakedOfTotalStakedPercentValue;
    public decimal veRiseOnChainPercentValue;
    public decimal unclaimedTokenBalanceValue;
    public decimal usdUnclaimedTokenBalanceValue;
    public decimal stakesCountValue;
    public decimal mementosCountValue;

#pragma warning disable IDE1006 // Naming Styles
#nullable disable
    public string reservesCoinBalance { get; set; }
    public string reservesTokenBalance { get; set; }
    public string liquidityToken { get; set; }
    public string liquidityCoin { get; set; }
    public string veAmount { get; set; }
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
    public string burn { get; set; }
    public string burnPercent { get; set; }
    public string totalSupply { get; set; }
    public string everSwap { get; set; }

    public string usdReservesCoinBalance { get; set; }
    public string usdReservesTokenBalance { get; set; }
    public string usdReservesBalance { get; set; }
    public string usdLiquidityToken { get; set; }
    public string usdLiquidityCoin { get; set; }
    public string usdStaked { get; set; }
    public string usdRewards { get; set; }
    public string usdVolumeTransfers { get; set; }
    public string usdVolumeBuy { get; set; }
    public string usdVolumeSell { get; set; }
    public string usdVolumeTrade { get; set; }
    public string usdEverSwap { get; set; }

    public string supplyOnChainPercent { get; set; }
    public string stakedOfTotalSupplyPercent { get; set; }
    public string stakedOfOnChainPercent { get; set; }
    public string stakedOfTotalStakedPercent { get; set; }
    public string veRiseOnChainPercent { get; set; }
    public string unclaimedTokenBalance { get; set; }
    public string usdUnclaimedTokenBalance { get; set; }
    public string stakesCount { get; set; }
    public string mementosCount { get; set; }
#nullable enable
#pragma warning restore IDE1006 // Naming Styles


    public void CreateStringRepresentations()
    {
        reservesTokenBalance = reservesTokenBalanceValue.ToString(reservesTokenBalanceValue >= 0 ? "0.00000000" : "0");
        reservesCoinBalance = reservesCoinBalanceValue.ToString(reservesCoinBalanceValue >= 0 ? "0.00000000" : "0");
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
        veAmount = veAmountValue.ToString("0.00000000");
        burn = burnValue.ToString("0.00000000");
        burnPercent = burnPercentValue.ToString("0.00000000");
        totalSupply = totalSupplyValue.ToString("0.00000000");
        everSwap = everSwapValue.ToString("0.00000000");

        usdReservesCoinBalance = usdReservesCoinBalanceValue.ToString("0.00000000");
        usdReservesTokenBalance = usdReservesTokenBalanceValue.ToString("0.00000000");
        usdReservesBalance = usdReservesBalanceValue.ToString("0.00000000");
        usdLiquidityToken = usdLiquidityTokenValue.ToString("0.00000000");
        usdLiquidityCoin = usdLiquidityCoinValue.ToString("0.00000000");
        usdStaked = usdStakedValue.ToString("0.00000000");
        usdRewards = usdRewardsValue.ToString("0.00000000");
        usdVolumeTransfers = usdVolumeTransfersValue.ToString("0.00000000");
        usdVolumeBuy = usdVolumeBuyValue.ToString("0.00000000");
        usdVolumeSell = usdVolumeSellValue.ToString("0.00000000");
        usdVolumeTrade = usdVolumeTradeValue.ToString("0.00000000");
        usdEverSwap = usdEverSwapValue.ToString("0.00000000");

        supplyOnChainPercent = supplyOnChainPercentValue.ToString("0.00000000");
        stakedOfTotalSupplyPercent = stakedOfTotalSupplyPercentValue.ToString("0.00000000");
        stakedOfOnChainPercent = stakedOfOnChainPercentValue.ToString("0.00000000");
        stakedOfTotalStakedPercent = stakedOfTotalStakedPercentValue.ToString("0.00000000");
        veRiseOnChainPercent = veRiseOnChainPercentValue.ToString("0.00000000");

        unclaimedTokenBalance = unclaimedTokenBalanceValue.ToString("0.00000000");
        usdUnclaimedTokenBalance = usdUnclaimedTokenBalanceValue.ToString("0.00000000");

        stakesCount = stakesCountValue.ToString("0.00000000");
        mementosCount = mementosCountValue.ToString("0.00000000");
    }
}
