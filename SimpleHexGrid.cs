using UnityEngine;

public class SimpleHexGrid : MonoBehaviour
{
    public GameObject Hexagon;
    public GameObject HexParentHolder;
    public int chunkSize = 50;
    public int heightMultiplier = 2;
    public float Scale = 15;
    public float seed;

    public float WaterLevel = 1;
    public float SandUpToThisHeight = 1.2f;
    public float GrassUpToThisHeight = 1.6f;

    public bool snappingHeights;
    public bool Randomize;

    public bool UpdateMesh;

    public Material Water;
    public Material Sand;
    public Material Grass;
    public Material Stone;

    private float HexZIncreaseValue = 1.5f;
    private float HexXIncreaseValue = 1.732f;

    void Start()
    {
        GenerateMesh();
    }

    private void Update()
    {
        if (UpdateMesh != false)
        {
            foreach (Transform child in HexParentHolder.transform)
            {
                Destroy(child.gameObject);
            }

            GenerateMesh();
            UpdateMesh = false;
        }
    }

    public void GenerateMesh()
    {
        if (Randomize)
        {
            seed = Random.Range(0, 1000000);
            Scale = Random.Range(7, 20);
            WaterLevel = Random.Range(1f, 2f);
            heightMultiplier = Mathf.RoundToInt(WaterLevel) + Random.Range(1, 5);
            SandUpToThisHeight = WaterLevel + heightMultiplier / 4 + Random.Range(0.2f, 2f);
            GrassUpToThisHeight = WaterLevel + SandUpToThisHeight + Random.Range(0.2f, 3f);
        }

        //For each direction x
        for (int x = 0; x < chunkSize; x++)
        {
            //for each direction y
            for (int z = 0; z < chunkSize; z++)
            {
                //find high for hex at this x and y cord
                CaculateHeights(x, z);
            }
        }
    }

    void CaculateHeights(float x, float z)
    {
        float newX = x * HexXIncreaseValue;
        float newZ = z * HexZIncreaseValue;

        float xCord = newX / Scale + seed;
        float zCord = newZ / Scale + seed;

        //generate height based on noise
        float roundedHeight = Mathf.PerlinNoise(xCord, zCord);

        //spawn Hex
        var Hex = Instantiate(Hexagon, new Vector3(newX, 0, newZ), Quaternion.Euler(new Vector3(90, Hexagon.transform.rotation.y, Hexagon.transform.rotation.z)));
        Hex.name = x + "," + z;

        if (snappingHeights)
        {
            //round that noise to the nearest decemel - times it by ten because mathf.round can only use whole numbers
            roundedHeight *= 10;
            roundedHeight = Mathf.Round(roundedHeight * heightMultiplier);

            //snap height to all even numbers
            if (roundedHeight % 2 == 0) { }
            else { roundedHeight -= 1f; }

            //divide result by ten to counter multiplying by ten earlier in this function
            roundedHeight /= 10;
        }
        else
        {
            roundedHeight *= heightMultiplier;
        }

        //set the height to double because it goes both up and down
        Hex.transform.localScale = new Vector3(Hex.transform.localScale.x, Hex.transform.localScale.y, roundedHeight * 2);

        //set the position back to zero
        Hex.transform.position = new Vector3(newX, 0, newZ);

        //if Hex in Odd row, then add offset to position
        if (z % 2 != 0) { Hex.transform.position = new Vector3(Hex.transform.position.x + HexXIncreaseValue / 2, Hex.transform.position.y, Hex.transform.position.z); }

        Hex.transform.parent = HexParentHolder.transform;
        SetHexType(Hex, roundedHeight);
    }

    void SetHexType(GameObject Hex, float HexHeight)
    {
        if (HexHeight <= WaterLevel)
        {
            //water
            Hex.GetComponent<MeshRenderer>().material = Water;
            //Hex.tag = ("Water");

            //set the height to water height
            Hex.transform.localScale = new Vector3(Hex.transform.localScale.x, Hex.transform.localScale.y, WaterLevel * 2);

            //set the position back to zero
            Hex.transform.position = new Vector3(Hex.transform.position.x, 0, Hex.transform.position.z);
        }

        if (HexHeight > WaterLevel)
        {
            if (HexHeight <= SandUpToThisHeight)
            {
                //sand
                Hex.GetComponent<MeshRenderer>().material = Sand;
                //Hex.tag = ("Sand");
            }
        }

        if (HexHeight > SandUpToThisHeight)
        {
            if (HexHeight <= GrassUpToThisHeight)
            {
                //grass
                Hex.GetComponent<MeshRenderer>().material = Grass;
                //Hex.tag = ("Grass");
            }
        }

        if (HexHeight > GrassUpToThisHeight)
        {
            //rock
            Hex.GetComponent<MeshRenderer>().material = Stone;
            //Hex.tag = ("Stone");
        }
    }
}
