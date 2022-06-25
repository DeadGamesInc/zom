using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class NFT_ERC721 {
    public static async Task<int> BalanceOf(string contract) {
        var config = Web3Config.Get();
        var args = JsonConvert.SerializeObject(new[] { PlayerPrefs.GetString("Account") });
        var response = await EVM.Call(config.Blockchain, config.Network, contract, ABIs.ERC721, "balanceOf", args, config.BlockchainNode);
        int.TryParse(response, out var count);
        return count;
    }

    public static async Task<string> MintReward(string contract) {
        var config = Web3Config.Get();
        var args = JsonConvert.SerializeObject(new[] { PlayerPrefs.GetString("Account") });
        var data = await EVM.CreateContractData(ABIs.ERC721, "mint", args);
        return await Web3Wallet.SendTransaction(config.ChainID, contract, "0", data);
    }
}
