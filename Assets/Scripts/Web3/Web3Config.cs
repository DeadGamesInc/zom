using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web3Config : MonoBehaviour {
    [SerializeField] public string ChainID;
    [SerializeField] public string Blockchain;
    [SerializeField] public string Network;
    [SerializeField] public string BlockchainNode;
    
    public static GameObject GetGameObject() => GameObject.Find("GameController");
    public static Web3Config Get() => GetGameObject().GetComponent<Web3Config>();
}
