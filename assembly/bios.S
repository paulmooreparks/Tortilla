.intel_syntax noprefix
.code16

.section begin, "a"

/* ID string */
.ascii "SALSABIOS v1.00 Paul M. Parks\0"
.align 16

.section main, "ax"

.global init

init:
   mov ax, cs
   mov ss, ax
   /* mov ah, 4
   mov bl, 6 */
   add ah, bl
   add [eax], bl
   xor bl, [eax]
   xor ah, bl
   mov sp, 0xF000
   push 2
   pop ax
   sub ax, 0x1002
   mov bx, 0xb800
   lea esi, [ebx + 8 * eax + 4]
   mov es, bx
   mov bx, 0xA0
   mov word ptr es:[bx], 0x0e41
   lea bx, [bx + 2]
   mov es:[bx], word ptr 0x0F2D
   lea bx, [bx + 2]
   mov es:[bx], word ptr 0x0E4F
   lea bx, [bx + 2]
   mov es:[bx], word ptr 0x0E4B

   hlt

/* This winds up directly in video memory */
.section video, "a"
   .byte 'S'
   .byte 0x0A
   .byte 'A'
   .byte 0x0A
   .byte 'L'
   .byte 0x0A
   .byte 'S'
   .byte 0x0A
   .byte 'A'
   .byte 0x0A
   .byte 'B'
   .byte 0x0C
   .byte 'I'
   .byte 0x0C
   .byte 'O'
   .byte 0x0C
   .byte 'S'
   .byte 0x0C
   .byte ' '
   .byte 0x0f
   .byte 'v'
   .byte 0x0F
   .byte '1'
   .byte 0x0F
   .byte '.'
   .byte 0x0F
   .byte '0'
   .byte 0x0F
   .byte '0'
   .byte 0x0F
   .byte ' '
   .byte 0x0F
   .byte 'P'
   .byte 0x0F
   .byte 'a'
   .byte 0x0F
   .byte 'u'
   .byte 0x0F
   .byte 'l'
   .byte 0x0F
   .byte ' '
   .byte 0x0F
   .byte 'M'
   .byte 0x0F
   .byte '.'
   .byte 0x0F
   .byte ' '
   .byte 0x0F
   .byte 'P'
   .byte 0x0F
   .byte 'a'
   .byte 0x0F
   .byte 'r'
   .byte 0x0F
   .byte 'k'
   .byte 0x0F
   .byte 's'
   .byte 0x0F
   .fill 3840, 2, 0x0f20

hello_string:
        .string "SALSABIOS v1.00 Paul M. Parks" 

.section reset, "ax"
   jmp init
   .align 16, 0xff
