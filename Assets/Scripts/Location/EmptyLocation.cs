public class EmptyLocation : LocationBase {
    public void OnMouseEnter() {
        var controller = LevelController.Get();
        controller.SetStatusText("EMPTY LOCATION");
        controller.SelectedEmptyLocation = gameObject;
    }

    public void OnMouseExit() {
        var controller = LevelController.Get();
        controller.SetStatusText("");
        controller.SelectedEmptyLocation = null;
    }
}
