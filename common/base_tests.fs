\ Copyright 2021 Bradley D. Nelson
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

( Tests Base Operations )
: test-empty-stack   depth 0 =assert ;
: test-add   123 111 + 234 =assert ;
: test-dup-depth   123 depth 1 =assert dup depth 2 =assert 2drop ;
: test-dup-values   456 dup 456 =assert 456 =assert ;
: test-2drop   123 456 2drop depth 0 =assert ;
: test-nip   123 456 nip depth 1 =assert 456 =assert ;
: 8throw   8 throw ;
: test-catch ['] 8throw catch 8 =assert depth 0 =assert ;
: throw-layer 456 >r 123 123 123 8throw 123 123 123 r> ;
: test-catch2 9 ['] throw-layer catch 8 =assert 9 =assert depth 0 =assert ;
: test-rdrop   111 >r 222 >r rdrop r> 111 =assert ;
: test-*/    1000000 22 7 */ 3142857 =assert ;
: test-bl   bl 32 =assert ;
: test-0=   123 0= 0 =assert 0 0= assert ;
: test-cells   123 cells cell+ cell/ 124 =assert ;
: test-aligned    127 aligned 128 =assert ;
: test-[char]   [char] * 42 =assert ;
2 3 * 4 * 5 * 6 * 7 * 8 * 9 * 10 * 11 * 12 * constant 2-12*
: test-fornext    1 10 for r@ 2 + * next 2-12* =assert ;
: test-foraft    1 11 for aft r@ 2 + * then next 2-12* =assert ;
: test-doloop     1 13 2 do i * loop 2-12* =assert ;
: inc-times ( a n -- a+n ) 0 ?do 1+ loop ;
: test-?do     123 40 inc-times 163 =assert ;
: test-?do2     123 0 inc-times 123 =assert ;
: test-<>   123 456 <> assert ;
: test-<>2   123 123 <> 0 =assert ;
: inc/2-times ( a n -- a+n/2 ) 0 ?do 1+ 2 +loop ;
: test-+loop   123 0 inc/2-times 123 =assert ;
: test-+loop2   123 6 inc/2-times 126 =assert ;

e: test-arithmetic
  3 4 + .
  out:\ 7 
;e

e: test-print-string
  : foo ." This is a test!" cr ;
  foo
  out: This is a test!
;e

e: test-print20
  : foo 20 0 do i . loop cr ;
  foo
  out: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 
;e

e: test-multiline
  : foo ." Hello" cr ." There" cr ." Test!" cr ; foo
  out: Hello
  out: There
  out: Test!
;e

e: test-value-to
  123 value foo
  foo . cr
  out: 123 
  55 to foo
  foo . cr
  out: 55 
  : bar 99 to foo ;
  foo . cr
  out: 55 
  bar foo . cr
  out: 99 
;e

e: test-comments-interp
  123 ( Interpretered comment ) 456
  789 \ Interpretered comment )
  789 =assert 456 =assert 123 =assert
;e

e: test-comments-compiled
  : foo 123 ( Compiled comment ) 456
        789 \ Interpretered comment )
        999 ;
  foo 999 =assert 789 =assert 456 =assert 123 =assert
;e

e: test-recurse
  : factorial   dup 0= if drop 1 else dup 1- recurse * then ;
  5 factorial 120 =assert
;e

e: test-accept
  in: 1234567890xxxxxx
  pad 10 accept
  pad swap type cr
  out: --> 1234567890
  out: 1234567890
  in: foo
  pad 10 accept
  pad swap type cr
  out: --> foo
  out: foo
;e

e: test-key
  in: 1
  key 49 =assert
  key nl =assert
;e

e: test-compiler-off
  : test [ 123 111 + literal ] ;
  test 234 =assert
;e

e: test-empty-string
  : test s" " ;
  test 0 =assert drop
;e

e: test-?dup
  123 ?dup 123 =assert 123 =assert
  depth 0 =assert
  0 ?dup 0 =assert
  depth 0 =assert
;e

e: test-lshift
  $1234 4 lshift $12340 =assert
;e

e: test-rshift
  $1234 4 rshift $123 =assert
  -1 cell 8 * 1- rshift 1 =assert
;e

e: test-arshift
  $1234 4 arshift $123 =assert
  -1 cell 8 * 1- arshift -1 =assert
;e

e: test-2@2!
  create foo 2 cells allot
  123 456 foo 2!
  999 foo 2@
  456 =assert
  123 =assert
  999 =assert
;e

e: test-/
  101 2 / 50 =assert
  101 -2 / -51 =assert
  -101 -2 / 50 =assert
  -101 2 / -51 =assert

  100 2 / 50 = assert
  100 -2 / -50 = assert
  -100 -2 / 50 = assert
  -100 2 / -50 = assert
;e

e: test-mod
  101 2 mod 1 =assert
  101 -2 mod -1 =assert
  -101 -2 mod -1 =assert
  -101 2 mod 1 =assert

  100 2 mod 0 =assert
  100 -2 mod 0 =assert
  -100 -2 mod 0 =assert
  -100 2 mod 0 =assert
;e

e: test-/mod-consistent
  101 2 /mod 50 =assert 1 =assert
  101 -2 /mod -51 =assert -1 =assert
  -101 -2 /mod 50 =assert -1 =assert
  -101 2 /mod -51 =assert 1 =assert

  100 2 /mod 50 = assert 0 =assert
  100 -2 /mod -50 = assert 0 =assert
  -100 -2 /mod 50 = assert 0 =assert
  -100 2 /mod -50 = assert 0 =assert
;e

e: test-/mod-consistent
  : /mod-check { a b -- } a b /mod b * + a = assert ;
  101 2 /mod-check
  101 -2 /mod-check
  -101 2 /mod-check
  -101 -2 /mod-check
  100 2 /mod-check
  100 -2 /mod-check
  -100 2 /mod-check
  -100 -2 /mod-check
;e

e: test-*/
  50 2 4 */ 25 =assert
  -50 2 4 */ -25 =assert
;e

e: test-/cell
  10 cells cell/ 10 =assert
  10 cells 1+ cell/ 10 =assert
  -10 cells cell/ -10 =assert
  -10 cells 1- cell/ -11 =assert
;e

e: test-u<
  1 3 u< assert
  3 1 u< 0= assert
  0 -1 u< assert
  -1 0 u< 0= assert
  0 0 u< 0= assert
  -1 -1 u< 0= assert
;e

e: test-postpone
  : test postpone if postpone + postpone then ; immediate
  : test2 test ;
  3 4 1 test2 . cr
  3 4 0 test2 . . cr
  out: 7 
  out: 4 3 
;e
