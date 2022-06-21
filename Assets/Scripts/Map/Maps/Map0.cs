using System.Collections;
using UnityEngine;

public class Map0 : BaseMap {
    public new void Initialize() {
        // Set Map Size
        Length = 16;
        Width = 16;
        CellSize = 30;

        base.Initialize();
    }

    // Specify map paths
    protected override MapPath[] initializePaths() {
        return new MapPath[] {
            toPath((8, 2),(10, 4), (12, 6), (14, 8), (12, 10), (10, 12), (8, 14), (6, 12), (4, 10), (2, 8), (4, 6), (6, 4), (8, 2)),
        };
    }

    protected override void InitializeLocations() {
        if (_levelController == null) return;
        _levelController.CreateEmpty(grid, (8, 1), GetNode(8, 2));
        _levelController.CreateEmpty(grid, (15, 8), GetNode(14, 8));
        _levelController.CreateEmpty(grid, (8, 15), GetNode(8, 14));
        _levelController.CreateEmpty(grid, (1, 8), GetNode(2, 8));
    }
}