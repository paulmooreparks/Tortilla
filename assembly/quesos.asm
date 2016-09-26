BITS 16
section	.text
	global _start       ;must be declared for using gcc
_start:                     ;tell linker entry point
    mov ax, [bx]
    mov eax, [ebx]
    mov [bx], ax
    mov [ebx], eax
    mov ebx, 0xb8000
    mov eax, 0x2f4b2f4f
    mov [ebx], eax
    mov ax, 0x41
    mov bx, 0x99
    mov cx, 0x11
    mov dx, 0xAB
    ; mov dword [0xb8000], 0x2f4b2f4f
    int 0x10
    mov [bp + si], ax
    mov [bx], bx
    mov [bx], cx
    mov [bx], dx
	hlt

section	.data

msg	db	'Hello, world!',0xa	;our dear string
len	equ	$ - msg			;length of our dear string

; compile me with 
; "c:\Program Files\NASM\nasm" -f bin -o quesos.com quesos.asm -l quesos.lst
