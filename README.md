# Meadow-Invaders
## Sprite like animations using 1Bit BMP resources for the Meadow F7 dev board

The meadow.foundation graphics library has methods to display 1 bit bitmaps in various colours on an LCD display.

It seemed obvious to load bitmaps from a resource file rather than hand coding them. BMP format seemed the obvious choice.

Turns out BMP format is backwards to how meadow wanted them. In particular the meadow display assumes 0,0 is in the upper left, 
but a BMP has the origin at the lower left. For some reason the 1bit encoding has 0 as on and 1 for off. (FF is 8 black bits 00 is 8 white bits).
BMP assumes the high bit is left most in the image, where as the meadow assumes the first (lowest bit) is the left most one.

Once i figured out how to read the header, and process the actual bits, I found it easier to use a BitArray to hold the image. 
This uses 8 times the memory, but it allows each pixel to be addressable in a boolean array. it does the bit unpacking for you. 
I then bypassed the graphics library and just set the pixels directly.

To support the 2 frame animation (each pair of invaders is the same alien with moving arms or legs), and display multiple bitmaps at the same time,
there is a list of sprites and each one updates the display before the frame is displayed. The sprite draws its next frame based on its internal properties, 
and when it reaches the right side the main loop resets the colour, speed and position and starts again. 

So what started as a test of the graphics library turned into something different...

The "invader" graphics were found on the <a href="https://www.reddit.com/r/joyetechlogos/comments/4hz7nj/space_invaders_full_set/">web</a> 
hope they are freely usable :wink:

![Space invaders](https://i.imgur.com/Tqm8eg9.png)

*NOTE* this is not a game - it is currently a non interactive graphics demo

use standard wiring for Meadow F7 and LCD
![Meadow Frizing](/MeadowInvaders/st7789_fritzing.jpg)

UPDATE: RC1 can achieve 9.23 fps if JIT is enabled

RC3-1 gets 9.49 fps - just slightly fasteer
