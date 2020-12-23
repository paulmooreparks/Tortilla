LABEL stdlib_strlen        AUTO
LABEL stdlib_strupr        AUTO

stdlib_strlen:
LABEL stdlib_strlen_loop   AUTO
LABEL stdlib_strlen_done   AUTO

PUSH G.H0
CLR A
stdlib_strlen_loop:
CMPIND $00 @G.H0
JZ stdlib_strlen_done
INC A.H0
INC G.H0
JMP stdlib_strlen_loop
stdlib_strlen_done:
POP G.H0
RET

stdlib_strupr:
LABEL stdlib_strupr_loop   AUTO
LABEL stdlib_strupr_done   AUTO
LABEL stdlib_strupr_continue AUTO

PUSH G.H0
CLR A

stdlib_strupr_loop:
CMPIND $00 @G.H0
JZ stdlib_strupr_done
CMPIND $61 @G.H0
JB stdlib_strupr_continue
CMPIND $7A @G.H0
JA stdlib_strupr_continue
LD @G.H0 A.B0
SUB $20 A.B0
ST A.B0 @G.H0
stdlib_strupr_continue:
INC G.H0
JMP stdlib_strupr_loop
stdlib_strupr_done:
POP G.H0
LD G.H0 A
RET
