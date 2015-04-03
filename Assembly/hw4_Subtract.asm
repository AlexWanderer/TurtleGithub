
#t0 = a, t1 = b
.data
	.globl main
	comma: .asciiz ","
	return: .asciiz "\n"
	
.text
	main: 
	addi $v0, $0, 12 #//get first char
	syscall			#checks $v0 for command, in this case takes input

	add $t0, $0, $v0 #save input value to some register, t0 in this case
	addi $v0, $0, 12
	syscall			#checks $v0 for command, in this case takes input
	add $t1, $0, $v0 #save input value to t1
	
	#begin subroutine
	addi $sp, $sp, -8 	#since we’ll have 2 parameters
	sw $t0, 4($sp)
	sw $t1, 0($sp) 		#store a second parameter
	jal subtract;
	addi $sp, $sp, 8 		#pop
	add $t3, $0, $v0	#$v0 is conventional storage for return value. 
	#end subroutine
	
	jal printReturn
	add $a0, $0, $t3 	#print a - b
	addi $v0, $0, 1 	#print int
	syscall 
		
	addi $v0, $0, 10 #exit function
	syscall
	
	printComma:
	addi $v0, $0, 4
	la $a0, comma
	syscall
	jr $ra
	
	printReturn:
	addi $v0, $0, 4
	la $a0, return
	syscall
	jr $ra
	
	subtract:
	lw $t4, 4($sp) #retrieve a
	lw $t5, 0($sp) #retrieve b
	
	sub $v0, $t4, $t5 #v0 = a - b
	jr $ra