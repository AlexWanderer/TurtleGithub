.data

msg1: .asciiz "task1 complete\n"
msg2: .asciiz "task2 complete\n"

.globl main
.align 2 #tells that we are aligning address in sets of 4 bytes. 

tcb1: .space 128
tcb2: .space 128
tid: .space 4

.text

main:
#;tid = 1
la $a0, tid
addi $t0, $0, 1
sw $t0, 0($a0)

la $a0, tcb1
	la $t0, task1
	sw $t0, 96($a0) #;TCB1's initial ra
la $a0, tcb2
	la $t0, task2
	sw $t0, 96($a0) #;TCB2's initial ra

j task1

task1:
	jal TS
	addi $v0, $0, 4 #print
	la $a0, msg1
	syscall
	b task1
	
task2:
	jal TS
	addi $v0, $0, 4 #print
	la $a0, msg2
	syscall
	b task2
	
TS:
#;if(tid == 1)
addi $sp, $sp, -4 	#push ;store a0 on stack so we can use for accessing TCB
sw $a0, 0($sp) 
la $a0, tid #;(tid is 1 or 2, we load it’s address so we can write to it)
lw $a0, 0($a0) #;loading location with 0 offset
beq $a0, 2, tidWas2
#;if(tid == 1)
	#;store cpu state to tcb1
	la $a0, tcb1
	sw $ra, 96($a0)
	sw $v0, 0($a0)
	sw $v1, 4($a0)
	sw $a1,	12($a0)
	sw $a2,	16($a0)
	sw $a3,	20($a0)
	sw $t0,	24($a0)
	sw $t1,	28($a0)
	sw $t2,	32($a0)
	sw $t3,	36($a0)
	sw $t4,	40($a0)
	sw $t5,	44($a0)
	sw $t6,	48($a0)
	sw $t7,	52($a0)
	sw $t8,	56($a0)
	sw $t9,	60($a0)
	sw $s0,	64($a0)
	sw $s1,	68($a0)
	sw $s2,	72($a0)
	sw $s3,	76($a0)
	sw $s4,	80($a0)
	sw $s5,	84($a0)
	sw $s6,	88($a0)
	sw $s7,	92($a0)
	
	lw $t0, 0($sp) #;retrieve a0's old value from stack to store in our snapshot
	sw $t0, 8($a0)
	addi $sp, $sp, 4 	#pop
	#;tid = 2;
	la $a0, tid
	addi $t0, $0, 2
	sw $t0, 0($a0)
	#;restore cpu state based on tcb2; //task1 is now ready to run
	la $a0, tcb2
	lw $ra, 96($a0)
	lw $v0, 0($a0)
	lw $v1, 4($a0)
	lw $a1,	12($a0)
	lw $a2,	16($a0)
	lw $a3,	20($a0)
	lw $t0,	24($a0)
	lw $t1,	28($a0)
	lw $t2,	32($a0)
	lw $t3,	36($a0)
	lw $t4,	40($a0)
	lw $t5,	44($a0)
	lw $t6,	48($a0)
	lw $t7,	52($a0)
	lw $t8,	56($a0)
	lw $t9,	60($a0)
	lw $s0,	64($a0)
	lw $s1,	68($a0)
	lw $s2,	72($a0)
	lw $s3,	76($a0)
	lw $s4,	80($a0)
	lw $s5,	84($a0)
	lw $s6,	88($a0)
	lw $s7,	92($a0)
	
	lw $a0, 8($a0) #;a0's original value restored, can't use after this point
	
b tidWas1 #;skip else block

#;else if (tid == 2)
tidWas2:
	#;store cpu state to tcb2
	la $a0, tcb2
	sw $ra, 96($a0)
	sw $v0, 0($a0)
	sw $v1, 4($a0)
	sw $t0, 24($a0)
	sw $a1,	12($a0)
	sw $a2,	16($a0)
	sw $a3,	20($a0)
	sw $t0,	24($a0)
	sw $t1,	28($a0)
	sw $t2,	32($a0)
	sw $t3,	36($a0)
	sw $t4,	40($a0)
	sw $t5,	44($a0)
	sw $t6,	48($a0)
	sw $t7,	52($a0)
	sw $t8,	56($a0)
	sw $t9,	60($a0)
	sw $s0,	64($a0)
	sw $s1,	68($a0)
	sw $s2,	72($a0)
	sw $s3,	76($a0)
	sw $s4,	80($a0)
	sw $s5,	84($a0)
	sw $s6,	88($a0)
	sw $s7,	92($a0)

	lw $t0, 0($sp) #;retrieve a0's old value from stack to store in our snapshot
	sw $t0, 8($a0)
	addi $sp, $sp, 4 	#pop
	#;tid = 1;
	la $a0, tid
	addi $t0, $0, 1
	sw $t0, 0($a0)
	#;restore cpu state based on tcb1; //task2 is now ready to run
	la $a0, tcb1
	lw $ra, 96($a0)
	lw $v0, 0($a0)
	lw $v1, 4($a0)
	lw $a1,	12($a0)
	lw $a2,	16($a0)
	lw $a3,	20($a0)
	lw $t0,	24($a0)
	lw $t1,	28($a0)
	lw $t2,	32($a0)
	lw $t3,	36($a0)
	lw $t4,	40($a0)
	lw $t5,	44($a0)
	lw $t6,	48($a0)
	lw $t7,	52($a0)
	lw $t8,	56($a0)
	lw $t9,	60($a0)
	lw $s0,	64($a0)
	lw $s1,	68($a0)
	lw $s2,	72($a0)
	lw $s3,	76($a0)
	lw $s4,	80($a0)
	lw $s5,	84($a0)
	lw $s6,	88($a0)
	lw $s7,	92($a0)
	
	lw $a0, 8($a0) #;a0's original value restored, can't use after this point
	
tidWas1:

jr $ra


	