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
            foreach(Vector2 v in specialBuildings)
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
            return
                from h in housesAndDistances
                select
                  from s in specialBuildings
                  where Vector2.Distance(h.Item1, s) <= h.Item2
                  select s;
        }

        private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding,
          Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
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

    }


#endif
}
