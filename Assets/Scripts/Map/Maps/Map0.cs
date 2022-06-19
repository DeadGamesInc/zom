public class Map0 : BaseMap {
    public new void Start() {
        // Set Map Size
        Length = 10;
        Width = 10;
        CellSize = 50;

        // Run BaseMap Start()
        base.Start();
    }

    // Specify map paths
    protected override MapPath[] initializePaths() {
        return new MapPath[] {
            toPath((5, 1), (9, 5), (5, 9), (1, 5), (5, 1)),
            toPath((1, 1), (2, 2), (2, 1), (3, 1))
        };
    }
}