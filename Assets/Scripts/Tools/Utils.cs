using UnityEngine;

public static class Utils {

	public static readonly float SQRT_2 = Mathf.Sqrt(2);

	public static bool IsInBounds (Vector2 position, Vector2 bounds) {
		return position.x >= 0 && position.x <= bounds.x && position.y >= 0 && position.y <= bounds.y;
	}

	public static int GridCoordsToIndex (int x, int y, int maxColumns) {
		return GridCoordsToIndex(new Vector2Int(x, y), maxColumns);
	}

	public static int GridCoordsToIndex (Vector2Int coords, int maxColumns) {
		return coords.x + coords.y * maxColumns;
	}

	public static Vector2Int GridIndexToCoords (int index, int maxColumns) {
		return new Vector2Int(index % maxColumns, (int) index / maxColumns);
	}

	public static Vector2Int GetCoordsFromPosition (Vector2 position, float gridSize) {
		return new Vector2Int(
			Mathf.RoundToInt(position.x / gridSize),
			Mathf.RoundToInt(position.y / gridSize)
		);
	}

	public static Vector2 GetPositionFromCoords (Vector2Int coords, float gridSize) {
		return new Vector2(
			coords.x * gridSize,
			coords.y * gridSize
		);
	}

	public static void PauseEditor () {
		UnityEditor.EditorApplication.isPaused = true;
	}

	public static Vector2 GetSceneSize () {
		float height = Camera.main.orthographicSize * 2;
		float width = height * Camera.main.aspect;
		return new Vector2(width, height);
	}

	// https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
	public static bool IsPowerOfTwo (int value) {
		return (value != 0) && ((value & (value - 1)) == 0);
	}

}
