Error codes

Each error when using the Wrist console has an error code, starting with WTE. Please refer to this file for an explanation of the error.

## WTE00001

*Input folder not found.*

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the input folder is not found in the file system.

## WTE00002

*Input folder not found.*

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the name of the home page is empty. It must be the name of one of the pages defined in the 'page' subdirectory of the input folder.

## WTE00003

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process the input folder.

## WTE00004

*Unexpected folder 'x'.*

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the input folder contains a subfolder that is neither 'page', 'area'... and cannot be processed. 

## WTE00005

*Missing folder 'x'.*

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the input folder is missing one of the expected subfolders: 'page', 'area'... and cannot continue.

## WTE00006

*Home page 'x' not found.*

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the home page name does not match one of the folders found in the 'page' subfolder of the input folder.

## WTE00007

*Color theme 'x' not found.*

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

In this case, the color theme name does not match one of the folders found in the 'color' subfolder of the input folder.

## WTE00008

Unexpected error during processing of the input folder.

This should never happen, please report it as a bug.

## WTE00009

*Layout specified for area 'x' but this area isn't used in page 'y'.*

Each page description file must contain the following line:

*default area layout*: `area`=`layout`, ... , `area`=`layout`

where each area is associated to a layout. 

In this case, one of the areas associated to a layout is actually not used in the page. The `area`=`layout` declaration should be removed for that area.

## WTE00010

*Area 'x' has not layout specified.*

Each page description file must contain the following line:

*default area layout*: `area`=`layout`, ... , `area`=`layout`

where each area is associated to a layout. 

In this case, one of the areas is used in a page but has no associated layout. Some `area`=`layout` declaration should be added for that area.

## WTE00011

*Translation key used in area 'x' but no translation file provided.*

or

*Object 'translation' is used but no translation file is specified.*

Some component in a file describing an area uses the `translation.strings[key]` syntax, to take advantage of the built-in internationalization system. However, this is only possible if a file called `translation.cvs` and containing translated sentences is found in the input folder.

In this case, the file was not found.

## WTE00012

*Translation key for page 'x' used in area 'y' not found.*

Some component in a file describing an area uses the `translation.strings[key]` syntax, to take advantage of the built-in internationalization system. However, this is only possible if a file called `translation.cvs` and containing translated sentences is found in the input folder.

Keys for page names can be specified using the special <current page> key, and the corresponding key in `translation.cvs` is page-`pagename`. For example, for page `home` the key should be 'page-home'.

In this case, the file was found, but doesn't contain the expected 'page-`pagename`' key.

## WTE00013

*DockPanel.Dock specified for x not included in a DockPanel.*

where *x* is some layout object, such as `StackPanel`, `Grid`, or `Control`.

Each layout object within a DockPanel can specify how it should be docked with the `DockPanel.Dock` attached property. For example, `DockPanel.Dock="Bottom"`. 

In this case, the attached property is specified but the layout object doesn't belong to a DockPanel. The attached property specification doesn't make sense, and should be removed, or the layout object moved into the proper DockPanel.

## WTE00014

*Grid.Column specified for x not included in a Grid.*

where *x* is some layout object, such as `StackPanel`, `Grid`, or `Control`.

Each layout object within a Grid can specify its row and column with the `Grid.Row` and `Grid.Column` attached properties. For example, `Grid.Column="2"`. 

In this case, the attached property is specified but the layout object doesn't belong to a Grid. The attached property specification doesn't make sense, and should be removed, or the layout object moved into the proper Grid.

## WTE00015

*Grid.Row specified for x not included in a Grid.*

where *x* is some layout object, such as `StackPanel`, `Grid`, or `Control`.

Each layout object within a Grid can specify its row and column with the `Grid.Row` and `Grid.Column` attached properties. For example, `Grid.Row="2"`. 

In this case, the attached property is specified but the layout object doesn't belong to a Grid. The attached property specification doesn't make sense, and should be removed, or the layout object moved into the proper Grid.

## WTE00016

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the subfolders of the input folder.

## WTE00017

*Empty name not valid.*

In various files used by the Wrist framework a name can be expected. This error happens if no name is provided. The line where the name is expected is displayed in the error message.

## WTE00018

*'{name}' only contains invalid characters.*

In various files used by the Wrist framework a name can be expected. The name can contain exotic characters and these are replaced with an underscore to make it a valid C#/Xaml name.

This error happens if the name is made only of underscore after all exotic characters are replaced. It can be fixed by changing them or adding some alphanumeric character to the name.

## WTE00019

*Unexpected empty line.*

In various files used by the Wrist framework a `key`=`value` pair can be expected. This error happens if the line where the key and value are expected is empty. The file where the line is expected is displayed in the error message.

## WTE00020

*<key>=<value> expected.*

or

*<key>:<value> expected.*


In various files used by the Wrist framework a `key`=`value` or `key`:`value` pair can be expected. This error happens if the line where the key and value are expected does not contain the separator (`=` or `:`) sign. The line where it is expected is displayed in the error message.

## WTE00021

*<key>=<value> expected, found empty key.*

or

*<key>:<value> expected, found empty key.*


In various files used by the Wrist framework a `key`=`value` or `key`:`value` pair can be expected. This error happens if the line, where the key and value are expected, contains the separator (`=` or `:`) sign, but would be interpreted as a pair with an empty key. The line where the pair is expected is displayed in the error message.

