/*
* Written  by Sam Arutyunyan, Fall 2014
* generic linked list
*/
#ifndef LINKED_LIST_H
#define LINKED_LIST_H


#include <stdlib.h>
#include <stdio.h>

#include "stock.h"

struct node
{
    void* data;
    struct node * next;
};

typedef struct node Node;

struct linkedList
{
    Node * head;
    int size;
};

typedef struct linkedList LinkedList;

LinkedList * linkedList();

void addFirst(void* d, LinkedList * myList);
void printList(void(*f)(Node* cur), LinkedList * myList);
void cleanUp(void(*f)(Node* n), LinkedList *myList);

void addLast(void* d, LinkedList* myList);
void addOrdered(void* d, LinkedList* myList, int(*comp)(void* one, void* two));
Node * removeByIndex(int index, LinkedList* myList);
int removeByValue(void* item, LinkedList* myList, int(*comp)(void* one, void* two), void(*cleanFunc)(Node* n));

#endif
