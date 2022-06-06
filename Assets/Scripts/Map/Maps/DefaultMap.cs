using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMap : BaseMap {
    // Tuple values cannot exceed the Map Height and Width
    protected override (int, int)[][] InitializePaths() {
        return new (int, int)[][] {
            new (int, int)[] { (5, 1), (9, 5), (5, 9), (1, 5), (5, 1) },
            new (int, int)[] { (1, 1), (2, 2), (2, 1), (3, 1) }
        };
    }
}