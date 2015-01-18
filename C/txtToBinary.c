/*
* Written  by Sam Arutyunyan, Fall 2014
* saves text as a .bin file
*/

#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>//open()
#include <unistd.h>//read()
#include <string.h>
#define MAX 100

void strip(char * str)
{
    int x = 0;

    for(x = 0; str[x] != '\0'; x++)
    {
        if(str[x] == '\r' || str[x] == '\n')
            str[x] = '\0';
    }// end while
}// end strip


FILE* openInputFile()
{
    char fileName[MAX];
    //printf("cleaning input buffer, maybe press enter?");
    //while(fgetc(stdin) != '\n');

    printf("Enter file name to open Input file.\n");
    fgets(fileName, MAX, stdin);
    strip(fileName);//make sure it didnt have \n
    FILE* fin = fopen(fileName, "r");
    while(fin == NULL)
    {
        printf("File did not open\n");
        printf("Enter name of an input file\n");
        fgets(fileName, MAX, stdin);
        strip(fileName);
        fin = fopen(fileName, "r");
    }
//    printf("\nopened input file with %s openInputFile_Prompt()\n", fileName);
    return fin;
}


int main()
{
    FILE* fin = openInputFile();

    int fd = open("myBinary.bin", O_CREAT | O_WRONLY, 0777);
    //printf("%d\n", fd);//fd is a 3 because 3 is the next available file descriptor in the system.
    //0 = stdin, 1 = stdout, 2 = stderr
    if(fd < 0)
    {
        printf("Error with opening output file\n");
        exit(-1);
    }

    int x;
    char temp[MAX];

    fscanf(fin, "%s", temp);
    while(!feof(fin))
    {
        x = strlen(temp);
        write(fd, &x, sizeof(int));
        write(fd, temp, x);

        fscanf(fin, "%s", temp);
    }

    fclose(fin);
    close(fd);

    return 0;
}
