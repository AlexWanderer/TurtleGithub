/*
* Written by Sam Arutyunyan Summer 2013
*/
#include "Graph.h"
#include <iostream>
using namespace std;

Graph::Graph(bool digraph)
{
	nextNodeIndex = 0;  isDigraph = digraph;
}

int  Graph::AddNode(GraphNode node)
{
	//if its less than count, we should be adding to a spot where there is an empty node space,
	//(in case a node was deleted previously?) otherwise to the nextNodeIndex          
	if (node.index < nodes.size())//make sure our node has a proper index
	{
		//make sure the client is not trying to add a node with the same ID as
		//a currently active node. if the node where we are trying to enter a new node is empty: -1
		//if the node in the graph node array of the element of the node we passed in has an index of -1
		if (nodes[node.index].index == -1)//by default the empty nodes should be -1
		{
			nodes[node.index] = node;// = nodes[node.Index] = node;
			return nextNodeIndex;//I guess since we didnt increase the size, we return the last spot            
		}

		//   cout << "Graph::AddNode>: Attempting to add a node with a duplicate ID";
		return -1;
	}
	else//the new node had a too high index so we are making sure it had nextNodeIndex
	{
		//make sure the new node has been indexed correctly
		if (node.index == nextNodeIndex)
		{
			nodes.push_back(node);
			list<GraphEdge> templist;
			edges.push_back(templist);
			return nextNodeIndex++;//this returns the index and then increments it
		}
		// System.out.println("Graph::AddNode>:invalid index");
		return -1;
	}
}

void Graph::AddEdge(GraphEdge edge)
{
	//first make sure the from and to nodes exist within the graph. nextNodeIndex should be same as .size()
	if ((edge.from < nextNodeIndex) && (edge.to < nextNodeIndex))
	{
		//make sure both nodes are active before adding the edge
		if (nodes[edge.to].index != -1 && nodes[edge.from].index != -1)
		{
			//add the edge, first making sure it is unique
			if (UniqueEdge(edge.from, edge.to))
			{
				//we always add a from edge, but if it is not a digraph we must add the returning edge
				edges[edge.from].push_back(edge);
			}
			else
			{
				//System.out.println("<Graph::AddEdge>: Edge Not added! Edge already exists."); 
			}

			//if the graph is undirected we must add another connection in the opposite
			//direction
			if (!isDigraph)
			{
				//check to make sure the edge is unique before adding
				if (UniqueEdge(edge.to, edge.from))//checks the reverse edge
				{
					GraphEdge NewEdge;


					NewEdge.to = edge.from;//we are assigning the reverse of the orignial edge (from and to becomes to and from)
					NewEdge.from = edge.to;
					edges[edge.to].push_back(NewEdge);
				}
				else
				{
					//System.out.println("<Graph::AddEdge>: Edge Not added! Edge already exists."); 
				}

			}
		}
		else
		{
			//System.out.println("<Graph::AddEdge>: Edge Not added! Inactive Node."); 
		}
	}
	else
	{
		//System.out.println("<Graph::AddEdge>: invalid node index"); 
	}
}

bool Graph::isEmpty(){ return nodes.size() == 0; }

bool Graph::isNodePresent(int nd)
{
	if (nd >= nodes.size() || nodes[nd].index < 0)//if the index is too high or -1 then return false
	{
		return false;
	}
	else return true;
}

//clears the graph ready for new node insertions.
void Graph::Clear(){ nextNodeIndex = 0; nodes.clear(); edges.clear(); }


//returns true if an edge is not already present in the graph. Used
//when adding edges to make sure no duplicates are created.
bool Graph::UniqueEdge(int from, int to)
{
	//loop through a node's lsit of edges to check if it connects just like checked edge	     	    
	for (list<GraphEdge>::const_iterator curEdge = edges[from].begin();
		curEdge != edges[from].end(); ++curEdge)
	{
		if (curEdge->to == to)
		{
			return false;
		}
	}

	return true;
}
