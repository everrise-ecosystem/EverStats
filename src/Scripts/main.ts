import { Big } from 'big.js';

interface Value {
    current: Big;
    interpolated: Big;
    previous: Big;
}
interface Sample {
    now: Value;
    yesterday: Value;
}
interface Stat {
    sample: Sample;
    precision: number;
    values: HTMLSpanElement[];
    change24hr: HTMLSpanElement[];
    valuesCssUpdated: boolean
}

interface PeriodData {
    date:string;
    reservesBalance:string;
    liquidityToken:string;
    liquidityCoin:string;
    staked:string;
    aveMultiplier:string;
    rewards:string;
    volumeTransfers:string;
    volumeBuy:string;
    volumeSell:string;
    volumeTrade:string;
    tokenPriceCoin:string;
    coinPriceStable:string;
    tokenPriceStable:string;
    marketCap:string;
    blockNumber:string;
    holders:string;
    usdReservesBalance:string;
    usdLiquidityToken:string;
    usdLiquidityCoin:string;
    usdStaked:string;
    usdRewards:string;
    usdVolumeTransfers:string;
    usdVolumeBuy:string;
    usdVolumeSell:string;
    usdVolumeTrade:string;
    supplyOnChainPercent:string;
    stakedOfTotalSupplyPercent:string;
    stakedOfOnChainPercent:string;
    stakedOfTotalStakedPercent:string;
}

interface ChainData {
    current:PeriodData;
    history24hrs: PeriodData;
    history48hrs: PeriodData;
}

interface BlockchainData {
    unified: ChainData;
    bsc: ChainData;
    eth: ChainData;
    poly: ChainData;
    ftm: ChainData;
    avax: ChainData;
}

interface Data {
    blockNumber: {
        current: number,
        history24hr: number,
    };
    holders: Stat;
    supplyOnChain: Stat;

    tokenPriceUSDT: Stat;
    tokenPriceCoin: Stat;
    coinPriceUSDT: Stat;

    liquidityToken: Stat;
    liquidityCoin: Stat;
    liquidityCoinUSDT: Stat;

    reservesCoin: Stat;
    reservesCoinUSDT: Stat;
    
    marketCapUSDT: Stat;
    liquidityPercent: Stat;
    reservesPercent: Stat;

    volumeTransfers: Stat;
    volumeTransfersUSDT: Stat;
    volumeBuy: Stat;
    volumeBuyUSDT: Stat;
    volumeSell: Stat;
    volumeSellUSDT: Stat;
    volumeTrade: Stat;
    volumeTradeUSDT: Stat;

    volumeTransfers24hr: Stat;
    volumeTransfersUSDT24hr: Stat;
    volumeBuy24hr: Stat;
    volumeBuyUSDT24hr: Stat;
    volumeSell24hr: Stat;
    volumeSellUSDT24hr: Stat;
    volumeTrade24hr: Stat;
    volumeTradeUSDT24hr: Stat;

    volumeTransfers7day: Stat;
    volumeTransfersUSDT7day: Stat;
    volumeBuy7day: Stat;
    volumeBuyUSDT7day: Stat;
    volumeSell7day: Stat;
    volumeSellUSDT7day: Stat;
    volumeTrade7day: Stat;
    volumeTradeUSDT7day: Stat;

    stakedToken: Stat;
    stakedUSDT: Stat;
    stakedPercent: Stat;
    stakedMultiplier: Stat;
    stakedOnChain: Stat;

    rewardsToken: Stat;
    rewardsToken24hr: Stat;
    rewardsToken7day: Stat;
    rewardsUSDT: Stat;
    rewardsUSDT24hr: Stat;
    rewardsUSDT7day: Stat;

    [Symbol.iterator]();
}

class Coin {
    public static readonly queryFrequency = 30;
    private supply = Big("71618033988");
    public network: string;
    public data: Data;
    public times = {
        timeCurrent: 0,
        timeNow: 0
    };

