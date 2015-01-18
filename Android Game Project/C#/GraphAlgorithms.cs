/*
 * Written by Sam Arutyunyan Summer 2013
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//search process: inside your game logic, when you need to search, create an instance of this class and pass it the graph, source, and target
//info. as soon as teh object is created (in the constructor) search() is run. then probably run the path function and return it to the code 
//that needs that information. when the pathfinding is done, will probably delete this object... 
class Graph_SearchDFS//I dont think depth first gets the shortest path.. >_>
{
    //to aid legibility
    enum NodeState {visited, unvisited, no_parent_assigned};

    //a reference to the graph to be searched
    Graph graph;

    //this records the indexes of all the nodes that are visited as the
    //search progresses
    List<int>  visited;//represents the array of nodes, holding only data on if it's visited or not

    //this holds the route taken to the target. Given a node index, the value
    //at that index is the node's parent. ie if the path to the target is
    //3-8-27, then m_Route[8] will hold 3 and m_Route[27] will hold 8.
    List<int>  route;

    //As the search progresses, this will hold all the edges the algorithm has
    //examined. THIS IS NOT NECESSARY FOR THE SEARCH, IT IS HERE PURELY
    //TO PROVIDE THE USER WITH SOME VISUAL FEEDBACK
    List<GraphEdge>  spanningTree;

    //the source and target node indices
    int               source,
                    target;

    //true if a path to the target has been found
    bool              found;

    public Graph_SearchDFS(Graph graph, int source, int target = -1)
    {    
        this.graph = graph;
        this.source = source;
        this.target = target;
        found = false;
        
        visited = new List<int>();
        spanningTree = new List<GraphEdge>();
        route = new List<int>();

        for(int i = 0; i < graph.NumNodes(); ++i)
        {
            visited.Add( (int)NodeState.unvisited);
            route.Add((int)NodeState.no_parent_assigned);
        }
                                                                    
        found = Search(); 
    }

    //returns true if the target node has been located
    bool   Found(){return found;}
    //returns a vector containing pointers to all the edges the search has examined
    List<GraphEdge> GetSearchTree(){return spanningTree;}        


    //-----------------------------------------------------------------------------
    //basically it just goes through every node until it finds its target. once it finds its target it returns and it knows that
    //it has built up an array (route) where it stored the path towards target. so target's index of route would be the parent node. once
    //we reach route[parent] we'll find it's parent and continue down the route. 

    bool Search()//Game AI page 240
    {
        //create a std stack of edges
        Stack<GraphEdge> stack = new Stack<GraphEdge>();

        //create a dummy edge and put on the stack
        GraphEdge Dummy = new GraphEdge(source, source, 0);
  
        stack.Push(Dummy);

        //while there are edges in the stack keep searching
        while (stack.Count > 0)
        {
            //grab the next edge. and remove it from the stack
            GraphEdge Next = stack.Pop();

            //make a note of the parent of the node this edge points to
            route[Next.To] = Next.From;//the first time through it should just point to itself

            //put it on the tree. (making sure the dummy edge is not placed on the tree)
            if (Next != Dummy)
            {
                spanningTree.Add(Next);
            }
   
            //and mark it visited
            visited[Next.To] = (int)NodeState.visited;

            //if the target has been found the method can return success
            if (Next.To == target)
            {
                return true;//assigns true to local variable: found
            }

            //push the edges leading from the node this edge points to onto
            //the stack (provided the edge does not point to a previously visited node)
            //sam's interpretation: each node position has a list of edges that branch off from it.
                //this list<graphEdge> belongs to the instance of a graph (a sparseGraph for example)
                //the first iteration of the stack, .To points to itself, so we access that node and loop
                //through its child edges, if those edges are unvisited we push them on to the stack. 
                //each newly pushed edge has it's own .To value which points to its connected edge.
            foreach(GraphEdge pE in graph.edges[Next.To])//for every edge in the next node
            {
                if (visited[pE.To] == (int)NodeState.unvisited)
                {
                    stack.Push(pE);//add it to the stack if we haven't visited it yet
                    //Debug.Log("pushed " + pE.From + "-" + pE.To +" onto stack.");
                }
            }

        }   

        //no path to target
        return false;
    }//end Search()

