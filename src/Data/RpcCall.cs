// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class RpcCall
{
    public RpcCall(string statsAddress, string blockNumber = "latest")
    {
        @params = new object[]
        {
            new DataParams {
                data = "0xc59d4847000000000000000000000000",
                to = statsAddress
            },
            blockNumber
        };
    }

    public string jsonrpc { get; set; } = "2.0";
    public int id { get; set; } = 1;
    public string method { get; set; } = "eth_call";
    public object[] @params { get; set; }
}