    constructor(network: string) {
        this.network = network;

        this.data = Coin.CreateStats(this.network);
    }

    public static CreateStats(network: string){
        const data = {
            blockNumber: {
                current: -1,
                history24hr: -1,
            },
            holders: Coin.CreateStat(`.ticker-${network}-holders`, 0),

            tokenPriceUSDT: Coin.CreateStat(`.ticker-${network}-token-price-usdt`, 8),
            tokenPriceCoin: Coin.CreateStat(`.ticker-${network}-token-price-coin`, 10),
            coinPriceUSDT: Coin.CreateStat(`.ticker-${network}-coin-price-usdt`, 3),

            liquidityToken: Coin.CreateStat(`.ticker-${network}-token-liquidity`, 2),
            liquidityCoin: Coin.CreateStat(`.ticker-${network}-coin-liquidity`, 2),
            liquidityCoinUSDT: Coin.CreateStat(`.ticker-${network}-coin-liquidity-usdt`, 2),
            liquidityPercent: Coin.CreateStat(`.ticker-${network}-liquidity-percent`, 4),

            reservesCoin: Coin.CreateStat(`.ticker-${network}-coin-reserves`, 2),
            reservesCoinUSDT: Coin.CreateStat(`.ticker-${network}-coin-reserves-usdt`, 2),
            reservesPercent: Coin.CreateStat(`.ticker-${network}-coin-reserves-percent`, 4),
            
            marketCapUSDT: Coin.CreateStat(`.ticker-${network}-market-cap`, 2),

            volumeTransfers: Coin.CreateStat(`.ticker-${network}-vol-txns`, 2),
            volumeTransfersUSDT: Coin.CreateStat(`.ticker-${network}-vol-txns-usdt`, 2),
            volumeBuy: Coin.CreateStat(`.ticker-${network}-vol-buy`, 2),
            volumeBuyUSDT: Coin.CreateStat(`.ticker-${network}-vol-buy-usdt`, 2),
            volumeSell: Coin.CreateStat(`.ticker-${network}-vol-sell`, 2),
            volumeSellUSDT: Coin.CreateStat(`.ticker-${network}-vol-sell-usdt`, 2),
            volumeTrade: Coin.CreateStat(`.ticker-${network}-vol-trade`, 2),
            volumeTradeUSDT: Coin.CreateStat(`.ticker-${network}-vol-trade-usdt`, 2),

            volumeTransfers24hr: Coin.CreateStat(`.ticker-${network}-vol-txns-24hr`, 2),
            volumeTransfersUSDT24hr: Coin.CreateStat(`.ticker-${network}-vol-txns-usdt-24hr`, 2),
            volumeBuy24hr: Coin.CreateStat(`.ticker-${network}-vol-buy-24hr`, 2),
            volumeBuyUSDT24hr: Coin.CreateStat(`.ticker-${network}-vol-buy-usdt-24hr`, 2),
            volumeSell24hr: Coin.CreateStat(`.ticker-${network}-vol-sell-24hr`, 2),
            volumeSellUSDT24hr: Coin.CreateStat(`.ticker-${network}-vol-sell-usdt-24hr`, 2),
            volumeTrade24hr: Coin.CreateStat(`.ticker-${network}-vol-trade-24hr`, 2),
            volumeTradeUSDT24hr: Coin.CreateStat(`.ticker-${network}-vol-trade-usdt-24hr`, 2),


            volumeTransfers7day: Coin.CreateStat(`.ticker-${network}-vol-txns-7day`, 2),
            volumeTransfersUSDT7day: Coin.CreateStat(`.ticker-${network}-vol-txns-usdt-7day`, 2),
            volumeBuy7day: Coin.CreateStat(`.ticker-${network}-vol-buy-7day`, 2),
            volumeBuyUSDT7day: Coin.CreateStat(`.ticker-${network}-vol-buy-usdt-7day`, 2),
            volumeSell7day: Coin.CreateStat(`.ticker-${network}-vol-sell-7day`, 2),
            volumeSellUSDT7day: Coin.CreateStat(`.ticker-${network}-vol-sell-usdt-7day`, 2),
            volumeTrade7day: Coin.CreateStat(`.ticker-${network}-vol-trade-7day`, 2),
            volumeTradeUSDT7day: Coin.CreateStat(`.ticker-${network}-vol-trade-usdt-7day`, 2),

            stakedToken: Coin.CreateStat(`.ticker-${network}-staked-token`, 2),
            stakedUSDT: Coin.CreateStat(`.ticker-${network}-staked-usdt`, 2),
            stakedPercent: Coin.CreateStat(`.ticker-${network}-staked-percent`, 4),
            stakedMultiplier: Coin.CreateStat(`.ticker-${network}-staked-multiplier`, 4),
            supplyOnChain: Coin.CreateStat(`.ticker-${network}-supply-percent`, 4),
            stakedOnChain: Coin.CreateStat(`.ticker-${network}-staked-onchain-percent`, 4),

            rewardsToken: Coin.CreateStat(`.ticker-${network}-rewards-token`, 2),
            rewardsToken24hr: Coin.CreateStat(`.ticker-${network}-rewards-token-24hr`, 2),
            rewardsToken7day: Coin.CreateStat(`.ticker-${network}-rewards-token-7day`, 2),
            rewardsUSDT: Coin.CreateStat(`.ticker-${network}-rewards-usdt`, 2),
            rewardsUSDT24hr: Coin.CreateStat(`.ticker-${network}-rewards-usdt-24hr`, 2),
            rewardsUSDT7day: Coin.CreateStat(`.ticker-${network}-rewards-usdt-7day`, 2),

            *[Symbol.iterator]() {
                yield data.coinPriceUSDT;
                yield data.holders;


                yield data.liquidityToken;
                yield data.liquidityCoin;

                yield data.reservesCoin;

                yield data.volumeTransfers;
                yield data.volumeBuy;
                yield data.volumeSell;
                yield data.volumeTrade;
                yield data.volumeSellUSDT;
                yield data.volumeBuyUSDT;
                yield data.volumeTransfersUSDT;
                yield data.volumeTradeUSDT;

                yield data.volumeTransfers24hr;
                yield data.volumeBuy24hr;
                yield data.volumeSell24hr;
                yield data.volumeTrade24hr;
                yield data.volumeSellUSDT24hr;
                yield data.volumeBuyUSDT24hr;
                yield data.volumeTransfersUSDT24hr;
                yield data.volumeTradeUSDT24hr;

                yield data.volumeTransfers7day;
                yield data.volumeBuy7day;
                yield data.volumeSell7day;
                yield data.volumeTrade7day;
                yield data.volumeSellUSDT7day;
                yield data.volumeBuyUSDT7day;
                yield data.volumeTransfersUSDT7day;
                yield data.volumeTradeUSDT7day;

                yield data.stakedToken;
                yield data.rewardsToken;
                yield data.rewardsToken24hr;
                yield data.rewardsToken7day;

                yield data.tokenPriceCoin;
                yield data.tokenPriceUSDT;
                yield data.marketCapUSDT;
                yield data.liquidityPercent;
                yield data.liquidityCoinUSDT;
                yield data.reservesCoinUSDT;
                yield data.reservesPercent;
                yield data.stakedUSDT;
                yield data.stakedPercent;
                yield data.stakedMultiplier;
                yield data.supplyOnChain;
                yield data.stakedOnChain;
                yield data.rewardsUSDT;
                yield data.rewardsUSDT24hr;
                yield data.rewardsUSDT7day;
            }
        };

        return data;
    }

