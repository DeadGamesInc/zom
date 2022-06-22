using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    public static LocationBase GetLocationBase(this GameObject obj) => obj.GetComponent<LocationBase>();
    
    public static void ChangeAlpha(this Material material, float alpha) {
        var oldColor = material.color;
        var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        material.SetColor("_Color", newColor);
    }
}
