/*
 * Author: Samuel Arutyunyan
 * Purpose: This program plays an adventure game
 * Date: November 07, 2012
 */

#include <iostream>
#include <fstream>
#include <string>
#include <ctime>
using namespace std;



struct Coord
{
	int x;
	int y;
};

template <size_t T>
void ProcessMove(char visibleBoard[][T], struct Coord *playerPos);
template <size_t T>
void CheckWalls(char realBoard[][T], char visibleBoard[][T], Coord *playerPos);//for drawing
template <size_t T>
void CheckWalls(string *moveArray, char visibleBoard[][T], struct Coord *playerPos);//for movement
template <size_t T>
void DrawBoard(char realBoard[][T], char visibleBoard[][T], Coord *playerPos, int boardWidth, int boardHeight);
template <size_t T>
void RedrawBoard(char visibleBoard[][T], int boardWidth, int boardHeight);
void DisplayWelcome();

int main()
{
	srand(time(0));
	bool dead = false;//players living state
	int gold = 0; //bags of gold?
	bool runGame = true;
	bool sword = false;//true when we obtain sword. 
	DisplayWelcome();

	//Setup our board: -------------------------------
	//our boards need extra padding for thsi code to work right. thats why its 30 for a limit of 20
	char realBoard[30][30];//our map from file
	char visibleBoard[30][30];//what the character can see
	Coord playerPos;
	playerPos.x = -1;//initialize a value
	playerPos.y = -1;

	for(int i=0;i<20;i++)
	{
		for(int j=0; j<20; j++)
		{
			visibleBoard[i][j] = ' ';
		}
	}

	ifstream inputStream("board.txt");
	int boardWidth = 0;
	int boardHeight = 0;
	inputStream >> boardWidth;
	inputStream >> boardHeight;
	//cout << boardWidth << " " << boardHeight << endl;
	inputStream.ignore();// ignores \n	

	

	for(int i=0;i<boardWidth;i++)
	{
		for(int j=0; j<boardHeight; j++)
		{
			inputStream.get(realBoard[i][j]); 
		}
		inputStream.ignore();// ignores \n
	}
	
	inputStream.close();
	//Done: setting up board -----------------------------

	/*
	for(int i=0;i<boardWidth;i++)
	{
		for(int j=0; j<boardHeight; j++)
		{
			cout << realBoard[i][j];
		}
		cout << endl;
	} */ //for debugging

	DrawBoard(realBoard, visibleBoard, &playerPos, boardWidth, boardHeight);

	while(runGame && !dead)
	{
		
		ProcessMove(visibleBoard, &playerPos);

		cout << "player position: " << playerPos.x << "," << playerPos.y << " board width/height: " << boardWidth << "/" << boardHeight << endl;


		//if we've gotten to the edge of the map. -1 for index starting at 0, -1 for edge of map. < 1 as opposed to < 0
		if(playerPos.x > boardWidth - 2 || playerPos.y > boardHeight - 2 || playerPos.x < 1 || playerPos.y < 1)
		{
			runGame = false;
			cout << "\n\nYou escaped the maze! congrats" << endl;
			break;
		}

		

		//add walls to visible board
		CheckWalls(realBoard, visibleBoard, &playerPos);

		//set player on board:
		visibleBoard[playerPos.y][playerPos.x] = 'U';

		RedrawBoard(visibleBoard, boardWidth, boardHeight);

		//if we picked up a sword
		if(realBoard[playerPos.y][playerPos.x] == 'S')
		{
			cout << "You've picked up a sword. This should come in handy..." << endl;
			sword = true;
		}

		//if we picked up gold
		if(realBoard[playerPos.y][playerPos.x] == 'G')
		{
			cout << "You found some gold...its very shiny" << endl;
			gold++;
		}

		//if we ran into a kobold
		if(realBoard[playerPos.y][playerPos.x] == 'K')
		{
			float battleChance = rand()%101; 
			cout << "You've run into a tiny Kobold. It viciously attacks you!" << endl;
			if(sword)//we have sword
			{
				if(battleChance <= 99)
				{
					cout << "You sliced the little critter apart with your sword" << endl;
				}
				else
				{
					cout << "The kobold was surprisingly strong, it gnawed you to death" << endl;
					dead = true;
					break;

				}
			}
			else//no sword
			{
				if(battleChance <= 75)
				{
					cout << "You fight off the kobold and it scurries away." << endl;
				}
				else
				{
					cout << "The kobold was surprisingly strong, it gnawed you to death" << endl;
					dead = true;
					break;
				}
			}
		}

		//if we ran into an ogre
		if(realBoard[playerPos.y][playerPos.x] == 'O')
			{
			float battleChance = rand()%101; 
			cout << "You've run into a bulky ogre. He charges towards you" << endl;
			if(sword)//we have sword
			{
				if(battleChance <= 96)
				{
					cout << "You heroicly run your blade through the ogre" << endl;
				}
				else
				{
					cout << "The ogre bashed you to death with his club" << endl;
					dead = true;
					break;
				}
			}
			else//no sword
			{
				if(battleChance <= 20)
				{
					cout << "You barely manage to defeat the ogre" << endl;
				}
				else
				{
					cout << "The ogre is carying your lifeless body to his camp, its his dinner time after all...." << endl;
					dead = true;
					break;
				}
			}
		}

		//ran into a dragon?
		if(realBoard[playerPos.y][playerPos.x] == 'D')
		{
			cout << "You thought you saw a dragon for a split second. But now theres flames everywhere....And your dead!" << endl;
			dead = true;
			break;
		}

		//fall into a pit
		if(realBoard[playerPos.y][playerPos.x] == 'P')
		{
			cout << "You fell into a pit, oh no!!!! (its a very deep pit too.. not the kind that you can crawl out of)" << endl;
			dead = true;
			break;
		}

	}

	if(dead)
	{
		cout << "Game over. Thanks for playing.." << endl;
	}
	else
	{
		cout << "You have " << gold << " bags of gold." << endl;
	}

}//end main()



