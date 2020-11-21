LABEL os_start              AUTO
LABEL user_start            AUTO
LABEL os_idle               AUTO
LABEL os_key_echo           AUTO
LABEL os_put_string         AUTO

LABEL os_bios_10_jump_table AUTO
LABEL os_bios_10            AUTO

$00000000: 
ADDRESS user_start                  ; INT 00 (Reset)
ADDRESS os_idle                     ; INT 01 (os_idle)
ADDRESS os_key_echo                 ; INT 02 (keyboard)
ADDRESS os_put_string               ; INT 03 
ADDRESS $0000`0000                  ; INT 04
ADDRESS $0000`0000                  ; INT 05 
ADDRESS $0000`0000                  ; INT 06 
ADDRESS $0000`0000                  ; INT 07 
ADDRESS $0000`0000                  ; INT 08 
ADDRESS $0000`0000                  ; INT 09 
ADDRESS $0000`0000                  ; INT 0A 
ADDRESS $0000`0000                  ; INT 0B 
ADDRESS $0000`0000                  ; INT 0C 
ADDRESS $0000`0000                  ; INT 0D 
ADDRESS $0000`0000                  ; INT 0E 
ADDRESS $0000`0000                  ; INT 0F 
ADDRESS os_bios_10                  ; INT 10 Classic BIOS
ADDRESS $0000`0000                  ; INT 11 
ADDRESS $0000`0000                  ; INT 12 
ADDRESS $0000`0000                  ; INT 13 
ADDRESS $0000`0000                  ; INT 14 
ADDRESS $0000`0000                  ; INT 15 
ADDRESS $0000`0000                  ; INT 16 
ADDRESS $0000`0000                  ; INT 17 
ADDRESS $0000`0000                  ; INT 18 
ADDRESS $0000`0000                  ; INT 19 
ADDRESS $0000`0000                  ; INT 1A 
ADDRESS $0000`0000                  ; INT 1B 
ADDRESS $0000`0000                  ; INT 1C 
ADDRESS $0000`0000                  ; INT 1D 
ADDRESS $0000`0000                  ; INT 1E 
ADDRESS $0000`0000                  ; INT 1F 
ADDRESS $0000`0000                  ; INT 20 
ADDRESS $0000`0000                  ; INT 21 
ADDRESS $0000`0000                  ; INT 22 
ADDRESS $0000`0000                  ; INT 23 
ADDRESS $0000`0000                  ; INT 24 
ADDRESS $0000`0000                  ; INT 25 
ADDRESS $0000`0000                  ; INT 26 
ADDRESS $0000`0000                  ; INT 27 
ADDRESS $0000`0000                  ; INT 28 
ADDRESS $0000`0000                  ; INT 29 
ADDRESS $0000`0000                  ; INT 2A 
ADDRESS $0000`0000                  ; INT 2B 
ADDRESS $0000`0000                  ; INT 2C 
ADDRESS $0000`0000                  ; INT 2D 
ADDRESS $0000`0000                  ; INT 2E 
ADDRESS $0000`0000                  ; INT 2F 
ADDRESS $0000`0000                  ; INT 30 
ADDRESS $0000`0000                  ; INT 31 
ADDRESS $0000`0000                  ; INT 32 
ADDRESS $0000`0000                  ; INT 33 
ADDRESS $0000`0000                  ; INT 34 
ADDRESS $0000`0000                  ; INT 35 
ADDRESS $0000`0000                  ; INT 36 
ADDRESS $0000`0000                  ; INT 37 
ADDRESS $0000`0000                  ; INT 38 
ADDRESS $0000`0000                  ; INT 39 
ADDRESS $0000`0000                  ; INT 3A 
ADDRESS $0000`0000                  ; INT 3B 
ADDRESS $0000`0000                  ; INT 3C 
ADDRESS $0000`0000                  ; INT 3D 
ADDRESS $0000`0000                  ; INT 3E 
ADDRESS $0000`0000                  ; INT 3F 
ADDRESS $0000`0000                  ; INT 40 
ADDRESS $0000`0000                  ; INT 41 
ADDRESS $0000`0000                  ; INT 42 
ADDRESS $0000`0000                  ; INT 43 
ADDRESS $0000`0000                  ; INT 44 
ADDRESS $0000`0000                  ; INT 45 
ADDRESS $0000`0000                  ; INT 46 
ADDRESS $0000`0000                  ; INT 47 
ADDRESS $0000`0000                  ; INT 48 
ADDRESS $0000`0000                  ; INT 49 
ADDRESS $0000`0000                  ; INT 4A 
ADDRESS $0000`0000                  ; INT 4B 
ADDRESS $0000`0000                  ; INT 4C 
ADDRESS $0000`0000                  ; INT 4D 
ADDRESS $0000`0000                  ; INT 4E 
ADDRESS $0000`0000                  ; INT 4F 
ADDRESS $0000`0000                  ; INT 50 
ADDRESS $0000`0000                  ; INT 51 
ADDRESS $0000`0000                  ; INT 52 
ADDRESS $0000`0000                  ; INT 53 
ADDRESS $0000`0000                  ; INT 54 
ADDRESS $0000`0000                  ; INT 55 
ADDRESS $0000`0000                  ; INT 56 
ADDRESS $0000`0000                  ; INT 57 
ADDRESS $0000`0000                  ; INT 58 
ADDRESS $0000`0000                  ; INT 59 
ADDRESS $0000`0000                  ; INT 5A 
ADDRESS $0000`0000                  ; INT 5B 
ADDRESS $0000`0000                  ; INT 5C 
ADDRESS $0000`0000                  ; INT 5D 
ADDRESS $0000`0000                  ; INT 5E 
ADDRESS $0000`0000                  ; INT 5F 
ADDRESS $0000`0000                  ; INT 60 
ADDRESS $0000`0000                  ; INT 61 
ADDRESS $0000`0000                  ; INT 62 
ADDRESS $0000`0000                  ; INT 63 
ADDRESS $0000`0000                  ; INT 64 
ADDRESS $0000`0000                  ; INT 65 
ADDRESS $0000`0000                  ; INT 66 
ADDRESS $0000`0000                  ; INT 67 
ADDRESS $0000`0000                  ; INT 68 
ADDRESS $0000`0000                  ; INT 69 
ADDRESS $0000`0000                  ; INT 6A 
ADDRESS $0000`0000                  ; INT 6B 
ADDRESS $0000`0000                  ; INT 6C 
ADDRESS $0000`0000                  ; INT 6D 
ADDRESS $0000`0000                  ; INT 6E 
ADDRESS $0000`0000                  ; INT 6F 
ADDRESS $0000`0000                  ; INT 70 
ADDRESS $0000`0000                  ; INT 71 
ADDRESS $0000`0000                  ; INT 72 
ADDRESS $0000`0000                  ; INT 73 
ADDRESS $0000`0000                  ; INT 74 
ADDRESS $0000`0000                  ; INT 75 
ADDRESS $0000`0000                  ; INT 76 
ADDRESS $0000`0000                  ; INT 77 
ADDRESS $0000`0000                  ; INT 78 
ADDRESS $0000`0000                  ; INT 79 
ADDRESS $0000`0000                  ; INT 7A 
ADDRESS $0000`0000                  ; INT 7B 
ADDRESS $0000`0000                  ; INT 7C 
ADDRESS $0000`0000                  ; INT 7D 
ADDRESS $0000`0000                  ; INT 7E 
ADDRESS $0000`0000                  ; INT 7F 
ADDRESS $0000`0000                  ; INT 80


