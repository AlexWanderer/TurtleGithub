/*
 * Author: Samuel Arutyunyan
 * Purpose: This program plays Battleship against the user.
 * Date: November 26, 2012
 */

#include <iostream>
#include <fstream>
#include <string>
#include <ctime>

using namespace std;

enum HittingTarget//tracks which ship we are hitting
{
	Neutral,//no ships
	Attacking//in process of sinking a ship

};

enum AlignmentDetected
{
	Unknown,
	Horizontal,
	Vertical
};

void SetupPlayerBoard(char playerBoard[11][12]);
void SetupEnemyBoard(char enemyReal[11][12], char enemyVisible[11][12]);
void DisplayBoards(char playerBoard[11][12], char enemyVisible[11][12]);
void PlayerAttack(char enemyReal[11][12], char enemyVisible[11][12], int enemyShipHits[5]);
void EnemyAttack(char playerBoard[11][12], int playerShipHits[5], AlignmentDetected &alignment, HittingTarget &hittingState, int &lastRow, int &lastColumn);
void PlayGame(char playerBoard[11][12], char enemyReal[11][12], char enemyVisible[11][12]);

int main()
{
	srand(time(0));

	//enemy's board of ships
	char enemyReal[11][12];
	//the enemy board that the player can actually see.
	char enemyVisible[11][12];
	SetupEnemyBoard(enemyReal, enemyVisible);//sets the enemy board from a file ships.txt
	

	//the player's board of ships. (10 for the board and 2 for the number spacing)
	char playerBoard[11][12];
	SetupPlayerBoard(playerBoard);//sets up the board (gets input from user)
	
	PlayGame(playerBoard, enemyReal, enemyVisible);
	

	cout << endl << "Game over. Thanks for playing..." << endl << endl;

}//End Main


