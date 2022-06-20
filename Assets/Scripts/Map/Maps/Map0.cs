using UnityEngine;

public class Map0 : BaseMap {
    public new void Start() {
        // Set Map Size
        Length = 16;
        Width = 16;
        CellSize = 30;

        // Run BaseMap Start()
        base.Start();
    }

    // Specify map paths
    protected override MapPath[] initializePaths() {
        return new MapPath[] {
            toPath((8, 2),(10, 4), (12, 6), (14, 8), (12, 10), (10, 12), (8, 14), (6, 12), (4, 10), (2, 8), (4, 6), (6, 4), (8, 2)),
            toPath((2, 3), (3, 4), (3, 3), (4, 3))
        };
    }

    protected override GameObject[] initializeLocations() {
        return new GameObject[] {
            // LocationBase.CreateEmpty(grid, (15, 8), GetNode(14, 8)),
            // LocationBase.CreateEmpty(grid, (1, 8), GetNode(2, 8))
        };
    }
}