using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public int rows = 10;
    public int columns = 10;

    public GameObject floor;
    public GameObject wall;
    public GameObject ladder;
    public GameObject enemy;
    public GameObject player;

    private Transform boardTransform;

    //A very simple board setup to testing initial project
    void BoardSetup()
    {
        GameObject toInstantiate;
        GameObject instance;
        boardTransform = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = wall;
                }
                else
                {
                    toInstantiate = floor;
                }

                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardTransform);
            }
        }
    }

    public void SetupScene()
    {

        BoardSetup();
        //Instantiate(player, new Vector3(columns - 2, rows - 2, 0f), Quaternion.identity);
        Instantiate(ladder, new Vector3(columns - 3, rows - 3, 0f), Quaternion.identity);
        Instantiate(enemy, new Vector3(columns - 4, rows - 4, 0f), Quaternion.identity);
    }
}
