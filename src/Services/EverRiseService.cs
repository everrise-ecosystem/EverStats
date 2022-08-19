using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Contracts.Contracts.EverRise.ContractDefinition;
using Nethereum.Contracts.Constants;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Contracts.Standards.ERC20;

namespace Contracts.Contracts.EverRise
{
    public partial class EverRiseService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, EverRiseDeployment everRiseDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<EverRiseDeployment>().SendRequestAndWaitForReceiptAsync(everRiseDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, EverRiseDeployment everRiseDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<EverRiseDeployment>().SendRequestAsync(everRiseDeployment);
        }

        public static async Task<EverRiseService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, EverRiseDeployment everRiseDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, everRiseDeployment, cancellationTokenSource);
            return new EverRiseService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public EverRiseService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<byte[]> DomainSeparatorQueryAsync(DomainSeparatorFunction domainSeparatorFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DomainSeparatorFunction, byte[]>(domainSeparatorFunction, blockParameter);
        }

        
        public Task<byte[]> DomainSeparatorQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DomainSeparatorFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> DomainTypehashQueryAsync(DomainTypehashFunction domainTypehashFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DomainTypehashFunction, byte[]>(domainTypehashFunction, blockParameter);
        }

        
        public Task<byte[]> DomainTypehashQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DomainTypehashFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> PermitTypehashQueryAsync(PermitTypehashFunction permitTypehashFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PermitTypehashFunction, byte[]>(permitTypehashFunction, blockParameter);
        }

        
        public Task<byte[]> PermitTypehashQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PermitTypehashFunction, byte[]>(null, blockParameter);
        }

        public Task<string> AddControlRoleRequestAsync(AddControlRoleFunction addControlRoleFunction)
        {
             return ContractHandler.SendRequestAsync(addControlRoleFunction);
        }

        public Task<TransactionReceipt> AddControlRoleRequestAndWaitForReceiptAsync(AddControlRoleFunction addControlRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addControlRoleFunction, cancellationToken);
        }

        public Task<string> AddControlRoleRequestAsync(string newController, byte role)
        {
            var addControlRoleFunction = new AddControlRoleFunction();
                addControlRoleFunction.NewController = newController;
                addControlRoleFunction.Role = role;
            
             return ContractHandler.SendRequestAsync(addControlRoleFunction);
        }

        public Task<TransactionReceipt> AddControlRoleRequestAndWaitForReceiptAsync(string newController, byte role, CancellationTokenSource cancellationToken = null)
        {
            var addControlRoleFunction = new AddControlRoleFunction();
                addControlRoleFunction.NewController = newController;
                addControlRoleFunction.Role = role;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addControlRoleFunction, cancellationToken);
        }

        public Task<string> AddExchangeHotWalletRequestAsync(AddExchangeHotWalletFunction addExchangeHotWalletFunction)
        {
             return ContractHandler.SendRequestAsync(addExchangeHotWalletFunction);
        }

        public Task<TransactionReceipt> AddExchangeHotWalletRequestAndWaitForReceiptAsync(AddExchangeHotWalletFunction addExchangeHotWalletFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addExchangeHotWalletFunction, cancellationToken);
        }

        public Task<string> AddExchangeHotWalletRequestAsync(string account)
        {
            var addExchangeHotWalletFunction = new AddExchangeHotWalletFunction();
                addExchangeHotWalletFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(addExchangeHotWalletFunction);
        }

        public Task<TransactionReceipt> AddExchangeHotWalletRequestAndWaitForReceiptAsync(string account, CancellationTokenSource cancellationToken = null)
        {
            var addExchangeHotWalletFunction = new AddExchangeHotWalletFunction();
                addExchangeHotWalletFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addExchangeHotWalletFunction, cancellationToken);
        }

        public Task<BigInteger> AllowanceQueryAsync(AllowanceFunction allowanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AllowanceFunction, BigInteger>(allowanceFunction, blockParameter);
        }

        
        public Task<BigInteger> AllowanceQueryAsync(string owner, string spender, BlockParameter blockParameter = null)
        {
            var allowanceFunction = new AllowanceFunction();
                allowanceFunction.Owner = owner;
                allowanceFunction.Spender = spender;
            
            return ContractHandler.QueryAsync<AllowanceFunction, BigInteger>(allowanceFunction, blockParameter);
        }

        public Task<AllowancesOutputDTO> AllowancesQueryAsync(AllowancesFunction allowancesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<AllowancesFunction, AllowancesOutputDTO>(allowancesFunction, blockParameter);
        }

        public Task<AllowancesOutputDTO> AllowancesQueryAsync(string returnValue1, string returnValue2, BlockParameter blockParameter = null)
        {
            var allowancesFunction = new AllowancesFunction();
                allowancesFunction.ReturnValue1 = returnValue1;
                allowancesFunction.ReturnValue2 = returnValue2;
            
            return ContractHandler.QueryDeserializingToObjectAsync<AllowancesFunction, AllowancesOutputDTO>(allowancesFunction, blockParameter);
        }

        public Task<ApprovalsOutputDTO> ApprovalsQueryAsync(ApprovalsFunction approvalsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<ApprovalsFunction, ApprovalsOutputDTO>(approvalsFunction, blockParameter);
        }

        public Task<ApprovalsOutputDTO> ApprovalsQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var approvalsFunction = new ApprovalsFunction();
                approvalsFunction.Account = account;
            
            return ContractHandler.QueryDeserializingToObjectAsync<ApprovalsFunction, ApprovalsOutputDTO>(approvalsFunction, blockParameter);
        }

        public Task<string> ApproveRequestAsync(ApproveFunction approveFunction)
        {
             return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(ApproveFunction approveFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<string> ApproveRequestAsync(string spender, BigInteger amount)
        {
            var approveFunction = new ApproveFunction();
                approveFunction.Spender = spender;
                approveFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(string spender, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var approveFunction = new ApproveFunction();
                approveFunction.Spender = spender;
                approveFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<string> ApproveNFTAndTokensRequestAsync(ApproveNFTAndTokensFunction approveNFTAndTokensFunction)
        {
             return ContractHandler.SendRequestAsync(approveNFTAndTokensFunction);
        }

        public Task<TransactionReceipt> ApproveNFTAndTokensRequestAndWaitForReceiptAsync(ApproveNFTAndTokensFunction approveNFTAndTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveNFTAndTokensFunction, cancellationToken);
        }

        public Task<string> ApproveNFTAndTokensRequestAsync(string bridgeAddress, BigInteger nftId, BigInteger tokenAmount)
        {
            var approveNFTAndTokensFunction = new ApproveNFTAndTokensFunction();
                approveNFTAndTokensFunction.BridgeAddress = bridgeAddress;
                approveNFTAndTokensFunction.NftId = nftId;
                approveNFTAndTokensFunction.TokenAmount = tokenAmount;
            
             return ContractHandler.SendRequestAsync(approveNFTAndTokensFunction);
        }

        public Task<TransactionReceipt> ApproveNFTAndTokensRequestAndWaitForReceiptAsync(string bridgeAddress, BigInteger nftId, BigInteger tokenAmount, CancellationTokenSource cancellationToken = null)
        {
            var approveNFTAndTokensFunction = new ApproveNFTAndTokensFunction();
                approveNFTAndTokensFunction.BridgeAddress = bridgeAddress;
                approveNFTAndTokensFunction.NftId = nftId;
                approveNFTAndTokensFunction.TokenAmount = tokenAmount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveNFTAndTokensFunction, cancellationToken);
        }

        public Task<bool> AutoBurnQueryAsync(AutoBurnFunction autoBurnFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AutoBurnFunction, bool>(autoBurnFunction, blockParameter);
        }

        
        public Task<bool> AutoBurnQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AutoBurnFunction, bool>(null, blockParameter);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction balanceOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        
        public Task<BigInteger> BalanceOfQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var balanceOfFunction = new BalanceOfFunction();
                balanceOfFunction.Account = account;
            
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        public Task<string> BridgeStakeNftInRequestAsync(BridgeStakeNftInFunction bridgeStakeNftInFunction)
        {
             return ContractHandler.SendRequestAsync(bridgeStakeNftInFunction);
        }

        public Task<TransactionReceipt> BridgeStakeNftInRequestAndWaitForReceiptAsync(BridgeStakeNftInFunction bridgeStakeNftInFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bridgeStakeNftInFunction, cancellationToken);
        }

        public Task<string> BridgeStakeNftInRequestAsync(string toAddress, BigInteger depositTokens, byte numOfMonths, ulong depositTime, BigInteger withdrawnAmount, bool achievementClaimed)
        {
            var bridgeStakeNftInFunction = new BridgeStakeNftInFunction();
                bridgeStakeNftInFunction.ToAddress = toAddress;
                bridgeStakeNftInFunction.DepositTokens = depositTokens;
                bridgeStakeNftInFunction.NumOfMonths = numOfMonths;
                bridgeStakeNftInFunction.DepositTime = depositTime;
                bridgeStakeNftInFunction.WithdrawnAmount = withdrawnAmount;
                bridgeStakeNftInFunction.AchievementClaimed = achievementClaimed;
            
             return ContractHandler.SendRequestAsync(bridgeStakeNftInFunction);
        }

        public Task<TransactionReceipt> BridgeStakeNftInRequestAndWaitForReceiptAsync(string toAddress, BigInteger depositTokens, byte numOfMonths, ulong depositTime, BigInteger withdrawnAmount, bool achievementClaimed, CancellationTokenSource cancellationToken = null)
        {
            var bridgeStakeNftInFunction = new BridgeStakeNftInFunction();
                bridgeStakeNftInFunction.ToAddress = toAddress;
                bridgeStakeNftInFunction.DepositTokens = depositTokens;
                bridgeStakeNftInFunction.NumOfMonths = numOfMonths;
                bridgeStakeNftInFunction.DepositTime = depositTime;
                bridgeStakeNftInFunction.WithdrawnAmount = withdrawnAmount;
                bridgeStakeNftInFunction.AchievementClaimed = achievementClaimed;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bridgeStakeNftInFunction, cancellationToken);
        }

        public Task<string> BridgeStakeNftOutRequestAsync(BridgeStakeNftOutFunction bridgeStakeNftOutFunction)
        {
             return ContractHandler.SendRequestAsync(bridgeStakeNftOutFunction);
        }

        public Task<TransactionReceipt> BridgeStakeNftOutRequestAndWaitForReceiptAsync(BridgeStakeNftOutFunction bridgeStakeNftOutFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bridgeStakeNftOutFunction, cancellationToken);
        }

        public Task<string> BridgeStakeNftOutRequestAsync(string fromAddress, BigInteger nftId)
        {
            var bridgeStakeNftOutFunction = new BridgeStakeNftOutFunction();
                bridgeStakeNftOutFunction.FromAddress = fromAddress;
                bridgeStakeNftOutFunction.NftId = nftId;
            
             return ContractHandler.SendRequestAsync(bridgeStakeNftOutFunction);
        }

        public Task<TransactionReceipt> BridgeStakeNftOutRequestAndWaitForReceiptAsync(string fromAddress, BigInteger nftId, CancellationTokenSource cancellationToken = null)
        {
            var bridgeStakeNftOutFunction = new BridgeStakeNftOutFunction();
                bridgeStakeNftOutFunction.FromAddress = fromAddress;
                bridgeStakeNftOutFunction.NftId = nftId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bridgeStakeNftOutFunction, cancellationToken);
        }

        public Task<BigInteger> BridgeVaultLockedBalanceQueryAsync(BridgeVaultLockedBalanceFunction bridgeVaultLockedBalanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BridgeVaultLockedBalanceFunction, BigInteger>(bridgeVaultLockedBalanceFunction, blockParameter);
        }

        
        public Task<BigInteger> BridgeVaultLockedBalanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BridgeVaultLockedBalanceFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> BurnAddressQueryAsync(BurnAddressFunction burnAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BurnAddressFunction, string>(burnAddressFunction, blockParameter);
        }

        
        public Task<string> BurnAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BurnAddressFunction, string>(null, blockParameter);
        }

        public Task<string> BusinessDevelopmentAddressQueryAsync(BusinessDevelopmentAddressFunction businessDevelopmentAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BusinessDevelopmentAddressFunction, string>(businessDevelopmentAddressFunction, blockParameter);
        }

        
        public Task<string> BusinessDevelopmentAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BusinessDevelopmentAddressFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> BusinessDevelopmentDivisorQueryAsync(BusinessDevelopmentDivisorFunction businessDevelopmentDivisorFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BusinessDevelopmentDivisorFunction, BigInteger>(businessDevelopmentDivisorFunction, blockParameter);
        }

        
        public Task<BigInteger> BusinessDevelopmentDivisorQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BusinessDevelopmentDivisorFunction, BigInteger>(null, blockParameter);
        }

        public Task<bool> BuyBackEnabledQueryAsync(BuyBackEnabledFunction buyBackEnabledFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BuyBackEnabledFunction, bool>(buyBackEnabledFunction, blockParameter);
        }

        
        public Task<bool> BuyBackEnabledQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BuyBackEnabledFunction, bool>(null, blockParameter);
        }

        public Task<string> CrossChainBuybackRequestAsync(CrossChainBuybackFunction crossChainBuybackFunction)
        {
             return ContractHandler.SendRequestAsync(crossChainBuybackFunction);
        }

        public Task<string> CrossChainBuybackRequestAsync()
        {
             return ContractHandler.SendRequestAsync<CrossChainBuybackFunction>();
        }

        public Task<TransactionReceipt> CrossChainBuybackRequestAndWaitForReceiptAsync(CrossChainBuybackFunction crossChainBuybackFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(crossChainBuybackFunction, cancellationToken);
        }

        public Task<TransactionReceipt> CrossChainBuybackRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<CrossChainBuybackFunction>(null, cancellationToken);
        }

        public Task<byte> DecimalsQueryAsync(DecimalsFunction decimalsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DecimalsFunction, byte>(decimalsFunction, blockParameter);
        }

        
        public Task<byte> DecimalsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DecimalsFunction, byte>(null, blockParameter);
        }

        public Task<string> EarlyWithdrawRequestAsync(EarlyWithdrawFunction earlyWithdrawFunction)
        {
             return ContractHandler.SendRequestAsync(earlyWithdrawFunction);
        }

        public Task<TransactionReceipt> EarlyWithdrawRequestAndWaitForReceiptAsync(EarlyWithdrawFunction earlyWithdrawFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(earlyWithdrawFunction, cancellationToken);
        }

        public Task<string> EarlyWithdrawRequestAsync(BigInteger nftId, BigInteger amount)
        {
            var earlyWithdrawFunction = new EarlyWithdrawFunction();
                earlyWithdrawFunction.NftId = nftId;
                earlyWithdrawFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(earlyWithdrawFunction);
        }

        public Task<TransactionReceipt> EarlyWithdrawRequestAndWaitForReceiptAsync(BigInteger nftId, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var earlyWithdrawFunction = new EarlyWithdrawFunction();
                earlyWithdrawFunction.NftId = nftId;
                earlyWithdrawFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(earlyWithdrawFunction, cancellationToken);
        }

        public Task<string> EnterStakingRequestAsync(EnterStakingFunction enterStakingFunction)
        {
             return ContractHandler.SendRequestAsync(enterStakingFunction);
        }

        public Task<TransactionReceipt> EnterStakingRequestAndWaitForReceiptAsync(EnterStakingFunction enterStakingFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(enterStakingFunction, cancellationToken);
        }

        public Task<string> EnterStakingRequestAsync(BigInteger amount, byte numOfMonths)
        {
            var enterStakingFunction = new EnterStakingFunction();
                enterStakingFunction.Amount = amount;
                enterStakingFunction.NumOfMonths = numOfMonths;
            
             return ContractHandler.SendRequestAsync(enterStakingFunction);
        }

        public Task<TransactionReceipt> EnterStakingRequestAndWaitForReceiptAsync(BigInteger amount, byte numOfMonths, CancellationTokenSource cancellationToken = null)
        {
            var enterStakingFunction = new EnterStakingFunction();
                enterStakingFunction.Amount = amount;
                enterStakingFunction.NumOfMonths = numOfMonths;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(enterStakingFunction, cancellationToken);
        }

        public Task<string> EverBridgeVaultQueryAsync(EverBridgeVaultFunction everBridgeVaultFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EverBridgeVaultFunction, string>(everBridgeVaultFunction, blockParameter);
        }

        
        public Task<string> EverBridgeVaultQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EverBridgeVaultFunction, string>(null, blockParameter);
        }

        public Task<string> ExcludeFromFeeRequestAsync(ExcludeFromFeeFunction excludeFromFeeFunction)
        {
             return ContractHandler.SendRequestAsync(excludeFromFeeFunction);
        }

        public Task<TransactionReceipt> ExcludeFromFeeRequestAndWaitForReceiptAsync(ExcludeFromFeeFunction excludeFromFeeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(excludeFromFeeFunction, cancellationToken);
        }

        public Task<string> ExcludeFromFeeRequestAsync(string account)
        {
            var excludeFromFeeFunction = new ExcludeFromFeeFunction();
                excludeFromFeeFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(excludeFromFeeFunction);
        }

        public Task<TransactionReceipt> ExcludeFromFeeRequestAndWaitForReceiptAsync(string account, CancellationTokenSource cancellationToken = null)
        {
            var excludeFromFeeFunction = new ExcludeFromFeeFunction();
                excludeFromFeeFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(excludeFromFeeFunction, cancellationToken);
        }

        public Task<string> ExtendLockTokensAndNftsRequestAsync(ExtendLockTokensAndNftsFunction extendLockTokensAndNftsFunction)
        {
             return ContractHandler.SendRequestAsync(extendLockTokensAndNftsFunction);
        }

        public Task<TransactionReceipt> ExtendLockTokensAndNftsRequestAndWaitForReceiptAsync(ExtendLockTokensAndNftsFunction extendLockTokensAndNftsFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(extendLockTokensAndNftsFunction, cancellationToken);
        }

        public Task<string> ExtendLockTokensAndNftsRequestAsync(ulong length)
        {
            var extendLockTokensAndNftsFunction = new ExtendLockTokensAndNftsFunction();
                extendLockTokensAndNftsFunction.Length = length;
            
             return ContractHandler.SendRequestAsync(extendLockTokensAndNftsFunction);
        }

        public Task<TransactionReceipt> ExtendLockTokensAndNftsRequestAndWaitForReceiptAsync(ulong length, CancellationTokenSource cancellationToken = null)
        {
            var extendLockTokensAndNftsFunction = new ExtendLockTokensAndNftsFunction();
                extendLockTokensAndNftsFunction.Length = length;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(extendLockTokensAndNftsFunction, cancellationToken);
        }

        public Task<BigInteger> GetAmountLockedQueryAsync(GetAmountLockedFunction getAmountLockedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAmountLockedFunction, BigInteger>(getAmountLockedFunction, blockParameter);
        }

        
        public Task<BigInteger> GetAmountLockedQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var getAmountLockedFunction = new GetAmountLockedFunction();
                getAmountLockedFunction.Account = account;
            
            return ContractHandler.QueryAsync<GetAmountLockedFunction, BigInteger>(getAmountLockedFunction, blockParameter);
        }

        public async Task<List<BigInteger>> GetAmountLockedUsingMultiCallAsync(IEnumerable<string> ownerAddresses,
            BlockParameter block = null,
             int numberOfCallsPerRequest = MultiQueryHandler.DEFAULT_CALLS_PER_REQUEST,
             string multiCallAddress = CommonAddresses.MULTICALL_ADDRESS)
        {
            var balanceCalls = new List<MulticallInputOutput<GetAmountLockedFunction, GetAmountLockedOutputDTO>>();
            foreach (var ownerAddress in ownerAddresses)
            {
                var balanceCall = new GetAmountLockedFunction() { Account = ownerAddress };
                balanceCalls.Add(new MulticallInputOutput<GetAmountLockedFunction, GetAmountLockedOutputDTO>(balanceCall,
                    ContractHandler.ContractAddress));

            }

            var multiqueryHandler = ContractHandler.EthApiContractService.GetMultiQueryHandler(multiCallAddress);
            var results = await multiqueryHandler.MultiCallAsync(numberOfCallsPerRequest, balanceCalls.ToArray());
            return balanceCalls.Select(x => x.Output.ReturnValue1).ToList();
        }

        public Task<bool> HasTokenStartedQueryAsync(HasTokenStartedFunction hasTokenStartedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HasTokenStartedFunction, bool>(hasTokenStartedFunction, blockParameter);
        }

        
        public Task<bool> HasTokenStartedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HasTokenStartedFunction, bool>(null, blockParameter);
        }

        public Task<BigInteger> HoldersQueryAsync(HoldersFunction holdersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HoldersFunction, BigInteger>(holdersFunction, blockParameter);
        }

        
        public Task<BigInteger> HoldersQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HoldersFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> IncludeInFeeRequestAsync(IncludeInFeeFunction includeInFeeFunction)
        {
             return ContractHandler.SendRequestAsync(includeInFeeFunction);
        }

        public Task<TransactionReceipt> IncludeInFeeRequestAndWaitForReceiptAsync(IncludeInFeeFunction includeInFeeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(includeInFeeFunction, cancellationToken);
        }

        public Task<string> IncludeInFeeRequestAsync(string account)
        {
            var includeInFeeFunction = new IncludeInFeeFunction();
                includeInFeeFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(includeInFeeFunction);
        }

        public Task<TransactionReceipt> IncludeInFeeRequestAndWaitForReceiptAsync(string account, CancellationTokenSource cancellationToken = null)
        {
            var includeInFeeFunction = new IncludeInFeeFunction();
                includeInFeeFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(includeInFeeFunction, cancellationToken);
        }

        public Task<string> IncreaseStakeRequestAsync(IncreaseStakeFunction increaseStakeFunction)
        {
             return ContractHandler.SendRequestAsync(increaseStakeFunction);
        }

        public Task<TransactionReceipt> IncreaseStakeRequestAndWaitForReceiptAsync(IncreaseStakeFunction increaseStakeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(increaseStakeFunction, cancellationToken);
        }

        public Task<string> IncreaseStakeRequestAsync(BigInteger nftId, BigInteger amount)
        {
            var increaseStakeFunction = new IncreaseStakeFunction();
                increaseStakeFunction.NftId = nftId;
                increaseStakeFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(increaseStakeFunction);
        }

        public Task<TransactionReceipt> IncreaseStakeRequestAndWaitForReceiptAsync(BigInteger nftId, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var increaseStakeFunction = new IncreaseStakeFunction();
                increaseStakeFunction.NftId = nftId;
                increaseStakeFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(increaseStakeFunction, cancellationToken);
        }

        public Task<bool> IsApprovedForAllQueryAsync(IsApprovedForAllFunction isApprovedForAllFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }

        
        public Task<bool> IsApprovedForAllQueryAsync(string account, string @operator, BlockParameter blockParameter = null)
        {
            var isApprovedForAllFunction = new IsApprovedForAllFunction();
                isApprovedForAllFunction.Account = account;
                isApprovedForAllFunction.Operator = @operator;
            
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }

        public Task<bool> IsExchangeHotWalletQueryAsync(IsExchangeHotWalletFunction isExchangeHotWalletFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsExchangeHotWalletFunction, bool>(isExchangeHotWalletFunction, blockParameter);
        }

        
        public Task<bool> IsExchangeHotWalletQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var isExchangeHotWalletFunction = new IsExchangeHotWalletFunction();
                isExchangeHotWalletFunction.Account = account;
            
            return ContractHandler.QueryAsync<IsExchangeHotWalletFunction, bool>(isExchangeHotWalletFunction, blockParameter);
        }

        public Task<bool> IsExcludedFromFeeQueryAsync(IsExcludedFromFeeFunction isExcludedFromFeeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsExcludedFromFeeFunction, bool>(isExcludedFromFeeFunction, blockParameter);
        }

        
        public Task<bool> IsExcludedFromFeeQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var isExcludedFromFeeFunction = new IsExcludedFromFeeFunction();
                isExcludedFromFeeFunction.Account = account;
            
            return ContractHandler.QueryAsync<IsExcludedFromFeeFunction, bool>(isExcludedFromFeeFunction, blockParameter);
        }

        public Task<bool> IsWalletLockedQueryAsync(IsWalletLockedFunction isWalletLockedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsWalletLockedFunction, bool>(isWalletLockedFunction, blockParameter);
        }

        
        public Task<bool> IsWalletLockedQueryAsync(string fromAddress, BlockParameter blockParameter = null)
        {
            var isWalletLockedFunction = new IsWalletLockedFunction();
                isWalletLockedFunction.FromAddress = fromAddress;
            
            return ContractHandler.QueryAsync<IsWalletLockedFunction, bool>(isWalletLockedFunction, blockParameter);
        }

        public Task<string> LeaveStakingRequestAsync(LeaveStakingFunction leaveStakingFunction)
        {
             return ContractHandler.SendRequestAsync(leaveStakingFunction);
        }

        public Task<TransactionReceipt> LeaveStakingRequestAndWaitForReceiptAsync(LeaveStakingFunction leaveStakingFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(leaveStakingFunction, cancellationToken);
        }

        public Task<string> LeaveStakingRequestAsync(BigInteger nftId, bool overrideNotClaimed)
        {
            var leaveStakingFunction = new LeaveStakingFunction();
                leaveStakingFunction.NftId = nftId;
                leaveStakingFunction.OverrideNotClaimed = overrideNotClaimed;
            
             return ContractHandler.SendRequestAsync(leaveStakingFunction);
        }

        public Task<TransactionReceipt> LeaveStakingRequestAndWaitForReceiptAsync(BigInteger nftId, bool overrideNotClaimed, CancellationTokenSource cancellationToken = null)
        {
            var leaveStakingFunction = new LeaveStakingFunction();
                leaveStakingFunction.NftId = nftId;
                leaveStakingFunction.OverrideNotClaimed = overrideNotClaimed;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(leaveStakingFunction, cancellationToken);
        }

        public Task<BigInteger> LiquidityFeeQueryAsync(LiquidityFeeFunction liquidityFeeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LiquidityFeeFunction, BigInteger>(liquidityFeeFunction, blockParameter);
        }

        
        public Task<BigInteger> LiquidityFeeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LiquidityFeeFunction, BigInteger>(null, blockParameter);
        }

        public Task<bool> LiquidityLockedQueryAsync(LiquidityLockedFunction liquidityLockedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LiquidityLockedFunction, bool>(liquidityLockedFunction, blockParameter);
        }

        
        public Task<bool> LiquidityLockedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LiquidityLockedFunction, bool>(null, blockParameter);
        }

        public Task<string> LockTokensAndNftsRequestAsync(LockTokensAndNftsFunction lockTokensAndNftsFunction)
        {
             return ContractHandler.SendRequestAsync(lockTokensAndNftsFunction);
        }

        public Task<TransactionReceipt> LockTokensAndNftsRequestAndWaitForReceiptAsync(LockTokensAndNftsFunction lockTokensAndNftsFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(lockTokensAndNftsFunction, cancellationToken);
        }

        public Task<string> LockTokensAndNftsRequestAsync(string altAccount, ulong length)
        {
            var lockTokensAndNftsFunction = new LockTokensAndNftsFunction();
                lockTokensAndNftsFunction.AltAccount = altAccount;
                lockTokensAndNftsFunction.Length = length;
            
             return ContractHandler.SendRequestAsync(lockTokensAndNftsFunction);
        }

        public Task<TransactionReceipt> LockTokensAndNftsRequestAndWaitForReceiptAsync(string altAccount, ulong length, CancellationTokenSource cancellationToken = null)
        {
            var lockTokensAndNftsFunction = new LockTokensAndNftsFunction();
                lockTokensAndNftsFunction.AltAccount = altAccount;
                lockTokensAndNftsFunction.Length = length;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(lockTokensAndNftsFunction, cancellationToken);
        }

        public Task<string> MirgateV1V2HolderRequestAsync(MirgateV1V2HolderFunction mirgateV1V2HolderFunction)
        {
             return ContractHandler.SendRequestAsync(mirgateV1V2HolderFunction);
        }

        public Task<TransactionReceipt> MirgateV1V2HolderRequestAndWaitForReceiptAsync(MirgateV1V2HolderFunction mirgateV1V2HolderFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mirgateV1V2HolderFunction, cancellationToken);
        }

        public Task<string> MirgateV1V2HolderRequestAsync(string holder, BigInteger amount)
        {
            var mirgateV1V2HolderFunction = new MirgateV1V2HolderFunction();
                mirgateV1V2HolderFunction.Holder = holder;
                mirgateV1V2HolderFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(mirgateV1V2HolderFunction);
        }

        public Task<TransactionReceipt> MirgateV1V2HolderRequestAndWaitForReceiptAsync(string holder, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var mirgateV1V2HolderFunction = new MirgateV1V2HolderFunction();
                mirgateV1V2HolderFunction.Holder = holder;
                mirgateV1V2HolderFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mirgateV1V2HolderFunction, cancellationToken);
        }

        public Task<string> MirgateV2StakerRequestAsync(MirgateV2StakerFunction mirgateV2StakerFunction)
        {
             return ContractHandler.SendRequestAsync(mirgateV2StakerFunction);
        }

        public Task<TransactionReceipt> MirgateV2StakerRequestAndWaitForReceiptAsync(MirgateV2StakerFunction mirgateV2StakerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mirgateV2StakerFunction, cancellationToken);
        }

        public Task<string> MirgateV2StakerRequestAsync(string toAddress, BigInteger rewards, BigInteger depositTokens, byte numOfMonths, ulong depositTime, BigInteger withdrawnAmount)
        {
            var mirgateV2StakerFunction = new MirgateV2StakerFunction();
                mirgateV2StakerFunction.ToAddress = toAddress;
                mirgateV2StakerFunction.Rewards = rewards;
                mirgateV2StakerFunction.DepositTokens = depositTokens;
                mirgateV2StakerFunction.NumOfMonths = numOfMonths;
                mirgateV2StakerFunction.DepositTime = depositTime;
                mirgateV2StakerFunction.WithdrawnAmount = withdrawnAmount;
            
             return ContractHandler.SendRequestAsync(mirgateV2StakerFunction);
        }

        public Task<TransactionReceipt> MirgateV2StakerRequestAndWaitForReceiptAsync(string toAddress, BigInteger rewards, BigInteger depositTokens, byte numOfMonths, ulong depositTime, BigInteger withdrawnAmount, CancellationTokenSource cancellationToken = null)
        {
            var mirgateV2StakerFunction = new MirgateV2StakerFunction();
                mirgateV2StakerFunction.ToAddress = toAddress;
                mirgateV2StakerFunction.Rewards = rewards;
                mirgateV2StakerFunction.DepositTokens = depositTokens;
                mirgateV2StakerFunction.NumOfMonths = numOfMonths;
                mirgateV2StakerFunction.DepositTime = depositTime;
                mirgateV2StakerFunction.WithdrawnAmount = withdrawnAmount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mirgateV2StakerFunction, cancellationToken);
        }

        public Task<string> NameQueryAsync(NameFunction nameFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(nameFunction, blockParameter);
        }

        
        public Task<string> NameQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> NoncesQueryAsync(NoncesFunction noncesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NoncesFunction, BigInteger>(noncesFunction, blockParameter);
        }

        
        public Task<BigInteger> NoncesQueryAsync(string owner, BlockParameter blockParameter = null)
        {
            var noncesFunction = new NoncesFunction();
                noncesFunction.Owner = owner;
            
            return ContractHandler.QueryAsync<NoncesFunction, BigInteger>(noncesFunction, blockParameter);
        }

        public Task<bool> OffchainBalanceExcludedQueryAsync(OffchainBalanceExcludedFunction offchainBalanceExcludedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OffchainBalanceExcludedFunction, bool>(offchainBalanceExcludedFunction, blockParameter);
        }

        
        public Task<bool> OffchainBalanceExcludedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OffchainBalanceExcludedFunction, bool>(null, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> PermitRequestAsync(PermitFunction permitFunction)
        {
             return ContractHandler.SendRequestAsync(permitFunction);
        }

        public Task<TransactionReceipt> PermitRequestAndWaitForReceiptAsync(PermitFunction permitFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(permitFunction, cancellationToken);
        }

        public Task<string> PermitRequestAsync(string owner, string spender, BigInteger value, BigInteger deadline, byte v, byte[] r, byte[] s)
        {
            var permitFunction = new PermitFunction();
                permitFunction.Owner = owner;
                permitFunction.Spender = spender;
                permitFunction.Value = value;
                permitFunction.Deadline = deadline;
                permitFunction.V = v;
                permitFunction.R = r;
                permitFunction.S = s;
            
             return ContractHandler.SendRequestAsync(permitFunction);
        }

        public Task<TransactionReceipt> PermitRequestAndWaitForReceiptAsync(string owner, string spender, BigInteger value, BigInteger deadline, byte v, byte[] r, byte[] s, CancellationTokenSource cancellationToken = null)
        {
            var permitFunction = new PermitFunction();
                permitFunction.Owner = owner;
                permitFunction.Spender = spender;
                permitFunction.Value = value;
                permitFunction.Deadline = deadline;
                permitFunction.V = v;
                permitFunction.R = r;
                permitFunction.S = s;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(permitFunction, cancellationToken);
        }

        public Task<string> RemoveControlRoleRequestAsync(RemoveControlRoleFunction removeControlRoleFunction)
        {
             return ContractHandler.SendRequestAsync(removeControlRoleFunction);
        }

        public Task<TransactionReceipt> RemoveControlRoleRequestAndWaitForReceiptAsync(RemoveControlRoleFunction removeControlRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeControlRoleFunction, cancellationToken);
        }

        public Task<string> RemoveControlRoleRequestAsync(string oldController, byte role)
        {
            var removeControlRoleFunction = new RemoveControlRoleFunction();
                removeControlRoleFunction.OldController = oldController;
                removeControlRoleFunction.Role = role;
            
             return ContractHandler.SendRequestAsync(removeControlRoleFunction);
        }

        public Task<TransactionReceipt> RemoveControlRoleRequestAndWaitForReceiptAsync(string oldController, byte role, CancellationTokenSource cancellationToken = null)
        {
            var removeControlRoleFunction = new RemoveControlRoleFunction();
                removeControlRoleFunction.OldController = oldController;
                removeControlRoleFunction.Role = role;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeControlRoleFunction, cancellationToken);
        }

        public Task<string> RemoveExchangeHotWalletRequestAsync(RemoveExchangeHotWalletFunction removeExchangeHotWalletFunction)
        {
             return ContractHandler.SendRequestAsync(removeExchangeHotWalletFunction);
        }

        public Task<TransactionReceipt> RemoveExchangeHotWalletRequestAndWaitForReceiptAsync(RemoveExchangeHotWalletFunction removeExchangeHotWalletFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeExchangeHotWalletFunction, cancellationToken);
        }

        public Task<string> RemoveExchangeHotWalletRequestAsync(string account)
        {
            var removeExchangeHotWalletFunction = new RemoveExchangeHotWalletFunction();
                removeExchangeHotWalletFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(removeExchangeHotWalletFunction);
        }

        public Task<TransactionReceipt> RemoveExchangeHotWalletRequestAndWaitForReceiptAsync(string account, CancellationTokenSource cancellationToken = null)
        {
            var removeExchangeHotWalletFunction = new RemoveExchangeHotWalletFunction();
                removeExchangeHotWalletFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeExchangeHotWalletFunction, cancellationToken);
        }

        public Task<string> RevokeApprovalsRequestAsync(RevokeApprovalsFunction revokeApprovalsFunction)
        {
             return ContractHandler.SendRequestAsync(revokeApprovalsFunction);
        }

        public Task<TransactionReceipt> RevokeApprovalsRequestAndWaitForReceiptAsync(RevokeApprovalsFunction revokeApprovalsFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeApprovalsFunction, cancellationToken);
        }

        public Task<string> RevokeApprovalsRequestAsync(bool tokens, bool nfts)
        {
            var revokeApprovalsFunction = new RevokeApprovalsFunction();
                revokeApprovalsFunction.Tokens = tokens;
                revokeApprovalsFunction.Nfts = nfts;
            
             return ContractHandler.SendRequestAsync(revokeApprovalsFunction);
        }

        public Task<TransactionReceipt> RevokeApprovalsRequestAndWaitForReceiptAsync(bool tokens, bool nfts, CancellationTokenSource cancellationToken = null)
        {
            var revokeApprovalsFunction = new RevokeApprovalsFunction();
                revokeApprovalsFunction.Tokens = tokens;
                revokeApprovalsFunction.Nfts = nfts;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeApprovalsFunction, cancellationToken);
        }

        public Task<bool> RolesQueryAsync(RolesFunction rolesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RolesFunction, bool>(rolesFunction, blockParameter);
        }

        
        public Task<bool> RolesQueryAsync(byte returnValue1, string returnValue2, BlockParameter blockParameter = null)
        {
            var rolesFunction = new RolesFunction();
                rolesFunction.ReturnValue1 = returnValue1;
                rolesFunction.ReturnValue2 = returnValue2;
            
            return ContractHandler.QueryAsync<RolesFunction, bool>(rolesFunction, blockParameter);
        }

        public Task<string> SetApprovalForAllRequestAsync(SetApprovalForAllFunction setApprovalForAllFunction)
        {
             return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(SetApprovalForAllFunction setApprovalForAllFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(string fromAddress, string @operator, bool approved)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
                setApprovalForAllFunction.FromAddress = fromAddress;
                setApprovalForAllFunction.Operator = @operator;
                setApprovalForAllFunction.Approved = approved;
            
             return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(string fromAddress, string @operator, bool approved, CancellationTokenSource cancellationToken = null)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
                setApprovalForAllFunction.FromAddress = fromAddress;
                setApprovalForAllFunction.Operator = @operator;
                setApprovalForAllFunction.Approved = approved;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<string> SetAutoBurnRequestAsync(SetAutoBurnFunction setAutoBurnFunction)
        {
             return ContractHandler.SendRequestAsync(setAutoBurnFunction);
        }

        public Task<TransactionReceipt> SetAutoBurnRequestAndWaitForReceiptAsync(SetAutoBurnFunction setAutoBurnFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAutoBurnFunction, cancellationToken);
        }

        public Task<string> SetAutoBurnRequestAsync(bool enabled)
        {
            var setAutoBurnFunction = new SetAutoBurnFunction();
                setAutoBurnFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAsync(setAutoBurnFunction);
        }

        public Task<TransactionReceipt> SetAutoBurnRequestAndWaitForReceiptAsync(bool enabled, CancellationTokenSource cancellationToken = null)
        {
            var setAutoBurnFunction = new SetAutoBurnFunction();
                setAutoBurnFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAutoBurnFunction, cancellationToken);
        }

        public Task<string> SetAutoTimeoutRequestAsync(SetAutoTimeoutFunction setAutoTimeoutFunction)
        {
             return ContractHandler.SendRequestAsync(setAutoTimeoutFunction);
        }

        public Task<TransactionReceipt> SetAutoTimeoutRequestAndWaitForReceiptAsync(SetAutoTimeoutFunction setAutoTimeoutFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAutoTimeoutFunction, cancellationToken);
        }

        public Task<string> SetAutoTimeoutRequestAsync(ushort tokensHrs, ushort nftsHrs)
        {
            var setAutoTimeoutFunction = new SetAutoTimeoutFunction();
                setAutoTimeoutFunction.TokensHrs = tokensHrs;
                setAutoTimeoutFunction.NftsHrs = nftsHrs;
            
             return ContractHandler.SendRequestAsync(setAutoTimeoutFunction);
        }

        public Task<TransactionReceipt> SetAutoTimeoutRequestAndWaitForReceiptAsync(ushort tokensHrs, ushort nftsHrs, CancellationTokenSource cancellationToken = null)
        {
            var setAutoTimeoutFunction = new SetAutoTimeoutFunction();
                setAutoTimeoutFunction.TokensHrs = tokensHrs;
                setAutoTimeoutFunction.NftsHrs = nftsHrs;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAutoTimeoutFunction, cancellationToken);
        }

        public Task<string> SetBurnAddressRequestAsync(SetBurnAddressFunction setBurnAddressFunction)
        {
             return ContractHandler.SendRequestAsync(setBurnAddressFunction);
        }

        public Task<TransactionReceipt> SetBurnAddressRequestAndWaitForReceiptAsync(SetBurnAddressFunction setBurnAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBurnAddressFunction, cancellationToken);
        }

        public Task<string> SetBurnAddressRequestAsync(string burnAddress)
        {
            var setBurnAddressFunction = new SetBurnAddressFunction();
                setBurnAddressFunction.BurnAddress = burnAddress;
            
             return ContractHandler.SendRequestAsync(setBurnAddressFunction);
        }

        public Task<TransactionReceipt> SetBurnAddressRequestAndWaitForReceiptAsync(string burnAddress, CancellationTokenSource cancellationToken = null)
        {
            var setBurnAddressFunction = new SetBurnAddressFunction();
                setBurnAddressFunction.BurnAddress = burnAddress;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBurnAddressFunction, cancellationToken);
        }

        public Task<string> SetBurnableTokensZeroRequestAsync(SetBurnableTokensZeroFunction setBurnableTokensZeroFunction)
        {
             return ContractHandler.SendRequestAsync(setBurnableTokensZeroFunction);
        }

        public Task<string> SetBurnableTokensZeroRequestAsync()
        {
             return ContractHandler.SendRequestAsync<SetBurnableTokensZeroFunction>();
        }

        public Task<TransactionReceipt> SetBurnableTokensZeroRequestAndWaitForReceiptAsync(SetBurnableTokensZeroFunction setBurnableTokensZeroFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBurnableTokensZeroFunction, cancellationToken);
        }

        public Task<TransactionReceipt> SetBurnableTokensZeroRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<SetBurnableTokensZeroFunction>(null, cancellationToken);
        }

        public Task<string> SetBusinessDevelopmentAddressRequestAsync(SetBusinessDevelopmentAddressFunction setBusinessDevelopmentAddressFunction)
        {
             return ContractHandler.SendRequestAsync(setBusinessDevelopmentAddressFunction);
        }

        public Task<TransactionReceipt> SetBusinessDevelopmentAddressRequestAndWaitForReceiptAsync(SetBusinessDevelopmentAddressFunction setBusinessDevelopmentAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBusinessDevelopmentAddressFunction, cancellationToken);
        }

        public Task<string> SetBusinessDevelopmentAddressRequestAsync(string newAddress)
        {
            var setBusinessDevelopmentAddressFunction = new SetBusinessDevelopmentAddressFunction();
                setBusinessDevelopmentAddressFunction.NewAddress = newAddress;
            
             return ContractHandler.SendRequestAsync(setBusinessDevelopmentAddressFunction);
        }

        public Task<TransactionReceipt> SetBusinessDevelopmentAddressRequestAndWaitForReceiptAsync(string newAddress, CancellationTokenSource cancellationToken = null)
        {
            var setBusinessDevelopmentAddressFunction = new SetBusinessDevelopmentAddressFunction();
                setBusinessDevelopmentAddressFunction.NewAddress = newAddress;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBusinessDevelopmentAddressFunction, cancellationToken);
        }

        public Task<string> SetBusinessDevelopmentDivisorRequestAsync(SetBusinessDevelopmentDivisorFunction setBusinessDevelopmentDivisorFunction)
        {
             return ContractHandler.SendRequestAsync(setBusinessDevelopmentDivisorFunction);
        }

        public Task<TransactionReceipt> SetBusinessDevelopmentDivisorRequestAndWaitForReceiptAsync(SetBusinessDevelopmentDivisorFunction setBusinessDevelopmentDivisorFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBusinessDevelopmentDivisorFunction, cancellationToken);
        }

        public Task<string> SetBusinessDevelopmentDivisorRequestAsync(BigInteger divisor)
        {
            var setBusinessDevelopmentDivisorFunction = new SetBusinessDevelopmentDivisorFunction();
                setBusinessDevelopmentDivisorFunction.Divisor = divisor;
            
             return ContractHandler.SendRequestAsync(setBusinessDevelopmentDivisorFunction);
        }

        public Task<TransactionReceipt> SetBusinessDevelopmentDivisorRequestAndWaitForReceiptAsync(BigInteger divisor, CancellationTokenSource cancellationToken = null)
        {
            var setBusinessDevelopmentDivisorFunction = new SetBusinessDevelopmentDivisorFunction();
                setBusinessDevelopmentDivisorFunction.Divisor = divisor;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBusinessDevelopmentDivisorFunction, cancellationToken);
        }

        public Task<string> SetBuyBackEnabledRequestAsync(SetBuyBackEnabledFunction setBuyBackEnabledFunction)
        {
             return ContractHandler.SendRequestAsync(setBuyBackEnabledFunction);
        }

        public Task<TransactionReceipt> SetBuyBackEnabledRequestAndWaitForReceiptAsync(SetBuyBackEnabledFunction setBuyBackEnabledFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuyBackEnabledFunction, cancellationToken);
        }

        public Task<string> SetBuyBackEnabledRequestAsync(bool enabled)
        {
            var setBuyBackEnabledFunction = new SetBuyBackEnabledFunction();
                setBuyBackEnabledFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAsync(setBuyBackEnabledFunction);
        }

        public Task<TransactionReceipt> SetBuyBackEnabledRequestAndWaitForReceiptAsync(bool enabled, CancellationTokenSource cancellationToken = null)
        {
            var setBuyBackEnabledFunction = new SetBuyBackEnabledFunction();
                setBuyBackEnabledFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuyBackEnabledFunction, cancellationToken);
        }

        public Task<string> SetBuybackMinAvailabilityRequestAsync(SetBuybackMinAvailabilityFunction setBuybackMinAvailabilityFunction)
        {
             return ContractHandler.SendRequestAsync(setBuybackMinAvailabilityFunction);
        }

        public Task<TransactionReceipt> SetBuybackMinAvailabilityRequestAndWaitForReceiptAsync(SetBuybackMinAvailabilityFunction setBuybackMinAvailabilityFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuybackMinAvailabilityFunction, cancellationToken);
        }

        public Task<string> SetBuybackMinAvailabilityRequestAsync(BigInteger amount, BigInteger numOfDecimals)
        {
            var setBuybackMinAvailabilityFunction = new SetBuybackMinAvailabilityFunction();
                setBuybackMinAvailabilityFunction.Amount = amount;
                setBuybackMinAvailabilityFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAsync(setBuybackMinAvailabilityFunction);
        }

        public Task<TransactionReceipt> SetBuybackMinAvailabilityRequestAndWaitForReceiptAsync(BigInteger amount, BigInteger numOfDecimals, CancellationTokenSource cancellationToken = null)
        {
            var setBuybackMinAvailabilityFunction = new SetBuybackMinAvailabilityFunction();
                setBuybackMinAvailabilityFunction.Amount = amount;
                setBuybackMinAvailabilityFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuybackMinAvailabilityFunction, cancellationToken);
        }

        public Task<string> SetBuybackTriggerTokenLimitRequestAsync(SetBuybackTriggerTokenLimitFunction setBuybackTriggerTokenLimitFunction)
        {
             return ContractHandler.SendRequestAsync(setBuybackTriggerTokenLimitFunction);
        }

        public Task<TransactionReceipt> SetBuybackTriggerTokenLimitRequestAndWaitForReceiptAsync(SetBuybackTriggerTokenLimitFunction setBuybackTriggerTokenLimitFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuybackTriggerTokenLimitFunction, cancellationToken);
        }

        public Task<string> SetBuybackTriggerTokenLimitRequestAsync(BigInteger buyBackTriggerLimit)
        {
            var setBuybackTriggerTokenLimitFunction = new SetBuybackTriggerTokenLimitFunction();
                setBuybackTriggerTokenLimitFunction.BuyBackTriggerLimit = buyBackTriggerLimit;
            
             return ContractHandler.SendRequestAsync(setBuybackTriggerTokenLimitFunction);
        }

        public Task<TransactionReceipt> SetBuybackTriggerTokenLimitRequestAndWaitForReceiptAsync(BigInteger buyBackTriggerLimit, CancellationTokenSource cancellationToken = null)
        {
            var setBuybackTriggerTokenLimitFunction = new SetBuybackTriggerTokenLimitFunction();
                setBuybackTriggerTokenLimitFunction.BuyBackTriggerLimit = buyBackTriggerLimit;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuybackTriggerTokenLimitFunction, cancellationToken);
        }

        public Task<string> SetBuybackUpperLimitRequestAsync(SetBuybackUpperLimitFunction setBuybackUpperLimitFunction)
        {
             return ContractHandler.SendRequestAsync(setBuybackUpperLimitFunction);
        }

        public Task<TransactionReceipt> SetBuybackUpperLimitRequestAndWaitForReceiptAsync(SetBuybackUpperLimitFunction setBuybackUpperLimitFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuybackUpperLimitFunction, cancellationToken);
        }

        public Task<string> SetBuybackUpperLimitRequestAsync(BigInteger buyBackLimit, BigInteger numOfDecimals)
        {
            var setBuybackUpperLimitFunction = new SetBuybackUpperLimitFunction();
                setBuybackUpperLimitFunction.BuyBackLimit = buyBackLimit;
                setBuybackUpperLimitFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAsync(setBuybackUpperLimitFunction);
        }

        public Task<TransactionReceipt> SetBuybackUpperLimitRequestAndWaitForReceiptAsync(BigInteger buyBackLimit, BigInteger numOfDecimals, CancellationTokenSource cancellationToken = null)
        {
            var setBuybackUpperLimitFunction = new SetBuybackUpperLimitFunction();
                setBuybackUpperLimitFunction.BuyBackLimit = buyBackLimit;
                setBuybackUpperLimitFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBuybackUpperLimitFunction, cancellationToken);
        }

        public Task<string> SetEverBridgeVaultAddressRequestAsync(SetEverBridgeVaultAddressFunction setEverBridgeVaultAddressFunction)
        {
             return ContractHandler.SendRequestAsync(setEverBridgeVaultAddressFunction);
        }

        public Task<TransactionReceipt> SetEverBridgeVaultAddressRequestAndWaitForReceiptAsync(SetEverBridgeVaultAddressFunction setEverBridgeVaultAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setEverBridgeVaultAddressFunction, cancellationToken);
        }

        public Task<string> SetEverBridgeVaultAddressRequestAsync(string contractAddress)
        {
            var setEverBridgeVaultAddressFunction = new SetEverBridgeVaultAddressFunction();
                setEverBridgeVaultAddressFunction.ContractAddress = contractAddress;
            
             return ContractHandler.SendRequestAsync(setEverBridgeVaultAddressFunction);
        }

        public Task<TransactionReceipt> SetEverBridgeVaultAddressRequestAndWaitForReceiptAsync(string contractAddress, CancellationTokenSource cancellationToken = null)
        {
            var setEverBridgeVaultAddressFunction = new SetEverBridgeVaultAddressFunction();
                setEverBridgeVaultAddressFunction.ContractAddress = contractAddress;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setEverBridgeVaultAddressFunction, cancellationToken);
        }

        public Task<string> SetLiquidityFeePercentRequestAsync(SetLiquidityFeePercentFunction setLiquidityFeePercentFunction)
        {
             return ContractHandler.SendRequestAsync(setLiquidityFeePercentFunction);
        }

        public Task<TransactionReceipt> SetLiquidityFeePercentRequestAndWaitForReceiptAsync(SetLiquidityFeePercentFunction setLiquidityFeePercentFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setLiquidityFeePercentFunction, cancellationToken);
        }

        public Task<string> SetLiquidityFeePercentRequestAsync(BigInteger liquidityFeeRate)
        {
            var setLiquidityFeePercentFunction = new SetLiquidityFeePercentFunction();
                setLiquidityFeePercentFunction.LiquidityFeeRate = liquidityFeeRate;
            
             return ContractHandler.SendRequestAsync(setLiquidityFeePercentFunction);
        }

        public Task<TransactionReceipt> SetLiquidityFeePercentRequestAndWaitForReceiptAsync(BigInteger liquidityFeeRate, CancellationTokenSource cancellationToken = null)
        {
            var setLiquidityFeePercentFunction = new SetLiquidityFeePercentFunction();
                setLiquidityFeePercentFunction.LiquidityFeeRate = liquidityFeeRate;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setLiquidityFeePercentFunction, cancellationToken);
        }

        public Task<string> SetLiquidityLockRequestAsync(SetLiquidityLockFunction setLiquidityLockFunction)
        {
             return ContractHandler.SendRequestAsync(setLiquidityLockFunction);
        }

        public Task<TransactionReceipt> SetLiquidityLockRequestAndWaitForReceiptAsync(SetLiquidityLockFunction setLiquidityLockFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setLiquidityLockFunction, cancellationToken);
        }

        public Task<string> SetLiquidityLockRequestAsync(bool enabled)
        {
            var setLiquidityLockFunction = new SetLiquidityLockFunction();
                setLiquidityLockFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAsync(setLiquidityLockFunction);
        }

        public Task<TransactionReceipt> SetLiquidityLockRequestAndWaitForReceiptAsync(bool enabled, CancellationTokenSource cancellationToken = null)
        {
            var setLiquidityLockFunction = new SetLiquidityLockFunction();
                setLiquidityLockFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setLiquidityLockFunction, cancellationToken);
        }

        public Task<string> SetMaxBuybackAmountUpdatedRequestAsync(SetMaxBuybackAmountUpdatedFunction setMaxBuybackAmountUpdatedFunction)
        {
             return ContractHandler.SendRequestAsync(setMaxBuybackAmountUpdatedFunction);
        }

        public Task<TransactionReceipt> SetMaxBuybackAmountUpdatedRequestAndWaitForReceiptAsync(SetMaxBuybackAmountUpdatedFunction setMaxBuybackAmountUpdatedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setMaxBuybackAmountUpdatedFunction, cancellationToken);
        }

        public Task<string> SetMaxBuybackAmountUpdatedRequestAsync(BigInteger maxAmount, BigInteger numOfDecimals)
        {
            var setMaxBuybackAmountUpdatedFunction = new SetMaxBuybackAmountUpdatedFunction();
                setMaxBuybackAmountUpdatedFunction.MaxAmount = maxAmount;
                setMaxBuybackAmountUpdatedFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAsync(setMaxBuybackAmountUpdatedFunction);
        }

        public Task<TransactionReceipt> SetMaxBuybackAmountUpdatedRequestAndWaitForReceiptAsync(BigInteger maxAmount, BigInteger numOfDecimals, CancellationTokenSource cancellationToken = null)
        {
            var setMaxBuybackAmountUpdatedFunction = new SetMaxBuybackAmountUpdatedFunction();
                setMaxBuybackAmountUpdatedFunction.MaxAmount = maxAmount;
                setMaxBuybackAmountUpdatedFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setMaxBuybackAmountUpdatedFunction, cancellationToken);
        }

        public Task<string> SetMinBuybackAmountRequestAsync(SetMinBuybackAmountFunction setMinBuybackAmountFunction)
        {
             return ContractHandler.SendRequestAsync(setMinBuybackAmountFunction);
        }

        public Task<TransactionReceipt> SetMinBuybackAmountRequestAndWaitForReceiptAsync(SetMinBuybackAmountFunction setMinBuybackAmountFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setMinBuybackAmountFunction, cancellationToken);
        }

        public Task<string> SetMinBuybackAmountRequestAsync(BigInteger minAmount, BigInteger numOfDecimals)
        {
            var setMinBuybackAmountFunction = new SetMinBuybackAmountFunction();
                setMinBuybackAmountFunction.MinAmount = minAmount;
                setMinBuybackAmountFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAsync(setMinBuybackAmountFunction);
        }

        public Task<TransactionReceipt> SetMinBuybackAmountRequestAndWaitForReceiptAsync(BigInteger minAmount, BigInteger numOfDecimals, CancellationTokenSource cancellationToken = null)
        {
            var setMinBuybackAmountFunction = new SetMinBuybackAmountFunction();
                setMinBuybackAmountFunction.MinAmount = minAmount;
                setMinBuybackAmountFunction.NumOfDecimals = numOfDecimals;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setMinBuybackAmountFunction, cancellationToken);
        }

        public Task<string> SetNumTokensSellToAddToLiquidityRequestAsync(SetNumTokensSellToAddToLiquidityFunction setNumTokensSellToAddToLiquidityFunction)
        {
             return ContractHandler.SendRequestAsync(setNumTokensSellToAddToLiquidityFunction);
        }

        public Task<TransactionReceipt> SetNumTokensSellToAddToLiquidityRequestAndWaitForReceiptAsync(SetNumTokensSellToAddToLiquidityFunction setNumTokensSellToAddToLiquidityFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setNumTokensSellToAddToLiquidityFunction, cancellationToken);
        }

        public Task<string> SetNumTokensSellToAddToLiquidityRequestAsync(BigInteger minimumTokensBeforeSwap)
        {
            var setNumTokensSellToAddToLiquidityFunction = new SetNumTokensSellToAddToLiquidityFunction();
                setNumTokensSellToAddToLiquidityFunction.MinimumTokensBeforeSwap = minimumTokensBeforeSwap;
            
             return ContractHandler.SendRequestAsync(setNumTokensSellToAddToLiquidityFunction);
        }

        public Task<TransactionReceipt> SetNumTokensSellToAddToLiquidityRequestAndWaitForReceiptAsync(BigInteger minimumTokensBeforeSwap, CancellationTokenSource cancellationToken = null)
        {
            var setNumTokensSellToAddToLiquidityFunction = new SetNumTokensSellToAddToLiquidityFunction();
                setNumTokensSellToAddToLiquidityFunction.MinimumTokensBeforeSwap = minimumTokensBeforeSwap;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setNumTokensSellToAddToLiquidityFunction, cancellationToken);
        }

        public Task<string> SetNumberOfBlocksForBuybackRequestAsync(SetNumberOfBlocksForBuybackFunction setNumberOfBlocksForBuybackFunction)
        {
             return ContractHandler.SendRequestAsync(setNumberOfBlocksForBuybackFunction);
        }

        public Task<TransactionReceipt> SetNumberOfBlocksForBuybackRequestAndWaitForReceiptAsync(SetNumberOfBlocksForBuybackFunction setNumberOfBlocksForBuybackFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setNumberOfBlocksForBuybackFunction, cancellationToken);
        }

        public Task<string> SetNumberOfBlocksForBuybackRequestAsync(BigInteger value)
        {
            var setNumberOfBlocksForBuybackFunction = new SetNumberOfBlocksForBuybackFunction();
                setNumberOfBlocksForBuybackFunction.Value = value;
            
             return ContractHandler.SendRequestAsync(setNumberOfBlocksForBuybackFunction);
        }

        public Task<TransactionReceipt> SetNumberOfBlocksForBuybackRequestAndWaitForReceiptAsync(BigInteger value, CancellationTokenSource cancellationToken = null)
        {
            var setNumberOfBlocksForBuybackFunction = new SetNumberOfBlocksForBuybackFunction();
                setNumberOfBlocksForBuybackFunction.Value = value;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setNumberOfBlocksForBuybackFunction, cancellationToken);
        }

        public Task<string> SetOffchainBalanceExcludedRequestAsync(SetOffchainBalanceExcludedFunction setOffchainBalanceExcludedFunction)
        {
             return ContractHandler.SendRequestAsync(setOffchainBalanceExcludedFunction);
        }

        public Task<TransactionReceipt> SetOffchainBalanceExcludedRequestAndWaitForReceiptAsync(SetOffchainBalanceExcludedFunction setOffchainBalanceExcludedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOffchainBalanceExcludedFunction, cancellationToken);
        }

        public Task<string> SetOffchainBalanceExcludedRequestAsync(bool enabled)
        {
            var setOffchainBalanceExcludedFunction = new SetOffchainBalanceExcludedFunction();
                setOffchainBalanceExcludedFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAsync(setOffchainBalanceExcludedFunction);
        }

        public Task<TransactionReceipt> SetOffchainBalanceExcludedRequestAndWaitForReceiptAsync(bool enabled, CancellationTokenSource cancellationToken = null)
        {
            var setOffchainBalanceExcludedFunction = new SetOffchainBalanceExcludedFunction();
                setOffchainBalanceExcludedFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOffchainBalanceExcludedFunction, cancellationToken);
        }

        public Task<string> SetRouterAddressRequestAsync(SetRouterAddressFunction setRouterAddressFunction)
        {
             return ContractHandler.SendRequestAsync(setRouterAddressFunction);
        }

        public Task<TransactionReceipt> SetRouterAddressRequestAndWaitForReceiptAsync(SetRouterAddressFunction setRouterAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setRouterAddressFunction, cancellationToken);
        }

        public Task<string> SetRouterAddressRequestAsync(string newAddress)
        {
            var setRouterAddressFunction = new SetRouterAddressFunction();
                setRouterAddressFunction.NewAddress = newAddress;
            
             return ContractHandler.SendRequestAsync(setRouterAddressFunction);
        }

        public Task<TransactionReceipt> SetRouterAddressRequestAndWaitForReceiptAsync(string newAddress, CancellationTokenSource cancellationToken = null)
        {
            var setRouterAddressFunction = new SetRouterAddressFunction();
                setRouterAddressFunction.NewAddress = newAddress;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setRouterAddressFunction, cancellationToken);
        }

        public Task<string> SetStakingAddressRequestAsync(SetStakingAddressFunction setStakingAddressFunction)
        {
             return ContractHandler.SendRequestAsync(setStakingAddressFunction);
        }

        public Task<TransactionReceipt> SetStakingAddressRequestAndWaitForReceiptAsync(SetStakingAddressFunction setStakingAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setStakingAddressFunction, cancellationToken);
        }

        public Task<string> SetStakingAddressRequestAsync(string contractAddress)
        {
            var setStakingAddressFunction = new SetStakingAddressFunction();
                setStakingAddressFunction.ContractAddress = contractAddress;
            
             return ContractHandler.SendRequestAsync(setStakingAddressFunction);
        }

        public Task<TransactionReceipt> SetStakingAddressRequestAndWaitForReceiptAsync(string contractAddress, CancellationTokenSource cancellationToken = null)
        {
            var setStakingAddressFunction = new SetStakingAddressFunction();
                setStakingAddressFunction.ContractAddress = contractAddress;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setStakingAddressFunction, cancellationToken);
        }

        public Task<string> SetSwapEnabledRequestAsync(SetSwapEnabledFunction setSwapEnabledFunction)
        {
             return ContractHandler.SendRequestAsync(setSwapEnabledFunction);
        }

        public Task<TransactionReceipt> SetSwapEnabledRequestAndWaitForReceiptAsync(SetSwapEnabledFunction setSwapEnabledFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setSwapEnabledFunction, cancellationToken);
        }

        public Task<string> SetSwapEnabledRequestAsync(bool enabled)
        {
            var setSwapEnabledFunction = new SetSwapEnabledFunction();
                setSwapEnabledFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAsync(setSwapEnabledFunction);
        }

        public Task<TransactionReceipt> SetSwapEnabledRequestAndWaitForReceiptAsync(bool enabled, CancellationTokenSource cancellationToken = null)
        {
            var setSwapEnabledFunction = new SetSwapEnabledFunction();
                setSwapEnabledFunction.Enabled = enabled;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setSwapEnabledFunction, cancellationToken);
        }

        public Task<string> SetTransactionCapRequestAsync(SetTransactionCapFunction setTransactionCapFunction)
        {
             return ContractHandler.SendRequestAsync(setTransactionCapFunction);
        }

        public Task<TransactionReceipt> SetTransactionCapRequestAndWaitForReceiptAsync(SetTransactionCapFunction setTransactionCapFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setTransactionCapFunction, cancellationToken);
        }

        public Task<string> SetTransactionCapRequestAsync(BigInteger txAmount)
        {
            var setTransactionCapFunction = new SetTransactionCapFunction();
                setTransactionCapFunction.TxAmount = txAmount;
            
             return ContractHandler.SendRequestAsync(setTransactionCapFunction);
        }

        public Task<TransactionReceipt> SetTransactionCapRequestAndWaitForReceiptAsync(BigInteger txAmount, CancellationTokenSource cancellationToken = null)
        {
            var setTransactionCapFunction = new SetTransactionCapFunction();
                setTransactionCapFunction.TxAmount = txAmount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setTransactionCapFunction, cancellationToken);
        }

        public Task<string> StakeTokenQueryAsync(StakeTokenFunction stakeTokenFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<StakeTokenFunction, string>(stakeTokenFunction, blockParameter);
        }

        
        public Task<string> StakeTokenQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<StakeTokenFunction, string>(null, blockParameter);
        }

        public Task<bool> SwapEnabledQueryAsync(SwapEnabledFunction swapEnabledFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SwapEnabledFunction, bool>(swapEnabledFunction, blockParameter);
        }

        
        public Task<bool> SwapEnabledQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SwapEnabledFunction, bool>(null, blockParameter);
        }

        public Task<string> SymbolQueryAsync(SymbolFunction symbolFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(symbolFunction, blockParameter);
        }

        
        public Task<string> SymbolQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> TotalBuyVolumeQueryAsync(TotalBuyVolumeFunction totalBuyVolumeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalBuyVolumeFunction, BigInteger>(totalBuyVolumeFunction, blockParameter);
        }

        
        public Task<BigInteger> TotalBuyVolumeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalBuyVolumeFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> TotalSellVolumeQueryAsync(TotalSellVolumeFunction totalSellVolumeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSellVolumeFunction, BigInteger>(totalSellVolumeFunction, blockParameter);
        }

        
        public Task<BigInteger> TotalSellVolumeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSellVolumeFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> TotalSupplyQueryAsync(TotalSupplyFunction totalSupplyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(totalSupplyFunction, blockParameter);
        }

        
        public Task<BigInteger> TotalSupplyQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> TransactionCapQueryAsync(TransactionCapFunction transactionCapFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransactionCapFunction, BigInteger>(transactionCapFunction, blockParameter);
        }

        
        public Task<BigInteger> TransactionCapQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransactionCapFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> TransferRequestAsync(TransferFunction transferFunction)
        {
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(TransferFunction transferFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferRequestAsync(string recipient, BigInteger amount)
        {
            var transferFunction = new TransferFunction();
                transferFunction.Recipient = recipient;
                transferFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(string recipient, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var transferFunction = new TransferFunction();
                transferFunction.Recipient = recipient;
                transferFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferBalanceRequestAsync(TransferBalanceFunction transferBalanceFunction)
        {
             return ContractHandler.SendRequestAsync(transferBalanceFunction);
        }

        public Task<TransactionReceipt> TransferBalanceRequestAndWaitForReceiptAsync(TransferBalanceFunction transferBalanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferBalanceFunction, cancellationToken);
        }

        public Task<string> TransferBalanceRequestAsync(BigInteger amount)
        {
            var transferBalanceFunction = new TransferBalanceFunction();
                transferBalanceFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(transferBalanceFunction);
        }

        public Task<TransactionReceipt> TransferBalanceRequestAndWaitForReceiptAsync(BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var transferBalanceFunction = new TransferBalanceFunction();
                transferBalanceFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferBalanceFunction, cancellationToken);
        }

        public Task<string> TransferExternalTokensRequestAsync(TransferExternalTokensFunction transferExternalTokensFunction)
        {
             return ContractHandler.SendRequestAsync(transferExternalTokensFunction);
        }

        public Task<TransactionReceipt> TransferExternalTokensRequestAndWaitForReceiptAsync(TransferExternalTokensFunction transferExternalTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferExternalTokensFunction, cancellationToken);
        }

        public Task<string> TransferExternalTokensRequestAsync(string tokenAddress, string to, BigInteger amount)
        {
            var transferExternalTokensFunction = new TransferExternalTokensFunction();
                transferExternalTokensFunction.TokenAddress = tokenAddress;
                transferExternalTokensFunction.To = to;
                transferExternalTokensFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(transferExternalTokensFunction);
        }

        public Task<TransactionReceipt> TransferExternalTokensRequestAndWaitForReceiptAsync(string tokenAddress, string to, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var transferExternalTokensFunction = new TransferExternalTokensFunction();
                transferExternalTokensFunction.TokenAddress = tokenAddress;
                transferExternalTokensFunction.To = to;
                transferExternalTokensFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferExternalTokensFunction, cancellationToken);
        }

        public Task<string> TransferFromRequestAsync(TransferFromFunction transferFromFunction)
        {
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(TransferFromFunction transferFromFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<string> TransferFromRequestAsync(string sender, string recipient, BigInteger amount)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.Sender = sender;
                transferFromFunction.Recipient = recipient;
                transferFromFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(string sender, string recipient, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.Sender = sender;
                transferFromFunction.Recipient = recipient;
                transferFromFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<string> TransferFromWithPermitRequestAsync(TransferFromWithPermitFunction transferFromWithPermitFunction)
        {
             return ContractHandler.SendRequestAsync(transferFromWithPermitFunction);
        }

        public Task<TransactionReceipt> TransferFromWithPermitRequestAndWaitForReceiptAsync(TransferFromWithPermitFunction transferFromWithPermitFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromWithPermitFunction, cancellationToken);
        }

        public Task<string> TransferFromWithPermitRequestAsync(string sender, string recipient, BigInteger amount, BigInteger deadline, byte v, byte[] r, byte[] s)
        {
            var transferFromWithPermitFunction = new TransferFromWithPermitFunction();
                transferFromWithPermitFunction.Sender = sender;
                transferFromWithPermitFunction.Recipient = recipient;
                transferFromWithPermitFunction.Amount = amount;
                transferFromWithPermitFunction.Deadline = deadline;
                transferFromWithPermitFunction.V = v;
                transferFromWithPermitFunction.R = r;
                transferFromWithPermitFunction.S = s;
            
             return ContractHandler.SendRequestAsync(transferFromWithPermitFunction);
        }

        public Task<TransactionReceipt> TransferFromWithPermitRequestAndWaitForReceiptAsync(string sender, string recipient, BigInteger amount, BigInteger deadline, byte v, byte[] r, byte[] s, CancellationTokenSource cancellationToken = null)
        {
            var transferFromWithPermitFunction = new TransferFromWithPermitFunction();
                transferFromWithPermitFunction.Sender = sender;
                transferFromWithPermitFunction.Recipient = recipient;
                transferFromWithPermitFunction.Amount = amount;
                transferFromWithPermitFunction.Deadline = deadline;
                transferFromWithPermitFunction.V = v;
                transferFromWithPermitFunction.R = r;
                transferFromWithPermitFunction.S = s;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromWithPermitFunction, cancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction transferOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction transferOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(string newOwner)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> TransferStakeRequestAsync(TransferStakeFunction transferStakeFunction)
        {
             return ContractHandler.SendRequestAsync(transferStakeFunction);
        }

        public Task<TransactionReceipt> TransferStakeRequestAndWaitForReceiptAsync(TransferStakeFunction transferStakeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferStakeFunction, cancellationToken);
        }

        public Task<string> TransferStakeRequestAsync(string fromAddress, string toAddress, BigInteger amountToTransfer)
        {
            var transferStakeFunction = new TransferStakeFunction();
                transferStakeFunction.FromAddress = fromAddress;
                transferStakeFunction.ToAddress = toAddress;
                transferStakeFunction.AmountToTransfer = amountToTransfer;
            
             return ContractHandler.SendRequestAsync(transferStakeFunction);
        }

        public Task<TransactionReceipt> TransferStakeRequestAndWaitForReceiptAsync(string fromAddress, string toAddress, BigInteger amountToTransfer, CancellationTokenSource cancellationToken = null)
        {
            var transferStakeFunction = new TransferStakeFunction();
                transferStakeFunction.FromAddress = fromAddress;
                transferStakeFunction.ToAddress = toAddress;
                transferStakeFunction.AmountToTransfer = amountToTransfer;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferStakeFunction, cancellationToken);
        }

        public Task<string> UniswapV2PairQueryAsync(UniswapV2PairFunction uniswapV2PairFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UniswapV2PairFunction, string>(uniswapV2PairFunction, blockParameter);
        }

        
        public Task<string> UniswapV2PairQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UniswapV2PairFunction, string>(null, blockParameter);
        }

        public Task<string> UniswapV2RouterQueryAsync(UniswapV2RouterFunction uniswapV2RouterFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UniswapV2RouterFunction, string>(uniswapV2RouterFunction, blockParameter);
        }

        
        public Task<string> UniswapV2RouterQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UniswapV2RouterFunction, string>(null, blockParameter);
        }

        public Task<string> UnlockTokensAndNftsRequestAsync(UnlockTokensAndNftsFunction unlockTokensAndNftsFunction)
        {
             return ContractHandler.SendRequestAsync(unlockTokensAndNftsFunction);
        }

        public Task<TransactionReceipt> UnlockTokensAndNftsRequestAndWaitForReceiptAsync(UnlockTokensAndNftsFunction unlockTokensAndNftsFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(unlockTokensAndNftsFunction, cancellationToken);
        }

        public Task<string> UnlockTokensAndNftsRequestAsync(string actualAccount)
        {
            var unlockTokensAndNftsFunction = new UnlockTokensAndNftsFunction();
                unlockTokensAndNftsFunction.ActualAccount = actualAccount;
            
             return ContractHandler.SendRequestAsync(unlockTokensAndNftsFunction);
        }

        public Task<TransactionReceipt> UnlockTokensAndNftsRequestAndWaitForReceiptAsync(string actualAccount, CancellationTokenSource cancellationToken = null)
        {
            var unlockTokensAndNftsFunction = new UnlockTokensAndNftsFunction();
                unlockTokensAndNftsFunction.ActualAccount = actualAccount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(unlockTokensAndNftsFunction, cancellationToken);
        }

        public Task<string> UpgradeCompleteRequestAsync(UpgradeCompleteFunction upgradeCompleteFunction)
        {
             return ContractHandler.SendRequestAsync(upgradeCompleteFunction);
        }

        public Task<string> UpgradeCompleteRequestAsync()
        {
             return ContractHandler.SendRequestAsync<UpgradeCompleteFunction>();
        }

        public Task<TransactionReceipt> UpgradeCompleteRequestAndWaitForReceiptAsync(UpgradeCompleteFunction upgradeCompleteFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(upgradeCompleteFunction, cancellationToken);
        }

        public Task<TransactionReceipt> UpgradeCompleteRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<UpgradeCompleteFunction>(null, cancellationToken);
        }

        public Task<string> WithdrawRequestAsync(WithdrawFunction withdrawFunction)
        {
             return ContractHandler.SendRequestAsync(withdrawFunction);
        }

        public Task<TransactionReceipt> WithdrawRequestAndWaitForReceiptAsync(WithdrawFunction withdrawFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction, cancellationToken);
        }

        public Task<string> WithdrawRequestAsync(BigInteger nftId, BigInteger amount, bool overrideNotClaimed)
        {
            var withdrawFunction = new WithdrawFunction();
                withdrawFunction.NftId = nftId;
                withdrawFunction.Amount = amount;
                withdrawFunction.OverrideNotClaimed = overrideNotClaimed;
            
             return ContractHandler.SendRequestAsync(withdrawFunction);
        }

        public Task<TransactionReceipt> WithdrawRequestAndWaitForReceiptAsync(BigInteger nftId, BigInteger amount, bool overrideNotClaimed, CancellationTokenSource cancellationToken = null)
        {
            var withdrawFunction = new WithdrawFunction();
                withdrawFunction.NftId = nftId;
                withdrawFunction.Amount = amount;
                withdrawFunction.OverrideNotClaimed = overrideNotClaimed;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction, cancellationToken);
        }
    }
}
