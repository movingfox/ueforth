\ Copyright 2022 Bradley D. Nelson
\
\ Licensed under the Apache License, Version 2.0 (the "License");
\ you may not use this file except in compliance with the License.
\ You may obtain a copy of the License at
\
\     http://www.apache.org/licenses/LICENSE-2.0
\
\ Unless required by applicable law or agreed to in writing, software
\ distributed under the License is distributed on an "AS IS" BASIS,
\ WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
\ See the License for the specific language governing permissions and
\ limitations under the License.

( Expand Graphics for Windows )

grf internals definitions
also windows

z" GrfClass" constant GrfClassName
z" uEforth" constant GrfWindowTitle

0 value hinstance
0 value GrfClass
0 value hwnd
0 value hdc
create ps PAINTSTRUCT allot
create msgbuf MSG allot
create binfo BITMAPINFO allot
0 value backbuffer
cell allocate throw to backbuffer

: rescale { w h }
  w width = h height = and if exit then
  w to width   h to height
  backbuffer w h * 4* resize throw to backbuffer
  backbuffer w h * 4* 255 fill
  binfo BITMAPINFO erase
  BITMAPINFOHEADER binfo ->bmiHeader ->biSize !
  w binfo ->bmiHeader ->biWidth !
  h negate binfo ->bmiHeader ->biHeight !
  1 binfo ->bmiHeader ->biPlanes !
  32 binfo ->bmiHeader ->biBitCount !
  BI_RGB binfo ->bmiHeader ->biCompression !
  RESIZED to event
;

: GrfWindowProc { hwnd msg w l }
  WM_QUIT msg = if
    FINISHED to event
    0 exit
  then
  WM_DESTROY msg = if
    0 PostQuitMessage
    0 exit
  then
  WM_PAINT msg = if
    hwnd ps BeginPaint drop
    hwnd ps EndPaint drop
    EXPOSED to event
    0 exit
  then
  WM_SIZE msg = if
    l GET_X_LPARAM $ffff and
    l GET_Y_LPARAM $ffff and rescale
    0 exit
  then
  WM_KEYDOWN msg = if
    PRESSED to event
  then
  WM_KEYUP msg = if
    RELEASED to event
  then
  WM_CHAR msg = if
  then
  WM_LBUTTONDOWN msg = if
    l GET_X_LPARAM to mouse-x
    l GET_Y_LPARAM to mouse-y
    PRESSED to event
  then
  WM_LBUTTONUP msg = if
    l GET_X_LPARAM to mouse-x
    l GET_Y_LPARAM to mouse-y
    RELEASED to event
  then
  WM_MOUSEMOVE msg = if
    l GET_X_LPARAM to mouse-x
    l GET_Y_LPARAM to mouse-y
    MOTION to event
    0 exit
  then
  hwnd msg w l DefWindowProcA
;

grf definitions
also internals
also windows

: window { width height }
  width 0< { fullscreen }
  fullscreen if 640 to width 480 to height then

  NULL GetModuleHandleA to hinstance
  1 1 rescale

  pad WINDCLASSA erase
    WindowProcShim pad ->lpfnWndProc !
    hinstance pad ->hInstance !
    GrfClassName pad ->lpszClassName !
    NULL IDC_ARROW LoadCursorA pad ->hCursor !
    hinstance IDI_MAIN_ICON LoadIconA pad ->hIcon !
  pad RegisterClassA to GrfClass

  0 GrfClass GrfWindowTitle WS_OVERLAPPEDWINDOW
    CW_USEDEFAULT CW_USEDEFAULT width height
    NULL NULL hinstance ['] GrfWindowProc callback
    CreateWindowExA to hwnd

  hwnd GetDC to hdc

  fullscreen if SW_SHOWMAXIMIZED else SW_SHOWDEFAULT then
  hwnd swap ShowWindow drop
  hwnd SetForegroundWindow drop
;

: wait
  event FINISHED = if exit then
  FINISHED to event
  begin msgbuf NULL 0 0 GetMessageA while
    msgbuf TranslateMessage drop
    msgbuf DispatchMessageA drop
    event FINISHED <> if exit then
  repeat
;

: poll
  event FINISHED = if exit then
  UNKNOWN to event
  msgbuf NULL 0 0 PM_REMOVE PeekMessageA if
    msgbuf TranslateMessage drop
    msgbuf DispatchMessageA drop
  then
;

: flip
  hdc 0 0 width height 0 0 width height
    backbuffer binfo DIB_RGB_COLORS SRCCOPY StretchDIBits drop
;

: pixel ( w h -- a ) width * + 4* backbuffer + ;

only forth definitions