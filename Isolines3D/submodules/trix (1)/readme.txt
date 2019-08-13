trix.exe

This program generates Triangulated Irregular Network, or TIN from scattered points on two-dimensional plane based on Delaunay's triangulation. This data structure allows data to be displayed as three-dimensional surface, or to be used for contouring and terrain analysis.

The source code is for a simple console program which only takes two arguments.
The first argument should be a name of the points file. Points are read from this  simple text file with a list of X-Y, or X-Y-Z
coordinates separated by comma. Below is example of input file for data
shown in the figure:
+-------------------+ 
| 200,790           |
| 370,760           |
| 60,670            |
| 360,890           |
| 280,620           |
| 30,880            |
| 230,960           |
+-------------------+ 
or, 
+-------------------+ 
| 200,790,100       | 
| 370,760,110       | 
| 60,670,115        | 
| 360,890,92        | 
| 280,620,125       | 
| 30,880,95         | 
| 230,960,110       |
+-------------------+ 
Data may have XY or XYZ coordinate. Although z coordinates are not used in the calculation, data with altitude information can be displayed in TriNET's demo functions. This list of triangles is saved to triangle file. Triangle file is generated in the same directory as the input file with extension "tri". 
If the point file xxxx.nod is triangulated, the results are saved to xxxx.tri in the same directory. 
The result of the triangulation will look like this. In this example, six triangles are generated and in the resulting "tri" file, sequential number of generated triangle vertices are listed as below
+-------------------+ 
| 1,6,7             |
| 1,3,6             |
| 2,7,4             |
| 1,2,5             |
| 1,7,2             |
| 1,5,3             |
+-------------------+ 
For example, the first triangle consists of the first, sixth and seventh vertices in the input file.

This program uses Delaunay's triangulation algorithm to construct TIN. There are a number of documents published or available on the net. I used a book written by John C. Davis, "Statistics and Data Analysis in Geology, Second Edition", John Wiley and Sons which included very plane but thorough description on this algorithm. 
The second argument to this program decides whether the convex hull is used or not. If this is set to N, or no second argument is given, in order to generate triangles beyond the area boundary, a set of pseudo points are generated around the extents of the points. 
Contrarily, if 'Y' is given to this parameter, the convex hull is generated and is used as a boundary of the input area. Convex hull (in 2D) is a smallest convex polygon, which include all input points. As shown in the example below, as compared to the network in the left, all parts in the convex hull are triangulated in the right figure, but it tends to create triangles of irregular shape(blue triangles in the right figure), which do not fulfill the condition that the largest inner angles of all generated triangles must be minimized.


This program was developed by Kazumi Sato.
Website: www.pixtopo.com
E-Mail: pixtopo@pixtopo.com
The author is not responsible to any loss or damage caused by this program.
Use at your own risk.

Usage: [Node File] (Use Convex Hull? Y/N )

===> Notes:
===> [Node File] must be a name of existing Node file.
===> [Node File] has a list of (X,Y) or (X,Y,Z) coordiinated 
===> delineated by comma(,). Output triangle file is created 
===> in the same directory with an extention 'tri'.
===> Option (Use Convex Hull?) determins whether all area in 
===> the convex hull are triangulated. This option must be Y or N. 
===> If this option is omitted, convex hull is not used.