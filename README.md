Windows Jedi
============

Tools for making working with Windows 7 more friendly.
I don't know if it works in Windows 8.x or 10 -- let me know if you have a copy.

Requirements
------------
Windows 7 with Aero enabled. Doesn't work without Aero features (yet)

What it does
------------
 1. **Replace `Win-Tab`** with something more useful (a bit like expose, but with a significantly different way of packing
   and selecting). Note that each thumbnail has a single character before it's name/label. Press this key to select the window.
   Thumbnails are ordered left-to-right by z-order (same as alt-tab). Pop-up class windows (like
   VisualStudio uses) are not shown by default.
   * Press quick-key to select a window and exit switcher
   * Press `tab` without `win` to toggle popups in the switcher.
 2. **Focus-mode** `Shift-F12` dim other windows and make a best effort to prevent the focus-stealing that plagues Windows
 3. **Hide popups**
    `Win-Space` toggles visibility of pop-up windows. This is handy when using Visual Studio's mess of
   a window set. Visibility toggle is conservative and system-wide. Stacking order is reversed on each display.
    `Ctrl-Win-Space` toggles translucency of popups, making them see-through.
 4. **Push-back** `Win-Escape` pushes the current foreground window to the back of the stack. Useful when you've got several reference windows open.
 5. **Alignment** `Win-Alt-n` for n = 1 to 9, position windows on their current screen.
 6. **Screen-switch** `Win-<arrow>` move a window to a physical screen in that direction (does not wrap)

Planned
-------
 * Virtual desktops - (send-to, move-to key) shortcuts like XMonad (use window-under-cursor as target?)
 * 'Reference window' - for dual monitor setup, a window marked reference always stays on the opposite monitor
   as the current focused window (so it's always visible to the side)
 * Keyboard navigation mode - display keyboard shortcuts to focus major aspects of the foreground window
 * Maybe: a focus-set, where a few windows are tiled together and the main one can be rotated in.
 * Maybe: hide cursor while typing, as just about no Windows apps do this correctly! 
 * Mirror window: for presenting -- show a copy of a window on a different screen / location (may also include zoom?)
 * Grid window position: using num-pad, like [Win]-[Left] etc, but with a quarters/thirds layout horz and vert.

Todo / Bugs
-----------
 * Win-Tab in focus mode should switch focus to selected window
 * Focus mode isn't strict enough. Should be more like switcher with only one item
 * Clean up the codebase
 * Move mouse position to window when selecting with `win-tab`?

Interesting code features
-------------------------
Adapted from a few places; there is multi-monitor full screen overlay, hot-key action hooks, and a fast box packing algorithm.
