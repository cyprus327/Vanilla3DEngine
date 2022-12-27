# Overview
## 3D Rendering and physics in vanilla C#, no OpenGL or any other libraries.
- Made to be very simple to use
- Comes with some primitives but also supports loading OBJ files
- Dynamic objects / static objects supported (collision detection uses GJK)
# Examples:
### Utah teapot
![utah teapot](https://i.postimg.cc/JzZbGwXC/image.png "Utah Teapot Example")
### Explosion of cubes and spheres
![explosion of primitives](https://i.postimg.cc/s2B1v7qQ/image.png "Explosion of Cubes and Spheres Example")
Code to generate the explosion
![code for primitives explosion](https://i.postimg.cc/sDDW3QjS/image.png "Code for primitives explosion")
# Basic Usage
## Getting started:
1. Create a class that inherits from Engine
2. Create a constructor that takes in a Size and string 
![1st steps code](https://i.postimg.cc/4NjHT8m7/image.png "Code for example class and constructor")
4. Override the Awake and Update methods
5. In Program.cs or wherever your Main method is, create a variable of type Engine and set it equal to the new class you've created
6. Call .Run() on the variable
## Update and Awake overrides:
Awake is run first as soon as the program begins, and Update is run every frame after that.
## GameObjects:
### Adding and removing objects:
GameObjects are added by using the Instantiate method, parameters being the GameObject to add and the ID of the GameObject. IDs are provided as a way to keep track of each GameObject and have the ability to change/delete individual objects after creation. To delete an object simply call the Delete method and pass in the ID of the object as the only parameter.
### GameObject class:
The GameObject class contains information about the Position and Rotation (both stored in Transform), Mass, Force, Velocity, Color, Mesh, and whether or not the GameObject is Dynamic.
### The Step method:
The Step method is used to calculate the physics on individual GameObjects, and is virtual so it can be overrode to provide custom physics to each object.
