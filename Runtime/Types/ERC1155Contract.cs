using AlephVault.Unity.EVMGames.Contracts.Types;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts
{
    /// <summary>
    ///   An interface to an ERC1155 contract.
    /// </summary>
    public class ERC1155Contract : BaseContract
    {
        public ERC1155Contract(Web3 web3, string address, ITransactionGasSetter transactionGasSetter = null) : base(web3, address, transactionGasSetter)
        {
        }
        
        // TODO implement this all.
    }
}