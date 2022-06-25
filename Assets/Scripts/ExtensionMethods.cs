using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public static class ExtensionMethods {
    public static MapBase GetMapBase(this GameObject obj) => obj.GetComponent<MapBase>();
    public static MapNode GetMapNode(this GameObject obj) => obj.GetComponent<MapNode>();
    public static LocationBase GetLocationBase(this GameObject obj) => obj.GetComponent<LocationBase>();
    public static Character GetCharacter(this GameObject obj) => obj.GetComponent<Character>();
    public static Card GetCard(this GameObject obj) => obj.GetComponent<Card>();
    public static Opponent GetOpponent(this GameObject obj) => obj.GetComponent<Opponent>();
    public static CinemachineVirtualCamera GetVirtualCamera(this GameObject obj) => obj.GetComponent<CinemachineVirtualCamera>();
    public static CoroutineRunner GetCoroutineRunner(this GameObject obj) => obj.GetComponent<CoroutineRunner>();
    
    public static void ChangeAlpha(this Material material, float alpha) {
        var oldColor = material.color;
        var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        material.SetColor("_Color", newColor);
    }
}
