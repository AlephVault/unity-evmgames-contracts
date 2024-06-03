using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC1155.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types
    {
        using Events;
        
        /// <summary>
        ///   An interface to the ERC1155 contract.
        /// </summary>
        public class ERC1155Contract : BaseContract
        {
            public ERC1155Contract(Web3 web3, string address, ITransactionGasSetter transactionGasSetter = null) : base(web3, address, transactionGasSetter)
            {
            }

            public Task<BigInteger> BalanceOf(string account, BigInteger id, BlockParameter blockParameter = null)
            {
                return Call<BalanceOfFunction, BigInteger>(new BalanceOfFunction()
                {
                    Account = account,
                    Id = id,
                }, blockParameter);
            }

            public Task<BigInteger[]> BalanceOfBatch(string[] accounts, BigInteger[] ids, BlockParameter blockParameter = null)
            {
                return Call<BalanceOfBatchFunction, BigInteger[]>(new BalanceOfBatchFunction()
                {
                    Accounts = accounts.ToList(),
                    Ids = ids.ToList(),
                }, blockParameter);
            }

            public Task<bool> IsApprovedForAll(string account, string _operator, BlockParameter blockParameter = null)
            {
                return Call<IsApprovedForAllFunction, bool>(new IsApprovedForAllFunction
                {
                    Account = account,
                    Operator = _operator,
                }, blockParameter);
            }

            public Task<TransactionReceipt> SafeBatchTransferFrom(string from, string to, BigInteger[] ids, BigInteger[] values, byte[] data)
            {
                return Send(new SafeBatchTransferFromFunction()
                {
                    From = from,
                    To = to,
                    Ids = ids.ToList(),
                    Amounts = values.ToList(),
                    Data = data,
                });
            }

            public Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger id, BigInteger value, byte[] data)
            {
                return Send(new SafeTransferFromFunction
                {
                    From = from,
                    To = to,
                    Id = id,
                    Amount = value,
                    Data = data,
                });
            }

            public Task<TransactionReceipt> SetApprovalForAll(string _operator, bool approved)
            {
                return Send(new SetApprovalForAllFunction
                {
                    Operator = _operator,
                    Approved = approved,
                });
            }

            public Task<bool> SupportsInterface(byte[] interfaceId, BlockParameter blockParameter = null)
            {
                return Call<SupportsInterfaceFunction, bool>(new SupportsInterfaceFunction
                {
                    InterfaceId = interfaceId,
                }, blockParameter);
            }

            public Task<string> Uri(BigInteger arg0, BlockParameter blockParameter = null)
            {
                return Call<UriFunction, string>(new UriFunction
                {
                    TokenId = arg0,
                }, blockParameter);
            }

            public EventsWorker<ApprovalForAllEventDTO> MakeApprovalForAllEventsWorker(Func<Event<ApprovalForAllEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }

            public EventsWorker<TransferBatchEventDTO> MakeTransferBatchEventsWorker(Func<Event<TransferBatchEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }

            public EventsWorker<TransferSingleEventDTO> MakeTransferSingleEventsWorker(Func<Event<TransferSingleEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }

            public EventsWorker<URIEventDTO> MakeURIEventsWorker(Func<Event<URIEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }
        }
        
    }
}