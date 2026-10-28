using UnityEngine;

/// <summary>
/// Builds lightweight procedural boat details at runtime, avoiding external model dependencies.
/// </summary>
public class BoatVisualModel : MonoBehaviour
{
    private Transform detailRoot;
    private Material white, dark, glass, red, teal, metal;

    void Awake()
    {
        white = MakeMaterial(new Color(0.91f, 0.92f, 0.86f));
        dark = MakeMaterial(new Color(0.035f, 0.09f, 0.12f));
        glass = MakeMaterial(new Color(0.10f, 0.34f, 0.42f));
        red = MakeMaterial(new Color(0.85f, 0.12f, 0.07f));
        teal = MakeMaterial(new Color(0.08f, 0.45f, 0.34f));
        metal = MakeMaterial(new Color(0.48f, 0.56f, 0.58f));
        detailRoot = new GameObject("Procedural Boat Details").transform;
        detailRoot.SetParent(transform, false);
        Configure("Starter Skiff");
    }

    public void Configure(string boatType)
    {
        if (!detailRoot) return;
        for (int i = detailRoot.childCount - 1; i >= 0; i--)
            Destroy(detailRoot.GetChild(i).gameObject);

        bool speedboat = boatType == "Speedboat";
        bool fishingBoat = boatType == "Fishing Boat";
        float width = fishingBoat ? 2.6f : speedboat ? 1.9f : 2.2f;
        float length = fishingBoat ? 5.3f : speedboat ? 4.4f : 4.8f;
        float deckY = fishingBoat ? 0.39f : speedboat ? 0.29f : 0.33f;
        Material hullMat = fishingBoat ? teal : speedboat ? red : white;

        // Deck trim, bow tip, stern swim platform and rub rail.
        Part("Deck", PrimitiveType.Cube, new Vector3(0, deckY, 0.05f), new Vector3(width * 0.83f, 0.10f, length * 0.78f), hullMat);
        Part("Bow cap", PrimitiveType.Sphere, new Vector3(0, deckY + 0.02f, length * 0.46f), new Vector3(width * 0.52f, 0.12f, 0.45f), hullMat);
        Part("Stern platform", PrimitiveType.Cube, new Vector3(0, deckY - 0.02f, -length * 0.49f), new Vector3(width * 0.72f, 0.12f, 0.35f), dark);
        Part("Port rub rail", PrimitiveType.Cube, new Vector3(-width * 0.48f, 0.06f, 0), new Vector3(0.06f, 0.08f, length * 0.82f), metal);
        Part("Starboard rub rail", PrimitiveType.Cube, new Vector3(width * 0.48f, 0.06f, 0), new Vector3(0.06f, 0.08f, length * 0.82f), metal);

        if (fishingBoat)
        {
            Part("Cabin roof", PrimitiveType.Cube, new Vector3(0, 0.92f, 0.15f), new Vector3(1.55f, 0.14f, 1.45f), white);
            Part("Cabin front", PrimitiveType.Cube, new Vector3(0, 0.66f, 0.72f), new Vector3(1.35f, 0.50f, 0.08f), glass);
            Part("Cabin port wall", PrimitiveType.Cube, new Vector3(-0.70f, 0.66f, 0.12f), new Vector3(0.08f, 0.50f, 1.12f), white);
            Part("Cabin starboard wall", PrimitiveType.Cube, new Vector3(0.70f, 0.66f, 0.12f), new Vector3(0.08f, 0.50f, 1.12f), white);
            Part("Aft fishing seat", PrimitiveType.Cube, new Vector3(0, 0.58f, -1.35f), new Vector3(0.95f, 0.16f, 0.45f), dark);
            for (int i = 0; i < 3; i++)
                Part("Rod holder " + i, PrimitiveType.Cylinder, new Vector3(-0.7f + i * 0.7f, 0.53f, -length * 0.34f), new Vector3(0.07f, 0.22f, 0.07f), metal);
        }
        else
        {
            Part("Console", PrimitiveType.Cube, new Vector3(0, 0.56f, 0.12f), new Vector3(0.75f, 0.50f, 0.50f), dark);
            Part("Windshield", PrimitiveType.Cube, new Vector3(0, 0.86f, 0.39f), new Vector3(0.78f, 0.24f, 0.06f), glass);
            Part("Driver seat", PrimitiveType.Cube, new Vector3(0, 0.53f, -0.65f), new Vector3(0.78f, 0.16f, 0.52f), speedboat ? red : dark);
            Part("Passenger seat", PrimitiveType.Cube, new Vector3(0, 0.53f, -1.25f), new Vector3(0.78f, 0.16f, 0.45f), dark);
        }

        // Outboard motor and steering details.
        Part("Outboard cowling", PrimitiveType.Cube, new Vector3(0, 0.48f, -length * 0.53f), new Vector3(fishingBoat ? 0.48f : 0.38f, 0.52f, 0.38f), dark);
        Part("Motor stripe", PrimitiveType.Cube, new Vector3(0, 0.55f, -length * 0.53f - 0.20f), new Vector3(0.40f, 0.06f, 0.025f), speedboat ? red : teal);
        Part("Navigation light", PrimitiveType.Sphere, new Vector3(0, 0.20f, length * 0.48f), Vector3.one * 0.10f, red);
        Part("Bow cleat port", PrimitiveType.Cube, new Vector3(-width * 0.32f, deckY + 0.08f, length * 0.36f), new Vector3(0.20f, 0.07f, 0.08f), metal);
        Part("Bow cleat starboard", PrimitiveType.Cube, new Vector3(width * 0.32f, deckY + 0.08f, length * 0.36f), new Vector3(0.20f, 0.07f, 0.08f), metal);
    }

    GameObject Part(string partName, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = partName;
        go.transform.SetParent(detailRoot, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        var renderer = go.GetComponent<Renderer>();
        if (renderer) renderer.sharedMaterial = mat;
        var collider = go.GetComponent<Collider>();
        if (collider) Destroy(collider);
        return go;
    }

    Material MakeMaterial(Color color)
    {
        var shader = Shader.Find("Standard");
        var material = new Material(shader);
        material.color = color;
        return material;
    }
}