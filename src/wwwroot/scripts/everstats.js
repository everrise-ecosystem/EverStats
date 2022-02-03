(() => {
  var __create = Object.create;
  var __defProp = Object.defineProperty;
  var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
  var __getOwnPropNames = Object.getOwnPropertyNames;
  var __getProtoOf = Object.getPrototypeOf;
  var __hasOwnProp = Object.prototype.hasOwnProperty;
  var __markAsModule = (target) => __defProp(target, "__esModule", { value: true });
  var __commonJS = (cb, mod) => function __require() {
    return mod || (0, cb[Object.keys(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
  };
  var __reExport = (target, module, desc) => {
    if (module && typeof module === "object" || typeof module === "function") {
      for (let key of __getOwnPropNames(module))
        if (!__hasOwnProp.call(target, key) && key !== "default")
          __defProp(target, key, { get: () => module[key], enumerable: !(desc = __getOwnPropDesc(module, key)) || desc.enumerable });
    }
    return target;
  };
  var __toModule = (module) => {
    return __reExport(__markAsModule(__defProp(module != null ? __create(__getProtoOf(module)) : {}, "default", module && module.__esModule && "default" in module ? { get: () => module.default, enumerable: true } : { value: module, enumerable: true })), module);
  };

  // node_modules/big.js/big.js
  var require_big = __commonJS({
    "node_modules/big.js/big.js"(exports, module) {
      (function(GLOBAL) {
        "use strict";
        var Big2, DP = 20, RM = 1, MAX_DP = 1e6, MAX_POWER = 1e6, NE = -7, PE = 21, STRICT = false, NAME = "[big.js] ", INVALID = NAME + "Invalid ", INVALID_DP = INVALID + "decimal places", INVALID_RM = INVALID + "rounding mode", DIV_BY_ZERO = NAME + "Division by zero", P = {}, UNDEFINED = void 0, NUMERIC = /^-?(\d+(\.\d*)?|\.\d+)(e[+-]?\d+)?$/i;
        function _Big_() {
          function Big3(n) {
            var x = this;
            if (!(x instanceof Big3))
              return n === UNDEFINED ? _Big_() : new Big3(n);
            if (n instanceof Big3) {
              x.s = n.s;
              x.e = n.e;
              x.c = n.c.slice();
            } else {
              if (typeof n !== "string") {
                if (Big3.strict === true) {
                  throw TypeError(INVALID + "number");
                }
                n = n === 0 && 1 / n < 0 ? "-0" : String(n);
              }
              parse(x, n);
            }
            x.constructor = Big3;
          }
          Big3.prototype = P;
          Big3.DP = DP;
          Big3.RM = RM;
          Big3.NE = NE;
          Big3.PE = PE;
          Big3.strict = STRICT;
          Big3.roundDown = 0;
          Big3.roundHalfUp = 1;
          Big3.roundHalfEven = 2;
          Big3.roundUp = 3;
          return Big3;
        }
        function parse(x, n) {
          var e, i, nl;
          if (!NUMERIC.test(n)) {
            throw Error(INVALID + "number");
          }
          x.s = n.charAt(0) == "-" ? (n = n.slice(1), -1) : 1;
          if ((e = n.indexOf(".")) > -1)
            n = n.replace(".", "");
          if ((i = n.search(/e/i)) > 0) {
            if (e < 0)
              e = i;
            e += +n.slice(i + 1);
            n = n.substring(0, i);
          } else if (e < 0) {
            e = n.length;
          }
          nl = n.length;
          for (i = 0; i < nl && n.charAt(i) == "0"; )
            ++i;
          if (i == nl) {
            x.c = [x.e = 0];
          } else {
            for (; nl > 0 && n.charAt(--nl) == "0"; )
              ;
            x.e = e - i - 1;
            x.c = [];
            for (e = 0; i <= nl; )
              x.c[e++] = +n.charAt(i++);
          }
          return x;
        }
        function round(x, sd, rm, more) {
          var xc = x.c;
          if (rm === UNDEFINED)
            rm = x.constructor.RM;
          if (rm !== 0 && rm !== 1 && rm !== 2 && rm !== 3) {
            throw Error(INVALID_RM);
          }
          if (sd < 1) {
            more = rm === 3 && (more || !!xc[0]) || sd === 0 && (rm === 1 && xc[0] >= 5 || rm === 2 && (xc[0] > 5 || xc[0] === 5 && (more || xc[1] !== UNDEFINED)));
            xc.length = 1;
            if (more) {
              x.e = x.e - sd + 1;
              xc[0] = 1;
            } else {
              xc[0] = x.e = 0;
            }
          } else if (sd < xc.length) {
            more = rm === 1 && xc[sd] >= 5 || rm === 2 && (xc[sd] > 5 || xc[sd] === 5 && (more || xc[sd + 1] !== UNDEFINED || xc[sd - 1] & 1)) || rm === 3 && (more || !!xc[0]);
            xc.length = sd--;
            if (more) {
              for (; ++xc[sd] > 9; ) {
                xc[sd] = 0;
                if (!sd--) {
                  ++x.e;
                  xc.unshift(1);
                }
              }
            }
            for (sd = xc.length; !xc[--sd]; )
              xc.pop();
          }
          return x;
        }
        function stringify(x, doExponential, isNonzero) {
          var e = x.e, s = x.c.join(""), n = s.length;
          if (doExponential) {
            s = s.charAt(0) + (n > 1 ? "." + s.slice(1) : "") + (e < 0 ? "e" : "e+") + e;
          } else if (e < 0) {
            for (; ++e; )
              s = "0" + s;
            s = "0." + s;
          } else if (e > 0) {
            if (++e > n) {
              for (e -= n; e--; )
                s += "0";
            } else if (e < n) {
              s = s.slice(0, e) + "." + s.slice(e);
            }
          } else if (n > 1) {
            s = s.charAt(0) + "." + s.slice(1);
          }
          return x.s < 0 && isNonzero ? "-" + s : s;
        }
        P.abs = function() {
          var x = new this.constructor(this);
          x.s = 1;
          return x;
        };
        P.cmp = function(y) {
          var isneg, x = this, xc = x.c, yc = (y = new x.constructor(y)).c, i = x.s, j = y.s, k = x.e, l = y.e;
          if (!xc[0] || !yc[0])
            return !xc[0] ? !yc[0] ? 0 : -j : i;
          if (i != j)
            return i;
          isneg = i < 0;
          if (k != l)
            return k > l ^ isneg ? 1 : -1;
          j = (k = xc.length) < (l = yc.length) ? k : l;
          for (i = -1; ++i < j; ) {
            if (xc[i] != yc[i])
              return xc[i] > yc[i] ^ isneg ? 1 : -1;
          }
          return k == l ? 0 : k > l ^ isneg ? 1 : -1;
        };
        P.div = function(y) {
          var x = this, Big3 = x.constructor, a = x.c, b = (y = new Big3(y)).c, k = x.s == y.s ? 1 : -1, dp = Big3.DP;
          if (dp !== ~~dp || dp < 0 || dp > MAX_DP) {
            throw Error(INVALID_DP);
          }
          if (!b[0]) {
            throw Error(DIV_BY_ZERO);
          }
          if (!a[0]) {
            y.s = k;
            y.c = [y.e = 0];
            return y;
          }
          var bl, bt, n, cmp, ri, bz = b.slice(), ai = bl = b.length, al = a.length, r = a.slice(0, bl), rl = r.length, q = y, qc = q.c = [], qi = 0, p = dp + (q.e = x.e - y.e) + 1;
          q.s = k;
          k = p < 0 ? 0 : p;
          bz.unshift(0);
          for (; rl++ < bl; )
            r.push(0);
          do {
            for (n = 0; n < 10; n++) {
              if (bl != (rl = r.length)) {
                cmp = bl > rl ? 1 : -1;
              } else {
                for (ri = -1, cmp = 0; ++ri < bl; ) {
                  if (b[ri] != r[ri]) {
                    cmp = b[ri] > r[ri] ? 1 : -1;
                    break;
                  }
                }
              }
              if (cmp < 0) {
                for (bt = rl == bl ? b : bz; rl; ) {
                  if (r[--rl] < bt[rl]) {
                    ri = rl;
                    for (; ri && !r[--ri]; )
                      r[ri] = 9;
                    --r[ri];
                    r[rl] += 10;
                  }
                  r[rl] -= bt[rl];
                }
                for (; !r[0]; )
                  r.shift();
              } else {
                break;
              }
            }
            qc[qi++] = cmp ? n : ++n;
            if (r[0] && cmp)
              r[rl] = a[ai] || 0;
            else
              r = [a[ai]];
          } while ((ai++ < al || r[0] !== UNDEFINED) && k--);
          if (!qc[0] && qi != 1) {
            qc.shift();
            q.e--;
            p--;
          }
          if (qi > p)
            round(q, p, Big3.RM, r[0] !== UNDEFINED);
          return q;
        };
        P.eq = function(y) {
          return this.cmp(y) === 0;
        };
        P.gt = function(y) {
          return this.cmp(y) > 0;
        };
        P.gte = function(y) {
          return this.cmp(y) > -1;
        };
        P.lt = function(y) {
          return this.cmp(y) < 0;
        };
        P.lte = function(y) {
          return this.cmp(y) < 1;
        };
        P.minus = P.sub = function(y) {
          var i, j, t, xlty, x = this, Big3 = x.constructor, a = x.s, b = (y = new Big3(y)).s;
          if (a != b) {
            y.s = -b;
            return x.plus(y);
          }
          var xc = x.c.slice(), xe = x.e, yc = y.c, ye = y.e;
          if (!xc[0] || !yc[0]) {
            if (yc[0]) {
              y.s = -b;
            } else if (xc[0]) {
              y = new Big3(x);
            } else {
              y.s = 1;
            }
            return y;
          }
          if (a = xe - ye) {
            if (xlty = a < 0) {
              a = -a;
              t = xc;
            } else {
              ye = xe;
              t = yc;
            }
            t.reverse();
            for (b = a; b--; )
              t.push(0);
            t.reverse();
          } else {
            j = ((xlty = xc.length < yc.length) ? xc : yc).length;
            for (a = b = 0; b < j; b++) {
              if (xc[b] != yc[b]) {
                xlty = xc[b] < yc[b];
                break;
              }
            }
          }
          if (xlty) {
            t = xc;
            xc = yc;
            yc = t;
            y.s = -y.s;
          }
          if ((b = (j = yc.length) - (i = xc.length)) > 0)
            for (; b--; )
              xc[i++] = 0;
          for (b = i; j > a; ) {
            if (xc[--j] < yc[j]) {
              for (i = j; i && !xc[--i]; )
                xc[i] = 9;
              --xc[i];
              xc[j] += 10;
            }
            xc[j] -= yc[j];
          }
          for (; xc[--b] === 0; )
            xc.pop();
          for (; xc[0] === 0; ) {
            xc.shift();
            --ye;
          }
          if (!xc[0]) {
            y.s = 1;
            xc = [ye = 0];
          }
          y.c = xc;
          y.e = ye;
          return y;
        };
        P.mod = function(y) {
          var ygtx, x = this, Big3 = x.constructor, a = x.s, b = (y = new Big3(y)).s;
          if (!y.c[0]) {
            throw Error(DIV_BY_ZERO);
          }
          x.s = y.s = 1;
          ygtx = y.cmp(x) == 1;
          x.s = a;
          y.s = b;
          if (ygtx)
            return new Big3(x);
          a = Big3.DP;
          b = Big3.RM;
          Big3.DP = Big3.RM = 0;
          x = x.div(y);
          Big3.DP = a;
          Big3.RM = b;
          return this.minus(x.times(y));
        };
        P.plus = P.add = function(y) {
          var e, k, t, x = this, Big3 = x.constructor;
          y = new Big3(y);
          if (x.s != y.s) {
            y.s = -y.s;
            return x.minus(y);
          }
          var xe = x.e, xc = x.c, ye = y.e, yc = y.c;
          if (!xc[0] || !yc[0]) {
            if (!yc[0]) {
              if (xc[0]) {
                y = new Big3(x);
              } else {
                y.s = x.s;
              }
            }
            return y;
          }
          xc = xc.slice();
          if (e = xe - ye) {
            if (e > 0) {
              ye = xe;
              t = yc;
            } else {
              e = -e;
              t = xc;
            }
            t.reverse();
            for (; e--; )
              t.push(0);
            t.reverse();
          }
          if (xc.length - yc.length < 0) {
            t = yc;
            yc = xc;
            xc = t;
          }
          e = yc.length;
          for (k = 0; e; xc[e] %= 10)
            k = (xc[--e] = xc[e] + yc[e] + k) / 10 | 0;
          if (k) {
            xc.unshift(k);
            ++ye;
          }
          for (e = xc.length; xc[--e] === 0; )
            xc.pop();
          y.c = xc;
          y.e = ye;
          return y;
        };
        P.pow = function(n) {
          var x = this, one = new x.constructor("1"), y = one, isneg = n < 0;
          if (n !== ~~n || n < -MAX_POWER || n > MAX_POWER) {
            throw Error(INVALID + "exponent");
          }
          if (isneg)
            n = -n;
          for (; ; ) {
            if (n & 1)
              y = y.times(x);
            n >>= 1;
            if (!n)
              break;
            x = x.times(x);
          }
          return isneg ? one.div(y) : y;
        };
        P.prec = function(sd, rm) {
          if (sd !== ~~sd || sd < 1 || sd > MAX_DP) {
            throw Error(INVALID + "precision");
          }
          return round(new this.constructor(this), sd, rm);
        };
        P.round = function(dp, rm) {
          if (dp === UNDEFINED)
            dp = 0;
          else if (dp !== ~~dp || dp < -MAX_DP || dp > MAX_DP) {
            throw Error(INVALID_DP);
          }
          return round(new this.constructor(this), dp + this.e + 1, rm);
        };
        P.sqrt = function() {
          var r, c, t, x = this, Big3 = x.constructor, s = x.s, e = x.e, half = new Big3("0.5");
          if (!x.c[0])
            return new Big3(x);
          if (s < 0) {
            throw Error(NAME + "No square root");
          }
          s = Math.sqrt(x + "");
          if (s === 0 || s === 1 / 0) {
            c = x.c.join("");
            if (!(c.length + e & 1))
              c += "0";
            s = Math.sqrt(c);
            e = ((e + 1) / 2 | 0) - (e < 0 || e & 1);
            r = new Big3((s == 1 / 0 ? "5e" : (s = s.toExponential()).slice(0, s.indexOf("e") + 1)) + e);
          } else {
            r = new Big3(s + "");
          }
          e = r.e + (Big3.DP += 4);
          do {
            t = r;
            r = half.times(t.plus(x.div(t)));
          } while (t.c.slice(0, e).join("") !== r.c.slice(0, e).join(""));
          return round(r, (Big3.DP -= 4) + r.e + 1, Big3.RM);
        };
        P.times = P.mul = function(y) {
          var c, x = this, Big3 = x.constructor, xc = x.c, yc = (y = new Big3(y)).c, a = xc.length, b = yc.length, i = x.e, j = y.e;
          y.s = x.s == y.s ? 1 : -1;
          if (!xc[0] || !yc[0]) {
            y.c = [y.e = 0];
            return y;
          }
          y.e = i + j;
          if (a < b) {
            c = xc;
            xc = yc;
            yc = c;
            j = a;
            a = b;
            b = j;
          }
          for (c = new Array(j = a + b); j--; )
            c[j] = 0;
          for (i = b; i--; ) {
            b = 0;
            for (j = a + i; j > i; ) {
              b = c[j] + yc[i] * xc[j - i - 1] + b;
              c[j--] = b % 10;
              b = b / 10 | 0;
            }
            c[j] = b;
          }
          if (b)
            ++y.e;
          else
            c.shift();
          for (i = c.length; !c[--i]; )
            c.pop();
          y.c = c;
          return y;
        };
        P.toExponential = function(dp, rm) {
          var x = this, n = x.c[0];
          if (dp !== UNDEFINED) {
            if (dp !== ~~dp || dp < 0 || dp > MAX_DP) {
              throw Error(INVALID_DP);
            }
            x = round(new x.constructor(x), ++dp, rm);
            for (; x.c.length < dp; )
              x.c.push(0);
          }
          return stringify(x, true, !!n);
        };
        P.toFixed = function(dp, rm) {
          var x = this, n = x.c[0];
          if (dp !== UNDEFINED) {
            if (dp !== ~~dp || dp < 0 || dp > MAX_DP) {
              throw Error(INVALID_DP);
            }
            x = round(new x.constructor(x), dp + x.e + 1, rm);
            for (dp = dp + x.e + 1; x.c.length < dp; )
              x.c.push(0);
          }
          return stringify(x, false, !!n);
        };
        P.toJSON = P.toString = function() {
          var x = this, Big3 = x.constructor;
          return stringify(x, x.e <= Big3.NE || x.e >= Big3.PE, !!x.c[0]);
        };
        P.toNumber = function() {
          var n = Number(stringify(this, true, true));
          if (this.constructor.strict === true && !this.eq(n.toString())) {
            throw Error(NAME + "Imprecise conversion");
          }
          return n;
        };
        P.toPrecision = function(sd, rm) {
          var x = this, Big3 = x.constructor, n = x.c[0];
          if (sd !== UNDEFINED) {
            if (sd !== ~~sd || sd < 1 || sd > MAX_DP) {
              throw Error(INVALID + "precision");
            }
            x = round(new Big3(x), sd, rm);
            for (; x.c.length < sd; )
              x.c.push(0);
          }
          return stringify(x, sd <= x.e || x.e <= Big3.NE || x.e >= Big3.PE, !!n);
        };
        P.valueOf = function() {
          var x = this, Big3 = x.constructor;
          if (Big3.strict === true) {
            throw Error(NAME + "valueOf disallowed");
          }
          return stringify(x, x.e <= Big3.NE || x.e >= Big3.PE, true);
        };
        Big2 = _Big_();
        Big2["default"] = Big2.Big = Big2;
        if (typeof define === "function" && define.amd) {
          define(function() {
            return Big2;
          });
        } else if (typeof module !== "undefined" && module.exports) {
          module.exports = Big2;
        } else {
          GLOBAL.Big = Big2;
        }
      })(exports);
    }
  });

  // src/scripts/main.ts
  var import_big = __toModule(require_big());
  var _Coin = class {
    constructor(network) {
      this.supply = (0, import_big.Big)("71618033988");
      this.times = {
        timeCurrent: 0,
        timeNow: 0
      };
      this.network = network;
      this.data = _Coin.CreateStats(this.network);
    }
    static CreateStats(network) {
      const data = {
        blockNumber: {
          current: -1,
          history24hr: -1
        },
        holders: _Coin.CreateStat(`.ticker-${network}-holders`, 0),
        tokenPriceUSDT: _Coin.CreateStat(`.ticker-${network}-token-price-usdt`, 8),
        tokenPriceCoin: _Coin.CreateStat(`.ticker-${network}-token-price-coin`, 10),
        coinPriceUSDT: _Coin.CreateStat(`.ticker-${network}-coin-price-usdt`, 3),
        liquidityToken: _Coin.CreateStat(`.ticker-${network}-token-liquidity`, 2),
        liquidityCoin: _Coin.CreateStat(`.ticker-${network}-coin-liquidity`, 2),
        liquidityCoinUSDT: _Coin.CreateStat(`.ticker-${network}-coin-liquidity-usdt`, 2),
        liquidityPercent: _Coin.CreateStat(`.ticker-${network}-liquidity-percent`, 4),
        reservesCoin: _Coin.CreateStat(`.ticker-${network}-coin-reserves`, 2),
        reservesCoinUSDT: _Coin.CreateStat(`.ticker-${network}-coin-reserves-usdt`, 2),
        reservesPercent: _Coin.CreateStat(`.ticker-${network}-coin-reserves-percent`, 4),
        marketCapUSDT: _Coin.CreateStat(`.ticker-${network}-market-cap`, 2),
        volumeTransfers: _Coin.CreateStat(`.ticker-${network}-vol-txns`, 2),
        volumeTransfersUSDT: _Coin.CreateStat(`.ticker-${network}-vol-txns-usdt`, 2),
        volumeBuy: _Coin.CreateStat(`.ticker-${network}-vol-buy`, 2),
        volumeBuyUSDT: _Coin.CreateStat(`.ticker-${network}-vol-buy-usdt`, 2),
        volumeSell: _Coin.CreateStat(`.ticker-${network}-vol-sell`, 2),
        volumeSellUSDT: _Coin.CreateStat(`.ticker-${network}-vol-sell-usdt`, 2),
        volumeTrade: _Coin.CreateStat(`.ticker-${network}-vol-trade`, 2),
        volumeTradeUSDT: _Coin.CreateStat(`.ticker-${network}-vol-trade-usdt`, 2),
        volumeTransfers24hr: _Coin.CreateStat(`.ticker-${network}-vol-txns-24hr`, 2),
        volumeTransfersUSDT24hr: _Coin.CreateStat(`.ticker-${network}-vol-txns-usdt-24hr`, 2),
        volumeBuy24hr: _Coin.CreateStat(`.ticker-${network}-vol-buy-24hr`, 2),
        volumeBuyUSDT24hr: _Coin.CreateStat(`.ticker-${network}-vol-buy-usdt-24hr`, 2),
        volumeSell24hr: _Coin.CreateStat(`.ticker-${network}-vol-sell-24hr`, 2),
        volumeSellUSDT24hr: _Coin.CreateStat(`.ticker-${network}-vol-sell-usdt-24hr`, 2),
        volumeTrade24hr: _Coin.CreateStat(`.ticker-${network}-vol-trade-24hr`, 2),
        volumeTradeUSDT24hr: _Coin.CreateStat(`.ticker-${network}-vol-trade-usdt-24hr`, 2),
        volumeTransfers7day: _Coin.CreateStat(`.ticker-${network}-vol-txns-7day`, 2),
        volumeTransfersUSDT7day: _Coin.CreateStat(`.ticker-${network}-vol-txns-usdt-7day`, 2),
        volumeBuy7day: _Coin.CreateStat(`.ticker-${network}-vol-buy-7day`, 2),
        volumeBuyUSDT7day: _Coin.CreateStat(`.ticker-${network}-vol-buy-usdt-7day`, 2),
        volumeSell7day: _Coin.CreateStat(`.ticker-${network}-vol-sell-7day`, 2),
        volumeSellUSDT7day: _Coin.CreateStat(`.ticker-${network}-vol-sell-usdt-7day`, 2),
        volumeTrade7day: _Coin.CreateStat(`.ticker-${network}-vol-trade-7day`, 2),
        volumeTradeUSDT7day: _Coin.CreateStat(`.ticker-${network}-vol-trade-usdt-7day`, 2),
        stakedToken: _Coin.CreateStat(`.ticker-${network}-staked-token`, 2),
        stakedUSDT: _Coin.CreateStat(`.ticker-${network}-staked-usdt`, 2),
        stakedPercent: _Coin.CreateStat(`.ticker-${network}-staked-percent`, 4),
        stakedMultiplier: _Coin.CreateStat(`.ticker-${network}-staked-multiplier`, 4),
        supplyOnChain: _Coin.CreateStat(`.ticker-${network}-supply-percent`, 4),
        stakedOnChain: _Coin.CreateStat(`.ticker-${network}-staked-onchain-percent`, 4),
        rewardsToken: _Coin.CreateStat(`.ticker-${network}-rewards-token`, 2),
        rewardsToken24hr: _Coin.CreateStat(`.ticker-${network}-rewards-token-24hr`, 2),
        rewardsToken7day: _Coin.CreateStat(`.ticker-${network}-rewards-token-7day`, 2),
        rewardsUSDT: _Coin.CreateStat(`.ticker-${network}-rewards-usdt`, 2),
        rewardsUSDT24hr: _Coin.CreateStat(`.ticker-${network}-rewards-usdt-24hr`, 2),
        rewardsUSDT7day: _Coin.CreateStat(`.ticker-${network}-rewards-usdt-7day`, 2),
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
    async Initialize() {
      this.times = {
        timeCurrent: performance.now() / 1e3,
        timeNow: performance.now() / 1e3
      };
    }
    animate() {
      this.times.timeCurrent = performance.now() / 1e3;
      const delta = Math.min(1, (this.times.timeCurrent - this.times.timeNow) / _Coin.queryFrequency);
      const deltaBig = (0, import_big.Big)(delta);
      for (var item of this.data) {
        const stat = item;
        const change = _Coin.calculateChanges(stat.sample, deltaBig);
        const value = stat.sample.now.interpolated;
        let now;
        if (value.eq(0)) {
          now = "-";
        } else {
          now = _Coin.format(stat.sample.now.interpolated.toFixed(stat.precision));
        }
        for (var element of stat.values) {
          element.textContent = now;
          if (!stat.valuesCssUpdated) {
            if (change > 0) {
              element.classList.remove("down");
              element.classList.add("up");
            } else if (change < 0) {
              element.classList.remove("up");
              element.classList.add("down");
            } else {
              element.classList.remove("down");
              element.classList.remove("up");
            }
          }
        }
        if (!stat.valuesCssUpdated) {
          stat.valuesCssUpdated = true;
        }
        let previous = stat.sample.yesterday.interpolated;
        if (!previous.eq(0)) {
          const change2 = stat.sample.now.interpolated.minus(previous).div(previous).mul(100);
          _Coin.updatePercentChange(change2, stat.change24hr);
        } else if (stat.sample.now.interpolated.gt(0)) {
          _Coin.updatePercentChangeInfinity(stat.change24hr);
        } else {
          _Coin.updatePercentChange((0, import_big.Big)(0), stat.change24hr);
        }
      }
    }
    static updatePercentChangeInfinity(elementsToUpdate) {
      const value = '+ <span class="infinity">&infin;&nbsp;</span>';
      for (var i = 0, ul = elementsToUpdate.length; i < ul; i++) {
        const element = elementsToUpdate[i];
        if (element.textContent.indexOf("\u221E") < 0) {
          element.classList.remove("down");
          element.classList.add("up");
          element.innerHTML = value;
        }
      }
    }
    static updatePercentChange(change, elementsToUpdate) {
      const value = (change.gt(0) ? "+" : change.lt(0) ? "" : "\xB1") + this.format(change.toFixed(2));
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
          if (change.gt(999.99)) {
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
    static RefreshData(data, chainData, times) {
      {
        const blockNumber = Number(chainData.current.blockNumber);
        if (blockNumber >= data.blockNumber.current) {
          data.blockNumber.current = blockNumber;
        } else {
          return;
        }
        _Coin.PopulateData(data, chainData.current, chainData, "now", "history24hrs", "current", "history7day");
      }
      const historyBlock = Number(chainData.history24hrs.blockNumber);
      if (data.blockNumber.history24hr <= historyBlock) {
        data.blockNumber.history24hr = historyBlock;
        _Coin.PopulateData(data, chainData.history24hrs, chainData, "yesterday", "history48hrs", "history7day", "history14day");
      }
      times.timeNow = performance.now() / 1e3;
    }
    static PopulateData(data, period, chainData, when, before24hrs, current7day, before7day) {
      let sample;
      sample = data.holders.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.holders));
      sample = data.coinPriceUSDT.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.coinPriceStable));
      const marketCap = (0, import_big.Big)(period.marketCap);
      sample = data.marketCapUSDT.sample;
      _Coin.updateMeasure(sample[when], marketCap);
      const usdReservesBalance = (0, import_big.Big)(period.usdReservesBalance);
      sample = data.reservesCoin.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.reservesBalance));
      sample = data.reservesCoinUSDT.sample;
      _Coin.updateMeasure(sample[when], usdReservesBalance);
      sample = data.reservesPercent.sample;
      _Coin.updateMeasure(sample[when], usdReservesBalance.mul(100).div(marketCap));
      sample = data.liquidityToken.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.liquidityToken));
      sample = data.liquidityCoin.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.liquidityCoin));
      sample = data.liquidityCoinUSDT.sample;
      const usdLiquidityCoin = (0, import_big.Big)(period.usdLiquidityCoin);
      _Coin.updateMeasure(sample[when], usdLiquidityCoin);
      sample = data.liquidityPercent.sample;
      _Coin.updateMeasure(sample[when], usdLiquidityCoin.mul(100).div(marketCap));
      sample = data.stakedToken.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.staked));
      sample = data.stakedMultiplier.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.aveMultiplier));
      sample = data.stakedUSDT.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.usdStaked));
      const rewards = (0, import_big.Big)(period.rewards);
      sample = data.rewardsToken.sample;
      _Coin.updateMeasure(sample[when], rewards);
      sample = data.rewardsUSDT.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.usdRewards));
      const tokenPriceStable = (0, import_big.Big)(period.tokenPriceStable);
      sample = data.tokenPriceUSDT.sample;
      _Coin.updateMeasure(sample[when], tokenPriceStable);
      let volume24hr = rewards.minus(chainData[before24hrs].rewards);
      sample = data.rewardsToken24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr);
      sample = data.rewardsUSDT24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));
      let volume7day = (0, import_big.Big)(chainData[current7day].rewards).minus((0, import_big.Big)(chainData[before7day].rewards));
      sample = data.rewardsToken7day.sample;
      _Coin.updateMeasure(sample[when], volume7day);
      sample = data.rewardsUSDT7day.sample;
      _Coin.updateMeasure(sample[when], volume7day.mul(tokenPriceStable));
      const volumeTransfers = (0, import_big.Big)(period.volumeTransfers);
      sample = data.volumeTransfers.sample;
      _Coin.updateMeasure(sample[when], volumeTransfers);
      sample = data.volumeTransfersUSDT.sample;
      _Coin.updateMeasure(sample[when], volumeTransfers.mul(tokenPriceStable));
      volume24hr = volumeTransfers.minus(chainData[before24hrs].volumeTransfers);
      sample = data.volumeTransfers24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr);
      sample = data.volumeTransfersUSDT24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));
      const volumeBuy = (0, import_big.Big)(period.volumeBuy);
      sample = data.volumeBuy.sample;
      _Coin.updateMeasure(sample[when], volumeBuy);
      sample = data.volumeBuyUSDT.sample;
      _Coin.updateMeasure(sample[when], volumeBuy.mul(tokenPriceStable));
      volume24hr = volumeBuy.minus(chainData[before24hrs].volumeBuy);
      sample = data.volumeBuy24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr);
      sample = data.volumeBuyUSDT24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));
      const volumeSell = (0, import_big.Big)(period.volumeSell);
      sample = data.volumeSell.sample;
      _Coin.updateMeasure(sample[when], volumeSell);
      sample = data.volumeSellUSDT.sample;
      _Coin.updateMeasure(sample[when], volumeSell.mul(tokenPriceStable));
      volume24hr = volumeSell.minus(chainData[before24hrs].volumeSell);
      sample = data.volumeSell24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr);
      sample = data.volumeSellUSDT24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));
      const volumeTrade = (0, import_big.Big)(period.volumeTrade);
      sample = data.volumeTrade.sample;
      _Coin.updateMeasure(sample[when], volumeTrade);
      sample = data.volumeTradeUSDT.sample;
      _Coin.updateMeasure(sample[when], volumeTrade.mul(tokenPriceStable));
      volume24hr = volumeTrade.minus(chainData[before24hrs].volumeTrade);
      sample = data.volumeTrade24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr);
      sample = data.volumeTradeUSDT24hr.sample;
      _Coin.updateMeasure(sample[when], volume24hr.mul(tokenPriceStable));
      sample = data.tokenPriceCoin.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.tokenPriceCoin));
      sample = data.stakedPercent.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.stakedOfTotalSupplyPercent).mul(100));
      sample = data.supplyOnChain.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.supplyOnChainPercent).mul(100));
      sample = data.stakedOnChain.sample;
      _Coin.updateMeasure(sample[when], (0, import_big.Big)(period.stakedOfOnChainPercent).mul(100));
      for (var item of data) {
        const stat = item;
        stat.valuesCssUpdated = false;
      }
    }
    static CreateStat(classValue, precision) {
      const classChange = classValue + "-change";
      return {
        sample: _Coin.CreateSample(),
        precision,
        values: [...document.querySelectorAll(classValue)],
        change24hr: [...document.querySelectorAll(classChange)],
        valuesCssUpdated: false
      };
    }
    static updateMeasure(measure, value) {
      measure.previous = measure.interpolated;
      measure.current = value;
      if (measure.previous.eq(0)) {
        measure.previous = measure.interpolated = measure.current;
      }
    }
    static CreateSample() {
      return {
        now: _Coin.CreateValue(),
        yesterday: _Coin.CreateValue()
      };
    }
    static CreateValue() {
      return {
        current: (0, import_big.Big)(0),
        interpolated: (0, import_big.Big)(0),
        previous: (0, import_big.Big)(0)
      };
    }
    static calculateChanges(sample, delta) {
      const change = _Coin.calculateValueChanges(sample.now, delta);
      _Coin.calculateValueChanges(sample.yesterday, delta);
      return change;
    }
    static calculateValueChanges(value, delta) {
      let change = 0;
      if (!value.interpolated.eq(value.current)) {
        const diff = value.current.minus(value.previous).times(delta);
        const min = _Coin.minimum(value.previous, value.current);
        const max = _Coin.maximum(value.previous, value.current);
        const newValue = _Coin.minimum(max, _Coin.maximum(min, value.previous.plus(diff)));
        if (newValue.gt(value.interpolated)) {
          change = 1;
        } else if (newValue.lt(value.interpolated)) {
          change = -1;
        }
        value.interpolated = newValue;
      }
      return change;
    }
    static format(n) {
      var parts = n.toString().split(".");
      return parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (parts[1] ? "." + parts[1] : "");
    }
    static minimum(x, y) {
      if (x.gt(y))
        return x;
      return y;
    }
    static maximum(x, y) {
      if (x.lt(y))
        return x;
      return y;
    }
  };
  var Coin = _Coin;
  Coin.queryFrequency = 30;
  var TokenStats = class {
    constructor() {
      this.coins = [
        new Coin("bsc"),
        new Coin("eth"),
        new Coin("poly"),
        new Coin("ftm"),
        new Coin("avax")
      ];
      this.isRefreshing = false;
      this.ticker = document.getElementById("ticker");
      this.tickerOffset = 0;
      this.times = {
        timeCurrent: 0,
        timeNow: 0
      };
      this.skipFrame = -1;
      this.isPaused = true;
      this.colors = [
        "rgba(234,185,2,1)",
        "rgba(103,109,143,1)",
        "rgba(124,65,213,1)",
        "rgba(25,105,255,1)",
        "rgba(232,65,66,1)",
        "rgba(255,255,255,1)"
      ];
    }
    async Initialize() {
      this.data = Coin.CreateStats("all");
      for (let i = 0, ul = this.coins.length; i < ul; i++) {
        this.coins[i].Initialize();
      }
      this.times = {
        timeCurrent: performance.now() / 1e3,
        timeNow: performance.now() / 1e3
      };
      await this.Refresh();
      if (this.ticker) {
        this.ticker.style.display = "";
        this.tickerMoving = true;
        for (var e of document.querySelectorAll(".needs-live-data")) {
          e.classList.remove("needs-live-data");
        }
        this.ticker.addEventListener("mouseenter", (e2) => {
          this.tickerMoving = false;
        });
        this.ticker.addEventListener("mouseleave", (e2) => {
          this.tickerMoving = true;
        });
      }
    }
    Refresh() {
      return this.isRefreshing ? this.refresh : this.refresh = this.RefreshData();
    }
    async RefreshData() {
      this.isRefreshing = true;
      try {
        const request = await fetch("https://everrise.azurewebsites.net/stats");
        const blockchainData = await request.json();
        for (let i = 0, ul = this.coins.length; i < ul; i++) {
          const coin = this.coins[i];
          const chainData = blockchainData[coin.network];
          Coin.RefreshData(coin.data, chainData, coin.times);
        }
        Coin.RefreshData(this.data, blockchainData["unified"], this.times);
      } finally {
        this.isRefreshing = false;
      }
    }
    animate() {
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
      this.times.timeCurrent = performance.now() / 1e3;
      const delta = Math.min(1, (this.times.timeCurrent - this.times.timeNow) / 30);
      const deltaBig = (0, import_big.Big)(delta);
      for (var item of this.data) {
        const stat = item;
        const change = Coin.calculateChanges(stat.sample, deltaBig);
        const value = stat.sample.now.interpolated;
        let now;
        if (value.eq(0)) {
          now = "-";
        } else {
          now = Coin.format(stat.sample.now.interpolated.toFixed(stat.precision));
        }
        for (var element of stat.values) {
          element.textContent = now;
          if (!stat.valuesCssUpdated) {
            if (change > 0) {
              element.classList.remove("down");
              element.classList.add("up");
            } else if (change < 0) {
              element.classList.remove("up");
              element.classList.add("down");
            } else {
              element.classList.remove("down");
              element.classList.remove("up");
            }
          }
        }
        if (!stat.valuesCssUpdated) {
          stat.valuesCssUpdated = true;
        }
        let previous = stat.sample.yesterday.interpolated;
        if (!previous.eq(0)) {
          const change2 = stat.sample.now.interpolated.minus(previous).div(previous).mul(100);
          Coin.updatePercentChange(change2, stat.change24hr);
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
    updatePriceSvg() {
      const graphWidth = 420;
      var min = 1e6;
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
      const priceGraphs = [...document.querySelectorAll(".graph-priceRange")];
      if (priceGraphs.length > 0 && range > 0) {
        const unifiedPrice = this.data.tokenPriceUSDT.sample.now.interpolated.toNumber();
        let graphs = "";
        let prices = new Array(this.coins.length);
        let positions = new Array(this.coins.length);
        for (let i = 0, ul = this.coins.length; i < ul; i++) {
          const coin = this.coins[i];
          const value2 = coin.data.tokenPriceUSDT.sample.now.interpolated.toNumber();
          prices[i] = value2;
          if (value2 <= 0)
            continue;
          const x = (value2 - minX) * graphWidth / range;
          positions[i] = x;
          const xText = x.toFixed(2);
          for (var icon of document.querySelectorAll(`.graph-icon-${coin.network}`)) {
            icon.setAttribute("x", (x - 8).toFixed(2));
            icon.setAttribute("y", "8");
          }
          graphs += `
                <line x1="${xText}" x2="${xText}" y1="22" y2="56" stroke="${this.colors[i]}" stroke-width="2"/>`;
        }
        if (unifiedPrice > 0) {
          const x = (unifiedPrice - minX) * graphWidth / range;
          const xText = x.toFixed(2);
          for (var icon of document.querySelectorAll(`.graph-icon-rise`)) {
            icon.setAttribute("x", (x - 8).toFixed(2));
            icon.setAttribute("y", "0");
          }
          for (let i = 0, ul = prices.length; i < ul; i++) {
            const price = prices[i];
            if (price <= 0)
              continue;
            let xPos;
            if (positions[i] > x) {
              xPos = x + (positions[i] - x) / 2 - 20;
            } else {
              xPos = positions[i] + (x - positions[i]) / 2 - 20;
            }
            const percentChange = (price - unifiedPrice) * 100 / unifiedPrice;
            const yText = 24 + i * 8;
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
    Start() {
      if (this.isPaused) {
        this.isPaused = false;
        this.animationRequest = requestAnimationFrame(() => this.animate());
        setTimeout(() => this.Refresh(), 15e3);
        this.dataRefreshRequest = setInterval(() => this.Refresh(), 3e4);
      }
    }
    Pause() {
      if (this.isPaused)
        return;
      this.isPaused = true;
      clearInterval(this.dataRefreshRequest);
      cancelAnimationFrame(this.animationRequest);
    }
  };
  var stats = new TokenStats();
  stats.Initialize();
  stats.Start();
})();
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vbm9kZV9tb2R1bGVzL2JpZy5qcy9iaWcuanMiLCAiLi4vLi4vc2NyaXB0cy9tYWluLnRzIl0sCiAgInNvdXJjZXNDb250ZW50IjogWyIvKlxyXG4gKiAgYmlnLmpzIHY2LjEuMVxyXG4gKiAgQSBzbWFsbCwgZmFzdCwgZWFzeS10by11c2UgbGlicmFyeSBmb3IgYXJiaXRyYXJ5LXByZWNpc2lvbiBkZWNpbWFsIGFyaXRobWV0aWMuXHJcbiAqICBDb3B5cmlnaHQgKGMpIDIwMjEgTWljaGFlbCBNY2xhdWdobGluXHJcbiAqICBodHRwczovL2dpdGh1Yi5jb20vTWlrZU1jbC9iaWcuanMvTElDRU5DRS5tZFxyXG4gKi9cclxuOyhmdW5jdGlvbiAoR0xPQkFMKSB7XHJcbiAgJ3VzZSBzdHJpY3QnO1xyXG4gIHZhciBCaWcsXHJcblxyXG5cclxuLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqIEVESVRBQkxFIERFRkFVTFRTICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqL1xyXG5cclxuXHJcbiAgICAvLyBUaGUgZGVmYXVsdCB2YWx1ZXMgYmVsb3cgbXVzdCBiZSBpbnRlZ2VycyB3aXRoaW4gdGhlIHN0YXRlZCByYW5nZXMuXHJcblxyXG4gICAgLypcclxuICAgICAqIFRoZSBtYXhpbXVtIG51bWJlciBvZiBkZWNpbWFsIHBsYWNlcyAoRFApIG9mIHRoZSByZXN1bHRzIG9mIG9wZXJhdGlvbnMgaW52b2x2aW5nIGRpdmlzaW9uOlxyXG4gICAgICogZGl2IGFuZCBzcXJ0LCBhbmQgcG93IHdpdGggbmVnYXRpdmUgZXhwb25lbnRzLlxyXG4gICAgICovXHJcbiAgICBEUCA9IDIwLCAgICAgICAgICAgIC8vIDAgdG8gTUFYX0RQXHJcblxyXG4gICAgLypcclxuICAgICAqIFRoZSByb3VuZGluZyBtb2RlIChSTSkgdXNlZCB3aGVuIHJvdW5kaW5nIHRvIHRoZSBhYm92ZSBkZWNpbWFsIHBsYWNlcy5cclxuICAgICAqXHJcbiAgICAgKiAgMCAgVG93YXJkcyB6ZXJvIChpLmUuIHRydW5jYXRlLCBubyByb3VuZGluZykuICAgICAgIChST1VORF9ET1dOKVxyXG4gICAgICogIDEgIFRvIG5lYXJlc3QgbmVpZ2hib3VyLiBJZiBlcXVpZGlzdGFudCwgcm91bmQgdXAuICAoUk9VTkRfSEFMRl9VUClcclxuICAgICAqICAyICBUbyBuZWFyZXN0IG5laWdoYm91ci4gSWYgZXF1aWRpc3RhbnQsIHRvIGV2ZW4uICAgKFJPVU5EX0hBTEZfRVZFTilcclxuICAgICAqICAzICBBd2F5IGZyb20gemVyby4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgKFJPVU5EX1VQKVxyXG4gICAgICovXHJcbiAgICBSTSA9IDEsICAgICAgICAgICAgIC8vIDAsIDEsIDIgb3IgM1xyXG5cclxuICAgIC8vIFRoZSBtYXhpbXVtIHZhbHVlIG9mIERQIGFuZCBCaWcuRFAuXHJcbiAgICBNQVhfRFAgPSAxRTYsICAgICAgIC8vIDAgdG8gMTAwMDAwMFxyXG5cclxuICAgIC8vIFRoZSBtYXhpbXVtIG1hZ25pdHVkZSBvZiB0aGUgZXhwb25lbnQgYXJndW1lbnQgdG8gdGhlIHBvdyBtZXRob2QuXHJcbiAgICBNQVhfUE9XRVIgPSAxRTYsICAgIC8vIDEgdG8gMTAwMDAwMFxyXG5cclxuICAgIC8qXHJcbiAgICAgKiBUaGUgbmVnYXRpdmUgZXhwb25lbnQgKE5FKSBhdCBhbmQgYmVuZWF0aCB3aGljaCB0b1N0cmluZyByZXR1cm5zIGV4cG9uZW50aWFsIG5vdGF0aW9uLlxyXG4gICAgICogKEphdmFTY3JpcHQgbnVtYmVyczogLTcpXHJcbiAgICAgKiAtMTAwMDAwMCBpcyB0aGUgbWluaW11bSByZWNvbW1lbmRlZCBleHBvbmVudCB2YWx1ZSBvZiBhIEJpZy5cclxuICAgICAqL1xyXG4gICAgTkUgPSAtNywgICAgICAgICAgICAvLyAwIHRvIC0xMDAwMDAwXHJcblxyXG4gICAgLypcclxuICAgICAqIFRoZSBwb3NpdGl2ZSBleHBvbmVudCAoUEUpIGF0IGFuZCBhYm92ZSB3aGljaCB0b1N0cmluZyByZXR1cm5zIGV4cG9uZW50aWFsIG5vdGF0aW9uLlxyXG4gICAgICogKEphdmFTY3JpcHQgbnVtYmVyczogMjEpXHJcbiAgICAgKiAxMDAwMDAwIGlzIHRoZSBtYXhpbXVtIHJlY29tbWVuZGVkIGV4cG9uZW50IHZhbHVlIG9mIGEgQmlnLCBidXQgdGhpcyBsaW1pdCBpcyBub3QgZW5mb3JjZWQuXHJcbiAgICAgKi9cclxuICAgIFBFID0gMjEsICAgICAgICAgICAgLy8gMCB0byAxMDAwMDAwXHJcblxyXG4gICAgLypcclxuICAgICAqIFdoZW4gdHJ1ZSwgYW4gZXJyb3Igd2lsbCBiZSB0aHJvd24gaWYgYSBwcmltaXRpdmUgbnVtYmVyIGlzIHBhc3NlZCB0byB0aGUgQmlnIGNvbnN0cnVjdG9yLFxyXG4gICAgICogb3IgaWYgdmFsdWVPZiBpcyBjYWxsZWQsIG9yIGlmIHRvTnVtYmVyIGlzIGNhbGxlZCBvbiBhIEJpZyB3aGljaCBjYW5ub3QgYmUgY29udmVydGVkIHRvIGFcclxuICAgICAqIHByaW1pdGl2ZSBudW1iZXIgd2l0aG91dCBhIGxvc3Mgb2YgcHJlY2lzaW9uLlxyXG4gICAgICovXHJcbiAgICBTVFJJQ1QgPSBmYWxzZSwgICAgIC8vIHRydWUgb3IgZmFsc2VcclxuXHJcblxyXG4vKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiovXHJcblxyXG5cclxuICAgIC8vIEVycm9yIG1lc3NhZ2VzLlxyXG4gICAgTkFNRSA9ICdbYmlnLmpzXSAnLFxyXG4gICAgSU5WQUxJRCA9IE5BTUUgKyAnSW52YWxpZCAnLFxyXG4gICAgSU5WQUxJRF9EUCA9IElOVkFMSUQgKyAnZGVjaW1hbCBwbGFjZXMnLFxyXG4gICAgSU5WQUxJRF9STSA9IElOVkFMSUQgKyAncm91bmRpbmcgbW9kZScsXHJcbiAgICBESVZfQllfWkVSTyA9IE5BTUUgKyAnRGl2aXNpb24gYnkgemVybycsXHJcblxyXG4gICAgLy8gVGhlIHNoYXJlZCBwcm90b3R5cGUgb2JqZWN0LlxyXG4gICAgUCA9IHt9LFxyXG4gICAgVU5ERUZJTkVEID0gdm9pZCAwLFxyXG4gICAgTlVNRVJJQyA9IC9eLT8oXFxkKyhcXC5cXGQqKT98XFwuXFxkKykoZVsrLV0/XFxkKyk/JC9pO1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBDcmVhdGUgYW5kIHJldHVybiBhIEJpZyBjb25zdHJ1Y3Rvci5cclxuICAgKi9cclxuICBmdW5jdGlvbiBfQmlnXygpIHtcclxuXHJcbiAgICAvKlxyXG4gICAgICogVGhlIEJpZyBjb25zdHJ1Y3RvciBhbmQgZXhwb3J0ZWQgZnVuY3Rpb24uXHJcbiAgICAgKiBDcmVhdGUgYW5kIHJldHVybiBhIG5ldyBpbnN0YW5jZSBvZiBhIEJpZyBudW1iZXIgb2JqZWN0LlxyXG4gICAgICpcclxuICAgICAqIG4ge251bWJlcnxzdHJpbmd8QmlnfSBBIG51bWVyaWMgdmFsdWUuXHJcbiAgICAgKi9cclxuICAgIGZ1bmN0aW9uIEJpZyhuKSB7XHJcbiAgICAgIHZhciB4ID0gdGhpcztcclxuXHJcbiAgICAgIC8vIEVuYWJsZSBjb25zdHJ1Y3RvciB1c2FnZSB3aXRob3V0IG5ldy5cclxuICAgICAgaWYgKCEoeCBpbnN0YW5jZW9mIEJpZykpIHJldHVybiBuID09PSBVTkRFRklORUQgPyBfQmlnXygpIDogbmV3IEJpZyhuKTtcclxuXHJcbiAgICAgIC8vIER1cGxpY2F0ZS5cclxuICAgICAgaWYgKG4gaW5zdGFuY2VvZiBCaWcpIHtcclxuICAgICAgICB4LnMgPSBuLnM7XHJcbiAgICAgICAgeC5lID0gbi5lO1xyXG4gICAgICAgIHguYyA9IG4uYy5zbGljZSgpO1xyXG4gICAgICB9IGVsc2Uge1xyXG4gICAgICAgIGlmICh0eXBlb2YgbiAhPT0gJ3N0cmluZycpIHtcclxuICAgICAgICAgIGlmIChCaWcuc3RyaWN0ID09PSB0cnVlKSB7XHJcbiAgICAgICAgICAgIHRocm93IFR5cGVFcnJvcihJTlZBTElEICsgJ251bWJlcicpO1xyXG4gICAgICAgICAgfVxyXG5cclxuICAgICAgICAgIC8vIE1pbnVzIHplcm8/XHJcbiAgICAgICAgICBuID0gbiA9PT0gMCAmJiAxIC8gbiA8IDAgPyAnLTAnIDogU3RyaW5nKG4pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcGFyc2UoeCwgbik7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIC8vIFJldGFpbiBhIHJlZmVyZW5jZSB0byB0aGlzIEJpZyBjb25zdHJ1Y3Rvci5cclxuICAgICAgLy8gU2hhZG93IEJpZy5wcm90b3R5cGUuY29uc3RydWN0b3Igd2hpY2ggcG9pbnRzIHRvIE9iamVjdC5cclxuICAgICAgeC5jb25zdHJ1Y3RvciA9IEJpZztcclxuICAgIH1cclxuXHJcbiAgICBCaWcucHJvdG90eXBlID0gUDtcclxuICAgIEJpZy5EUCA9IERQO1xyXG4gICAgQmlnLlJNID0gUk07XHJcbiAgICBCaWcuTkUgPSBORTtcclxuICAgIEJpZy5QRSA9IFBFO1xyXG4gICAgQmlnLnN0cmljdCA9IFNUUklDVDtcclxuICAgIEJpZy5yb3VuZERvd24gPSAwO1xyXG4gICAgQmlnLnJvdW5kSGFsZlVwID0gMTtcclxuICAgIEJpZy5yb3VuZEhhbGZFdmVuID0gMjtcclxuICAgIEJpZy5yb3VuZFVwID0gMztcclxuXHJcbiAgICByZXR1cm4gQmlnO1xyXG4gIH1cclxuXHJcblxyXG4gIC8qXHJcbiAgICogUGFyc2UgdGhlIG51bWJlciBvciBzdHJpbmcgdmFsdWUgcGFzc2VkIHRvIGEgQmlnIGNvbnN0cnVjdG9yLlxyXG4gICAqXHJcbiAgICogeCB7QmlnfSBBIEJpZyBudW1iZXIgaW5zdGFuY2UuXHJcbiAgICogbiB7bnVtYmVyfHN0cmluZ30gQSBudW1lcmljIHZhbHVlLlxyXG4gICAqL1xyXG4gIGZ1bmN0aW9uIHBhcnNlKHgsIG4pIHtcclxuICAgIHZhciBlLCBpLCBubDtcclxuXHJcbiAgICBpZiAoIU5VTUVSSUMudGVzdChuKSkge1xyXG4gICAgICB0aHJvdyBFcnJvcihJTlZBTElEICsgJ251bWJlcicpO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIERldGVybWluZSBzaWduLlxyXG4gICAgeC5zID0gbi5jaGFyQXQoMCkgPT0gJy0nID8gKG4gPSBuLnNsaWNlKDEpLCAtMSkgOiAxO1xyXG5cclxuICAgIC8vIERlY2ltYWwgcG9pbnQ/XHJcbiAgICBpZiAoKGUgPSBuLmluZGV4T2YoJy4nKSkgPiAtMSkgbiA9IG4ucmVwbGFjZSgnLicsICcnKTtcclxuXHJcbiAgICAvLyBFeHBvbmVudGlhbCBmb3JtP1xyXG4gICAgaWYgKChpID0gbi5zZWFyY2goL2UvaSkpID4gMCkge1xyXG5cclxuICAgICAgLy8gRGV0ZXJtaW5lIGV4cG9uZW50LlxyXG4gICAgICBpZiAoZSA8IDApIGUgPSBpO1xyXG4gICAgICBlICs9ICtuLnNsaWNlKGkgKyAxKTtcclxuICAgICAgbiA9IG4uc3Vic3RyaW5nKDAsIGkpO1xyXG4gICAgfSBlbHNlIGlmIChlIDwgMCkge1xyXG5cclxuICAgICAgLy8gSW50ZWdlci5cclxuICAgICAgZSA9IG4ubGVuZ3RoO1xyXG4gICAgfVxyXG5cclxuICAgIG5sID0gbi5sZW5ndGg7XHJcblxyXG4gICAgLy8gRGV0ZXJtaW5lIGxlYWRpbmcgemVyb3MuXHJcbiAgICBmb3IgKGkgPSAwOyBpIDwgbmwgJiYgbi5jaGFyQXQoaSkgPT0gJzAnOykgKytpO1xyXG5cclxuICAgIGlmIChpID09IG5sKSB7XHJcblxyXG4gICAgICAvLyBaZXJvLlxyXG4gICAgICB4LmMgPSBbeC5lID0gMF07XHJcbiAgICB9IGVsc2Uge1xyXG5cclxuICAgICAgLy8gRGV0ZXJtaW5lIHRyYWlsaW5nIHplcm9zLlxyXG4gICAgICBmb3IgKDsgbmwgPiAwICYmIG4uY2hhckF0KC0tbmwpID09ICcwJzspO1xyXG4gICAgICB4LmUgPSBlIC0gaSAtIDE7XHJcbiAgICAgIHguYyA9IFtdO1xyXG5cclxuICAgICAgLy8gQ29udmVydCBzdHJpbmcgdG8gYXJyYXkgb2YgZGlnaXRzIHdpdGhvdXQgbGVhZGluZy90cmFpbGluZyB6ZXJvcy5cclxuICAgICAgZm9yIChlID0gMDsgaSA8PSBubDspIHguY1tlKytdID0gK24uY2hhckF0KGkrKyk7XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIHg7XHJcbiAgfVxyXG5cclxuXHJcbiAgLypcclxuICAgKiBSb3VuZCBCaWcgeCB0byBhIG1heGltdW0gb2Ygc2Qgc2lnbmlmaWNhbnQgZGlnaXRzIHVzaW5nIHJvdW5kaW5nIG1vZGUgcm0uXHJcbiAgICpcclxuICAgKiB4IHtCaWd9IFRoZSBCaWcgdG8gcm91bmQuXHJcbiAgICogc2Qge251bWJlcn0gU2lnbmlmaWNhbnQgZGlnaXRzOiBpbnRlZ2VyLCAwIHRvIE1BWF9EUCBpbmNsdXNpdmUuXHJcbiAgICogcm0ge251bWJlcn0gUm91bmRpbmcgbW9kZTogMCAoZG93biksIDEgKGhhbGYtdXApLCAyIChoYWxmLWV2ZW4pIG9yIDMgKHVwKS5cclxuICAgKiBbbW9yZV0ge2Jvb2xlYW59IFdoZXRoZXIgdGhlIHJlc3VsdCBvZiBkaXZpc2lvbiB3YXMgdHJ1bmNhdGVkLlxyXG4gICAqL1xyXG4gIGZ1bmN0aW9uIHJvdW5kKHgsIHNkLCBybSwgbW9yZSkge1xyXG4gICAgdmFyIHhjID0geC5jO1xyXG5cclxuICAgIGlmIChybSA9PT0gVU5ERUZJTkVEKSBybSA9IHguY29uc3RydWN0b3IuUk07XHJcbiAgICBpZiAocm0gIT09IDAgJiYgcm0gIT09IDEgJiYgcm0gIT09IDIgJiYgcm0gIT09IDMpIHtcclxuICAgICAgdGhyb3cgRXJyb3IoSU5WQUxJRF9STSk7XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKHNkIDwgMSkge1xyXG4gICAgICBtb3JlID1cclxuICAgICAgICBybSA9PT0gMyAmJiAobW9yZSB8fCAhIXhjWzBdKSB8fCBzZCA9PT0gMCAmJiAoXHJcbiAgICAgICAgcm0gPT09IDEgJiYgeGNbMF0gPj0gNSB8fFxyXG4gICAgICAgIHJtID09PSAyICYmICh4Y1swXSA+IDUgfHwgeGNbMF0gPT09IDUgJiYgKG1vcmUgfHwgeGNbMV0gIT09IFVOREVGSU5FRCkpXHJcbiAgICAgICk7XHJcblxyXG4gICAgICB4Yy5sZW5ndGggPSAxO1xyXG5cclxuICAgICAgaWYgKG1vcmUpIHtcclxuXHJcbiAgICAgICAgLy8gMSwgMC4xLCAwLjAxLCAwLjAwMSwgMC4wMDAxIGV0Yy5cclxuICAgICAgICB4LmUgPSB4LmUgLSBzZCArIDE7XHJcbiAgICAgICAgeGNbMF0gPSAxO1xyXG4gICAgICB9IGVsc2Uge1xyXG5cclxuICAgICAgICAvLyBaZXJvLlxyXG4gICAgICAgIHhjWzBdID0geC5lID0gMDtcclxuICAgICAgfVxyXG4gICAgfSBlbHNlIGlmIChzZCA8IHhjLmxlbmd0aCkge1xyXG5cclxuICAgICAgLy8geGNbc2RdIGlzIHRoZSBkaWdpdCBhZnRlciB0aGUgZGlnaXQgdGhhdCBtYXkgYmUgcm91bmRlZCB1cC5cclxuICAgICAgbW9yZSA9XHJcbiAgICAgICAgcm0gPT09IDEgJiYgeGNbc2RdID49IDUgfHxcclxuICAgICAgICBybSA9PT0gMiAmJiAoeGNbc2RdID4gNSB8fCB4Y1tzZF0gPT09IDUgJiZcclxuICAgICAgICAgIChtb3JlIHx8IHhjW3NkICsgMV0gIT09IFVOREVGSU5FRCB8fCB4Y1tzZCAtIDFdICYgMSkpIHx8XHJcbiAgICAgICAgcm0gPT09IDMgJiYgKG1vcmUgfHwgISF4Y1swXSk7XHJcblxyXG4gICAgICAvLyBSZW1vdmUgYW55IGRpZ2l0cyBhZnRlciB0aGUgcmVxdWlyZWQgcHJlY2lzaW9uLlxyXG4gICAgICB4Yy5sZW5ndGggPSBzZC0tO1xyXG5cclxuICAgICAgLy8gUm91bmQgdXA/XHJcbiAgICAgIGlmIChtb3JlKSB7XHJcblxyXG4gICAgICAgIC8vIFJvdW5kaW5nIHVwIG1heSBtZWFuIHRoZSBwcmV2aW91cyBkaWdpdCBoYXMgdG8gYmUgcm91bmRlZCB1cC5cclxuICAgICAgICBmb3IgKDsgKyt4Y1tzZF0gPiA5Oykge1xyXG4gICAgICAgICAgeGNbc2RdID0gMDtcclxuICAgICAgICAgIGlmICghc2QtLSkge1xyXG4gICAgICAgICAgICArK3guZTtcclxuICAgICAgICAgICAgeGMudW5zaGlmdCgxKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIC8vIFJlbW92ZSB0cmFpbGluZyB6ZXJvcy5cclxuICAgICAgZm9yIChzZCA9IHhjLmxlbmd0aDsgIXhjWy0tc2RdOykgeGMucG9wKCk7XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIHg7XHJcbiAgfVxyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gYSBzdHJpbmcgcmVwcmVzZW50aW5nIHRoZSB2YWx1ZSBvZiBCaWcgeCBpbiBub3JtYWwgb3IgZXhwb25lbnRpYWwgbm90YXRpb24uXHJcbiAgICogSGFuZGxlcyBQLnRvRXhwb25lbnRpYWwsIFAudG9GaXhlZCwgUC50b0pTT04sIFAudG9QcmVjaXNpb24sIFAudG9TdHJpbmcgYW5kIFAudmFsdWVPZi5cclxuICAgKi9cclxuICBmdW5jdGlvbiBzdHJpbmdpZnkoeCwgZG9FeHBvbmVudGlhbCwgaXNOb256ZXJvKSB7XHJcbiAgICB2YXIgZSA9IHguZSxcclxuICAgICAgcyA9IHguYy5qb2luKCcnKSxcclxuICAgICAgbiA9IHMubGVuZ3RoO1xyXG5cclxuICAgIC8vIEV4cG9uZW50aWFsIG5vdGF0aW9uP1xyXG4gICAgaWYgKGRvRXhwb25lbnRpYWwpIHtcclxuICAgICAgcyA9IHMuY2hhckF0KDApICsgKG4gPiAxID8gJy4nICsgcy5zbGljZSgxKSA6ICcnKSArIChlIDwgMCA/ICdlJyA6ICdlKycpICsgZTtcclxuXHJcbiAgICAvLyBOb3JtYWwgbm90YXRpb24uXHJcbiAgICB9IGVsc2UgaWYgKGUgPCAwKSB7XHJcbiAgICAgIGZvciAoOyArK2U7KSBzID0gJzAnICsgcztcclxuICAgICAgcyA9ICcwLicgKyBzO1xyXG4gICAgfSBlbHNlIGlmIChlID4gMCkge1xyXG4gICAgICBpZiAoKytlID4gbikge1xyXG4gICAgICAgIGZvciAoZSAtPSBuOyBlLS07KSBzICs9ICcwJztcclxuICAgICAgfSBlbHNlIGlmIChlIDwgbikge1xyXG4gICAgICAgIHMgPSBzLnNsaWNlKDAsIGUpICsgJy4nICsgcy5zbGljZShlKTtcclxuICAgICAgfVxyXG4gICAgfSBlbHNlIGlmIChuID4gMSkge1xyXG4gICAgICBzID0gcy5jaGFyQXQoMCkgKyAnLicgKyBzLnNsaWNlKDEpO1xyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiB4LnMgPCAwICYmIGlzTm9uemVybyA/ICctJyArIHMgOiBzO1xyXG4gIH1cclxuXHJcblxyXG4gIC8vIFByb3RvdHlwZS9pbnN0YW5jZSBtZXRob2RzXHJcblxyXG5cclxuICAvKlxyXG4gICAqIFJldHVybiBhIG5ldyBCaWcgd2hvc2UgdmFsdWUgaXMgdGhlIGFic29sdXRlIHZhbHVlIG9mIHRoaXMgQmlnLlxyXG4gICAqL1xyXG4gIFAuYWJzID0gZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIHggPSBuZXcgdGhpcy5jb25zdHJ1Y3Rvcih0aGlzKTtcclxuICAgIHgucyA9IDE7XHJcbiAgICByZXR1cm4geDtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gMSBpZiB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgaXMgZ3JlYXRlciB0aGFuIHRoZSB2YWx1ZSBvZiBCaWcgeSxcclxuICAgKiAgICAgICAtMSBpZiB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgaXMgbGVzcyB0aGFuIHRoZSB2YWx1ZSBvZiBCaWcgeSwgb3JcclxuICAgKiAgICAgICAgMCBpZiB0aGV5IGhhdmUgdGhlIHNhbWUgdmFsdWUuXHJcbiAgICovXHJcbiAgUC5jbXAgPSBmdW5jdGlvbiAoeSkge1xyXG4gICAgdmFyIGlzbmVnLFxyXG4gICAgICB4ID0gdGhpcyxcclxuICAgICAgeGMgPSB4LmMsXHJcbiAgICAgIHljID0gKHkgPSBuZXcgeC5jb25zdHJ1Y3Rvcih5KSkuYyxcclxuICAgICAgaSA9IHgucyxcclxuICAgICAgaiA9IHkucyxcclxuICAgICAgayA9IHguZSxcclxuICAgICAgbCA9IHkuZTtcclxuXHJcbiAgICAvLyBFaXRoZXIgemVybz9cclxuICAgIGlmICgheGNbMF0gfHwgIXljWzBdKSByZXR1cm4gIXhjWzBdID8gIXljWzBdID8gMCA6IC1qIDogaTtcclxuXHJcbiAgICAvLyBTaWducyBkaWZmZXI/XHJcbiAgICBpZiAoaSAhPSBqKSByZXR1cm4gaTtcclxuXHJcbiAgICBpc25lZyA9IGkgPCAwO1xyXG5cclxuICAgIC8vIENvbXBhcmUgZXhwb25lbnRzLlxyXG4gICAgaWYgKGsgIT0gbCkgcmV0dXJuIGsgPiBsIF4gaXNuZWcgPyAxIDogLTE7XHJcblxyXG4gICAgaiA9IChrID0geGMubGVuZ3RoKSA8IChsID0geWMubGVuZ3RoKSA/IGsgOiBsO1xyXG5cclxuICAgIC8vIENvbXBhcmUgZGlnaXQgYnkgZGlnaXQuXHJcbiAgICBmb3IgKGkgPSAtMTsgKytpIDwgajspIHtcclxuICAgICAgaWYgKHhjW2ldICE9IHljW2ldKSByZXR1cm4geGNbaV0gPiB5Y1tpXSBeIGlzbmVnID8gMSA6IC0xO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIENvbXBhcmUgbGVuZ3Rocy5cclxuICAgIHJldHVybiBrID09IGwgPyAwIDogayA+IGwgXiBpc25lZyA/IDEgOiAtMTtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gYSBuZXcgQmlnIHdob3NlIHZhbHVlIGlzIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZyBkaXZpZGVkIGJ5IHRoZSB2YWx1ZSBvZiBCaWcgeSwgcm91bmRlZCxcclxuICAgKiBpZiBuZWNlc3NhcnksIHRvIGEgbWF4aW11bSBvZiBCaWcuRFAgZGVjaW1hbCBwbGFjZXMgdXNpbmcgcm91bmRpbmcgbW9kZSBCaWcuUk0uXHJcbiAgICovXHJcbiAgUC5kaXYgPSBmdW5jdGlvbiAoeSkge1xyXG4gICAgdmFyIHggPSB0aGlzLFxyXG4gICAgICBCaWcgPSB4LmNvbnN0cnVjdG9yLFxyXG4gICAgICBhID0geC5jLCAgICAgICAgICAgICAgICAgIC8vIGRpdmlkZW5kXHJcbiAgICAgIGIgPSAoeSA9IG5ldyBCaWcoeSkpLmMsICAgLy8gZGl2aXNvclxyXG4gICAgICBrID0geC5zID09IHkucyA/IDEgOiAtMSxcclxuICAgICAgZHAgPSBCaWcuRFA7XHJcblxyXG4gICAgaWYgKGRwICE9PSB+fmRwIHx8IGRwIDwgMCB8fCBkcCA+IE1BWF9EUCkge1xyXG4gICAgICB0aHJvdyBFcnJvcihJTlZBTElEX0RQKTtcclxuICAgIH1cclxuXHJcbiAgICAvLyBEaXZpc29yIGlzIHplcm8/XHJcbiAgICBpZiAoIWJbMF0pIHtcclxuICAgICAgdGhyb3cgRXJyb3IoRElWX0JZX1pFUk8pO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIERpdmlkZW5kIGlzIDA/IFJldHVybiArLTAuXHJcbiAgICBpZiAoIWFbMF0pIHtcclxuICAgICAgeS5zID0gaztcclxuICAgICAgeS5jID0gW3kuZSA9IDBdO1xyXG4gICAgICByZXR1cm4geTtcclxuICAgIH1cclxuXHJcbiAgICB2YXIgYmwsIGJ0LCBuLCBjbXAsIHJpLFxyXG4gICAgICBieiA9IGIuc2xpY2UoKSxcclxuICAgICAgYWkgPSBibCA9IGIubGVuZ3RoLFxyXG4gICAgICBhbCA9IGEubGVuZ3RoLFxyXG4gICAgICByID0gYS5zbGljZSgwLCBibCksICAgLy8gcmVtYWluZGVyXHJcbiAgICAgIHJsID0gci5sZW5ndGgsXHJcbiAgICAgIHEgPSB5LCAgICAgICAgICAgICAgICAvLyBxdW90aWVudFxyXG4gICAgICBxYyA9IHEuYyA9IFtdLFxyXG4gICAgICBxaSA9IDAsXHJcbiAgICAgIHAgPSBkcCArIChxLmUgPSB4LmUgLSB5LmUpICsgMTsgICAgLy8gcHJlY2lzaW9uIG9mIHRoZSByZXN1bHRcclxuXHJcbiAgICBxLnMgPSBrO1xyXG4gICAgayA9IHAgPCAwID8gMCA6IHA7XHJcblxyXG4gICAgLy8gQ3JlYXRlIHZlcnNpb24gb2YgZGl2aXNvciB3aXRoIGxlYWRpbmcgemVyby5cclxuICAgIGJ6LnVuc2hpZnQoMCk7XHJcblxyXG4gICAgLy8gQWRkIHplcm9zIHRvIG1ha2UgcmVtYWluZGVyIGFzIGxvbmcgYXMgZGl2aXNvci5cclxuICAgIGZvciAoOyBybCsrIDwgYmw7KSByLnB1c2goMCk7XHJcblxyXG4gICAgZG8ge1xyXG5cclxuICAgICAgLy8gbiBpcyBob3cgbWFueSB0aW1lcyB0aGUgZGl2aXNvciBnb2VzIGludG8gY3VycmVudCByZW1haW5kZXIuXHJcbiAgICAgIGZvciAobiA9IDA7IG4gPCAxMDsgbisrKSB7XHJcblxyXG4gICAgICAgIC8vIENvbXBhcmUgZGl2aXNvciBhbmQgcmVtYWluZGVyLlxyXG4gICAgICAgIGlmIChibCAhPSAocmwgPSByLmxlbmd0aCkpIHtcclxuICAgICAgICAgIGNtcCA9IGJsID4gcmwgPyAxIDogLTE7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIGZvciAocmkgPSAtMSwgY21wID0gMDsgKytyaSA8IGJsOykge1xyXG4gICAgICAgICAgICBpZiAoYltyaV0gIT0gcltyaV0pIHtcclxuICAgICAgICAgICAgICBjbXAgPSBiW3JpXSA+IHJbcmldID8gMSA6IC0xO1xyXG4gICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBJZiBkaXZpc29yIDwgcmVtYWluZGVyLCBzdWJ0cmFjdCBkaXZpc29yIGZyb20gcmVtYWluZGVyLlxyXG4gICAgICAgIGlmIChjbXAgPCAwKSB7XHJcblxyXG4gICAgICAgICAgLy8gUmVtYWluZGVyIGNhbid0IGJlIG1vcmUgdGhhbiAxIGRpZ2l0IGxvbmdlciB0aGFuIGRpdmlzb3IuXHJcbiAgICAgICAgICAvLyBFcXVhbGlzZSBsZW5ndGhzIHVzaW5nIGRpdmlzb3Igd2l0aCBleHRyYSBsZWFkaW5nIHplcm8/XHJcbiAgICAgICAgICBmb3IgKGJ0ID0gcmwgPT0gYmwgPyBiIDogYno7IHJsOykge1xyXG4gICAgICAgICAgICBpZiAoclstLXJsXSA8IGJ0W3JsXSkge1xyXG4gICAgICAgICAgICAgIHJpID0gcmw7XHJcbiAgICAgICAgICAgICAgZm9yICg7IHJpICYmICFyWy0tcmldOykgcltyaV0gPSA5O1xyXG4gICAgICAgICAgICAgIC0tcltyaV07XHJcbiAgICAgICAgICAgICAgcltybF0gKz0gMTA7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcltybF0gLT0gYnRbcmxdO1xyXG4gICAgICAgICAgfVxyXG5cclxuICAgICAgICAgIGZvciAoOyAhclswXTspIHIuc2hpZnQoKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgfVxyXG4gICAgICB9XHJcblxyXG4gICAgICAvLyBBZGQgdGhlIGRpZ2l0IG4gdG8gdGhlIHJlc3VsdCBhcnJheS5cclxuICAgICAgcWNbcWkrK10gPSBjbXAgPyBuIDogKytuO1xyXG5cclxuICAgICAgLy8gVXBkYXRlIHRoZSByZW1haW5kZXIuXHJcbiAgICAgIGlmIChyWzBdICYmIGNtcCkgcltybF0gPSBhW2FpXSB8fCAwO1xyXG4gICAgICBlbHNlIHIgPSBbYVthaV1dO1xyXG5cclxuICAgIH0gd2hpbGUgKChhaSsrIDwgYWwgfHwgclswXSAhPT0gVU5ERUZJTkVEKSAmJiBrLS0pO1xyXG5cclxuICAgIC8vIExlYWRpbmcgemVybz8gRG8gbm90IHJlbW92ZSBpZiByZXN1bHQgaXMgc2ltcGx5IHplcm8gKHFpID09IDEpLlxyXG4gICAgaWYgKCFxY1swXSAmJiBxaSAhPSAxKSB7XHJcblxyXG4gICAgICAvLyBUaGVyZSBjYW4ndCBiZSBtb3JlIHRoYW4gb25lIHplcm8uXHJcbiAgICAgIHFjLnNoaWZ0KCk7XHJcbiAgICAgIHEuZS0tO1xyXG4gICAgICBwLS07XHJcbiAgICB9XHJcblxyXG4gICAgLy8gUm91bmQ/XHJcbiAgICBpZiAocWkgPiBwKSByb3VuZChxLCBwLCBCaWcuUk0sIHJbMF0gIT09IFVOREVGSU5FRCk7XHJcblxyXG4gICAgcmV0dXJuIHE7XHJcbiAgfTtcclxuXHJcblxyXG4gIC8qXHJcbiAgICogUmV0dXJuIHRydWUgaWYgdGhlIHZhbHVlIG9mIHRoaXMgQmlnIGlzIGVxdWFsIHRvIHRoZSB2YWx1ZSBvZiBCaWcgeSwgb3RoZXJ3aXNlIHJldHVybiBmYWxzZS5cclxuICAgKi9cclxuICBQLmVxID0gZnVuY3Rpb24gKHkpIHtcclxuICAgIHJldHVybiB0aGlzLmNtcCh5KSA9PT0gMDtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gdHJ1ZSBpZiB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgaXMgZ3JlYXRlciB0aGFuIHRoZSB2YWx1ZSBvZiBCaWcgeSwgb3RoZXJ3aXNlIHJldHVyblxyXG4gICAqIGZhbHNlLlxyXG4gICAqL1xyXG4gIFAuZ3QgPSBmdW5jdGlvbiAoeSkge1xyXG4gICAgcmV0dXJuIHRoaXMuY21wKHkpID4gMDtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gdHJ1ZSBpZiB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgaXMgZ3JlYXRlciB0aGFuIG9yIGVxdWFsIHRvIHRoZSB2YWx1ZSBvZiBCaWcgeSwgb3RoZXJ3aXNlXHJcbiAgICogcmV0dXJuIGZhbHNlLlxyXG4gICAqL1xyXG4gIFAuZ3RlID0gZnVuY3Rpb24gKHkpIHtcclxuICAgIHJldHVybiB0aGlzLmNtcCh5KSA+IC0xO1xyXG4gIH07XHJcblxyXG5cclxuICAvKlxyXG4gICAqIFJldHVybiB0cnVlIGlmIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZyBpcyBsZXNzIHRoYW4gdGhlIHZhbHVlIG9mIEJpZyB5LCBvdGhlcndpc2UgcmV0dXJuIGZhbHNlLlxyXG4gICAqL1xyXG4gIFAubHQgPSBmdW5jdGlvbiAoeSkge1xyXG4gICAgcmV0dXJuIHRoaXMuY21wKHkpIDwgMDtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gdHJ1ZSBpZiB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgaXMgbGVzcyB0aGFuIG9yIGVxdWFsIHRvIHRoZSB2YWx1ZSBvZiBCaWcgeSwgb3RoZXJ3aXNlXHJcbiAgICogcmV0dXJuIGZhbHNlLlxyXG4gICAqL1xyXG4gIFAubHRlID0gZnVuY3Rpb24gKHkpIHtcclxuICAgIHJldHVybiB0aGlzLmNtcCh5KSA8IDE7XHJcbiAgfTtcclxuXHJcblxyXG4gIC8qXHJcbiAgICogUmV0dXJuIGEgbmV3IEJpZyB3aG9zZSB2YWx1ZSBpcyB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgbWludXMgdGhlIHZhbHVlIG9mIEJpZyB5LlxyXG4gICAqL1xyXG4gIFAubWludXMgPSBQLnN1YiA9IGZ1bmN0aW9uICh5KSB7XHJcbiAgICB2YXIgaSwgaiwgdCwgeGx0eSxcclxuICAgICAgeCA9IHRoaXMsXHJcbiAgICAgIEJpZyA9IHguY29uc3RydWN0b3IsXHJcbiAgICAgIGEgPSB4LnMsXHJcbiAgICAgIGIgPSAoeSA9IG5ldyBCaWcoeSkpLnM7XHJcblxyXG4gICAgLy8gU2lnbnMgZGlmZmVyP1xyXG4gICAgaWYgKGEgIT0gYikge1xyXG4gICAgICB5LnMgPSAtYjtcclxuICAgICAgcmV0dXJuIHgucGx1cyh5KTtcclxuICAgIH1cclxuXHJcbiAgICB2YXIgeGMgPSB4LmMuc2xpY2UoKSxcclxuICAgICAgeGUgPSB4LmUsXHJcbiAgICAgIHljID0geS5jLFxyXG4gICAgICB5ZSA9IHkuZTtcclxuXHJcbiAgICAvLyBFaXRoZXIgemVybz9cclxuICAgIGlmICgheGNbMF0gfHwgIXljWzBdKSB7XHJcbiAgICAgIGlmICh5Y1swXSkge1xyXG4gICAgICAgIHkucyA9IC1iO1xyXG4gICAgICB9IGVsc2UgaWYgKHhjWzBdKSB7XHJcbiAgICAgICAgeSA9IG5ldyBCaWcoeCk7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgeS5zID0gMTtcclxuICAgICAgfVxyXG4gICAgICByZXR1cm4geTtcclxuICAgIH1cclxuXHJcbiAgICAvLyBEZXRlcm1pbmUgd2hpY2ggaXMgdGhlIGJpZ2dlciBudW1iZXIuIFByZXBlbmQgemVyb3MgdG8gZXF1YWxpc2UgZXhwb25lbnRzLlxyXG4gICAgaWYgKGEgPSB4ZSAtIHllKSB7XHJcblxyXG4gICAgICBpZiAoeGx0eSA9IGEgPCAwKSB7XHJcbiAgICAgICAgYSA9IC1hO1xyXG4gICAgICAgIHQgPSB4YztcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICB5ZSA9IHhlO1xyXG4gICAgICAgIHQgPSB5YztcclxuICAgICAgfVxyXG5cclxuICAgICAgdC5yZXZlcnNlKCk7XHJcbiAgICAgIGZvciAoYiA9IGE7IGItLTspIHQucHVzaCgwKTtcclxuICAgICAgdC5yZXZlcnNlKCk7XHJcbiAgICB9IGVsc2Uge1xyXG5cclxuICAgICAgLy8gRXhwb25lbnRzIGVxdWFsLiBDaGVjayBkaWdpdCBieSBkaWdpdC5cclxuICAgICAgaiA9ICgoeGx0eSA9IHhjLmxlbmd0aCA8IHljLmxlbmd0aCkgPyB4YyA6IHljKS5sZW5ndGg7XHJcblxyXG4gICAgICBmb3IgKGEgPSBiID0gMDsgYiA8IGo7IGIrKykge1xyXG4gICAgICAgIGlmICh4Y1tiXSAhPSB5Y1tiXSkge1xyXG4gICAgICAgICAgeGx0eSA9IHhjW2JdIDwgeWNbYl07XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICAvLyB4IDwgeT8gUG9pbnQgeGMgdG8gdGhlIGFycmF5IG9mIHRoZSBiaWdnZXIgbnVtYmVyLlxyXG4gICAgaWYgKHhsdHkpIHtcclxuICAgICAgdCA9IHhjO1xyXG4gICAgICB4YyA9IHljO1xyXG4gICAgICB5YyA9IHQ7XHJcbiAgICAgIHkucyA9IC15LnM7XHJcbiAgICB9XHJcblxyXG4gICAgLypcclxuICAgICAqIEFwcGVuZCB6ZXJvcyB0byB4YyBpZiBzaG9ydGVyLiBObyBuZWVkIHRvIGFkZCB6ZXJvcyB0byB5YyBpZiBzaG9ydGVyIGFzIHN1YnRyYWN0aW9uIG9ubHlcclxuICAgICAqIG5lZWRzIHRvIHN0YXJ0IGF0IHljLmxlbmd0aC5cclxuICAgICAqL1xyXG4gICAgaWYgKChiID0gKGogPSB5Yy5sZW5ndGgpIC0gKGkgPSB4Yy5sZW5ndGgpKSA+IDApIGZvciAoOyBiLS07KSB4Y1tpKytdID0gMDtcclxuXHJcbiAgICAvLyBTdWJ0cmFjdCB5YyBmcm9tIHhjLlxyXG4gICAgZm9yIChiID0gaTsgaiA+IGE7KSB7XHJcbiAgICAgIGlmICh4Y1stLWpdIDwgeWNbal0pIHtcclxuICAgICAgICBmb3IgKGkgPSBqOyBpICYmICF4Y1stLWldOykgeGNbaV0gPSA5O1xyXG4gICAgICAgIC0teGNbaV07XHJcbiAgICAgICAgeGNbal0gKz0gMTA7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIHhjW2pdIC09IHljW2pdO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIFJlbW92ZSB0cmFpbGluZyB6ZXJvcy5cclxuICAgIGZvciAoOyB4Y1stLWJdID09PSAwOykgeGMucG9wKCk7XHJcblxyXG4gICAgLy8gUmVtb3ZlIGxlYWRpbmcgemVyb3MgYW5kIGFkanVzdCBleHBvbmVudCBhY2NvcmRpbmdseS5cclxuICAgIGZvciAoOyB4Y1swXSA9PT0gMDspIHtcclxuICAgICAgeGMuc2hpZnQoKTtcclxuICAgICAgLS15ZTtcclxuICAgIH1cclxuXHJcbiAgICBpZiAoIXhjWzBdKSB7XHJcblxyXG4gICAgICAvLyBuIC0gbiA9ICswXHJcbiAgICAgIHkucyA9IDE7XHJcblxyXG4gICAgICAvLyBSZXN1bHQgbXVzdCBiZSB6ZXJvLlxyXG4gICAgICB4YyA9IFt5ZSA9IDBdO1xyXG4gICAgfVxyXG5cclxuICAgIHkuYyA9IHhjO1xyXG4gICAgeS5lID0geWU7XHJcblxyXG4gICAgcmV0dXJuIHk7XHJcbiAgfTtcclxuXHJcblxyXG4gIC8qXHJcbiAgICogUmV0dXJuIGEgbmV3IEJpZyB3aG9zZSB2YWx1ZSBpcyB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgbW9kdWxvIHRoZSB2YWx1ZSBvZiBCaWcgeS5cclxuICAgKi9cclxuICBQLm1vZCA9IGZ1bmN0aW9uICh5KSB7XHJcbiAgICB2YXIgeWd0eCxcclxuICAgICAgeCA9IHRoaXMsXHJcbiAgICAgIEJpZyA9IHguY29uc3RydWN0b3IsXHJcbiAgICAgIGEgPSB4LnMsXHJcbiAgICAgIGIgPSAoeSA9IG5ldyBCaWcoeSkpLnM7XHJcblxyXG4gICAgaWYgKCF5LmNbMF0pIHtcclxuICAgICAgdGhyb3cgRXJyb3IoRElWX0JZX1pFUk8pO1xyXG4gICAgfVxyXG5cclxuICAgIHgucyA9IHkucyA9IDE7XHJcbiAgICB5Z3R4ID0geS5jbXAoeCkgPT0gMTtcclxuICAgIHgucyA9IGE7XHJcbiAgICB5LnMgPSBiO1xyXG5cclxuICAgIGlmICh5Z3R4KSByZXR1cm4gbmV3IEJpZyh4KTtcclxuXHJcbiAgICBhID0gQmlnLkRQO1xyXG4gICAgYiA9IEJpZy5STTtcclxuICAgIEJpZy5EUCA9IEJpZy5STSA9IDA7XHJcbiAgICB4ID0geC5kaXYoeSk7XHJcbiAgICBCaWcuRFAgPSBhO1xyXG4gICAgQmlnLlJNID0gYjtcclxuXHJcbiAgICByZXR1cm4gdGhpcy5taW51cyh4LnRpbWVzKHkpKTtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gYSBuZXcgQmlnIHdob3NlIHZhbHVlIGlzIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZyBwbHVzIHRoZSB2YWx1ZSBvZiBCaWcgeS5cclxuICAgKi9cclxuICBQLnBsdXMgPSBQLmFkZCA9IGZ1bmN0aW9uICh5KSB7XHJcbiAgICB2YXIgZSwgaywgdCxcclxuICAgICAgeCA9IHRoaXMsXHJcbiAgICAgIEJpZyA9IHguY29uc3RydWN0b3I7XHJcblxyXG4gICAgeSA9IG5ldyBCaWcoeSk7XHJcblxyXG4gICAgLy8gU2lnbnMgZGlmZmVyP1xyXG4gICAgaWYgKHgucyAhPSB5LnMpIHtcclxuICAgICAgeS5zID0gLXkucztcclxuICAgICAgcmV0dXJuIHgubWludXMoeSk7XHJcbiAgICB9XHJcblxyXG4gICAgdmFyIHhlID0geC5lLFxyXG4gICAgICB4YyA9IHguYyxcclxuICAgICAgeWUgPSB5LmUsXHJcbiAgICAgIHljID0geS5jO1xyXG5cclxuICAgIC8vIEVpdGhlciB6ZXJvP1xyXG4gICAgaWYgKCF4Y1swXSB8fCAheWNbMF0pIHtcclxuICAgICAgaWYgKCF5Y1swXSkge1xyXG4gICAgICAgIGlmICh4Y1swXSkge1xyXG4gICAgICAgICAgeSA9IG5ldyBCaWcoeCk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIHkucyA9IHgucztcclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuICAgICAgcmV0dXJuIHk7XHJcbiAgICB9XHJcblxyXG4gICAgeGMgPSB4Yy5zbGljZSgpO1xyXG5cclxuICAgIC8vIFByZXBlbmQgemVyb3MgdG8gZXF1YWxpc2UgZXhwb25lbnRzLlxyXG4gICAgLy8gTm90ZTogcmV2ZXJzZSBmYXN0ZXIgdGhhbiB1bnNoaWZ0cy5cclxuICAgIGlmIChlID0geGUgLSB5ZSkge1xyXG4gICAgICBpZiAoZSA+IDApIHtcclxuICAgICAgICB5ZSA9IHhlO1xyXG4gICAgICAgIHQgPSB5YztcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICBlID0gLWU7XHJcbiAgICAgICAgdCA9IHhjO1xyXG4gICAgICB9XHJcblxyXG4gICAgICB0LnJldmVyc2UoKTtcclxuICAgICAgZm9yICg7IGUtLTspIHQucHVzaCgwKTtcclxuICAgICAgdC5yZXZlcnNlKCk7XHJcbiAgICB9XHJcblxyXG4gICAgLy8gUG9pbnQgeGMgdG8gdGhlIGxvbmdlciBhcnJheS5cclxuICAgIGlmICh4Yy5sZW5ndGggLSB5Yy5sZW5ndGggPCAwKSB7XHJcbiAgICAgIHQgPSB5YztcclxuICAgICAgeWMgPSB4YztcclxuICAgICAgeGMgPSB0O1xyXG4gICAgfVxyXG5cclxuICAgIGUgPSB5Yy5sZW5ndGg7XHJcblxyXG4gICAgLy8gT25seSBzdGFydCBhZGRpbmcgYXQgeWMubGVuZ3RoIC0gMSBhcyB0aGUgZnVydGhlciBkaWdpdHMgb2YgeGMgY2FuIGJlIGxlZnQgYXMgdGhleSBhcmUuXHJcbiAgICBmb3IgKGsgPSAwOyBlOyB4Y1tlXSAlPSAxMCkgayA9ICh4Y1stLWVdID0geGNbZV0gKyB5Y1tlXSArIGspIC8gMTAgfCAwO1xyXG5cclxuICAgIC8vIE5vIG5lZWQgdG8gY2hlY2sgZm9yIHplcm8sIGFzICt4ICsgK3kgIT0gMCAmJiAteCArIC15ICE9IDBcclxuXHJcbiAgICBpZiAoaykge1xyXG4gICAgICB4Yy51bnNoaWZ0KGspO1xyXG4gICAgICArK3llO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIFJlbW92ZSB0cmFpbGluZyB6ZXJvcy5cclxuICAgIGZvciAoZSA9IHhjLmxlbmd0aDsgeGNbLS1lXSA9PT0gMDspIHhjLnBvcCgpO1xyXG5cclxuICAgIHkuYyA9IHhjO1xyXG4gICAgeS5lID0geWU7XHJcblxyXG4gICAgcmV0dXJuIHk7XHJcbiAgfTtcclxuXHJcblxyXG4gIC8qXHJcbiAgICogUmV0dXJuIGEgQmlnIHdob3NlIHZhbHVlIGlzIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZyByYWlzZWQgdG8gdGhlIHBvd2VyIG4uXHJcbiAgICogSWYgbiBpcyBuZWdhdGl2ZSwgcm91bmQgdG8gYSBtYXhpbXVtIG9mIEJpZy5EUCBkZWNpbWFsIHBsYWNlcyB1c2luZyByb3VuZGluZ1xyXG4gICAqIG1vZGUgQmlnLlJNLlxyXG4gICAqXHJcbiAgICogbiB7bnVtYmVyfSBJbnRlZ2VyLCAtTUFYX1BPV0VSIHRvIE1BWF9QT1dFUiBpbmNsdXNpdmUuXHJcbiAgICovXHJcbiAgUC5wb3cgPSBmdW5jdGlvbiAobikge1xyXG4gICAgdmFyIHggPSB0aGlzLFxyXG4gICAgICBvbmUgPSBuZXcgeC5jb25zdHJ1Y3RvcignMScpLFxyXG4gICAgICB5ID0gb25lLFxyXG4gICAgICBpc25lZyA9IG4gPCAwO1xyXG5cclxuICAgIGlmIChuICE9PSB+fm4gfHwgbiA8IC1NQVhfUE9XRVIgfHwgbiA+IE1BWF9QT1dFUikge1xyXG4gICAgICB0aHJvdyBFcnJvcihJTlZBTElEICsgJ2V4cG9uZW50Jyk7XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKGlzbmVnKSBuID0gLW47XHJcblxyXG4gICAgZm9yICg7Oykge1xyXG4gICAgICBpZiAobiAmIDEpIHkgPSB5LnRpbWVzKHgpO1xyXG4gICAgICBuID4+PSAxO1xyXG4gICAgICBpZiAoIW4pIGJyZWFrO1xyXG4gICAgICB4ID0geC50aW1lcyh4KTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4gaXNuZWcgPyBvbmUuZGl2KHkpIDogeTtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gYSBuZXcgQmlnIHdob3NlIHZhbHVlIGlzIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZyByb3VuZGVkIHRvIGEgbWF4aW11bSBwcmVjaXNpb24gb2Ygc2RcclxuICAgKiBzaWduaWZpY2FudCBkaWdpdHMgdXNpbmcgcm91bmRpbmcgbW9kZSBybSwgb3IgQmlnLlJNIGlmIHJtIGlzIG5vdCBzcGVjaWZpZWQuXHJcbiAgICpcclxuICAgKiBzZCB7bnVtYmVyfSBTaWduaWZpY2FudCBkaWdpdHM6IGludGVnZXIsIDEgdG8gTUFYX0RQIGluY2x1c2l2ZS5cclxuICAgKiBybT8ge251bWJlcn0gUm91bmRpbmcgbW9kZTogMCAoZG93biksIDEgKGhhbGYtdXApLCAyIChoYWxmLWV2ZW4pIG9yIDMgKHVwKS5cclxuICAgKi9cclxuICBQLnByZWMgPSBmdW5jdGlvbiAoc2QsIHJtKSB7XHJcbiAgICBpZiAoc2QgIT09IH5+c2QgfHwgc2QgPCAxIHx8IHNkID4gTUFYX0RQKSB7XHJcbiAgICAgIHRocm93IEVycm9yKElOVkFMSUQgKyAncHJlY2lzaW9uJyk7XHJcbiAgICB9XHJcbiAgICByZXR1cm4gcm91bmQobmV3IHRoaXMuY29uc3RydWN0b3IodGhpcyksIHNkLCBybSk7XHJcbiAgfTtcclxuXHJcblxyXG4gIC8qXHJcbiAgICogUmV0dXJuIGEgbmV3IEJpZyB3aG9zZSB2YWx1ZSBpcyB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgcm91bmRlZCB0byBhIG1heGltdW0gb2YgZHAgZGVjaW1hbCBwbGFjZXNcclxuICAgKiB1c2luZyByb3VuZGluZyBtb2RlIHJtLCBvciBCaWcuUk0gaWYgcm0gaXMgbm90IHNwZWNpZmllZC5cclxuICAgKiBJZiBkcCBpcyBuZWdhdGl2ZSwgcm91bmQgdG8gYW4gaW50ZWdlciB3aGljaCBpcyBhIG11bHRpcGxlIG9mIDEwKiotZHAuXHJcbiAgICogSWYgZHAgaXMgbm90IHNwZWNpZmllZCwgcm91bmQgdG8gMCBkZWNpbWFsIHBsYWNlcy5cclxuICAgKlxyXG4gICAqIGRwPyB7bnVtYmVyfSBJbnRlZ2VyLCAtTUFYX0RQIHRvIE1BWF9EUCBpbmNsdXNpdmUuXHJcbiAgICogcm0/IHtudW1iZXJ9IFJvdW5kaW5nIG1vZGU6IDAgKGRvd24pLCAxIChoYWxmLXVwKSwgMiAoaGFsZi1ldmVuKSBvciAzICh1cCkuXHJcbiAgICovXHJcbiAgUC5yb3VuZCA9IGZ1bmN0aW9uIChkcCwgcm0pIHtcclxuICAgIGlmIChkcCA9PT0gVU5ERUZJTkVEKSBkcCA9IDA7XHJcbiAgICBlbHNlIGlmIChkcCAhPT0gfn5kcCB8fCBkcCA8IC1NQVhfRFAgfHwgZHAgPiBNQVhfRFApIHtcclxuICAgICAgdGhyb3cgRXJyb3IoSU5WQUxJRF9EUCk7XHJcbiAgICB9XHJcbiAgICByZXR1cm4gcm91bmQobmV3IHRoaXMuY29uc3RydWN0b3IodGhpcyksIGRwICsgdGhpcy5lICsgMSwgcm0pO1xyXG4gIH07XHJcblxyXG5cclxuICAvKlxyXG4gICAqIFJldHVybiBhIG5ldyBCaWcgd2hvc2UgdmFsdWUgaXMgdGhlIHNxdWFyZSByb290IG9mIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZywgcm91bmRlZCwgaWZcclxuICAgKiBuZWNlc3NhcnksIHRvIGEgbWF4aW11bSBvZiBCaWcuRFAgZGVjaW1hbCBwbGFjZXMgdXNpbmcgcm91bmRpbmcgbW9kZSBCaWcuUk0uXHJcbiAgICovXHJcbiAgUC5zcXJ0ID0gZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIHIsIGMsIHQsXHJcbiAgICAgIHggPSB0aGlzLFxyXG4gICAgICBCaWcgPSB4LmNvbnN0cnVjdG9yLFxyXG4gICAgICBzID0geC5zLFxyXG4gICAgICBlID0geC5lLFxyXG4gICAgICBoYWxmID0gbmV3IEJpZygnMC41Jyk7XHJcblxyXG4gICAgLy8gWmVybz9cclxuICAgIGlmICgheC5jWzBdKSByZXR1cm4gbmV3IEJpZyh4KTtcclxuXHJcbiAgICAvLyBOZWdhdGl2ZT9cclxuICAgIGlmIChzIDwgMCkge1xyXG4gICAgICB0aHJvdyBFcnJvcihOQU1FICsgJ05vIHNxdWFyZSByb290Jyk7XHJcbiAgICB9XHJcblxyXG4gICAgLy8gRXN0aW1hdGUuXHJcbiAgICBzID0gTWF0aC5zcXJ0KHggKyAnJyk7XHJcblxyXG4gICAgLy8gTWF0aC5zcXJ0IHVuZGVyZmxvdy9vdmVyZmxvdz9cclxuICAgIC8vIFJlLWVzdGltYXRlOiBwYXNzIHggY29lZmZpY2llbnQgdG8gTWF0aC5zcXJ0IGFzIGludGVnZXIsIHRoZW4gYWRqdXN0IHRoZSByZXN1bHQgZXhwb25lbnQuXHJcbiAgICBpZiAocyA9PT0gMCB8fCBzID09PSAxIC8gMCkge1xyXG4gICAgICBjID0geC5jLmpvaW4oJycpO1xyXG4gICAgICBpZiAoIShjLmxlbmd0aCArIGUgJiAxKSkgYyArPSAnMCc7XHJcbiAgICAgIHMgPSBNYXRoLnNxcnQoYyk7XHJcbiAgICAgIGUgPSAoKGUgKyAxKSAvIDIgfCAwKSAtIChlIDwgMCB8fCBlICYgMSk7XHJcbiAgICAgIHIgPSBuZXcgQmlnKChzID09IDEgLyAwID8gJzVlJyA6IChzID0gcy50b0V4cG9uZW50aWFsKCkpLnNsaWNlKDAsIHMuaW5kZXhPZignZScpICsgMSkpICsgZSk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByID0gbmV3IEJpZyhzICsgJycpO1xyXG4gICAgfVxyXG5cclxuICAgIGUgPSByLmUgKyAoQmlnLkRQICs9IDQpO1xyXG5cclxuICAgIC8vIE5ld3Rvbi1SYXBoc29uIGl0ZXJhdGlvbi5cclxuICAgIGRvIHtcclxuICAgICAgdCA9IHI7XHJcbiAgICAgIHIgPSBoYWxmLnRpbWVzKHQucGx1cyh4LmRpdih0KSkpO1xyXG4gICAgfSB3aGlsZSAodC5jLnNsaWNlKDAsIGUpLmpvaW4oJycpICE9PSByLmMuc2xpY2UoMCwgZSkuam9pbignJykpO1xyXG5cclxuICAgIHJldHVybiByb3VuZChyLCAoQmlnLkRQIC09IDQpICsgci5lICsgMSwgQmlnLlJNKTtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gYSBuZXcgQmlnIHdob3NlIHZhbHVlIGlzIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZyB0aW1lcyB0aGUgdmFsdWUgb2YgQmlnIHkuXHJcbiAgICovXHJcbiAgUC50aW1lcyA9IFAubXVsID0gZnVuY3Rpb24gKHkpIHtcclxuICAgIHZhciBjLFxyXG4gICAgICB4ID0gdGhpcyxcclxuICAgICAgQmlnID0geC5jb25zdHJ1Y3RvcixcclxuICAgICAgeGMgPSB4LmMsXHJcbiAgICAgIHljID0gKHkgPSBuZXcgQmlnKHkpKS5jLFxyXG4gICAgICBhID0geGMubGVuZ3RoLFxyXG4gICAgICBiID0geWMubGVuZ3RoLFxyXG4gICAgICBpID0geC5lLFxyXG4gICAgICBqID0geS5lO1xyXG5cclxuICAgIC8vIERldGVybWluZSBzaWduIG9mIHJlc3VsdC5cclxuICAgIHkucyA9IHgucyA9PSB5LnMgPyAxIDogLTE7XHJcblxyXG4gICAgLy8gUmV0dXJuIHNpZ25lZCAwIGlmIGVpdGhlciAwLlxyXG4gICAgaWYgKCF4Y1swXSB8fCAheWNbMF0pIHtcclxuICAgICAgeS5jID0gW3kuZSA9IDBdO1xyXG4gICAgICByZXR1cm4geTtcclxuICAgIH1cclxuXHJcbiAgICAvLyBJbml0aWFsaXNlIGV4cG9uZW50IG9mIHJlc3VsdCBhcyB4LmUgKyB5LmUuXHJcbiAgICB5LmUgPSBpICsgajtcclxuXHJcbiAgICAvLyBJZiBhcnJheSB4YyBoYXMgZmV3ZXIgZGlnaXRzIHRoYW4geWMsIHN3YXAgeGMgYW5kIHljLCBhbmQgbGVuZ3Rocy5cclxuICAgIGlmIChhIDwgYikge1xyXG4gICAgICBjID0geGM7XHJcbiAgICAgIHhjID0geWM7XHJcbiAgICAgIHljID0gYztcclxuICAgICAgaiA9IGE7XHJcbiAgICAgIGEgPSBiO1xyXG4gICAgICBiID0gajtcclxuICAgIH1cclxuXHJcbiAgICAvLyBJbml0aWFsaXNlIGNvZWZmaWNpZW50IGFycmF5IG9mIHJlc3VsdCB3aXRoIHplcm9zLlxyXG4gICAgZm9yIChjID0gbmV3IEFycmF5KGogPSBhICsgYik7IGotLTspIGNbal0gPSAwO1xyXG5cclxuICAgIC8vIE11bHRpcGx5LlxyXG5cclxuICAgIC8vIGkgaXMgaW5pdGlhbGx5IHhjLmxlbmd0aC5cclxuICAgIGZvciAoaSA9IGI7IGktLTspIHtcclxuICAgICAgYiA9IDA7XHJcblxyXG4gICAgICAvLyBhIGlzIHljLmxlbmd0aC5cclxuICAgICAgZm9yIChqID0gYSArIGk7IGogPiBpOykge1xyXG5cclxuICAgICAgICAvLyBDdXJyZW50IHN1bSBvZiBwcm9kdWN0cyBhdCB0aGlzIGRpZ2l0IHBvc2l0aW9uLCBwbHVzIGNhcnJ5LlxyXG4gICAgICAgIGIgPSBjW2pdICsgeWNbaV0gKiB4Y1tqIC0gaSAtIDFdICsgYjtcclxuICAgICAgICBjW2otLV0gPSBiICUgMTA7XHJcblxyXG4gICAgICAgIC8vIGNhcnJ5XHJcbiAgICAgICAgYiA9IGIgLyAxMCB8IDA7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIGNbal0gPSBiO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIEluY3JlbWVudCByZXN1bHQgZXhwb25lbnQgaWYgdGhlcmUgaXMgYSBmaW5hbCBjYXJyeSwgb3RoZXJ3aXNlIHJlbW92ZSBsZWFkaW5nIHplcm8uXHJcbiAgICBpZiAoYikgKyt5LmU7XHJcbiAgICBlbHNlIGMuc2hpZnQoKTtcclxuXHJcbiAgICAvLyBSZW1vdmUgdHJhaWxpbmcgemVyb3MuXHJcbiAgICBmb3IgKGkgPSBjLmxlbmd0aDsgIWNbLS1pXTspIGMucG9wKCk7XHJcbiAgICB5LmMgPSBjO1xyXG5cclxuICAgIHJldHVybiB5O1xyXG4gIH07XHJcblxyXG5cclxuICAvKlxyXG4gICAqIFJldHVybiBhIHN0cmluZyByZXByZXNlbnRpbmcgdGhlIHZhbHVlIG9mIHRoaXMgQmlnIGluIGV4cG9uZW50aWFsIG5vdGF0aW9uIHJvdW5kZWQgdG8gZHAgZml4ZWRcclxuICAgKiBkZWNpbWFsIHBsYWNlcyB1c2luZyByb3VuZGluZyBtb2RlIHJtLCBvciBCaWcuUk0gaWYgcm0gaXMgbm90IHNwZWNpZmllZC5cclxuICAgKlxyXG4gICAqIGRwPyB7bnVtYmVyfSBEZWNpbWFsIHBsYWNlczogaW50ZWdlciwgMCB0byBNQVhfRFAgaW5jbHVzaXZlLlxyXG4gICAqIHJtPyB7bnVtYmVyfSBSb3VuZGluZyBtb2RlOiAwIChkb3duKSwgMSAoaGFsZi11cCksIDIgKGhhbGYtZXZlbikgb3IgMyAodXApLlxyXG4gICAqL1xyXG4gIFAudG9FeHBvbmVudGlhbCA9IGZ1bmN0aW9uIChkcCwgcm0pIHtcclxuICAgIHZhciB4ID0gdGhpcyxcclxuICAgICAgbiA9IHguY1swXTtcclxuXHJcbiAgICBpZiAoZHAgIT09IFVOREVGSU5FRCkge1xyXG4gICAgICBpZiAoZHAgIT09IH5+ZHAgfHwgZHAgPCAwIHx8IGRwID4gTUFYX0RQKSB7XHJcbiAgICAgICAgdGhyb3cgRXJyb3IoSU5WQUxJRF9EUCk7XHJcbiAgICAgIH1cclxuICAgICAgeCA9IHJvdW5kKG5ldyB4LmNvbnN0cnVjdG9yKHgpLCArK2RwLCBybSk7XHJcbiAgICAgIGZvciAoOyB4LmMubGVuZ3RoIDwgZHA7KSB4LmMucHVzaCgwKTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4gc3RyaW5naWZ5KHgsIHRydWUsICEhbik7XHJcbiAgfTtcclxuXHJcblxyXG4gIC8qXHJcbiAgICogUmV0dXJuIGEgc3RyaW5nIHJlcHJlc2VudGluZyB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgaW4gbm9ybWFsIG5vdGF0aW9uIHJvdW5kZWQgdG8gZHAgZml4ZWRcclxuICAgKiBkZWNpbWFsIHBsYWNlcyB1c2luZyByb3VuZGluZyBtb2RlIHJtLCBvciBCaWcuUk0gaWYgcm0gaXMgbm90IHNwZWNpZmllZC5cclxuICAgKlxyXG4gICAqIGRwPyB7bnVtYmVyfSBEZWNpbWFsIHBsYWNlczogaW50ZWdlciwgMCB0byBNQVhfRFAgaW5jbHVzaXZlLlxyXG4gICAqIHJtPyB7bnVtYmVyfSBSb3VuZGluZyBtb2RlOiAwIChkb3duKSwgMSAoaGFsZi11cCksIDIgKGhhbGYtZXZlbikgb3IgMyAodXApLlxyXG4gICAqXHJcbiAgICogKC0wKS50b0ZpeGVkKDApIGlzICcwJywgYnV0ICgtMC4xKS50b0ZpeGVkKDApIGlzICctMCcuXHJcbiAgICogKC0wKS50b0ZpeGVkKDEpIGlzICcwLjAnLCBidXQgKC0wLjAxKS50b0ZpeGVkKDEpIGlzICctMC4wJy5cclxuICAgKi9cclxuICBQLnRvRml4ZWQgPSBmdW5jdGlvbiAoZHAsIHJtKSB7XHJcbiAgICB2YXIgeCA9IHRoaXMsXHJcbiAgICAgIG4gPSB4LmNbMF07XHJcblxyXG4gICAgaWYgKGRwICE9PSBVTkRFRklORUQpIHtcclxuICAgICAgaWYgKGRwICE9PSB+fmRwIHx8IGRwIDwgMCB8fCBkcCA+IE1BWF9EUCkge1xyXG4gICAgICAgIHRocm93IEVycm9yKElOVkFMSURfRFApO1xyXG4gICAgICB9XHJcbiAgICAgIHggPSByb3VuZChuZXcgeC5jb25zdHJ1Y3Rvcih4KSwgZHAgKyB4LmUgKyAxLCBybSk7XHJcblxyXG4gICAgICAvLyB4LmUgbWF5IGhhdmUgY2hhbmdlZCBpZiB0aGUgdmFsdWUgaXMgcm91bmRlZCB1cC5cclxuICAgICAgZm9yIChkcCA9IGRwICsgeC5lICsgMTsgeC5jLmxlbmd0aCA8IGRwOykgeC5jLnB1c2goMCk7XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIHN0cmluZ2lmeSh4LCBmYWxzZSwgISFuKTtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gYSBzdHJpbmcgcmVwcmVzZW50aW5nIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZy5cclxuICAgKiBSZXR1cm4gZXhwb25lbnRpYWwgbm90YXRpb24gaWYgdGhpcyBCaWcgaGFzIGEgcG9zaXRpdmUgZXhwb25lbnQgZXF1YWwgdG8gb3IgZ3JlYXRlciB0aGFuXHJcbiAgICogQmlnLlBFLCBvciBhIG5lZ2F0aXZlIGV4cG9uZW50IGVxdWFsIHRvIG9yIGxlc3MgdGhhbiBCaWcuTkUuXHJcbiAgICogT21pdCB0aGUgc2lnbiBmb3IgbmVnYXRpdmUgemVyby5cclxuICAgKi9cclxuICBQLnRvSlNPTiA9IFAudG9TdHJpbmcgPSBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgeCA9IHRoaXMsXHJcbiAgICAgIEJpZyA9IHguY29uc3RydWN0b3I7XHJcbiAgICByZXR1cm4gc3RyaW5naWZ5KHgsIHguZSA8PSBCaWcuTkUgfHwgeC5lID49IEJpZy5QRSwgISF4LmNbMF0pO1xyXG4gIH07XHJcblxyXG5cclxuICAvKlxyXG4gICAqIFJldHVybiB0aGUgdmFsdWUgb2YgdGhpcyBCaWcgYXMgYSBwcmltaXR2ZSBudW1iZXIuXHJcbiAgICovXHJcbiAgUC50b051bWJlciA9IGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBuID0gTnVtYmVyKHN0cmluZ2lmeSh0aGlzLCB0cnVlLCB0cnVlKSk7XHJcbiAgICBpZiAodGhpcy5jb25zdHJ1Y3Rvci5zdHJpY3QgPT09IHRydWUgJiYgIXRoaXMuZXEobi50b1N0cmluZygpKSkge1xyXG4gICAgICB0aHJvdyBFcnJvcihOQU1FICsgJ0ltcHJlY2lzZSBjb252ZXJzaW9uJyk7XHJcbiAgICB9XHJcbiAgICByZXR1cm4gbjtcclxuICB9O1xyXG5cclxuXHJcbiAgLypcclxuICAgKiBSZXR1cm4gYSBzdHJpbmcgcmVwcmVzZW50aW5nIHRoZSB2YWx1ZSBvZiB0aGlzIEJpZyByb3VuZGVkIHRvIHNkIHNpZ25pZmljYW50IGRpZ2l0cyB1c2luZ1xyXG4gICAqIHJvdW5kaW5nIG1vZGUgcm0sIG9yIEJpZy5STSBpZiBybSBpcyBub3Qgc3BlY2lmaWVkLlxyXG4gICAqIFVzZSBleHBvbmVudGlhbCBub3RhdGlvbiBpZiBzZCBpcyBsZXNzIHRoYW4gdGhlIG51bWJlciBvZiBkaWdpdHMgbmVjZXNzYXJ5IHRvIHJlcHJlc2VudFxyXG4gICAqIHRoZSBpbnRlZ2VyIHBhcnQgb2YgdGhlIHZhbHVlIGluIG5vcm1hbCBub3RhdGlvbi5cclxuICAgKlxyXG4gICAqIHNkIHtudW1iZXJ9IFNpZ25pZmljYW50IGRpZ2l0czogaW50ZWdlciwgMSB0byBNQVhfRFAgaW5jbHVzaXZlLlxyXG4gICAqIHJtPyB7bnVtYmVyfSBSb3VuZGluZyBtb2RlOiAwIChkb3duKSwgMSAoaGFsZi11cCksIDIgKGhhbGYtZXZlbikgb3IgMyAodXApLlxyXG4gICAqL1xyXG4gIFAudG9QcmVjaXNpb24gPSBmdW5jdGlvbiAoc2QsIHJtKSB7XHJcbiAgICB2YXIgeCA9IHRoaXMsXHJcbiAgICAgIEJpZyA9IHguY29uc3RydWN0b3IsXHJcbiAgICAgIG4gPSB4LmNbMF07XHJcblxyXG4gICAgaWYgKHNkICE9PSBVTkRFRklORUQpIHtcclxuICAgICAgaWYgKHNkICE9PSB+fnNkIHx8IHNkIDwgMSB8fCBzZCA+IE1BWF9EUCkge1xyXG4gICAgICAgIHRocm93IEVycm9yKElOVkFMSUQgKyAncHJlY2lzaW9uJyk7XHJcbiAgICAgIH1cclxuICAgICAgeCA9IHJvdW5kKG5ldyBCaWcoeCksIHNkLCBybSk7XHJcbiAgICAgIGZvciAoOyB4LmMubGVuZ3RoIDwgc2Q7KSB4LmMucHVzaCgwKTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4gc3RyaW5naWZ5KHgsIHNkIDw9IHguZSB8fCB4LmUgPD0gQmlnLk5FIHx8IHguZSA+PSBCaWcuUEUsICEhbik7XHJcbiAgfTtcclxuXHJcblxyXG4gIC8qXHJcbiAgICogUmV0dXJuIGEgc3RyaW5nIHJlcHJlc2VudGluZyB0aGUgdmFsdWUgb2YgdGhpcyBCaWcuXHJcbiAgICogUmV0dXJuIGV4cG9uZW50aWFsIG5vdGF0aW9uIGlmIHRoaXMgQmlnIGhhcyBhIHBvc2l0aXZlIGV4cG9uZW50IGVxdWFsIHRvIG9yIGdyZWF0ZXIgdGhhblxyXG4gICAqIEJpZy5QRSwgb3IgYSBuZWdhdGl2ZSBleHBvbmVudCBlcXVhbCB0byBvciBsZXNzIHRoYW4gQmlnLk5FLlxyXG4gICAqIEluY2x1ZGUgdGhlIHNpZ24gZm9yIG5lZ2F0aXZlIHplcm8uXHJcbiAgICovXHJcbiAgUC52YWx1ZU9mID0gZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIHggPSB0aGlzLFxyXG4gICAgICBCaWcgPSB4LmNvbnN0cnVjdG9yO1xyXG4gICAgaWYgKEJpZy5zdHJpY3QgPT09IHRydWUpIHtcclxuICAgICAgdGhyb3cgRXJyb3IoTkFNRSArICd2YWx1ZU9mIGRpc2FsbG93ZWQnKTtcclxuICAgIH1cclxuICAgIHJldHVybiBzdHJpbmdpZnkoeCwgeC5lIDw9IEJpZy5ORSB8fCB4LmUgPj0gQmlnLlBFLCB0cnVlKTtcclxuICB9O1xyXG5cclxuXHJcbiAgLy8gRXhwb3J0XHJcblxyXG5cclxuICBCaWcgPSBfQmlnXygpO1xyXG5cclxuICBCaWdbJ2RlZmF1bHQnXSA9IEJpZy5CaWcgPSBCaWc7XHJcblxyXG4gIC8vQU1ELlxyXG4gIGlmICh0eXBlb2YgZGVmaW5lID09PSAnZnVuY3Rpb24nICYmIGRlZmluZS5hbWQpIHtcclxuICAgIGRlZmluZShmdW5jdGlvbiAoKSB7IHJldHVybiBCaWc7IH0pO1xyXG5cclxuICAvLyBOb2RlIGFuZCBvdGhlciBDb21tb25KUy1saWtlIGVudmlyb25tZW50cyB0aGF0IHN1cHBvcnQgbW9kdWxlLmV4cG9ydHMuXHJcbiAgfSBlbHNlIGlmICh0eXBlb2YgbW9kdWxlICE9PSAndW5kZWZpbmVkJyAmJiBtb2R1bGUuZXhwb3J0cykge1xyXG4gICAgbW9kdWxlLmV4cG9ydHMgPSBCaWc7XHJcblxyXG4gIC8vQnJvd3Nlci5cclxuICB9IGVsc2Uge1xyXG4gICAgR0xPQkFMLkJpZyA9IEJpZztcclxuICB9XHJcbn0pKHRoaXMpO1xyXG4iLCAiaW1wb3J0IHsgQmlnIH0gZnJvbSAnYmlnLmpzJztcclxuXHJcbmludGVyZmFjZSBWYWx1ZSB7XHJcbiAgICBjdXJyZW50OiBCaWc7XHJcbiAgICBpbnRlcnBvbGF0ZWQ6IEJpZztcclxuICAgIHByZXZpb3VzOiBCaWc7XHJcbn1cclxuaW50ZXJmYWNlIFNhbXBsZSB7XHJcbiAgICBub3c6IFZhbHVlO1xyXG4gICAgeWVzdGVyZGF5OiBWYWx1ZTtcclxufVxyXG5pbnRlcmZhY2UgU3RhdCB7XHJcbiAgICBzYW1wbGU6IFNhbXBsZTtcclxuICAgIHByZWNpc2lvbjogbnVtYmVyO1xyXG4gICAgdmFsdWVzOiBIVE1MU3BhbkVsZW1lbnRbXTtcclxuICAgIGNoYW5nZTI0aHI6IEhUTUxTcGFuRWxlbWVudFtdO1xyXG4gICAgdmFsdWVzQ3NzVXBkYXRlZDogYm9vbGVhblxyXG59XHJcblxyXG5pbnRlcmZhY2UgUGVyaW9kRGF0YSB7XHJcbiAgICBkYXRlOnN0cmluZztcclxuICAgIHJlc2VydmVzQmFsYW5jZTpzdHJpbmc7XHJcbiAgICBsaXF1aWRpdHlUb2tlbjpzdHJpbmc7XHJcbiAgICBsaXF1aWRpdHlDb2luOnN0cmluZztcclxuICAgIHN0YWtlZDpzdHJpbmc7XHJcbiAgICBhdmVNdWx0aXBsaWVyOnN0cmluZztcclxuICAgIHJld2FyZHM6c3RyaW5nO1xyXG4gICAgdm9sdW1lVHJhbnNmZXJzOnN0cmluZztcclxuICAgIHZvbHVtZUJ1eTpzdHJpbmc7XHJcbiAgICB2b2x1bWVTZWxsOnN0cmluZztcclxuICAgIHZvbHVtZVRyYWRlOnN0cmluZztcclxuICAgIHRva2VuUHJpY2VDb2luOnN0cmluZztcclxuICAgIGNvaW5QcmljZVN0YWJsZTpzdHJpbmc7XHJcbiAgICB0b2tlblByaWNlU3RhYmxlOnN0cmluZztcclxuICAgIG1hcmtldENhcDpzdHJpbmc7XHJcbiAgICBibG9ja051bWJlcjpzdHJpbmc7XHJcbiAgICBob2xkZXJzOnN0cmluZztcclxuICAgIHVzZFJlc2VydmVzQmFsYW5jZTpzdHJpbmc7XHJcbiAgICB1c2RMaXF1aWRpdHlUb2tlbjpzdHJpbmc7XHJcbiAgICB1c2RMaXF1aWRpdHlDb2luOnN0cmluZztcclxuICAgIHVzZFN0YWtlZDpzdHJpbmc7XHJcbiAgICB1c2RSZXdhcmRzOnN0cmluZztcclxuICAgIHVzZFZvbHVtZVRyYW5zZmVyczpzdHJpbmc7XHJcbiAgICB1c2RWb2x1bWVCdXk6c3RyaW5nO1xyXG4gICAgdXNkVm9sdW1lU2VsbDpzdHJpbmc7XHJcbiAgICB1c2RWb2x1bWVUcmFkZTpzdHJpbmc7XHJcbiAgICBzdXBwbHlPbkNoYWluUGVyY2VudDpzdHJpbmc7XHJcbiAgICBzdGFrZWRPZlRvdGFsU3VwcGx5UGVyY2VudDpzdHJpbmc7XHJcbiAgICBzdGFrZWRPZk9uQ2hhaW5QZXJjZW50OnN0cmluZztcclxuICAgIHN0YWtlZE9mVG90YWxTdGFrZWRQZXJjZW50OnN0cmluZztcclxufVxyXG5cclxuaW50ZXJmYWNlIENoYWluRGF0YSB7XHJcbiAgICBjdXJyZW50OlBlcmlvZERhdGE7XHJcbiAgICBoaXN0b3J5MjRocnM6IFBlcmlvZERhdGE7XHJcbiAgICBoaXN0b3J5NDhocnM6IFBlcmlvZERhdGE7XHJcbn1cclxuXHJcbmludGVyZmFjZSBCbG9ja2NoYWluRGF0YSB7XHJcbiAgICB1bmlmaWVkOiBDaGFpbkRhdGE7XHJcbiAgICBic2M6IENoYWluRGF0YTtcclxuICAgIGV0aDogQ2hhaW5EYXRhO1xyXG4gICAgcG9seTogQ2hhaW5EYXRhO1xyXG4gICAgZnRtOiBDaGFpbkRhdGE7XHJcbiAgICBhdmF4OiBDaGFpbkRhdGE7XHJcbn1cclxuXHJcbmludGVyZmFjZSBEYXRhIHtcclxuICAgIGJsb2NrTnVtYmVyOiB7XHJcbiAgICAgICAgY3VycmVudDogbnVtYmVyLFxyXG4gICAgICAgIGhpc3RvcnkyNGhyOiBudW1iZXIsXHJcbiAgICB9O1xyXG4gICAgaG9sZGVyczogU3RhdDtcclxuICAgIHN1cHBseU9uQ2hhaW46IFN0YXQ7XHJcblxyXG4gICAgdG9rZW5QcmljZVVTRFQ6IFN0YXQ7XHJcbiAgICB0b2tlblByaWNlQ29pbjogU3RhdDtcclxuICAgIGNvaW5QcmljZVVTRFQ6IFN0YXQ7XHJcblxyXG4gICAgbGlxdWlkaXR5VG9rZW46IFN0YXQ7XHJcbiAgICBsaXF1aWRpdHlDb2luOiBTdGF0O1xyXG4gICAgbGlxdWlkaXR5Q29pblVTRFQ6IFN0YXQ7XHJcblxyXG4gICAgcmVzZXJ2ZXNDb2luOiBTdGF0O1xyXG4gICAgcmVzZXJ2ZXNDb2luVVNEVDogU3RhdDtcclxuICAgIFxyXG4gICAgbWFya2V0Q2FwVVNEVDogU3RhdDtcclxuICAgIGxpcXVpZGl0eVBlcmNlbnQ6IFN0YXQ7XHJcbiAgICByZXNlcnZlc1BlcmNlbnQ6IFN0YXQ7XHJcblxyXG4gICAgdm9sdW1lVHJhbnNmZXJzOiBTdGF0O1xyXG4gICAgdm9sdW1lVHJhbnNmZXJzVVNEVDogU3RhdDtcclxuICAgIHZvbHVtZUJ1eTogU3RhdDtcclxuICAgIHZvbHVtZUJ1eVVTRFQ6IFN0YXQ7XHJcbiAgICB2b2x1bWVTZWxsOiBTdGF0O1xyXG4gICAgdm9sdW1lU2VsbFVTRFQ6IFN0YXQ7XHJcbiAgICB2b2x1bWVUcmFkZTogU3RhdDtcclxuICAgIHZvbHVtZVRyYWRlVVNEVDogU3RhdDtcclxuXHJcbiAgICB2b2x1bWVUcmFuc2ZlcnMyNGhyOiBTdGF0O1xyXG4gICAgdm9sdW1lVHJhbnNmZXJzVVNEVDI0aHI6IFN0YXQ7XHJcbiAgICB2b2x1bWVCdXkyNGhyOiBTdGF0O1xyXG4gICAgdm9sdW1lQnV5VVNEVDI0aHI6IFN0YXQ7XHJcbiAgICB2b2x1bWVTZWxsMjRocjogU3RhdDtcclxuICAgIHZvbHVtZVNlbGxVU0RUMjRocjogU3RhdDtcclxuICAgIHZvbHVtZVRyYWRlMjRocjogU3RhdDtcclxuICAgIHZvbHVtZVRyYWRlVVNEVDI0aHI6IFN0YXQ7XHJcblxyXG4gICAgdm9sdW1lVHJhbnNmZXJzN2RheTogU3RhdDtcclxuICAgIHZvbHVtZVRyYW5zZmVyc1VTRFQ3ZGF5OiBTdGF0O1xyXG4gICAgdm9sdW1lQnV5N2RheTogU3RhdDtcclxuICAgIHZvbHVtZUJ1eVVTRFQ3ZGF5OiBTdGF0O1xyXG4gICAgdm9sdW1lU2VsbDdkYXk6IFN0YXQ7XHJcbiAgICB2b2x1bWVTZWxsVVNEVDdkYXk6IFN0YXQ7XHJcbiAgICB2b2x1bWVUcmFkZTdkYXk6IFN0YXQ7XHJcbiAgICB2b2x1bWVUcmFkZVVTRFQ3ZGF5OiBTdGF0O1xyXG5cclxuICAgIHN0YWtlZFRva2VuOiBTdGF0O1xyXG4gICAgc3Rha2VkVVNEVDogU3RhdDtcclxuICAgIHN0YWtlZFBlcmNlbnQ6IFN0YXQ7XHJcbiAgICBzdGFrZWRNdWx0aXBsaWVyOiBTdGF0O1xyXG4gICAgc3Rha2VkT25DaGFpbjogU3RhdDtcclxuXHJcbiAgICByZXdhcmRzVG9rZW46IFN0YXQ7XHJcbiAgICByZXdhcmRzVG9rZW4yNGhyOiBTdGF0O1xyXG4gICAgcmV3YXJkc1Rva2VuN2RheTogU3RhdDtcclxuICAgIHJld2FyZHNVU0RUOiBTdGF0O1xyXG4gICAgcmV3YXJkc1VTRFQyNGhyOiBTdGF0O1xyXG4gICAgcmV3YXJkc1VTRFQ3ZGF5OiBTdGF0O1xyXG5cclxuICAgIFtTeW1ib2wuaXRlcmF0b3JdKCk7XHJcbn1cclxuXHJcbmNsYXNzIENvaW4ge1xyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBxdWVyeUZyZXF1ZW5jeSA9IDMwO1xyXG4gICAgcHJpdmF0ZSBzdXBwbHkgPSBCaWcoXCI3MTYxODAzMzk4OFwiKTtcclxuICAgIHB1YmxpYyBuZXR3b3JrOiBzdHJpbmc7XHJcbiAgICBwdWJsaWMgZGF0YTogRGF0YTtcclxuICAgIHB1YmxpYyB0aW1lcyA9IHtcclxuICAgICAgICB0aW1lQ3VycmVudDogMCxcclxuICAgICAgICB0aW1lTm93OiAwXHJcbiAgICB9O1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKG5ldHdvcms6IHN0cmluZykge1xyXG4gICAgICAgIHRoaXMubmV0d29yayA9IG5ldHdvcms7XHJcblxyXG4gICAgICAgIHRoaXMuZGF0YSA9IENvaW4uQ3JlYXRlU3RhdHModGhpcy5uZXR3b3JrKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIENyZWF0ZVN0YXRzKG5ldHdvcms6IHN0cmluZyl7XHJcbiAgICAgICAgY29uc3QgZGF0YSA9IHtcclxuICAgICAgICAgICAgYmxvY2tOdW1iZXI6IHtcclxuICAgICAgICAgICAgICAgIGN1cnJlbnQ6IC0xLFxyXG4gICAgICAgICAgICAgICAgaGlzdG9yeTI0aHI6IC0xLFxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBob2xkZXJzOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1ob2xkZXJzYCwgMCksXHJcblxyXG4gICAgICAgICAgICB0b2tlblByaWNlVVNEVDogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdG9rZW4tcHJpY2UtdXNkdGAsIDgpLFxyXG4gICAgICAgICAgICB0b2tlblByaWNlQ29pbjogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdG9rZW4tcHJpY2UtY29pbmAsIDEwKSxcclxuICAgICAgICAgICAgY29pblByaWNlVVNEVDogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tY29pbi1wcmljZS11c2R0YCwgMyksXHJcblxyXG4gICAgICAgICAgICBsaXF1aWRpdHlUb2tlbjogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdG9rZW4tbGlxdWlkaXR5YCwgMiksXHJcbiAgICAgICAgICAgIGxpcXVpZGl0eUNvaW46IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LWNvaW4tbGlxdWlkaXR5YCwgMiksXHJcbiAgICAgICAgICAgIGxpcXVpZGl0eUNvaW5VU0RUOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1jb2luLWxpcXVpZGl0eS11c2R0YCwgMiksXHJcbiAgICAgICAgICAgIGxpcXVpZGl0eVBlcmNlbnQ6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LWxpcXVpZGl0eS1wZXJjZW50YCwgNCksXHJcblxyXG4gICAgICAgICAgICByZXNlcnZlc0NvaW46IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LWNvaW4tcmVzZXJ2ZXNgLCAyKSxcclxuICAgICAgICAgICAgcmVzZXJ2ZXNDb2luVVNEVDogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tY29pbi1yZXNlcnZlcy11c2R0YCwgMiksXHJcbiAgICAgICAgICAgIHJlc2VydmVzUGVyY2VudDogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tY29pbi1yZXNlcnZlcy1wZXJjZW50YCwgNCksXHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICBtYXJrZXRDYXBVU0RUOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1tYXJrZXQtY2FwYCwgMiksXHJcblxyXG4gICAgICAgICAgICB2b2x1bWVUcmFuc2ZlcnM6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC10eG5zYCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVRyYW5zZmVyc1VTRFQ6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC10eG5zLXVzZHRgLCAyKSxcclxuICAgICAgICAgICAgdm9sdW1lQnV5OiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS12b2wtYnV5YCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZUJ1eVVTRFQ6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC1idXktdXNkdGAsIDIpLFxyXG4gICAgICAgICAgICB2b2x1bWVTZWxsOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS12b2wtc2VsbGAsIDIpLFxyXG4gICAgICAgICAgICB2b2x1bWVTZWxsVVNEVDogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXNlbGwtdXNkdGAsIDIpLFxyXG4gICAgICAgICAgICB2b2x1bWVUcmFkZTogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXRyYWRlYCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVRyYWRlVVNEVDogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXRyYWRlLXVzZHRgLCAyKSxcclxuXHJcbiAgICAgICAgICAgIHZvbHVtZVRyYW5zZmVyczI0aHI6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC10eG5zLTI0aHJgLCAyKSxcclxuICAgICAgICAgICAgdm9sdW1lVHJhbnNmZXJzVVNEVDI0aHI6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC10eG5zLXVzZHQtMjRocmAsIDIpLFxyXG4gICAgICAgICAgICB2b2x1bWVCdXkyNGhyOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS12b2wtYnV5LTI0aHJgLCAyKSxcclxuICAgICAgICAgICAgdm9sdW1lQnV5VVNEVDI0aHI6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC1idXktdXNkdC0yNGhyYCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVNlbGwyNGhyOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS12b2wtc2VsbC0yNGhyYCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVNlbGxVU0RUMjRocjogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXNlbGwtdXNkdC0yNGhyYCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVRyYWRlMjRocjogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXRyYWRlLTI0aHJgLCAyKSxcclxuICAgICAgICAgICAgdm9sdW1lVHJhZGVVU0RUMjRocjogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXRyYWRlLXVzZHQtMjRocmAsIDIpLFxyXG5cclxuXHJcbiAgICAgICAgICAgIHZvbHVtZVRyYW5zZmVyczdkYXk6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC10eG5zLTdkYXlgLCAyKSxcclxuICAgICAgICAgICAgdm9sdW1lVHJhbnNmZXJzVVNEVDdkYXk6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC10eG5zLXVzZHQtN2RheWAsIDIpLFxyXG4gICAgICAgICAgICB2b2x1bWVCdXk3ZGF5OiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS12b2wtYnV5LTdkYXlgLCAyKSxcclxuICAgICAgICAgICAgdm9sdW1lQnV5VVNEVDdkYXk6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXZvbC1idXktdXNkdC03ZGF5YCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVNlbGw3ZGF5OiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS12b2wtc2VsbC03ZGF5YCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVNlbGxVU0RUN2RheTogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXNlbGwtdXNkdC03ZGF5YCwgMiksXHJcbiAgICAgICAgICAgIHZvbHVtZVRyYWRlN2RheTogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXRyYWRlLTdkYXlgLCAyKSxcclxuICAgICAgICAgICAgdm9sdW1lVHJhZGVVU0RUN2RheTogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tdm9sLXRyYWRlLXVzZHQtN2RheWAsIDIpLFxyXG5cclxuICAgICAgICAgICAgc3Rha2VkVG9rZW46IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXN0YWtlZC10b2tlbmAsIDIpLFxyXG4gICAgICAgICAgICBzdGFrZWRVU0RUOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1zdGFrZWQtdXNkdGAsIDIpLFxyXG4gICAgICAgICAgICBzdGFrZWRQZXJjZW50OiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1zdGFrZWQtcGVyY2VudGAsIDQpLFxyXG4gICAgICAgICAgICBzdGFrZWRNdWx0aXBsaWVyOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1zdGFrZWQtbXVsdGlwbGllcmAsIDQpLFxyXG4gICAgICAgICAgICBzdXBwbHlPbkNoYWluOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1zdXBwbHktcGVyY2VudGAsIDQpLFxyXG4gICAgICAgICAgICBzdGFrZWRPbkNoYWluOiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1zdGFrZWQtb25jaGFpbi1wZXJjZW50YCwgNCksXHJcblxyXG4gICAgICAgICAgICByZXdhcmRzVG9rZW46IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXJld2FyZHMtdG9rZW5gLCAyKSxcclxuICAgICAgICAgICAgcmV3YXJkc1Rva2VuMjRocjogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tcmV3YXJkcy10b2tlbi0yNGhyYCwgMiksXHJcbiAgICAgICAgICAgIHJld2FyZHNUb2tlbjdkYXk6IENvaW4uQ3JlYXRlU3RhdChgLnRpY2tlci0ke25ldHdvcmt9LXJld2FyZHMtdG9rZW4tN2RheWAsIDIpLFxyXG4gICAgICAgICAgICByZXdhcmRzVVNEVDogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tcmV3YXJkcy11c2R0YCwgMiksXHJcbiAgICAgICAgICAgIHJld2FyZHNVU0RUMjRocjogQ29pbi5DcmVhdGVTdGF0KGAudGlja2VyLSR7bmV0d29ya30tcmV3YXJkcy11c2R0LTI0aHJgLCAyKSxcclxuICAgICAgICAgICAgcmV3YXJkc1VTRFQ3ZGF5OiBDb2luLkNyZWF0ZVN0YXQoYC50aWNrZXItJHtuZXR3b3JrfS1yZXdhcmRzLXVzZHQtN2RheWAsIDIpLFxyXG5cclxuICAgICAgICAgICAgKltTeW1ib2wuaXRlcmF0b3JdKCkge1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS5jb2luUHJpY2VVU0RUO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS5ob2xkZXJzO1xyXG5cclxuXHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLmxpcXVpZGl0eVRva2VuO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS5saXF1aWRpdHlDb2luO1xyXG5cclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEucmVzZXJ2ZXNDb2luO1xyXG5cclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lVHJhbnNmZXJzO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVCdXk7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnZvbHVtZVNlbGw7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnZvbHVtZVRyYWRlO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVTZWxsVVNEVDtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lQnV5VVNEVDtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lVHJhbnNmZXJzVVNEVDtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lVHJhZGVVU0RUO1xyXG5cclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lVHJhbnNmZXJzMjRocjtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lQnV5MjRocjtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lU2VsbDI0aHI7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnZvbHVtZVRyYWRlMjRocjtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lU2VsbFVTRFQyNGhyO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVCdXlVU0RUMjRocjtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lVHJhbnNmZXJzVVNEVDI0aHI7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnZvbHVtZVRyYWRlVVNEVDI0aHI7XHJcblxyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVUcmFuc2ZlcnM3ZGF5O1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVCdXk3ZGF5O1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVTZWxsN2RheTtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lVHJhZGU3ZGF5O1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVTZWxsVVNEVDdkYXk7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnZvbHVtZUJ1eVVTRFQ3ZGF5O1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS52b2x1bWVUcmFuc2ZlcnNVU0RUN2RheTtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudm9sdW1lVHJhZGVVU0RUN2RheTtcclxuXHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnN0YWtlZFRva2VuO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS5yZXdhcmRzVG9rZW47XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnJld2FyZHNUb2tlbjI0aHI7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnJld2FyZHNUb2tlbjdkYXk7XHJcblxyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS50b2tlblByaWNlQ29pbjtcclxuICAgICAgICAgICAgICAgIHlpZWxkIGRhdGEudG9rZW5QcmljZVVTRFQ7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLm1hcmtldENhcFVTRFQ7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLmxpcXVpZGl0eVBlcmNlbnQ7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLmxpcXVpZGl0eUNvaW5VU0RUO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS5yZXNlcnZlc0NvaW5VU0RUO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS5yZXNlcnZlc1BlcmNlbnQ7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnN0YWtlZFVTRFQ7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnN0YWtlZFBlcmNlbnQ7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnN0YWtlZE11bHRpcGxpZXI7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnN1cHBseU9uQ2hhaW47XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnN0YWtlZE9uQ2hhaW47XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnJld2FyZHNVU0RUO1xyXG4gICAgICAgICAgICAgICAgeWllbGQgZGF0YS5yZXdhcmRzVVNEVDI0aHI7XHJcbiAgICAgICAgICAgICAgICB5aWVsZCBkYXRhLnJld2FyZHNVU0RUN2RheTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH07XHJcblxyXG4gICAgICAgIHJldHVybiBkYXRhO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBhc3luYyBJbml0aWFsaXplKCkge1xyXG4gICAgICAgIHRoaXMudGltZXMgPSB7XHJcbiAgICAgICAgICAgIHRpbWVDdXJyZW50OiBwZXJmb3JtYW5jZS5ub3coKSAvIDEwMDAsXHJcbiAgICAgICAgICAgIHRpbWVOb3c6IHBlcmZvcm1hbmNlLm5vdygpIC8gMTAwMFxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgYW5pbWF0ZSgpIHtcclxuICAgICAgICB0aGlzLnRpbWVzLnRpbWVDdXJyZW50ID0gcGVyZm9ybWFuY2Uubm93KCkgLyAxMDAwO1xyXG4gICAgICAgIGNvbnN0IGRlbHRhID0gTWF0aC5taW4oMSwgKHRoaXMudGltZXMudGltZUN1cnJlbnQgLSB0aGlzLnRpbWVzLnRpbWVOb3cpIC8gQ29pbi5xdWVyeUZyZXF1ZW5jeSk7XHJcbiAgICAgICAgY29uc3QgZGVsdGFCaWcgPSBCaWcoZGVsdGEpO1xyXG5cclxuICAgICAgICBmb3IgKHZhciBpdGVtIG9mIHRoaXMuZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zdCBzdGF0ID0gaXRlbSBhcyBTdGF0O1xyXG4gICAgICAgICAgICBjb25zdCBjaGFuZ2UgPSBDb2luLmNhbGN1bGF0ZUNoYW5nZXMoc3RhdC5zYW1wbGUsIGRlbHRhQmlnKTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IHZhbHVlID0gc3RhdC5zYW1wbGUubm93LmludGVycG9sYXRlZDtcclxuICAgICAgICAgICAgbGV0IG5vdzogc3RyaW5nO1xyXG4gICAgICAgICAgICBpZiAodmFsdWUuZXEoMCkpe1xyXG4gICAgICAgICAgICAgICAgbm93ID0gXCItXCI7XHJcbiAgICAgICAgICAgIH0gZWxzZXtcclxuICAgICAgICAgICAgICAgIG5vdyA9IENvaW4uZm9ybWF0KHN0YXQuc2FtcGxlLm5vdy5pbnRlcnBvbGF0ZWQudG9GaXhlZChzdGF0LnByZWNpc2lvbikpO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBmb3IgKHZhciBlbGVtZW50IG9mIHN0YXQudmFsdWVzKSB7XHJcbiAgICAgICAgICAgICAgICBlbGVtZW50LnRleHRDb250ZW50ID0gbm93O1xyXG4gICAgICAgICAgICAgICAgaWYgKCFzdGF0LnZhbHVlc0Nzc1VwZGF0ZWQpe1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjaGFuZ2UgPiAwKXtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxlbWVudC5jbGFzc0xpc3QucmVtb3ZlIChcImRvd25cIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LmFkZCAoXCJ1cFwiKTtcclxuICAgICAgICAgICAgICAgICAgICB9IGVsc2UgaWYgKGNoYW5nZSA8IDApe1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUgKFwidXBcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LmFkZCAoXCJkb3duXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZSAoXCJkb3duXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUgKFwidXBcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBpZiAoIXN0YXQudmFsdWVzQ3NzVXBkYXRlZCkge1xyXG4gICAgICAgICAgICAgICAgc3RhdC52YWx1ZXNDc3NVcGRhdGVkID0gdHJ1ZTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgbGV0IHByZXZpb3VzID0gc3RhdC5zYW1wbGUueWVzdGVyZGF5LmludGVycG9sYXRlZDtcclxuICAgICAgICAgICAgaWYgKCFwcmV2aW91cy5lcSgwKSkge1xyXG4gICAgICAgICAgICAgICAgY29uc3QgY2hhbmdlID0gc3RhdC5zYW1wbGUubm93LmludGVycG9sYXRlZC5taW51cyhwcmV2aW91cykuZGl2KHByZXZpb3VzKS5tdWwoMTAwKTtcclxuICAgICAgICAgICAgICAgIENvaW4udXBkYXRlUGVyY2VudENoYW5nZShjaGFuZ2UsIHN0YXQuY2hhbmdlMjRocik7XHJcbiAgICAgICAgICAgIH0gZWxzZSBpZiAoc3RhdC5zYW1wbGUubm93LmludGVycG9sYXRlZC5ndCgwKSkge1xyXG4gICAgICAgICAgICAgICAgQ29pbi51cGRhdGVQZXJjZW50Q2hhbmdlSW5maW5pdHkoc3RhdC5jaGFuZ2UyNGhyKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgIENvaW4udXBkYXRlUGVyY2VudENoYW5nZShCaWcoMCksIHN0YXQuY2hhbmdlMjRocik7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcbiAgICBwdWJsaWMgc3RhdGljIHVwZGF0ZVBlcmNlbnRDaGFuZ2VJbmZpbml0eShlbGVtZW50c1RvVXBkYXRlOiBIVE1MU3BhbkVsZW1lbnRbXSkge1xyXG4gICAgICAgIGNvbnN0IHZhbHVlID0gXCIrIDxzcGFuIGNsYXNzPVxcXCJpbmZpbml0eVxcXCI+JmluZmluOyZuYnNwOzwvc3Bhbj5cIjtcclxuICAgICAgICBmb3IgKHZhciBpID0gMCwgdWwgPSBlbGVtZW50c1RvVXBkYXRlLmxlbmd0aDsgaSA8IHVsOyBpKyspIHtcclxuICAgICAgICAgICAgY29uc3QgZWxlbWVudCA9IGVsZW1lbnRzVG9VcGRhdGVbaV07XHJcbiAgICAgICAgICAgIGlmIChlbGVtZW50LnRleHRDb250ZW50LmluZGV4T2YoXCJcdTIyMUVcIikgPCAwKSB7XHJcbiAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUoXCJkb3duXCIpO1xyXG4gICAgICAgICAgICAgICAgZWxlbWVudC5jbGFzc0xpc3QuYWRkKFwidXBcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgXHJcbiAgICAgICAgICAgICAgICBlbGVtZW50LmlubmVySFRNTCA9IHZhbHVlO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgdXBkYXRlUGVyY2VudENoYW5nZShjaGFuZ2U6IEJpZywgZWxlbWVudHNUb1VwZGF0ZTogSFRNTFNwYW5FbGVtZW50W10pIHtcclxuICAgICAgICBjb25zdCB2YWx1ZSA9IChjaGFuZ2UuZ3QoMCkgPyBcIitcIiA6IGNoYW5nZS5sdCgwKSA/IFwiXCIgOiBcIlx1MDBCMVwiKSArXHJcbiAgICAgICAgICAgIHRoaXMuZm9ybWF0KGNoYW5nZS50b0ZpeGVkKDIpKTtcclxuICAgICAgICBmb3IgKHZhciBpID0gMCwgdWwgPSBlbGVtZW50c1RvVXBkYXRlLmxlbmd0aDsgaSA8IHVsOyBpKyspIHtcclxuICAgICAgICAgICAgY29uc3QgZWxlbWVudCA9IGVsZW1lbnRzVG9VcGRhdGVbaV07XHJcbiAgICAgICAgICAgIGlmIChlbGVtZW50LnRleHRDb250ZW50ICE9PSB2YWx1ZSkge1xyXG4gICAgICAgICAgICAgICAgaWYgKGNoYW5nZS5ndCgwKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZShcImRvd25cIik7XHJcbiAgICAgICAgICAgICAgICAgICAgZWxlbWVudC5jbGFzc0xpc3QuYWRkKFwidXBcIik7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2UgaWYgKGNoYW5nZS5sdCgwKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZShcInVwXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LmFkZChcImRvd25cIik7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZShcInVwXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZShcImRvd25cIik7XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKGNoYW5nZS5ndCg5OTkuOTkpKXtcclxuICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5hZGQoXCJsYXJnZVwiKTtcclxuICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUoXCJtZWRpdW1cIik7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2UgaWYgKGNoYW5nZS5ndGUoOTkuOTkpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgZWxlbWVudC5jbGFzc0xpc3QucmVtb3ZlKFwibGFyZ2VcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgZWxlbWVudC5jbGFzc0xpc3QuYWRkKFwibWVkaXVtXCIpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUoXCJsYXJnZVwiKTtcclxuICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUoXCJtZWRpdW1cIik7XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgZWxlbWVudC50ZXh0Q29udGVudCA9IHZhbHVlO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwdWJsaWMgc3RhdGljIFJlZnJlc2hEYXRhKFxyXG4gICAgICAgIGRhdGE6IERhdGEsIFxyXG4gICAgICAgIGNoYWluRGF0YTogQ2hhaW5EYXRhLCBcclxuICAgICAgICB0aW1lczoge1xyXG4gICAgICAgICAgICB0aW1lQ3VycmVudDogbnVtYmVyO1xyXG4gICAgICAgICAgICB0aW1lTm93OiBudW1iZXI7XHJcbiAgICAgICAgfSkge1xyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY29uc3QgYmxvY2tOdW1iZXIgPSBOdW1iZXIoY2hhaW5EYXRhLmN1cnJlbnQuYmxvY2tOdW1iZXIpO1xyXG4gICAgICAgICAgICBpZiAoYmxvY2tOdW1iZXIgPj0gZGF0YS5ibG9ja051bWJlci5jdXJyZW50KXtcclxuICAgICAgICAgICAgICAgIGRhdGEuYmxvY2tOdW1iZXIuY3VycmVudCA9IGJsb2NrTnVtYmVyO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgLy8gUmVqZWN0IG9sZCBibG9ja3NcclxuICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgQ29pbi5Qb3B1bGF0ZURhdGEoZGF0YSwgY2hhaW5EYXRhLmN1cnJlbnQsIGNoYWluRGF0YSwgXCJub3dcIiwgXCJoaXN0b3J5MjRocnNcIiwgXCJjdXJyZW50XCIsIFwiaGlzdG9yeTdkYXlcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBoaXN0b3J5QmxvY2sgPSBOdW1iZXIoY2hhaW5EYXRhLmhpc3RvcnkyNGhycy5ibG9ja051bWJlcik7XHJcbiAgICAgICAgaWYgKGRhdGEuYmxvY2tOdW1iZXIuaGlzdG9yeTI0aHIgPD0gaGlzdG9yeUJsb2NrKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgZGF0YS5ibG9ja051bWJlci5oaXN0b3J5MjRociA9IGhpc3RvcnlCbG9jaztcclxuXHJcbiAgICAgICAgICAgIENvaW4uUG9wdWxhdGVEYXRhKGRhdGEsIGNoYWluRGF0YS5oaXN0b3J5MjRocnMsIGNoYWluRGF0YSwgXCJ5ZXN0ZXJkYXlcIiwgXCJoaXN0b3J5NDhocnNcIiwgXCJoaXN0b3J5N2RheVwiLCBcImhpc3RvcnkxNGRheVwiKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgXHJcbiAgICAgICAgdGltZXMudGltZU5vdyA9IHBlcmZvcm1hbmNlLm5vdygpIC8gMTAwMDtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHN0YXRpYyBQb3B1bGF0ZURhdGEoZGF0YTogRGF0YSwgcGVyaW9kOiBQZXJpb2REYXRhLCBjaGFpbkRhdGE6IENoYWluRGF0YSwgd2hlbjogc3RyaW5nLCBiZWZvcmUyNGhyczogc3RyaW5nLCBjdXJyZW50N2RheTogc3RyaW5nLCBiZWZvcmU3ZGF5OiBzdHJpbmcpIHtcclxuICAgICAgICBsZXQgc2FtcGxlOiBTYW1wbGU7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5ob2xkZXJzLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCBCaWcocGVyaW9kLmhvbGRlcnMpKTtcclxuXHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5jb2luUHJpY2VVU0RULnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCBCaWcocGVyaW9kLmNvaW5QcmljZVN0YWJsZSkpO1xyXG5cclxuICAgICAgICBjb25zdCBtYXJrZXRDYXAgPSBCaWcocGVyaW9kLm1hcmtldENhcCk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5tYXJrZXRDYXBVU0RULnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCBtYXJrZXRDYXApO1xyXG5cclxuICAgICAgICBjb25zdCB1c2RSZXNlcnZlc0JhbGFuY2UgPSBCaWcocGVyaW9kLnVzZFJlc2VydmVzQmFsYW5jZSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5yZXNlcnZlc0NvaW4uc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIEJpZyhwZXJpb2QucmVzZXJ2ZXNCYWxhbmNlKSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5yZXNlcnZlc0NvaW5VU0RULnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB1c2RSZXNlcnZlc0JhbGFuY2UpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEucmVzZXJ2ZXNQZXJjZW50LnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB1c2RSZXNlcnZlc0JhbGFuY2UubXVsKDEwMCkuZGl2KG1hcmtldENhcCkpO1xyXG5cclxuICAgICAgICBzYW1wbGUgPSBkYXRhLmxpcXVpZGl0eVRva2VuLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCBCaWcocGVyaW9kLmxpcXVpZGl0eVRva2VuKSk7XHJcblxyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEubGlxdWlkaXR5Q29pbi5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgQmlnKHBlcmlvZC5saXF1aWRpdHlDb2luKSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5saXF1aWRpdHlDb2luVVNEVC5zYW1wbGU7XHJcbiAgICAgICAgY29uc3QgdXNkTGlxdWlkaXR5Q29pbiA9IEJpZyhwZXJpb2QudXNkTGlxdWlkaXR5Q29pbik7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdXNkTGlxdWlkaXR5Q29pbik7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5saXF1aWRpdHlQZXJjZW50LnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB1c2RMaXF1aWRpdHlDb2luLm11bCgxMDApLmRpdihtYXJrZXRDYXApKTtcclxuXHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5zdGFrZWRUb2tlbi5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgQmlnKHBlcmlvZC5zdGFrZWQpKTtcclxuICAgICAgICBzYW1wbGUgPSBkYXRhLnN0YWtlZE11bHRpcGxpZXIuc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIEJpZyhwZXJpb2QuYXZlTXVsdGlwbGllcikpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEuc3Rha2VkVVNEVC5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgQmlnKHBlcmlvZC51c2RTdGFrZWQpKTtcclxuXHJcbiAgICAgICAgY29uc3QgcmV3YXJkcyA9IEJpZyhwZXJpb2QucmV3YXJkcyk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5yZXdhcmRzVG9rZW4uc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIHJld2FyZHMpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEucmV3YXJkc1VTRFQuc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIEJpZyhwZXJpb2QudXNkUmV3YXJkcykpO1xyXG5cclxuICAgICAgICBjb25zdCB0b2tlblByaWNlU3RhYmxlID0gQmlnKHBlcmlvZC50b2tlblByaWNlU3RhYmxlKTtcclxuICAgICAgICBzYW1wbGUgPSBkYXRhLnRva2VuUHJpY2VVU0RULnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB0b2tlblByaWNlU3RhYmxlKTtcclxuXHJcbiAgICAgICAgbGV0IHZvbHVtZTI0aHIgPSByZXdhcmRzLm1pbnVzKGNoYWluRGF0YVtiZWZvcmUyNGhyc10ucmV3YXJkcyk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5yZXdhcmRzVG9rZW4yNGhyLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB2b2x1bWUyNGhyKTtcclxuICAgICAgICBzYW1wbGUgPSBkYXRhLnJld2FyZHNVU0RUMjRoci5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lMjRoci5tdWwodG9rZW5QcmljZVN0YWJsZSkpO1xyXG5cclxuICAgICAgICBsZXQgdm9sdW1lN2RheSA9IEJpZyhjaGFpbkRhdGFbY3VycmVudDdkYXldLnJld2FyZHMpLm1pbnVzKEJpZyhjaGFpbkRhdGFbYmVmb3JlN2RheV0ucmV3YXJkcykpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEucmV3YXJkc1Rva2VuN2RheS5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lN2RheSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS5yZXdhcmRzVVNEVDdkYXkuc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIHZvbHVtZTdkYXkubXVsKHRva2VuUHJpY2VTdGFibGUpKTtcclxuXHJcbiAgICAgICAgY29uc3Qgdm9sdW1lVHJhbnNmZXJzID0gQmlnKHBlcmlvZC52b2x1bWVUcmFuc2ZlcnMpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lVHJhbnNmZXJzLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB2b2x1bWVUcmFuc2ZlcnMpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lVHJhbnNmZXJzVVNEVC5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lVHJhbnNmZXJzLm11bCh0b2tlblByaWNlU3RhYmxlKSk7XHJcblxyXG4gICAgICAgIHZvbHVtZTI0aHIgPSB2b2x1bWVUcmFuc2ZlcnMubWludXMoY2hhaW5EYXRhW2JlZm9yZTI0aHJzXS52b2x1bWVUcmFuc2ZlcnMpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lVHJhbnNmZXJzMjRoci5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lMjRocik7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS52b2x1bWVUcmFuc2ZlcnNVU0RUMjRoci5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lMjRoci5tdWwodG9rZW5QcmljZVN0YWJsZSkpO1xyXG5cclxuICAgICAgICBjb25zdCB2b2x1bWVCdXkgPSBCaWcocGVyaW9kLnZvbHVtZUJ1eSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS52b2x1bWVCdXkuc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIHZvbHVtZUJ1eSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS52b2x1bWVCdXlVU0RULnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB2b2x1bWVCdXkubXVsKHRva2VuUHJpY2VTdGFibGUpKTtcclxuXHJcbiAgICAgICAgdm9sdW1lMjRociA9IHZvbHVtZUJ1eS5taW51cyhjaGFpbkRhdGFbYmVmb3JlMjRocnNdLnZvbHVtZUJ1eSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS52b2x1bWVCdXkyNGhyLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB2b2x1bWUyNGhyKTtcclxuICAgICAgICBzYW1wbGUgPSBkYXRhLnZvbHVtZUJ1eVVTRFQyNGhyLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB2b2x1bWUyNGhyLm11bCh0b2tlblByaWNlU3RhYmxlKSk7XHJcblxyXG4gICAgICAgIGNvbnN0IHZvbHVtZVNlbGwgPSBCaWcocGVyaW9kLnZvbHVtZVNlbGwpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lU2VsbC5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lU2VsbCk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS52b2x1bWVTZWxsVVNEVC5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lU2VsbC5tdWwodG9rZW5QcmljZVN0YWJsZSkpO1xyXG5cclxuICAgICAgICB2b2x1bWUyNGhyID0gdm9sdW1lU2VsbC5taW51cyhjaGFpbkRhdGFbYmVmb3JlMjRocnNdLnZvbHVtZVNlbGwpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lU2VsbDI0aHIuc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIHZvbHVtZTI0aHIpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lU2VsbFVTRFQyNGhyLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB2b2x1bWUyNGhyLm11bCh0b2tlblByaWNlU3RhYmxlKSk7XHJcblxyXG4gICAgICAgIGNvbnN0IHZvbHVtZVRyYWRlID0gQmlnKHBlcmlvZC52b2x1bWVUcmFkZSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS52b2x1bWVUcmFkZS5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lVHJhZGUpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lVHJhZGVVU0RULnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCB2b2x1bWVUcmFkZS5tdWwodG9rZW5QcmljZVN0YWJsZSkpO1xyXG5cclxuICAgICAgICB2b2x1bWUyNGhyID0gdm9sdW1lVHJhZGUubWludXMoY2hhaW5EYXRhW2JlZm9yZTI0aHJzXS52b2x1bWVUcmFkZSk7XHJcbiAgICAgICAgc2FtcGxlID0gZGF0YS52b2x1bWVUcmFkZTI0aHIuc2FtcGxlO1xyXG4gICAgICAgIENvaW4udXBkYXRlTWVhc3VyZShzYW1wbGVbd2hlbl0sIHZvbHVtZTI0aHIpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEudm9sdW1lVHJhZGVVU0RUMjRoci5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgdm9sdW1lMjRoci5tdWwodG9rZW5QcmljZVN0YWJsZSkpO1xyXG5cclxuICAgICAgICBzYW1wbGUgPSBkYXRhLnRva2VuUHJpY2VDb2luLnNhbXBsZTtcclxuICAgICAgICBDb2luLnVwZGF0ZU1lYXN1cmUoc2FtcGxlW3doZW5dLCBCaWcocGVyaW9kLnRva2VuUHJpY2VDb2luKSk7XHJcblxyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEuc3Rha2VkUGVyY2VudC5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgQmlnKHBlcmlvZC5zdGFrZWRPZlRvdGFsU3VwcGx5UGVyY2VudCkubXVsKDEwMCkpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEuc3VwcGx5T25DaGFpbi5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgQmlnKHBlcmlvZC5zdXBwbHlPbkNoYWluUGVyY2VudCkubXVsKDEwMCkpO1xyXG4gICAgICAgIHNhbXBsZSA9IGRhdGEuc3Rha2VkT25DaGFpbi5zYW1wbGU7XHJcbiAgICAgICAgQ29pbi51cGRhdGVNZWFzdXJlKHNhbXBsZVt3aGVuXSwgQmlnKHBlcmlvZC5zdGFrZWRPZk9uQ2hhaW5QZXJjZW50KS5tdWwoMTAwKSk7XHJcbiAgICAgICAgXHJcbiAgICAgICAgZm9yICh2YXIgaXRlbSBvZiBkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHN0YXQgPSBpdGVtIGFzIFN0YXQ7XHJcbiAgICAgICAgICAgIHN0YXQudmFsdWVzQ3NzVXBkYXRlZCA9IGZhbHNlO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHN0YXRpYyBDcmVhdGVTdGF0KGNsYXNzVmFsdWU6IHN0cmluZywgcHJlY2lzaW9uOiBudW1iZXIpOiBTdGF0IHtcclxuICAgICAgICBjb25zdCBjbGFzc0NoYW5nZSA9IGNsYXNzVmFsdWUgKyBcIi1jaGFuZ2VcIjtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBzYW1wbGU6IENvaW4uQ3JlYXRlU2FtcGxlKCksXHJcbiAgICAgICAgICAgIHByZWNpc2lvbjogcHJlY2lzaW9uLFxyXG4gICAgICAgICAgICB2YWx1ZXM6IFsuLi5kb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKGNsYXNzVmFsdWUpXSBhcyBIVE1MU3BhbkVsZW1lbnRbXSwgIFxyXG4gICAgICAgICAgICBjaGFuZ2UyNGhyOiBbLi4uZG9jdW1lbnQucXVlcnlTZWxlY3RvckFsbChjbGFzc0NoYW5nZSldIGFzIEhUTUxTcGFuRWxlbWVudFtdLCBcclxuICAgICAgICAgICAgdmFsdWVzQ3NzVXBkYXRlZDogZmFsc2VcclxuICAgICAgICB9O1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgdXBkYXRlTWVhc3VyZShtZWFzdXJlOiBWYWx1ZSwgdmFsdWU6IEJpZykge1xyXG4gICAgICAgIG1lYXN1cmUucHJldmlvdXMgPSBtZWFzdXJlLmludGVycG9sYXRlZDtcclxuICAgICAgICBtZWFzdXJlLmN1cnJlbnQgPSB2YWx1ZTtcclxuICAgICAgICBpZiAobWVhc3VyZS5wcmV2aW91cy5lcSgwKSkge1xyXG4gICAgICAgICAgICBtZWFzdXJlLnByZXZpb3VzID0gbWVhc3VyZS5pbnRlcnBvbGF0ZWQgPSBtZWFzdXJlLmN1cnJlbnQ7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgc3RhdGljIENyZWF0ZVNhbXBsZSgpOiBTYW1wbGUge1xyXG4gICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgIG5vdzogQ29pbi5DcmVhdGVWYWx1ZSgpLFxyXG4gICAgICAgICAgICB5ZXN0ZXJkYXk6IENvaW4uQ3JlYXRlVmFsdWUoKSxcclxuICAgICAgICB9O1xyXG4gICAgfVxyXG4gICAgcHJpdmF0ZSBzdGF0aWMgQ3JlYXRlVmFsdWUoKTogVmFsdWUge1xyXG4gICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgIGN1cnJlbnQ6IEJpZygwKSxcclxuICAgICAgICAgICAgaW50ZXJwb2xhdGVkOiBCaWcoMCksXHJcbiAgICAgICAgICAgIHByZXZpb3VzOiBCaWcoMClcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBjYWxjdWxhdGVDaGFuZ2VzKHNhbXBsZTogU2FtcGxlLCBkZWx0YTogQmlnKTogbnVtYmVyIHtcclxuICAgICAgICBjb25zdCBjaGFuZ2UgPSBDb2luLmNhbGN1bGF0ZVZhbHVlQ2hhbmdlcyhzYW1wbGUubm93LCBkZWx0YSk7XHJcbiAgICAgICAgQ29pbi5jYWxjdWxhdGVWYWx1ZUNoYW5nZXMoc2FtcGxlLnllc3RlcmRheSwgZGVsdGEpO1xyXG4gICAgICAgIHJldHVybiBjaGFuZ2U7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBjYWxjdWxhdGVWYWx1ZUNoYW5nZXModmFsdWU6IFZhbHVlLCBkZWx0YTogQmlnKTogbnVtYmVyIHtcclxuICAgICAgICBsZXQgY2hhbmdlID0gMDtcclxuICAgICAgICBpZiAoIXZhbHVlLmludGVycG9sYXRlZC5lcSh2YWx1ZS5jdXJyZW50KSkge1xyXG4gICAgICAgICAgICBjb25zdCBkaWZmID0gdmFsdWUuY3VycmVudC5taW51cyh2YWx1ZS5wcmV2aW91cykudGltZXMoZGVsdGEpO1xyXG4gICAgICAgICAgICBjb25zdCBtaW4gPSBDb2luLm1pbmltdW0odmFsdWUucHJldmlvdXMsIHZhbHVlLmN1cnJlbnQpO1xyXG4gICAgICAgICAgICBjb25zdCBtYXggPSBDb2luLm1heGltdW0odmFsdWUucHJldmlvdXMsIHZhbHVlLmN1cnJlbnQpO1xyXG4gICAgICAgICAgICBjb25zdCBuZXdWYWx1ZSA9IENvaW4ubWluaW11bShtYXgsIENvaW4ubWF4aW11bShtaW4sIHZhbHVlLnByZXZpb3VzLnBsdXMoZGlmZikpKTtcclxuICAgICAgICAgICAgaWYgKG5ld1ZhbHVlLmd0KHZhbHVlLmludGVycG9sYXRlZCkpe1xyXG4gICAgICAgICAgICAgICAgY2hhbmdlID0gMTtcclxuICAgICAgICAgICAgfSBlbHNlIGlmIChuZXdWYWx1ZS5sdCh2YWx1ZS5pbnRlcnBvbGF0ZWQpKSB7XHJcbiAgICAgICAgICAgICAgICBjaGFuZ2UgPSAtMVxyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICB2YWx1ZS5pbnRlcnBvbGF0ZWQgPSBuZXdWYWx1ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBjaGFuZ2U7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBmb3JtYXQobikge1xyXG4gICAgICAgIHZhciBwYXJ0cyA9IG4udG9TdHJpbmcoKS5zcGxpdChcIi5cIik7XHJcbiAgICAgICAgcmV0dXJuIHBhcnRzWzBdLnJlcGxhY2UoL1xcQig/PShcXGR7M30pKyg/IVxcZCkpL2csIFwiLFwiKSArIChwYXJ0c1sxXSA/IFwiLlwiICsgcGFydHNbMV0gOiBcIlwiKTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHN0YXRpYyBtaW5pbXVtKHg6IEJpZywgeTogQmlnKSB7XHJcbiAgICAgICAgaWYgKHguZ3QoeSkpIHJldHVybiB4O1xyXG4gICAgICAgIHJldHVybiB5O1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgc3RhdGljIG1heGltdW0oeDogQmlnLCB5OiBCaWcpIHtcclxuICAgICAgICBpZiAoeC5sdCh5KSkgcmV0dXJuIHg7XHJcbiAgICAgICAgcmV0dXJuIHk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmNsYXNzIFRva2VuU3RhdHMge1xyXG4gICAgcHJpdmF0ZSBjb2lucyA9IFtcclxuICAgICAgICBuZXcgQ29pbihcImJzY1wiKSxcclxuICAgICAgICBuZXcgQ29pbihcImV0aFwiKSxcclxuICAgICAgICBuZXcgQ29pbihcInBvbHlcIiksXHJcbiAgICAgICAgbmV3IENvaW4oXCJmdG1cIiksXHJcbiAgICAgICAgbmV3IENvaW4oXCJhdmF4XCIpXHJcbiAgICBdO1xyXG4gICAgcHJpdmF0ZSBkYXRhOiBEYXRhO1xyXG5cclxuICAgIHByaXZhdGUgcmVmcmVzaDogUHJvbWlzZTx2b2lkPjtcclxuICAgIHByaXZhdGUgaXNSZWZyZXNoaW5nOiBib29sZWFuID0gZmFsc2U7XHJcbiAgICBwcml2YXRlIHRpY2tlcjogSFRNTEVsZW1lbnQgPSBkb2N1bWVudC5nZXRFbGVtZW50QnlJZChcInRpY2tlclwiKTtcclxuICAgIHByaXZhdGUgdGlja2VyT2Zmc2V0ID0gMDtcclxuICAgIHByaXZhdGUgdGlja2VyTW92aW5nOiBib29sZWFuO1xyXG4gICAgcHJpdmF0ZSB0aW1lcyA9IHtcclxuICAgICAgICB0aW1lQ3VycmVudDogMCxcclxuICAgICAgICB0aW1lTm93OiAwXHJcbiAgICB9O1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKCkge1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBhc3luYyBJbml0aWFsaXplKCkge1xyXG5cclxuICAgICAgICB0aGlzLmRhdGEgPSBDb2luLkNyZWF0ZVN0YXRzKFwiYWxsXCIpO1xyXG5cclxuICAgICAgICBmb3IgKGxldCBpID0gMCwgdWwgPSB0aGlzLmNvaW5zLmxlbmd0aDsgaSA8IHVsOyBpKyspIHtcclxuICAgICAgICAgICAgdGhpcy5jb2luc1tpXS5Jbml0aWFsaXplKCk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHRoaXMudGltZXMgPSB7XHJcbiAgICAgICAgICAgIHRpbWVDdXJyZW50OiBwZXJmb3JtYW5jZS5ub3coKSAvIDEwMDAsXHJcbiAgICAgICAgICAgIHRpbWVOb3c6IHBlcmZvcm1hbmNlLm5vdygpIC8gMTAwMFxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgYXdhaXQgdGhpcy5SZWZyZXNoKCk7XHJcblxyXG4gICAgICAgIGlmICh0aGlzLnRpY2tlcil7XHJcbiAgICAgICAgICAgIHRoaXMudGlja2VyLnN0eWxlLmRpc3BsYXkgPSBcIlwiO1xyXG4gICAgICAgICAgICB0aGlzLnRpY2tlck1vdmluZyA9IHRydWU7XHJcblxyXG4gICAgICAgICAgICBmb3IgKHZhciBlIG9mIGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoXCIubmVlZHMtbGl2ZS1kYXRhXCIpKSB7XHJcbiAgICAgICAgICAgICAgICAoZSBhcyBIVE1MRWxlbWVudCkuY2xhc3NMaXN0LnJlbW92ZShcIm5lZWRzLWxpdmUtZGF0YVwiKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgdGhpcy50aWNrZXIuYWRkRXZlbnRMaXN0ZW5lcignbW91c2VlbnRlcicsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgdGhpcy50aWNrZXJNb3ZpbmcgPSBmYWxzZTtcclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICB0aGlzLnRpY2tlci5hZGRFdmVudExpc3RlbmVyKCdtb3VzZWxlYXZlJywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLnRpY2tlck1vdmluZyA9IHRydWU7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIFJlZnJlc2goKSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuaXNSZWZyZXNoaW5nID8gdGhpcy5yZWZyZXNoIDogKHRoaXMucmVmcmVzaCA9IHRoaXMuUmVmcmVzaERhdGEoKSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3luYyBSZWZyZXNoRGF0YSgpIHtcclxuICAgICAgICB0aGlzLmlzUmVmcmVzaGluZyA9IHRydWU7XHJcbiAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgY29uc3QgcmVxdWVzdCA9IGF3YWl0IGZldGNoKFwiaHR0cHM6Ly9ldmVycmlzZS5henVyZXdlYnNpdGVzLm5ldC9zdGF0c1wiKTtcclxuICAgICAgICAgICAgY29uc3QgYmxvY2tjaGFpbkRhdGEgPSAoYXdhaXQgcmVxdWVzdC5qc29uKCkpIGFzIEJsb2NrY2hhaW5EYXRhO1xyXG5cclxuICAgICAgICAgICAgZm9yIChsZXQgaSA9IDAsIHVsID0gdGhpcy5jb2lucy5sZW5ndGg7IGkgPCB1bDsgaSsrKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBjb2luID0gdGhpcy5jb2luc1tpXTtcclxuICAgICAgICAgICAgICAgIGNvbnN0IGNoYWluRGF0YSA9IGJsb2NrY2hhaW5EYXRhW2NvaW4ubmV0d29ya10gYXMgQ2hhaW5EYXRhO1xyXG4gICAgICAgICAgICAgICAgQ29pbi5SZWZyZXNoRGF0YShjb2luLmRhdGEsIGNoYWluRGF0YSwgY29pbi50aW1lcyk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIENvaW4uUmVmcmVzaERhdGEodGhpcy5kYXRhLCBibG9ja2NoYWluRGF0YVtcInVuaWZpZWRcIl0sIHRoaXMudGltZXMpO1xyXG4gICAgICAgIH0gZmluYWxseSB7XHJcbiAgICAgICAgICAgIHRoaXMuaXNSZWZyZXNoaW5nID0gZmFsc2U7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgc2tpcEZyYW1lID0gLTE7XHJcbiAgICBwcml2YXRlIGFuaW1hdGUoKSB7XHJcbiAgICAgICAgdGhpcy5hbmltYXRpb25SZXF1ZXN0ID0gcmVxdWVzdEFuaW1hdGlvbkZyYW1lKCgpID0+IHRoaXMuYW5pbWF0ZSgpKTtcclxuXHJcbiAgICAgICAgdGhpcy5za2lwRnJhbWUrKztcclxuICAgICAgICBpZiAodGhpcy5za2lwRnJhbWUgPiAzKSB7XHJcbiAgICAgICAgICAgIHRoaXMuc2tpcEZyYW1lID0gMDtcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKHRoaXMuc2tpcEZyYW1lICE9PSAwKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGZvciAobGV0IGkgPSAwLCB1bCA9IHRoaXMuY29pbnMubGVuZ3RoOyBpIDwgdWw7IGkrKykge1xyXG4gICAgICAgICAgICB0aGlzLmNvaW5zW2ldLmFuaW1hdGUoKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMudGltZXMudGltZUN1cnJlbnQgPSBwZXJmb3JtYW5jZS5ub3coKSAvIDEwMDA7XHJcbiAgICAgICAgY29uc3QgZGVsdGEgPSBNYXRoLm1pbigxLCAodGhpcy50aW1lcy50aW1lQ3VycmVudCAtIHRoaXMudGltZXMudGltZU5vdykgLyAzMCk7XHJcbiAgICAgICAgY29uc3QgZGVsdGFCaWcgPSBCaWcoZGVsdGEpO1xyXG5cclxuICAgICAgICBmb3IgKHZhciBpdGVtIG9mIHRoaXMuZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zdCBzdGF0ID0gaXRlbSBhcyBTdGF0O1xyXG4gICAgICAgICAgICBjb25zdCBjaGFuZ2UgPSBDb2luLmNhbGN1bGF0ZUNoYW5nZXMoc3RhdC5zYW1wbGUsIGRlbHRhQmlnKTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IHZhbHVlID0gc3RhdC5zYW1wbGUubm93LmludGVycG9sYXRlZDtcclxuICAgICAgICAgICAgbGV0IG5vdzogc3RyaW5nO1xyXG4gICAgICAgICAgICBpZiAodmFsdWUuZXEoMCkpe1xyXG4gICAgICAgICAgICAgICAgbm93ID0gXCItXCI7XHJcbiAgICAgICAgICAgIH0gZWxzZXtcclxuICAgICAgICAgICAgICAgIG5vdyA9IENvaW4uZm9ybWF0KHN0YXQuc2FtcGxlLm5vdy5pbnRlcnBvbGF0ZWQudG9GaXhlZChzdGF0LnByZWNpc2lvbikpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICBmb3IgKHZhciBlbGVtZW50IG9mIHN0YXQudmFsdWVzKSB7XHJcbiAgICAgICAgICAgICAgICBlbGVtZW50LnRleHRDb250ZW50ID0gbm93O1xyXG4gICAgICAgICAgICAgICAgaWYgKCFzdGF0LnZhbHVlc0Nzc1VwZGF0ZWQpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY2hhbmdlID4gMCl7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZSAoXCJkb3duXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5hZGQgKFwidXBcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgfSBlbHNlIGlmIChjaGFuZ2UgPCAwKXtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxlbWVudC5jbGFzc0xpc3QucmVtb3ZlIChcInVwXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5hZGQgKFwiZG93blwiKTtcclxuICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUgKFwiZG93blwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxlbWVudC5jbGFzc0xpc3QucmVtb3ZlIChcInVwXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgaWYgKCFzdGF0LnZhbHVlc0Nzc1VwZGF0ZWQpIHtcclxuICAgICAgICAgICAgICAgIHN0YXQudmFsdWVzQ3NzVXBkYXRlZCA9IHRydWU7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGxldCBwcmV2aW91cyA9IHN0YXQuc2FtcGxlLnllc3RlcmRheS5pbnRlcnBvbGF0ZWQ7XHJcbiAgICAgICAgICAgIGlmICghcHJldmlvdXMuZXEoMCkpIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IGNoYW5nZSA9IHN0YXQuc2FtcGxlLm5vdy5pbnRlcnBvbGF0ZWQubWludXMocHJldmlvdXMpLmRpdihwcmV2aW91cykubXVsKDEwMCk7XHJcbiAgICAgICAgICAgICAgICBDb2luLnVwZGF0ZVBlcmNlbnRDaGFuZ2UoY2hhbmdlLCBzdGF0LmNoYW5nZTI0aHIpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMudXBkYXRlUHJpY2VTdmcoKTtcclxuXHJcbiAgICAgICAgaWYgKHRoaXMudGlja2VyTW92aW5nKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IGhhbGZXaWR0aCA9IHRoaXMudGlja2VyLmdldEJvdW5kaW5nQ2xpZW50UmVjdCgpLndpZHRoIC8gMjtcclxuICAgICAgICAgICAgaWYgKHRoaXMudGlja2VyT2Zmc2V0ID4gaGFsZldpZHRoKSB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLnRpY2tlck9mZnNldCA9IDA7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgdGhpcy50aWNrZXIuc3R5bGUudHJhbnNmb3JtID0gYHRyYW5zbGF0ZSgtJHt0aGlzLnRpY2tlck9mZnNldH1weCwwKWA7XHJcbiAgICAgICAgICAgIHRoaXMudGlja2VyT2Zmc2V0ICs9IDAuNTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBpc1BhdXNlZCA9IHRydWU7XHJcbiAgICBwcml2YXRlIGFuaW1hdGlvblJlcXVlc3Q6IG51bWJlcjtcclxuICAgIHByaXZhdGUgZGF0YVJlZnJlc2hSZXF1ZXN0OiBudW1iZXI7XHJcblxyXG4gICAgcHJpdmF0ZSBjb2xvcnMgPSBbXHJcbiAgICAgICAgXCJyZ2JhKDIzNCwxODUsMiwxKVwiLFxyXG4gICAgICAgIFwicmdiYSgxMDMsMTA5LDE0MywxKVwiLFxyXG4gICAgICAgIFwicmdiYSgxMjQsNjUsMjEzLDEpXCIsXHJcbiAgICAgICAgXCJyZ2JhKDI1LDEwNSwyNTUsMSlcIixcclxuICAgICAgICBcInJnYmEoMjMyLDY1LDY2LDEpXCIsXHJcbiAgICAgICAgXCJyZ2JhKDI1NSwyNTUsMjU1LDEpXCJcclxuICAgIF07XHJcblxyXG4gICAgcHJpdmF0ZSB1cGRhdGVQcmljZVN2ZygpIHtcclxuICAgICAgICBjb25zdCBncmFwaFdpZHRoID0gNDIwO1xyXG4gICAgICAgIHZhciBtaW4gPSAxMDAwMDAwO1xyXG4gICAgICAgIHZhciBtYXggPSAwO1xyXG4gICAgICAgIGZvciAobGV0IGkgPSAwLCB1bCA9IHRoaXMuY29pbnMubGVuZ3RoOyBpIDwgdWw7IGkrKykge1xyXG4gICAgICAgICAgICB2YXIgdmFsdWUgPSB0aGlzLmNvaW5zW2ldLmRhdGEudG9rZW5QcmljZVVTRFQuc2FtcGxlLm5vdy5pbnRlcnBvbGF0ZWQudG9OdW1iZXIoKTtcclxuICAgICAgICAgICAgaWYgKG1pbiA+IHZhbHVlKSB7XHJcbiAgICAgICAgICAgICAgICBtaW4gPSB2YWx1ZTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBpZiAobWF4IDwgdmFsdWUpIHtcclxuICAgICAgICAgICAgICAgIG1heCA9IHZhbHVlO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB2YXIgcmFuZ2UgPSBtYXggLSBtaW47XHJcbiAgICAgICAgdmFyIGJ1ZmZlciA9IHJhbmdlICogMC4wMjU7XHJcbiAgICAgICAgdmFyIG1pblggPSBtaW4gLSBidWZmZXI7XHJcbiAgICAgICAgcmFuZ2UgKj0gMS4wNTtcclxuXHJcbiAgICAgICAgY29uc3QgcHJpY2VHcmFwaHMgPSBbLi4uZG9jdW1lbnQucXVlcnlTZWxlY3RvckFsbChcIi5ncmFwaC1wcmljZVJhbmdlXCIpXSBhcyBTVkdHRWxlbWVudFtdO1xyXG4gICAgICAgIGlmIChwcmljZUdyYXBocy5sZW5ndGggPiAwICYmIHJhbmdlID4gMCkge1xyXG4gICAgICAgICAgICBjb25zdCB1bmlmaWVkUHJpY2UgPSB0aGlzLmRhdGEudG9rZW5QcmljZVVTRFQuc2FtcGxlLm5vdy5pbnRlcnBvbGF0ZWQudG9OdW1iZXIoKTtcclxuICAgICAgICAgICAgbGV0IGdyYXBocyA9IFwiXCI7XHJcbiAgICAgICAgICAgIGxldCBwcmljZXMgPSBuZXcgQXJyYXkodGhpcy5jb2lucy5sZW5ndGgpO1xyXG4gICAgICAgICAgICBsZXQgcG9zaXRpb25zID0gbmV3IEFycmF5KHRoaXMuY29pbnMubGVuZ3RoKTtcclxuICAgICAgICAgICAgZm9yIChsZXQgaSA9IDAsIHVsID0gdGhpcy5jb2lucy5sZW5ndGg7IGkgPCB1bDsgaSsrKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBjb2luID0gdGhpcy5jb2luc1tpXTtcclxuICAgICAgICAgICAgICAgIGNvbnN0IHZhbHVlID0gY29pbi5kYXRhLnRva2VuUHJpY2VVU0RULnNhbXBsZS5ub3cuaW50ZXJwb2xhdGVkLnRvTnVtYmVyKCk7XHJcbiAgICAgICAgICAgICAgICBwcmljZXNbaV0gPSB2YWx1ZTtcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAodmFsdWUgPD0gMCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc3QgeCA9ICgoKHZhbHVlIC0gbWluWCkgKiBncmFwaFdpZHRoKSAvIHJhbmdlKTtcclxuICAgICAgICAgICAgICAgIHBvc2l0aW9uc1tpXSA9IHg7XHJcbiAgICAgICAgICAgICAgICBjb25zdCB4VGV4dCA9IHgudG9GaXhlZCgyKTtcclxuXHJcbiAgICAgICAgICAgICAgICBmb3IgKHZhciBpY29uIG9mIGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoYC5ncmFwaC1pY29uLSR7Y29pbi5uZXR3b3JrfWApKXtcclxuICAgICAgICAgICAgICAgICAgICBpY29uLnNldEF0dHJpYnV0ZShcInhcIiwgKHggLSA4KS50b0ZpeGVkKDIpKTtcclxuICAgICAgICAgICAgICAgICAgICBpY29uLnNldEF0dHJpYnV0ZShcInlcIiwgXCI4XCIpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGdyYXBocyArPSBgXHJcbiAgICAgICAgICAgICAgICA8bGluZSB4MT1cIiR7eFRleHR9XCIgeDI9XCIke3hUZXh0fVwiIHkxPVwiMjJcIiB5Mj1cIjU2XCIgc3Ryb2tlPVwiJHt0aGlzLmNvbG9yc1tpXX1cIiBzdHJva2Utd2lkdGg9XCIyXCIvPmA7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgXHJcbiAgICAgICAgICAgIGlmICh1bmlmaWVkUHJpY2UgPiAwKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCB4ID0gKCgodW5pZmllZFByaWNlIC0gbWluWCkgKiBncmFwaFdpZHRoKSAvIHJhbmdlKTtcclxuICAgICAgICAgICAgICAgIGNvbnN0IHhUZXh0ID0geC50b0ZpeGVkKDIpO1xyXG5cclxuICAgICAgICAgICAgICAgIGZvciAodmFyIGljb24gb2YgZG9jdW1lbnQucXVlcnlTZWxlY3RvckFsbChgLmdyYXBoLWljb24tcmlzZWApKXtcclxuICAgICAgICAgICAgICAgICAgICBpY29uLnNldEF0dHJpYnV0ZShcInhcIiwgKHggLSA4KS50b0ZpeGVkKDIpKTtcclxuICAgICAgICAgICAgICAgICAgICBpY29uLnNldEF0dHJpYnV0ZShcInlcIiwgXCIwXCIpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgZm9yIChsZXQgaSA9IDAsIHVsID0gcHJpY2VzLmxlbmd0aDsgaSA8IHVsOyBpKyspIHtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBwcmljZSA9IHByaWNlc1tpXTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocHJpY2UgPD0gMCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCB4UG9zO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChwb3NpdGlvbnNbaV0gPiB4KXtcclxuICAgICAgICAgICAgICAgICAgICAgICAgeFBvcyA9IHggKyAocG9zaXRpb25zW2ldIC0geCkgLyAyIC0gMjA7XHJcbiAgICAgICAgICAgICAgICAgICAgfWVsc2V7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHhQb3MgPSBwb3NpdGlvbnNbaV0gKyAoeCAtIHBvc2l0aW9uc1tpXSkgLyAyIC0gMjA7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBwZXJjZW50Q2hhbmdlID0gKCgocHJpY2UgLSB1bmlmaWVkUHJpY2UpICogMTAwKSAvdW5pZmllZFByaWNlKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgeVRleHQgPSAoMjQgKyBpICogOCk7XHJcbiAgICAgICAgICAgICAgICAgICAgZ3JhcGhzICs9IGBcclxuICAgICAgICAgICAgICAgICAgICA8bGluZSB4MT1cIiR7cG9zaXRpb25zW2ldLnRvRml4ZWQoMil9XCIgc3Ryb2tlLWRhc2hhcnJheT1cIjJcIiB4Mj1cIiR7eFRleHR9XCIgeTE9XCIke3lUZXh0fVwiIHkyPVwiJHt5VGV4dH1cIiBzdHJva2U9XCIke3RoaXMuY29sb3JzW2ldfVwiIHN0cm9rZS13aWR0aD1cIjFcIi8+XHJcbiAgICAgICAgICAgICAgICAgICAgPHRleHQgeD1cIiR7eFBvcy50b0ZpeGVkKDIpfVwiIHk9XCIke3lUZXh0IC0gNH1cIiBjbGFzcz1cImdyYXBoLXRleHQtcGVyY2VudFwiPiR7cGVyY2VudENoYW5nZSA+IDAgPyBcIitcIiA6IFwiXCJ9JHtwZXJjZW50Q2hhbmdlLnRvRml4ZWQoMil9JTwvdGV4dD5gO1xyXG5cclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBncmFwaHMgKz0gYFxyXG4gICAgICAgICAgICAgICAgPGxpbmUgeDE9XCIke3hUZXh0fVwiIHgyPVwiJHt4VGV4dH1cIiB5MT1cIjE4XCIgeTI9XCI1NlwiIHN0cm9rZT1cIiR7dGhpcy5jb2xvcnNbdGhpcy5jb2xvcnMubGVuZ3RoIC0gMV19XCIgc3Ryb2tlLXdpZHRoPVwiM1wiLz5cclxuICAgICAgICAgICAgICAgIDx0ZXh0IHg9XCIwXCIgeT1cIjY2XCIgY2xhc3M9XCJncmFwaC10ZXh0LXByaWNlXCI+JHttaW4udG9GaXhlZCg3KX08L3RleHQ+XHJcbiAgICAgICAgICAgICAgICA8dGV4dCB4PVwiJHt4VGV4dH1cIiB5PVwiNjZcIiBjbGFzcz1cImdyYXBoLXRleHQtcHJpY2VcIiB0ZXh0LWFuY2hvcj1cIm1pZGRsZVwiPiR7dW5pZmllZFByaWNlLnRvRml4ZWQoNyl9PC90ZXh0PlxyXG4gICAgICAgICAgICAgICAgPHRleHQgeD1cIiR7Z3JhcGhXaWR0aH1cIiB5PVwiNjZcIiBjbGFzcz1cImdyYXBoLXRleHQtcHJpY2VcIiB0ZXh0LWFuY2hvcj1cImVuZFwiPiR7bWF4LnRvRml4ZWQoNyl9PC90ZXh0PmA7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGZvciAodmFyIGcgb2YgcHJpY2VHcmFwaHMpIHtcclxuICAgICAgICAgICAgICAgIGcuaW5uZXJIVE1MID0gZ3JhcGhzO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgU3RhcnQoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMuaXNQYXVzZWQpIHtcclxuICAgICAgICAgICAgdGhpcy5pc1BhdXNlZCA9IGZhbHNlO1xyXG4gICAgICAgICAgICB0aGlzLmFuaW1hdGlvblJlcXVlc3QgPSByZXF1ZXN0QW5pbWF0aW9uRnJhbWUoKCkgPT4gdGhpcy5hbmltYXRlKCkpO1xyXG4gICAgICAgICAgICBzZXRUaW1lb3V0KCgpID0+IHRoaXMuUmVmcmVzaCgpLCAxNTAwMCk7XHJcbiAgICAgICAgICAgIHRoaXMuZGF0YVJlZnJlc2hSZXF1ZXN0ID0gc2V0SW50ZXJ2YWwoKCkgPT4gdGhpcy5SZWZyZXNoKCksIDMwMDAwKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFBhdXNlKCkge1xyXG4gICAgICAgIGlmICh0aGlzLmlzUGF1c2VkKSByZXR1cm47XHJcblxyXG4gICAgICAgIHRoaXMuaXNQYXVzZWQgPSB0cnVlO1xyXG4gICAgICAgIGNsZWFySW50ZXJ2YWwodGhpcy5kYXRhUmVmcmVzaFJlcXVlc3QpO1xyXG4gICAgICAgIGNhbmNlbEFuaW1hdGlvbkZyYW1lKHRoaXMuYW5pbWF0aW9uUmVxdWVzdCk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmNvbnN0IHN0YXRzID0gbmV3IFRva2VuU3RhdHMoKTtcclxuc3RhdHMuSW5pdGlhbGl6ZSgpXHJcbnN0YXRzLlN0YXJ0KCk7Il0sCiAgIm1hcHBpbmdzIjogIjs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7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
