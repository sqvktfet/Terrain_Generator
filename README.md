# Terrain_Generator

Member 1: Dongjie Lin, dongjiel, 651824

Member 2: Sheng Lin, slin2, 652698

repo-link: https://github.com/sqvktfet/Terrain_Generator.git

###Methods generating terrain step by step:
1. Applying Diamond Square Algorithm to calculate heightmap for generating mesh vertices.
2. The terrain is generated with raw mesh. The x and z value of mesh vertices are their relative position to the origin of the world, the y value is the corresponding height in heightmap achieved from step 1.
3. Adding vertices to mesh.
4. Adding triangles to mesh.
5. Defining color for each vertex.
6. Defining normal vector for each vertex.

###Details in implementing Diamond Square Algorithm:
1. Initial values for 4 corners are calculated by “seed + randomError”, where seed is a tested value that has best performance for the algorithm.
2. The algorithm will have n iterations. For each iteration, the “diamond” step is executed first and then followed by the “square” step. Either step calculates height by adding a random value to the average values of surrounding points.
3. For each iteration, the range of random value is reduced by 50 percent. The random value can be either positive or negative, so that the occurrence of both mountain and valley will be possible.

###Details in defining the color for each vertex:
1. Calculate the height difference between the highest vertex and lowest vertex. 
2. Divide it into several classes.
3. For each class, a fixed color is assigned.
4. The color assigned for each vertex will only depend on which class it falls in.

	Hence, for every generation, it will be able to see both dark valley and bright peak.



###Details in Player and Camera Controller Implementation:

For Player or Camera Controller, there are three parts in total, player position movement controlled by ADWS button, player view rotation controlled by mouse, and player view rolling controlled by QE button. Generally, we create a player object to deal with the movement of the player and set the camera object, which aims at the rest rotation part, as the child of the player object.

First part is handled in PositionUpdate() method. In each frame, when input (W,A,S,D) is detected, the player’s movement in x and z axis is calculated, and the movement in y axis is computed based on the movement in x and z axis and the direction the player are currently facing (the value  is mouseLook.y). Then the value of the movement is applied to the translation of the player. After the translation, a checking of out_of_bound is handled by methods isXOutOfBound () and isZOutOfBound (), to prevent the player going out of the border of the terrain.

The method to deal with player view rotation is MouseViewRotationUpdate(). We simply get the mouse movement (Mouse X and Mouse Y) and make the rotation more smooth by using Mathf.Lerp() method.

The third task is the player view rolling, which is responsible by the method called CameraRollingUpdate(). This one is quite similar to the second task. We first get the input of Q or E, and do the rotation operation around the direction that the player is facing.

