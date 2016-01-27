using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntryPoint
{


    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public void Add(T1 item1, T2 item2)
        {
            Add(new Tuple<T1, T2>(item1, item2));
        }
    }







#if WINDOWS || LINUX
    public static class Program
    {

        [STAThread]
        static void Main()
        {

            var fullscreen = false;
        read_input:
            switch (Microsoft.VisualBasic.Interaction.InputBox("Which assignment shall run next? (1, 2, 3, 4, or q for quit)", "Choose assignment", VirtualCity.GetInitialValue()))
            {
                case "1":
                    using (var game = VirtualCity.RunAssignment1(SortSpecialBuildingsByDistance, fullscreen))
                        game.Run();
                    break;
                case "2":
                    using (var game = VirtualCity.RunAssignment2(FindSpecialBuildingsWithinDistanceFromHouse, fullscreen))
                        game.Run();
                    break;
                case "3":
                    using (var game = VirtualCity.RunAssignment3(FindRoute, fullscreen))
                        game.Run();
                    break;
                case "4":
                    using (var game = VirtualCity.RunAssignment4(FindRoutesToAll, fullscreen))
                        game.Run();
                    break;
                case "q":
                    return;
            }
            goto read_input;
        }

        private static IEnumerable<Vector2> SortSpecialBuildingsByDistance(Vector2 house, IEnumerable<Vector2> specialBuildings)
        {
            // Create empty tuple list
            TupleList<Vector2, float> SpecialBuildingsTuple = new TupleList<Vector2, float>();

            // For every Vector2 in specialBuildings add it to the tuple list + add the distance between the house and that vector
            foreach (Vector2 v in specialBuildings)
            {
                SpecialBuildingsTuple.Add(v, Vector2.Distance(house, v));
            }

            // Order the tuple list
            merge_sort(SpecialBuildingsTuple, 0, SpecialBuildingsTuple.Count - 1);

            // Return every vector in the tuple list and convert it to a list
            return SpecialBuildingsTuple.Select(t => t.Item1).ToList();
        }

        private static IEnumerable<IEnumerable<Vector2>> FindSpecialBuildingsWithinDistanceFromHouse(
          IEnumerable<Vector2> specialBuildings,
          IEnumerable<Tuple<Vector2, float>> housesAndDistances)
        {
            var KDtree = new Empty<Vector2>() as IKDTree<Vector2>;


            foreach (Vector2 specialBuilding in specialBuildings)
            {
                KDtree = Insert(KDtree, specialBuilding, KDtree.IsXSorted);
            }

            List<List<Vector2>> buildings_in_range_all = new List<List<Vector2>>();

            foreach (Tuple<Vector2, float> house in housesAndDistances)
            {
                List<Vector2> buildings_in_range = new List<Vector2>();
                get_buildings_in_range(house.Item1, house.Item2, KDtree, buildings_in_range);
                buildings_in_range_all.Add(buildings_in_range);

                Console.WriteLine(buildings_in_range.Count);
            }


            return buildings_in_range_all;



        }

        private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding,
          Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {

            List<Tuple<Vector2, Vector2>> path = new List<Tuple<Vector2, Vector2>>();

            int roadnummer = 0;
            Dictionary<Vector2, int> cache = new Dictionary<Vector2, int>();
            foreach (Tuple<Vector2, Vector2> road in roads)
            {
                if (!cache.ContainsKey(road.Item1))
                {
                    cache.Add(road.Item1, roadnummer++);
                }

                if (!cache.ContainsKey(road.Item2))
                {
                    cache.Add(road.Item2, roadnummer++);
                }
            }


            int[][] node_with_neigbours = new int[cache.Count][];

            foreach (var item in cache)
            {
                List<int> neighbours = new List<int>();
                foreach (var road in roads)
                {
                    if (item.Key == road.Item1)
                        neighbours.Add(cache[road.Item2]);
                }
                node_with_neigbours[item.Value] = neighbours.ToArray();
            }

            int[] pred = new int[cache.Count];
            pred[cache[startingBuilding]] = cache[startingBuilding]; 


            float[] dist = new float[cache.Count];


            for (int i = 0; i < cache.Count; i++)
            {
             dist[i] = float.MaxValue;

             
            }

            dist[cache[startingBuilding]] = 0;


            //foreach (Tuple<Vector2, Vector2> road in roads)
            //{
            //    matrix_dist[cache[road.Item1], cache[road.Item2]] = Vector2.Distance(road.Item1, road.Item2);
            //}

            var priority_que = new BinaryEmpty<Tuple<int, float>>() as ITree<Tuple<int, float>>;

            foreach (var item in cache)
            {
                var tuple = new Tuple<int, float>(item.Value, dist[item.Value]);
                priority_que = Insert(priority_que, tuple);
            }

            while (!priority_que.IsEmpty)
            {
                int matrix_index = priority_que.get_min().Item1;
                priority_que = delete(priority_que, priority_que.get_min());
                foreach (var node in node_with_neigbours[matrix_index])
                {
                    float distance_via_to_node = dist[matrix_index]
                        + Vector2.Distance(cache.FirstOrDefault(x => x.Value == matrix_index).Key,
                        cache.FirstOrDefault(x => x.Value == node).Key);

                    float distance_direct_to_node = dist[node];

                    if (distance_via_to_node < distance_direct_to_node)
                    {
                        dist[node] = distance_via_to_node;

                        Tuple<int, float> tuple_direct_to_node = new Tuple<int, float>(node, distance_direct_to_node);
                        Tuple<int, float> tuple_via_to_node = new Tuple<int, float>(node, distance_via_to_node);


                        priority_que = update(priority_que, tuple_direct_to_node, tuple_via_to_node);

                        pred[node] = matrix_index;
                    }
                }
            }

            List<int> result = new List<int>();
            result.Add(cache[destinationBuilding]);
            pathcalc(pred, cache[startingBuilding], cache[destinationBuilding], result);

            List<Vector2> vectors = new List<Vector2>();
         

            for(int i = 0; i < result.Count; i++)
            {
                vectors.Add(cache.FirstOrDefault(x => x.Value == result[i]).Key);
            }

            path = pathlist(vectors);

            return path;

            Console.WriteLine("from start to end " + dist[cache[destinationBuilding]] );

            var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
            List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
            var prevRoad = startingRoad;
            for (int i = 0; i < 30; i++)
            {
                prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, destinationBuilding)).First());
                fakeBestPath.Add(prevRoad);
            }
            return fakeBestPath;
        }

        private static IEnumerable<IEnumerable<Tuple<Vector2, Vector2>>> FindRoutesToAll(Vector2 startingBuilding,
          IEnumerable<Vector2> destinationBuildings, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            IEnumerable<Vector2> nodes = get_all_nodes(roads);
            int size_nodes = nodes.Count();
            int size_roads = roads.Count();
            int[,] matrix = new int[size_nodes, size_nodes];
            Console.Write(size_nodes);
            // Create matrix
            for (int i = 0; i < size_nodes; i++)
            {
                for (int j = 0; j < size_roads; j++)
                {

                    if (equals(nodes.ElementAt(i), nodes.ElementAt(j)))
                        matrix[i, j] = 0;
                    else if (equals(roads.ElementAt(i).Item2, roads.ElementAt(j).Item1))
                        matrix[i, j] = 1;
                    else
                        matrix[i, j] = Int32.MaxValue;

                }
            }
            Console.WriteLine("Done matrix 1");
            matrix = floyd_warshall(matrix);
            printMatrix(matrix);



            List<List<Tuple<Vector2, Vector2>>> result = new List<List<Tuple<Vector2, Vector2>>>();
            foreach (var d in destinationBuildings)
            {
                var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
                List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
                var prevRoad = startingRoad;
                for (int i = 0; i < 30; i++)
                {
                    prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, d)).First());
                    fakeBestPath.Add(prevRoad);
                }
                result.Add(fakeBestPath);
            }
            return result;
        }

        static void PrintPreOrder<T>(IKDTree<T> t)
        {
            if (t.IsEmpty) return;
            Console.WriteLine(t.Value);
            PrintPreOrder(t.Left);
            PrintPreOrder(t.Right);
        }

        static void get_buildings_in_range(Vector2 house, float radius, IKDTree<Vector2> KDTree, List<Vector2> l)
        {

            if (!KDTree.IsEmpty)
            {

                if (KDTree.IsXSorted)
                {
                    Console.WriteLine("House: " + house.X + "," + house.Y + " building: " + KDTree.Value.X + "," + KDTree.Value.Y + " Radius: " + radius + " X");
                    if (Math.Abs(house.X - KDTree.Value.X) < radius)
                    {
                        Console.WriteLine(Math.Abs(house.X - KDTree.Value.X));
                        if (Vector2.Distance(KDTree.Value, house) <= radius)
                        {
                            Console.WriteLine(Vector2.Distance(KDTree.Value, house) + " " + radius);
                            l.Add(KDTree.Value);
                        }
                        get_buildings_in_range(house, radius, KDTree.Left, l);
                        get_buildings_in_range(house, radius, KDTree.Right, l);
                    }

                    else if ((house.X - KDTree.Value.X) > radius)
                    {
                        get_buildings_in_range(house, radius, KDTree.Right, l);
                    }
                    else if ((KDTree.Value.X - house.X) > radius)
                    {
                        get_buildings_in_range(house, radius, KDTree.Left, l);
                    }

                }
                else
                {
                    Console.WriteLine("House: " + house.X + "," + house.Y + " building: " + KDTree.Value.X + "," + KDTree.Value.Y + " Radius: " + radius + " Y");
                    if (Math.Abs(house.Y - KDTree.Value.Y) <= radius)
                    {
                        Console.WriteLine(Math.Abs(house.Y - KDTree.Value.Y));
                        if (Vector2.Distance(KDTree.Value, house) < radius)
                        {
                            Console.WriteLine(Vector2.Distance(KDTree.Value, house) + " " + radius);
                            l.Add(KDTree.Value);
                        }
                        get_buildings_in_range(house, radius, KDTree.Left, l);
                        get_buildings_in_range(house, radius, KDTree.Right, l);
                    }

                    else if ((house.Y - KDTree.Value.Y) > radius)
                    {
                        get_buildings_in_range(house, radius, KDTree.Right, l);
                    }
                    else if ((KDTree.Value.Y - house.Y) > radius)
                    {
                        get_buildings_in_range(house, radius, KDTree.Left, l);
                    }
                }
            }
            else
            {
                Console.WriteLine("end of tree");
            }

        }

        static ITree<Tuple<int, float>> Insert (ITree<Tuple<int, float>> t, Tuple<int, float> tuple)
        {
            if (t.IsEmpty)
                return new BinaryTree<Tuple<int, float>>(new BinaryEmpty<Tuple<int, float>>(), tuple, new BinaryEmpty<Tuple<int, float>>());

            if (t.Value.Item2 == tuple.Item2 && t.Value.Item1 == tuple.Item1)
                return t;

            if (tuple.Item2 < t.Value.Item2)
                return new BinaryTree<Tuple<int, float>>(Insert(t.Left, tuple), t.Value, t.Right);
            else
                return new BinaryTree<Tuple<int, float>>(t.Left, t.Value, Insert(t.Right, tuple));
        }

        static IKDTree<Vector2> Insert(IKDTree<Vector2> t, Vector2 v, bool parentIsX)
        {
            if (t.IsEmpty)
            {
                if (parentIsX)
                    return new Node<Vector2>(new Empty<Vector2>(), v, new Empty<Vector2>(), false);
                else
                    return new Node<Vector2>(new Empty<Vector2>(), v, new Empty<Vector2>(), true);
            }
            if (t.IsXSorted)
            {
                if (t.Value == v)
                    return t;

                if (v.X < t.Value.X)
                    return new Node<Vector2>(Insert(t.Left, v, t.IsXSorted), t.Value, t.Right, true);
                else
                    return new Node<Vector2>(t.Left, t.Value, Insert(t.Right, v, t.IsXSorted), true);
            }
            else
            {
                if (t.Value == v)
                    return t;

                if (v.Y < t.Value.Y)
                    return new Node<Vector2>(Insert(t.Left, v, t.IsXSorted), t.Value, t.Right, false);
                else
                    return new Node<Vector2>(t.Left, t.Value, Insert(t.Right, v, t.IsXSorted), false);
            }
        }

        public static void merge_sort(TupleList<Vector2, float> list, int left, int right)
        {
            // If left index is smaller then right index
            if (left < right)
            {
                int mid = (left + right) / 2; // Get the middle index
                merge_sort(list, left, mid); // Merge_sort new left part
                merge_sort(list, mid + 1, right); // Merge sort new right part
                merge(list, left, mid, right); // Merge the lists
            }
        }

        public static List<Tuple<Vector2, Vector2>> pathlist(List<Vector2> nodes)
        {
            List<Tuple<Vector2, Vector2>> result = new List<Tuple<Vector2, Vector2>>();
            for(int i = nodes.Count -1; i > 0; i--)
            {
                result.Add(new Tuple<Vector2, Vector2>(nodes[i - 1], nodes[i]));
            }
            return result;
        }

        static void pathcalc(int[] pred, int startbuilding, int current_building, List<int> result)
        {
            if(pred[current_building] != startbuilding)
            {
                result.Add(pred[current_building]);
                pathcalc(pred, startbuilding, pred[current_building], result);
            }
            else
            {
                result.Add(pred[startbuilding]);
            }
        }

        public static void merge(TupleList<Vector2, float> list, int left, int mid, int right)
        {

            int range_one = mid - left + 1; // Range of the left_list
            int range_two = right - mid; // Range of the right_list

            var left_list = new TupleList<Vector2, float> { };
            var right_list = new TupleList<Vector2, float> { };

            // Fill the new temp left list
            for (int i = 0; i < range_one; i++)
            {
                left_list.Add(list[left + i].Item1, list[left + i].Item2);
            }

            // Fill the new temp right list
            for (int i = 0; i < range_two; i++)
            {
                right_list.Add(list[mid + i + 1].Item1, list[mid + i + 1].Item2);
            }

            // To prevent the index out of bound exception we add the maxvalue as last element
            left_list.Add(new Vector2(float.MaxValue), float.MaxValue);
            right_list.Add(new Vector2(float.MaxValue), float.MaxValue);

            int index_left_list = 0;
            int index_right_list = 0;

            // Put the temp list elements in the correct order in the orginal list
            for (int k = left; k <= right; k++)
            {
                if (left_list[index_left_list].Item2 <= right_list[index_right_list].Item2)
                {
                    list[k] = left_list[index_left_list];
                    index_left_list++;
                }
                else
                {
                    list[k] = right_list[index_right_list];
                    index_right_list++;
                }
            }


        }

        public static int[,] floyd_warshall(int[,] M)
        {
            for (int k = 0; k < M.Length; k++)
            {
                for (int i = 0; i < M.Length; i++)
                {
                    for (int j = 0; j < M.Length; j++)
                    {
                        // to keep track.;
                        if (M[i, k] + M[k, j] < M[i, j])
                        {
                            M[i, j] = M[i, k] + M[k, j];
                            //P[i][j] = k;
                        }
                        // or not to keep track.
                        //M[i][j] = min(M[i][j], M[i][k] + M[k][j]);
                    }
                }
            }
            return M;
        }

        public static void printMatrix(int[,] Matrix)
        {
            Console.Write("\n\t");
            for (int j = 0; j < Matrix.Length; j++)
            {
                Console.Write(j + "\t");
            }
            Console.WriteLine();
            for (int j = 0; j < 35; j++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            for (int i = 0; i < Matrix.Length; i++)
            {
                Console.Write(i + " |\t");
                for (int j = 0; j < Matrix.Length; j++)
                {
                    Console.Write(Matrix[i, j]);
                    Console.Write("\t");
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n");
        }

        public static IEnumerable<Vector2> get_all_nodes(IEnumerable<Tuple<Vector2, Vector2>> list)
        {
            List<Vector2> list_with_nodes = new List<Vector2>();
            foreach (var item in list)
            {
                bool dont_add = false;
                foreach (var vector in list_with_nodes)
                {
                    if (equals(item.Item1, vector))
                    {
                        dont_add = true;
                        break;
                    }
                }
                if (!dont_add)
                    list_with_nodes.Add(item.Item1);
            }

            return list_with_nodes;
        }

        public static bool equals(Vector2 v1, Vector2 v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return true;
            }
            return false;
        }

        static ITree<Tuple<int, float>> update(ITree<Tuple<int, float>> root, Tuple<int, float> old_value, Tuple<int, float> new_value)
        {
            return Insert(delete(root, old_value), new_value);
        }

        static ITree<Tuple<int, float>> delete(ITree<Tuple<int, float>> root, Tuple<int, float> value)
        {
            if (root.IsEmpty)
                return root;

            if (value.Item2 < root.Value.Item2)
                root.Left = delete(root.Left, value);

            if (value.Item2 > root.Value.Item2)
                root.Right = delete(root.Right, value);

            if (value.Item2 == root.Value.Item2)
            {
                if (root.Left.IsEmpty && root.Right.IsEmpty)
                {
                    root = new BinaryEmpty<Tuple<int, float>>() as ITree<Tuple<int, float>>;
                    return root;
                }
                else if(root.Left.IsEmpty)
                {
                    root = root.Right;
                }
                else if (root.Right.IsEmpty)
                {
                    root = root.Left;
                }
                else
                {
                    root.Value = root.Right.get_min();
                    root.Right = delete(root.Right, root.Right.get_min());
                }
            }

            return root;

        }

    }




#endif
}
