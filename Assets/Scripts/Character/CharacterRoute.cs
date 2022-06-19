using UnityEngine;

public class CharacterRoute {
    public MapNode CurrentTarget;
    public Vector3 CurrentTargetWorldPos;
    public MapPath Path;
    public int CurrentPathIndex;
    private BaseMap map;

    public CharacterRoute(BaseMap map, MapNode start, MapNode end) {
        MapPath? path = map.GetShortestPathBetweenNodes(start, end);
        if (!path.HasValue || (Path = path.Value).path.Length < 2) throw new MovementException("Path to target does not exist");
        CurrentPathIndex = 0;
        this.map = map;
        NextPosition();
    }

    public bool NextPosition() {
        CurrentPathIndex++;
        if (CurrentPathIndex == Path.path.Length) return false;
        CurrentTarget = Path.path[CurrentPathIndex];
        CurrentTargetWorldPos = map.GetNodeWorldPosition(CurrentTarget);
        return true;
    }
}