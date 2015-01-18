/*
* Written  by Sam Arutyunyan, Fall 2014
* Various functions for working with file input and output
*/
#include "fileUtil.h"

FILE* openInputFile_Prompt()
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

FILE* openInputFile_Args(int argc, char* argv[])
{
    if(argc < 2)
        exit(-1);

    char fileName[MAX];

    strcpy(fileName, argv[1]);
    strip(fileName);//make sure it didnt have \n


    FILE* fin = fopen(fileName, "r");
    while(fin == NULL)
    {
        char temp[MAX];
        //printf("cleaning input buffer, maybe press enter?");
        //while(fgetc(stdin) != '\n');

        printf("File did not open\n");
        printf("Enter name of an input file\n");
        fgets(temp, MAX, stdin);
        strip(temp);
        fin = fopen(temp, "r");
        printf("filename: %s\n", temp);

    }
//    printf("\nopened input file with openInputFile_Args()\n");
    return fin;
}

FILE* openInputFile_String(char* fileName)
{
    strip(fileName);//make sure it didnt have \n
    FILE* fin = fopen(fileName, "r");
    while(fin == NULL)
    {
        char temp[MAX];
 //       printf("cleaning input buffer, maybe press enter?");
        //while(fgetc(stdin) != '\n');
        printf("File did not open\n");
        printf("Enter name of an input file\n");
        fgets(temp, MAX, stdin);
        strip(temp);
        fin = fopen(temp, "r");
    }
//    printf("\npassed file name %s openInputFile_String()\n", fileName);
    return fin;
}

FILE* openOutputFile_FileName(char* fileName)
{
    strip(fileName);//make sure it didnt have \n
    FILE* fout = fopen(fileName, "w");
    while(fout == NULL)
    {
        char temp[MAX];
 //       printf("cleaning input buffer, maybe press enter?");
        //while(fgetc(stdin) != '\n');
        printf("File did not open\n");
        printf("Enter name for output file\n");
        fgets(temp, MAX, stdin);
        strip(temp);
        fout = fopen(temp, "w");
    }
 //   printf("\npassed file name %s openOutputFile_FileName()\n", fileName);
    return fout;
}

FILE* openOutputFile_Prompt()
{
    char fileName[MAX];
 //   printf("cleaning input buffer, maybe press enter?");
    //while(fgetc(stdin) != '\n');

    printf("Enter file name for output file.\n");
    fgets(fileName, MAX, stdin);
    strip(fileName);//make sure it didnt have \n
    FILE* fout = fopen(fileName, "w");
    while(fout == NULL)
    {
        printf("File did not open\n");
        printf("Enter name of an output file\n");
        fgets(fileName, MAX, stdin);
        strip(fileName);
        fout = fopen(fileName, "w");
    }
 //   printf("\nopening %s openOutputFile_Prompt()\n", fileName);
    return fout;
}

char* readFileName()
{
//    printf("\n===READING FROM readFileName()====");
    char fileName[MAX];
 //   printf("cleaning input buffer, maybe press enter?");
    //while(fgetc(stdin) != '\n');

    printf("Enter a file name.\n");
    fgets(fileName, MAX, stdin);
    strip(fileName);//make sure it didnt have \n

    char* temp = (char*)calloc(strlen(fileName) + 1, sizeof(char));
    strcpy(temp, fileName);
 //   printf("\nreturning name of %s from readFileName()\n", temp);
    return temp;
}
int countRecords(FILE* fin, int linesPerRecord)
{
    int count = 0;
    char temp[MAX];

    fgets(temp, MAX, fin);
    //presumes a \n at end of file.
    while(!feof(fin))//has eof in fin been set.
    {
        count++;
        fgets(temp, MAX, fin);
    }

    return count / linesPerRecord;
}

void displayFile_TotalLines(int num, FILE* fin)
{
 //   printf("\ndisplaying from: displayFile_TotalLines()\n");
    if (num == 0) return;
    int i = 0;
    char temp[MAX];

    fgets(temp, MAX, fin);
    strip(temp);
    while(!feof(fin) && i < num)//has eof in fin been set.
    {
        printf("%s\n", temp);
        i++;
        fgets(temp, MAX, fin);
        strip(temp);
    }
}

void displayFile_FilePointer(FILE* fin)//disp location of pointer
{
 //   printf("\ndisplaying from: displayFile_FilePointer()\n");
    if(fin == NULL)
        printf("File pointer is null");
    else
        printf("File pointer at: %p\n", &fin);
}

void displayFile_OutputFile(FILE* fin, FILE* fout)//display to file instead of to screen
{
 //   printf("\ndisplaying from: displayFile_OutputFile()\n");
    char temp[MAX];
    fgets(temp, MAX, fin);
    strip(temp);
    while(!feof(fin))//has eof in fin been set.
    {
        fprintf(fout, "%s\n", temp);
        fgets(temp, MAX, fin);
        strip(temp);
    }
}

void strip(char * str)
{
    int x = 0;

    for(x = 0; str[x] != '\0'; x++)
    {
        if(str[x] == '\r' || str[x] == '\n')
            str[x] = '\0';
    }// end while
}// end strip