    public async Initialize() {
        this.times = {
            timeCurrent: performance.now() / 1000,
            timeNow: performance.now() / 1000
        }
    }

    public animate() {
        this.times.timeCurrent = performance.now() / 1000;
        const delta = Math.min(1, (this.times.timeCurrent - this.times.timeNow) / Coin.queryFrequency);
        const deltaBig = Big(delta);

        for (var item of this.data) {
            const stat = item as Stat;
            const change = Coin.calculateChanges(stat.sample, deltaBig);

            const value = stat.sample.now.interpolated;
            let now: string;
            if (value.eq(0)){
                now = "-";
            } else{
                now = Coin.format(stat.sample.now.interpolated.toFixed(stat.precision));
            }

            for (var element of stat.values) {
                element.textContent = now;
                if (!stat.valuesCssUpdated){
                    if (change > 0){
                        element.classList.remove ("down");
                        element.classList.add ("up");
                    } else if (change < 0){
                        element.classList.remove ("up");
                        element.classList.add ("down");
                    } else {
                        element.classList.remove ("down");
                        element.classList.remove ("up");
                    }
                }
            }

            if (!stat.valuesCssUpdated) {
                stat.valuesCssUpdated = true;
            }

            let previous = stat.sample.yesterday.interpolated;
            if (!previous.eq(0)) {
                const change = stat.sample.now.interpolated.minus(previous).div(previous).mul(100);
                Coin.updatePercentChange(change, stat.change24hr);
            } else if (stat.sample.now.interpolated.gt(0)) {
                Coin.updatePercentChangeInfinity(stat.change24hr);
            } else {
                Coin.updatePercentChange(Big(0), stat.change24hr);
            }
        }
    }
    public static updatePercentChangeInfinity(elementsToUpdate: HTMLSpanElement[]) {
        const value = "+ <span class=\"infinity\">&infin;&nbsp;</span>";
        for (var i = 0, ul = elementsToUpdate.length; i < ul; i++) {
            const element = elementsToUpdate[i];
            if (element.textContent.indexOf("∞") < 0) {
                element.classList.remove("down");
                element.classList.add("up");
                    
                element.innerHTML = value;
            }
        }
    }

