GUI app for using googles img2webp. Create animated snippets of your screen in webp format with support for trimming 

The app requires the web2img.exe in the same folder as the apps executable.
This dependency can be downloaded from here:
https://developers.google.com/speed/webp/download


##Installation & Usage

Download the latest relase, unpack it to a folder and add web2img to the same folder.
When starting the app you mark a area of the screen which you want recorded. 
Then hit escape when you are finished or the maximum recording time has elapsed.


##Docs

The app use System.Graphics to capture a part of the screen and write it to an image file at an interval of 10 fps.
SharpHook nuget package is used to hook into global escape keyboard key for stopping the recording early.
The image files are then passed to img2webp to create a webp.