## WTE00022

*<key>=<value> expected, found empty value.*

or

*<key>:<value> expected, found empty value.*


In various files used by the Wrist framework a `key`=`value` or `key`:`value` pair can be expected. This error happens if the line, where the key and value are expected, contains the separator (`=` or `:`) sign, but would be interpreted as a pair with an empty value. The line where the pair is expected is displayed in the error message.

## WTE00023

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'... and some of these contain Xaml files.

This error is issued when the console fails to process one of the Xaml files.

## WTE00024

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'... and some of these contain Xaml files.

This error is issued when the console fails to process one of the files in the 'area' subfolder.

## WTE00025

*Component type expected.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

In this case, `Type` has not been found.

## WTE00026

*Unknown component type 'x'.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

In this case, `Type` has been found, but is not one of the supported values (`area`, `text`, `button`...)

## WTE00027

*Unknown token 'x'.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

In this case, `Key` has been found, but is not one of the supported values (`name` for instance). Each component type support different key tokens.

## WTE00028

*'x' is repeated.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

In this case, `Key` has been found more than once. Each component type support different key tokens, but they should be specified only once at most.

## WTE00029

*Area name not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *area*, the key `name` must be specified and have as value a simple, non-empty string.

In this case, the `name` key was not specified.

## WTE00030

*Area name not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *area*, the key `name` must be specified and have as value a simple, non-empty string.

In this case, the `name` key was specified but using a complex syntax such as `object`.`property`.

## WTE00031

*Button content not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *button*, keys `content` and `goto` must be specified, and `goto` must have as value a simple, non-empty string.

In this case, the `content` key was not specified.

## WTE00032

*Button goto page name not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *button*, keys `content` and `goto` must be specified, and `goto` must have as value a simple, non-empty string.

In this case, the `goto` key was not specified.

## WTE00033

*Button goto page name not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *button*, keys `content` and `goto` must be specified, and `goto` must have as value a simple, non-empty string.

In this case, the `goto` key was specified but using a complex syntax such as `object`.`property`.

## WTE00034

*CheckBox content not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *checkbox*, keys `content` and `checked` must be specified, and `checked` must use the `object`.`property` syntax. 

In this case, the `content` key was not specified.

## WTE00035

*CheckBox checked property not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *checkbox*, keys `content` and `checked` must be specified, and `checked` must use the `object`.`property` syntax. 

In this case, the `checked` key was not specified.

## WTE00036

*Checked property cannot be a static name.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *checkbox*, keys `content` and `checked` must be specified, and `checked` must use the `object`.`property` syntax. 

In this case, the `checked` key was specified, but using a simple string instead of the expected `object`.`property` syntax.

## WTE00037

*Checked property cannot use a key.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *checkbox*, keys `content` and `checked` must be specified, and `checked` must use the `object`.`property` syntax. 

In this case, the `checked` key was specified, but using the invalid `object`.`property`[`key`] syntax.

## WTE00038

*Text not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *text*, the `text` key must be specified, and if `decoration` is specified, it must have as value a simple, non-empty string.

In this case, the `text` key was not specified.

## WTE00039

*Decoration can only be a constant.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *text*, the `text` key must be specified, and if `decoration` is specified, it must have as value a simple, non-empty string.

In this case, the `decoration` key was specified, but is not a simple, non-empty string.

## WTE00040

*Invalid decoration for 'x'.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *text*, the `text` key must be specified, and if `decoration` is specified, it must have as value a simple, non-empty string.

In this case, the `decoration` key was specified, but the only supported values are *OverLine*, *Strikethrough* and *Underline*.

## WTE00041

*Text not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `text` key was not specified.

## WTE00042

*Text must be a string property.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `text` key was specified, but not with a value using the `object`.`property` syntax.

## WTE00043

*The only valid value for the 'accepts return' property is 'Yes'.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `accepts return` key was specified, but not with the only supported value *Yes*.

## WTE00044

*Decoration can only be a constant.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `decoration` key was specified, but not as one of the supported values.

## WTE00045

*Horizontal scrollbar can only be a constant.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `horizontal scrollbar` key was specified, but not as one of the supported values.

## WTE00046

*Vertical scrollbar can only be a constant.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `vertical scrollbar` key was specified, but not as one of the supported values.

## WTE00047

*Invalid decoration for 'x'.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `decoration` key was specified, but the only supported values are *OverLine*, *Strikethrough* and *Underline*.

## WTE00048

*Invalid horizontal scrollbar for 'x'.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `horizontal scrollbar` key was specified, but the only supported values are *Disabled*, *Auto*, *Hidden* and *Visible*.

## WTE00049

*Invalid vertical scrollbar for 'x'.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *edit*, the `text` key must be specified with a value using the `object`.`property` syntax, and other optional keys can only take some very specific values.

In this case, the `vertical scrollbar` key was specified, but the only supported values are *Disabled*, *Auto*, *Hidden* and *Visible*.

## WTE00050

*Text not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *password edit*, the `text` key must be specified with a value using the `object`.`property` syntax.

In this case, the `text` key was not specified.

## WTE00051

*Text must be a string property.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *password edit*, the `text` key must be specified with a value using the `object`.`property` syntax.

In this case, the `text` key was specified, but not with a value using the `object`.`property` syntax.

