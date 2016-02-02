/*
 * Written by Sam Arutyunyan Summer 2013
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Base graph class. can be derived for specific types of graphs.

public class Graph
{

    public List<GraphNode> nodes = new List<GraphNode>();//all the nodes that make up this graph

    //each node in the graph has an associated list of edges that are connected to it
    public List<List<GraphEdge>> edges = new List<List<GraphEdge>>();

    bool isDigraph;
    int nextNodeIndex;//index of the next node to be added: might not be necesary with List

    //for a given node in this graph, returns the list of edges connected to it
    public List<GraphEdge> GetConnections(GraphNode node)
    {
        return edges[node.Index];
    }    
  
    //constructor
    public Graph(bool digraph){ nextNodeIndex = 0;  isDigraph = digraph;}

    //returns the node at the given index
    public  GraphNode  GetNode(int idx)
    {
        if( (idx < nodes.Count ) && idx >=0 )
        {
            return nodes[idx];
        }
        else
        {
            Debug.LogError("Error in SparseGraph::GetNode()");
            return null;
        }    
    }


  //const method for obtaining a reference to an edge
    public  GraphEdge GetEdge(int from, int to)
    {
        if( !(from < nodes.Count && from >=0 && nodes[from].Index != -1) )
        {
            Debug.LogError("Error in SparseGraph::GetEdge()");
            return null;
        }

        if( !(to < nodes.Count && to >=0 && nodes[to].Index != -1 ))
        {
            Debug.LogError("Error in SparseGraph::GetEdge()");
            return null;
        }

        foreach(GraphEdge curEdge in edges[from])
        {
            if (curEdge.To == to) return curEdge;
        }

        Debug.LogError("Error in SparseGraph::GetEdge(). Edge did not exist!");
        return null;
    }
    

    //retrieves the next free node index
    public int   GetNextFreeNodeIndex(){return nextNodeIndex;}
  
    //adds a node to the graph and returns its index
    public int   AddNode(GraphNode node)
    {
        //i'm just not sure why we would ever have a proper index with a -1 ... >_>

        //if its less than count, we should be adding to a spot where there is an empty node space,
        //otherwise to the nextNodeIndex          
        if (node.Index < nodes.Count)//make sure our node has a proper index
        {
            //make sure the client is not trying to add a node with the same ID as
            //a currently active node. if the node where we are trying to enter a new node is empty: -1
            if (nodes[node.Index].Index == -1)//by default the empty nodes should be -1
            {
                nodes[node.Index] = node;
                return nextNodeIndex;//I guess since we didnt increase the size, we return the last spot            
            }
            
            Debug.LogError("SparseGraph::AddNode>: Attempting to add a node with a duplicate ID");
                return -1;
        }  
        else//the new node had a too high index so we are making sure it had nextNodeIndex
        {
            //make sure the new node has been indexed correctly
            if (node.Index == nextNodeIndex )
            {
                nodes.Add(node);
                edges.Add(new List<GraphEdge>());
                return nextNodeIndex++;//this returns the index and then increments it
            }
            Debug.LogError("SparseGraph::AddNode>:invalid index");
            return -1;
        }
}

  //removes a node by setting its index to invalid_node_index
 public  void  RemoveNode(int node)
 {
     if (node < nodes.Count)
     {
         //set this node's index to invalid_node_index
         nodes[node].Index = -1;

         //if the graph is not directed remove all edges leading to this node and then
         //clear the edges leading from the node
         if (!isDigraph)
         {
             //visit each neighbour and erase any edges leading to this node. since each edge goes 2 ways, we can loop
             //out from this node to all it's edges and then loop through all the edges belonging to those nodes to 
             //get rid of the ones that point back to this node. 
             foreach (GraphEdge curEdge in edges[node])//for each edge belonging to this node
             {
                 for (int i = 0; i < edges[curEdge.To].Count; i++)
                 {
                     if (edges[curEdge.To][i].To == node)//for every node in the graph check if their .to is the node we are deleting
                     {
                         edges[curEdge.To].Remove(edges[curEdge.To][i]);
                         break;
                     }
                 }
             }

             //finally, clear this node's edges
             edges[node].Clear();
         }

         //if a digraph remove the edges the slow way
         else//we already set this node index to -1 at the top of this method.
         {
             CullInvalidEdges();//removes any edges connected to -1 nodes
         }
     }
     else
     {
         Debug.LogError("SparseGraph::RemoveNode>: invalid node index");
     }

 }//end RemoveNode()

  //Use this to add an edge to the graph. The method will ensure that the
  //edge passed as a parameter is valid before adding it to the graph. If the
  //graph is a digraph then a similar edge connecting the nodes in the opposite
  //direction will be automatically added.
    public void  AddEdge(GraphEdge edge)
    {
        //first make sure the from and to nodes exist within the graph. nextNodeIndex should be same as .Count
        if ((edge.From < nextNodeIndex) && (edge.To < nextNodeIndex))
        {
            //make sure both nodes are active before adding the edge
            if (nodes[edge.To].Index != -1 && nodes[edge.From].Index != -1)
            {
                //add the edge, first making sure it is unique
                if (UniqueEdge(edge.From, edge.To))
                {
                    //we always add a from edge, but if it is not a digraph we must add the returning edge
                    edges[edge.From].Add(edge);
                }
                else
                { Debug.LogError("<SparseGraph::AddEdge>: Edge Not added! Edge already exists."); }

                //if the graph is undirected we must add another connection in the opposite
                //direction
                if (!isDigraph)
                {           
                    //check to make sure the edge is unique before adding
                    if (UniqueEdge(edge.To, edge.From))//checks the reverse edge
                    {
                        GraphEdge NewEdge = new GraphEdge();
                                                
                        NewEdge.CopyEdge(edge);//we will hard code the from and to, but we want to save all other data (such as costs)
                       
                        NewEdge.to = edge.from;//we are assigning the reverse of the orignial edge (from and to becomes to and from)
                        NewEdge.from = edge.to;
                        edges[edge.To].Add(NewEdge);
                    }
                    else
                    { Debug.LogError("<SparseGraph::AddEdge>: Edge Not added! Edge already exists."); }

                }
            }
            else
            { Debug.LogError("<SparseGraph::AddEdge>: Edge Not added! Inactive Node."); }
        }
        else
        { Debug.LogError("<SparseGraph::AddEdge>: invalid node index"); }
    }

  //removes the edge connecting from and to, from the graph (if present). If
  //a digraph then the edge connecting the nodes in the opposite direction 
  //will also be removed.
    public void  RemoveEdge(int from, int to)
    {
        if ( from < nodes.Count && to < nodes.Count) //shouldnt we make sure its not -1 >_>
        {  
            if (!isDigraph)
            {
                for(int i = 0; i < edges[to].Count; i++)         
                {
                    //orignial implementation moved the iterator to the new index, i'm hoping this isnt necesary with foreach
                    //although, foreach isn't supposed to allow modifying...
                    if (edges[to][i].To == from) { edges[to].Remove(edges[to][i]); break; }
                }
            }

            for (int i = 0; i < edges[to].Count; i++)   
            {
                if (edges[to][i].To == to) { edges[from].Remove(edges[to][i]); break; }
            }
        }
        else
            Debug.LogError("<SparseGraph::RemoveEdge>:invalid node index");
    }

  //sets the cost of an edge
    public void  SetEdgeCost(int from, int to, float newCost)//its expected to be called for both directions in a non digraph
    {
        //make sure the nodes given are valid
        if( from < nodes.Count && to < nodes.Count  )
        {
            //visit each neighbour and erase any edges leading to this node
            foreach(GraphEdge curEdge in edges[from])  
            {
                if (curEdge.To == to)
                {
                    curEdge.Cost = newCost;
                    break;
                }
            }
        }
        else
            Debug.LogError("<SparseGraph::SetEdgeCost>: invalid index");
    }

    //returns the number of active + inactive nodes present in the graph
    public int   NumNodes(){return nodes.Count;}
  
    //returns the number of active nodes present in the graph (this method's
    //performance can be improved greatly by caching the value)
    public int NumActiveNodes()//TODO optimize
    {
        int count = 0;

        for (int n=0; n<nodes.Count; ++n) if (nodes[n].Index != -1) ++count;

        return count;
    }

    //returns the total number of edges present in the graph
    public int NumEdges()//TODO optimize
    {
        int tot = 0;
    
        //every list inside our list of edge lists
        foreach(List<GraphEdge> curEdge in edges)
        {
            tot += curEdge.Count;
        }

        return tot;
    }

    //returns true if the graph is directed
    public bool  IsDigraph{ get {return isDigraph;}}

    //returns true if the graph contains no nodes
    public bool	isEmpty(){return nodes.Count == 0;}

    //returns true if a node with the given index is present in the graph
    public bool isNodePresent(int nd)
    {
        if (nd >= nodes.Count || nodes[nd].Index == -1)//if the index is too high or -1 then return false
        {
            return false;
        }
        else return true;  
    }

  //returns true if an edge connecting the nodes 'to' and 'from'
  //is present in the graph
  bool isEdgePresent(int from, int to)
  {
      if (isNodePresent(from) && isNodePresent(from))
        {
          foreach(GraphEdge curEdge in edges[from])
            {
              if (curEdge.To == to) return true;
            }

            return false;
        }
        else return false;
  }


  //clears the graph ready for new node insertions
  void Clear(){nextNodeIndex = 0; nodes.Clear(); edges.Clear();}

  void RemoveEdges()
  {
      edges.Clear();
  }

  //returns true if an edge is not already present in the graph. Used
  //when adding edges to make sure no duplicates are created.
  bool UniqueEdge(int from, int to)
  {
      //loop through a node's lsit of edges to check if it connects just like checked edge
      foreach (GraphEdge curEdge in edges[from])
      {
          if (curEdge.To == to)
          {
              return false;
          }
      }

      return true;
  }

  //iterates through all the edges in the graph and removes any that point
  //to an invalidated node
  void CullInvalidEdges()
  {
      foreach (List<GraphEdge> curEdgeList in edges)
      {
          for (int i = 0; i < curEdgeList.Count; i++)
          {
              if (nodes[curEdgeList[i].To].Index == -1 || nodes[curEdgeList[i].From].Index == -1)
              {
                  curEdgeList.RemoveAt(i);
              }
          }
      }
  }
   
}//end class Graph
