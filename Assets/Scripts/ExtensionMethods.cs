using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    public static LocationBase GetLocationBase(this GameObject obj) => obj.GetComponent<LocationBase>();
    public static Character GetCharacter(this GameObject obj) => obj.GetComponent<Character>();
    public static Card GetCard(this GameObject obj) => obj.GetComponent<Card>();
    public static Opponent GetOpponent(this GameObject obj) => obj.GetComponent<Opponent>();
    
    public static void ChangeAlpha(this Material material, float alpha) {
        var oldColor = material.color;
        var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        material.SetColor("_Color", newColor);
    }
}