## WTE00052

*Source not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *image*, the `source` key must be specified, as well a `width` and `height` key that must be integer constants.

In this case, the `source` key was not specified.

## WTE00053

*Width not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *image*, the `source` key must be specified, as well a `width` and `height` key that must be integer constants.

In this case, the `width` key was not specified.

## WTE00054

*Width can only be a static value.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *image*, the `source` key must be specified, as well a `width` and `height` key that must be integer constants.
If `Type` is *popup*, the `width` and `height` key can be specified but they must be integer constants.

In this case, the `width` key was specified, but is not an integer constant.

## WTE00055

*Height not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *image*, the `source` key must be specified, as well a `width` and `height` key that must be integer constants.

In this case, the `height` key was not specified.

## WTE00056

*Height can only be a static value.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *image*, the `source` key must be specified, as well a `width` and `height` key that must be integer constants.
If `Type` is *popup*, the `width` and `height` key can be specified but they must be integer constants.

In this case, the `height` key was specified, but is not an integer constant.

## WTE00057

*Source not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *popup*, the `source` and `area` keys must be specified, and must be simple non-empty strings.

In this case, the `source` key was not specified.

## WTE00058

*Area not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *popup*, the `source` and `area` keys must be specified, and must be simple non-empty strings.

In this case, the `area` key was not specified.

## WTE00059

*Area name can only be a static name.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *popup*, the `source` and `area` keys must be specified, and must be simple non-empty strings.

In this case, the `area` key was specified but is not a simple, non-empty string.

## WTE00060

*Index not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *selector*, the `index` and `items` keys must be specified, both using the `object`.`property` syntax.

In this case, the `index` key was not specified.

## WTE00061

*Index must be an integer property.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *selector*, the `index` and `items` keys must be specified, both using the `object`.`property` syntax.

In this case, the `index` key was specified but not using the `object`.`property` syntax.

## WTE00062

*Items not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *selector*, the `index` and `items` keys must be specified, both using the `object`.`property` syntax.

In this case, the `items` key was not specified.

## WTE00063

*Items must be a list property.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *selector*, the `index` and `items` keys must be specified, both using the `object`.`property` syntax.

In this case, the `items` key was specified but not using the `object`.`property` syntax.

## WTE00064

*Index not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *index*, the `index` key must be specified, using the `object`.`property` syntax.

In this case, the `index` key was not specified.

## WTE00065

*Index must be an integer, state or boolean property.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *index*, the `index` key must be specified, using the `object`.`property` syntax.

In this case, the `index` key was specified, but not using the `object`.`property` syntax.

## WTE00066

*Item not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *container*, the `item` key must be specified, as well as the `area` key that must be a simple, non-empty string.

In this case, the `item` key was not specified.

## WTE00067

*Area not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *container*, the `item` key must be specified, as well as the `area` key that must be a simple, non-empty string.

In this case, the `area` key was not specified.

## WTE00068

*Area name can only be a static name.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *container*, the `item` key must be specified, as well as the `area` key that must be a simple, non-empty string.

In this case, the `area` key was specified, but is not a simple, non-empty string.

## WTE00069

*Items not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *container list*, the `items` key must be specified, as well as the `area` key that must be a simple, non-empty string.

In this case, the `items` key was not specified.

## WTE00070

*Area not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *container list*, the `items` key must be specified, as well as the `area` key that must be a simple, non-empty string.

In this case, the `area` key was not specified.

## WTE00071

*Area name can only be a static name.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *container list*, the `items` key must be specified, as well as the `area` key that must be a simple, non-empty string.

In this case, the `area` key was specified, but is not a simple, non-empty string.

## WTE00072

*Radio button content not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *radio button*, the `content`, `index`, `group name` and `group index` keys must be specified, with the `index` key using the `object`.`property` syntax, the `group name` key a simple non-empty string and the `group index` key an integer constant.

In this case, the `content` key was not specified.

## WTE00073

*Index not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *radio button*, the `content`, `index`, `group name` and `group index` keys must be specified, with the `index` key using the `object`.`property` syntax, the `group name` key a simple non-empty string and the `group index` key an integer constant.

In this case, the `index` key was not specified.

## WTE00074

*Index must be an integer, state or boolean property.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *radio button*, the `content`, `index`, `group name` and `group index` keys must be specified, with the `index` key using the `object`.`property` syntax, the `group name` key a simple non-empty string and the `group index` key an integer constant.

In this case, the `index` key was specified, but not using the `object`.`property` syntax.

## WTE00075

*Group name not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *radio button*, the `content`, `index`, `group name` and `group index` keys must be specified, with the `index` key using the `object`.`property` syntax, the `group name` key a simple non-empty string and the `group index` key an integer constant.

In this case, the `group name` key was not specified.

## WTE00076

*Group name can only be a static name.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *radio button*, the `content`, `index`, `group name` and `group index` keys must be specified, with the `index` key using the `object`.`property` syntax, the `group name` key a simple non-empty string and the `group index` key an integer constant.

In this case, the `group name` key was specified, but is not a simple non-empty string.

## WTE00077

*Group index not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *radio button*, the `content`, `index`, `group name` and `group index` keys must be specified, with the `index` key using the `object`.`property` syntax, the `group name` key a simple non-empty string and the `group index` key an integer constant.