    public static updatePercentChange(change: Big, elementsToUpdate: HTMLSpanElement[]) {
        const value = (change.gt(0) ? "+" : change.lt(0) ? "" : "±") +
            this.format(change.toFixed(2));
        for (var i = 0, ul = elementsToUpdate.length; i < ul; i++) {
            const element = elementsToUpdate[i];
            if (element.textContent !== value) {
                if (change.gt(0)) {
                    element.classList.remove("down");
                    element.classList.add("up");
                } else if (change.lt(0)) {
                    element.classList.remove("up");
                    element.classList.add("down");
                } else {
                    element.classList.remove("up");
                    element.classList.remove("down");
                }

                if (change.gt(999.99)){
                    element.classList.add("large");
                    element.classList.remove("medium");
                } else if (change.gte(99.99)) {
                    element.classList.remove("large");
                    element.classList.add("medium");
                } else {
                    element.classList.remove("large");
                    element.classList.remove("medium");
                }

                element.textContent = value;
            }
        }
    }
    
    public static RefreshData(
        data: Data, 
        chainData: ChainData, 
        times: {
            timeCurrent: number;
            timeNow: number;
        }) {
        {
            const blockNumber = Number(chainData.current.blockNumber);
            if (blockNumber >= data.blockNumber.current){
                data.blockNumber.current = blockNumber;
            } else {
                // Reject old blocks
                return;
            }

            Coin.PopulateData(data, chainData.current, chainData, "now", "history24hrs", "current", "history7day");
        }

        const historyBlock = Number(chainData.history24hrs.blockNumber);
        if (data.blockNumber.history24hr <= historyBlock)
        {
            data.blockNumber.history24hr = historyBlock;

            Coin.PopulateData(data, chainData.history24hrs, chainData, "yesterday", "history48hrs", "history7day", "history14day");
        }
        
        times.timeNow = performance.now() / 1000;
    }

