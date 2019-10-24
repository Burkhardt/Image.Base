# RaiImage

Classes to manage image files in directory trees (within dropbox or outside, windows or macOS or linux).

## namespace

RaiImage

## purpose

simplifies the access of images in a file system.

- ImageFile
- ImageTreeFile
- ImageTypes
- Pane
- Panes
- Src

## example

    var count = ImageTreeFile.MoveToTree(
                fromDir: p["from"], 
                toDirRoot: p["to"], 
                filter: p["filter"], 
                remove: p["remove"]); 
    Console.WriteLine($"{count} files moved.");