In this case, the `group index` key was not specified.

## WTE00078

*Group index not specified.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *radio button*, the `content`, `index`, `group name` and `group index` keys must be specified, with the `index` key using the `object`.`property` syntax, the `group name` key a simple non-empty string and the `group index` key an integer constant.

In this case, the `group index` key was specified, but is not an integer constant.

## WTE00079

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the background files.

## WTE00080

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the color theme files.

## WTE00081

*Color 'x' defined more than once.*

A color theme file contains lines that associate a color name to a color, using the `color name`: `color` syntax. The `color name` is used to group identical colors with a recognizable name, and `color` indicate the true color used, either using a #RGB syntax or naming the color.

In this case, there was two lines specifying the same `color name`.

## WTE00082

*Missing 'Button' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *Button* style was not provided.

## WTE00083

*Missing 'TextBox' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *TextBox* style was not provided.

## WTE00084

*Missing 'Image' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *Image* style was not provided.

## WTE00085

*Missing 'ListBox' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *ListBox* style was not provided.

## WTE00086

*Missing 'TextBlock' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *TextBlock* style was not provided.

## WTE00087

*Missing 'RadioButton' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *RadioButton* style was not provided.

## WTE00088

*Missing 'CheckBox' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *CheckBox* style was not provided.

## WTE00089

*Missing 'PasswordBox' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *PasswordBox* style was not provided.

## WTE00090

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the design Xaml files.

## WTE00091

This error should never happen. Please report it as a bug.

## WTE00092

*Key 'x' found multiple times.*

Xaml files used in a design can be split in separate files, for example to group all resources used in one style. However, they must be compatible with each other.

In this case, a resource with a key defined by `x:Key="x"` was found more than once.

## WTE00093

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the layout Xaml files.

## WTE00094

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the object files.

## WTE00095

*'properties' expected.*

An object file describes the object properties and events.

This error is issued if the file doesn't begin with `properties`.

## WTE00096

*Object already contains a property called 'x'.*

An object file describes the object properties and events.

This error is issued if the file declares two or more properties with the same name.

## WTE00097

*Maximum length specified more than once.*

An object file describes the object properties and events.

This error is issued if the file declares a property with the `maximum length` specifier more than once.

## WTE00098

*Invalid empty object name.*

An object file describes the object properties and events.

This error is issued if the file declares a property of the *object* kind, but with an empty object type name.

## WTE00099

*Object name specified more than once.*

An object file describes the object properties and events.

This error is issued if the file declares a property with the `object` specifier more than once.

## WTE00100

*Unknown specifier 'x'.*

An object file describes the object properties and events.

This error is issued if the file declares a property but with an unknown specifier.

## WTE00101

*Specifiers 'maximum length' not valid for this property type.*

An object file describes the object properties and events.

This error is issued if the file declares a property with the `maximum length` specifier, but this specifier is not supported for the property type.

## WTE00102

*Object name not specified for 'item'.*

An object file describes the object properties and events.

This error is issued if the file declares a property of type *item*, but without the `object` specifier that would provide the item object type.

## WTE00103

*Object name not specified for 'items'.*

An object file describes the object properties and events.

This error is issued if the file declares a property of type *items*, but without the `object` specifier that would provide the item list object type.

## WTE00104

*Unknown property type 'x'.*

An object file describes the object properties and events.

This error is issued if the file declares a property of an unknown type.

## WTE00105

*'events' expected.*

An object file describes the object properties and events.

This error is issued if the file doesn't contain a line with `events` after properties have been declared.

## WTE00106

*Event name 'x' specified more than once.*

An object file describes the object properties and events.

This error is issued if the file declares more than one events with the same name.

## WTE00107

*Event name cannot be empty.*

An object file describes the object properties and events.

This error is issued if the file declares an event with an empty name.

## WTE00108

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the page files.

## WTE00109

*Missing area name.*

A page description file must include the name of the root area, a list of layouts to associate to each nested area, the design to use, the page width and height, an optional background style and a background color. 

This error is issued if the file does not specify the root area name.

## WTE00110

*Missing default area layout.*

A page description file must include the name of the root area, a list of layouts to associate to each nested area, the design to use, the page width and height, an optional background style and a background color. 

This error is issued if the file does not specify the list of layouts to associate to each nested area.

## WTE00111

*Missing design name.*

A page description file must include the name of the root area, a list of layouts to associate to each nested area, the design to use, the page width and height, an optional background style and a background color. 

This error is issued if the file does not specify the design to use.

## WTE00112

*Missing width.*

A page description file must include the name of the root area, a list of layouts to associate to each nested area, the design to use, the page width and height, an optional background style and a background color. 

This error is issued if the file does not specify the page width.

## WTE00113

*Missing height.*

A page description file must include the name of the root area, a list of layouts to associate to each nested area, the design to use, the page width and height, an optional background style and a background color. 

This error is issued if the file does not specify the page height.

## WTE00114

*Missing background color.*

A page description file must include the name of the root area, a list of layouts to associate to each nested area, the design to use, the page width and height, an optional background style and a background color. 

This error is issued if the file does not specify the color name for the page background.

## WTE00115

*Specifier 'x' was unexpected.*

A page description file must include the name of the root area, a list of layouts to associate to each nested area, the design to use, the page width and height, an optional background style and a background color. 

