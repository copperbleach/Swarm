using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public float cellSize = 1f;

    // Visualizes the grid in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int x = 0; x <= width; x++)
        {
            Gizmos.DrawLine(new Vector3(x * cellSize, 0), new Vector3(x * cellSize, height * cellSize));
        }
        for (int y = 0; y <= height; y++)
        {
            Gizmos.DrawLine(new Vector3(0, y * cellSize), new Vector3(width * cellSize, y * cellSize));
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize + (cellSize / 2f), y * cellSize + (cellSize / 2f));
    }
}