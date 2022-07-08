// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class RpcCall
{
    public RpcCall(string address, string method, int id, string blockNumber = "latest")
    {
        @params = new object[]
        {
            new DataParams {
                data = method,
                to = address
            },
            blockNumber
        };
        this.id = id;
    }
    public RpcCall(string address, int id, string blockNumber = "latest")
    {
        @params = new object[]
        {
            address,
            blockNumber
        };
        this.method = "eth_getBalance";
        this.id = id;
    }

    public string jsonrpc { get; set; } = "2.0";
    public int id { get; set; }
    public string method { get; set; } = "eth_call";
    public object[] @params { get; set; }
}