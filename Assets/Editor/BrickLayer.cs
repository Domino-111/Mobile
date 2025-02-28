using UnityEngine;
using System.Collections.Generic;

//This namespace has all the stuff to do with editor functionality
using UnityEditor;
using UnityEditor.SceneManagement;

//EditorWindow is the base class for any custom editor windows
public class BrickLayer : EditorWindow
{
    public static BrickLayer thisWindow;

    //MenuItem lets us run a static function from the menu bar
                            //menuItem is the path (e.g. File/Save)
    [MenuItem("Brick Layer", menuItem = "MyGame/Brick Layer")]

    public static void Open()
    {
        //Get or open a new window
        thisWindow = GetWindow<BrickLayer>("Brick Layer");
    }

    public int rows, columns;
    public GameObject prefab;

    public Transform parent;

    public List<GameObject> bricksInScene = new List<GameObject>();

    //This runs every frame the window is currently active (last thing selected)
    private void OnGUI()
    {
        //EditorGUILayout has all the functions to auto-layout our window fields
                //(GameObject) means to cast our result into the GameObject type
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to spawn", prefab, typeof(GameObject), false);

        //"true" here lets us use scene objects as well as assets
        parent = (Transform)EditorGUILayout.ObjectField("Parent game object", parent, typeof(Transform), true);

        //Set rows to whatever value is put in the field (which will be rows if no new number has been added)
                //Prevent the input result from going below 0
        rows = Mathf.Clamp(EditorGUILayout.IntField("Rows", rows), 0, 8);
        columns = Mathf.Clamp(EditorGUILayout.IntField("Columns", columns), 0, 9);

        //Lets you define a condition under which the following GUI items can be interacted with or not
        EditorGUI.BeginDisabledGroup(prefab == null);

        //In one go, draw the button and check if it's been clicked
        if (GUILayout.Button("Lay Bricks"))
        {
            SpawnObjects();
        }

        EditorGUI.EndDisabledGroup();

        if (bricksInScene.Count > 0)
        {
            if (GUILayout.Button("Clear bricks"))
            {
                ClearObjects();
            }
        }
    }

    private void SpawnObjects()
    {
        float gap = 0.1f;

        //Get the prefab's x scale
        float brickWidth = prefab.transform.localScale.x;

        //Get the brick height as well as the gap to leave
        float brickHeightPlusGap = prefab.transform.localScale.y + gap;

        //Get the width of all our bricks, plus theier gaps (1 less than the brick count), then half that
                                                                      //and then add half our brick width on to re-centre
        float halfWidth = -(brickWidth * columns + gap * (columns - 1)) / 2f + brickWidth/2f;

        int count = 1;

        //Repeat down rows for as many columns we have
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //Spawn a brick in the top left and using our width and height to calculate where to spawn the next one
                GameObject spawned = (GameObject)PrefabUtility.InstantiatePrefab(prefab); 
                spawned.transform.position = new Vector3(halfWidth + i * brickWidth + gap * i, 4.5f - brickHeightPlusGap * j);

                spawned.name += count;

                if (parent)
                {
                    spawned.transform.parent = parent;
                }

                bricksInScene.Add(spawned);

                count++;
            }
        }

        MarkSceneDirty();
    }

    private void ClearObjects()
    {
        for (int i = 0; i < bricksInScene.Count;)
        {
            GameObject current = bricksInScene[0];

            //Remove the object from the list
            bricksInScene.Remove(current);

            if (current == null || !current.scene.IsValid())
            {
                continue;
            }
            
            //Destroy the object
            DestroyImmediate(current);
        }

        MarkSceneDirty();
    }

    private void MarkSceneDirty()
    {
        //Get the currently active scene and mark it dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
