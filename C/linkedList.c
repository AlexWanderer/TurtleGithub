/*
* Written  by Sam Arutyunyan, Fall 2014
* generic linked list
*/
#include "linkedList.h"

void sort(int(*f)(void* one, void* two), LinkedList* myList);
int tickerSort(void* one, void* two);

LinkedList* linkedList()
{
    //calloc: head will be NULL, size will be set to 0
    return (LinkedList*)calloc(1, sizeof(LinkedList));
}

void addFirst(void* d, LinkedList* myList)
{
    Node* nn = (Node*)calloc(1, sizeof(Node));
    //nn->data = buildStock(d->ticker, d->companyName, d->purchasePrice, d->currentPrice);
    nn->data = d;

    nn->next = myList->head;
    myList->head = nn;
    myList->size++;
}

void printList(void(*f)(Node* cur),LinkedList* myList)
{
    if(myList->size == 0)
        printf("Empty List\n");
    else
    {
        Node* cur = myList->head;
        printf("\n----------------\n");
        while(cur!= NULL)
        {
            f(cur);
            cur = cur->next;
        }
        printf("\n----------------\n");
    }
}

void cleanUp(void(*f)(Node* n), LinkedList* myList)
{
    Node* cur = myList->head;
    while(cur!= NULL)
    {
        myList->head = cur->next;

        f(cur);

        free(cur);
        cur = myList->head;
    }

}

void addLast(void* d, LinkedList* myList)
{
    Node* nn = (Node*)calloc(1, sizeof(Node));
    //nn->data = buildStock(d->ticker, d->companyName, d->purchasePrice, d->currentPrice);
    nn->data = d;

    if(myList->size < 1)
    {
        myList->head = nn;
    }
    else
    {
        Node* cur = myList->head;
        while(cur != NULL && cur->next != NULL)//cur should never be NULL anyways >_>
        {
            cur = cur->next;
        }
        cur->next = nn;
    }

    myList->size++;
}

//for a ticker sort, comp would pass tickerSort function
void addOrdered(void* d, LinkedList* myList, int(*comp)(void* one, void* two))
{
    sort(comp, myList);

    Node* nn = (Node*)calloc(1, sizeof(Node));
    //nn->data = buildStock(d->ticker, d->companyName, d->purchasePrice, d->currentPrice);
    nn->data = d;

    if(myList->size < 1)
    {
        myList->head = nn;
    }
    else if (myList->size == 1)
    {
        if(comp(d, myList->head->data) == 1)//if new node is smaller than head
        {
            nn->next = myList->head;
            myList->head = nn;
        }
        else
        {
            myList->head->next = nn;
        }
    }
    else
    {
        Node* cur = myList->head;

        while(cur->next != NULL && comp(cur->next->data, d) == 1)
        {
            cur = cur->next;
        }
        nn->next = cur->next;
        cur->next = nn;
    }

    myList->size++;

}

//returns a pointer to the node removed. its up to the caller to
//free that memory
Node* removeByIndex(int index, LinkedList* myList)
{
    if(index < 0 || index >= myList->size)
    {
        printf("Index out of bounds\n");
        exit(-1);
    }

    Node* cur = myList->head;
    Node* prev = NULL;
    int count = 0;
    for(count = 0; count < index; count++)
    {
        prev = cur;
        cur = cur->next;
    }
    //now cur is on the one we want to get rid of.
    //prev is on the one before.
    if(prev == NULL)
        myList->head = cur->next;
    else
        prev->next = cur->next;

    myList->size--;

    return cur;

}

int removeByValue(void* item, LinkedList* myList, int(*comp)(void* one, void* two), void(*cleanFunc)(Node* n))
{
    Node* cur = myList->head;
    Node* prev = NULL;

    while(cur != NULL);
    {
        if( comp(cur->data, item) == 0)
        {
            if(myList->head == cur)
                myList->head = cur->next;
            else
                prev->next = cur->next;

            cleanFunc(cur);
            free(cur);
            myList->size--;

            return 1;//true
        }
        prev = cur;
        cur = cur->next;
    }
    //now cur is on the one we want to get rid of.
    //prev is on the one before.


    return 0;//false
}
