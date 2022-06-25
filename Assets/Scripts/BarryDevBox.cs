using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarryDevBox : MonoBehaviour {
    public async void Balance() {
        print(await NFT_ERC721.BalanceOf("0xD48ab8a75C0546Cf221e674711b6C38257a545b6"));
    }

    public async void Mint() {
        print(await NFT_ERC721.MintReward("0xD48ab8a75C0546Cf221e674711b6C38257a545b6"));
    }
}
