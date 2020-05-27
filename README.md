# Flotilla
[![Donate](https://img.shields.io/badge/donate-$$$-brightgreen.svg)](https://blendogames.itch.io/flotilla/purchase)

Source code of tactical space game Flotilla (2010)

## About
In 2010 I made the game [Flotilla](https://blendogames.com/flotilla), and now on its 10th birthday I'm releasing its [source code](https://github.com/blendogames/Flotilla). Beware: Flotilla is basically the project where I learned how to program. It's filled with strange and not-great code. More details [here.](http://blendogames.com/news/post/2020-02-27-flotilla_source_release)

[Here is a short video](https://www.youtube.com/watch?v=ayELDEOlQmE) of what the game looks like. Flotilla is available for purchase from [itch.io](https://blendogames.itch.io/flotilla) or [Steam](http://store.steampowered.com/app/55000). The game consists of two main parts:
- turn-based tactical combat.
- space exploration with branching events.

Flotilla originally used the [XNA framework](https://en.wikipedia.org/wiki/Microsoft_XNA), and last year was ported to [FNA](https://fna-xna.github.io) by the amazing [Ethan Lee](http://www.flibitijibibo.com).

## Compiling and running
- Flotilla's code is written in C# and a .sln solution for Visual Studio 2015 is provided.
- The solution expects to find a folder containing the [FNA source code](https://github.com/FNA-XNA/FNA). Place the FNA source code folder next to your Flotilla project folder (do not put FNA inside your Flotilla folder).
- Download [FNA's native libraries](http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2) and place them in the folder containing your project binaries.
- To run the game, you'll need the game assets from a purchased version of Flotilla. Copy Flotilla's **Content** and **WindowsContent** folders into the folder containing your project binaries. (Flotilla can be purchased from [itch.io](https://blendogames.itch.io/flotilla) or [Steam](http://store.steampowered.com/app/55000))

## License
Flotilla's source code is released under the zlib license. In short: you are free to use this source code for personal or commercial purposes. Read the license details here: [LICENSE.md](https://github.com/blendogames/Flotilla/blob/master/LICENSE.md)

Please note this license only applies to Flotilla's source code. Flotilla's game assets (art, models, textures, audio, etc.) are not open-source and are not to be redistributed.

## Credits

- Created by [Brendon Chung](http://blendogames.com).
- FNA port by [Ethan Lee](http://www.flibitijibibo.com).
- Audio by [Soundsnap](http://soundsnap.com).
- Special Thanks to Daniel Wiksten, Drew Marlowe, Neil Mehta, Sherman Wang, Tom Nguyen, and Venny Wong.

### Libraries used

- [FNA](https://fna-xna.github.io)
- [SDL2](https://www.libsdl.org)
- [MojoShader](https://icculus.org/mojoshader)
- [FAudio](https://github.com/FNA-XNA/FAudio)
- [SDL_image](https://www.libsdl.org/projects/SDL_image)
- [libtheorafile](https://www.theora.org)