    private static PopulateData(data: Data, period: PeriodData, chainData: ChainData, when: string, before24hrs: string, current7day: string, before7day: string) {
        let sample: Sample;
        sample = data.holders.sample;
        Coin.updateMeasure(sample[when], Big(period.holders));

        sample = data.coinPriceUSDT.sample;
        Coin.updateMeasure(sample[when], Big(period.coinPriceStable));

        const marketCap = Big(period.marketCap);
        sample = data.marketCapUSDT.sample;
        Coin.updateMeasure(sample[when], marketCap);

        const usdReservesBalance = Big(period.usdReservesBalance);
        sample = data.reservesCoin.sample;
        Coin.updateMeasure(sample[when], Big(period.reservesBalance));
        sample = data.reservesCoinUSDT.sample;
        Coin.updateMeasure(sample[when], usdReservesBalance);
        sample = data.reservesPercent.sample;
        Coin.updateMeasure(sample[when], usdReservesBalance.mul(100).div(marketCap));

        sample = data.liquidityToken.sample;
        Coin.updateMeasure(sample[when], Big(period.liquidityToken));

        sample = data.liquidityCoin.sample;
        Coin.updateMeasure(sample[when], Big(period.liquidityCoin));
        sample = data.liquidityCoinUSDT.sample;
        const usdLiquidityCoin = Big(period.usdLiquidityCoin);
        Coin.updateMeasure(sample[when], usdLiquidityCoin);
        sample = data.liquidityPercent.sample;
        Coin.updateMeasure(sample[when], usdLiquidityCoin.mul(100).div(marketCap));

        sample = data.stakedToken.sample;
        Coin.updateMeasure(sample[when], Big(period.staked));
        sample = data.stakedMultiplier.sample;
        Coin.updateMeasure(sample[when], Big(period.aveMultiplier));
        sample = data.stakedUSDT.sample;
        Coin.updateMeasure(sample[when], Big(period.usdStaked));

        const rewards = Big(period.rewards);
        sample = data.rewardsToken.sample;
        Coin.updateMeasure(sample[when], rewards);
        sample = data.rewardsUSDT.sample;
        Coin.updateMeasure(sample[when], Big(period.usdRewards));

        const tokenPriceStable = Big(period.tokenPriceStable);
        sample = data.tokenPriceUSDT.sample;
        Coin.updateMeasure(sample[when], tokenPriceStable);

        let volume24hr = rewards.minus(chainData[before24hrs].rewards);
        sample = data.rewardsToken24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr);
        sample = data.rewardsUSDT24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));

        let volume7day = Big(chainData[current7day].rewards).minus(Big(chainData[before7day].rewards));
        sample = data.rewardsToken7day.sample;
        Coin.updateMeasure(sample[when], volume7day);
        sample = data.rewardsUSDT7day.sample;
        Coin.updateMeasure(sample[when], volume7day.mul(tokenPriceStable));

        const volumeTransfers = Big(period.volumeTransfers);
        sample = data.volumeTransfers.sample;
        Coin.updateMeasure(sample[when], volumeTransfers);
        sample = data.volumeTransfersUSDT.sample;
        Coin.updateMeasure(sample[when], volumeTransfers.mul(tokenPriceStable));

        volume24hr = volumeTransfers.minus(chainData[before24hrs].volumeTransfers);
        sample = data.volumeTransfers24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr);
        sample = data.volumeTransfersUSDT24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));

        const volumeBuy = Big(period.volumeBuy);
        sample = data.volumeBuy.sample;
        Coin.updateMeasure(sample[when], volumeBuy);
        sample = data.volumeBuyUSDT.sample;
        Coin.updateMeasure(sample[when], volumeBuy.mul(tokenPriceStable));

        volume24hr = volumeBuy.minus(chainData[before24hrs].volumeBuy);
        sample = data.volumeBuy24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr);
        sample = data.volumeBuyUSDT24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));

        const volumeSell = Big(period.volumeSell);
        sample = data.volumeSell.sample;
        Coin.updateMeasure(sample[when], volumeSell);
        sample = data.volumeSellUSDT.sample;
        Coin.updateMeasure(sample[when], volumeSell.mul(tokenPriceStable));

        volume24hr = volumeSell.minus(chainData[before24hrs].volumeSell);
        sample = data.volumeSell24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr);
        sample = data.volumeSellUSDT24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));

        const volumeTrade = Big(period.volumeTrade);
        sample = data.volumeTrade.sample;
        Coin.updateMeasure(sample[when], volumeTrade);
        sample = data.volumeTradeUSDT.sample;
        Coin.updateMeasure(sample[when], volumeTrade.mul(tokenPriceStable));

        volume24hr = volumeTrade.minus(chainData[before24hrs].volumeTrade);
        sample = data.volumeTrade24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr);
        sample = data.volumeTradeUSDT24hr.sample;
        Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));

        sample = data.tokenPriceCoin.sample;
        Coin.updateMeasure(sample[when], Big(period.tokenPriceCoin));

        sample = data.stakedPercent.sample;
        Coin.updateMeasure(sample[when], Big(period.stakedOfTotalSupplyPercent).mul(100));
        sample = data.supplyOnChain.sample;
        Coin.updateMeasure(sample[when], Big(period.supplyOnChainPercent).mul(100));
        sample = data.stakedOnChain.sample;
        Coin.updateMeasure(sample[when], Big(period.stakedOfOnChainPercent).mul(100));
        
        for (var item of data) {
            const stat = item as Stat;
            stat.valuesCssUpdated = false;
        }
    }

    private static CreateStat(classValue: string, precision: number): Stat {
        const classChange = classValue + "-change";
        return {
            sample: Coin.CreateSample(),
            precision: precision,
            values: [...document.querySelectorAll(classValue)] as HTMLSpanElement[],  
            change24hr: [...document.querySelectorAll(classChange)] as HTMLSpanElement[], 
            valuesCssUpdated: false
        };
    }

    public static updateMeasure(measure: Value, value: Big) {
        measure.previous = measure.interpolated;
        measure.current = value;
        if (measure.previous.eq(0)) {
            measure.previous = measure.interpolated = measure.current;
        }
    }

    private static CreateSample(): Sample {
        return {
            now: Coin.CreateValue(),
            yesterday: Coin.CreateValue(),
        };
    }
    private static CreateValue(): Value {
        return {
            current: Big(0),
            interpolated: Big(0),
            previous: Big(0)
        }
    }

    public static calculateChanges(sample: Sample, delta: Big): number {
        const change = Coin.calculateValueChanges(sample.now, delta);
        Coin.calculateValueChanges(sample.yesterday, delta);
        return change;
    }

    public static calculateValueChanges(value: Value, delta: Big): number {
        let change = 0;
        if (!value.interpolated.eq(value.current)) {
            const diff = value.current.minus(value.previous).times(delta);
            const min = Coin.minimum(value.previous, value.current);
            const max = Coin.maximum(value.previous, value.current);
            const newValue = Coin.minimum(max, Coin.maximum(min, value.previous.plus(diff)));
            if (newValue.gt(value.interpolated)){
                change = 1;
            } else if (newValue.lt(value.interpolated)) {
                change = -1
            }

            value.interpolated = newValue;
        }

        return change;
    }

    public static format(n) {
        var parts = n.toString().split(".");
        return parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (parts[1] ? "." + parts[1] : "");
    }

    private static minimum(x: Big, y: Big) {
        if (x.gt(y)) return x;
        return y;
    }

    private static maximum(x: Big, y: Big) {
        if (x.lt(y)) return x;
        return y;
    }
}

