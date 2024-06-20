using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator : MonoBehaviour
{
    public MazeConstructor mazeContructor;
    // width of the maze cells
    public float width = 3.75f;

    // height of the maze walls
    public float height = 3.5f;

    private void Start()
    {
        // create a new instance of MazeConstructor
        MazeConstructor mazeConstructor = new MazeConstructor();
        // generate maze data with specified dimensions
        // int[,] mazeData = mazeConstructor.GenerateMazeDataFromDimensions(30, 30);

        // generate the maze using the generated data
        GenerateMaze(mazeContructor.Data);
    }
    public void GenerateMaze(int[,] data)
    {
        // generate the floor mesh from maze data
        Mesh floorMesh = GenerateFloorMeshFromData(data);
        // create a game object for the floor and assign the floor mesh and material
        CreateMazeGameObject("MazeFloor", floorMesh, "floor-mat");

        // generate the wall mesh from maze data
        Mesh wallMesh = GenerateWallMeshFromData(data);
        // create a game object for the walls and assign the wall mesh and material
        CreateMazeGameObject("MazeWalls", wallMesh, "wall-mat");

        // generate the roof mesh from maze data
        Mesh roofMesh = GenerateRoofMeshFromData(data);
        // create a game object for the roof and assign the roof mesh and material
        CreateMazeGameObject("MazeRoof", roofMesh, "roof-mat");
    }

    public Mesh GenerateFloorMeshFromData(int[,] data)
    {
        // create a new mesh for the floor
        Mesh floorMesh = new Mesh();
        // lists to store vertices, uvs, and triangles for the floor mesh
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // get the maximum row index
        int rMax = data.GetUpperBound(0);
        // get the maximum column index
        int cMax = data.GetUpperBound(1);

        // loop through each cell in the maze data
        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                // if the cell is not a wall (value != 1)...
                if (data[i, j] != 1)
                {
                    // generate a quad for the floor at the cell position
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, 0, i * width),
                        Quaternion.LookRotation(Vector3.down),  // rotated 180 degrees
                        new Vector3(width, width, 1)
                    ), ref vertices, ref uvs, ref triangles);
                }
            }
        }

        // assign the vertices, uvs, and triangles to the floor mesh
        floorMesh.vertices = vertices.ToArray();
        floorMesh.uv = uvs.ToArray();
        floorMesh.triangles = triangles.ToArray();
        // recalculate the normals for the floor mesh
        floorMesh.RecalculateNormals();

        // add a collider to the floor mesh
        MeshCollider floorCollider = gameObject.AddComponent<MeshCollider>();
        floorCollider.sharedMesh = floorMesh;

        // return the floor mesh
        return floorMesh;
    }

    private Mesh GenerateRoofMeshFromData(int[,] data)
    {
        // create a new mesh for the roof
        Mesh roofMesh = new Mesh();
        // lists to store vertices, uvs, and triangles for the roof mesh
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // get the maximum row index
        int rMax = data.GetUpperBound(0);
        // get the maximum column index
        int cMax = data.GetUpperBound(1);

        // loop through each cell in the maze data
        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                // if the cell is not a wall (value != 1)...
                if (data[i, j] != 1)
                {
                    // generate a quad for the roof at the cell position
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, height, i * width),
                        Quaternion.LookRotation(Vector3.up),  // rotated 180 degrees
                        new Vector3(width, width, 1)
                    ), ref vertices, ref uvs, ref triangles);
                }
            }
        }

        // assign the vertices, uvs, and triangles to the roof mesh
        roofMesh.vertices = vertices.ToArray();
        roofMesh.uv = uvs.ToArray();
        roofMesh.triangles = triangles.ToArray();
        // recalculate the normals for the roof mesh
        roofMesh.RecalculateNormals();

        // return the roof mesh
        return roofMesh;
    }

    private Mesh GenerateWallMeshFromData(int[,] data)
    {
        // create a new mesh for the walls
        Mesh wallMesh = new Mesh();
        // lists to store vertices, uvs, and triangles for the wall mesh
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // get the maximum row index
        int rMax = data.GetUpperBound(0);
        // get the maximum column index
        int cMax = data.GetUpperBound(1);
        // calculate half of the wall height
        float halfH = height * .5f;

        // loop through each cell in the maze data
        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                // if the cell is not a wall (value != 1)...
                if (data[i, j] != 1)
                {
                    // generate quads for the walls based on the neighboring cells
                    if (i - 1 < 0 || data[i - 1, j] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i - .5f) * width),
                            Quaternion.LookRotation(Vector3.back),  // inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);

                    if (j + 1 > cMax || data[i, j + 1] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j + .5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.right),  // inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);

                    if (j - 1 < 0 || data[i, j - 1] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j - .5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.left),  // inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);

                    if (i + 1 > rMax || data[i + 1, j] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i + .5f) * width),
                            Quaternion.LookRotation(Vector3.forward),  // inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);
                }
            }
        }

        // assign the vertices, uvs, and triangles to the wall mesh
        wallMesh.vertices = vertices.ToArray();
        wallMesh.uv = uvs.ToArray();
        wallMesh.triangles = triangles.ToArray();
        // recalculate the normals for the wall mesh
        wallMesh.RecalculateNormals();

        // return the wall mesh
        return wallMesh;
    }

    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices, ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        // get the current number of vertices
        int index = newVertices.Count;

        // define the vertices of the quad
        Vector3 vert1 = new Vector3(-.5f, -.5f, 0);
        Vector3 vert2 = new Vector3(-.5f, .5f, 0);
        Vector3 vert3 = new Vector3(.5f, .5f, 0);
        Vector3 vert4 = new Vector3(.5f, -.5f, 0);

        // transform and add the vertices to the list
        newVertices.Add(matrix.MultiplyPoint3x4(vert1));
        newVertices.Add(matrix.MultiplyPoint3x4(vert2));
        newVertices.Add(matrix.MultiplyPoint3x4(vert3));
        newVertices.Add(matrix.MultiplyPoint3x4(vert4));

        // add corresponding UV coordinates
        newUVs.Add(new Vector2(1, 0));
        newUVs.Add(new Vector2(1, 1));
        newUVs.Add(new Vector2(0, 1));
        newUVs.Add(new Vector2(0, 0));

        // add triangles to define the two faces of the quad
        newTriangles.Add(index + 0);
        newTriangles.Add(index + 1);
        newTriangles.Add(index + 2);
        newTriangles.Add(index + 0);
        newTriangles.Add(index + 2);
        newTriangles.Add(index + 3);
    }

    private void CreateMazeGameObject(string name, Mesh mesh, string materialName)
    {
        // create a new GameObject for the maze part
        GameObject mazePart = new GameObject(name);
        // position it at the world origin
        mazePart.transform.position = Vector3.zero;

        // add a MeshFilter component and assign the mesh
        MeshFilter meshFilter = mazePart.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // add a MeshRenderer component and assign the material
        MeshRenderer meshRenderer = mazePart.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>(materialName);

        // add a MeshCollider component and assign the mesh
        MeshCollider meshCollider = mazePart.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}