This error is issued if the file contains another, unknown specifier.

## WTE00116

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'...

This error is issued when the console fails to process one of the resource files.

## WTE00117

*Unknown area 'x'.*

This error happens when a component refers to an area that has not been declared in area files.

## WTE00118

*Unknown area 'x'.*

This error happens when a page specifies a root area that has not been declared in area files.

## WTE00119

*Unknown area 'x'.*

This error happens when a page specifies the layout for an area, but that area has not been declared in area files.

## WTE00120

*Unknown layout 'x'.*

This error happens when a page specifies the layout for an area, but that layout has not been declared in layout files.

## WTE00121

*Unknown design 'x'.*

This error happens when a page specifies a design that has not been declared in design files.

## WTE00122

*Unknown background 'x'.*

This error happens when a page specifies a background that has not been declared in background files.

## WTE00123

*'x' not parsed as a width.*

This error happens when a page specifies a background and a width that is not an integer constant.

## WTE00124

*Only valid width when no background is 'stretch'.*

This error happens when a page specifies no background but width that is not `stretch`.

## WTE00125

*Specifier 'x' found more than once.*

This error happens when a page contains the same specifier more than once. Remove all but one occurence to fix it.

## WTE00126

*'x' not parsed as a height (only valid values are integer constants or 'infinite').*

This error happens when a page specifies a height that is neither `infinite` or an integer constant.

## WTE00127

*Background color 'x' not found in color theme 'y'.*

This error happens when a page specifies a background color and a color theme that does not contain this color name.

## WTE00128

*'x' not parsed as a width.*

This error happens when an image component specifies a width that is not an integer constant or, in the case of *image*, `resource`.

## WTE00129

*'x' not parsed as a height.*

This error happens when an image component specifies a height that is not an integer constant or, in the case of *image*, `resource`.

## WTE00130

*Radio button 'x' not referenced in a page.*

