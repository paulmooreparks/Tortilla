/* salsaBIOS v1.00 Paul M. Parks */

.intel_syntax noprefix
.code16

.section begin, "a"

/* Pre-populate the interrupt vector table. */

int00:
/* INT 0x00 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

int01:
/* INT 0x01 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

int02:
/* INT 0x02 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

int03:
/* INT 0x03 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x04 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x05 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x06 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x07 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x08 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x09 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x0A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x0B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x0C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x0D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x0E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x0F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x10 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x11 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x12 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x13 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x14 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x15 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x16 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x17 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x18 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x19 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x1A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x1B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x1C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x1D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x1E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x1F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x20 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x21 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x22 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x23 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x24 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x25 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x26 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x27 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x28 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x29 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x2A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x2B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x2C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x2D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x2E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x2F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x00 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x31 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x32 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x33 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x34 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x35 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x36 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x37 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x38 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x39 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x3A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x3B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x3C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x3D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x3E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x3F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x40 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x41 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x42 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x43 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x44 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x45 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x46 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x47 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x48 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x49 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x4A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x4B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x4C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x4D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x4E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x4F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x50 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x51 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x52 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x53 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x54 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x55 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x56 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x57 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x58 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x59 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x5A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x5B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x5C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x5D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x5E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x5F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x60 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x61 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x62 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x63 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x64 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x65 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x66 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x67 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x68 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x69 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x6A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x6B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x6C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x6D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x6E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x6F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x70 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x71 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x72 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x73 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x74 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x75 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x76 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x77 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x78 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x79 */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x7A */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x7B */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x7C */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x7D */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x7E */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */

/* INT 0x7F */
.word dummyint	/* interrupt handler entry point */
.word 0xf000	/* code segment */


.section main, "ax"

.global init

init:
	xor ax, ax
	mov es, ax
	mov ax, 0x22
	mov bx, 4
	mul bx
	mov bx, ax
	cli
	mov word ptr es:[bx], offset dummyint
	mov word ptr es:[bx + 2], cs
	sti
	/* Just playing around with interrupts. These should call the dummy handler. */
 	int 0x22
	int 0
	/* int 3 */

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

dummyint:
	/* Here goes the body of your handler */
	iret

hello_string:
	.string "salsaBIOS v1.00 Paul M. Parks\0" 

/* This winds up directly in video memory */
.section video, "a"
   .fill 4096, 2, 0x0f20

.section reset, "ax"
	jmp init
	.align 16, 0xcc
