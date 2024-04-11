# 02: Procedural Generation

In today's class, we will take our procedurally generated maze data and use it to build a 3D maze in our project.

## Vertices, Edges, Faces and Meshes

Before we continue, it is important that we have a basic understanding of the fundamentals of 3D graphics. There are some key terms that you should know:

- A **vertex** (plural "vertices") is simply a defined point in 3D space. For example, a vertex could be: (0, 3.5, 6.1). If you were to sketch a dot on a sheet of graph paper, you could imagine it as a vertex.
- An **edge** is a connection between two vertices. An edge is analagous to the edge of a real world object e.g. the front top edge of your desk. If you sketched a second vertex on your sheet of paper and drew a line between the two vertices, you would have created an edge.
- A **face** is a flat surface that is contained within some edges (these are also refered to as "polygons"). If you joined three vertices together with edges to form a triangle and shaded it in, you could imagine that shaded area as a face.
- A **mesh** is a collection of vertices, connected by edges to form faces. A sufficiently detailed mesh can create any 3D object imaginable.

This is a Blender resource, but the first section of the article recaps what is written above ^
> Resource: <https://en.wikibooks.org/wiki/Blender_3D:_Noob_to_Pro/What_is_a_Mesh%3F>

## Task 1: Generating a Mesh

Before we can make our entire maze, let's get some practice making meshes in code.

A `Mesh` is a Unity class that is used to represent a **3D mesh.** A `Mesh` instance can be assigned to a `MeshFilter` component with a `MeshRenderer` to display it ingame.

We are going to define a basic type of mesh, called a **quad**. A quad is just a flat, 4-sided shape in 3D space. This is the kind of thing that's best explained with an example, so here is some example code to demonstrate a typical process for setting up a mesh:

```csharp

public Mesh TestMesh()
{
    // Our mesh will need a list of vertices.
    List<Vector3> newVertices = new List<Vector3>();

    // Declare 4 vertices. These points will form a square shape.
    newVertices = new List<Vector3>
    {
        new Vector3(-.5f, -.5f, 0),
        new Vector3(-.5f, .5f, 0),
        new Vector3(.5f, .5f, 0),
        new Vector3(.5f, -.5f, 0)
    };

    // Triangles are a way of defining edges and faces together.
    // The way this list is structured is quite specific:
    //     - Each element represents the index of a vertex from our newVertices list.
    //     - Every 3 indices indicate a triangle, so:
    //           - { 0, 1, 2... indicates a triangle between vertices 0, 1, and 2
    //           -  ...0, 2, 3} indicates a triangle between vertices 0, 2 and 3.
    //     - From these triangles, our mesh determines its edges and faces.
    List<int> newTriangles = new List<int> { 0, 1, 2, 0, 2, 3 };

    // UVs store the texture co-ordinates of each vertex 
    // (used to figure out which part of the texture goes on which part of the mesh.)
    List<Vector2> newUVs = new List<Vector2>();

    // Each UV is associated with a vertex. 
    // Don't worry about these too much, we won't do anything more advanced with them than this.
    newUVs = new List<Vector2>
    {
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1),
        new Vector2(0, 0)
    };

    // Declare a new mesh instance.
    Mesh mesh = new Mesh();
    // Call SetVertices to assign our list of vertices to the mesh.
    mesh.SetVertices(newVertices);
    // Assign the UVs. The 0 isn't important, you can look it up if you're curious.
    mesh.SetUVs(0, newUVs);
    // Assign the triangles. Again, the 0 isn't important for this example. 
    // It's only relevant if our mesh is broken up into multiple sub-meshes (which we will do for the maze).
    mesh.SetTriangles(newTriangles, 0);

    return mesh;
}

```

> Resource: <https://docs.unity3d.com/ScriptReference/Mesh.html>

