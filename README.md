Windows Jedi
============

Tools for making working with Windows 7 more friendly.
I don't know if it works in Windows 8.x -- let me know if you have a copy.

Requirements
------------
Windows 7 with Aero enabled. Doesn't work without Aero features.

What it does
------------
 1. Replace Win-Tab with something more useful (a bit like expose, but with a significantly different way of packing and selecting)
 2. Focus-mode: dim other windows and make a best effort to prevent the focus-stealing that plagues Windows
 
Planned
-------
 * Keyboard navigation mode - display keyboard shortcuts to focus major aspects of the foreground window
 * Maybe: a focus-set, where a few windows are tiled together and the main one can be rotated in.
 * send-to-back key - focused window goes to the bottom of the stack and next window down gains focus
 * hide/show floating windows - momentarily set floating windows (specifically VisualStudio tool windows) to transparent.
 * Maybe: hide cursor while typing, as just about no Windows apps do this correctly! 

Todo / Bugs
-----------
 * Win-Tab in focus mode should switch focus to selected window
 * Win-Tab doesn't handle maximised windows well if their 'restored' size is significantly different
 * Focus mode isn't strict enough. Should be more like switcher with only one item
 * Window hooks are gc pinned - need to go through and carefully check disposal and ensure no memory leaks

Interesting code features
-------------------------
Stolen and adapted from a few places; there is multi-monitor full screen overlay, hot-key action hooks, and a fast box packing algorithm.