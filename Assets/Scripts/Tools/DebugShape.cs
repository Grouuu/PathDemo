using UnityEngine;

/**
 * From https://medium.com/@davidzulic/unity-drawing-custom-debug-shapes-part-1-4941d3fda905
 */
class Debug : UnityEngine.Debug {

    public static void DrawCircle (Vector3 position, float radius, int segments, Color color) {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0) {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++) {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            DrawLine(lineStart, lineEnd, color);
        }
    }

    public static void DrawRectangle (Vector3 position, Vector2 size, Color color, float duration = 10) {
        DrawRectangle(new Rect(position, size), color, duration);
    }

    public static void DrawRectangle (Vector4 rect, Color color, float duration = 10) {
        DrawRectangle(new Rect(new Vector2(rect.x, rect.y), new Vector2(rect.z - rect.x, rect.y - rect.w)), color, duration);
    }

    public static void DrawRectangle (Rect rect, Color color, float duration = 10) {
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), color, duration);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y - rect.height), color, duration);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y - rect.height), new Vector3(rect.x, rect.y - rect.height), color, duration);
        Debug.DrawLine(new Vector3(rect.x, rect.y - rect.height), new Vector3(rect.x, rect.y), color, duration);
    }

    public static void DrawCenteredRectangle (Vector3 position, Vector3 size, Color color, float duration = 10) {
        Vector3 halfSize = size / 2;
        Debug.DrawLine(new Vector3(position.x - halfSize.x, position.y + halfSize.y), new Vector3(position.x + halfSize.x, position.y + halfSize.y), color, duration);
        Debug.DrawLine(new Vector3(position.x + halfSize.x, position.y + halfSize.y), new Vector3(position.x + halfSize.x, position.y - halfSize.y), color, duration);
        Debug.DrawLine(new Vector3(position.x + halfSize.x, position.y - halfSize.y), new Vector3(position.x - halfSize.x, position.y - halfSize.y), color, duration);
        Debug.DrawLine(new Vector3(position.x - halfSize.x, position.y - halfSize.y), new Vector3(position.x - halfSize.x, position.y + halfSize.y), color, duration);
    }
}