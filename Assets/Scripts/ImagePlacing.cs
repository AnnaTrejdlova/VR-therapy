using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ImagePlacing : MonoBehaviour
{
    public Height height = new Height(0.5f,1.5f,2.5f);
    public GameObject prefab;
    [Range(1f, 10f)]
    public float spaceBetween = 4f;
    public bool setSeed = false;
    public bool liveUpdate = false;

    private List<GameObject> gameObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        PlaceImages();
    }

    // Update is called once per frame
    void Update()
    {
        if (liveUpdate)
        {
            foreach (var obj in gameObjects)
            {
                Destroy(obj);
            }

            PlaceImages();
        }
    }
    
    public void PlaceImages(Position position = Position.Sitting)
    {

        // Get all walls
        GameObject[] walls = GameObject.FindGameObjectsWithTag("ImagePlaceable");
        Debug.Log("Number of walls: " + walls.Length);

        // Get all images
        Texture[] images = Resources.LoadAll<Texture>("Images");
        Debug.Log("Number of images: " + images.Length);

        System.Random rng;
        if (setSeed)
        {
            rng = new System.Random(0);
        }
        else
        {
            rng = new System.Random();
        }

        var shuffledImages = images.OrderBy(a => rng.Next()).ToList();

        float[] heightsList = new float[3] { height.lowHeight, height.midHeight, height.highHeight };
        int maxHeight = 3;
        if (position == Position.Sitting)
        {
            maxHeight -= 1;
        }

        foreach (var wall in walls)
        {
            var t = wall.transform;
            int imageCount = (int)Math.Floor(t.lossyScale.x / spaceBetween);
            float _spaceBetween = t.lossyScale.x / (imageCount + 1);
            for (int i = 0; i < imageCount; i++)
            {
                var k = UnityEngine.Random.Range(0, maxHeight);

                // Instatiate image object and place it
                GameObject imageObject = Instantiate(prefab,
                    t.position
                        + t.forward * -0.08f
                        + t.up * (-t.lossyScale.y / 2 + heightsList[k])
                        + t.right * (-t.lossyScale.x / 2 + _spaceBetween * (i + 1)),
                    Quaternion.AngleAxis(t.rotation.eulerAngles.y, new Vector3(0, 1, 0))
                );
                gameObjects.Add(imageObject);

                // Add texture to the object
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                if (shuffledImages.Count == 0)
                {
                    shuffledImages = images.OrderBy(a => rng.Next()).ToList();
                }
                mat.mainTexture = shuffledImages[0];
                shuffledImages.RemoveAt(0);
                imageObject.GetComponent<MeshRenderer>().material = mat;
            }
        }
    }
}

[Serializable]
public struct Height
{
    [Range(0f, 4f)]
    public float lowHeight;
    [Range(0f, 4f)]
    public float midHeight;
    [Range(0f, 4f)]
    public float highHeight;

    public Height(float lowHeight, float midHeight, float highHeight)
    {
        this.lowHeight = lowHeight;
        this.midHeight = midHeight;
        this.highHeight = highHeight;
    }
}


public enum Position
{
    Sitting,
    Standing
}