/***************************
** ProcessMove() **
* takes input from user and figures out which way to move the character by assigning its position.
* @param - visibleBoard - The board which is visible to the player.
* @param - playerPos - Coord structure object with a x and y which determines player position.
*/
template <size_t T>
void ProcessMove(char visibleBoard[][T], struct Coord *playerPos)
{
	char input;
	string moveArray = "";//holds allowed directions to move

	CheckWalls(&moveArray, visibleBoard, playerPos);//assigns n,e,s,w to array
	
	cout << endl << "Enter a direction to move. <n, s, e, w>" << endl;
	//cout << "Possible ones are: " << moveArray << endl;
	cin >> input;

	
	//validate input
	while(moveArray.find(input) == string::npos )
	{		
		if (input == 'n' || input == 's' || input == 'w' || input == 'e')
			cout << "You can't walk through a wall." << endl;
		else
		{
			cout << "Only n, s, e, and w, are valid directions to move in." << endl;
		}
		
		cout << "Enter a direction to move. <n, s, e, w>";
		cin >> input;
	
	}

	//now move char...
	//delete old player:
	visibleBoard[playerPos->y][playerPos->x] = ' ';
	//draw new one
	switch(input)
	{
	case 'n':
		playerPos->y -= 1;
		break;
	case 's':
		playerPos->y += 1;
		break;
	case 'e':
		playerPos->x += 1;
		break;
	case 'w':
		playerPos->x -= 1;
		break;
	
	}
}

/***************************
** CheckWalls() **
* Figures out if there are any walls near the player: U and adds them to the visibleBoard.
* @param - realBoard - The board which was read in from the file and contains all elements of the board/map.
* @param - visibleBoard - The board which is visible to the player.
* @param - playerPos - Coord structure object with a x and y which determines player position.
*/
template <size_t T>
void CheckWalls(char realBoard[][T], char visibleBoard[][T], struct Coord *playerPos)//check walls for display
{
	//x = j, y = i. they are reversed for visibleBoard

	if(realBoard[playerPos->y][playerPos->x + 1] == 'W')
	{
		visibleBoard[playerPos->y][playerPos->x + 1] = 'W';
	}

	if(realBoard[playerPos->y][playerPos->x - 1] == 'W')
	{
		visibleBoard[playerPos->y][playerPos->x - 1] = 'W';
	}

	if(realBoard[playerPos->y + 1][playerPos->x] == 'W')
	{
		visibleBoard[playerPos->y + 1][playerPos->x] = 'W';
	}

	if(realBoard[playerPos->y - 1][playerPos->x] == 'W')
	{
		visibleBoard[playerPos->y - 1][playerPos->x] = 'W';
	}
}

