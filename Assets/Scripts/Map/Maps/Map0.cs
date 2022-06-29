public class Map0 : MapBase {
    protected override MapPath[] InitializePaths() => new[] { ToPath((8, 2),(10, 4), (12, 6), (14, 8), (12, 10), (10, 12), (8, 14), (6, 12), (4, 10), (2, 8), (4, 6), (6, 4)), };
    
    public new void Initialize() {
        Length = 16;
        Width = 16;
        CellSize = 30;
        DistanceUnit = 4;

        base.Initialize();
    }

    protected override void InitializeLocations() {
        var controller = LevelController.Get();
        controller.CreateStarterLocation(Grid, (8, 1), GetNode(8, 2), 0);
        controller.CreateEmptyLocation(Grid, (15, 8), GetNode(14, 8));
        controller.CreateStarterLocation(Grid, (8, 15), GetNode(8, 14), 1);
        controller.CreateEmptyLocation(Grid, (1, 8), GetNode(2, 8));
    }

    protected override void InitializeBrainsNodes() {
        BrainsNode.Create(7, 1, Grid, GetNode(8, 2));
        BrainsNode.Create(9, 1, Grid, GetNode(8, 2));
        BrainsNode.Create(15, 7, Grid, GetNode(14, 8));
        BrainsNode.Create(15, 9, Grid, GetNode(14, 8));
        BrainsNode.Create(7, 15, Grid, GetNode(8, 14));
        BrainsNode.Create(9, 15, Grid, GetNode(8, 14));
        BrainsNode.Create(1, 7, Grid, GetNode(2, 8));
        BrainsNode.Create(1, 9, Grid, GetNode(2, 8));
    }
}