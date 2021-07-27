# Collection-Of-Algorithms
This WPF app is a fun project to visualize interesting algorithms.

## Algorithms
Here are the algorithms that are used and how they work in this app.

##### [Cellular automaton](https://en.wikipedia.org/wiki/Cellular_automaton)
![Cellular Automaton](https://i.imgur.com/NbbCUsy.png)

In this app, you can generate a grid filled with cells that are determined via the probabilities of the different colored cells. The image that this generates is a 300x300 bitmap image with **Grid Size** determining how many cells are the in one line. 

Once every parameter is set, you can hit **Generate** to generate a grid of cells.
If you hit **Refine** the cells will change depending on the rules (might make it so the user can set their custom rules).
The **Auto Refine** check box, when checked, will refine the image until there's no more change between the current and previous generation.
If show grid is not checked the black borders around the cells will dissapear when you hit **Generate** .

#### Pattern Graph

This algorithm visualizes if a word repeats in a text. The visual representation of a .txt file is mirrored on the diagonal.
**Browse** will one a dialog box to select a .txt file to read from (max. 300 words seperated by space).
**Generate** will generate the repetition visualization.

**Show Grid** will draw the grid around the cells, though it will obscure the visualization if the text is long.
**Dark Mode** inverts the colors of the visualization.
(Both of these settings are applied when the **Generate** button is pressed)

#### [Mandelbrot Set](https://en.wikipedia.org/wiki/Mandelbrot_set)

![Mandelbrot](https://i.imgur.com/8N5TwEE.png)

You can generate and save a visualization of the mandelbrot set (mandelbrot fractal). In the app, the mandelbrot set is generated at max 300x300 bitmap image while you can save at max 2048x2048 bitmap image. 
