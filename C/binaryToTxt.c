/*
* Written  by Sam Arutyunyan, Fall 2014
* converts .bin data into readable text
*/
#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>//open()
#include <unistd.h>//read()
#include <string.h>
#define MAX 100

void convertToText(int fd)
{
    int res, x;
    char temp[MAX];

    res = read(fd, &x, sizeof(int));
    res = read(fd, temp, x);

    while(res > 0)
    {
        temp[x] = '\0';
        printf("%s\n", temp);
        res = read(fd, &x, sizeof(int));
        res = read(fd, temp, x);
    }
}

void convert(int fd)
{
    lseek(fd, 0, SEEK_SET);

    int res, x;
    char temp[MAX];

    res = read(fd, &x, sizeof(int));
    res = read(fd, temp, x);

    while(res > 0)//while something is being read from binary file
    {
        temp[x] = '\0';
        write(1, temp, x);//write to stdout
        write(1, "\n", 1);

        res = read(fd, &x, sizeof(int));
        res = read(fd, temp, x);
    }
}

int main()
{
    //open bin file and print it as text
    int fd = open("myBinary.bin", O_RDONLY);

    if(fd < 0)
    {
        printf("Error with opening binary file\n");
        exit(-1);
    }

    convertToText(fd);
    write(1, "\n", 1);
    convert(fd);

    close(fd);

    return 0;
}


