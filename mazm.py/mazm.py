# constants defined here to avoid magic numbers
COMMENT_PREFIX = "//"
DEFAULT_START_PROG_ADDR = 0
DEFAULT_START_MEM_ADDR = 0

# declare our instance variables here
# now we can access them inside our class methods
_symbol_table = {}
_user_def_vars = {}

_next_program_address = DEFAULT_START_PROG_ADDR
_next_memory_address = DEFAULT_START_MEM_ADDR

_lines = []

def generate_symbol_table(line):
    print (line)

def generate_code(line):
    print (line)

def assemble(asm_file):
    # add each line in asm_file to our internal list
    for line in open(asm_file, 'r'):
        # we can stirp them here to avoid the extra conditional
        _lines.append(line.strip())

    # iterate over each line
    for line in _lines:
        if not line.startswith(COMMENT_PREFIX):
            generate_symbol_table(line)
            generate_code(line)

def main():
    asmFile = "test.mazm"
    assemble(asmFile)

