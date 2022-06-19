public class Map0 : BaseMap {
    public new void Start() {
        // Set Map Size
        Length = 14;
        Width = 14;
        CellSize = 35;

        // Run BaseMap Start()
        base.Start();
    }

    // Specify map paths
    protected override MapPath[] initializePaths() {
        return new MapPath[] {
            toPath((7, 1),(9, 3), (11, 5), (13, 7), (11, 9), (9, 11), (7, 13), (5, 11), (3, 9), (1, 7), (3, 5), (5, 3), (7, 1)),
            toPath((1, 2), (2, 3), (2, 2), (3, 2))
        };
    }
}