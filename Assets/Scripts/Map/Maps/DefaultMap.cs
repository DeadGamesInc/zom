public class DefaultMap : BaseMap {
    // Tuple values cannot exceed the Map Height and Width
    protected override MapPath[] InitializePaths() {
        return new MapPath[] {
            toPath((5, 1), (9, 5), (5, 9), (1, 5), (5, 1)),
            toPath((1, 1), (2, 2), (2, 1), (3, 1))
        };
    }
}