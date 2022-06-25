using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour {
    public void ConsecutiveRun(List<IEnumerator> coroutines) {
        StartCoroutine(consecutiveRun(coroutines));
    }
    
    private IEnumerator consecutiveRun(List<IEnumerator> coroutines) {
        if (coroutines.Any()) {
            var coroutine = StartCoroutine(coroutines.First());
            yield return coroutine;
            coroutines.RemoveAt(0);
            StartCoroutine(consecutiveRun(coroutines));
        }
    }
}