    //-----------------------------------------------------------------------------

    //returns a vector of node indexes that comprise the shortest path
    //from the source to the target
    public List<int> GetPathToTarget() //tells ai: go to this node, then go to this node... 
    {
      List<int> path = new List<int>();

      //just return an empty path if no path to target found or if
      //no target has been specified
      if (!found || target<0) return path;

      int nd = target;

        //assigns the target and each iteration through the route list it will add more items to the front, eventually target will be the very
        //last node in path. I believe this is what causes a reversal of the original path which is backwards..
      path.Insert(0, nd);//insert can be done on a empty list but only at index 0

      while (nd != source)
      {
        nd = route[nd];

        path.Insert(0,nd);
      }

      return path;
    }

        

}//end class GraphSearchDFS

class Graph_SearchBFS//I dont think depth first gets the shortest path.. >_>
{
    //to aid legibility
    enum NodeState { visited, unvisited, no_parent_assigned };

    //a reference to the graph to be searched
    Graph graph;

    //this records the indexes of all the nodes that are visited as the
    //search progresses
    List<int> visited;//represents the array of nodes, holding only data on if it's visited or not

    //this holds the route taken to the target. Given a node index, the value
    //at that index is the node's parent. ie if the path to the target is
    //3-8-27, then m_Route[8] will hold 3 and m_Route[27] will hold 8.
    List<int> route;

    //As the search progresses, this will hold all the edges the algorithm has
    //examined. THIS IS NOT NECESSARY FOR THE SEARCH, IT IS HERE PURELY
    //TO PROVIDE THE USER WITH SOME VISUAL FEEDBACK
    List<GraphEdge> spanningTree;

    //the source and target node indices
    int source,
                    target;

    //true if a path to the target has been found
    bool found;

    public Graph_SearchBFS(Graph graph, int source, int target = -1)
    {
        this.graph = graph;
        this.source = source;
        this.target = target;
        found = false;

        visited = new List<int>();
        spanningTree = new List<GraphEdge>();
        route = new List<int>();

        for (int i = 0; i < graph.NumNodes(); ++i)
        {
            visited.Add((int)NodeState.unvisited);
            route.Add((int)NodeState.no_parent_assigned);
        }

        found = Search();
    }

    //returns true if the target node has been located
    bool Found() { return found; }
    //returns a vector containing pointers to all the edges the search has examined
    List<GraphEdge> GetSearchTree() { return spanningTree; }


    //-----------------------------------------------------------------------------
    //goes through all the nodes 1 space away, then 2, then 3, so on...
    bool Search()//Game AI page 249
    {
        //create a std stack of edges
        Queue<GraphEdge> Q = new Queue<GraphEdge>();

        //create a dummy edge and put on the stack
        GraphEdge Dummy = new GraphEdge(source, source, 0);

        Q.Enqueue(Dummy);

        //mark the source node as visited
        visited[source] = (int)NodeState.visited;

        //while there are edges in the stack keep searching
        while (Q.Count > 0)
        {
            //grab the next edge. and remove it from the stack
            GraphEdge Next = Q.Dequeue();

            //make a note of the parent of the node this edge points to
            route[Next.To] = Next.From;//the first time through it should just point to itself

            //put it on the tree. (making sure the dummy edge is not placed on the tree)
            if (Next != Dummy)
            {
                spanningTree.Add(Next);
            }            

            //if the target has been found the method can return success
            if (Next.To == target)
            {
                return true;//assigns true to local variable: found
            }

            //push the edges leading from the node at the end of this edge 
            //onto the queue
            foreach (GraphEdge pE in graph.edges[Next.To])//for every edge in the next node
            {
                if (visited[pE.To] == (int)NodeState.unvisited)
                {
                    Q.Enqueue(pE);//if the node hasn't already been visited we can push the edge onto the queue
                    //and mark it visited
                    visited[pE.To] = (int)NodeState.visited;
                }
            }
        }

        //no path to target
        return false;
    }//end Search()

