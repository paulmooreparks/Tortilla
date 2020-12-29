INCLUDE "core.asm"
INCLUDE "stdlib.asm"

LABEL hw_string AUTO

$00001000:
   LD hw_string G
   CALL stdlib_puts
   CALL stdlib_shutdown

;******************************************************************************
; This label points to the start of the string we want to output.

hw_string: 
   STRING "Hello, world!\0"
