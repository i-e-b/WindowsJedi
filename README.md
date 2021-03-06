Windows Jedi
============

Tools for making working with Windows more friendly.
Under Windows 10, this must be run as Administrator to take effect.

Requirements
------------
Windows 7 or higher with Aero enabled. Doesn't work without Aero features (yet)

What it does
------------
 1. **Replace `Win-Tab`** with something more useful (a bit like expose, but with a significantly different way of packing
   and selecting). Note that each thumbnail has a single character before it's name/label. Press this key to select the window.
   Thumbnails are ordered left-to-right by z-order (same as alt-tab). Pop-up class windows (like
   VisualStudio uses) are not shown by default.
   * Press quick-key to select a window and exit switcher
   * Press `tab` without `win` to toggle popups in the switcher
   * Press `esc` to exit switcher without changing window focus.
 2. **Focus-mode** `Shift-F12` dim other windows and make a best effort to prevent the focus-stealing that plagues Windows
 3. **Hide popups**
    `Win-Space` toggles visibility of pop-up windows. This is handy when using Visual Studio's mess of
   a window set. Visibility toggle is conservative and system-wide. Stacking order is reversed on each display.
    `Ctrl-Win-Space` toggles translucency of popups, making them see-through.
 4. **Push-back** `Win-Escape` pushes the current foreground window to the back of the stack. Useful when you've got several reference windows open.
 5. **Alignment** `Win-Alt-n` for n = 1 to 9, position windows on their current screen.
 6. **Screen-switch** `Win-<arrow>` move a window to a physical screen in that direction (does not wrap)
 7. **Fade everything** `Ctrl-Win-Space` toggle a low alpha for all screens, so all windows are visible at once (doesn't work for VisualStudio)
 8. **Reference Window** `Win-Minus` set the active window to be a reference - it will move to a screen beside the front-most window (works on multi-screen systems only)

Other stuff
-----------
There is a file typer that will play back keyboard events from the text of a 
file to the active window. Useful for VM driving and badly behaved UIs.

Ideas
-----
 * Record to animated GIF - Overlay window with prep, record and stop buttons. Capture to an animated GIF.
 * Keyboard navigation mode - display keyboard shortcuts to focus major aspects of the foreground window
 * Maybe: a focus-set, where a few windows are tiled together and the main one can be rotated in.

Bugs
-----------
 * Capture screen needs to resize when switching dpi, or rescale resultant capture
 * win-tab screen needs to detect changes to dektop layout (adding/removing screens)
 * screen overlays (for focus/win-tab) need to be 1-per-screen to handle odd layouts.
Interesting code features
-------------------------
Adapted from a few places; there is multi-monitor full screen overlay, hot-key action hooks, and a fast box packing algorithm.
