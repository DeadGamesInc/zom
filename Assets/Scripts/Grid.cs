public class Grid {
    private int width;
    private int height;
    private int[,] gridArray;

    private Grid(int width, int height) {
        this.width = width;
        this.height = height;

        gridArray = new int[width, height];
    }
}