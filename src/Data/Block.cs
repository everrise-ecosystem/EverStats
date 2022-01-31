// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class Block
{
#nullable disable
    public string jsonrpc { get; set; }
    public int id { get; set; }
    public BlockResult result { get; set; }
}
