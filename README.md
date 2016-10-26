# pdfk
A wrapper library of iTextSharp for drawing graphs.  
Drawing graphs to pdf in a ***"WYTIWYT"***(What You Think Is What You Get) way. 

# Dependency
iTextSharp: <https://github.com/itext/itextsharp>    
Newtonsoft Json: <http://www.newtonsoft.com/json>

The respective dll files should be placed at ***./lib*** directory.

# Usage
    Usage: pdfk.exe fonts_dir style graphics output
    
* **fonts_dir:**    
When you want to use fonts more than embedded, you can specify directory contains font files(such as .ttf files).
You need to manually register those fonts in ***./FontFactory.cs***.
This parameter is ignored by default.

* **style:**
Path to style json file.

* **graphics:**
Path to graphics json file.

* **output:**
Path to output pdf file.


# Sample
Sample is used to demonstrate the formats of style and graphics json files.
This sample is from "Rainbow over the Windows"(Ruxcon 2016 topic presented by [zer0mem](<https://github.com/zer0mem>) and me).
After building this project, you can run ***./sample/ruxcon_RotW_graph.py*** to create sample pdf file.

# Format
## style json format
Block tracks a group of graphics operators, these operators inside a block will be rendered using this block's coordinates. 

A block's coordinates should be specified at style json file. 
* **page**: this block will rendered in which page. Available options include "all", "none", "odd", "even", "[index]", "[start_index]:[end_index]".
* **x**: left boundary coordinate in its parent block, value is specified in units, each unit is ***1/PageProperty.RESOLUTION*** of its parent block's width in horizontal direction.
* **y**: bottom boundary coordinate in its parent block, value is specified in units, each unit is ***1/PageProperty.RESOLUTION*** of its parent block's height in vertical direction.
* **width**: width coordinate in its parent block, value is specified in horizontal units.
* **height**: height coordinate in its parent block, value is specified in vertical units.
* **rows**: this block will be divided into **rows** cells equally in vertical direction.
* **columns**: this block will be divided into **columns** cells equally in horizontal direction.

A special **main** style can be used in style json file, it will define global options for document.
* **pages**: how many pages contained in this document.
* **width**: width of document page, in pixels.
* **height**: height of document page, in pixels.
* **show_page**: 1 for showing page numbers at the bottom of each page, 0 for hiding.

## graphics json format
Graphics json file defines the graphics operators for drawing each block.

Each graphics operator is in the following format:
        
        <opr_name>(params_1, params_2, ...);
        
Each block can have **M** cells(rows * columns), and those graphics operators can be spread into **N** separated strings, and those strings will be fitted into **M** cells in such a way:

        cells_content = [None] * M
        for idx in range(N):
            cells_content[idx % M] = strings[N]
            
The available built-in graphics operators are:
* **fill(red, green, blue)**: set RGB color for fill.
* **fill()**: fill current path.
* **stroke(red, green, blue)**: set RGB color for stroke.
* **stroke()**: stroke current path.
* **Bg(red, green, blue)**: set RGB background color.
* **BT()**: begin text.
* **ET()**: end text.
* **Tj(content)**: show text.
* **Tf(font, size)**: set font name and font size.
* **Tr(render_mode)**: set text rendering mode.
* **Td(x, y)**: set text start position.
* **w(stroke_width)**: set stroke width.
* **Tc(character_spacing)**: set character spacing.
* **Tw(word_spacing)**: set word spacing.
* **TL(leading)**: set leading.
* **Tz(horizontal_scaling)**: set horizontal scaling.
* **Ts(text_rise)**: set text rise.
* **J(line_cap)**: set line cap.
* **j(line_join)**: set line join.
* **M(miter_limit)**: set miterlimit.
* **dash(dash_unit)**: set dash in units, if parameter is 0, then no dash.
* **moveTo(x, y)**: move to (x, y).
* **lineTo(x, y)**: line from current position to (x, y).
* **rect(x, y, width, height)**: draw a rectangle.
* **line(x1, y1, x2, y2)**: line from (x1, y1) to (x2, y2).
* **grid(x, y, width, height, columns, rows)**: draw grids.
* **curveTo(x1, y1, x2, y2, x3, y3)**: triple besier curve from current position to (x3, y3), with handle point (x1, y1) and (x2, y2).
* **fs()**: fill and stroke().
* **closePath()**: close current path.
* **saveState()**: save graphics state.

Those graphics operators are all pretty strait-forward, they are just some simple wrappers of pdf built-in operators(More information in [PDF Manual](<http://wwwimages.adobe.com/content/dam/Adobe/en/devnet/pdf/pdfs/PDF32000_2008.pdf>))

A new-defined block can also be referenced by other blocks within graphics json:
      

        "outline":
      	[
      	    "line(0, 0, 0, 10000);",
      	    "line(10000, 0, 10000, 10000);",
      	    "line(0, 0, 10000, 0);",
      	    "line(0, 10000, 10000, 10000);",
      	],
      	"text_1":
      	[
      	    "outline();BT();Tf(\"courier_b\", 4000);Td(1000, 3000);Tj(\"I am surrounded by outline!\");ET();"
      	]
      	
But now no parameters can be passed to a new-defined block, so it only works in **MACRO** way.
            