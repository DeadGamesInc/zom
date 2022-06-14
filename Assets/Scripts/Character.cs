using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    private BaseMap map;
    
    // Start is called before the first frame update
    void Start() {
        // For testing purposes
        StartCoroutine(setMap());
    }

    // Update is called once per frame
    void Update() {
        // Debug.Log("yoo");

        // setMapPostion(1, 1);

    }

    private IEnumerator setMap() {
        GameObject mapObject;
        while ((mapObject = GameObject.Find("Map")) == null) yield return null;
        map = mapObject.GetComponent<BaseMap>();
        SetMapPosition(5, 1);

    }

    public Vector3 SetMapPosition(int x, int z) {
        return transform.position = map.GetNodeWorldPosition(x, z);
    }
}