/***************************
** CheckWalls() **
* checks if there is a wall near player and assigns n e s w values to moveArray.
* @param - moveArray - a string array which determines which direciton player can move.
* @param - visibleBoard - The board which is visible to the player.
* @param - playerPos - Coord structure object with a x and y which determines player position.
*/
template <size_t T>
void CheckWalls(string *moveArray, char visibleBoard[][T], struct Coord *playerPos)//check walls for movement
{
	//the moveArray is set to 0 every time it is called from ProcessMove()
	if(visibleBoard[playerPos->y][playerPos->x + 1] != 'W')//e
	{
		*moveArray += "e";
	}

	if(visibleBoard[playerPos->y][playerPos->x - 1] != 'W')//w
	{
		*moveArray += "w";
	}

	if(visibleBoard[playerPos->y + 1][playerPos->x] != 'W')//s
	{
		*moveArray += "s";
	}

	if(visibleBoard[playerPos->y - 1][playerPos->x] != 'W')//n
	{
		*moveArray += "n";
	}
}

/***************************
** DrawBoard() **
* Draws the board onto the screen and places the player. 
* @param - realBoard - The board which was read in from the file and contains all elements of the board/map.
* @param - visibleBoard - The board which is visible to the player.
* @param - playerPos - Coord structure object with a x and y which determines player position.
* @param - boardWidth - the width of the board. 
* @param - boardHeight - the height of the board.
*/
template <size_t T>
void DrawBoard(char realBoard[][T], char visibleBoard[][T], struct Coord* playerPos, int boardWidth, int boardHeight)
{
	//find player position
	for(int i=0;i<boardWidth;i++)
	{
		for(int j=0; j<boardHeight; j++)
		{
			if (realBoard[i][j] == 'U')
			{
				playerPos->x = j;
				playerPos->y = i;
			}
		}
	}

	//add walls to visible board
	CheckWalls(realBoard, visibleBoard, playerPos);

	//set player on board:
	visibleBoard[playerPos->y][playerPos->x] = 'U';

	//display our visible board:
	RedrawBoard(visibleBoard, boardWidth, boardHeight);
}

/***************************
** RedrawBoard() **
* updates the board by redrawing it. all values have already been assigned to visibleBoard from other functions
* @param - visibleBoard - The board which is visible to the player.
* @param - playerPos - Coord structure object with a x and y which determines player position.
* @param - boardWidth - the width of the board. 
* @param - boardHeight - the height of the board.
*/
template <size_t T>
void RedrawBoard(char visibleBoard[][T], int boardWidth, int boardHeight)
{
	cout << "\n\n\n\n\n\n\n\n\n\n";
	for(int i=0;i<boardWidth;i++)
	{
		for(int j=0; j<boardHeight; j++)
		{
			cout << visibleBoard[i][j];
		}
		cout << endl;
	}
}


/*
* displays first message to player explaining where they are. 
*/
void DisplayWelcome()
{
	cout << "You wake up in a cold damp dark area.  You're laying on the ground in a pool of blood and vomit." <<
		"It appears to be your own.  Wow!  What a wild night last night was.  You remember so little, but your" <<
		"head pounds and you wish you were home in bed (or even in Dave's 220 class - anywhere but here).  " <<
		"Oh well.  You stagger to your feet and bump up against a slimy wall.  Ewwwwww!  Well, time to get out of here." <<
		"You notice your pockets are empty.  Even your trusty dagger is gone.  This so sucks.  Well, you're not getting " <<
		"home by standing here...  Get moving!" << endl;
}