Looks like a lot (and it kinda is - there's a reason people invented 3DS Max and Maya). The basic process is:
- Make a list of all the **vertices** (points) you would like to include in your **mesh**
- Make a list of how you want those points to be connected (**triangles**)
- Slap some **UV coordinates** in (really, just copy the code for this)
- Declare your **new mesh** and assign everything to it.

Badaboom, we have a quad mesh. This is the only type of mesh we are going to use for our maze - each segment of the maze will simply be several quads rotated in various ways.

### Actually making it render:

Your first task is to take this quad generation code and get the `Mesh` it generates to **render** on the screen.

To do this, you will need to consider the following:

- It's currently only a function. You should put it inside a class whose purpose is to generate mazes.
- In this class' `Start` function, it will need to do the following:
    - Create a new `GameObject` from scratch (not a prefab). I'll let you figure out how to do this.
    - Give that `GameObject` a name and position it at the world origin. (0, 0, 0).
    - Add a `MeshFilter` component to your `GameObject`. This component stores the mesh data, so you will need to assign the output of your `TestMesh` function.
    - Add a `MeshRenderer` component. This is responsible for assigning a material to our mesh so that we can actually see it.
    - Assign a material to that `MeshRenderer`. The material we are going to use can be found in the project and is called "wall-mat".

> Tip: You may also like to temporarily place a camera in your scene. Remember to remove it later once we get to the first person character implementation.

If you managed to get this working, and you assigned your maze generator component to a game object in the scene, you should see a new `GameObject` get created when you run your game. This game object should appear as a flat one-sided wall with a dark brown material on it.

## Task 2: Transformations

This quad is neat, but you can imagine how difficult it would be to calculate each vertex of our maze, and connect them all up, making sure that the triangles are all correct.

Thankfully, we can **transform** quads just like we would move, rotate and scale an object in the editor. We'll be able to make many copies of this quad, all transformed differently to create our maze.

The best way to do this is with a Translation, Rotation and Scale **Matrix**, or in Unity terms: `Matrix4x4TRS`.
We can define a `Matrix4x4TRS` as though it were a transformation we were going to make to a game object in the Unity editor, then apply it to all the vertices in our mesh with the `.MultiplyPoint3x4` method. Here's an example:

```csharp

public void TransformTestMesh(Mesh testMesh) 
{
    // This will store all of the new vertices once they have been transformed.
    List<Vector3> transformedVertices = new List<Vector3>();

    // Define the transformation we are going to apply to our mesh.
    // It's the same concept as changing the transform values of a game object in the editor.
    Matrix4x4 translation = Matrix4x4.TRS(
        new Vector3(4, 0, 0), // new position.
        Quaternion.Euler(90, 0, 0), // new rotation.
        Vector3.one); // new scale.
    
    for (var i = 0; i < mesh.vertices.Length; i++)
    {
        // Apply the transformation to the vertex, then add it to our list of new vertices.
        transformedVertices.Add(translation.MultiplyPoint3x4(newVertices[i]));
    }

    // Update the vertex list of the mesh.
    mesh.SetVertices(transformedVertices);
}


```

With this example in hand, do some experimentation.

- Create 3 **individual meshes** from your mesh generator class.
- Each mesh should be **positioned** differently in the world.
- One mesh should be **rotated** differently than the others.
- One mesh should be **scaled** differently from the others.


# Task 3: The Maze

Time to actually generate our maze.

This is still a tricky task, so you will be provided some starter code. Where there is a comment marked TODO, you must implement that part of the code yourself.

```csharp
using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator : MonoBehaviour
{
    float width = 3.75f; // You can change these if you'd like your maze to be bigger/smaller.
    float height = 3.5f;

    public Mesh GenerateMazeMeshFromData(int[,] data)
    {
        // TODO: Declare your mesh, new vertices list and new UVs list here. 
        // The lists will initially be empty.

        // TODO: Set the subMeshCount of your mesh to 2. This will allow us to
        // display different materials on the floor and walls.

        // TODO: Create 2 lists for your triangles, one for the floor triangles
        // and 1 for the wall triangles.

        // The rows and columns of our data we are going to span.
        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);

        // Gives us half of the cell height. This is used to position our wall quads.
        float halfH = height * .5f;

        // Iterate over our data.
        for (int i = 0; i <= rMax; i++)
            for (int j = 0; j <= cMax; j++)
                if (data[i, j] != 1)
                {
                    // -- FLOOR AND CEILING --
                    // Generate a quad for the floor
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, 0, i * width),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);
                    
                    // TODO: Generate a quad for the ceiling.
                    // Like with the floor, it should use floorTriangles.
                    // -------------------------

                    // -- WALLS --
                    // If the cell to our back is out of bounds, or closed, 
                    // generate a quad to the back of this cell.
                    if (i - 1 < 0 || data[i - 1, j] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i - .5f) * width),
                            Quaternion.LookRotation(Vector3.forward),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);

                    // TODO: If the cell to our right is out of bounds, or closed,
                    // generate a quad to the right of this cell. All of the walls
                    // should use wallTriangles.

                    // TODO: If the cell to our front us is out of bounds, or closed,
                    // generate a quad to the front of this cell.

                    // TODO: If the cell to our left us is out of bounds, or closed,
                    // generate a quad to the left of this cell.
                    // ------------

                }

        // TODO: Set the vertices and UVs of the maze mesh.

        // Sets triangles for the 2 submeshes (one for the floor, and another for the walls.)
        maze.SetTriangles(floorTriangles.ToArray(), 0);
        maze.SetTriangles(wallTriangles.ToArray(), 1);

        // Ensures the maze renders properly with lighting etc.
        maze.RecalculateNormals();

        return maze;
    }

    // Adds a quad like we did in our examples.
    // We have to provide a matrix that will perform a transformation on the vertices.
    // newVertices, newUVs and newTriangles are all passed in using the "ref" keyword.
    // This allows us to cumultaively build up a massive list of vertices, UVs and tris
    // as we loop through the data arrays, which will ultimately get assigned to the mesh.
    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
        ref List<Vector2> newUVs, ref List<int> newTriangles)
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

        newTriangles.Add(index + 2);
        newTriangles.Add(index + 1);
        newTriangles.Add(index);

        newTriangles.Add(index + 3);
        newTriangles.Add(index + 2);
        newTriangles.Add(index);
    }
}

```

<details>
<summary>Solution Example</summary>

```csharp

using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator : MonoBehaviour
{
    // generator params
    public float width = 3.75f;     // how wide are hallways
    public float height = 3.5f;    // how tall are hallways

    public Mesh GenerateMazeMeshFromData(int[,] data)
    {
        Mesh maze = new Mesh();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        maze.subMeshCount = 2;
        List<int> floorTriangles = new List<int>();
        List<int> wallTriangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        float halfH = height * .5f;

        for (int i = 0; i <= rMax; i++)
            for (int j = 0; j <= cMax; j++)
                if (data[i, j] != 1)
                {
                    // floor
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, 0, i * width),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);

                    // ceiling
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, height, i * width),
                        Quaternion.LookRotation(Vector3.down),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);

                    if (i - 1 < 0 || data[i-1, j] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i-.5f) * width),
                            Quaternion.LookRotation(Vector3.forward),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);

                    if (j + 1 > cMax || data[i, j+1] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j+.5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.left),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);

                    if (j - 1 < 0 || data[i, j-1] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j-.5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.right),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);

                    if (i + 1 > rMax || data[i+1, j] == 1)
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i+.5f) * width),
                            Quaternion.LookRotation(Vector3.back),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                }

        maze.vertices = newVertices.ToArray();
        maze.uv = newUVs.ToArray();

        maze.SetTriangles(floorTriangles.ToArray(), 0);
        maze.SetTriangles(wallTriangles.ToArray(), 1);
        maze.RecalculateNormals();

        return maze;
    }

    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
        ref List<Vector2> newUVs, ref List<int> newTriangles)
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

        newTriangles.Add(index+2);
        newTriangles.Add(index+1);
        newTriangles.Add(index);

        newTriangles.Add(index+3);
        newTriangles.Add(index+2);
        newTriangles.Add(index);
    }
}
```
</details>


If you managed to implement all of that successfully, well done. Now all that's left is to hook up the game object you created programatically in task 1 to the mesh generated by this function, and pass in your randomly generated maze data. You will also have to assign both the floor and wall materials to the `MeshRenderer`, but if you've made it this far, I'm sure you can figure that part out :)

## Conclusion

As you can tell, making meshes by code can be quite laborious. However, it gives us incredible power to create interesting and dynamic procedural content for our games, and leads to often interesting and challenging algorithmic problems.

The main takeaways from this class are:
- 3D meshes are made up of vertices. You can create any 3D object with a sufficiently detailed mesh.
- We can create meshes programatically using Unity's `Mesh` class. A mesh must be assigned to a `MeshFilter` with a `MeshRenderer` to display it.
- We can transform meshes using Matrices. When transforming a mesh, we go through each vertex and apply the transformation to it, then reassign all of the vertices to the mesh.