class TokenStats {
    private coins = [
        new Coin("bsc"),
        new Coin("eth"),
        new Coin("poly"),
        new Coin("ftm"),
        new Coin("avax")
    ];
    private data: Data;

    private refresh: Promise<void>;
    private isRefreshing: boolean = false;
    private ticker: HTMLElement = document.getElementById("ticker");
    private tickerOffset = 0;
    private tickerMoving: boolean;
    private times = {
        timeCurrent: 0,
        timeNow: 0
    };

    constructor() {
    }

    public async Initialize() {

        this.data = Coin.CreateStats("all");

        for (let i = 0, ul = this.coins.length; i < ul; i++) {
            this.coins[i].Initialize();
        }
        this.times = {
            timeCurrent: performance.now() / 1000,
            timeNow: performance.now() / 1000
        }

        await this.Refresh();

        if (this.ticker){
            this.ticker.style.display = "";
            this.tickerMoving = true;

            for (var e of document.querySelectorAll(".needs-live-data")) {
                (e as HTMLElement).classList.remove("needs-live-data");
            }

            this.ticker.addEventListener('mouseenter', e => {
                this.tickerMoving = false;
            });

            this.ticker.addEventListener('mouseleave', e => {
                this.tickerMoving = true;
            });
        }
    }

    private Refresh() {
        return this.isRefreshing ? this.refresh : (this.refresh = this.RefreshData());
    }

