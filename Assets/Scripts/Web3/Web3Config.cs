using UnityEngine;

public class Web3Config : MonoBehaviour {
    [SerializeField] public string ChainID, Blockchain, Network, BlockchainNode;
    
    private static GameObject GetGameObject() => GameObject.Find("GameController");
    public static Web3Config Get() => GetGameObject().GetComponent<Web3Config>();
}
