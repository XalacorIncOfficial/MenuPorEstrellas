using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
  
    public class WorldProperties
    {
        public string Name;
        public int ID;
        public int Lives;
        public int Seconds;

        public int Columns;
        public List<int> EnemiesID;

    }

    static public List<WorldProperties> Worlds = new List<WorldProperties>()
    {
        new WorldProperties()
        {
            Name = "World 1", ID = 0, Lives = 3, Seconds = 40, Columns = 2, EnemiesID = new List<int>(){0, 1}
        },

        new WorldProperties()
        {
            Name = "World 2", ID = 1, Lives = 2, Seconds = 60, Columns = 3, EnemiesID = new List<int>(){2, 1, 0}
        },

        new WorldProperties()
        {
            Name = "World 3", ID = 2, Lives = 4, Seconds = 80, Columns = 4, EnemiesID = new List<int>(){2, 1, 1, 2, 3}
        },

        new WorldProperties()
        {
            Name = "World 4", ID = 3, Lives = 3, Seconds = 65, Columns = 5, EnemiesID = new List<int>(){ 3, 1, 0, 2}
        },

        new WorldProperties()
        {
            Name = "World 5", ID = 4, Lives = 4, Seconds = 70, Columns = 6, EnemiesID = new List<int>(){4, 3, 2, 2, 1}
        },

        new WorldProperties()
        {
            Name = "World 6", ID = 5, Lives = 7, Seconds = 90, Columns = 7, EnemiesID = new List<int>(){5, 4, 3, 2, 1, 0}
        },
    };

    static public WorldProperties GetWorldByID(int _id)
    {
        for (int i = 0; i < Worlds.Count; i++)
        {
            if(Worlds[i].ID == _id)
            {
                return Worlds[i];
            }
        }
        return null;
    }
}