Radio buttons must appear in multiples (a single radio button doesn't make sense) and in the same page. They are grouped using a common group name, and each radio button in a group must have its own unique index.
 
This error happens when a radio button is declared in an area this is not used by any page.

## WTE00130

*Radio button 'x' not referenced in a page.*

Radio buttons must appear in multiples (a single radio button doesn't make sense) and in the same page. They are grouped using a common group name, and each radio button in a group must have its own unique index.
 
This error happens when a radio button is declared in an area this is not used by any page.

## WTE00131

*Group name 'x' is only referencing one radio button.*

Radio buttons must appear in multiples (a single radio button doesn't make sense) and in the same page. They are grouped using a common group name, and each radio button in a group must have its own unique index.
 
In this case, a radio button has a group name that indicates it's the only radio button of its group.

## WTE00132

*Another radio button of the same group name 'x' is using this index.*

Radio buttons must appear in multiples (a single radio button doesn't make sense) and in the same page. They are grouped using a common group name, and each radio button in a group must have its own unique index.
 
In this case, another radio button of the same group is sharing this index.

## WTE00133

*Invalid type for property 'x'.*

This error happens when a text component is referencing an object property but that property is of an incompatible type.

## WTE00134

*Events must use the Object.Event syntax.*

For components that raise events (such as buttons), the event must be specified for a corresponding object.

In this case, the component doesn't use the proper syntax to declare the event to rise.

## WTE00135

*Unknown object 'x'.*

For components that raise events (such as buttons), the event must be specified for a corresponding object.

In this case, the component declares an event but the corresponding object cannot be found.

## WTE00136

*Unknown event 'x'.*

For components that raise events (such as buttons), the event must be specified for a corresponding object.

In this case, the component declares an event but the corresponding object does not declare it.

## WTE00137

*The only valid property for object 'translation' is 'strings'.*

The *translation* object is a special object with only one string dictionary property, *strings*, and no event.

In this case, a component is trying to use another property than *strings*.

## WTE00138

*The only valid property for object 'translation' is 'strings'.*

The *translation* object is a special object with only one string dictionary property, *strings*, and no event.

In this case, a component is trying to use the *strings* without a dictionary key.

## WTE00139

*The translation file doesn't contain key 'x'.*

The *translation* object is a special object with only one string dictionary property, *strings*, and no event.

In this case, a component is trying to use the *strings* with a dictionary key that is not found in *translation.cvs*.

## WTE00140

*Unknown object '{objectSource.Name}'.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the object could not be found.

## WTE00141

*Unknown property 'x' in object 'y'.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the object doesn't declare a property with this name.

## WTE00142

*Unknown static resource 'x'.*

This error happens when a component refers to a resource, but this resource has not been declared.

## WTE00143

*'x.y' must be used with a key.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is a dictionary and the compontent doesn't use a key.

## WTE00144

*'x.y' cannot be used with a key.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not a dictionary and the compontent is trying to use a key.

## WTE00145

*'x.y' must be a string list property.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not a string list and only this type of property is supported by the compontent (a selector *items* property for instance).

## WTE00146

*'x.y' must be an integer property.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not an integer property and only this type of property is supported by the compontent (a selector *index* property for instance).

## WTE00147

*'x.y' must be a boolean property.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not a boolean property and only this type of property is supported by the compontent (a checkbox *checked* property for instance).

## WTE00148

*'x.y' must be a string property.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not a string property and only this type of property is supported by the compontent (an edit *text* property for instance).

## WTE00149

*'x.y' must be an item property.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not an item property and only this type of property is supported by the compontent (an container *item* property for instance).

## WTE00150

*Unknown object 'x' for item 'y'.*

This error happens when a component refers to an item property but the object for the item type has not been declared.

## WTE00151

*'x.y' must be an item list property.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not an item list property and only this type of property is supported by the compontent (an container list *items* property for instance).

## WTE00152

*Unknown object 'x' for item list 'y'.*

This error happens when a component refers to an item list property but the object for the item type has not been declared.

## WTE00153

*'x.y' must be an integer, enum or boolean property.*

This error happens when a component refers to a property using the `object`.`property` syntax, but the property is not convertible to an integer and only this type of property is supported by the compontent (a radio button *checked* property for instance).

## WTE00154

*Area 'x' used in two different contexts.*

Areas can be used to specify what's displayed in a page or a part of a page, but also to specify what's displayed in an object, typically within a container or container list component. The same area can only be used in one of these contexts, and not both.
   
In this case, both a page and container or container list component try to use the same area.

## WTE00155

*Area 'x' used in two different contexts.*

Areas can be used to specify what's displayed in a page or a part of a page, but also to specify what's displayed in an object, typically within a container or container list component. The same area can only be used in one of these contexts, and not both.
   
In this case, two containers or containers list component try to use the same area, but they refer to different objects.

## WTE00156

*Control is referencing 'x' but this name doesn't exist.*

This error is issued when a Control in a layout file refers to the name of a component, but the area associated to this layout doesn't not declare a component with this name.

## WTE00157

*Invalid wrapping for 'x'.*

This error is issued when a Control in a layout file specifies a wrapping that is neither *Wrap* or *NoWrap*.

## WTE00158

*Unknown dock value 'x'.*

This error is issued when a Control in a layout file specifies a docking that is neither *left*, *right*, *top* or *NoWrap*.

## WTE00159

This error should never be issued, please report it as a bug.

## WTE00160

*Dock value already specified for this element.*

This error is issued when a Control in a layout file specifies more than one docking. 

## WTE00161

*Unknown Column value 'x'.*

This error is issued when a Control in a layout file specifies a column that is not an integer constant.

## WTE00162

This error should never be issued, please report it as a bug.

## WTE00163

*Column value already specified for this element.*

This error is issued when a Control in a layout file specifies more than one column. 

## WTE00164

*Unknown Row value 'x'.*

This error is issued when a Control in a layout file specifies a row that is not an integer constant.

## WTE00165

This error should never be issued, please report it as a bug.

## WTE00166

*Row value already specified for this element.*

This error is issued when a Control in a layout file specifies more than one row. 

## WTE0016

*Invalid column count.*

This error is issued when a Grid in a layout file specifies an invalid ColumnCount. 

## WTE00168

*Invalid row count.*

This error is issued when a Grid in a layout file specifies an invalid RowCount. 

## WTE00169

*Too many values in 'x', expected at most y.*

This error is issued when a Grid in a layout file specifies a set of widths or heights for columns and rows, but it specifies more than the Grid has columns or rows. 

## WTE00170

*'x' not parsed as a column width.*

or

*'x' not parsed as a row height.*


This error is issued when a Grid in a layout file specifies a set of widths or heights for columns and rows, but one of the specifiers is neither a number constant or `auto`. 

## WTE00171

*StatePanel is referencing 'x' but this index doesn't exist.*

A StatePanel is used to display one among several controls or panels, based on an indexing component. The indexing component can be for instance a radio button.
 
This error is issued when the indexing component has not been found. 

## WTE00172

*StatePanel is referencing 'x' but this component is not an index.*

A StatePanel is used to display one among several controls or panels, based on an indexing component. The indexing component can be for instance a radio button.
 
This error is issued when the indexing component is of the wrong type. 

## WTE00173

*Incompatible use of event.*

An event is signaled for instance when the user clicks on a button, and is either page-agnostic or provides the name of a page to switch to. These two mods are not compatible.
 
This error is issued when a event is used in both contexts. It can be fixed by having two different events, sharing code. 

## WTE00174

*Object 'x' not found.*

This error is issued when object declares a property of type item, and the name of a object for the item, but this object name is not found.

## WTE00175

*Object 'x' not found.*

This error is issued when object declares a property of type item list, and the name of a object for each item, but this object name is not found.

## WTE00176

*Unknown page name 'x'.*

This error is issued when a button declares a page name to go to on a click, but this page isn't declared.

## WTE00177

*A custom page must be set with a 'before' event.*

This error is issued when a button declares that it will go to a custom page on a click, but doesn't declare a 'before' event to provide the page name.

## WTE00178

*The translation file is expected to have a header.*

A translation file translation.cvs, found at the root of the input folder, should contain:
- A header, with Key in the first column and the name in subsequent columns, separated by a tab character.
- Lines of sentences (separated by a tab character), except for the first column that contains a key.

This error is issued when the header is not recognized.

## WTE00179

*The translation file is expected to have a header starting with 'Key' for the first column.*

A translation file translation.cvs, found at the root of the input folder, should contain:
- A header, with Key in the first column and the name in subsequent columns, separated by a tab character.
- Lines of sentences (separated by a tab character), except for the first column that contains a key.

This error is issued when the header is recognized but the first column doesn't contain `Key`.

## WTE00180

*The translation file is expected to have the name of a language at the header of column N.*

A translation file translation.cvs, found at the root of the input folder, should contain:
- A header, with Key in the first column and the name in subsequent columns, separated by a tab character.
- Lines of sentences (separated by a tab character), except for the first column that contains a key.

This error is issued when the header is recognized but the Nth column doesn't contain a language name.

## WTE00181

*Language 'x' found more than once in the header.*

A translation file translation.cvs, found at the root of the input folder, should contain:
- A header, with Key in the first column and the name in subsequent columns, separated by a tab character.
- Lines of sentences (separated by a tab character), except for the first column that contains a key.

This error is issued when the header is recognized but contains a repeated language name.

## WTE00182

*Inconsistent format at line N.*

A translation file translation.cvs, found at the root of the input folder, should contain:
- A header, with Key in the first column and the name in subsequent columns, separated by a tab character.
- Lines of sentences (separated by a tab character), except for the first column that contains a key.

This error is issued when a lines contains less, or more, sentences than languages declared in the header.

## WTE00183

*Invalid key 'x' at line N.*

A translation file translation.cvs, found at the root of the input folder, should contain:
- A header, with Key in the first column and the name in subsequent columns, separated by a tab character.
- Lines of sentences (separated by a tab character), except for the first column that contains a key.

This error is issued when a lines an invalid key in its first column. Valid characters for a key are `a...z`, `A...Z`, `0...9`, `_` and `-`.

## WTE00184

*Translation for key 'x' found at line N but this key already has an entry.*

A translation file translation.cvs, found at the root of the input folder, should contain:
- A header, with Key in the first column and the name in subsequent columns, separated by a tab character.
- Lines of sentences (separated by a tab character), except for the first column that contains a key.

This error is issued when two or more lines have the same key in the first column.

## WTE00185

*Invalid PNG file.*

This error is issued when a .png resource file cannot be process.

## WTE00186

*Source can only be a static name.*

When processing area files, each line is expected to be in the following format:

`Name`: `Type` [, optional `key`=`value` pairs, ...]

where `Name` is the component name, `Type` its type, and there can be zero or more specifiers in the form of `key`=`value` pairs separated by commas.

If `Type` is *popup*, the `source` and `area` keys must be specified, and must be simple non-empty strings.

In this case, the `source` key was specified but is not a simple, non-empty string.

## WTE00187

*Unknown ColumnSpan value 'x'.*

This error is issued when a Control in a layout file specifies a column span that is not an integer constant.

## WTE00188

This error should never be issued, please report it as a bug.

## WTE00189

*ColumnSpan value already specified for this element.*

This error is issued when a Control in a layout file specifies more than one column span. 

## WTE00190

*Unknown RowSpan value 'x'.*

This error is issued when a Control in a layout file specifies a row span that is not an integer constant.

## WTE00191

This error should never be issued, please report it as a bug.

## WTE00192

*RowSpan value already specified for this element.*

This error is issued when a Control in a layout file specifies more than one row span. 

## WTE00193

*Invalid width.*

A control or panel in a layout file can specify a width, height, margin, horizontal alignment, vertical alignment or a dynamic property to enable the control/panel.

In this case, the specified width is invalid.

## WTE00194

*Invalid height.*

A control or panel in a layout file can specify a width, height, margin, horizontal alignment, vertical alignment or a dynamic property to enable the control/panel.

In this case, the specified height is invalid.

## WTE00195

*Invalid margin.*

A control or panel in a layout file can specify a width, height, margin, horizontal alignment, vertical alignment or a dynamic property to enable the control/panel.

In this case, the specified margin is invalid.

## WTE00196

*Invalid horizontal alignment.*

A control or panel in a layout file can specify a width, height, margin, horizontal alignment, vertical alignment or a dynamic property to enable the control/panel.

In this case, the specified horizontal alignment is invalid.

## WTE00197

*Invalid vertical alignment.*

A control or panel in a layout file can specify a width, height, margin, horizontal alignment, vertical alignment or a dynamic property to enable the control/panel.

In this case, the specified vertical alignment is invalid.

## WTE00198

*Dynamic property 'x' not found.*

A control or panel in a layout file can specify a width, height, margin, horizontal alignment, vertical alignment or a dynamic property to enable the control/panel.

In this case, the dynamic property name specified is not found among properties in the dynamics of this page.

## WTE00199

*Dynamic property 'x' is not boolean.*

A control or panel in a layout file can specify a width, height, margin, horizontal alignment, vertical alignment or a dynamic property to enable the control/panel.

In this case, the dynamic property name specified has been found but is not of boolean type.

## WTE00200

*StatePanel has no index.*

A StatePanel is used to display one among several controls or panels, based on an indexing component. The indexing component can be for instance a radio button.
 
This error is issued when there is no indexing component declared. 

## WTE00201

*StatePanel is referencing 'x' but doesn't have the two items expected for a boolean property.*

A StatePanel is used to display one among several controls or panels, based on an indexing component. The indexing component can be for instance a radio button.
 
This error is issued when the indexing component is of the boolean type but there is not exactly two items inside the StatePanel (first one for the value `False`, last one for the value `True`). 

## WTE00202

*StatePanel must have at least two items.*

A StatePanel is used to display one among several controls or panels, based on an indexing component. The indexing component can be for instance a radio button.
 
This error is issued when the StatePanel has less than two items. At least two items are expected since the GUI represent different values over an index range of at least [0,1].

## WTE00203

*Panel 'x' has no item.*
 
This error is issued when a panel (such as a *DockPanel* or a *Grid*) has no item. To represent an empty panel, use the *Empty* component.

## WTE00204

*Invalid max width.*

A panel in a layout file can specify a max width or max height.

In this case, the specified max width is invalid.

## WTE00205

*Invalid max height.*

A panel in a layout file can specify a max width or max height.

In this case, the specified max height is invalid.

## WTE00206

*Missing 'ToggleButton' style.*

A design specifies styles for each of the component used in Wrist: *TextBlock*, *Button*, *TextBox* and so on. If no specific style is be used, it must still be provided for each component, typically with no setter. For example, the *Image* style can be specified in a design as follow:

    <Style x:Key="{x:Type Image}" TargetType="{x:Type Image}"/>

In this case, the *ToggleButton* style (for popups) was not provided.

## WTE00207

*BorderDecoration must have one nested item.*

A BorderDecoration is used to decorate one item. This error is issued if the BorderDecoration has no nested item. You can use the Empty component to fix it.   

## WTE00208

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'... and some of these contain Xaml files.

This error is issued when the console fails to process one of the files in the 'dynamic' subfolder.

## WTE00209

*Missing dynamic property name.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case the property name is missing.

## WTE00210

*Invalid dynamic property type 'x'.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case the property name is found but the type is bad. Only `boolean` is allowed.

## WTE00211

*Expected operator, integer constant or object property.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case a line could not be parsed as an operator (ex: AND), an integer constant, or the property of an object using the `object`.`property` syntax.

## WTE00212

*Operand not following an operator.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case an operand was found without an operator expecting it.

## WTE00213

*Dynamic property 'x' with no expression.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case the property name and types were found, but not operators or expressions.

## WTE00214

*Dynamic property 'x' not completely specified.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case the property name and types were found, but some operator is missing an operand.

## WTE00215

*Indentation expected.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case an operator or expression has no indentation.

## WTE00216

*Expression partially indented.*

A dynamic property consist of a name and type separated by a colon, and followed by lines of operators or expressions.

In this case an operator or expression is indented, but not with the same number of spaces than other lines. For example, other lines use multiple of 4 space but the guilty line has an indentation not multiple of 4.

## WTE00217

*Dynamic file not found.*

A dynamic file in the 'dynamic' folder with the same name as the page is expected.

## WTE00218

*Control must reference a name.*

This error is issued when a Control in a layout file does not refer to the name of a component.

## WTE00219

*Close popup can only be a property.*

This error is issued when a popup component declares a *close popup* but not with the `object`.`property syntax`.

## WTE00220

*Component 'x' is used more than once in page 'y'.*

This error is issued when in all combined layouts used by a page a component is used more than once. It cannot be made the target of a unit test operation.

## WTE00221

*Invalid Type for Empty component.*

This error is issued when `<Empy Type="..."/>` uses a *Type* that is not `"Button"`.

## WTE00222

*Custom page destination 'x' is repeated.*

This error is issued when a button declares a goto using the *<custom page: page 1; page 2; ... page n>* syntax but one of the pages is present more than once.

## WTE00223

*A custom page must declare at least one destination page.*

This error is issued when a button declares a goto using the *<custom page: page 1; page 2; ... page n>* syntax but there not even page 1.
 
## WTE00224

*Component 'x' must be a button.*

This error is issued when the target of a click operation is not a button.

## WTE00225

*Component 'x' must be an edit or password edit.*

This error is issued when the target of a fill operation is not an edit or password edit.

## WTE00226

*Component 'x' must be a selector.*

This error is issued when the target of a select operation is not a selector.

## WTE00227

*Page 'x' not found.*

This error is issued when a unit test operation applies to a page that doesn't exist.

## WTE00228

*Area 'x' not found.*

This error is issued when a unit test operation applies to an area that doesn't exist.

## WTE00229

*Component 'x' not found in area 'y'.*

This error is issued when the target of a unit test operation is not found in the area where it must be declared.

## WTE00230

*Component 'x' must be a popup.*

This error is issued when the target of a toggle operation is not a popup.

## WTE00231

The Wrist console must be run with a valid input folder, output folder, the name of a home page and that of a color theme. The input folder contains subdirectories such as 'page', 'area'... and some of these contain Xaml files.

This error is issued when the console fails to process the unit test file.

## WTE00232

*Invalid line in the unit testing file.*

This error is issued when a line in the unit test file doesn't follow this syntax: *page name*, *operation*, *area name*, *component name with optional parameter*.

## WTE00233

*Invalid line in the unit testing file.*

This error is issued when a fill operation is missing the component parameter.

## WTE00234

*Invalid line in the unit testing file.*

This error is issued when a select operation is missing the component parameter.

## WTE00235

*Invalid line in the unit testing file.*

This error is issued when a select operation has the component parameter but it's not an integer constant.

## WTE00236

*Unknown unit testing operation 'x'.*

This error is issued when a unit test operation is none of the reconginized operations such as *click*, *fill*, *select* or *toggle*.


