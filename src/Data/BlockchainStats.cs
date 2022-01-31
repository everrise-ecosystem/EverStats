// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace EverStats.Data;
public class BlockchainStats
{
    public readonly string Chain;

    public BlockchainStats(string chain)
    {
        Chain = chain;
    }

#nullable disable
    public BlockchainSample current { get; set; }
    public BlockchainSample history24hrs { get; set; }
    public BlockchainSample history48hrs { get; set; }
    public BlockchainSample history7day { get; set; }
    public BlockchainSample history14day { get; set; }

    public void CreateStringRepresentations()
    {
        current?.CreateStringRepresentations();
        history24hrs?.CreateStringRepresentations();
        history48hrs?.CreateStringRepresentations();
        history7day?.CreateStringRepresentations();
        history14day?.CreateStringRepresentations();
    }
}