/***************************
** SetupPlayerBoard() **
* Allows Player to input his ships into a 2d array:
* @param - playerBoard - The 2d array/board of ships which belong to the player
*/
void SetupPlayerBoard(char playerBoard[11][12])
{
	/*
	//creates template grid for testing
	char newPlayerBoard[11][12] =
	{
		{' ', ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' },
		{' ', '1', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '2', ' ', 'B', ' ', ' ', ' ', 'C', ' ', ' ', ' ', ' ' },
		{' ', '3', ' ', 'B', ' ', ' ', ' ', 'C', 'D', 'D', ' ', ' ' },
		{' ', '4', ' ', 'B', ' ', ' ', ' ', 'C', ' ', ' ', ' ', ' ' },
		{' ', '5', ' ', 'B', ' ', ' ', ' ', ' ', ' ', ' ', 'S', ' ' },
		{' ', '6', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'S', ' ' },
		{' ', '7', ' ', ' ', 'A', 'A', 'A', 'A', 'A', ' ', 'S', ' ' },
		{' ', '8', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '9', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{'1', '0', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' }
	};
	*/
	//creates template grid
	char newPlayerBoard[11][12] =
	{
		{' ', ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' },
		{' ', '1', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '2', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '3', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '4', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '5', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '6', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '7', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '8', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '9', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{'1', '0', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' }
	};

	//assign template grid to our board array
		for(int i=0;i<11;i++)
		{
			for(int j=0; j<12; j++)
			{
				playerBoard[i][j] = newPlayerBoard[i][j];
			}	
		}  


	//iterate through each ship
	string name;
	int spaces;
	char icon; //designated icon for ship. eg: Carrier is 'A'
	int row;	
	char charColumn;

	for(int cnt = 0; cnt < 5; cnt++)
	{
		cout << "##Setting Up Player Board##" << endl;
		cout << "--All ships are laid out to the right and down from their starting position--" << endl << endl;	

		//cout << "cnt is: " << cnt << endl;//FOR DEBUGGING
		switch(cnt)
		{
		case 0:
			name = "Carrier";
			spaces = 5;
			icon = 'A';
			break;
		case 1:
			name = "Battleship";
			spaces = 4;
			icon = 'B';
			break;
		case 2:
			name = "Submarine";
			spaces = 3;
			icon = 'S';
			break;
		case 3:
			name = "Cruiser";
			spaces = 3;
			icon = 'C';
			break;
		case 4:
			name = "Destroyer";
			spaces = 2;
			icon = 'D';
			break;
		}

		//displays board
		for(int i=0;i<11;i++)
		{
			for(int j=0; j<12; j++)
			{
				cout << playerBoard[i][j];
			}
			cout << endl;
		}

		cout << endl << "Enter the starting row position(1-10) for your " << " " << name << ". (" << spaces << " spaces)";
		cin >> row;
		while( !(row == 1 || row == 2 || row == 3 || row == 4 || row == 5 || row == 6 || row == 7 || row == 8 || row == 9 || row == 10) )
		{
			cin.clear();
			cin.ignore();
			cout << "valid input is 1-10." << endl;
			cout << endl << "Enter the starting row position(1-10) for your " << " " << name << ". (" << spaces << " spaces)";
			cin >> row;
		}

		cout << endl << "Enter the starting column position(A-J) for your" << " " << name << ". (" << spaces << " spaces)";
		cin >> charColumn;
		charColumn = toupper(charColumn);
		while( !(charColumn == 'A' || charColumn == 'B' || charColumn == 'C' || charColumn == 'D' || charColumn == 'E' || charColumn == 'F' || charColumn == 'G' || charColumn == 'H' || charColumn == 'I' || charColumn == 'J') )
		{
			cout << "valid input is A-J" << endl;
			cout << endl << "Enter the starting column position(A-J) for your" << " " << name << ". (" << spaces << " spaces)";
			cin >> charColumn;
			charColumn = toupper(charColumn);
		}

		int column = charColumn;//will be assigned based on the character entered for column
		column -= 64;//ascii A is 65, we want it to be 1
		//cout << "the row is: " << row << endl;//FOR DEBUGGING
		//cout << "the col is: " << charColumn << endl;//FOR DEBUGGING
		
		char alignment;//vertical or horizontal
		cout << "Please indicate ship alignment: 'h' for horizontal. 'v' for vertical." << endl;
		cin >> alignment;
		alignment = toupper(alignment);

		while (!(alignment == 'H' || alignment == 'V'))
		{
			cout << "Invalid input, try again." << endl;
			cout << "Please indicate ship alignment: 'h' for horizontal. 'v' for vertical." << endl;
			cin >> alignment;
			alignment = toupper(alignment);
		}	

		bool shipPlaced = true;//if we successfully place a ship

		//make sure the ship fits on the board
		if(alignment == 'H')
		{
			//cout << "in horizontal alignment: " << endl; //FOR DEBUGGING	
			if( (10 - column) <= spaces )//our grid max is 10
			{
				cout << endl <<  "The ship won't fit on the board!" << endl << endl;//need to return to top of for loop 
				cnt--;//and keep counter from increasing
				continue;
			}
			//cout << "successfully placed ship." << endl; //FOR DEBUGGING
			//ship position was successful:
			
			for(int cnt = 0; cnt < spaces; cnt++)//check if we are overlapping another ship
			{
				if(playerBoard[row][column + cnt + 1] != ' ')//if spot is not blank
				{					
					shipPlaced = false;
					break;
				}				
			}

			if(shipPlaced != false)//if we did not overlap
			{
				for(int cnt = 0; cnt < spaces; cnt++)//now place the ship
				{
					playerBoard[row][column + cnt + 1] = icon;
				}
			}
		}

		
		if(alignment == 'V')
		{
			//cout << "in vertical alignment: " << endl; //FOR DEBUGGING
			if( (10 - row) <= spaces )//our grid max is 10
			{
				cout << endl << "The ship won't fit on the board!" << endl <<endl;//need to return to top of for loop 
				cnt--;//and keep counter from increasing
				continue;
			}
			//cout << "successfully placed ship?" << endl; //FOR DEBUGGING
			//ship position was successful:
			for(int cnt = 0; cnt < spaces; cnt++)//check if we are overlapping another ship
			{
				if(playerBoard[row + cnt][column + 1] != ' ')//if spot is not blank
				{
					shipPlaced = false;
					break;
				}				
			}
			if(shipPlaced != false)//if we did not overlap
			{
				for(int cnt = 0; cnt < spaces; cnt++)//now place the ship
				{
					playerBoard[row + cnt][column + 1] = icon;
				}
			}
		}

		if (shipPlaced == false)
		{
			cout << endl << "There is another ship in the way" << endl << endl;
			cnt--;
			continue;//continue through the ship placing loop, redoing this current ship.
		}

	}//end for loop (through all ship types)

	//at this point we have all our ships assigned.

	cout << endl << "Your final Player board is: " << endl << endl;
	//displays board
		for(int i=0;i<11;i++)
		{
			for(int j=0; j<12; j++)
			{
				cout << playerBoard[i][j];
			}
			cout << endl;
		}

	cout << "Beginning Battle..." << endl << endl;
}//end SetupPlayerBoard() function

/***************************
** SetupEnemyBoard() **
* Reads 2d array of enemy ships from a file and assings to a 2d array variable:
* @param - enemyReal - The real 2d array which contains the positions of the enemy ships
* @param - enemyVisible - The enemy board which the player sees as they attack (it starts out blank)
*/
void SetupEnemyBoard(char enemyReal[11][12],  char enemyVisible[11][12])
{
	ifstream computerShipFile("ships.txt");

	for(int i=0;i<11;i++)
	{
		for(int j=0; j<12; j++)
		{
			computerShipFile.get(enemyReal[i][j]); 
		}
		computerShipFile.ignore();// ignores \n
	}

	computerShipFile.close();

	char newEnemyBoard[11][12] =
	{
		{' ', ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' },
		{' ', '1', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '2', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '3', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '4', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '5', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '6', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '7', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '8', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{' ', '9', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
		{'1', '0', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' }
	};

	//assign template grid to enemy board array
		for(int i=0;i<11;i++)
		{
			for(int j=0; j<12; j++)
			{
				enemyVisible[i][j] = newEnemyBoard[i][j];
			}	
		}  
}

/***************************
** DisplayBoards() **
* outputs both the player and enemey's boards to the screen side by side
* @param - playerBoard - The 2d array/board of ships which belong to the player
* @param - enemyVisible - The enemy board which the player sees as they attack (it starts out blank)
*/
void DisplayBoards(char playerBoard[11][12], char enemyVisible[11][12])
{
	cout << endl << " Human Ships \t Enemy Ships" << endl << endl;
	for(int i=0;i<11;i++)
	{
		for(int j=0; j<12; j++)//A row of player board
		{
			cout << playerBoard[i][j];
		}
		cout << "\t";
		for(int j=0; j<12; j++)//A row of Enemy board
		{
			cout << enemyVisible[i][j];
		}
		cout << endl;
	}

	cout << endl << endl;
}

/***************************
** PlayerAttack() **
* Processes player's attack agains the enemy. The enemy board is updated, ships that are hit or sunk are calcualted, as well as misses.
* @param - enemyReal - The real 2d array which contains the positions of the enemy ships
* @param - enemyVisible - The enemy board which the player sees as they attack (it starts out blank)
* @param - enemyShipHits - array that holds the number of times each ship is hit, so that we know when one is sunk.
* @param - enemySunk - //keeps track of how many enemy ships have sunk, determines game win for the player
*/
void PlayerAttack(char enemyReal[11][12], char enemyVisible[11][12], int enemyShipHits[5], int &enemySunk)
{
	int attackRow;
	char charColumn;
	bool attackSuccess = false;//to check if we are hitting a space with a 'X' or an 'O'

	while(attackSuccess == false)
	{
		cout << "Enter the row number to shoot at: ";
		cin >> attackRow;
		while( !(attackRow == 1 || attackRow == 2 || attackRow == 3 || attackRow == 4 || attackRow == 5 || attackRow == 6 || attackRow == 7 || attackRow == 8 || attackRow == 9 || attackRow == 10) )
		{
			cin.clear();
			cin.ignore();
			cout << "valid input is 1-10." << endl;
			cout << "Enter the row number to shoot at: ";
			cin >> attackRow;
		}

		cout << "Enter the column letter to shoot at: ";
		cin >> charColumn;
		charColumn = toupper(charColumn);
		while( !(charColumn == 'A' || charColumn == 'B' || charColumn == 'C' || charColumn == 'D' || charColumn == 'E' || charColumn == 'F' || charColumn == 'G' || charColumn == 'H' || charColumn == 'I' || charColumn == 'J') )
		{
			cout << "valid input is A-J" << endl;
			cout << "Enter the column letter to shoot at: ";
			cin >> charColumn;
			charColumn = toupper(charColumn);
		}
		
		//this is a temp attackColumn for checking... i think O_o
		int attackColumn = charColumn;//the int value of the char
		attackColumn -= 64;//ascii A is 65, we want it to be 1

		//adjust attackColumn to fit our grid 12 length grid
		attackColumn += 1;//valid columns are 2 - 11

		if (enemyVisible[attackRow][attackColumn] == 'X' || enemyVisible[attackRow][attackColumn] == 'O')
			{ 
				cout << "You already Hit that location" << endl;
				continue; 
			}
		else { attackSuccess = true; }
	}//end while

	int attackColumn = charColumn;//the int value of the char
	attackColumn -= 64;//ascii A is 65, we want it to be 1
	//adjust attackColumn to fit our grid 12 length grid
	attackColumn += 1;//valid columns are 2 - 11

	//Check if we hit anything------------------

	bool sunkShip = false;//if we sunk an enemy ship
	//if we hit a ship
		enemyVisible[attackRow][attackColumn] = 'X';//assume we hit, and set this. (will change to 'O' at end of switch)
	switch(enemyReal[attackRow][attackColumn])
	{
		
	case 'A':
		enemyShipHits[0]++;
		if (enemyShipHits[0] >= 5)
			{ sunkShip = true; }
		break;
	case 'B':
		enemyShipHits[1]++;
		if (enemyShipHits[1] >= 4)
			{ sunkShip = true; }
		break;
	case 'S':
		enemyShipHits[2]++;
		if (enemyShipHits[2] >= 3)
			{ sunkShip = true; }
		break;
	case 'C':
		enemyShipHits[3]++;
		if (enemyShipHits[3] >= 3)
			{ sunkShip = true; }
		break;
	case 'D':
		enemyShipHits[4]++;
		if (enemyShipHits[4] >= 2)
			{ sunkShip = true; }
		break;
	case ' ':
		enemyVisible[attackRow][attackColumn] = 'O';//missed
		break;
	default:
		break;
	}

	if(enemyVisible[attackRow][attackColumn] == 'X')
	{
		cout << attackRow << charColumn << " was a hit!\t" ;
		if(sunkShip == true)
		{
			cout << endl << "You sank an enemy ship.";
			enemySunk++;
		}
		cout << endl;
	}
	else
		{ cout << endl << endl << "You shoot " << attackRow << charColumn << " and miss..." << endl; }

}

/***************************
** EnemyAttack() **
* Processes enemy's attack agains the player. The player board is updated, ships that are hit or sunk are calcualted, as well as misses.
* @param - playerBoard - The 2d array/board of ships which belong to the player
* @param - playerShipHits - array that holds the number of times each ship is hit, so that we know when one is sunk.
* @param - hittingState - Enum which keeps track of what kind of ship the enemy is hitting (so it better knows how to target) 
* @param - alignment - detects if the ship being attacked is in vertical or horizontal alignmen
* @param - lastRow - tracks the last row that was attacked in order to logically attack in same area.
* @param - lastColumn - tracks the last column that was attacked in order to logically attack in same area.
* @param - playerSunk - //keeps track of how many player ships have sunk, determines game win for the computer
*/
void EnemyAttack(char playerBoard[11][12], int playerShipHits[5], HittingTarget &hittingState, AlignmentDetected &alignment, int &lastRow, int &lastColumn, int &playerSunk)
{
	//cout << "-----------Beginning Enemy Attack...------------" << endl;//FOR DEBUGGING!-----------------------
	int attackRow;
	int attackColumn;
	char charColumn = 'Z'; //the char equivalent of the columns grid for displaying which grid slot was hit
	bool attackSuccess = false;//to check if we are hitting a space with a 'X' or an 'O'
	int fails = 0; //once fails goes above 5 we just do random because my ai failed

	while(attackSuccess == false)
	{	
		if(hittingState == Neutral || fails > 5)
		{
			//set attackRow and attackColumn to be randomly generated between 1 and 10 inclusive
			attackRow = (rand() % 11) + 1;
			attackColumn = (rand() % 11) + 1;
			attackColumn++; //adjust for 12 length grid. 
		}//end if(hittingState == Neutral || fails > 5)
		else if (hittingState == Attacking)
		{
			int direction;//will be set 0-3 to determine direction to set next attack row and column
			if(alignment == Unknown)
			{
				//hit random spot around last known hit
				direction = rand() % 4;
				switch(direction)
				{
				case 0:
					attackRow = lastRow;
					attackColumn = lastColumn - 1;//goes left one space
					break;
				case 1:
					attackRow = lastRow - 1;//goes north one space
					attackColumn = lastColumn;
					break;
				case 2:
					attackRow = lastRow;
					attackColumn = lastColumn + 1;//goes right one space
					break;
				case 3:
					attackRow = lastRow + 1;//goes down one space
					attackColumn = lastColumn;
					break;
				}
			}//end if(alignment == Unknown)
			else if (alignment == Horizontal)
			{
				bool left = true;//tries to move left otherwise will go to right
				while (fails < 5)
				{
					if (left)
					{
						//cout << "Attempting to move left" << endl;//FOR DEBUGGING!-----------------------
						if(playerBoard[lastRow][lastColumn] == 'O')
						//check both sides, and go the direction of the 'X'
						{
							if(playerBoard[lastRow][lastColumn + 1] == 'X')
							{
								left = false;
								continue;
							}
							else//we should be moving left
							{
								attackRow = lastRow;
								attackColumn = lastColumn - 1;
								while(true)
								{
									//if we're landing on an invalid spot, keep moving left
									if(playerBoard[attackRow][attackColumn] == 'X' || playerBoard[attackRow][attackColumn] == 'O')
									{
										fails++;
										attackColumn--;
										if(attackColumn < 2)
										{
											//bring it back to 2 and break out.
											attackColumn = 2;
											break;
										}
										continue;
										
									}
									else
									{
										break;
									}
								}
							}
							
						}//end if(playerBoard[lastRow][lastColumn] == 'O')
						else//we're standing one an X. on of the directions should be blank
						{
							if(playerBoard[lastRow][lastColumn + 1] == 'X')
								{
									left = false;
									continue;
								}
								else//we should be moving left
								{	
									attackRow = lastRow;
									attackColumn = lastColumn - 1;
									while(true)
									{
										//if we're landing on an invalid spot, keep moving left
										if(playerBoard[attackRow][attackColumn] == 'X' || playerBoard[attackRow][attackColumn] == 'O')
										{
											attackColumn--;
											if(attackColumn < 2)
											{
												//bring it back to 2 and break out.
												attackColumn = 2;
												break;
											}
											continue;
										
										}
										else
										{

											break;
										}
									}
								}
						//if we reach this part of the while loop, break out
							break;
						}//end else we're standing one an X
					}//end if (left)
					else//we're moving right
					{
						//cout << "Attempting to move Right" << endl;//FOR DEBUGGING!-----------------------
						attackRow = lastRow;
						attackColumn = lastColumn + 1;
						while(true)
						{
									//if we're landing on an invalid spot, keep moving right
									if(playerBoard[attackRow][attackColumn] == 'X' || playerBoard[attackRow][attackColumn] == 'O')
									{
										fails++;
										attackColumn++;
										if(attackColumn > 11)
										{
											//bring it back to 11 and break out.
											attackColumn = 11;
											break;
										}
										continue;
										
									}
									else
									{
										break;
									}
						}
					}//end else we're moving right
				}//end while (true). above if(left)
					
			}//end else if(alignment == Horizontal)
			else if (alignment == Vertical)
			{
				//cout << "Alignment is Vertical" << endl;//FOR DEBUGGING!-----------------------
				//hit random up or down of the last hit spot---------------------

				bool up = true;//tries to move left otherwise will go to right
				while (fails < 5)
				{
					if (up)
					{
						//cout << "Attempting to move Up" << endl;//FOR DEBUGGING!-----------------------
						if(playerBoard[lastRow][lastColumn] == 'O')
						//check both sides, and go the direction of the 'X'
						{
							if(playerBoard[lastRow + 1][lastColumn] == 'X')
							{
								up = false;
								continue;
							}
							else//we should be moving up
							{
								attackRow = lastRow  - 1;
								attackColumn = lastColumn;
								while(true)
								{
									//if we're landing on an invalid spot, keep moving up
									if(playerBoard[attackRow][attackColumn] == 'X' || playerBoard[attackRow][attackColumn] == 'O')
									{
										fails++;
										attackRow--;
										if(attackRow < 1)
										{
											//bring it back to 1 and break out.
											attackRow = 1;											
											break;
										}
										continue;
										
									}
									else
									{
										fails++;
										break;
									}
								}
							}
							
						}//end if(playerBoard[lastRow][lastColumn] == 'O')
						else//we're standing one an X. on of the directions should be blank
						{
							if(playerBoard[lastRow + 1][lastColumn] == 'X')
								{
									up = false;
									continue;
								}
								else//we should be moving up
								{	
									attackRow = lastRow - 1;
									attackColumn = lastColumn;
									while(true)
									{
										//if we're landing on an invalid spot, keep moving left
										if(playerBoard[attackRow][attackColumn] == 'X' || playerBoard[attackRow][attackColumn] == 'O')
										{
											fails++;
											attackRow--;
											if(attackRow < 1)
											{
												//bring it back to 2 and break out.
												attackRow = 1;												
												break;
											}
											continue;
										
										}
										else
										{
											fails++;
											break;
										}
									}
								}
						
						}
					}//end if(up)
					else//we're moving down
					{
						//cout << "Attempting to move Down" << endl;//FOR DEBUGGING!-----------------------
						attackRow = lastRow + 1;
						attackColumn = lastColumn;
						while(true)
						{
								//if we're landing on an invalid spot, keep moving right
								if(playerBoard[attackRow][attackColumn] == 'X' || playerBoard[attackRow][attackColumn] == 'O')
								{
									fails++;
									attackRow++;
									if(attackRow > 10)
									{
										//bring it back to 11 and break out.
										attackRow = 10;		
										break;
									}
									continue;
										
								}
								else
								{
									fails++;
									break;
								}
						}
					}//end else down
				}//end while(true)

				//------------------
			}//end else if (alignment == Vertical)
		}//end else if (hittingState == Attacking)
		//cout << "Finished setting attack coords, checking if they are valid." << endl;//FOR DEBUGGING!-----------------------
		
		//make sure we did not go out of the grid
		if(attackRow < 1 || attackRow > 10 || attackColumn < 2 || attackColumn > 11)
		{
			//out of bounds, restart loop (hitting randomly until we successfully hit a good spot)
			fails++;
			continue;
		}

		//make sure it did not choose an area that was already hit
		if (playerBoard[attackRow][attackColumn] == 'X' || playerBoard[attackRow][attackColumn] == 'O')
			{ 
				//this location was already hit, starting loop again
				fails++;
				continue; 
			}
		else { attackSuccess = true; }
	}//end while (attack was successful)

	//finish figuring out attack (after we've checked for all possible attacking states
	charColumn = attackColumn -1 + 64;//ascii A is 65. -1 for the 12 grid offset
	//this is for displaying to screen

	//cout << "-------About to Check for hits------" << endl;//FOR DEBUGGING!-----------------------
	//Check if we hit anything------------------

	bool sunkShip = false;//if we sunk an enemy ship
	//if we hit a ship
	switch(playerBoard[attackRow][attackColumn])
	{
	case ' ':
		playerBoard[attackRow][attackColumn] = 'O';//missed
		break;
	case 'A':
		playerBoard[attackRow][attackColumn] = 'X';
		playerShipHits[0]++;
		if (playerShipHits[0] >= 5)
			{ sunkShip = true; }
		break;
	case 'B':
		playerBoard[attackRow][attackColumn] = 'X';
		playerShipHits[1]++;
		if (playerShipHits[1] >= 4)
			{ sunkShip = true; }
		break;
	case 'S':
		playerBoard[attackRow][attackColumn] = 'X';
		playerShipHits[2]++;
		if (playerShipHits[2] >= 3)
			{ sunkShip = true; }
		break;
	case 'C':
		playerBoard[attackRow][attackColumn] = 'X';
		playerShipHits[3]++;
		if (playerShipHits[3] >= 3)
			{ sunkShip = true; }
		break;
	case 'D':
		playerBoard[attackRow][attackColumn] = 'X';
		playerShipHits[4]++;
		if (playerShipHits[4] >= 2)
			{ sunkShip = true; }
		break;
	
	}

	if(playerBoard[attackRow][attackColumn] == 'X')
	{
		//Ifwe've hit, lets detect alignment:
		if(playerBoard[attackRow][attackColumn + 1] == 'X' || playerBoard[attackRow][attackColumn - 1] == 'X'){ alignment = Horizontal;}
		else if(playerBoard[attackRow + 1][attackColumn] == 'X' || playerBoard[attackRow - 1][attackColumn] == 'X'){ alignment = Vertical;}
		hittingState = Attacking;//we hit something, go to attacking state
		cout << attackRow << charColumn << " was a hit!\t" ;
		
		if(sunkShip == true)
		{
			hittingState = Neutral;//set back to neutral since it finished off our ship
			playerSunk++;//keeps track of how many player ships have sunk, determines game win
			cout << "The computer sank one of your ships.";
		}
		cout << endl;
	}
	else
		{ cout << "The Computer shoots " << attackRow << charColumn << " and misses..." << endl; }

	//set last row and last column for next round:
	//Should not be changed if we missed
	if(playerBoard[attackRow][attackColumn] == 'X')
	{
		lastRow = attackRow;
		lastColumn = attackColumn;
	}
}

/***************************
** PlayGame() **
* Plays the game. Allows player and computer to take turns shooting at eachother, updates the display, and determines a winner. 
* @param - playerBoard - The 2d array/board of ships which belong to the player
* @param - enemyReal - The real 2d array which contains the positions of the enemy ships
* @param - enemyVisible - The enemy board which the player sees as they attack (it starts out blank)
*/
void PlayGame(char playerBoard[11][12], char enemyReal[11][12], char enemyVisible[11][12])
{
	HittingTarget hittingState = Neutral;//the state of our enemy for attacking our ship
	AlignmentDetected alignment = Unknown;
	int lastRow;//tracks the last row that enemy attacked
	int lastColumn;//tracks the last row that enemy attacked

	//this array keep track of the various player ships hits to see if they've been sunk
	int playerShipHits[5] = { 0, 0, 0, 0, 0 };//Carrier, Battleship, Submarine, Cruiser, Destroyer.
	//this array keep track of the various enemy ships hits to see if they've been sunk
	int enemyShipHits[5] = { 0, 0, 0, 0, 0 };//Carrier, Battleship, Submarine, Cruiser, Destroyer.
	//keeps track of how many player ships have sunk, determines game win for enemy
	int playerSunk = 0;
	//keeps track of how many enemy ships have sunk, determines game win for player
	int enemySunk = 0;

	bool runGame = true;
	
	//game loop continues until either playerSunk or enemySunk has reached 5
	while(runGame)
	{
		if(playerSunk == 5)
		{
			runGame = false;
			cout << endl << "All your ships are destroyed. The Computer has won." << endl;
			break;
		}
		else if(enemySunk == 5)
		{
			runGame = false;
			cout << endl<< "You have destroyed all enemy ships. You Win!" << endl;
			break;
		}

		DisplayBoards(playerBoard, enemyVisible);

		PlayerAttack(enemyReal, enemyVisible, enemyShipHits, enemySunk);

		if(playerSunk == 5)
		{
			runGame = false;
			cout << endl << "All your ships are destroyed. The Computer has won." << endl;
			break;
		}
		else if(enemySunk == 5)
		{
			runGame = false;
			cout << endl << "You have destroyed all enemy ships. You Win!" << endl;
			break;
		}

		EnemyAttack(playerBoard, playerShipHits, hittingState, alignment, lastRow, lastColumn, playerSunk);

	
	}

}