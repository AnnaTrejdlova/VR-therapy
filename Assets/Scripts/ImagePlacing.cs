using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

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
            while (gameObjects.Count > 0)
            {
                Destroy(gameObjects[0]);
                gameObjects.RemoveAt(0);
            }

            PlaceImages();
        }
    }
    
    public void PlaceImages()
    {
        // Get all walls
        GameObject[] walls = GameObject.FindGameObjectsWithTag("ImagePlaceable");
        Debug.Log("Number of walls: " + walls.Length);

        // Get all images
        Texture[] images = Resources.LoadAll<Texture>("Images");
        Debug.Log("Number of images in Assets: " + images.Length);

        if (setSeed)
            Random.InitState(0);

        Position position = ApplicationModel.position;
        Difficulty difficulty = ApplicationModel.difficulty;

        if (!liveUpdate)
        {
            spaceBetween = new Dictionary<Difficulty, float>()
            {
                { Difficulty.Easy, 6f },
                { Difficulty.Medium, 5f },
                { Difficulty.Hard, 4f },
            }[difficulty];
        }

        int totalImages = 0;
        foreach (var wall in walls)
        {
            totalImages += (int)Math.Floor(wall.transform.lossyScale.x / spaceBetween);
        }
        Debug.Log("Number of images to draw: " + totalImages);

        // Prepare image list
        var imageList = new List<Texture>();
        while (imageList.Count < totalImages)
        {
            List<Texture> shuffledImages = images.OrderBy(a => Random.Range(0, int.MaxValue)).ToList();
            imageList.AddRange(shuffledImages.GetRange(0, Math.Min(totalImages - imageList.Count, images.Length)));
        }
        WriteImageListToFile(imageList, false);

        float[] heightsList = new float[3] { height.lowHeight, height.midHeight, height.highHeight };
        int maxHeight = 3;
        if (position == Position.Sitting)
            maxHeight -= 1;

        foreach (var wall in walls)
        {
            var t = wall.transform;
            int imageCount = (int)Math.Floor(t.lossyScale.x / spaceBetween);
            float _spaceBetween = t.lossyScale.x / (imageCount + 1);
            for (int i = 0; i < imageCount; i++)
            {
                var k = Random.Range(0, maxHeight);

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
                mat.mainTexture = imageList[0];
                imageList.RemoveAt(0);
                imageObject.GetComponent<MeshRenderer>().material = mat;
            }
        }

        Debug.Log("Number of placed images: " + gameObjects.Count);
    }

    static void WriteImageListToFile(List<Texture> ImageList, bool appendFile = true)
    {
        string path = Application.persistentDataPath + "/test.txt"; // Application.persistentDataPath = %userprofile%\AppData\LocalLow\<companyname>\

        using (var writer = new StreamWriter(path, appendFile))
        {
            writer.Write(string.Join("\n", ImageList.Select(image => image.name)));
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
