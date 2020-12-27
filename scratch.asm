INCLUDE "core.asm"
INCLUDE "stdlib.asm"

LABEL foobar AUTO

$00001000:
LD $01 A
LD $02 B
LD $03 C
LD $04 D
LD $05 E
LD $06 G
LD $07 H
LD $08 J
LD $09 K
LD $0A L
LD $0B M
LD $0C Z

MOD $00 A
HALT

