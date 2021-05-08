**This project is no longer maintained**

# Wrist
A framework to create Web sites using C# and Xaml.

## Status

Still alpha.

This project is based on the CSHTML5 framework, a tool that takes C# and Xaml files and convert them to .js to be executed on a web site server. What Wrist mainly does is to provide a structure and pre-defined controls to hide many of the messy details of CSHTML5.

## Features

Wrist supports the following features.

### Separation of concepts

A Web site is expected to be split in pages. Each page is defined by:
- Its components, for example some lines of text, a couple buttons, and so on. Components are just the bare concept of text and button.
- Its layout, where components are located and how they are moved if the browser is resized.
- Its design, the artist side of it. For example whether buttons have borders, round corners, and so on.
- Its color theme. Change the color theme and the entire site can be changed with new colors instantly (as instantly as a recompilation can be).
- Its background, for pages that don't extend to the left and right sides.
- The code implementation of concepts, which is separated from the site.

Each aspect of a page is specified separately: there is a file that specifies components, another the layout and so on.

One advantage of this approach is that many things can be factorized. For example, one can define a page header and reuse it in all pages.

### Translation

Built-in support for pages translated in different languages. You provide a single CVS file with translated sentences and the code to decide which language the user has selected, and the site can display the proper text.

### Well-known controls

Supported controls are: read-only text (TextBlock), editable text (TextBox, PasswordBox), buttons (Button, CheckBox, RadioButton), image (Image), lists (ListBox), popup (Popup).

Wrist also has containers, for a single object or a list of objects.

### Layout

Components can be aligned in panels, using the usual StackPanel, DockPanel, WrapPanel and Grid.

### Style

Design for each type of component is required in the form of a default style, but one can define multiple styles and use them specifically in pages. For example, there can be several button styles. These styles are similar to WPF styles and specified in Xaml files.

### Objects

For each concept supported by a site, there must be a corresponding object with properties and event. Wrist will connect them to the user interface, and programmers can implement operations.

For instance, the concept of signing in can be represented with a login object, having properties id, password... and events like register, signin, signout... The login logic is not provided since it would probably mean storing info in a database and other implementation-specific details.

## Examples

There is no example of live sites using this framework (yet).
 
## Building and using the framework

Building steps:

+ Clone or download the repository on your computer.
+ Modify `NetTools/SecretUuid.cs` to use your own id (so you don't mess up with mine). The project will not compile until you have completed this step.
+ If you cloned the project, you can tell Git to ignore your change with the following command: `git update-index --assume-unchanged NetTools/SecretUuid.cs`.
+ Open the `Wrist.sln` solution and compile it. The configuration you select doesn't matter, you'll do this step only once.
+ In the root folder, run WristConsole with arguments to generate the C# project of your site. For example, to generate the Easly site, run `WristConsole/bin/x64/Debug/WristConsole "Samples/easly" "./AppCSHtml5" "home" "default" "QACHALLENGE=1"`.
+ Open the `AppCSHtml5.sln` solution.
+ Build the NetTools project. This will download a required Nuget package.
+ Close and restart Visual Studio for this solution.
+ Build the solution. This step may take some time.
+ Upload the content of `AppCSHtml5/bin/Debug/Output` to your site's root directory on the server.
