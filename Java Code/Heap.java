/* Written by Sam Arutyunyan November 2014
 * Simple Min heap for (Programming assignment 2. Algorithms Class)
 * Revised November 2014
 */

import java.util.Arrays;

/* i's left child is 2i+1
 * i's right child is 2i+2
 * i's parent is (i-1)/2
*/

//Assignment looks for 10k largest values. Therefore this will be a Min heap
public class Heap
{
	public Node heapArray[];	
	public int size, maxSize;
	
	class Node
	{
		public int value;
		int index;//index of that value in original file		
		public Node(int ind, int val)
		{
			index = ind;
			value = val;
		}
		public Node()
		{
			index = -1;
			value = 0;
		}
		public Node(Node what)
		{
			value = what.value;
			index = what.index;
		}
	}
	public Heap()
	{
		heapArray = new Node[16];//default starting to 16		
		maxSize = 16;
		size = 0;
	}
	
	public Heap(int startingSize)
	{
		maxSize = startingSize > 0 ? startingSize : 1;//prevent starting size of 0
		heapArray = new Node[maxSize];				
		size = 0;
	}
	
	//adds node to end of array
	public void append(int val, int ind)
	{
		size++;
		int curIndex = size - 1;
		
		//if it gets full, double size 
		//>NOTE: maxSize is guaranteed to always be > 0
		if(size > maxSize)
		{
			heapArray = Arrays.copyOf(heapArray, maxSize * 2);
			maxSize = maxSize * 2;
		}
		
		heapArray[curIndex] = new Node(ind, val);			
	}
	
	
	//replace value at index with new value.
	public void replace(int replaceIndex, int value, int index)
	{
		//verify valid index
		if(replaceIndex >= 0 && replaceIndex < size)
		{
			heapArray[replaceIndex].value = value;		
			heapArray[replaceIndex].index = index;		
			minHeapify(replaceIndex);
		}
	}
	
	public void remove(int removeIndex)
	{
		//verify valid index
		if(removeIndex >= 0 && removeIndex < size)
		{
			heapArray[removeIndex].value = heapArray[size - 1].value;		
			heapArray[removeIndex].index = heapArray[size - 1].index;		
			
			size--;
			minHeapify(0);			
		}
	}
	
	//Subtrees of rootIndex are heaps, but rootIndex itself might not be
	public void minHeapify(int rootIndex)
	{
		
		int x = rootIndex;//current
		int y = 2*x+1;//left child
		int z = 2*x+2;//right child	
		
		//while there is at least 1 child that is smaller than x 
		//>make sure z and y are within proper index range
		while( ( y < size && heapArray[y].value < heapArray[x].value) || ( z < size && heapArray[z].value < heapArray[x].value ))
		{
			//replace with the largest of the 2 children. 			
			//if z exists AND is smaller than y, swap with z
			if(z < size && heapArray[z].value < heapArray[y].value)
			{
				Node tempVal = new Node();
				tempVal = heapArray[x];
				heapArray[x] = heapArray[z];
				heapArray[z] = tempVal;
				x = z;
			}
			else//z didnt exist or wasn't bigger, we swap with y
			{
				Node tempVal = new Node();
				tempVal = heapArray[x];
				heapArray[x] = heapArray[y];
				heapArray[y] = tempVal;
				x = y;
			}
			
			y = 2*x+1;//left child
			z = 2*x+2;//right child			
		}
	}
	
	//presumes the entire heap array is not a proper heap, makes it into one.
	public void buildHeap()
	{
		//can skip all the leaf nodes
		for(int i = (size/2)-1; i >=0; i--)
		{
			minHeapify(i);			
		}
	}	
	
	public String toString()
	{
		if(size < 1) return "";
		
		String s = "[";
		for(int i = 0; i < size - 1; i++)
		{
			s += heapArray[i].value + ", ";			
		}
		s += heapArray[size-1].value + "]";
		return s;
		//return Arrays.toString(heapArray);
	}
	
}