    //-----------------------------------------------------------------------------

    //returns a vector of node indexes that comprise the shortest path
    //from the source to the target
    public List<int> GetPathToTarget() //tells ai: go to this node, then go to this node... 
    {
        List<int> path = new List<int>();

        //just return an empty path if no path to target found or if
        //no target has been specified
        if (!found || target < 0) return path;

        int nd = target;

        //assigns the target and each iteration through the route list it will add more items to the front, eventually target will be the very
        //last node in path. I believe this is what causes a reversal of the original path which is backwards..
        path.Insert(0, nd);//insert can be done on a empty list but only at index 0

        while (nd != source)
        {
            nd = route[nd];

            path.Insert(0, nd);
        }

        return path;
    }



}//end class GraphSearchBFS


class Graph_SearchDijkstra
{
    //a reference to the graph to be searched
    Graph graph;

    //this vector contains the edges that comprise the shortest path tree -
    //a directed subtree of the graph that encapsulates the best paths from 
    //every node on the SPT to the source node.
    List<GraphEdge>      shortestPathTree;//the parent nodes, except it holds edges instead of nodes. 

    //this is indexed into by node index and holds the total cost of the best
    //path found so far to the given node. For example, m_CostToThisNode[5]
    //will hold the total cost of all the edges that comprise the best path
    //to node 5, found so far in the search (if node 5 is present and has 
    //been visited)
    List<float>            costToThisNode; 

    //this is an indexed (by node) vector of 'parent' edges leading to nodes 
    //connected to the SPT but that have not been added to the SPT yet. This is
    //a little like the stack or queue used in BST and DST searches.
    List<GraphEdge>     searchFrontier;

    //the source and target node indices
    int source, target;

    public Graph_SearchDijkstra(Graph graph, int source, int target = -1)
    {    
        this.graph = graph;//this is assigning our graph to be the same graph that was passed. (Dont mondify!)
        this.source = source;
        this.target = target;

        shortestPathTree = new List<GraphEdge>();
        costToThisNode = new List<float>();
        searchFrontier = new List<GraphEdge>();

        for(int i = 0; i < graph.NumNodes(); ++i)
        {
            //mark all nodes as unvisited
            shortestPathTree.Add(null);
            costToThisNode.Add(0);
            searchFrontier.Add(null);
        }

                                                                    
        Search(); 
    }

    //returns the vector of edges that defines the SPT. If a target was given
    //in the constructor then this will be an SPT comprising of all the nodes
    //examined before the target was found, else it will contain all the nodes
    //in the graph.
    public List<GraphEdge> GetSPT(){return shortestPathTree;}    

    //returns the total cost to the target
    public float GetCostToTarget(){return costToThisNode[target];}

    //returns the total cost to the given node. uint == unsigned int in c+
    public float GetCostToNode(uint nd){return costToThisNode[(int)nd];}//why are we passing uint? for size?

