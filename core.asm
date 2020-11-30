LABEL os_start              AUTO
LABEL user_start            AUTO
LABEL os_core               AUTO
LABEL os_core_jump_table    AUTO
LABEL os_key_input          AUTO

LABEL bios_10_jump_table    AUTO
LABEL bios_10               AUTO
LABEL bios_16_jump_table    AUTO
LABEL bios_16               AUTO

LABEL syscall               AUTO
LABEL syscall_jump_table    AUTO

$00000000: 
ADDRESS user_start                  ; INT 00 (Reset)
ADDRESS $0000`0000                  ; INT 01 
ADDRESS $0000`0000                  ; INT 02 
ADDRESS $0000`0000                  ; INT 03 
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
ADDRESS bios_10                     ; INT 10 Classic BIOS
ADDRESS $0000`0000                  ; INT 11 
ADDRESS $0000`0000                  ; INT 12 
ADDRESS $0000`0000                  ; INT 13 
ADDRESS $0000`0000                  ; INT 14 
ADDRESS $0000`0000                  ; INT 15 
ADDRESS bios_16                     ; INT 16 Classic BIOS
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
ADDRESS os_key_input                ; INT 21 
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
ADDRESS os_core                     ; INT 40 
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
ADDRESS syscall                     ; INT 80


os_start:
LABEL bios_nop                      AUTO
LABEL bios_10_clear_screen          AUTO
LABEL bios_10_set_video_mode        AUTO    
LABEL bios_10_set_cursor_shape      AUTO    
LABEL bios_10_set_cursor_pos        AUTO    
LABEL bios_10_get_cursor_pos        AUTO    
LABEL bios_10_scroll_up             AUTO    
LABEL bios_10_scroll_down           AUTO    
LABEL bios_10_read_char_and_attr    AUTO 
LABEL bios_10_write_char_and_attr   AUTO 
LABEL bios_10_write_char            AUTO
LABEL bios_10_set_color             AUTO

bios_10_jump_table:
ADDRESS bios_10_set_video_mode      ; AH = $00
ADDRESS bios_10_set_cursor_shape    ; AH = $01
ADDRESS bios_10_set_cursor_pos      ; AH = $02
ADDRESS bios_10_get_cursor_pos      ; AH = $03
ADDRESS bios_10_clear_screen        ; AH = $04
ADDRESS bios_nop                    ; AH = $05
ADDRESS bios_10_scroll_up           ; AH = $06
ADDRESS bios_10_scroll_down         ; AH = $07
ADDRESS bios_10_read_char_and_attr  ; AH = $08
ADDRESS bios_10_write_char_and_attr ; AH = $09
ADDRESS bios_10_write_char          ; AH = $0A
ADDRESS bios_10_set_color           ; AH = $0B

bios_10:
PUSH B
CLR B
LD A.B1 B.H0
MUL $04 B.H0
LD bios_10_jump_table B.H1
ADD B.H0 B.H1
CALL @B.H1
POP B
RET


bios_10_set_video_mode:
OUT A $7F
RET

bios_10_set_cursor_shape:       
OUT A $7F
RET
         
bios_10_set_cursor_pos:         
OUT A $7F
RET
         
bios_10_get_cursor_pos:
PUSH B
CLR B
IN $7F B
POP B
RET
         
bios_nop:                    
NOP
RET
         
bios_10_scroll_up:              
OUT A $7F
RET
         
bios_10_scroll_down:            
OUT A $7F
RET
         
bios_10_read_char_and_attr:     
NOP
RET
         
bios_10_write_char_and_attr:    
OUT A $7F
RET

bios_10_write_char:
OUT A $7F
RET

bios_10_clear_screen:
OUT A $7F
RET

bios_10_set_color:
OUT A $7F
RET


LABEL bios_16_get_keystroke         AUTO
LABEL bios_16_check_for_keystroke   AUTO
LABEL bios_16_get_shift_flags       AUTO
LABEL bios_16_set_rate_delay        AUTO
LABEL bios_16_get_enh_keystroke     AUTO
LABEL bios_16_check_enh_keystroke   AUTO
LABEL bios_16_get_ext_shift_states  AUTO

