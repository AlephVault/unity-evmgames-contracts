using System;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Contracts.Types;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    using TokensContractComponents.Functions;
    using TokensContractComponents.Events;
    
    /// <summary>
    ///   An interface to the Tokens contract.
    /// </summary>
    public class TokensContract : BaseContract
    {
        public TokensContract(Web3 web3, string address, ITransactionGasSetter transactionGasSetter = null) : base(web3, address, transactionGasSetter)
        {
        }

        public Task<BigInteger> AMOUNT(BlockParameter blockParameter = null)
        {
            return Call<AMOUNTMessage, BigInteger>(new AMOUNTMessage
            {
            }, blockParameter);
        }

        public Task<BigInteger> BalanceOf(string account, BigInteger id, BlockParameter blockParameter = null)
        {
            return Call<BalanceOfMessage, BigInteger>(new BalanceOfMessage
            {
                Account = account,
                Id = id,
            }, blockParameter);
        }

        public Task<BigInteger[]> BalanceOfBatch(string[] accounts, BigInteger[] ids, BlockParameter blockParameter = null)
        {
            return Call<BalanceOfBatchMessage, BigInteger[]>(new BalanceOfBatchMessage
            {
                Accounts = accounts,
                Ids = ids,
            }, blockParameter);
        }

        public Task<bool> IsApprovedForAll(string account, string _operator, BlockParameter blockParameter = null)
        {
            return Call<IsApprovedForAllMessage, bool>(new IsApprovedForAllMessage
            {
                Account = account,
                Operator = _operator,
            }, blockParameter);
        }

        public Task<TransactionReceipt> SafeBatchTransferFrom(string from, string to, BigInteger[] ids, BigInteger[] values, byte[] data)
        {
            return Send(new SafeBatchTransferFromMessage
            {
                From = from,
                To = to,
                Ids = ids,
                Values = values,
                Data = data,
            });
        }

        public Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger id, BigInteger value, byte[] data)
        {
            return Send(new SafeTransferFromMessage
            {
                From = from,
                To = to,
                Id = id,
                Value = value,
                Data = data,
            });
        }

        public Task<TransactionReceipt> SetApprovalForAll(string _operator, bool approved)
        {
            return Send(new SetApprovalForAllMessage
            {
                Operator = _operator,
                Approved = approved,
            });
        }

        public Task<bool> SupportsInterface(byte[] interfaceId, BlockParameter blockParameter = null)
        {
            return Call<SupportsInterfaceMessage, bool>(new SupportsInterfaceMessage
            {
                InterfaceId = interfaceId,
            }, blockParameter);
        }

        public Task<string> Uri(BigInteger arg0, BlockParameter blockParameter = null)
        {
            return Call<UriMessage, string>(new UriMessage
            {
                Arg0 = arg0,
            }, blockParameter);
        }

        public Task<TransactionReceipt> Mint(string to, BigInteger id)
        {
            return Send(new MintMessage
            {
                To = to,
                Id = id,
            });
        }

        public EventsWorker<ApprovalForAllEvent> MakeApprovalForAllEventsWorker(Func<Event<ApprovalForAllEvent>, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
        {
            return MakeEventsWorker(filterMaker, fromBlock);
        }

        public EventsWorker<TransferBatchEvent> MakeTransferBatchEventsWorker(Func<Event<TransferBatchEvent>, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
        {
            return MakeEventsWorker(filterMaker, fromBlock);
        }

        public EventsWorker<TransferSingleEvent> MakeTransferSingleEventsWorker(Func<Event<TransferSingleEvent>, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
        {
            return MakeEventsWorker(filterMaker, fromBlock);
        }
        
        public EventsWorker<URIEvent> MakeURIEventsWorker(Func<Event<URIEvent>, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
        {
            return MakeEventsWorker(filterMaker, fromBlock);
        }
    }
}
