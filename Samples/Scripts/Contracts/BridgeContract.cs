using System;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Contracts.Types;
using AlephVault.Unity.EVMGames.Contracts.Types.Events;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    using BridgeContractComponents.Functions;
    using BridgeContractComponents.Events;
    
    /// <summary>
    ///   An interface to the Bridge contract.
    /// </summary>
    public class BridgeContract : BaseContract
    {
        // Feel free to change the address according to what's
        // the value when testing a sample blockchain from the
        // https://github.com/AlephVault/solidity-bridge-contract
        // repository.
        private const string ADDRESS = "0x49B176fec7d8BB8324E71F1F679f8dcC297Cb3E5";

        // This constructor was slightly modified.
        public BridgeContract(Web3 web3, ITransactionGasSetter transactionGasSetter = null)
            : base(web3, new AddressUtil().ConvertToChecksumAddress(ADDRESS), transactionGasSetter)
        {
        }

        public Task<BigInteger> ACTIVE(BlockParameter blockParameter = null)
        {
            return Call<ACTIVEFunction, BigInteger>(new ACTIVEFunction
            {
            }, blockParameter);
        }

        public Task<BigInteger> CREATED(BlockParameter blockParameter = null)
        {
            return Call<CREATEDFunction, BigInteger>(new CREATEDFunction
            {
            }, blockParameter);
        }

        public Task<byte[]> IERC1155OK(BlockParameter blockParameter = null)
        {
            return Call<IERC1155OKFunction, byte[]>(new IERC1155OKFunction
            {
            }, blockParameter);
        }

        public Task<byte[]> PARCELNONE(BlockParameter blockParameter = null)
        {
            return Call<PARCELNONEFunction, byte[]>(new PARCELNONEFunction
            {
            }, blockParameter);
        }

        public Task<BridgedResourceTypesOutput> BridgedResourceTypes(BigInteger arg0, BlockParameter blockParameter = null)
        {
            return CallMulti<BridgedResourceTypesFunction, BridgedResourceTypesOutput>(new BridgedResourceTypesFunction
            {
                Arg0 = arg0,
            }, blockParameter);
        }

        public async Task<TokensContract> Economy(Web3 web3, ITransactionGasSetter transactionGasSetter, BlockParameter blockParameter = null)
        {
            string address = await Call<EconomyFunction, string>(new EconomyFunction{}, blockParameter);
            return new TokensContract(web3, address, transactionGasSetter);
        }
        
        // This method is new.
        public Task<TokensContract> Economy(BlockParameter blockParameter = null)
        {
            return Economy(Web3, TransactionGasSetter, blockParameter);
        }

        public Task<string> Owner(BlockParameter blockParameter = null)
        {
            return Call<OwnerFunction, string>(new OwnerFunction
            {
            }, blockParameter);
        }

        public Task<ParcelsOutput> Parcels(byte[] arg0, BlockParameter blockParameter = null)
        {
            return CallMulti<ParcelsFunction, ParcelsOutput>(new ParcelsFunction
            {
                Arg0 = arg0,
            }, blockParameter);
        }

        public Task<TransactionReceipt> RenounceOwnership()
        {
            return Send(new RenounceOwnershipFunction
            {
            });
        }

        public Task<bool> Terminated(BlockParameter blockParameter = null)
        {
            return Call<TerminatedFunction, bool>(new TerminatedFunction
            {
            }, blockParameter);
        }

        public Task<TransactionReceipt> TransferOwnership(string newOwner)
        {
            return Send(new TransferOwnershipFunction
            {
                NewOwner = newOwner,
            });
        }

        public Task<TransactionReceipt> SendUnits(string to, BigInteger id, BigInteger units)
        {
            return Send(new SendUnitsFunction
            {
                To = to,
                Id = id,
                Units = units,
            });
        }

        public Task<TransactionReceipt> SendTokens(string to, BigInteger id, BigInteger value, byte[] data)
        {
            return Send(new SendTokensFunction
            {
                To = to,
                Id = id,
                Value = value,
                Data = data,
            });
        }

        public Task<TransactionReceipt> DefineBridgedResourceType(BigInteger id, BigInteger amountPerUnit)
        {
            return Send(new DefineBridgedResourceTypeFunction
            {
                Id = id,
                AmountPerUnit = amountPerUnit,
            });
        }

        public Task<TransactionReceipt> RemoveBridgedResourceType(BigInteger id)
        {
            return Send(new RemoveBridgedResourceTypeFunction
            {
                Id = id,
            });
        }

        public Task<TransactionReceipt> Terminate()
        {
            return Send(new TerminateFunction
            {
            });
        }

        public Task<TransactionReceipt> OnERC1155Received(string arg0, string from, BigInteger id, BigInteger value, byte[] data)
        {
            return Send(new OnERC1155ReceivedFunction
            {
                Arg0 = arg0,
                From = from,
                Id = id,
                Value = value,
                Data = data,
            });
        }

        public Task<TransactionReceipt> OnERC1155BatchReceived(string arg0, string arg1, BigInteger[] arg2, BigInteger[] arg3, byte[] arg4)
        {
            return Send(new OnERC1155BatchReceivedFunction
            {
                Arg0 = arg0,
                Arg1 = arg1,
                Arg2 = arg2,
                Arg3 = arg3,
                Arg4 = arg4,
            });
        }

        public Task<bool> SupportsInterface(byte[] interfaceId, BlockParameter blockParameter = null)
        {
            return Call<SupportsInterfaceFunction, bool>(new SupportsInterfaceFunction
            {
                InterfaceId = interfaceId,
            }, blockParameter);
        }

        public EventsWorker<BridgedResourceTypeDefinedEventDTO> MakeBridgedResourceTypeDefinedEventsWorker(Func<Event<BridgedResourceTypeDefinedEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
        {
            return MakeEventsWorker(filterMaker, fromBlock);
        }

        public EventsWorker<BridgedResourceTypeRemovedEventDTO> MakeBridgedResourceTypeRemovedEventsWorker(Func<Event<BridgedResourceTypeRemovedEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
        {
            return MakeEventsWorker(filterMaker, fromBlock);
        }

        public EventsWorker<OwnershipTransferredEventDTO> MakeOwnershipTransferredEventsWorker(Func<Event<OwnershipTransferredEventDTO>, BlockParameter, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
        {
            return MakeEventsWorker(filterMaker, fromBlock);
        }
    }
}
