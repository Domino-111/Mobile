using UnityEngine;
using UnityEditor;

public class SecondBall : EditorWindow
{
    public static SecondBall thisWindow;

    [MenuItem("Second Ball", menuItem = "MyGame/Second Ball")]

    public static void Open()
    {
        //Get or open a new window
        thisWindow = GetWindow<SecondBall>("Second Ball");
    }

    public int xCoord, yCoord;
    public GameObject prefab;

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to spawn", prefab, typeof(GameObject), false);

        xCoord = Mathf.Clamp(EditorGUILayout.IntField("X Coordinate", xCoord), -5, 5);
        yCoord = Mathf.Clamp(EditorGUILayout.IntField("Y Coordinate", yCoord), -5, 5);

        if (GUILayout.Button("Summon Second Ball"))
        {
            SpawnBall();
        }
    }

    private void SpawnBall()
    {
        float xPos = prefab.transform.localScale.x;
        float yPos = prefab.transform.localScale.y;


        GameObject spawned = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        spawned.transform.position = new Vector3(xPos, yPos);

    }
}