    private async RefreshData() {
        this.isRefreshing = true;
        try {
            const request = await fetch("https://everrise.azurewebsites.net/stats");
            const blockchainData = (await request.json()) as BlockchainData;

            for (let i = 0, ul = this.coins.length; i < ul; i++) {
                const coin = this.coins[i];
                const chainData = blockchainData[coin.network] as ChainData;
                Coin.RefreshData(coin.data, chainData, coin.times);
            }

            Coin.RefreshData(this.data, blockchainData["unified"], this.times);
        } finally {
            this.isRefreshing = false;
        }
    }

    private skipFrame = -1;
    private animate() {
        this.animationRequest = requestAnimationFrame(() => this.animate());

        this.skipFrame++;
        if (this.skipFrame > 3) {
            this.skipFrame = 0;
        }
        if (this.skipFrame !== 0) {
            return;
        }

        for (let i = 0, ul = this.coins.length; i < ul; i++) {
            this.coins[i].animate();
        }

        this.times.timeCurrent = performance.now() / 1000;
        const delta = Math.min(1, (this.times.timeCurrent - this.times.timeNow) / 30);
        const deltaBig = Big(delta);

        for (var item of this.data) {
            const stat = item as Stat;
            const change = Coin.calculateChanges(stat.sample, deltaBig);

            const value = stat.sample.now.interpolated;
            let now: string;
            if (value.eq(0)){
                now = "-";
            } else{
                now = Coin.format(stat.sample.now.interpolated.toFixed(stat.precision));
            }
            
            for (var element of stat.values) {
                element.textContent = now;
                if (!stat.valuesCssUpdated) {
                    if (change > 0){
                        element.classList.remove ("down");
                        element.classList.add ("up");
                    } else if (change < 0){
                        element.classList.remove ("up");
                        element.classList.add ("down");
                    } else {
                        element.classList.remove ("down");
                        element.classList.remove ("up");
                    }
                }
            }

            if (!stat.valuesCssUpdated) {
                stat.valuesCssUpdated = true;
            }

            let previous = stat.sample.yesterday.interpolated;
            if (!previous.eq(0)) {
                const change = stat.sample.now.interpolated.minus(previous).div(previous).mul(100);
                Coin.updatePercentChange(change, stat.change24hr);
            }
        }
        
        this.updatePriceSvg();

        if (this.tickerMoving) {
            const halfWidth = this.ticker.getBoundingClientRect().width / 2;
            if (this.tickerOffset > halfWidth) {
                this.tickerOffset = 0;
            }
            this.ticker.style.transform = `translate(-${this.tickerOffset}px,0)`;
            this.tickerOffset += 0.5;
        }
    }

    private isPaused = true;
    private animationRequest: number;
    private dataRefreshRequest: number;

    private colors = [
        "rgba(234,185,2,1)",
        "rgba(103,109,143,1)",
        "rgba(124,65,213,1)",
        "rgba(25,105,255,1)",
        "rgba(232,65,66,1)",
        "rgba(255,255,255,1)"
    ];