bios_16_jump_table:
ADDRESS bios_16_get_keystroke       ; AH = $00
ADDRESS bios_16_check_for_keystroke ; AH = $01
ADDRESS bios_16_get_shift_flags     ; AH = $02
ADDRESS bios_16_set_rate_delay      ; AH = $03
ADDRESS bios_nop                    ; AH = $04
ADDRESS bios_nop                    ; AH = $05
ADDRESS bios_nop                    ; AH = $06
ADDRESS bios_nop                    ; AH = $07
ADDRESS bios_nop                    ; AH = $08
ADDRESS bios_nop                    ; AH = $09
ADDRESS bios_nop                    ; AH = $0A
ADDRESS bios_nop                    ; AH = $0B
ADDRESS bios_nop                    ; AH = $0C
ADDRESS bios_nop                    ; AH = $0D
ADDRESS bios_nop                    ; AH = $0E
ADDRESS bios_nop                    ; AH = $0F
ADDRESS bios_nop                    ; AH = $10
ADDRESS bios_nop                    ; AH = $11
ADDRESS bios_nop                    ; AH = $12


bios_16:
PUSH B
CLR B
LD A.B1 B.H0
MUL $04 B.H0
LD bios_16_jump_table B.H1
ADD B.H0 B.H1
CALL @B.H1
POP B
RET

bios_16_get_keystroke:
PUSH B
CLR B
IN $60 B
LD B.Q0 A.Q0 ; character
POP B
RET

bios_16_check_for_keystroke:
RET

bios_16_get_shift_flags:
RET

bios_16_set_rate_delay:
RET


LABEL os_idle               AUTO
LABEL os_put_string         AUTO

os_core_jump_table:
ADDRESS os_idle         ; A.Q0 = $0000
ADDRESS os_idle         ; A.Q1 = $0001
ADDRESS os_put_string   ; A.Q0 = $0002

os_core:
PUSH B
CLR B
LD A.Q0 B.H0
MUL $04 B.H0
LD os_core_jump_table B.H1
ADD B.H0 B.H1
CALL @B.H1
POP B
RET

; Idle routine is executed when the OS isn't doing any work. HALT stops the clock,
; and any interrupt will wake the CPU, jump to the interrupt handler, and return 
; to the JMP instruction, which goes right back to HALT.

os_idle:
LABEL os_idle_halt AUTO

ADD $08 S.H0 ; This routine never returns, so adjust the stack back above the return address
os_idle_halt:
HALT
JMP os_idle_halt


os_put_string:
LABEL os_put_string_exit    AUTO
PUSH B
PUSH A.H0
CLR B
CLR A.H1
LD $0A B.B1
os_put_string_next_char:
LD @A.H0 B.B0
CMP $00 B.B0
JZ os_put_string_exit
OUT B $7F
INC A.H0
INC A.H1
JMP os_put_string_next_char
exit_output:
POP A.H0
POP B
RET


; Echo routine
os_key_input:
PUSH A
CLR A
INT $16 ; get keystroke
LD $01 A.B5 ; Count
LD $0A A.B1 ; Function to execute
INT $10     ; BIOS interrupt
POP A
RET


LABEL sys_nop       AUTO
LABEL sys_read      AUTO    ; $00 #0
LABEL sys_write     AUTO    ; $01 #1
LABEL sys_exit      AUTO    ; $3C #60
LABEL sys_reboot    AUTO    ; $A9 #169

