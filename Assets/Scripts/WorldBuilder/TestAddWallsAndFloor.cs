using Assets.Scripts;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{

    public class TestAddWallsAndFloor : MonoBehaviour
    {

        [SerializeField] public GameObject baseRockPrefab;
        [SerializeField] public GameObject baseRockAltPrefab;
        [SerializeField] public GameObject baseRockAlt2Prefab;
        [SerializeField] public GameObject baseRockAlt3Prefab;
        [SerializeField] public GameObject topRockPrefab;
        [SerializeField] public GameObject bottomRockPrefab;
        [SerializeField] public GameObject rightRockPrefab;
        [SerializeField] public GameObject leftRockPrefab;
        [SerializeField] public GameObject curve180to270TilePrefab;
        [SerializeField] public GameObject curve180to90TilePrefab;
        [SerializeField] public GameObject curve270to0TilePrefab;
        [SerializeField] public GameObject curve0to90TilePrefab;
        // Use this for initialization
        void Start()
        {

            int wallThickness = 10;
            int upperLeftX = -112;
            int upperLeftY = 51;
            int lowerRightX = 112;
            int lowerRightY = -51;


            bool shouldBuildWorld = false;
            // shouldBuildWorld = true;
            if (shouldBuildWorld)
            {


                int increment = 1;

                // floor
                for (int y = lowerRightY; y < lowerRightY + wallThickness; y += increment)
                {
                    for (int x = upperLeftX; x < lowerRightX; x += increment)
                    {
                        GameObject prefab = baseRockPrefab;
                        if (y == lowerRightY + wallThickness - 1) prefab = topRockPrefab;
                        if (x < upperLeftX + wallThickness) prefab = baseRockPrefab;
                        if (y == lowerRightY + wallThickness - 1 && x == upperLeftX + wallThickness) prefab = curve180to270TilePrefab;
                        if (x > lowerRightX - wallThickness) prefab = baseRockPrefab;
                        if (y == lowerRightY + wallThickness - 1 && x == lowerRightX - wallThickness) prefab = curve180to90TilePrefab;

                        AddTile(prefab, x, y);

                    }
                }
                // ceiling
                for (int y = upperLeftY - wallThickness; y < upperLeftY; y += increment)
                {
                    for (int x = upperLeftX; x < lowerRightX; x += increment)
                    {
                        GameObject prefab = baseRockPrefab;
                        if (y == upperLeftY - wallThickness) prefab = bottomRockPrefab;
                        if (x < upperLeftX + wallThickness) prefab = baseRockPrefab;
                        if (y == upperLeftY - wallThickness && x == upperLeftX + wallThickness) prefab = curve270to0TilePrefab;
                        if (x > lowerRightX - wallThickness) prefab = baseRockPrefab;
                        if (y == upperLeftY - wallThickness && x == lowerRightX - wallThickness) prefab = curve0to90TilePrefab;

                        AddTile(prefab, x, y);
                    }
                }
                // left wall
                for (int y = lowerRightY + wallThickness; y < upperLeftY - wallThickness; y += increment)
                {
                    for (int x = upperLeftX; x < upperLeftX + wallThickness + 1; x += increment)
                    {
                        GameObject prefab = baseRockPrefab;
                        if (x == upperLeftX + wallThickness) prefab = rightRockPrefab;

                        AddTile(prefab, x, y);
                    }
                }
                // right wall
                for (int y = lowerRightY + wallThickness; y < upperLeftY - wallThickness; y += increment)
                {
                    for (int x = lowerRightX - wallThickness; x < lowerRightX; x += increment)
                    {
                        GameObject prefab = baseRockPrefab;
                        if (x == lowerRightX - wallThickness) prefab = leftRockPrefab;

                        AddTile(prefab, x, y);
                    }
                }

            }

        }

        private void AddTile(GameObject prefab, int x, int y)
        {
            Vector3 position = new Vector3(x / 10f, y / 10f);
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            if (prefab == baseRockPrefab)
            {
                // random chance to alternate out the tile
                int rando = RNG.getRandomInt(0, 120);
                if (rando < 10)
                {
                    prefab = baseRockAltPrefab;
                }
                else if (rando < 20)
                {
                    prefab = baseRockAlt2Prefab;
                }
                else if (rando < 30)
                {
                    prefab = baseRockAlt3Prefab;
                }

            }
            GameObject obj = Instantiate(prefab, position, rotation);
            //obj.transform.localScale = new Vector3(2, 2, 2);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
