#Error codes

Each error when using the Wrist console has an error code, starting with WTE. Please refer to this file for an explanation of the error.

## WTE00001

Input folder not found.

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the input folder is not found in the file system.

## WTE00002

Input folder not found.

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the name of the home page is empty. It must be the name of one of the pages defined in the 'page' subdirectory of the input folder.

## WTE00003

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process the input folder.

## WTE00004

Unexpected folder 'x'.

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the input folder contains a subfolder that is neither 'page', 'area'... and cannot be processed. 

## WTE00005

Missing folder 'x'.

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the input folder is missing one of the expected subfolders: 'page', 'area'... and cannot continue.

## WTE00006

Home page 'x' not found.

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the home page name does not match one of the folders found in the 'page' subfolder of the input folder.

## WTE00007

Color theme 'x' not found.

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the color theme name does not match one of the folders found in the 'color' subfolder of the input folder.

## WTE00008

Unexpected error during processing of the input folder.

This should never happen, please report it as a bug.

## WTE00009

Layout specified for area 'x' but this area isn't used in page 'y'.

Each page description file must contain the following line:

*default area layout*: `area`=`layout`, ... , `area`=`layout`

where each area is associated to a layout. 

In this case, one of the areas associated to a layout is actually not used in the page. The `area`=`layout` declaration should be removed for that area.