syscall_jump_table:
ADDRESS sys_read            ; $00 #0
ADDRESS sys_write           ; $01 #1
ADDRESS sys_nop             ; $02 #
ADDRESS sys_nop             ; $03 #
ADDRESS sys_nop             ; $04 #
ADDRESS sys_nop             ; $05 #
ADDRESS sys_nop             ; $06 #
ADDRESS sys_nop             ; $07 #
ADDRESS sys_nop             ; $08 #
ADDRESS sys_nop             ; $09 #
ADDRESS sys_nop             ; $0A #
ADDRESS sys_nop             ; $0B #
ADDRESS sys_nop             ; $0C #
ADDRESS sys_nop             ; $0D #
ADDRESS sys_nop             ; $0E #
ADDRESS sys_nop             ; $0F #
ADDRESS sys_nop             ; $10 #
ADDRESS sys_nop             ; $11 #
ADDRESS sys_nop             ; $12 #
ADDRESS sys_nop             ; $13 #
ADDRESS sys_nop             ; $14 #
ADDRESS sys_nop             ; $15 #
ADDRESS sys_nop             ; $16 #
ADDRESS sys_nop             ; $17 #
ADDRESS sys_nop             ; $18 #
ADDRESS sys_nop             ; $19 #
ADDRESS sys_nop             ; $1A #
ADDRESS sys_nop             ; $1B #
ADDRESS sys_nop             ; $1C #
ADDRESS sys_nop             ; $1D #
ADDRESS sys_nop             ; $1E #
ADDRESS sys_nop             ; $1F #
ADDRESS sys_nop             ; $20 #
ADDRESS sys_nop             ; $21 #
ADDRESS sys_nop             ; $22 #
ADDRESS sys_nop             ; $23 #
ADDRESS sys_nop             ; $24 #
ADDRESS sys_nop             ; $25 #
ADDRESS sys_nop             ; $26 #
ADDRESS sys_nop             ; $27 #
ADDRESS sys_nop             ; $28 #
ADDRESS sys_nop             ; $29 #
ADDRESS sys_nop             ; $2A #
ADDRESS sys_nop             ; $2B #
ADDRESS sys_nop             ; $2C #
ADDRESS sys_nop             ; $2D #
ADDRESS sys_nop             ; $2E #
ADDRESS sys_nop             ; $2F #
ADDRESS sys_nop             ; $30 #
ADDRESS sys_nop             ; $31 #
ADDRESS sys_nop             ; $32 #
ADDRESS sys_nop             ; $33 #
ADDRESS sys_nop             ; $34 #
ADDRESS sys_nop             ; $35 #
ADDRESS sys_nop             ; $36 #
ADDRESS sys_nop             ; $37 #
ADDRESS sys_nop             ; $38 #
ADDRESS sys_nop             ; $39 #
ADDRESS sys_nop             ; $3A #
ADDRESS sys_nop             ; $3B #
ADDRESS sys_exit            ; $3C #60
ADDRESS sys_nop             ; $3D #
ADDRESS sys_nop             ; $3E #
ADDRESS sys_nop             ; $3F #
ADDRESS sys_nop             ; $41 #
ADDRESS sys_nop             ; $42 #
ADDRESS sys_nop             ; $43 #
ADDRESS sys_nop             ; $44 #
ADDRESS sys_nop             ; $45 #
ADDRESS sys_nop             ; $46 #
ADDRESS sys_nop             ; $47 #
ADDRESS sys_nop             ; $48 #
ADDRESS sys_nop             ; $49 #
ADDRESS sys_nop             ; $4A #
ADDRESS sys_nop             ; $4B #
ADDRESS sys_nop             ; $4C #
ADDRESS sys_nop             ; $40 #
ADDRESS sys_nop             ; $4D #
ADDRESS sys_nop             ; $4E #
ADDRESS sys_nop             ; $4F #
ADDRESS sys_nop             ; $50 #
ADDRESS sys_nop             ; $51 #
ADDRESS sys_nop             ; $52 #
ADDRESS sys_nop             ; $53 #
ADDRESS sys_nop             ; $54 #
ADDRESS sys_nop             ; $55 #
ADDRESS sys_nop             ; $56 #
ADDRESS sys_nop             ; $57 #
ADDRESS sys_nop             ; $58 #
ADDRESS sys_nop             ; $59 #
ADDRESS sys_nop             ; $5A #
ADDRESS sys_nop             ; $5B #
ADDRESS sys_nop             ; $5C #
ADDRESS sys_nop             ; $5D #
ADDRESS sys_nop             ; $5E #
ADDRESS sys_nop             ; $5F #
ADDRESS sys_nop             ; $60 #
ADDRESS sys_nop             ; $61 #
ADDRESS sys_nop             ; $62 #
ADDRESS sys_nop             ; $63 #
ADDRESS sys_nop             ; $64 #
ADDRESS sys_nop             ; $65 #
ADDRESS sys_nop             ; $66 #
ADDRESS sys_nop             ; $67 #
ADDRESS sys_nop             ; $68 #
ADDRESS sys_nop             ; $69 #
ADDRESS sys_nop             ; $6A #
ADDRESS sys_nop             ; $6B #
ADDRESS sys_nop             ; $6C #
ADDRESS sys_nop             ; $6D #
ADDRESS sys_nop             ; $6E #
ADDRESS sys_nop             ; $6F #
ADDRESS sys_nop             ; $70 #
ADDRESS sys_nop             ; $71 #
ADDRESS sys_nop             ; $72 #
ADDRESS sys_nop             ; $73 #
ADDRESS sys_nop             ; $74 #
ADDRESS sys_nop             ; $75 #
ADDRESS sys_nop             ; $76 #
ADDRESS sys_nop             ; $77 #
ADDRESS sys_nop             ; $78 #
ADDRESS sys_nop             ; $79 #
ADDRESS sys_nop             ; $7A #
ADDRESS sys_nop             ; $7B #
ADDRESS sys_nop             ; $7C #
ADDRESS sys_nop             ; $7D #
ADDRESS sys_nop             ; $7E #
ADDRESS sys_nop             ; $7F #
ADDRESS sys_nop             ; $80 #
ADDRESS sys_nop             ; $81 #
ADDRESS sys_nop             ; $82 #
ADDRESS sys_nop             ; $83 #
ADDRESS sys_nop             ; $84 #
ADDRESS sys_nop             ; $85 #
ADDRESS sys_nop             ; $86 #
ADDRESS sys_nop             ; $87 #
ADDRESS sys_nop             ; $88 #
ADDRESS sys_nop             ; $89 #
ADDRESS sys_nop             ; $8A #
ADDRESS sys_nop             ; $8B #
ADDRESS sys_nop             ; $8C #
ADDRESS sys_nop             ; $8D #
ADDRESS sys_nop             ; $8E #
ADDRESS sys_nop             ; $8F #
ADDRESS sys_nop             ; $90 #
ADDRESS sys_nop             ; $91 #
ADDRESS sys_nop             ; $92 #
ADDRESS sys_nop             ; $93 #
ADDRESS sys_nop             ; $94 #
ADDRESS sys_nop             ; $95 #
ADDRESS sys_nop             ; $96 #
ADDRESS sys_nop             ; $97 #
ADDRESS sys_nop             ; $98 #
ADDRESS sys_nop             ; $99 #
ADDRESS sys_nop             ; $9A #
ADDRESS sys_nop             ; $9B #
ADDRESS sys_nop             ; $9C #
ADDRESS sys_nop             ; $9D #
ADDRESS sys_nop             ; $9E #
ADDRESS sys_nop             ; $9F #
ADDRESS sys_nop             ; $A0 #
ADDRESS sys_nop             ; $A1 #
ADDRESS sys_nop             ; $A2 #
ADDRESS sys_nop             ; $A3 #
ADDRESS sys_nop             ; $A4 #
ADDRESS sys_nop             ; $A5 #
ADDRESS sys_nop             ; $A6 #
ADDRESS sys_nop             ; $A7 #
ADDRESS sys_nop             ; $A8 #
ADDRESS sys_reboot          ; $A9 #169
ADDRESS sys_nop             ; $AA #
ADDRESS sys_nop             ; $AB #
ADDRESS sys_nop             ; $AC #
ADDRESS sys_nop             ; $AD #
ADDRESS sys_nop             ; $AE #
ADDRESS sys_nop             ; $AF #
ADDRESS sys_nop             ; $B0 #
ADDRESS sys_nop             ; $B1 #
ADDRESS sys_nop             ; $B2 #
ADDRESS sys_nop             ; $B3 #
ADDRESS sys_nop             ; $B4 #
ADDRESS sys_nop             ; $B5 #
ADDRESS sys_nop             ; $B6 #
ADDRESS sys_nop             ; $B7 #
ADDRESS sys_nop             ; $B8 #
ADDRESS sys_nop             ; $B9 #
ADDRESS sys_nop             ; $BA #
ADDRESS sys_nop             ; $BB #
ADDRESS sys_nop             ; $BC #
ADDRESS sys_nop             ; $BD #
ADDRESS sys_nop             ; $BE #
ADDRESS sys_nop             ; $BF #
ADDRESS sys_nop             ; $C0 #
ADDRESS sys_nop             ; $C1 #
ADDRESS sys_nop             ; $C2 #
ADDRESS sys_nop             ; $C3 #
ADDRESS sys_nop             ; $C4 #
ADDRESS sys_nop             ; $C5 #
ADDRESS sys_nop             ; $C6 #
ADDRESS sys_nop             ; $C7 #
ADDRESS sys_nop             ; $C8 #
ADDRESS sys_nop             ; $C9 #
ADDRESS sys_nop             ; $CA #
ADDRESS sys_nop             ; $CB #
ADDRESS sys_nop             ; $CC #
ADDRESS sys_nop             ; $CD #
ADDRESS sys_nop             ; $CE #
ADDRESS sys_nop             ; $CF #
ADDRESS sys_nop             ; $D0 #
ADDRESS sys_nop             ; $D1 #
ADDRESS sys_nop             ; $D2 #
ADDRESS sys_nop             ; $D3 #
ADDRESS sys_nop             ; $D4 #
ADDRESS sys_nop             ; $D5 #
ADDRESS sys_nop             ; $D6 #
ADDRESS sys_nop             ; $D7 #
ADDRESS sys_nop             ; $D8 #
ADDRESS sys_nop             ; $D9 #
ADDRESS sys_nop             ; $DA #
ADDRESS sys_nop             ; $DB #
ADDRESS sys_nop             ; $DC #
ADDRESS sys_nop             ; $DD #
ADDRESS sys_nop             ; $DE #
ADDRESS sys_nop             ; $DF #
ADDRESS sys_nop             ; $E0 #
ADDRESS sys_nop             ; $E1 #
ADDRESS sys_nop             ; $E2 #
ADDRESS sys_nop             ; $E3 #
ADDRESS sys_nop             ; $E4 #
ADDRESS sys_nop             ; $E5 #
ADDRESS sys_nop             ; $E6 #
ADDRESS sys_nop             ; $E7 #
ADDRESS sys_nop             ; $E8 #
ADDRESS sys_nop             ; $E9 #
ADDRESS sys_nop             ; $EA #
ADDRESS sys_nop             ; $EB #
ADDRESS sys_nop             ; $EC #
ADDRESS sys_nop             ; $ED #
ADDRESS sys_nop             ; $EE #
ADDRESS sys_nop             ; $EF #
ADDRESS sys_nop             ; $F0 #
ADDRESS sys_nop             ; $F1 #
ADDRESS sys_nop             ; $F2 #
ADDRESS sys_nop             ; $F3 #
ADDRESS sys_nop             ; $F4 #
ADDRESS sys_nop             ; $F5 #
ADDRESS sys_nop             ; $F6 #
ADDRESS sys_nop             ; $F7 #
ADDRESS sys_nop             ; $F8 #
ADDRESS sys_nop             ; $F9 #
ADDRESS sys_nop             ; $FA #
ADDRESS sys_nop             ; $FB #
ADDRESS sys_nop             ; $FC #
ADDRESS sys_nop             ; $FD #
ADDRESS sys_nop             ; $FE #
ADDRESS sys_nop             ; $FF #


syscall:
MUL $04 A.H0
LD syscall_jump_table A.H1
ADD A.H0 A.H1
CALL @A.H1
RET

sys_nop:
RET

sys_read:
RET

sys_write:
; PARAMETERS
; G unsigned int fd
; H const char *buf
; J size_t count
; 
; RETURN
; A size_t number of bytes written

LABEL sys_write_exit AUTO
LABEL sys_write_next_char AUTO

PUSH B
PUSH Z.H0
CLR A
CLR B
CLR Z.H0
LD $0A B.B1
sys_write_next_char:
CMP Z.H0 J.H0
JZ sys_write_exit
LD @H.H0 B.B0
OUT B $7F
INC A
DEC J.H0
INC H.H0
JMP sys_write_next_char
sys_write_exit:
POP Z.H0
POP B
RET

sys_reboot:
RET

user_start:
