using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveGenCore{
        private int presission = 2; //distance from field border where seeds of caves cannot be set
        public const int poleArraySize = 207;
        public char[,] poleCaves = new char[poleArraySize, poleArraySize]; //The main cell in memory for the field.
        public char[,] polePaths = new char[poleArraySize, poleArraySize];
        public char[,] poleRivers = new char[poleArraySize, poleArraySize];
        public char[,] poleCreatures = new char[poleArraySize, poleArraySize];
        private int[,] poleUsed = new int[poleArraySize, poleArraySize];
        public int limit_x, limit_y; //Field size limits
        public List<int[]> seeds_pos = new List<int[]>();
        public List<int[]> edges = new List<int[]>();
        private int[][] stepWays = new int[4][]  { //Directions generator can move while creating new cell of cave
            new int[2] {0, 1},
            new int[2] {0, -1},
            new int[2] {1, 0},
            new int[2] {-1, 0}
        };

        public CaveGenCore(int sx, int sy) //Field constructor
        {
            limit_x = sx;
            limit_y = sy;
            for (int i = 0; i < limit_x; i++)
            {
                for (int j = 0; j < limit_y; j++)
                {
                    poleRivers[i, j] = '.';
                    polePaths[i, j] = '.';
                    poleCaves[i, j] = '.';
                    poleCreatures[i, j] = '.'; //It fills all field cells with #
                }
            }
        }
        public void ClearPole(ref char[,] poleToClear) //Field constructor
        {
            for (int i = 0; i < limit_x; i++)
            {
                for (int j = 0; j < limit_y; j++)
                {
                    poleToClear[i, j] = '.';
                }
            }
        }

        private bool possibleCell(int[] Pos)
        { //Check if given cordinates exist on current field
            if (Pos[0] >= limit_x || Pos[1] >= limit_y) return false;

            if (Pos[0] < 0 || Pos[1] < 0) return false;

            return true;
        }
        public char getCell(int x, int y){
            int Caves = 1, Paths = 1, Rivers = 1, Creatures = 1;
            x--; 
            y--;
            char cell = '.';
            if (Caves == 1 && poleCaves[x, y] != '.') cell = poleCaves[x, y];
            if (Paths == 1 && polePaths[x, y] != '.') cell = polePaths[x, y];
            if (Rivers == 1 && poleRivers[x, y] != '.') cell = poleRivers[x, y];
            if (Creatures == 1 && poleCreatures[x, y] != '.') cell = poleCreatures[x, y];
            return cell;
        }
        private void Grow(int step, int size, int[] pos)
        { //Basic DFS that growing caves from seed (it's recursive)

            foreach (int[] stepWay in stepWays)
            { //Go through all possible direction to step

                int[] newPos = { pos[0] + stepWay[0], pos[1] + stepWay[1] }; //Creates new cordinates using current cordinates and chosen direction

                if (!possibleCell(newPos)) {/*Console.Write("1\n")*/; continue; } //Check if cell on the field

                if (poleCaves[newPos[0], newPos[1]] == '.')
                {

                    if (Random.Range(1, size) <= size - step)
                    { //Choose with chance size-step/size if new cave cell will be created
                        poleCaves[newPos[0], newPos[1]] = 's';
                        Grow(step + 1, size, newPos);
                    }

                }

            }

        }

        private void GrowBFS(int size, int[] pos)
        { //Basic DFS that growing caves from seed (it's recursive)

            Queue<int[]> growTer = new Queue<int[]>();
            int[] start = { pos[0], pos[1], 0 };

            growTer.Enqueue(start);

            while (growTer.Count > 0)
            {
                int[] currPos = growTer.Dequeue();

                foreach (int[] stepWay in stepWays)
                {
                    int[] newPos = { currPos[0] + stepWay[0], currPos[1] + stepWay[1] };

                    if (!possibleCell(newPos)) continue;
                    if (poleCaves[newPos[0], newPos[1]] != '.') continue;
                    if (Random.Range(1, size) > size - currPos[2]) continue;

                    poleCaves[newPos[0], newPos[1]] = 's';
                    int[] nextStep = { newPos[0], newPos[1], currPos[2] + 1 };
                    growTer.Enqueue(nextStep);
                }
            }

        }

        private int Check(int[] currPos, in List<int[]> seeds_pos)
        {
            int minDist = limit_x + limit_y;

            foreach (int[] pos in seeds_pos)
            {
                minDist = Mathf.Min(minDist, Mathf.Abs(currPos[0] - pos[0]) + Mathf.Abs(currPos[1] - pos[1]));
            }

            return minDist;
        }

        private void addEdges(int seedsNumber, ref List<int[]> seeds_pos, ref List<int[]> edges)
        {// adds all possible edges and sorts them
            for (int i = 0; i < seedsNumber; i++)
            {
                for (int j = i + 1; j < seedsNumber; j++)
                {
                    int x1 = seeds_pos[i][0], x2 = seeds_pos[j][0], y1 = seeds_pos[i][1], y2 = seeds_pos[j][1];

                    int[] newEdge = { (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2), i, j }; // Edge = {squared distance between points/nodes, node 1, node 2};
                    edges.Add(newEdge);
                }
            }
            edges = edges.OrderBy(arr => arr[0]).ToList();// sort by first element from smallest to largest
        }

        public List<int[]> generateSkeleton(int seedsNumber, ref List<int[]> edges)
        {// generates a skeleton of connections between caves
            DisjointSetUnioin DSU = new DisjointSetUnioin();
            DSU.setNodeNumber(seedsNumber);
            List<int[]> Skeleton = new List<int[]>();

            foreach (int[] edge in edges)
            {

                if (DSU.findParent(edge[1]) != DSU.findParent(edge[2]))
                {
                    DSU.uniteSets(edge[1], edge[2]);
                    //If sets are disjoint then we should unite them
                    int[] skeletonEdge = new int[] { edge[1], edge[2] };
                    Skeleton.Add(skeletonEdge);
                }

            }
            return Skeleton;
        }

        private void generatePointsToConnect(int distLimit, int[] point1, int[] point2, ref List<int[]> pointsToConnect)
        {//adds points to connect with the given limit 
            int distance = (point1[0] - point2[0]) * (point1[0] - point2[0]) + (point1[1] - point2[1]) * (point1[1] - point2[1]);
            //squared distance between given points
            if (distance <= distLimit)
            {// if the points are at the appropriate distance, they are added to the list
                int[] newPointsToConnect = new int[] { point1[0], point1[1], point2[0], point2[1] };
                pointsToConnect.Add(newPointsToConnect);
                return;
            }
        //else finds the middle point, moves it randomly and tries to do the same
        int randLimit = (int)Mathf.Sqrt(distance);
        randLimit /= 3;
        int shiftX = Random.Range(-randLimit, randLimit), shiftY = Random.Range(-randLimit, randLimit);

        int coordinateX = (point1[0] + point2[0]) / 2 + shiftX;
            int coordinateY = (point1[1] + point2[1]) / 2 + shiftY;
            coordinateX = Mathf.Max(Mathf.Min(coordinateX, limit_x), 0);
            coordinateY = Mathf.Max(Mathf.Min(coordinateY, limit_y), 0);

            int[] midPoint = new int[] { coordinateX, coordinateY };

            generatePointsToConnect(distLimit, point1, midPoint, ref pointsToConnect);
            generatePointsToConnect(distLimit, midPoint, point2, ref pointsToConnect);
        }

        private void connectPoints(int[] point1, int[] point2, int radius)
        {
            int t, s, a = Mathf.Abs(point2[1] - point1[1]), b = Mathf.Abs(point2[0] - point1[0]);
            int x1 = Mathf.Min(point1[0], point2[0]), x2 = Mathf.Max(point1[0], point2[0]);
            int y1 = Mathf.Min(point1[1], point2[1]), y2 = Mathf.Max(point1[1], point2[1]);

            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    s = Mathf.Abs(y - point1[1]);
                    t = Mathf.Abs(x - point1[0]);

                    if (radius * radius * (a * a + b * b) >= (a * t - b * s) * (a * t - b * s) && poleCaves[x, y] != 'S')
                    {
                        polePaths[x, y] = 's';
                    }
                }
            }
        }

        private void generateLine(int distLimit, ref List<int[]> seeds_pos, ref List<int[]> edges)
        {//creates a connection between caves
            List<int[]> pointsToConnect = new List<int[]>();

            foreach (int[] edge in edges)
            {//generates and adds all points to the list that should be connected
                generatePointsToConnect(distLimit, seeds_pos[edge[0]], seeds_pos[edge[1]], ref pointsToConnect);
            }

            foreach (int[] points in pointsToConnect)
            {//connects the points
                int[] point1 = new int[] { points[0], points[1] };
                int[] point2 = new int[] { points[2], points[3] };
                connectPoints(point1, point2, 1);
            }
        }

        public void GenerateSeedsOfCaves(int number, int dist)
        {
            ClearPole(ref poleCaves);
            seeds_pos.Clear();
            int startNumber = number, distCofic = Mathf.Max(limit_x * limit_y / number / 25, dist);

            for (int i = 1; i <= number; i++)
            {
                int[] seed_pos;
                if(limit_x>=10 && limit_y>=10)
                     seed_pos = new int[]{ Random.Range(presission + 1, limit_x - presission),  Random.Range(presission + 1, limit_y - presission) };
                else
                    seed_pos = new int[]{ Random.Range(0, limit_x),  Random.Range(0, limit_y) };
                if (number < startNumber * 10 && Check(seed_pos, seeds_pos) < distCofic) { number++; continue; }

                poleCaves[seed_pos[0], seed_pos[1]] = 'S';
                seeds_pos.Add(seed_pos);
            }

           // Console.WriteLine(number);
        }

        public void GrowCaves(int sizeLow, int sizeHigh)
        {
            foreach (int[] seed in seeds_pos) GrowBFS(Random.Range(sizeLow, sizeHigh+1), seed);
        }

        public void CreatePaths(int number)
        {
            ClearPole(ref polePaths);
            edges.Clear();
            addEdges(number, ref seeds_pos, ref edges);
            edges = generateSkeleton(number, ref edges);
            generateLine(12, ref seeds_pos, ref edges);
        }
    public void runThrough()
    {
        for(int i = 0; i<=limit_x; i++){
            for(int j = 0; j<=limit_y; j++){
                poleUsed[i, j] = 0;
            }
        }
        foreach(int[] seed in seeds_pos) CheckDFS(seed);
    }
    public void CheckDFS(int[] pos){
        poleUsed[pos[0], pos[1]] = 1;
        int neighborsCave = 0;
        foreach(int[] step in stepWays){
            int[] newPos = {pos[0]+step[0], pos[1]+step[1]};
            if(!possibleCell(newPos)) continue;
            if(poleCaves[newPos[0], newPos[1]] == 's' || poleCaves[newPos[0], newPos[1]] == 'S') neighborsCave += 1;
        }
        if(neighborsCave>=3 && poleCaves[pos[0], pos[1]] != 'S') poleCaves[pos[0], pos[1]] = 's';
        if(poleCaves[pos[0], pos[1]] == 's' || poleCaves[pos[0], pos[1]] == 'S'){
            foreach(int[] step in stepWays){
                int[] newPos = {pos[0]+step[0], pos[1]+step[1]};
                if(!possibleCell(newPos)) continue;
                if(poleUsed[newPos[0], newPos[1]] == 0) CheckDFS(newPos);
            }
        }
    }
    public void GenerateCaves(int number, int sizeLow, int sizeHigh)
    {
        GenerateSeedsOfCaves(number, (sizeLow + sizeHigh)/2);
        GrowCaves(sizeLow, sizeHigh);
        runThrough();
    }
}
