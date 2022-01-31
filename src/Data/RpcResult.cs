// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class RpcResult
{
#nullable disable
    public string jsonrpc { get; set; }
    public int id { get; set; }
    public string result { get; set; }
}
