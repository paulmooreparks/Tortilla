.intel_syntax noprefix
.code16

.section begin, "a"

/* ID string */
.ascii "salsaBIOS v1.00 Paul M. Parks"

.section main, "ax"

.global init

init:
    XOR AX, AX
    MOV ES, AX
	mov ax, 0x22
	mov bx, 4
	MUL bx
	mov bx, ax
    CLI
	MOV WORD PTR ES:[bx], OFFSET int22
	MOV WORD PTR ES:[bx + 2], CS
	STI
	INT 0x22

	mov ax, cs
	mov ds, ax
	mov ss, ax
	mov sp, 0xF000
	mov bx, 0xb800
	mov es, bx
	xor di, di
	lea si, hello_string

read_string:
	mov bx, si
	mov dl, ds:[bx]
	jz done
	mov dh, 0x0F
	mov bx, di
	mov word ptr es:[bx], dx
	lea di, [bx + 2]
	inc si
	jmp read_string

done:
	/* Write 'A-OK' on second line */
	mov bx, 0xA0
	mov word ptr es:[bx], 0x0E41
	lea bx, [bx + 2]
	mov es:[bx], word ptr 0x0E2D
	lea bx, [bx + 2]
	mov es:[bx], word ptr 0x0E4F
	lea bx, [bx + 2]
	mov es:[bx], word ptr 0x0E4B

	hlt

	/* Stuff to test later */
	mov ah, 4
	mov bl, 6

int22:
	/* Here goes the body of your handler */
	IRET

hello_string:
	.string "salsaBIOS v1.00 Paul M. Parks\0" 

/* This winds up directly in video memory */
.section video, "a"
   .fill 4096, 2, 0x0f20

.section reset, "ax"
	jmp init
	.align 16, 0xff
