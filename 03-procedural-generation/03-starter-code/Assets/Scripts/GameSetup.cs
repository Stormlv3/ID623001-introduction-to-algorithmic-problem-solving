using UnityEngine;

public class GameSetup : MonoBehaviour
{


    [SerializeField] private MazeMeshGenerator mazeMeshGenerator;
    [SerializeField] private MazeConstructor mazeConstructor;

    [SerializeField] private GameObject playerControllerPrefab;

    public void Start()
    {
        GeneratePlayer();
    }

    public void GeneratePlayer()
    {
        int xCoord = 1;
        int zCoord = 1;
        Vector3 startPos = new Vector3(xCoord * mazeMeshGenerator.width, 1, zCoord * mazeMeshGenerator.width);

        GameObject player = Instantiate(playerControllerPrefab, startPos, Quaternion.identity);
    }
}
