using System.Collections.Generic;

namespace il_y.BracketSystem.Core.Services.Parser
{
    public static class FileList<T> where T : class, new()
    {
        public static List<T> CheckList(List<T> list)
        {
            var newList = new List<T>();

            foreach (var item in list)
                //var newItem = new T();

                //bool containsItem = list.Any(n => n.Equals(item));

                if (newList.Contains(item) == false)
                    newList.Add(item);

            return newList;
        }
    }
}