    void Search()
    {
        //create an indexed priority queue that sorts smallest to largest
        //(front to back).Note that the maximum number of elements the iPQ
        //may contain is N. This is because no node can be represented on the 
        //queue more than once.. keeps every node index ordered by cost
        PriorityQueue queue = new PriorityQueue(costToThisNode, graph.NumNodes());

        //put the source node on the queue
        queue.Insert(source);//first we insert the index of the source node. 

        //while the queue is not empty
        while(!queue.empty())
        {
            //get lowest cost node from the queue. Don't forget, the return value
            //is a *node index*, not the node itself. This node is the node not already
            //on the SPT that is the closest to the source node
            int NextClosestNode = queue.Pop();

            //move this edge from the frontier to the shortest path tree. searchFrontier holds the parent edges to each node
            shortestPathTree[NextClosestNode] = new GraphEdge(); 
            shortestPathTree[NextClosestNode].CopyEdge(searchFrontier[NextClosestNode]);

            //if the target has been found exit
            if (NextClosestNode == target) return;

            //for each edge connected to the next closest node
            foreach(GraphEdge pE in graph.edges[NextClosestNode])//for every edge in the next node            
            {   
                //the total cost to the node this edge points to is the cost to the
                //current node plus the cost of the edge connecting them.
                float NewCost = costToThisNode[NextClosestNode] + pE.Cost;
               // Debug.Log("cost to node[" + NextClosestNode + "] " + costToThisNode[NextClosestNode]);

                //if this edge has never been on the frontier make a note of the cost
                //to get to the node it points to, then add the edge to the frontier
                //and the destination node to the PQ.
                if (searchFrontier[pE.To] == null)
                {
                    costToThisNode[pE.To] = NewCost;

                    queue.Insert(pE.To);
                    searchFrontier[pE.to] = new GraphEdge();
                    searchFrontier[pE.To].CopyEdge(pE);
                }

                //else test to see if the cost to reach the destination node via the
                //current node is cheaper than the cheapest cost found so far. If
                //this path is cheaper, we assign the new cost to the destination
                //node, update its entry in the PQ to reflect the change and add the
                //edge to the frontier
                else if ( (NewCost < costToThisNode[pE.To]) && (shortestPathTree[pE.To] == null) )
                {
                    costToThisNode[pE.To] = NewCost;

                    //because the cost is less than it was previously, the PQ must be
                    //re-sorted to account for this.
                    queue.ChangePriority(pE.To);
                    searchFrontier[pE.To] = new GraphEdge();
                    searchFrontier[pE.To].CopyEdge(pE);
                }
            }
        }
    }


    //returns a vector of node indexes that comprise the shortest path
    //from the source to the target. It calculates the path by working
    //backwards through the SPT from the target node.
    public List<int> GetPathToTarget()
    {
        List<int> path = new List<int>();

            //just return an empty path if no path to target found or if
            //no target has been specified
            if (target < 0) return path;

            int nd = target;

            //assigns the target and each iteration through the route list it will add more items to the front, eventually target will be the very
            //last node in path. I believe this is what causes a reversal of the original path which is backwards..
            path.Insert(0, nd);//insert can be done on a empty list but only at index 0

            while (nd != source && shortestPathTree[nd] != null)
            {
                nd = shortestPathTree[nd].From;

                path.Insert(0, nd);
            }

            return path;
    }

}//end class Graph_SearchDijkstra


class Graph_SearchAStar
{
    //a reference to the graph to be searched
    Graph graph;

    //indexed into my node. Contains the 'real' accumulative cost to that node
    List<float> gCosts;//cumulative regular cost

    //indexed into by node. Contains the cost from adding m_GCosts[n] to
    //the heuristic cost from n to the target node. This is the vector the
    //iPQ indexes into.
    List<float> fCosts;//final cost: g + heuristic

    //this vector contains the edges that comprise the shortest path tree -
    //a directed subtree of the graph that encapsulates the best paths from 
    //every node on the SPT to the source node.
    List<GraphEdge> shortestPathTree;//the parent nodes, except it holds edges instead of nodes. 

    //this is an indexed (by node) vector of 'parent' edges leading to nodes 
    //connected to the SPT but that have not been added to the SPT yet. This is
    //a little like the stack or queue used in BST and DST searches.
    List<GraphEdge> searchFrontier;

    //the source and target node indices
    int source, target;

    public Graph_SearchAStar(Graph graph, int source, int target = -1)
    {
        this.graph = graph;//this is assigning our graph to be the same graph that was passed. (Dont mondify!)
        this.source = source;
        this.target = target;

        shortestPathTree = new List<GraphEdge>();
        gCosts = new List<float>();
        fCosts = new List<float>();
        searchFrontier = new List<GraphEdge>();

        for (int i = 0; i < graph.NumNodes(); ++i)
        {
            //mark all nodes as unvisited
            shortestPathTree.Add(null);
            gCosts.Add(0);
            fCosts.Add(0);
            searchFrontier.Add(null);
        }


        Search();
    }

    //returns the vector of edges that defines the SPT. If a target was given
    //in the constructor then this will be an SPT comprising of all the nodes
    //examined before the target was found, else it will contain all the nodes
    //in the graph.
    public List<GraphEdge> GetSPT() { return shortestPathTree; }