    private updatePriceSvg() {
        const graphWidth = 420;
        var min = 1000000;
        var max = 0;
        for (let i = 0, ul = this.coins.length; i < ul; i++) {
            var value = this.coins[i].data.tokenPriceUSDT.sample.now.interpolated.toNumber();
            if (min > value) {
                min = value;
            }
            if (max < value) {
                max = value;
            }
        }

        var range = max - min;
        var buffer = range * 0.025;
        var minX = min - buffer;
        range *= 1.05;

        const priceGraphs = [...document.querySelectorAll(".graph-priceRange")] as SVGGElement[];
        if (priceGraphs.length > 0 && range > 0) {
            const unifiedPrice = this.data.tokenPriceUSDT.sample.now.interpolated.toNumber();
            let graphs = "";
            let prices = new Array(this.coins.length);
            let positions = new Array(this.coins.length);
            for (let i = 0, ul = this.coins.length; i < ul; i++) {
                const coin = this.coins[i];
                const value = coin.data.tokenPriceUSDT.sample.now.interpolated.toNumber();
                prices[i] = value;

                if (value <= 0) continue;

                const x = (((value - minX) * graphWidth) / range);
                positions[i] = x;
                const xText = x.toFixed(2);

                for (var icon of document.querySelectorAll(`.graph-icon-${coin.network}`)){
                    icon.setAttribute("x", (x - 8).toFixed(2));
                    icon.setAttribute("y", "8");
                }

                graphs += `
                <line x1="${xText}" x2="${xText}" y1="22" y2="56" stroke="${this.colors[i]}" stroke-width="2"/>`;
            }
            
            if (unifiedPrice > 0) {
                const x = (((unifiedPrice - minX) * graphWidth) / range);
                const xText = x.toFixed(2);

                for (var icon of document.querySelectorAll(`.graph-icon-rise`)){
                    icon.setAttribute("x", (x - 8).toFixed(2));
                    icon.setAttribute("y", "0");
                }
                for (let i = 0, ul = prices.length; i < ul; i++) {
                    const price = prices[i];
                    if (price <= 0) continue;

                    let xPos;
                    if (positions[i] > x){
                        xPos = x + (positions[i] - x) / 2 - 20;
                    }else{
                        xPos = positions[i] + (x - positions[i]) / 2 - 20;
                    }

                    const percentChange = (((price - unifiedPrice) * 100) /unifiedPrice);

                    const yText = (24 + i * 8);
                    graphs += `
                    <line x1="${positions[i].toFixed(2)}" stroke-dasharray="2" x2="${xText}" y1="${yText}" y2="${yText}" stroke="${this.colors[i]}" stroke-width="1"/>
                    <text x="${xPos.toFixed(2)}" y="${yText - 4}" class="graph-text-percent">${percentChange > 0 ? "+" : ""}${percentChange.toFixed(2)}%</text>`;

                }

                graphs += `
                <line x1="${xText}" x2="${xText}" y1="18" y2="56" stroke="${this.colors[this.colors.length - 1]}" stroke-width="3"/>
                <text x="0" y="66" class="graph-text-price">${min.toFixed(7)}</text>
                <text x="${xText}" y="66" class="graph-text-price" text-anchor="middle">${unifiedPrice.toFixed(7)}</text>
                <text x="${graphWidth}" y="66" class="graph-text-price" text-anchor="end">${max.toFixed(7)}</text>`;
            }

            for (var g of priceGraphs) {
                g.innerHTML = graphs;
            }
        }

    }

    public Start() {
        if (this.isPaused) {
            this.isPaused = false;
            this.animationRequest = requestAnimationFrame(() => this.animate());
            setTimeout(() => this.Refresh(), 15000);
            this.dataRefreshRequest = setInterval(() => this.Refresh(), 30000);
        }
    }

    public Pause() {
        if (this.isPaused) return;

        this.isPaused = true;
        clearInterval(this.dataRefreshRequest);
        cancelAnimationFrame(this.animationRequest);
    }
}

const stats = new TokenStats();
stats.Initialize()
stats.Start();