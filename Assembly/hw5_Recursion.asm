#Sam Arutyunyan
.data

.globl main

comma: .asciiz ","
nl: .asciiz "\n"

.text
main:

addi $v0, $0, 5 	#get int
syscall
add $t0, $0, $v0

addi $sp, $sp, -4 	#push
sw $t0, 0($sp)
jal addrec
addi $sp, $sp, 4 	#pop

add $a0, $0, $v0 	#print returned value in v0
addi $v0, $0, 1		#print int
syscall

jal printReturn

add $a0, $0, $t0 	#print original t0
addi $v0, $0, 1		#print int
syscall

addi $v0, $0, 10 #exit function
syscall

addrec:
	addi $sp, $sp, -4 	#push
	sw $t0, 0($sp)		#save t0's value so we can use register
	lw $t0, 4($sp) 	#retrieve passed parameter

	bgt $t0, 0, else 	#if(t0 = 0) 
		addi $v0, $0, 0 #return 0 
		lw $t0, 0($sp) 	#restore t0's original value
		addi $sp, $sp, 4 	#pop
		jr $ra 	#return out of function

	else:
		addi $sp, $sp, -4 	#push
		sw $ra, 0($sp) 		#store ra for later

		addi $sp, $sp, -4 	#push

		sw $t0, 0($sp) 		#store n

		addi $sp, $sp, -4 	#push
		addi $t0, $t0, -1	#t0 -= 1
		sw $t0, 0($sp) 		#store n-1

		jal addrec

		#restore t0 and ra from stack
		lw $ra, 8($sp)
		lw $t0, 4($sp)
		add $v0, $v0, $t0	#v0 = v0 + n

		lw $t0, 12($sp) #restore t0's original value

		addi $sp, $sp, 16 	#pop

		jr $ra


printComma:
	addi $v0, $0, 4
	la $a0, comma
	syscall
	jr $ra
	
printReturn:
	addi $v0, $0, 4
	la $a0, nl
	syscall
	jr $ra