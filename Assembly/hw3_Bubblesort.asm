#Sam Arutyunyan
.data

msg: .asciiz ","
.globl main
.align 2 #tells that we are aligning address in sets of 4
array: .space 40	# //enough for 10 elements. 
#array: .word 3, 2, 5, 6, 1	#//initialize values (for testing before turn-in)
.text
main:
#how many elements:
	addi $v0, $0, 5 #5 = get input
	syscall			#checks $v0 for command, in this case takes input
	add $t9, $0, $v0 #save input value to some register, t0 in this case	
	
	add $t8, $0, $t9 #t8 = n - 1
	addi $t8, $t9, -1
takeInput:
	addi $t1, $0, 0 		#set int i
	la $s0, array #load array into s0, s0 will increment and load into a0
	forStart3:		
		beq $t1, $t9, forExit3
		
		addi $v0, $0, 5 #5 = get input
		syscall			#checks $v0 for command, in this case takes input
		add $t0, $0, $v0 #save input value to some register, t0 in this case	
		sw $t0, 0($s0);
	
		addi $t1, $t1, 1 	#i++
		addi $s0, $s0, 4	#this is i++ to the memory
		b forStart3			#restart for loop
	forExit3:
	

sort:
addi $t0, $0, 1#t0 = swap?
addi $v0, $0, 1

whileStart:
	beq $t0, 0, whileExit	#check while condition

	addi $t0, $0, 0			#set swap to false
	addi $t1, $0, 0 		#set int i
	la $s0, array #load array into s0, s0 will increment and load into a0
	forStart:
		beq $t1, $t8, forExit 	#check for loop condition. (5 elements, index 0 to 4.)
			
		lw $t2, 0($s0)		#a[i]
		lw $t3, 4($s0)		#a[i+1]
 
		#if a[i] > a[i+1], swap
		ble $t2, $t3 ifExit
			sw $t3, 0($s0) 	#store whatever is in $t3 to address location stored at $a0
			sw $t2, 4($s0)
			addi $t0, $0, 1 #swapped = true
		ifExit:
		
		addi $t1, $t1, 1 	#i++
		addi $s0, $s0, 4	#this is i++ to the memory
		
		b forStart			#restart for loop
	forExit:
	b whileStart			#restart while loop
whileExit:

printResult:
	addi $t1, $0, 0 		#set int i
	addi $v0, $0, 1
	la $s0, array #load array into s0, s0 will increment and load into a0
	forStart2:		
		beq $t1, $t9, forExit2
		
		lw $a0, 0($s0)
		syscall		
		addi $t1, $t1, 1 	#i++
		addi $s0, $s0, 4	#this is i++ to the memory
		b forStart2			#restart for loop
	forExit2:

addi $v0, $0, 10 #exit function
syscall