os_start:
LABEL bios_set_video_mode   AUTO    ; AH = $00
LABEL bios_set_cursor_shape AUTO    ; AH = $01
LABEL bios_set_cursor_pos   AUTO    ; AH = $02
LABEL bios_get_cursor_pos   AUTO    ; AH = $03
LABEL bios_nop              AUTO    ; AH = $04
LABEL bios_scroll_up        AUTO    ; AH = $06
LABEL bios_scroll_down      AUTO    ; AH = $07
LABEL bios_read_char_and_attr  AUTO ; AH = $08
LABEL bios_write_char_and_attr AUTO ; AH = $09
LABEL bios_write_char       AUTO    ; AH = $0A      

os_bios_10_jump_table:
ADDRESS bios_set_video_mode         ; AH = $00
ADDRESS bios_set_cursor_shape       ; AH = $01
ADDRESS bios_set_cursor_pos         ; AH = $02
ADDRESS bios_get_cursor_pos         ; AH = $03
ADDRESS bios_nop                    ; AH = $04
ADDRESS bios_nop                    ; AH = $05
ADDRESS bios_scroll_up              ; AH = $06
ADDRESS bios_scroll_down            ; AH = $07
ADDRESS bios_read_char_and_attr     ; AH = $08
ADDRESS bios_write_char_and_attr    ; AH = $09
ADDRESS bios_write_char             ; AH = $0A

os_bios_10:
PUSH A
PUSH B
CLR B
LD A.B1 B.B1
MUL $04 B.B1
LD os_bios_10_jump_table B.H1
ADD B.B1 B.H1
CALL @B.H1
POP B
POP A
RET


bios_set_video_mode:
NOP
RET

bios_set_cursor_shape:       
NOP
RET
         
bios_set_cursor_pos:         
NOP
RET
         
bios_get_cursor_pos:
NOP
RET
         
bios_nop:                    
NOP
RET
         
bios_scroll_up:              
NOP
RET
         
bios_scroll_down:            
NOP
RET
         
bios_read_char_and_attr:     
NOP
RET
         
bios_write_char_and_attr:    
NOP
RET

bios_write_char:
PUSH B
CLR B
LD $01 B.B0 ; opcode
LD A.B5 B.B1 ; count
LD A.B0 B.B2 ; character
OUT B $01
POP B
RET         

LABEL output         AUTO
LABEL exit_output    AUTO

os_put_string:
PUSH A
PUSH B
CLR B
LD $01 B.B0
next_char:
LD @A.H0 B.B2
CMP $00 B.B2
JZ exit_output
OUT B $01
INC A.H0
JMP next_char
exit_output:
POP B
POP A
RET


output:
LD C.Q0 A.Q1
OUT A $01
LD C.Q1 A.Q1
OUT A $01
LD C.Q2 A.Q1
OUT A $01
LD C.Q3 A.Q1
OUT A $01
RET         

; Idle routine is executed when the OS isn't doing any work. HALT stops the clock,
; and any interrupt will wake the CPU, jump to the interrupt handler, and return 
; to the JMP instruction, which goes right back to HALT.

LABEL os_idle_halt AUTO

os_idle:
ADD $08 S.H0 ; This routine never returns, so adjust the stack back above the return address
os_idle_halt:
HALT
JMP os_idle_halt

; Echo routine
os_key_echo:
PUSH A
PUSH B
CLR B
IN $01 B
CLR A
LD $01 A.B0 ; opcode
LD $01 A.B1 ; count
LD B.B0 A.B2 ; character
OUT A $01 ; video port
POP B
POP A
RET

user_start:
