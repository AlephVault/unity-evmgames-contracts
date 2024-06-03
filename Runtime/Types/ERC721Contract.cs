using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC721.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types
    {
        using Events;
        
        /// <summary>
        ///   An interface to the ERC721 contract.
        /// </summary>
        public class ERC721Contract : BaseContract
        {
            public ERC721Contract(Web3 web3, string address, ITransactionGasSetter transactionGasSetter = null) : base(web3, address, transactionGasSetter)
            {
            }

            public Task<TransactionReceipt> Approve(string to, BigInteger tokenId)
            {
                return Send(new ApproveFunction()
                {
                    To = to,
                    TokenId = tokenId,
                });
            }

            public Task<BigInteger> BalanceOf(string owner, BlockParameter blockParameter = null)
            {
                return Call<BalanceOfFunction, BigInteger>(new BalanceOfFunction
                {
                    Owner = owner,
                }, blockParameter);
            }

            public Task<string> GetApproved(BigInteger tokenId, BlockParameter blockParameter = null)
            {
                return Call<GetApprovedFunction, string>(new GetApprovedFunction
                {
                    TokenId = tokenId,
                }, blockParameter);
            }

            public Task<bool> IsApprovedForAll(string owner, string _operator, BlockParameter blockParameter = null)
            {
                return Call<IsApprovedForAllFunction, bool>(new IsApprovedForAllFunction
                {
                    Owner = owner,
                    Operator = _operator,
                }, blockParameter);
            }

            public Task<string> Name(BlockParameter blockParameter = null)
            {
                return Call<NameFunction, string>(new NameFunction
                {
                }, blockParameter);
            }

            public Task<string> OwnerOf(BigInteger tokenId, BlockParameter blockParameter = null)
            {
                return Call<OwnerOfFunction, string>(new OwnerOfFunction
                {
                    TokenId = tokenId,
                }, blockParameter);
            }

            public Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger tokenId, byte[] data)
            {
                return Send(new SafeTransferFrom1Function
                {
                    From = from,
                    To = to,
                    TokenId = tokenId,
                    Data = data
                });
            }

            public Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger tokenId)
            {
                return Send(new SafeTransferFromFunction()
                {
                    From = from,
                    To = to,
                    TokenId = tokenId,
                });
            }

            public Task<TransactionReceipt> SetApprovalForAll(string _operator, bool approved)
            {
                return Send(new SetApprovalForAllFunction()
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

            public Task<string> Symbol(BlockParameter blockParameter = null)
            {
                return Call<SymbolFunction, string>(new SymbolFunction
                {
                }, blockParameter);
            }

            public Task<string> TokenURI(BigInteger tokenId, BlockParameter blockParameter = null)
            {
                return Call<TokenURIFunction, string>(new TokenURIFunction
                {
                    TokenId = tokenId,
                }, blockParameter);
            }

            public Task<TransactionReceipt> TransferFrom(string from, string to, BigInteger tokenId)
            {
                return Send(new TransferFromFunction
                {
                    From = from,
                    To = to,
                    TokenId = tokenId,
                });
            }

            public EventsWorker<ApprovalEventDTO> MakeApprovalEventsWorker(Func<Event<ApprovalEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }

            public EventsWorker<ApprovalForAllEventDTO> MakeApprovalForAllEventsWorker(Func<Event<ApprovalForAllEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }

            public EventsWorker<TransferEventDTO> MakeTransferEventsWorker(Func<Event<TransferEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }
        }
    }
}
