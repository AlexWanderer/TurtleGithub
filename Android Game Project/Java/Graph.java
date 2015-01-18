/*
 * Written by Sam Arutyunyan Summer 2013
 */
package com.example.research;

import java.util.ArrayList;
//for a given node in this graph, returns the ArrayList of edges connected to it: edges.get(GraphNode.GetIndex());

public class Graph 
{	
	public ArrayList <GraphNode> nodes = new ArrayList <GraphNode>();//all the nodes that make up this graph

    //each node in the graph has an associated ArrayList of edges that are connected to it
    public ArrayList < ArrayList <GraphEdge>> edges = new ArrayList < ArrayList <GraphEdge>>();

    boolean isDigraph;//assigned in the constructor
    int nextNodeIndex;//index of the next node to be added: might not be necessary with ArrayList
  
    //constructor
    public Graph(boolean digraph){ nextNodeIndex = 0;  isDigraph = digraph;}	    
  
    //adds a node to the graph and returns its index
    public int   AddNode(GraphNode node)
    { 
        //if its less than count, we should be adding to a spot where there is an empty node space,
        //(in case a node was deleted previously?) otherwise to the nextNodeIndex          
        if (node.index < nodes.size())//make sure our node has a proper index
        {
            //make sure the client is not trying to add a node with the same ID as
            //a currently active node. if the node where we are trying to enter a new node is empty: -1
        	//if the node in the graph node array of the element of the node we passed in has an index of -1
            if (nodes.get(node.index).index == -1)//by default the empty nodes should be -1
            {	       	            	
                nodes.set(node.index, node);// = nodes[node.Index] = node;
                return nextNodeIndex;//I guess since we didnt increase the size, we return the last spot            
            }
            
            System.out.println("Graph::AddNode>: Attempting to add a node with a duplicate ID");
                return -1;
        }  
        else//the new node had a too high index so we are making sure it had nextNodeIndex
        {
            //make sure the new node has been indexed correctly
            if (node.index == nextNodeIndex )
            {
                nodes.add(node);
                ArrayList<GraphEdge> templist = new ArrayList<GraphEdge>();
                edges.add(templist);
                return nextNodeIndex++;//this returns the index and then increments it
            }
            System.out.println("Graph::AddNode>:invalid index");
            return -1;
        }
    }		  

	 //Use this to add an edge to the graph. The method will ensure that the
	 //edge passed as a parameter is valid before adding it to the graph. If the
	 //graph is a digraph then a similar edge connecting the nodes in the opposite
	 //direction will be automatically added.
	 public void  AddEdge(GraphEdge edge)
	 {
        //first make sure the from and to nodes exist within the graph. nextNodeIndex should be same as .size()
        if ((edge.from < nextNodeIndex) && (edge.to < nextNodeIndex))
        {
            //make sure both nodes are active before adding the edge
            if (nodes.get(edge.to).index != -1 && nodes.get(edge.from).index != -1)
            {
                //add the edge, first making sure it is unique
                if (UniqueEdge(edge.from, edge.to))
                {
                    //we always add a from edge, but if it is not a digraph we must add the returning edge
                    edges.get(edge.from).add(edge);
                }
                else
                { System.out.println("<Graph::AddEdge>: Edge Not added! Edge already exists."); }

                //if the graph is undirected we must add another connection in the opposite
                //direction
            	if (!isDigraph)
                {           
                    //check to make sure the edge is unique before adding
                    if (UniqueEdge(edge.to, edge.from))//checks the reverse edge
                    {
                        GraphEdge NewEdge = new GraphEdge();
                                                
                        NewEdge.CopyEdge(edge);//we will hard code the from and to, but we want to save all other data (such as costs)
                       
                        NewEdge.to = edge.from;//we are assigning the reverse of the orignial edge (from and to becomes to and from)
                        NewEdge.from = edge.to;
                        edges.get(edge.to).add(NewEdge);
                    }
                    else
                    { System.out.println("<Graph::AddEdge>: Edge Not added! Edge already exists."); }

            	}
        	}
        	else
        	{ System.out.println("<Graph::AddEdge>: Edge Not added! Inactive Node."); }
        }
        else
        { System.out.println("<Graph::AddEdge>: invalid node index"); }
	    }
	  
	    
	    //returns true if the graph contains no nodes
	    public boolean	isEmpty(){return nodes.size() == 0;}

	    //returns true if a node with the given index is present in the graph
	    public boolean isNodePresent(int nd)
	    {	    	
	        if (nd >= nodes.size() || nodes.get(nd).index < 0)//if the index is too high or -1 then return false
	        {
	            return false;
	        }
	        else return true;  
	    }

	  //clears the graph ready for new node insertions.
	  void Clear(){nextNodeIndex = 0; nodes.clear(); edges.clear();}


	  //returns true if an edge is not already present in the graph. Used
	  //when adding edges to make sure no duplicates are created.
	  boolean UniqueEdge(int from, int to)
	  {
	      //loop through a node's lsit of edges to check if it connects just like checked edge	     	    
	        for(int i = 0; i < edges.get(from).size(); i++)
	        {
	        	if (edges.get(from).get(i).to == to) return false;	        	
	        }
	       
	      return true;
	  }

	  //iterates through all the edges in the graph and removes any that point
	  //to an invalidated node
	  void CullInvalidEdges()
	  {
		  /*we're not gonna use a digraph so I wont implement this
	      foreach (ArrayList<GraphEdge> curEdgeArrayList in edges)
	      {
	          for (int i = 0; i < curEdgeArrayList.size(); i++)
	          {
	              if (nodes[curEdgeArrayList[i].To].Index == -1 || nodes[curEdgeArrayList[i].From].Index == -1)
	              {
	                  curEdgeArrayList.RemoveAt(i);
	              }
	          }
	      }*/
		  System.out.println("Error in Graph::CullInvalidEdges(). You shouldn't be running this function >_>");
	  }
}