    //returns the total cost to the target
    public float GetCostToTarget(){return gCosts[target];}

    void Search()
    {
        //create an indexed priority queue that sorts smallest to largest
        //(front to back).Note that the maximum number of elements the iPQ
        //may contain is N. This is because no node can be represented on the 
        //queue more than once.. keeps every node index ordered by cost
        PriorityQueue queue = new PriorityQueue(fCosts, graph.NumNodes());

        //put the source node on the queue
        queue.Insert(source);//first we insert the index of the source node. 

        //while the queue is not empty
        while (!queue.empty())
        {
            //get lowest cost node from the queue. Don't forget, the return value
            //is a *node index*, not the node itself. This node is the node not already
            //on the SPT that is the closest to the source node
            int NextClosestNode = queue.Pop();

            //move this edge from the frontier to the shortest path tree. searchFrontier holds the parent edges to each node
            shortestPathTree[NextClosestNode] = new GraphEdge();//since it started out as null
            shortestPathTree[NextClosestNode].CopyEdge(searchFrontier[NextClosestNode]);

            
            //if the target has been found exit
            if (NextClosestNode == target) return;

            //for each edge connected to the next closest node
            foreach (GraphEdge pE in graph.edges[NextClosestNode])//for every edge in the next node            
            {                
                //calculate the heuristic cost from this node to the target (H)   
                //my implementation retrieves the vector between the 2 nodes and gets it's square magnitude, so the square distance    
                //>>Calculating a manhattan distance might be optimal
                float hCost = Vector3.SqrMagnitude((graph.nodes[target]).position - (graph.nodes[pE.to]).position);

                //calculate the 'real' cost to this node from the source (G)
                float gCost = gCosts[NextClosestNode] + pE.Cost;
                // Debug.Log("cost to node[" + NextClosestNode + "] " + costToThisNode[NextClosestNode]);

                //if the node has not been added to the frontier, add it and update
                //the G and F costs
                if (searchFrontier[pE.To] == null)
                {
                    fCosts[pE.To] = gCost + hCost;
                    gCosts[pE.To] = gCost;

                    queue.Insert(pE.To);
                    searchFrontier[pE.To] = new GraphEdge();
                    searchFrontier[pE.To].CopyEdge(pE);
                }

                //else test to see if the cost to reach the destination node via the
                //current node is cheaper than the cheapest cost found so far. If
                //this path is cheaper, we assign the new cost to the destination
                //node, update its entry in the PQ to reflect the change and add the
                //edge to the frontier
                else if (gCost < gCosts[pE.To] && shortestPathTree[pE.To] == null)
                {
                    fCosts[pE.To] = gCost + hCost;
                    gCosts[pE.To] = gCost;

                    //because the cost is less than it was previously, the PQ must be
                    //re-sorted to account for this.
                    queue.ChangePriority(pE.To);

                    searchFrontier[pE.To] = new GraphEdge();
                    searchFrontier[pE.To].CopyEdge(pE);
                }
            }
        }
        
    }//end Search()


    //returns a vector of node indexes that comprise the shortest path
    //from the source to the target. It calculates the path by working
    //backwards through the SPT from the target node.
    public List<int> GetPathToTarget()
    {
        /*
        Debug.Log("debug.log>_>");
        foreach (GraphEdge edge in shortestPathTree)
        {
            if (edge != null)
            {
                if (edge.from != -1 && edge.to != -1)
                    Debug.Log("from: " + edge.from);
            }
        }*/

        List<int> path = new List<int>();

        //just return an empty path if no path to target found or if
        //no target has been specified
        if (target < 0) return path;

        int nd = target;

        //assigns the target and each iteration through the route list it will add more items to the front, eventually target will be the very
        //last node in path. I believe this is what causes a reversal of the original path which is backwards..
        path.Insert(0, nd);//insert can be done on a empty list but only at index 0

        while (nd != source && shortestPathTree[nd] != null)
        {
            nd = shortestPathTree[nd].From;

            path.Insert(0, nd);
        }

        return path;
    }

}//end class Graph_SearchAStar

