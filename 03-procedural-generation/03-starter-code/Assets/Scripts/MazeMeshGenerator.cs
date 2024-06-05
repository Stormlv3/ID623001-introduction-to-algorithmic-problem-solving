using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator : MonoBehaviour
{
    public float width = 3.75f; // Width of the maze cells.
    public float height = 3.5f; // Height of the maze walls.

    public void GenerateMaze(int[,] data)
    {
        // Generate the floor mesh
        Mesh floorMesh = GenerateFloorMeshFromData(data);
        CreateMazeGameObject("MazeFloor", floorMesh, "floor-mat");

        // Generate the wall mesh
        Mesh wallMesh = GenerateWallMeshFromData(data);
        CreateMazeGameObject("MazeWalls", wallMesh, "wall-mat");

        // Generate the roof mesh
        Mesh roofMesh = GenerateRoofMeshFromData(data);
        CreateMazeGameObject("MazeRoof", roofMesh, "roof-mat");
    }

    public Mesh GenerateFloorMeshFromData(int[,] data)
    {
        Mesh floorMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (data[i, j] != 1)
                {
                    // Generate a quad for the floor
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, 0, i * width),
                        Quaternion.LookRotation(Vector3.down),  // Rotated 180 degrees
                        new Vector3(width, width, 1)
                    ), ref vertices, ref uvs, ref triangles);
                }
            }
        }

        floorMesh.vertices = vertices.ToArray();
        floorMesh.uv = uvs.ToArray();
        floorMesh.triangles = triangles.ToArray();
        floorMesh.RecalculateNormals();

        // Add a collider to the floor mesh
        MeshCollider floorCollider = gameObject.AddComponent<MeshCollider>();
        floorCollider.sharedMesh = floorMesh;

        return floorMesh;
    }


    private Mesh GenerateRoofMeshFromData(int[,] data)
    {
        Mesh roofMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (data[i, j] != 1)
                {
                    // Generate a quad for the ceiling
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, height, i * width),
                        Quaternion.LookRotation(Vector3.up),  // Rotated 180 degrees
                        new Vector3(width, width, 1)
                    ), ref vertices, ref uvs, ref triangles);
                }
            }
        }

        roofMesh.vertices = vertices.ToArray();
        roofMesh.uv = uvs.ToArray();
        roofMesh.triangles = triangles.ToArray();
        roofMesh.RecalculateNormals();

        return roofMesh;
    }

    private Mesh GenerateWallMeshFromData(int[,] data)
    {
        Mesh wallMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        float halfH = height * .5f;

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (data[i, j] != 1)
                {
                    // Generate quads for the walls
                    if (i - 1 < 0 || data[i - 1, j] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i - .5f) * width),
                            Quaternion.LookRotation(Vector3.back),  // Inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);

                    if (j + 1 > cMax || data[i, j + 1] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j + .5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.right),  // Inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);

                    if (j - 1 < 0 || data[i, j - 1] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j - .5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.left),  // Inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);

                    if (i + 1 > rMax || data[i + 1, j] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i + .5f) * width),
                            Quaternion.LookRotation(Vector3.forward),  // Inward facing
                            new Vector3(width, height, 1)
                        ), ref vertices, ref uvs, ref triangles);
                }
            }
        }

        wallMesh.vertices = vertices.ToArray();
        wallMesh.uv = uvs.ToArray();
        wallMesh.triangles = triangles.ToArray();
        wallMesh.RecalculateNormals();

        return wallMesh;
    }

    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices, ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        Vector3 vert1 = new Vector3(-.5f, -.5f, 0);
        Vector3 vert2 = new Vector3(-.5f, .5f, 0);
        Vector3 vert3 = new Vector3(.5f, .5f, 0);
        Vector3 vert4 = new Vector3(.5f, -.5f, 0);

        newVertices.Add(matrix.MultiplyPoint3x4(vert1));
        newVertices.Add(matrix.MultiplyPoint3x4(vert2));
        newVertices.Add(matrix.MultiplyPoint3x4(vert3));
        newVertices.Add(matrix.MultiplyPoint3x4(vert4));

        newUVs.Add(new Vector2(1, 0));
        newUVs.Add(new Vector2(1, 1));
        newUVs.Add(new Vector2(0, 1));
        newUVs.Add(new Vector2(0, 0));

        newTriangles.Add(index + 0);
        newTriangles.Add(index + 1);
        newTriangles.Add(index + 2);
        newTriangles.Add(index + 0);
        newTriangles.Add(index + 2);
        newTriangles.Add(index + 3);
    }

    private void CreateMazeGameObject(string name, Mesh mesh, string materialName)
    {
        // Create a new GameObject.
        GameObject mazePart = new GameObject(name);
        
        // Position it at the world origin.
        mazePart.transform.position = Vector3.zero;

        // Add a MeshFilter component to our GameObject.
        MeshFilter meshFilter = mazePart.AddComponent<MeshFilter>();
        
        // Assign the mesh.
        meshFilter.mesh = mesh;

        // Add a MeshRenderer component.
        MeshRenderer meshRenderer = mazePart.AddComponent<MeshRenderer>();
        
        // Assign a material to the MeshRenderer (make sure you have the material).
        meshRenderer.material = Resources.Load<Material>(materialName);

        // Add a MeshCollider component to the GameObject.
        MeshCollider meshCollider = mazePart.AddComponent<MeshCollider>();
        
        // Assign the mesh to the collider.
        meshCollider.sharedMesh = mesh;
    }

    private void Start()
    {
        // Create a new instance of MazeConstructor
        MazeConstructor mazeConstructor = new MazeConstructor();
        int[,] mazeData = mazeConstructor.GenerateMazeDataFromDimensions(30, 30);

        // Generate the maze using the data
        GenerateMaze(mazeData);
    }
}