﻿// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class BlockResult
{
#nullable disable
    public string difficulty { get; set; }
    public string extraData { get; set; }
    public string gasLimit { get; set; }
    public string gasUsed { get; set; }
    public string hash { get; set; }
    public string logsBloom { get; set; }
    public string miner { get; set; }
    public string mixHash { get; set; }
    public string nonce { get; set; }
    public string number { get; set; }
    public string parentHash { get; set; }
    public string receiptsRoot { get; set; }
    public string sha3Uncles { get; set; }
    public string size { get; set; }
    public string stateRoot { get; set; }
    public string timestamp { get; set; }
    public string totalDifficulty { get; set; }
    public string[] transactions { get; set; }
    public string transactionsRoot { get; set; }
    public object[] uncles { get; set; }
}
